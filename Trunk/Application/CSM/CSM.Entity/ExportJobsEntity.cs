using System;
using CSM.Common.Utilities;
using CSM.Entity.Common;

namespace CSM.Entity
{
    [Serializable]
    public class ExportJobsEntity
    {
        public string No { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobType { get; set; }
        public string JobStatus { get; set; }
        public DateTime? JobDateTime { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string SRID { get; set; }
        public string SRCreator { get; set; }
        public string SROwner { get; set; }
        public string SRState { get; set; }
        public string SRStatus { get; set; }
        public string AttachFile { get; set; }
        public string Remark { get; set; }
        public string PoolName { get; set; }

        public string JobDateDisplay
        {
            get { return JobDateTime.FormatDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
    }

    [Serializable]
    public class ExportJobsSearchFilter : Pager
    {
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstName { get; set; }
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LastName { get; set; }
        public short? JobStatus { get; set; }
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FromValue { get; set; }
        public string AttachFile { get; set; }
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Subject { get; set; }
        public string ActionBy { get; set; }
        public string ActionBranch { get; set; }
        public string JobDateFrom { get; set; }
        public string JobDateTo { get; set; }
        public DateTime? JobDateFromValue { get { return JobDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? JobDateToValue { get { return JobDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        
        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string JobTimeFrom { get; set; }
        
        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string JobTimeTo { get; set; }

        public string OwnerSR { get; set; }
        public string OwnerBranch { get; set; }
        public string ActionDateFrom { get; set; }
        public string ActionDateTo { get; set; }
        public DateTime? ActionDateFromValue { get { return ActionDateFrom.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }
        public DateTime? ActionDateToValue { get { return ActionDateTo.ParseDateTime(Constants.DateTimeFormat.DefaultShortDate); } }

        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string ActionTimeFrom { get; set; }

        [LocalizedRegex("^([1-9]|[0-1][0-9]|[2][0-3]):([0-5][0-9])$", "ValErr_InvalidTimeFormat")]
        public string ActionTimeTo { get; set; }
        
        public string CreatorSR { get; set; }
        public string CreatorBranch { get; set; }

        public UserEntity LoginUser { get; set; }
        
        [LocalizedRegex("([0-9]+)", "ValErr_NumericOnly")]
        [LocalizedMinLengthAttribute(Constants.MinLenght.SearchTerm, ErrorMessageResourceName = "ValErr_MinLength",
            ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string SRId { get; set; }

        public DateTime? JobDateTimeFromValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(JobTimeFrom) ? (JobTimeFrom + ":00") : "00:00:00";
                return (JobDateFrom + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public DateTime? JobDateTimeToValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(JobTimeTo) ? (JobTimeTo + ":59") : "23:59:59";
                return (JobDateTo + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public DateTime? ActionDateTimeFromValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(ActionTimeFrom) ? (ActionTimeFrom + ":00") : "00:00:00";
                return (ActionDateFrom + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }
        
        public DateTime? ActionDateTimeToValue
        {
            get
            {
                string strTime = !string.IsNullOrEmpty(ActionTimeTo) ? (ActionTimeTo + ":59") : "23:59:59";
                return (ActionDateTo + ' ' + strTime).ParseDateTime(Constants.DateTimeFormat.DefaultFullDateTime);
            }
        }

        public string JobStatusDesc { get; set; }
        public string AttachFileDesc { get; set; }
        public string ActionByName { get; set; }
        public string ActionBranchName { get; set; }
        public string OwnerSRName { get; set; }
        public string OwnerBranchName { get; set; }
        public string CreatorSRName { get; set; }
        public string CreatorBranchName { get; set; }
    }
}
