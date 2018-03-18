using System;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;

namespace CSM.Web.Models
{
    [Serializable]
    public class NoteViewModel
    {
        public int? CustomerId { get; set; }
        public int? NoteId { get; set; }

        [Display(Name = "Lbl_EffectiveDate", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string EffectiveDate { get; set; }

        [Display(Name = "Lbl_ExpiryDate", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string ExpiryDate { get; set; }

        [Display(Name = "Lbl_Detail", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]

        public string Detail { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }
        public DateTime? EffectiveDateValue { get { return EffectiveDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? ExpiryDateValue { get { return ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
    }
}