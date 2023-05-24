using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using SMS.Interfaces;

namespace SMS.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Vendor/
        private ICustomerRepository customer { get; set; }
        public CustomerController(ICustomerRepository _customer)
        {
            customer = _customer;
        }

        #region Customer CRUD

        [CustomAuthorize(Roles = "Admin, Customer")]
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
                var SearchResult = customer.GetAll(UI.BusinessUnit).Where(e =>
                    e.SearchText.ToLower().Contains(q.ToLower()))
                    .OrderByDescending(e => e.Name);
                ViewBag.SearchText = q;

                return View(SearchResult);
            }

            return View(customer.GetAll(UI.BusinessUnit).OrderByDescending(e => e.Name));
        }

        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult Create()
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
            return View(new Customer { BusinessUnit = UI.BusinessUnit });
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult Create(Customer Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                customer.Add(Obj);
                customer.Save();
                TempData["Info"] = "New Customer Created";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("Create");
        }
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult Edit(int CID)
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
            var oldone = customer.Get(CID);
            return View(oldone);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult Edit(Customer Obj)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);

            try
            {

                Obj.EntryDateTime = DateTime.Now;
                Obj.HostName = Request.ServerVariables["REMOTE_HOST"];
                Obj.UserID = UI.User.UserID;
                customer.Update(Obj);
                customer.Save();
                TempData["Info"] = "Update has been made";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }

            return RedirectToAction("Edit", new { VID = Obj.CustomerID });
        }

        #endregion

        #region Contact
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult ManageContact(int CID)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Info"] != null)
            {
                ViewBag.Info = TempData["Info"];
            }
            var cus = customer.Get(CID);
            if (cus == null)
            {
                TempData["Error"] = "No customer selected";
                return RedirectToAction("Index");
            }
            ViewBag.Cus = cus;
            return View(new Contact { CustomerID = cus.CustomerID });
        }
        [HttpPost]
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult SaveContact(Contact Contact)
        {
            var UI = UserInfo.GetUserInfo(User.Identity.Name);
            try
            {
                Contact.EntryDateTime = DateTime.Now;
                Contact.HostName = Request.ServerVariables["REMOTE_HOST"];
                Contact.UserID = UI.User.UserID;
                if (Contact.ContactID == 0)
                {
                    customer.AddContact(Contact);
                }
                else
                {
                    customer.UpdateContact(Contact);
                }
                customer.Save();
                TempData["Info"] = "Contact Saved";
            }
            catch (Exception err)
            {
                TempData["Error"] = err.Message.ToString();
            }
            return RedirectToAction("ManageContact", new { CID = Contact.CustomerID });
        }
        [CustomAuthorize(Roles = "Admin, Customer")]
        public ActionResult EditContact(int ID)
        {
            var oldone = customer.GetContact(ID);
            ViewBag.Cus = oldone.Customer;
            return View("ManageContact",oldone);
        }
        #endregion
    }
}