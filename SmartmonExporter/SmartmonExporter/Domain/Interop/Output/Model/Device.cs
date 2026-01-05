namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed class Device(string name, string infoName, string type, string protocol)
{
    public string Name { get; } = name;
    public string InfoName { get; } = infoName;
    public string Type { get; } = type;
    public string Protocol { get; } = protocol;
    public string? SerialNumber { get; set; }
}
