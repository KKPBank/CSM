using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;

namespace CSM.Web.Models
{
    [Serializable]
    public class ProductViewModel
    {
        #region "Local Declaration"

        private bool m_IsEdit = false;

        #endregion

        [Display(Name = "Lbl_ProductGroup", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? ProductGroupId { get; set; }

        [Display(Name = "Lbl_Product", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? ProductId { get; set; }

        [Display(Name = "Lbl_Type", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? TypeId { get; set; }

        [Display(Name = "Lbl_Area", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? AreaId { get; set; }

        [Display(Name = "Lbl_FromSRStatus", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? FromSRStatusId { get; set; }
        public string FromSRStatusName { get; set; }
        public int? FromSRStateId { get; set; }
        public string FromSRStateName { get; set; }

        public SelectList FromStatusList { get; set; }
        public List<int> ToSRStatusLeftId { get; set; }
        public MultiSelectList ToSRStatusLeftList { get; set; }

        [Display(Name = "Lbl_ToSRStatus", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public List<int> ToSRStatusRightId { get; set; }

        public MultiSelectList ToSRStatusRightList { get; set; }
        public IEnumerable<ProductSREntity> ProductList { get; set; }
        public ProductSearchFilter SearchFilter { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public int? SubAreaId { get; set; }
        public string SubAreaName { get; set; }

        public bool IsEdit
        {
            get { return m_IsEdit; }
            set { m_IsEdit = value; }
        }

        public ProductViewModel(ProductSearchFilter searchFilter)
        {
            if (searchFilter != null)
            {
                this.ProductGroupId = searchFilter.ProductGroupId;
                this.ProductId = searchFilter.ProductId;
                this.CampaignId = searchFilter.CampaignId;
                this.TypeId = searchFilter.TypeId;
                this.AreaId = searchFilter.AreaId;
                this.SubAreaId = searchFilter.SubAreaId;
            }
        }

        public ProductViewModel()
        {

        }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; }
    }
}