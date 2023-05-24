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
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace SMS.Repositories
{
    public class ProductRepository:IProductRepository
    {
        #region Properties
        private SMSContext db { get; set; }
        
        public ProductRepository(SMSContext _db)
        {
            db = _db;
        }
        public ProductRepository()
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
        #region Product Category
        public ProductCategory GetCategory(int ID)
        {
            return db.ProductCategories.Find(ID);
        }

        public IEnumerable<ProductCategory> GetCategoryAll()
        {
            return db.ProductCategories;
        }

        public void AddCategory(ProductCategory obj)
        {
            if (db.ProductCategories.Any(a => a.CategoryName == obj.CategoryName))
                throw new Exception(obj.CategoryName + " already exist!");
            
            db.ProductCategories.Add(obj);
        }

        public void UpdateCategory(ProductCategory obj)

        {
            var oldone = GetCategory(obj.ProductCategoryID);
            if ((oldone.CategoryName != obj.CategoryName) && db.Products.Any(a => a.ProductName == obj.CategoryName))
                throw new Exception(obj.CategoryName + " already exist!");
            
            oldone.CategoryName = obj.CategoryName;
            oldone.ParentId = obj.ParentId;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.HostName = obj.HostName;
            oldone.UserID = obj.UserID;
        }
        

        public SelectList GetCategorySelectList()
        {
            return new SelectList(GetCategoryAll().OrderBy(a => a.CategoryName), "ProductCategoryID", "CategoryName");
        }
        
        #endregion
        #region Product
        public Product GetProduct(int ID)
        {
            return db.Products.Find(ID);
        }

        public IEnumerable<Product> GetProductAll(EBusinessUnit BU)
        {
            if(BU==EBusinessUnit.ALL)
                return db.Products;
            return db.Products.Where(c => c.BusinessUnit == BU);
        }

        public void AddProduct(Product obj)
        {
            if (db.Products.Any(a => a.ProductName == obj.ProductName))
                throw new Exception(obj.ProductName+" already exist!");
            else if (db.Products.Any(a => a.Prefix == obj.Prefix))
                throw new Exception("Prefix "+obj.Prefix + " used by another!");
            
            obj.Prefix = obj.Prefix.ToUpper();
            db.Products.Add(obj);
        }

        public void UpdateProduct(Product obj)
        {
            var oldone = GetProduct(obj.ProductID);
            if ((oldone.ProductName!=obj.ProductName)&& db.Products.Any(a => a.ProductName == obj.ProductName))
                throw new Exception(obj.ProductName + " already exist!");
            else if ((oldone.Prefix!=obj.Prefix)&&db.Products.Any(a => a.Prefix == obj.Prefix))
                throw new Exception("Prefix " + obj.Prefix + " used by another!");
            
            oldone.ProductName = obj.ProductName;
            oldone.EntryDateTime = obj.EntryDateTime;
            oldone.HostName = obj.HostName;
            if((obj.ImageURL??"")!="")
            {
                oldone.ImageURL = obj.ImageURL;
            }
            oldone.ProductCategoryID = obj.ProductCategoryID;
            oldone.Prefix = obj.Prefix.ToUpper();
            
            oldone.UserID = obj.UserID;
        }
        public ProductProperty GetProductProperty(int ProductID)
        {
            return db.ProductProperties.Find(ProductID);
        }

        public IEnumerable<ProductProperty> GetProductPropertyByProduct(int ProductID)
        {
            return db.ProductProperties.Where(p => p.ProductID==ProductID);
        }
        public void AddProductProperty(ProductProperty obj, int ProductID)
        {
            if(db.ProductProperties.Any(ac=>ac.ProductID==ProductID&&ac.PopertyID==obj.PopertyID))
            {
                throw new Exception("You have already added this property");
            }
            obj.ProductID = ProductID;
            obj.Sl = GetLatestSerialForProductProperty(ProductID);
            db.ProductProperties.Add(obj);
        }
        private int GetLatestSerialForProductProperty(int ProductID)
        {
            if(!db.ProductProperties.Any(ac=>ac.ProductID==ProductID))
            {
                return 1;
            }
            return db.ProductProperties.Where(ac => ac.ProductID == ProductID).Max(ac => ac.Sl)+1;
        }

        public void ModifyProductProperty(ProductProperty obj)
        {
            var oldone = GetProductProperty(obj.ProductPropertyID);
            if ((oldone.PopertyID!=obj.PopertyID)&&(db.ProductProperties.Any(ac => ac.ProductID == obj.ProductID && ac.PopertyID == obj.PopertyID)))
            {
                throw new Exception("You have already added this property");
            }
            oldone.IsRequired = obj.IsRequired;
            oldone.IsUnique = obj.IsUnique;
            oldone.PopertyID = obj.PopertyID;
        }

        public SelectList GetProductSelectList(EBusinessUnit BU)
        {
            return new SelectList(GetProductAll(BU).OrderBy(a=>a.ProductName),"ProductID","ProductName");
        }
        
        
        #endregion
        #region Property
        public Property GetProperty(int ID)
        {
            return db.Properties.Find(ID);
        }
        public IEnumerable<Property> GetPropertyAll()
        {
            return db.Properties;
        }
        public void AddProperty(Property obj)
        {
            if (db.Properties.Any(p => p.Name == obj.Name))
                throw new Exception(obj.Name+" already exist");
            db.Properties.Add(obj);
        }
        public void UpdateProperty(Property Obj)
        {
            var oldone = GetProperty(Obj.PropertyID);
            if (oldone.Name!=Obj.Name&& db.Properties.Any(p => p.Name == Obj.Name))
                throw new Exception(Obj.Name + " already exist");
            oldone.EntryDateTime = Obj.EntryDateTime;
            oldone.HostName = Obj.HostName;
            oldone.Name = Obj.Name;
            oldone.UserID = Obj.UserID;
        }
        public SelectList GetPropertySelectlist()
        {
            return new SelectList(GetPropertyAll().OrderBy(a => a.Name), "PropertyID", "Name");
        }
        #endregion

        #region SAP Product
        public SAPProduct GetSapProduct(string DocNo,int ItemNo)
        {
            return db.SAPProducts.Find(DocNo,ItemNo);
        }
        public IEnumerable<SAPProduct> GetSAPGenset()
        {
            return db.SAPProducts.Where(g => g.GRP == "FG15" || g.GRP == "FG25" || g.GRP == "FG08");
        }
        
        public IEnumerable<SAPProduct> GetSAPProdAll()
        {
            return db.SAPProducts;
        }
        public void AddSAPProduct(SAPProduct obj)
        {
            db.SAPProducts.Add(obj);
        }

        public async Task<IEnumerable<SAPProduct>> SyncSAPProduct(string SalesOrg, DateTime fromdt, DateTime todt, string PC)
        {
            IEnumerable<SAPProduct> products = new List<SAPProduct>();
            string uri = Common.BaseUrl + "get_product?sap-client=" + Common.Client + "&org=" + SalesOrg + "&fromdt=" + fromdt.ToString("yyyyMMdd") +
                "&todt=" + todt.ToString("yyyyMMdd");

            var Credential = new NetworkCredential(Common.UserID, Common.Password);
            var Handler = new HttpClientHandler { Credentials = Credential };
            HttpClient _client = new HttpClient(Handler);

            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<IEnumerable<SAPProduct>>(content);
            }
            return products;
        }
        public SAPProduct GetSPBySerial(string Serial)
        {
            return db.SAPProducts.Where(s => s.ENGINESERIAL == Serial).FirstOrDefault();
        }
        #endregion

        #region SAP Product Group Category
        public IEnumerable<ProductGrpCat> GetPGCAll(EBusinessUnit BU)
        {
            return db.ProductGrpCats.Where(b => b.BU == BU);
        }
        public ProductGrpCat GetPGC(string Grp)
        {
            return db.ProductGrpCats.Find(Grp);
        }
        #endregion
        public void Save()
        {
            db.SaveChanges();
        }
    }
}