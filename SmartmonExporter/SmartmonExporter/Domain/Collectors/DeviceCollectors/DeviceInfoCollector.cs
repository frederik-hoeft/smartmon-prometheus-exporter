using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceInfoCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    // Run first in the pipeline to initialize serial number for other collectors
    // Collectors run sequentially (not concurrently), so mutation is safe
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
        PrometheusLabel? modelFamily = Prometheus.OptionalLabel("model_family", deviceInfo.ModelFamily);
        PrometheusLabel? modelName = Prometheus.OptionalLabel("model_name", deviceInfo.ModelName);
        PrometheusLabel? deviceModel = Prometheus.OptionalLabel("device_model", deviceInfo.DeviceModel);
        PrometheusLabel? serialLabel = Prometheus.OptionalLabel("serial_number", device.SerialNumber);
        PrometheusLabel? firmwareVersion = Prometheus.OptionalLabel("firmware_version", deviceInfo.FirmwareVersion);
        PrometheusLabel? vendor = Prometheus.OptionalLabel("vendor", deviceInfo.Vendor);
        PrometheusLabel? product = Prometheus.OptionalLabel("product", deviceInfo.Product);
        PrometheusLabel? revision = Prometheus.OptionalLabel("revision", deviceInfo.Revision);
        PrometheusLabel? lunId = Prometheus.OptionalLabel("lun_id", deviceInfo.LunId);

        prometheus.AddMetric("device_info", Prometheus.Gauge("Device information"), includeTimeStamp: false, samples => samples
            .AddSample(value: true, disk, type, modelFamily, modelName, deviceModel, serialLabel, firmwareVersion, vendor, product, revision, lunId));

        prometheus.AddMetric("smart_support_available", Prometheus.Gauge("SMART support available"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport?.Available ?? false, disk, type, serialLabel));
        prometheus.AddMetric("smart_support_enabled", Prometheus.Gauge("SMART support enabled"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport?.Enabled ?? false, disk, type, serialLabel));

        return true; // Continue with the next collector
    }
}