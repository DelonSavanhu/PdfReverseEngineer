using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App1.Services
{
    public interface IPathService
    {
        string InternalFolder { get; }
        string PublicExternalFolder { get; }
        string PrivateExternalFolder { get; }
        int checkVersion();
    }
}