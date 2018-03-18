using System;
using System.Collections.Generic;
using CSM.Entity;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;
using System.Web.Mvc;
using CSM.Entity.Common;
using Newtonsoft.Json;

namespace CSM.Web.Models
{
    [Serializable]
    public class ConfigurationViewModel
    {
        #region "Local Declaration"
        private string m_JsonRole;
        private List<RoleEntity> m_Roles;
        #endregion

        [Display(Name = "Lbl_SystemName", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.ConfigName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SystemName { get; set; }

        [Display(Name = "Lbl_Url", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.ConfigUrl, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedRegex(@"(http|https):\/\/[\w-]+(\.[\w-]+)+([\w.,@?^=%&amp;:\/~+#-]*[\w@?^=%&amp;\/~+#-])?", "ValErr_InvalidURL")]
        public string Url { get; set; }

        [Display(Name = "Lbl_MainMenu", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? MenuId { get; set; }

        //[Display(Name = "Lbl_ImageAttach", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[LocalizedRequired(ErrorMessage = "ValErr_Required")]
        public HttpPostedFileBase File { get; set; }

        public List<CheckBoxes> RoleCheckBoxes { get; set; }
        public int? ConfigureUrlId { get; set; }
        public short? Status { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList MenuList { get; set; }
        public IEnumerable<ConfigureUrlEntity> ConfigureUrlList { get; set; }
        public ConfigureUrlSearchFilter SearchFilter { get; set; }
        public string CreateUser { get; set; } // Login user
        public string UpdateUser { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; }
        public string FileUrl { get; set; }

        [Display(Name = "Displayed Image")]        
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string FontName { get; set; }

        public string JsonRole
        {
            get { return m_JsonRole; }
            set
            {
                m_JsonRole = value;
                try
                {
                    m_Roles = JsonConvert.DeserializeObject<List<RoleEntity>>(m_JsonRole);
                }
                catch (Exception)
                {
                    m_Roles = new List<RoleEntity>();
                }
            }
        }

        public List<RoleEntity> RoleList
        {
            get { return m_Roles; }
            set { m_Roles = value; }
        }

        public List<FontViewModel> FontList { get; set; }
        public int? PageIndexOfFont { get; set; }
    }

    public class FontViewModel
    {
        public string Font_1 { get; set; }
        public string Font_2 { get; set; }
        public string Font_3 { get; set; }
        public string Font_4 { get; set; }       
    }
}