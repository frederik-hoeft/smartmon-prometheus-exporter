namespace SmartmonExporter.Configuration.Model;

public class AppSettings : IRequireValidation
{
    public bool DebugMode { get; set; }

    public string SmartctlPath { get; init; } = "/usr/sbin/smartctl";

    public required string OutputPath { get; set; }

    public string? PrometheusNamespace { get; init; }

    public bool WriteToConsole { get; set; }

    public string[]? Devices { get; init; }

    public void AssertIsValid() => _ = this is
    {
        OutputPath.Length: > 0,
    } ? true : throw new InvalidOperationException("invalid configuration");
}