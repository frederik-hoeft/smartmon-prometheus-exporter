using SmartmonExporter.Configuration;
using SmartmonExporter.Domain;
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

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "No -Async suffix because the name is used for CLI command parsing.")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ConsoleAppFramework requires instance method.")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Visual Studio is confused.")]
    public async Task Export(string configPath = "settings.json", CancellationToken cancellationToken = default)
    {
        await using DefaultServiceProvider serviceProvider = await GetServiceProviderAsync(configPath, cancellationToken);
        IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
        IMetricsExporter metricsExporter = serviceProvider.GetService<IMetricsExporter>();
        string prometheusNamespace = "smartmon";
        if (configuration.Settings.PrometheusNamespace is not null)
        {
            prometheusNamespace = $"{configuration.Settings.PrometheusNamespace}_{prometheusNamespace}";
        }
        string metrics = await metricsExporter.ExportAsync(prometheusNamespace, cancellationToken);
        await File.WriteAllTextAsync(configuration.Settings.OutputPath, metrics, cancellationToken);
    }
}