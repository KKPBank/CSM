using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using log4net;
using CSM.Common.Utilities;

namespace CSM.Business.Common
{
    public class BaseFacade
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseFacade));

        protected static string GetMessageResource(string errorCode, bool auditLog)
        {
            return GetMessageResource(null, null, errorCode, auditLog);
        }

        protected static string GetMessageResource(string systemName, string serviceName, string errorCode, bool auditLog)
        {
            string key = ApplicationHelpers.GetMessageKey(systemName, serviceName, errorCode);
            string value = DatabaseResourceManager.Instance.GetString(key, ApplicationHelpers.GetCultureInfo());

            if (string.IsNullOrWhiteSpace(value))
            {
                key = ApplicationHelpers.GetMessageKey(null, null, Constants.ErrorCode.CSM0003);
                value = DatabaseResourceManager.Instance.GetString(key, ApplicationHelpers.GetCultureInfo());
                Logger.Debug("O:-- Cannot find message resources under key --:ErrorCode/" + errorCode);
            }

            if (!auditLog)
            {
                IList<object> list = StringHelpers.ConvertStringToList(value, ':');
                if (list != null)
                    value = (string)list[1];
            }

            return value;
        }

        protected static dynamic[] DoValidation(object toValidate)
        {
            return ApplicationHelpers.DoValidation(toValidate);
        }
    }
}
