using SmartmonExporter.Interop.Output.Model;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Interop.Output;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(RootBase))]
[JsonSerializable(typeof(SmartctlVersion))]
[JsonSerializable(typeof(SmartctlDevices))]
[JsonSerializable(typeof(SmartctlDevice))]
[JsonSerializable(typeof(SmartctlDeviceHealth))]
[JsonSerializable(typeof(SmartctlSataDeviceInfo))]
[JsonSerializable(typeof(SmartctlAtaDeviceAttributes))]
internal sealed partial class SmartctlJsonSerializerContext : JsonSerializerContext;