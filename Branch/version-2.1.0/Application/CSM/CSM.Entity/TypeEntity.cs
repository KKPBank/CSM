using System;
using System.Collections.Generic;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class TypeEntity
    {
      public int?  TypeId {get; set;}
      public string TypeName { get; set; }
      public List<TypeItemEntity> TypeList { get; set; }
    }

    public class TypeItemEntity
    {
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public UserEntity CreateUserName { get; set; }
        public UserEntity UpdateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int? CreateUser { get; set; }
        public int? UpdateUser { get; set; }
        public string IsActive { get; set; }
    }

    [Serializable]
    public class TypeSearchFilter : Pager
    {
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string Status { get; set; }
    }
}
