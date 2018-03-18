using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSM.Entity
{
    [Serializable]
    public class ProductSREntity
    {
        #region "Local Declaration"

        private bool m_IsEdit = true;

        #endregion

        public int? SRStatusChangeId { get; set; }
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int? AreaId { get; set; }
        public string AreaName { get; set; }
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }
        public int FromSRStatusId { get; set; }
        public string FromSRStatusName { get; set; }
        public int? FromSRStateId { get; set; }
        public string FromSRStateName { get; set; }
        public List<int> ToSRStatusIds { get; set; }
        public List<SRStatusEntity> ToSRStatusList { get; set; }
        
        public bool IsEdit
        {
            get { return m_IsEdit; }
            set { m_IsEdit = value; }
        }

        public string ToSRStatusDisplay
        {
            get
            {
                if (ToSRStatusList == null) return string.Empty;
                return StringHelpers.ConvertListToString(ToSRStatusList.Select(x => x.SRStatusName).ToList<object>(), "<br>");
            }
        }

        public string JsonProductSR
        {
            get
            {
                dynamic jsonObject = new JObject();
                jsonObject.ProductGroupId = this.ProductGroupId;
                jsonObject.ProductId = this.ProductId;
                jsonObject.CampaignId = this.CampaignId;
                jsonObject.TypeId = this.TypeId;
                jsonObject.AreaId = this.AreaId;
                jsonObject.SubAreaId = this.SubAreaId;
                jsonObject.FromSRStatus = this.FromSRStatusId;
                return JsonConvert.SerializeObject(jsonObject);
            }
        }

        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Updatedate { get; set; }
    }

    [Serializable]
    public class ProductSearchFilter : Pager
    {
        public int? ProductGroupId { get; set; }
        public int? ProductId { get; set; }
        public int? CampaignId { get; set; }
        public int? TypeId { get; set; }
        public int? AreaId { get; set; }
        public int? SubAreaId { get; set; }
        public int? FromSRState { get; set; }
        public int? ToSRState { get; set; }
        public int? FromSRStatus { get; set; }
        public int? ToSRStatus { get; set; }
    }
}
