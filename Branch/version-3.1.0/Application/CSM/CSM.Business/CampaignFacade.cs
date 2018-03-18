using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using CSM.Business.Common;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Campaign;
using log4net;
using CSM.Service.Messages.Common;
using CMT = CSM.Service.CmtService;
using SLM = CSM.Service.LeadService;
using System.IO;
using System.Globalization;
using CSM.Data.DataAccess;

namespace CSM.Business
{
    public class CampaignFacade : BaseFacade, ICampaignFacade
    {
        private ICommonFacade _commonFacade;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CampaignFacade));

        private readonly CSMContext _context;

        public CampaignFacade()
        {
            _context = new CSMContext();
        }

        public List<CampaignDetail> GetCampaignListByCustomer(AuditLogEntity auditLog, string cardNo, string hasOffered,
            string isInterested, string customerFlag, int campaignNum)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CmtService.GetCampaignByCustomers").Add("CardNo", cardNo.MaskCardNo())
                .Add("HasOffered", hasOffered).Add("IsInterested", isInterested).Add("CustomerFlag", customerFlag).ToInputLogString());

            RecommendCampaignResponse resCamp = GetCampaignByCustomer(auditLog, cardNo, hasOffered, isInterested, customerFlag, campaignNum);

            //RecommendCampaignResponse resCamp = new RecommendCampaignResponse()
            //{
            //    RecommendCampaignDetails = new List<CampaignDetail>()
            //    {
            //          new CampaignDetail { CampaignId = "XXXXX"},
            //          new CampaignDetail { CampaignId = "XXXXX"},
            //          new CampaignDetail { CampaignId = "XXXXX"}
            //    }
            //};

            return resCamp != null ? resCamp.RecommendCampaignDetails : null;
        }

        public UpdateCampaignFlagsResponse SaveCampaignFlags(AuditLogEntity auditLog, string cardNo, CampaignSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CmtService.UpdateCustomerFlags").Add("CardNo", cardNo.MaskCardNo())
                .Add("HasOffered", searchFilter.HasOffered).Add("IsInterested", searchFilter.IsInterested)
                .Add("UpdatedBy", searchFilter.UpdatedBy).ToInputLogString());

            return UpdateCampaignFlagsByCustomer(auditLog, cardNo, searchFilter);
        }

        public Ticket CreateLead(AuditLogEntity auditLog, CampaignSearchFilter searchFilter)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call LeadService.InsertLead").Add("CampaignId", searchFilter.CampaignId)
                .Add("FirstName", searchFilter.FirstName).Add("LastName", searchFilter.LastName).Add("PhoneNo", searchFilter.PhoneNo)
                .Add("Email", searchFilter.Email).ToInputLogString());

            return InsertLead(auditLog, searchFilter);
        }

        public List<CampaignServiceEntity> GetCampaignList(int? productGroupId = null, int? productId = null)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call CampaignFacade.GetCampaignList").Add("productGroupId", productGroupId)
                .Add("productId", productId).ToInputLogString());
            return (new CampaignServiceDataAccess(_context)).GetCampaign(productGroupId, productId).ToList();
        }

        #region "CMT"

        private RecommendCampaignResponse GetCampaignByCustomer(AuditLogEntity auditLog, string cardNo, string hasOffered,
            string isInterested, string customerFlag, int campaignNum)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CmtService.CampaignByCustomer--");

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.CampaignByCustomer);
                CMT.Header2 header = new CMT.Header2
                {
                    password = profile.password,
                    reference_no = profile.reference_no,
                    service_name = profile.service_name,
                    system_code = profile.system_code,
                    transaction_date = profile.transaction_date,
                    user_name = profile.user_name
                };

                CMT.CampaignByCustomersRequest reqCamp = new CMT.CampaignByCustomersRequest
                {
                    header = header,
                    CampByCitizenIdBody = new CMT.ReqCampByCusEntity
                    {
                        CitizenId = cardNo,
                        HasOffered = hasOffered,
                        IsInterested = isInterested,
                        CustomerFlag = customerFlag,
                        Command = profile.command,
                        RequestDate = DateTime.Now.FormatDateTime("yyyyMMdd"),
                        Channel = profile.channel_id,
                        CampaignNum = campaignNum,
                        ProductTypeId = "0,2"
                    }
                };

                CMT.CampaignByCustomersResponse resCamp = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                // Avoid error codes
                _commonFacade = new CommonFacade();
                List<string> exceptionErrorCodes = _commonFacade.GetExceptionErrorCodes(Constants.SystemName.CMT, Constants.ServiceName.CampaignByCustomer);

                try
                {
                    Retry.Do(() =>
                    {
                        flgCatchErrorCode = string.Empty;
                        Logger.DebugFormat("-- XMLRequest --\n{0}", reqCamp.SerializeObject());
                        using (var client = new CMT.CmtServiceClient())
                        {
                            resCamp = ((CMT.ICmtService)client).CampaignByCustomers(reqCamp);
                            if (client != null)
                            {
                                ((ICommunicationObject) client).Abort();
                            }
                        }

                    }, TimeSpan.FromSeconds(WebConfig.GetServiceRetryInterval()), WebConfig.GetServiceRetryNo());
                }
                catch (AggregateException aex)
                {
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });
                }

                if (!string.IsNullOrEmpty(flgCatchErrorCode))
                {
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(flgCatchErrorCode, true));
                    throw new CustomException(GetMessageResource(flgCatchErrorCode, false));
                }

                #endregion

                if (resCamp != null)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", resCamp.SerializeObject());

                    RecommendCampaignResponse response = new RecommendCampaignResponse();
                    response.StatusResponse.Status = resCamp.status.status;
                    response.StatusResponse.ErrorCode = resCamp.status.error_code;
                    response.StatusResponse.Description = resCamp.status.description;

                    if (exceptionErrorCodes != null && exceptionErrorCodes.Contains(resCamp.status.error_code))
                    {
                        response.StatusResponse.Status = Constants.StatusResponse.Success;
                    }

                    if (Constants.StatusResponse.Success.Equals(response.StatusResponse.Status))
                    {
                        if (resCamp.detail != null)
                        {
                            response.CitizenId = resCamp.detail.CitizenId;

                            if (resCamp.detail.CitizenIds != null && resCamp.detail.CitizenIds.Count() > 0)
                            {
                                var results = resCamp.detail.CitizenIds.Where(x => x != null).Select(x => new CampaignDetail
                                   {
                                       CampaignCriteria = x.CampaignCreiteria,
                                       CampaignDesc = x.CampaignDescription,
                                       CampaignId = x.CampaignId,
                                       CampaignName = x.CampaignName,
                                       CampaignOffer = x.CampaignOffer,
                                       CampaignScore = x.CampaignScore,
                                       Channel = x.Channel,
                                       CitizenIds = x.CitizenIds,
                                       DescCust = x.DescCust,
                                       StrExpireDate = x.ExpireDate,
                                       IsInterested = x.IsInterested,
                                       ContractNoRefer = x.ContractNo,
                                       ProductTypeId = x.ProductTypeId,
                                       ProductTypeName = x.ProductTypeName
                                   });
                                
                                response.RecommendCampaignDetails = results.OrderBy(x => x.ExpireDate).ToList();
                            }
                        }

                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);
                        return response;
                    }

                    // Log DB
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.CMT, Constants.ServiceName.CampaignByCustomer,
                        response.StatusResponse.ErrorCode, true));
                    throw new CustomException(GetMessageResource(Constants.SystemName.CMT, Constants.ServiceName.CampaignByCustomer,
                        response.StatusResponse.ErrorCode, false));
                }
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return null;
        }

        private UpdateCampaignFlagsResponse UpdateCampaignFlagsByCustomer(AuditLogEntity auditLog, string cardNo, CampaignSearchFilter searchFilter)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--CmtService.UpdateCustomerFlags--");

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.CampaignByCustomer);

                var criteria = new CMT.UpdInquiry
                {
                    CampaignId = searchFilter.CampaignId,
                    HasOffered = searchFilter.HasOffered,
                    IsInterested = searchFilter.IsInterested,
                    UpdatedBy = searchFilter.UpdatedBy,
                    Comments = searchFilter.Comments,
                    Command = profile.command
                };

                CMT.UpdateCustomerFlagsRequest reqCamp = new CMT.UpdateCustomerFlagsRequest
                {
                    header = new CMT.Header
                    {
                        password = profile.password,
                        reference_no = profile.reference_no,
                        service_name = profile.service_name,
                        system_code = profile.system_code,
                        transaction_date = profile.transaction_date,
                        user_name = profile.user_name
                    },
                    UpdateCustFlag = new CMT.ReqUpdFlagEntity
                    {
                        CitizenId = cardNo,
                        UpdInquiries = new CMT.UpdInquiry[] { criteria }
                    }
                };

                CMT.UpdateCustomerFlagsResponse resCamp = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                try
                {
                    Retry.Do(() =>
                    {
                        flgCatchErrorCode = string.Empty;
                        Logger.DebugFormat("-- XMLRequest --\n{0}", reqCamp.SerializeObject());
                        using (var client = new CMT.CmtServiceClient())
                        {
                            resCamp = ((CMT.ICmtService)client).UpdateCustomerFlags(reqCamp);
                            if (client != null)
                            {
                                ((ICommunicationObject) client).Abort();
                            }
                        }

                    }, TimeSpan.FromSeconds(WebConfig.GetServiceRetryInterval()), WebConfig.GetServiceRetryNo());
                }
                catch (AggregateException aex)
                {
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });
                }

                if (!string.IsNullOrEmpty(flgCatchErrorCode))
                {
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(flgCatchErrorCode, true));
                    throw new CustomException(GetMessageResource(flgCatchErrorCode, false));
                }

                #endregion

                if (resCamp != null)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", resCamp.SerializeObject());

                    UpdateCampaignFlagsResponse response = new UpdateCampaignFlagsResponse();
                    response.StatusResponse.Status = resCamp.status.status;
                    response.StatusResponse.ErrorCode = resCamp.status.error_code;
                    response.StatusResponse.Description = resCamp.status.description;

                    if (Constants.StatusResponse.Success.Equals(response.StatusResponse.Status))
                    {
                        response.CitizenId = resCamp.detail.CitizenId;
                        if (resCamp.detail.Result != null)
                            response.UpdateStatus = resCamp.detail.Result.UpdateStatus;

                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);
                        return response;
                    }

                    // Log DB
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.CMT, Constants.ServiceName.UpdateCustomerFlags,
                        response.StatusResponse.ErrorCode, true));
                    throw new CustomException(GetMessageResource(Constants.SystemName.CMT, Constants.ServiceName.UpdateCustomerFlags,
                        response.StatusResponse.ErrorCode, false));
                }
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return null;
        }

        #endregion

        #region "SLM"

        private Ticket InsertLead(AuditLogEntity auditLog, CampaignSearchFilter searchFilter)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--LeadService.InsertLead--");

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.InsertLead);

                SLM.InsertLeadRequest reqLead = new SLM.InsertLeadRequest();
                reqLead.RequestHeader = new SLM.Header
                {
                    ChannelID = profile.channel_id,
                    Encoding = "",
                    Username = profile.user_name,
                    Password = profile.password,
                    Version = ""
                };

                DataSet ds = ReadInsertLeadXml();
                DataTable dtMandatory = ds.Tables["mandatory"];
                dtMandatory.Rows[0]["firstname"] = searchFilter.FirstName;
                dtMandatory.Rows[0]["telNo1"] = searchFilter.PhoneNo;
                dtMandatory.Rows[0]["campaign"] = searchFilter.CampaignId;
                DataTable dtCustomerInfo = ds.Tables["customerInfo"];
                dtCustomerInfo.Rows[0]["lastname"] = searchFilter.LastName;
                dtCustomerInfo.Rows[0]["email"] = searchFilter.Email;
                dtCustomerInfo.Rows[0]["cid"] = searchFilter.CardNo;
                dtCustomerInfo.Rows[0]["contractNoRefer"] = searchFilter.ContractNoRefer ?? string.Empty;
                DataTable dtCustomerDetail = ds.Tables["customerDetail"];
                dtCustomerDetail.Rows[0]["availableTime"] = searchFilter.AvailableTime.ToSLMTimeFormat();
                dtCustomerDetail.Rows[0]["detail"] = searchFilter.Comments;
                DataTable dtAppInfo = ds.Tables["appInfo"];
                dtAppInfo.Rows[0]["lastOwner"] = searchFilter.OwnerLeadCode;
                reqLead.RequestXml = ds.ToXml();

                SLM.InsertLeadResponse resLead = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                try
                {
                    Retry.Do(() =>
                    {
                        flgCatchErrorCode = string.Empty;
                        Logger.DebugFormat("-- XMLRequest --\n{0}", reqLead.SerializeObject());

                        using (var client = new SLM.LeadServiceClient())
                        {
                            resLead = ((SLM.ILeadService)client).InsertLead(reqLead);
                            if (client != null)
                            {
                                ((ICommunicationObject) client).Abort();
                            }
                        }

                    }, TimeSpan.FromSeconds(WebConfig.GetServiceRetryInterval()), WebConfig.GetServiceRetryNo());
                }
                catch (AggregateException aex)
                {
                    aex.Handle((x) =>
                    {
                        if (x is EndpointNotFoundException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("EndpointNotFoundException occur:\n", x);
                            return true;
                        }
                        else if (x is CommunicationException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0002;
                            Logger.Error("CommunicationException occur:\n", x);
                            return true;
                        }
                        else if (x is TimeoutException)
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0001;
                            Logger.Error("TimeoutException occur:\n", x);
                            return true;
                        }
                        else
                        {
                            flgCatchErrorCode = Constants.ErrorCode.CSM0003;
                            Logger.Error("Exception occur:\n", x);
                            return true;
                        }
                    });
                }

                if (!string.IsNullOrEmpty(flgCatchErrorCode))
                {
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(flgCatchErrorCode, true));
                    throw new CustomException(GetMessageResource(flgCatchErrorCode, false));
                }

                #endregion

                if (resLead != null)
                {
                    Logger.DebugFormat("-- XMLResponse --\n{0}", resLead.SerializeObject());
                    var xdoc = XDocument.Parse(resLead.ResponseStatus);
                    Ticket ticket = (xdoc.Descendants("ticket")
                                .Select(x => new Ticket
                                {
                                    TicketId = x.Attribute("id").Value,
                                    ResponseCode = x.Element("responseCode").Value,
                                    ResponseMessage = x.Element("responseMessage").Value,
                                    StrResponseDate = x.Element("responseDate").Value,
                                    StrResponseTime = x.Element("responseTime").Value,
                                })).FirstOrDefault();

                    if (Constants.TicketResponse.SLMSuccess.Equals(ticket.ResponseCode))
                    {
                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);
                        return ticket;
                    }

                    // Log DB
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.SLM, Constants.ServiceName.InsertLead,
                        ticket.ResponseCode, true));
                    throw new CustomException(GetMessageResource(Constants.SystemName.SLM, Constants.ServiceName.InsertLead,
                        ticket.ResponseCode, false));
                }
            }
            finally
            {
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return null;
        }

        #endregion

        #region "Functions"

        private dynamic GetHeaderByServiceName<T>(string serviceName)
        {
            _commonFacade = new CommonFacade();
            return _commonFacade.GetHeaderByServiceName<T>(serviceName);
        }

        private static string GetWebServiceXml(string fileName)
        {
            try
            {
                var appDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/Xml/WebService");

                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}.xml", appDataPath, fileName);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return null;
        }

        private static DataSet ReadInsertLeadXml()
        {
            DataSet ds = null;
            const string fileName = "InsertLeadRequest";

            try
            {
                ds = new DataSet();
                ds.Locale = CultureInfo.CurrentCulture;
                ds.ReadXml(GetWebServiceXml(fileName));
                return ds;
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                    ds = null;
                }
            }
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
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                    if (_context != null) { _context.Dispose(); }
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
