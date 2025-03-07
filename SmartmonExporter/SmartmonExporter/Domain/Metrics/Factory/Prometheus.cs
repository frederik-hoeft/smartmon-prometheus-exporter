﻿using System.Numerics;

namespace SmartmonExporter.Domain.Metrics.Factory;

public static class Prometheus
{
    public static PrometheusLabel Label(string name, string value) => new(name, value);

    public static PrometheusLabel Label<T>(string name, T value) where T : INumber<T> => new(name, $"{value}");

    public static PrometheusMetricTypeDescriptor Untyped(string description) => new(PrometheusMetricType.Untyped, description);

    public static PrometheusMetricTypeDescriptor Counter(string description) => new(PrometheusMetricType.Counter, description);

    public static PrometheusMetricTypeDescriptor Gauge(string description) => new(PrometheusMetricType.Gauge, description);
}
