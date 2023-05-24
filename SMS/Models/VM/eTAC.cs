using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class eTAC
    {
        public int code { get; set; }
        public DateTime Expire { get; set; }

        public void Generate()
        {
            Random rnd = new Random();
            code = rnd.Next(1000, 9999);
            Expire = DateTime.Now.AddMinutes(3);
        }
    }
}