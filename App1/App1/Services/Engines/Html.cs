using App1.Models;
using App1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Engines
{
    public class Html : BaseService,IHtml
    {
        public async Task<Html_Response> HtmlToPDF(Html_Request request)
        {
            Html_Response response = new Html_Response();
            try
            {
                string url = this.baseUrl + "/html/post";
                string result = await this.uploadContent(url, "input", (String[])request.filename.ToArray(typeof(string)), null);
                return new Html_Response { response = result };
            }
            catch (Exception ex)
            {
                return new Html_Response { response = ex.Message };
            }

        }
    }
}
