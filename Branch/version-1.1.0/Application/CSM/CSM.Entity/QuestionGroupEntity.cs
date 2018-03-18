using System;
using System.Collections.Generic;
using CSM.Entity.Common;

namespace CSM.Entity
{
    public class QuestionGroupEntity
    {
        public List<QuestionGroupItemEntity> QuestionGroupList { get; set; }
        public List<QuestionGroupQuestionItemEntity> QuestionGroupQuestionList { get; set; }

    }

    public class QuestionGroupProductEntity
    {
        public List<QuestionGroupProductEntity> QuestionGroupProductList { get; set; }
        public string Product { get; set; }
        public string ProductGroup { get; set; }
    }

    public class QuestionGroupItemEntity
    {
        public int? QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public string QuestionGroupProduct { get; set; }
        public int QuestionGroupProductId { get; set; }
        public UserEntity CreateUserName { get; set; }
        public UserEntity UpdateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Status { get; set; }
        public int UserId { get; set; }
        public int CreateUser { get; set; }
        public int UpdateUser { get; set; }
        public int QuestionNo { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }

    public class QuestionGroupDuplicateEntity
    {
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class QuestionGroupTableItemEntity
    {
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public bool? IsActive { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int QuestionNo { get; set; }
    }

    public class QuestionGroupQuestionItemEntity
    {
        public int? QuestionGroupQuestionId { get; set; }
        public int? QuestionGroupId { get; set; }
        public int? QuestionId { get; set; }
        public string QuestionName { get; set; }
        public int? SeqNo { get; set; }
        public string QuestionGroupQuestionName { get; set; }
        public bool Status { get; set; }
        public UserEntity CreateUserName { get; set; }
        public UserEntity UpdateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int CreateUser { get; set; }
        public int UpdateUser { get; set; }
        public int UserId { get; set; }
    }

    public class QuestionGroupQuestionEntity
    {
        public List<QuestionGroupQuestionItemEntity> QuestionGroupQuestionList { get; set; }
    }
    [Serializable]
    public class QuestionGroupSearchFilter : Pager
    {
        public string QuestionGroupName { get; set; }
        public int? ProductId { get; set; }
        public string Status { get; set; }
    }
    [Serializable]
    public class QuestionGroupQuestionSearchFilter : Pager
    {
        
    }
    [Serializable]
    public class QuestionSelectSearchFilter : Pager
    {
        public string QuestionName { get; set; }
        public string QuestionIdList { get; set; }
        public int? ProductId { get; set; }
    }

    public class QuestionGroupInQuestionSearchFilter : Pager
    {
        public int? QuestionGroupId { get; set; }
    }

    public class QuestionGroupEditTableItemEntity
    {
        public int MapProductQuestionGroupId { get; set; }
        public int MapProductId { get; set; }
        public int QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public int QuestionNo { get; set; }
        public int PassAmount { get; set; }
        public int Seq { get; set; }
    }

}
