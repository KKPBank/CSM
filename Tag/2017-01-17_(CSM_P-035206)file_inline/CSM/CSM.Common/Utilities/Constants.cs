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
        public const int DisplaySenderName = 35;
        //public const int ActivityDescriptionMaxLength = 15000;
        //public const int EmailBodyMaxLength = 15000;

        public const string DefaultSubscriptionTypeForUser = "18"; //"19";
        public const string RemarkLink = "<input id=\"remarkLink\" value=\"แสดงรายละเอียด\" class=\"btn btn-info btn-sm\" onclick=\"onlinkRemarkClick(); return false;\" type=\"button\">";

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
            public const string CreateBranch = "CreateBranch";

            public const string SyncSRStatusFromReplyEmail = "Sync SR Status from Reply Email";
            public const string ReSubmitActivityToCARSystem = "Re-Submit Activity to CAR System";
            public const string ReSubmitActivityToCBSHPSystem = "Re-Submit Activity to CBS-HP System";
            public const string SubmitActivityToCARSystem = "Submit Activity to CAR System";

            public const string Search = "Search";

            public const string ImportHRIS = "Import HRIS files";
            public const string ExportSRDaily = "Report SR (Daily)";
            public const string ExportSRAccum = "Report SR (Accum)";
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
            public const string CSM0003 = "CSM0003";      // CSM0003  Unknown Error

            public const string CSMProd001 = "001";
            public const string CSMProd002 = "002";
            public const string CSMProd003 = "003";

            public const string CSMCust001 = "001";
            public const string CSMCust002 = "002";
            public const string CSMCust003 = "003";

            public const string CSMBranch001 = "001";       //001 header bad request
            public const string CSMBranch002 = "002";       //002 body bad request
            public const string CSMBranch003 = "003";       //003 save or update error
            public const string CSMBranch004 = "004";       //004 Unknown error

            public const string CSMCalendar001 = "001";     //001 header bad request
            public const string CSMCalendar002 = "002";     //002 body bad request
            public const string CSMCalendar003 = "003";       //003 save or update error
            public const string CSMCalendar004 = "004";       //004 body invalid branch code

            public const string CSMSaveSr001 = "001"; //001 header bad request
            public const string CSMGetSr001 = "001"; //001 header bad request
            public const string CSMUpdateSr001 = "001"; //001 header bad request
            public const string CSMSearchSr001 = "001"; //001 header bad request


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
            public const string NotifyExportSRReportFailed = "NotifyExportSRReportFailed";
        }

        public static class MaxLength
        {
            public const int CardNo = 20;
            public const int PhoneNo = 20;
            public const int Username = 50;
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

            public const int BdwCardNo = 50;
            public const int BdwTitleTH = 50;
            public const int BdwNameTH = 255;
            public const int BdwSurnameTH = 255;
            public const int BdwTitleEN = 50;
            public const int BdwNameEN = 255;
            public const int BdwSurnameEN = 255;
            public const int BdwAccountNo = 100;
            public const int BdwLoanMain = 255;
            public const int BdwProductGroup = 255;
            public const int BdwProduct = 255;
            public const int BdwRelationship = 100;
            public const int BdwPhone = 255;
            public const int BdwCampaign = 255;
            public const int BdwCardTypeCode = 10;
            public const int BdwAccountStatus = 10;

            #endregion

            // import Hp Activity
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

            public const int MailSubject = 1000;
        }

        public static class MinLenght
        {
            public const int SearchTerm = 2;
            public const int AutoComplete = 0;
            public const int PhoneNo = 9;
        }

        public static class CisMaxLength
        {
            public const int AddressTypeName = 50;
            public const int NameTH = 255;
            public const int NameEN = 255;
            public const int TaxId = 50;
            public const int HostBusinessCountryCode = 10;
            public const int ValuePerShare = 50;
            public const int AuthorizedShareCapital = 50;
            public const int RegisterDate = 50;
            public const int IdCountryIssue = 10;
            public const int BusinessCatCode = 10;
            public const int Stock = 50;
            public const int CountryNameTH = 255;
            public const int CountryNameEN = 255;
            public const int MailTypeCode = 10;
            public const int DistrictNameTH = 100;
            public const int DistrictNameEN = 100;
            public const int EmailTypeDesc = 100;
            public const int TitleNameCustom = 50;
            public const int FirstNameTH = 255;
            public const int MidNameTH = 255;
            public const int LastNameTH = 255;
            public const int FirstNameEN = 255;
            public const int MidNameEN = 255;
            public const int LastNameEN = 255;
            public const int BirthDate = 50;
            public const int MaritalCode = 10;
            public const int Nationality1Code = 10;
            public const int Nationality2Code = 10;
            public const int Nationality3Code = 10;
            public const int ReligionCode = 10;
            public const int EducationCode = 10;
            public const int Position = 255;
            public const int CompanyName = 255;
            public const int AnnualIncome = 50;
            public const int SourceIncome = 50;
            public const int TotalWealthPeriod = 50;
            public const int ChannelHome = 50;
            public const int AnnualIncomePeriod = 10;
            public const int OccupationCode = 10;
            public const int OccupationSubtypeCode = 10;
            public const int CountryIncome = 50;
            public const int TotalWealth = 50;
            public const int SourceIncomeRem = 100;
            public const int PhoneTypeDesc = 100;
            public const int ProductCode = 50;
            public const int ProductType = 50;
            public const int ProductDesc = 100;
            public const int System = 50;
            public const int ProductFlag = 1;
            public const int SubscrDesc = 255;
            public const int ProvinceNameTH = 100;
            public const int ProvinceNameEN = 100;
            public const int SubdistrictNameTH = 100;
            public const int SubdistrictNameEN = 100;
            public const int AddressNumber = 255;
            public const int Village = 255;
            public const int Building = 255;
            public const int FloorNo = 100;
            public const int RoomNo = 100;
            public const int Moo = 100;
            public const int Street = 255;
            public const int Soi = 255;
            public const int SubDistrictValue = 255;
            public const int DistrictValue = 255;
            public const int ProvinceValue = 255;
            //public const int EmailTypeName = 100;
            public const int RefNo = 50;
            public const int Text1 = 255;
            public const int Text2 = 255;
            public const int Text3 = 255;
            public const int Text4 = 255;
            public const int Text5 = 255;
            public const int Text6 = 255;
            public const int Text7 = 255;
            public const int Text8 = 255;
            public const int Text9 = 255;
            public const int Text10 = 255;
            public const int Number1 = 50;
            public const int Number2 = 50;
            public const int Number3 = 50;
            public const int Number4 = 50;
            public const int Number5 = 50;
            public const int Date1 = 50;
            public const int Date2 = 50;
            public const int Date3 = 50;
            public const int Date4 = 50;
            public const int Date5 = 50;
            public const int SubscrStatus = 1;
            public const int CreatedChannel = 50;
            //public const int AccountNo = 50;
            public const int UpdatedChannel = 50;
            public const int BranchName = 255;
            public const int CustTypeTH = 100;
            public const int CustTypeEN = 100;
            public const int CardTypeEN = 100;
            public const int CardTypeTH = 100;
            public const int TitleNameTH = 100;
            public const int TitleNameEN = 100;
            public const int TitleTypeGroup = 10;

            public const int AddressTypeCode = 10;
            public const int Status = 1;
            public const int CardId = 50;
            public const int CustTypeCode = 10;
            public const int IsicCode = 10;
            public const int BusinessCode = 10;
            public const int FixedAsset = 50;
            public const int FixedAssetExcludeLand = 50;
            public const int NumberOfEmployee = 10;
            public const int ShareInfoFlag = 10;
            public const int FlgMstApp = 10;
            public const int FirstBranch = 50;
            public const int PlaceCustUpdated = 20;
            public const int DateCustUpdated = 20;
            public const int MarketingId = 10;
            public const int CountryCode = 10;
            public const int MailAccount = 100;
            public const int EmailTypeCode = 10;
            public const int GenderCode = 10;
            public const int KKCisId = 50;
            public const int EntityCode = 10;
            public const int PostCode = 10;
            public const int PostalValue = 10;
           
            public const int CardTypeCode = 10;
            public const int CustId = 50;
            public const int CustTypeGroup = 2;
            public const int DistrictCode = 10;
            public const int PhoneExt = 100;
            public const int PhoneNum = 100;
            public const int PhoneTypeCode = 10;
            public const int ProdGroup = 50;
            public const int ProdType = 50;
            public const int ProvinceCode = 10;
            public const int SubdistrictCode = 10;
            public const int SubscrCode = 50;
            public const int TitleId = 10;
            
            public const int CreatedDate = 20;
            public const int UpdatedDate = 20;
            public const int CreatedBy = 100;
            public const int UpdatedBy = 100;

            public const int SysCustSubscrId = 50;

        }

        public static class HRIMaxLength
        {
            public const int ContactEmail = 50;
            public const int Branch = 6;
            public const int BranchDesc = 100;
            public const int EmployeeId = 15;
            public const int TitleId = 5;
            public const int Title = 100;
            public const int FName = 30;
            public const int LName = 40;
            public const int Nickname = 30;
            public const int FullNameEng = 71;
            public const int ETitle = 100;
            public const int EFName = 31;
            public const int ELName = 41;
            public const int Sex = 1;
            public const int BirthDay = 10;
            public const int EmpType = 1;
            public const int EmpTypeDesc = 16;
            public const int Position = 10;
            public const int PositionDesc = 100;
            public const int BU1 = 10;
            public const int BU1Desc = 100;
            public const int BU2 = 10;
            public const int BU2Desc = 100;
            public const int BU3 = 10;
            public const int BU3Desc = 100;
            public const int BU4 = 10;
            public const int BU4Desc = 100;
            public const int Job = 10;
            public const int JobPosition = 100;
            public const int StartDate = 10;
            public const int FirstHireDate = 10;
            public const int ResignDate = 10;
            public const int Status = 1;
            public const int EmpStatus = 11;
            public const int Email = 50;
            public const int NotesAddress = 1;
            public const int WorkArea = 10;
            public const int WorkAreaDesc = 100;
            public const int CostCenter = 10;
            public const int CostCenterDesc = 100;
            public const int TelExt = 200;
            public const int Boss = 20;
            public const int BossName = 71;
            public const int Assessor1 = 10;
            public const int Assessor1Name = 71;
            public const int Assessor2 = 10;
            public const int Assessor2Name = 71;
            public const int Assessor3 = 10;
            public const int Assessor3Name = 71;
            public const int TelNo = 200;
            public const int MobileNo = 200;
            public const int ADUser = 100;
            public const int OfficerId = 2;
            public const int OfficerDesc = 50;
            public const int AdditionJob = 15;
            public const int UnitBoss = 20;
            public const int UnitBossName = 71;
            public const int IDNO = 4;
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

            public const string SLMUrlNewLead = "SLM_URL_NEW_LEAD";
            public const string SLMUrlViewLead = "SLM_URL_VIEW_LEAD";

            public const string ReportExportDate = "REPORT_EXPORT_DATE";

            public const string BatchInterval = "BATCH_INTERVAL";

            public const string HRPathImport = "HR_PATH_IMPORT";
            public const string HRPathError = "HR_PATH_ERROR";
            public const string HRPathSource = "HR_PATH_SOURCE";
            public const string HRFilePrefix = "HR_FILE_PREFIX";
            public const string HRSFTPHost = "HR_SFTP_HOST";
            public const string HRSFTPPort = "HR_SFTP_PORT";
            public const string HRSFTPRemoteDir = "HR_SFTP_REMOTE_DIR";
            public const string HRSFTPDownload = "HR_SFTP_DOWNLOAD";

            public const string DelegateMailConfig = "DELEGATE_EMAIL_CONFIG";

            public const string NumDaysSRReport = "NUM_DAYS_SR_REPORT";
            public const string TriggerDays = "TRIGGER_DAYS";
            public const string ReportPath = "REPORT_PATH";

            public const string InlineContentLimit = "INLINE_CONTENT_LIMIT"; // Email inline content size limit in Byte
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
            public const string CustomerService = "http://www.kiatnakinbank.com/services/CSM/CSMCustomerService";
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
            public const string NotProcess = "NOTPROCESS";
        }

        public static class TicketResponse
        {
            public const string SLMSuccess = "10000";
            public const string COCSuccess = "30000";
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
            public const string BDW = "BDW";
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
            public const int CPNPageId = 4;

            public const string DefaultPageCode = "DEFAULT";
            public const string AFSPageCode = "AFS";
            public const string NCBPageCode = "NCB";
            public const string CPNPageCode = "CPN";
        }

        public static class SrLogAction
        {
            public const string ChangeStatus = "Change Status";
            public const string ChangeOwner = "Change Owner";
            public const string Delegate = "Delegate";
            public const string Secret = "Secret";
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

        public static class SRStateId
        {
            public const int Draft = 1;
            public const int Open = 2;
            public const int WaitingCustomer = 3;
            public const int InProgress = 4;
            public const int RouteBack = 5;
            public const int Cancelled = 6;
            public const int Closed = 7;
            public const int WaitingGovernment = 8;
            public const int WaitingOther = 9;
            public const int InProgressTier1 = 10;
            public const int InProgressTier2 = 11;
            public const int InProgressTier3 = 12;
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
            public const string CPN = "CPN";
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
            public const string DataTypeHeader = "H";
            public const string DataTypeDetail = "D";

            public const int LengthOfHeaderCisCorporate = 35;
            public const int LengthOfHeaderCisIndividual = 52;
            public const int LengthOfHeaderCisProductGroup = 12;
            public const int LengthOfHeaderCisSubscription = 40; //39;
            public const int LengthOfHeaderCisTitle = 9;
            public const int LengthOfHeaderCisProvince = 7;
            public const int LengthOfHeaderCisDistrict = 8;
            public const int LengthOfHeaderCisSubDistrict = 9;
            public const int LengthOfHeaderCisPhoneType = 6;
            public const int LengthOfHeaderEmailType = 6;
            public const int LengthOfHeaderCisSubscriptionAddress = 33; //32;
            public const int LengthOfHeaderCisSubscribePhone = 18; //17;
            public const int LengthOfHeaderCisSubscribeMail = 17; //16;
            public const int LengthOfHeaderCisAddressType = 6;
            public const int LengthOfHeaderCisCisSubscriptionType = 11;
            public const int LengthOfHeaderCisCustomerPhone = 15;
            public const int LengthOfHeaderCisCustomerEmail = 14;
            public const int LengthOfHeaderCisCountry = 7;

            public const int LengthOfDetailCisCorporate = 33;
            public const int LengthOfDetailCisIndividual = 50;
            public const int LengthOfDetailCisProductGroup = 10;
            public const int LengthOfDetailCisSubscription = 38; //37;
            public const int LengthOfDetailCisTitle = 7;
            public const int LengthOfDetailCisProvince = 5;
            public const int LengthOfDetailCisDistrict = 6;
            public const int LengthOfDetailCisSubDistrict = 7;
            public const int LengthOfDetailCisPhoneType = 4;
            public const int LengthOfDetailEmailType = 4;
            public const int LengthOfDetailCisSubscriptionAddress = 31; //30;
            public const int LengthOfDetailCisSubscribePhone = 16; //15;
            public const int LengthOfDetailCisSubscribeMail = 15; //14;
            public const int LengthOfDetailCisAddressType = 4;
            public const int LengthOfDetailCisCisSubscriptionType = 9;
            public const int LengthOfDetailCisCustomerPhone = 13;
            public const int LengthOfDetailCisCustomerEmail = 12;
            public const int LengthOfDetailCisCountry = 5;
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

        public static class ImportHRI
        {
            public const int ColumnCount = 57;
        }

        public static class TitleLanguage
        {
            public const string TitleTh = "TH";
            public const string TitleEn = "EN";
        }

        public const int CommandTimeout = 600; //180; แก้จาก 3 นาทีเป็น 10 เนื่องจากมี Error ตอน Import HP
        public const int BatchCommandTimeout = 900;

        public static class BatchProcessStatus
        {
            public const int Fail = 0;
            public const int Success = 1;
            public const int Processing = 2;
        }

        public static class BatchProcessCode
        {
            public const string ImportAFS = "A";
            public const string CreateCommPool = "M";
            public const string ExportAFS = "E";
            public const string ExportMarketing = "N";
            public const string ImportBDW = "B";
            public const string ImportCIS = "C";
            public const string ImportHP = "H";
            public const string ImportHR = "G";

            public const string SyncSRStatusFromReplyEmail = "R";
            public const string ReSubmitActivityToCARSystem = "S";
            public const string ReSubmitActivityToCBSHPSystem = "T";

            public const string ExportDailySRReport = "D";
            public const string ExportMonthlySRReport = "F";
        }

        public const string SystemUserName = "SYSTEM";

        public static class AttachmentPrefix
        {
            public const string Customer = "C";
            public const string Sr = "S";
            public const string News = "N";
            public const string Job = "J";
        }

        public static class CAR_DataItemLableEntity
        {
            public static class CustomerInfo
            {
                public const string SubType = "Subscription Type";
                public const string SubId = "Subscription ID";
                public const string BirthDay = "วันเกิด";
                public const string Title = "คำนำหน้า";
                public const string FirstName = "ชื่อลูกค้า";
                public const string LastName = "นามสกุลลูกค้า";
                public const string TitleEn = "Title";
                public const string FirstNameEn = "First Name";
                public const string LastNameEn = "Last Name";
                public const string PhoneNo1 = "เบอร์โทรศัพท์ #1";
                public const string PhoneNo2 = "เบอร์โทรศัพท์ #2";
                public const string PhoneNo3 = "เบอร์โทรศัพท์ #3";
                public const string Fax = "เบอร์แฟกซ์";
                public const string Email = "อีเมล์";
                public const string EmployeeCode = "รหัสพนักงาน";
                public const string ANo = "A Number.";
                public const string CallId = "Call ID.";
            }

            public static class ContractInfo
            {
                public const string AccountNo = "เลขที่บัญชี/สัญญา";
                public const string AccountStatus = "สถานะบัญชี";
                public const string AccountCarNo = "ทะเบียนรถ";
                public const string AccountProductGroup = "Product Group";
                public const string AccountProduct = "Product";
                public const string AccountBranchName = "ชื่อสาขา";
                public const string SubscriptionTypeName = "Subscription Type";
                public const string CardNo = "Subscription ID";
                public const string BirthDate = "วันเกิด";
                public const string TitleTh = "คำนำหน้า";
                public const string FirstNameTh = "ชื่อลูกค้า";
                public const string LastNameTh = "นามสกุลลูกค้า";
                public const string TitleEn = "Title";
                public const string FirstNameEn = "First Name";
                public const string LastNameEn = "Last Name";
                public const string PhoneNo1 = "เบอร์โทรศัพท์ #1";
                public const string PhoneNo2 = "เบอร์โทรศัพท์ #2";
                public const string PhoneNo3 = "เบอร์โทรศัพท์ #3";
                public const string Fax = "เบอร์แฟกซ์";
                public const string Email = "อีเมล์";
                public const string ContactAccountNo = "เลขที่สัญญาที่เกี่ยวข้องกับผู้ติดต่อ";
                public const string RelationshipName = "ความสัมพันธ์";
            }

            public static class ProductInfo
            {
                public const string ProductGroupName = "Product Group";
                public const string ProductName = "Product";
                public const string CampaignServiceName = "Campaign/Service";
            }

            public static class OfficeInfo
            {
                public const string CreateUser = "Officer";
            }

            public static class ActivityInfo
            {
                public const string AreaCode = "Area Code";
                public const string AreaName = "Area";
                public const string SubAreaCode = "Sub Area Code";
                public const string SubAreaName = "Sub Area";
                public const string TypeCode = "Type Code";
                public const string TypeName = "Type";
                public const string ChannelCode = "SR Channel";
                public const string MediaSourceName = "Media Source";
                public const string Subject = "Subject";
                public const string Remark = "Remark";
                public const string Verify = "Verify";
                public const string VerifyResult = "Verify Result";
                public const string SRCreatorBranchName = "Creator Branch";
                public const string SRCreatorUserFullName = "Creator SR";
                public const string OwnerBranchName = "Owner Branch";
                public const string OwnerUserFullName = "Owner SR";
                public const string DelegateBranchName = "Delegate Branch";
                public const string DelegateUserFullName = "Delegate SR";
                public const string SendEmail = "Send E-Mail";
                public const string EmailTo = "E-Mail To";
                public const string EmailCc = "E-Mail Cc";
                public const string EmailBcc = "E-Mail Bcc";
                public const string EmailSubject = "E-Mail Subject";
                public const string EmailBody = "EmailBody";
                public const string EmailAttachments = "E-Mail Attachments";
                public const string ActivityDescription = "รายละเอียดการติดต่อ";
                public const string ActivityTypeName = "Activity Type";
                public const string SRStateName = "SR State";
                public const string SRStatusName = "SR Status";

                public class DefaultPage
                {
                    public const string Address = "ที่อยู่ในการจัดส่งเอกสาร";
                }

                public class AFSPage
                {
                    public const string AFSAssetNo = "รหัสสินทรัพย์รอขาย";
                    public const string AFSAssetDesc = "รายละเอียดทรัพย์";
                }

                public class NCBPage
                {
                    public const string NCBCustomerBirthDate = "วันเกิด/วันที่จดทะเบียน (พ.ศ.)";
                    public const string NCBMarketingBranchUpper1Name = "Marketing Branch Upper #1";
                    public const string NCBMarketingBranchUpper2Name = "Marketing Branch Upper #2";
                    public const string NCBMarketingBranchName = "Marketing Branch";
                    public const string NCBMarketingFullName = "Marketing";
                    public const string NCBCheckStatus = "NCB Check Status";
                }

                public class CPNPage
                {
                    public const string CPN_ProductGroupName = "Product Group by Compaint";
                    public const string CPN_ProductName = "Product by Compaint";
                    public const string CPN_CampaignName = "Campaign by complaint";
                    public const string CPN_SubjectName = "หัวข้อ";
                    public const string CPN_TypeName = "ประเภทการร้องเรียน";
                    public const string CPN_RootCauseName = "สาเหตุการร้องเรียน";
                    public const string CPN_IssueName = "ประเด็นการร้องเรียน";
                    public const string CPN_IsSecret = "ความลับ";
                    public const string CPN_BUGroupName = "กลุ่มธุรกิจ";
                    public const string CPN_BU1Desc = "ทีม";
                    public const string CPN_BU2Desc = "ฝ่าย";
                    public const string CPN_BU3Desc = "สังกัด";
                    public const string CPN_BUBranch = "สาขา";
                    public const string CPN_IsSummary = "Summary";
                    public const string CPN_CauseCustomer = "Cause Customer";
                    public const string CPN_CauseCustomerDetail = "Cause Customer Detail";
                    public const string CPN_CauseStaff = "Cause Staff";
                    public const string CPN_CauseStaffDetail = "Cause Staff Detail";
                    public const string CPN_CauseSystem = "Cause System";
                    public const string CPN_CauseSystemDetail = "Cause System Detail";
                    public const string CPN_CauseProcess = "Cause Process";
                    public const string CPN_CauseProcessDetail = "Cause Process Detail";
                    public const string CPN_CauseSummaryName = "สรุปสาเหตุความเสี่ยง";
                    public const string CPN_SummaryName = "สรุปข้อร้องเรียนตามความเสี่ยง";
                    public const string CPN_Fixed_Detail = "สรุปการแก้ไขปัญหา";
                }
            }
        }

        public static class IVRLang
        {
            public const string Thai = "TH";
            public const string English = "ENG";
        }
    }
}