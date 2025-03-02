using SmartmonExporter.Configuration.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Configuration;

[JsonSourceGenerationOptions(ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true)]
[JsonSerializable(typeof(AppSettings))]
internal sealed partial class AppSettingsJsonSerializerContext : JsonSerializerContext;