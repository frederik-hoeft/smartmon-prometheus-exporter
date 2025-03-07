using SmartmonExporter.Configuration.Model;

namespace SmartmonExporter.Configuration;

internal interface IConfiguration
{
    AppSettings Settings { get; }

    ValueTask<bool> TryReloadAsync(string settingsPath, CancellationToken cancellationToken);
}
