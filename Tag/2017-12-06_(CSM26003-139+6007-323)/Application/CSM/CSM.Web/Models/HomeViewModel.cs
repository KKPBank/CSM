using System;
using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Web.Models
{
    [Serializable]
    public class HomeViewModel
    {
        public NewsSearchFilter NewsUnreadSearchFilter { get; set; }
        public NewsSearchFilter NewsReadSearchFilter { get; set; }
        public IEnumerable<NewsEntity> NewsUnreadList { get; set; }
        public IEnumerable<NewsEntity> NewsReadList { get; set; }
        public AcceptNewsViewModel AcceptNewsInfo { get; set; }
        public SrSearchFilter GroupSrSearchFilter { get; set; }
        public IEnumerable<SrEntity> GroupServiceRequestList { get; set; }
        public SrSearchFilter IndividualSrSearchFilter { get; set; }
        public IEnumerable<SrEntity> IndividualServiceRequestList { get; set; }
    }
}