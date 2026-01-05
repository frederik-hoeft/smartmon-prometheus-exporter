using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceInfoCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    // Run first in the pipeline to initialize serial number for other collectors
    public int Priority => int.MinValue;

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceInfo deviceInfo = await smartctlRunner.RunAsync<SmartctlDeviceInfo>(["--info"], device.Name, cancellationToken);
        
        // Lazy-initialize serial number in the device object for subsequent collectors
        if (!string.IsNullOrWhiteSpace(deviceInfo.SerialNumber))
        {
            device.SerialNumber = deviceInfo.SerialNumber;
        }
        
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

        List<PrometheusLabel> supportLabels = [disk, type];
        if (!string.IsNullOrWhiteSpace(device.SerialNumber))
        {
            supportLabels.Add(Prometheus.Label("serial_number", device.SerialNumber));
        }

        if (deviceInfo.SmartSupport is not null)
        {
            prometheus.AddMetric("smart_support_available", Prometheus.Gauge("SMART support available"), includeTimeStamp: false, samples => samples
                .AddSample(value: deviceInfo.SmartSupport.Available, [.. supportLabels]));
            prometheus.AddMetric("smart_support_enabled", Prometheus.Gauge("SMART support enabled"), includeTimeStamp: false, samples => samples
                .AddSample(value: deviceInfo.SmartSupport.Enabled, [.. supportLabels]));
        }

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