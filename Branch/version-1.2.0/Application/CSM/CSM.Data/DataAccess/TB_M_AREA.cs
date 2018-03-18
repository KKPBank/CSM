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
    public partial class TB_M_AREA
    {
        public TB_M_AREA()
        {
            this.TB_C_SR_STATUS_CHANGE = new HashSet<TB_C_SR_STATUS_CHANGE>();
            this.TB_M_AREA_SUBAREA = new HashSet<TB_M_AREA_SUBAREA>();
            this.TB_M_SLA = new HashSet<TB_M_SLA>();
            this.TB_M_MAP_PRODUCT = new HashSet<TB_M_MAP_PRODUCT>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int AREA_ID { get; set; }
        public string AREA_NAME { get; set; }
        public bool AREA_IS_ACTIVE { get; set; }
        public Nullable<decimal> AREA_CODE { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
    
        public virtual ICollection<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE { get; set; }
        public virtual ICollection<TB_M_AREA_SUBAREA> TB_M_AREA_SUBAREA { get; set; }
        public virtual ICollection<TB_M_SLA> TB_M_SLA { get; set; }
        public virtual ICollection<TB_M_MAP_PRODUCT> TB_M_MAP_PRODUCT { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
