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
    public partial class TB_C_PARAMETER
    {
        public int PARAMETER_ID { get; set; }
        public string PARAMETER_NAME { get; set; }
        public string PARAMETER_VALUE { get; set; }
        public string PARAMETER_DESC { get; set; }
        public Nullable<int> UPDATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string PARAMTER_TYPE { get; set; }
    }
}
