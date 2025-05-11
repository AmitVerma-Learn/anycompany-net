using System;
using System.IO;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Platform-agnostic interface for file system operations
    /// </summary>
    public interface IFileSystemService
    {
        string GetApplicationDataPath();
        string GetLogDirectory();
        string CombinePaths(params string[] paths);
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void WriteTextToFile(string path, string content, bool append = false);
        string ReadTextFromFile(string path);
    }

    /// <summary>
    /// Implementation of IFileSystemService for .NET Framework
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        public string GetApplicationDataPath()
        {
            // Platform-agnostic way to get application data path
            string appDataPath = Environment.GetEnvironmentVariable("APPDATA") ?? 
                                Environment.GetEnvironmentVariable("HOME") ?? 
                                Directory.GetCurrentDirectory();
            return appDataPath;
        }

        public string GetLogDirectory()
        {
            return CombinePaths(GetApplicationDataPath(), "AnyCompanyBankingProduct", "Logs");
        }

        public string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void WriteTextToFile(string path, string content, bool append = false)
        {
            if (append)
            {
                File.AppendAllText(path, content);
            }
            else
            {
                File.WriteAllText(path, content);
            }
        }

        public string ReadTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
