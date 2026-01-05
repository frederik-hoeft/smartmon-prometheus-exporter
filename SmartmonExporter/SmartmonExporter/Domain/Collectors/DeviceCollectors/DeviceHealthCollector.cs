using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Domain.Metrics;
using SmartmonExporter.Domain.Metrics.Factory;
using System.Buffers;
using System.Collections.Immutable;

namespace SmartmonExporter.Domain.Collectors.DeviceCollectors;

internal sealed class DeviceHealthCollector(ISmartctlRunner smartctlRunner) : IDeviceMetricCollector
{
    public int Priority => 20;

    private static ImmutableArray<(SmartctlErrorCode Status, string Name)> StatusFlags =>
    [
        (SmartctlErrorCode.CommandExecutionError, "command_execution_error"),
        (SmartctlErrorCode.DiskFailing, "disk_failing"),
        (SmartctlErrorCode.DiskPreFail, "disk_prefail"),
        (SmartctlErrorCode.DiskPreFailInPast, "disk_prefail_in_past"),
        (SmartctlErrorCode.LogContainsErrors, "log_contains_errors"),
        (SmartctlErrorCode.OpenDeviceFailed, "open_device_failed"),
        (SmartctlErrorCode.SelfTestErrors, "self_test_errors"),
    ];

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceHealth deviceHealth = await smartctlRunner.RunAsync<SmartctlDeviceHealth>(["--health"], device.Name, cancellationToken);
        
        PrometheusLabel? serialLabel = Prometheus.OptionalLabel("serial_number", device.SerialNumber);
        
        prometheus.AddMetric("smart_status_summary", Prometheus.Gauge("SMART status summary"), includeTimeStamp: false, samples =>
        {
            DiskHealth health = DiskHealth.Ok;
            // Maximum size: StatusFlags + 1 for health + 2 for disk/type + 1 for optional serial_number
            int maxLength = StatusFlags.Length + 1 + 2 + 1;
            PrometheusLabel?[] buffer = ArrayPool<PrometheusLabel?>.Shared.Rent(maxLength);
            Span<PrometheusLabel?> labels = buffer.AsSpan();
            int i = 0;
            for (; i < StatusFlags.Length; ++i)
            {
                (SmartctlErrorCode status, string name) = StatusFlags[i];
                if (deviceHealth.Smartctl.ExitStatus.HasFlag(status))
                {
                    labels[i] = Prometheus.Label(name, "yes");
                    health |= status switch
                    {
                        SmartctlErrorCode.CommandExecutionError => DiskHealth.Failed,
                        SmartctlErrorCode.DiskFailing => DiskHealth.Failing,
                        SmartctlErrorCode.DiskPreFail => DiskHealth.PreFail,
                        SmartctlErrorCode.DiskPreFailInPast => DiskHealth.Degraded,
                        SmartctlErrorCode.LogContainsErrors => DiskHealth.Degraded,
                        SmartctlErrorCode.OpenDeviceFailed => DiskHealth.Failed,
                        SmartctlErrorCode.SelfTestErrors => DiskHealth.Degraded,
                        _ => DiskHealth.Ok
                    };
                }
                else
                {
                    labels[i] = Prometheus.Label(name, "no");
                }
            }
            labels[i++] = Prometheus.Label("health", GetHealthStatus(health));
            labels[i++] = Prometheus.Label("disk", device.Name);
            labels[i++] = Prometheus.Label("type", device.Type);
            labels[i++] = serialLabel;
            
            samples.AddSample(value: health is DiskHealth.Ok or DiskHealth.Degraded, labels[..i]);
            ArrayPool<PrometheusLabel?>.Shared.Return(buffer);
        });

        prometheus.AddMetric("smart_status_passed", Prometheus.Gauge("SMART status passed"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceHealth.SmartStatus?.Passed is true, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type), serialLabel));
        return deviceHealth.SmartStatus is not null;
    }

    private static string GetHealthStatus(DiskHealth health) => health switch
    {
        DiskHealth.Ok => "ok",
        DiskHealth.Degraded => "degraded",
        DiskHealth.PreFail => "prefail",
        DiskHealth.Failing => "failing",
        DiskHealth.Failed => "failed",
        _ => "unknown"
    };

    private enum DiskHealth
    {
        Ok = 0x0,
        Degraded = 0x1,
        PreFail = 0x3,
        Failing = 0x7,
        Failed = 0xf
    }
}