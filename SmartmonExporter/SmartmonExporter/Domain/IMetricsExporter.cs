namespace SmartmonExporter.Domain;

public interface IMetricsExporter
{
    Task<string> ExportAsync(string prometheusNamespace, CancellationToken cancellationToken);
}
