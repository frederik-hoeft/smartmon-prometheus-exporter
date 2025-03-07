namespace SmartmonExporter.Json.Aot;

[AttributeUsage(AttributeTargets.Class)]
public sealed class JsonTypeInfoBindingsGeneratorAttribute : Attribute
{
    public BindingsGenerationMode GenerationMode { get; set; } = BindingsGenerationMode.Safe;
}
