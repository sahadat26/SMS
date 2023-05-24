using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SPServiceDetail
    {
        
        [Key,Column(Order=0)]
        [ForeignKey("SPService")]
        public int ServiceID { get; set; }
        public virtual SPService SPService { get; set; }
        [Key,Column(Order=1)]
        public int Sl { get; set; }
        [ForeignKey("ServiceWork")]
        public int WorkID { get; set; }
        public virtual ServiceWork ServiceWork { get; set; }
        public string ServiceDetail { get; set; }
        public bool IsDone { get; set; }
    }
}