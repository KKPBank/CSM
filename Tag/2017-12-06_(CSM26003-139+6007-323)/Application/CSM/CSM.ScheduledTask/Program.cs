using System;
using CSM.ScheduledTask.Mail;
using CSM.ScheduledTask.AFS;
using CSM.ScheduledTask.BDW;
using CSM.ScheduledTask.CIS;
using CSM.ScheduledTask.HP;
using CSM.ScheduledTask.ServiceRequest;
using log4net;
using System.Globalization;

namespace CSM.ScheduledTask
{
    class Program
    {
        private static ILog _log;

        static void Main(string[] args)
        {
            try
            {
                // Set logfile name and application name variables
                log4net.GlobalContext.Properties["ApplicationCode"] = "CSM_SCHEDTASK";
                log4net.GlobalContext.Properties["ServerName"] = System.Environment.MachineName;
                log4net.ThreadContext.Properties["UserID"] = GetCurrentUser();
                _log = LogManager.GetLogger(typeof(Program));
            }
            catch (Exception ex)
            {
                _log.Error("Exception occur:\n", ex);
            }

            //for (int i = 0; i < args.Length; i++)
            //    Console.WriteLine("Arg: {0}", args[i]);

            foreach (string arg in args)
            {
                var commandLine = arg.Substring(0, 2);
                switch (commandLine.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "/M":
                        _log.Info("I:--START--:--Get Mailbox--");
                        MailProcess.GetMailboxJobAsync();
                        _log.Info("O:--SUCCESS--:--Get Mailbox--");
                        break;
                    case "/A":
                        _log.Info("I:--START--:--Get AFSFile--");
                        ReadFileProcess.GetFileAFSJobAsync();
                        _log.Info("O:--SUCCESS--:--Get AFSFile--");
                        break;
                    case "/E":
                        _log.Info("I:--START--:--Export AFSFile--");
                        ExportFileProcess.ExportFileAFSJobAsync();
                        _log.Info("O:--SUCCESS--:--Export AFSFile--");
                        break;
                    case "/N":
                        _log.Info("I:--START--:--Export NCBFile--");
                        ExportFileNCBProcess.ExportFileNCBJobAsync();
                        _log.Info("O:--SUCCESS--:--Export NCBFile--");
                        break;
                    case "/B":
                        _log.Info("I:--START--:--Get BDWFile--");
                        ReadFileBdwProcess.GetFileBDWJobAsync();
                        _log.Info("O:--SUCCESS--:--Get BDWFile--");
                        break;
                    case "/C":
                        _log.Info("I:--START--:--Get CISFile--"); 
                        ReadFileCisProcess.GetFileCISJobAsync();
                        _log.Info("O:--SUCCESS--:--Get CISFile--");
                        break;
                    case "/H":
                        _log.Info("I:--START--:--Get HPFile--");
                        ReadFileHpProcess.GetFileHpJobAsync();                         
                        _log.Info("O:--SUCCESS--:--Get HPFile--");
                        break;
                    case "/R":
                        _log.Info("I:--START--:--Create SR Activity from Reply Email--");
                        ServiceRequestProcess.CreateSRActivityFromReplyEmail();
                        _log.Info("O:--SUCCESS--:--Create SR Activity from Reply Email--");
                        break;
                    case "/S":
                        _log.Info("I:--START--:--Re-Submit SR Activity to CAR System--");
                        ServiceRequestProcess.ReSubmitActivityToCARSystem();
                        _log.Info("O:--SUCCESS--:--Re-Submit SR Activity to CAR System--");
                        break;
                    case "/T":
                        _log.Info("I:--START--:--Re-Submit SR Activity to CBSHP System (Log100)--");
                        ServiceRequestProcess.ReSubmitActivityToCBSHPSystem();
                        _log.Info("O:--SUCCESS--:--Re-Submit SR Activity to CBSHP System (Log100)--");
                        break;
                    default:
                        // do other stuff...
                        break;
                }
            }
        }

        private static string GetCurrentUser()
        {
            try
            {
                string domainName = Environment.UserDomainName.ToUpperInvariant();
                string accountName = Environment.UserName.ToUpperInvariant();
                return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", domainName, accountName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
