using CSM.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class AfsexportEntity
    {
        public DateTime? CreatedDate { get; set; }

        public string CreatedDateDisplay
        {
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.FormatDateTime(Constants.DateTimeFormat.DefaultShortDate) : "";
            }
        }
        public string CreatedBy { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public string PhoneNo
        {
            get
            {
                return StringHelpers.ConvertListToString(PhoneList.Select(x => x.PhoneNo).ToList<object>(), ",");
            }
        }

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Property { get; set; }
        public string AssetInspection { get; set; }
        public string CallBackRequest { get; set; }
        public string DocumentRequest { get; set; }
        public string LocationEnquiry { get; set; }
        public string PriceEnquiry { get; set; }
        public string PriceIssuedCallBack { get; set; }
        public string CallBackRequired { get; set; }
        public string MediaSource { get; set; }

        public List<PhoneEntity> PhoneList { get; set; }
        public int SR_ID { get; set; }
    }
}
