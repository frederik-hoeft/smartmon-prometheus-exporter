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

    private static ImmutableArray<(SmartctlExitStatus Status, string Name)> StatusFlags =>
    [
        (SmartctlExitStatus.CommandExecutionError, "command_execution_error"),
        (SmartctlExitStatus.DiskFailing, "disk_failing"),
        (SmartctlExitStatus.DiskPreFail, "disk_prefail"),
        (SmartctlExitStatus.DiskPreFailInPast, "disk_prefail_in_past"),
        (SmartctlExitStatus.LogContainsErrors, "log_contains_errors"),
        (SmartctlExitStatus.OpenDeviceFailed, "open_device_failed"),
        (SmartctlExitStatus.SelfTestErrors, "self_test_errors"),
    ];

    public async ValueTask<bool> TryCollectAsync(Device device, PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDeviceHealth deviceHealth = await smartctlRunner.RunAsync<SmartctlDeviceHealth>(["--health"], device.Name, cancellationToken);
        prometheus.AddMetric("smart_status_summary", Prometheus.Gauge("SMART status summary"), includeTimeStamp: false, samples =>
        {
            DiskHealth health = DiskHealth.Ok;
            // + 1 for the overall health status + 2 for the disk and type labels
            int length = StatusFlags.Length + 1 + 2;
            PrometheusLabel[] buffer = ArrayPool<PrometheusLabel>.Shared.Rent(length);
            Span<PrometheusLabel> labels = buffer.AsSpan(0, length);
            int i = 0;
            for (; i < StatusFlags.Length; ++i)
            {
                (SmartctlExitStatus status, string name) = StatusFlags[i];
                if (deviceHealth.Smartctl.ExitStatus.HasFlag(status))
                {
                    labels[i] = Prometheus.Label(name, "yes");
                    health |= status switch
                    {
                        SmartctlExitStatus.CommandExecutionError => DiskHealth.Failed,
                        SmartctlExitStatus.DiskFailing => DiskHealth.Failing,
                        SmartctlExitStatus.DiskPreFail => DiskHealth.PreFail,
                        SmartctlExitStatus.DiskPreFailInPast => DiskHealth.Degraded,
                        SmartctlExitStatus.LogContainsErrors => DiskHealth.Degraded,
                        SmartctlExitStatus.OpenDeviceFailed => DiskHealth.Failed,
                        SmartctlExitStatus.SelfTestErrors => DiskHealth.Degraded,
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
            labels[i] = Prometheus.Label("type", device.Type);
            samples.AddSample(value: health is DiskHealth.Ok or DiskHealth.Degraded, labels);
            ArrayPool<PrometheusLabel>.Shared.Return(buffer);
        });
        prometheus.AddMetric("smart_status_passed", Prometheus.Gauge("SMART status passed"), includeTimeStamp: false, samples => samples
            .AddSample(value: deviceHealth.SmartStatus?.Passed is true, Prometheus.Label("disk", device.Name), Prometheus.Label("type", device.Type)));
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