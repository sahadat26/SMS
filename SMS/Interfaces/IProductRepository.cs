using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.Interfaces
{
    public interface IProductRepository:IDisposable
    {
        #region Product Category
        ProductCategory GetCategory(int ID);
        IEnumerable<ProductCategory> GetCategoryAll();
        void AddCategory(ProductCategory obj);
        void UpdateCategory(ProductCategory obj);
        SelectList GetCategorySelectList();
        #endregion

        #region Product
        Product GetProduct(int ID);
        IEnumerable<Product> GetProductAll(EBusinessUnit BU);
        void AddProduct(Product obj);
        void UpdateProduct(Product obj);
        ProductProperty GetProductProperty(int ProductPropertyID);
        IEnumerable<ProductProperty> GetProductPropertyByProduct(int ProductID);
        void AddProductProperty(ProductProperty obj, int ProductID);
        void ModifyProductProperty(ProductProperty obj);
        SelectList GetProductSelectList(EBusinessUnit BU);
        #endregion

        #region Property
        Property GetProperty(int ID);
        IEnumerable<Property> GetPropertyAll();
        void AddProperty(Property obj);
        void UpdateProperty(Property Obj);
        SelectList GetPropertySelectlist();
        #endregion

        #region SAP Product
        SAPProduct GetSapProduct(string DocNo,int ItemNo);
        SAPProduct GetSPBySerial(string Serial);
        IEnumerable<SAPProduct> GetSAPGenset();
        IEnumerable<SAPProduct> GetSAPProdAll();
        void AddSAPProduct(SAPProduct obj);

        Task<IEnumerable<SAPProduct>> SyncSAPProduct(string SalesOrg, DateTime fromdt, DateTime todt, string PC);
        #endregion

        #region SAP Product Group Category
        IEnumerable<ProductGrpCat> GetPGCAll(EBusinessUnit BU);
        ProductGrpCat GetPGC(string Grp);
        #endregion
        void Save();
    }
}
