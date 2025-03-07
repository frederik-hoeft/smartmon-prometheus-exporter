using SmartmonExporter.Configuration.Model;
using SmartmonExporter.Configuration.Providers;
using System.Text.Json;

namespace SmartmonExporter.Configuration;

internal sealed class ConfigurationImpl(AppSettingsJsonSerializerContext jsonSerializerContext, IResourceProvider appSettingsProvider) : IConfiguration
{
    private AppSettings? _settings;

    public AppSettings Settings => _settings ?? throw new InvalidOperationException("configuration has not been loaded");

    public async ValueTask<bool> TryReloadAsync(string settingsPath, CancellationToken cancellationToken)
    {
        await using Stream? stream = appSettingsProvider.OpenRead(settingsPath);
        if (stream is not null)
        {
            AppSettings? settings = await JsonSerializer.DeserializeAsync(stream, jsonSerializerContext.GetTypeInfo<AppSettings>(), cancellationToken);
            if (settings is not null)
            {
                settings.AssertIsValid();
                _settings = settings;
                return true;
            }
        }
        return false;
    }
}
