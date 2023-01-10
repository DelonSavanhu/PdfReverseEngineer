using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Services.Engines
{
    public class DownloadEventArg : EventArgs
    {
        public bool FileSaved = false;
        public DownloadEventArg(bool fileSaved)
        {
            FileSaved = fileSaved;
        }
    }
}
