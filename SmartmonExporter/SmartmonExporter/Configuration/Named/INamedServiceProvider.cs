namespace SmartmonExporter.Configuration.Named;

internal interface INamedServiceProvider
{
    T? GetService<T>(string name) where T : class;
}