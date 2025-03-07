using SmartmonExporter.Configuration;
using SmartmonExporter.Domain.Interop.Output;
using SmartmonExporter.Domain.Interop.Output.Model;
using System.Buffers;
using System.Text.Json;

namespace SmartmonExporter.Domain.Interop;

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
        ReadOnlyMemory<string> args = argsBuffer.AsMemory(0, nextIndex);
        string command = configuration.Settings.SmartctlPath;
        Out<string> output = new();
        int exitCode = await processRunner.RunAsync(command, args, output, cancellationToken);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"smartctl failed with exit code {exitCode} while running '{command} {string.Join(' ', args.Span!)}': {output.Value}");
        }
        if (!output.TryGetValue(out string? outputValue))
        {
            throw new InvalidOperationException("smartctl output is empty");
        }
        TResult? result = JsonSerializer.Deserialize(outputValue, jsonSerializerContext.GetTypeInfo<TResult>()) 
            ?? throw new InvalidOperationException("smartctl output deserialization failed");
        if (result.Smartctl.ExitStatus != 0)
        {
            throw new InvalidOperationException($"smartctl failed with exit status {result.Smartctl.ExitStatus}");
        }
        ArrayPool<string>.Shared.Return(argsBuffer);
        return result;
    }
}
