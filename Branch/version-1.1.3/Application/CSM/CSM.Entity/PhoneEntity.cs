namespace CSM.Entity
{
    public class PhoneEntity
    {
        public int? PhoneId { get; set; }
        public int? PhoneTypeId { get; set; }       
        public string PhoneNo { get; set; }
        public string PhoneTypeName { get; set; }
        public string PhoneTypeCode { get; set; }
        public int? CustomerId { get; set; }
        public int? ContactId { get; set; }
    }

    public class PhoneTypeEntity
    {      
        public int? PhoneTypeId { get; set; }       
        public string PhoneTypeName { get; set; }
        public string PhoneTypeCode { get; set; }
    }
}
