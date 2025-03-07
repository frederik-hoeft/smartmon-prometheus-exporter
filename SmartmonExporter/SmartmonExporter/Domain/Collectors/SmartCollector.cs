using SmartmonExporter.Domain.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Pipelines;
using System.Collections.Immutable;

namespace SmartmonExporter.Domain.Collectors;

internal sealed class SmartCollector(ISmartctlRunner smartctlRunner, IEnumerable<IDeviceMetricCollector> diskMetricCollectors) : IMetricsCollector
{
    private readonly ImmutableArray<IDeviceMetricCollector> _diskMetricCollectors = diskMetricCollectors.CreatePipeline();

    public int Priority => 0;

    public async ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDevices devices = await smartctlRunner.RunAsync<SmartctlDevices>(["--scan-open"], device: null, cancellationToken);
        foreach (Device device in devices.Devices)
        {
            foreach (IDeviceMetricCollector diskMetricCollector in _diskMetricCollectors)
            {
                bool success = await diskMetricCollector.TryCollectAsync(device, prometheus, cancellationToken);
                if (!success)
                {
                    break;
                }
            }
        }
    }
}
