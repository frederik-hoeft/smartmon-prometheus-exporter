using System.Text.Json.Serialization;

namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record AtaVersion(string String, int MajorValue, int MinorValue)
{
    [JsonIgnore]
    public Version Version { get; } = new(MajorValue, MinorValue);
}
