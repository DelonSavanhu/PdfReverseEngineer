using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace App1.Models
{
    public class BaseModel_Request
    {
        public ArrayList filename { get; set; } 
    }
    public class BaseModel_Response
    {
        public string response = "";
        //public HttpStatus status = HttpStatus.OK;
        public string Message = "";
        public string url = "";
    }
}
