using SmartmonExporter.Data.Metrics;
using SmartmonExporter.Interop;
using SmartmonExporter.Interop.Output.Model;

namespace SmartmonExporter.Data.Collectors;

public class SmartCollector(ISmartctlRunner smartctlRunner) : IMetricsCollector
{
    public int Priority => 0;

    public async ValueTask CollectAsync(PrometheusBuilder prometheus, CancellationToken cancellationToken)
    {
        SmartctlDevices devices = await smartctlRunner.RunAsync<SmartctlDevices>(["--scan-open"], device: null, cancellationToken);
        // TODO: ...
    }
}
