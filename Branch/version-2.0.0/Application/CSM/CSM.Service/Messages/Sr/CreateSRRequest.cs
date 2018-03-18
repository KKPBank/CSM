using System.Runtime.Serialization;
using CSM.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CSM.Service.Messages.Branch;

namespace CSM.Service.Messages.Sr
{
    [DataContract]
    public class CreateSRRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public string CustomerSubscriptionTypeCode { get; set; }

        [DataMember(Order = 3)]
        public string CustomerCardNo { get; set; }

        [DataMember(Order = 4)]
        public string AccountNo { get; set; }

        [DataMember(Order = 5)]
        public string ContactSubscriptionTypeCode { get; set; }

        [DataMember(Order = 6)]
        public string ContactCardNo { get; set; }

        [DataMember(Order = 7)]
        public string ContactAccountNo { get; set; }

        [DataMember(Order = 8)]
        public string ContactRelationshipName { get; set; }

        [DataMember(Order = 9)]
        public string Subject { get; set; }

        [DataMember(Order = 10)]
        public string Remark { get; set; }

        [DataMember(Order = 11)]
        public string CallID { get; set; }

        [DataMember(Order = 12)]
        public string ANo { get; set; }

        //public string ProductGroupCode { get; set; }
        //public string ProductCode { get; set; }

        [DataMember(Order = 13)]
        public string CampaignServiceCode { get; set; }

        [DataMember(Order = 14)]
        public decimal AreaCode { get; set; }

        [DataMember(Order = 15)]
        public decimal SubAreaCode { get; set; }

        [DataMember(Order = 16)]
        public decimal TypeCode { get; set; }

        [DataMember(Order = 17)]
        public string ChannelCode { get; set; }

        [DataMember(Order = 18)]
        public string MediaSourceName { get; set; }

        [DataMember(Order = 19)]
        public string CreatorEmployeeCode { get; set; }

        [DataMember(Order = 20)]
        public string OwnerEmployeeCode { get; set; }

        [DataMember(Order = 21)]
        public string DelegateEmployeeCode { get; set; }

        [DataMember(Order = 22)]
        public string SRStatusCode { get; set; }

        //public string SRPageCode { get; set; }
        //public bool? IsVerify { get; set; }

        [DataMember(Order = 23)]
        public string IsVerifyPass { get; set; }

        [DataMember(Order = 24)]
        public string DefaultHouseNo { get; set; }

        [DataMember(Order = 25)]
        public string DefaultVillage { get; set; }

        [DataMember(Order = 26)]
        public string DefaultBuilding { get; set; }

        [DataMember(Order = 27)]
        public string DefaultFloorNo { get; set; }

        [DataMember(Order = 28)]
        public string DefaultRoomNo { get; set; }

        [DataMember(Order = 29)]
        public string DefaultMoo { get; set; }

        [DataMember(Order = 30)]
        public string DefaultSoi { get; set; }

        [DataMember(Order = 31)]
        public string DefaultStreet { get; set; }

        [DataMember(Order = 32)]
        public string DefaultTambol { get; set; }

        [DataMember(Order = 33)]
        public string DefaultAmphur { get; set; }

        [DataMember(Order = 34)]
        public string DefaultProvince { get; set; }

        [DataMember(Order = 35)]
        public string DefaultZipCode { get; set; }

        [DataMember(Order = 36)]
        public string AFSAssetNo { get; set; }

        [DataMember(Order = 37)]
        public DateTime? NCBCustomerBirthDate { get; set; }

        [DataMember(Order = 38)]
        public string NCBMarketingEmployeeCode { get; set; }

        [DataMember(Order = 39)]
        public string NCBCheckStatus { get; set; }

        [DataMember(Order = 40)]
        public string ActivityDescription { get; set; }

        [DataMember(Order = 41)]
        public int ActivityTypeId { get; set; }

        [DataMember(Order = 42)]
        public bool? IsSendDelegateEmail { get; set; }

        [DataMember(Order = 43)]
        public bool? IsSendEmail { get; set; }

        [DataMember(Order = 44)]
        public string SendEmailSender { get; set; }

        [DataMember(Order = 45)]
        public string SendEmailTo { get; set; }

        [DataMember(Order = 46)]
        public string SendEmailCc { get; set; }

        [DataMember(Order = 47)]
        public string SendEmailSubject { get; set; }

        [DataMember(Order = 48)]
        public string SendEmailBody { get; set; }

