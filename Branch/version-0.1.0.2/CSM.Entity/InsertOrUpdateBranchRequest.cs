using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class InsertOrUpdateBranchRequest
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ChannelCode { get; set; }
        public string UpperBranchCode { get; set; }
        public int StartTimeHour { get; set; }
        public int StartTimeMinute { get; set; }
        public int EndTimeHour { get; set; }
        public int EndTimeMinute { get; set; }
        public string HomeNo { get; set; }
        public string Moo { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Soi { get; set; }
        public string Street { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Tambol { get; set; }
        public string Zipcode { get; set; }
        public short Status { get; set; }

        public string ActionUsername { get; set; }
    }

    public class InsertOrUpdateBranchResponse
    {
        public bool IsSuccess { get; set; }
        public bool IsNewBranch { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UpdateBranchCalendarRequest
    {
        public DateTime HolidayDate { get; set; }
        public string HolidayDesc { get; set; }
        public List<string> BranchCodeList { get; set; }
        public int UpdateMode { get; set; }
        public string ActionUsername { get; set; }
    }

    public class UpdateBranchCalendarResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
