namespace SmartmonExporter.Interop.Output.Model;

public abstract record RootBase
{
    public required int[] JsonFormatVersion { get; init; }

    public required Smartctl Smartctl { get; init; }

    public LocalTime LocalTime { get; init; }
}
