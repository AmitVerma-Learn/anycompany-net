using System;
using System.Collections.Generic;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Simple service locator to manage service dependencies
    /// </summary>
    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> _instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private ServiceLocator()
        {
            // Register default services
            RegisterService<IFileSystemService>(new FileSystemService());
            RegisterService<IConsoleService>(new ConsoleService());
            RegisterService<IConfigurationService>(new ConfigurationService());
        }

        public static ServiceLocator Instance => _instance.Value;

        public void RegisterService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }
            
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }
    }
}
