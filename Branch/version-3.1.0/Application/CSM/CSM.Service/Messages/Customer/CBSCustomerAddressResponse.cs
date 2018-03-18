using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Service.Messages.Customer
{
    public class CBSCustomerAddressResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public List<CSM.Entity.CustomerAddressEntity> CustomerAddress { get; set; }
    }
}
