using App1.Models;
using App1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Engines
{
    public class DocX : BaseService,IDocX
    {
        public async Task<DocX_Response> UploadDocx(DocX_Request request)
        {
            DocX_Response response = new DocX_Response();
            try
            {
                string url = this.baseUrl + "/docx/topdf";
                Console.WriteLine("URL: " + url);
                string result = await this.uploadContent(url, "input", (String[])request.filename.ToArray(typeof(string)), null);
                return new DocX_Response { response=result };
            }
            catch (Exception ex) {
                return new DocX_Response { response = ex.Message };
            }
            
        }
    }
}
