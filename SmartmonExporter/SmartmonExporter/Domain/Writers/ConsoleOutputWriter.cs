using SmartmonExporter.Configuration;

namespace SmartmonExporter.Domain.Writers;

internal sealed class ConsoleOutputWriter(IConfiguration configuration) : IOutputWriter
{
    public int Priority => 10;

    public ValueTask WriteAsync(string metrics, CancellationToken cancellationToken)
    {
        if (!configuration.Settings.WriteToConsole)
        {
            return ValueTask.CompletedTask;
        }
        Console.WriteLine(metrics);
        return ValueTask.CompletedTask;
    }
}