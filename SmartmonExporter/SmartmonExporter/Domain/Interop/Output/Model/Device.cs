namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record class Device(string Name, string InfoName, string Type, string Protocol)
{
    public string? SerialNumber { get; set; }
}
