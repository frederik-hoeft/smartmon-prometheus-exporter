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

        List<PrometheusLabel> labels = 
        [
            Prometheus.Label("disk", device.Name),
            Prometheus.Label("type", device.Type)
        ];

        if (!string.IsNullOrWhiteSpace(device.SerialNumber))
        {
            labels.Add(Prometheus.Label("serial_number", device.SerialNumber));
        }

        prometheus.AddMetric("device_active", Prometheus.Gauge("Device active status"), includeTimeStamp: false, samples => samples
            .AddSample(value: active, [.. labels]));

        return active; // Continue only if the device is active
    }
}