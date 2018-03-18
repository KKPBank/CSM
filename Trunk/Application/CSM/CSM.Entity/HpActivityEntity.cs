using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class HpActivityEntity
    {
        public int? HpActivityId { get; set; }

        [Display(Name = "CHANNEL")]
        [LocalizedStringLength(Constants.MaxLength.Channel, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Channel { get; set; }

        [Display(Name = "TYPE")]
        [LocalizedStringLength(Constants.MaxLength.Type, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Type { get; set; }

        [Display(Name = "AREA")]
        [LocalizedStringLength(Constants.MaxLength.Area, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Area { get; set; }

        [Display(Name = "STATUS")]
        [LocalizedStringLength(Constants.MaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }

        [Display(Name = "DESCRIPTION")]
        [LocalizedStringLength(Constants.MaxLength.Description, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Description { get; set; }

        [Display(Name = "COMMENT")]
        [LocalizedStringLength(Constants.MaxLength.Comment, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Comment { get; set; }

        [Display(Name = "ASSET_INTO")]
        [LocalizedStringLength(Constants.MaxLength.AssetInfo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetInfo { get; set; }

        [Display(Name = "CONTACT_INFO")]
        [LocalizedStringLength(Constants.MaxLength.ContactInfo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactInfo { get; set; }

        [Display(Name = "A_NO")]
        [LocalizedStringLength(Constants.MaxLength.Ano, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Ano { get; set; }

        [Display(Name = "CALL_ID")]
        [LocalizedStringLength(Constants.MaxLength.CallId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CallId { get; set; }

        [Display(Name = "CONTACT_NAME")]
        [LocalizedStringLength(Constants.MaxLength.ContactName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactName { get; set; }

        [Display(Name = "CONTACT_LAST_NAME")]
        [LocalizedStringLength(Constants.MaxLength.ContactLastName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactLastName { get; set; }

        [Display(Name = "CONTACT_PHONE")]
        [LocalizedStringLength(Constants.MaxLength.ContactPhone, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactPhone { get; set; }

        [Display(Name = "DONE_FLG")]
        [LocalizedStringLength(Constants.MaxLength.DoneFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DoneFlg { get; set; }

        [Display(Name = "CREATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.CreateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateDate { get; set; }

        [Display(Name = "CREATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.CreateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreateBy { get; set; }

        [Display(Name = "START_DATE")]
        [LocalizedStringLength(Constants.MaxLength.StartDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string StartDate { get; set; }

        [Display(Name = "END_DATE")]
        [LocalizedStringLength(Constants.MaxLength.EndDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EndDate { get; set; }

        [Display(Name = "OWNER_LOGIN")]
        [LocalizedStringLength(Constants.MaxLength.OwnerLogin, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OwnerLogin { get; set; }

        [Display(Name = "OWNER_PER_ID")]
        [LocalizedStringLength(Constants.MaxLength.OwnerPerId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OwnerPerId { get; set; }

        [Display(Name = "UPDATE_DATE")]
        [LocalizedStringLength(Constants.MaxLength.UpdateDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateDate { get; set; }

        [Display(Name = "UPDATE_BY")]
        [LocalizedStringLength(Constants.MaxLength.UpdateBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdateBy { get; set; }

        [Display(Name = "SR_NO")]
        [LocalizedStringLength(Constants.MaxLength.SrId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SrNo { get; set; }

        [Display(Name = "CALL_FLG")]
        [LocalizedStringLength(Constants.MaxLength.CallFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CallFlg { get; set; }

        [Display(Name = "ENQ_FLG")]
        [LocalizedStringLength(Constants.MaxLength.EnqFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EnqFlg { get; set; }

        [Display(Name = "LOC_ENQ_FLG")]
        [LocalizedStringLength(Constants.MaxLength.LocEnqFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LocEnqFlg { get; set; }

        [Display(Name = "DOC_REQ_FLG")]
        [LocalizedStringLength(Constants.MaxLength.DocReqFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DocReqFlg { get; set; }

        [Display(Name = "PRI_ISSUED_FLG")]
        [LocalizedStringLength(Constants.MaxLength.PriIssuedFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PriIssuedFlg { get; set; }

        [Display(Name = "ASSET_INSPECT_FLG")]
        [LocalizedStringLength(Constants.MaxLength.AssetInspectFlg, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetInspectFlg { get; set; }

        [Display(Name = "PLAN_START_DATE")]
        [LocalizedStringLength(Constants.MaxLength.PlanStartDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PlanstartDate { get; set; }

        [Display(Name = "CONTACT_FAX")]
        [LocalizedStringLength(Constants.MaxLength.ContactFax, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactFax { get; set; }

        [Display(Name = "CONTACT_EMAIL")]
        [LocalizedStringLength(Constants.MaxLength.ContactEmail, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ContactEmail { get; set; }

        public string Error { get; set; }
        public int? SrId { get; set; }
        public int? SrStatusId { get; set; }

    }

    [Serializable]
    public class HpStatusEntity
    {
        public int? HpStatusId { get; set; }
        public string HpLangIndeCode { get; set; }
        public string HpSubject { get; set; }
    }
}
