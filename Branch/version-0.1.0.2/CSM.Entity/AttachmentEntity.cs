using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;

namespace CSM.Entity
{
    [Serializable]
    public class AttachmentEntity
    {
        #region "Local Declaration"
        private bool m_IsDelete = false;
        #endregion

        public int AttachmentId { get; set; }
        public int? CommPoolId { get; set; }
        public int? NewsId { get; set; }
        public string Url { get; set; }
        public string TempPath { get; set; }
        public string ContentType { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] ByteArray { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateUserId { get; set; }
        public List<AttachmentTypeEntity> AttachTypeList { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string DocumentLevel { get; set; }        
        public string SrNo { get; set; }
        public short? Status { get; set; }
        public int? CustomerId { get; set; }

        public string FileExtension { get; set; }

        public int? FileSize { get; set; }

        public bool IsDelete
        {
            get { return m_IsDelete; }
            set { m_IsDelete = value; }
        }

        public string CreateDateDisplay
        {
            get { return CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }
        public string ExpiryDateDisplay
        {
            get { return ExpiryDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate); }
        }

        public string StatusDisplay
        {
            get { return Constants.ApplicationStatus.GetMessage(this.Status); }
        }

        public string CreateUserFullName { get; set; }

        //Motif
        public int? SrId { get; set; }
        public int? SrActivityId { get; set; }
        public int? SrAttachmentId { get; set; }
    }

    [Serializable]
    public class AttachmentTypeEntity : DocumentTypeEntity
    {
        public int? AttachmentId { get; set; }
        public int? CreateUserId { get; set; }
    }

    public class AttachmentSearchFilter : Pager
    {
        public int? CustomerId { get; set; }        
    }
}
