using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.OTP
{
    [DataContract]
    public class OTPResultSvcRequest : IValidatableObject
    {
        [DataMember(IsRequired = true, Order = 0)]
        public Header Header { get; set; }

        [DataMember(IsRequired = true, Order = 1), MaxLength(30)]
        public string CALL_ID { get; set; }

        [DataMember(IsRequired = true, Order = 2), MaxLength(30)]
        public string TXN_ID { get; set; }

        [DataMember(IsRequired = true, Order = 3), MaxLength(10)]
        public string VERIFY_OTP_STATUS { get; set; }

        [DataMember(Order = 4), MaxLength(10)]
        public string VERIFY_OTP_FAILURE_CODE { get; set; }

        [DataMember(Order = 5), MaxLength(255)]
        public string VERIFY_OTP_FAILURE_REASON { get; set; }

        [DataMember(Order = 6), MaxLength(10)]
        public string CAA_FAILURE_CODE { get; set; }

        [DataMember(Order = 7), MaxLength(255)]
        public string CAA_FAILURE_REASON { get; set; }

        [DataMember(Order = 8), MaxLength(10)]
        public string MOBILE_PHONE { get; set; }

        [DataMember(Order = 9), MaxLength(255)]
        public string OTP_PREFIX { get; set; }

        [DataMember(Order = 10), MaxLength(255)]
        public string OTP_DETAIL { get; set; }

        [DataMember(Order = 11), MaxLength(255)]
        public string OTP_SUFFIX { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            //if (this.ProductGroup == null)
            //{
            //    results.Add(new ValidationResult("ProductGroup object is required."));
            //}
            return results;
        }
    }

    [DataContract]
    public class OTPResultSvcResponse : IValidatableObject
    {
        [DataMember(IsRequired = true, Order = 0)]
        public Header Header { get; set; }

        [DataMember(IsRequired = true, Order = 1), MaxLength(10)]
        public string STATUS { get; set; }

        [DataMember(Order = 2), MaxLength(10)]
        public string ERROR_CODE { get; set; }

        [DataMember(Order = 3), MaxLength(255)]
        public string ERROR_DESC { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            //if (this.ProductGroup == null)
            //{
            //    results.Add(new ValidationResult("ProductGroup object is required."));
            //}
            return results;
        }
    }
}
