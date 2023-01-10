using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Services
{
    public class CustomException:Exception
    {
        public CustomException(string message) : base(message) { }

    }
}
