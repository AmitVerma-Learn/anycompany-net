using System;
using System.Text;
using System.Threading;

namespace AnyCompanyBankingProduct.Services
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public class LoggingService
    {
        private static readonly Lazy<LoggingService> _instance = new Lazy<LoggingService>(() => new LoggingService());
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();
        private LogLevel _minimumLogLevel;
        private readonly IFileSystemService _fileSystem;
        private readonly IConsoleService _console;

        private LoggingService()
        {
            // Use service locator to get dependencies
            try
            {
                _fileSystem = ServiceLocator.Instance.GetService<IFileSystemService>();
                _console = ServiceLocator.Instance.GetService<IConsoleService>();
            }
            catch
            {
                // Fallback to direct instantiation if service locator is not initialized yet
                _fileSystem = new FileSystemService();
                _console = new ConsoleService();
            }

            string logDirectory = _fileSystem.GetLogDirectory();
            
            if (!_fileSystem.DirectoryExists(logDirectory))
            {
                _fileSystem.CreateDirectory(logDirectory);
            }
            
            _logFilePath = _fileSystem.CombinePaths(logDirectory, $"app_{DateTime.Now:yyyyMMdd}.log");
            _minimumLogLevel = LogLevel.Info; // Default log level
        }

        public static LoggingService Instance => _instance.Value;

        public void SetLogLevel(LogLevel level)
        {
            _minimumLogLevel = level;
        }

        public void Log(LogLevel level, string message)
        {
            if (level < _minimumLogLevel)
                return;

            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{Thread.CurrentThread.ManagedThreadId}] {message}";
                
                lock (_lockObject)
                {
                    _fileSystem.WriteTextToFile(_logFilePath, logEntry + Environment.NewLine, true);
                }
                
                // If it's an error or critical, also write to console if in debug mode
                #if DEBUG
                if (level >= LogLevel.Error)
                {
                    _console.WriteLineWithColor($"[{level}] {message}", level == LogLevel.Critical ? ConsoleColor.Red : ConsoleColor.Yellow);
                }
                #endif
            }
            catch (Exception ex)
            {
                // Write to console in case of logging failure
                #if DEBUG
                _console.WriteLineWithColor($"Logging error: {ex.Message}", ConsoleColor.Red);
                #endif
                
                // Try to write to a fallback location
                try
                {
                    string desktopPath = Environment.GetEnvironmentVariable("USERPROFILE") ?? 
                                        Environment.GetEnvironmentVariable("HOME") ?? 
                                        _fileSystem.GetApplicationDataPath();
                    string fallbackPath = _fileSystem.CombinePaths(desktopPath, "logging_error.txt");
                    _fileSystem.WriteTextToFile(fallbackPath, 
                        $"{DateTime.Now}: Failed to write log: {ex.Message}{Environment.NewLine}", true);
                }
                catch
                {
                    // Last resort - truly fail silently
                }
            }
        }

        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message) => Log(LogLevel.Error, message);
        public void LogCritical(string message) => Log(LogLevel.Critical, message);

        public void LogException(Exception ex, string context = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(!string.IsNullOrEmpty(context) ? $"Exception in {context}:" : "Exception:");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"Message: {ex.InnerException.Message}");
                sb.AppendLine($"StackTrace: {ex.InnerException.StackTrace}");
            }
            
            Log(LogLevel.Error, sb.ToString());
        }
    }
}
