using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CSM.Service.Messages.OTP;

namespace CSM.WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICSMOtpService" in both code and config file together.
    [ServiceContract]
    public interface ICSMOtpService
    {
        [OperationContract]
        OTPResultSvcResponse OTPRequest(OTPResultSvcRequest req);

        [OperationContract]
        OTPResultSvcResponse OTPResult(OTPResultSvcRequest req);
    }
}
