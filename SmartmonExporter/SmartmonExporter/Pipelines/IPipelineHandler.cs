namespace SmartmonExporter.Pipelines;

public interface IPipelineHandler
{
    int Priority { get; }
}
