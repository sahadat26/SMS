using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Customer:EntryInfo
    {
        public int CustomerID { get; set; }
        public EBusinessUnit BusinessUnit { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(300)]
        public string Address { get; set; }
        [StringLength(300)]
        public string OfficePhone { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactPerson { get; set; }
        [Display(Name="Designation")]
        [StringLength(100)]
        public string JobTitle { get; set; }
        [Required]
        [StringLength(100)]
        public string Mobile { get; set; }
        [StringLength(100)]
        public string Email { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public Customer()
        {
            Contacts = new HashSet<Contact>();
        }

        public string SearchText
        {
            get
            {
                return Name + " " + JobTitle + " " + ContactPerson + " " + Mobile + " " + Email;
            }
        }

        public string DisplayText
        {
            get
            {
                return Name + " - " + Address;
            }
        }
    }
}