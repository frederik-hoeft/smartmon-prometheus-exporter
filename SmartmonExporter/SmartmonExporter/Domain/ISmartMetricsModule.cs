using Jab;
using SmartmonExporter.Domain.Collectors;
using SmartmonExporter.Domain.Collectors.DeviceCollectors;
using SmartmonExporter.Domain.Collectors.DeviceCollectors.DeviceAttributeCollectors;
using SmartmonExporter.Domain.Interop;
using SmartmonExporter.Domain.Interop.Output;
using SmartmonExporter.Domain.Writers;

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
[Singleton<IDeviceAttributeCollector, AtaDeviceAttributeCollector>]
[Singleton<IDeviceAttributeCollector, NvmeDeviceAttributeCollector>]
[Singleton<IOutputWriter, FileOutputWriter>]
[Singleton<IOutputWriter, ConsoleOutputWriter>]
public interface ISmartMetricsModule
{
    internal static SmartctlJsonSerializerContext GetSmartctlJsonSerializerContext() => SmartctlJsonSerializerContext.Default;
}