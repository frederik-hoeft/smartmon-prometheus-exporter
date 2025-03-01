using System.Text.Json.Serialization;

namespace SmartmonExporter.Interop.Output.Model;

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
