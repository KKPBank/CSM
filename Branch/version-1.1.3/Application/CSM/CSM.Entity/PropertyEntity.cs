using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Entity
{
    [Serializable]
    public class PropertyEntity
    {
        public int? RowId { get; set; }

        [Display(Name = "IF_ROW_STAT")]
        [LocalizedStringLength(Constants.MaxLength.IfRowStat, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IfRowStat { get; set; }

        [Display(Name = "IF_ROW_BATCH_NUM")]
        [LocalizedStringLength(Constants.MaxLength.IfRowBatchNum, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IfRowBatchNum { get; set; }

        [Display(Name = "AST_ASSET_NUM")]
        [LocalizedStringLength(Constants.MaxLength.AssetNum, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetNum { get; set; }

        [Display(Name = "AST_TYPE_CD")]
        [LocalizedStringLength(Constants.MaxLength.AssetType, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetType { get; set; }

        [Display(Name = "AST_TRADEINTYPE_CD")]
        [LocalizedStringLength(Constants.MaxLength.AssetTradeInType, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetTradeInType { get; set; }

        [Display(Name = "AST_STATUS_CD")]
        [LocalizedStringLength(Constants.MaxLength.AssetStatus, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetStatus { get; set; }

        [Display(Name = "AST_DESC_TEXT")]
        [LocalizedStringLength(Constants.MaxLength.AssetDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetDesc { get; set; }

        [Display(Name = "AST_NAME")]
        [LocalizedStringLength(Constants.MaxLength.AssetName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetName { get; set; }

        [Display(Name = "AST_COMMENTS")]
        [LocalizedStringLength(Constants.MaxLength.AssetComments, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetComments { get; set; }

        [Display(Name = "AST_REF_NUMBER_1")]
        [LocalizedStringLength(Constants.MaxLength.AssetRefNo1, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetRefNo1 { get; set; }

        [Display(Name = "AST_LOT_NUM")]
        [LocalizedStringLength(Constants.MaxLength.AssetLot, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetLot { get; set; }

        [Display(Name = "AST_PURCH_LOC_DESC")]
        [LocalizedStringLength(Constants.MaxLength.AssetPurch, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AssetPurch { get; set; }

        //[Required(ErrorMessage = "Mapping error")]  
        public int? EmployeeId { get; set; }
        public string SaleName { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Error { get; set; }
    }
}
