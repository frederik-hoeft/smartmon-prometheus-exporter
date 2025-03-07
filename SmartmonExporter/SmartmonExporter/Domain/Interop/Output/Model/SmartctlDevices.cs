namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record SmartctlDevices(Device[]? Devices) : RootBase;
