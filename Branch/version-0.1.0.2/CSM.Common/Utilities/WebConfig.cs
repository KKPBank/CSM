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

        private const string SkipAD = "SkipAD";
        private const string LdapDomainString = "LDAP_DOMAIN";
        private const string LdapUsernameString = "LDAP_USERNAME";
        private const string LdapPasswordString = "LDAP_PASSWORD";
        private const string LdapUacDisabledString = "LDAP_UAC_DISABLED";
        private const string LdapConnectionString = "ADConnectionString";

        private const string TaskUsername = "TaskUsername";
        private const string TaskPassword = "TaskPassword";
        private const string TotalCountToProcess = "TotalCountToProcess";
        private const string TaskEmailToAddress = "EmailToAddress";
        private const string MailTemplatesPath = "MailTemplatesPath";

        private const string POP3EmailServerString = "Email-Server";
        private const string POP3PortString = "POP3-Port";
        private const string POP3MailUseSsl = "MailUseSsl";

        private const string AFSProperty = "AFS-Property";
        private const string AFSSaleZone = "AFS-SaleZone";
        private const string ActivityAFS = "ActivityAFS";
        private const string IVR_SSHServer = "IVR-SSH-Server";
        private const string IVR_SSHPort = "IVR-SSH-Port";
        private const string IVR_SSHUsername = "IVR-SSH-Username";
        private const string IVR_SSHPassword = "IVR-SSH-Password";
        private const string IVR_SSH_InsertRemoteDir = "IVR-SSH-InsertRemoteDir";
        private const string IVR_SSH_UpdateRemoteDir = "IVR-SSH-UpdateRemoteDir";

        private const string New_EmployeeNCB = "New_EmployeeNCB";
        private const string Update_EmployeeNCB = "Update_EmployeeNCB";

        private const string BDW_ContactPrefix = "BDW-Contact-Prefix";
        private const string BDW_SSHServer = "BDW-SSH-Server";
        private const string BDW_SSHPort = "BDW-SSH-Port";
        private const string BDW_SSHUsername = "BDW-SSH-Username";
        private const string BDW_SSHPassword = "BDW-SSH-Password";
        private const string BDW_SSHRemoteDir = "BDW-SSH-RemoteDir";

        private const string Cis_CorporatePrefix = "Cis_CorporatePrefix";
        private const string Cis_IndividualPrefix = "Cis_IndividualPrefix";
        private const string Cis_ProductGroupPrefix = "Cis_ProductGroupPrefix";
        private const string Cis_SubScriptionPrefix = "Cis_SubScriptionPrefix";
        private const string Cis_TitlePrefix = "Cis_TitlePrefix";
        private const string Cis_ProvincePrefix = "Cis_ProvincePrefix";
        private const string Cis_DistrictPrefix = "Cis_DistrictPrefix";
        private const string Cis_SubDistrictPrefix = "Cis_SubDistrictPrefix";
        private const string Cis_PhoneTypePrefix = "Cis_PhoneTypePrefix";
        private const string Cis_MailTypePrefix = "Cis_MailTypePrefix";
        private const string Cis_SubMailPrefix = "Cis_SubMailPrefix";
        private const string Cis_SubPhonePrefix = "Cis_SubPhonePrefix";
        private const string Cis_SubAddressPrefix = "Cis_SubAddressPrefix";
        private const string Cis_AddressTypePrefix = "Cis_AddressTypePrefix";
        private const string Cis_SubScriptionTypePrefix = "Cis_SubScriptionTypePrefix";
        private const string Cis_CustomerPhonePrefix = "Cis_CustomerPhonePrefix";
        private const string Cis_CustomerEmailPrefix = "Cis_CustomerEmailPrefix";
        private const string Cis_CountryPrefix = "Cis_CountryPrefix";

        private const string Hp_ActivityPrefix = "Hp_ActivityPrefix";

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

        //public static string GetCSMDocumentFolder()
        //{
        //    return GetAppSetting(CSMDocumentFolder);
        //}
        //public static string GetSrDocumentFolder()
        //{
        //    return GetAppSetting(SrDocumentFolder);
        //}

        //public static string GetJobDocumentFolder()
        //{
        //    return GetAppSetting(JobDocumentFolder);
        //}

        public static string GetPOP3EmailServer()
        {
            return GetAppSetting(POP3EmailServerString);
        }

        public static bool SetPOP3EmailServer(string serverName)
        {
            return UpdateAppSetting(POP3EmailServerString, serverName);
        }

        public static int GetPOP3Port()
        {
            string portNumber = GetAppSetting(POP3PortString);
            return portNumber.ToNullable<int>() ?? 110;
        }

        public static bool SetPOP3Port(string port)
        {
            return UpdateAppSetting(POP3PortString, port);
        }

        public static bool GetMailUseSsl()
        {
            bool useSsl = false;

            if (!String.IsNullOrEmpty(GetAppSetting(POP3MailUseSsl)))
            {
                useSsl = GetAppSetting(POP3MailUseSsl).ToBoolean();
            }

            return useSsl;
        }

        #endregion

        #region "AFS Service"

        public static string GetAFSProperty()
        {
            return GetAppSetting(AFSProperty);
        }

        public static string GetAFSSaleZone()
        {
            return GetAppSetting(AFSSaleZone);
        }

        public static string GetActivityAFS()
        {
            return GetAppSetting(ActivityAFS);
        }

        public static string GetNewEmployeeNCBAFS()
        {
            return GetAppSetting(New_EmployeeNCB);
        }

        public static string GetUpdateEmployeeNCBAFS()
        {
            return GetAppSetting(Update_EmployeeNCB);
        }

        public static string GetIVRSshServer()
        {
            return GetAppSetting(IVR_SSHServer);
        }

        public static int GetIVRSshPort()
        {
            return GetAppSetting(IVR_SSHPort).ToNullable<int>() ?? 22;
        }

        public static string GetIVRSshUsername()
        {
            return GetAppSetting(IVR_SSHUsername);
        }

        public static string GetIVRSshPassword()
        {
            return GetAppSetting(IVR_SSHPassword);
        }

        public static string GetIVRSshInsertRemoteDir()
        {
            return GetAppSetting(IVR_SSH_InsertRemoteDir);
        }

        public static string GetIVRSshUpdateRemoteDir()
        {
            return GetAppSetting(IVR_SSH_UpdateRemoteDir);
        }

        #endregion

        #region "BDW Service"

        public static string GetBDWContactPrefix()
        {
            return GetAppSetting(BDW_ContactPrefix);
        }

        public static string GetBDWSshServer()
        {
            return GetAppSetting(BDW_SSHServer);
        }

        public static int GetBDWSshPort()
        {
            return GetAppSetting(BDW_SSHPort).ToNullable<int>() ?? 22;
        }

        public static string GetBDWSshUsername()
        {
            return GetAppSetting(BDW_SSHUsername);
        }

        public static string GetBDWSshPassword()
        {
            return GetAppSetting(BDW_SSHPassword);
        }

        public static string GetBDWSshRemoteDir()
        {
            return GetAppSetting(BDW_SSHRemoteDir);
        }

        #endregion

        #region "CIS Service"

        public static string GetCisCorprate()
        {
            return GetAppSetting(Cis_CorporatePrefix);
        }

        public static string GetIndividual()
        {
            return GetAppSetting(Cis_IndividualPrefix);
        }

        public static string GetProductGroup()
        {
            return GetAppSetting(Cis_ProductGroupPrefix);
        }

        public static string GetSubscription()
        {
            return GetAppSetting(Cis_SubScriptionPrefix);
        }

        public static string GetTitle()
        {
            return GetAppSetting(Cis_TitlePrefix);
        }

        public static string GetProvince()
        {
            return GetAppSetting(Cis_ProvincePrefix);
        }

        public static string GetDistrict()
        {
            return GetAppSetting(Cis_DistrictPrefix);
        }

        public static string GetSubDistrict()
        {
            return GetAppSetting(Cis_SubDistrictPrefix);
        }

        public static string GetPhonetype()
        {
            return GetAppSetting(Cis_PhoneTypePrefix);
        }
        public static string GetMailtype()
        {
            return GetAppSetting(Cis_MailTypePrefix);
        }
        public static string GetSubmail()
        {
            return GetAppSetting(Cis_SubMailPrefix);
        }
        public static string GetSubphone()
        {
            return GetAppSetting(Cis_SubPhonePrefix);
        }
        public static string GetSubaddress()
        {
            return GetAppSetting(Cis_SubAddressPrefix);
        }
        public static string GetAddresstype()
        {
            return GetAppSetting(Cis_AddressTypePrefix);
        }
        public static string GetSubScriptionType()
        {
            return GetAppSetting(Cis_SubScriptionTypePrefix);
        }
        public static string GetCustomerPhone()
        {
            return GetAppSetting(Cis_CustomerPhonePrefix);
        }
        public static string GetCustomerEmail()
        {
            return GetAppSetting(Cis_CustomerEmailPrefix);
        }
        public static string GetCountry()
        {
            return GetAppSetting(Cis_CountryPrefix);
        }

        #endregion

        #region "HP Service"
        public static string GetHpActivity()
        {
            return GetAppSetting(Hp_ActivityPrefix);
        }
        #endregion

        #region "Common Appsettings"
        
        public static string GetSoftwareVersion()
        {
            return GetAppSetting(SoftwareVersion);
        }

        public static int GetServiceRetryInterval()
        {
            return GetAppSetting(ServiceRetryInterval).ToNullable<int>() ?? 120;
        }

        public static int GetServiceRetryNo()
        {
            return GetAppSetting(ServiceRetryNo).ToNullable<int>() ?? 3;
        }

        public static int GetTotalCountToProcess()
        {
            return GetAppSetting(TotalCountToProcess).ToNullable<int>() ?? 5;
        }

        #endregion

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
    }
}