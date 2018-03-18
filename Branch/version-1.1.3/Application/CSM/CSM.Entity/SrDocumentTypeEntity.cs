using CSM.Entity.Common;
using System;
using System.Collections.Generic;

namespace CSM.Entity
{
    public class SrDocumentTypeEntity
    {
        public int SrAttachId { get; set; }
        public string SrAttachName { get; set; }
        public bool? AttachToEmail { get; set; }
        public string SrAttachDesc { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? CreateDate { get; set; }

        public string ExpireDateDisplay { get; set; }
        public List<CheckBoxes> SelectAttachArray { get; set; }
        public string SelectAttachStr { get; set; }

    }
}