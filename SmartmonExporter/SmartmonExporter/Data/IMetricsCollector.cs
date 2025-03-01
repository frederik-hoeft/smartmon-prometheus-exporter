using SmartmonExporter.Pipelines;
using SmartmonExporter.Data.Metrics;

namespace SmartmonExporter.Data;

public interface IMetricsCollector : IPipelineHandler
{
    ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken);
}