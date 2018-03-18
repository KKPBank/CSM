using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSM.Entity;
using System.ComponentModel.DataAnnotations;
using CSM.Common.Resources;

namespace CSM.Web.Models
{
    public class TypeViewModel
    {
        public TypeSearchFilter SearchFilter { get; set; }
        public IEnumerable<TypeItemEntity> TypeList { get; set; }
        public List<SelectListItem> TypeIsActiveList { get; set; }
        public int? TypeId { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string TypeName { get; set; }
        [Required(ErrorMessageResourceType = typeof(SRResource), ErrorMessageResourceName = "ValidationRequiredField")]
        public string TypeCode { get; set; }

        public bool Status { get; set; }

        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }

        public int? CreateUser { get; set; }
        public UserEntity CreateUserName { get; set; }
        public int? UpdateUser { get; set; }
        public UserEntity UpdateUserName { get; set; }

    }

    public class TypeSaveViewModel
    {
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public int? CreateUser { get; set; }
        public UserEntity CreateUserName { get; set; }
        public int? UpdateUser { get; set; }
        public UserEntity UpdateUserName { get; set; }
    }
}