﻿using System.Text;

namespace SmartmonExporter.Data.Metrics;

public class PrometheusMetricBuilder(string metricNamespace, string metricName, PrometheusMetricTypeDescriptor metricType, StringBuilder builder, bool includeTimeStamp, bool includeHeader)
{
    private bool _wroteHeader;

    private void WriteHeader()
    {
        if (includeHeader && !_wroteHeader)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
            }
            builder.Append("# HELP ").Append(metricNamespace).Append('_').Append(metricName).Append(' ').AppendLine(metricType.Description);
            builder.Append("# TYPE ").Append(metricNamespace).Append('_').Append(metricName).Append(' ').AppendLine(metricType.Type.ToPrometheusString());
            _wroteHeader = true;
        }
    }

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
        WriteHeader();
        builder.Append(metricNamespace).Append('_').Append(metricName);
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