        [IgnoreDataMember]
        public string AddressDiplay
        {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(DefaultHouseNo) ? string.Format(CultureInfo.InvariantCulture, "เลขที่ {0} ", DefaultHouseNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultVillage) ? string.Format(CultureInfo.InvariantCulture, " หมู่บ้าน {0} ", DefaultVillage) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultBuilding) ? string.Format(CultureInfo.InvariantCulture, " อาคาร {0} ", DefaultBuilding) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultFloorNo) ? string.Format(CultureInfo.InvariantCulture, " ชั้น {0} ", DefaultFloorNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultRoomNo) ? string.Format(CultureInfo.InvariantCulture, " ห้อง {0} ", DefaultRoomNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultMoo) ? string.Format(CultureInfo.InvariantCulture, " หมู่ {0} ", DefaultMoo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultStreet) ? string.Format(CultureInfo.InvariantCulture, " ถนน {0} ", DefaultStreet) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultSoi) ? string.Format(CultureInfo.InvariantCulture, " ซอย {0} ", DefaultSoi) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultTambol) ? string.Format(CultureInfo.InvariantCulture, " แขวง {0} ", DefaultTambol) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultAmphur) ? string.Format(CultureInfo.InvariantCulture, " เขต {0} ", DefaultAmphur) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultProvince) ? string.Format(CultureInfo.InvariantCulture, " จังหวัด {0} ", DefaultProvince) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultZipCode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", DefaultZipCode) : string.Empty;
                // strAddress += !string.IsNullOrEmpty(DefaultCountry) ? string.Format(" ประเทศ {0} ", DefaultCountry) : string.Empty;
                return strAddress;
            }
        }
    }

    [DataContract]
    public class CreateSRResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public string SRNo { get; set; }

        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class UpdateSRRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public string SRNo { get; set; }

        [DataMember(Order = 3)]
        public bool? IsUpdateInfo { get; set; }

        [DataMember(Order = 4)]
        public string Subject { get; set; }

        [DataMember(Order = 5)]
        public string Remark { get; set; }

        [DataMember(Order = 6)]
        public string OwnerEmployeeCode { get; set; }

        [DataMember(Order = 7)]
        public string DelegateEmployeeCode { get; set; }

        [DataMember(Order = 8)]
        public string SRStatusCode { get; set; }

        [DataMember(Order = 9)]
        public string DefaultHouseNo { get; set; }

        [DataMember(Order = 10)]
        public string DefaultVillage { get; set; }

        [DataMember(Order = 11)]
        public string DefaultBuilding { get; set; }

        [DataMember(Order = 12)]
        public string DefaultFloorNo { get; set; }

        [DataMember(Order = 13)]
        public string DefaultRoomNo { get; set; }

        [DataMember(Order = 14)]
        public string DefaultMoo { get; set; }

        [DataMember(Order = 15)]
        public string DefaultSoi { get; set; }

        [DataMember(Order = 16)]
        public string DefaultStreet { get; set; }

        [DataMember(Order = 17)]
        public string DefaultTambol { get; set; }

        [DataMember(Order = 18)]
        public string DefaultAmphur { get; set; }

        [DataMember(Order = 19)]
        public string DefaultProvince { get; set; }

        [DataMember(Order = 20)]
        public string DefaultZipCode { get; set; }
        
        [DataMember(Order = 21)]
        public string AFSAssetNo { get; set; }

        [DataMember(Order = 22)]
        public DateTime? NCBCustomerBirthDate { get; set; }

        [DataMember(Order = 23)]
        public string NCBMarketingEmployeeCode { get; set; }

        [DataMember(Order = 24)]
        public string NCBCheckStatus { get; set; }

        [DataMember(Order = 25)]
        public string ActivityDescription { get; set; }

        [DataMember(Order = 26)]
        public int ActivityTypeId { get; set; }

        [DataMember(Order = 27)]
        public bool? IsSendDelegateEmail { get; set; }

        [DataMember(Order = 28)]
        public bool? IsSendEmail { get; set; }

        [DataMember(Order = 29)]
        public string SendEmailSender { get; set; }

        [DataMember(Order = 30)]
        public string SendEmailTo { get; set; }

        [DataMember(Order = 31)]
        public string SendEmailCc { get; set; }

        [DataMember(Order = 32)]
        public string SendEmailSubject { get; set; }

        [DataMember(Order = 33)]
        public string SendEmailBody { get; set; }

        [DataMember(Order = 34)]
        public string UpdateByEmployeeCode { get; set; }

        [IgnoreDataMember]
        public string AddressDiplay
        {
            get
            {
                string strAddress = "";
                strAddress += !string.IsNullOrEmpty(DefaultHouseNo) ? string.Format(CultureInfo.InvariantCulture, "เลขที่ {0} ", DefaultHouseNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultVillage) ? string.Format(CultureInfo.InvariantCulture, " หมู่บ้าน {0} ", DefaultVillage) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultBuilding) ? string.Format(CultureInfo.InvariantCulture, " อาคาร {0} ", DefaultBuilding) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultFloorNo) ? string.Format(CultureInfo.InvariantCulture, " ชั้น {0} ", DefaultFloorNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultRoomNo) ? string.Format(CultureInfo.InvariantCulture, " ห้อง {0} ", DefaultRoomNo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultMoo) ? string.Format(CultureInfo.InvariantCulture, " หมู่ {0} ", DefaultMoo) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultStreet) ? string.Format(CultureInfo.InvariantCulture, " ถนน {0} ", DefaultStreet) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultSoi) ? string.Format(CultureInfo.InvariantCulture, " ซอย {0} ", DefaultSoi) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultTambol) ? string.Format(CultureInfo.InvariantCulture, " แขวง {0} ", DefaultTambol) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultAmphur) ? string.Format(CultureInfo.InvariantCulture, " เขต {0} ", DefaultAmphur) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultProvince) ? string.Format(CultureInfo.InvariantCulture, " จังหวัด {0} ", DefaultProvince) : string.Empty;
                strAddress += !string.IsNullOrEmpty(DefaultZipCode) ? string.Format(CultureInfo.InvariantCulture, " รหัสไปรษณีย์ {0}", DefaultZipCode) : string.Empty;
                //strAddress += !string.IsNullOrEmpty(DefaultCountry) ? string.Format(" ประเทศ {0} ", DefaultCountry) : string.Empty;
                return strAddress;
            }
        }
    }

    [DataContract]
    public class UpdateSRResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public string SRNo { get; set; }

        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class SearchSRRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public string CustomerCardNo { get; set; }

        [DataMember(Order = 3)]
        public string CustomerSubscriptionTypeCode { get; set; }

        [DataMember(Order = 4)]
        public string AccountNo { get; set; }

        [DataMember(Order = 5)]
        public string ContactCardNo { get; set; }

        [DataMember(Order = 6)]
        public string ContactSubscriptionTypeCode { get; set; }

        [DataMember(Order = 7)]
        public string ProductGroupCode { get; set; }

        [DataMember(Order = 8)]
        public string ProductCode { get; set; }

        [DataMember(Order = 9)]
        public string CampaignServiceCode { get; set; }

        [DataMember(Order = 10)]
        public decimal AreaCode { get; set; }

        [DataMember(Order = 11)]
        public decimal SubAreaCode { get; set; }

        [DataMember(Order = 12)]
        public decimal TypeCode { get; set; }

        [DataMember(Order = 13)]
        public string ChannelCode { get; set; }

        [DataMember(Order = 14)]
        public string EmployeeCodeforOwnerSR { get; set; }

        [DataMember(Order = 15)]
        public string EmployeeCodeforDelegateSR { get; set; }

        [DataMember(Order = 16)]
        public string SRStatusCode { get; set; }

        [DataMember(Order = 17)]
        public string ActivityTypeCode { get; set; }

        [DataMember(Order = 18)]
        public int StartPageIndex { get; set; }

        [DataMember(Order = 19)]
        public int PageSize { get; set; }
    }

    [DataContract]
    public class SearchSRResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 3)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 4)]
        public int StartPageIndex { get; set; }

        [DataMember(Order = 5)]
        public int PageSize { get; set; }

        [DataMember(Order = 6)]
        public int TotalRecords { get; set; }

        [DataMember(Order = 7)]
        public List<SearchSRResponseItem> SearchSRResponseItems { get; set; }
    }

    public class SearchSRResponseItem
    {
        public int SrId { get; set; }
        public string SrNo { get; set; }
        public int? ThisAlert { get; set; }
        public DateTime? NextSLA { get; set; }
        public int? TotalWorkingHours { get; set; }
        public string CustomerFirstNameTh { get; set; }
        public string CustomerLastNameTh { get; set; }
        public string CustomerFirstNameEn { get; set; }
        public string CustomerLastNameEn { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerSubscriptionTypeCode { get; set; }
        public string CustomerSubscriptionTypeName { get; set; }
        public string CustomerCardNo { get; set; }
        public string AccountNo { get; set; }
        public string ContactSubscriptionTypeCode { get; set; }
        public string ContactCardNo { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string ANo { get; set; }
        public string CallId { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceCode { get; set; }
        public string CampaignServiceName { get; set; }
        public decimal? AreaCode { get; set; }
        public string AreaName { get; set; }
        public decimal? SubAreaCode { get; set; }
        public string SubAreaName { get; set; }
        public decimal? TypeCode { get; set; }
        public string TypeName { get; set; }
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public string ActivityTypeName { get; set; }
        public string MediaSourceName { get; set; }
        public string SrStatusCode { get; set; }
        public string SrStatusName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string OwnerUserEmployeeCode { get; set; }
        public string OwnerUserPositionCode { get; set; }
        public string OwnerUserFirstName { get; set; }
        public string OwnerUserLastName { get; set; }
        public string OwnerUserFullName { get; set; }
        public string OwnerBranchName { get; set; }
        public string DelegateUserEmployeeCode { get; set; }
        public string DelegateUserPosition { get; set; }
        public string DelegateUserFirstName { get; set; }
        public string DelegateUserLastName { get; set; }
        public string DelegateUserFullName { get; set; }
        public string DelegateBranchName { get; set; }
        public bool? IsVerifyQuestion { get; set; }
        public string VerifyQuestionPass { get; set; }
        public string DefaultHouseNo { get; set; }
        public string DefaultMoo { get; set; }
        public string DefaultVillage { get; set; }
        public string DefaultBuilding { get; set; }
        public string DefaultFloorNo { get; set; }
        public string DefaultRoomNo { get; set; }
        public string DefaultSoi { get; set; }
        public string DefaultStreet { get; set; }
        public string DefaultTambol { get; set; }
        public string DefaultAmphur { get; set; }
        public string DefaultProvince { get; set; }
        public string DefaultZipCode { get; set; }
        public string AFSAssetNo { get; set; }
        public string AFSAssetDesc { get; set; }
        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBCheckStatus { get; set; }
        public string NCBMarkeingFullName { get; set; }
        public string NCBMarkeingBranchName { get; set; }
        public string NCBMarkeingBranchUpper1Name { get; set; }
        public string NCBMarkeingBranchUpper2Name { get; set; }

        public void ReFormatData()
        {
            this.CustomerFirstName = GetCustomerFirstName();
            this.CustomerLastName = GetCustomerLastName();
            this.OwnerUserFullName = GetOwnerUserFullName();
            this.DelegateUserFullName = GetDelegateUserFullName();
        }

        public string GetCustomerFirstName()
        {
            if (!string.IsNullOrEmpty(CustomerFirstNameTh))
                return CustomerFirstNameTh;

            return CustomerFirstNameEn;
        }

        public string GetCustomerLastName()
        {
            if (!string.IsNullOrEmpty(CustomerFirstNameTh))
                return CustomerLastNameTh;

            return CustomerLastNameEn;
        }

        public string GetOwnerUserFullName()
        {
            string[] names = new string[2] { this.OwnerUserFirstName.NullSafeTrim(), this.OwnerUserLastName.NullSafeTrim() };

            if (names.Any(x => !string.IsNullOrEmpty(x)))
            {
                string positionCode = this.OwnerUserPositionCode.NullSafeTrim();
                string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
            }

            return string.Empty;
        }

        public string GetDelegateUserFullName()
        {
            string[] names = new string[2] { this.DelegateUserFirstName.NullSafeTrim(), this.DelegateUserLastName.NullSafeTrim() };

            if (names.Any(x => !string.IsNullOrEmpty(x)))
            {
                string positionCode = this.DelegateUserPosition.NullSafeTrim();
                string fullName = names.Where(x => !string.IsNullOrEmpty(x)).Aggregate((i, j) => i + " " + j);
                return !string.IsNullOrEmpty(positionCode) ? string.Format(CultureInfo.InvariantCulture, "{0} - {1}", positionCode, fullName) : fullName;
            }

            return string.Empty;
        }
    }

    [DataContract]
    public class GetSRResponse
    {
        [DataMember(Order = 1)]
        public bool IsSuccess { get; set; }

        [DataMember(Order = 2)]
        public bool IsFound { get; set; }

        [DataMember(Order = 3)]
        public string ErrorCode { get; set; }

        [DataMember(Order = 4)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 5)]
        public int SRId { get; set; }

        [DataMember(Order = 6)]
        public string SRNo { get; set; }

        [DataMember(Order = 7)]
        public bool SLA { get; set; }

        [DataMember(Order = 8)]
        public int AlertNo { get; set; }

        [DataMember(Order = 9)]
        public DateTime NextSLA { get; set; }

        [DataMember(Order = 10)]
        public int TotalWorkingHours { get; set; }

        [DataMember(Order = 11)]
        public string CustomerSubscriptionType { get; set; }

        [DataMember(Order = 12)]
        public string CustomerCardNo { get; set; }

        [DataMember(Order = 13)]
        public string AccountNO { get; set; }

        [DataMember(Order = 14)]
        public string ContactSubscriptionType { get; set; }

        [DataMember(Order = 15)]
        public string ContactCardNo { get; set; }

        [DataMember(Order = 16)]
        public string Subject { get; set; }

        [DataMember(Order = 17)]
        public string Remark { get; set; }

        [DataMember(Order = 18)]
        public string ANo { get; set; }

        [DataMember(Order = 19)]
        public string ProductGroupName { get; set; }

        [DataMember(Order = 20)]
        public string ProductName { get; set; }

        [DataMember(Order = 21)]
        public string CampaignServiceName { get; set; }

        [DataMember(Order = 22)]
        public string AreaName { get; set; }

        [DataMember(Order = 23)]
        public string SubAreaName { get; set; }

        [DataMember(Order = 24)]
        public string TypeName { get; set; }

        [DataMember(Order = 25)]
        public string SRChannelName { get; set; }

        [DataMember(Order = 26)]
        public string MediaSourceName { get; set; }

        [DataMember(Order = 27)]
        public string CurrentSRStatus { get; set; }

        [DataMember(Order = 28)]
        public string CreateDate { get; set; }

        [DataMember(Order = 29)]
        public string CloseDate { get; set; }

        [DataMember(Order = 30)]
        public DateTime? CreateDateDt { get; set; }

        [DataMember(Order = 31)]
        public DateTime? CloseDateDt { get; set; }

        [DataMember(Order = 32)]
        public string OwnerSREmployeeCode { get; set; }

        [DataMember(Order = 33)]
        public string DelegateSREmployeeCode { get; set; }

        [DataMember(Order = 34)]
        public bool IsVerifyQuestion { get; set; }

        [DataMember(Order = 35)]
        public string VerifyQuestionPass { get; set; }

        [DataMember(Order = 36)]
        public string DefaultHouseNo { get; set; }

        [DataMember(Order = 37)]
        public string DefaultVillage { get; set; }

        [DataMember(Order = 38)]
        public string DefaultBuilding { get; set; }

        [DataMember(Order = 39)]
        public string DefaultFloorNo { get; set; }

        [DataMember(Order = 40)]
        public string DefaultRoomNo { get; set; }

        [DataMember(Order = 41)]
        public string DefaultMoo { get; set; }

        [DataMember(Order = 42)]
        public string DefaultSoi { get; set; }

        [DataMember(Order = 43)]
        public string DefaultStreet { get; set; }

        [DataMember(Order = 44)]
        public string DefaultTambol { get; set; }

        [DataMember(Order = 45)]
        public string DefaultAmphur { get; set; }

        [DataMember(Order = 46)]
        public string DefaultProvince { get; set; }

        [DataMember(Order = 47)]
        public string DefaultZipCode { get; set; }

        [DataMember(Order = 48)]
        public string AFSAssetNo { get; set; }

        [DataMember(Order = 49)]
        public DateTime? NCBCustomerBirthDate { get; set; }

        [DataMember(Order = 50)]
        public string NCBMarketingEmployeeCode { get; set; }

        [DataMember(Order = 51)]
        public string NCBCheckStatus { get; set; }
    }

    [DataContract]
    public class GetSRRequest
    {
        [DataMember(Order = 1)]
        public WebServiceHeader Header { get; set; }

        [DataMember(Order = 2)]
        public string SrNo { get; set; }
    }
}
