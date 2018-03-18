using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;

namespace CSM.Web.Models
{
    [Serializable]
    public class JobViewModel
    {
        public int? JobId { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public int SequenceNo { get; set; }
        public string StatusDisplay { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Channel { get; set; }
        public string ProductGroup { get; set; }  
        public string Product { get; set; }
        public string Type { get; set; }
        public string SubArea { get; set; }   

        //SR
        public string SrNo { get; set; }
        public int SR_Status { get; set; }
        public string SrChannel { get; set; }
        public string SrSubject { get; set; }

        [AllowHtml]
        public string SrRemark { get; set; }
        public string SrOwner { get; set; }
        public string SrDelegator { get; set; }
        public string SrCreateDate { get; set; }
        public string SrCreateUser { get; set; } 
        public string SrUpdateUser { get; set; }
        public string SrUpdateDate { get; set; }
        public string SrMediaSource { get; set; } 

        //Customer
        public string FirstNameThai { get; set; }
        public string LastNameThai { get; set; }
        public string FirstNameEnglish { get; set; }
        public string LastNameEnglish { get; set; }
        public string ActionBy { get; set; }

        //Attachment
        public List<AttachmentEntity> Attachments { get; set; }
        public IEnumerable<CommunicationPoolEntity> CommunicationPoolList { get; set; }
        public CommPoolSearchFilter SearchFilter { get; set; }

        public string IsSelected { get; set; }
        
        [Display(Name = "Lbl_Remark", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string Remark { get; set; }
        public int? JobStatus { get; set; }
    }
}