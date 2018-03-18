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
    
    public partial class TB_M_ACCOUNT_ADDRESS
    {
        public TB_M_ACCOUNT_ADDRESS()
        {
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int ADDRESS_ID { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
        public string ADDRESS_TYPE_CODE { get; set; }
        public string ADDRESS_TYPE_NAME { get; set; }
        public string ADDRESS_NO { get; set; }
        public string VILLAGE { get; set; }
        public string BUILDING { get; set; }
        public string FLOOR_NO { get; set; }
        public string ROOM_NO { get; set; }
        public string MOO { get; set; }
        public string STREET { get; set; }
        public string SOI { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string COUNTRY { get; set; }
        public string POSTCODE { get; set; }
        public Nullable<decimal> KKCIS_ID { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string PRODUCT_GROUP { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string SUBSCRIPTION_CODE { get; set; }
    
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
        public virtual TB_M_CUSTOMER TB_M_CUSTOMER { get; set; }
    }
}
