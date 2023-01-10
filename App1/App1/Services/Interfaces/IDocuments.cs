using App1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services.Interfaces
{
   public interface IDocuments
   {
        List<Document> GetDocuments(string path);
   }
}
