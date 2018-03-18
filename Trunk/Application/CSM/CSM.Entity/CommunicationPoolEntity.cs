using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class CommunicationPoolEntity
    {
        #region "Local Declaration"

        private bool m_IsDeleted = true;

        #endregion

        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactSurname { get; set; }
        public string Subject { get; set; }
        public int JobId { get; set; }
        public int SequenceNo { get; set; }
        public int? SRId { get; set; }
        public string SRNo { get; set; }
        public string SRStatusCode { get; set; }
        public ServiceRequestEntity ServiceRequest { get; set; }
        public List<AttachmentEntity> Attachments { get; set; }
        public string AttachmentsDisplay { get; set; }
        public PoolEntity PoolEntity { get; set; }
        public ChannelEntity ChannelEntity { get; set; }
        public DateTime RecvDateTime { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        [AllowHtml]
        public string PlainText { get; set; }

        public int MessageNumber { get; set; }
        public UserEntity UpdateUser { get; set; }
        public short? Status { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Remark { get; set; }
        public string UID { get; set; }

        public string StatusDisplay
        {
            get { return Constants.JobStatus.GetMessage(this.Status); }
        }
        
        public string UpdateDateDisplay
        {
            get { return UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        
        public string CreateDateDisplay
        {
            get { return CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public string ContactFullName
        {
            get
            {
                string[] names = new string[2] { this.ContactName.NullSafeTrim(), this.ContactSurname.NullSafeTrim() };

                if (names.Any(x => !string.IsNullOrEmpty(x)))
                {
                    return names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                }

                return string.Empty;
            }
        }

        public bool IsDeleted
        {
            get { return m_IsDeleted; }
            set { m_IsDeleted = value; }
        }

        public CommunicationPoolEntity()
        {
            this.Attachments = new List<AttachmentEntity>();
        }
    }

    [Serializable]
    public class CommPoolSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength", 
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstName { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastName { get; set; }
        public short? JobStatus { get; set; }
        public int? Channel { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Subject { get; set; }

        public short? SRState { get; set; }
        public short? SRStatus { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CreatorSR { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OwnerSR { get; set; }

        public int? OwnerBranchId { get; set; }
        public int? OwnerUserId { get; set; }
        public int? CreatorBranchId { get; set; }
        public int? CreatorUserId { get; set; }

        public UserEntity User { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string From { get; set; }

        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ActionBy { get; set; }

        public string DateFrom { get; set; }
        public DateTime? DateFromValue { get { return DateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public string DateTo { get; set; }
        public DateTime? DateToValue { get { return DateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public string JobDateFrom { get; set; }
        public DateTime? JobDateFromValue { get { return JobDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public string JobDateTo { get; set; }
        public DateTime? JobDateToValue { get { return JobDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SRId { get; set; }
    }
}
