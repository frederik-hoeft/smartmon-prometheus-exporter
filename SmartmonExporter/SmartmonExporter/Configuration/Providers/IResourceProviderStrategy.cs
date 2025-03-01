using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Configuration.Providers;

public interface IResourceProviderStrategy : IPipelineHandler
{
    Stream? OpenRead(string resourceName);
}