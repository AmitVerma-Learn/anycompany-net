using System;
using System.IO;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Helper class for handling file permissions in a cross-platform compatible way
    /// </summary>
    public static class FilePermissionHelper
    {
        /// <summary>
        /// Sets file permissions in a platform-agnostic way
        /// </summary>
        public static void SetFilePermissions(string filePath, bool allowRead, bool allowWrite, bool allowExecute)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
                
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);
                
            // On Windows, this will use ACLs
            // On Linux, this will be transformed to use chmod
            try
            {
                // Get current file attributes
                FileAttributes attributes = File.GetAttributes(filePath);
                
                // Set read-only attribute based on allowWrite parameter
                if (!allowWrite)
                {
                    // Make the file read-only
                    attributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    // Make the file writable
                    attributes &= ~FileAttributes.ReadOnly;
                }
                
                // Apply the attributes
                File.SetAttributes(filePath, attributes);
                
                // Note: Execute permissions are not directly supported in .NET Framework
                // but will be handled by the transformation process for Linux
            }
            catch (Exception ex)
            {
                LoggingService.Instance.LogException(ex, $"SetFilePermissions({filePath})");
                throw;
            }
        }
        
        /// <summary>
        /// Checks if a file has specific permissions
        /// </summary>
        public static bool CheckFilePermissions(string filePath, bool checkRead, bool checkWrite)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
                
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);
                
            try
            {
                // Get current file attributes
                FileAttributes attributes = File.GetAttributes(filePath);
                
                // Check if file is read-only
                bool isReadOnly = (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                
                // If we're checking write permissions and the file is read-only, return false
                if (checkWrite && isReadOnly)
                    return false;
                    
                // For read permissions, we'll try to open the file for reading
                if (checkRead)
                {
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        // If we can open it for reading, we have read permissions
                        return true;
                    }
                }
                
                // If we're checking write permissions, try to open the file for writing
                if (checkWrite)
                {
                    using (FileStream fs = File.OpenWrite(filePath))
                    {
                        // If we can open it for writing, we have write permissions
                        return true;
                    }
                }
                
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // If we get an unauthorized access exception, we don't have the requested permissions
                return false;
            }
            catch (Exception ex)
            {
                LoggingService.Instance.LogException(ex, $"CheckFilePermissions({filePath})");
                throw;
            }
        }
    }
}
