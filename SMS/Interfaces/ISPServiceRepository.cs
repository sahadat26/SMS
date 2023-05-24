using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface ISPServiceRepository:IDisposable
    {
        #region SP Service Crud
        SPService Get(int ID);
        IEnumerable<SPService> GetAll(EBusinessUnit BU);
        void Add(SPService obj,UserInfo UI);
        void Update(SPService Obj,UserInfo UI);
        void Cancel(int ID);
        
        
        #endregion
        #region SP Servie Detail
        SPServiceDetail GetDetail(int ServiceID,int Sl);
        IEnumerable<SPServiceDetail> GetAllDetail(int ServiceID);
        
        #endregion
        #region Service Work
        ServiceWork GetServiceWork(int ID);
        IEnumerable<ServiceWork> GetAllServiceWork(EBusinessUnit BU);
        SelectList SelectListSW(EBusinessUnit BU);
        #endregion
        void Save();
    }
}
