﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class Property:EntryInfo
    {
        public int PropertyID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}