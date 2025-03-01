namespace SmartmonExporter.Interop.Output.Model;

internal sealed record class SmartctlVersion : RootBase;

internal sealed record class SmartctlDevices(Device[] Devices) : RootBase;

internal sealed record class Device(string Name, string InfoName, string Type, string Protocol);