namespace SmartmonExporter.Configuration.Model;

public class AppSettings : IRequireValidation
{
    public bool DebugMode { get; init; }

    public string SmartctlPath { get; init; } = "/usr/sbin/smartctl";

    public required string OutputPath { get; init; }

    public void AssertIsValid() => _ = this is
    {
        OutputPath.Length: > 0,
    } ? true : throw new InvalidOperationException("invalid configuration");
}