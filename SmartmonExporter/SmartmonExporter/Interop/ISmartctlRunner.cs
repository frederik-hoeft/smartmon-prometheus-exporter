using SmartmonExporter.Configuration;
using SmartmonExporter.Interop.Output;
using SmartmonExporter.Interop.Output.Model;
using System.Buffers;
using System.Text.Json;

namespace SmartmonExporter.Interop;

public interface ISmartctlRunner
{
    Task<TResult> RunAsync<TResult>(string[] arguments, string? device, CancellationToken cancellationToken) where TResult : RootBase;
}

internal sealed class SmartctlRunner(IConfiguration configuration, SmartctlJsonSerializerContext jsonSerializerContext, IChildProcessRunner processRunner) : ISmartctlRunner
{
    public async Task<TResult> RunAsync<TResult>(string[] arguments, string? device, CancellationToken cancellationToken) where TResult : RootBase
    {
        int requiredLength = arguments.Length;
        bool hasJson = true;
        if (arguments is not ["--json" or "-j", ..])
        {
            hasJson = false;
            requiredLength++;
        }
        if (device is not null)
        {
            requiredLength++;
        }
        string[] argsBuffer = ArrayPool<string>.Shared.Rent(requiredLength);
        int nextIndex = 0;
        if (!hasJson)
        {
            argsBuffer[nextIndex++] = "--json";
        }
        arguments.CopyTo(argsBuffer, nextIndex);
        nextIndex += arguments.Length;
        if (device is not null)
        {
            argsBuffer[nextIndex++] = device;
        }
        ReadOnlyMemory<string> args = argsBuffer.AsMemory(0, nextIndex - 1);
        string command = configuration.Settings.SmartctlPath;
        Out<string> output = new();
        int exitCode = await processRunner.RunAsync(command, args, output, cancellationToken);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"smartctl failed with exit code {exitCode}");
        }
        if (!output.TryGetValue(out string? outputValue))
        {
            throw new InvalidOperationException("smartctl output is empty");
        }
        object? rawResult = JsonSerializer.Deserialize(outputValue, typeof(TResult), jsonSerializerContext);
        if (rawResult is not TResult result)
        {
            throw new InvalidOperationException("smartctl output deserialization failed");
        }
        if (result.Smartctl.ExitStatus != 0)
        {
            throw new InvalidOperationException($"smartctl failed with exit status {result.Smartctl.ExitStatus}");
        }
        ArrayPool<string>.Shared.Return(argsBuffer);
        return result;
    }
}
