using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using System.IO;
using SMS.Reports;

namespace SMS.Controllers
{
    public class JobController : Controller
    {
        //
        // GET: /Job/
        private IJobRepository job { get; set; }
        private IProductRepository product { get; set; }
        private ICustomerRepository customer { get; set; }
        private IUserRepository user { get; set; }
        private IContractRepository contract { get; set; }
        
        public JobController(IJobRepository _job,IProductRepository _product,
            ICustomerRepository _customer,IUserRepository _user,
            IContractRepository _contract)
        {
            job = _job;
            customer = _customer;
            user = _user;
            product = _product;
            contract = _contract;
        }

        #region Query CRUD

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult Queries(DateTime? Start, DateTime? End, string q = "",EState QState=0)
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
            try
            {
                bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
                if (Start == null && End == null)
                {
                    Start = DateTime.Today.AddMonths(-1);
                    End = DateTime.Today;
                }

                var list = job.GetAllQuery(UI).Where(b => b.QueryDate >= Start
                    && b.QueryDate <= End)
                    .OrderByDescending(b => b.QueryDate);
                if(QState>0)
                {
                    if (q.Length > 0)
                    {
                        if (WithDate)
                        {
                            list = job.GetAllQuery(UI).Where(b => b.QueryDate >= Start
                                && b.QueryDate <= End&&b.State==QState)
                            .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                            .OrderByDescending(b => b.QueryDate);
                            ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                        }
                        else
                        {
                            list = job.GetAllQuery(UI).Where(b=>b.State==QState)
                            .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                            .OrderByDescending(b => b.QueryDate);
                            ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                        }
                    }
                    else
                    {
                        if (WithDate)
                        {
                            list = job.GetAllQuery(UI).Where(b => b.QueryDate >= Start
                                && b.QueryDate <= End && b.State == QState)

                            .OrderByDescending(b => b.QueryDate);
                            //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                        }
                        else
                        {
                            list = job.GetAllQuery(UI).Where(b=>b.State==QState)

                            .OrderByDescending(b => b.QueryDate);
                            //ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                        }
                    }
                }
                else
                {
                    if (q.Length > 0)
                    {
                        if (WithDate)
                        {
                            list = job.GetAllQuery(UI).Where(b => b.QueryDate >= Start
                                && b.QueryDate <= End)
                            .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                            .OrderByDescending(b => b.QueryDate);
                            ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                        }
                        else
                        {
                            list = job.GetAllQuery(UI)
                            .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                            .OrderByDescending(b => b.QueryDate);
                            ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                        }
                    }
                    else
                    {
                        if (WithDate)
                        {
                            list = job.GetAllQuery(UI).Where(b => b.QueryDate >= Start
                                && b.QueryDate <= End)

                            .OrderByDescending(b => b.QueryDate);
                            //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                        }
                        else
                        {
                            list = job.GetAllQuery(UI)

                            .OrderByDescending(b => b.QueryDate);
                            //ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                        }
                    }
                }
                ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
                ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
                ViewBag.Keywords = q;
                ViewBag.Chk = WithDate;
                ViewBag.QState = QState;
                return View(list);
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
                var list = job.GetAllQuery(UI).Where(b => b.QueryDate >= DateTime.Today.AddMonths(-1)
                    && b.QueryDate <= DateTime.Today)
                    .OrderByDescending(b => b.QueryDate);
                return View(list);
            }
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult CanceledQueries(DateTime? Start, DateTime? End, string q = "")
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
            try
            {
                bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
                if (Start == null && End == null)
                {
                    Start = DateTime.Today.AddMonths(-1);
                    End = DateTime.Today;
                }

                var list = job.GetCanceledJob(UI).Where(b => b.JobCardCreateDate >= Start
                    && b.JobCardCreateDate <= End && b.State == EState.Canceled)
                    .OrderByDescending(b => b.JobCardCreateDate);
                if (q.Length > 0)
                {
                    if (WithDate)
                    {
                        list = job.GetCanceledJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.State == EState.Canceled)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobCardCreateDate);
                        ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetCanceledJob(UI).Where(b => b.State == EState.Canceled)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobCardCreateDate);
                        ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                else
                {
                    if (WithDate)
                    {
                        list = job.GetCanceledJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.State == EState.Canceled)

                        .OrderByDescending(b => b.JobCardCreateDate);
                        //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetCanceledJob(UI)
                        .Where(b => b.State == EState.Canceled)
                        .OrderByDescending(b => b.JobCardCreateDate);
                        //ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
                ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
                ViewBag.Keywords = q;
                ViewBag.Chk = WithDate;

                return View(list);
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
                var list = job.GetCanceledJob(UI).Where(b => b.JobCardCreateDate >= DateTime.Today.AddMonths(-1)
                    && b.JobCardCreateDate <= DateTime.Today && b.State == EState.Canceled)
                    .OrderByDescending(b => b.JobCardCreateDate);
                return View(list);
            }
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult CreateQuery()
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
            
            ViewBag.Customers = customer.GetSelectList(UI.BusinessUnit);
            return View(new Job { BusinessUnit = UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult CreateQuery(Job Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                job.AddQuery(Obj);
                job.Save();
                TempData["Info"] = "New Query has been created";
                return RedirectToAction("ManageQueryDetail", new { JID = Obj.JobID });
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
            }

            ViewBag.Customers = customer.GetSelectList(UI.BusinessUnit);
            return View(Obj);
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditQuery(int JID)
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
            var oldone = job.Get(JID);

            ViewBag.Customers = customer.GetSelectList(UI.BusinessUnit);
            ViewBag.Contacts = new SelectList(customer.GetAllContact(oldone.CustomerID), "ContactID", "ContactPerson");
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditQuery(Job Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                job.UpdateQuery(Obj);
                job.Save();
                TempData["Info"] = "Update has been made";
                return RedirectToAction("ManageQueryDetail", new { JID = Obj.JobID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("EditQuery", new { JID = Obj.JobID });
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult CancelQuery(int JID)
        {
            var oldone = job.Get(JID);
            return PartialView(oldone);
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        [HttpPost]
        public JsonResult CancelQuery(int JobID, string Reason)
        {
            try
            {
                job.CancelQuery(JobID, Reason);
                job.Save();
                return Json(new { success = true, message = "The Job has been canceled" });
            }
            catch (Exception err)
            {
                return Json(new { success = false, message = err.Message.ToString() });
            }
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        [HttpPost]
        public JsonResult RestoreQuery(int JobID)
        {
            try
            {
                job.RestoreQuery(JobID);
                job.Save();
                return Json(new { success = true, message = "The Job has been restored" });
            }
            catch (Exception err)
            {
                return Json(new { success = false, message = err.Message.ToString() });
            }
        }

        #endregion
        #region Job Card CRUD

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult JobCards(DateTime? Start, DateTime? End, string q = "")
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
            try
            {
                bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
                if (Start == null && End == null)
                {
                    Start = DateTime.Today.AddMonths(-1);
                    End = DateTime.Today;
                }

                var list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                    && b.JobCardCreateDate <= End && b.State==EState.Progress)
                    .OrderByDescending(b => b.JobCardCreateDate);
                if (q.Length > 0)
                {
                    if (WithDate)
                    {
                        list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.State == EState.Progress)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobCardCreateDate);
                        ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetAllJob(UI).Where(b=>b.State==EState.Progress)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobCardCreateDate);
                        ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                else
                {
                    if (WithDate)
                    {
                        list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.State == EState.Progress)

                        .OrderByDescending(b => b.JobCardCreateDate);
                        //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetAllJob(UI)
                        .Where(b=>b.State==EState.Progress)
                        .OrderByDescending(b => b.JobCardCreateDate);
                        //ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
                ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
                ViewBag.Keywords = q;
                ViewBag.Chk = WithDate;

                return View(list);
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
                var list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= DateTime.Today.AddMonths(-1)
                    && b.JobCardCreateDate <= DateTime.Today && b.State == EState.Progress)
                    .OrderByDescending(b => b.JobCardCreateDate);
                return View(list);
            }
        }
        
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult JobCardArchive(DateTime? Start, DateTime? End, string q = "")
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
            try
            {
                bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
                if (Start == null && End == null)
                {
                    Start = DateTime.Today.AddMonths(-1);
                    End = DateTime.Today;
                }

                var list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                    && b.JobCardCreateDate <= End&&b.Status==EStatus.Close)
                    .OrderByDescending(b => b.JobFinishedDate);
                if (q.Length > 0)
                {
                    if (WithDate)
                    {
                        list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.Status == EStatus.Close)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobFinishedDate);
                        ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetAllJob(UI).Where(b=>b.Status==EStatus.Close)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.JobFinishedDate);
                        ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                else
                {
                    if (WithDate)
                    {
                        list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= Start
                            && b.JobCardCreateDate <= End && b.Status == EStatus.Close)

                        .OrderByDescending(b => b.JobFinishedDate);
                        //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = job.GetAllJob(UI).Where(b=>b.Status==EStatus.Close)
                        .OrderByDescending(b => b.JobFinishedDate);
                        //ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
                ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
                ViewBag.Keywords = q;
                ViewBag.Chk = WithDate;

                return View(list);
            }
            catch (Exception err)
            {
                ViewBag.Error = err.Message.ToString();
                var list = job.GetAllJob(UI).Where(b => b.JobCardCreateDate >= DateTime.Today.AddMonths(-1)
                    && b.JobCardCreateDate <= DateTime.Today && b.Status == EStatus.Close)
                    .OrderByDescending(b => b.JobFinishedDate);
                return View(list);
            }
        }
        
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SaveJobCard(int JID)
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
            var oldone = job.Get(JID);
            if(oldone.Products.Count==0)
            {
                TempData["Error"] = "No product Added";
                return RedirectToAction("ManageQueryDetail", new {JID=oldone.JobID });
            }
            ViewBag.JobTitles = job.GetJobTitles(UI.BusinessUnit);
            ViewBag.Wings = user.GetWingSelectlist(UI.BusinessUnit);
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SaveJobCard(Job Obj, HttpPostedFileBase FileToUpload)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {
                #region Upload Validation
                if (FileToUpload != null)
                {

                    if (FileToUpload.ContentLength > 1048576)
                    {
                        throw new Exception("you can upload max 1mb file!");
                    }
                    else if (!(FileToUpload.ContentType == "application/pdf"))
                    {
                        throw new Exception("Given file not a valid pdf");
                    }
                    else
                    {

                        string destination = Server.MapPath("~/Upload/Attachment/");
                        string ext = Path.GetExtension(FileToUpload.FileName);
                        string NewFileName = "job_" + job.RandFileName(999999, ext) + ext;
                        if ((Obj.AttachmentURL ?? "") != "")
                        {
                            NewFileName = Obj.AttachmentURL;
                        }

                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.AttachmentURL = NewFileName;
                    }

                }
                #endregion
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                job.SaveJob(Obj);
                job.Save();
                TempData["Info"] = "Job Card has been saved";
                return RedirectToAction("ManageManPowerCost", new { JID = Obj.JobID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("SaveJobCard", new { JID = Obj.JobID });
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult FinishJob(int JID)
        {
            var oldone = job.Get(JID);
            oldone.JobFinishedDate = DateTime.Today;
            return PartialView(oldone);
        }
        [CustomAuthorize(Roles = "Admin, Job")]
        [HttpPost]
        public JsonResult FinishJob(int JobID, DateTime FinishDate)
        {
            try
            {
                job.FinishJob(JobID, FinishDate);
                job.Save();
                return Json(new { success = true, message = "The Job has been finished" });
            }
            catch (Exception err)
            {
                return Json(new { success = false, message = err.Message.ToString() });
            }
        }


        

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult JobCardPreview(int JID)
        {
            var j = job.Get(JID);

            return View("ReportPreview", new JobCard(j));
        }
        #endregion
        #region Product Line
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult ManageQueryDetail(int JID,int ProID=0)
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
            var j = job.Get(JID);
            if (j == null)
            {
                TempData["Error"] = "No Job selected";
                return RedirectToAction("Queries");
            }
            ViewBag.Job =j;
            if(j.QueryFor==EQueryFor.ServiceContract)
            {
                var list = contract.GetSPAll(j.ServiceContractID.GetValueOrDefault(0)).ToList();
                ViewBag.SP = new SelectList(list,"ServiceProductID","DisplayText");
                return View("SavePLFromService");

            }
            else if (j.QueryFor == EQueryFor.Warranty)
            {
                var list = contract.GetWPAll(j.WarrantyContractID.GetValueOrDefault(0)).ToList();
                            
                ViewBag.WP = new SelectList(list, "WarrantyProductID", "DisplayText");
                return View("SavePLFromWarranty");
            }
            else if (j.QueryFor == EQueryFor.OnCall)
            {
                ViewBag.Products = product.GetProductSelectList(j.BusinessUnit);
                if (ProID > 0)
                {
                    var pro = product.GetProduct(ProID);
                    foreach (var p in pro.Properties)
                    {
                        p.PropertyValue = job.GetPropertyValue(p.ProductPropertyID);
                    }
                    return View("SaveProductLine", new ProductLine
                    {
                        JobID = j.JobID,
                        ProductID = ProID,
                        Product = pro
                    });
                }
                return View("SaveProductLine", new ProductLine { JobID = j.JobID });
            }

            return Content("Invalid Selection");
        }
        
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SaveProductLine(ProductLine pl)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                pl.EntryDateTime = DateTime.Now;
                pl.HostName = Request.ServerVariables["REMOTE_HOST"];
                pl.UserID = UI.User.UserID;
                if (pl.ProductLineID == 0)
                {
                    job.AddPL(pl);
                }
                else
                {
                    job.UpdatePL(pl);
                }
                job.Save();
                TempData["Info"] = "Product has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageQueryDetail", new { JID = pl.JobID });
        }
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SavePLFromService(int SPID,int JobID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                var HostName = Request.ServerVariables["REMOTE_HOST"];
                var UserID = UI.User.UserID;
                job.AddPLFromService(SPID,JobID,UserID,HostName);
                job.Save();
                TempData["Info"] = "Product has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageQueryDetail", new { JID = JobID });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SavePLFromWarranty(int WPID, int JobID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                var HostName = Request.ServerVariables["REMOTE_HOST"];
                var UserID = UI.User.UserID;
                job.AddPLFromWarranty(WPID, JobID, UserID, HostName);
                job.Save();
                TempData["Info"] = "Product has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageQueryDetail", new { JID = JobID });
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditProductLine(int ID)
        {
            var oldone = job.GetPL(ID);
            ViewBag.Products = product.GetProductSelectList(oldone.Product.BusinessUnit);
            ViewBag.Job = oldone.Job;
            foreach (var p in oldone.Product.Properties)
            {
                p.PropertyValue = job.GetPropertyValue(p.ProductPropertyID);
            }
            return View("SaveProductLine", oldone);
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult DeleteProductLine(int PLID,int JobID)
        {
            try
            {
                job.DeletePL(PLID);
                job.Save();
            }
            catch(Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageQueryDetail", new {JID=JobID });
        }
        
        #endregion
        #region ManPower Cost
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult ManageManPowerCost(int JID,int EmpID=0,EDailyAllowanceType DAType=0,EAccomodationType AType=0)
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
            var j = job.Get(JID);
            if (j == null)
            {
                TempData["Error"] = "No Job selected";
                return RedirectToAction("JobCards");
            }
            ViewBag.Job = j;
            ViewBag.Employees = user.GetSelectList(UI);
            if(EmpID>0)
            {
                var emp = user.Get(EmpID);
                return View("SaveManPowerCost", new ManpowerCost 
                { 
                    JobID = JID,
                    EmployeeID=EmpID,
                    Employee=emp,
                    WorkingHourRate=emp.Designation.WorkingHourRate,
                    OTHourRate=emp.Designation.OTHourRate,
                    HolidayHourRate=emp.Designation.HolidayHourRate,
                    FoodAllowance=emp.Designation.FoodAllowance,
                    DailyAllowance=DAType==EDailyAllowanceType.WithCompanyVehicle?emp.Designation.DailyAllowanceCV:
                    emp.Designation.DailyAllowance,
                    Accomodation=AType==EAccomodationType.OtherCity?emp.Designation.OCAccomodation:
                    emp.Designation.MCAccomodation,
                    StartTime=DateTime.Now,
                    EndTime=DateTime.Now.AddHours(1),
                    DailyAllowanceType=DAType,
                    AccomodationType=AType
                });
            }

            return View("SaveManPowerCost", new ManpowerCost {JobID=JID,StartTime=DateTime.Now,EndTime=DateTime.Now.AddHours(1) });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult SaveManPowerCost(ManpowerCost mc)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                mc.EntryDateTime = DateTime.Now;
                mc.HostName = Request.ServerVariables["REMOTE_HOST"];
                mc.UserID = UI.User.UserID;
                int DID = user.Get(mc.EmployeeID).DesignationID;
                if (mc.ManpowerCostID == 0)
                {
                    job.AddMC(mc,DID);
                }
                else
                {
                    job.UpdateMC(mc,DID);
                }
                job.Save();
                TempData["Info"] = "Manpower Cost has been saved";
                return RedirectToAction("ManageManPowerCost", new { JID = mc.JobID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageManPowerCost", new { JID = mc.JobID,EmpID=mc.EmployeeID });
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult DeleteManpowerCost(int ID,int JobID)
        {
            try
            {
                job.DeleteMC(ID);
                job.Save();
                TempData["Info"] = "The entry has been deleted";
            }
            catch(Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageManPowerCost", new { JID = JobID });
        }

        #endregion
        #region JSON
        [HttpPost]
        [CustomAuthorize]
        public ActionResult OnChangeCustomer(int CID=0)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            var contactlist = (from row in customer.GetAllContact(CID)
                        select new
                        {
                            ID = row.ContactID,
                            Name = row.ContactPerson
                        }).ToList();
            var servicelist=(from row in contract.GetSCAll(UI).Where(sc=>sc.CustomerID==CID&&sc.Status==EStatus.Open)
                             select new
                             {
                                 ID=row.ServiceContractID,
                                 Name=row.DisplayText
                             }
                             ).ToList();
            var warrantylist = (from row in contract.GetWCAll(UI).Where(wc => wc.CustomerID == CID && wc.Status == EStatus.Open)
                               select new
                               {
                                   ID = row.WarrantyContractID,
                                   Name = row.DisplayText
                               }
                             ).ToList();
            return Json(new {contacts=contactlist,services=servicelist,warranties=warrantylist });
        }
        
        #endregion

        
    }
}