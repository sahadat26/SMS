using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
namespace SMS.Controllers
{
    public class PermissionController : Controller
    {
        //
        // GET: /Permission/

        SMSContext db = new SMSContext();
        [CustomAuthorize(Roles = "Admin,User")]
        public ActionResult SetPermission(int UserID)
        {
            Permission _Permission;
            List<Permission> lstPermission = new List<Permission>();
            SMS.Models.User _User = db.Users.Find(UserID);
            foreach (var _role in db.Roles.Where(r=>r.Name!="Admin").ToList())
            {
                _Permission = new Permission();
                _Permission.RoleID = _role.RoleID;
                _Permission.Roles = _role;
                if (_User.Roles.Any(r => r.RoleID == _role.RoleID))
                {
                    _Permission.Status = true;
                }
                else
                {
                    _Permission.Status = false;
                }
                lstPermission.Add(_Permission);
            }
            ViewBag.UserID = UserID;
            return View(lstPermission);
        }

        [CustomAuthorize(Roles = "Admin,User")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetPermission()
        {
            try
            {
                if (Request["UserID"] == null)
                {
                    ViewBag.Error = "No User Selected!";
                }
                else
                {
                    int UserID = Convert.ToInt32(Request["UserID"]);
                    SMS.Models.User _User = db.Users.Find(UserID);
                    foreach (var _role in _User.Roles.ToList())
                    {
                        _User.Roles.Remove(_role);
                    }
                    foreach (var _role in db.Roles.ToList())
                    {
                        if (Request["Permission_"+_role.RoleID]=="true,false")
                        {
                            _User.Roles.Add(_role);
                        }
                        
                    }
                   
                    db.SaveChanges();
                    return RedirectToAction("Index","Account");
                }
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
