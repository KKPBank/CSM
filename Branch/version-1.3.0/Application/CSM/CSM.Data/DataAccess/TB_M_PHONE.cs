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
    public partial class TB_M_PHONE
    {
        public int PHONE_ID { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
        public Nullable<int> PHONE_TYPE_ID { get; set; }
        public string PHONE_NO { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string PHONE_EXT { get; set; }
    
        public virtual TB_M_CUSTOMER TB_M_CUSTOMER { get; set; }
        public virtual TB_M_PHONE_TYPE TB_M_PHONE_TYPE { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
    }
}
