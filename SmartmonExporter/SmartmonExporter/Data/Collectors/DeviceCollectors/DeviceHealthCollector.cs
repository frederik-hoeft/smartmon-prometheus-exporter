using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Data.Metrics.Factory;
using SmartmonExporter.Interop;
using SmartmonExporter.Interop.Output.Model;

namespace SmartmonExporter.Data.Collectors.DeviceCollectors;

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
