namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record SmartctlDeviceHealth(SmartStatus SmartStatus) : SmartctlDevice;
