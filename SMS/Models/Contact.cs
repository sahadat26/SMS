using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Contact:EntryInfo
    {
        public int ContactID { get; set; }
        [StringLength(300)]
        public string Address { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; }
        [StringLength(100)]
        public string JobTitle { get; set; }
        [Required]
        [StringLength(100)]
        public string Mobile { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
    }
}