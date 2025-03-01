namespace SmartmonExporter.Configuration.Providers;

public interface IResourceProvider
{
    IResourceProviderStrategy? PreferredStrategy { get; set; }

    Stream? OpenRead(string resourceName);
}
