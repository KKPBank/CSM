using System;

namespace CSM.Entity
{
    [Serializable]
    public class AccountTypeEntity
    {
        public int AccountTypeId { get; set; }
        public string SystemCode { get; set; }
        public string AccountTypeCode { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeName { get; set; }
    }
}
