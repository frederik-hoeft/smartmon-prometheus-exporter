using SmartmonExporter.Data.Metrics.Factory;
using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Interop.Output.Model;

namespace SmartmonExporter.Data.Collectors.DeviceCollectors;

internal sealed class TimestampCollector : IDeviceMetricCollector
{
    public int Priority => int.MinValue; // Ensure this runs first in the pipeline

    public ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        string metricName = "smartctl_run";
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        prometheus.AddMetric(metricName, Prometheus.Gauge("smartctl run timestamp"), includeTimeStamp: false, samples => samples
            .AddSample(value: timestamp, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type)));

        return ValueTask.FromResult(true); // Continue with the next collector
    }
}
