using System;
using System.Collections.Generic;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class AreaEntity
    {
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public List<AreaItemEntity> AreaList { get; set; }
    }

    public class AreaItemEntity
    {
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public UserEntity UpdateUser { get; set; }
        public UserEntity CreateUser { get; set; }
        public string IsActive { get; set; }

        public bool Status { get; set; }
        public int UserId { get; set; }
    }
    [Serializable]
    public class AreaSearchFilter : Pager
    {
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public string Status { get; set; }
    }
}
