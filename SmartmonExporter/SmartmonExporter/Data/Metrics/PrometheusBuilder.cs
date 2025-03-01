using System.Text;

namespace SmartmonExporter.Data.Metrics;

public class PrometheusBuilder(string prometheusNamespace, int capacity)
{
    private readonly StringBuilder _builder = new(capacity);

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
        PrometheusMetricBuilder builder = new(prometheusNamespace, name, metricType: default, _builder, includeTimeStamp, includeHeader: false);
        builder.AddSample(value, labels);
        return this;
    }

    public PrometheusBuilder AddMetric(string name, PrometheusMetricTypeDescriptor type, bool includeTimeStamp, Action<PrometheusMetricBuilder> addSamples)
    {
        PrometheusMetricBuilder builder = new(prometheusNamespace, name, type, _builder, includeTimeStamp, includeHeader: true);
        addSamples(builder);
        return this;
    }

    public string Build() => _builder.ToString();
}
