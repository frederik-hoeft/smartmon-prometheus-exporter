namespace SmartmonExporter.Data.Metrics.Factory;

public static class Prometheus
{
    public static PrometheusLabel Label(string name, string value) => new(name, value);

    public static PrometheusMetricTypeDescriptor Untyped(string description) => new(PrometheusMetricType.Untyped, description);

    public static PrometheusMetricTypeDescriptor Counter(string description) => new(PrometheusMetricType.Counter, description);

    public static PrometheusMetricTypeDescriptor Gauge(string description) => new(PrometheusMetricType.Gauge, description);
}
