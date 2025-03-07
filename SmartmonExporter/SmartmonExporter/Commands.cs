using SmartmonExporter.Configuration;
using SmartmonExporter.Domain;
using SmartmonExporter.Domain.Writers;
using System.Diagnostics.CodeAnalysis;

namespace SmartmonExporter;

public class Commands
{
    private static async ValueTask<DefaultServiceProvider> GetServiceProviderAsync(string configPath, CancellationToken cancellationToken)
    {
        DefaultServiceProvider serviceProvider = new();
        IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
        if (configuration is ConfigurationImpl impl)
        {
            await impl.TryReloadAsync(configPath, cancellationToken);
        }
        return serviceProvider;
    }

    /// <summary>
    /// Export metrics to Prometheus format.
    /// </summary>
    /// <param name="configPath">The path to the configuration file.</param>
    /// <param name="writeToStdout">Whether to write the output to stdout instead of the configured output path. If left unspecified, the output path specified in the configuration file will be used.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "No -Async suffix because the name is used for CLI command parsing.")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ConsoleAppFramework requires instance method.")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Visual Studio is confused.")]
    public async Task Export(string configPath = "settings.json", bool? writeToStdout = default, CancellationToken cancellationToken = default)
    {
        await using DefaultServiceProvider serviceProvider = await GetServiceProviderAsync(configPath, cancellationToken);
        IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
        if (writeToStdout.HasValue)
        {
            configuration.Settings.WriteToConsole = writeToStdout.Value;
        }
        // Disable debug mode if writing to console.
        if (configuration.Settings.WriteToConsole)
        {
            configuration.Settings.DebugMode = false;
        }
        IMetricsExporter metricsExporter = serviceProvider.GetService<IMetricsExporter>();
        string prometheusNamespace = "smartmon";
        if (configuration.Settings.PrometheusNamespace is not null)
        {
            prometheusNamespace = $"{configuration.Settings.PrometheusNamespace}_{prometheusNamespace}";
        }
        string metrics = await metricsExporter.ExportAsync(prometheusNamespace, cancellationToken);
        IEnumerable<IOutputWriter> writers = serviceProvider.GetService<IEnumerable<IOutputWriter>>();
        foreach (IOutputWriter writer in writers)
        {
            await writer.WriteAsync(metrics, cancellationToken);
        }
    }
}