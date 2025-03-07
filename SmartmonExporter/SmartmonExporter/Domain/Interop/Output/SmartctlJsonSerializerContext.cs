using SmartmonExporter.Domain.Interop.Output.Model;
using SmartmonExporter.Json.Aot;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Domain.Interop.Output;

[JsonTypeInfoBindingsGenerator(GenerationMode = BindingsGenerationMode.Optimized)]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(SmartctlVersion))]
[JsonSerializable(typeof(SmartctlDevices))]
[JsonSerializable(typeof(SmartctlDevice))]
[JsonSerializable(typeof(SmartctlDeviceInfo))]
[JsonSerializable(typeof(SmartctlDeviceHealth))]
[JsonSerializable(typeof(SmartctlSataDeviceInfo))]
[JsonSerializable(typeof(SmartctlAtaDeviceAttributes))]
[JsonSerializable(typeof(SmartctlNvmeDeviceAttributes))]
internal sealed partial class SmartctlJsonSerializerContext : AotJsonSerializerContext;