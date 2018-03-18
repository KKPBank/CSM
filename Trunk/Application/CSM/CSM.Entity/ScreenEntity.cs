using System;
using System.Collections.Generic;

namespace CSM.Entity
{
    [Serializable]
    public class ScreenEntity
    {
        public int? MenuId { get; set; }
        public int ScreenId { get; set; }
        public string ScreenCode { get; set; }
        public string ScreenName { get; set; }
        public List<RoleEntity> Roles { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }

    public static class ScreenCode
    {
        public const string MainPage = "SC0202";

        public const string SearchCustomer = "SC0301";
        public const string NewCustomer = "SC0302";
        public const string EditCustomer = "SC0303";

        public const string ViewCustomerNote = "SC0304";
        public const string ViewCustomerProducts = "SC0305";
        public const string ViewCustomerLeads = "SC0306";
        public const string ViewCustomerSR = "SC0307";
        public const string ViewCustomerActivity = "SC0308";
        public const string ViewCustomerCampaigns = "SC0309";
        public const string ViewCustomerContact = "SC0310";
        public const string ViewCustomerDocument = "SC0311";
        public const string ViewCustomerLogging = "SC0312";

        public const string SearchCommPool = "SC0601";
        public const string ViewCommPool = "SC0602";

        public const string SearchNews = "SC0701";
        public const string ManageNews = "SC0702";

        public const string SearchConfigUrl = "SC0705";
        public const string ManageConfigUrl = "SC0706";

        public const string SearchNoteForCustomer = "SC0709";
        public const string ManageNoteForCustomer = "SC0710";

        public const string SearchContactRlat = "SC0707";
        public const string ManageContactRlat = "SC0708";

        public const string SearchAuditLog = "SC0750";

        public const string SearchSRStatus = "SC0713";
        public const string ManageSRStatus = "SC0714";

        public const string SearchConfigCommPool = "SC0751";
        public const string ManageConfigCommPool = "SC0758";

        public const string MainReport = "SC1001";
        public const string ReportNCB = "SC1003";
        public const string ReportSR = "SC1004";
        public const string ReportCommPool = "SC1006";
        public const string ReportVerify = "SC1007";
        public const string ReportVerifyDetail = "SC1008";
        public const string ReportComplaint = "SC1010";
    }
}
