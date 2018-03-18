using System;
using System.Collections.Generic;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Utilities;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CSM.Web.Models
{
    [Serializable]
    public class ContactViewModel
    {
        public int? RelationshipId { get; set; }

        [Display(Name = "Lbl_RelationName",ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedStringLengthAttribute(Constants.MaxLength.RelationshipName, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RelationshipName { get; set; }

        [Display(Name = "Lbl_RelationDesc", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedStringLengthAttribute(Constants.MaxLength.RelationshipDesc, ErrorMessageResourceName = "ValErr_StringLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string RelationshipDesc { get; set; }

        [Display(Name = "Lbl_Status_Thai", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public short? Status { get; set; }

        public SelectList StatusList { get; set; }
      
        public string CreateUser { get; set; } // Login user
        public string UpdateUser { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; } 

        public IEnumerable<RelationshipEntity> RelationshipList { get; set; }
        public RelationshipSearchFilter SearchFilter { get; set; }
    }

    [Serializable]
    public class CustomerContactViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public ContactSearchFilter SearchFilter { get; set; }
        public IEnumerable<ContactEntity> ContactList { get; set; }

    }

    [Serializable]
    public class ContactEditViewModel
    {
        #region "Local Declaration"
        private string m_JsonContactRelationship;
        private List<CustomerContactEntity> m_ContactRelationship;
        #endregion

        public SelectList SubscriptTypeList { get; set; }
        public SelectList TitleThaiList { get; set; }
        public SelectList TitleEnglishList { get; set; }
        public SelectList PhoneTypeList { get; set; }
        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public int? RelationshipId { get; set; }
        public int? ContactId { get; set; }
        public string SubscriptType { get; set; }
        public string TitleThai { get; set; }
        [Display(Name = "Lbl_FirstNameThai_Contact", ResourceType = typeof(CSM.Common.Resources.Resource))]        
        //[LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        [LocalizedRegex(@"([\-ก-๙0-9()., ]+)", "ValErr_NoSpecialCharacterThai")]
        public string FirstNameThai { get; set; }
        [Display(Name = "Lbl_LastNameThai_Contact", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex(@"([\-ก-๙0-9()., ]+)", "ValErr_NoSpecialCharacterThai")]
        public string LastNameThai { get; set; }
        public string TitleEnglish { get; set; }
        [Display(Name = "Lbl_FirstNameEnglish", ResourceType = typeof(CSM.Common.Resources.Resource))]       
        [LocalizedRegex(@"([\-a-zA-Z0-9()., ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string FirstNameEnglish { get; set; }
        [LocalizedRegex(@"([\-a-zA-Z0-9()., ]+)", "ValErr_NoSpecialCharacterEnglish")]
        public string LastNameEnglish { get; set; }
        public string CardNo { get; set; }
        public string BirthDate { get; set; }
        public DateTime? BirthDateValue { get { return BirthDate.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        [Display(Name = "Lbl_Email", ResourceType = typeof(CSM.Common.Resources.Resource))]
        //[DataType(DataType.EmailAddress)]
        //[LocalizedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", "ValErr_InvalidEmail")]
        public string Email { get; set; }
        [Display(Name = "Lb_PhoneNo_1", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string PhoneType1 { get; set; }
        public string PhoneType2 { get; set; }
        public string PhoneType3 { get; set; }
        [Display(Name = "Lb_PhoneNo_1", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string PhoneNo1 { get; set; }
        [Display(Name = "Lb_PhoneNo_2", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string PhoneNo2 { get; set; }
        [Display(Name = "Lb_PhoneNo_3", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9#]+)", "ValErr_NumericAndExtOnly")]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public string PhoneNo3 { get; set; }
        [Display(Name = "Lbl_Fax", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]     
        public string Fax { get; set; }
        public string IsEdit { get; set; } // 1 = true, 0 = false        
        public string IsConfirm { get; set; } // 1 = true, 0 = false     
        public string IsNameConfirm { get; set; }

        public IEnumerable<ContactEntity> ContactList { get; set; }

        public string JsonContactRelationship
        {
            get { return m_JsonContactRelationship; }
            set
            {
                m_JsonContactRelationship = value;
                try
                {
                    m_ContactRelationship = JsonConvert.DeserializeObject<List<CustomerContactEntity>>(m_JsonContactRelationship);
                }
                catch (Exception)
                {
                    m_ContactRelationship = new List<CustomerContactEntity>();
                }
            }
        }

        public List<CustomerContactEntity> ContactRelationshipList
        {
            get { return m_ContactRelationship; }
            set { m_ContactRelationship = value; }
        }
    }

    [Serializable]
    public class ContactRelationshipViewModel
    {
        #region "Local Declaration"

        private string m_JsonContactRelationship;
        private List<CustomerContactEntity> m_ContactRelationship;     
  
        #endregion

        public SelectList AccountList { get; set; }

        public SelectList RelationshipList { get; set; }

        public int? ListIndex { get; set; }
        public int? CustomerId { get; set; }
        public int? ContactId { get; set; }
        [Display(Name = "Lbl_AccountNo", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int? AccountId { get; set; }
        public string Product { get; set; }
        [Display(Name = "Lbl_Relationship", ResourceType = typeof(CSM.Common.Resources.Resource))]
        [LocalizedRequired(ErrorMessage = "ValErr_RequiredField")]
        public int RelationshipId { get; set; }

        public string JsonContactRelationship
        {
            get { return m_JsonContactRelationship; }
            set
            {
                m_JsonContactRelationship = value;
                try
                {
                    m_ContactRelationship = JsonConvert.DeserializeObject<List<CustomerContactEntity>>(m_JsonContactRelationship);
                }
                catch (Exception)
                {
                    m_ContactRelationship = new List<CustomerContactEntity>();
                }
            }
        }

        public List<CustomerContactEntity> ContactRelationshipList
        {
            get { return m_ContactRelationship; }
            set { m_ContactRelationship = value; }
        }
    }

}