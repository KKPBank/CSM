using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Entity;
using CSM.Service.CBSAccountService;

namespace CSM.Service.Messages.Customer
{
    public class CBSCasaAccountDetailResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public InquiryCASAAccountDetailResponse CasaDetail { get; set; }
    }
}
