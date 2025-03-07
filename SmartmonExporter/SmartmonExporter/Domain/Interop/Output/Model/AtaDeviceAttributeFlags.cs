namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record AtaDeviceAttributeFlags(long Value, string String, bool Prefailure, bool UpdatedOnline, bool Performance, bool ErrorRate, bool EventCount, bool AutoKeep);
