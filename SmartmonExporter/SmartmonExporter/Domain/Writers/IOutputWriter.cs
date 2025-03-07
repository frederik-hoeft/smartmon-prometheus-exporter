using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Domain.Writers;

public interface IOutputWriter : IPipelineHandler
{
    ValueTask WriteAsync(string metrics, CancellationToken cancellationToken);
}