using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class CustomerLogViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public CustomerLogSearchFilter SearchFilter { get; set; }
        public IEnumerable<CustomerLogEntity> CustomerLogList { get; set; }
    }
}