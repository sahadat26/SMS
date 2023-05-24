using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;
using System.IO;
using System.Data.Entity.Validation;

namespace SMS.Controllers
{
    public class ContractController : Controller
    {
        //
        // GET: /Vendor/
        private IContractRepository contract { get; set; }
        private IProductRepository product { get; set; }
        private ICustomerRepository customer { get; set; }
        private IUserRepository user { get; set; }


        public ContractController(IContractRepository _contract, IProductRepository _product,
            ICustomerRepository _customer,IUserRepository _user)
        {
            contract = _contract;
            customer = _customer;
            user = _user;
            product = _product;
        }
        #region Service Contract

        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult ServiceContracts(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            contract.SCAutoClose();
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

                var list = contract.GetSCAll(UI).Where(b => b.ContractEndDate >= Start
                    && b.ContractEndDate <= End)
                    .OrderByDescending(b => b.ServiceContractID);
                if (q.Length > 0)
                {
                    if (WithDate)
                    {
                        list = contract.GetSCAll(UI).Where(b => b.ContractEndDate >= Start
                            && b.ContractEndDate <= End)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.ServiceContractID);
                        ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = contract.GetSCAll(UI)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.ServiceContractID);
                        ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                else
                {
                    if (WithDate)
                    {
                        list = contract.GetSCAll(UI).Where(b => b.ContractEndDate >= Start
                            && b.ContractEndDate <= End)

                        .OrderByDescending(b => b.ServiceContractID);
                        //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = contract.GetSCAll(UI)

                        .OrderByDescending(b => b.ServiceContractID);
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
                var list = contract.GetSCAll(UI).Where(b => b.ContractEndDate >= DateTime.Today.AddMonths(-1)
                    && b.ContractEndDate <= DateTime.Today)
                    .OrderByDescending(b => b.ServiceContractID);
                return View(list);
            }
        }

        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult CreateSC()
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
            return View(new ServiceContract { BusinessUnit = UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult CreateSC(ServiceContract Obj, HttpPostedFileBase FileToUpload)
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
                        string NewFileName = "srv_" + contract.ServiceRandFileName(999999, ext) + ext;
                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.AttachmentURL = NewFileName;
                    }

                }
                #endregion
                Obj.BusinessUnit = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                contract.AddSC(Obj);
                contract.Save();
                TempData["Info"] = "New Service Contract has been created";
                return RedirectToAction("ManageSP", new {SCID=Obj.ServiceContractID });
            }
            //catch (DbEntityValidationException err)
            //{
            //    String errMsg = "";
            //    foreach (var error in err.EntityValidationErrors)
            //    {
            //        foreach (var failure in error.ValidationErrors)
            //        {
            //            errMsg = failure.ErrorMessage;
            //        }
            //    }
            //    TempData["Error"] = errMsg;
            //}
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("CreateSC");
        }
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult EditSC(int SCID)
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
            var oldone = contract.GetSC(SCID);
            ViewBag.Customers = customer.GetSelectList(UI.BusinessUnit);
            ViewBag.Contacts = new SelectList(customer.GetAllContact(oldone.CustomerID), "ContactID", "ContactPerson");
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult EditSC(ServiceContract Obj, HttpPostedFileBase FileToUpload)
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
                        string NewFileName = "srv_" + contract.ServiceRandFileName(999999, ext) + ext;
                        if ((Obj.AttachmentURL ?? "") != "")
                        {
                            NewFileName = Obj.AttachmentURL;
                        }

                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.AttachmentURL = NewFileName;
                    }

                }
                #endregion
                Obj.BusinessUnit = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                contract.UpdateSC(Obj);
                contract.Save();
                TempData["Info"] = "Update has been made";
                return RedirectToAction("ManageSP", new { SCID = Obj.ServiceContractID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("EditSC", new { SID = Obj.ServiceContractID });
        }

        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult RenewContract(int SCID)
        {
            var sc = contract.GetSC(SCID);
            return PartialView(sc);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult RenewContract(int ServiceContractID, DateTime ContractStartDate, DateTime ContractEndDate)
        {
            try
            {
                contract.RenewContract(ServiceContractID, ContractStartDate, ContractEndDate);
                contract.Save();
                return Json(new { success = true, message = "The renew action has been done",SCID=ServiceContractID });
            }
            catch(Exception err)
            {
                return Json(new { success = false, message = err.Message.ToString() });
            }
        }
        #endregion
        #region Service Product
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult ManageSP(int SCID, int ProID = 0)
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
            var sc = contract.GetSC(SCID);
            if (sc == null)
            {
                TempData["Error"] = "No contract selected";
                return RedirectToAction("ServiceContracts");
            }
            ViewBag.SC = sc;
            
                ViewBag.Products = product.GetProductSelectList(sc.BusinessUnit);
                if (ProID > 0)
                {
                    var pro = product.GetProduct(ProID);
                    foreach (var p in pro.Properties)
                    {
                        p.PropertyValue = contract.GetSPPropertyValue(p.ProductPropertyID);
                    }
                    return View("SaveSP", new ServiceProduct
                    {
                        ServiceContractID= sc.ServiceContractID,
                        ProductID = ProID,
                        Product = pro
                    });
                }
                return View("SaveSP", new ServiceProduct { ServiceContractID = sc.ServiceContractID });
            
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult SaveSP(ServiceProduct sp)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                sp.EntryDateTime = DateTime.Now;
                sp.HostName = Request.ServerVariables["REMOTE_HOST"];
                sp.UserID = UI.User.UserID;
                if (sp.ServiceProductID == 0)
                {
                    contract.AddSP(sp);
                }
                else
                {
                    contract.UpdateSP(sp);
                }
                contract.Save();
                TempData["Info"] = "Product has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageSP", new { SCID = sp.ServiceContractID });
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditSP(int ID)
        {
            var oldone = contract.GetSP(ID);
            ViewBag.Products = product.GetProductSelectList(oldone.Product.BusinessUnit);
            ViewBag.SC = oldone.ServiceContract;
            foreach (var p in oldone.Product.Properties)
            {
                p.PropertyValue = contract.GetSPPropertyValue(p.ProductPropertyID);
            }
            return View("SaveSP", oldone);
        }

        #endregion

        #region Service Contract Collection
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult ManageCollection(int SCID,int SPID=0)
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
            var sc = contract.GetSC(SCID);
            var sp=contract.GetSP(SPID);
            var splist = contract.GetSPAll(SCID);
            ViewBag.Products = new SelectList(splist,"ServiceProductID","DisplayText");
            ViewBag.SC = sc;
            ViewBag.SP = sp;
            if(sp!=null)
            {
                return View("SaveCollection", new 
                ServiceContractCollection { 
                    ServiceProductID = SPID,
                    CollectedAmount=sp.ContractAmount,
                    CollectionDate=DateTime.Today
                });
            }
            return View("SaveCollection", new ServiceContractCollection {
                ServiceProductID=SPID,
                CollectionDate=DateTime.Today
            });
        }
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult SaveCollection(ServiceContractCollection collection,int SCID)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                collection.EntryDateTime = DateTime.Now;
                collection.HostName = Request.ServerVariables["REMOTE_HOST"];
                collection.UserID = UI.User.UserID;
                if (collection.ServiceContractCollectionID == 0)
                {
                    contract.AddCol(collection);
                }
                else
                {
                    contract.UpdateCol(collection);
                }
                contract.Save();
                TempData["Info"] = "Collection info has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageCollection", new { SCID = SCID });
        }
        
        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditCollection(int ID)
        {
            var oldone = contract.GetCol(ID);
            var sc = contract.GetSC(oldone.ServiceProduct.ServiceContractID);
            var sp = contract.GetSP(oldone.ServiceProductID);
            var splist = contract.GetSPAll(oldone.ServiceProduct.ServiceContractID);
            ViewBag.Products = new SelectList(splist, "ServiceProductID", "DisplayText");
            ViewBag.SC = sc;
            ViewBag.SP = sp;
            return View("SaveCollection", oldone);
        }
        #endregion

        #region Warranty Contracts

        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult WarrantyContracts(DateTime? Start, DateTime? End, string q = "")
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            contract.WCAutoClose();
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

                var list = contract.GetWCAll(UI).Where(b => b.WarrantyEnd >= Start
                    && b.WarrantyEnd <= End)
                    .OrderByDescending(b => b.WarrantyContractID);
                if (q.Length > 0)
                {
                    if (WithDate)
                    {
                        list = contract.GetWCAll(UI).Where(b => b.WarrantyEnd >= Start
                            && b.WarrantyEnd <= End)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.WarrantyContractID);
                        ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = contract.GetWCAll(UI)
                        .Where(b => b.SearchText.ToLower().Contains(q.ToLower()))
                        .OrderByDescending(b => b.WarrantyContractID);
                        ViewBag.Info = "Search by '" + q + "' found " + list.Count() + " record(s)";
                    }
                }
                else
                {
                    if (WithDate)
                    {
                        list = contract.GetWCAll(UI).Where(b => b.WarrantyEnd >= Start
                            && b.WarrantyEnd <= End)

                        .OrderByDescending(b => b.WarrantyContractID);
                        //ViewBag.Info = "Search by '" + q + "' with date range found " + list.Count() + " record(s)";
                    }
                    else
                    {
                        list = contract.GetWCAll(UI)

                        .OrderByDescending(b => b.WarrantyContractID);
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
                var list = contract.GetWCAll(UI).Where(b => b.WarrantyEnd >= DateTime.Today.AddMonths(-1)
                    && b.WarrantyEnd <= DateTime.Today)
                    .OrderByDescending(b => b.WarrantyContractID);
                return View(list);
            }
        }

        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult CreateWC()
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
            return View(new WarrantyContract { BusinessUnit = UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult CreateWC(WarrantyContract Obj, HttpPostedFileBase FileToUpload)
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
                        string NewFileName = "warr_" + contract.WarrantyRandFileName(999999, ext) + ext;
                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.AttachmentURL = NewFileName;
                    }

                }
                #endregion
                Obj.BusinessUnit = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                contract.AddWC(Obj);
                contract.Save();
                TempData["Info"] = "New Warranty Contract has been created";
                return RedirectToAction("ManageWP", new {WCID=Obj.WarrantyContractID });
            }
            //catch (DbEntityValidationException err)
            //{
            //    String errMsg = "";
            //    foreach (var error in err.EntityValidationErrors)
            //    {
            //        foreach (var failure in error.ValidationErrors)
            //        {
            //            errMsg = failure.ErrorMessage;
            //        }
            //    }
            //    TempData["Error"] = errMsg;
            //}
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("CreateWC");
        }
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult EditWC(int WCID)
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
            var oldone = contract.GetWC(WCID);
            ViewBag.Customers = customer.GetSelectList(UI.BusinessUnit);
            ViewBag.Contacts = new SelectList(customer.GetAllContact(oldone.CustomerID), "ContactID", "ContactPerson");
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult EditWC(WarrantyContract Obj, HttpPostedFileBase FileToUpload)
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
                        string NewFileName = "warr_" + contract.WarrantyRandFileName(999999, ext) + ext;
                        if ((Obj.AttachmentURL ?? "") != "")
                        {
                            NewFileName = Obj.AttachmentURL;
                        }

                        FileToUpload.SaveAs(destination + NewFileName);
                        Obj.AttachmentURL = NewFileName;
                    }

                }
                #endregion
                Obj.BusinessUnit = UI.BusinessUnit;
                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                contract.UpdateWC(Obj);
                contract.Save();
                TempData["Info"] = "Update has been made";
                return RedirectToAction("ManageWP", new { WCID = Obj.WarrantyContractID });
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("EditWC", new { WID = Obj.WarrantyContractID });
        }

        #endregion
        #region Warranty Product
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult ManageWP(int WCID, int ProID = 0)
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
            var wc = contract.GetWC(WCID);
            if (wc == null)
            {
                TempData["Error"] = "No contract selected";
                return RedirectToAction("WarrantyContracts");
            }
            ViewBag.WC = wc;

            ViewBag.Products = product.GetProductSelectList(wc.BusinessUnit);
            if (ProID > 0)
            {
                var pro = product.GetProduct(ProID);
                foreach (var p in pro.Properties)
                {
                    p.PropertyValue = contract.GetWPPropertyValue(p.ProductPropertyID);
                }
                return View("SaveWP", new WarrantyProduct
                {
                    WarrantyContractID = wc.WarrantyContractID,
                    ProductID = ProID,
                    Product = pro
                });
            }
            return View("SaveWP", new WarrantyProduct { WarrantyContractID = wc.WarrantyContractID });

        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Contract")]
        public ActionResult SaveWP(WarrantyProduct wp)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                wp.EntryDateTime = DateTime.Now;
                wp.HostName = Request.ServerVariables["REMOTE_HOST"];
                wp.UserID = UI.User.UserID;
                if (wp.WarrantyProductID == 0)
                {
                    contract.AddWP(wp);
                }
                else
                {
                    contract.UpdateWP(wp);
                }
                contract.Save();
                TempData["Info"] = "Product has been saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageWP", new { WCID = wp.WarrantyContractID });
        }

        [CustomAuthorize(Roles = "Admin, Job")]
        public ActionResult EditWP(int ID)
        {
            var oldone = contract.GetWP(ID);
            ViewBag.Products = product.GetProductSelectList(oldone.Product.BusinessUnit);
            ViewBag.WC = oldone.WarrantyContract;
            foreach (var p in oldone.Product.Properties)
            {
                p.PropertyValue = contract.GetWPPropertyValue(p.ProductPropertyID);
            }
            return View("SaveWP", oldone);
        }

        #endregion
        #region JSON
        [HttpPost]
        [CustomAuthorize]
        public ActionResult GetContacts(int CID=0)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            var list = (from row in customer.GetAllContact(CID)
                        select new
                        {
                            ID = row.ContactID,
                            Name = row.ContactPerson
                        }).ToList();
            return Json(list);
        }
        #endregion
    }
}