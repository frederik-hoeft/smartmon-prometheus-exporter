using SmartmonExporter.Pipelines;
using SmartmonExporter.Domain.Metrics;

namespace SmartmonExporter.Domain;

internal interface IMetricsCollector : IPipelineHandler
{
    ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken);
}