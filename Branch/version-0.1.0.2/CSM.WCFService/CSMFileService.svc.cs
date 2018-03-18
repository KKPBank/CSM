using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using CSM.Business;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.SchedTask;
using log4net;

namespace CSM.WCFService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CSMFileService : ICSMFileService
    {
        private long _elapsedTime;
        private string _logDetail;
        private IAFSFacade _afsFacade;
        private readonly ILog _logger;
        private object sync = new Object();
        private List<SaleZoneEntity> _saleZones;
        private List<PropertyEntity> _properties;
        private System.Diagnostics.Stopwatch _stopwatch;

        private IBdwFacade _bdwFacade;
        private List<BdwContactEntity> _bdwContacts;

        private ICisFacade _cisFacade;
        private List<CisCorporateEntity> _cisCor;
        private List<CisIndividualEntity> _cisIndiv;
        private List<CisProductGroupEntity> _cisProd;
        private List<CisSubscriptionEntity> _cisSub;
        private List<CisTitleEntity> _cisTitle;
        private List<CisProvinceEntity> _cisProvince;
        private List<CisDistrictEntity> _cisDistrict;
        private List<CisSubDistrictEntity> _cisSubDistrict;
        private List<CisPhoneTypeEntity> _cisPhonetype;
        private List<CisEmailTypeEntity> _cisEmailtype;
        private List<CisSubscribeAddressEntity> _cisSubaddress;
        private List<CisSubscribePhoneEntity> _cisSubphone;
        private List<CisSubscribeMailEntity> _cisSubemail;
        private List<CisAddressTypeEntity> _cisAddtype;
        private List<CisSubscriptionTypeEntity> _cisSubType;
        private List<CisCustomerPhoneEntity> _cisCusPhone;
        private List<CisCustomerEmailEntity> _cisCusEmail;
        private List<CisCountryEntity> _cisCountry;

        private IHpFacade _hpFacade;
        private List<HpActivityEntity> _hpActivity;

        #region "Constructor"
        public CSMFileService()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                // Set logfile name and application name variables
                GlobalContext.Properties["ApplicationCode"] = "CSMWS";
                GlobalContext.Properties["ServerName"] = Environment.MachineName;
                _logger = LogManager.GetLogger(typeof(CSMFileService));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occur:\n", ex);
            }
        }
        #endregion

        public ImportAFSTaskResponse GetFileAFS(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            string fiProp = string.Empty;
            string fiSaleZone = string.Empty;
            ImportAFSTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get GetFileAFS--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }

            try
            {
                _logger.Info("I:--START--:--Get GetFileAFS--");

                #region "AFS Settings"

                int numOfProp = 0;
                int numOfErrProp = 0;
                int numOfComplete = 0;
                int numOfSaleZones = 0;
                int numOfErrSaleZone = 0;

                _afsFacade = new AFSFacade();
                string afsProperty = WebConfig.GetAFSProperty();
                string afsSaleZone = WebConfig.GetAFSSaleZone();
                string afsPath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathImport);
                string afsExportPath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathError);

                bool isValidFile_SaleZone = true;
                string msgValidateFileError_SaleZone = "";

                bool isValidFile_Property = true;
                string msgValidateFileError_Property = "";

                #endregion

                _saleZones = _afsFacade.ReadFileSaleZone(afsPath, afsSaleZone, ref numOfSaleZones, ref fiSaleZone, ref isValidFile_SaleZone, ref msgValidateFileError_SaleZone);
                _properties = _afsFacade.ReadFileProperty(afsPath, afsProperty, ref numOfProp, ref fiProp, ref isValidFile_Property, ref msgValidateFileError_Property, isValidFile_SaleZone);

                if ((_saleZones == null && isValidFile_SaleZone == false) || (_properties == null && isValidFile_Property == false))
                {
                    _logDetail += (isValidFile_SaleZone == false) ? "[SaleZone]: " + msgValidateFileError_SaleZone : "";
                    _logDetail += (isValidFile_Property == false) ? " [Property]: " + msgValidateFileError_Property : "";

                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    goto Outer;
                }

                Task.Factory.StartNew(() => Parallel.ForEach(_properties, i =>
                {
                    lock (sync)
                    {
                        GetSaleZoneInfo(i);
                    }
                })).Wait();

                if (!_afsFacade.SaveSaleZones(_saleZones, fiSaleZone))
                {
                    _logger.Info("I:--FAILED--:--Save Sale Zones--");
                    _logDetail = "Failed save Sale Zones";
                    goto Outer;
                }

                if (_afsFacade.SaveAFSProperties(_properties))
                {
                    if (_afsFacade.SaveCompleteProperties(ref numOfComplete))
                    {
                        bool exportProp = _afsFacade.ExportErrorProperties(afsExportPath, fiProp, ref numOfErrProp);
                        if (!exportProp)
                        {
                            _logDetail = "Failed to export AFS Property";
                            goto Outer;
                        }

                        bool exportSaleZone = _afsFacade.ExportErrorSaleZones(afsExportPath, fiSaleZone, ref numOfErrSaleZone);
                        if (!exportSaleZone)
                        {
                            _logDetail = "Failed to export AFS Property";
                            goto Outer;
                        }

                        _logger.Info("I:--SUCCESS--:--Get GetFileAFS--");
                    }
                    else
                    {
                        _logger.Info("I:--FAILED--:--Cannot save AFS Asset--");
                        _logDetail = "Failed to save AFS Asset";
                        goto Outer;
                    }
                }
                else
                {
                    _logger.Info("I:--FAILED--:--Save AFS Property--");
                    _logDetail = "Failed to save AFS Property";
                    goto Outer;
                }

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportAFSTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    FileList = new List<object> { fiProp, fiSaleZone },
                    NumOfProp = numOfProp,
                    NumOfSaleZones = numOfSaleZones,
                    NumOfComplete = numOfComplete,
                    NumOfErrProp = numOfErrProp,
                    NumOfErrSaleZone = numOfErrSaleZone
                };

                _afsFacade.SaveLogSuccess(taskResponse);
                return taskResponse;
            }
            catch (Exception ex)
            {
                _logDetail = ex.Message;
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                _logger.Error("Exception occur:\n", ex);
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ImportAFSTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },
                FileList = new List<object>() { fiProp, fiSaleZone }
            };

            _afsFacade.SaveLogError(taskResponse);
            return taskResponse;
        }

        public ExportAFSTaskResponse ExportFileAFS(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            ExportAFSTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ExportFileAFS--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }
            try
            {
                _logger.Info("I:--START--:--Get ExportFileAFS--");

                #region "AFS Settings"
                int NumOfActivity = 0;

                _afsFacade = new AFSFacade();
                string afsactivities = WebConfig.GetActivityAFS();
                string afsexportpath = _afsFacade.GetParameter(Constants.ParameterName.AFSPathExport);
                #endregion

                bool exportAc = _afsFacade.ExportActivityAFS(afsexportpath, afsactivities, ref NumOfActivity);
                if (!exportAc)
                {
                    _logDetail = "Fail to export activity";
                    goto Outer;
                }

                _logger.Info("I:--SUCCESS--:--Get ExportFileAFS--");

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportAFSTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success,
                        ErrorCode = string.Empty
                    },
                    NumOfActivity = NumOfActivity
                };

                _afsFacade.SaveLogExportSuccess(taskResponse);
                return taskResponse;
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);
                _logDetail = "Fail to export activity";
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ExportAFSTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },

            };

            _afsFacade.SaveLogExportError(taskResponse);
            return taskResponse;
        }

        public ExportNCBTaskResponse ExportFileNCB(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            ExportNCBTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get ExportFileNCB--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }
            try
            {
                _logger.Info("I:--START--:--Get ExportFileNCB--");

                #region "AFS Settings"

                int numOfNew = 0;
                int numOfUpdate = 0;
                string newEmplFile = string.Empty;
                string updateEmplFile = string.Empty;

                _afsFacade = new AFSFacade();
                string afsemployeesnew = WebConfig.GetNewEmployeeNCBAFS();
                string afsemployeesupdate = WebConfig.GetUpdateEmployeeNCBAFS();
                string afsexportpath = _afsFacade.GetParameter(Constants.ParameterName.CICPathExport);

                #endregion

                bool exportEmNew = _afsFacade.ExportEmployeeNCBNew(afsexportpath, afsemployeesnew, ref numOfNew, ref newEmplFile);
                bool exportEmUpdate = _afsFacade.ExportEmplyeeNCBUpdate(afsexportpath, afsemployeesupdate, ref numOfUpdate, ref updateEmplFile);

                if (!exportEmNew && !exportEmUpdate)
                {
                    _logDetail = "Fail to export marketing";
                    goto Outer;
                }

                if (!_afsFacade.UploadFilesViaFTP(newEmplFile, updateEmplFile))
                {
                    _logDetail = "Fail to upload file via SFTP";
                    goto Outer;
                }

                _logger.Info("I:--SUCCESS--:--Get ExportFileNCB--");

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ExportNCBTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success,
                        ErrorCode = string.Empty
                    },
                    NumOfNew = numOfNew,
                    NumOfUpdate = numOfUpdate
                };

                _afsFacade.SaveLogExportMarketingSuccess(taskResponse);
                return taskResponse;
            }
            catch (Exception ex)
            {
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", ex.Message);
                _logger.Error("Exception occur:\n", ex);
                _logDetail = "Fail to export marketing";
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ExportNCBTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },
            };

            _afsFacade.SaveLogExportMarketingError(taskResponse);
            return taskResponse;
        }

        public ImportBDWTaskResponse GetFileBDW(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            string fiBdwContact = string.Empty;
            ImportBDWTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get GetFileBDW--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }

            int numOfBdwContact = 0;
            int numOfError = 0;
            int numOfComplete = 0;
            string messageError = string.Empty;

            try
            {
                _logger.Info("I:--START--:--Get GetFileBDW--");

                #region "BDW Contact Settings"

                _bdwFacade = new BdwFacade();
                string bdwContactFilePrefix = WebConfig.GetBDWContactPrefix();
                string bdwPath = _bdwFacade.GetParameter(Constants.ParameterName.BDWPathImport);
                string bdwExportPath = _bdwFacade.GetParameter(Constants.ParameterName.BDWPathError);

                bool isValidFile = true;
                string msgValidateFileError = "";

                #endregion

                if (!_bdwFacade.DownloadFilesViaFTP(bdwPath, bdwContactFilePrefix))
                {
                    _logDetail = "Cannot download files from SFTP";
                    goto Outer;
                }

                _bdwContacts = _bdwFacade.ReadFileBdwContact(bdwPath, bdwContactFilePrefix, ref numOfBdwContact, ref fiBdwContact, ref isValidFile, ref msgValidateFileError);

                if (_bdwContacts == null && isValidFile == false)
                {
                    _logDetail = msgValidateFileError; //string.Format(" File name : {0}  is invalid file format.", fiBdwContact);
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    goto Outer;
                }

                if (_bdwFacade.SaveBdwContact(_bdwContacts))
                {
                    if (_bdwFacade.SaveCompleteBdwContact(ref numOfComplete, ref numOfError, ref messageError))
                    {
                        if (numOfError > 0)
                        {
                            bool exportError = _bdwFacade.ExportErrorBdwContact(bdwExportPath, fiBdwContact, ref numOfError);
                            if (!exportError)
                            {
                                _logDetail = "Failed to export BDW Contact";
                                goto Outer;
                            }
                        }

                        _logger.Info("I:--SUCCESS--:--Get GetFileBDW--");
                    }
                    else
                    {
                        _logger.Info("I:--FAILED--:--Cannot save BDW Contact --");
                        _logDetail = "Failed to save BDW Contact";
                        goto Outer;
                    }
                }
                else
                {
                    _logger.Info("I:--FAILED--:--Save BDW Contact--");
                    _logDetail = "Failed to save BDW Contact";
                    goto Outer;
                }

                if (!_bdwFacade.DeleteFilesViaFTP(bdwContactFilePrefix))
                {
                    _logDetail = "Cannot delete files from SFTP";
                    goto Outer;
                }

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportBDWTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    FileList = new List<object> { fiBdwContact },
                    NumOfBdwContact = numOfBdwContact,
                    NumOfComplete = numOfComplete,
                    NumOfError = numOfError
                };

                _bdwFacade.SaveLogSuccess(taskResponse);
                return taskResponse;
            }
            catch (Exception ex)
            {
                _logDetail = ex.Message;
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                _logger.Error("Exception occur:\n", ex);
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ImportBDWTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },
                FileList = new List<object>() { fiBdwContact }
            };

            _bdwFacade.SaveLogError(taskResponse);
            return taskResponse;
        }

        public ImportCISTaskResponse GetFileCIS(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            string fiCor = string.Empty;
            string fiIndiv = string.Empty;
            string fiProd = string.Empty;
            string fiSub = string.Empty;
            string fiTitle = string.Empty;
            string fiPro = string.Empty;
            string fiDis = string.Empty;
            string fiSubDis = string.Empty;
            string fiPhonetype = string.Empty;
            string fiEmailtype = string.Empty;
            string fiSubAdd = string.Empty;
            string fiSubPhone = string.Empty;
            string fiSubMail = string.Empty;
            string fiAddtype = string.Empty;
            string fiSubType = string.Empty;
            string fiCusPhone = string.Empty;
            string fiCusEmail = string.Empty;
            string fiCountry = string.Empty;
            List<object> FileErrorList = null;

            string msgValidateFileTitleError = "";
            string msgValidateFileCorporateError = "";
            string msgValidateFileIndividualError = "";
            string msgValidateFileProductGroupError = "";
            string msgValidateFileSubscriptionError = "";
            string msgValidateFileProvinceError = "";
            string msgValidateFileDistrictError = "";
            string msgValidateFileSubDistrictError = "";
            string msgValidateFilePhoneTypeError = "";
            string msgValidateFileEmailTypeError = "";
            string msgValidateFileAddressTypeError = "";
            string msgValidateFileSubscribeAddressError = "";
            string msgValidateFileSubscribePhoneError = "";
            string msgValidateFileSubscribeMailError = "";
            string msgValidateFileSubscriptionTypeError = "";
            string msgValidateFileCustomerPhoneError = "";
            string msgValidateFileCustomerEmailError = "";
            string msgValidateFileCountryError = "";

            ImportCISTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get GetFileCIS--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }

            try
            {
                _logger.Info("I:--START--:--Get GetFileCIS--");

                #region "CIS Settings"

                // Corperate
                int numOfCor = 0;
                int numOfErrCor = 0;
                int numOfCorComplete = 0;

                // Individual
                int numOfIndiv = 0;
                int numOfErrIndiv = 0;
                int numOfIndivComplete = 0;

                // Subscription
                int numOfSub = 0;
                int numOfSubComplete = 0;
                int numOfErrSub = 0;

                // SubscribeAddress
                int numOfSubAdd = 0;
                int numOfAddressComplete = 0;
                int numOfErrAddress = 0;

                // SubscribePhone
                int numOfSubPhone = 0;
                int numOfPhoneComplete = 0;
                int numOfErrPhone = 0;

                // SubscribeMail
                int numOfSubMail = 0;
                int numOfEmailComplete = 0;
                int numOfErrEmail = 0;

                // SubscriptionType
                int numOfSubType = 0;
                int numOfSubTypeComplete = 0;
                int numOfSubTypeError = 0;

                // CusPhone
                int numOfCusPhone = 0;
                int numOfCusPhoneComplete = 0;
                int numOfCusPhoneError = 0;

                // CusEmail
                int numOfCusEmail = 0;
                int numOfCusEmailComplete = 0;
                int numOfCusEmailError = 0;

                // ProductGroup
                int numOfProd = 0;
                int numOfProdComplete = 0;
                int numOfProdError = 0;

                // Title
                int numOfTitle = 0;
                int numOfTitleComplete = 0;
                int numOfTitleError = 0;

                // Province
                int numOfPro = 0;
                int numOfProvinceComplete = 0;
                int numOfProvinceError = 0;

                // District
                int numOfDis = 0;
                int numOfDistrictComplete = 0;
                int numOfDistrictError = 0;

                // SubDistrict
                int numOfSubDis = 0;
                int numOfSubDistrictComplete = 0;
                int numOfSubDistrictError = 0;

                // AddressType
                int numOfAddtype = 0;
                int numOfAddtypeComplete = 0;
                int numOfAddtypeError = 0;

                // PhoneType
                int numOfPhonetype = 0;
                int numOfPhonetypeComplete = 0;
                int numOfPhonetypeError = 0;

                // Emailtype
                int numOfEmailtype = 0;
                int numOfEmailtypeComplete = 0;
                int numOfEmailtypeError = 0;

                // Country
                int numOfCountry = 0;
                int numOfCountryComplete = 0;
                int numOfCountryError = 0;


                string messageError = string.Empty;

                bool isValidHeaderTitle = true;
                bool isValidHeaderCorporate = true;
                bool isValidHeaderIndividual = true;
                bool isValidHeaderCisProductGroup = true;
                bool isValidHeaderCisSubscription = true;
                bool isValidHeaderCisProvince = true;
                bool isValidHeaderCisDistrict = true;
                bool isValidHeaderCisSubDistrict = true;
                bool isValidHeaderCisPhoneType = true;
                bool isValidHeaderCisEmailType = true;
                bool isValidHeaderCisSubscibeAddress = true;
                bool isValidHeaderCisSubscribePhone = true;
                bool isValidHeaderCisSubscribeMail = true;
                bool isValidHeaderCisAddressType = true;
                bool isValidHeaderCisSubscriptionType = true;
                bool isValidHeaderCisCustomerPhone = true;
                bool isValidHeaderCisCustomerEmail = true;
                bool isValidHeaderCisCountry = true;

                // SaveSuccess
                bool saveCorporate = false;
                bool saveIndividual = false;
                bool saveSubscribeAddress = false;
                bool saveSubscribePhone = false;
                bool saveSubscribeEmail = false;
                bool saveCustomerPhone = false;
                bool saveCustomerEmail = false;
                bool saveSubscription = false;
                bool saveSubscriptionType = false;

                _cisFacade = new CisFacade();
                string cisPathImport = _cisFacade.GetParameter(Constants.ParameterName.CISPathImport);
                string cisPathError = _cisFacade.GetParameter(Constants.ParameterName.CISPathError);

                string CisCorporate = WebConfig.GetCisCorprate();
                string CisIndividual = WebConfig.GetIndividual();
                string CisProductGroup = WebConfig.GetProductGroup();
                string CisSubscription = WebConfig.GetSubscription();
                string CisTitle = WebConfig.GetTitle();
                string CisProvince = WebConfig.GetProvince();
                string CisDistrict = WebConfig.GetDistrict();
                string CisSubDistrict = WebConfig.GetSubDistrict();
                string CisPhonetype = WebConfig.GetPhonetype();
                string CisEmailtype = WebConfig.GetMailtype();
                string CisSubmail = WebConfig.GetSubmail();
                string CisSubphone = WebConfig.GetSubphone();
                string CisSubaddress = WebConfig.GetSubaddress();
                string CisAddresstype = WebConfig.GetAddresstype();
                string CisSubType = WebConfig.GetSubScriptionType();
                string CisCusPhone = WebConfig.GetCustomerPhone();
                string CisCusEmail = WebConfig.GetCustomerEmail();
                string CisCountry = WebConfig.GetCountry();

                #endregion

                #region "Delete all temp table [Interface table]"

                _cisFacade.DeleteAllCisTableInterface();

                #endregion

                #region "Read File"

                _cisTitle = _cisFacade.ReadFileCisTitle(cisPathImport, CisTitle, ref numOfTitle, ref fiTitle, ref isValidHeaderTitle, ref msgValidateFileTitleError);

                _cisCor = _cisFacade.ReadFileCisCorporate(cisPathImport, CisCorporate, ref numOfCor, ref fiCor, ref isValidHeaderCorporate, ref msgValidateFileCorporateError);
                _cisIndiv = _cisFacade.ReadFileCisIndividual(cisPathImport, CisIndividual, ref numOfIndiv, ref fiIndiv, ref isValidHeaderIndividual, ref msgValidateFileIndividualError);

                _cisProd = _cisFacade.ReadFileCisProductGroup(cisPathImport, CisProductGroup, ref numOfProd, ref fiProd, ref isValidHeaderCisProductGroup, ref msgValidateFileProductGroupError);
                _cisSub = _cisFacade.ReadFileCisSubscription(cisPathImport, CisSubscription, ref numOfSub, ref fiSub, ref isValidHeaderCisSubscription, ref msgValidateFileSubscriptionError);
                _cisProvince = _cisFacade.ReadFileCisProvince(cisPathImport, CisProvince, ref numOfPro, ref fiPro, ref isValidHeaderCisProvince, ref msgValidateFileProvinceError);
                _cisDistrict = _cisFacade.ReadFileCisDistrict(cisPathImport, CisDistrict, ref numOfDis, ref fiDis, ref isValidHeaderCisDistrict, ref msgValidateFileDistrictError);
                _cisSubDistrict = _cisFacade.ReadFileCisSubDistrict(cisPathImport, CisSubDistrict, ref numOfSubDis, ref fiSubDis, ref isValidHeaderCisSubDistrict, ref msgValidateFileSubDistrictError);
                _cisPhonetype = _cisFacade.ReadFileCisPhoneType(cisPathImport, CisPhonetype, ref numOfPhonetype, ref fiPhonetype, ref isValidHeaderCisPhoneType, ref msgValidateFilePhoneTypeError);
                _cisEmailtype = _cisFacade.ReadFileCisEmailType(cisPathImport, CisEmailtype, ref numOfEmailtype, ref fiEmailtype, ref isValidHeaderCisEmailType, ref msgValidateFileEmailTypeError);
                _cisSubaddress = _cisFacade.ReadFileCisSubscribeAddress(cisPathImport, CisSubaddress, ref numOfSubAdd, ref fiSubAdd, ref isValidHeaderCisSubscibeAddress, ref msgValidateFileSubscribeAddressError);
                _cisSubphone = _cisFacade.ReadFileCisSubscribePhone(cisPathImport, CisSubphone, ref numOfSubPhone, ref fiSubPhone, ref isValidHeaderCisSubscribePhone, ref msgValidateFileSubscribePhoneError);
                _cisSubemail = _cisFacade.ReadFileCisSubscribeMail(cisPathImport, CisSubmail, ref numOfSubMail, ref fiSubMail, ref isValidHeaderCisSubscribeMail, ref msgValidateFileSubscribeMailError);
                _cisAddtype = _cisFacade.ReadFileCisAddressType(cisPathImport, CisAddresstype, ref numOfAddtype, ref fiAddtype, ref isValidHeaderCisAddressType, ref msgValidateFileAddressTypeError);
                _cisSubType = _cisFacade.ReadFileCisSubscriptionType(cisPathImport, CisSubType, ref numOfSubType, ref fiSubType, ref isValidHeaderCisSubscriptionType, ref msgValidateFileSubscriptionTypeError);
                _cisCusPhone = _cisFacade.ReadFileCisCustomerPhone(cisPathImport, CisCusPhone, ref numOfCusPhone, ref fiCusPhone, ref isValidHeaderCisCustomerPhone, ref msgValidateFileCustomerPhoneError);
                _cisCusEmail = _cisFacade.ReadFileCisCustomerEmail(cisPathImport, CisCusEmail, ref numOfCusEmail, ref fiCusEmail, ref isValidHeaderCisCustomerEmail, ref msgValidateFileCustomerEmailError);
                _cisCountry = _cisFacade.ReadFileCisCountry(cisPathImport, CisCountry, ref numOfCountry, ref fiCountry, ref isValidHeaderCisCountry, ref msgValidateFileCountryError);

                #endregion

                FileErrorList = new List<object>();

                #region "Save to Table Interface"

                if (_cisTitle == null && isValidHeaderTitle == false)
                {
                    _logDetail = msgValidateFileTitleError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiTitle))
                    {
                        FileErrorList.Add(fiTitle);
                    }
                }
                else
                {
                    var saveTitle = _cisFacade.SaveCisTitle(_cisTitle, fiTitle);
                    if (saveTitle) { numOfTitleComplete = numOfTitle; }
                    else { numOfTitleError = numOfTitle; }
                }

                if (_cisCor == null && isValidHeaderCorporate == false)
                {
                    _logDetail = msgValidateFileCorporateError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiCor))
                    {
                        FileErrorList.Add(fiCor);
                    }
                }
                else
                {
                    saveCorporate = _cisFacade.SaveCisCorporate(_cisCor, fiCor);
                }

                if (_cisIndiv == null && isValidHeaderIndividual == false)
                {
                    _logDetail = msgValidateFileIndividualError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiIndiv))
                    {
                        FileErrorList.Add(fiIndiv);
                    }
                }
                else
                {
                    saveIndividual = _cisFacade.SaveCisIndividual(_cisIndiv, fiIndiv);
                }

                if (_cisProd == null && isValidHeaderCisProductGroup == false)
                {
                    _logDetail = msgValidateFileProductGroupError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiProd))
                    {
                        FileErrorList.Add(fiProd);
                    }
                }
                else
                {
                    var saveProductGroup = _cisFacade.SaveCisProductGroup(_cisProd, fiProd);
                    if (saveProductGroup) { numOfProdComplete = numOfProd; }
                    else { numOfProdError = numOfProd; }
                }

                if (_cisSub == null && isValidHeaderCisSubscription == false)
                {
                    _logDetail = msgValidateFileSubscriptionError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSub))
                    {
                        FileErrorList.Add(fiSub);
                    }
                }
                else
                {
                    saveSubscription = _cisFacade.SaveCisSubscription(_cisSub, fiSub);
                }

                // Country
                if (_cisCountry == null && isValidHeaderCisCountry == false)
                {
                    _logDetail = msgValidateFileCountryError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiCountry))
                    {
                        FileErrorList.Add(fiCountry);
                    }
                }
                else
                {
                    if (_cisFacade.SaveCisCountry(_cisCountry, fiCountry))
                    { numOfCountryComplete = numOfCountry; }
                    else { numOfCountryError = numOfCountry; }
                }

                // Province
                if (_cisProvince == null && isValidHeaderCisProvince == false)
                {
                    _logDetail = msgValidateFileProvinceError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiPro))
                    {
                        FileErrorList.Add(fiPro);
                    }
                }
                else
                {
                    var saveProvince = _cisFacade.SaveCisProvince(_cisProvince, fiPro);
                    if (saveProvince) { numOfProvinceComplete = numOfPro; }
                    else { numOfProvinceError = numOfPro; }
                }

                if (_cisDistrict == null && isValidHeaderCisDistrict == false)
                {
                    _logDetail = msgValidateFileDistrictError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiDis))
                    {
                        FileErrorList.Add(fiDis);
                    }
                }
                else
                {
                    var saveDistrict = _cisFacade.SaveCisDistrict(_cisDistrict, fiDis);
                    if (saveDistrict) { numOfDistrictComplete = numOfDis; }
                    else { numOfDistrictError = numOfDis; }
                }

                if (_cisSubDistrict == null && isValidHeaderCisSubDistrict == false)
                {
                    _logDetail = msgValidateFileSubDistrictError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSubDis))
                    {
                        FileErrorList.Add(fiSubDis);
                    }
                }
                else
                {
                    var saveSubDistrict = _cisFacade.SaveCisSubDistrict(_cisSubDistrict, fiSubDis);
                    if (saveSubDistrict) { numOfSubDistrictComplete = numOfSubDis; }
                    else { numOfSubDistrictError = numOfSubDis; }
                }

                if (_cisAddtype == null && isValidHeaderCisAddressType == false)
                {
                    _logDetail = msgValidateFileAddressTypeError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiAddtype))
                    {
                        FileErrorList.Add(fiAddtype);
                    }
                }
                else
                {
                    var savAddresstype = _cisFacade.SaveCisAddressType(_cisAddtype, fiAddtype);
                    if (savAddresstype) { numOfAddtypeComplete = numOfAddtype; }
                    else { numOfAddtypeError = numOfAddtype; }
                }

                if (_cisPhonetype == null && isValidHeaderCisPhoneType == false)
                {
                    _logDetail = msgValidateFilePhoneTypeError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiPhonetype))
                    {
                        FileErrorList.Add(fiPhonetype);
                    }
                }
                else
                {
                    var savePhonetype = _cisFacade.SaveCisPhoneType(_cisPhonetype, fiPhonetype);
                    if (savePhonetype) { numOfPhonetypeComplete = numOfPhonetype; }
                    else { numOfPhonetypeError = numOfPhonetype; }
                }

                if (_cisEmailtype == null && isValidHeaderCisEmailType == false)
                {
                    _logDetail = msgValidateFileEmailTypeError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiEmailtype))
                    {
                        FileErrorList.Add(fiEmailtype);
                    }
                }
                else
                {
                    var saveEmailtype = _cisFacade.SaveCisEmailType(_cisEmailtype, fiEmailtype);
                    if (saveEmailtype) { numOfEmailtypeComplete = numOfEmailtype; }
                    else { numOfEmailtypeError = numOfEmailtype; }
                }

                if (_cisSubaddress == null && isValidHeaderCisSubscibeAddress == false)
                {
                    _logDetail = msgValidateFileSubscribeAddressError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSubAdd))
                    {
                        FileErrorList.Add(fiSubAdd);
                    }
                }
                else
                {
                    saveSubscribeAddress = _cisFacade.SaveCisSubscribeAddress(_cisSubaddress, fiSubAdd);
                }

                if (_cisSubphone == null && isValidHeaderCisSubscribePhone == false)
                {
                    _logDetail = msgValidateFileSubscribePhoneError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSubPhone))
                    {
                        FileErrorList.Add(fiSubPhone);
                    }
                }
                else
                {
                    saveSubscribePhone = _cisFacade.SaveCisSubscribePhone(_cisSubphone, fiSubPhone);
                }

                if (_cisSubemail == null && isValidHeaderCisSubscribeMail == false)
                {
                    _logDetail = msgValidateFileSubscribeMailError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSubMail))
                    {
                        FileErrorList.Add(fiSubMail);
                    }
                }
                else
                {
                    saveSubscribeEmail = _cisFacade.SaveCisSubscribeEmail(_cisSubemail, fiSubMail);
                }

                if (_cisSubType == null && isValidHeaderCisSubscriptionType == false)
                {
                    _logDetail = msgValidateFileSubscriptionTypeError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiSubType))
                    {
                        FileErrorList.Add(fiSubType);
                    }
                }
                else
                {
                    saveSubscriptionType = _cisFacade.SaveCisSubscriptionType(_cisSubType, fiSubType);
                }

                if (_cisCusPhone == null && isValidHeaderCisCustomerPhone == false)
                {
                    _logDetail = msgValidateFileCustomerPhoneError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiCusPhone))
                    {
                        FileErrorList.Add(fiCusPhone);
                    }
                }
                else
                {
                    saveCustomerPhone = _cisFacade.SaveCisCustomerPhone(_cisCusPhone, fiCusPhone);
                }

                if (_cisCusEmail == null && isValidHeaderCisCustomerEmail == false)
                {
                    _logDetail = msgValidateFileCustomerEmailError;
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    if (!string.IsNullOrEmpty(fiCusPhone))
                    {
                        FileErrorList.Add(fiCusPhone);
                    }
                }
                else
                {
                    saveCustomerEmail = _cisFacade.SaveCisCustomerEmail(_cisCusEmail, fiCusEmail);
                }

                #endregion

                #region "comment out"
                //if ((!saveCorporate) && (!saveIndividual))               
                //{
                //    _logger.Info("I:--FAILED--:--Save CIS Corporate & Save CIS Individual--");
                //    _logDetail = "Failed Save CIS Corporate & Save CIS Individual";
                //    goto Outer;
                //}
                #endregion

                #region "Save to Table Master&Transaction"

                #region "[SubscriptionType]"

                if (saveSubscriptionType)
                {
                    var saveSubscriptionTypeComplete = _cisFacade.SaveCisSubscriptionTypeComplete(ref numOfSubTypeComplete, ref numOfSubTypeError, ref messageError);
                    if (saveSubscriptionTypeComplete)
                    {
                        if (numOfSubTypeError > 0)
                        {
                            bool exportSubType = _cisFacade.ExportSubscriptionTypeCIS(cisPathError, CisSubType);
                            if (!exportSubType)
                            {
                                _logDetail = "Failed to export CIS SubscriptionType";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                #region "[Corporate]"

                if (saveCorporate)
                {
                    var saveCorporateComplete = _cisFacade.SaveCisCorporateComplete(ref numOfCorComplete, ref numOfErrCor, ref messageError);
                    if (saveCorporateComplete)
                    {
                        if (numOfErrCor > 0)
                        {
                            bool exportCorp = _cisFacade.ExportCorporateCIS(cisPathError, CisCorporate);
                            if (!exportCorp)
                            {
                                _logDetail = "Failed to export CIS Corporate";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                #region "[Individual]"

                if (saveIndividual)
                {
                    var saveIndividualComplete = _cisFacade.SaveCisIndividualComplete(ref numOfIndivComplete, ref numOfErrIndiv, ref messageError);
                    if (saveIndividualComplete)
                    {
                        if (numOfErrIndiv > 0)
                        {
                            bool exportIndiv = _cisFacade.ExportIndividualCIS(cisPathError, CisIndividual);
                            if (!exportIndiv)
                            {
                                _logDetail = "Failed to export CIS Individual";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                #region "[SubscribeAddress]"

                if (saveSubscribeAddress)
                {
                    _cisFacade.SaveCisSubscribeAddressComplete(ref numOfAddressComplete, ref numOfErrAddress, ref messageError);
                    if (numOfErrAddress > 0)
                    {
                        bool exportAddress = _cisFacade.ExportSubscribeAddressCIS(cisPathError, CisSubaddress);
                        if (!exportAddress)
                        {
                            _logDetail = "Failed to export CIS Subscribe Address";
                            goto Outer;
                        }
                    }
                }

                #endregion

                #region "[SubscribePhone]"

                if (saveSubscribePhone)
                {
                    _cisFacade.SaveCisSubscribePhoneComplete(ref numOfPhoneComplete, ref numOfErrPhone, ref messageError);
                    if (numOfErrPhone > 0)
                    {
                        bool exportPhone = _cisFacade.ExportSubscribePhoneCIS(cisPathError, CisSubphone);
                        if (!exportPhone)
                        {
                            _logDetail = "Failed to export CIS Subscribe Phone";
                            goto Outer;
                        }
                    }
                }

                #endregion

                #region "[SubscribeEmail]"

                if (saveSubscribeEmail)
                {
                    _cisFacade.SaveCisSubscribeEmailComplete(ref numOfEmailComplete, ref numOfErrEmail, ref messageError);
                    if (numOfErrEmail > 0)
                    {
                        bool exportEmail = _cisFacade.ExportSubscribeEmailCIS(cisPathError, CisSubmail);
                        if (!exportEmail)
                        {
                            _logDetail = "Failed to export CIS Subscribe Mail";
                            goto Outer;
                        }
                    }
                }

                #endregion

                #region "[CustomerPhone]"

                if (saveCustomerPhone)
                {
                    var saveCustomerPhoneComplet = _cisFacade.SaveCisCustomerPhoneComplete(ref numOfCusPhoneComplete, ref numOfCusPhoneError, ref messageError);
                    if (saveCustomerPhoneComplet)
                    {
                        if (numOfCusPhoneError > 0)
                        {
                            bool exportCusphone = _cisFacade.ExportCustomerPhoneCIS(cisPathError, CisCusPhone);
                            if (!exportCusphone)
                            {
                                _logDetail = "Failed to export CIS CustomerPhone";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                #region "[CustomerEmail]"

                if (saveCustomerEmail)
                {
                    var saveCustomerEmailComplete = _cisFacade.SaveCisCustomerEmailComplete(ref numOfCusEmailComplete, ref numOfCusEmailError, ref messageError);
                    if (saveCustomerEmailComplete)
                    {
                        if (numOfCusEmailError > 0)
                        {
                            bool exportCusEmail = _cisFacade.ExportCustomerEmailCIS(cisPathError, CisCusEmail);
                            if (!exportCusEmail)
                            {
                                _logDetail = "Failed to export CIS CustomerEmail";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                #region "[Subscription]"

                if (saveSubscription)
                {
                    if (_cisFacade.SaveCisSubscriptionComplete(ref numOfSubComplete, ref numOfErrSub, ref messageError))
                    {
                        if (numOfErrSub > 0)
                        {
                            bool exportSub = _cisFacade.ExportSubscriptionCIS(cisPathError, CisSubscription);
                            if (!exportSub)
                            {
                                _logDetail = "Failed to export CIS Subscription";
                                goto Outer;
                            }
                        }
                    }
                }

                #endregion

                _logger.Info("I:--SUCCESS--:--Get GetFileCIS--");

                #endregion

                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportCISTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    FileList = new List<object> { fiTitle, fiCor, fiIndiv, fiProd, fiSub, fiCountry, fiPro, fiDis, fiSubDis, fiPhonetype, fiEmailtype, fiAddtype, fiSubAdd, fiSubPhone, fiSubMail, fiSubType, fiCusPhone, fiCusEmail },
                    //FileErrorList = FileErrorList,
                    FileErrorList = new List<object>{msgValidateFileTitleError
                                    , msgValidateFileCorporateError
                                    , msgValidateFileIndividualError
                                    , msgValidateFileProductGroupError
                                    , msgValidateFileSubscriptionError
                                    , msgValidateFileCountryError
                                    , msgValidateFileProvinceError
                                    , msgValidateFileDistrictError
                                    , msgValidateFileSubDistrictError
                                    , msgValidateFilePhoneTypeError
                                    , msgValidateFileEmailTypeError
                                    , msgValidateFileAddressTypeError
                                    , msgValidateFileSubscribeAddressError
                                    , msgValidateFileSubscribePhoneError
                                    , msgValidateFileSubscribeMailError
                                    , msgValidateFileSubscriptionTypeError
                                    , msgValidateFileCustomerPhoneError
                                    , msgValidateFileCustomerEmailError},


                    // Corperate
                    NumOfCor = numOfCor,
                    NumOfErrCor = numOfErrCor,
                    NumOfCorComplete = numOfCorComplete,
                    // Individual
                    NumOfIndiv = numOfIndiv,
                    NumOfErrIndiv = numOfErrIndiv,
                    NumOfIndivComplete = numOfIndivComplete,
                    // Subscription       
                    NumOfSub = numOfSub,
                    NumOfSubComplete = numOfSubComplete,
                    NumOfErrSub = numOfErrSub,
                    // SubscribeAddress        
                    NumOfSubAdd = numOfSubAdd,
                    NumOfAddressComplete = numOfAddressComplete,
                    numOfErrAddress = numOfErrAddress,
                    // SubscribePhone        
                    NumOfSubPhone = numOfSubPhone,
                    NumOfPhoneComplete = numOfPhoneComplete,
                    NumOfErrPhone = numOfErrPhone,
                    // SubscribeMail        
                    NumOfSubMail = numOfSubMail,
                    NumOfEmailComplete = numOfEmailComplete,
                    NumOfErrEmail = numOfErrEmail,
                    // SubscriptionType
                    NumOfSubType = numOfSubType,
                    NumOfSubTypeComplete = numOfSubTypeComplete,
                    NumOfSubTypeError = numOfSubTypeError,
                    // CusPhone 
                    NumOfCusPhone = numOfCusPhone,
                    NumOfCusPhoneComplete = numOfCusPhoneComplete,
                    NumOfCusPhoneError = numOfCusPhoneError,
                    // CusEmail   
                    NumOfCusEmail = numOfCusEmail,
                    NumOfCusEmailComplete = numOfCusEmailComplete,
                    NumOfCusEmailError = numOfCusEmailError,
                    // ProductGroup

                    NumOfProd = numOfProd,
                    NumOfProdComplete = numOfProdComplete,
                    NumOfProdError = numOfProdError,
                    // Title        
                    NumOfTitle = numOfTitle,
                    NumOfTitleComplete = numOfTitleComplete,
                    NumOfTitleError = numOfTitleError,
                    // Country         
                    NumOfCountry = numOfCountry,
                    NumOfCountryComplete = numOfCountryComplete,
                    NumOfCountryError = numOfCountryError,
                    // Province        
                    NumOfPro = numOfPro,
                    NumOfProvinceComplete = numOfProvinceComplete,
                    NumOfProvinceError = numOfProvinceError,
                    // District        
                    NumOfDis = numOfDis,
                    NumOfDistrictComplete = numOfDistrictComplete,
                    NumOfDistrictError = numOfDistrictError,
                    // SubDistrict        
                    NumOfSubDis = numOfSubDis,
                    NumOfSubDistrictComplete = numOfSubDistrictComplete,
                    NumOfSubDistrictError = numOfSubDistrictError,
                    // AddressType        
                    NumOfAddressType = numOfAddtype,
                    NumOfAddressTypeComplete = numOfAddtypeComplete,
                    NumOfAddressTypeError = numOfAddtypeError,
                    // PhoneType        
                    NumOfPhonetype = numOfPhonetype,
                    NumOfPhonetypeComplete = numOfPhonetypeComplete,
                    NumOfPhonetypeError = numOfPhonetypeError,
                    // Emailtype        
                    NumOfEmailtype = numOfEmailtype,
                    NumOfEmailtypeComplete = numOfEmailtypeComplete,
                    NumOfEmailtypeError = numOfEmailtypeError
                };

                _cisFacade.SaveLogSuccess(taskResponse);
                return taskResponse;
            }
            catch (Exception ex)
            {
                _logDetail = ex.Message;
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                _logger.Error("Exception occur:\n", ex);
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ImportCISTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },
                FileList = new List<object> { fiTitle, fiCor, fiIndiv, fiProd, fiSub, fiCountry, fiPro, fiDis, fiSubDis, fiPhonetype, fiEmailtype, fiSubAdd, fiSubPhone, fiSubMail, fiAddtype, fiSubType, fiCusPhone, fiCusEmail },
                FileErrorList = FileErrorList
            };

            _cisFacade.SaveLogError(taskResponse);
            return taskResponse;
        }

        public ImportHpTaskResponse GetFileHP(string username, string password)
        {
            ThreadContext.Properties["EventClass"] = ApplicationHelpers.GetCurrentMethod(1);
            ThreadContext.Properties["RemoteAddress"] = ApplicationHelpers.GetClientIP();
            ThreadContext.Properties["UserID"] = "CSM_SCHEDTASK";

            string fiHpActivity = string.Empty;
            ImportHpTaskResponse taskResponse;
            DateTime schedDateTime = DateTime.Now;

            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Debug("-- Start Cron Job --:--Get GetFileHP--");

            if (!ApplicationHelpers.Authenticate(username, password))
            {
                _logger.DebugFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
            }

            int NumOfTotal = 0;
            int numOfError = 0;
            int numOfComplete = 0;
            string messageError = "";

            try
            {
                _logger.Info("I:--START--:--Get GetFileHP--");

                #region "Hp Settings"
                _hpFacade = new HpFacade();

                string hpActivityFilePrefix = WebConfig.GetHpActivity();
                string hpImportPath = _hpFacade.GetParameter(Constants.ParameterName.HPPathImport);
                string hpExportPath = _hpFacade.GetParameter(Constants.ParameterName.HPPathError);

                bool isValidFile = true;
                string msgValidateFileError = "";

                #endregion

                _hpActivity = _hpFacade.ReadFileHpActivity(hpImportPath, hpActivityFilePrefix, ref NumOfTotal, ref fiHpActivity, ref isValidFile, ref msgValidateFileError);

                if (_hpActivity == null && isValidFile == false)
                {
                    _logDetail = msgValidateFileError; //string.Format(" File name : {0}  is invalid file format.", fiHpActivity);
                    _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                    goto Outer;
                }


                #region "pass basic validate "

                if (_hpFacade.SaveHpActivity(_hpActivity, fiHpActivity))
                {

                    if (_hpFacade.SaveHpActivityComplete(ref numOfComplete, ref numOfError, ref messageError))
                    {
                        _hpFacade.SaveServiceRequestActivity(); //  module Sr

                        if (numOfError > 0)
                        {
                            bool exportError = _hpFacade.ExportActivityHP(hpExportPath, fiHpActivity);
                            if (!exportError)
                            {
                                _logDetail = "Failed to export HP Activity";
                                goto Outer;
                            }
                        }

                        _logger.Info("I:--SUCCESS--:--Get GetFileHP--");
                    }
                    else
                    {
                        _logger.Info("I:--FAILED--:--Cannot save HP Activity --");
                        _logDetail = "Failed to save HP Activity";
                        goto Outer;
                    }



                }
                else
                {
                    _logger.Info("I:--FAILED--:--Cannot save HP Activity --");
                    _logDetail = "Failed to save HP Activity";
                    goto Outer;
                }


                _stopwatch.Stop();
                _elapsedTime = _stopwatch.ElapsedMilliseconds;
                _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

                taskResponse = new ImportHpTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = _elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Success
                    },
                    FileList = new List<object> { fiHpActivity },
                    NumOfTotal = NumOfTotal,
                    NumOfComplete = numOfComplete,
                    NumOfError = numOfError
                };

                _hpFacade.SaveLogSuccess(taskResponse);
                return taskResponse;

                #endregion

            }
            catch (Exception ex)
            {
                _logDetail = ex.Message;
                _logger.InfoFormat("O:--FAILED--:Error Message/{0}", _logDetail);
                _logger.Error("Exception occur:\n", ex);
            }

        Outer:
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            _logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + _elapsedTime);

            taskResponse = new ImportHpTaskResponse
            {
                SchedDateTime = schedDateTime,
                ElapsedTime = _elapsedTime,
                StatusResponse = new StatusResponse
                {
                    Status = Constants.StatusResponse.Failed,
                    ErrorCode = string.Empty,
                    Description = _logDetail
                },
                FileList = new List<object>() { fiHpActivity }
            };

            _hpFacade.SaveLogError(taskResponse);
            return taskResponse;
        }

        #region "Functions"

        private void GetSaleZoneInfo(PropertyEntity prop)
        {
            SaleZoneEntity saleZone = _saleZones.FirstOrDefault(x => x.District == prop.AssetLot && x.Province == prop.AssetPurch);
            if (saleZone != null)
            {
                prop.EmployeeId = saleZone.EmployeeId;
                prop.SaleName = saleZone.SaleName;
                prop.MobileNo = saleZone.MobileNo;
                prop.PhoneNo = saleZone.PhoneNo;
                prop.Email = saleZone.Email;
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
                    if (_afsFacade != null) { _afsFacade.Dispose(); }
                    if (_hpFacade != null) { _hpFacade.Dispose(); }
                    if (_bdwFacade != null) { _bdwFacade.Dispose(); }
                    if (_cisFacade != null) { _cisFacade.Dispose(); }
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
