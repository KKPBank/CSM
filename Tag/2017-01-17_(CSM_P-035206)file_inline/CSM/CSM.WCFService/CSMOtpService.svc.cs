using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSM.Service.Messages.OTP;
using CSM.Business;

namespace CSM.WCFService
{
    public class CSMOtpService : ICSMOtpService
    {
        ILog _logger = LogManager.GetLogger(typeof(CSMOtpService));

        public CSMOtpService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMMasterService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        public OTPResultSvcResponse OTPRequest(OTPResultSvcRequest req)
        {
            OTPResultSvcResponse res;
            using (ComplaintFacade facade = new ComplaintFacade())
            {
                res = facade.OTPRequest(req);
            }
            return res;
        }

        public OTPResultSvcResponse OTPResult(OTPResultSvcRequest req)
        {
            OTPResultSvcResponse res;
            using (ComplaintFacade facade = new ComplaintFacade())
            {
                res = facade.OTPResult(req);
            }
            return res;
        }
    }
}
