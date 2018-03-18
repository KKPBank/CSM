using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class SlaEntity
    {

    }

    public class SlaItemEntity
    {
        public int? SlaId { get; set; }
        public int ProductId { get; set; }
        public int ProductGroupId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int TypeId { get; set; }
        public int ChannelId { get; set; }
        public int SrStateId { get; set; }
        public int SrStatusId { get; set; }
        public int SlaMinute { get; set; }
        public int SlaTimes { get; set; }
        public int? AlertChiefTimes { get; set; }
        public int? AlertChief1Times { get; set; }
        public int? AlertChief2Times { get; set; }
        public int? AlertChief3Times { get; set; }
        public int SlaDay { get; set; }
        public int UserId { get; set; }

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string ChannelName { get; set; }
        public string StateName { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateDate { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class SlaSearchFilter : Pager
    {
        public int? ProductGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignServiceId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int? TypeId { get; set; }
        public int? ChannelId { get; set; }
        public int? SrStateId { get; set; }
        public int? SrStatusId { get; set; }
    }
}
