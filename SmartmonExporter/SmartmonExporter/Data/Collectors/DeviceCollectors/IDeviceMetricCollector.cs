using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Interop.Output.Model;
using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Data.Collectors.DeviceCollectors;

internal interface IDeviceMetricCollector : IPipelineHandler
{
    ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken);
}
