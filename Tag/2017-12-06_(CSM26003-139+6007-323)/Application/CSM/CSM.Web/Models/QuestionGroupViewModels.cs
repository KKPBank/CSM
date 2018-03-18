using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CSM.Common.Resources;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class QuestionGroupViewModel
    {
        public IEnumerable<QuestionGroupItemEntity> QuestionGroupList { get; set; }
        public QuestionGroupSearchFilter SearchFilter { get; set; }
        public List<SelectListItem> QuestionGroupIsActiveList { get; set; }
        public QuestionGroupQuestionSearchFilter QuestionGroupQuestionSearchFilter { get; set; }
        public int? QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public bool Status { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public int UpdateUser { get; set; }
        public string UpdateUserName { get; set; }
        public List<QuestionGroupTableModel> QuestionGroupTableList { get; set; } 


    }

    public class QuestionGroupTableModel
    {
        public int? id { get; set; }
        public string question_name { get; set; }
    }

    public class QuestionGroupEditViewModel
    {
        public int? QuestionGroupId { get; set; }
        public int? QuestionId { get; set; }
        [Display(Name = "ชื่อกลุ่มคำถาม")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string QuestionGroupName { get; set; }
        public string QuestionGroupProductName { get; set; }
        public bool? Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public int UpdateUser { get; set; }
        public string UpdateUserName { get; set; }
        public string QuestionGroupProductGroup { get; set; }
        public string DisplayCreateDate { get; set; }
        public string DisplayUpdateDate { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredLookup")]
        public int? QuestionGroupProductId { get; set; }

        public string QuestionGroupDescription { get; set; }
        public QuestionSelectSearchFilter SearchFilter { get; set; }
        public IEnumerable<QuestionItemEntity> QuestionList { get; set; }
        public QuestionGroupInQuestionSearchFilter QuestionGroupInQuestionSearchFilter { get; set; }
        public IEnumerable<QuestionGroupQuestionItemEntity> QuestionGroupInQuestionList { get; set; }

        public string QuestionGroupProductIds { get; set; }

    }
    public class QuestionGroupSaveViewModel
    {
        public int? QuestionGroupId { get; set; }
        public int? QuestionId { get; set; }
        public string QuestionGroupName { get; set; }
        public string QuestionGroupProduct { get; set; }
        public string QuestionGroupProductGroup { get; set; }
        public bool Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public int UpdateUser { get; set; }
        public string UpdateUserName { get; set; }
        public string QuestionGroupProductIds { get; set; }
        public string Description { get; set; }
    }

    public class QuestionGroupProductViewModel
    {
        public string Product { get; set; }
        public string ProductGroup { get; set; }

    } 
}