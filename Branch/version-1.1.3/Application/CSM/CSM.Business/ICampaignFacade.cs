using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.Campaign;
using CSM.Service.Messages.Common;
using System;

namespace CSM.Business
{
    public interface ICampaignFacade : IDisposable
    {
        List<CampaignDetail> GetCampaignListByCustomer(AuditLogEntity auditLog, string cardNo, string hasOffered,
            string isInterested, string customerFlag, int campaignNum);
        UpdateCampaignFlagsResponse SaveCampaignFlags(AuditLogEntity auditLog, string cardNo, CampaignSearchFilter searchFilter);
        Ticket CreateLead(AuditLogEntity auditLog, CampaignSearchFilter searchFilter);
    }
}
