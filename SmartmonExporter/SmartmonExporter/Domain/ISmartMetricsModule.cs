using Jab;
using SmartmonExporter.Data.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Collectors;
using SmartmonExporter.Domain.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output;

namespace SmartmonExporter.Domain;

[ServiceProviderModule]
[Singleton<SmartctlJsonSerializerContext>(Factory = nameof(GetSmartctlJsonSerializerContext))]
[Singleton<IChildProcessRunner, DefaultChildProcessRunner>]
[Singleton<ISmartctlRunner, SmartctlRunner>]
[Singleton<IMetricsExporter, MetricsExporter>]
[Singleton<IMetricsCollector, VersionCollector>]
[Singleton<IMetricsCollector, SmartCollector>]
[Singleton<IDeviceMetricCollector, TimestampCollector>]
[Singleton<IDeviceMetricCollector, DeviceActiveCollector>]
[Singleton<IDeviceMetricCollector, DeviceInfoCollector>]
[Singleton<IDeviceMetricCollector, DeviceHealthCollector>]
[Singleton<IDeviceMetricCollector, DeviceAttributeCollector>]
public interface ISmartMetricsModule
{
    internal static SmartctlJsonSerializerContext GetSmartctlJsonSerializerContext() => SmartctlJsonSerializerContext.Default;
}