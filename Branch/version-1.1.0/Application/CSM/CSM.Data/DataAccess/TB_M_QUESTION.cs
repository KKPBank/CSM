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
    
    public partial class TB_M_QUESTION
    {
        public TB_M_QUESTION()
        {
            this.TB_M_QUESTIONGROUP_QUESTION = new HashSet<TB_M_QUESTIONGROUP_QUESTION>();
            this.TB_T_SR_VERIFY_RESULT_QUESTION = new HashSet<TB_T_SR_VERIFY_RESULT_QUESTION>();
        }
    
        public int QUESTION_ID { get; set; }
        public string QUESTION_NAME { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
        public Nullable<int> CREATE_USER { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
    
        public virtual ICollection<TB_M_QUESTIONGROUP_QUESTION> TB_M_QUESTIONGROUP_QUESTION { get; set; }
        public virtual TB_R_USER TB_R_USER { get; set; }
        public virtual TB_R_USER TB_R_USER1 { get; set; }
        public virtual ICollection<TB_T_SR_VERIFY_RESULT_QUESTION> TB_T_SR_VERIFY_RESULT_QUESTION { get; set; }
    }
}