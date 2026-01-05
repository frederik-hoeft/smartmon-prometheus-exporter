namespace SmartmonExporter.Domain;

internal interface IMetricsExporter
{
    Task<string> ExportAsync(string prometheusNamespace, CancellationToken cancellationToken);
}
