using Jab;
using SmartmonExporter.Data.Collectors;
using SmartmonExporter.Data.Collectors.DeviceCollectors;

namespace SmartmonExporter.Data;

[ServiceProviderModule]
[Singleton<IMetricsExporter, MetricsExporter>]
[Singleton<IMetricsCollector, VersionCollector>]
[Singleton<IMetricsCollector, SmartCollector>]
[Singleton<IDeviceMetricCollector, TimestampCollector>]
[Singleton<IDeviceMetricCollector, DeviceActiveCollector>]
[Singleton<IDeviceMetricCollector, DeviceInfoCollector>]
[Singleton<IDeviceMetricCollector, DeviceHealthCollector>]
[Singleton<IDeviceMetricCollector, DeviceAttributeCollector>]
public interface ISmartMetricsModule;