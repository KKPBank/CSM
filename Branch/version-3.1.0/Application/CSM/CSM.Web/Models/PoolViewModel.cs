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
    public class PoolViewModel
    {
        #region "Local Declaration"
        private string m_JsonBranch;
        private List<PoolBranchEntity> m_SelectedBranch;
        #endregion

        public PoolSearchFilter SearchFilter { get; set; }
        public IEnumerable<PoolEntity> PoolList { get; set; }
        public List<PoolBranchEntity> PoolBranchList { get; set; }
        public SelectList StatusList { get; set; }

        [Display(Name = "Lbl_PoolName", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.PoolName, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PoolName { get; set; }

        [Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        //[LocalizedRegex(@"^(([^<>()[\]\\.,;:\s@\""]+"
        //                + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
        //                + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
        //                + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
        //                + @"[a-zA-Z]{2,}))$", "ValErr_InvalidEmail")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Email, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Email { get; set; }

        [Display(Name = "Lbl_Status", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public short? Status { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Lbl_Password", ResourceType = typeof (CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Password, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Lbl_ConfirmPasswd", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.Password, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string ConfirmPasswd { get; set; }

        [LocalizedStringLengthAttribute(Constants.MaxLength.PoolDesc, ErrorMessageResourceName = "ValErr_StringLength",
           ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PoolDesc { get; set; }

        public int? PoolId { get; set; }
        public int? BranchId { get; set; }
        
        public string JsonBranch
        {
            get { return m_JsonBranch; }
            set
            {
                m_JsonBranch = value;
                try
                {
                    m_SelectedBranch = JsonConvert.DeserializeObject<List<PoolBranchEntity>>(m_JsonBranch);
                }
                catch (Exception)
                {
                    m_SelectedBranch = new List<PoolBranchEntity>();
                }
            }
        }
        
        public List<PoolBranchEntity> SelectedBranch
        {
            get { return m_SelectedBranch; }
            set { m_SelectedBranch = value; }
        }

        public PoolViewModel()
        {
            this.m_SelectedBranch = new List<PoolBranchEntity>();
        }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; } 
    }
}