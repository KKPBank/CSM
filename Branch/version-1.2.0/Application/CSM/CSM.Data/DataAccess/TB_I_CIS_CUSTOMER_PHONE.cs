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
    public partial class TB_I_CIS_CUSTOMER_PHONE
    {
        public int CIS_PHONE_ID { get; set; }
        public string KKCIS_ID { get; set; }
        public string CUST_ID { get; set; }
        public string CARD_ID { get; set; }
        public string CARD_TYPE_CODE { get; set; }
        public string CUST_TYPE_GROUP { get; set; }
        public string PHONE_TYPE_CODE { get; set; }
        public string PHONE_NUM { get; set; }
        public string PHONE_EXT { get; set; }
        public string CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public string UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
        public Nullable<int> PHONE_TYPE_ID { get; set; }
        public string ERROR { get; set; }
    }
}