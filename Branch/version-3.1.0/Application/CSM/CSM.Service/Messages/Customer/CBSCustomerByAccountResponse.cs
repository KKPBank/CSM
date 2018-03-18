using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Entity;

namespace CSM.Service.Messages.Customer
{
    public class CBSCustomerByAccountResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public IEnumerable<CustomerEntity> CustomerList { get; set; }
    }
}
