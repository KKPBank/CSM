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
    public partial class TB_M_PHONE_TYPE
    {
        public TB_M_PHONE_TYPE()
        {
            this.TB_M_CONTACT_PHONE = new HashSet<TB_M_CONTACT_PHONE>();
            this.TB_M_PHONE = new HashSet<TB_M_PHONE>();
        }
    
        public int PHONE_TYPE_ID { get; set; }
        public string PHONE_TYPE_NAME { get; set; }
        public Nullable<short> STATUS { get; set; }
        public string PHONE_TYPE_CODE { get; set; }
    
        public virtual ICollection<TB_M_CONTACT_PHONE> TB_M_CONTACT_PHONE { get; set; }
        public virtual ICollection<TB_M_PHONE> TB_M_PHONE { get; set; }
    }
}
