using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Entity.Common;

namespace CSM.Web.Models
{
    [Serializable]
    public class BranchViewModel
    {
        public BranchSearchFilter SearchFilter { get; set; }
        public IEnumerable<BranchEntity> BranchList { get; set; }
        public List<CheckBoxes> BranchCheckBoxes { get; set; }
    }
}