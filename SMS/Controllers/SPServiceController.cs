using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using System.Data.Entity.Validation;

namespace SMS.Controllers
{
    public class SPServiceController : Controller
    {
        //
        // GET: /SPService/
        private ISPServiceRepository service { get; set; }
        private IProductRepository product { get; set; }
        private IUserRepository user { get; set; }
        public SPServiceController(ISPServiceRepository _service,IProductRepository _product,IUserRepository _user)
        {
            service = _service;
            product = _product;
            user = _user;
        }

        #region SP Service CRUD

        [CustomAuthorize]
        public ActionResult Index(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
            if (!Start.HasValue && !End.HasValue)
            {

                Start = new DateTime?(DateTime.Today.AddDays(-90));
                End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            ViewBag.SearchText = q;
            ViewBag.Chk = WithDate;
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
                
                if(WithDate)
                {
                    var SearchResult = service.GetAll(UI.BusinessUnit)
                    .Where(c => c.JobCompletionDate >= Start && c.JobCompletionDate <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.JobCompletionDate);
                    return View(SearchResult);
                }
                else
                {
                    var SearchResult = service.GetAll(UI.BusinessUnit)
                    
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.JobCompletionDate);
                    return View(SearchResult);
                }
                
            }
            
            if(WithDate)
            {
                return View(service.GetAll(UI.BusinessUnit)
                    .Where(c => c.JobCompletionDate >= Start && c.JobCompletionDate <= End)
                    .OrderByDescending(e => e.JobCompletionDate));
            }
            return View(service.GetAll(UI.BusinessUnit)
                    .OrderByDescending(e => e.JobCompletionDate));
        }
        [CustomAuthorize]
        public ActionResult IndexPartial(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            bool WithDate = (Request["WithDate"] ?? "") == "true,false" ? true : false;
            if (!Start.HasValue && !End.HasValue)
            {

                Start = new DateTime?(DateTime.Today.AddDays(-90));
                End = new DateTime?(DateTime.Today.AddDays(1));

            }
            ViewBag.StartDate = Start.Value.ToString("MM/dd/yyyy");
            ViewBag.EndDate = End.Value.ToString("MM/dd/yyyy");
            ViewBag.SearchText = q;
            ViewBag.Chk = WithDate;
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

                if (WithDate)
                {
                    var SearchResult = service.GetAll(UI.BusinessUnit)
                    .Where(c => c.JobCompletionDate >= Start && c.JobCompletionDate <= End)
                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.JobCompletionDate);
                    return PartialView("_IndexPartial",SearchResult);
                }
                else
                {
                    var SearchResult = service.GetAll(UI.BusinessUnit)

                    .Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.JobCompletionDate);
                    return PartialView("_IndexPartial", SearchResult);
                }

            }

            if (WithDate)
            {
                return PartialView("_IndexPartial", service.GetAll(UI.BusinessUnit)
                    .Where(c => c.JobCompletionDate >= Start && c.JobCompletionDate <= End)
                    .OrderByDescending(e => e.JobCompletionDate));
            }
            return PartialView("_IndexPartial", service.GetAll(UI.BusinessUnit)
                    .OrderByDescending(e => e.JobCompletionDate));
            
        }

        [CustomAuthorize]
        public ActionResult NewService(string DocNo="",int ItemNo=0)
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
            if(DocNo==""||ItemNo==0)
            {
                TempData["Error"] = "No Product Selected";
                return RedirectToAction("Index");
            }
            var selproduct = product.GetSapProduct(DocNo,ItemNo);
            if(selproduct==null)
            {
                TempData["Error"] = "No Product found for DocNo: "+DocNo;
                return RedirectToAction("Index");
            }
            var spservice = new SPService
            {
                BU=UI.BusinessUnit,
                DocNo=selproduct.DOCNO,
                ItemNo=selproduct.ITEMNO,
                JobCompletionDate=DateTime.Today,
                SAPProduct=selproduct,
                ServicePersonID=UI.User.UserID,
                ServicePerson=UI.User,
                UserID=UI.User.UserID
            };

            var works = service.GetAllServiceWork(UI.BusinessUnit);
            int c=1;
            foreach(var work in works)
            {
                var detail = new SPServiceDetail();
                detail.IsDone = false;
                detail.ServiceDetail = "";
                detail.ServiceWork = work;
                detail.Sl = c++;
                detail.WorkID = work.ID;
                spservice.Details.Add(detail);
            }
            return View(spservice);
        }

        [HttpPost]
        [CustomAuthorize()]
        public ActionResult NewService(SPService Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                UI.Host = Request.ServerVariables["REMOTE_HOST"];
                int i=0;
                foreach(var detail in Obj.Details.ToList())
                {
                    if(Request["Obj.Details["+i+"].IsDone"]=="on")
                    {
                        detail.IsDone = true;
                    }
                    i++;
                }
                service.Add(Obj,UI);
                Obj.Details = (List<SPServiceDetail>)Session["Details"];
                service.Save();
                Session.Remove("Details");
                TempData["Info"] = "New Service Posted";
                var prod = product.GetSapProduct(Obj.DocNo, Obj.ItemNo);
                return RedirectToAction("Index", new {q=prod.ENGINESERIAL });
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
            var selproduct = product.GetSapProduct(Obj.DocNo, Obj.ItemNo);
            Obj.SAPProduct = selproduct;
            Obj.ServicePerson = UI.User;
            
            foreach (var detail in Obj.Details.ToList())
            {
                var work = service.GetServiceWork(detail.WorkID);
                detail.ServiceWork = work;
            }
            return View(Obj);
        }
        [CustomAuthorize()]
        public ActionResult Preview(int ID)
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
            var oldone = service.Get(ID);
            return View(oldone);
        }

        [CustomAuthorize()]
        public ActionResult Edit(int ID)
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
            var oldone = service.Get(ID);
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            if(!users.Any(m=>m==oldone.ServicePersonID))
            {
                TempData["Error"] = "This Service not belong to logged user";
                return RedirectToAction("Index", new { q = oldone.SAPProduct.ENGINESERIAL });
            }
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize()]
        public ActionResult Edit(SPService Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                UI.Host = Request.ServerVariables["REMOTE_HOST"];
                int i = 0;
                foreach (var detail in Obj.Details.ToList())
                {
                    if (Request["Obj.Details[" + i + "].IsDone"] == "on")
                    {
                        detail.IsDone = true;
                    }
                    i++;
                }
                service.Update(Obj, UI);
                service.Save();
                TempData["Info"] = "Service History Updated";
                var prod = product.GetSapProduct(Obj.DocNo, Obj.ItemNo);
                return RedirectToAction("Index", new {q=prod.ENGINESERIAL });
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
            var selproduct = product.GetSapProduct(Obj.DocNo, Obj.ItemNo);
            Obj.SAPProduct = selproduct;
            Obj.ServicePerson = UI.User;
            foreach (var det in Obj.Details.ToList())
            {
                var work = service.GetServiceWork(det.WorkID);
                det.ServiceWork = work;
            }
            return View(Obj);
        }

        #endregion
        
        
        
    }
}