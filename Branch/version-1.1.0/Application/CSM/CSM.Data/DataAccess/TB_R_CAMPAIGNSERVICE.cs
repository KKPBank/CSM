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
    
    public partial class TB_R_CAMPAIGNSERVICE
    {
        public TB_R_CAMPAIGNSERVICE()
        {
            this.TB_C_SR_STATUS_CHANGE = new HashSet<TB_C_SR_STATUS_CHANGE>();
            this.TB_M_MAP_PRODUCT = new HashSet<TB_M_MAP_PRODUCT>();
            this.TB_M_SLA = new HashSet<TB_M_SLA>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int CAMPAIGNSERVICE_ID { get; set; }
        public string CAMPAIGNSERVICE_NAME { get; set; }
        public int PRODUCT_ID { get; set; }
        public bool CAMPAIGNSERVICE_IS_ACTIVE { get; set; }
        public string CREATE_USER { get; set; }
        public string UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string CAMPAIGNSERVICE_CODE { get; set; }
    
        public virtual ICollection<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE { get; set; }
        public virtual ICollection<TB_M_MAP_PRODUCT> TB_M_MAP_PRODUCT { get; set; }
        public virtual ICollection<TB_M_SLA> TB_M_SLA { get; set; }
        public virtual TB_R_PRODUCT TB_R_PRODUCT { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
