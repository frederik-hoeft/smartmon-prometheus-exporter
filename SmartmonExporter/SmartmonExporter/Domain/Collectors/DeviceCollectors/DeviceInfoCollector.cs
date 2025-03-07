using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceInfoCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 10;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceInfo deviceInfo = await smartctlRunner.RunAsync<SmartctlDeviceInfo>(["--info"], device.Name, cancellationToken);
        PrometheusLabel disk = Prometheus.Label("disk", device.Name);
        PrometheusLabel type = Prometheus.Label("type", device.Type);
        List<PrometheusLabel> labels =
        [
            disk,
            type,
        ];

        labels.AddIfNotNull("model_family", deviceInfo.ModelFamily);
        labels.AddIfNotNull("model_name", deviceInfo.ModelName);
        labels.AddIfNotNull("device_model", deviceInfo.DeviceModel);
        labels.AddIfNotNull("serial_number", deviceInfo.SerialNumber);
        labels.AddIfNotNull("firmware_version", deviceInfo.FirmwareVersion);
        labels.AddIfNotNull("vendor", deviceInfo.Vendor);
        labels.AddIfNotNull("product", deviceInfo.Product);
        labels.AddIfNotNull("revision", deviceInfo.Revision);
        labels.AddIfNotNull("lun_id", deviceInfo.LunId);

        prometheus.AddMetric("device_info", Prometheus.Gauge("Device information"), includeTimeStamp: false, samples => samples.AddSample(value: true, [.. labels]));

        prometheus.AddMetric("smart_support_available", Prometheus.Gauge("SMART support available"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport.Available, disk, type));
        prometheus.AddMetric("smart_support_enabled", Prometheus.Gauge("SMART support enabled"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport.Enabled, disk, type));

        return true; // Continue with the next collector
    }
}

file static class ListExtensions
{
    public static void AddIfNotNull(this List<PrometheusLabel> labels, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            labels.Add(Prometheus.Label(name, value));
        }
    }
}