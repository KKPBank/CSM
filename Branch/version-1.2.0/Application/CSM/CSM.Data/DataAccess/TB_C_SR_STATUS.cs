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
    public partial class TB_C_SR_STATUS
    {
        public TB_C_SR_STATUS()
        {
            this.TB_C_SR_STATUS_CHANGE = new HashSet<TB_C_SR_STATUS_CHANGE>();
            this.TB_C_SR_STATUS_CHANGE1 = new HashSet<TB_C_SR_STATUS_CHANGE>();
            this.TB_M_SLA = new HashSet<TB_M_SLA>();
            this.TB_T_SR_REPLY_EMAIL = new HashSet<TB_T_SR_REPLY_EMAIL>();
            this.TB_L_SR_LOGGING = new HashSet<TB_L_SR_LOGGING>();
            this.TB_L_SR_LOGGING1 = new HashSet<TB_L_SR_LOGGING>();
            this.TB_T_SR_ACTIVITY = new HashSet<TB_T_SR_ACTIVITY>();
            this.TB_T_SR_ACTIVITY1 = new HashSet<TB_T_SR_ACTIVITY>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int SR_STATUS_ID { get; set; }
        public string SR_STATUS_CODE { get; set; }
        public string SR_STATUS_NAME { get; set; }
    
        public virtual ICollection<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE { get; set; }
        public virtual ICollection<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE1 { get; set; }
        public virtual ICollection<TB_M_SLA> TB_M_SLA { get; set; }
        public virtual ICollection<TB_T_SR_REPLY_EMAIL> TB_T_SR_REPLY_EMAIL { get; set; }
        public virtual ICollection<TB_L_SR_LOGGING> TB_L_SR_LOGGING { get; set; }
        public virtual ICollection<TB_L_SR_LOGGING> TB_L_SR_LOGGING1 { get; set; }
        public virtual ICollection<TB_T_SR_ACTIVITY> TB_T_SR_ACTIVITY { get; set; }
        public virtual ICollection<TB_T_SR_ACTIVITY> TB_T_SR_ACTIVITY1 { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
