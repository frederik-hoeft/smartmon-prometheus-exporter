using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Data.Metrics.Factory;
using SmartmonExporter.Interop;
using SmartmonExporter.Interop.Output.Model;

namespace SmartmonExporter.Data.Collectors.DeviceCollectors;

internal sealed class DeviceInfoCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 10;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceInfo deviceInfo = await smartctlRunner.RunAsync<SmartctlDeviceInfo>(["--info"], device.Name, cancellationToken);
        PrometheusLabel disk = Prometheus.Label("disk", device.Name);
        PrometheusLabel type = Prometheus.Label("type", device.Type);
        prometheus.AddMetric("device_info", Prometheus.Gauge("Device information"), includeTimeStamp: false, samples => samples.AddSample(value: true,
            disk, type, 
            Prometheus.Label("model_family", deviceInfo.ModelFamily), 
            Prometheus.Label("model_name", deviceInfo.ModelName), 
            Prometheus.Label("device_model", "empty"), 
            Prometheus.Label("serial_number", deviceInfo.SerialNumber),
            Prometheus.Label("firmware_version", deviceInfo.FirmwareVersion),
            Prometheus.Label("vendor", "empty"),
            Prometheus.Label("product", "empty"),
            Prometheus.Label("revision", "empty"),
            Prometheus.Label("lun_id", "empty")));

        prometheus.AddMetric("smart_support_available", Prometheus.Gauge("SMART support available"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport.Available, disk, type));
        prometheus.AddMetric("smart_support_enabled", Prometheus.Gauge("SMART support enabled"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport.Enabled, disk, type));

        return true; // Continue with the next collector
    }
}
