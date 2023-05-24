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
    public class SPServiceRepository:ISPServiceRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        public SPServiceRepository(SMSContext _db)
        {
            db = _db;
        }
        public SPServiceRepository()
            : this(new SMSContext())
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



        #region SAP Product Service
        public SPService Get(int ID)
        {
            return db.SPServices.Find(ID);
        }

        public IEnumerable<SPService> GetAll(EBusinessUnit BU)
        {
            return db.SPServices.Where(c => c.BU == BU);
        }

        public void Add(SPService obj, UserInfo UI)
        {
            if(obj.DocNo==""||obj.ItemNo==0)
            {
                throw new Exception("No Product Selected");
            }
            else if(obj.CustomerStatus==0)
            {
                throw new Exception("Customer Status Not Selected");
            }
            else if (obj.JobCategory == 0)
            {
                throw new Exception("Job Category Not Selected");
            }
            else if (obj.Details.Where(c=>c.IsDone==true).Count()==0)
            {
                throw new Exception("No work performed!");
            }
            else
            {
                var newservice = new SPService();
                newservice.BU = obj.BU;
                newservice.Cancel = false;
                newservice.CustomerStatus = obj.CustomerStatus;
                newservice.DocNo = obj.DocNo;
                newservice.EntryDateTime = DateTime.Now;
                newservice.HostName = UI.Host;
                newservice.ItemNo = obj.ItemNo;
                newservice.JobCategory = obj.JobCategory;
                newservice.JobCompletionDate = obj.JobCompletionDate;
                newservice.RunningHour = obj.RunningHour;
                newservice.ServicePersonID = obj.ServicePersonID;
                newservice.UserID = UI.User.UserID;
                db.SPServices.Add(newservice);

                //----------------------------add detail part--------------
                foreach(var det in obj.Details)
                {
                    var work = GetServiceWork(det.WorkID);
                    if(work==null)
                    {
                        throw new Exception("No work selected");
                    }
                    else if(work.DetailRequired==true&&det.IsDone==true&&(det.ServiceDetail??"")=="")
                    {
                        throw new Exception("Work Detail required for: "+work.WorkName);
                    }
                    else
                    {
                        var newdet = new SPServiceDetail();
                        newdet.ServiceDetail = (det.ServiceDetail??"");
                        newdet.ServiceID = newservice.ID;
                        newdet.Sl = det.Sl;
                        newdet.WorkID = det.WorkID;
                        newdet.IsDone = det.IsDone;
                        db.SPServiceDetails.Add(newdet);
                    }
                }

                //----------------------EQ Model,Rating,Serialupdate-----------------
                var oldprod = db.SAPProducts.Find(obj.DocNo, obj.ItemNo);
                if (oldprod != null)
                {
                    oldprod.EQModel = obj.SAPProduct.EQModel;
                    oldprod.EQSerial = obj.SAPProduct.EQSerial;
                    oldprod.Rating = obj.SAPProduct.Rating;
                }
            }
        }

        public void Update(SPService obj, UserInfo UI)
        {
            if (obj.DocNo == "" || obj.ItemNo == 0)
            {
                throw new Exception("No Product Selected");
            }
            else if (obj.CustomerStatus == 0)
            {
                throw new Exception("Customer Status Not Selected");
            }
            else if (obj.JobCategory == 0)
            {
                throw new Exception("Job Category Not Selected");
            }
            else if (obj.Details.Where(c => c.IsDone == true).Count() == 0)
            {
                throw new Exception("No work performed!");
            }
            else
            {
                var updservice = Get(obj.ID);
                updservice.CustomerStatus = obj.CustomerStatus;
                updservice.EntryDateTime = DateTime.Now;
                updservice.HostName = UI.Host;
                updservice.JobCategory = obj.JobCategory;
                updservice.JobCompletionDate = obj.JobCompletionDate;
                updservice.RunningHour = obj.RunningHour;

                //----------------------------update detail part--------------
                foreach (var det in obj.Details)
                {
                    var work = GetServiceWork(det.WorkID);
                    if (work == null)
                    {
                        throw new Exception("No work selected");
                    }
                    else if (work.DetailRequired == true && det.IsDone == true && (det.ServiceDetail ?? "") == "")
                    {
                        throw new Exception("Work Detail required for: " + work.WorkName);
                    }
                    else
                    {
                        var upddet = GetDetail(det.ServiceID,det.Sl);
                        upddet.ServiceDetail = det.ServiceDetail;
                        upddet.WorkID = det.WorkID;
                        upddet.IsDone = det.IsDone;
                    }
                }

                //----------------------EQ Model,Rating,Serialupdate-----------------
                var oldprod = db.SAPProducts.Find(obj.DocNo,obj.ItemNo);
                if(oldprod!=null)
                {
                    oldprod.EQModel = obj.SAPProduct.EQModel;
                    oldprod.EQSerial = obj.SAPProduct.EQSerial;
                    oldprod.Rating = obj.SAPProduct.Rating;
                }
                
            }
        }

        public void Cancel(int ID)
        {
            var obj = Get(ID);
            if(obj!=null)
            {
                obj.Cancel = true;
            }
        }
        #endregion
        #region Service Detail

        public SPServiceDetail GetDetail(int ServiceID,int Sl)
        {
            return db.SPServiceDetails.Find(ServiceID,Sl);
        }

        public IEnumerable<SPServiceDetail> GetAllDetail(int ServiceID)
        {
            return db.SPServiceDetails.Where(c=>c.ServiceID==ServiceID);
        }
        #endregion
        #region Service Work
        public ServiceWork GetServiceWork(int ID)
        {
            return db.ServiceWorks.Find(ID);
        }

        public IEnumerable<ServiceWork> GetAllServiceWork(EBusinessUnit BU)
        {
            return db.ServiceWorks.Where(c => c.BU == BU);
        }

        public SelectList SelectListSW(EBusinessUnit BU)
        {
            return new SelectList(GetAllServiceWork(BU), "ID", "WorkName");
        }
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}