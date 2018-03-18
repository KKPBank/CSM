using CSM.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class CreateSRRequest
    {
        public string CustomerSubscriptionTypeCode { get; set; }
        public string CustomerCardNo { get; set; }
        public string AccountNo { get; set; }
        public string ContactSubscriptionTypeCode { get; set; }
        public string ContactCardNo { get; set; }
        public string ContactAccountNo { get; set; }
        public string ContactRelationshipName { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string CallID { get; set; }
        public string ANo { get; set; }
//        public string ProductGroupCode { get; set; }
//        public string ProductCode { get; set; }
        public string CampaignServiceCode { get; set; }
        public decimal AreaCode { get; set; }
        public decimal SubAreaCode { get; set; }
        public decimal TypeCode { get; set; }
        public string ChannelCode { get; set; }
        public string MediaSourceName { get; set; }
        public string CreatorEmployeeCode { get; set; }
        public string OwnerEmployeeCode { get; set; }
        public string DelegateEmployeeCode { get; set; }
        public string SRStatusCode { get; set; }
//        public string SRPageCode { get; set; }
//        public bool?   IsVerify { get; set; }
        public string IsVerifyPass { get; set; }
        public string DefaultHouseNo { get; set; }
        public string DefaultVillage { get; set; }
        public string DefaultBuilding { get; set; }
        public string DefaultFloorNo { get; set; }
        public string DefaultRoomNo { get; set; }
        public string DefaultMoo { get; set; }
        public string DefaultSoi { get; set; }
        public string DefaultStreet { get; set; }
        public string DefaultTambol { get; set; }
        public string DefaultAmphur { get; set; }
        public string DefaultProvince { get; set; }
        public string DefaultZipCode { get; set; }

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
//                strAddress += !string.IsNullOrEmpty(DefaultCountry) ? string.Format(" ประเทศ {0} ", DefaultCountry) : string.Empty;

                return strAddress;
            }
        }

        public string AFSAssetNo { get; set; }
        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBMarketingEmployeeCode { get; set; }
        public string NCBCheckStatus { get; set; }
        public string ActivityDescription { get; set; }
        public int    ActivityTypeId { get; set; }
        public bool?  IsSendDelegateEmail { get; set; }
        public bool?  IsSendEmail { get; set; }
        public string SendEmailSender { get; set; }
        public string SendEmailTo { get; set; }
        public string SendEmailCc { get; set; }
        public string SendEmailSubject { get; set; }
        public string SendEmailBody { get; set; }
    }

    public class CreateSRResponse
    {
        public bool IsSuccess { get; set; }
        public string SRNo { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class UpdateSRRequest
    {
        public string SRNo { get; set; }
        public bool? IsUpdateInfo { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string OwnerEmployeeCode { get; set; }
        public string DelegateEmployeeCode { get; set; }
        public string SRStatusCode { get; set; }
        public string DefaultHouseNo { get; set; }
        public string DefaultVillage { get; set; }
        public string DefaultBuilding { get; set; }
        public string DefaultFloorNo { get; set; }
        public string DefaultRoomNo { get; set; }
        public string DefaultMoo { get; set; }
        public string DefaultSoi { get; set; }
        public string DefaultStreet { get; set; }
        public string DefaultTambol { get; set; }
        public string DefaultAmphur { get; set; }
        public string DefaultProvince { get; set; }
        public string DefaultZipCode { get; set; }

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
                //                strAddress += !string.IsNullOrEmpty(DefaultCountry) ? string.Format(" ประเทศ {0} ", DefaultCountry) : string.Empty;

                return strAddress;
            }
        }

        public string AFSAssetNo { get; set; }
        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBMarketingEmployeeCode { get; set; }
        public string NCBCheckStatus { get; set; }
        public string ActivityDescription { get; set; }
        public int ActivityTypeId { get; set; }
        public bool? IsSendDelegateEmail { get; set; }
        public bool? IsSendEmail { get; set; }
        public string SendEmailSender { get; set; }
        public string SendEmailTo { get; set; }
        public string SendEmailCc { get; set; }
        public string SendEmailSubject { get; set; }
        public string SendEmailBody { get; set; }
        public string UpdateByEmployeeCode { get; set; }
    }

    public class UpdateSRResponse
    {
        public bool IsSuccess { get; set; }
        public string SRNo { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class SearchSRRequest
    {
        public string CustomerCardNo { get; set; }
        public string CustomerSubscriptionTypeCode { get; set; }
        public string AccountNo { get; set; }
        public string ContactCardNo { get; set; }
        public string ContactSubscriptionTypeCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public string CampaignServiceCode { get; set; }
        public decimal AreaCode { get; set; }
        public decimal SubAreaCode { get; set; }
        public decimal TypeCode { get; set; }
        public string ChannelCode { get; set; }
        public string EmployeeCodeforOwnerSR { get; set; }
        public string EmployeeCodeforDelegateSR { get; set; }
        public string SRStatusCode { get; set; }
        public string ActivityTypeCode { get; set; }

        public int StartPageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchSRResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public int StartPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

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
            else
                return CustomerFirstNameEn;
        }
        public string GetCustomerLastName()
        {
            if (!string.IsNullOrEmpty(CustomerFirstNameTh))
                return CustomerLastNameTh;
            else
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

    public class GetSRResponse
    {
        public bool IsSuccess { get; set; }
        public bool IsFound { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }


        public int SRId { get; set; }
        public string SRNo { get; set; }

        public bool SLA { get; set; }
        public int AlertNo { get; set; }
        public DateTime NextSLA { get; set; }
        public int TotalWorkingHours { get; set; }

        public string CustomerSubscriptionType { get; set; }
        public string CustomerCardNo { get; set; }
        public string AccountNO { get; set; }
        public string ContactSubscriptionType { get; set; }
        public string ContactCardNo { get; set; }
        public string Subject { get; set; }
        public string Remark { get; set; }
        public string ANo { get; set; }

        public string ProductGroupName { get; set; }
        public string ProductName { get; set; }
        public string CampaignServiceName { get; set; }
        public string AreaName { get; set; }
        public string SubAreaName { get; set; }
        public string TypeName { get; set; }

        public string SRChannelName { get; set; }
        public string MediaSourceName { get; set; }


        public string CurrentSRStatus { get; set; }
        public string CreateDate { get; set; }
        public string CloseDate { get; set; }
        public string OwnerSREmployeeCode { get; set; }
        public string DelegateSREmployeeCode { get; set; }


        public bool IsVerifyQuestion { get; set; }
        public string VerifyQuestionPass { get; set; }
        public string DefaultHouseNo { get; set; }
        public string DefaultVillage { get; set; }
        public string DefaultBuilding { get; set; }
        public string DefaultFloorNo { get; set; }
        public string DefaultRoomNo { get; set; }
        public string DefaultMoo { get; set; }
        public string DefaultSoi { get; set; }
        public string DefaultStreet { get; set; }
        public string DefaultTambol { get; set; }
        public string DefaultAmphur { get; set; }
        public string DefaultProvince { get; set; }
        public string DefaultZipCode { get; set; }
        public string AFSAssetNo { get; set; }
        public DateTime? NCBCustomerBirthDate { get; set; }
        public string NCBMarketingEmployeeCode { get; set; }
        public string NCBCheckStatus { get; set; }

    }
}
