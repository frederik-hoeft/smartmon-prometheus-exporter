using SmartmonExporter.Domain.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;
using System.Text.RegularExpressions;

namespace SmartmonExporter.Data.Collectors.DeviceCollectors;

internal sealed partial class DeviceAttributeCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 30;

    [GeneratedRegex(@"(?<num>^[0-9]+)")]
    private partial Regex RawNumberRegex { get; }

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlAtaDeviceAttributes deviceAttributes = await smartctlRunner.RunAsync<SmartctlAtaDeviceAttributes>(["--attributes"], device.Name, cancellationToken);
        PrometheusLabel disk = Prometheus.Label("disk", device.Name);
        PrometheusLabel type = Prometheus.Label("type", device.Type);
        foreach (AtaDeviceAttribute attribute in deviceAttributes.AtaDeviceAttributes.Table)
        {
            string normalizedName = attribute.Name.Replace('-', '_').ToLowerInvariant();
            PrometheusLabel id = Prometheus.Label("smart_id", attribute.Id);

            prometheus.AddMetric($"{normalizedName}_value", Prometheus.Gauge("Device attribute value"), includeTimeStamp: false, samples => samples
                .AddSample(value: attribute.Value, disk, type, id));

            prometheus.AddMetric($"{normalizedName}_worst", Prometheus.Gauge("Device attribute worst"), includeTimeStamp: false, samples => samples
                .AddSample(value: attribute.Worst, disk, type, id));

            prometheus.AddMetric($"{normalizedName}_threshold", Prometheus.Gauge("Device attribute threshold"), includeTimeStamp: false, samples => samples
                .AddSample(value: attribute.Threshold, disk, type, id));

            string rawValue = attribute.Raw.String;
            Match match = RawNumberRegex.Match(attribute.Raw.String);
            if (match.Success)
            {
                rawValue = match.Groups["num"].Value;
            }
            prometheus.AddMetric($"{normalizedName}_raw_value", Prometheus.Gauge("Device attribute raw value"), includeTimeStamp: false, samples => samples
                .AddSample(value: rawValue, disk, type, id));
        }

        if (deviceAttributes.AtaDeviceAttributes.Temperature is Temperature temperature)
        {
            prometheus.AddMetric("temperature_current", Prometheus.Gauge("Current device temperature"), includeTimeStamp: false, samples => samples
                .AddSample(value: temperature.Current, disk, type));
        }

        return true; // Continue with the next collector
    }
}