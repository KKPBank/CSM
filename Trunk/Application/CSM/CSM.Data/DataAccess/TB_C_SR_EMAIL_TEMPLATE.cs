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
    
    public partial class TB_C_SR_EMAIL_TEMPLATE
    {
        public TB_C_SR_EMAIL_TEMPLATE()
        {
            this.TB_T_SR_ACTIVITY = new HashSet<TB_T_SR_ACTIVITY>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int SR_EMAIL_TEMPLATE_ID { get; set; }
        public string SR_EMAIL_TEMPLATE_CODE { get; set; }
        public string SR_EMAIL_TEMPLATE_NAME { get; set; }
        public string SR_EMAIL_TEMPLATE_TYPE { get; set; }
        public string SR_EMAIL_TEMPLATE_SENDER { get; set; }
        public string SR_EMAIL_TEMPLATE_SUBJECT { get; set; }
        public string SR_EMAIL_TEMPLATE_CONTENT { get; set; }
        public string SR_EMAIL_TEMPLATE_REMARK { get; set; }
    
        public virtual ICollection<TB_T_SR_ACTIVITY> TB_T_SR_ACTIVITY { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
