using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Web.Models
{
    [Serializable]
    public class ActivityViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public ActivitySearchFilter SearchFilter { get; set; }
        public IEnumerable<ServiceRequestActivityResult> ActivityList { get; set; }
    }
}