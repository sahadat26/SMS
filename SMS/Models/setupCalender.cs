using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class setupCalender
    {
        [Key]
        public int Id { get; set; }

        public int year { get; set; }
        public int duration { get; set; }
        public int month { get; set; }

       
        public DateTime startDate { get; set; }
       
        public DateTime endDate { get; set; }
        public int? EntryBy { get; set; }

    }
}