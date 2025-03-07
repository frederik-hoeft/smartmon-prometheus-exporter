using SmartmonExporter.Pipelines;
using SmartmonExporter.Domain.Metrics;

namespace SmartmonExporter.Domain;

public interface IMetricsCollector : IPipelineHandler
{
    ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken);
}