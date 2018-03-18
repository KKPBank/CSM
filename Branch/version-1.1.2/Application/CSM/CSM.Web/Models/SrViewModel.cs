using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    public class SrViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public SrSearchFilter SearchFilter { get; set; }
        public IEnumerable<SrEntity> SrList { get; set; }
    }
}