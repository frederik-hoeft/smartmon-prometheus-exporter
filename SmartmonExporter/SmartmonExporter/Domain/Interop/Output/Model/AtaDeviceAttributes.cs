namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record AtaDeviceAttributes(int Revision, AtaDeviceAttribute[] Table, PowerOnTime PowerOnTime, int PowerCycleCount, Temperature? Temperature);
