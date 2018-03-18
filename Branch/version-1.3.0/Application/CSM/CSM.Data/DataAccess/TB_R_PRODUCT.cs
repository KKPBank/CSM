//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSM.Data.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class TB_R_PRODUCT
    {
        public TB_R_PRODUCT()
        {
            this.TB_C_SR_STATUS_CHANGE = new HashSet<TB_C_SR_STATUS_CHANGE>();
            this.TB_M_MAP_PRODUCT = new HashSet<TB_M_MAP_PRODUCT>();
            this.TB_M_QUESTIONGROUP = new HashSet<TB_M_QUESTIONGROUP>();
            this.TB_M_SLA = new HashSet<TB_M_SLA>();
            this.TB_R_CAMPAIGNSERVICE = new HashSet<TB_R_CAMPAIGNSERVICE>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public int PRODUCTGROUP_ID { get; set; }
        public bool PRODUCT_IS_ACTIVE { get; set; }
        public string CREATE_USER { get; set; }
        public string UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
    
        public virtual ICollection<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE { get; set; }
        public virtual ICollection<TB_M_MAP_PRODUCT> TB_M_MAP_PRODUCT { get; set; }
        public virtual ICollection<TB_M_QUESTIONGROUP> TB_M_QUESTIONGROUP { get; set; }
        public virtual ICollection<TB_M_SLA> TB_M_SLA { get; set; }
        public virtual ICollection<TB_R_CAMPAIGNSERVICE> TB_R_CAMPAIGNSERVICE { get; set; }
        public virtual TB_R_PRODUCTGROUP TB_R_PRODUCTGROUP { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
