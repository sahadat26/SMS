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
    public class JobRepository:IJobRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        private UserRepository user { get; set; }
        private ContractRepository contract { get; set; }
        
        public JobRepository(SMSContext _db,UserRepository _user,
            ContractRepository _contract)
        {
            db = _db;
            user = _user;
            contract = _contract;
        }
        public JobRepository()
            : this(new SMSContext(),new UserRepository(),new ContractRepository())
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

        #region Job
        public Job Get(int ID)
        {
            return db.Jobs.Find(ID);
        }

        public IEnumerable<Job> GetAllQuery(UserInfo UI)
        {
            if(UI.User.BusinessUnit==EBusinessUnit.ALL)
            {
                if (UI.BusinessUnit == EBusinessUnit.ALL)
                {
                    return db.Jobs.Where(j => j.State != EState.Canceled);
                }
                else
                {
                    return db.Jobs.Where(j => j.State != EState.Canceled&&j.BusinessUnit==UI.BusinessUnit);
                }
                    
            }
            
            
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.Jobs.Where(j => j.State != EState.Canceled)
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        public IEnumerable<Job> GetCanceledJob(UserInfo UI)
        {
            if (UI.BusinessUnit == EBusinessUnit.ALL)
                return db.Jobs.Where(j => j.State == EState.Canceled);
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.Jobs.Where(j => j.State == EState.Canceled)
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        
        public IEnumerable<Job> GetAssignedJob(int UID)
        {
            var UI = db.Users.Find(UID);
            if (UI.BusinessUnit == EBusinessUnit.ALL)
                return db.Jobs.Where(j => j.State != EState.Canceled);
            var users = user.GetAllUser(UI.UserID, new List<int>());
            var list = from row in db.ManpowerCosts
                       join prow in users on row.EmployeeID equals prow
                       join nrow in db.Jobs.Where(j => j.State != EState.Canceled) on row.JobID equals nrow.JobID
                       select nrow;
            return list;
        }
        public IEnumerable<Job> GetAllJob(UserInfo UI)
        {
            if(UI.User.BusinessUnit==EBusinessUnit.ALL)
            {
                if (UI.BusinessUnit == EBusinessUnit.ALL)
                    return db.Jobs.Where(r => r.JobCardCreateDate != null).Where(j => j.State != EState.Canceled);
                else
                    return db.Jobs.Where(r => r.JobCardCreateDate != null).Where(j => j.State != EState.Canceled&&j.BusinessUnit==UI.BusinessUnit);
            }
            
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.Jobs.Where(r => r.JobCardCreateDate != null).Where(j => j.State != EState.Canceled)
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        public void AddQuery(Job obj)
        {
            if (db.Jobs.Where(j => j.State != EState.Canceled).Any(r => r.QueryDate == obj.QueryDate && r.CustomerID == obj.CustomerID))
            {
                throw new Exception("Already the Query recorded!");
            }
            else if((obj.QueryFor==EQueryFor.ServiceContract)&&obj.ServiceContractID==null)
            {
                throw new Exception("No Service Contract Selected");
            }
            else if ((obj.QueryFor == EQueryFor.Warranty) && obj.WarrantyContractID == null)
            {
                throw new Exception("No Warranty Contract Selected");
            }
            obj.Status = EStatus.Open;
            obj.State = EState.Pending;
            db.Jobs.Add(obj);
        }

        public void CancelQuery(int JID, string Reason)
        {
            var oldone = Get(JID);
            if(oldone.State==EState.Pending&&oldone.Status==EStatus.Open)
            {
                if(Reason=="")
                {
                    throw new Exception("Cancel reason is required");
                }
                if(oldone.Products.Count>0)
                {
                    throw new Exception("Some product have been added");
                }
                oldone.Status = EStatus.Close;
                oldone.State = EState.Canceled;
                oldone.LostReason = Reason;
            }
            else
            {
                throw new Exception("The Query is not cancelable");
            }
        }
        public void RestoreQuery(int JID)
        {
            var oldone = Get(JID);
            if(oldone.State!=EState.Canceled)
            {
                throw new Exception("The query is not canceled");
            }
            oldone.State = EState.Pending;
            oldone.Status = EStatus.Open;
            oldone.LostReason = "";
        }
        public void UpdateQuery(Job obj)
        {
            var oldone = Get(obj.JobID);
            if (oldone.Status == EStatus.Close)
            {
                throw new Exception("The query already closed");
            }
            else if (((oldone.QueryDate != obj.QueryDate) || (oldone.CustomerID != obj.CustomerID)) && db.Jobs.Where(j => j.State != EState.Canceled).Any(r => r.QueryDate == obj.QueryDate && r.CustomerID == obj.CustomerID))
            {
                throw new Exception("Already the Query recorded!");
            }
            else if ((obj.QueryFor == EQueryFor.ServiceContract) && obj.ServiceContractID == null)
            {
                throw new Exception("No Service Contract Selected");
            }
            else if ((obj.QueryFor == EQueryFor.Warranty) && obj.WarrantyContractID == null)
            {
                throw new Exception("No Warranty Contract Selected");
            }
            oldone.QueryDescription = obj.QueryDescription;
            oldone.QueryDate = obj.QueryDate;
            oldone.ContactID = obj.ContactID;
            oldone.CustomerID = obj.CustomerID;
            oldone.ServiceContractID = obj.ServiceContractID;
            oldone.WarrantyContractID = obj.WarrantyContractID;
        }
        public void SaveJob(Job obj)
        {
            var oldone = Get(obj.JobID);
            if (oldone.Status == EStatus.Close)
            {
                throw new Exception("The query already closed");
            }
            else if (oldone.Products.Count==0)
            {
                throw new Exception("No Product Added");
            }
            else if (((oldone.JobCardCreateDate != obj.JobCardCreateDate) || (oldone.CustomerID != obj.CustomerID)) && db.Jobs.Where(j => j.State != EState.Canceled).Any(r => r.JobCardCreateDate == obj.JobCardCreateDate && r.CustomerID == obj.CustomerID))
            {
                throw new Exception("Already the Job recorded!");
            }
            else if(obj.JobCardCreateDate==null)
            {
                throw new Exception("Create Date is Required");
            }
            else if ((obj.JobTitle??"").Length<3)
            {
                throw new Exception("Job Title is Required");
            }
            else if ((obj.WingID??0)==0)
            {
                throw new Exception("Revenue Wing Required");
            }
            else if (obj.JobCardCreateDate < oldone.QueryDate)
            {
                throw new Exception("Job Create DateTime not smaller than Query Date");
            }
            else
            {
                if ((obj.AttachmentURL ?? "") != "")
                {
                    oldone.AttachmentURL = obj.AttachmentURL;
                }
                oldone.State = EState.Progress;
                oldone.JobCardCreateDate = obj.JobCardCreateDate;
                oldone.JobTitle = obj.JobTitle;
                oldone.ServiceCharge = obj.ServiceCharge;
                oldone.WorkshopCharge = obj.WorkshopCharge;
                oldone.JobNo = obj.JobNo;
                oldone.BillNo = obj.BillNo;
                oldone.WingID = obj.WingID;
                oldone.IsTour = obj.IsTour;
            }
            
        }
        public void LostQuery(int JID,string Reason)
        {
            var oldone = Get(JID);
            if (oldone.JobCardCreateDate != null)
                throw new Exception("Job card created for this query!");
            else if ((Reason ?? "") == "")
                throw new Exception("Reason of lost is required");
            oldone.LostReason = Reason;
            oldone.Status = EStatus.Close;
            oldone.State = EState.Canceled;
        }
        public void FinishJob(int JID, DateTime FinishDate)
        {
            var oldone = Get(JID);
            if (FinishDate < oldone.JobCardCreateDate)
                throw new Exception("Finish Date must greate or equal Job Card Create Date");
            else if(oldone.ManpowerCosts.Count==0)
            {
                throw new Exception("No Manpower Cost Added!");
            }
            else if (oldone.Products.Count == 0)
            {
                throw new Exception("No Product Added!");
            }
            oldone.State = EState.Finished;
            oldone.Status = EStatus.Close;
            oldone.JobFinishedDate = FinishDate;
        }
        public int RandFileName(int max,string ext)
        {
            Random rnd = new Random();
            int num = rnd.Next(max);
            string url=num+ext;
            while (db.Jobs.Where(j => j.State != EState.Canceled).Any(r => r.AttachmentURL == url))
            {
                num = rnd.Next(max);
                url = num + ext;
            }
            return num;
        }
        public List<string> GetJobTitles(EBusinessUnit BU)
        {
            var lst = db.Jobs.Where(j => j.State != EState.Canceled).Select(j => j.JobTitle).Distinct();
            return lst.ToList();
        }
        #endregion
        #region Product Line
        public ProductLine GetPL(int ID)
        {
            return db.ProductLines.Find(ID);
        }

        public IEnumerable<ProductLine> GetPLAll(UserInfo UI)
        {
            if(UI.BusinessUnit==EBusinessUnit.ALL)
                return db.ProductLines;
            
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.ProductLines
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }

        public IEnumerable<ProductLine> GetPLByJob(int JobID)
        {
            return db.ProductLines.Where(pl => pl.JobID == JobID);
        }
        public ProductDetail GetPLDetail(int ID)
        {
            return db.ProductDetails.Find(ID);
        }
        public IEnumerable<ProductDetail> GetPLDetails()
        {
            return db.ProductDetails;
        }
        public List<string> GetPropertyValue(int ProductPropertyID)
        {
            return db.ProductDetails.Where(s => s.ProductPropertyID == ProductPropertyID).
                Select(c => c.Value).Distinct().ToList();
        }
        public void AddPL(ProductLine obj)
        {
            var job = Get(obj.JobID);
            //if (job.State>EState.Pending)
            //{
            //    throw new Exception("The Job Card already created");
            //}
            if (job.QueryFor == EQueryFor.OnCall && obj.ProductID == 0)
                throw new Exception("No product selected");
            
            var pro = db.Products.Find(obj.ProductID);
            #region Set Properties
            if (pro.Properties.Count != obj.Details.Count)
            {
                throw new Exception("No Property Added");
            }
            else
            {
                foreach (var p in pro.Properties)
                {
                    var d = obj.Details.Where(c => c.ProductPropertyID == p.ProductPropertyID).FirstOrDefault();
                    if (p.IsRequired && (d.Value ?? "") == "")
                    {
                        throw new Exception("Value is required for " + p.Property.Name);
                    }
                    else if (p.IsUnique && db.ProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
                    {
                        throw new Exception("Value is unique for " + p.Property.Name);
                    }
                    d.Value = (d.Value??"").ToUpper();
                    d.EntryDateTime = obj.EntryDateTime;
                    d.HostName = obj.HostName;
                    d.UserID = obj.UserID;
                }
            }
            #endregion
            db.ProductLines.Add(obj);
        }

        public void AddPLFromService(int SPID, int JobID, int UserID, string HostName)
        {
            var job = Get(JobID);
            //if (job.State > EState.Pending)
            //{
            //    throw new Exception("The Job Card already created");
            //}
            var sp = contract.GetSP(SPID);
            if(sp==null)
            {
                throw new Exception("No product selected");
            }
            var pl = new ProductLine
            {
                EntryDateTime=DateTime.Now,
                HostName=HostName,
                JobID=JobID,
                ProductID=sp.ProductID,
                UserID=UserID,
                ServiceProductID=SPID
            };
            foreach(var p in sp.Details)
            {
                var d = new ProductDetail();
                d.EntryDateTime = DateTime.Now;
                d.HostName = HostName;
                d.ProductPropertyID = p.ProductPropertyID;
                d.UserID = UserID;
                d.Value=(p.Value??"").ToUpper();
                pl.Details.Add(d);
            }
            db.ProductLines.Add(pl);
        }
        public void AddPLFromWarranty(int WPID, int JobID, int UserID, string HostName)
        {
            var job = Get(JobID);
            //if (job.State > EState.Pending)
            //{
            //    throw new Exception("The Job Card already created");
            //}
            var wp = contract.GetWP(WPID);
            if (wp == null)
            {
                throw new Exception("No product selected");
            }
            var pl = new ProductLine
            {
                EntryDateTime = DateTime.Now,
                HostName = HostName,
                JobID = JobID,
                ProductID = wp.ProductID,
                UserID = UserID,
                WarrantyProductID=WPID
            };
            foreach (var p in wp.Details)
            {
                var d = new ProductDetail();
                d.EntryDateTime = DateTime.Now;
                d.HostName = HostName;
                d.ProductPropertyID = p.ProductPropertyID;
                d.UserID = UserID;
                d.Value = (p.Value ?? "").ToUpper();
                pl.Details.Add(d);
            }
            db.ProductLines.Add(pl);
        }
        public void AddPLDetail(ProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var productproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if (db.ProductDetails.Any(a => a.ProductLineID == property.ProductLineID && a.ProductPropertyID == property.ProductPropertyID))
                throw new Exception("You have already added this property");
            else if (productproperty.IsRequired && (property.Value ?? "") == "")
            {
                throw new Exception("Value is required for " + productproperty.Property.Name);
            }
            else if (productproperty.IsUnique && db.ProductDetails
                .Any(a => a.ProductPropertyID == property.ProductPropertyID && a.Value == property.Value))
            {
                throw new Exception("Value is unique for " + productproperty.Property.Name);
            }
            db.ProductDetails.Add(property);
        }

        public void ModifyPLDetail(ProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var oldone = GetPLDetail(property.ProductDetailID);
            var proproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if ((property.ProductPropertyID != oldone.ProductPropertyID)
                && db.ProductDetails.Any(a => a.ProductLineID == property.ProductLineID && a.ProductPropertyID == property.ProductPropertyID))
                throw new Exception("You have already added this property");
            else if (proproperty.IsRequired && (property.Value ?? "") == "")
            {
                throw new Exception("Value is required for " + proproperty.Property.Name);
            }
            else if ((property.Value != oldone.Value) && proproperty.IsUnique && db.ProductDetails
                .Any(a => a.ProductPropertyID == property.ProductPropertyID && a.Value == property.Value))
            {
                throw new Exception("Value is unique for " + proproperty.Property.Name);
            }
            oldone.Value = property.Value;
            oldone.ProductPropertyID = property.ProductPropertyID;
            oldone.EntryDateTime = property.EntryDateTime;
            oldone.HostName = property.HostName;
            oldone.UserID = property.UserID;
            oldone.Value = property.Value;

        }

        public void UpdatePL(ProductLine obj)
        {
            var job = Get(obj.JobID);
            if (job.Status == EStatus.Close)
            {
                throw new Exception("The query already closed");
            }
            else if (job.QueryFor == EQueryFor.OnCall && obj.ProductID == 0)
                throw new Exception("No product selected");
            
            var pl = GetPL(obj.ProductLineID);
            
            pl.ProductID = obj.ProductID;
            var pro = db.Products.Find(obj.ProductID);
            #region Set Properties
            if (pro.Properties.Count != obj.Details.Count)
            {
                throw new Exception("No Property Added");
            }
            else
            {
                foreach (var p in pl.Details)
                {
                    var d = obj.Details.Where(c => c.ProductPropertyID == p.ProductPropertyID).FirstOrDefault();
                    if (p.ProductProperty.IsRequired && (d.Value ?? "") == "")
                    {
                        throw new Exception("Value is required for " + p.ProductProperty.Property.Name);
                    }
                    else if (p.Value != d.Value && p.ProductProperty.IsUnique && db.ProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
                    {
                        throw new Exception("Value is unique for " + p.ProductProperty.Property.Name);
                    }
                    p.Value = d.Value.ToUpper();
                    p.EntryDateTime = obj.EntryDateTime;
                    p.HostName = obj.HostName;
                    p.UserID = obj.UserID;
                }
            }
            #endregion
        }
        public SelectList GetPLSelectList(UserInfo UI)
        {
            return new SelectList(GetPLAll(UI), "ProductLineID", "DisplayText");
        }
        public void DeletePL(int PLID)
        {
            var pl = GetPL(PLID);
            if(pl==null)
            {
                throw new Exception("No Product Found");
            }
            //else if (pl.Job.State > EState.Pending)
            //{
            //    throw new Exception("The Job Card already created");
            //}
            db.ProductLines.Remove(pl);
        }
        #endregion
        #region ManPower Cost
        public ManpowerCost GetMC(int ID)
        {
            return db.ManpowerCosts.Find(ID);
        }

        public IEnumerable<ManpowerCost> GetMCAll(UserInfo UI)
        {
            if(UI.BusinessUnit==EBusinessUnit.ALL)
                return db.ManpowerCosts;
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.ManpowerCosts
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        public IEnumerable<ManpowerCost> GetMCByJob(int JID)
        {
            return db.ManpowerCosts.Where(mc=>mc.JobID==JID);
        }
        public void AddMC(ManpowerCost obj,int DID)
        {
            var job = Get(obj.JobID);
            if(job.Status==EStatus.Close)
            {
                throw new Exception("The Job already closed");
            }
            //else if (db.ManpowerCosts.Any(m => m.JobID == obj.JobID && m.EmployeeID == obj.EmployeeID))
            //    throw new Exception("Given employee already added");
            else if (obj.StartTime < job.JobCardCreateDate)
            {
                throw new Exception("Start DateTime not smaller than Job Create Date");
            }
            else if(obj.StartTime>obj.EndTime)
            {
                throw new Exception("Start DateTime not greater than End DateTime");
            }
            var des = db.Designations.Find(DID);
            obj.FoodAllowance = des.FoodAllowance;
            obj.HolidayHourRate = des.HolidayHourRate;
            obj.OTHourRate = des.OTHourRate;
            obj.WorkingHourRate = des.WorkingHourRate;
            if(obj.DailyAllowanceType==EDailyAllowanceType.NoVehicle)
            {
                obj.DailyAllowance = des.DailyAllowance;
            }
            else if (obj.DailyAllowanceType == EDailyAllowanceType.WithCompanyVehicle)
            {
                obj.DailyAllowance = des.DailyAllowanceCV;
            }
            if (obj.AccomodationType == EAccomodationType.MetropolitanCity)
            {
                obj.Accomodation = des.MCAccomodation;
            }
            else if (obj.AccomodationType == EAccomodationType.OtherCity)
            {
                obj.Accomodation = des.OCAccomodation;
            }
            db.ManpowerCosts.Add(obj);
        }

        public void UpdateMC(ManpowerCost obj,int DID)
        {
            var oldone = GetMC(obj.ManpowerCostID);
            if (oldone.Job.Status == EStatus.Close)
            {
                throw new Exception("The Job already closed");
            }
            else if (oldone.EmployeeID!=obj.EmployeeID&& db.ManpowerCosts.Any(m => m.JobID == obj.JobID && m.EmployeeID == obj.EmployeeID))
                throw new Exception("Given employee already added");
            var des = db.Designations.Find(DID);
            oldone.EmployeeID = obj.EmployeeID;
            oldone.EndTime = obj.EndTime;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.FoodAllowance = des.FoodAllowance;
            oldone.HolidayHour = obj.HolidayHour;
            oldone.HolidayHourRate = des.HolidayHourRate;
            oldone.HostName = obj.HostName;
            oldone.NumberOfMeal = obj.NumberOfMeal;
            oldone.OTHour = obj.OTHour;
            oldone.TravelHour = obj.TravelHour;
            oldone.OTHourRate = des.OTHourRate;
            oldone.StartTime = obj.StartTime;
            oldone.TransportCost = obj.TransportCost;
            oldone.UserID = obj.UserID;
            oldone.WorkDescription = obj.WorkDescription;
            oldone.WorkingHour = obj.WorkingHour;
            oldone.WorkingHourRate = des.WorkingHourRate;
            oldone.OtherCost = obj.OtherCost;
        }

        public void DeleteMC(int MCID)
        {
            var oldone = GetMC(MCID);
            if (oldone.Job.Status == EStatus.Close)
            {
                throw new Exception("The Job already closed");
            }
            db.ManpowerCosts.Remove(oldone);
        }

        public SelectList GetMCSelectList(UserInfo UI)
        {
            return new SelectList(GetMCAll(UI), "ManpowerCostID", "Employee.DisplayText");
        }


        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}