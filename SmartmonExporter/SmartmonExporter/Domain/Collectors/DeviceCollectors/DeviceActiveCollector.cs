using SmartmonExporter.Domain.Metrics.Factory;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceActiveCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 0;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDevice smartctlDevice = await smartctlRunner.RunAsync<SmartctlDevice>(["-n", "standby"], device.Name, cancellationToken);
        bool active = smartctlDevice.Smartctl.ExitStatus == 0;

        prometheus.AddMetric("device_active", Prometheus.Gauge("Device active status"), includeTimeStamp: false, samples => samples
            .AddSample(value: active, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type)));

        return active; // Continue only if the device is active
    }
}