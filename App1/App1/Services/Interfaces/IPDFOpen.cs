using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Interfaces
{
    public interface IPDFOpen
    { 
         Task SaveAndView(string fileName, String contentType,PDFOpenContext context);
        void WordPdf(string filename, string path);
    }
    /// <summary>
    /// Where should the PDF file open. In the app or out of the app.
    /// </summary>
    public enum PDFOpenContext
    {
        InApp,
        ChooseApp
    }
}
