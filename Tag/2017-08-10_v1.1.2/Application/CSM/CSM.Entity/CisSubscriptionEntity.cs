using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    public class CisSubscriptionEntity
    {
        [Display(Name = "KKCISID")]
        [LocalizedStringLength(Constants.CisMaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }

        [Display(Name = "CUSTID")]
        [LocalizedStringLength(Constants.CisMaxLength.CustId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.CardId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.CardTypeCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdGroup, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.CisMaxLength.ProdType, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrCode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "REF_NO")]
        [LocalizedStringLength(Constants.CisMaxLength.RefNo, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RefNo { get; set; }

        [Display(Name = "BRANCH_NAME")]
        [LocalizedStringLength(Constants.CisMaxLength.BranchName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BranchName { get; set; }

        [Display(Name = "TEXT1")]
        [LocalizedStringLength(Constants.CisMaxLength.Text1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text1 { get; set; }

        [Display(Name = "TEXT2")]
        [LocalizedStringLength(Constants.CisMaxLength.Text2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text2 { get; set; }

        [Display(Name = "TEXT3")]
        [LocalizedStringLength(Constants.CisMaxLength.Text3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text3 { get; set; }

        [Display(Name = "TEXT4")]
        [LocalizedStringLength(Constants.CisMaxLength.Text4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text4 { get; set; }

        [Display(Name = "TEXT5")]
        [LocalizedStringLength(Constants.CisMaxLength.Text5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text5 { get; set; }

        [Display(Name = "TEXT6")]
        [LocalizedStringLength(Constants.CisMaxLength.Text6, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text6 { get; set; }

        [Display(Name = "TEXT7")]
        [LocalizedStringLength(Constants.CisMaxLength.Text7, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text7 { get; set; }

        [Display(Name = "TEXT8")]
        [LocalizedStringLength(Constants.CisMaxLength.Text8, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text8 { get; set; }

        [Display(Name = "TEXT9")]
        [LocalizedStringLength(Constants.CisMaxLength.Text9, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text9 { get; set; }

        [Display(Name = "TEXT10")]
        [LocalizedStringLength(Constants.CisMaxLength.Text10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text10 { get; set; }

        [Display(Name = "NUMBER1")]
        [LocalizedStringLength(Constants.CisMaxLength.Number1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number1 { get; set; }

        [Display(Name = "NUMBER2")]
        [LocalizedStringLength(Constants.CisMaxLength.Number2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number2 { get; set; }

        [Display(Name = "NUMBER3")]
        [LocalizedStringLength(Constants.CisMaxLength.Number3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number3 { get; set; }

        [Display(Name = "NUMBER4")]
        [LocalizedStringLength(Constants.CisMaxLength.Number4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number4 { get; set; }

        [Display(Name = "NUMBER5")]
        [LocalizedStringLength(Constants.CisMaxLength.Number5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number5 { get; set; }

        [Display(Name = "DATE1")]
        [LocalizedStringLength(Constants.CisMaxLength.Date1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date1 { get; set; }

        [Display(Name = "DATE2")]
        [LocalizedStringLength(Constants.CisMaxLength.Date2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date2 { get; set; }

        [Display(Name = "DATE3")]
        [LocalizedStringLength(Constants.CisMaxLength.Date3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date3 { get; set; }

        [Display(Name = "DATE4")]
        [LocalizedStringLength(Constants.CisMaxLength.Date4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date4 { get; set; }

        [Display(Name = "DATE5")]
        [LocalizedStringLength(Constants.CisMaxLength.Date5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date5 { get; set; }

        [Display(Name = "SUBSCR_STATUS")]
        [LocalizedStringLength(Constants.CisMaxLength.SubscrStatus, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrStatus { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "CREATED_CHANNEL")]
        [LocalizedStringLength(Constants.CisMaxLength.CreatedChannel, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedChanel { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedDate, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATED_BY")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedBy, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }

        [Display(Name = "UPDATED_CHANNEL")]
        [LocalizedStringLength(Constants.CisMaxLength.UpdatedChannel, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedChannel { get; set; }

        [Display(Name = "SYSCUSTSUBSCR_ID")]
        [LocalizedStringLength(Constants.CisMaxLength.SysCustSubscrId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SysCustSubscrId { get; set; }

        public string Error { get; set; }
    }
}
