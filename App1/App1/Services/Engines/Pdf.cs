using App1.Models;
using App1.Services.Interfaces;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Engines
{
    public class Pdf : BaseService,IPDF
    {
        public async Task<Image_Response> ImageToPDF(Image_Request request)
        {
            Image_Response response = new Image_Response();
            try
            {
                string url = this.baseUrl + "/images/post";
                string result = await this.uploadContent(url, "file2", (String[])request.filename.ToArray(typeof(string)), null);
                return new Image_Response { response = result };
            }
            catch (Exception ex)
            {
                return new Image_Response { response = ex.Message };
            }

        }

        public async Task<Pdf_Response> Merge(Pdf_Request request)
        {
            Pdf_Response response = new Pdf_Response();
            try
            {
                string url = this.baseUrl + "/pdf/merge";
                string result = await this.uploadContent(url, "file2", (String[])request.filename.ToArray(typeof(string)), null);
                return new Pdf_Response { response = result };
            }
            catch (Exception ex)
            {
                return new Pdf_Response { response = ex.Message };
            }

        }

        public async Task<Pdf_Response> PDFToDocX(Pdf_Request request)
        {

            Pdf_Response response = new Pdf_Response();
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("format", request.Extension);
                string url = this.baseUrl + "/pdf/todocx";
                string result = await this.uploadContentWithFields(url, "file2", (String[])request.filename.ToArray(typeof(string)),null, dict);
                return new Pdf_Response { response = result };
            }
            catch (Exception ex)
            {
                return new Pdf_Response { response = ex.Message };
            }
        }


/*        public async Task<Pdf_Response> PDFToDocX2(Pdf_Request request)
        {

            Pdf_Response response = new Pdf_Response();
            try
            {
                //Creates a new Word document.
                WordDocument m_wordDocument = new WordDocument();

                //Adds new section to the document.
                IWSection section = m_wordDocument.AddSection();

                //Sets the page margins to zero.
                section.PageSetup.Margins.All = 0;

                //Adds new paragraph to the section.
                IWParagraph firstParagraph = section.AddParagraph();

                SizeF defaultPageSize = new SizeF(m_wordDocument.LastSection.PageSetup.PageSize.Width, m_wordDocument.LastSection.PageSetup.PageSize.Height);

                //Loads the PDF document from the given file path.
                foreach (var fn in request.filename) {
                    using (PdfLoadedDocument m_loadedDocument = new PdfLoadedDocument(fn))
                    {
                        for (int i = 0; i < m_loadedDocument.Pages.Count; i++)
                        {
                           
                            //Exports the PDF document page as image with the given size.
                            using (Image image = m_loadedDocument.ExportAsImage(i, defaultPageSize, false))
                            {
                                //Adds image to the paragraph
                                IWPicture picture = firstParagraph.AppendPicture(image);

                                //Sets width and height for the image.
                                picture.Width = image.Width;
                                picture.Height = image.Height;
                            }
                        }
                    };

                    //Saves the Word document
                    m_wordDocument.Save("Save.docx");
                }
            }
            catch (Exception ex)
            {
                return new Pdf_Response { response = ex.Message };
            }
        }*/



        public async Task<Pdf_Response> PdfToImages(Pdf_Request request)
        {
            Pdf_Response response = new Pdf_Response();
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("extension", request.Extension);
                string url = this.baseUrl + "/images/pdftoimages";
                string result = await this.uploadContentWithFields(url, "file2", (String[])request.filename.ToArray(typeof(string)),null,dict);
                return new Pdf_Response { response = result };
            }
            catch (Exception ex)
            {
                return new Pdf_Response { response = ex.Message };
            }
        }
    }
}
