using SmartmonExporter.Configuration.Model;
using SmartmonExporter.Configuration.Providers;
using System.Text.Json;

namespace SmartmonExporter.Configuration;

internal sealed class ConfigurationImpl(AppSettingsJsonSerializerContext jsonSerializerContext, IResourceProvider appSettingsProvider) : IConfiguration
{
    private const string APP_SETTINGS_FILE = "settings.json";
    private AppSettings? _settings;

    public AppSettings Settings => _settings ?? throw new InvalidOperationException("configuration has not been loaded");

    public async ValueTask<bool> TryReloadAsync(CancellationToken cancellationToken)
    {
        await using Stream? stream = appSettingsProvider.OpenRead(APP_SETTINGS_FILE);
        if (stream is not null)
        {
            object? settings = await JsonSerializer.DeserializeAsync(stream, typeof(AppSettings), jsonSerializerContext, cancellationToken);
            _settings = settings as AppSettings;
            _settings?.AssertIsValid();
            return _settings is not null;
        }
        return false;
    }
}
