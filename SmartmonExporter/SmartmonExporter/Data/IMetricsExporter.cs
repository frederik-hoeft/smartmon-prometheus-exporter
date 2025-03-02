namespace SmartmonExporter.Data;

public interface IMetricsExporter
{
    Task<string> ExportAsync(string prometheusNamespace, CancellationToken cancellationToken);
}
