using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace AnyCompanyBankingProduct.Services
{
    /// <summary>
    /// Cross-platform compatible HTTP client helper
    /// </summary>
    public class HttpClientHelper
    {
        private readonly LoggingService _logger;
        
        public HttpClientHelper()
        {
            _logger = LoggingService.Instance;
        }
        
        /// <summary>
        /// Performs a GET request in a cross-platform compatible way
        /// </summary>
        public string GetString(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
                
            _logger.LogInfo($"Making GET request to {url}");
            
            try
            {
                // Create a request for the URL
                WebRequest request = WebRequest.Create(url);
                
                // Get the response
                using (WebResponse response = request.GetResponse())
                {
                    // Get the stream containing content returned by the server
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content
                            string responseFromServer = reader.ReadToEnd();
                            _logger.LogInfo($"Received {responseFromServer.Length} bytes from {url}");
                            return responseFromServer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"GetString({url})");
                throw;
            }
        }
        
        /// <summary>
        /// Performs a POST request in a cross-platform compatible way
        /// </summary>
        public string PostString(string url, string data, string contentType = "application/json")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
                
            _logger.LogInfo($"Making POST request to {url}");
            
            try
            {
                // Create a request for the URL
                WebRequest request = WebRequest.Create(url);
                
                // Set the Method property to POST
                request.Method = "POST";
                
                // Set the ContentType property
                request.ContentType = contentType;
                
                // Convert the data to a byte array
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                
                // Set the ContentLength property
                request.ContentLength = byteArray.Length;
                
                // Get the request stream
                using (Stream dataStream = request.GetRequestStream())
                {
                    // Write the data to the request stream
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                
                // Get the response
                using (WebResponse response = request.GetResponse())
                {
                    // Get the stream containing content returned by the server
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content
                            string responseFromServer = reader.ReadToEnd();
                            _logger.LogInfo($"Received {responseFromServer.Length} bytes from {url}");
                            return responseFromServer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"PostString({url})");
                throw;
            }
        }
    }
}
