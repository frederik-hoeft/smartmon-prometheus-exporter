namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record Trim(bool Supported, bool Deterministic, bool Zeroed);
