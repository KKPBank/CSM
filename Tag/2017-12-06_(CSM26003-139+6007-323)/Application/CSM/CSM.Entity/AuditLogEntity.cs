using System;
using System.Web.Mvc;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class AuditLogEntity
    {
        public int? AuditLogId { get; set; }
        public string IpAddress { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string Detail { get; set; }

        [AllowHtml]
        public string DetailDisplay
        {
            get
            {
                string content = "";

                if (!string.IsNullOrWhiteSpace(this.Detail))
                {
                    content = this.Detail.TextToHtml();
                }

                return content;
            }
        }

        public LogStatus Status { get; set; }
        public string StatusDisplay
        {
            get { return Status.ToShortString(); }
        }

        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedDateDisplay
        {
            get { return CreatedDate.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime); }
        }

        public UserEntity User { get; set; }
    }

    [Serializable]
    public class AuditLogSearchFilter : Pager
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public DateTime? DateFromValue { get { return DateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? DateToValue { get { return DateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public string Module { get; set; }
        public string Action { get; set; }
        public short? Status { get; set; }
    }

    public enum LogStatus
    {
        Fail = 0,
        Success = 1
    }

    public static class LogStatusExtension
    {
        public static string ToShortString(this LogStatus flag)
        {
            switch (flag)
            {
                case LogStatus.Fail:
                    return "Fail";
                case LogStatus.Success:
                    return "Success";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
