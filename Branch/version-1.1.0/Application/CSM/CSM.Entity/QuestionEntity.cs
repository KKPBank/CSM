using System;
using System.Collections.Generic;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class QuestionEntity
    {
        public List<QuestionItemEntity> QuestionList { get; set; }

    }

    public class QuestionItemEntity
    {
        public int? QuestionId { get; set; }
        public string QuestionName { get; set; }
        public UserEntity CreateUserName { get; set; }
        public UserEntity UpdateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Status { get; set; }
        public int UserId { get; set; }
        public int? CreateUser { get; set; }
        public int? UpdateUser { get; set; }
        public string IsActive { get; set; }
        public int SeqNo { get; set; }
    }

    public class QuestionSearchFilter : Pager
    {
        public string QuestionName { get; set; }
        public string Status { get; set; }
    }
}
