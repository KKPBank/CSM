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
    
    public partial class TB_T_NEWS_ATTACHMENT
    {
        public TB_T_NEWS_ATTACHMENT()
        {
            this.TB_T_ATTACHMENT_TYPE = new HashSet<TB_T_ATTACHMENT_TYPE>();
        }
    
        public int NEWS_ATTACHMENT_ID { get; set; }
        public Nullable<int> NEWS_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string CONTENT_TYPE { get; set; }
        public string URL { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string ATTACHMENT_DESC { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public Nullable<int> FILE_SIZE { get; set; }
    
        public virtual ICollection<TB_T_ATTACHMENT_TYPE> TB_T_ATTACHMENT_TYPE { get; set; }
        public virtual TB_T_NEWS TB_T_NEWS { get; set; }
    }
}
