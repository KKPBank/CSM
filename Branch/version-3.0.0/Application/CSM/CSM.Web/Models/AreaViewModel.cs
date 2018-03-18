using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class AreaViewModel
    {
        public AreaSearchFilter SearchFilter { get; set; }
        public IEnumerable<AreaItemEntity> AreaList { get; set; }

        public SubAreaSearchFilter SubAreaSearchFilter { get; set; }
        public SelectSubAreaSearchFilter SelectSearchFilter { get; set; }
        public IEnumerable<SubAreaItemEntity> SubAreaList { get; set; }
        public IEnumerable<SubAreaItemEntity> SelectSubAreaList { get; set; }
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string AreaName { get; set; }
        [LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public string AreaCode { get; set; }
    }

    public class AreaSaveViewModel
    {
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public bool Status { get; set; }

    }

    public class AreaEditViewModel
    {
        public int? AreaId { get; set; }
        public string txtAreaName { get; set; }
        public string AreaCode { get; set; }
        public bool selectStatus { get; set; }
        public string txtCreateUser { get; set; }
        public string txtUpdateUser { get; set; }
        public string txtCreateDateTime { get; set; }
        public string txtUpdateDateTime { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public SelectSubAreaSearchFilter SubAreaSearchFilter { get; set; }
        public IEnumerable<SubAreaItemEntity> SubAreaList { get; set; }
        public SelectSubAreaSearchFilter SelectSearchFilter { get; set; }
        public IEnumerable<SubAreaItemEntity> SelectSubAreaList { get; set; }
    }
}