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
    
    public partial class TB_M_SUBSCRIPT_TYPE
    {
        public TB_M_SUBSCRIPT_TYPE()
        {
            this.TB_M_CONTACT = new HashSet<TB_M_CONTACT>();
            this.TB_M_CUSTOMER = new HashSet<TB_M_CUSTOMER>();
        }
    
        public int SUBSCRIPT_TYPE_ID { get; set; }
        public string SUBSCRIPT_TYPE_CODE { get; set; }
        public string SUBSCRIPT_TYPE_NAME { get; set; }
        public Nullable<short> STATUS { get; set; }
        public string CARD_TYPE_CODE { get; set; }
        public string CUST_TYPE_GROUP { get; set; }
        public string ID_TYPE_CODE { get; set; }
    
        public virtual ICollection<TB_M_CONTACT> TB_M_CONTACT { get; set; }
        public virtual ICollection<TB_M_CUSTOMER> TB_M_CUSTOMER { get; set; }
    }
}
