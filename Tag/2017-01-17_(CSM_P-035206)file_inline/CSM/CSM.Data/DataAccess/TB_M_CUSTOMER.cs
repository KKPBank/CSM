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
    
    public partial class TB_M_CUSTOMER
    {
        public TB_M_CUSTOMER()
        {
            this.TB_L_CUSTOMER_LOG = new HashSet<TB_L_CUSTOMER_LOG>();
            this.TB_M_ACCOUNT = new HashSet<TB_M_ACCOUNT>();
            this.TB_M_ACCOUNT_ADDRESS = new HashSet<TB_M_ACCOUNT_ADDRESS>();
            this.TB_M_ACCOUNT_EMAIL = new HashSet<TB_M_ACCOUNT_EMAIL>();
            this.TB_M_ACCOUNT_PHONE = new HashSet<TB_M_ACCOUNT_PHONE>();
            this.TB_M_ADDRESS = new HashSet<TB_M_ADDRESS>();
            this.TB_M_NOTE = new HashSet<TB_M_NOTE>();
            this.TB_M_PHONE = new HashSet<TB_M_PHONE>();
            this.TB_M_CUSTOMER_CONTACT = new HashSet<TB_M_CUSTOMER_CONTACT>();
            this.TB_M_CUSTOMER_ATTACHMENT = new HashSet<TB_M_CUSTOMER_ATTACHMENT>();
            this.TB_T_SR_ATTACHMENT = new HashSet<TB_T_SR_ATTACHMENT>();
            this.TB_T_SR = new HashSet<TB_T_SR>();
        }
    
        public int CUSTOMER_ID { get; set; }
        public Nullable<short> TYPE { get; set; }
        public Nullable<int> SUBSCRIPT_TYPE_ID { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string FIRST_NAME_TH { get; set; }
        public string FIRST_NAME_EN { get; set; }
        public string LAST_NAME_TH { get; set; }
        public string LAST_NAME_EN { get; set; }
        public string CARD_NO { get; set; }
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }
        public string EMAIL { get; set; }
        public Nullable<int> TITLE_TH_ID { get; set; }
        public Nullable<int> TITLE_EN_ID { get; set; }
        public Nullable<int> EMPLOYEE_ID { get; set; }
        public string FAX { get; set; }
        public Nullable<decimal> KKCIS_ID { get; set; }
        public string CARD_TYPE_CODE { get; set; }
    
        public virtual ICollection<TB_L_CUSTOMER_LOG> TB_L_CUSTOMER_LOG { get; set; }
        public virtual ICollection<TB_M_ACCOUNT> TB_M_ACCOUNT { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_ADDRESS> TB_M_ACCOUNT_ADDRESS { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_EMAIL> TB_M_ACCOUNT_EMAIL { get; set; }
        public virtual ICollection<TB_M_ACCOUNT_PHONE> TB_M_ACCOUNT_PHONE { get; set; }
        public virtual ICollection<TB_M_ADDRESS> TB_M_ADDRESS { get; set; }
        public virtual ICollection<TB_M_NOTE> TB_M_NOTE { get; set; }
        public virtual ICollection<TB_M_PHONE> TB_M_PHONE { get; set; }
        public virtual ICollection<TB_M_CUSTOMER_CONTACT> TB_M_CUSTOMER_CONTACT { get; set; }
        public virtual ICollection<TB_M_CUSTOMER_ATTACHMENT> TB_M_CUSTOMER_ATTACHMENT { get; set; }
        public virtual ICollection<TB_T_SR_ATTACHMENT> TB_T_SR_ATTACHMENT { get; set; }
        public virtual TB_M_SUBSCRIPT_TYPE TB_M_SUBSCRIPT_TYPE { get; set; }
        public virtual TB_M_TITLE TB_M_TITLE { get; set; }
        public virtual TB_M_TITLE TB_M_TITLE1 { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
        public virtual TB_R_USER TB_R_USER2 { get; set; }
        public virtual ICollection<TB_T_SR> TB_T_SR { get; set; }
    }
}