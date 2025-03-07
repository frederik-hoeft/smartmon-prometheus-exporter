namespace SmartmonExporter.Domain.Interop.Output.Model;

public abstract record RootBase
{
    public int[] JsonFormatVersion { get; init; } = null!;

    public Smartctl Smartctl { get; init; } = null!;

    public LocalTime LocalTime { get; init; }
}
