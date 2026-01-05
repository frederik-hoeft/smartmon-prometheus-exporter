using SmartmonExporter.Domain.Interop.Output.Model;

namespace SmartmonExporter.Domain.Interop;

internal interface ISmartctlRunner
{
    Task<TResult> RunAsync<TResult>(string[] arguments, string? device, CancellationToken cancellationToken) where TResult : RootBase;
}