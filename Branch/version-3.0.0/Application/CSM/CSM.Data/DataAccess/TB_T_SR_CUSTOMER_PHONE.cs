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
    
    public partial class TB_T_SR_CUSTOMER_PHONE
    {
        public int SR_CUSTOMER_PHONE_ID { get; set; }
        public Nullable<int> SR_CUSTOMER_ID { get; set; }
        public Nullable<int> PHONE_TYPE_ID { get; set; }
        public string PHONE_NO { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string PHONE_EXT { get; set; }
    
        public virtual TB_M_PHONE_TYPE TB_M_PHONE_TYPE { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
        public virtual TB_T_SR_CUSTOMER TB_T_SR_CUSTOMER { get; set; }
    }
}
