using System;
using System.Collections.Generic;
using System.DirectoryServices;
using CSM.Common.Utilities;
using log4net;
using CSM.Common.Resources;

namespace CSM.Common.Securities
{
    public class LdapAuthentication : IDisposable
    {
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LdapAuthentication));

        private string _path;
        private string _email;
        private string _filterCn;
        private string _employeeId;
        private DirectoryEntry _entry;

        public LdapAuthentication()
        {
            _path = WebConfig.GetLdapConnectionString();
        }

        public string GetEmployeeId()
        {
            return _employeeId;
        }

        public string GetEmail()
        {
            return _email;
        }

        public string Login(string username, string passwd)
        {
            return CheckAuthenticated(WebConfig.GetLdapDomain(), username, passwd);
        }

        private string CheckAuthenticated(string domain, string username, string passwd)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Check Authenticated").Add("Domain", domain).Add("Username", username).ToInputLogString());

            DirectorySearcher search = null;
            string domainAndUsername = domain + @"\" + username.ToLower().NullSafeTrim();
            _entry = new DirectoryEntry(_path, domainAndUsername, passwd);

            // Check user lock
            string rootDomain = WebConfig.GetLdapDomain();
            string rootUsername = WebConfig.GetLdapUsername();
            string rootPassword = WebConfig.GetLdapPassword();
            string rootDomainAndUsername = rootDomain + @"\" + rootUsername;

            try
            {
                // Bind to the native AdsObject to force authentication.
                //object obj = entry.NativeObject;

                // Find cn and userAccountControl filter by SAMAccountName
                search = new DirectorySearcher(_entry);
                search.Filter = string.Format("(SAMAccountName={0})", username);
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("description");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("userAccountControl");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return "NO_RESULT";
                }

                // Update the new path to the user in the directory.
                _path = result.Path;
                _filterCn = (string) result.Properties["cn"][0];

                // Check employeeId (in description parameter)
                if (result.Properties["description"].Count != 0)
                    _employeeId = (string) result.Properties["description"][0];
                else
                    _employeeId = string.Empty;

                // Check email (in mail parameter)
                if (result.Properties["mail"].Count != 0)
                    _email = (string) result.Properties["mail"][0];
                else
                    _email = string.Empty;

                Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Domain", domain).Add("Username", username)
                    .Add("Path", _path).Add("CN", _filterCn).Add("EmployeeID", _employeeId).Add("Email", _email).ToSuccessLogString());

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Domain", domain)
                    .Add("Username", username).Add("Error Message", ex.Message).ToFailLogString());

                DirectorySearcher ds = null;
                DirectoryEntry rootEntry = null;

                try
                {
                    rootEntry = new DirectoryEntry(_path, rootDomainAndUsername, rootPassword);
                    ds = new DirectorySearcher(rootEntry);
                    ds.SearchScope = SearchScope.Subtree;
                    ds.Filter = String.Format("(&(objectCategory=person)(anr={0}))", username);

                    Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication")
                        .Add("ds.Filter", string.Format("(&(objectCategory=person)(anr={0}))", username))
                        .ToOutputLogString());

                    SearchResultCollection src = ds.FindAll();
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username)
                        .Add("src.Count", src.Count).ToOutputLogString());

                    Int64 lockoutTime = 0;

                    foreach (SearchResult sr in src)
                    {
                        if ((sr != null) && (sr.Properties["userAccountControl"].Count > 0))
                        {
                            ResultPropertyValueCollection uacCollection = sr.Properties["userAccountControl"];
                            Int32 userAccountControl = (Int32) uacCollection[0];
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username)
                                .Add("UserAccountControl", userAccountControl).ToOutputLogString());

                            if (sr.Properties["lockouttime"].Count > 0)
                            {
                                ResultPropertyValueCollection valueCollection = sr.Properties["lockouttime"];
                                lockoutTime = (Int64) valueCollection[0];
                            }

                            Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username)
                                .Add("LockoutTime", lockoutTime).ToOutputLogString());

                            // Check userAccountControl
                            bool enabledUser = true;
                            IList<string> uacDisabled = StringHelpers.ConvertStringToList(WebConfig.GetLdapUacDisabled());

                            for (int i = 0; i < uacDisabled.Count; i++)
                            {
                                if (userAccountControl == Int32.Parse(uacDisabled[i]))
                                    enabledUser = false;
                            }

                            Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username)
                                .Add("AccountEnabled", enabledUser).ToOutputLogString());

                            if (!enabledUser)
                                return "ACCOUNTDISABLE";
                            else if (lockoutTime > 0)
                                return "LOCKOUT";
                        }
                    }
                }
                catch (InvalidOperationException exc)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username).Add("Error Message", exc.Message).ToFailLogString());
                    Logger.Error("InvalidOperationException occur:\n", ex);
                    throw new CustomException(Resource.Msg_CannotConnectToAD);
                }
                catch (NotSupportedException exc)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username).Add("Error Message", exc.Message).ToFailLogString());
                    Logger.Error("NotSupportedException occur:\n", ex);
                    throw new CustomException(Resource.Msg_CannotConnectToAD);
                }
                catch (Exception exc)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("01:LdapAuthentication").Add("Username", username).Add("Error Message", exc.Message).ToFailLogString());
                    Logger.Error("Exception occur:\n", ex);
                    throw new CustomException(Resource.Msg_CannotConnectToAD);
                }
                finally
                {
                    if (ds != null)
                    {
                        ds.Dispose();
                        ds = null;
                    }
                    if (rootEntry != null)
                    {
                        rootEntry.Dispose();
                        rootEntry = null;
                    }
                }

                return "FAILED";
            }
            finally
            {
                if (search != null)
                {
                    search.Dispose();
                    search = null;
                }
            }
        }

        #region "IDisposable"

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _entry.Dispose();
                }
            }
            _disposed = true;
        }

        #endregion
    }
}