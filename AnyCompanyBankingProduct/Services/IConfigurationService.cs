using System;
using System.Configuration;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Platform-agnostic interface for configuration operations
    /// </summary>
    public interface IConfigurationService
    {
        string GetConnectionString(string name);
        string GetAppSetting(string key);
        string GetEnvironmentVariable(string name);
    }

    /// <summary>
    /// Implementation of IConfigurationService for .NET Framework
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly LoggingService _logger;

        public ConfigurationService()
        {
            _logger = LoggingService.Instance;
        }

        public string GetConnectionString(string name)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning($"Connection string '{name}' not found or empty");
                }
                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"GetConnectionString({name})");
                return null;
            }
        }

        public string GetAppSetting(string key)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value))
                {
                    _logger.LogWarning($"App setting '{key}' not found or empty");
                }
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"GetAppSetting({key})");
                return null;
            }
        }

        public string GetEnvironmentVariable(string name)
        {
            try
            {
                return Environment.GetEnvironmentVariable(name);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"GetEnvironmentVariable({name})");
                return null;
            }
        }
    }
}
