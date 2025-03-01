namespace SmartmonExporter.Configuration;

internal static class ServiceProviderExtensions
{
    public static async Task AddConfigurationAsync(this DefaultServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
        if (configuration is ConfigurationImpl impl)
        {
            await impl.TryReloadAsync(cancellationToken);
        }
    }
}
