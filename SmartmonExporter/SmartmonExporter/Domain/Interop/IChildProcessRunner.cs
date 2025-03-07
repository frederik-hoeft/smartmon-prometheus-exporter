namespace SmartmonExporter.Domain.Interop;

public interface IChildProcessRunner
{
    Task<int> RunAsync(string command, ReadOnlyMemory<string> arguments, Out<string> output, CancellationToken cancellationToken);
}