using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Services.Interfaces
{
    public interface IDownloader
    {
        void DownloadFile(string url, string folder);
        event EventHandler OnFileDownloaded;
    }
}
