using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using System.ComponentModel.DataAnnotations;
namespace SMS
{
    public static class HtmlHelperExtension
    {
        private const string CheckedAttribute = " checked='checked'";

        public static MvcHtmlString CheckedIfMatch(object expected, object actual)
        {
            
            return new MvcHtmlString(Equals(expected, actual) ? CheckedAttribute : string.Empty);
        }
    }

    public static class Extensions
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }



    
}