using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public abstract class EntryInfo
    {

        public int UserID { get; set;}

        public DateTime EntryDateTime { get; set; }
        [StringLength(100)]
        public string HostName { get; set; }

    }

}