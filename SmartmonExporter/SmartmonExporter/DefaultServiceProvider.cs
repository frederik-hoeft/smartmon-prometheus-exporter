using Jab;
using SmartmonExporter.Configuration;
using SmartmonExporter.Configuration.Named;
using SmartmonExporter.Configuration.Providers;
using SmartmonExporter.Data;
using SmartmonExporter.Interop;
using SmartmonExporter.Interop.Output;

namespace SmartmonExporter;

[ServiceProvider]
[Singleton<AppSettingsJsonSerializerContext>]
[Singleton<SmartctlJsonSerializerContext>]
[Singleton<IConfiguration, ConfigurationImpl>]
[Singleton<INamedServiceProvider, NamedServiceProvider>]
[Singleton<IResourceProvider, ResourceProvider>]
[Singleton<IResourceProviderStrategy, FileSystemResourceProviderStrategy>]
[Singleton<IChildProcessRunner, DefaultChildProcessRunner>]
[Singleton<ISmartctlRunner, SmartctlRunner>]
[Import<ISmartMetricsModule>]
internal sealed partial class DefaultServiceProvider;