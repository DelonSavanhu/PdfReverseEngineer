using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        PdfMerger,
        Documents,
        PdfToDox,
        DocxToPdf,
        ImagesToPdf,
        PdfToImages,
        PdfToHtml,
        HtmlToPdf,
        PdfToDocx,
        ScanToPdf,
        ImageEdit
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
        public string Icon { get; set; }
    }
}
