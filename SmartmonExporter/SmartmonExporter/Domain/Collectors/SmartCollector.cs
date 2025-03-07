using SmartmonExporter.Configuration;
using SmartmonExporter.Domain.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Pipelines;
using System.Collections.Immutable;
using System.Data.SqlTypes;

namespace SmartmonExporter.Domain.Collectors;

internal sealed class SmartCollector(IConfiguration configuration, ISmartctlRunner smartctlRunner, IEnumerable<IDeviceMetricCollector> diskMetricCollectors) : IMetricsCollector
{
    private readonly ImmutableArray<IDeviceMetricCollector> _diskMetricCollectors = diskMetricCollectors.CreatePipeline();

    public int Priority => 0;

    public async ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        ICollection<Device>? devices;
        if (configuration.Settings.Devices is not null)
        {
            List<Device> deviceList = [];
            foreach (string device in configuration.Settings.Devices)
            {
                SmartctlDevice smartctlDevice = await smartctlRunner.RunAsync<SmartctlDevice>(["-n", "standby"], device, cancellationToken);
                if (smartctlDevice.Device is not null)
                {
                    deviceList.Add(smartctlDevice.Device);
                }
            }
            devices = deviceList;
        }
        else
        {
            SmartctlDevices smartctlDevices = await smartctlRunner.RunAsync<SmartctlDevices>(["--scan-open"], device: null, cancellationToken);
            devices = smartctlDevices.Devices;
        }
        if (devices is not { Count: > 0 })
        {
            Console.WriteLine($"smartctl failed to scan devices: no devices found");
            return;
        }
        foreach (Device device in devices)
        {
            foreach (IDeviceMetricCollector diskMetricCollector in _diskMetricCollectors)
            {
                bool success = await diskMetricCollector.TryCollectAsync(device, prometheus, cancellationToken);
                if (!success)
                {
                    break;
                }
            }
        }
    }
}
