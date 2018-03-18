using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class SubAreaEntity
    {
        public int? SubareaId { get; set; }
        public string SubareaName { get; set; }
        public List<SubAreaItemEntity> SubAreaList { get; set; } 
    }

    public class SubAreaItemEntity
    {
        public int? SubAreaId { get; set; }
        public string SubAreaName { get;set; }
        public string SubAreaCode { get; set; }
        public bool IsActive { get;set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public int UserId { get; set; }
        public bool IsEdit { get; set; }

    }

    public class AreaSubAreaEntity
    {
        public List<AreaSubAreaItemEntity> AreaSubAreaList { get; set; }
    }

    public class AreaSubAreaItemEntity
    {
        public int AreaSubAreaId { get; set; }
        public int SubAreaId { get; set; }
        public string SubAreaName { get; set; }
        public bool IsActive { get; set; }
        public string CreateUserFirstName { get; set; }
        public string CreateUserLastName { get; set; }
        public DateTime? CreateDate { get; set; }

    }
    [Serializable]
    public class SubAreaSearchFilter : Pager
    {
        
    }
    [Serializable]
    public class SelectSubAreaSearchFilter : Pager
    {
        public string SubAreaName { get; set; }
        public string SubAreaCode { get; set; }
        public string SubAreaIdList { get; set; }

        public int AreaId { get; set; }
    }
}
