using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Models
{
    public class Pdf_Response : BaseModel_Response
    {
    }
    public class Pdf_Request : BaseModel_Request
    {

        public string Extension { get; set; }

    }
}
