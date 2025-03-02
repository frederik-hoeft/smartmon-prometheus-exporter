﻿using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Interop.Output.Model;

public readonly record struct LocalTime(long TimeT, string AscTime);

public sealed record Smartctl
(
    [property: JsonPropertyName("version")] int[] VersionRaw,
    string SvnRevision,
    string PlatformInfo,
    string BuildInfo,
    string[] Argv,
    int ExitStatus,
    string? DriveDatabaseVersion
)
{
    [JsonIgnore]
    public Version Version { get; } = VersionRaw switch
    {
        [int major] => new Version(major, 0),
        [int major, int minor] => new Version(major, minor),
        [int major, int minor, int build] => new Version(major, minor, build),
        [int major, int minor, int build, int revision] => new Version(major, minor, build, revision),
        _ => throw new InvalidOperationException("invalid version")
    };
}

public abstract record RootBase
{
    public required int[] JsonFormatVersion { get; init; }

    public required Smartctl Smartctl { get; init; }

    public LocalTime LocalTime { get; init; }
}

internal sealed record SmartctlVersion : RootBase;

internal sealed record SmartctlDevices(Device[] Devices) : RootBase;

internal sealed record Device(string Name, string InfoName, string Type, string Protocol);

internal record SmartctlDevice : RootBase
{
    public required Device Device { get; init; }
}

internal sealed record UserCapacity(long Blocks, long Bytes);

internal sealed record FormFactor(string Name, int AtaValue);

internal sealed record Trim(bool Supported, bool Deterministic, bool Zeroed);

internal sealed record Wwn(int Naa, int Oui, long Id);

internal sealed record AtaVersion(string String, int MajorValue, int MinorValue)
{
    [JsonIgnore]
    public Version Version { get; } = new(MajorValue, MinorValue);
}

internal sealed record SataVersion(string String, int Value);

internal sealed record SataInterfaceSpeedValue(string String, int SataValue, int UnitsPerSecond, long BitsPerUnit);

internal sealed record SataInterfaceSpeed(SataInterfaceSpeedValue Max, SataInterfaceSpeedValue Current);

internal sealed record SmartSupport(bool Available, bool Enabled);

internal sealed record SmartStatus(bool Passed);

internal sealed record SmartctlDeviceHealth(SmartStatus SmartStatus) : SmartctlDevice;

internal record SmartctlDeviceInfo : SmartctlDevice
{
    public required string ModelFamily { get; init; }

    public required string ModelName { get; init; }

    public required string SerialNumber { get; init; }

    public required string FirmwareVersion { get; init; }

    public required UserCapacity UserCapacity { get; init; }

    public int LogicalBlockSize { get; init; }

    public int PhysicalBlockSize { get; init; }

    public int RotationRate { get; init; }

    public required FormFactor FormFactor { get; init; }

    public required Trim Trim { get; init; }

    public bool InSmartctlDatabase { get; init; }

    public required SmartSupport SmartSupport { get; init; }
}

internal sealed record SmartctlSataDeviceInfo : SmartctlDeviceInfo
{
    public required Wwn Wwn { get; init; }

    public required AtaVersion AtaVersion { get; init; }

    public required SataVersion SataVersion { get; init; }

    public required SataInterfaceSpeed SataInterfaceSpeed { get; init; }
}

internal sealed record SmartctlAtaDeviceAttributes(AtaDeviceAttributes AtaDeviceAttributes) : SmartctlDevice;

internal sealed record AtaDeviceAttributes(int Revision, AtaDeviceAttribute[] Table, PowerOnTime PowerOnTime, int PowerCycleCount, Temperature? Temperature);

internal sealed record AtaDeviceAttribute
(
    int Id,
    string Name,
    int Value,
    int Worst,
    [property: JsonPropertyName("thresh")]int Threshold,
    string WhenFailed,
    AtaDeviceAttributeFlags Flags,
    AtaDeviceAttributeRaw Raw
);

internal sealed record AtaDeviceAttributeFlags(int Value, string String, bool Prefailure, bool UpdatedOnline, bool Performance, bool ErrorRate, bool EventCount, bool AutoKeep);

internal sealed record AtaDeviceAttributeRaw(int Value, string String);

internal sealed record PowerOnTime(int Hours);

internal sealed record Temperature(int Current, int Min, int Max);