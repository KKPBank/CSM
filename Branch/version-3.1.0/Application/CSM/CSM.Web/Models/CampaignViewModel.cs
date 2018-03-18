using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Service.Messages.Campaign;

namespace CSM.Web.Models
{
    [Serializable]
    public class CampaignViewModel
    {
        [Display(Name = "Lbl_CustomerName", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string FirstName { get; set; }

        //[Display(Name = "Lbl_CustomerSurname", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string LastName { get; set; }

        [Display(Name = "Lbl_CardNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string CardNo { get; set; }

        [Display(Name = "Lbl_PhoneNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string PhoneNo { get; set; }

        //[Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        public string Email { get; set; }

        [Display(Name = "Lbl_Interested", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string Interested { get; set; }

        public string CampaignId { get; set; }
        public int? CustomerType { get; set; }
        public string CampaignName { get; set; }
        public SelectList CustomerTypeList { get; set; }
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public IEnumerable<CampaignDetail> CampaignList { get; set; }
        public IEnumerable<CampaignDetail> RecommendedCampaignList { get; set; }
        public string Comments { get; set; }
        public string OwnerLead { get; set; }
        public string OwnerBranch { get; set; }
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }

        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string AvailableTime { get; set; }

        public string ContractNoRefer { get; set; }
    }
}