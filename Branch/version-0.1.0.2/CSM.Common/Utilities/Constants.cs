using System.Net;
using CSM.Common.Resources;

///<summary>
/// Class Name : Constants
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.Common.Utilities
{
    public static class Constants
    {
        public const int CompanyStartYear = 2013;
        public const string ConfigUrlPath = "~/Templates/ConfigUrl/";
        public const string NoImage50 = "~/Images/no_image_50.png";
        public const string NoImage30 = "~/Images/no_image_30.png";
        public const string PassPhrase = "gdupi9bok8bo";
        public const string UnknownFileExt = ".unknown";
        public const string NotKnown = "NA";

        public const int BatchInboundActivityTypeId = 2;
        public const int EmailInboundActivityTypeId = 6;
        public const int EmailOutboundActivityTypeId = 7;
        public const int CallCenterChannelId = 2;
        public const int DisplayMaxLength = 60;
        public const int ActivityDescriptionMaxLength = 8000;
        
        public const string DefaultSubscriptionTypeForUser = "19";

        public static class ApplicationStatus
        {
            public const int All = -1;
            public const int Active = 1;
            public const int Inactive = 0;

            public static string GetMessage(short? status)
            {
                //if (status == null)
                //{
                //    return "Draft";
                //}

                if (status == Inactive)
                {
                    return Resource.Ddl_Status_Inactive;
                }

                if (status == Active)
                {
                    return Resource.Ddl_Status_Active;
                }

                if (status == All)
                {
                    return Resource.Ddl_Status_All;
                }

                return string.Empty;
            }
        }

        public static class EmployeeStatus
        {
            public const int Active = 1;
            public const int Termiated = 0;

            public static string GetMessage(short? status)
            {
                if (status.HasValue)
                {
                    if (status == Active)
                    {
                        return Resource.Emp_Status_Active;
                    }
                    if (status == Termiated)
                    {
                        return Resource.Emp_Status_Termiate;
                    }
                }

                return string.Empty;
            }
        }

        public static class ReportSRStatus
        {
            public const bool Pass = true;
            public const bool Fail = false;

            public static string GetMessage(string status)
            {
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (VerifyResultStatus.Pass.Equals(status))
                    {
                        return Resource.Ddl_VerifyResult_Pass;
                    }
                    else if (VerifyResultStatus.Fail.Equals(status))
                    {
                        return Resource.Ddl_VerifyResult_Fail;
                    }
                    else
                    {
                        return Resource.Ddl_VerifyResult_Skip;
                    }
                }

                return "N/A";
            }
        }

        public static class AccountStatus
        {
            public const string Active = "A";
        }

        public static class ReportVerify
        {
            public const string Pass = VerifyResultStatus.Pass;
            public const string Fail = VerifyResultStatus.Fail;

            public static string GetMessage(string status)
            {
                if (status == Pass)
                {
                    return "ถูก";
                }
                if (status == Fail)
                {
                    return "ผิด";
                }
                return "ข้าม";
            }
        }

        public static class AuditLogStatus
        {
            public const int Success = 1;
            public const int Fail = 0;
        }

        public static class VerifyResultStatus
        {
            public const string Pass = "PASS";
            public const string Fail = "FAIL";
            public const string Skip = "SKIP";
        }

        public static class NCBCheckStatus
        {
            public const string Found = "Found";
            public const string NotFound = "Not Found";
        }

        public static class AttachFile
        {
            public const int Yes = 1;
            public const int No = 0;
        }

        public static class Module
        {
            public const string Batch = "Batch";
            public const string Authentication = "Authentication";
            public const string Customer = "Customer";
            public const string WebService = "WebService";
            public const string ServiceRequest = "ServiceRequest";
        }

        public static class AuditAction
        {
            public const string Login = "Login";
            public const string Logout = "Logout";
            public const string CreateJobs = "Create Jobs";
            public const string CloseSR = "Close SR";
            public const string ImportAFS = "Import AFS files";
            public const string CreateCommPool = "Create communication pool";
            public const string Export = "Export activity AFS";
            public const string RecommendedCampaign = "Recommended Campaign";
            public const string ExportMarketing = "Export marketing data";
            public const string ExistingLead = "Existing Lead";
            public const string ImportBDW = "Import BDW files";
            public const string ImportCIS = "Import CIS files";
            public const string ExportCIS = "Export CIS files";
            public const string ActivityLog = "Activity Log";
            public const string ImportHP = "Import HP files";
            public const string CreateProductMaster = "CreateProductMaster";

            public const string SyncSRStatusFromReplyEmail = "Sync SR Status from Reply Email";
            public const string ReSubmitActivityToCARSystem = "Re-Submit Activity to CAR System";
            public const string ReSubmitActivityToCBSHPSystem = "Re-Submit Activity to CBS-HP System";

            public const string Search = "Search";
        }

        public static class StatusType
        {
            public const string Job = "JOB";
            public const string SR = "SR";
        }

        public static class JobStatus
        {
            public const int Open = 0;
            public const int Refer = 1;
            public const int Done = 2;

            public static string GetMessage(int? status)
            {
                if (status == Open)
                {
                    return Resource.Lbl_JobStatusOpen;
                }

                if (status == Refer)
                {
                    return Resource.Lbl_JobStatusRefer;
                }

                if (status == Done)
                {
                    return Resource.Lbl_JobStatusDone;
                }

                return string.Empty;
            }
        }

        public static class CacheKey
        {
            public const string AllParameters = "CACHE_PARAMETERS"; // List of Parameters
            public const string MainMenu = "CACHE_MAINMENU";
            public const string ScreenRoles = "CACHE_SCREEN_ROLES";
            public const string CustomerTab = "CACHE_CUSTOMER_TAB";
            public const string PageSizeList = "CACHE_PAGESIZE_LIST";
        }

        public static class CultureShortName
        {
            public const string EnglishUS = "EN";
            public const string Thai = "TH";
        }

        public static class DateTimeFormat
        {
            public const string ShortTime = "HH:mm";
            public const string FullTime = "HH:mm:ss";
            public const string ShortDate = "dd MMM yyyy";
            public const string FullDateTime = "dd MMM yyyy HH:mm:ss";
            public const string DefaultShortDate = "dd/MM/yyyy";
            public const string DefaultFullDateTime = "dd/MM/yyyy HH:mm:ss";
            public const string CalendarShortDate = "dd-MM-yyyy";
            public const string CalendarFullDateTime = "dd-MM-yyyy HH:mm:ss";
            public const string StoreProcedureDate = "yyyy-MM-dd";
            public const string StoreProcedureDateTime = "yyyy-MM-dd HH:mm:ss";
            public const string ReportDateTime = "dd/MM/yyyy HH:mm:ss";
            public const string ExportDateTime = "yyyyMMdd_HHmm";
            public const string ExportCISDatetime = "dd-MMM-yyyy HH:mm:ss";
            public const string ExportAfsDateTime = "yyyyMMdd";
        }

        public static class ErrorCode
        {
            public const string CSM0001 = "CSM0001";      // CSM0001 *Connot connect to the system
            public const string CSM0002 = "CSM0002";      // CSM0002 *End point not found
            public const string CSM0003 = "CSM0003";      // CSM  Unknown Error

            public const string CSM_PROD001 = "001";
            public const string CSM_PROD002 = "002";
            public const string CSM_PROD003 = "003";
        }

        public static class KnownCulture
        {
            public const string EnglishUS = "en-US";
            public const string Thai = "th-TH";
        }

        public static class MailSubject
        {
            public const string NotifySyncEmailFailed = "NotifySyncEmailFailed";
            public const string NotifySyncEmailSuccess = "NotifySyncEmailSuccess";
            public const string NotifyImportAssetFailed = "NotifyImportAssetFailed";
            public const string NotifyImportAssetSuccess = "NotifyImportAssetSuccess";
            public const string NotifyExportActivityFailed = "NotifyExportActivityFailed";
            public const string NotifyExportActivitySuccess = "NotifyExportActivitySuccess";
            public const string NotifyFailExportActvity = "NotifyFailExportActvity";
            public const string NotifyImportContactFailed = "NotifyImportContactFailed";
            public const string NotifyImportContactSuccess = "NotifyImportContactSuccess";
            public const string NotifyImportCISSuccess = "NotifyImportCISSuccess";
            public const string NotifyImportCISFailed = "NotifyImportCISFailed";
            public const string NotifyImportHPSuccess = "NotifyImportHPSuccess";
            public const string NotifyImportHPFailed = "NotifyImportHPFailed";
            public const string NotifyCreateSrFromReplyEmailSuccess = "NotifyCreateSrFromReplyEmailSuccess";
            public const string NotifyCreateSrFromReplyEmailFailed = "NotifyCreateSrFromReplyEmailFailed";
            public const string NotifyReSubmitActivityToCARSystemSuccess = "NotifyReSubmitActivityToCARSystemSuccess";
            public const string NotifyReSubmitActivityToCARSystemFailed = "NotifyReSubmitActivityToCARSystemFailed";
            public const string NotifyReSubmitActivityToCBSHPSystemSuccess = "NotifyReSubmitActivityToCBSHPSystemSuccess";
            public const string NotifyReSubmitActivityToCBSHPSystemFailed = "NotifyReSubmitActivityToCBSHPSystemFailed";
        }

        public static class MaxLength
        {
            public const int CardNo = 20;
            public const int PhoneNo = 20;
            public const int Username = 15;
            public const int Password = 20;
            public const int AttachName = 100;
            public const int AttachDesc = 500;
            public const int IfRowStat = 50;
            public const int IfRowBatchNum = 50;
            public const int AssetNum = 50;
            public const int AssetType = 50;
            public const int AssetTradeInType = 50;
            public const int AssetStatus = 1;
            public const int AssetDesc = 200;
            public const int AssetName = 200;
            public const int AssetComments = 500;
            public const int AssetRefNo1 = 100;
            public const int AssetLot = 100;
            public const int AssetPurch = 100;
            public const int Amphur = 100;
            public const int Province = 100;
            public const int SaleName = 100;
            public const int EmployeeId = 10;
            public const int Email = 100; //50;
            public const int NewsContent = 8000;
            public const int Note = 1000;
            public const int PoolName = 200;
            public const int PoolDesc = 500;
            public const int ConfigName = 100;
            public const int ConfigUrl = 100;
            public const int ConfigImage = 100;
            public const int RelationshipName = 100;
            public const int RelationshipDesc = 255;
            public const int FirstName = 255; //100;
            public const int LastName = 255; //100;
            public const int RemarkCloseJob = 1000;

            #region "Import BwdContact"

            public const int BwdCardNo = 50;
            public const int BwdTitleTh = 50;
            public const int BwdNameTh = 255;
            public const int BwdSurnameTh = 255;
            public const int BwdTitleEn = 50;
            public const int BwdNameEn = 255;
            public const int BwdSurnameEn = 255;
            public const int BwdAccountNo = 100;
            public const int BwdLoanMain = 255;
            public const int BwdProductGroup = 255;
            public const int BwdProduct = 255;
            public const int BwdRelationship = 100;
            public const int BwdPhone = 255;
            public const int BwdCampaign = 255;
            public const int BwdCardTypeCode = 10;
            public const int BwdAccountStatus = 10;

            #endregion

            //Import CIS Title
            public const int TitleId = 10;
            public const int TitleNameTH = 100;
            public const int TitleNameEN = 100;
            public const int TitleTypeGroup = 10;
            public const int GenderCode = 10;

            //Import CIS Corporate & Individual
            public const int CustId = 50;
            public const int CarId = 50;
            //public const int Title_Id = 50;
            public const int NameTh = 255;
            public const int LastNameTh = 255;
            public const int NameEn = 255;
            public const int LastNameEn = 255;
            public const int TaxId = 50;
            public const int CusttypeGroup = 2;
            public const int CardtypeCode = 50;
            public const int KKCisId = 50;
            public const int Update_Date = 50;
            public const int RegisterDate = 50;
            public const int BirthDate = 50;
            public const int CustTypeCode = 10;

            //Import Product Group
            public const int product_code = 50;
            public const int product_type = 50;
            public const int product_desc = 100;
            public const int system = 50;
            public const int product_flag = 1;
            public const int enity_code = 10;
            public const int subscr_code = 10;
            public const int subscr_desc = 255;

            //Import Subscription
            public const int card_id = 50;
            public const int card_type_code = 10;
            public const int prod_group = 50;
            public const int prod_type = 50;
            public const int subscrcode = 50;
            public const int ref_no = 50;
            public const int branch_name = 255;
            public const int text1 = 255;
            public const int text2 = 255;
            public const int text3 = 255;
            public const int text4 = 255;
            public const int text5 = 255;
            public const int text6 = 255;
            public const int text7 = 255;
            public const int text8 = 255;
            public const int text9 = 255;
            public const int text10 = 255;
            public const int number1 = 50;
            public const int number2 = 50;
            public const int number3 = 50;
            public const int number4 = 50;
            public const int number5 = 50;
            public const int date1 = 50;
            public const int date2 = 50;
            public const int date3 = 50;
            public const int date4 = 50;
            public const int date5 = 50;
            public const int subscr_status = 1;
            public const int created_date = 50;
            public const int created_by = 100;
            public const int created_channel = 50;
            public const int updated_date = 100;
            public const int updated_by = 50;
            public const int updated_channel = 50;

            //Import CIS Province
            public const int province_code = 50;
            public const int province_name_th = 100;
            public const int province_name_en = 100;

            //Import CIS District 
            public const int distric_code = 50;
            public const int district_name_th = 50;
            public const int district_name_en = 100;

            //Import CIS Sub District
            public const int district_code = 50;
            public const int subdistrict_code = 50;
            public const int subdistrict_name_th = 100;
            public const int subdistrict_name_en = 100;
            public const int postcode = 10;

            //Import CIS Address Type
            public const int AddresstypeCode = 10;
            public const int AddresstypeName = 50;

            //Import CIS Phone Type
            public const int phone_type_code = 10;
            public const int phone_type_desc = 100;

            //Import CIS Email Type
            public const int email_type_code = 10;
            public const int email_type_desc = 100;

            //Import CIS Subscibe Address  
            public const int address_card_type_code = 10;
            public const int address_type_code = 10;
            public const int address_number = 255;
            public const int village = 255;
            public const int building = 255;
            public const int floor_no = 100;
            public const int room_no = 100;
            public const int moo = 100;
            public const int street = 255;
            public const int soi = 255;
            public const int sub_district_code = 10;
            public const int add_district_code = 10;
            public const int provice_code = 10;
            public const int country_code = 10;
            public const int postal_code = 10;
            public const int subsc_code = 10;
            public const int sub_district_value = 255;
            public const int district_value = 255;
            public const int province_value = 255;
            public const int postal_value = 10;
            public const int address_created_by = 100;
            public const int address_updated_by = 100;

            //Import CIS Subscibephone & Subscribeemial
            public const int Card_Id = 50;
            public const int CardTypeCode = 10;
            public const int Prod_Group = 10;
            public const int Prod_Type = 10;
            public const int SubscrCode = 10;
            public const int PhoneTypeCode = 10;
            public const int PhoneNum = 50;
            public const int PhoneExt = 50;
            public const int MailTypeCode = 100;
            public const int MailAccount = 100;
            public const int CreatedDate = 20;
            public const int CreatedBy = 50;
            public const int UpdatedDate = 20;
            public const int UpdatedBy = 50;

            //import Hp Activity
            public const int Channel = 30;
            public const int Type = 30;
            public const int Area = 30;
            public const int Status = 30;
            public const int Description = 150;
            public const int Comment = 1500;
            public const int AssetInfo = 15;
            public const int ContactInfo = 15;
            public const int Ano = 40;
            public const int CallId = 30;
            public const int ContactName = 50;
            public const int ContactLastName = 50;
            public const int ContactPhone = 40;
            public const int DoneFlg = 1;
            public const int CreateDate = 20;
            public const int CreateBy = 15;
            public const int StartDate = 20;
            public const int EndDate = 20;
            public const int OwnerLogin = 50;
            public const int OwnerPerId = 1;
            public const int UpdateDate = 20;
            public const int UpdateBy = 1;
            public const int SrId = 15;
            public const int CallFlg = 1;
            public const int EnqFlg = 1;
            public const int LocEnqFlg = 1;
            public const int DocReqFlg = 1;
            public const int PriIssuedFlg = 1;
            public const int AssetInspectFlg = 1;
            public const int PlanStartDate = 20;
            public const int ContactFax = 50;
            public const int ContactEmail = 50;

            //Import CustomerPhone & CustomerEmail
            public const int Cus_KKCisId = 50;
            public const int Cus_CusId = 50;
            public const int Cus_CardId = 50;
            public const int Cus_CardtypeCode = 10;
            public const int Cus_CusttypeGroup = 10;
            public const int Cus_MailtypeGroup = 10;
            public const int Cus_PhonetypeCode = 2;
            public const int Cus_PhoneNum = 100;
            public const int Cus_PhoneExt = 100;
            public const int Cus_MailAccount = 100;
            public const int Cus_CreateDate = 20;
            public const int Cus_CreateBy = 100;
            public const int Cus_UpdateDate = 20;
            public const int Cus_UpdateBy = 100;

            //Import CIS Country
            public const int CountryCode = 10;
            public const int CountryNameTh = 255;
            public const int CountryNameEn = 255;
        }

        public static class MinLenght
        {
            public const int SearchTerm = 2;
            public const int AutoComplete = 0;
        }

        public static class ParameterName
        {
            public const string AFSPathImport = "AFS_PATH_IMPORT";
            public const string AFSPathExport = "AFS_PATH_EXPORT";
            public const string AFSPathError = "AFS_PATH_ERROR";
            public const string CICPathExport = "CIC_PATH_EXPORT";
            //public const string CICPathExport = "CIC_PATH_SOURCE";
            public const string RegexFileExt = "REGEX_FILE_EXT";    // Regular Expression to validate the file extension
            //public const string RegexConfigIcon = "REGEX_CONFIG_ICON";
            public const string MaxRetrieveMail = "MAXIMUM_RETRIEVE_MAIL"; // Maximum retrieve emails by communication pool
            public const string AttachmentPathJob = "ATTACHMENT_PATH_JOB";
            public const string AttachmentPathNews = "ATTACHMENT_PATH_NEWS";
            public const string AttachmentPathCustomer = "ATTACHMENT_PATH_CUSTOMER";
            public const string AttachmentPathSr = "ATTACHMENT_PATH_SR";
            public const string ReportTimeStart = "REPORT_TIME_START";
            public const string ReportTimeEnd = "REPORT_TIME_END";
            public const string BDWPathImport = "BDW_PATH_IMPORT";
            public const string CISPathImport = "CIS_PATH_IMPORT";
            public const string CISPathError = "CIS_PATH_ERROR";
            public const string BDWPathError = "BDW_PATH_ERROR";
            public const string PageSizeStart = "PAGE_SIZE_START";
            public const string HPPathImport = "HP_PATH_IMPORT";
            public const string HPPathError = "HP_PATH_ERROR";
            public const string NumMonthsActivity = "NUM_MONTHS_ACTIVITY";
            public const string SingleFileSize = "SINGLE_FILE_SIZE";
            public const string TotalFileSize = "TOTAL_FILE_SIZE";
            public const string OfficePhoneNo = "OFFICE_PHONE_NO";
            public const string OfficeHour = "OFFICE_HOUR";
            public const string ProductGroupSubmitCBSHP = "PRODUCTGROUP_SUBMIT_CBSHP";
            public const string TextDummyAccountNo = "TEXT_DUMMY_ACCOUNT_NO";
            public const string CisPathSource = "CIS_PATH_SOURCE";
            public const string AfsPathSource = "AFS_PATH_SOURCE";
            public const string BdwPathSource = "BDW_PATH_SOURCE";
            public const string HpPathSource = "HP_PATH_SOURCE";

            public const string MaxMinuteBatchCreateSRActivityFromReplyEmail = "MAX_MINUTE_BATCH_CREATE_SR_ACTIVITY_FROM_REPLY_EMAIL";
            public const string MaxMinuteBatchReSubmitActivityToCARSystem = "MAX_MINUTE_BATCH_RESUBMIT_ACTIVITY_TO_CAR_SYSTEM";
            public const string MaxMinuteBatchReSubmitActivityToCBSHPSystem = "MAX_MINUTE_BATCH_RESUBMIT_ACTIVITY_TO_CBSHP_SYSTEM";

            public const string ReportExportDate = "REPORT_EXPORT_DATE";
        }

        public static class ServiceName
        {
            public const string CampaignByCustomer = "CampaignByCustomer";
            public const string UpdateCustomerFlags = "UpdateCustomerFlags";
            public const string InsertLead = "InsertLead";
            public const string SearchLead = "SearchLead";
            public const string CreateActivityLog = "CreateActivityLog";
            public const string InquiryActivityLog = "InquiryActivityLog";
        }

        public static class ServicesNamespace
        {
            public const string MailService = "http://www.kiatnakinbank.com/services/CSM/CSMMailService";
            public const string FileService = "http://www.kiatnakinbank.com/services/CSM/CSMFileService";
            public const string MasterService = "http://www.kiatnakinbank.com/services/CSM/CSMMasterService";
            public const string BranchService = "http://www.kiatnakinbank.com/services/CSM/CSMBranchService";
            public const string UserService = "http://www.kiatnakinbank.com/services/CSM/CSMUserService";
            public const string SRService = "http://www.kiatnakinbank.com/services/CSM/CSMSRService";
        }

        public static class StackTraceError
        {
            public const string InnerException = "[Source={0}]<br>[Message={1}]<br>[Stack trace={2}]";
            public const string Exception = "<font size='1.7'>Application Error<br>{0}</font>";
        }

        public static class StatusResponse
        {
            public const string Success = "SUCCESS";
            public const string Failed = "FAILED";
        }

        public static class TicketResponse
        {
            public const string SLM_Success = "10000";
            public const string COC_Success = "30000";
        }

        public static class ActivityResponse
        {
            public const string Success = "CAS-I-000";
        }

        public static class SystemName
        {
            public const string CSM = "CSM";
            public const string CMT = "CMT";
            public const string SLM = "SLM";
            public const string COC = "COC";
            public const string CAR = "CAR";
        }

        public static class PhoneTypeCode
        {
            public const string Mobile  = "02";
            public const string Fax     = "05"; //"FAX";
        }

        public static class DocumentCategory
        {
            public const int Customer = 1;
            public const int ServiceRequest = 2;
            public const int Announcement = 3;
        }

        public static class CustomerType
        {
            public const int Customer = 1;
            public const int Prospect = 2;
            public const int Employee = 3;

            public static string GetMessage(int? customerType)
            {
                if (customerType.HasValue)
                {
                    switch (customerType.Value)
                    {
                        case Customer:
                            return Resource.Ddl_CustomerType_Customer;
                        case Prospect:
                            return Resource.Ddl_CustomerType_Prospect;
                        case Employee:
                            return Resource.Ddl_CustomerType_Employee;
                        default:
                            return string.Empty;
                    }
                }

                return string.Empty;
            }
        }

        public static class SubscriptTypeCode
        {
            public const string Personal = "18"; //"01";
        }

        public static class ChannelCode
        {
            public const string Email = "EMAIL";
            public const string Fax = "FAX";
            public const string KKWebSite = "KKWEB";
        }

        public static class DocumentLevel
        {
            public const string Customer = "Customer";
            public const string Sr = "SR";
        }

        public static class SRPage
        {
            public const int DefaultPageId = 1;
            public const int AFSPageId = 2;
            public const int NCBPageId = 3;

            public const string DefaultPageCode = "DEFAULT";
            public const string AFSPageCode = "AFS";
            public const string NCBPageCode = "NCB";
        }

        public static class SrLogAction
        {
            public const string ChangeStatus = "Change Status";
            public const string ChangeOwner = "Change Owner";
            public const string Delegate = "Delegate";
        }


        public static class SRStatusId
        {
            public const int Draft = 1;
            public const int Open = 2;
            public const int WaitingCustomer = 3;
            public const int InProgress = 4;
            public const int RouteBack = 5;
            public const int Cancelled = 6;
            public const int Closed = 7;

            public static int[] JobOnHandStatuses { get { return new int[] { Open, WaitingCustomer, InProgress, RouteBack }; } }

            public static string GetStatusName(int id)
            {
                switch (id)
                {
                    case Draft:
                        return "Draft";
                    case Open:
                        return "Open";
                    case WaitingCustomer:
                        return "Waiting Customer";
                    case InProgress:
                        return "In Progress";
                    case RouteBack:
                        return "Route Back";
                    case Cancelled:
                        return "Cancelled";
                    case Closed:
                        return "Closed";
                    default:
                        return "";
                }
            }
        }

        public static class SRStatusCode
        {
            public const string Draft = "DR";
            public const string Open = "OP";
            public const string WaitingCustomer = "WA";
            public const string InProgress = "IP";
            public const string RouteBack = "RB";
            public const string Cancelled = "CC";
            public const string Closed = "CL";
        }

        public static class SrRoleCode
        {
            public const string ITAdministrator = "IT";
            public const string UserAdministrator = "UA";
            public const string ContactCenterManager = "CM";
            public const string ContactCenterSupervisor = "CS";
            public const string ContactCenterFollowUp = "FL";
            public const string ContactCenterAgent = "CA";
            public const string BranchManager = "BM";
            public const string Branch = "BA";
            public const string NCB = "NCB";
        }

        public static class AddressType
        {
            public const string SendingDoc = "ที่อยู่ส่งเอกสาร";
        }

        public static class CMTParamConfig
        {
            public const string Offered = "Y";
            public const string NoOffered = "N";
            public const string Interested = "Y";
            public const string NoInterested = "N";
            public const string RecommendCampaign = "AND";
            public const string RecommendedCampaign = "OR";
            public const int NumRecommendCampaign = 5;
            public const int NumRecommendedCampaign = 30;

            public static string GetInterestedMessage(string interested)
            {
                if (Interested.Equals(interested))
                {
                    return Resource.Msg_Interested;
                }

                if (NoInterested.Equals(interested))
                {
                    return Resource.Msg_NoInterested;
                }

                return string.Empty;
            }
        }

        public static class CustomerLog
        {
            public const string AddCustomer = "เพิ่มข้อมูลลูกค้า";
            public const string EditCustomer = "แก้ไขข้อมูลลูกค้า";
            public const string AddDocument = "เพิ่มเอกสาร";
            public const string EditDocument = "แก้ไขเอกสาร";
            public const string DeleteDocument = "ลบเอกสาร";
            public const string AddContact = "เพิ่มผู้ติดต่อ";
            public const string EditContact = "แก้ไขผู้ติดต่อ";
            public const string DeleteContact = "ลบผู้ติดต่อ";
        }

        public static class Page
        {
            public const string CommunicationPage = "Commu";
            public const string CustomerPage = "Customer";
            public const string ServiceRequestPage = "ServiceRequest";
        }

        public static class Sla
        {
            public const int Due = 1;
            public const int OverDue = 2;
        }

        public static class CallType
        {
            public const string NCB = "NCB";
            public const string ContactCenter = "CC";
        }

        public static class ImportBDWContact
        {
            public const string DataTypeHeader = "H";
            public const string DataTypeDetail = "D";
            public const int LengthOfHeader = 3;
            public const int LengthOfDetail = 19; //18;
        }

        public static class ImportCisData
        {
            public const int LengthOfHeaderCisCorporate = 32;
            public const int LengthOfHeaderCisIndividual = 49;
            public const int LengthOfHeaderCisProductGroup = 8;
            public const int LengthOfHeaderCisSubscription = 36; //35;
            public const int LengthOfHeaderCisTitle = 5;
            public const int LengthOfHeaderCisProvince = 3;
            public const int LengthOfHeaderCisDistrict = 4;
            public const int LengthOfHeaderCisSubDistrict = 5;
            public const int LengthOfHeaderCisPhoneType = 2;
            public const int LengthOfHeaderEmailType = 2;
            public const int LengthOfHeaderCisSubscriptionAddress = 29;
            public const int LengthOfHeaderCisSubscribePhone = 14;
            public const int LengthOfHeaderCisSubscribeMail = 13;
            public const int LengthOfHeaderCisAddressType = 2;
            public const int LengthOfHeaderCisCisSubscriptionType = 7;
            public const int LengthOfHeaderCisCustomerPhone = 12;
            public const int LengthOfHeaderCisCustomerEmail = 11;
            public const int LengthOfHeaderCisCountry = 3;
        }

        public static class ImportAfs
        {
            public const int LengthOfProperty = 13;
            public const int LengthOfSaleZone = 7;
        }

        public static class ImportHp
        {
            public const int LengthOfDetail = 33;
        }

        public static class TitleLanguage
        {
            public const string TitleTh = "TH";
            public const string TitleEn = "EN";
        }

        public const int CommandTimeout = 180;
        public const int BatchCommandTimeout = 900;
        public static int[] IgnoreHttpStatuses = { (int)HttpStatusCode.Unauthorized, (int)HttpStatusCode.Forbidden };
    }
}