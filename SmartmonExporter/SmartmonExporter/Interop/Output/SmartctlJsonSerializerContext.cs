using SmartmonExporter.Configuration.Model;
using SmartmonExporter.Interop.Output.Model;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Interop.Output;

[JsonSerializable(typeof(VersionRoot))]
internal sealed partial class SmartctlJsonSerializerContext : JsonSerializerContext;