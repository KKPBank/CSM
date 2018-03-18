using CSM.Common.Utilities;
using CSM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Web.Models
{
    [Serializable]
    public class TDAccountInfoViewModel
    {
        public int? CustomerNumber { get; set; }
        public string CustomerType { get; set; }
        public string CountryOfCitizenship { get; set; }
        public string PrimaryName { get; set; }
        public string PrimaryLastName { get; set; }
        public string IDNumber { get; set; }
        public string Mobile
        {
            get { return StringHelpers.ConvertListToString(MobileList.Select(x => x.PhoneNo).ToList<object>(), ","); }
        }
        public List<PhoneEntity> MobileList { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string AccountNo { get; set; }
    }
}