namespace SmartmonExporter.Domain.Interop.Output.Model;

internal record SmartctlDeviceInfo : SmartctlDevice
{
    public string? ModelFamily { get; init; }

    public string? ModelName { get; init; }

    public string? SerialNumber { get; init; }

    public string? FirmwareVersion { get; init; }

    public string? DeviceModel { get; init; }

    public string? Vendor { get; init; }

    public string? Product { get; init; }

    public string? Revision { get; init; }

    public string? LunId { get; init; }

    public UserCapacity UserCapacity { get; init; } = null!;

    public int LogicalBlockSize { get; init; }

    public int PhysicalBlockSize { get; init; }

    public int RotationRate { get; init; }

    public FormFactor FormFactor { get; init; } = null!;

    public Trim Trim { get; init; } = null!;

    public bool InSmartctlDatabase { get; init; }

    public SmartSupport SmartSupport { get; init; } = null!;
}
