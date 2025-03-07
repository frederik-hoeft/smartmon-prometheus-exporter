using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SmartmonExporter.Json.Aot.Generator.Internals;
using System.Collections.Immutable;
using System.Text;

namespace SmartmonExporter.Json.Aot.Generator;

[Generator(LanguageNames.CSharp)]
public class GenericJsonTypeInfoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<Model> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "SmartmonExporter.Json.Aot.JsonTypeInfoBindingsGeneratorAttribute",
            predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            transform: static (context, _) =>
            {
                ISymbol targetClass = context.TargetSymbol;
                ImmutableArray<AttributeData> attributes = targetClass.GetAttributes();
                AttributeData genericJsonTypeInfoBindingsAttribute = attributes.FirstOrDefault(static attr => attr.AttributeClass.EqualsFullyQualifiedType(["SmartmonExporter", "Json", "Aot"], "JsonTypeInfoBindingsGeneratorAttribute"))
                    ?? throw new InvalidOperationException($"{nameof(GenericJsonTypeInfoGenerator)} requires JsonTypeInfoBindingsGeneratorAttribute to be applied to the class");
                ImmutableArray<AttributeData> jsonSerializables = 
                [
                    .. attributes.Where(static attr => attr.AttributeClass.EqualsFullyQualifiedType(["System", "Text", "Json", "Serialization"], "JsonSerializableAttribute"))
                ];
                return new Model(
                    Namespace: targetClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                    Class: targetClass,
                    GeneratorAttribute: genericJsonTypeInfoBindingsAttribute,
                    JsonSerializableAttributes: jsonSerializables);
            }
        );
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            JsonSerializableAttributeParser parser = new(context);
            string? overrideModifier = GetOptionalOverrideModifier(model);

            StringBuilder sourceBuilder = new(
                $$"""
                #nullable enable
  
                namespace {{model.Namespace}};
 
                partial class {{model.Class.Name}}
                {
                    public {{overrideModifier}}global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>? GetTypeInfoOrDefault<T>()
                    {
                        // the JIT will optimize this switch statement away
                        return (object?)null switch
                        {

                """);

            string indent = new(' ', 3 * 4);
            // 1 = Optimized, 0 = Boxed Cast
            bool useFastTypeCast = model.GeneratorAttribute.NamedArguments.Any(static arg => arg is { Key: "GenerationMode", Value.Value: 1 });

            foreach (AttributeData jsonSerializable in model.JsonSerializableAttributes)
            {
                INamedTypeSymbol? type = parser.GetTargetType(jsonSerializable);
                if (type is null)
                {
                    continue;
                }
                sourceBuilder.Append(indent)
                    .Append($"_ when typeof(T) == typeof({type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}) => ");
                if (useFastTypeCast)
                {
                    sourceBuilder.AppendLine($"global::System.Runtime.CompilerServices.Unsafe.As<global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>>({type.Name}),");
                }
                else
                {
                    sourceBuilder.AppendLine($"(global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)(object?){type.Name},");
                }
            }
            sourceBuilder.Append(
                """
                            _ => null,
                        };
                    }
                }
                """);

            SourceText sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);

            context.AddSource($"{model.Class.Name}.GenericJsonTypeInfoProvider.g.cs", sourceText);
        });
    }

    private static string? GetOptionalOverrideModifier(Model model)
    {
        // if the class inherits from AotJsonSerializerContext, then we need to generate an override for GetTypeInfoOrDefault<T>
        string? overrideModifier = null;

        // traverse the inheritance hierarchy to see if the class inherits from AotJsonSerializerContext
        for (INamedTypeSymbol? namedTypeSymbol = model.Class as INamedTypeSymbol; namedTypeSymbol is not null; namedTypeSymbol = namedTypeSymbol.BaseType)
        {
            if (namedTypeSymbol.EqualsFullyQualifiedType(["SmartmonExporter", "Json", "Aot"], "AotJsonSerializerContext"))
            {
                // include the space after the override keyword
                overrideModifier = "override ";
                break;
            }
        }

        return overrideModifier;
    }

    private sealed record Model(string Namespace, ISymbol Class, AttributeData GeneratorAttribute, ImmutableArray<AttributeData> JsonSerializableAttributes);
}
