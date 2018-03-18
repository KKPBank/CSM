using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Entity;

namespace CSM.Web.Models
{
    [Serializable]
    public class AcceptNewsViewModel
    {
        public int? NewsId { get; set; }
        public string AnnounceDate { get; set; }
        public string ExpiryDate { get; set; }
        public string FullName { get; set; } // ผู้ประกาศ
        [AllowHtml]
        public string Content { get; set; }

        public string Topic { get; set; }

        public bool IsAcceptNews { get; set; }
        public bool IsShowAcceptNews { get; set; }
        public List<AttachmentEntity> AttachmentList { get; set; }

    }
}