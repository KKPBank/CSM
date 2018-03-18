using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;

namespace CSM.Entity
{
    [Serializable]
    public class NoteEntity
    {
        public int? NoteId { get; set; }
        public int? CustomerId { get; set; }
        public string Detail { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public UserEntity CreateUser { get; set; }
        public UserEntity UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string EffectiveDateDisplay
        {
            get
            {
                return EffectiveDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
            }
        }

        public string ExpiryDateDisplay
        {
            get
            {
                return ExpiryDate.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate);
            }
        }
        public string CreateDateDisplay
        {
            get
            {
                return CreateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public string UpdateDateDisplay
        {
            get
            {
                return UpdateDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }
    }

    public class NoteSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
