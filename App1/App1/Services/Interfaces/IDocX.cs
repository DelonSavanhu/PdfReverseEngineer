using App1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Interfaces
{
    public interface IDocX
    {
       Task<DocX_Response> UploadDocx(DocX_Request request);
       
    }
}
