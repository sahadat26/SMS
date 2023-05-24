using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SMS.Models;
namespace SMS.Models
{
    public class SMSContext:DbContext
    {
        #region Common
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<EmailText> EmailTexts { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }
        public DbSet<ServiceContract> ServiceContracts { get; set; }
        public DbSet<ServiceContractCollection> ServiceContractCollections { get; set; }
        #endregion
        #region Service Management
        public DbSet<Designation> Designations { get; set; }
        public DbSet<OrgSetting> OrgSetting { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductGrpCat> ProductGrpCats { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SAPProduct> SAPProducts { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ServiceProduct> ServiceProducts { get; set; }
        public DbSet<ServiceProductDetail> ServiceProductDetails { get; set; }
        public DbSet<WarrantyContract> WarrantyContracts { get; set; }
        public DbSet<WarrantyProduct> WarrantyProducts { get; set; }
        public DbSet<WarrantyProductDetail> WarrantyProductDetails { get; set; }
        public DbSet<RevenueWing> RevenueWings { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<ProductLine> ProductLines { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ManpowerCost> ManpowerCosts { get; set; }
        public DbSet<ServiceWork> ServiceWorks { get; set; }
        public DbSet<SPService> SPServices { get; set; }
        public DbSet<SPServiceDetail> SPServiceDetails { get; set; }
        #endregion
        #region ASE Commission
        public DbSet<SC_ConditionMatrix> ConditionMatrix { get; set; }
        public DbSet<SC_ASETarget> ASETarget { get; set; }
        public DbSet<SC_Sales> SC_Sales { get; set; }
        public DbSet<SC_Collection> SC_Collection { get; set; }
        public DbSet<SC_ASEContribution> SC_ASEContribution { get; set; }
        public DbSet<SC_ASEApprovedData> SC_ASEApprovedData { get; set; }
        public DbSet<setupCalender> setupCalender { get; set; }
        
        #endregion
        public SMSContext():base("name=SMS"){}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {    
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}
