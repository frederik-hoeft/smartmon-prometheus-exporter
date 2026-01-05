using SmartmonExporter.Domain.Metrics.Factory;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Interop.Output.Model;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class TimestampCollector : IDeviceMetricCollector
{
    // Run after DeviceInfoCollector to ensure serial number is initialized
    // DeviceInfoCollector has Priority = int.MinValue
    public int Priority => int.MinValue + 1;

    public ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        string metricName = "smartctl_run";
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        PrometheusLabel? serialLabel = !string.IsNullOrWhiteSpace(device.SerialNumber) 
            ? Prometheus.Label("serial_number", device.SerialNumber) 
            : null;

        prometheus.AddMetric(metricName, Prometheus.Gauge("smartctl run timestamp"), includeTimeStamp: false, samples => samples
            .AddSample(value: timestamp, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type), serialLabel));

        return ValueTask.FromResult(true); // Continue with the next collector
    }
}
