using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Metrics.Factory;
using SmartmonExporter.Domain.Metrics;
using System.Text.RegularExpressions;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors.DeviceAttributeCollectors;

internal sealed partial class NvmeDeviceAttributeCollector(ISmartctlRunner smartctlRunner) : IDeviceAttributeCollector
{
    public int Priority => 10;

    [GeneratedRegex(@"nvme")]
    private partial Regex DeviceTypeRegex { get; }

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        if (!DeviceTypeRegex.IsMatch(device.Type))
        {
            return false; // Skip this collector
        }
        SmartctlNvmeDeviceAttributes deviceAttributes = await smartctlRunner.RunAsync<SmartctlNvmeDeviceAttributes>(["--attributes"], device.Name, cancellationToken);
        PrometheusLabel disk = Prometheus.Label("disk", device.Name);
        PrometheusLabel type = Prometheus.Label("type", device.Type);
        
        foreach ((string key, long value) in deviceAttributes.NvmeSmartHealthInformationLog)
        {
            string normalizedName = key.Replace('-', '_').ToLowerInvariant();
            
            List<PrometheusLabel> labels = [disk, type];
            if (!string.IsNullOrWhiteSpace(device.SerialNumber))
            {
                labels.Add(Prometheus.Label("serial_number", device.SerialNumber));
            }

            prometheus.AddMetric(normalizedName, Prometheus.Gauge($"NVMe SMART health information log entry {key}"), includeTimeStamp: false, samples => samples
                .AddSample(value, [.. labels]));
        }

        if (deviceAttributes.Temperature is Temperature temperature)
        {
            List<PrometheusLabel> tempLabels = [disk, type];
            if (!string.IsNullOrWhiteSpace(device.SerialNumber))
            {
                tempLabels.Add(Prometheus.Label("serial_number", device.SerialNumber));
            }

            prometheus.AddMetric("temperature_current", Prometheus.Gauge("Current device temperature"), includeTimeStamp: false, samples => samples
                .AddSample(value: temperature.Current, [.. tempLabels]));
        }

        return true; // Successfully collected
    }
}
