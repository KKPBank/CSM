using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Resources;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace CSM.Entity
{
    public class SRStatusEntity
    {
        public int SRStatusId { get; set; }
        [Display(Name = "Status Code"), Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [LocalizedStringLength(50, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(Resource))]
        public string SRStatusCode { get; set; }
        [Display(Name = "Status Name"), Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        [LocalizedStringLength(500, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(Resource))]
        public string SRStatusName { get; set; }
        [Display(Name = "Status"), Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string Status { get; set; }
        public string StatusDisplay
        {
            get
            {
                return Status == "A" ? "Active" : "Inactive";
            }
        }
        [Display(Name = "Send to HP Log 100"), Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public bool? SendHP { get; set; }
        [Display(Name = "Send to Rule"), Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public bool? SendRule { get; set; }

        public int? SRStateId { get; set; }
        public SRStateEntity SRState { get; set; }

        //[Display(Name = "Lbl_SR_Page", ResourceType = typeof(Resource))]
        //[LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public List<int> SRPageIdList { get; set; }
        public List<SrPageItemEntity> SRPages { get; set; }
        //public string SRPagesDisplay
        //{
        //    get
        //    {
        //        return SRPages != null && SRPages.Count > 0 ? (from a in SRPages
        //                                  select a.SrPageName)
        //                .Aggregate((accum, item) => accum + (string.IsNullOrWhiteSpace(accum) ? "" : ", ") + item)
        //                : null;
        //    }
        //}

        public List<int> Old_SRPageIdList { get; set; }
        public MultiSelectList SRPageList { get; set; }

        public List<int> SRPageIdAll { get; set; }
        public MultiSelectList SRPageListAll { get; set; }

        public UserEntity CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateDateDisplay
        { get { return CreateDate.ToDisplay(); } }

        public UserEntity UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateDateDisplay
        { get { return UpdateDate.ToDisplay(); } }
    }

    public class SRStateEntity
    {
        public int? SRStateId { get; set; }
        public string SRStateName { get; set; }
    }

    public class SRStatusSearchFilter : Pager
    {
        public int? SRStatusId { get; set; }
        public int? SRStateId { get; set; }
        public int? SRPageId { get; set; }
        public string Status { get; set; }
    }
}
