using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Models
{
    public class Config
    {
        public int id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public DateTime createdOn { get; set; }
    }
    public class Configs
    {
        public List<Config> configs { get; set; }
    }
}
