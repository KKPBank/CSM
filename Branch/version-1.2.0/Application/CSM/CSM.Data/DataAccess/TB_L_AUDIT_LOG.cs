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
    public partial class TB_L_AUDIT_LOG
    {
        public int AUDIT_LOG_ID { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string ACTION { get; set; }
        public Nullable<short> STATUS { get; set; }
        public string DETAIL { get; set; }
        public string MODULE { get; set; }
        public string IP_ADDRESS { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
    
        public virtual TB_R_USER TB_R_USER { get; set; }
    }
}
