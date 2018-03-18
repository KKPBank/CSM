using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class SubAreaViewModel
    {
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }
        public string SubAreaCode { get; set; }
        public bool Status { get; set; }

        public List<SubAreaTableModel> SubAreaTableList { get; set; }
        public SelectSubAreaSearchFilter SubAreaSearchFilter { get; set; }
    }

    public class SubAreaSearchModel
    {
        public SubAreaSearchFilter SearchFilter { get; set; }
        public SelectSubAreaSearchFilter SelectSearchFilter { get; set; }
        public IEnumerable<SubAreaItemEntity> SubAreaList { get; set; }
        public IEnumerable<SubAreaItemEntity> SelectSubAreaList { get; set; } 
    }
    [Serializable]
    public class SubAreaTableModel
    {
        public int id { get; set; }
        public string area_name { get; set; }
        public string area_code { get; set; }
        public string status { get; set; }
        public string update_name { get; set; }
        public string update_date { get; set; }
    }
}