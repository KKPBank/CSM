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
    public partial class TB_T_ATTACHMENT_TYPE
    {
        public int ATTACHMENT_TYPE_ID { get; set; }
        public Nullable<int> CUSTOMER_ATTACHMENT_ID { get; set; }
        public Nullable<int> NEWS_ATTACHMENT_ID { get; set; }
        public Nullable<int> SR_ATTACHMENT_ID { get; set; }
        public Nullable<int> JOB_ATTACHMENT_ID { get; set; }
        public Nullable<int> DOCUMENT_TYPE_ID { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
    
        public virtual TB_M_CUSTOMER_ATTACHMENT TB_M_CUSTOMER_ATTACHMENT { get; set; }
        public virtual TB_M_DOCUMENT_TYPE TB_M_DOCUMENT_TYPE { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_T_JOB_ATTACHMENT TB_T_JOB_ATTACHMENT { get; set; }
        public virtual TB_T_NEWS_ATTACHMENT TB_T_NEWS_ATTACHMENT { get; set; }
        public virtual TB_T_SR_ATTACHMENT TB_T_SR_ATTACHMENT { get; set; }
    }
}