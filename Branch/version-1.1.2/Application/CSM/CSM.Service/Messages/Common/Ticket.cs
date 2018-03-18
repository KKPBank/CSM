using System;
using System.Collections.Generic;
using CSM.Common.Utilities;

namespace CSM.Service.Messages.Common
{
    [Serializable]
    public class Ticket
    {
        public string TicketId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string StrResponseDate { get; set; }
        public string StrResponseTime { get; set; }
        public string TotalLeads { get; set; }
        public List<TicketItem> Items { get; set; }

        public DateTime? ResponseDateTime
        {
            get { return string.Format("{0} {1}", StrResponseDate, StrResponseTime).ParseDateTime("dd-MM-yyyy HH:mm:ss"); }
        }
    }

    [Serializable]
    public class TicketItem
    {
        public string TicketId { get; set; }
        public string CardNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelNo1 { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignDesc { get; set; }
        public string ProductGroupId { get; set; }
        public string ProductGroupDesc { get; set; }
        public string ProductId { get; set; }
        public string ProductDesc { get; set; }
        public string ChannelId { get; set; }
        public string OwnerLead { get; set; }
        public string OwnerLeadName { get; set; }
        public string DelegateLead { get; set; }
        public string DelegateLeadName { get; set; }
        public string CreatedUser { get; set; }
        public string CreatedUserName { get; set; }
        public string StrCreatedDate { get; set; }
        public string StrCreatedTime { get; set; }
        public string StrAssignedDate { get; set; }
        public string StrAssignedTime { get; set; }
        public string OwnerBranch { get; set; }
        public string OwnerBranchName { get; set; }
        public string DelegateBranch { get; set; }
        public string DelegateBranchName { get; set; }
        public string CreatedBranch { get; set; }
        public string CreatedBranchName { get; set; }

        public DateTime? CreatedDate
        {
            get { return string.Format("{0} {1}", StrCreatedDate, StrCreatedTime).ParseDateTime("yyyyMMdd HH:mm:ss"); }
        }
    }
}
