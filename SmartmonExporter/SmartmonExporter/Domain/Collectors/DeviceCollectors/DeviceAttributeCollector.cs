using SmartmonExporter.Domain.Collectors.DeviceCollectors.DeviceAttributeCollectors;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Pipelines;
using System.Collections.Immutable;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed partial class DeviceAttributeCollector(IEnumerable<IDeviceAttributeCollector> attributeCollectors) : IDeviceMetricCollector
{
    private readonly ImmutableArray<IDeviceAttributeCollector> _collectors = attributeCollectors.CreatePipeline();

    public int Priority => 30;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        foreach (IDeviceAttributeCollector collector in _collectors)
        {
            bool success = await collector.TryCollectAsync(device, prometheus, cancellationToken);
            if (success)
            {
                return true;
            }
        }
        return false;
    }
}