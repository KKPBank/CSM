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
    public partial class TB_C_SR_PAGE
    {
        public TB_C_SR_PAGE()
        {
            this.TB_C_ROLE_SR_PAGE = new HashSet<TB_C_ROLE_SR_PAGE>();
            this.TB_M_MAP_PRODUCT = new HashSet<TB_M_MAP_PRODUCT>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int SR_PAGE_ID { get; set; }
        public string SR_PAGE_CODE { get; set; }
        public string SR_PAGE_NAME { get; set; }
    
        public virtual ICollection<TB_C_ROLE_SR_PAGE> TB_C_ROLE_SR_PAGE { get; set; }
        public virtual ICollection<TB_M_MAP_PRODUCT> TB_M_MAP_PRODUCT { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}