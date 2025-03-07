using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Metrics.Factory;
using SmartmonExporter.Domain.Metrics;
using System.Text.RegularExpressions;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors.DeviceAttributeCollectors;

internal sealed partial class AtaDeviceAttributeCollector(ISmartctlRunner smartctlRunner) : IDeviceAttributeCollector
{
    public int Priority => 0;

    [GeneratedRegex(@"ata")]
    private partial Regex DeviceTypeRegex { get; }

    [GeneratedRegex(@"(?<num>^[0-9]+)")]
    private partial Regex RawNumberRegex { get; }

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        if (!DeviceTypeRegex.IsMatch(device.Type))
        {
            return false; // Skip this collector
        }
        SmartctlAtaDeviceAttributes deviceAttributes = await smartctlRunner.RunAsync<SmartctlAtaDeviceAttributes>(["--attributes"], device.Name, cancellationToken);
        PrometheusLabel disk = Prometheus.Label("disk", device.Name);
        PrometheusLabel type = Prometheus.Label("type", device.Type);
        foreach (AtaDeviceAttribute attribute in deviceAttributes.AtaSmartAttributes.Table)
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

        if (deviceAttributes.AtaSmartAttributes.Temperature is Temperature temperature)
        {
            prometheus.AddMetric("temperature_current", Prometheus.Gauge("Current device temperature"), includeTimeStamp: false, samples => samples
                .AddSample(value: temperature.Current, disk, type));
        }

        return true; // Successfully collected
    }
}
