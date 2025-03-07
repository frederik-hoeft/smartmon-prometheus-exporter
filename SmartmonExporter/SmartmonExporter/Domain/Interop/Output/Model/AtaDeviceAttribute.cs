using System.Text.Json.Serialization;

namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record AtaDeviceAttribute
(
    int Id,
    string Name,
    long Value,
    long Worst,
    [property: JsonPropertyName("thresh")] long Threshold,
    string WhenFailed,
    AtaDeviceAttributeFlags Flags,
    AtaDeviceAttributeRaw Raw
);
