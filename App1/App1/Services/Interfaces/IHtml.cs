using App1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Interfaces
{
    public interface IHtml
    {
        Task<Html_Response> HtmlToPDF(Html_Request request); 
    }
}
