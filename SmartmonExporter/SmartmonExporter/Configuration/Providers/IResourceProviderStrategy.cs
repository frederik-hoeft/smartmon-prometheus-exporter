using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Configuration.Providers;

internal interface IResourceProviderStrategy : IPipelineHandler
{
    Stream? OpenRead(string resourceName);
}