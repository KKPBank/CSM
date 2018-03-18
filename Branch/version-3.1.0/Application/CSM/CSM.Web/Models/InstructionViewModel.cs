using System;
using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    [Serializable]
    public class InstructionViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public InstructionSearchFilter SearchFilter { get; set; }
        public InstructionEntity DetailInstruction { get; set; }
        public IEnumerable<InstructionEntity> InstructionList { get; set; }
    }
}