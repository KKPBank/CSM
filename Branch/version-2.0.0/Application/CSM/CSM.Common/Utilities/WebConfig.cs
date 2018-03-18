using System;
using System.Configuration;
using System.Web.Configuration;
using log4net;

///<summary>
/// Class Name : WebConfig
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
    public class WebConfig
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebConfig));

        #region "AppSettings"

        private const string EmailServerString = "Email-Server";
        private const string EmailServerPortString = "Email-Server-Port";
        private const string MailEnable = "MailEnable";
        private const string MailAuthenMethod = "MailAuthenMethod";
        private const string MailAuthenUser = "MailAuthenUser";
        private const string MailAuthenPassword = "MailAuthenPassword";
        private const string MailSenderEmail = "MailSenderEmail";
        private const string MailSenderName = "MailSenderName";
        private const string FixDestinationMail = "FixDestinationMail";
        private const string DefaultPageSize = "DefaultPageSize";
        private const string SoftwareVersion = "SoftwareVersion";
        private const string ServiceRetryNo = "ServiceRetryNo";
        private const string ServiceRetryInterval = "ServiceRetryInterval";
        private const string DisplayMinimumRequire = "DisplayMinimumRequire";

        private const string SkipAD = "SkipAD";
        private const string LdapDomainString = "LDAP_DOMAIN";
        private const string LdapUsernameString = "LDAP_USERNAME";
        private const string LdapPasswordString = "LDAP_PASSWORD";
        private const string LdapUacDisabledString = "LDAP_UAC_DISABLED";
        private const string LdapConnectionString = "ADConnectionString";

        private const string TaskUsername = "TaskUsername";
        private const string TaskPassword = "TaskPassword";
        private const string WebUsername = "WebUsername";
        private const string WebPassword = "WebPassword";
        private const string TotalCountToProcess = "TotalCountToProcess";
        private const string TaskEmailToAddress = "EmailToAddress";
        private const string MailTemplatesPath = "MailTemplatesPath";

        private const string Pop3EmailServerString = "POP3-Server";
        private const string Pop3PortString = "POP3-Port";
        private const string Pop3MailUseSsl = "MailUseSsl";

        private const string AfsProperty = "AFS-Property";
        private const string AfsSaleZone = "AFS-SaleZone";
        private const string ActivityAfs = "ActivityAFS";
        private const string IvrSshServer = "IVR-SSH-Server";
        private const string IvrSshPort = "IVR-SSH-Port";
        private const string IvrSshUsername = "IVR-SSH-Username";
        private const string IvrSshPassword = "IVR-SSH-Password";
        private const string IvrSshInsertRemoteDir = "IVR-SSH-InsertRemoteDir";
        private const string IvrSshUpdateRemoteDir = "IVR-SSH-UpdateRemoteDir";

        private const string NewEmployeeNcb = "New_EmployeeNCB";
        private const string UpdateEmployeeNcb = "Update_EmployeeNCB";

        private const string BdwContactPrefix = "BDW-Contact-Prefix";
        private const string BdwSshServer = "BDW-SSH-Server";
        private const string BdwSshPort = "BDW-SSH-Port";
        private const string BdwSshUsername = "BDW-SSH-Username";
        private const string BdwSshPassword = "BDW-SSH-Password";
        private const string BdwSshRemoteDir = "BDW-SSH-RemoteDir";

        private const string CisSshServer = "CIS-SSH-Server";
        private const string CisSshPort = "CIS-SSH-Port";
        private const string CisSshUsername = "CIS-SSH-Username";

        private const string CisSshPassword = "CIS-SSH-Password";
        private const string CisSshMdmRemoteDir = "CIS-SSH-MdmRemoteDir";
        private const string CisSshCustRemoteDir = "CIS-SSH-CustRemoteDir";

        private const string CisCorporatePrefix = "Cis_CorporatePrefix";
        private const string CisIndividualPrefix = "Cis_IndividualPrefix";
        private const string CisProductGroupPrefix = "Cis_ProductGroupPrefix";
        private const string CisSubScriptionPrefix = "Cis_SubScriptionPrefix";
        private const string CisTitlePrefix = "Cis_TitlePrefix";
        private const string CisProvincePrefix = "Cis_ProvincePrefix";
        private const string CisDistrictPrefix = "Cis_DistrictPrefix";
        private const string CisSubDistrictPrefix = "Cis_SubDistrictPrefix";
        private const string CisPhoneTypePrefix = "Cis_PhoneTypePrefix";
        private const string CisMailTypePrefix = "Cis_MailTypePrefix";
        private const string CisSubMailPrefix = "Cis_SubMailPrefix";
        private const string CisSubPhonePrefix = "Cis_SubPhonePrefix";
        private const string CisSubAddressPrefix = "Cis_SubAddressPrefix";
        private const string CisAddressTypePrefix = "Cis_AddressTypePrefix";
        private const string CisSubScriptionTypePrefix = "Cis_SubScriptionTypePrefix";
        private const string CisCustomerPhonePrefix = "Cis_CustomerPhonePrefix";
        private const string CisCustomerEmailPrefix = "Cis_CustomerEmailPrefix";
        private const string CisCountryPrefix = "Cis_CountryPrefix";
        
        private const string HpActivityPrefix = "Hp_ActivityPrefix";

        private const string SLMEncryptPassword = "SLMEncryptPassword";
        private const string RemarkDisplayMaxLength = "RemarkDisplayMaxLength";

        const string ReportSRSshHost = "ReportSR-SSH-Host";
        const string ReportSRSshPort = "ReportSR-SSH-Port";
        const string ReportSRSshUserName = "ReportSR-SSH-Username";
        const string ReportSRSshPassword = "ReportSR-SSH-Password";
        const string ReportSRSshDailyDir = "ReportSR-SSH-DailyRemoteDir";
        const string ReportSRSshAccumDir = "ReportSR-SSH-AccumRemoteDir";

        const string EditTextMaxLength = "EditTextMaxLength";

        #endregion

        #region "Retrieve data in Web.config"

        public static string GetConnectionString(string name)
        {
            try
            {
                ConfigurationManager.RefreshSection("connectionStrings");
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception e)
            {
                log.Error(string.Format("{0}, Failed to get connection string information.", name), e);
                return string.Empty;
            }
        }

        private static string GetAppSetting(string name)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                return ConfigurationManager.AppSettings[name];
            }
            catch (Exception e)
            {
                log.Error(string.Format("{0}, Failed to get application string information.", name), e);
                return string.Empty;
            }
        }

        #endregion

        #region "Save data to Web.config"

        public static bool UpdateAppSetting(string key, string value)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                var appSettingsSection = (AppSettingsSection)config.GetSection("appSettings");

                if (appSettingsSection != null)
                {
                    appSettingsSection.Settings[key].Value = value;
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error(string.Format("{0}, Failed to modify application setting information.", key), e);
                return false;
            }
        }

        #endregion

        #region "Mail Service"

        public static string GetEmailServer()
        {
            return GetAppSetting(EmailServerString);
        }

        public static bool SetEmailServer(string serverName)
        {
            return UpdateAppSetting(EmailServerString, serverName);
        }

        public static int GetEmailServerPort()
        {
            string portNumber = GetAppSetting(EmailServerPortString);
            return portNumber.ToNullable<int>() ?? 25;
        }

        public static bool SetEmailServerPort(string port)
        {
            return UpdateAppSetting(EmailServerPortString, port);
        }

        public static bool GetMailEnable()
        {
            bool MailEnableValue = true;

            if (!String.IsNullOrEmpty(GetAppSetting(MailEnable)))
            {
                MailEnableValue = Boolean.Parse(GetAppSetting(MailEnable));
            }

            return MailEnableValue;
        }

        public static string GetMailAuthenMethod()
        {
            string MailAuthenMethodValue = "default";

            if (!String.IsNullOrEmpty(GetAppSetting(MailAuthenMethod)))
            {
                MailAuthenMethodValue = GetAppSetting(MailAuthenMethod);
            }

            return MailAuthenMethodValue;
        }

        public static string GetMailAuthenUser()
        {
            string MailAuthenUserValue = string.Empty;

            if (!String.IsNullOrEmpty(GetAppSetting(MailAuthenUser)))
            {
                MailAuthenUserValue = GetAppSetting(MailAuthenUser);
            }

            return MailAuthenUserValue;
        }

        public static string GetMailAuthenPassword()
        {
            string MailAuthenPasswordValue = string.Empty;

            if (!String.IsNullOrEmpty(GetAppSetting(MailAuthenPassword)))
            {
                MailAuthenPasswordValue = GetAppSetting(MailAuthenPassword);
            }

            return MailAuthenPasswordValue;
        }

        public static string GetFixDestinationMail()
        {
            string FixDestinationMailValue = "";

            if (!String.IsNullOrEmpty(GetAppSetting(FixDestinationMail)))
            {
                FixDestinationMailValue = GetAppSetting(FixDestinationMail);
            }

            return FixDestinationMailValue;
        }

        public static string GetSenderEmail()
        {
            string senderEmail = string.Empty;
            if (!String.IsNullOrEmpty(GetAppSetting(MailSenderEmail)))
            {
                senderEmail = GetAppSetting(MailSenderEmail);
            }
            return senderEmail;
        }

        public static string GetSenderName()
        {
            string senderName = string.Empty;
            if (!String.IsNullOrEmpty(GetAppSetting(MailSenderName)))
            {
                senderName = GetAppSetting(MailSenderName);
            }
            return senderName;
        }
        
        public static string GetPOP3EmailServer()
        {
            return GetAppSetting(Pop3EmailServerString);
        }

        public static bool SetPOP3EmailServer(string serverName)
        {
            return UpdateAppSetting(Pop3EmailServerString, serverName);
        }

        public static int GetPOP3Port()
        {
            string portNumber = GetAppSetting(Pop3PortString);
            return portNumber.ToNullable<int>() ?? 110;
        }

        public static bool SetPOP3Port(string port)
        {
            return UpdateAppSetting(Pop3PortString, port);
        }

        public static bool GetMailUseSsl()
        {
            bool useSsl = false;

            if (!String.IsNullOrEmpty(GetAppSetting(Pop3MailUseSsl)))
            {
                useSsl = GetAppSetting(Pop3MailUseSsl).ToBoolean();
            }

            return useSsl;
        }

        #endregion

        #region "AFS Service"

        public static string GetAFSProperty()
        {
            return GetAppSetting(AfsProperty);
        }

        public static string GetAFSSaleZone()
        {
            return GetAppSetting(AfsSaleZone);
        }

        public static string GetActivityAFS()
        {
            return GetAppSetting(ActivityAfs);
        }

        public static string GetNewEmployeeNCBAFS()
        {
            return GetAppSetting(NewEmployeeNcb);
        }

        public static string GetUpdateEmployeeNCBAFS()
        {
            return GetAppSetting(UpdateEmployeeNcb);
        }

        public static string GetIVRSshServer()
        {
            return GetAppSetting(IvrSshServer);
        }

        public static int GetIVRSshPort()
        {
            return GetAppSetting(IvrSshPort).ToNullable<int>() ?? 22;
        }

        public static string GetIVRSshUsername()
        {
            return GetAppSetting(IvrSshUsername);
        }

        public static string GetIVRSshPassword()
        {
            return GetAppSetting(IvrSshPassword);
        }

        public static string GetIVRSshInsertRemoteDir()
        {
            return GetAppSetting(IvrSshInsertRemoteDir);
        }

        public static string GetIVRSshUpdateRemoteDir()
        {
            return GetAppSetting(IvrSshUpdateRemoteDir);
        }

        #endregion

        #region "BDW Service"

        public static string GetBDWContactPrefix()
        {
            return GetAppSetting(BdwContactPrefix);
        }

        public static string GetBDWSshServer()
        {
            return GetAppSetting(BdwSshServer);
        }

        public static int GetBDWSshPort()
        {
            return GetAppSetting(BdwSshPort).ToNullable<int>() ?? 22;
        }

        public static string GetBDWSshUsername()
        {
            return GetAppSetting(BdwSshUsername);
        }

        public static string GetBDWSshPassword()
        {
            return GetAppSetting(BdwSshPassword);
        }

        public static string GetBDWSshRemoteDir()
        {
            return GetAppSetting(BdwSshRemoteDir);
        }

        #endregion

        #region "CIS Service"

        public static string GetCisSshServer()
        {
            return GetAppSetting(CisSshServer);
        }

        public static int GetCisSshPort()
        {
            return GetAppSetting(CisSshPort).ToNullable<int>() ?? 22;
        }

        public static string GetCisSshUsername()
        {
            return GetAppSetting(CisSshUsername);
        }

        public static string GetCisSshPassword()
        {
            return GetAppSetting(CisSshPassword);
        }

        public static string GetCisSshMdmRemoteDir()
        {
            return GetAppSetting(CisSshMdmRemoteDir);
        }

        public static string GetCisSshCustRemoteDir()
        {
            return GetAppSetting(CisSshCustRemoteDir);
        }
        
        public static string GetCisCorprate()
        {
            return GetAppSetting(CisCorporatePrefix);
        }

        public static string GetIndividual()
        {
            return GetAppSetting(CisIndividualPrefix);
        }

        public static string GetProductGroup()
        {
            return GetAppSetting(CisProductGroupPrefix);
        }

        public static string GetSubscription()
        {
            return GetAppSetting(CisSubScriptionPrefix);
        }

        public static string GetTitle()
        {
            return GetAppSetting(CisTitlePrefix);
        }

        public static string GetProvince()
        {
            return GetAppSetting(CisProvincePrefix);
        }

        public static string GetDistrict()
        {
            return GetAppSetting(CisDistrictPrefix);
        }

        public static string GetSubDistrict()
        {
            return GetAppSetting(CisSubDistrictPrefix);
        }

        public static string GetPhonetype()
        {
            return GetAppSetting(CisPhoneTypePrefix);
        }
        public static string GetMailtype()
        {
            return GetAppSetting(CisMailTypePrefix);
        }
        public static string GetSubmail()
        {
            return GetAppSetting(CisSubMailPrefix);
        }
        public static string GetSubphone()
        {
            return GetAppSetting(CisSubPhonePrefix);
        }
        public static string GetSubaddress()
        {
            return GetAppSetting(CisSubAddressPrefix);
        }
        public static string GetAddresstype()
        {
            return GetAppSetting(CisAddressTypePrefix);
        }
        public static string GetSubScriptionType()
        {
            return GetAppSetting(CisSubScriptionTypePrefix);
        }
        public static string GetCustomerPhone()
        {
            return GetAppSetting(CisCustomerPhonePrefix);
        }
        public static string GetCustomerEmail()
        {
            return GetAppSetting(CisCustomerEmailPrefix);
        }
        public static string GetCountry()
        {
            return GetAppSetting(CisCountryPrefix);
        }

        #endregion

        #region "HP Service"
        public static string GetHpActivity()
        {
            return GetAppSetting(HpActivityPrefix);
        }
        #endregion

        #region SR Report Service

        public static string GetReportSRSshHost() { return GetAppSetting(ReportSRSshHost); }

        public static int GetReportSRSshPort()
        {
            string val = GetAppSetting(ReportSRSshPort) ?? "22";
            int retVal;
            if (int.TryParse(val, out retVal))
            {
                return retVal;
            }
            return 22;
        }

        public static string GetReportSRSshUser() { return GetAppSetting(ReportSRSshUserName); }

        public static string GetReportSRSshPassword() { return GetAppSetting(ReportSRSshPassword); }

        public static string GetReportSRSshDailyDir() { return GetAppSetting(ReportSRSshDailyDir); }

        public static string GetReportSRSshAccumDir() { return GetAppSetting(ReportSRSshAccumDir); }

        #endregion

        #region "Common Appsettings"

        public static string GetSoftwareVersion()
        {
            return GetAppSetting(SoftwareVersion);
        }

        public static int GetServiceRetryInterval()
        {
            return GetAppSetting(ServiceRetryInterval).ToNullable<int>() ?? 1;
        }

        public static int GetServiceRetryNo()
        {
            return GetAppSetting(ServiceRetryNo).ToNullable<int>() ?? 0;
        }

        public static int GetTotalCountToProcess()
        {
            return GetAppSetting(TotalCountToProcess).ToNullable<int>() ?? 1;
        }

        #endregion

        internal static string GetDisplayMinimunRequirement()
        {
            return GetAppSetting(DisplayMinimumRequire);
        }

        #region "Scheduled Task"

        public static string GetTaskUsername()
        {
            return GetAppSetting(TaskUsername);
        }

        public static string GetTaskPassword()
        {
            return GetAppSetting(TaskPassword);
        }

        public static string GetTaskEmailToAddress()
        {
            return GetAppSetting(TaskEmailToAddress);
        }

        public static string GetMailTemplatesPath()
        {
            return GetAppSetting(MailTemplatesPath);
        }

        public static string GetWebUsername()
        {
            return GetAppSetting(WebUsername);
        }

        public static string GetWebPassword()
        {
            return GetAppSetting(WebPassword);
        }

        #endregion

        #region "Manage Ldap Information"

        public static bool IsSkipAD()
        {
            return GetAppSetting(SkipAD).ToNullable<bool>() ?? true;
        }

        public static string GetLdapDomain()
        {
            return GetAppSetting(LdapDomainString).NullSafeTrim();
        }

        public static string GetLdapUsername()
        {
            return GetAppSetting(LdapUsernameString).NullSafeTrim();
        }

        public static string GetLdapPassword()
        {
            return GetAppSetting(LdapPasswordString).NullSafeTrim();
        }

        public static string GetLdapUacDisabled()
        {
            return GetAppSetting(LdapUacDisabledString).NullSafeTrim();
        }

        public static string GetLdapConnectionString()
        {
            return GetConnectionString(LdapConnectionString);
        }

        public static string GetLdapDomainName()
        {
            try
            {
                string connectionString = GetLdapConnectionString();

                int startIndex = connectionString.IndexOf("LDAP://") + 7;
                int endIndex = connectionString.IndexOf(":", startIndex);
                int length = endIndex - startIndex;

                string result = "";
                if (startIndex > 0)
                {
                    result = connectionString.Substring(startIndex, length);
                }

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        public static string GetSLMEncryptPassword()
        {
            return GetAppSetting(SLMEncryptPassword).NullSafeTrim();
        }

        public static int GetRemarkDisplayMaxLength()
        {
            string maxLength = GetAppSetting(RemarkDisplayMaxLength);
            int result;
            if (int.TryParse(maxLength, out result))
            {
                return result;
            }
            else
            {
                return 5000;
            }
        }

        #region OPT Service

        public static string GetOTPUserName() { return GetAppSetting("OTP_USER"); }
        public static string GetOTPPassword() { return GetAppSetting("OTP_PWD"); }
        public static string GetOTPServiceName() { return GetAppSetting("OTP_SERVICE_NAME"); }
        public static string GetOTPMethod() { return GetAppSetting("OTP_METHOD"); }
        public static string GetOTPSystemCode() { return GetAppSetting("OTP_SYSTEM_CODE"); }
        public static int GetOTPMaxWait()
        {
            string val = GetAppSetting("OTP_MAX_WAIT_GEN") ?? "1800";
            int retVal;
            if (int.TryParse(val, out retVal)) {
                return retVal;
            }
            return 1800; //Default 1800 วินาที = 30 นาที
        }

        #endregion

        #region Import HRIS File

        public static string GetHRIUser() { return GetAppSetting("HRIS_SFTP_USER"); }

        public static string GetHRIPassword() { return GetAppSetting("HRIS_SFTP_PASSWORD"); }

        #endregion

        public static string GetDefaultIP()
        {
            return GetAppSetting("Default-IP");
        }

        public static int GetEditTextMaxLength()
        {
            string cfg = GetAppSetting(EditTextMaxLength);
            int res;
            if (int.TryParse(cfg, out res))
            {
                return res;
            }
            return 15000;
        }
    }
}