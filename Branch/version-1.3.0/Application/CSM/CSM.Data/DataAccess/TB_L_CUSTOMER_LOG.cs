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
    public partial class TB_L_CUSTOMER_LOG
    {
        public int LOG_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string DETAIL { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_USER { get; set; }
    
        public virtual TB_M_CUSTOMER TB_M_CUSTOMER { get; set; }
    }
}
