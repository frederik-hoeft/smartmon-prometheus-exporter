using SmartmonExporter.Configuration.Model;
using SmartmonExporter.Json.Aot;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Configuration;

[JsonTypeInfoBindingsGenerator(GenerationMode = BindingsGenerationMode.Optimized)]
[JsonSourceGenerationOptions(ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true)]
[JsonSerializable(typeof(AppSettings))]
internal sealed partial class AppSettingsJsonSerializerContext : AotJsonSerializerContext;