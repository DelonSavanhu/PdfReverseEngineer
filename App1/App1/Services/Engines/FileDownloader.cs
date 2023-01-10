using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Engines
{
    public class FileDownloader
    {
        private int bufferSize = 4095;
        private HttpClient _client=new HttpClient();

        public async Task<String> DownloadFileAsync(string url,string path)
        {
            string filePath = "";
            string fileName = "";
            try
            {
                if (!Directory.Exists(path)) {
                    DirectoryInfo di = Directory.CreateDirectory(path);                  
                }
                
                
                // Step 1 : Get call
                var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
                }

                // Step 2 : Filename
                fileName = response.Content.Headers?.ContentDisposition?.FileName;
                fileName = fileName.Replace("\"", "");
                // Step 3 : Get total of data
                var totalData = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                //var canSendProgress = totalData != -1L && progress != null;

                // Step 4 : Get total of data
                filePath = Path.Combine(path, fileName);
                using (var client = new WebClient())
                {
                   client.DownloadFile(url, filePath);
                }
            }
            catch (Exception e)
            {
                // Manage the exception as you need here.
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return filePath;
        }
        private Stream OpenStream(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize);
        }
    }
}
