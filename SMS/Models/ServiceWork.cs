using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ServiceWork
    {
        public int ID { get; set; }
        public string WorkName { get; set; }
        public EBusinessUnit BU { get; set; }
        public bool DetailRequired { get; set; }
    }
}