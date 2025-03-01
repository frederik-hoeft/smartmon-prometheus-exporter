using Jab;
using OrchestratorNg.Server.Configuration;
using SmartmonExporter.Configuration;
using SmartmonExporter.Configuration.Named;
using SmartmonExporter.Configuration.Providers;

namespace SmartmonExporter;

[ServiceProvider]
[Singleton<AppSettingsJsonSerializerContext>]
[Singleton<IConfiguration, ConfigurationImpl>]
[Singleton<INamedServiceProvider, NamedServiceProvider>]
[Singleton<IResourceProvider, ResourceProvider>]
[Singleton<IResourceProviderStrategy, FileSystemResourceProviderStrategy>]
internal partial class DefaultServiceProvider;