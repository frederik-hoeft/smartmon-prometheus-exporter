using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceHealthCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 20;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceHealth deviceHealth = await smartctlRunner.RunAsync<SmartctlDeviceHealth>(["--health"], device.Name, cancellationToken);
        prometheus.AddMetric("smart_status_passed", Prometheus.Gauge("SMART status passed"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceHealth.SmartStatus.Passed, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type)));
        return true; // Continue with the next collector
    }
}
