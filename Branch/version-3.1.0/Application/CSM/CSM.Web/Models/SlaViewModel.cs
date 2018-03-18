using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Resources;

namespace CSM.Web.Models
{
    public class SlaViewModel
    {
        public int SrChannelId { get; set; }
        public int SrStatusId { get; set; }
        public int SrStateId { get; set; }
        public List<SelectListItem> SrChannelList { get; set; }
        public List<SelectListItem> SrStatusList { get; set; }

        public IEnumerable<SlaItemEntity> SlaList { get; set; }
        public SlaSearchFilter SearchFilter { get; set; }
    }

    public class SlaCreateModel
    {
        [Display(Name = "Channel")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int SrChannelId { get; set; }

        [Display(Name = "SR Status")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? SrStatusId { get; set; }
        public int? SrStateId { get; set; }

        public List<SelectListItem> SrChannelList { get; set; }
        public List<SelectListItem> SrStatusList { get; set; }


        [Display(Name = "Product Group")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ProductGroupId { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ProductId { get; set; }

        public int? CampaignId { get; set; }

        [Display(Name="Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? AreaId { get; set; }

        public int? SubAreaId { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? TypeId { get; set; }

        [Display(Name = "SLA (Minute)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int? SlaMunite { get; set; }

        [Display(Name = "SLA (Times)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int? SlaTimes { get; set; }

        [Display(Name = "SLA (Days)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int? SlaDays { get; set; }

        [Display(Name = "SLA (ผบ.เบื้องต้น)")]
        public int? AlertChiefTimes { get; set; }

        [Display(Name = "SLA (ผบ.1)")]
        public int? AlertChief1Times { get; set; }

        [Display(Name = "SLA (ผบ.2)")]
        public int? AlertChief2Times { get; set; }

        [Display(Name = "SLA (ปธ.สาย)")]
        public int? AlertChief3Times { get; set; }
    }

    public class SlaEditModel
    {
        public int? SlaId { get; set; }

        [Display(Name = "Product Group")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        public int? CampaignServiceId { get; set; }
        public string CampaignServiceName { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int ChannelId { get; set; }

        [Display(Name = "SLA (Minute)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int SlaMinute { get; set; }

        [Display(Name = "SLA (Times)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int SlaTimes { get; set; }

        [Display(Name = "SLA (Days)")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int SlaDay { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public int CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public int UpdateUser { get; set; }
        public string UpdateUserName { get; set; }

        [Display(Name = "SR Channel")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int SrChannelId { get; set; }

        [Display(Name = "SR Status")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public int? SrStatusId { get; set; }
        public string SrStatusName { get; set; }
        public int? SrStateId { get; set; }
        public string SrStateName { get; set; }

        public List<SelectListItem> SrChannelList { get; set; }
        public List<SelectListItem> SrStatusList { get; set; }

        [Display(Name = "SLA (ผบ.เบื้องต้น)")]
        public int? AlertChiefTimes { get; set; }

        [Display(Name = "SLA (ผบ.1)")]
        public int? AlertChief1Times { get; set; }

        [Display(Name = "SLA (ผบ.2)")]
        public int? AlertChief2Times { get; set; }

        [Display(Name = "SLA (ปธ.สาย)")]
        public int? AlertChief3Times { get; set; }
    }

    public class SlaSaveModel
    {
        public int? SlaId { get; set; }
        public int ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int ChannelId { get; set; }
        public int SrStatusId { get; set; }
        public int SlaMinute { get; set; }
        public int SlaTimes { get; set; }
        public int SlaDay { get; set; }
        public int? AlertChiefTimes { get; set; }
        public int? AlertChief1Times { get; set; }
        public int? AlertChief2Times { get; set; }
        public int? AlertChief3Times { get; set; }
    }

    public class SlaDeleteModel
    {
        public int SlaId { get; set; }
    }
}