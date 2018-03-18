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
    public partial class TB_M_ACCOUNT
    {
        public TB_M_ACCOUNT()
        {
            this.TB_M_CUSTOMER_CONTACT = new HashSet<TB_M_CUSTOMER_CONTACT>();
            this.TB_M_ACCOUNT_ADDRESS = new HashSet<TB_M_ACCOUNT_ADDRESS>();
            this.TB_M_ACCOUNT_EMAIL = new HashSet<TB_M_ACCOUNT_EMAIL>();
            this.TB_M_ACCOUNT_PHONE = new HashSet<TB_M_ACCOUNT_PHONE>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int ACCOUNT_ID { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
        public string PRODUCT_GROUP { get; set; }
        public string PRODUCT { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string BRANCH_NAME { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string CAR_NO { get; set; }
        public string GRADE { get; set; }
        public Nullable<decimal> KKCIS_ID { get; set; }
        public string PRODUCT_DESC { get; set; }
        public string SUBSCRIPTION_CODE { get; set; }
        public string SUBSCRIPTION_DESC { get; set; }
        public Nullable<bool> IS_DEFAULT { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ACCOUNT_DESC { get; set; }
        public Nullable<decimal> CIS_SUBSCRIPTION_ID { get; set; }
    
        public virtual ICollection<TB_M_CUSTOMER_CONTACT> TB_M_CUSTOMER_CONTACT { get; set; }
        public virtual TB_M_CUSTOMER TB_M_CUSTOMER { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_ADDRESS> TB_M_ACCOUNT_ADDRESS { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_EMAIL> TB_M_ACCOUNT_EMAIL { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_PHONE> TB_M_ACCOUNT_PHONE { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}
