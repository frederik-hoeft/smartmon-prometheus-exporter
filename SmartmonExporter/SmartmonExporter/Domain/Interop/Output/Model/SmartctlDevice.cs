namespace SmartmonExporter.Domain.Interop.Output.Model;

internal record SmartctlDevice : RootBase
{
    public Device Device { get; init; } = null!;
}
