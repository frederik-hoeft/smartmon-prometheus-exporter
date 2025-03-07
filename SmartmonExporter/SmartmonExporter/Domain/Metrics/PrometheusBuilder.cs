using System.Text;

namespace SmartmonExporter.Domain.Metrics;

public class PrometheusBuilder(string prometheusNamespace, int capacity = -1)
{
    private readonly StringBuilder _builder = capacity == -1 ? new() : new(capacity);
    private readonly Dictionary<string, PrometheusMetric> _metrics = [];

    public PrometheusBuilder AddSimpleMetric(string name, bool value, bool includeTimeStamp, params PrometheusLabel[] labels) =>
        AddSimpleMetric(name, value ? "1" : "0", includeTimeStamp, labels);

    public PrometheusBuilder AddSimpleMetric(string name, float value, bool includeTimeStamp, params PrometheusLabel[] labels) =>
        AddSimpleMetric(name, (double)value, includeTimeStamp, labels);

    public PrometheusBuilder AddSimpleMetric(string name, long value, bool includeTimeStamp, params PrometheusLabel[] labels) =>
        AddSimpleMetric(name, value.ToString(), includeTimeStamp, labels);

    public PrometheusBuilder AddSimpleMetric(string name, int value, bool includeTimeStamp, params PrometheusLabel[] labels) =>
        AddSimpleMetric(name, value.ToString(), includeTimeStamp, labels);

    public PrometheusBuilder AddSimpleMetric(string name, double value, bool includeTimeStamp, params PrometheusLabel[] labels) =>
        AddSimpleMetric(name, value.ToPrometheusString(), includeTimeStamp, labels);

    private PrometheusBuilder AddSimpleMetric(string name, string value, bool includeTimeStamp, params PrometheusLabel[] labels)
    {
        if (!_metrics.TryGetValue(name, out PrometheusMetric? metric))
        {
            metric = new PrometheusMetric(prometheusNamespace, name);
            _metrics[name] = metric;
        }
        PrometheusMetricBuilder builder = metric.CreateBuilder(null, includeTimeStamp);
        builder.AddSample(value, labels);
        return this;
    }

    public PrometheusBuilder AddMetric(string name, PrometheusMetricTypeDescriptor type, bool includeTimeStamp, Action<PrometheusMetricBuilder> addSamples)
    {
        if (!_metrics.TryGetValue(name, out PrometheusMetric? metric))
        {
            metric = new PrometheusMetric(prometheusNamespace, name, type);
            _metrics[name] = metric;
        }
        PrometheusMetricBuilder builder = metric.CreateBuilder(type, includeTimeStamp);
        addSamples(builder);
        return this;
    }

    public string Build()
    {
        foreach (PrometheusMetric metric in _metrics.Values.OrderBy(metric => metric.Name))
        {
            metric.WriteTo(_builder);
        }
        return _builder.ToString();
    }
}

public class PrometheusMetric(string prometheusNamespace, string name, PrometheusMetricTypeDescriptor? type = default)
{
    internal readonly StringBuilder _builder = new();

    public string Namespace { get; } = prometheusNamespace;

    public string Name { get; } = name;

    public PrometheusMetricTypeDescriptor? Type { get; private set; } = type;

    public PrometheusMetricBuilder CreateBuilder(PrometheusMetricTypeDescriptor? type, bool includeTimeStamp)
    {
        if (!Type.HasValue)
        {
            Type = type;
        }
        else if (Type != type)
        {
            throw new InvalidOperationException($"Metric type mismatch for {Namespace}_{Name}");
        }
        return new PrometheusMetricBuilder(this, includeTimeStamp);
    }

    public void WriteTo(StringBuilder builder)
    {
        if (Type.HasValue)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
            }
            builder.Append("# HELP ").Append(Namespace).Append('_').Append(Name).Append(' ').AppendLine(Type.Value.Description);
            builder.Append("# TYPE ").Append(Namespace).Append('_').Append(Name).Append(' ').AppendLine(Type.Value.Type.ToPrometheusString());
        }
        builder.Append(_builder);
    }
}