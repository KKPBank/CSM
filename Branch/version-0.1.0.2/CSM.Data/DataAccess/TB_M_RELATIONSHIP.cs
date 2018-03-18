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
    
    public partial class TB_M_RELATIONSHIP
    {
        public TB_M_RELATIONSHIP()
        {
            this.TB_M_CUSTOMER_CONTACT = new HashSet<TB_M_CUSTOMER_CONTACT>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int RELATIONSHIP_ID { get; set; }
        public string RELATIONSHIP_NAME { get; set; }
        public string RELATIONSHIP_DESC { get; set; }
        public Nullable<short> STATUS { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public Nullable<bool> IS_DEFAULT { get; set; }
    
        public virtual ICollection<TB_M_CUSTOMER_CONTACT> TB_M_CUSTOMER_CONTACT { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
    }
}
