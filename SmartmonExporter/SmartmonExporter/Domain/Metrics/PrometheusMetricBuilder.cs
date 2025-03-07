using System.Text;

namespace SmartmonExporter.Domain.Metrics;

public class PrometheusMetricBuilder(PrometheusMetric metric, bool includeTimeStamp)
{
    public PrometheusMetricBuilder AddSample(bool value, params ReadOnlySpan<PrometheusLabel> labels) =>
        AddSample(value ? "1" : "0", labels);

    public PrometheusMetricBuilder AddSample(float value, params ReadOnlySpan<PrometheusLabel> labels) =>
        AddSample((double)value, labels);

    public PrometheusMetricBuilder AddSample(long value, params ReadOnlySpan<PrometheusLabel> labels) =>
        AddSample(value.ToString(), labels);

    public PrometheusMetricBuilder AddSample(int value, params ReadOnlySpan<PrometheusLabel> labels) =>
        AddSample(value.ToString(), labels);

    public PrometheusMetricBuilder AddSample(double value, params ReadOnlySpan<PrometheusLabel> labels) =>
        AddSample(value.ToPrometheusString(), labels);

    internal PrometheusMetricBuilder AddSample(string value, params ReadOnlySpan<PrometheusLabel> labels)
    {
        StringBuilder builder = metric._builder;
        builder.Append(metric.Namespace).Append('_').Append(metric.Name);
        if (labels.Length > 0)
        {
            builder.Append('{');
            for (int i = 0; i < labels.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(labels[i].Name).Append("=\"").Append(labels[i].Value).Append('"');
            }
            builder.Append('}');
        }
        builder.Append(' ').Append(value);
        if (includeTimeStamp)
        {
            builder.Append(' ').Append(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
        builder.AppendLine();
        return this;
    }
}
