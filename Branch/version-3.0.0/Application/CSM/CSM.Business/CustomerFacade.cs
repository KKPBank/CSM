﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using CSM.Business.Common;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.Common;
using log4net;
using COC = CSM.Service.CentralizeOperationCenterService;
//using CBS = CSM.Service.
using System.Globalization;

namespace CSM.Business
{
    public class CustomerFacade : BaseFacade, ICustomerFacade
    {
        private readonly CSMContext _context;
        private ICommonFacade _commonFacade;
        private ICustomerDataAccess _customerDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CustomerFacade));

        public CustomerFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<RelationshipEntity> GetAllRelationships(RelationshipSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAllRelationships(searchFilter);
        }

        public RelationshipEntity GetRelationshipByID(int relationshipId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetRelationshipByID(relationshipId);
        }
        public bool IsDuplicateRelationship(RelationshipEntity relationEntity)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.IsDuplicateRelationship(relationEntity);
        }

        public bool SaveContactRelation(RelationshipEntity relationEntity)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveContactRelation(relationEntity);
        }

        public IEnumerable<CustomerEntity> SearchCustomer(CustomerSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SearchCustomer(searchFilter);
        }

        //public IEnumerable<CustomerEntity> GetCustomerList(CustomerSearchFilter searchFilter)
        //{
        //    _customerDataAccess = new CustomerDataAccess(_context);
        //    return _customerDataAccess.GetCustomerList(searchFilter);
        //}

        public CustomerEntity GetCustomerByID(int customerId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerByID(customerId);
        }

        public List<CustomerEntity> GetCustomerByPhoneNo(int? customerId, List<string> lstPhoneNo)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerByPhoneNo(customerId, lstPhoneNo);
        }

        public List<CustomerEntity> GetCustomerByName(string customerTHName)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerByName(customerTHName);
        }

        public bool SaveCustomer(CustomerEntity customerEntity)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveCustomer(customerEntity);
        }

        public bool IsDuplicateCardNo(int? customerId, int? subscriptTypeId, string cardNo)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.IsDuplicateCardNo(customerId, subscriptTypeId, cardNo);
        }

        public List<int?> GetCustomerIdWithCallId(string phoneNo)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerIdWithCallId(phoneNo);
        }

        public IEnumerable<NoteEntity> GetNoteList(NoteSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetNoteList(searchFilter);
        }

        public NoteEntity GetNoteByID(int noteId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetNoteByID(noteId);
        }

        public bool SaveNote(NoteEntity noteEntity)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveNote(noteEntity);
        }

        public IEnumerable<AttachmentEntity> GetAttachmentList(AttachmentSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAttachmentList(searchFilter);
        }

        public AttachmentEntity GetAttachmentByID(int attachmentId, string documentLevel)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAttachmentByID(attachmentId, documentLevel);
        }

        public void SaveCustomerAttachment(AttachmentEntity attachment)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            _customerDataAccess.SaveCustomerAttachment(attachment);
        }

        public void DeleteCustomerAttachment(int attachmentId, int updateBy)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            _customerDataAccess.DeleteCustomerAttachment(attachmentId, updateBy);
        }

        public IEnumerable<AccountEntity> GetAccountList(AccountSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountList(searchFilter);
        }

        public IEnumerable<ContactEntity> GetContactList(ContactSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetContactList(searchFilter);
        }

        public bool DeleteCustomerContact(int customerContactId, int updateBy)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.DeleteCustomerContact(customerContactId, updateBy);
        }

        public ContactEntity GetContactByID(int contactId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetContactByID(contactId);
        }

        public bool SaveContact(ContactEntity contactEntity, List<CustomerContactEntity> lstCustomerContact)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveContact(contactEntity, lstCustomerContact);
        }

        public List<CustomerContactEntity> GetContactRelationshipList(int contactId, int customerId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetContactRelationshipList(contactId, customerId);
        }

        public List<AccountEntity> GetAccountByCustomerId(int customerId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountByCustomerId(customerId);
        }

        public List<ContactEntity> GetContactByPhoneNo(int? contactId, string firstNameTh, string lastNameTh,
            string firstNameEn, string lastNameEn, List<string> lstPhoneNo)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetContactByPhoneNo(contactId, firstNameTh, lastNameTh, firstNameEn, lastNameEn, lstPhoneNo);
        }

        public IEnumerable<SrEntity> GetSrList(SrSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetSrList(searchFilter);
        }

        public IEnumerable<SrEntity> GetCustomerSrList(SrSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerSrList(searchFilter);
        }

        public IEnumerable<CustomerLogEntity> GetCustomerLogList(CustomerLogSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCustomerLogList(searchFilter);
        }

        public Ticket GetLeadList(AuditLogEntity auditLog, string cardNo)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call COCService.SearchLead").ToInputLogString());

            return SearchLead(auditLog, cardNo);
        }

        public bool SaveCallId(string callId, string phoneNo, string cardNo, string callType, int userId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveCallId(callId, phoneNo, cardNo, callType, userId);
        }

        public CallInfoEntity GetCallInfoByCallId(string callId)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetCallInfoByCallId(callId);
        }

        public ExistingProductEntity GetExistingProductDetail(ExistingProductSearchFilter searchFilter)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetExistingProductDetail(searchFilter);
        }

        public bool SaveContactSr(ContactEntity contactEntity)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.SaveContactSr(contactEntity);
        }

        #region "Inquiry CBS Customer Webservice"
        //IEnumerable<CustomerEntity> getCustomerByInformationWS(CustomerSearchFilter searchFileter, WebServicePagingEntity vPaging)
        //{

        //    IEnumerable<CustomerEntity> result = new List<CustomerEntity>();

        //    try
        //    {
        //        InquiryCustomerByInformation req = new InquiryCustomerByInformation();

        //        _commonFacade = new CommonFacade();
        //        Header profile = _commonFacade.GetHeaderByServiceName<Header>(Constants.ServiceName.InquiryCustomerByInformation);

        //        req.Header = new InquiryCustomerByInformationHeader
        //        {
        //            ReferenceNo = DateTime.Now.FormatDateTime(Constants.DateTimeFormat.WebserviceReferenceNoDateTime),
        //            TransactionDateTime = DateTime.Now,
        //            ServiceName = Constants.ServiceName.InquiryCustomerByInformation,
        //            SystemCode = profile.system_code,
        //            ChannelID = profile.channel_id
        //        };

        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("FirstName", searchFileter.FirstName).Add("LastName", searchFileter.LastName).Add("CardNo", searchFileter.CardNo).Add("PhoneNo", searchFileter.PhoneNo));

        //        string searchFirstName = string.IsNullOrEmpty(searchFileter.FirstName) ? "" : searchFileter.FirstName;
        //        string searchLastName = string.IsNullOrEmpty(searchFileter.LastName) ? "" : searchFileter.LastName;
        //        req.InquiryCustomerByInformationRequest = new InquiryCustomerByInformationInquiryCustomerByInformationRequest();

        //        if (searchFirstName != "")
        //        {
        //            req.InquiryCustomerByInformationRequest.FirstName = searchFirstName;
        //            if (searchFirstName.StartsWith("*") || searchFirstName.EndsWith("*"))
        //            {
        //                req.InquiryCustomerByInformationRequest.FirstName = searchFirstName.Replace("*", "%");
        //                req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "N";
        //            }
        //            else
        //            {
        //                req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "Y";
        //            }
        //        }
        //        else
        //        {
        //            //req.InquiryCustomerByInformationRequest.ExactSearchFirstNameFlag = "N";
        //        }

        //        if (searchLastName != "")
        //        {
        //            req.InquiryCustomerByInformationRequest.LastName = searchLastName;
        //            if (searchLastName.StartsWith("*") || searchLastName.EndsWith("*"))
        //            {
        //                req.InquiryCustomerByInformationRequest.LastName = searchLastName.Replace("*", "%");
        //                req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "N";
        //            }
        //            else
        //            {
        //                req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "Y";
        //            }
        //        }
        //        else
        //        {
        //            //req.InquiryCustomerByInformationRequest.ExactSearchLastNameFlag = "N";
        //        }

        //        if (string.IsNullOrEmpty(searchFileter.CardNo) == false)
        //        {
        //            req.InquiryCustomerByInformationRequest.IDNumber = searchFileter.CardNo;
        //        }

        //        if (string.IsNullOrEmpty(searchFileter.PhoneNo) == false)
        //        {
        //            req.InquiryCustomerByInformationRequest.ElectronicAddress = string.IsNullOrEmpty(searchFileter.PhoneNo) ? "" : searchFileter.PhoneNo;
        //        }

        //        req.Paging = new InquiryCustomerByInformationPaging();
        //        req.Paging.PageNumber = vPaging.PageNumber;
        //        req.Paging.PageSize = vPaging.PageSize;

        //        InquiryCustomerByInformationResponse res = new InquiryCustomerByInformationResponse();
        //        res = ws.InquiryCustomerByInformation(req);
        //        if (res.Header.CustomerList != null)
        //        {
        //            result = (from c in res.Header.CustomerList
        //                      select new CustomerEntity
        //                      {
        //                          CustomerId = 0,
        //                          CIF_ID = Convert.ToDecimal(c.CustomerNumber),
        //                          CustomerNumber = Convert.ToDecimal(c.CustomerNumber),
        //                          FirstNameThai = c.PrimaryName,
        //                          FirstNameEnglish = c.AlternateFirstName,
        //                          LastNameThai = c.PrimaryLastName,
        //                          LastNameEnglish = c.AlternateLastName,
        //                          FirstNameThaiEng = !string.IsNullOrEmpty(c.PrimaryName) ? c.PrimaryName : c.AlternateFirstName,
        //                          LastNameThaiEng = !string.IsNullOrEmpty(c.PrimaryLastName) ? c.PrimaryLastName : c.AlternateLastName,
        //                          BirthDate = c.BirthDate,
        //                          TitleThai = new TitleEntity { TitleName = c.Title },
        //                          TitleEnglish = new TitleEntity { TitleName = c.AlternateTitle },
        //                          CardNo = c.IDNumber,
        //                          IDTypeCode = c.IDTypeCode,
        //                          SubscriptType = new SubscriptTypeEntity { SubscriptTypeCode = c.IDTypeCode, SubscriptTypeName = c.IDTypeCode },
        //                          CountryOfCitizenship = c.CountryOfCitizenship,
        //                          //AccountNo = c.ACCOUNT_NO,
        //                          //Account = new AccountEntity
        //                          //{
        //                          //    AccountNo = c.ACCOUNT_NO,
        //                          //    AccountId = c.ACCOUNT_ID,
        //                          //    Product = c.PRODUCT,
        //                          //    ProductGroup = c.PRODUCT_GROUP,
        //                          //    Status = c.STATUS,
        //                          //    BranchCode = c.BRANCH_CODE,
        //                          //    BranchName = c.BRANCH_NAME,
        //                          //    AccountDesc = c.ACCOUNT_DESC,
        //                          //    Grade = c.GRADE
        //                          //},
        //                          CustomerType = Constants.CustomerType.Customer,
        //                          CustomerCategory = c.CustomerCategory,
        //                          //SubscriptType = new SubscriptTypeEntity
        //                          //{
        //                          //    SubscriptTypeId = c.SUBSCRIPT_TYPE_ID,
        //                          //    SubscriptTypeName = c.SUBSCRIPT_TYPE_NAME,
        //                          //},
        //                          //Registration = c.CAR_NO,
        //                          //StrPhoneNo = c.PhoneList,
        //                      }).AsQueryable();

        //            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Found Data", result.Count<CustomerEntity>() + " Row(s)"));
        //        }
        //        else
        //        {
        //            Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("ReferenceNo:", req.Header.ReferenceNo).Add("Not Found Data", ""));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Exception occur:\n", ex);
        //        Logger.Info(_logMsg.Clear().SetPrefixMsg("InquiryCustomerByInformation").Add("Error Message", ex.Message).ToFailLogString());
        //    }


        //    return result;
        //}
        #endregion

        #region "COC"

        private Ticket SearchLead(AuditLogEntity auditLog, string cardNo)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Debug("I:--START--:--COCService.SearchLead--");

            try
            {
                Header profile = GetHeaderByServiceName<Header>(Constants.ServiceName.SearchLead);

                COC.ServiceRequest reqLead = new COC.ServiceRequest();
                reqLead.RequestHeader = new COC.Header
                {
                    ChannelID = profile.channel_id,
                    Username = profile.user_name,
                    Password = profile.password,
                    System = profile.system_code
                };

                DataSet ds = ReadSearchLeadXml();
                DataTable dt = ds.Tables[0];
                dt.Rows[0]["cid"] = cardNo;
                dt.Rows[0]["firstname"] = "";
                dt.Rows[0]["lastname"] = "";
                dt.Rows[0]["status"] = "";
                dt.Rows[0]["productGroupId"] = "";
                dt.Rows[0]["productId"] = "";
                dt.Rows[0]["maxRecord"] = "30";
                reqLead.RequestXml = ds.ToXml();

                COC.ServiceResponse resLead = null;

                #region "Call Service"

                string flgCatchErrorCode = string.Empty;

                // Avoid error codes
                _commonFacade = new CommonFacade();
                List<string> exceptionErrorCodes = _commonFacade.GetExceptionErrorCodes(Constants.SystemName.COC, Constants.ServiceName.SearchLead);

                try
                {
                    Retry.Do(() =>
                    {
                        flgCatchErrorCode = string.Empty;
                        Logger.DebugFormat("-- XMLRequest --\n{0}", reqLead.SerializeObject());

                        using (var client = new COC.ServiceClient())
                        {
                            resLead = ((COC.IService)client).SearchLead(reqLead);
                            if (client != null)
                            {
                                ((ICommunicationObject)client).Abort();
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

                    var xdoc = XDocument.Parse(resLead.ResponseXml);

                    Ticket ticket = (xdoc.Descendants("ticket")
                                .Select(x => new Ticket
                                {
                                    TicketId = x.Attribute("id").Value,
                                    ResponseCode = x.Element("responseCode").Value,
                                    ResponseMessage = x.Element("responseMessage").Value,
                                    StrResponseDate = x.Element("responseDate").Value,
                                    StrResponseTime = x.Element("responseTime").Value,
                                    TotalLeads = x.Element("totalLeads") == null ? string.Empty : x.Element("totalLeads").Value
                                })).FirstOrDefault();


                    if (exceptionErrorCodes != null && exceptionErrorCodes.Contains(ticket.ResponseCode))
                    {
                        ticket.ResponseCode = Constants.TicketResponse.COCSuccess;
                    }

                    if (Constants.TicketResponse.COCSuccess.Equals(ticket.ResponseCode))
                    {
                        List<TicketItem> items = null;

                        if (xdoc.Descendants("result") != null && xdoc.Descendants("result").Count() > 0)
                        {
                            items = xdoc.Descendants("result")
                                .Select(x => new TicketItem
                                {
                                    TicketId = x.Element("ticketId").Value,
                                    CardNo = x.Element("cid").Value,
                                    FirstName = x.Element("firstname").Value,
                                    LastName = x.Element("lastname").Value,
                                    TelNo1 = x.Element("telNo1").Value,
                                    Status = x.Element("status").Value,
                                    StatusDesc = x.Element("statusDesc").Value,
                                    CampaignCode = x.Element("campaign").Value,
                                    CampaignDesc = x.Element("campaignDesc").Value,
                                    ProductGroupId = x.Element("productGroupId").Value,
                                    ProductGroupDesc = x.Element("productGroupDesc").Value,
                                    ProductId = x.Element("productId").Value,
                                    ProductDesc = x.Element("productDesc").Value,
                                    ChannelId = x.Element("channelId").Value,
                                    OwnerLead = x.Element("ownerLead").Value,
                                    OwnerLeadName = x.Element("ownerLeadName").Value,
                                    DelegateLead = x.Element("delegateLead").Value,
                                    DelegateLeadName = x.Element("delegateLeadName").Value,
                                    CreatedUser = x.Element("createdUser").Value,
                                    CreatedUserName = x.Element("createdUserName").Value,
                                    StrCreatedDate = x.Element("createdDate").Value,
                                    StrCreatedTime = x.Element("createdTime").Value,
                                    StrAssignedDate = x.Element("assignedDate").Value,
                                    StrAssignedTime = x.Element("assignedTime").Value,
                                    OwnerBranch = x.Element("ownerBranch").Value,
                                    OwnerBranchName = x.Element("ownerBranchName").Value,
                                    DelegateBranch = x.Element("delegateBranch").Value,
                                    DelegateBranchName = x.Element("delegateBranchName").Value,
                                    CreatedBranch = x.Element("createdBranch").Value,
                                    CreatedBranchName = x.Element("createdBranchName").Value
                                }).ToList();
                        }

                        ticket.Items = items;
                        AppLog.AuditLog(auditLog, LogStatus.Success, string.Empty);
                        return ticket;
                    }

                    // Log DB
                    AppLog.AuditLog(auditLog, LogStatus.Fail, GetMessageResource(Constants.SystemName.COC, Constants.ServiceName.SearchLead,
                        ticket.ResponseCode, true));
                    throw new CustomException(GetMessageResource(Constants.SystemName.COC, Constants.ServiceName.SearchLead,
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

        #region"Lookup Data for AutoComplete"

        public List<AccountEntity> GetAccountBranchByName(string searchTerm, int pageSize, int pageNum)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountBranchByName(searchTerm, pageSize, pageNum);
        }
        public int GetAccountBranchCountByName(string searchTerm)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountBranchCountByName(searchTerm);
        }
        public List<AccountEntity> GetAccountProductByName(string searchTerm, int pageSize, int pageNum)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountProductByName(searchTerm, pageSize, pageNum);
        }
        public int GetAccountProductCountByName(string searchTerm)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountProductCountByName(searchTerm);
        }
        public List<AccountEntity> GetAccountGradeByName(string searchTerm, string product, int pageSize, int pageNum)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountGradeByName(searchTerm, product, pageSize, pageNum);
        }
        public int GetAccountGradeCountByName(string searchTerm, string product)
        {
            _customerDataAccess = new CustomerDataAccess(_context);
            return _customerDataAccess.GetAccountGradeCountByName(searchTerm, product);
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
                throw;
            }
        }

        private static DataSet ReadSearchLeadXml()
        {
            DataSet ds = null;

            try
            {
                const string fileName = "SearchLeadRequest";

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
                    if (_context != null) { _context.Dispose(); }
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
