using App1.Services;
using App1.Services.Interfaces;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using Xamarin.Forms;

namespace App1.libs
{
    public class Misc
    {
        public double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        public string GetIcon(string path)
        {
            string img = "file.png"; //default type
            FileInfo oFileInfo = new FileInfo(path);
            //string mimeType = MimeTypes.GetMimeType(oFileInfo.Name);
            if (oFileInfo.Extension.ToLower().Equals(".pdf"))
            {
                img = "pdf.png";
            } else if (oFileInfo.Extension.ToLower().Equals(".zip"))
            {
                img = "zip.png";
            }
            else if (oFileInfo.Extension.ToLower().Equals(".docx") || oFileInfo.Extension.ToLower().Equals(".doc") || oFileInfo.Extension.ToLower().Equals(".xls") || oFileInfo.Extension.ToLower().Equals(".xlxs") || oFileInfo.Extension.ToLower().Equals(".ppt") || oFileInfo.Extension.ToLower().Equals(".pptx"))
            {
                img = "microsoft.png";                         
            }
            else if (oFileInfo.Extension.ToLower().Equals(".png") || oFileInfo.Extension.ToLower().Equals(".jpeg") || oFileInfo.Extension.ToLower().Equals(".jpg"))
            {
                img = path;
            }
            return img;
        }

        public string GetPath()
        {
            try
            {

                string path = "";
                IPathService pathFinder = DependencyService.Get<IPathService>();
                //path = Path.Combine(pathFinder.PublicExternalFolder, "PdfUtility");
                path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PdfUtility");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public int GetAndroidVersion()
        {
            IPathService pathFinder = DependencyService.Get<IPathService>();
            return pathFinder.checkVersion();
        }
        public string SaveByteArrayToFileWithFileStream(byte[] data, string name)
        {
            try
            {
                string path = GetPath() + "/adhoc/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = path+ name;
                var stream = File.Create(filePath);
                stream.Write(data, 0, data.Length);
                stream.Dispose(); //release when done
                return filePath;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public void RemoveAdhoc()
        {
            string filePath = GetPath() + "/adhoc/";
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath,true); // we don't need it anymore
            }
        }
        public Dictionary<string,object> RemoveDuplicates(Dictionary<string,object> myDict)
        {
            HashSet<object> knownValues = new HashSet<object>();
            Dictionary<string, object> uniqueValues = new Dictionary<string, object>();

            foreach (var pair in myDict)
            {
                if (knownValues.Add(pair.Value))
                {
                    uniqueValues.Add(pair.Key, pair.Value);
                }
            }
            return uniqueValues;
        }
        public List<string> RemoveDuplicates(List<string> og)
        {
            return og.Distinct().ToList();
        }
        public string[] RemoveDuplicates(string[] s)
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }

        public bool AllowedTypes(string Extension,string [] AllowedTypes)
        {
            return AllowedTypes.Contains(Extension) == true ? true : false;
        }

        public void ShowNotification(string Title, string Message,bool ClearFromHistory,string Icon)
        {

            var options=new NotificationOptions()
            {
                Title = Title,
                Description = Message,
                IsClickable = ClearFromHistory,
                WindowsOptions = new WindowsOptions() { LogoUri = Icon },
                ClearFromHistory = true,
                AllowTapInNotificationCenter = false,
                AndroidOptions = new AndroidOptions()
                {
                    HexColor = "#F99D1C",
                    ForceOpenAppOnNotificationTap = true
                }
            };

            var notificator = DependencyService.Get<IToastNotificator>();

            notificator.Notify((INotificationResult result) =>
            {
                System.Diagnostics.Debug.WriteLine("Notification [" + result.Id + "] Result Action: " + result.Action);
            }, options);
        }
        void ShowToast(INotificationOptions options)
        {
            //("notifier initilized");
            
        }

        public bool CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = File.Create(destPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
            if (File.Exists(destPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<IPdfImage> PdfGetImages(string path, int pageNo)
        {
            try {
                using (PdfDocument document = PdfDocument.Open(path))
                 {
                UglyToad.PdfPig.Content.Page page1 = document.GetPage(pageNo);
                return page1.GetImages();
                 }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            }


    }
}
