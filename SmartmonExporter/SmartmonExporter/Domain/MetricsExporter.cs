using SmartmonExporter.Pipelines;
using System.Collections.Immutable;
using SmartmonExporter.Domain.Metrics;

namespace SmartmonExporter.Domain;

internal sealed class MetricsExporter(IEnumerable<IMetricsCollector> collectors) : IMetricsExporter
{
    private readonly ImmutableArray<IMetricsCollector> _collectors = collectors.CreatePipeline();

    public async Task<string> ExportAsync(string prometheusNamespace, CancellationToken cancellationToken)
    {
        PrometheusBuilder builder = new(prometheusNamespace, capacity: _collectors.Length * 256);

        foreach (IMetricsCollector collector in _collectors)
        {
            await collector.CollectAsync(builder, cancellationToken);
        }

        string metrics = builder.Build();
        return metrics;
    }
}
