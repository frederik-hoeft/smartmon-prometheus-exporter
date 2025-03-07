namespace SmartmonExporter.Domain.Metrics;

public record DiskContext(string? Disk)
{
    public bool IsActive { get; set; }
}
