using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    public class CisSubscriptionEntity
    {
        [Display(Name = "KKCISID")]
        [LocalizedStringLength(Constants.MaxLength.KKCisId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string KKCisId { get; set; }

        [Display(Name = "CUSTID")]
        [LocalizedStringLength(Constants.MaxLength.CustId, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CustId { get; set; }

        [Display(Name = "CARD_ID")]
        [LocalizedStringLength(Constants.MaxLength.card_id, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardId { get; set; }

        [Display(Name = "CARD_TYPE_CODE")]
        [LocalizedStringLength(Constants.MaxLength.card_type_code, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CardTypeCode { get; set; }

        [Display(Name = "PROD_GROUP")]
        [LocalizedStringLength(Constants.MaxLength.prod_group, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdGroup { get; set; }

        [Display(Name = "PROD_TYPE")]
        [LocalizedStringLength(Constants.MaxLength.prod_type, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ProdType { get; set; }

        [Display(Name = "SUBSCR_CODE")]
        [LocalizedStringLength(Constants.MaxLength.subscrcode, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrCode { get; set; }

        [Display(Name = "REF_NO")]
        [LocalizedStringLength(Constants.MaxLength.ref_no, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RefNo { get; set; }

        [Display(Name = "BRANCH_NAME")]
        [LocalizedStringLength(Constants.MaxLength.branch_name, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BranchName { get; set; }

        [Display(Name = "TEXT1")]
        [LocalizedStringLength(Constants.MaxLength.text1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text1 { get; set; }

        [Display(Name = "TEXT2")]
        [LocalizedStringLength(Constants.MaxLength.text2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text2 { get; set; }

        [Display(Name = "TEXT3")]
        [LocalizedStringLength(Constants.MaxLength.text3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text3 { get; set; }

        [Display(Name = "TEXT4")]
        [LocalizedStringLength(Constants.MaxLength.text4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text4 { get; set; }

        [Display(Name = "TEXT5")]
        [LocalizedStringLength(Constants.MaxLength.text5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text5 { get; set; }

        [Display(Name = "TEXT6")]
        [LocalizedStringLength(Constants.MaxLength.text6, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text6 { get; set; }

        [Display(Name = "TEXT7")]
        [LocalizedStringLength(Constants.MaxLength.text7, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text7 { get; set; }

        [Display(Name = "TEXT8")]
        [LocalizedStringLength(Constants.MaxLength.text8, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text8 { get; set; }

        [Display(Name = "TEXT9")]
        [LocalizedStringLength(Constants.MaxLength.text9, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text9 { get; set; }

        [Display(Name = "TEXT10")]
        [LocalizedStringLength(Constants.MaxLength.text10, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Text10 { get; set; }

        [Display(Name = "NUMBER1")]
        [LocalizedStringLength(Constants.MaxLength.number1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number1 { get; set; }

        [Display(Name = "NUMBER2")]
        [LocalizedStringLength(Constants.MaxLength.number2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number2 { get; set; }

        [Display(Name = "NUMBER3")]
        [LocalizedStringLength(Constants.MaxLength.number3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number3 { get; set; }

        [Display(Name = "NUMBER4")]
        [LocalizedStringLength(Constants.MaxLength.number4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number4 { get; set; }

        [Display(Name = "NUMBER5")]
        [LocalizedStringLength(Constants.MaxLength.number5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Number5 { get; set; }

        [Display(Name = "DATE1")]
        [LocalizedStringLength(Constants.MaxLength.date1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date1 { get; set; }

        [Display(Name = "DATE2")]
        [LocalizedStringLength(Constants.MaxLength.date2, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date2 { get; set; }

        [Display(Name = "DATE3")]
        [LocalizedStringLength(Constants.MaxLength.date3, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date3 { get; set; }

        [Display(Name = "DATE4")]
        [LocalizedStringLength(Constants.MaxLength.date4, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date4 { get; set; }

        [Display(Name = "DATE5")]
        [LocalizedStringLength(Constants.MaxLength.date5, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Date5 { get; set; }

        [Display(Name = "SUBSCR_STATUS")]
        [LocalizedStringLength(Constants.MaxLength.subscr_status, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SubscrStatus { get; set; }

        [Display(Name = "CREATED_DATE")]
        [LocalizedStringLength(Constants.MaxLength.created_date, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedDate { get; set; }

        [Display(Name = "CREATED_BY")]
        [LocalizedStringLength(Constants.MaxLength.address_created_by, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedBy { get; set; }

        [Display(Name = "CREATED_CHANNEL")]
        [LocalizedStringLength(Constants.MaxLength.created_channel, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatedChanel { get; set; }

        [Display(Name = "UPDATED_DATE")]
        [LocalizedStringLength(Constants.MaxLength.updated_date, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedDate { get; set; }

        [Display(Name = "UPDATED_BY")]
        [LocalizedStringLength(Constants.MaxLength.address_updated_by, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedBy { get; set; }

        [Display(Name = "UPDATED_CHANNEL")]
        [LocalizedStringLength(Constants.MaxLength.updated_channel, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UpdatedChannel { get; set; }
    }
}
