using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal interface IDeviceMetricCollector : IPipelineHandler
{
    ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken);
}
