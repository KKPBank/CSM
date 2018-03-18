using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity;
using Newtonsoft.Json;

namespace CSM.Web.Models
{
    [Serializable]
    public class NewsViewModel
    {
        #region "Local Declaration"
        private string m_JsonBranch;
        private string m_JsonAttach;
        private List<NewsBranchEntity> m_SelectedBranch;
        private List<AttachmentEntity> m_Attach;
        #endregion

        public NewsSearchFilter SearchFilter { get; set; }
        public IEnumerable<NewsEntity> NewsList { get; set; }
        public List<NewsBranchEntity> NewsBranchList { get; set; }
        public SelectList StatusList { get; set; }

        [Display(Name = "Lbl_Topic", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string Topic { get; set; }

        [Display(Name = "Lbl_Status_Thai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public short? Status { get; set; }        
        public int? NewsId { get; set; }
        public int? BranchId { get; set; }

        [Display(Name = "Lbl_AnnounceDate", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string AnnounceDate { get; set; }
        public string ExpiryDate { get; set; }
        public string FullName { get; set; } // ผู้ประกาศ

        [Display(Name = "Lbl_Content", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [AllowHtml]
        public string Content { get; set; }
        
        public string JsonBranch
        {
            get { return m_JsonBranch; }
            set
            {
                m_JsonBranch = value;
                try
                {
                    m_SelectedBranch = JsonConvert.DeserializeObject<List<NewsBranchEntity>>(m_JsonBranch);
                }
                catch (Exception)
                {
                    m_SelectedBranch = new List<NewsBranchEntity>();
                }
            }
        }

        public List<NewsBranchEntity> SelectedBranch
        {
            get { return m_SelectedBranch; }
            set { m_SelectedBranch = value; }
        }

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

        public List<AttachmentEntity> AttachmentList
        {
            get { return m_Attach; }
            set { m_Attach = value; }
        }

        public DateTime? AnnounceDateValue { get { return AnnounceDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? ExpiryDateValue { get { return ExpiryDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        public NewsViewModel()
        {
            this.m_SelectedBranch = new List<NewsBranchEntity>();
            this.m_Attach = new List<AttachmentEntity>();
        }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; }
    }
}