using SmartmonExporter.Pipelines;

namespace SmartmonExporter.Domain.Writers;

internal interface IOutputWriter : IPipelineHandler
{
    ValueTask WriteAsync(string metrics, CancellationToken cancellationToken);
}