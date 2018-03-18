using System;
using System.Collections.Generic;
using CSM.Common.Utilities;
using CSM.Service.CARLogService;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Activity
{
    [Serializable]
    public class InquiryActivityLogResponse
    {
        public StatusResponse StatusResponse { get; set; }
        public List<ActivityDataItem> ActivityDataItems { get; set; }

        public InquiryActivityLogResponse()
        {
            this.StatusResponse = new StatusResponse();
        }
    }

    [Serializable]
    public class ActivityDataItem
    {
        public string SystemCode { get; set; }
        public DateTime? ActivityDateTime { get; set; }
        public decimal ActivityID { get; set; }
        public decimal ActivityTypeID { get; set; }
        public string ActivityTypeName { get; set; }
        public decimal AreaID { get; set; }
        public string AreaName { get; set; }
        public string CISID { get; set; }
        public string CampaignID { get; set; }
        public string CampaignName { get; set; }
        public string ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string ContractID { get; set; }
        public decimal TypeID { get; set; }
        public string TypeName { get; set; }
        public string TicketID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        public string SrID { get; set; }
        public decimal SubAreaID { get; set; }
        public string SubAreaName { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public string SubscriptionTypeName { get; set; }
        public string SubscriptionID { get; set; }
        public string LeadID { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public ContractInfo ContractInfo { get; set; }
        public ActivityInfo ActivityInfo { get; set; }
        public OfficerInfo OfficerInfo { get; set; }
        public ProductInfo ProductInfo { get; set; }
        public List<DataItem> CustomerInfoDataItems { get; set; }
        public List<DataItem> ContractInfoDataItems { get; set; }
        public List<DataItem> ActivityInfoDataItems { get; set; }
        public List<DataItem> OfficerInfoDataItems { get; set; }
        public List<DataItem> ProductInfoDataItems { get; set; }

        public string ActivityDateTimeDisplay
        {
            get { return this.ActivityDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string SrNoDisplay
        {
            get
            {
                return !string.IsNullOrEmpty(SrID) ? SrID : !string.IsNullOrEmpty(TicketID) ? TicketID : !string.IsNullOrEmpty(LeadID) ? LeadID : "N/A";
            }
        }

        public bool? Is_Secret { get; set; }
        public bool? IsNotSendCAR { get; set; }
        public short? SendCARStatus { get; set; }
    }

    [Serializable]
    public class ActivityInfo
    {
//        public string Detail { get; set; }
//        public string IsSendEmail { get; set; } // Y ; N
//        public string To { get; set; }
//        public string Subject { get; set; }
//        public string Note { get; set; }

        public string CreatorBranch { get; set; }
        public string CreatorSR { get; set; }
        public string OwnerBranch { get; set; }
        public string OwnerSR { get; set; }
        public string DelegateBranch { get; set; }
        public string DelegateSR { get; set; }
        public string SendEmail { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailAttachments { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityType { get; set; }
        public string SRState { get; set; }
        public string SRStatus { get; set; }
        public string Is_Secret { get; set; }
    }

    [Serializable]
    public class ContractInfo
    {
        public string AccountNo { get; set; }
        public string CreateSystem { get; set; }
        public string RegistrationNo { get; set; }
        public string FullName { get; set; }
    }

    [Serializable]
    public class CustomerInfo
    {
        public string SubscriptionID { get; set; }
        public string SubscriptionType { get; set; }
        public string FullName { get; set; }
    }

    [Serializable]
    public class OfficerInfo
    {
        public string FullName { get; set; }
    }

    [Serializable]
    public class ProductInfo
    {
        public string ProductGroup { get; set; }
        public string Product { get; set; }
        public string Campaign { get; set; }
    }
}
