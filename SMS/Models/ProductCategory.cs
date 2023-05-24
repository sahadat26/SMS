using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class ProductCategory:EntryInfo
    {
        public int ProductCategoryID { get; set; }
        [Required]
        [Display(Name="Category Name")]
        public string CategoryName { get; set; }
        [Display(Name="Parent Category Name")]
        public int? ParentId { get; set; }

        public ProductCategory ParentCategory
        {
            get
            {
                if (ParentId == null)
                    return new ProductCategory();
                SMSContext db = new SMSContext();
                return db.ProductCategories.Find(ParentId);
            }
        }
    }
}