using App1.libs;
using App1.Models;
using App1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace App1.Services.Engines
{
    public class Documents : IDocuments
    {
        Misc misc = new Misc();
        public List<Document> GetDocuments(string path)
        {
            List<Document> myDocuments = new List<Document>();
            try
            {

                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] fileEntries = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

                //string[] fileEntries = Directory.GetFiles(path);
                // foreach (string fN in fileEntries)
                foreach (FileInfo oFileInfo in fileEntries)
                {
                    //Console.WriteLine(fN+"\n");
                    //FileInfo oFileInfo = new FileInfo(fN);
                    string mimeType = MimeTypes.GetMimeType(oFileInfo.Name);
                   


                    string size =Math.Round(ConvertBytesToMegabytes(oFileInfo.Length),2).ToString()+ " MB";
                    string date = oFileInfo.CreationTime.ToString("MMM dd. HH:mm");// +" | "+size;
                    myDocuments.Add(new Document {
                         name=oFileInfo.Name,
                         path=oFileInfo.FullName,
                         size=size,
                         type=mimeType,
                         img=misc.GetIcon(oFileInfo.FullName),
                         date=date
                    });
                }
            }
            catch (Exception  ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return myDocuments;
        }

        private double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

       

    }
}
