using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SMS
{
    public class CustomAuthorize:AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            
            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                string url = HttpContext.Current.Request.RawUrl;
                filterContext.Result = new RedirectResult("/Account/LogOn?ReturnUrl="+url);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Account/NoPermission");
            }
            
            
        }
    }
}