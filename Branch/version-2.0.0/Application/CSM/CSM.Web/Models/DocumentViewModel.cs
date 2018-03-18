using CSM.Entity;
using System;
using System.Collections.Generic;

namespace CSM.Web.Models
{
     [Serializable]
    public class DocumentViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public AttachmentSearchFilter SearchFilter { get; set; }
        public IEnumerable<AttachmentEntity> AttachmentList { get; set; }
    }
}