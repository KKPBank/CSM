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
    
    public partial class TB_T_NEWS
    {
        public TB_T_NEWS()
        {
            this.TB_T_NEWS_ATTACHMENT = new HashSet<TB_T_NEWS_ATTACHMENT>();
            this.TB_T_READ_NEWS = new HashSet<TB_T_READ_NEWS>();
            this.TB_T_NEWS_BRANCH = new HashSet<TB_T_NEWS_BRANCH>();
        }
    
        public int NEWS_ID { get; set; }
        public string TOPIC { get; set; }
        public string CONTENT { get; set; }
        public Nullable<System.DateTime> ANNOUNCE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public Nullable<short> STATUS { get; set; }
    
        public virtual ICollection<TB_T_NEWS_ATTACHMENT> TB_T_NEWS_ATTACHMENT { get; set; }
        public virtual ICollection<TB_T_READ_NEWS> TB_T_READ_NEWS { get; set; }
        public virtual ICollection<TB_T_NEWS_BRANCH> TB_T_NEWS_BRANCH { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
    }
}