using System;
using System.Collections.Generic;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class BankEntity
    {
        public int BankId { get; set; }
        public string BankNo { get; set; }
        public string BankName { get; set; }
        public int? Status { get; set; }

        //public string BankDisplay
        //{
        //    get {
        //        string ret = BankNo;
        //        if (BankName != "")
        //            ret += " - " + BankName;

        //        return ret;
        //    }
        //}
    }
}
