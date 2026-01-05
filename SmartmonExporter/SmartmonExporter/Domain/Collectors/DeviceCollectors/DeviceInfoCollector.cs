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
        PrometheusLabel? modelFamily = !string.IsNullOrWhiteSpace(deviceInfo.ModelFamily) 
            ? Prometheus.Label("model_family", deviceInfo.ModelFamily) : null;
        PrometheusLabel? modelName = !string.IsNullOrWhiteSpace(deviceInfo.ModelName) 
            ? Prometheus.Label("model_name", deviceInfo.ModelName) : null;
        PrometheusLabel? deviceModel = !string.IsNullOrWhiteSpace(deviceInfo.DeviceModel) 
            ? Prometheus.Label("device_model", deviceInfo.DeviceModel) : null;
        PrometheusLabel? serialLabel = !string.IsNullOrWhiteSpace(device.SerialNumber) 
            ? Prometheus.Label("serial_number", device.SerialNumber) : null;
        PrometheusLabel? firmwareVersion = !string.IsNullOrWhiteSpace(deviceInfo.FirmwareVersion) 
            ? Prometheus.Label("firmware_version", deviceInfo.FirmwareVersion) : null;
        PrometheusLabel? vendor = !string.IsNullOrWhiteSpace(deviceInfo.Vendor) 
            ? Prometheus.Label("vendor", deviceInfo.Vendor) : null;
        PrometheusLabel? product = !string.IsNullOrWhiteSpace(deviceInfo.Product) 
            ? Prometheus.Label("product", deviceInfo.Product) : null;
        PrometheusLabel? revision = !string.IsNullOrWhiteSpace(deviceInfo.Revision) 
            ? Prometheus.Label("revision", deviceInfo.Revision) : null;
        PrometheusLabel? lunId = !string.IsNullOrWhiteSpace(deviceInfo.LunId) 
            ? Prometheus.Label("lun_id", deviceInfo.LunId) : null;

        prometheus.AddMetric("device_info", Prometheus.Gauge("Device information"), includeTimeStamp: false, samples => samples
            .AddSample(value: true, disk, type, modelFamily, modelName, deviceModel, serialLabel, firmwareVersion, vendor, product, revision, lunId));

        prometheus.AddMetric("smart_support_available", Prometheus.Gauge("SMART support available"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport?.Available ?? false, disk, type, serialLabel));
        prometheus.AddMetric("smart_support_enabled", Prometheus.Gauge("SMART support enabled"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceInfo.SmartSupport?.Enabled ?? false, disk, type, serialLabel));

        return true; // Continue with the next collector
    }
}