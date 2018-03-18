using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;
using System.ComponentModel.DataAnnotations;

namespace CSM.Service.Messages.Customer
{
    [MessageContract]
    public class ImportSubscriptionRequest : IValidatableObject
    {
        [MessageHeader]
        public Header Header { get; set; }

        [MessageBodyMember]
        public string ImportDate { get; set; }

        [MessageBodyMember]
        public bool SkipSFTP { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.ImportDate))
            {
                results.Add(new ValidationResult("ImportDate is required."));
            }

            if (!string.IsNullOrWhiteSpace(this.ImportDate))
            {
                if (this.ImportDateValue == null)
                {
                    results.Add(new ValidationResult("ImportDate is invalid date format"));
                } 
            }

            return results;
        }

        [IgnoreDataMember]
        public DateTime? ImportDateValue
        {
            get { return this.ImportDate.ParseDateTime(Constants.DateTimeFormat.StoreProcedureDate); }
        } 
    }
}
