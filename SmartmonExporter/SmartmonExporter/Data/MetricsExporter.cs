using SmartmonExporter.Pipelines;
using SmartmonExporter.Data.Metrics;
using System.Collections.Immutable;

namespace SmartmonExporter.Data;

internal sealed class MetricsExporter(IEnumerable<IMetricsCollector> collectors) : IMetricsExporter
{
    private readonly ImmutableArray<IMetricsCollector> _collectors = collectors.CreatePipeline();

    public async Task<string> ExportAsync(string prometheusNamespace, IServiceProvider serviceProvider)
    {
        PrometheusBuilder builder = new(prometheusNamespace, capacity: _collectors.Length * 256);

        foreach (IMetricsCollector collector in _collectors)
        {
            await collector.CollectAsync(builder, serviceProvider);
        }

        string metrics = builder.Build();
        return metrics;
    }
}
