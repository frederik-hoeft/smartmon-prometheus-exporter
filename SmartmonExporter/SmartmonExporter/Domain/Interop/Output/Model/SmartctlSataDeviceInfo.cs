namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record SmartctlSataDeviceInfo : SmartctlDeviceInfo
{
    public required Wwn Wwn { get; init; }

    public required AtaVersion AtaVersion { get; init; }

    public required SataVersion SataVersion { get; init; }

    public required SataInterfaceSpeed SataInterfaceSpeed { get; init; }
}
