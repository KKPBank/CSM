using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Entity.Common;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace CSM.Web.Models
{
    [Serializable]
    public class AttachViewModel
    {
        #region "Local Declaration"
        private string m_DocDesc = string.Empty;
        private string m_JsonAttach;
        private List<AttachmentEntity> m_Attach;

        private string m_JsonAttachType;
        private List<AttachmentTypeEntity> m_AttachType;        
        #endregion

        public int? ListIndex { get; set; }
        public int? CustomerId { get; set; }
        public int? NewsId { get; set; }

        [Display(Name = "Lbl_FileAttach", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public HttpPostedFileBase FileAttach { get; set; }

        [Display(Name = "Lbl_AttachName", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.AttachName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string DocName { get; set; }
        
        [Display(Name = "Lbl_AttachDesc", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.AttachDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string DocDesc
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(m_DocDesc))
                {
                    return m_DocDesc.ReplaceEmptyLine();
                }

                return m_DocDesc;
            }
            set { m_DocDesc = value; }
        }
       
        public string Filename { get; set; }
        public string ExpiryDate { get; set; }
        public List<CheckBoxes> DocTypeCheckBoxes { get; set; }

        public string JsonAttach
        {
            get { return m_JsonAttach; }
            set
            {
                m_JsonAttach = value;
                try
                {
                    m_Attach = JsonConvert.DeserializeObject<List<AttachmentEntity>>(m_JsonAttach);
                }
                catch (Exception)
                {
                    m_Attach = new List<AttachmentEntity>();
                }
            }
        }

        public string AttachTypeDisplay { get; set; }        

        public List<AttachmentEntity> AttachmentList
        {
            get { return m_Attach; }
            set { m_Attach = value; }
        }

        public int? AttachmentId { get; set; }

        public string JsonAttachType
        {
            get { return m_JsonAttachType; }
            set
            {
                m_JsonAttachType = value;
                try
                {
                    m_AttachType = JsonConvert.DeserializeObject<List<AttachmentTypeEntity>>(m_JsonAttachType);
                }
                catch (Exception)
                {
                    m_AttachType = new List<AttachmentTypeEntity>();
                }
            }
        }

        public List<AttachmentTypeEntity> AttachTypeList
        {
            get { return m_AttachType; }
            set { m_AttachType = value; }
        }

        //public SelectList StatusList { get; set; }
        public short? Status { get; set; }
        public string StatusDisplay
        {
            get { return Constants.ApplicationStatus.GetMessage(this.Status); }
        }
        public string CreateUserFullName { get; set; }

    }
}