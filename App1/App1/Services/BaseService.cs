using App1.libs;
using App1.Models;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public class BaseService:IDisposable
    {
        //public string baseUrl = "http://172.105.181.118:8080";
        //public string baseUrl = "https://media.afrodeb.com:8081";
        public string baseUrl = "http://192.168.43.128:8081";
        Dictionary<string, object> dict = new Dictionary<string, object>();
        Misc misc = new Misc();
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
              return;

            if (disposing)
            {
                handle.Dispose();
                dict.Clear();
            }

            disposed = true;
        }
        public async Task<String> UploadAsync(string url, string serverName, string[] filename, Stream fileStream, byte[] fileBytes = null)
        {
            try {
                string[] newArray = misc.RemoveDuplicates(filename);
                string r = "";
                var client = new HttpClient();
                var formData = new MultipartFormDataContent();
                    foreach (string fn in newArray) {
                        using (FileStream fs = File.OpenRead(fn))
                        {
                            string []splitFile = fn.Split('/');
                            string fName = splitFile[splitFile.Length - 1];

                            HttpContent fileStreamContent = new StreamContent(fs);
                            formData.Add(fileStreamContent, serverName, fName);
                            fs.Position = 0;
                            fileStreamContent.Dispose();
                            fs.Dispose();
                        }
                    }
                
                    var response = await client.PostAsync(url, formData);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        return "falied to reach server";
                    }
                    filename = null;
                    r = await response.Content.ReadAsStringAsync();
                   return r;
               // }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
            }

        public async Task<String> uploadContent(string url, string serverName, string[] filename, Stream fileStream, byte[] fileBytes = null) {
            try
            {
                string[] newArray = misc.RemoveDuplicates(filename);
                var client = new HttpClient();
                string r = "";
               
                foreach (string fn in newArray) {
                    string[] splitFile = fn.Split('/');
                    string fName = splitFile[splitFile.Length - 1];
                    string mimeType = MimeTypes.GetMimeType(fName);
                    dict.Add(fName, new FormFile() { Name = fName, ContentType = mimeType, FilePath = fn,ServerName=serverName });
                }
                var serverDict = misc.RemoveDuplicates(dict);
                r = RequestHelper.PostMultipart(url, serverDict);
                filename = null;
                return r;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return ex.Message;
            }
        }
        public async Task<String> uploadContentWithFields(string url, string serverName, string[] filename, Stream fileStream, Dictionary<string, string> fields)
        {
            string[] newArray = misc.RemoveDuplicates(filename);
            //fields = misc.RemoveDuplicates(fields);
            try
            {
                var client = new HttpClient();
                string r = "";
                foreach (string fn in newArray)
                {
                    string[] splitFile = fn.Split('/');
                    string fName = splitFile[splitFile.Length - 1];
                    string mimeType = MimeTypes.GetMimeType(fName);
                    dict.Add(fName, new FormFile() { Name = fName, ContentType = mimeType, FilePath = fn, ServerName = serverName });
                }
                if (fields != null)
                {
                    foreach (KeyValuePair<string, string> entry in fields)
                    {
                        dict.Add(entry.Key,entry.Value);
                    
                    }
                }
                r = RequestHelper.PostMultipart(url, dict);
                dict.Clear();                
                return r;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.Message;
            }
        }

        public void unzipMyFiles(string path,string file)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(file, path);
        }
        public async Task<bool> UnzipFileAsync(string zipFilePath, string unzipFolderPath)
        {
            try
            {
                var entry = new ZipEntry(Path.GetFileNameWithoutExtension(zipFilePath));
                var fileStreamIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                var zipInStream = new ZipInputStream(fileStreamIn);
                entry = zipInStream.GetNextEntry();
                while (entry != null && entry.CanDecompress)
                {
                    var outputFile = unzipFolderPath + @"/" + entry.Name;
                    var outputDirectory = Path.GetDirectoryName(outputFile);
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    if (entry.IsFile)
                    {
                        var fileStreamOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        int size;
                        byte[] buffer = new byte[4096];
                        do
                        {
                            size = await zipInStream.ReadAsync(buffer, 0, buffer.Length);
                            await fileStreamOut.WriteAsync(buffer, 0, size);
                        } while (size > 0);
                        fileStreamOut.Close();
                    }

                    entry = zipInStream.GetNextEntry();
                }
                zipInStream.Close();
                fileStreamIn.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<List<Config>> GetConfigs()
        {            
            List<Config> results = new List<Config>();
            //results.configs = new List<Config>();
            try
            {
                string url = baseUrl+"/config/index";
                var client = new HttpClient();
                var res = await client.GetAsync(url);
                var result =await res.Content.ReadAsStringAsync();               
                results = JsonConvert.DeserializeObject<List<Config>>(result);
                return results;
            }
            catch (Exception ex)
            {
                //throw ex;
                var config = new Config { createdOn = DateTime.Now, id = 0, name = "ads", value = "1" };
                var r= new List<Config>();
                r.Add(config);
                return r;      
            }
        }


    }
}
