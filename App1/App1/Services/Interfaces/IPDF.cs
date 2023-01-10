using App1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Interfaces
{
    public interface IPDF
    {
        Task<Pdf_Response> Merge(Pdf_Request request);
        Task<Pdf_Response> PDFToDocX(Pdf_Request request);
      //  Task<Pdf_Response> PDFToDocX2(Pdf_Request request);
        Task<Image_Response> ImageToPDF(Image_Request request);
        Task<Pdf_Response> PdfToImages(Pdf_Request request);
    }
}
