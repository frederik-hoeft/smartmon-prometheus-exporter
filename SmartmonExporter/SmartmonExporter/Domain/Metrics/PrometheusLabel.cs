namespace SmartmonExporter.Domain.Metrics;

public readonly record struct PrometheusLabel(string Name, string Value);
