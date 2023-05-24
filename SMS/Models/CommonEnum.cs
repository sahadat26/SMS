using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public enum EAccomodationType
    {
        MetropolitanCity=1,OtherCity
    }

    public enum EDailyAllowanceType
    {
        NoVehicle=1,WithCompanyVehicle
    }
    public enum EActions
    {
        EmployeeAddRequest,ApproveEmployee
    }
    public enum EConditionType
    {
        Spare=1,Lube,Service,TDS_VDS,Contract
    }
    public enum ETargetType
    {
        Monthly=1,Yearly
    }
    public enum EReportBy
    {
        ShowDeliveryDate,ShowCommissioningDate
    }

    public enum EContractType
    {
        NA=1,New,Renew
    }
    public enum EDuration
    {
        Daily = 1, Weekly, Monthly, Quarterly,HalfYearly,Yearly
    }

    public enum EMonth
    {
        January= 1,February,March,April,May,June,July,Auguest,September,October,November,December  
    }

    public enum EUserType
    {
        Select = 0,Supervisor,ProductManager,HOD
    }


    public enum EStatus
    {
        Open=1,Close
    }

    public enum EState
    {
        Pending=1,Progress,Finished,Canceled
    }
    
    public enum EQueryFor
    {
        OnCall=1,ServiceContract,Warranty,OperationMaintenance
    }
    public enum EQueryType
    {
        Regular = 1, Complain, COMMISSIONING, OTHERS, RENTAL, OW, UW, CSA

    }
    public enum EJobCategory
    {
        ProjectSoution=1,Inspection_HealthCheckUp,Schedule_RoutineMaintenance,Troubleshooting,
        SiteAssesment_BOQ,RepairJob,OverhaulingTop,OverhaulingMajor
    }
    public enum EBusinessUnit
    {
        ALL=1,SnS,MVD,AMD
    }
    public enum EModule
    {
        Service_Management=1,ASE_Commission
    }
    public enum EFilterUser
    {
        ByLoggedUser,ByAssignedJob
    }
    
}