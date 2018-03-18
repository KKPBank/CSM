using System.Collections.Generic;

namespace CSM.Service.Messages.Common
{
    public class StatusResponse
    {
        public string Description { get; set; }
        public string ErrorCode { get; set; }
        public string Status { get; set; }
        public List<string> BranchCodeNotFoundList { get; set; } 
    }
}
