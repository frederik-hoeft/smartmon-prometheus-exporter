namespace SmartmonExporter.Domain.Metrics;

internal readonly record struct PrometheusLabel(string Name, string Value);
