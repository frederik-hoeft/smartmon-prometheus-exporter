namespace SmartmonExporter.Domain.Metrics;

internal sealed record DiskContext(string? Disk)
{
    public bool IsActive { get; set; }
}
