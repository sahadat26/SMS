using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS.Interfaces;
using SMS.Models;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Data.Entity;

namespace SMS.Repositories
{
    public class CustomerRepository:ICustomerRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        private JobRepository job { get; set; }
        public CustomerRepository(SMSContext _db,JobRepository _job)
        {
            db = _db;
            job = _job;
        }
        public CustomerRepository()
            : this(new SMSContext(),new JobRepository())
        {

        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Customer
        public Customer Get(int ID)
        {
            return db.Customers.Find(ID);
        }

        public IEnumerable<Customer> GetAll(EBusinessUnit BU)
        {
            if (BU == EBusinessUnit.ALL)
                return db.Customers;
            return db.Customers.Where(v=>v.BusinessUnit==BU);
        }

        public void Add(Customer obj)
        {
            if (db.Customers.Any(v => v.Name == obj.Name&&v.BusinessUnit==obj.BusinessUnit))
                throw new Exception(obj.Name+" already exist");
            db.Customers.Add(obj);
        }

        public void Update(Customer Obj)
        {
            Obj.Contacts = null;
            db.Entry(Obj).State = EntityState.Modified;
        }

        public SelectList GetSelectList(EBusinessUnit BU)
        {
            return new SelectList(GetAll(BU).OrderBy(v=>v.Name),"CustomerID","DisplayText");
        }
        public SelectList GetSelectListByAssignedJob(UserInfo UI)
        {
            var customers = job.GetAssignedJob(UI.User.UserID).Select(j=>j.Customer).Distinct();
            return new SelectList(customers.OrderBy(c=>c.Name),"CustomerID","DisplayText");
        }
        public SelectList GetSelectListByJob(UserInfo UI)
        {
            var customers = job.GetAllJob(UI).Select(j => j.Customer).Distinct();
            return new SelectList(customers.OrderBy(c => c.Name), "CustomerID", "DisplayText");
        }
        #endregion
        #region Contact
        public Contact GetContact(int ID)
        {
            return db.Contacts.Find(ID);
        }

        public IEnumerable<Contact> GetAllContact(int VendorID)
        {
            return db.Contacts.Where(vc=>vc.CustomerID==VendorID);
        }

        public void AddContact(Contact obj)
        {
            if (db.Contacts.Any(vc => vc.Mobile == obj.Mobile))
                throw new Exception("The contact already exist");
            db.Contacts.Add(obj);
        }

        public void UpdateContact(Contact Obj)
        {
            Obj.Customer = null;
            db.Entry(Obj).State = EntityState.Modified;
        }

        
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}