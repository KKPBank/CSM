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

namespace CSM.Business
{
    public class CisFacade : ICisFacade
    {
        private readonly CSMContext _context;
        private ICommonFacade _commonFacade;
        private ICisDataAccess _cisDataAccess;
        private AuditLogEntity _auditLog;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CisFacade));

        public CisFacade()
        {
            _context = new CSMContext();
        }

        public string GetParameter(string paramName)
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(paramName);
            return param != null ? param.ParamValue : string.Empty;
        }

        public List<CisCorporateEntity> ReadFileCisCorporate(string filePath, string fiPrefix, ref int numOfCor, ref string fiCor, ref bool isValidHeader, ref string msgValidateFileError)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiCor = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisCorporate)
                    {
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 32);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisCorporate)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiCor, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiCor, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiCor); //Move file
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiCor);
                        isValidHeader = false; // ref value
                        _cisDataAccess.DeleteCisCorporate();
                        goto Outer;
                    }
                    #endregion

                    List<CisCorporateEntity> cisCorporate = (from source in results.Skip(1)
                                                             select new CisCorporateEntity
                                                             {
                                                                 KKCisId = source[0].NullSafeTrim(),
                                                                 CustId = source[1].NullSafeTrim(),
                                                                 CardId = source[2].NullSafeTrim(),
                                                                 CardTypeCode = source[3].NullSafeTrim(),
                                                                 CustTypeCode = source[4].NullSafeTrim(),
                                                                 CustTypeGroup = source[5].NullSafeTrim(),
                                                                 TitleId = source[6].NullSafeTrim(),
                                                                 NameTh = source[7].NullSafeTrim(),
                                                                 NameEn = source[8].NullSafeTrim(),
                                                                 IsicCode = source[9].NullSafeTrim(),
                                                                 TaxId = source[10].NullSafeTrim(),
                                                                 HostBusinessCountryCode = source[11].NullSafeTrim(),
                                                                 ValuePerShare = source[12].NullSafeTrim(),
                                                                 AuthorizedShareCapital = source[13].NullSafeTrim(),
                                                                 RegisterDate = source[14].NullSafeTrim(),
                                                                 BusinessCode = source[15].NullSafeTrim(),
                                                                 FixedAsset = source[16].NullSafeTrim(),
                                                                 FixedAssetexcludeLand = source[17].NullSafeTrim(),
                                                                 NumberOfEmployee = source[18].NullSafeTrim(),
                                                                 ShareInfoFlag = source[19].NullSafeTrim(),
                                                                 FlgmstApp = source[20].NullSafeTrim(),
                                                                 FirstBranch = source[21].NullSafeTrim(),
                                                                 PlaceCustUpdated = source[22].NullSafeTrim(),
                                                                 DateCustUpdated = source[23].NullSafeTrim(),
                                                                 IdCountryIssue = source[24].NullSafeTrim(),
                                                                 BusinessCatCode = source[25].NullSafeTrim(),
                                                                 MarketingId = source[26].NullSafeTrim(),
                                                                 Stock = source[27].NullSafeTrim(),
                                                                 CreatedDate = source[28].NullSafeTrim(),
                                                                 CreatedBy = source[29].NullSafeTrim(),
                                                                 UpdatedDate = source[30].NullSafeTrim(),
                                                                 UpdatedBy = source[31].NullSafeTrim()
                                                             }).ToList();


                    #region "Validate MaxLength"
                    int inxErr = 2;
                    List<string> lstErrMaxLength = new List<string>();
                    foreach (CisCorporateEntity corporate in cisCorporate)
                    {
                        corporate.Error = ValidateMaxLength(corporate);
                        if (!string.IsNullOrEmpty(corporate.Error))
                        {
                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                        }
                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiCor, string.Join(",", lstErrMaxLength.ToArray()));
                        MoveFileError(filePath, fiCor);
                        Logger.DebugFormat("File:{0} Invalid MaxLength", fiCor);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisIndividualEntity> ReadFileCisIndividual(string filePath, string fiPrefix, ref int numOfIndiv, ref string fiIndiv, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 49);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisIndividual)
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
                                                                   KKCisId = source[0].NullSafeTrim(),
                                                                   CustId = source[1].NullSafeTrim(),
                                                                   CardId = source[2].NullSafeTrim(),
                                                                   CardtypeCode = source[3].NullSafeTrim(),
                                                                   CusttypeCode = source[4].NullSafeTrim(),
                                                                   CusttypeGroup = source[5].NullSafeTrim(),
                                                                   TitleId = source[6].NullSafeTrim(),
                                                                   TitlenameCustom = source[7].NullSafeTrim(),
                                                                   FirstnameTh = source[8].NullSafeTrim(),
                                                                   MidnameTh = source[9].NullSafeTrim(),
                                                                   LastnameTh = source[10].NullSafeTrim(),
                                                                   FirstnameEn = source[11].NullSafeTrim(),
                                                                   MidnameEn = source[12].NullSafeTrim(),
                                                                   LastnameEn = source[13].NullSafeTrim(),
                                                                   BirthDate = source[14].NullSafeTrim(),
                                                                   GenderCode = source[15].NullSafeTrim(),
                                                                   MaritalCode = source[16].NullSafeTrim(),
                                                                   Nationality1Code = source[17].NullSafeTrim(),
                                                                   Nationality2Code = source[18].NullSafeTrim(),
                                                                   Nationality3Code = source[19].NullSafeTrim(),
                                                                   ReligionCode = source[20].NullSafeTrim(),
                                                                   EducationCode = source[21].NullSafeTrim(),
                                                                   Position = source[22].NullSafeTrim(),
                                                                   BusinessCode = source[23].NullSafeTrim(),
                                                                   CompanyName = source[24].NullSafeTrim(),
                                                                   IsicCode = source[25].NullSafeTrim(),
                                                                   AnnualIncome = source[26].NullSafeTrim(),
                                                                   SourceIncome = source[27].NullSafeTrim(),
                                                                   TotalwealthPeriod = source[28].NullSafeTrim(),
                                                                   FlgmstApp = source[29].NullSafeTrim(),
                                                                   ChannelHome = source[30].NullSafeTrim(),
                                                                   FirstBranch = source[31].NullSafeTrim(),
                                                                   ShareinfoFlag = source[32].NullSafeTrim(),
                                                                   PlacecustUpdated = source[33].NullSafeTrim(),
                                                                   DatecustUpdated = source[34].NullSafeTrim(),
                                                                   AnnualincomePeriod = source[35].NullSafeTrim(),
                                                                   MarketingId = source[36].NullSafeTrim(),
                                                                   NumberofEmployee = source[37].NullSafeTrim(),
                                                                   FixedAsset = source[38].NullSafeTrim(),
                                                                   FixedassetExcludeland = source[39].NullSafeTrim(),
                                                                   OccupationCode = source[40].NullSafeTrim(),
                                                                   OccupationsubtypeCode = source[41].NullSafeTrim(),
                                                                   CountryIncome = source[42].NullSafeTrim(),
                                                                   TotalwealTh = source[43].NullSafeTrim(),
                                                                   SourceIncomerem = source[44].NullSafeTrim(),
                                                                   CreatedDate = source[45].NullSafeTrim(),
                                                                   CreatedBy = source[46].NullSafeTrim(),
                                                                   UpdateDate = source[47].NullSafeTrim(),
                                                                   UpdatedBy = source[48].NullSafeTrim()
                                                               }).ToList();

                    #region "Validate MaxLength"
                    int inxErr = 2;
                    List<string> lstErrMaxLength = new List<string>();
                    foreach (CisIndividualEntity individual in cisIndividual)
                    {
                        individual.Error = ValidateMaxLength(individual);
                        if (!string.IsNullOrEmpty(individual.Error))
                        {
                            lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                        }
                        inxErr++;
                    }

                    if (lstErrMaxLength.Count > 0)
                    {
                        Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiIndiv, string.Join(",", lstErrMaxLength.ToArray()));
                        MoveFileError(filePath, fiIndiv);
                        Logger.DebugFormat("File:{0} Invalid MaxLength", fiIndiv);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisProductGroupEntity> ReadFileCisProductGroup(string filePath, string fiPrefix, ref int numOfProd, ref string fiProd, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 8);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisProductGroup)
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
                                                                          ProductCode = source[0].NullSafeTrim(),
                                                                          ProductType = source[1].NullSafeTrim(),
                                                                          ProductDesc = source[2].NullSafeTrim(),
                                                                          SYSTEM = source[3].NullSafeTrim(),
                                                                          SubscrCode = source[4].NullSafeTrim(),
                                                                          SubscrDesc = source[5].NullSafeTrim(),
                                                                          ProductFlag = source[6].NullSafeTrim(),
                                                                          EntityCode = source[7].NullSafeTrim()
                                                                      }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisProductGroupEntity productGroup in cisProductGroup)
                        {
                            Error = ValidateMaxLength(productGroup);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiProd, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiProd);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiProd);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubscriptionEntity> ReadFileCisSubscription(string filePath, string fiPrefix, ref int numOfSub, ref string fiSub, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 34);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisSubscription)
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
                                                                           KKCisId = source[0].NullSafeTrim(),
                                                                           CustId = source[1].NullSafeTrim(),
                                                                           CardId = source[2].NullSafeTrim(),
                                                                           CardTypeCode = source[3].NullSafeTrim(),
                                                                           ProdGroup = source[4].NullSafeTrim(),
                                                                           ProdType = source[5].NullSafeTrim(),
                                                                           SubscrCode = source[6].NullSafeTrim(),
                                                                           RefNo = source[7].NullSafeTrim(),
                                                                           BranchName = source[8].NullSafeTrim(),
                                                                           Text1 = source[9].NullSafeTrim(),
                                                                           Text2 = source[10].NullSafeTrim(),
                                                                           Text3 = source[11].NullSafeTrim(),
                                                                           Text4 = source[12].NullSafeTrim(),
                                                                           Text5 = source[13].NullSafeTrim(),
                                                                           Text6 = source[14].NullSafeTrim(),
                                                                           Text7 = source[15].NullSafeTrim(),
                                                                           Text8 = source[16].NullSafeTrim(),
                                                                           Text9 = source[17].NullSafeTrim(),
                                                                           Text10 = source[18].NullSafeTrim(),
                                                                           Number1 = source[19].NullSafeTrim(),
                                                                           Number2 = source[20].NullSafeTrim(),
                                                                           Number3 = source[21].NullSafeTrim(),
                                                                           Number4 = source[22].NullSafeTrim(),
                                                                           Number5 = source[23].NullSafeTrim(),
                                                                           Date1 = source[24].NullSafeTrim(),
                                                                           Date2 = source[25].NullSafeTrim(),
                                                                           Date3 = source[26].NullSafeTrim(),
                                                                           Date4 = source[27].NullSafeTrim(),
                                                                           Date5 = source[28].NullSafeTrim(),
                                                                           SubscrStatus = source[29].NullSafeTrim(),
                                                                           CreatedDate = source[30].NullSafeTrim(),
                                                                           CreatedBy = source[31].NullSafeTrim(),
                                                                           CreatedChanel = source[32].NullSafeTrim(),
                                                                           UpdatedDate = source[33].NullSafeTrim(),
                                                                           UpdatedBy = source[34].NullSafeTrim(),
                                                                           UpdatedChannel = source[35].NullSafeTrim()
                                                                       }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscriptionEntity subScription in cisSubscription)
                        {
                            Error = ValidateMaxLength(subScription);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSub, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSub);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSub);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisTitleEntity> ReadFileCisTitle(string filePath, string fiPrefix, ref int numOfTi, ref string fiTi, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 5);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisTitle)
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
                                                             TitleID = source[0].NullSafeTrim(),
                                                             TitleNameTH = source[1].NullSafeTrim(),
                                                             TitleNameEN = source[2].NullSafeTrim(),
                                                             TitleTypeGroup = source[3].NullSafeTrim(),
                                                             GenderCode = source[4].NullSafeTrim(),
                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisTitleEntity Title in cisTitle)
                        {
                            Error = ValidateMaxLength(Title);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiTi, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiTi);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiTi);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisProvinceEntity> ReadFileCisProvince(string filePath, string fiPrefix, ref int numOfPro, ref string fiPro, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
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
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisProvince)
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
                                                                ProvinceCode = source[0].NullSafeTrim(),
                                                                ProvinceNameTH = source[1].NullSafeTrim(),
                                                                ProvinceNameEN = source[2].NullSafeTrim()
                                                            }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisProvinceEntity Province in cisProvince)
                        {
                            Error = ValidateMaxLength(Province);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiPro, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiPro);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiPro);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisDistrictEntity> ReadFileCisDistrict(string filePath, string fiPrefix, ref int numOfDis, ref string fiDis, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 4);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisDistrict)
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
                                                                   ProvinceCode = source[0].NullSafeTrim(),
                                                                   DistrictCode = source[1].NullSafeTrim(),
                                                                   DistrictNameTH = source[2].NullSafeTrim(),
                                                                   DistrictNameEN = source[3].NullSafeTrim()
                                                               }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisDistrictEntity District in cisDistrict)
                        {
                            Error = ValidateMaxLength(District);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiDis, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiDis);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiDis);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubDistrictEntity> ReadFileCisSubDistrict(string filePath, string fiPrefix, ref int numOfSubDis, ref string fiSubDis, ref bool isValidHeader, ref string msgValidateFileError)
        {
            try
            {
                _cisDataAccess = new CisDataAccess(_context);
                IEnumerable<string> files = Directory.EnumerateFiles(filePath, string.Format(CultureInfo.InvariantCulture, "{0}*.txt", fiPrefix)); // lazy file system lookup

                if (files.Any())
                {
                    fiSubDis = Path.GetFileName(files.First());
                    IEnumerable<string[]> results = StreamDataHelpers.ReadPipe(files.First());

                    #region "Validate file format"
                    bool isValidFormat = false;
                    string[] header = results.FirstOrDefault();
                    if (header.Length == Constants.ImportCisData.LengthOfHeaderCisSubDistrict)
                    {
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 5);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisSubDistrict)
                            {
                                lstLengthNotMatch.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inx.ToString(CultureInfo.InvariantCulture)));
                            }

                            inx++;
                        }

                        if (lstLengthNotMatch.Count > 0 && lstLengthNotMatch.Count <= 5000)
                        {
                            Logger.DebugFormat("File:{0} Invalid format @line[{1}]", fiSubDis, string.Join(",", lstLengthNotMatch.ToArray()));
                        }
                        else if (lstLengthNotMatch.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid format {1} records", fiSubDis, lstLengthNotMatch.Count.ToString(CultureInfo.InvariantCulture));
                        }

                        isValidFormat = (lstLengthNotMatch.Count == 0);
                    }

                    if (isValidFormat == false)
                    {
                        MoveFileError(filePath, fiSubDis);
                        msgValidateFileError = string.Format(CultureInfo.InvariantCulture, " File name : {0}  is invalid file format.", fiSubDis);
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
                                                                         DistrictCode = source[0].NullSafeTrim(),
                                                                         SubDistrictCode = source[1].NullSafeTrim(),
                                                                         SubDistrictNameTH = source[2].NullSafeTrim(),
                                                                         SubDistrictNameEN = source[3].NullSafeTrim(),
                                                                         PostCode = source[4].NullSafeTrim()
                                                                     }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubDistrictEntity SubDistrict in cisSubDistrict)
                        {
                            Error = ValidateMaxLength(SubDistrict);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSubDis, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubDis);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSubDis);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisPhoneTypeEntity> ReadFileCisPhoneType(string filePath, string fiPrefix, ref int numOfPhonetype, ref string fiPhonetype, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 2);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisPhoneType)
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
                                                                       PhoneTypecode = source[0].NullSafeTrim(),
                                                                       PhoneTypeDesc = source[1].NullSafeTrim()
                                                                   }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisPhoneTypeEntity Phonetype in cisPhonetype)
                        {
                            Error = ValidateMaxLength(Phonetype);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiPhonetype, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiPhonetype);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiPhonetype);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisEmailTypeEntity> ReadFileCisEmailType(string filePath, string fiPrefix, ref int numOfEmailtype, ref string fiEmailtype, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 2);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderEmailType)
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
                                                                     MailTypecode = source[0].NullSafeTrim(),
                                                                     MailTypeDesc = source[1].NullSafeTrim()
                                                                 }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisEmailTypeEntity Emailtype in cisEmailtype)
                        {
                            Error = ValidateMaxLength(Emailtype);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiEmailtype, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiEmailtype);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiEmailtype);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubscribeAddressEntity> ReadFileCisSubscribeAddress(string filePath, string fiPrefix, ref int numOfSubAdd, ref string fiSubAdd, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 29);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisSubscriptionAddress)
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
                                                                             KKCisId = source[0].NullSafeTrim(),
                                                                             CustId = source[1].ToNullable<int>(),
                                                                             CardId = source[2].NullSafeTrim(),
                                                                             CardTypeCode = source[3].NullSafeTrim(),
                                                                             //CustTypeGroup = source[4].ToString(),
                                                                             ProdGroup = source[4].NullSafeTrim(),
                                                                             ProdType = source[5].NullSafeTrim(),
                                                                             SubscrCode = source[6].NullSafeTrim(),
                                                                             AddressTypeCode = source[7].NullSafeTrim(),
                                                                             AddressNumber = source[8].NullSafeTrim(),
                                                                             Village = source[9].NullSafeTrim(),
                                                                             Building = source[10].NullSafeTrim(),
                                                                             FloorNo = source[11].NullSafeTrim(),
                                                                             RoomNo = source[12].NullSafeTrim(),
                                                                             Moo = source[13].NullSafeTrim(),
                                                                             Street = source[14].NullSafeTrim(),
                                                                             Soi = source[15].NullSafeTrim(),
                                                                             SubDistrictCode = source[16].NullSafeTrim(),
                                                                             DistrictCode = source[17].NullSafeTrim(),
                                                                             ProvinceCode = source[18].NullSafeTrim(),
                                                                             CountryCode = source[19].NullSafeTrim(),
                                                                             PostalCode = source[20].NullSafeTrim(),
                                                                             SubDistrictValue = source[21].NullSafeTrim(),
                                                                             DistrictValue = source[22].NullSafeTrim(),
                                                                             ProvinceValue = source[23].NullSafeTrim(),
                                                                             PostalValue = source[24].NullSafeTrim(),
                                                                             CreatedDate = source[25].NullSafeTrim(),
                                                                             CreatedBy = source[26].NullSafeTrim(),
                                                                             UpdatedDate = source[27].NullSafeTrim(),
                                                                             UpdatedBy = source[28].NullSafeTrim()
                                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribeAddressEntity Subaddress in cisSubaddress)
                        {
                            Error = ValidateMaxLength(Subaddress);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                //lstErrMaxLength.Add(string.Format("{0}", inxErr.ToString()));
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}({1})", inxErr.ToString(CultureInfo.InvariantCulture), Error));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSubAdd, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubAdd);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSubAdd);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubscribePhoneEntity> ReadFileCisSubscribePhone(string filePath, string fiPrefix, ref int numOfSubPhone, ref string fiSubPhone, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 14);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisSubscribePhone)
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
                                                                         KKCisId = source[0].NullSafeTrim(),
                                                                         CustId = source[1].NullSafeTrim(),
                                                                         CardId = source[2].NullSafeTrim(),
                                                                         CardTypeCode = source[3].NullSafeTrim(),
                                                                         ProdGroup = source[4].NullSafeTrim(),
                                                                         ProdType = source[5].NullSafeTrim(),
                                                                         SubscrCode = source[6].NullSafeTrim(),
                                                                         PhoneTypeCode = source[7].NullSafeTrim(),
                                                                         PhoneNum = source[8].NullSafeTrim(),
                                                                         PhoneExt = source[9].NullSafeTrim(),
                                                                         CreatedDate = source[10].NullSafeTrim(),
                                                                         CreatedBy = source[11].NullSafeTrim(),
                                                                         UpdatedDate = source[12].NullSafeTrim(),
                                                                         UpdatedBy = source[13].NullSafeTrim()
                                                                     }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribePhoneEntity Subphone in cisSubphone)
                        {
                            Error = ValidateMaxLength(Subphone);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSubPhone, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubPhone);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSubPhone);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubscribeMailEntity> ReadFileCisSubscribeMail(string filePath, string fiPrefix, ref int numOfSubMail, ref string fiSubMail, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 13);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisSubscribeMail)
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
                                                                        KKCisId = source[0].NullSafeTrim(),
                                                                        CustId = source[1].NullSafeTrim(),
                                                                        CardId = source[2].NullSafeTrim(),
                                                                        CardTypeCode = source[3].NullSafeTrim(),
                                                                        ProdGroup = source[4].NullSafeTrim(),
                                                                        ProdType = source[5].NullSafeTrim(),
                                                                        SubscrCode = source[6].NullSafeTrim(),
                                                                        MailTypeCode = source[7].NullSafeTrim(),
                                                                        MailAccount = source[8].NullSafeTrim(),
                                                                        CreatedDate = source[9].NullSafeTrim(),
                                                                        CreatedBy = source[10].NullSafeTrim(),
                                                                        UpdatedDate = source[11].NullSafeTrim(),
                                                                        UpdatedBy = source[12].NullSafeTrim()
                                                                    }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisSubscribeMailEntity Submail in cisSubmail)
                        {
                            Error = ValidateMaxLength(Submail);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSubMail, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubMail);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSubMail);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisAddressTypeEntity> ReadFileCisAddressType(string filePath, string fiPrefix, ref int numOfAddtype, ref string fiAddtype, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 2);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisAddressType)
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
                                                                      AddressTypeCode = source[0].NullSafeTrim(),
                                                                      AddressTypeDesc = source[1].NullSafeTrim()
                                                                  }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisAddressTypeEntity AddType in cisAddType)
                        {
                            Error = ValidateMaxLength(AddType);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiAddtype, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiAddtype);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiAddtype);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisSubscriptionTypeEntity> ReadFileCisSubscriptionType(string filePath, string fiPrefix, ref int numOfSubType, ref string fiSubType, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 7);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisCisSubscriptionType)
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
                                                                                   CustTypeGroup = source[0].NullSafeTrim(),
                                                                                   CustTypeCode = source[1].NullSafeTrim(),
                                                                                   CustTypeTh = source[2].NullSafeTrim(),
                                                                                   CustTypeEn = source[3].NullSafeTrim(),
                                                                                   CardTypeCode = source[4].NullSafeTrim(),
                                                                                   CardTypeEn = source[5].NullSafeTrim(),
                                                                                   CardTypeTh = source[6].NullSafeTrim(),
                                                                               }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        foreach (CisSubscriptionTypeEntity SubscriptionType in cisSubscriptionType)
                        {
                            SubscriptionType.Error = ValidateMaxLength(SubscriptionType);
                            if (!string.IsNullOrEmpty(SubscriptionType.Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiSubType, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiSubType);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiSubType);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisCustomerPhoneEntity> ReadFileCisCustomerPhone(string filePath, string fiPrefix, ref int numOfCusPhone, ref string fiCusPhone, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 12);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisCustomerPhone)
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
                                                                                KKCisId = source[0].NullSafeTrim(),
                                                                                CustId = source[1].NullSafeTrim(),
                                                                                CardId = source[2].NullSafeTrim(),
                                                                                CardTypeCode = source[3].NullSafeTrim(),
                                                                                CustTypeGroup = source[4].NullSafeTrim(),
                                                                                PhoneTypeCode = source[5].NullSafeTrim(),
                                                                                PhoneNum = source[6].NullSafeTrim(),
                                                                                PhoneExt = source[7].NullSafeTrim(),
                                                                                CreateDate = source[8].NullSafeTrim(),
                                                                                CreateBy = source[9].NullSafeTrim(),
                                                                                UpdateDate = source[10].NullSafeTrim(),
                                                                                UpdateBy = source[11].NullSafeTrim()
                                                                            }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCustomerPhoneEntity CustomerPhone in cisCustomerPhone)
                        {
                            Error = ValidateMaxLength(CustomerPhone);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiCusPhone, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCusPhone);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiCusPhone);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisCustomerEmailEntity> ReadFileCisCustomerEmail(string filePath, string fiPrefix, ref int numOfCusEmail, ref string fiCusEmail, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
                    }

                    if (isValidFormat)
                    {
                        // Validate Body
                        //int cntBody = results.Skip(1).Count(x => x.Length != 11);
                        //isValidFormat = (cntBody == 0);

                        int inx = 2;
                        List<string> lstLengthNotMatch = new List<string>();
                        foreach (var source in results.Skip(1))
                        {
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisCustomerEmail)
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
                                                                             KKCisId = source[0].NullSafeTrim(),
                                                                             CustId = source[1].NullSafeTrim(),
                                                                             CardId = source[2].NullSafeTrim(),
                                                                             CardTypeCode = source[3].NullSafeTrim(),
                                                                             CustTypeGroup = source[4].NullSafeTrim(),
                                                                             MailTypeCode = source[5].NullSafeTrim(),
                                                                             MailAccount = source[6].NullSafeTrim(),
                                                                             CreatedDate = source[7].NullSafeTrim(),
                                                                             CreatedBy = source[8].NullSafeTrim(),
                                                                             UpdatedDate = source[9].NullSafeTrim(),
                                                                             UpdatedBy = source[10].NullSafeTrim()
                                                                         }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCustomerEmailEntity CustomerEmail in cisCustomerEmail)
                        {
                            Error = ValidateMaxLength(CustomerEmail);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiCusEmail, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCusEmail);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiCusEmail);
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
                    goto Outer;
                }
            Outer:
                return null;
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
        public List<CisCountryEntity> ReadFileCisCountry(string filePath, string fiPrefix, ref int numOfCountry, ref string fiCountry, ref bool isValidHeader, ref string msgValidateFileError)
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
                        isValidFormat = true;
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
                            if (source.Length != Constants.ImportCisData.LengthOfHeaderCisCountry)
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
                                                                 CountryCode = source[0].NullSafeTrim(),
                                                                 CountryNameTH = source[1].NullSafeTrim(),
                                                                 CountryNameEN = source[2].NullSafeTrim()
                                                             }).ToList();

                        #region "Validate MaxLength"
                        int inxErr = 2;
                        List<string> lstErrMaxLength = new List<string>();
                        string Error = "";
                        foreach (CisCountryEntity countryEntity in cisCountry)
                        {
                            Error = ValidateMaxLength(countryEntity);
                            if (!string.IsNullOrEmpty(Error))
                            {
                                lstErrMaxLength.Add(string.Format(CultureInfo.InvariantCulture, "{0}", inxErr.ToString(CultureInfo.InvariantCulture)));
                            }
                            inxErr++;
                        }

                        if (lstErrMaxLength.Count > 0)
                        {
                            Logger.DebugFormat("File:{0} Invalid MaxLength @line[{1}]", fiCountry, string.Join(",", lstErrMaxLength.ToArray()));
                            MoveFileError(filePath, fiCountry);
                            Logger.DebugFormat("File:{0} Invalid MaxLength", fiCountry);
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
                    goto Outer;
                }
            Outer:
                return null;
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
                return validationResults.Select(x => x.ErrorMessage).Aggregate((i, j) => i + Environment.NewLine + j);
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

        public bool SaveCisCorporate(List<CisCorporateEntity> cisCorporates, string fiCor)
        {
            if (!string.IsNullOrWhiteSpace(fiCor))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCorporate(cisCorporates);
            }

            return true;
        }
        public bool SaveCisCorporateComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCorporateComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisIndividual(List<CisIndividualEntity> cisIndividuals, string fiIndiv)
        {
            if (!string.IsNullOrWhiteSpace(fiIndiv))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisIndividual(cisIndividuals);
            }

            return true;
        }
        public bool SaveCisIndividualComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisIndividualComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisProductGroup(List<CisProductGroupEntity> cisProductGroup, string fiProd)
        {
            if (!string.IsNullOrWhiteSpace(fiProd))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisProductGroup(cisProductGroup);
            }

            return true;
        }
        public bool SaveCisSubscription(List<CisSubscriptionEntity> cisSubscription, string fiSub)
        {
            if (!string.IsNullOrWhiteSpace(fiSub))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscription(cisSubscription);
            }

            return true;
        }
        public bool SaveCisSubscriptionComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscriptionComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisTitle(List<CisTitleEntity> cisTitles, string fiTitle)
        {
            if (!string.IsNullOrEmpty(fiTitle))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisTitle(cisTitles);
            }
            return true;
        }
        public bool SaveCisProvince(List<CisProvinceEntity> cisProvinces, string fiProvince)
        {
            if (!string.IsNullOrEmpty(fiProvince))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisProvince(cisProvinces);
            }
            return true;
        }
        public bool SaveCisDistrict(List<CisDistrictEntity> cisDistricts, string fiDistrict)
        {
            if (!string.IsNullOrEmpty(fiDistrict))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisDistrict(cisDistricts);
            }
            return true;
        }
        public bool SaveCisSubDistrict(List<CisSubDistrictEntity> cisSubDistricts, string fiSubDistrict)
        {
            if (!string.IsNullOrEmpty(fiSubDistrict))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubDistrict(cisSubDistricts);
            }
            return true;
        }
        public bool SaveCisPhoneType(List<CisPhoneTypeEntity> cisPhones, string fiPhoneType)
        {
            if (!string.IsNullOrEmpty(fiPhoneType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisPhoneType(cisPhones);
            }
            return true;
        }
        public bool SaveCisEmailType(List<CisEmailTypeEntity> cisEmails, string fiEmailType)
        {
            if (!string.IsNullOrEmpty(fiEmailType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisEmailType(cisEmails);
            }
            return true;
        }
        public bool SaveCisSubscribeAddress(List<CisSubscribeAddressEntity> cisSubscribeAdds, string fiSubAdds)
        {
            if (!string.IsNullOrEmpty(fiSubAdds))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribeAddress(cisSubscribeAdds);
            }
            return true;
        }
        public bool SaveCisSubscribePhone(List<CisSubscribePhoneEntity> cisSubscribePhones, string fiSubPhone)
        {
            if (!string.IsNullOrEmpty(fiSubPhone))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribePhone(cisSubscribePhones);
            }
            return true;
        }
        public bool SaveCisSubscribeEmail(List<CisSubscribeMailEntity> cisSubscribeMails, string fiSubEmail)
        {
            if (!string.IsNullOrEmpty(fiSubEmail))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscribeEmail(cisSubscribeMails);
            }
            return true;
        }
        public bool SaveCisSubscribeAddressComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribeAddressComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisSubscribePhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribePhoneComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisSubscribeEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscribeEmailComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisAddressType(List<CisAddressTypeEntity> cisAddresstypes, string fiAddtype)
        {
            if (!string.IsNullOrEmpty(fiAddtype))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisAddressType(cisAddresstypes);
            }
            return true;
        }
        public bool SaveCisSubscriptionType(List<CisSubscriptionTypeEntity> cisSubscriptionType, string fiSubType)
        {
            if (!string.IsNullOrWhiteSpace(fiSubType))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisSubscriptionType(cisSubscriptionType);
            }

            return true;
        }
        public bool SaveCisSubscriptionTypeComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisSubscriptionTypeComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisCustomerPhone(List<CisCustomerPhoneEntity> cisCustomerPhone, string fiCusPhone)
        {
            if (!string.IsNullOrEmpty(fiCusPhone))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCustomerPhone(cisCustomerPhone);
            }
            return true;
        }
        public bool SaveCisCustomerPhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCustomerPhoneComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisCustomerEmail(List<CisCustomerEmailEntity> cisCustomerEmail, string fiCusEmail)
        {
            if (!string.IsNullOrEmpty(fiCusEmail))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCustomerEmail(cisCustomerEmail);
            }
            return true;
        }
        public bool SaveCisCustomerEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            _cisDataAccess = new CisDataAccess(_context);
            return _cisDataAccess.SaveCisCustomerEmailComplete(ref numOfComplete, ref numOfError, ref messageError);
        }
        public bool SaveCisCountry(List<CisCountryEntity> cisCountry, string fiCountry)
        {
            if (!string.IsNullOrEmpty(fiCountry))
            {
                _cisDataAccess = new CisDataAccess(_context);
                return _cisDataAccess.SaveCisCountry(cisCountry);
            }
            return true;
        }
        public bool ExportIndividualCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var individualList = _cisDataAccess.GetCISIndivExport();
            return ExportIndividualCIS(filePath, fileName, individualList);
        }
        private static bool ExportIndividualCIS(string filePath, string fileName, List<CisIndividualEntity> indivexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in indivexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                x.UpdatedBy 
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportCorporateCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var corporateList = _cisDataAccess.GetCISCorExport();
            return ExportCorporateCIS(filePath, fileName, corporateList);
        }
        private static bool ExportCorporateCIS(string filePath, string fileName, List<CisCorporateEntity> corexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in corexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                x.UpdatedBy 
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportSubscriptionCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var subscriptionList = _cisDataAccess.GetCisSubscriptionExport();
            return ExportSubscriptionCIS(filePath, fileName, subscriptionList);
        }
        private static bool ExportSubscriptionCIS(string filePath, string fileName, List<CisSubscriptionEntity> subexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("PROD_GROUP", typeof(string));
                dt.Columns.Add("PROD_TYPE", typeof(string));
                dt.Columns.Add("SUBSCR_CODE", typeof(string));
                dt.Columns.Add("REF_NO", typeof(string));
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

                var result = from x in subexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.KKCisId,  
                                x.CustId,   
                                x.CardId,   
                                x.CardTypeCode,  
                                x.ProdGroup,   
                                x.ProdType,  
                                x.SubscrCode,  
                                x.RefNo,  
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
                                x.UpdatedChannel
                             }, false);

                //KKCIS_ID|CUST_ID|CARD_ID|CARD_TYPE_CODE|PROD_GROUP|PROD_TYPE|SUBSCR_CODE|REF_NO|TEXT1|TEXT2|TEXT3|TEXT4|TEXT5|TEXT6|TEXT7|TEXT8|TEXT9|TEXT10|NUMBER1|NUMBER2|NUMBER3|NUMBER4|NUMBER5|DATE1|DATE2|DATE3|DATE4|DATE5|SUBSCR_STATUS|CREATED_DATE|CREATED_BY|CREATED_CHANEL|UPDATED_DATE|UPDATED_BY|UPDATED_CHANNEL

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportSubscribeAddressCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var addressList = _cisDataAccess.GetCisSubscribeAddressExport();
            return ExportSubscribeAddressCIS(filePath, fileName, addressList);
        }
        private static bool ExportSubscribeAddressCIS(string filePath, string fileName, List<CisSubscribeAddressEntity> addreexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("KKCIS_ID", typeof(string));
                dt.Columns.Add("CUST_ID", typeof(string));
                dt.Columns.Add("CARD_ID", typeof(string));
                dt.Columns.Add("CARD_TYPE_CODE", typeof(string));
                dt.Columns.Add("CUST_TYPE_GROUP", typeof(string));
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

                var result = from x in addreexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.KKCisId, 
                                x.CustId, 
                                x.CardId, 
                                x.CardTypeCode, 
                                x.CustTypeGroup, 
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
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportSubscribePhoneCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var phoneList = _cisDataAccess.GetCisSubscribePhoneExport();
            return ExportSubscribePhoneCIS(filePath, fileName, phoneList);
        }
        private static bool ExportSubscribePhoneCIS(string filePath, string fileName, List<CisSubscribePhoneEntity> phoneexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in phoneexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                x.UpdatedBy
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportSubscribeEmailCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var emailList = _cisDataAccess.GetCisSubscriptEmailExport();
            return ExportSubscribeEmailCIS(filePath, fileName, emailList);
        }
        private static bool ExportSubscribeEmailCIS(string filePath, string fileName, List<CisSubscribeMailEntity> emailexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in emailexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                x.UpdatedBy
                             }, false);

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                //const string dateStr = "20160613";
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportSubscriptionTypeCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var subscriptionTypeList = _cisDataAccess.GetCisSubscriptionTypeExport();
            return ExportSubscriptionTypeCIS(filePath, fileName, subscriptionTypeList);
        }
        private static bool ExportSubscriptionTypeCIS(string filePath, string fileName, List<CisSubscriptionTypeEntity> subTypeexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
                dt.Columns.Add("CustTypeGroup", typeof(string));
                dt.Columns.Add("CustTypeCode", typeof(string));
                dt.Columns.Add("CustTypeTh", typeof(string));
                dt.Columns.Add("CustTypeEn", typeof(string));
                dt.Columns.Add("CardTypeCode", typeof(string));
                dt.Columns.Add("CardTypeEn", typeof(string));
                dt.Columns.Add("CardTypeTh", typeof(string));

                var result = from x in subTypeexport
                             select dt.LoadDataRow(new object[]
                             {
                                x.CustTypeGroup,  
                                x.CustTypeCode,   
                                x.CustTypeTh,   
                                x.CustTypeEn,  
                                x.CardTypeCode,   
                                x.CardTypeEn,  
                                x.CardTypeTh,  
                           
                             }, false);

                //CustTypeGroup|CustTypeCode|CustTypeTh|CustTypeEn|CardTypeCode|CardTypeEn|CardTypeTh

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportCustomerPhoneCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var customerphoneList = _cisDataAccess.GetCisCustomerPhoneExport();
            return ExportCustomerPhoneCIS(filePath, fileName, customerphoneList);
        }
        private static bool ExportCustomerPhoneCIS(string filePath, string fileName, List<CisCustomerPhoneEntity> customerPhoneexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in customerPhoneexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                 x.UpdateBy  
                           
                             }, false);

                //KKCIS_ID|CUST_ID|CARD_ID|CARD_TYPE_CODE|CUST_TYPE_GROUP|PHONE_TYPE_CODE|PHONE_NUM|PHONE_EXT|CREATED_DATE|CREATED_BY|UPDATED_DATE|UPDATED_BY

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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
        public bool ExportCustomerEmailCIS(string filePath, string fileName)
        {
            _cisDataAccess = new CisDataAccess(_context);
            var customeremailList = _cisDataAccess.GetCisCustomerEmailExport();
            return ExportCustomerEmailCIS(filePath, fileName, customeremailList);
        }
        private static bool ExportCustomerEmailCIS(string filePath, string fileName, List<CisCustomerEmailEntity> customerEmailexport)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Locale = CultureInfo.CurrentCulture;
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

                var result = from x in customerEmailexport
                             select dt.LoadDataRow(new object[]
                             {
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
                                 x.UpdatedBy  
                           
                             }, false);

                //KKCIS_ID|CUST_ID|CARD_ID|CARD_TYPE_CODE|CUST_TYPE_GROUP|MAIL_TYPE_CODE|MAILACCOUNT|CREATED_DATE|CREATED_BY|UPDATED_DATE|UPDATED_BY

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                string dateStr = DateTime.Now.FormatDateTime("yyyyMMdd");
                string targetFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}_{2}.txt", filePath, fileName, dateStr);

                using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                {
                    sw.WriteLine(string.Join("|", columnNames));

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

        public void DeleteAllCisTableInterface()
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
        public void SaveLogSuccess(ImportCISTaskResponse taskResponse)
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
                _auditLog.Status = LogStatus.Success;
                _auditLog.Detail = sb.ToString();
                AppLog.AuditLog(_auditLog);
            }
        }
        public void SaveLogError(ImportCISTaskResponse taskResponse)
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
