using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class SC_ASEApprovedData:EntryInfo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("user")]
        public int ASEID { get; set; }

        [Column(TypeName = "Date")]
        public DateTime startDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime endDate { get; set; }

        public int duration { get; set; }

        public decimal spareSalesDuringPeriod { get; set; }
        public decimal spareSalesTarget { get; set; }
        public decimal spareTarget { get; set; }
        public decimal spareCollection { get; set; }
        public decimal spareCollectionPerCent { get; set; }
        public decimal spareCommision { get; set; }
        public decimal spareBonus { get; set; }
        public decimal spareAdditionalperCent { get; set; }
        public decimal spareAdditionalAmount { get; set; }
        public decimal spareTotalCommision { get; set; }
        public virtual User user { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return user.FullName;
            }
        }
        [NotMapped]
        public string UserName
        {
            get
            {
                return user.UserName;
            }
        }
        [NotMapped]
        public string Wing
        {
            get
            {
                return user.Wing;
            }
        }



        [NotMapped]
        public decimal spareTotalCommisionCalculation
        {
            get
            {
                return spareCommision + spareBonus + spareAdditionalAmount;
            }
        }



        public decimal lubeOilCollection { get; set; }
        public decimal lubeOilCommision { get; set; }
        public decimal lubeOilBonus { get; set; }


        public decimal serviceCollection { get; set; }

        public decimal serviceBonus { get; set; }

        public decimal serviceTDSVDSCollection { get; set; }
        public decimal serviceTDSVDSBonus { get; set; }


        public decimal csaExisting { get; set; }
        public decimal csaNew { get; set; }
        public decimal csaCommision { get; set; }

        public decimal totalCommision { get; set; }

        [NotMapped]
        public decimal totalCommisionCalculation
        {
            get
            {
                return Math.Round(spareTotalCommisionCalculation + lubeOilCommision + serviceBonus + csaCommision + serviceTDSVDSBonus, 2);
            }
        }

        public int month { get; set; }
        public int year { get; set; }



        public decimal BasicSalary { get; set; }

        public decimal ConveyanceAllowance { get; set; }

        public decimal FoodAllowance { get; set; }

        public decimal MobileAllowance { get; set; }

        public decimal AdditionalAllowance { get; set; }


        public decimal DeductionAllowance { get; set; }

        [NotMapped]
        public decimal totalBasicAllowance
        {
            get
            {
                return Math.Round((BasicSalary + ConveyanceAllowance + FoodAllowance + MobileAllowance + AdditionalAllowance) - DeductionAllowance, 2);
            }
        }

        [NotMapped]
        public decimal grandTotal {
            get
            {
                return totalCommisionCalculation + totalBasicAllowance;
            }
        }

       

        public int status { get; set; }

        [NotMapped]
        public string statusApp
        {
            get
            {
                return status == 1 ? "Supervisor Approved" : status == 2 ? "Product Manager Approved" : "HOD Approved";
            }
        }


        public DateTime? PMAppDate { get; set; }
        public int? PMUserId { get; set; }

        public DateTime? HODAppDate { get; set; }
        public int? HODUserId { get; set; }
    }
}