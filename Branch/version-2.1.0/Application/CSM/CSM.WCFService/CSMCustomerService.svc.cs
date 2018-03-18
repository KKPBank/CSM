using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.Customer;
using CSM.Service.Messages.SchedTask;
using log4net;

namespace CSM.WCFService
{
    public class CSMCustomerService : ICSMCustomerService
    {
        const string Delimeter = "\n";
        private ICisFacade _cisFacade;
        private readonly ILog _logger;
        private ICommonFacade _commonFacade;
        private System.Diagnostics.Stopwatch _stopwatch;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();

        #region "Constructor"

        public CSMCustomerService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMCustomerService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }

        #endregion

        public ImportSubscriptionResponse ImportSubscription(ImportSubscriptionRequest request)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();

            if (request != null && request.Header != null)
            {
                ThreadContext.Properties["UserID"] = request.Header.user_name.ToUpperInvariant();
            }

            _logger.Info(_logMsg.Clear().SetPrefixMsg("Call CustomerService.ImportSubscription").ToInputLogString());
            _logger.Debug("I:--START--:--CustomerService.ImportSubscription--");

            ImportSubscriptionResponse response = new ImportSubscriptionResponse();
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                bool valid = false;
                _commonFacade = new CommonFacade();

                if (request == null)
                {
                    throw new Exception("CustomerService.ImportSubscription : Request is null!");
                }

                if (request.Header != null)
                {
                    valid = _commonFacade.VerifyServiceRequest<Header>(request.Header);
                    response.Header = new Header
                    {
                        reference_no = request.Header.reference_no,
                        service_name = request.Header.service_name,
                        system_code = request.Header.system_code,
                        transaction_date = request.Header.transaction_date
                    };
                }

                _logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());

                #region "Call Validator"

                if (!valid)
                {
                    response.StatusResponse = new StatusResponse
                    {
                        ErrorCode = Constants.ErrorCode.CSMCust001,
                        Status = Constants.StatusResponse.Failed,
                        Description = "Bad Request, the header is not valid"
                    };

                    return response;
                }
                else
                {
                    dynamic[] results = ApplicationHelpers.DoValidation(request);
                    valid = (bool) results[0];
                    List<ValidationResult> validationResults = (List<ValidationResult>) results[1];

                    if (!valid)
                    {
                        response.StatusResponse = new StatusResponse();
                        response.StatusResponse.ErrorCode = Constants.ErrorCode.CSMCust002;
                        response.StatusResponse.Status = Constants.StatusResponse.Failed;

                        if (validationResults != null && validationResults.Count > 0)
                        {
                            string msg = "Bad Request, the body is not valid:\n{0}";
                            response.StatusResponse.Description = string.Format(CultureInfo.InvariantCulture, msg,
                                validationResults.Select(x => x.ErrorMessage).Aggregate((i, j) => i + Delimeter + j));
                        }

                        _logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                        return response;
                    }
                }

                #endregion

                ImportSubscriptionAsync(request.ImportDateValue.Value, request.SkipSFTP).ConfigureAwait(false);

                response.StatusResponse = new StatusResponse
                {
                    ErrorCode = string.Empty,
                    Status = Constants.StatusResponse.Success,
                    Description = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
                response.StatusResponse = new StatusResponse
                {
                    ErrorCode = Constants.ErrorCode.CSMCust003,
                    Status = Constants.StatusResponse.Failed,
                    Description = "Fail to import customer data"
                };
            }
            finally
            {
                _logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                _stopwatch.Stop();
                _logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", _stopwatch.ElapsedMilliseconds);
            }

            return response;
        }

        #region "Functions"

        private async Task<ImportCISTaskResponse> ImportSubscriptionAsync(DateTime importDate, bool skipSFTP)
        {
            string strDate = importDate.FormatDateTime(Constants.DateTimeFormat.ExportAfsDateTime);
            _cisFacade = new CisFacade();
            var task = Task.Run(() => _cisFacade.GetFileCIS(WebConfig.GetTaskUsername(), WebConfig.GetTaskPassword(), skipSFTP, strDate));
            var result = await task.ConfigureAwait(false);
            return result;
        }

        #endregion
        
        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_cisFacade != null) { _cisFacade.Dispose(); }
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
