using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IJobRepository:IDisposable
    {
        #region Job and Query
        Job Get(int ID);
        IEnumerable<Job> GetAllQuery(UserInfo UI);
        IEnumerable<Job> GetAllJob(UserInfo UI);
        IEnumerable<Job> GetCanceledJob(UserInfo UI);
        IEnumerable<Job> GetAssignedJob(int UID);
        void AddQuery(Job obj);
        void UpdateQuery(Job Obj);
        void SaveJob(Job Obj);
        void CancelQuery(int JID,string Reason);
        void RestoreQuery(int JID);
        void LostQuery(int JID,string Reason);
        void FinishJob(int JID,DateTime FinishDate);
        int RandFileName(int max,string ext);
        List<string> GetJobTitles(EBusinessUnit BU);
        #endregion
        
        #region Product Line
        ProductLine GetPL(int ID);
        IEnumerable<ProductLine> GetPLAll(UserInfo UI);
        IEnumerable<ProductLine> GetPLByJob(int JobID);
        void AddPL(ProductLine obj);
        ProductDetail GetPLDetail(int ID);
        IEnumerable<ProductDetail> GetPLDetails();
        List<string> GetPropertyValue(int ProductPropertyID);
        void AddPLDetail(ProductDetail property);
        void AddPLFromService(int SPID,int JobID,int UserID,string HostName);
        void AddPLFromWarranty(int WPID, int JobID, int UserID, string HostName);
        void ModifyPLDetail(ProductDetail property);
        
        void UpdatePL(ProductLine obj);
        void DeletePL(int PLID);
        SelectList GetPLSelectList(UserInfo UI);
        #endregion

        #region Man Power Cost
        ManpowerCost GetMC(int ID);
        IEnumerable<ManpowerCost> GetMCAll(UserInfo UI);
        IEnumerable<ManpowerCost> GetMCByJob(int JID);
        void AddMC(ManpowerCost obj,int DID);
        void UpdateMC(ManpowerCost obj,int DID);
        void DeleteMC(int MCID);
        SelectList GetMCSelectList(UserInfo UI);
        #endregion
        void Save();
    }
}
