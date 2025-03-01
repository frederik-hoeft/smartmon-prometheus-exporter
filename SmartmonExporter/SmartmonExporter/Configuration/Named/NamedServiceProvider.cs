﻿using Jab;

namespace SmartmonExporter.Configuration.Named;

internal sealed class NamedServiceProvider(IServiceProvider serviceProvider) : INamedServiceProvider, IServiceProvider
{
    public T? GetService<T>(string name) where T : class
    {
        if (serviceProvider is INamedServiceProvider<T> namedServiceProvider)
        {
            return namedServiceProvider.GetService(name);
        }
        return null;
    }

    public object? GetService(Type serviceType) => serviceProvider.GetService(serviceType);

    public T? GetService<T>() where T : class => (T?)serviceProvider.GetService(typeof(T));
}
