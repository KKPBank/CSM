using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.Activity;

namespace CSM.Web.Models
{
    [Serializable]
    public class ActivityViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public ActivitySearchFilter SearchFilter { get; set; }
        public IEnumerable<ActivityDataItem> ActivityList { get; set; }
    }
}