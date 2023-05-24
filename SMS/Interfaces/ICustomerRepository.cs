using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface ICustomerRepository:IDisposable
    {
        #region Customer
        Customer Get(int ID);
        IEnumerable<Customer> GetAll(EBusinessUnit PType);
        void Add(Customer obj);
        void Update(Customer Obj);
        SelectList GetSelectList(EBusinessUnit PType);
        SelectList GetSelectListByAssignedJob(UserInfo UI);
        SelectList GetSelectListByJob(UserInfo UI);
        #endregion
        #region Customer Contact
        Contact GetContact(int ID);
        IEnumerable<Contact> GetAllContact(int VendorID);
        void AddContact(Contact obj);
        void UpdateContact(Contact Obj);
        
        #endregion
        void Save();
    }
}
