namespace SmartmonExporter.Configuration.Named;

public interface INamedServiceProvider
{
    T? GetService<T>(string name) where T : class;
}