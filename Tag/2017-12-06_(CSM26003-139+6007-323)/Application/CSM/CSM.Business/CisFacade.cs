using System.IO;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using CSM.Service.Messages.Common;

namespace CSM.Business
{
    public class CisFacade : ICisFacade
    {
        private CSMContext _context;
        private AuditLogEntity _auditLog;
        private CSMMailSender _mailSender;
        private ICommonFacade _commonFacade;
        private ICisDataAccess _cisDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CisFacade));

        public ImportCISTaskResponse GetFileCIS(string username, string password, bool skipSftp, string strDate)
        {
            string logDetail = string.Empty;
            ImportCISTaskResponse taskResponse = null;
            string processDetials = string.Empty;

            try
            {
                long elapsedTime = 0;
                DateTime schedDateTime = DateTime.Now;
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                Logger.Debug("-- Start Cron Job --:--Get GetFileCIS--");

                #region "BatchProcess Start"

                if (AppLog.BatchProcessStart(Constants.BatchProcessCode.ImportCIS, schedDateTime) == false)
                {
                    Logger.Info("I:--NOT PROCESS--:--GetFileCIS--");

                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                    Logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                    taskResponse = new ImportCISTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.NotProcess,
                            ErrorCode = string.Empty,
                            Description = "",
                        },

                    };

                    return taskResponse;
                }

                #endregion

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
                List<object> fileErrorList = null;

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

                #region "Declare Data Date"

                string dataDateOfCor = string.Empty;
                string dataDateOfIndiv = string.Empty;
                string dataDateOfProd = string.Empty;
                string dataDateOfSub = string.Empty;
                string dataDateOfTitle = string.Empty;
                string dataDateOfPro = string.Empty;
                string dataDateOfDis = string.Empty;
                string dataDateOfSubDis = string.Empty;
                string dataDateOfPhonetype = string.Empty;
                string dataDateOfEmailtype = string.Empty;
                string dataDateOfSubAdd = string.Empty;
                string dataDateOfSubPhone = string.Empty;
                string dataDateOfSubMail = string.Empty;
                string dataDateOfAddtype = string.Empty;
                string dataDateOfSubType = string.Empty;
                string dataDateOfCusPhone = string.Empty;
                string dataDateOfCusEmail = string.Empty;
                string dataDateOfCountry = string.Empty;

                #endregion

                try
                {
                    Logger.Info("I:--START--:--Get GetFileCIS--");

                    if (!ApplicationHelpers.Authenticate(username, password))
                    {
                        logDetail = "Username and/or Password Invalid.";
                        Logger.InfoFormat("O:--LOGIN--:Error Message/{0}", "Username and/or Password Invalid.");
                        goto Outer;
                    }

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

                    string cisPathImport = GetParameter(Constants.ParameterName.CISPathImport);
                    string cisPathError = GetParameter(Constants.ParameterName.CISPathError);

                    string fiCisCorporate = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetCisCorprate());
                    string fiCisIndividual = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetIndividual());
                    string fiCisProductGroup = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetProductGroup());
                    string fiCisSubscription = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubscription());
                    string fiCisTitle = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetTitle());
                    string fiCisProvince = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetProvince());
                    string fiCisDistrict = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetDistrict());
                    string fiCisSubDistrict = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubDistrict());
                    string fiCisPhoneType = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetPhonetype());
                    string fiCisEmailType = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetMailtype());
                    string fiCisSubmail = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubmail());
                    string fiCisSubphone = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubphone());
                    string fiCisSubaddress = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubaddress());
                    string fiCisAddressType = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetAddresstype());
                    string fiCisSubType = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetSubScriptionType());
                    string fiCisCusPhone = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetCustomerPhone());
                    string fiCisCusEmail = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetCustomerEmail());
                    string fiCisCountry = string.Format(CultureInfo.InvariantCulture, "{0}_", WebConfig.GetCountry());

                    #endregion

                    if (!skipSftp)
                    {
                        List<string> fiPrefixes = new List<string>() { fiCisCorporate, fiCisIndividual, fiCisProductGroup, fiCisSubscription, fiCisTitle,
                            fiCisProvince, fiCisDistrict, fiCisSubDistrict, fiCisPhoneType, fiCisEmailType, fiCisSubmail, fiCisSubphone, fiCisSubaddress,
                            fiCisAddressType, fiCisSubType, fiCisCusPhone, fiCisCusEmail, fiCisCountry };

                        if (!DownloadFilesViaFTP(cisPathImport, fiPrefixes, strDate))
                        {
                            logDetail = "Cannot download files from SFTP";
                            goto Outer;
                        }
                    }

                    if (_context == null)
                    {
                        _context = new CSMContext();
                    }

                    #region "Delete all temp table [Interface table]"

                    DeleteAllCisTableInterface();

                    #endregion

                    #region "Read File"

                    List<CisTitleEntity> cisTitle = ReadFileCisTitle(cisPathImport, fiCisTitle, ref numOfTitle, ref fiTitle,
                        ref isValidHeaderTitle, ref msgValidateFileTitleError, ref dataDateOfTitle);

                    List<CisCorporateEntity> cisCor = ReadFileCisCorporate(cisPathImport, fiCisCorporate, ref numOfCor, ref fiCor,
                        ref isValidHeaderCorporate, ref msgValidateFileCorporateError, ref dataDateOfCor);
                    List<CisIndividualEntity> cisIndiv = ReadFileCisIndividual(cisPathImport, fiCisIndividual, ref numOfIndiv,
                        ref fiIndiv, ref isValidHeaderIndividual, ref msgValidateFileIndividualError, ref dataDateOfIndiv);

                    List<CisProductGroupEntity> cisProd = ReadFileCisProductGroup(cisPathImport, fiCisProductGroup, ref numOfProd,
                        ref fiProd, ref isValidHeaderCisProductGroup, ref msgValidateFileProductGroupError, ref dataDateOfProd);
                    List<CisSubscriptionEntity> cisSub = ReadFileCisSubscription(cisPathImport, fiCisSubscription, ref numOfSub, ref fiSub,
                        ref isValidHeaderCisSubscription, ref msgValidateFileSubscriptionError, ref dataDateOfSub);
                    List<CisProvinceEntity> cisProvince = ReadFileCisProvince(cisPathImport, fiCisProvince, ref numOfPro, ref fiPro,
                        ref isValidHeaderCisProvince, ref msgValidateFileProvinceError, ref dataDateOfPro);
                    List<CisDistrictEntity> cisDistrict = ReadFileCisDistrict(cisPathImport, fiCisDistrict, ref numOfDis, ref fiDis,
                        ref isValidHeaderCisDistrict, ref msgValidateFileDistrictError, ref dataDateOfDis);
                    List<CisSubDistrictEntity> cisSubDistrict = ReadFileCisSubDistrict(cisPathImport, fiCisSubDistrict, ref numOfSubDis,
                        ref fiSubDis, ref isValidHeaderCisSubDistrict, ref msgValidateFileSubDistrictError, ref dataDateOfSubDis);
                    List<CisPhoneTypeEntity> cisPhonetype = ReadFileCisPhoneType(cisPathImport, fiCisPhoneType, ref numOfPhonetype,
                        ref fiPhonetype, ref isValidHeaderCisPhoneType, ref msgValidateFilePhoneTypeError, ref dataDateOfPhonetype);
                    List<CisEmailTypeEntity> cisEmailtype = ReadFileCisEmailType(cisPathImport, fiCisEmailType, ref numOfEmailtype,
                        ref fiEmailtype, ref isValidHeaderCisEmailType, ref msgValidateFileEmailTypeError, ref dataDateOfEmailtype);
                    List<CisSubscribeAddressEntity> cisSubaddress = ReadFileCisSubscribeAddress(cisPathImport, fiCisSubaddress,
                        ref numOfSubAdd, ref fiSubAdd, ref isValidHeaderCisSubscibeAddress,
                        ref msgValidateFileSubscribeAddressError, ref dataDateOfSubAdd);
                    List<CisSubscribePhoneEntity> cisSubphone = ReadFileCisSubscribePhone(cisPathImport, fiCisSubphone, ref numOfSubPhone,
                        ref fiSubPhone, ref isValidHeaderCisSubscribePhone, ref msgValidateFileSubscribePhoneError, ref dataDateOfSubPhone);
                    List<CisSubscribeMailEntity> cisSubemail = ReadFileCisSubscribeMail(cisPathImport, fiCisSubmail, ref numOfSubMail,
                        ref fiSubMail, ref isValidHeaderCisSubscribeMail, ref msgValidateFileSubscribeMailError, ref dataDateOfSubMail);
                    List<CisAddressTypeEntity> cisAddtype = ReadFileCisAddressType(cisPathImport, fiCisAddressType, ref numOfAddtype,
                        ref fiAddtype, ref isValidHeaderCisAddressType, ref msgValidateFileAddressTypeError, ref dataDateOfAddtype);
                    List<CisSubscriptionTypeEntity> cisSubType = ReadFileCisSubscriptionType(cisPathImport, fiCisSubType, ref numOfSubType,
                        ref fiSubType, ref isValidHeaderCisSubscriptionType, ref msgValidateFileSubscriptionTypeError, ref dataDateOfSubType);
                    List<CisCustomerPhoneEntity> cisCusPhone = ReadFileCisCustomerPhone(cisPathImport, fiCisCusPhone, ref numOfCusPhone,
                        ref fiCusPhone, ref isValidHeaderCisCustomerPhone, ref msgValidateFileCustomerPhoneError, ref dataDateOfCusPhone);
                    List<CisCustomerEmailEntity> cisCusEmail = ReadFileCisCustomerEmail(cisPathImport, fiCisCusEmail, ref numOfCusEmail,
                        ref fiCusEmail, ref isValidHeaderCisCustomerEmail, ref msgValidateFileCustomerEmailError, ref dataDateOfCusEmail);
                    List<CisCountryEntity> cisCountry = ReadFileCisCountry(cisPathImport, fiCisCountry, ref numOfCountry,
                        ref fiCountry, ref isValidHeaderCisCountry, ref msgValidateFileCountryError, ref dataDateOfCountry);

                    #endregion

                    bool isAllFileNotFound = false;
                    bool hasInvalidFormat = false;

                    #region "Check All file format & FileNotFound"

                    if (fiCor == "" && fiIndiv == "" && fiProd == "" && fiSub == "" && fiTitle == "" && fiPro == "" &&
                        fiDis == "" && fiSubDis == "" && fiPhonetype == "" && fiEmailtype == "" && fiSubAdd == "" &&
                        fiSubPhone == "" && fiSubMail == "" && fiAddtype == "" && fiSubType == "" && fiCusPhone == "" &&
                        fiCusEmail == "" && fiCountry == ""
                        )
                    {
                        isAllFileNotFound = true;
                    }
                    else if ((fiCor != "" && cisCor == null) || (fiIndiv != "" && cisIndiv == null) || (fiProd != "" && cisProd == null) ||
                        (fiSub != "" && cisSub == null) || (fiTitle != "" && cisTitle == null) || (fiPro != "" && cisProvince == null) ||
                        (fiDis != "" && cisDistrict == null) || (fiSubDis != "" && cisSubDistrict == null) || (fiPhonetype != "" && cisPhonetype == null) ||
                        (fiEmailtype != "" && cisEmailtype == null) || (fiSubAdd != "" && cisSubaddress == null) || (fiSubPhone != "" && cisSubphone == null) ||
                        (fiSubMail != "" && cisSubemail == null) || (fiAddtype != "" && cisAddtype == null) || (fiSubType != "" && cisSubType == null) ||
                        (fiCusPhone != "" && cisCusPhone == null) || (fiCusEmail != "" && cisCusEmail == null) || (fiCountry != "" && cisCountry == null))
                    {
                        #region Move File to Error

                        string cisPathSource = this.GetParameter(Constants.ParameterName.CisPathSource);

                        if (fiCor != "" && cisCor != null)
                        {
                            MoveFileError(cisPathSource, fiCor);
                        }

                        if (fiIndiv != "" && cisIndiv != null)
                        {
                            MoveFileError(cisPathSource, fiIndiv);
                        }

                        if (fiProd != "" && cisProd != null)
                        {
                            MoveFileError(cisPathSource, fiProd);
                        }

                        if (fiSub != "" && cisSub != null)
                        {
                            MoveFileError(cisPathSource, fiSub);
                        }

                        if (fiTitle != "" && cisTitle != null)
                        {
                            MoveFileError(cisPathSource, fiTitle);
                        }

                        if (fiPro != "" && cisProvince != null)
                        {
                            MoveFileError(cisPathSource, fiPro);
                        }

                        if (fiDis != "" && cisDistrict != null)
                        {
                            MoveFileError(cisPathSource, fiDis);
                        }

                        if (fiSubDis != "" && cisSubDistrict != null)
                        {
                            MoveFileError(cisPathSource, fiSubDis);
                        }

                        if (fiPhonetype != "" && cisPhonetype != null)
                        {
                            MoveFileError(cisPathSource, fiPhonetype);
                        }

                        if (fiEmailtype != "" && cisEmailtype != null)
                        {
                            MoveFileError(cisPathSource, fiEmailtype);
                        }

                        if (fiSubAdd != "" && cisSubaddress != null)
                        {
                            MoveFileError(cisPathSource, fiSubAdd);
                        }

                        if (fiSubPhone != "" && cisSubphone != null)
                        {
                            MoveFileError(cisPathSource, fiSubPhone);
                        }

                        if (fiSubMail != "" && cisSubemail != null)
                        {
                            MoveFileError(cisPathSource, fiSubMail);
                        }

                        if (fiAddtype != "" && cisAddtype != null)
                        {
                            MoveFileError(cisPathSource, fiAddtype);
                        }

                        if (fiSubType != "" && cisSubType != null)
                        {
                            MoveFileError(cisPathSource, fiSubType);
                        }

                        if (fiCusPhone != "" && cisCusPhone != null)
                        {
                            MoveFileError(cisPathSource, fiCusPhone);
                        }

                        if (fiCusEmail != "" && cisCusEmail != null)
                        {
                            MoveFileError(cisPathSource, fiCusEmail);
                        }

                        if (fiCountry != "" && cisCountry != null)
                        {
                            MoveFileError(cisPathSource, fiCountry);
                        }

                        #endregion

                        hasInvalidFormat = true;
                    }

                    #endregion

                    if (isAllFileNotFound == false && hasInvalidFormat == false)
                    {
                        fileErrorList = new List<object>();

                        #region "Save to Table Interface"

                        if (cisTitle == null && isValidHeaderTitle == false)
                        {
                            logDetail = msgValidateFileTitleError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiTitle))
                            {
                                fileErrorList.Add(fiTitle);
                            }
                        }
                        else
                        {
                            var saveTitle = SaveCisTitle(cisTitle, fiTitle);
                            if (saveTitle)
                            {
                                numOfTitleComplete = numOfTitle;
                            }
                            else
                            {
                                numOfTitleError = numOfTitle;
                            }
                        }

                        if (cisCor == null && isValidHeaderCorporate == false)
                        {
                            logDetail = msgValidateFileCorporateError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiCor))
                            {
                                fileErrorList.Add(fiCor);
                            }
                        }
                        else
                        {
                            saveCorporate = SaveCisCorporate(cisCor, fiCor);
                        }

                        if (cisIndiv == null && isValidHeaderIndividual == false)
                        {
                            logDetail = msgValidateFileIndividualError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiIndiv))
                            {
                                fileErrorList.Add(fiIndiv);
                            }
                        }
                        else
                        {
                            saveIndividual = SaveCisIndividual(cisIndiv, fiIndiv);
                        }

                        if (cisProd == null && isValidHeaderCisProductGroup == false)
                        {
                            logDetail = msgValidateFileProductGroupError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiProd))
                            {
                                fileErrorList.Add(fiProd);
                            }
                        }
                        else
                        {
                            var saveProductGroup = SaveCisProductGroup(cisProd, fiProd);
                            if (saveProductGroup)
                            {
                                numOfProdComplete = numOfProd;
                            }
                            else
                            {
                                numOfProdError = numOfProd;
                            }
                        }

                        if (cisSub == null && isValidHeaderCisSubscription == false)
                        {
                            logDetail = msgValidateFileSubscriptionError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSub))
                            {
                                fileErrorList.Add(fiSub);
                            }
                        }
                        else
                        {
                            saveSubscription = SaveCisSubscription(cisSub, fiSub);
                        }

                        // Country
                        if (cisCountry == null && isValidHeaderCisCountry == false)
                        {
                            logDetail = msgValidateFileCountryError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiCountry))
                            {
                                fileErrorList.Add(fiCountry);
                            }
                        }
                        else
                        {
                            if (SaveCisCountry(cisCountry, fiCountry))
                            {
                                numOfCountryComplete = numOfCountry;
                            }
                            else
                            {
                                numOfCountryError = numOfCountry;
                            }
                        }

                        // Province
                        if (cisProvince == null && isValidHeaderCisProvince == false)
                        {
                            logDetail = msgValidateFileProvinceError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiPro))
                            {
                                fileErrorList.Add(fiPro);
                            }
                        }
                        else
                        {
                            var saveProvince = SaveCisProvince(cisProvince, fiPro);
                            if (saveProvince)
                            {
                                numOfProvinceComplete = numOfPro;
                            }
                            else
                            {
                                numOfProvinceError = numOfPro;
                            }
                        }

                        if (cisDistrict == null && isValidHeaderCisDistrict == false)
                        {
                            logDetail = msgValidateFileDistrictError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiDis))
                            {
                                fileErrorList.Add(fiDis);
                            }
                        }
                        else
                        {
                            var saveDistrict = SaveCisDistrict(cisDistrict, fiDis);
                            if (saveDistrict)
                            {
                                numOfDistrictComplete = numOfDis;
                            }
                            else
                            {
                                numOfDistrictError = numOfDis;
                            }
                        }

                        if (cisSubDistrict == null && isValidHeaderCisSubDistrict == false)
                        {
                            logDetail = msgValidateFileSubDistrictError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSubDis))
                            {
                                fileErrorList.Add(fiSubDis);
                            }
                        }
                        else
                        {
                            var saveSubDistrict = SaveCisSubDistrict(cisSubDistrict, fiSubDis);
                            if (saveSubDistrict)
                            {
                                numOfSubDistrictComplete = numOfSubDis;
                            }
                            else
                            {
                                numOfSubDistrictError = numOfSubDis;
                            }
                        }

                        if (cisAddtype == null && isValidHeaderCisAddressType == false)
                        {
                            logDetail = msgValidateFileAddressTypeError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiAddtype))
                            {
                                fileErrorList.Add(fiAddtype);
                            }
                        }
                        else
                        {
                            var savAddresstype = SaveCisAddressType(cisAddtype, fiAddtype);
                            if (savAddresstype)
                            {
                                numOfAddtypeComplete = numOfAddtype;
                            }
                            else
                            {
                                numOfAddtypeError = numOfAddtype;
                            }
                        }

                        if (cisPhonetype == null && isValidHeaderCisPhoneType == false)
                        {
                            logDetail = msgValidateFilePhoneTypeError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiPhonetype))
                            {
                                fileErrorList.Add(fiPhonetype);
                            }
                        }
                        else
                        {
                            var savePhonetype = SaveCisPhoneType(cisPhonetype, fiPhonetype);
                            if (savePhonetype)
                            {
                                numOfPhonetypeComplete = numOfPhonetype;
                            }
                            else
                            {
                                numOfPhonetypeError = numOfPhonetype;
                            }
                        }

                        if (cisEmailtype == null && isValidHeaderCisEmailType == false)
                        {
                            logDetail = msgValidateFileEmailTypeError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiEmailtype))
                            {
                                fileErrorList.Add(fiEmailtype);
                            }
                        }
                        else
                        {
                            var saveEmailtype = SaveCisEmailType(cisEmailtype, fiEmailtype);
                            if (saveEmailtype)
                            {
                                numOfEmailtypeComplete = numOfEmailtype;
                            }
                            else
                            {
                                numOfEmailtypeError = numOfEmailtype;
                            }
                        }

                        if (cisSubaddress == null && isValidHeaderCisSubscibeAddress == false)
                        {
                            logDetail = msgValidateFileSubscribeAddressError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSubAdd))
                            {
                                fileErrorList.Add(fiSubAdd);
                            }
                        }
                        else
                        {
                            saveSubscribeAddress = SaveCisSubscribeAddress(cisSubaddress, fiSubAdd);
                        }

                        if (cisSubphone == null && isValidHeaderCisSubscribePhone == false)
                        {
                            logDetail = msgValidateFileSubscribePhoneError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSubPhone))
                            {
                                fileErrorList.Add(fiSubPhone);
                            }
                        }
                        else
                        {
                            saveSubscribePhone = SaveCisSubscribePhone(cisSubphone, fiSubPhone);
                        }

                        if (cisSubemail == null && isValidHeaderCisSubscribeMail == false)
                        {
                            logDetail = msgValidateFileSubscribeMailError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSubMail))
                            {
                                fileErrorList.Add(fiSubMail);
                            }
                        }
                        else
                        {
                            saveSubscribeEmail = SaveCisSubscribeEmail(cisSubemail, fiSubMail);
                        }

                        if (cisSubType == null && isValidHeaderCisSubscriptionType == false)
                        {
                            logDetail = msgValidateFileSubscriptionTypeError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiSubType))
                            {
                                fileErrorList.Add(fiSubType);
                            }
                        }
                        else
                        {
                            saveSubscriptionType = SaveCisSubscriptionType(cisSubType, fiSubType);
                        }

                        if (cisCusPhone == null && isValidHeaderCisCustomerPhone == false)
                        {
                            logDetail = msgValidateFileCustomerPhoneError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiCusPhone))
                            {
                                fileErrorList.Add(fiCusPhone);
                            }
                        }
                        else
                        {
                            saveCustomerPhone = SaveCisCustomerPhone(cisCusPhone, fiCusPhone);
                        }

                        if (cisCusEmail == null && isValidHeaderCisCustomerEmail == false)
                        {
                            logDetail = msgValidateFileCustomerEmailError;
                            Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                            if (!string.IsNullOrEmpty(fiCusPhone))
                            {
                                fileErrorList.Add(fiCusPhone);
                            }
                        }
                        else
                        {
                            saveCustomerEmail = SaveCisCustomerEmail(cisCusEmail, fiCusEmail);
                        }

                        #endregion

                        #region "Comment out"

                        //if ((!saveCorporate) && (!saveIndividual))               
                        //{
                        //    Logger.Info("I:--FAILED--:--Save CIS Corporate & Save CIS Individual--");
                        //    _logDetail = "Failed Save CIS Corporate & Save CIS Individual";
                        //    goto Outer;
                        //}

                        #endregion

                        #region "Save to Table Master and Transaction"

                        #region "[SubscriptionType]"

                        if (saveSubscriptionType)
                        {
                            var saveSubscriptionTypeComplete =
                                SaveCisSubscriptionTypeComplete(ref numOfSubTypeComplete, ref numOfSubTypeError,
                                    ref messageError);
                            if (saveSubscriptionTypeComplete)
                            {
                                if (numOfSubTypeError > 0)
                                {
                                    bool exportSubType = ExportSubscriptionTypeCIS(cisPathError, fiSubType,
                                        dataDateOfSubType);
                                    if (!exportSubType)
                                    {
                                        logDetail = "Failed to export CIS SubscriptionType";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region "[Corporate]"

                        if (saveCorporate)
                        {
                            var saveCorporateComplete = SaveCisCorporateComplete(ref numOfCorComplete,
                                ref numOfErrCor, ref messageError);
                            if (saveCorporateComplete)
                            {
                                if (numOfErrCor > 0)
                                {
                                    bool exportCorp = ExportCorporateCIS(cisPathError, fiCor, dataDateOfCor);
                                    if (!exportCorp)
                                    {
                                        logDetail = "Failed to export CIS Corporate";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region "[Individual]"

                        if (saveIndividual)
                        {
                            var saveIndividualComplete = SaveCisIndividualComplete(ref numOfIndivComplete,
                                ref numOfErrIndiv, ref messageError);
                            if (saveIndividualComplete)
                            {
                                if (numOfErrIndiv > 0)
                                {
                                    bool exportIndiv = ExportIndividualCIS(cisPathError, fiIndiv, dataDateOfIndiv);
                                    if (!exportIndiv)
                                    {
                                        logDetail = "Failed to export CIS Individual";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region "[Subscription]"

                        if (saveSubscription)
                        {
                            if (SaveCisSubscriptionComplete(ref numOfSubComplete, ref numOfErrSub,
                                ref messageError))
                            {
                                if (numOfErrSub > 0)
                                {
                                    bool exportSub = ExportSubscriptionCIS(cisPathError, fiSub, dataDateOfSub);
                                    if (!exportSub)
                                    {
                                        logDetail = "Failed to export CIS Subscription";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region "[SubscribeAddress]"

                        if (saveSubscribeAddress)
                        {
                            SaveCisSubscribeAddressComplete(ref numOfAddressComplete, ref numOfErrAddress,
                                ref messageError);
                            if (numOfErrAddress > 0)
                            {
                                bool exportAddress = ExportSubscribeAddressCIS(cisPathError, fiSubAdd, dataDateOfSubAdd);
                                if (!exportAddress)
                                {
                                    logDetail = "Failed to export CIS Subscribe Address";
                                    goto Outer;
                                }
                            }
                        }

                        #endregion

                        #region "[SubscribePhone]"

                        if (saveSubscribePhone)
                        {
                            SaveCisSubscribePhoneComplete(ref numOfPhoneComplete, ref numOfErrPhone,
                                ref messageError);
                            if (numOfErrPhone > 0)
                            {
                                bool exportPhone = ExportSubscribePhoneCIS(cisPathError, fiSubPhone, dataDateOfSubPhone);
                                if (!exportPhone)
                                {
                                    logDetail = "Failed to export CIS Subscribe Phone";
                                    goto Outer;
                                }
                            }
                        }

                        #endregion

                        #region "[SubscribeEmail]"

                        if (saveSubscribeEmail)
                        {
                            SaveCisSubscribeEmailComplete(ref numOfEmailComplete, ref numOfErrEmail,
                                ref messageError);
                            if (numOfErrEmail > 0)
                            {
                                bool exportEmail = ExportSubscribeEmailCIS(cisPathError, fiSubMail, dataDateOfSubMail);
                                if (!exportEmail)
                                {
                                    logDetail = "Failed to export CIS Subscribe Mail";
                                    goto Outer;
                                }
                            }
                        }

                        #endregion

                        #region "[CustomerPhone]"

                        if (saveCustomerPhone)
                        {
                            var saveCustomerPhoneComplet = SaveCisCustomerPhoneComplete(
                                ref numOfCusPhoneComplete, ref numOfCusPhoneError, ref messageError);
                            if (saveCustomerPhoneComplet)
                            {
                                if (numOfCusPhoneError > 0)
                                {
                                    bool exportCusphone = ExportCustomerPhoneCIS(cisPathError, fiCusPhone,
                                        dataDateOfCusPhone);
                                    if (!exportCusphone)
                                    {
                                        logDetail = "Failed to export CIS CustomerPhone";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion

                        #region "[CustomerEmail]"

                        if (saveCustomerEmail)
                        {
                            var saveCustomerEmailComplete =
                                SaveCisCustomerEmailComplete(ref numOfCusEmailComplete, ref numOfCusEmailError,
                                    ref messageError);
                            if (saveCustomerEmailComplete)
                            {
                                if (numOfCusEmailError > 0)
                                {
                                    bool exportCusEmail = ExportCustomerEmailCIS(cisPathError, fiCusEmail,
                                        dataDateOfCusEmail);
                                    if (!exportCusEmail)
                                    {
                                        logDetail = "Failed to export CIS CustomerEmail";
                                        goto Outer;
                                    }
                                }
                            }
                        }

                        #endregion


                        Logger.Info("I:--SUCCESS--:--Get GetFileCIS--");

                        #endregion

                    }

                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                    Logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                    taskResponse = new ImportCISTaskResponse
                    {
                        SchedDateTime = schedDateTime,
                        ElapsedTime = elapsedTime,
                        StatusResponse = new StatusResponse
                        {
                            Status = Constants.StatusResponse.Success
                        },
                        FileList =
                            new List<object>
                            {
                                fiTitle,
                                fiCor,
                                fiIndiv,
                                fiProd,
                                fiSub,
                                fiCountry,
                                fiPro,
                                fiDis,
                                fiSubDis,
                                fiPhonetype,
                                fiEmailtype,
                                fiAddtype,
                                fiSubAdd,
                                fiSubPhone,
                                fiSubMail,
                                fiSubType,
                                fiCusPhone,
                                fiCusEmail
                            },
                        FileErrorList = new List<object>
                        {
                            msgValidateFileTitleError
                            ,
                            msgValidateFileCorporateError
                            ,
                            msgValidateFileIndividualError
                            ,
                            msgValidateFileProductGroupError
                            ,
                            msgValidateFileSubscriptionError
                            ,
                            msgValidateFileCountryError
                            ,
                            msgValidateFileProvinceError
                            ,
                            msgValidateFileDistrictError
                            ,
                            msgValidateFileSubDistrictError
                            ,
                            msgValidateFilePhoneTypeError
                            ,
                            msgValidateFileEmailTypeError
                            ,
                            msgValidateFileAddressTypeError
                            ,
                            msgValidateFileSubscribeAddressError
                            ,
                            msgValidateFileSubscribePhoneError
                            ,
                            msgValidateFileSubscribeMailError
                            ,
                            msgValidateFileSubscriptionTypeError
                            ,
                            msgValidateFileCustomerPhoneError
                            ,
                            msgValidateFileCustomerEmailError
                        },

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

                    if (isAllFileNotFound)
                    {
                        taskResponse.StatusResponse.Status = Constants.StatusResponse.Success;
                        processDetials = "File Not Found";
                        SaveLogWithFileNotFound(taskResponse);
                    }
                    else if (hasInvalidFormat)
                    {
                        var sb = new StringBuilder("");

                        #region File invalid format

                        if (fiCor != "" && cisCor == null)
                        {
                            sb.AppendFormat("{0} \n", fiCor);
                        }

                        if (fiIndiv != "" && cisIndiv == null)
                        {
                            sb.AppendFormat("{0} \n", fiIndiv);
                        }

                        if (fiProd != "" && cisProd == null)
                        {
                            sb.AppendFormat("{0} \n", fiProd);
                        }

                        if (fiSub != "" && cisSub == null)
                        {
                            sb.AppendFormat("{0} \n", fiSub);
                        }

                        if (fiTitle != "" && cisTitle == null)
                        {
                            sb.AppendFormat("{0} \n", fiTitle);
                        }

                        if (fiPro != "" && cisProvince == null)
                        {
                            sb.AppendFormat("{0} \n", fiPro);
                        }

                        if (fiDis != "" && cisDistrict == null)
                        {
                            sb.AppendFormat("{0} \n", fiDis);
                        }

                        if (fiSubDis != "" && cisSubDistrict == null)
                        {
                            sb.AppendFormat("{0} \n", fiSubDis);
                        }

                        if (fiPhonetype != "" && cisPhonetype == null)
                        {
                            sb.AppendFormat("{0} \n", fiPhonetype);
                        }

                        if (fiEmailtype != "" && cisEmailtype == null)
                        {
                            sb.AppendFormat("{0} \n", fiEmailtype);
                        }

                        if (fiSubAdd != "" && cisSubaddress == null)
                        {
                            sb.AppendFormat("{0} \n", fiSubAdd);
                        }

                        if (fiSubPhone != "" && cisSubphone == null)
                        {
                            sb.AppendFormat("{0} \n", fiSubPhone);
                        }

                        if (fiSubMail != "" && cisSubemail == null)
                        {
                            sb.AppendFormat("{0} \n", fiSubMail);
                        }

                        if (fiAddtype != "" && cisAddtype == null)
                        {
                            sb.AppendFormat("{0} \n", fiAddtype);
                        }

                        if (fiSubType != "" && cisSubType == null)
                        {
                            sb.AppendFormat("{0} \n", fiSubType);
                        }

                        if (fiCusPhone != "" && cisCusPhone == null)
                        {
                            sb.AppendFormat("{0} \n", fiCusPhone);
                        }

                        if (fiCusEmail != "" && cisCusEmail == null)
                        {
                            sb.AppendFormat("{0} \n", fiCusEmail);
                        }

                        if (fiCountry != "" && cisCountry == null)
                        {
                            sb.AppendFormat("{0} \n", fiCountry);
                        }

                        #endregion

                        taskResponse.StatusResponse.Status = Constants.StatusResponse.Failed;
                        processDetials = string.Format(CultureInfo.InvariantCulture, "Invalid files \n {0}\n", sb.ToString());
                        SaveLogWithInvalidFile(taskResponse, processDetials);
                    }
                    else
                    {
                        if (numOfErrCor > 0 || numOfErrIndiv > 0 || numOfErrSub > 0 || numOfErrAddress > 0 ||
                            numOfErrPhone > 0 || numOfErrEmail > 0 || numOfSubTypeError > 0 || numOfCusPhoneError > 0 ||
                            numOfCusEmailError > 0 || numOfProdError > 0 || numOfTitleError > 0 || numOfCountryError > 0 ||
                            numOfProvinceError > 0 || numOfDistrictError > 0 || numOfSubDistrictError > 0 ||
                            numOfAddtypeError > 0 || numOfPhonetypeError > 0 || numOfEmailtypeError > 0)
                        {
                            taskResponse.StatusResponse.Status = Constants.StatusResponse.Failed;
                            SaveLogSuccessOrFail(taskResponse, LogStatus.Fail);
                        }
                        else
                        {
                            taskResponse.StatusResponse.Status = Constants.StatusResponse.Success;
                            SaveLogSuccessOrFail(taskResponse, LogStatus.Success);
                        }

                        processDetials = taskResponse.ToString();
                    }

                    return taskResponse;
                }
                catch (Exception ex)
                {
                    logDetail = ex.Message;
                    Logger.InfoFormat("O:--FAILED--:Error Message/{0}", logDetail);
                    Logger.Error("Exception occur:\n", ex);
                }

            Outer:
                stopwatch.Stop();
                elapsedTime = stopwatch.ElapsedMilliseconds;
                Logger.Debug("-- Finish Cron Job --:ElapsedMilliseconds/" + elapsedTime);

                taskResponse = new ImportCISTaskResponse
                {
                    SchedDateTime = schedDateTime,
                    ElapsedTime = elapsedTime,
                    StatusResponse = new StatusResponse
                    {
                        Status = Constants.StatusResponse.Failed,
                        ErrorCode = string.Empty,
                        Description = logDetail
                    },
                    FileList =
                        new List<object>
                        {
                            fiTitle,
                            fiCor,
                            fiIndiv,
                            fiProd,
                            fiSub,
                            fiCountry,
                            fiPro,
                            fiDis,
                            fiSubDis,
                            fiPhonetype,
                            fiEmailtype,
                            fiSubAdd,
                            fiSubPhone,
                            fiSubMail,
                            fiAddtype,
                            fiSubType,
                            fiCusPhone,
                            fiCusEmail
                        },
                    FileErrorList = fileErrorList
                };

                processDetials = taskResponse.StatusResponse.Description;
                SaveLogError(taskResponse);
                return taskResponse;
            }
            finally
            {
                // Send mail to system administrator
                ImportCisSendMail(taskResponse);

                #region "BatchProcess End"

                if (taskResponse != null && taskResponse.StatusResponse != null &&
                    taskResponse.StatusResponse.Status != Constants.StatusResponse.NotProcess)
                {

                    int batchStatus = (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                        ? Constants.BatchProcessStatus.Success
                        : Constants.BatchProcessStatus.Fail;

                    DateTime endTime = taskResponse.SchedDateTime.AddMilliseconds(taskResponse.ElapsedTime);
                    TimeSpan processTime = endTime.Subtract(taskResponse.SchedDateTime);

                    AppLog.BatchProcessEnd(Constants.BatchProcessCode.ImportCIS, batchStatus, endTime, processTime,
                        processDetials);
                }

                #endregion
            }
        }

        private string GetParameter(string paramName)
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(paramName);
            return param != null ? param.ParamValue : string.Empty;
        }

        private List<CisCorporateEntity> ReadFileCisCorporate(string filePath, string fiPrefix, ref int numOfCor, ref string fiCor, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath,
                    string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiCor = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCorporate)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisCorporate - 2].NullSafeTrim();
                        // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiCor);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiCor);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord =
                            header[Constants.ImportCisData.LengthOfHeaderCisCorporate - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiCor);
                        }
                    }

                    if (isValidFormat)
                    {

                        int inx = 2;
                        var lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisCorporate ||
                                source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}",
                                    inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiCor,
                                string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiCor,
                                lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiCor); //Move file
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture,
                            " File name : {0}  is invalid file format.", fiCor);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisCorporate();
                        goto Outer;
                    }

                    #endregion

                    List<CisCorporateEntity> cisCorporate = (from source in results.Skip(1)
                                                             select new CisCorporateEntity
                                                             {
                                                                 KKCisId = source[1].NullSafeTrim(),
                                                                 CustId = source[2].NullSafeTrim(),
                                                                 CardId = source[3].NullSafeTrim(),
                                                                 CardTypeCode = source[4].NullSafeTrim(),
                                                                 CustTypeCode = source[5].NullSafeTrim(),
                                                                 CustTypeGroup = source[6].NullSafeTrim(),
                                                                 TitleId = source[7].NullSafeTrim(),
                                                                 NameTh = source[8].NullSafeTrim(),
                                                                 NameEn = source[9].NullSafeTrim(),
                                                                 IsicCode = source[10].NullSafeTrim(),
                                                                 TaxId = source[11].NullSafeTrim(),
                                                                 HostBusinessCountryCode = source[12].NullSafeTrim(),
                                                                 ValuePerShare = source[13].NullSafeTrim(),
                                                                 AuthorizedShareCapital = source[14].NullSafeTrim(),
                                                                 RegisterDate = source[15].NullSafeTrim(),
                                                                 BusinessCode = source[16].NullSafeTrim(),
                                                                 FixedAsset = source[17].NullSafeTrim(),
                                                                 FixedAssetexcludeLand = source[18].NullSafeTrim(),
                                                                 NumberOfEmployee = source[19].NullSafeTrim(),
                                                                 ShareInfoFlag = source[20].NullSafeTrim(),
                                                                 FlgmstApp = source[21].NullSafeTrim(),
                                                                 FirstBranch = source[22].NullSafeTrim(),
                                                                 PlaceCustUpdated = source[23].NullSafeTrim(),
                                                                 DateCustUpdated = source[24].NullSafeTrim(),
                                                                 IdCountryIssue = source[25].NullSafeTrim(),
                                                                 BusinessCatCode = source[26].NullSafeTrim(),
                                                                 MarketingId = source[27].NullSafeTrim(),
                                                                 Stock = source[28].NullSafeTrim(),
                                                                 CreatedDate = source[29].NullSafeTrim(),
                                                                 CreatedBy = source[30].NullSafeTrim(),
                                                                 UpdatedDate = source[31].NullSafeTrim(),
                                                                 UpdatedBy = source[32].NullSafeTrim()

                                                             }).ToList();


                    #region "Validate MaxLength"

                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (CisCorporateEntity corporate in cisCorporate)
                    {
                        corporate.Error = ValidateMaxLength(corporate);
                        if (!string.IsNullOrEmpty(corporate.Error))
                        {
                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), corporate.Error));
                        }
                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiCor, string.Join("\n", lstErrMaxLength.ToArray()));
                        MoveFileError(filePath, fiCor);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiCor);
                        isValidHeader = false; // ref value
                        goto Outer;
                    }

                    #endregion

                    numOfCor = cisCorporate.Count;
                    return cisCorporate;
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisCorporate();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiCor))
                {
                    MoveFileSource(filePath, fiCor);
                }
            }
            return null;
        }

        private List<CisIndividualEntity> ReadFileCisIndividual(string filePath, string fiPrefix, ref int numOfIndiv, ref string fiIndiv, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiIndiv = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisIndividual)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisIndividual - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiIndiv);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiIndiv);
                    }

                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisIndividual - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiIndiv);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        var lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisIndividual || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiIndiv, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiIndiv, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiIndiv);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiIndiv);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisIndividual();
                        goto Outer;
                    }
                    #endregion

                    List<CisIndividualEntity> cisIndividual = (from source in results.Skip(1)
                                                               select new CisIndividualEntity
                                                               {
                                                                   KKCisId = source[1].NullSafeTrim(),
                                                                   CustId = source[2].NullSafeTrim(),
                                                                   CardId = source[3].NullSafeTrim(),
                                                                   CardtypeCode = source[4].NullSafeTrim(),
                                                                   CusttypeCode = source[5].NullSafeTrim(),
                                                                   CusttypeGroup = source[6].NullSafeTrim(),
                                                                   TitleId = source[7].NullSafeTrim(),
                                                                   TitlenameCustom = source[8].NullSafeTrim(),
                                                                   FirstnameTh = source[9].NullSafeTrim(),
                                                                   MidnameTh = source[10].NullSafeTrim(),
                                                                   LastnameTh = source[11].NullSafeTrim(),
                                                                   FirstnameEn = source[12].NullSafeTrim(),
                                                                   MidnameEn = source[13].NullSafeTrim(),
                                                                   LastnameEn = source[14].NullSafeTrim(),
                                                                   BirthDate = source[15].NullSafeTrim(),
                                                                   GenderCode = source[16].NullSafeTrim(),
                                                                   MaritalCode = source[17].NullSafeTrim(),
                                                                   Nationality1Code = source[18].NullSafeTrim(),
                                                                   Nationality2Code = source[19].NullSafeTrim(),
                                                                   Nationality3Code = source[20].NullSafeTrim(),
                                                                   ReligionCode = source[21].NullSafeTrim(),
                                                                   EducationCode = source[22].NullSafeTrim(),
                                                                   Position = source[23].NullSafeTrim(),
                                                                   BusinessCode = source[24].NullSafeTrim(),
                                                                   CompanyName = source[25].NullSafeTrim(),
                                                                   IsicCode = source[26].NullSafeTrim(),
                                                                   AnnualIncome = source[27].NullSafeTrim(),
                                                                   SourceIncome = source[28].NullSafeTrim(),
                                                                   TotalwealthPeriod = source[29].NullSafeTrim(),
                                                                   FlgmstApp = source[30].NullSafeTrim(),
                                                                   ChannelHome = source[31].NullSafeTrim(),
                                                                   FirstBranch = source[32].NullSafeTrim(),
                                                                   ShareinfoFlag = source[33].NullSafeTrim(),
                                                                   PlacecustUpdated = source[34].NullSafeTrim(),
                                                                   DatecustUpdated = source[35].NullSafeTrim(),
                                                                   AnnualincomePeriod = source[36].NullSafeTrim(),
                                                                   MarketingId = source[37].NullSafeTrim(),
                                                                   NumberofEmployee = source[38].NullSafeTrim(),
                                                                   FixedAsset = source[39].NullSafeTrim(),
                                                                   FixedassetExcludeland = source[40].NullSafeTrim(),
                                                                   OccupationCode = source[41].NullSafeTrim(),
                                                                   OccupationsubtypeCode = source[42].NullSafeTrim(),
                                                                   CountryIncome = source[43].NullSafeTrim(),
                                                                   TotalwealTh = source[44].NullSafeTrim(),
                                                                   SourceIncomerem = source[45].NullSafeTrim(),
                                                                   CreatedDate = source[46].NullSafeTrim(),
                                                                   CreatedBy = source[47].NullSafeTrim(),
                                                                   UpdateDate = source[48].NullSafeTrim(),
                                                                   UpdatedBy = source[49].NullSafeTrim()
                                                               }).ToList();

                    #region "Validate MaxLength"
                    int inxErr = 2;
                    var lstErrMaxLength = new List<string>();
                    foreach (CisIndividualEntity individual in cisIndividual)
                    {
                        individual.Error = ValidateMaxLength(individual);
                        if (!string.IsNullOrEmpty(individual.Error))
                        {
                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), individual.Error));
                        }
                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiIndiv, string.Join("\n", lstErrMaxLength.ToArray()));
                        MoveFileError(filePath, fiIndiv);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiIndiv);
                        isValidHeader = false; // ref value
                        goto Outer;
                    }

                    #endregion

                    numOfIndiv = cisIndividual.Count;
                    return cisIndividual;
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisIndividual();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiIndiv))
                {
                    MoveFileSource(filePath, fiIndiv);
                }
            }
            return null;
        }

        private List<CisProductGroupEntity> ReadFileCisProductGroup(string filePath, string fiPrefix, ref int numOfProd, ref string fiProd, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiProd = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisProductGroup)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisProductGroup - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiProd);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiProd);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisProductGroup - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiProd);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisProductGroup || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiProd, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiProd, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiProd);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiProd);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisProductGroup();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisProductGroupEntity> cisProductGroup = (from source in results.Skip(1)
                                                                       select new CisProductGroupEntity
                                                                       {
                                                                           ProductCode = source[1].NullSafeTrim(),
                                                                           ProductType = source[2].NullSafeTrim(),
                                                                           ProductDesc = source[3].NullSafeTrim(),
                                                                           SYSTEM = source[4].NullSafeTrim(),
                                                                           SubscrCode = source[5].NullSafeTrim(),
                                                                           SubscrDesc = source[6].NullSafeTrim(),
                                                                           ProductFlag = source[7].NullSafeTrim(),
                                                                           EntityCode = source[8].NullSafeTrim(),
                                                                           Status = source[9].NullSafeTrim(),
                                                                       }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisProductGroupEntity productGroup in cisProductGroup)
                        {
                            Error = ValidateMaxLength(productGroup);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiProd, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiProd);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiProd);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfProd = cisProductGroup.Count;
                        return cisProductGroup;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisProductGroup();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiProd))
                {
                    MoveFileSource(filePath, fiProd);
                }
            }
            return null;
        }

        private List<CisSubscriptionEntity> ReadFileCisSubscription(string filePath, string fiPrefix, ref int numOfSub, ref string fiSub, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSub = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubscription)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisSubscription - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSub);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSub);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisSubscription - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSub);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisSubscription || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSub, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSub, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSub);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSub);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubscription();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubscriptionEntity> cisSubscription = (from source in results.Skip(1)
                                                                       select new CisSubscriptionEntity
                                                                       {
                                                                           KKCisId = source[1].NullSafeTrim(),
                                                                           CustId = source[2].NullSafeTrim(),
                                                                           CardId = source[3].NullSafeTrim(),
                                                                           CardTypeCode = source[4].NullSafeTrim(),
                                                                           ProdGroup = source[5].NullSafeTrim(),
                                                                           ProdType = source[6].NullSafeTrim(),
                                                                           SubscrCode = source[7].NullSafeTrim(),
                                                                           RefNo = source[8].NullSafeTrim(),
                                                                           BranchName = source[9].NullSafeTrim(),
                                                                           Text1 = source[10].NullSafeTrim(),
                                                                           Text2 = source[11].NullSafeTrim(),
                                                                           Text3 = source[12].NullSafeTrim(),
                                                                           Text4 = source[13].NullSafeTrim(),
                                                                           Text5 = source[14].NullSafeTrim(),
                                                                           Text6 = source[15].NullSafeTrim(),
                                                                           Text7 = source[16].NullSafeTrim(),
                                                                           Text8 = source[17].NullSafeTrim(),
                                                                           Text9 = source[18].NullSafeTrim(),
                                                                           Text10 = source[19].NullSafeTrim(),
                                                                           Number1 = source[20].NullSafeTrim(),
                                                                           Number2 = source[21].NullSafeTrim(),
                                                                           Number3 = source[22].NullSafeTrim(),
                                                                           Number4 = source[23].NullSafeTrim(),
                                                                           Number5 = source[24].NullSafeTrim(),
                                                                           Date1 = source[25].NullSafeTrim(),
                                                                           Date2 = source[26].NullSafeTrim(),
                                                                           Date3 = source[27].NullSafeTrim(),
                                                                           Date4 = source[28].NullSafeTrim(),
                                                                           Date5 = source[29].NullSafeTrim(),
                                                                           SubscrStatus = source[30].NullSafeTrim(),
                                                                           CreatedDate = source[31].NullSafeTrim(),
                                                                           CreatedBy = source[32].NullSafeTrim(),
                                                                           CreatedChanel = source[33].NullSafeTrim(),
                                                                           UpdatedDate = source[34].NullSafeTrim(),
                                                                           UpdatedBy = source[35].NullSafeTrim(),
                                                                           UpdatedChannel = source[36].NullSafeTrim(),
                                                                           SysCustSubscrId = source[37].NullSafeTrim()
                                                                       }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscriptionEntity subScription in cisSubscription)
                        {
                            Error = ValidateMaxLength(subScription);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSub, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSub);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSub);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSub = cisSubscription.Count;
                        return cisSubscription;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubscription();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSub))
                {
                    MoveFileSource(filePath, fiSub);
                }
            }
            return null;
        }

        private List<CisTitleEntity> ReadFileCisTitle(string filePath, string fiPrefix, ref int numOfTi, ref string fiTi, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiTi = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisTitle)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisTitle - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiTi);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiTi);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisTitle - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiTi);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisTitle || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiTi, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiTi, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiTi);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiTi);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisTitle();
                        goto Outer;
                    }

                    #endregion

                    if (isValidFormat)
                    {
                        List<CisTitleEntity> cisTitle = (from source in results.Skip(1)
                                                         select new CisTitleEntity
                                                         {
                                                             TitleID = source[1].NullSafeTrim(),
                                                             TitleNameTH = source[2].NullSafeTrim(),
                                                             TitleNameEN = source[3].NullSafeTrim(),
                                                             TitleTypeGroup = source[4].NullSafeTrim(),
                                                             GenderCode = source[5].NullSafeTrim(),
                                                             Status = source[6].NullSafeTrim(),
                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisTitleEntity Title in cisTitle)
                        {
                            Error = ValidateMaxLength(Title);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiTi, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiTi);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiTi);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfTi = cisTitle.Count;
                        return cisTitle;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisTitle();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiTi))
                {
                    MoveFileSource(filePath, fiTi);
                }
            }
            return null;
        }

        private List<CisProvinceEntity> ReadFileCisProvince(string filePath, string fiPrefix, ref int numOfPro, ref string fiPro, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiPro = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisProvince)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisProvince - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiPro);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiPro);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisProvince - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiPro);
                        }
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 3);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisProvince || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiPro, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiPro, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiPro);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiPro);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisProvince();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisProvinceEntity> cisProvince = (from source in results.Skip(1)
                                                               select new CisProvinceEntity
                                                               {
                                                                   ProvinceCode = source[1].NullSafeTrim(),
                                                                   ProvinceNameTH = source[2].NullSafeTrim(),
                                                                   ProvinceNameEN = source[3].NullSafeTrim(),
                                                                   Status = source[4].NullSafeTrim(),
                                                               }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisProvinceEntity Province in cisProvince)
                        {
                            Error = ValidateMaxLength(Province);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiPro, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiPro);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiPro);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfPro = cisProvince.Count;
                        return cisProvince;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisProvince();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiPro))
                {
                    MoveFileSource(filePath, fiPro);
                }
            }
            return null;
        }

        private List<CisDistrictEntity> ReadFileCisDistrict(string filePath, string fiPrefix, ref int numOfDis, ref string fiDis, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiDis = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisDistrict)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisDistrict - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiDis);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiDis);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisDistrict - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiDis);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisDistrict || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiDis, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiDis, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiDis);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiDis);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisDistrict();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisDistrictEntity> cisDistrict = (from source in results.Skip(1)
                                                               select new CisDistrictEntity
                                                               {
                                                                   ProvinceCode = source[1].NullSafeTrim(),
                                                                   DistrictCode = source[2].NullSafeTrim(),
                                                                   DistrictNameTH = source[3].NullSafeTrim(),
                                                                   DistrictNameEN = source[4].NullSafeTrim(),
                                                                   Status = source[5].NullSafeTrim()
                                                               }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisDistrictEntity District in cisDistrict)
                        {
                            Error = ValidateMaxLength(District);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiDis, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiDis);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiDis);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfDis = cisDistrict.Count;
                        return cisDistrict;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisDistrict();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiDis))
                {
                    MoveFileSource(filePath, fiDis);
                }
            }
            return null;
        }

        private List<CisSubDistrictEntity> ReadFileCisSubDistrict(string filePath, string fiPrefix, ref int numOfSubDis, ref string fiSubDis, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath,
                    string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubDis = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"

                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubDistrict)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisSubDistrict - 2].NullSafeTrim();
                        // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSubDis);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSubDis);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord =
                            header[Constants.ImportCisData.LengthOfHeaderCisSubDistrict - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSubDis);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisSubDistrict ||
                                source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}",
                                    inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubDis,
                                string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubDis,
                                lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubDis);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture,
                            " File name : {0}  is invalid file format.", fiSubDis);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubDistrict();
                        goto Outer;
                    }

                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubDistrictEntity> cisSubDistrict = (from source in results.Skip(1)
                                                                     select new CisSubDistrictEntity
                                                                     {
                                                                         DistrictCode = source[1].NullSafeTrim(),
                                                                         SubDistrictCode = source[2].NullSafeTrim(),
                                                                         SubDistrictNameTH = source[3].NullSafeTrim(),
                                                                         SubDistrictNameEN = source[4].NullSafeTrim(),
                                                                         PostCode = source[5].NullSafeTrim(),
                                                                         Status = source[6].NullSafeTrim()
                                                                     }).ToList();

                        #region "Validate MaxLength"

                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubDistrictEntity SubDistrict in cisSubDistrict)
                        {
                            Error = ValidateMaxLength(SubDistrict);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSubDis, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubDis);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSubDis);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSubDis = cisSubDistrict.Count;
                        return cisSubDistrict;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubDistrict();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSubDis))
                {
                    MoveFileSource(filePath, fiSubDis);
                }
            }
            return null;
        }

        private List<CisPhoneTypeEntity> ReadFileCisPhoneType(string filePath, string fiPrefix, ref int numOfPhonetype, ref string fiPhonetype, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiPhonetype = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisPhoneType)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisPhoneType - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiPhonetype);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiPhonetype);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisPhoneType - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiPhonetype);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisPhoneType || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiPhonetype, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiPhonetype, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiPhonetype);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiPhonetype);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisPhoneType();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisPhoneTypeEntity> cisPhonetype = (from source in results.Skip(1)
                                                                 select new CisPhoneTypeEntity
                                                                 {
                                                                     PhoneTypecode = source[1].NullSafeTrim(),
                                                                     PhoneTypeDesc = source[2].NullSafeTrim(),
                                                                     Status = source[3].NullSafeTrim()
                                                                 }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisPhoneTypeEntity Phonetype in cisPhonetype)
                        {
                            Error = ValidateMaxLength(Phonetype);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiPhonetype, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiPhonetype);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiPhonetype);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfPhonetype = cisPhonetype.Count;
                        return cisPhonetype;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisPhoneType();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiPhonetype))
                {
                    MoveFileSource(filePath, fiPhonetype);
                }
            }
            return null;
        }

        private List<CisEmailTypeEntity> ReadFileCisEmailType(string filePath, string fiPrefix, ref int numOfEmailtype, ref string fiEmailtype, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiEmailtype = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderEmailType)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderEmailType - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiEmailtype);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiEmailtype);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderEmailType - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiEmailtype);
                        }
                    }

                    if (isValidFormat)
                    {

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailEmailType || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiEmailtype, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiEmailtype, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiEmailtype);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiEmailtype);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisEmailType();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisEmailTypeEntity> cisEmailtype = (from source in results.Skip(1)
                                                                 select new CisEmailTypeEntity
                                                                 {
                                                                     MailTypecode = source[1].NullSafeTrim(),
                                                                     MailTypeDesc = source[2].NullSafeTrim(),
                                                                     Status = source[3].NullSafeTrim()
                                                                 }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisEmailTypeEntity Emailtype in cisEmailtype)
                        {
                            Error = ValidateMaxLength(Emailtype);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiEmailtype, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiEmailtype);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiEmailtype);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfEmailtype = cisEmailtype.Count;
                        return cisEmailtype;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisEmailType();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiEmailtype))
                {
                    MoveFileSource(filePath, fiEmailtype);
                }
            }
            return null;
        }

        private List<CisSubscribeAddressEntity> ReadFileCisSubscribeAddress(string filePath, string fiPrefix, ref int numOfSubAdd, ref string fiSubAdd, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubAdd = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubscriptionAddress)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisSubscriptionAddress - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSubAdd);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSubAdd);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisSubscriptionAddress - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSubAdd);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisSubscriptionAddress || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubAdd, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubAdd, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubAdd);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSubAdd);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubscribeAddress();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubscribeAddressEntity> cisSubaddress = (from source in results.Skip(1)
                                                                         select new CisSubscribeAddressEntity
                                                                         {
                                                                             KKCisId = source[1].NullSafeTrim(),
                                                                             CustId = source[2].NullSafeTrim(),
                                                                             CardId = source[3].NullSafeTrim(),
                                                                             CardTypeCode = source[4].NullSafeTrim(),
                                                                             ProdGroup = source[5].NullSafeTrim(),
                                                                             ProdType = source[6].NullSafeTrim(),
                                                                             SubscrCode = source[7].NullSafeTrim(),
                                                                             AddressTypeCode = source[8].NullSafeTrim(),
                                                                             AddressNumber = source[9].NullSafeTrim(),
                                                                             Village = source[10].NullSafeTrim(),
                                                                             Building = source[11].NullSafeTrim(),
                                                                             FloorNo = source[12].NullSafeTrim(),
                                                                             RoomNo = source[13].NullSafeTrim(),
                                                                             Moo = source[14].NullSafeTrim(),
                                                                             Street = source[15].NullSafeTrim(),
                                                                             Soi = source[16].NullSafeTrim(),
                                                                             SubDistrictCode = source[17].NullSafeTrim(),
                                                                             DistrictCode = source[18].NullSafeTrim(),
                                                                             ProvinceCode = source[19].NullSafeTrim(),
                                                                             CountryCode = source[20].NullSafeTrim(),
                                                                             PostalCode = source[21].NullSafeTrim(),
                                                                             SubDistrictValue = source[22].NullSafeTrim(),
                                                                             DistrictValue = source[23].NullSafeTrim(),
                                                                             ProvinceValue = source[24].NullSafeTrim(),
                                                                             PostalValue = source[25].NullSafeTrim(),
                                                                             CreatedDate = source[26].NullSafeTrim(),
                                                                             CreatedBy = source[27].NullSafeTrim(),
                                                                             UpdatedDate = source[28].NullSafeTrim(),
                                                                             UpdatedBy = source[29].NullSafeTrim(),
                                                                             SysCustSubscrId = source[30].NullSafeTrim()
                                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribeAddressEntity Subaddress in cisSubaddress)
                        {
                            Error = ValidateMaxLength(Subaddress);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSubAdd, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubAdd);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSubAdd);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSubAdd = cisSubaddress.Count;
                        return cisSubaddress;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubscribeAddress();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSubAdd))
                {
                    MoveFileSource(filePath, fiSubAdd);
                }
            }
            return null;
        }

        private List<CisSubscribePhoneEntity> ReadFileCisSubscribePhone(string filePath, string fiPrefix, ref int numOfSubPhone, ref string fiSubPhone, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubPhone = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubscribePhone)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisSubscribePhone - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSubPhone);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSubPhone);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisSubscribePhone - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSubPhone);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisSubscribePhone || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubPhone, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubPhone, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubPhone);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSubPhone);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubscribePhone();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubscribePhoneEntity> cisSubphone = (from source in results.Skip(1)
                                                                     select new CisSubscribePhoneEntity
                                                                     {
                                                                         KKCisId = source[1].NullSafeTrim(),
                                                                         CustId = source[2].NullSafeTrim(),
                                                                         CardId = source[3].NullSafeTrim(),
                                                                         CardTypeCode = source[4].NullSafeTrim(),
                                                                         ProdGroup = source[5].NullSafeTrim(),
                                                                         ProdType = source[6].NullSafeTrim(),
                                                                         SubscrCode = source[7].NullSafeTrim(),
                                                                         PhoneTypeCode = source[8].NullSafeTrim(),
                                                                         PhoneNum = source[9].NullSafeTrim(),
                                                                         PhoneExt = source[10].NullSafeTrim(),
                                                                         CreatedDate = source[11].NullSafeTrim(),
                                                                         CreatedBy = source[12].NullSafeTrim(),
                                                                         UpdatedDate = source[13].NullSafeTrim(),
                                                                         UpdatedBy = source[14].NullSafeTrim(),
                                                                         SysCustSubscrId = source[15].NullSafeTrim()
                                                                     }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribePhoneEntity Subphone in cisSubphone)
                        {
                            Error = ValidateMaxLength(Subphone);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSubPhone, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubPhone);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSubPhone);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSubPhone = cisSubphone.Count;
                        return cisSubphone;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubscribePhone();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSubPhone))
                {
                    MoveFileSource(filePath, fiSubPhone);
                }
            }
            return null;
        }

        private List<CisSubscribeMailEntity> ReadFileCisSubscribeMail(string filePath, string fiPrefix, ref int numOfSubMail, ref string fiSubMail, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubMail = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubscribeMail)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisSubscribeMail - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSubMail);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSubMail);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisSubscribeMail - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSubMail);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisSubscribeMail || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubMail, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubMail, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubMail);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSubMail);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubscribeEmail();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubscribeMailEntity> cisSubmail = (from source in results.Skip(1)
                                                                   select new CisSubscribeMailEntity
                                                                   {
                                                                       KKCisId = source[1].NullSafeTrim(),
                                                                       CustId = source[2].NullSafeTrim(),
                                                                       CardId = source[3].NullSafeTrim(),
                                                                       CardTypeCode = source[4].NullSafeTrim(),
                                                                       ProdGroup = source[5].NullSafeTrim(),
                                                                       ProdType = source[6].NullSafeTrim(),
                                                                       SubscrCode = source[7].NullSafeTrim(),
                                                                       MailTypeCode = source[8].NullSafeTrim(),
                                                                       MailAccount = source[9].NullSafeTrim(),
                                                                       CreatedDate = source[10].NullSafeTrim(),
                                                                       CreatedBy = source[11].NullSafeTrim(),
                                                                       UpdatedDate = source[12].NullSafeTrim(),
                                                                       UpdatedBy = source[13].NullSafeTrim(),
                                                                       SysCustSubscrId = source[14].NullSafeTrim()
                                                                   }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribeMailEntity Submail in cisSubmail)
                        {
                            Error = ValidateMaxLength(Submail);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSubMail, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubMail);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSubMail);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSubMail = cisSubmail.Count;
                        return cisSubmail;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubscribeEmail();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSubMail))
                {
                    MoveFileSource(filePath, fiSubMail);
                }
            }
            return null;
        }

        private List<CisAddressTypeEntity> ReadFileCisAddressType(string filePath, string fiPrefix, ref int numOfAddtype, ref string fiAddtype, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiAddtype = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisAddressType)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisAddressType - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiAddtype);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiAddtype);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisAddressType - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiAddtype);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisAddressType || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiAddtype, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiAddtype, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiAddtype);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiAddtype);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisAddressType();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisAddressTypeEntity> cisAddType = (from source in results.Skip(1)
                                                                 select new CisAddressTypeEntity
                                                                 {
                                                                     AddressTypeCode = source[1].NullSafeTrim(),
                                                                     AddressTypeDesc = source[2].NullSafeTrim(),
                                                                     Status = source[3].NullSafeTrim()
                                                                 }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisAddressTypeEntity AddType in cisAddType)
                        {
                            Error = ValidateMaxLength(AddType);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiAddtype, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiAddtype);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiAddtype);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfAddtype = cisAddType.Count;
                        return cisAddType;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisAddressType();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiAddtype))
                {
                    MoveFileSource(filePath, fiAddtype);
                }
            }
            return null;
        }

        private List<CisSubscriptionTypeEntity> ReadFileCisSubscriptionType(string filePath, string fiPrefix, ref int numOfSubType, ref string fiSubType, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubType = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCisSubscriptionType)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisCisSubscriptionType - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiSubType);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiSubType);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisCisSubscriptionType - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiSubType);
                        }
                    }

                    if (isValidFormat)
                    {

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisCisSubscriptionType || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubType, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubType, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubType);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSubType);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisSubscriptionType();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisSubscriptionTypeEntity> cisSubscriptionType = (from source in results.Skip(1)
                                                                               select new CisSubscriptionTypeEntity
                                                                               {
                                                                                   CustTypeGroup = source[1].NullSafeTrim(),
                                                                                   CustTypeCode = source[2].NullSafeTrim(),
                                                                                   CustTypeTh = source[3].NullSafeTrim(),
                                                                                   CustTypeEn = source[4].NullSafeTrim(),
                                                                                   CardTypeCode = source[5].NullSafeTrim(),
                                                                                   CardTypeEn = source[6].NullSafeTrim(),
                                                                                   CardTypeTh = source[7].NullSafeTrim(),
                                                                                   Status = source[8].NullSafeTrim(),
                                                                               }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        foreach (CisSubscriptionTypeEntity SubscriptionType in cisSubscriptionType)
                        {
                            SubscriptionType.Error = ValidateMaxLength(SubscriptionType);
                            if (!string.IsNullOrEmpty(SubscriptionType.Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), SubscriptionType.Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiSubType, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubType);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiSubType);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfSubType = cisSubscriptionType.Count;
                        return cisSubscriptionType;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisSubscriptionType();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiSubType))
                {
                    MoveFileSource(filePath, fiSubType);
                }
            }
            return null;
        }

        private List<CisCustomerPhoneEntity> ReadFileCisCustomerPhone(string filePath, string fiPrefix, ref int numOfCusPhone, ref string fiCusPhone, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiCusPhone = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCustomerPhone)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisCustomerPhone - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiCusPhone);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiCusPhone);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisCustomerPhone - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiCusPhone);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisCustomerPhone || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiCusPhone, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiCusPhone, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiCusPhone);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiCusPhone);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisCustomerPhone();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisCustomerPhoneEntity> cisCustomerPhone = (from source in results.Skip(1)
                                                                         select new CisCustomerPhoneEntity
                                                                         {
                                                                             KKCisId = source[1].NullSafeTrim(),
                                                                             CustId = source[2].NullSafeTrim(),
                                                                             CardId = source[3].NullSafeTrim(),
                                                                             CardTypeCode = source[4].NullSafeTrim(),
                                                                             CustTypeGroup = source[5].NullSafeTrim(),
                                                                             PhoneTypeCode = source[6].NullSafeTrim(),
                                                                             PhoneNum = source[7].NullSafeTrim(),
                                                                             PhoneExt = source[8].NullSafeTrim(),
                                                                             CreateDate = source[9].NullSafeTrim(),
                                                                             CreateBy = source[10].NullSafeTrim(),
                                                                             UpdateDate = source[11].NullSafeTrim(),
                                                                             UpdateBy = source[12].NullSafeTrim()
                                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCustomerPhoneEntity CustomerPhone in cisCustomerPhone)
                        {
                            Error = ValidateMaxLength(CustomerPhone);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiCusPhone, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCusPhone);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiCusPhone);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfCusPhone = cisCustomerPhone.Count;
                        return cisCustomerPhone;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisCustomerPhone();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiCusPhone))
                {
                    MoveFileSource(filePath, fiCusPhone);
                }
            }
            return null;
        }

        private List<CisCustomerEmailEntity> ReadFileCisCustomerEmail(string filePath, string fiPrefix, ref int numOfCusEmail, ref string fiCusEmail, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiCusEmail = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCustomerEmail)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisCustomerEmail - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiCusEmail);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiCusEmail);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisCustomerEmail - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiCusEmail);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisCustomerEmail || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiCusEmail, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiCusEmail, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiCusEmail);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiCusEmail);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisCustomerEmail();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisCustomerEmailEntity> cisCustomerEmail = (from source in results.Skip(1)
                                                                         select new CisCustomerEmailEntity
                                                                         {
                                                                             KKCisId = source[1].NullSafeTrim(),
                                                                             CustId = source[2].NullSafeTrim(),
                                                                             CardId = source[3].NullSafeTrim(),
                                                                             CardTypeCode = source[4].NullSafeTrim(),
                                                                             CustTypeGroup = source[5].NullSafeTrim(),
                                                                             MailTypeCode = source[6].NullSafeTrim(),
                                                                             MailAccount = source[7].NullSafeTrim(),
                                                                             CreatedDate = source[8].NullSafeTrim(),
                                                                             CreatedBy = source[9].NullSafeTrim(),
                                                                             UpdatedDate = source[10].NullSafeTrim(),
                                                                             UpdatedBy = source[11].NullSafeTrim()
                                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCustomerEmailEntity CustomerEmail in cisCustomerEmail)
                        {
                            Error = ValidateMaxLength(CustomerEmail);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiCusEmail, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCusEmail);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiCusEmail);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfCusEmail = cisCustomerEmail.Count;
                        return cisCustomerEmail;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisCustomerEmail();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiCusEmail))
                {
                    MoveFileSource(filePath, fiCusEmail);
                }
            }
            return null;
        }

        private List<CisCountryEntity> ReadFileCisCountry(string filePath, string fiPrefix, ref int numOfCountry, ref string fiCountry, ref bool isValidHeader, ref string msgValidateFileError, ref string dataDate)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiCountry = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCountry)
                    {
                        dataDate = header[Constants.ImportCisData.LengthOfHeaderCisCountry - 2].NullSafeTrim(); // ref value

                        if (header[0] == Constants.ImportCisData.DataTypeHeader)
                        {
                            isValidFormat = true;
                        }
                        else
                        {
                            Logger.DebugFormat("File:{0} dataType of header mismatch", fiCountry);
                        }
                    }
                    else
                    {
                        Logger.DebugFormat("File:{0} length of header mismatch", fiCountry);
                    }


                    // Check TotalRecord
                    if (isValidFormat)
                    {
                        int? totalRecord = header[Constants.ImportCisData.LengthOfHeaderCisCountry - 1].ToNullable<int>();
                        int cntDetail = results.Skip(1).Count();

                        if (totalRecord != cntDetail)
                        {
                            isValidFormat = false;
                            Logger.DebugFormat("File:{0} TotalRecord mismatch", fiCountry);
                        }
                    }

                    if (isValidFormat)
                    {
                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfDetailCisCountry || source[0].NullSafeTrim() != Constants.ImportCisData.DataTypeDetail)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiCountry, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiCountry, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiCountry);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiCountry);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisCountry();
                        goto Outer;
                    }
                    #endregion

                    if (isValidFormat)
                    {
                        List<CisCountryEntity> cisCountry = (from source in results.Skip(1)
                                                             select new CisCountryEntity
                                                             {
                                                                 CountryCode = source[1].NullSafeTrim(),
                                                                 CountryNameTH = source[2].NullSafeTrim(),
                                                                 CountryNameEN = source[3].NullSafeTrim(),
                                                                 Status = source[4].NullSafeTrim()
                                                             }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        var lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCountryEntity countryEntity in cisCountry)
                        {
                            Error = ValidateMaxLength(countryEntity);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "@Line {0}: {1}", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength \n{1}", fiCountry, string.Join("\n", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCountry);
                            msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0} is Invalid MaxLength.", fiCountry);
                            isValidHeader = false; // ref value
                            goto Outer;
                        }

                        #endregion

                        numOfCountry = cisCountry.Count;
                        return cisCountry;
                    }
                }
                else
                {
                    msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " {0} File not found.", fiPrefix);
                    isValidHeader = false;
                }
            Outer:
                return null;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw new CustomException("{0}: {1}", fiPrefix, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                _cisDataAccess.DeleteCisCountry();
                isValidHeader = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(fiCountry))
                {
                    MoveFileSource(filePath, fiCountry);
                }
            }
            return null;
        }

        private static string ValidateMaxLength<T>(T obj)
        {
            ValidationContext vc = new ValidationContext(obj, null, null);
            var validationResults = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(obj, vc, validationResults, true);
            if (!valid)
            {
                return validationResults.Select(x => x.ErrorMessage).Aggregate((i, j) => i + ", " + j);
            }

            return null;
        }

        private void MoveFileSource(string filePath, string fileName)
        {
            string cisPathSource = this.GetParameter(Constants.ParameterName.CisPathSource);

            string filePathImport = filePath + @"\" + fileName;
            string fileSource = cisPathSource + @"\" + fileName;
            var copy = StreamDataHelpers.TryToCopy(filePathImport, fileSource);
            if (copy)
            {
                Logger.InfoFormat("-- Move File: {0} to Path: {1} --", filePathImport, fileSource);
                StreamDataHelpers.TryToDelete(filePathImport);
                Logger.InfoFormat("-- Delete File: {0} --", filePathImport);
            }
        }

        private void MoveFileError(string filePath, string fileName)
        {
            string cisPathError = this.GetParameter(Constants.ParameterName.CISPathError);

            string filePathImport = filePath + @"\" + fileName;
            string fileError = cisPathError + @"\" + fileName;
            var copy = StreamDataHelpers.TryToCopy(filePathImport, fileError);
            if (copy)
            {
                Logger.InfoFormat("-- Move File: {0} to Path: {1} --", filePathImport, fileError);
                //StreamDataHelpers.TryToDelete(filePathImport);
                //Logger.Info(string.Format("-- Delete File: {0} --", filePathImport)); 
            }
        }

        private bool SaveCisCorporate(List<CisCorporateEntity> cisCorporates, string fiCor)
        {
            if (!string.IsNullOrWhiteSpace(fiCor))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCorporate(cisCorporates);
            }

            return true;
        }

        private bool SaveCisCorporateComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCorporateComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisIndividual(List<CisIndividualEntity> cisIndividuals, string fiIndiv)
        {
            if (!string.IsNullOrWhiteSpace(fiIndiv))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisIndividual(cisIndividuals);
            }

            return true;
        }

        private bool SaveCisIndividualComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisIndividualComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisProductGroup(List<CisProductGroupEntity> cisProductGroup, string fiProd)
        {
            if (!string.IsNullOrWhiteSpace(fiProd))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisProductGroup(cisProductGroup);
            }

            return true;
        }

        private bool SaveCisSubscription(List<CisSubscriptionEntity> cisSubscription, string fiSub)
        {
            if (!string.IsNullOrWhiteSpace(fiSub))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscription(cisSubscription);
            }

            return true;
        }

        private bool SaveCisSubscriptionComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscriptionComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisTitle(List<CisTitleEntity> cisTitles, string fiTitle)
        {
            if (!string.IsNullOrEmpty(fiTitle))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisTitle(cisTitles);
            }
            return true;
        }

        private bool SaveCisProvince(List<CisProvinceEntity> cisProvinces, string fiProvince)
        {
            if (!string.IsNullOrEmpty(fiProvince))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisProvince(cisProvinces);
            }
            return true;
        }

        private bool SaveCisDistrict(List<CisDistrictEntity> cisDistricts, string fiDistrict)
        {
            if (!string.IsNullOrEmpty(fiDistrict))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisDistrict(cisDistricts);
            }
            return true;
        }

        private bool SaveCisSubDistrict(List<CisSubDistrictEntity> cisSubDistricts, string fiSubDistrict)
        {
            if (!string.IsNullOrEmpty(fiSubDistrict))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubDistrict(cisSubDistricts);
            }
            return true;
        }

        private bool SaveCisPhoneType(List<CisPhoneTypeEntity> cisPhones, string fiPhoneType)
        {
            if (!string.IsNullOrEmpty(fiPhoneType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisPhoneType(cisPhones);
            }
            return true;
        }

        private bool SaveCisEmailType(List<CisEmailTypeEntity> cisEmails, string fiEmailType)
        {
            if (!string.IsNullOrEmpty(fiEmailType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisEmailType(cisEmails);
            }
            return true;
        }

        private bool SaveCisSubscribeAddress(List<CisSubscribeAddressEntity> cisSubscribeAdds, string fiSubAdds)
        {
            if (!string.IsNullOrEmpty(fiSubAdds))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribeAddress(cisSubscribeAdds);
            }
            return true;
        }

        private bool SaveCisSubscribePhone(List<CisSubscribePhoneEntity> cisSubscribePhones, string fiSubPhone)
        {
            if (!string.IsNullOrEmpty(fiSubPhone))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribePhone(cisSubscribePhones);
            }
            return true;
        }

        private bool SaveCisSubscribeEmail(List<CisSubscribeMailEntity> cisSubscribeMails, string fiSubEmail)
        {
            if (!string.IsNullOrEmpty(fiSubEmail))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribeEmail(cisSubscribeMails);
            }
            return true;
        }

        private bool SaveCisSubscribeAddressComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribeAddressComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisSubscribePhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribePhoneComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisSubscribeEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribeEmailComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisAddressType(List<CisAddressTypeEntity> cisAddresstypes, string fiAddtype)
        {
            if (!string.IsNullOrEmpty(fiAddtype))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisAddressType(cisAddresstypes);
            }
            return true;
        }

        private bool SaveCisSubscriptionType(List<CisSubscriptionTypeEntity> cisSubscriptionType, string fiSubType)
        {
            if (!string.IsNullOrWhiteSpace(fiSubType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscriptionType(cisSubscriptionType);
            }

            return true;
        }

        private bool SaveCisSubscriptionTypeComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscriptionTypeComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisCustomerPhone(List<CisCustomerPhoneEntity> cisCustomerPhone, string fiCusPhone)
        {
            if (!string.IsNullOrEmpty(fiCusPhone))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCustomerPhone(cisCustomerPhone);
            }
            return true;
        }

        private bool SaveCisCustomerPhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCustomerPhoneComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisCustomerEmail(List<CisCustomerEmailEntity> cisCustomerEmail, string fiCusEmail)
        {
            if (!string.IsNullOrEmpty(fiCusEmail))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCustomerEmail(cisCustomerEmail);
            }
            return true;
        }

        private bool SaveCisCustomerEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCustomerEmailComplete(ref numOfComplete, ref numOfError, ref messageError);
        }

        private bool SaveCisCountry(List<CisCountryEntity> cisCountry, string fiCountry)
        {
            if (!string.IsNullOrEmpty(fiCountry))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCountry(cisCountry);
            }
            return true;
        }

        private bool ExportIndividualCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var individualList = _cisDataAccess.GetCISIndivExport();
            return ExportIndividualCIS(filePath, fileName, individualList, dataDate);
        }

        private static bool ExportIndividualCIS(string filePath, string fileName, List<CisIndividualEntity> indivexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
                dt.Columns.Add("TITLE_ID", typeof(string));
                dt.Columns.Add("TITLE_NAME_CUSTOM", typeof(string));
                dt.Columns.Add("FIRST_NAME_TH", typeof(string));
                dt.Columns.Add("MID_NAME_TH", typeof(string));
                dt.Columns.Add("LAST_NAME_TH", typeof(string));
                dt.Columns.Add("FIRST_NAME_EN", typeof(string));
                dt.Columns.Add("MID_NAME_EN", typeof(string));
                dt.Columns.Add("LAST_NAME_EN", typeof(string));
                dt.Columns.Add("BIRTH_DATE", typeof(string));
                dt.Columns.Add("GENDER_CODE", typeof(string));
                dt.Columns.Add("MARITAL_CODE", typeof(string));
                dt.Columns.Add("NATIONALITY1_CODE", typeof(string));
                dt.Columns.Add("NATIONALITY2_CODE", typeof(string));
                dt.Columns.Add("NATIONALITY3_CODE", typeof(string));
                dt.Columns.Add("RELIGION_CODE", typeof(string));
                dt.Columns.Add("EDUCATION_CODE", typeof(string));
                dt.Columns.Add("POSITION", typeof(string));
                dt.Columns.Add("BUSINESS_CODE", typeof(string));
                dt.Columns.Add("COMPANY_NAME", typeof(string));
                dt.Columns.Add("ISIC_CODE", typeof(string));
                dt.Columns.Add("ANNUAL_INCOME", typeof(string));
                dt.Columns.Add("SOURCE_INCOME", typeof(string));
                dt.Columns.Add("TOTAL_WEALTH_PERIOD", typeof(string));
                dt.Columns.Add("FLG_MST_APP", typeof(string));
                dt.Columns.Add("CHANNEL_HOME", typeof(string));
                dt.Columns.Add("FIRST_BRANCH", typeof(string));
                dt.Columns.Add("SHARE_INFO_FLAG", typeof(string));
                dt.Columns.Add("PLACE_CUST_UPDATED", typeof(string));
                dt.Columns.Add("DATE_CUST_UPDATED", typeof(string));
                dt.Columns.Add("ANNUAL_INCOME_PERIOD", typeof(string));
                dt.Columns.Add("MARKETING_ID", typeof(string));
                dt.Columns.Add("NUMBER_OF_EMPLOYEE", typeof(string));
                dt.Columns.Add("FIXED_ASSET", typeof(string));
                dt.Columns.Add("FIXED_ASSET_EXCLUDE_LAND", typeof(string));
                dt.Columns.Add("OCCUPATION_CODE", typeof(string));
                dt.Columns.Add("OCCUPATION_SUBTYPE_CODE", typeof(string));
                dt.Columns.Add("COUNTRY_INCOME", typeof(string));
                dt.Columns.Add("TOTAL_WEALTH", typeof(string));
                dt.Columns.Add("SOURCE_INCOME_REM", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in indivexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardtypeCode,
                                x.CusttypeCode,
                                x.CusttypeGroup,
                                x.TitleId,
                                x.TitlenameCustom,
                                x.FirstnameTh,
                                x.MidnameTh,
                                x.LastnameTh,
                                x.FirstnameEn,
                                x.MidnameEn,
                                x.LastnameEn,
                                x.BirthDate,
                                x.GenderCode,
                                x.MaritalCode,
                                x.Nationality1Code,
                                x.Nationality2Code,
                                x.Nationality3Code,
                                x.ReligionCode,
                                x.EducationCode,
                                x.Position,
                                x.BusinessCode,
                                x.CompanyName,
                                x.IsicCode,
                                x.AnnualIncome,
                                x.SourceIncome,
                                x.TotalwealthPeriod,
                                x.FlgmstApp,
                                x.ChannelHome,
                                x.FirstBranch,
                                x.ShareinfoFlag,
                                x.PlacecustUpdated,
                                x.DatecustUpdated,
                                x.AnnualincomePeriod,
                                x.MarketingId,
                                x.NumberofEmployee,
                                x.FixedAsset,
                                x.FixedassetExcludeland,
                                x.OccupationCode,
                                x.OccupationsubtypeCode,
                                x.CountryIncome,
                                x.TotalwealTh,
                                x.SourceIncomerem,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.UpdateDate,
                                x.UpdatedBy,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportCorporateCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var corporateList = _cisDataAccess.GetCISCorExport();
            return ExportCorporateCIS(filePath, fileName, corporateList, dataDate);
        }

        private static bool ExportCorporateCIS(string filePath, string fileName, List<CisCorporateEntity> corexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
                dt.Columns.Add("TITLE_ID", typeof(string));
                dt.Columns.Add("NAME_TH", typeof(string));
                dt.Columns.Add("NAME_EN", typeof(string));
                dt.Columns.Add("ISIC_CODE", typeof(string));
                dt.Columns.Add("TAX_ID", typeof(string));
                dt.Columns.Add("HOST_BUSINESS_COUNTRY_CODE", typeof(string));
                dt.Columns.Add("VALUE_PER_SHARE", typeof(string));
                dt.Columns.Add("AUTHORIZED_SHARE_CAPITAL", typeof(string));
                dt.Columns.Add("REGISTER_DATE", typeof(string));
                dt.Columns.Add("BUSINESS_CODE", typeof(string));
                dt.Columns.Add("FIXED_ASSET", typeof(string));
                dt.Columns.Add("FIXED_ASSET_EXCLUDE_LAND", typeof(string));
                dt.Columns.Add("NUMBER_OF_EMPLOYEE", typeof(string));
                dt.Columns.Add("SHARE_INFO_FLAG", typeof(string));
                dt.Columns.Add("FLG_MST_APP", typeof(string));
                dt.Columns.Add("FIRST_BRANCH", typeof(string));
                dt.Columns.Add("PLACE_CUST_UPDATED", typeof(string));
                dt.Columns.Add("DATE_CUST_UPDATED", typeof(string));
                dt.Columns.Add("ID_COUNTRY_ISSUE", typeof(string));
                dt.Columns.Add("BUSINESS_CAT_CODE", typeof(string));
                dt.Columns.Add("MARKETING_ID", typeof(string));
                dt.Columns.Add("STOCK", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in corexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardTypeCode,
                                x.CustTypeCode,
                                x.CustTypeGroup,
                                x.TitleId,
                                x.NameTh,
                                x.NameEn,
                                x.IsicCode,
                                x.TaxId,
                                x.HostBusinessCountryCode,
                                x.ValuePerShare,
                                x.AuthorizedShareCapital,
                                x.RegisterDate,
                                x.BusinessCode,
                                x.FixedAsset,
                                x.FixedAssetexcludeLand,
                                x.NumberOfEmployee,
                                x.ShareInfoFlag,
                                x.FlgmstApp,
                                x.FirstBranch,
                                x.PlaceCustUpdated,
                                x.DateCustUpdated,
                                x.IdCountryIssue,
                                x.BusinessCatCode,
                                x.MarketingId,
                                x.Stock,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.UpdatedDate,
                                x.UpdatedBy,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }

                    //sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportSubscriptionCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var subscriptionList = _cisDataAccess.GetCisSubscriptionExport();
            return ExportSubscriptionCIS(filePath, fileName, subscriptionList, dataDate);
        }

        private static bool ExportSubscriptionCIS(string filePath, string fileName, List<CisSubscriptionEntity> subexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("PROD_GROUP", typeof(string));
                dt.Columns.Add("PROD_TYPE", typeof(string));
                dt.Columns.Add("SUBSCR_CODE", typeof(string));
                dt.Columns.Add("REF_NO", typeof(string));
                dt.Columns.Add("BRANCH_NAME", typeof(string));
                dt.Columns.Add("TEXT1", typeof(string));
                dt.Columns.Add("TEXT2", typeof(string));
                dt.Columns.Add("TEXT3", typeof(string));
                dt.Columns.Add("TEXT4", typeof(string));
                dt.Columns.Add("TEXT5", typeof(string));
                dt.Columns.Add("TEXT6", typeof(string));
                dt.Columns.Add("TEXT7", typeof(string));
                dt.Columns.Add("TEXT8", typeof(string));
                dt.Columns.Add("TEXT9", typeof(string));
                dt.Columns.Add("TEXT10", typeof(string));
                dt.Columns.Add("NUMBER1", typeof(string));
                dt.Columns.Add("NUMBER2", typeof(string));
                dt.Columns.Add("NUMBER3", typeof(string));
                dt.Columns.Add("NUMBER4", typeof(string));
                dt.Columns.Add("NUMBER5", typeof(string));
                dt.Columns.Add("DATE1", typeof(string));
                dt.Columns.Add("DATE2", typeof(string));
                dt.Columns.Add("DATE3", typeof(string));
                dt.Columns.Add("DATE4", typeof(string));
                dt.Columns.Add("DATE5", typeof(string));
                dt.Columns.Add("SUBSCR_STATUS", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("CREATED_CHANEL", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_CHANNEL", typeof(string));
                dt.Columns.Add("SYSCUSTSUBSCR_ID", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in subexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardTypeCode,
                                x.ProdGroup,
                                x.ProdType,
                                x.SubscrCode,
                                x.RefNo,
                                x.BranchName,
                                x.Text1,
                                x.Text2,
                                x.Text3,
                                x.Text4,
                                x.Text5,
                                x.Text6,
                                x.Text7,
                                x.Text8,
                                x.Text9,
                                x.Text10,
                                x.Number1,
                                x.Number2,
                                x.Number3,
                                x.Number4,
                                x.Number5,
                                x.Date1,
                                x.Date2,
                                x.Date3,
                                x.Date4,
                                x.Date5,
                                x.SubscrStatus,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.CreatedChanel,
                                x.UpdatedDate,
                                x.UpdatedBy,
                                x.UpdatedChannel,
                                x.SysCustSubscrId,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportSubscribeAddressCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var addressList = _cisDataAccess.GetCisSubscribeAddressExport();
            return ExportSubscribeAddressCIS(filePath, fileName, addressList, dataDate);
        }

        private static bool ExportSubscribeAddressCIS(string filePath, string fileName, List<CisSubscribeAddressEntity> addreexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                //dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
                dt.Columns.Add("PROD_GROUP", typeof(string));
                dt.Columns.Add("PROD_TYPE", typeof(string));
                dt.Columns.Add("SUBSCR_CODE", typeof(string));
                dt.Columns.Add("ADDRESS_TYPE_CODE", typeof(string));
                dt.Columns.Add("ADDRESS_NUMBER", typeof(string));
                dt.Columns.Add("VILLAGE", typeof(string));
                dt.Columns.Add("BUILDING", typeof(string));
                dt.Columns.Add("FLOOR_NO", typeof(string));
                dt.Columns.Add("ROOM_NO", typeof(string));
                dt.Columns.Add("MOO", typeof(string));
                dt.Columns.Add("STREET", typeof(string));
                dt.Columns.Add("SOI", typeof(string));
                dt.Columns.Add("SUB_DISTRICT_CODE", typeof(string));
                dt.Columns.Add("DISTRICT_CODE", typeof(string));
                dt.Columns.Add("PROVINCE_CODE", typeof(string));
                dt.Columns.Add("COUNTRY_CODE", typeof(string));
                dt.Columns.Add("POSTAL_CODE", typeof(string));
                dt.Columns.Add("SUB_DISTRICT_VALUE", typeof(string));
                dt.Columns.Add("DISTRICT_VALUE", typeof(string));
                dt.Columns.Add("PROVINCE_VALUE", typeof(string));
                dt.Columns.Add("POSTAL_VALUE", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("SYSCUSTSUBSCR_ID", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in addreexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardTypeCode, 
                                //x.CustTypeGroup, 
                                x.ProdGroup,
                                x.ProdType,
                                x.SubscrCode,
                                x.AddressTypeCode,
                                x.AddressNumber,
                                x.Village,
                                x.Building,
                                x.FloorNo,
                                x.RoomNo,
                                x.Moo,
                                x.Street,
                                x.Soi,
                                x.SubDistrictCode,
                                x.DistrictCode,
                                x.ProvinceCode,
                                x.CountryCode,
                                x.PostalCode,
                                x.SubDistrictValue,
                                x.DistrictValue,
                                x.ProvinceValue,
                                x.PostalValue,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.UpdatedDate,
                                x.UpdatedBy,
                                x.SysCustSubscrId,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportSubscribePhoneCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var phoneList = _cisDataAccess.GetCisSubscribePhoneExport();
            return ExportSubscribePhoneCIS(filePath, fileName, phoneList, dataDate);
        }

        private static bool ExportSubscribePhoneCIS(string filePath, string fileName, List<CisSubscribePhoneEntity> phoneexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("PROD_GROUP", typeof(string));
                dt.Columns.Add("PROD_TYPE", typeof(string));
                dt.Columns.Add("SUBSCR_CODE", typeof(string));
                dt.Columns.Add("PHONE_TYPE_CODE", typeof(string));
                dt.Columns.Add("PHONE_NUM", typeof(string));
                dt.Columns.Add("PHONE_EXT", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("SYSCUSTSUBSCR_ID", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in phoneexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardTypeCode,
                                x.ProdGroup,
                                x.ProdType,
                                x.SubscrCode,
                                x.PhoneTypeCode,
                                x.PhoneNum,
                                x.PhoneExt,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.UpdatedDate,
                                x.UpdatedBy,
                                x.SysCustSubscrId,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportSubscribeEmailCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var emailList = _cisDataAccess.GetCisSubscriptEmailExport();
            return ExportSubscribeEmailCIS(filePath, fileName, emailList, dataDate);
        }

        private static bool ExportSubscribeEmailCIS(string filePath, string fileName, List<CisSubscribeMailEntity> emailexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("PROD_GROUP", typeof(string));
                dt.Columns.Add("PROD_TYPE", typeof(string));
                dt.Columns.Add("SUBSCR_CODE", typeof(string));
                dt.Columns.Add("MAIL_TYPE_CODE", typeof(string));
                dt.Columns.Add("MAILACCOUNT", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("SYSCUSTSUBSCR_ID", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in emailexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.KKCisId,
                                x.CustId,
                                x.CardId,
                                x.CardTypeCode,
                                x.ProdGroup,
                                x.ProdType,
                                x.SubscrCode,
                                x.MailTypeCode,
                                x.MailAccount,
                                x.CreatedDate,
                                x.CreatedBy,
                                x.UpdatedDate,
                                x.UpdatedBy,
                                x.SysCustSubscrId,
                                x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportSubscriptionTypeCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var subscriptionTypeList = _cisDataAccess.GetCisSubscriptionTypeExport();
            return ExportSubscriptionTypeCIS(filePath, fileName, subscriptionTypeList, dataDate);
        }

        private static bool ExportSubscriptionTypeCIS(string filePath, string fileName, List<CisSubscriptionTypeEntity> subTypeexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("CustTypeGroup", typeof(string));
                dt.Columns.Add("CustTypeCode", typeof(string));
                dt.Columns.Add("CustTypeTh", typeof(string));
                dt.Columns.Add("CustTypeEn", typeof(string));
                dt.Columns.Add("CardTypeCode", typeof(string));
                dt.Columns.Add("CardTypeEn", typeof(string));
                dt.Columns.Add("CardTypeTh", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                dt.Columns.Add("Error", typeof(string));

                var result = from x in subTypeexport
                             select dt.LoadDataRow(new object[]
                             {
                                Constants.ImportCisData.DataTypeDetail,
                                x.CustTypeGroup,
                                x.CustTypeCode,
                                x.CustTypeTh,
                                x.CustTypeEn,
                                x.CardTypeCode,
                                x.CardTypeEn,
                                x.CardTypeTh,
                                x.Status,
                                x.Error
                             }, false);


                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportCustomerPhoneCIS(string filePath, string fileName, string dataDate)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var customerphoneList = _cisDataAccess.GetCisCustomerPhoneExport();
            return ExportCustomerPhoneCIS(filePath, fileName, customerphoneList, dataDate);
        }

        private static bool ExportCustomerPhoneCIS(string filePath, string fileName, List<CisCustomerPhoneEntity> customerPhoneexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
                dt.Columns.Add("PHONE_TYPE_CODE", typeof(string));
                dt.Columns.Add("PHONE_NUM", typeof(string));
                dt.Columns.Add("PHONE_EXT", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in customerPhoneexport
                             select dt.LoadDataRow(new object[]
                             {
                                 Constants.ImportCisData.DataTypeDetail,
                                 x.KKCisId,
                                 x.CustId,
                                 x.CardId,
                                 x.CardTypeCode,
                                 x.CustTypeGroup,
                                 x.PhoneTypeCode,
                                 x.PhoneNum,
                                 x.PhoneExt,
                                 x.CreateDate,
                                 x.CreateBy,
                                 x.UpdateDate,
                                 x.UpdateBy,
                                 x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        private bool ExportCustomerEmailCIS(string filePath, string fileName, string dataDate)
        {
            if (_context == null)
            {
                _context = new CSMContext();
            }
            _cisDataAccess = new CisDataAccess(_context);
            var customeremailList = _cisDataAccess.GetCisCustomerEmailExport();
            return ExportCustomerEmailCIS(filePath, fileName, customeremailList, dataDate);
        }

        private static bool ExportCustomerEmailCIS(string filePath, string fileName, List<CisCustomerEmailEntity> customerEmailexport, string dataDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add(Constants.ImportCisData.DataTypeHeader, typeof(string));
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
                dt.Columns.Add("MAIL_TYPE_CODE", typeof(string));
                dt.Columns.Add("MAILACCOUNT", typeof(string));
                dt.Columns.Add("CREATED_DATE", typeof(string));
                dt.Columns.Add("CREATED_BY", typeof(string));
                dt.Columns.Add("UPDATED_DATE", typeof(string));
                dt.Columns.Add("UPDATED_BY", typeof(string));
                dt.Columns.Add("ERROR", typeof(string));

                var result = from x in customerEmailexport
                             select dt.LoadDataRow(new object[]
                             {
                                 Constants.ImportCisData.DataTypeDetail,
                                 x.KKCisId,
                                 x.CustId,
                                 x.CardId,
                                 x.CardTypeCode,
                                 x.CustTypeGroup,
                                 x.MailTypeCode,
                                 x.MailAccount,
                                 x.CreatedDate,
                                 x.CreatedBy,
                                 x.UpdatedDate,
                                 x.UpdatedBy,
                                 x.Error
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", filePath, fileName);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames) + string.Format(CultureInfo.InvariantCulture, "|{0}|{1}", dataDate, result.Count()));

                    foreach (DataRow row in result)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                        sw.WriteLine(string.Join("|", fields));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;

        }

        private void DeleteAllCisTableInterface()
        {
            _cisDataAccess = new CisDataAccess(_context);
            _cisDataAccess.DeleteCisCorporate();
            _cisDataAccess.DeleteCisIndividual();
            _cisDataAccess.DeleteCisSubscription();
            _cisDataAccess.DeleteCisSubscribeAddress();
            _cisDataAccess.DeleteCisSubscribePhone();
            _cisDataAccess.DeleteCisSubscribeEmail();
            _cisDataAccess.DeleteCisSubscriptionType();
            _cisDataAccess.DeleteCisCustomerPhone();
            _cisDataAccess.DeleteCisCustomerEmail();

            //_cisDataAccess.DeleteCisTitle();
            //_cisDataAccess.DeleteCisProductGroup();
            //_cisDataAccess.DeleteCisCountry();
            //_cisDataAccess.DeleteCisAddressType();
            //_cisDataAccess.DeleteCisProvince();
            //_cisDataAccess.DeleteCisDistrict();
            //_cisDataAccess.DeleteCisSubDistrict();
            //_cisDataAccess.DeleteCisPhoneType();
            //_cisDataAccess.DeleteCisEmailType();
        }

        private void SaveLogSuccessOrFail(ImportCISTaskResponse taskResponse, LogStatus logStatus)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.Append(taskResponse.ToString());
                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportCIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = logStatus;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }

        private void SaveLogError(ImportCISTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.AppendFormat("Error Message = {0}\n", taskResponse.StatusResponse.Description);

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportCIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
            }
        }

        private void SaveLogWithFileNotFound(ImportCISTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.AppendFormat("Message = {0}\n", "File Not Found");

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportCIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
            }
        }

        private void SaveLogWithInvalidFile(ImportCISTaskResponse taskResponse, string filesInvalid)
        {
            if (taskResponse != null)
            {
                StringBuilder sb = new StringBuilder("");
                sb.AppendFormat("วัน เวลาที่ run task scheduler = {0}\n",
                    taskResponse.SchedDateTime.FormatDateTime(Constants.DateTimeFormat.DefaultFullDateTime));
                sb.AppendFormat("ElapsedTime = {0} (ms)\n", taskResponse.ElapsedTime);
                sb.AppendFormat("Error Message = {0}", filesInvalid);

                _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = Constants.AuditAction.ImportCIS;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                _auditLog.Status = LogStatus.Fail;
                _auditLog.Detail = sb.ToString();
                _auditLog.CreateUserId = null;
                AppLog.AuditLog(_auditLog);
            }
        }

        /// <summary>
        /// This will list the contents of the current directory.
        /// </summary>
        private bool DownloadFilesViaFTP(string localPath, List<string> fiPrefix, string strDate)
        {
            try
            {
                bool isFileFound = false;
                string host = WebConfig.GetCisSshServer();
                int port = WebConfig.GetCisSshPort();
                string username = WebConfig.GetCisSshUsername();
                string password = WebConfig.GetCisSshPassword();
                string[] remoteDirectory = new string[] { WebConfig.GetCisSshMdmRemoteDir(), WebConfig.GetCisSshCustRemoteDir() }; // . always refers to the current directory.

                var fiPrefixWithDate = fiPrefix.Select(x => string.Format(CultureInfo.InvariantCulture, "{0}{1}", x, strDate).ToUpperInvariant());

                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();

                    foreach (var dir in remoteDirectory)
                    {
                        var files = sftp.ListDirectory(dir).Where(x => fiPrefixWithDate.Contains(Path.GetFileNameWithoutExtension(x.Name).ToUpperInvariant()));
                        isFileFound = (isFileFound || files.Any());

                        if (isFileFound)
                        {
                            // Download file to local via SFTP
                            foreach (var file in files)
                            {
                                DownloadFile(sftp, file, localPath);
                            }

                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").ToSuccessLogString());
                        }
                        else
                        {
                            Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("Error Message", "File Not Found").ToFailLogString());
                        }
                    }

                    sftp.Disconnect();
                }
                return isFileFound;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download Files Via FTP").Add("Error Message", ex.Message).ToInputLogString());
            }

            return false;
        }

        #region "Functions"

        private void DownloadFile(SftpClient client, SftpFile file, string directory)
        {
            try
            {
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("FileName", file.FullName).ToInputLogString());

                using (var fileStream = File.OpenWrite(Path.Combine(directory, file.Name)))
                {
                    client.DownloadFile(file.FullName, fileStream);
                    // fileStream.Close();
                }

                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").ToSuccessLogString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Download File").Add("Error Message", ex.Message).ToInputLogString());
                throw;
            }
        }

        private void ImportCisSendMail(ImportCISTaskResponse result)
        {
            try
            {
                _mailSender = CSMMailSender.GetCSMMailSender();


                if (Constants.StatusResponse.Failed.Equals(result.StatusResponse.Status))
                {
                    if (!string.IsNullOrWhiteSpace(result.StatusResponse.Description))
                    {
                        _mailSender.NotifyImportCISFailed(WebConfig.GetTaskEmailToAddress(), result.SchedDateTime,
                        result.StatusResponse.Description);
                    }
                    else
                    {
                        _mailSender.NotifyImportCISFailed(WebConfig.GetTaskEmailToAddress(), result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
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
