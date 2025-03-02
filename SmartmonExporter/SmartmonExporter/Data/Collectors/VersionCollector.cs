﻿using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Data.Metrics.Factory;
using SmartmonExporter.Interop;
using SmartmonExporter.Interop.Output.Model;

namespace SmartmonExporter.Data.Collectors;

internal sealed class VersionCollector(ISmartctlRunner smartctlRunner) : IMetricsCollector
{
    public int Priority => -1;

    public async ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlVersion version = await smartctlRunner.RunAsync<SmartctlVersion>(["--version"], device: null, cancellationToken);
        prometheus.AddMetric("smartctl_version", Prometheus.Gauge("smartctl version"), includeTimeStamp: false, samples => samples
            .AddSample(value: 1, Prometheus.Label("version", version.Smartctl.Version.ToString())));
    }
}
