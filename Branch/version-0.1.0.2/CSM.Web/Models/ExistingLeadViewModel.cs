using System;
using CSM.Service.Messages.Common;

namespace CSM.Web.Models
{
    [Serializable]
    public class ExistingLeadViewModel
    {
        public Ticket Ticket { get; set; }
        public CustomerInfoViewModel CustomerInfo { get; set; }
    }
}