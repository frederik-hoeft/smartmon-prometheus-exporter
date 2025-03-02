using System.Diagnostics;
using System.Text;

namespace SmartmonExporter.Interop;

internal sealed class DefaultChildProcessRunner : IChildProcessRunner
{
    public async Task<int> RunAsync(string command, ReadOnlyMemory<string> arguments, Out<string> output, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command, nameof(command));

        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = string.Join(' ', arguments.Span!),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        StringBuilder outputBuilder = new();
        DataReceivedHandler stdoutHandler = new(outputBuilder);
        DataReceivedHandler stderrHandler = new(outputBuilder);
        process.OutputDataReceived += stdoutHandler.OnDataReceived;
        process.ErrorDataReceived += stderrHandler.OnDataReceived;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken);
        process.OutputDataReceived -= stdoutHandler.OnDataReceived;
        process.ErrorDataReceived -= stderrHandler.OnDataReceived;
        output.SetValue(outputBuilder.ToString());
        return process.ExitCode;
    }

    private class DataReceivedHandler(StringBuilder builder)
    {
        public void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                builder.AppendLine(e.Data);
            }
        }
    }
}