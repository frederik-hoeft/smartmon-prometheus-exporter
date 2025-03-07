namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record SataInterfaceSpeedValue(string String, int SataValue, int UnitsPerSecond, long BitsPerUnit);
