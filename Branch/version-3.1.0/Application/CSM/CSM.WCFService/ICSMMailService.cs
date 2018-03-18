using System.ServiceModel;
using CSM.Common.Utilities;
using CSM.Service.Messages.SchedTask;
using System;

///<summary>
/// Class Name : ICSMMailService
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.WCFService
{
    [ServiceContract(Namespace = Constants.ServicesNamespace.MailService)]
    public interface ICSMMailService : IDisposable
    {
        [OperationContract]
        JobTaskResponse GetMailbox(string username, string password);
    }
}
