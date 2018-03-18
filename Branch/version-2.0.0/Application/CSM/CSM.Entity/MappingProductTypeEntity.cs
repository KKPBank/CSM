using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class MappingProductEntity
    {
        public MappingProductEntity()
        {
            MappingProductQuestionGroups = new List<MappingProductQuestionGroupEntity>();
        }

        public int MappingProductId { get; set; }
        public int ProductGroupId { get; set; }
        public int ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int TypeId { get; set; }
        
        public UserEntity DefaultOwnerUser { get; set; }
        public BranchEntity DefaultOwnerBranch { get; set; }

        public bool IsVerify { get; set; }
        public bool IsActive { get; set; }
        public int SrPageId { get; set; }
        public string SrPageCode { get; set; }
        public List<MappingProductQuestionGroupEntity> MappingProductQuestionGroups { get; set; }

        public bool IsVerifyOTP { get; set; }
        public bool IsSRSecret { get; set; }
        public OTPTemplateEntity OTPTemplate { get; set; }
        public HpStatusEntity HpStatus { get; set; }
    }

    public class MappingProductQuestionGroupEntity
    {
        public MappingProductQuestionGroupEntity()
        {
            Questions = new List<MappingProductQuestionEntity>();
        }
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public int RequireAmountPass { get; set; }
        public int SeqNo { get; set; }
        public List<MappingProductQuestionEntity> Questions { get; set; }
    }

    public class MappingProductQuestionEntity
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public string Result { get; set; }
    }

    public class MappingProductTypeItemEntity
    {
        public int? MapProductId { get; set; }
        public int? ProductGroupId { get; set; }
        public int ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int? OwnerUserId { get; set; }
        public UserEntity OwnerUser { get; set; }
        public int SrPageId { get; set; }
        public bool IsVerify { get; set; }

        public string HPSubject { get; set; }
        public string HPLanguageIndependentCode { get; set; }

        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
        public DateTime? CreateDate { get; set; }

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public int? OwnerBranchId { get; set; }
        public string Verify { get; set; }
        public string OwnerSrName { get; set; }
        public string OwnerBranchName { get; set; }
        public string SrPageName { get; set; }
        public List<string> QuestionGroupNameList { get; set; }
        public string QuestionGroupName
        {
            get
            {
                if (QuestionGroupNameList != null && QuestionGroupNameList.Count > 0)
                    return string.Join("<br/>", QuestionGroupNameList);
                else
                    return string.Empty;
            }
        }
        public string Active { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsVerifyOTP { get; set; }
        public bool IsSRSecret { get; set; }
        public OTPTemplateEntity OTPTemplate { get; set; }
        public HpStatusEntity HpStatus { get; set; }
        public string HPStatusStr
        {
            get
            { return (HpStatus != null ? $"{HpStatus.HpLangIndeCode}-{HpStatus.HpSubject}" : string.Empty); }
        }
        public string OTPTemplateName { get { return (OTPTemplate != null ? OTPTemplate.OTPTemplateName : string.Empty); } }
        public string VerifyOTP { get { return (IsVerifyOTP ? "Yes" : "No"); } }
    }

    public class ProductQuestionGroupEditItemEntity
    {
        public int MapProductQuestionId { get; set; }
        public int MapProductId { get; set; }
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public int TotalAmount { get; set; }
        public int ReqAmountPass { get; set; }
        public int Seq { get; set; }
    }

    [Serializable]
    public class ProductQuestionGroupItemEntity
    {
        public string id { get; set; }
        public string pass_value { get; set; }
        public string seq { get; set; }
    }

    [Serializable]
    public class MappingProductSearchFilter : Pager
    {
        public int? ProductGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public string IsVerify { get; set; }
        public int? OwnerId { get; set; }
        public string IsActive { get; set; }
        public bool? IsVerifyOTP { get; set; }
    }

    public class QuestionGroupEditSearchFilter : Pager
    {
        public int? MapProductId { get; set; }
    }

    [Serializable]
    public class OTPTemplateEntity
    {
        public int? OTPTemplateId { get; set; }
        public string OTPTemplateCode { get; set; }
        public string OTPTemplateName { get; set; }
    }
}
