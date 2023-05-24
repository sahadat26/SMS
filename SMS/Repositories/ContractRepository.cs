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
    public class ContractRepository:IContractRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        private UserRepository user { get; set; }
        
        public ContractRepository(SMSContext _db,UserRepository _user)
        {
            db = _db;
            user = _user;
        }
        public ContractRepository()
            : this(new SMSContext(),new UserRepository())
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

        #region Service Contract
        public ServiceContract GetSC(int ID)
        {
            return db.ServiceContracts.Find(ID);
        }
        public IEnumerable<ServiceContract> GetSCAll(UserInfo UI)
        {
            if (UI.BusinessUnit == EBusinessUnit.ALL)
                return db.ServiceContracts;
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.ServiceContracts
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        public void AddSC(ServiceContract obj)
        {
            if (obj.ContractStartDate > obj.ContractEndDate)
                throw new Exception("End Date must greater than start date");
            obj.Status=EStatus.Open;
            db.ServiceContracts.Add(obj);
        }
        public void UpdateSC(ServiceContract obj)
        {
            if (obj.ContractStartDate > obj.ContractEndDate)
                throw new Exception("End Date must greater than start date");
            var oldone = GetSC(obj.ServiceContractID);
            if((obj.AttachmentURL??"")!="")
            {
                oldone.AttachmentURL = obj.AttachmentURL;
            }
            oldone.BillingCycle = obj.BillingCycle;
            oldone.BusinessUnit = obj.BusinessUnit;
            oldone.ContactID = obj.ContactID;
            oldone.ContractEndDate = obj.ContractEndDate;
            oldone.ContractStartDate = obj.ContractStartDate;
            oldone.CustomerID = obj.CustomerID;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.HostName = obj.HostName;
            oldone.UserID = obj.UserID;
        }
        public SelectList SelectListSC(UserInfo UI)
        {
            return new SelectList(GetSCAll(UI), "ServiceContractID", "DisplayText");
        }
        public int ServiceRandFileName(int max, string ext)
        {
            Random rnd = new Random();
            int num = rnd.Next(max);
            string url = num + ext;
            while (db.ServiceContracts.Any(r => r.AttachmentURL == url))
            {
                num = rnd.Next(max);
                url = num + ext;
            }
            return num;
        }
        public void SCAutoClose()
        {
            var waitingforclose = db.ServiceContracts.Where(sc => sc.ContractEndDate < DateTime.Today&&sc.Status==EStatus.Open);
            foreach(var sc in waitingforclose)
            {
                sc.Status = EStatus.Close;
            }
            Save();
        }
        public void RenewContract(int SCID, DateTime Start, DateTime End)
        {
            var sc = GetSC(SCID);
            if((sc.ContractEndDate>Start)||(Start>=End))
            {
                throw new Exception("Date Range must equal or greater than previous Contract End Date");
            }
            sc.RevisionNo = sc.RevisionNo + 1;
            sc.ContractStartDate = Start;
            sc.ContractEndDate = End;
            sc.Status = EStatus.Open;
        }
        #endregion

        #region Service Product
        public ServiceProduct GetSP(int ID)
        {
            return db.ServiceProducts.Find(ID);
        }

        public IEnumerable<ServiceProduct> GetSPAll(int SCID)
        {
            return db.ServiceProducts.Where(c => c.ServiceContractID == SCID);
        }
        public ServiceProductDetail GetSPD(int ID)
        {
            return db.ServiceProductDetails.Find(ID);
        }
        public IEnumerable<ServiceProductDetail> GetSPDAll()
        {
            return db.ServiceProductDetails;
        }
        public List<string> GetSPPropertyValue(int ProductPropertyID)
        {
            return db.ServiceProductDetails.Where(s => s.ProductPropertyID == ProductPropertyID).
                Select(c => c.Value).Distinct().ToList();
        }
        public void AddSP(ServiceProduct obj)
        {
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
                    else if (p.IsUnique && db.ServiceProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
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
            db.ServiceProducts.Add(obj);
        }

        public void AddSPD(ServiceProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var productproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if (db.ServiceProductDetails.Any(a => a.ServiceProductID == property.ServiceProductID && a.ProductPropertyID == property.ProductPropertyID))
                throw new Exception("You have already added this property");
            else if (productproperty.IsRequired && (property.Value ?? "") == "")
            {
                throw new Exception("Value is required for " + productproperty.Property.Name);
            }
            else if (productproperty.IsUnique && db.ServiceProductDetails
                .Any(a => a.ProductPropertyID == property.ProductPropertyID && a.Value == property.Value))
            {
                throw new Exception("Value is unique for " + productproperty.Property.Name);
            }
            db.ServiceProductDetails.Add(property);
        }

        public void ModifySPD(ServiceProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var oldone = GetSPD(property.ServiceProductDetailID);
            
            var proproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if ((property.ProductPropertyID != oldone.ProductPropertyID)
                && db.ServiceProductDetails.Any(a => a.ServiceProductID == property.ServiceProductID && a.ProductPropertyID == property.ProductPropertyID))
                throw new Exception("You have already added this property");
            else if (proproperty.IsRequired && (property.Value ?? "") == "")
            {
                throw new Exception("Value is required for " + proproperty.Property.Name);
            }
            else if ((property.Value != oldone.Value) && proproperty.IsUnique && db.ServiceProductDetails
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

        public void UpdateSP(ServiceProduct obj)
        {
            var ser = GetSP(obj.ServiceProductID);
            if (db.ProductLines.Any(sp => sp.Job.ServiceContractID == ser.ServiceContractID
                && sp.ProductID == ser.ProductID))
                throw new Exception("Job card has been created with this product!");
            ser.ProductID = obj.ProductID;
            ser.EntryDateTime = obj.EntryDateTime;
            ser.HostName = obj.HostName;
            ser.UserID = obj.UserID;
            ser.ContractAmount = obj.ContractAmount;
            var pro = db.Products.Find(obj.ProductID);
            #region Set Properties
            if (pro.Properties.Count != obj.Details.Count)
            {
                throw new Exception("No Property Added");
            }
            else
            {
                foreach (var p in ser.Details)
                {
                    var d = obj.Details.Where(c => c.ProductPropertyID == p.ProductPropertyID).FirstOrDefault();
                    if (p.ProductProperty.IsRequired && (d.Value ?? "") == "")
                    {
                        throw new Exception("Value is required for " + p.ProductProperty.Property.Name);
                    }
                    else if (p.Value != d.Value && p.ProductProperty.IsUnique && db.ServiceProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
                    {
                        throw new Exception("Value is unique for " + p.ProductProperty.Property.Name);
                    }
                    p.Value = (d.Value??"").ToUpper();
                    p.EntryDateTime = obj.EntryDateTime;
                    p.HostName = obj.HostName;
                    p.UserID = obj.UserID;
                }
            }
            #endregion
        }

        

        
        #endregion

        #region Service Contract Collection
        public ServiceContractCollection GetCol(int ID)
        {
            return db.ServiceContractCollections.Find(ID);
        }
        public IEnumerable<ServiceContractCollection> GetColAll(int SCID)
        {
            return db.ServiceContractCollections.Where(c=>c.ServiceProduct.ServiceContractID==SCID);
        }
        public void AddCol(ServiceContractCollection obj)
        {
            if (db.ServiceContractCollections.Any(c => c.ServiceProductID == obj.ServiceProductID
                && c.CollectionDate == obj.CollectionDate))
                throw new Exception("The collection already made");
            db.ServiceContractCollections.Add(obj);
        }
        public void UpdateCol(ServiceContractCollection obj)
        {
            var oldone = GetCol(obj.ServiceContractCollectionID);
            if (oldone.CollectionDate!=obj.CollectionDate &&db.ServiceContractCollections.Any(c => c.ServiceProductID == obj.ServiceProductID
                && c.CollectionDate == obj.CollectionDate))
                throw new Exception("The collection already made");
            oldone.CollectedAmount = obj.CollectedAmount;
            oldone.CollectionDate = obj.CollectionDate;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.HostName = obj.HostName;
            oldone.UserID = obj.UserID;
        }

        #endregion

        #region Warranty Contract
        public WarrantyContract GetWC(int ID)
        {
            return db.WarrantyContracts.Find(ID);
        }
        public IEnumerable<WarrantyContract> GetWCAll(UserInfo UI)
        {
            if (UI.BusinessUnit == EBusinessUnit.ALL)
                return db.WarrantyContracts;
            var users = user.GetAllUser(UI.User.UserID, new List<int>());
            var list = from row in db.WarrantyContracts
                       join prow in users on row.UserID equals prow
                       select row;
            return list;
        }
        public void AddWC(WarrantyContract obj)
        {
            if (obj.WarrantyStart > obj.WarrantyEnd)
                throw new Exception("End Date must greater than start date");
            obj.Status = EStatus.Open;
            db.WarrantyContracts.Add(obj);
        }
        public void UpdateWC(WarrantyContract obj)
        {
            if (obj.WarrantyStart > obj.WarrantyEnd)
                throw new Exception("End Date must greater than start date");
            var oldone = GetWC(obj.WarrantyContractID);
            if ((obj.AttachmentURL ?? "") != "")
            {
                oldone.AttachmentURL = obj.AttachmentURL;
            }
            
            oldone.BusinessUnit = obj.BusinessUnit;
            oldone.ContactID = obj.ContactID;
            oldone.PurchaseDate = obj.PurchaseDate;
            oldone.InstallDate = obj.InstallDate;
            oldone.WarrantyEnd = obj.WarrantyEnd;
            oldone.WarrantyStart = obj.WarrantyStart;
            oldone.CustomerID = obj.CustomerID;
            oldone.ExpiredRunningHour = obj.ExpiredRunningHour;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.HostName = obj.HostName;
            oldone.UserID = obj.UserID;
        }
        public SelectList SelectListWC(UserInfo UI)
        {
            return new SelectList(GetWCAll(UI), "WarrantyContractID", "DisplayText");
        }
        public int WarrantyRandFileName(int max, string ext)
        {
            Random rnd = new Random();
            int num = rnd.Next(max);
            string url = num + ext;
            while (db.WarrantyContracts.Any(r => r.AttachmentURL == url))
            {
                num = rnd.Next(max);
                url = num + ext;
            }
            return num;
        }
        public void WCAutoClose()
        {
            var waitingforclose = db.WarrantyContracts.Where(wc => wc.WarrantyEnd < DateTime.Today && wc.Status == EStatus.Open);
            foreach (var wc in waitingforclose)
            {
                wc.Status = EStatus.Close;
            }
            Save();
        }
        #endregion

        #region Warranty Product
        public WarrantyProduct GetWP(int ID)
        {
            return db.WarrantyProducts.Find(ID);
        }

        public IEnumerable<WarrantyProduct> GetWPAll(int WCID)
        {
            return db.WarrantyProducts.Where(w => w.WarrantyContractID == WCID);
        }
        public WarrantyProductDetail GetWPD(int ID)
        {
            return db.WarrantyProductDetails.Find(ID);
        }
        public IEnumerable<WarrantyProductDetail> GetWPDAll()
        {
            return db.WarrantyProductDetails;
        }

        public void AddWP(WarrantyProduct obj)
        {
            
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
                    else if (p.IsUnique && db.WarrantyProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
                    {
                        throw new Exception("Value is unique for " + p.Property.Name);
                    }
                    d.Value = (d.Value ?? "").ToUpper();
                    d.EntryDateTime = obj.EntryDateTime;
                    d.HostName = obj.HostName;
                    d.UserID = obj.UserID;
                }
            }
            #endregion
            db.WarrantyProducts.Add(obj);
        }

        public void AddWPD(WarrantyProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var productproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if (db.WarrantyProductDetails.Any(a => a.WarrantyProductID == property.WarrantyProductID && a.ProductPropertyID == property.ProductPropertyID))
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
            db.WarrantyProductDetails.Add(property);
        }

        public void ModifyWPD(WarrantyProductDetail property)
        {
            property.Value = (property.Value ?? "").ToUpper();
            var oldone = GetSPD(property.WarrantyProductDetailID);
            var proproperty = db.ProductProperties.Find(property.ProductPropertyID);
            if ((property.ProductPropertyID != oldone.ProductPropertyID)
                && db.WarrantyProductDetails.Any(a => a.WarrantyProductID == property.WarrantyProductID && a.ProductPropertyID == property.ProductPropertyID))
                throw new Exception("You have already added this property");
            else if (proproperty.IsRequired && (property.Value ?? "") == "")
            {
                throw new Exception("Value is required for " + proproperty.Property.Name);
            }
            else if ((property.Value != oldone.Value) && proproperty.IsUnique && db.WarrantyProductDetails
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

        public void UpdateWP(WarrantyProduct obj)
        {
            var ser = GetWP(obj.WarrantyProductID);
            if (db.ProductLines.Any(sp => sp.Job.WarrantyContractID == ser.WarrantyContractID
                && sp.ProductID == ser.ProductID))
                throw new Exception("Job card has been created with this product!");
            ser.ProductID = obj.ProductID;
            ser.EntryDateTime = obj.EntryDateTime;
            ser.HostName = obj.HostName;
            ser.UserID = obj.UserID;
            ser.ServiceAmount = obj.ServiceAmount;
            var pro = db.Products.Find(obj.ProductID);
            #region Set Properties
            if (pro.Properties.Count != obj.Details.Count)
            {
                throw new Exception("No Property Added");
            }
            else
            {
                foreach (var p in ser.Details)
                {
                    var d = obj.Details.Where(c => c.ProductPropertyID == p.ProductPropertyID).FirstOrDefault();
                    if (p.ProductProperty.IsRequired && (d.Value ?? "") == "")
                    {
                        throw new Exception("Value is required for " + p.ProductProperty.Property.Name);
                    }
                    else if (p.Value != d.Value && p.ProductProperty.IsUnique && db.WarrantyProductDetails.Any(c => c.ProductPropertyID == d.ProductPropertyID && c.Value == d.Value))
                    {
                        throw new Exception("Value is unique for " + p.ProductProperty.Property.Name);
                    }
                    p.Value = (d.Value ?? "").ToUpper();
                    p.EntryDateTime = obj.EntryDateTime;
                    p.HostName = obj.HostName;
                    p.UserID = obj.UserID;
                }
            }
            #endregion
        }

        public List<string> GetWPPropertyValue(int ProductPropertyID)
        {
            return db.WarrantyProductDetails.Where(s => s.ProductPropertyID == ProductPropertyID).
                Select(c => c.Value).Distinct().ToList();
        }
        
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}