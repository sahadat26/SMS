using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SMS.Models;
using System.Data.Entity.Validation;
using System.Data.Entity;
using SMS.Repositories;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn
        private AuthenticationMembershipProvider MembershipService { get; set; }
        private AuthenticationRoleProvider AuthorizationService { get; set; }
        private UserRepository user { get; set; }
        private EmailRepository Email { get; set; }
        
        SMSContext db = new SMSContext();

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null)
                MembershipService = new AuthenticationMembershipProvider();
            if (AuthorizationService == null)
                AuthorizationService = new AuthenticationRoleProvider();
            if (user == null)
                user = new UserRepository();
            if (Email == null)
                Email = new EmailRepository();
            base.Initialize(requestContext);
        }

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult Index(string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }

            if (q.Length > 0)
            {
                var SearchResult = user.GetAll(UI).Where(e => 
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.UserName);
                ViewBag.SearchText = q;
                
                return View(SearchResult.Where(e=>e.UserName!="admin"));
            }
            
            return View(user.GetAll(UI).Where(e=>e.UserName!="admin").OrderByDescending(e => e.UserName));
        }


        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult NotExist()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Error = TempData["Info"];
            }
            return View(user.GetAll(UI).Where(m => m.Exist == false).OrderBy(i => i.UserName));
        }
        public ActionResult LogOn(string ReturnUrl="")
        {
            string ReturnFullUrl = string.Empty;
            if(TempData["Error"]!=null)
            {
                ViewBag.Error = TempData["Error"];
            }

            if(ReturnUrl!="")
            {
                
                string ItemNo = string.Empty;
                try
                {
                    ItemNo = Request.QueryString["ItemNo"].ToString();
                }
                catch (Exception ex)
                {

                }

                ReturnFullUrl = ReturnUrl + "&ItemNo=" + ItemNo;
                TempData["ReturnFullUrl"] = ReturnFullUrl;
            }


            return View(new LogOnModel() { ReturnUrl = ReturnFullUrl });
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model)
        {
            try
            {   
                if (MembershipService.AuthenticateUser(model))
                {
                    string cookieval = model.Module + "|"+ model.BusinessUnit + "|" + model.UserName;
                    FormsAuthentication.SetAuthCookie(cookieval, model.RememberMe);
                    if(model.Module==EModule.ASE_Commission)
                    {
                        return RedirectToAction("IndexCom", "Dashboard");
                    }
                    if(model.ReturnUrl!="")
                    {
                       
                       // return Redirect("~" + model.ReturnUrl);
                        if (model.ReturnUrl != null)
                        {
                            if (model.ReturnUrl!="/")
                            {
                                return Redirect("~" + TempData["ReturnFullUrl"].ToString());
                            }
                           
                        }
                        
                       
                       
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["Error"] = "User Name or Password or Business Unit not correct!!";
                }
            
            }
            catch(Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Remove("User");
            return RedirectToAction("LogOn", "Account");
        }

        //
        // GET: /Account/Register

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult Register()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            ViewBag.Departments = user.GetDepartments(UI);
            
            ViewBag.Supervisors = user.GetSelectList(UI);
            ViewBag.Designations = user.GetDesignations(UI.BusinessUnit);
            return View(new User { Exist = true,BusinessUnit=UI.BusinessUnit });
        }

        //
        // POST: /Account/Register

        [CustomAuthorize(Roles = "Admin, User")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(User obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                
                obj.CreateDate = DateTime.Now;
                user.Add(obj);
                user.Save();
                return RedirectToAction("SetPermission", "Permission", new { UserID=obj.UserID});
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                ViewBag.Error = errMsg;
            }
            catch(Exception err)
            {
                ViewBag.Error = err.Message.ToString();
            }
            ViewBag.Departments = user.GetDepartments(UI);
            
            ViewBag.Supervisors = user.GetSelectList(UI);
            ViewBag.Designations = user.GetDesignations(UI.BusinessUnit);
            return View(obj);
               

            //// If we got this far, something failed, redisplay form
            
            //return View(model);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult GetDesignation(int DesignationID=0)
        {
            var d = user.GetD(DesignationID)??new Designation();
            return PartialView("_DesignationRate",d);
        }

        //
        // GET: /Account/ChangePassword

        [CustomAuthorize()]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [CustomAuthorize()]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var loggedUser=SMS.Models.User.getUser(User.Identity.Name);
                if (MembershipService.ChangePassword(loggedUser.UserName, model.OldPassword, model.NewPassword))
                {
                    ViewBag.Info = "Password Changed Successfully";
                    return View(new ChangePasswordModel());
                }
                else
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        [CustomAuthorize(Roles = "Admin, User")]
        [HttpPost]
        public JsonResult ActiveDeactive(int UID)
        {
            try
            {
                user.ActiveDeactive(UID);
                user.Save();

                return Json(new { flag = "y", msg = "User Status Changed" });
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                return Json(new { flag = "n", msg = errMsg.ToString() });
            }
            catch (Exception err)
            {
                return Json(new { flag = "n", msg = err.Message.ToString() });
            }
        }

        [CustomAuthorize(Roles = "Admin, User")]
        [HttpPost]
        public JsonResult ASEUpdate(int UID)
        {
            try
            {
                user.ASEUpdate(UID);
                user.Save();

                return Json(new { flag = "y", msg = "User ASE Status Changed" });
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                return Json(new { flag = "n", msg = errMsg.ToString() });
            }
            catch (Exception err)
            {
                return Json(new { flag = "n", msg = err.Message.ToString() });
            }
        }

        [CustomAuthorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult ResetPassword(int UserID)
        {
            try
            {
                var user = db.Users.Find(UserID);
                user.Password = CommonMethod.GetMD5("1234");
                user.ConfirmPassword = user.Password;
                db.SaveChanges();
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                
                return Json(err.Message.ToString());
            }


            // If we got this far, something failed, redisplay form
            return Json("Password set to '1234'");
        }

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult Edit(int UserID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            ViewBag.Departments = user.GetDepartments(UI);
            ViewBag.Supervisors = user.GetSelectList(UI);


            ViewBag.Designations = user.GetDesignations(UI.BusinessUnit);
            return View(user.Get(UserID));
        }

        [CustomAuthorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult Edit(User obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            
            try
            {
                obj.CreateDate = DateTime.Now;
                user.Update(obj);
                user.Save();
                return RedirectToAction("SetPermission", "Permission", new { UserID = obj.UserID });
            }
            catch (DbEntityValidationException err)
            {
                String errMsg = "";
                foreach (var error in err.EntityValidationErrors)
                {
                    foreach (var failure in error.ValidationErrors)
                    {
                        errMsg = failure.ErrorMessage;
                    }
                }
                ViewBag.Error = errMsg;
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
            }
            ViewBag.Departments = user.GetDepartments(UI);
            
            ViewBag.Supervisors = user.GetSelectList(UI);
            ViewBag.Designations = user.GetDesignations(UI.BusinessUnit);
            return View(obj);
        }

        public ActionResult NoPermission()
        {
            ViewBag.Permission = "You have no permission to access this page!";
            return View();
        }

        public async Task<ActionResult> SyncUser()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            var org = user.OrgAll(UI.BusinessUnit).FirstOrDefault();
            //foreach(var org in units)
            //{
                try
                {
                    var users = await user.SyncEmployee(org.ProfitCenter);
                    if (users.Count() > 0)
                    {
                        var tobeadded = (from row in users
                                         where !(from prow in user.GetAll(UI)
                                                 select prow.UserName).Contains(row.EmployeeID)
                                         select row).ToList();

                        var tobeupdated = (from row in user.GetAll(UI)
                                           where (from prow in users
                                                  select prow.EmployeeID).Contains(row.UserName)
                                           select row).ToList();
                        var tobedisabled = (from row in user.GetAll(UI).Where(u=>u.IsASE==false)
                                            where !(from prow in users
                                                    select prow.EmployeeID).Contains(row.UserName)
                                            select row).ToList();
                        //----------------adding new users----------------------
                        int[] roles = new int[] { 2, 5, 6, 7 };
                        
                        
                        foreach (var add in tobeadded)
                        {
                            var u = new User();
                            u.BusinessUnit = UI.BusinessUnit;
                            u.CreateDate = DateTime.Now;
                            u.Department = add.Department.Trim();
                            u.DesignationID = (user.GetDByGrade(add.JobGrade) ?? new Designation() { DesignationID = 1 }).DesignationID;
                            u.Exist = true;
                            u.Email = (add.Email??"").Trim();
                            u.FullName = add.FullName.Trim();
                            u.Password = "1234";
                            u.UserName = add.EmployeeID.Trim();
                            u.IsASE = false;
                            u.ProfitCenter = add.ProfitCenter.Trim();
                            //---------adding roles--------------
                            foreach (var id in roles)
                            {
                                var role = user.GetRole(id);
                                if (role != null)
                                {
                                    u.Roles.Add(role);
                                }
                            }
                            user.Add(u);
                        }
                        //-----------------update existing user-----------
                        foreach (var upd in tobeupdated)
                        {
                            var u = users.Where(c => c.EmployeeID == upd.UserName).FirstOrDefault();
                            if (u != null)
                            {
                                upd.FullName = u.FullName.Trim();
                                upd.Email = (u.Email??"").Trim();
                                upd.Department = u.Department.Trim();
                                upd.DesignationID = (user.GetDByGrade(u.JobGrade) ?? new Designation() { DesignationID = 1 }).DesignationID;
                                upd.ProfitCenter = u.ProfitCenter.Trim();
                                upd.ConfirmPassword = upd.Password;
                            }
                        }
                        //------------disable existing user-------
                        foreach (var dis in tobedisabled)
                        {
                            dis.ConfirmPassword = dis.Password;
                            dis.Exist = false;
                        }
                        user.Save();
                        TempData["Info"] = "New User Added: " + tobeadded.Count + " . Updated User: "
                            + tobeupdated.Count + ". Disabled User: " + tobedisabled.Count;
                    }
                }
                catch (DbEntityValidationException err)
                {
                    String errMsg = "";
                    foreach (var error in err.EntityValidationErrors)
                    {
                        foreach (var failure in error.ValidationErrors)
                        {
                            errMsg = failure.ErrorMessage;
                        }
                    }
                    TempData["Error"] = errMsg;
                }
                catch (Exception err)
                {
                    TempData["Error"] = err.Message.ToString();
                }
            //}
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //#region  Open
        
        //public ActionResult UserLogOn()
        //{
        //    ViewBag.Users = user.GetSelectList();
        //    return PartialView();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SendeTAC(int UserID = 0)
        //{
        //    try
        //    {
        //        var emp = user.Get(UserID);
        //        #region Send Email
        //        if (emp == null)
        //        {
        //            return Json(new { success = false, message = "Please Select Your Name" });
        //        }

        //        var ES = Email.GetSingleEmailSetting();
        //        eTAC etac = new eTAC();
        //        etac.Generate();
        //        string body = "your one time password (OTP) is: " + etac.code +
        //            " and has been expired within 3 minitues";
        //        string subject = "New OTP Generated";
        //        Email.SendEmail(ES, emp.Email, subject, body);
        //        Session["etac"] = etac;
        //        return Json(new { success = true, message = "OTP has been sent to your email." });
        //        #endregion
        //    }
        //    catch (Exception err)
        //    {
        //        return Json(new { success = false, message = err.Message.ToString() });
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SetPassword(string pass, string conf, int otp = 0, int UserID = 0)
        //{
        //    try
        //    {
        //        #region Validate User
        //        if (UserID == 0)
        //        {
        //            return Json(new { success = false, message = "please slect your name" });
        //        }
        //        #endregion
        //        #region validate otp
        //        if (pass != conf)
        //        {
        //            return Json(new { success = false, message = "password doesn't match" });
        //        }
        //        var u = user.Get(UserID);
        //        if (Session["etac"] == null)
        //        {
        //            return Json(new { success = false, message = "no OTP Generated yet" });
        //        }
        //        var etac = (eTAC)Session["etac"];
        //        if (etac.code != otp || etac.Expire < DateTime.Now)
        //        {
        //            return Json(new { success = false, message = "otp has been expired or invalid" });
        //        }
        //        user.SetPassword(u, pass);
        //        user.Save();
        //        Session.Remove("etac");

        //        return Json(new { success = true, message = "your password has been reset, now you can login with this password." });
        //        #endregion
        //    }
        //    catch (Exception err)
        //    {
        //        return Json(new { success = false, message = err.Message.ToString() });
        //    }

        //}
        
        //#endregion

        #region Designation CRUD

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult IndexD(string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }

            if (q.Length > 0)
            {
                var SearchResult = user.GetAllD(UI.BusinessUnit).Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.Name);
                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(user.GetAllD(UI.BusinessUnit).OrderByDescending(e => e.Name));
        }

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult CreateD()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            return View(new Designation());
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult CreateD(Designation Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                Obj.BU = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                user.AddD(Obj);
                user.Save();
                TempData["Info"] = "New designation Created";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("CreateD");
        }
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult EditD(int DID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            var oldone = user.GetD(DID);
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult EditD(Designation Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                Obj.BU = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                user.UpdateD(Obj);
                user.Save();
                TempData["Info"] = "Update has been made";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("EditD", new { DID = Obj.DesignationID });
        }

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult AdjustManhourCost()
        {
            return PartialView();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult AdjustManhourCost(DateTime Start,DateTime End)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                user.AdjustRate(UI.BusinessUnit, Start, End);
                user.Save();
                return Json(new { flag = "yes", msg = "Adjustment have been made" });
            }
            catch(Exception err)
            {
                return Json(new { flag="no",msg=err.Message.ToString()});
            }
        }
        #endregion

        #region CRUD Revenue Wing
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult ManageWing()
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            ViewBag.Wings = user.GetWingAll(UI.BusinessUnit).ToList();
            return PartialView(new RevenueWing());
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult WingAddUpdate(RevenueWing Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                Obj.BU = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = SMS.Models.User.getUser(User.Identity.Name).UserID;
                if (Obj.RevenueWingID == 0)
                {
                    user.AddWing(Obj);
                }
                else
                {
                    user.UpdateWing(Obj);
                }
                user.Save();

                var Wings = user.GetWingAll(UI.BusinessUnit);
                return Json(new { flag = "true", msg = "Wing has been saved", listdata = Wings });
            }
            catch (Exception err)
            {
                return Json(new { flag = "false", msg = err.Message.ToString() });
            }

        }

        [CustomAuthorize(Roles = "Admin, User")]
        public ActionResult WingEdit(int ID)
        {
            var oldone = user.GetWing(ID);
            return Json(oldone, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
