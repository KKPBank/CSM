using CSM.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CSM.Common.Resources;

namespace CSM.Web.Models
{
    public class MappingProductTypeViewModel
    {
        public List<SelectListItem> SrPageList { get; set; }
        public IEnumerable<MappingProductTypeItemEntity> MappingProductList { get; set; }
        public MappingProductSearchFilter SearchFilter { get; set; }
        public List<SelectListItem> VerifyList { get; set; } 
        public List<SelectListItem> ActiveList { get; set; }

        public QuestionSelectSearchFilter QuestionGroupSearchFilter { get; set; }
        public IEnumerable<QuestionGroupTableItemEntity> QuestionGroupList { get; set; }
        
        public int? MapProductId { get; set; }
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

        [Display(Name = "Sub Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? TypeId { get; set; }
        public string TypeName { get; set; }

        public int? OwnerBranchId { get; set; }
        public string OwnerBranchName { get; set; }

        public int? OwnerSrId { get; set; }
        public string OwnerSrName { get; set; }
        public bool IsVerify { get; set; }
        public bool IsActive { get; set; }
        public int SrPageId { get; set; }
        public string SrPageName { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }

        public bool IsVerifyOTP { get; set; }
        public bool IsSRSecret { get; set; }
        public OTPTemplateEntity OTPTemplate { get; set; } = new OTPTemplateEntity();
        public HpStatusEntity HpStatus { get; set; } = new HpStatusEntity();
    }

    public class MappingProductTypeEditModel
    {
        public int? MapProductId { get; set; }

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

        [Display(Name = "Sub Area")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int? OwnerBranchId { get; set; }
        public string OwnerBranchName { get; set; }
        public int? OwnerSrId { get; set; }
        public string OwnerSrName { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public List<SelectListItem> SrPageList { get; set; }
        public List<SelectListItem> VerifyList { get; set; }
        public List<SelectListItem> ActiveList { get; set; }
        public bool IsVerify { get; set; }
        public bool IsActive { get; set; }
        public int SrPageId { get; set; }
        public QuestionGroupEditSearchFilter SearchFilter { get; set; }
        public IEnumerable<QuestionGroupEditTableItemEntity> QuestionGroupList { get; set; }

        public QuestionSelectSearchFilter QuestionGroupSearchFilter { get; set; }
        public IEnumerable<QuestionGroupTableItemEntity> QuestionGroupModalList { get; set; }
		public string QuestionGroupListJson { get; set; }

        public bool IsVerifyOTP { get; set; }
        public bool IsSRSecret { get; set; }
        public OTPTemplateEntity OTPTemplate { get; set; } = new OTPTemplateEntity();
        public HpStatusEntity HpStatus { get; set; } = new HpStatusEntity();
    }

    public class MappingProductTypeSaveModel
    {
        public int? MapProductId { get; set; }
        public int ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int? OwnerUserId { get; set; }
        public int? OwnerSrId { get; set; }
        public int SrPageId { get; set; }
        public bool IsVerify { get; set; }
        public bool IsActive { get; set; }
        public string QuestionGroupList { get; set; }

        public bool IsVerifyOTP { get; set; }
        public bool IsSRSecret { get; set; }
        public int? OTPTemplateId { get; set; }
        public int? HpStatusId { get; set; }
        public string HpLangIndeCode { get; set; }
        public string HpSubject { get; set; }
    }

    public class ProductQuestionGroupListSaveModel
    {
        public int id { get; set; }
        public int pass_value { get; set; }
        public int seq { get; set; }
    }

    public class TableViewModel
    {
        public QuestionSelectSearchFilter QuestionGroupSearchFilter { get; set; } 
        public List<QuestionGroupTableViewModel> QuestionGroupTableList { get; set; }
        public IEnumerable<QuestionGroupEditTableItemEntity> QuestionGroupTableEditList { get; set; }
    }

    public class QuestionGroupTableViewModel
    {
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public string QuestionGroupNo { get; set; }
        public int? QuestionGroupPassAmount { get; set; }

        public int QuestionSeq { get; set; }
    }

    public class ViewQuestionGroupModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }

        public List<ViewQuestionModel> QuestionList { get; set; }  
    }

    public class ViewQuestionModel
    {
        public int SeqNo { get; set; }
        public string QuestionName { get; set; }
    }

}