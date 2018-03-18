using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class SrPageEntity
    {
        public List<SrPageItemEntity> SrPageList { get; set; } 
    }

    public class SrPageItemEntity
    {
        //public int SrPageId { get; set; }
        public int? SrPageId { get; set; }
        public string SrPageCode { get; set; }
        public string SrPageName { get; set; }
    }
}
