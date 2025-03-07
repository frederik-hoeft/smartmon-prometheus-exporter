using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Domain.Interop.Output.Model;

public sealed record Smartctl
(
    [property: JsonPropertyName("version")] int[] VersionRaw,
    string SvnRevision,
    string PlatformInfo,
    string BuildInfo,
    string[] Argv,
    SmartctlExitStatus ExitStatus,
    [property: JsonConverter(typeof(DriveDatabaseVersionConverter))] string? DriveDatabaseVersion
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

[Flags]
public enum SmartctlExitStatus : int
{
    Ok =                    0x0,
    ArgParseError =         0x1,
    OpenDeviceFailed =      0x2,
    CommandExecutionError = 0x4,
    DiskFailing =           0x8,
    DiskPreFail =           0x10,
    DiskPreFailInPast =     0x20,
    LogContainsErrors =     0x40,
    SelfTestErrors =        0x80,
}

/*
"drive_database_version": {
    "string": "7.3/5319"
},
OR
"drive_database_version": "7.3/5319"
*/
internal sealed class DriveDatabaseVersionConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return document.RootElement.ValueKind switch
        {
            JsonValueKind.Object => document.RootElement.GetProperty("string").GetString(),
            JsonValueKind.String => document.RootElement.GetString(),
            _ => throw new JsonException("invalid drive database version")
        };
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}