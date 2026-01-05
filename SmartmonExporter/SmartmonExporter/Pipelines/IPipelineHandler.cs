namespace SmartmonExporter.Pipelines;

internal interface IPipelineHandler
{
    int Priority { get; }
}
