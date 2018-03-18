using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CSM.Common.Resources;
using CSM.Entity;

namespace CSM.Web.Models
{
    public class QuestionViewModel
    {
        public QuestionSearchFilter SearchFilter { get; set; }
        public IEnumerable<QuestionItemEntity> QuestionList { get; set; }
        public List<SelectListItem> QuestionIsActiveList { get; set; }
        public int? QuestionId { get; set; }
        [Display(Name = "Question")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string QuestionName { get; set; }
        public bool? Status { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int? CreateUser { get; set; }
        public UserEntity CreateUserName { get; set; }
        public int? UpdateUser { get; set; }
        public UserEntity UpdateUserName { get; set; }

    }

    public class QuestionSaveViewModel
    {
        public int? QuestionId { get; set; }
        public string QuestionName { get; set; }
        public bool? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int? CreateUser { get; set; }
        public UserEntity CreateUserName { get; set; }
        public int? UpdateUser { get; set; }
        public UserEntity UpdateUserName { get; set; }
    }
}