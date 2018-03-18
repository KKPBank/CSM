using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSM.Entity;
using log4net;
using System.Linq;
using CSM.Common.Utilities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CSM.Data.DataAccess
{
    public class CisDataAccess : ICisDataAccess
    {
        private readonly CSMContext _context;
        //private object sync = new Object();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CisDataAccess));

        public CisDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.BatchCommandTimeout;
        }

        public int? GetCustomerIdByCisCode(string cisCode)
        {
            var query = _context.TB_M_CUSTOMER.Where(x => x.CARD_NO == cisCode);
            return query.Any() ? query.First().CUSTOMER_ID : new Nullable<int>();
        }

        public bool SaveCisCorporate(List<CisCorporateEntity> cisCorporates)
        {
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisCorporates != null && cisCorporates.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CORPORATE");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisCorporates.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstcisCorporate = from cisCorporate in cisCorporates.Skip(k * pageSize).Take(pageSize)
                                              select
                                              new TB_I_CIS_CORPORATE
                                              {
                                                  KKCIS_ID = cisCorporate.KKCisId,
                                                  CUST_ID = cisCorporate.CustId,
                                                  CARD_ID = cisCorporate.CardId,
                                                  CARD_TYPE_CODE = cisCorporate.CardTypeCode,
                                                  CUST_TYPE_CODE = cisCorporate.CustTypeCode,
                                                  CUST_TYPE_GROUP = cisCorporate.CustTypeGroup,
                                                  TITLE_ID = cisCorporate.TitleId,
                                                  NAME_TH = cisCorporate.NameTh,
                                                  NAME_EN = cisCorporate.NameEn,
                                                  ISIC_CODE = cisCorporate.IsicCode,
                                                  TAX_ID = cisCorporate.TaxId,
                                                  HOST_BUSINESS_COUNTRY_CODE = cisCorporate.HostBusinessCountryCode,
                                                  VALUE_PER_SHARE = cisCorporate.ValuePerShare,
                                                  AUTHORIZED_SHARE_CAPITAL = cisCorporate.AuthorizedShareCapital,
                                                  REGISTER_DATE = cisCorporate.RegisterDate,
                                                  BUSINESS_CODE = cisCorporate.BusinessCode,
                                                  FIXED_ASSET = cisCorporate.FixedAsset,
                                                  FIXED_ASSET_EXCLUDE_LAND = cisCorporate.FixedAssetexcludeLand,
                                                  NUMBER_OF_EMPLOYEE = cisCorporate.NumberOfEmployee,
                                                  SHARE_INFO_FLAG = cisCorporate.ShareInfoFlag,
                                                  FLG_MST_APP = cisCorporate.FlgmstApp,
                                                  FIRST_BRANCH = cisCorporate.FirstBranch,
                                                  PLACE_CUST_UPDATED = cisCorporate.PlaceCustUpdated,
                                                  DATE_CUST_UPDATED = cisCorporate.DateCustUpdated,
                                                  ID_COUNTRY_ISSUE = cisCorporate.IdCountryIssue,
                                                  BUSINESS_CAT_CODE = cisCorporate.BusinessCatCode,
                                                  MARKETING_ID = cisCorporate.MarketingId,
                                                  STOCK = cisCorporate.Stock,
                                                  CREATE_DATE = cisCorporate.CreatedDate,
                                                  CREATE_BY = cisCorporate.CreatedBy,
                                                  UPDATE_BY = cisCorporate.UpdatedBy,
                                                  UPDATE_DATE = cisCorporate.UpdatedDate,
                                                  ERROR = ValidateError(cisCorporate)

                                              };
                        //lock (sync)
                        //{
                        //    vc = new ValidationContext(lstcisCorporate, null, null);
                        //    var validationResults = new List<ValidationResult>();
                        //    bool valid = Validator.TryValidateObject(lstcisCorporate, vc, validationResults, true);
                        //    if (!valid)
                        //    {
                        //        //dbCorporate.ERROR =
                        //        lstcisCorporate.SingleOrDefault().ERROR =
                        //            validationResults.Select(x => x.ErrorMessage)
                        //                .Aggregate((i, j) => i + Environment.NewLine + j);
                        //    }
                        //}

                        DataTable dt = DataTableHelpers.ConvertTo(lstcisCorporate);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);
                        bc.DestinationTableName = "TB_I_CIS_CORPORATE";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);

                    })).Wait();

                    cisCorporates = null;

                    //foreach (CisCorporateEntity cisCorporate in cisCorporates)
                    //{
                    //    TB_I_CIS_CORPORATE dbCorporate = new TB_I_CIS_CORPORATE();

                    //    dbCorporate.CUST_ID = cisCorporate.CustId;
                    //    dbCorporate.CAR_ID = cisCorporate.CarId;
                    //    dbCorporate.TITLE_ID = cisCorporate.TitleId;
                    //    dbCorporate.NAME_TH = cisCorporate.NameTh;
                    //    dbCorporate.NAME_EN = cisCorporate.NameEn;
                    //    dbCorporate.TAX_ID = cisCorporate.TaxId;
                    //    dbCorporate.CUST_TYPE_GROUP = cisCorporate.CustTypeGroup;
                    //    dbCorporate.CUST_TYPE_CODE = cisCorporate.CustTypeCode;
                    //    dbCorporate.CARD_TYPE_CODE = cisCorporate.CardTypeCode;
                    //    dbCorporate.KKCIS_ID = cisCorporate.KKcisId;
                    //    dbCorporate.UPDATE_DATE = DateTime.Now.ToString();
                    //    dbCorporate.CUSTOMER_ID = null;
                    //    dbCorporate.REGISTER_DATE = cisCorporate.RegisDate;

                    //    vc = new ValidationContext(cisCorporate, null, null);
                    //    var validationResults = new List<ValidationResult>();
                    //    bool valid = Validator.TryValidateObject(cisCorporate, vc, validationResults, true);
                    //    if (!valid)
                    //    {
                    //        dbCorporate.ERROR =
                    //            validationResults.Select(x => x.ErrorMessage)
                    //                .Aggregate((i, j) => i + Environment.NewLine + j);
                    //    }

                    //    _context.TB_I_CIS_CORPORATE.Add(dbCorporate);
                    //}

                    this.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
        }

        public bool SaveCisCorporateComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_CORPORATE(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisIndividual(List<CisIndividualEntity> cisIndividuals)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //ValidationContext vc = null;
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisIndividuals != null && cisIndividuals.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_INDIVIDUAL");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisIndividuals.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstIndividual = from cisIndividual in cisIndividuals.Skip(k * pageSize).Take(pageSize)
                                            select
                                            new TB_I_CIS_INDIVIDUAL
                                            {
                                                KKCIS_ID = cisIndividual.KKCisId,
                                                CUST_ID = cisIndividual.CustId,
                                                CARD_ID = cisIndividual.CardId,
                                                CARD_TYPE_CODE = cisIndividual.CardtypeCode,
                                                CUST_TYPE_CODE = cisIndividual.CusttypeCode,
                                                CUST_TYPE_GROUP = cisIndividual.CusttypeGroup,
                                                TITLE_ID = cisIndividual.TitleId,
                                                TITLE_NAME_CUSTOM = cisIndividual.TitlenameCustom,
                                                FIRST_NAME_TH = cisIndividual.FirstnameTh,
                                                MID_NAME_TH = cisIndividual.MidnameTh,
                                                LAST_NAME_TH = cisIndividual.LastnameTh,
                                                FIRST_NAME_EN = cisIndividual.FirstnameEn,
                                                MID_NAME_EN = cisIndividual.MidnameEn,
                                                LAST_NAME_EN = cisIndividual.LastnameEn,
                                                BIRTH_DATE = cisIndividual.BirthDate,
                                                GENDER_CODE = cisIndividual.GenderCode,
                                                MARITAL_CODE = cisIndividual.MaritalCode,
                                                NATIONALITY1_CODE = cisIndividual.Nationality1Code,
                                                NATIONALITY2_CODE = cisIndividual.Nationality2Code,
                                                NATIONALITY3_CODE = cisIndividual.Nationality3Code,
                                                RELIGION_CODE = cisIndividual.ReligionCode,
                                                EDUCATION_CODE = cisIndividual.EducationCode,
                                                POSITION = cisIndividual.Position,
                                                BUSINESS_CODE = cisIndividual.BusinessCode,
                                                COMPANY_NAME = cisIndividual.CompanyName,
                                                ISIC_CODE = cisIndividual.IsicCode,
                                                ANNUAL_INCOME = cisIndividual.AnnualIncome,
                                                SOURCE_INCOME = cisIndividual.SourceIncome,
                                                TOTAL_WEALTH_PERIOD = cisIndividual.TotalwealthPeriod,
                                                FLG_MST_APP = cisIndividual.FlgmstApp,
                                                CHANNEL_HOME = cisIndividual.ChannelHome,
                                                FIRST_BRANCH = cisIndividual.FirstBranch,
                                                SHARE_INFO_FLAG = cisIndividual.ShareinfoFlag,
                                                PLACE_CUST_UPDATED = cisIndividual.PlacecustUpdated,
                                                DATE_CUST_UPDATED = cisIndividual.DatecustUpdated,
                                                ANNUAL_INCOME_PERIOD = cisIndividual.AnnualincomePeriod,
                                                MARKETING_ID = cisIndividual.MarketingId,
                                                NUMBER_OF_EMPLOYEE = cisIndividual.NumberofEmployee,
                                                FIXED_ASSET = cisIndividual.FixedAsset,
                                                FIXED_ASSET_EXCLUDE_LAND = cisIndividual.FixedassetExcludeland,
                                                OCCUPATION_CODE = cisIndividual.OccupationCode,
                                                OCCUPATION_SUBTYPE_CODE = cisIndividual.OccupationsubtypeCode,
                                                COUNTRY_INCOME = cisIndividual.CountryIncome,
                                                TOTAL_WEALTH = cisIndividual.TotalwealTh,
                                                SOURCE_INCOME_REM = cisIndividual.SourceIncomerem,
                                                CREATE_DATE = cisIndividual.CreatedDate,
                                                CREATE_BY = cisIndividual.CreatedBy,
                                                UPDATE_DATE = cisIndividual.UpdateDate,
                                                UPDATE_BY = cisIndividual.UpdatedBy,
                                                ERROR = ValidateError(cisIndividual)
                                            };

                        DataTable dt = DataTableHelpers.ConvertTo(lstIndividual);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);
                        bc.DestinationTableName = "TB_I_CIS_INDIVIDUAL";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);

                    })).Wait();

                    cisIndividuals = null;

                    //foreach (CisIndividualEntity cisIndividual in cisIndividuals)
                    //{
                    //    TB_I_CIS_INDIVIDUAL dbIndividual = new TB_I_CIS_INDIVIDUAL();

                    //    dbIndividual.CUST_ID = cisIndividual.CustId;
                    //    dbIndividual.CARD_ID = cisIndividual.CardId;
                    //    dbIndividual.FIRST_NAME_TH = cisIndividual.FirstNameTh;
                    //    dbIndividual.FIRST_NAME_EN = cisIndividual.FirstNameEn;
                    //    dbIndividual.LAST_NAME_TH = cisIndividual.LastNameTh;
                    //    dbIndividual.LAST_NAME_EN = cisIndividual.LastNameEn;
                    //    dbIndividual.BIRTH_DATE = cisIndividual.BirthDate;
                    //    dbIndividual.CUST_TYPE_GROUP = cisIndividual.CustTypeGroup;
                    //    dbIndividual.CUST_TYPE_CODE = cisIndividual.CustTypeCode;
                    //    dbIndividual.CARD_TYPE_CODE = cisIndividual.CardTypeCode;
                    //    dbIndividual.TITLE_ID = cisIndividual.TitleId;
                    //    dbIndividual.KKCIS_ID = cisIndividual.KKcisId;
                    //    dbIndividual.UPDATE_DATE = DateTime.Now.ToString();
                    //    dbIndividual.CUSTOMER_ID = null;

                    //    vc = new ValidationContext(cisIndividual, null, null);
                    //    var validationResults = new List<ValidationResult>();
                    //    bool valid = Validator.TryValidateObject(cisIndividual, vc, validationResults, true);
                    //    if (!valid)
                    //    {
                    //        dbIndividual.ERROR =
                    //            validationResults.Select(x => x.ErrorMessage)
                    //                .Aggregate((i, j) => i + Environment.NewLine + j);
                    //    }

                    //    _context.TB_I_CIS_INDIVIDUAL.Add(dbIndividual);
                    //}

                    //this.Save();
                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                //_context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
            //}
        }
        public bool SaveCisIndividualComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_INDIVIDUAL(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisProductGroup(List<CisProductGroupEntity> cisProductGroups)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisProductGroups != null && cisProductGroups.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PRODUCT_GROUP");
                            this.Save();

                            foreach (CisProductGroupEntity cisProductGroup in cisProductGroups)
                            {
                                TB_I_CIS_PRODUCT_GROUP dbProductGroup = new TB_I_CIS_PRODUCT_GROUP();

                                dbProductGroup.PRODUCT_CODE = cisProductGroup.ProductCode;
                                dbProductGroup.PRODUCT_TYPE = cisProductGroup.ProductType;
                                dbProductGroup.PRODUCT_DESC = cisProductGroup.ProductDesc;
                                dbProductGroup.SYSTEM = cisProductGroup.SYSTEM;
                                dbProductGroup.PRODUCT_FLAG = cisProductGroup.ProductFlag;
                                dbProductGroup.ENTITY_CODE = cisProductGroup.EntityCode;
                                dbProductGroup.SUBSCR_CODE = cisProductGroup.SubscrCode;
                                dbProductGroup.SUBSCR_DESC = cisProductGroup.SubscrDesc;
                                dbProductGroup.STATUS = cisProductGroup.Status;

                                _context.TB_I_CIS_PRODUCT_GROUP.Add(dbProductGroup);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisSubscription(List<CisSubscriptionEntity> cisSubscriptions)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisSubscriptions != null && cisSubscriptions.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisSubscriptions.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstSubscription = from cisSubscription in cisSubscriptions.Skip(k * pageSize).Take(pageSize)
                                              select
                                              new TB_I_CIS_SUBSCRIPTION
                                              {
                                                  KKCIS_ID = cisSubscription.KKCisId,
                                                  CUST_ID = cisSubscription.CustId,
                                                  CARD_ID = cisSubscription.CardId,
                                                  CARD_TYPE_CODE = cisSubscription.CardTypeCode,
                                                  PROD_GROUP = cisSubscription.ProdGroup,
                                                  PROD_TYPE = cisSubscription.ProdType,
                                                  SUBSCR_CODE = cisSubscription.SubscrCode,
                                                  BRANCH_NAME = cisSubscription.BranchName,
                                                  REF_NO = cisSubscription.RefNo,
                                                  TEXT1 = cisSubscription.Text1,
                                                  TEXT2 = cisSubscription.Text2,
                                                  TEXT3 = cisSubscription.Text3,
                                                  TEXT4 = cisSubscription.Text4,
                                                  TEXT5 = cisSubscription.Text5,
                                                  TEXT6 = cisSubscription.Text6,
                                                  TEXT7 = cisSubscription.Text7,
                                                  TEXT8 = cisSubscription.Text8,
                                                  TEXT9 = cisSubscription.Text9,
                                                  TEXT10 = cisSubscription.Text10,
                                                  NUMBER1 = cisSubscription.Number1,
                                                  NUMBER2 = cisSubscription.Number2,
                                                  NUMBER3 = cisSubscription.Number3,
                                                  NUMBER4 = cisSubscription.Number4,
                                                  NUMBER5 = cisSubscription.Number5,
                                                  DATE1 = cisSubscription.Date1,
                                                  DATE2 = cisSubscription.Date2,
                                                  DATE3 = cisSubscription.Date3,
                                                  DATE4 = cisSubscription.Date4,
                                                  DATE5 = cisSubscription.Date5,
                                                  SUBSCR_STATUS = cisSubscription.SubscrStatus,
                                                  CREATE_DATE = cisSubscription.CreatedDate,
                                                  CREATE_BY = cisSubscription.CreatedBy,
                                                  CREATE_CHANNEL = cisSubscription.CreatedChanel,
                                                  UPDATE_DATE = cisSubscription.UpdatedDate,
                                                  UPDATE_BY = cisSubscription.UpdatedBy,
                                                  UPDATED_CHANNEL = cisSubscription.UpdatedChannel,
                                                  SYSCUSTSUBSCR_ID = cisSubscription.SysCustSubscrId
                                              };

                        DataTable dt = DataTableHelpers.ConvertTo(lstSubscription);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);

                        bc.DestinationTableName = "TB_I_CIS_SUBSCRIPTION";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    cisSubscriptions = null;

                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                //_context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
            //}
        }

        public bool SaveCisSubscriptionComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_SUBSCRIPTION(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisTitle(List<CisTitleEntity> cisTitles)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisTitles != null && cisTitles.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_TITLE");
                            this.Save();

                            foreach (CisTitleEntity cisTitle in cisTitles)
                            {
                                TB_I_CIS_TITLE dbTitle = new TB_I_CIS_TITLE();

                                dbTitle.TITLE_ID = cisTitle.TitleID;
                                dbTitle.TITLE_NAME_TH = cisTitle.TitleNameTH;
                                dbTitle.TITLE_NAME_EN = cisTitle.TitleNameEN;
                                dbTitle.TITLE_TYPE_GROUP = cisTitle.TitleTypeGroup;
                                dbTitle.GENDER_CODE = cisTitle.GenderCode;
                                dbTitle.STATUS = cisTitle.Status;

                                _context.TB_I_CIS_TITLE.Add(dbTitle);
                            }

                            this.Save();
                            this.UpdateMasterTitle();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisProvince(List<CisProvinceEntity> cisProvinces)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisProvinces != null && cisProvinces.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PROVINCE");
                            this.Save();

                            foreach (CisProvinceEntity cisProvince in cisProvinces)
                            {
                                TB_I_CIS_PROVINCE dbProvince = new TB_I_CIS_PROVINCE();

                                dbProvince.PROVINCE_CODE = cisProvince.ProvinceCode;
                                dbProvince.PROVINCE_NAME_TH = cisProvince.ProvinceNameTH;
                                dbProvince.PROVINCE_NAME_EN = cisProvince.ProvinceNameEN;
                                dbProvince.STATUS = cisProvince.Status;

                                _context.TB_I_CIS_PROVINCE.Add(dbProvince);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisDistrict(List<CisDistrictEntity> cisDistricts)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisDistricts != null && cisDistricts.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_DISTRICT");
                            this.Save();

                            foreach (CisDistrictEntity cisDistrict in cisDistricts)
                            {
                                TB_I_CIS_DISTRICT dbDistrict = new TB_I_CIS_DISTRICT();

                                dbDistrict.PROVINCE_CODE = cisDistrict.ProvinceCode;
                                dbDistrict.DISTRICT_CODE = cisDistrict.DistrictCode;
                                dbDistrict.DISTRICT_NAME_TH = cisDistrict.DistrictNameTH;
                                dbDistrict.DISTRICT_NAME_EN = cisDistrict.DistrictNameEN;
                                dbDistrict.STATUS = cisDistrict.Status;

                                _context.TB_I_CIS_DISTRICT.Add(dbDistrict);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisSubDistrict(List<CisSubDistrictEntity> cisSubDistricts)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisSubDistricts != null && cisSubDistricts.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBDISTRICT");
                            this.Save();

                            foreach (CisSubDistrictEntity cisSubDistrict in cisSubDistricts)
                            {
                                TB_I_CIS_SUBDISTRICT dbDistrict = new TB_I_CIS_SUBDISTRICT();

                                dbDistrict.DISTRICT_CODE = cisSubDistrict.DistrictCode;
                                dbDistrict.SUBDISTRICT_CODE = cisSubDistrict.SubDistrictCode;
                                dbDistrict.SUBDISTRICT_NAME_TH = cisSubDistrict.SubDistrictNameTH;
                                dbDistrict.SUBDISTRICT_NAME_EN = cisSubDistrict.SubDistrictNameEN;
                                dbDistrict.POSTCODE = cisSubDistrict.PostCode;
                                dbDistrict.STATUS = cisSubDistrict.Status;

                                _context.TB_I_CIS_SUBDISTRICT.Add(dbDistrict);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisPhoneType(List<CisPhoneTypeEntity> cisPhones)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisPhones != null && cisPhones.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PHONE_TYPE");
                            this.Save();

                            foreach (CisPhoneTypeEntity cisPhone in cisPhones)
                            {
                                TB_I_CIS_PHONE_TYPE dbPhonetype = new TB_I_CIS_PHONE_TYPE();

                                dbPhonetype.PHONE_TYPE_CODE = cisPhone.PhoneTypecode;
                                dbPhonetype.PHONE_TYPE_DESC = cisPhone.PhoneTypeDesc;
                                dbPhonetype.STATUS = cisPhone.Status;

                                _context.TB_I_CIS_PHONE_TYPE.Add(dbPhonetype);

                                this.UpdateMasterPhoneType(cisPhone);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisEmailType(List<CisEmailTypeEntity> cisEmails)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisEmails != null && cisEmails.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_EMAIL_TYPE");
                            this.Save();

                            foreach (CisEmailTypeEntity cisEmail in cisEmails)
                            {
                                TB_I_CIS_EMAIL_TYPE dbEmailtype = new TB_I_CIS_EMAIL_TYPE();

                                dbEmailtype.EMAIL_TYPE_CODE = cisEmail.MailTypecode;
                                dbEmailtype.EMAIL_TYPE_DESC = cisEmail.MailTypeDesc;
                                dbEmailtype.STATUS = cisEmail.Status;

                                _context.TB_I_CIS_EMAIL_TYPE.Add(dbEmailtype);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisSubscribeAddress(List<CisSubscribeAddressEntity> cisSubscribeAdds)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisSubscribeAdds != null && cisSubscribeAdds.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_ADDRESS");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisSubscribeAdds.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstSubscribeAdd = from cisSubscribeAdd in cisSubscribeAdds.Skip(k * pageSize).Take(pageSize)
                                              select
                                              new TB_I_CIS_SUBSCRIPTION_ADDRESS
                                              {
                                                  CUST_ID = cisSubscribeAdd.CustId,
                                                  CARD_ID = cisSubscribeAdd.CardId,
                                                  CARD_TYPE_CODE = cisSubscribeAdd.CardTypeCode,
                                                  ADDRESS_TYPE_CODE = cisSubscribeAdd.AddressTypeCode,
                                                  ADDRESS_NUMBER = cisSubscribeAdd.AddressNumber,
                                                  VILLAGE = cisSubscribeAdd.Village,
                                                  BUILDING = cisSubscribeAdd.Building,
                                                  FLOOR_NO = cisSubscribeAdd.FloorNo,
                                                  ROOM_NO = cisSubscribeAdd.RoomNo,
                                                  MOO = cisSubscribeAdd.Moo,
                                                  STREET = cisSubscribeAdd.Street,
                                                  SOI = cisSubscribeAdd.Soi,
                                                  SUB_DISTRICT_CODE = cisSubscribeAdd.SubDistrictCode,
                                                  DISTRICT_CODE = cisSubscribeAdd.DistrictCode,
                                                  PROVINCE_CODE = cisSubscribeAdd.ProvinceCode,
                                                  COUNTRY_CODE = cisSubscribeAdd.CountryCode,
                                                  POSTAL_CODE = cisSubscribeAdd.PostalCode,
                                                  KKCIS_ID = cisSubscribeAdd.KKCisId,
                                                  UPDATE_DATE = cisSubscribeAdd.UpdatedDate,
                                                  PROD_GROUP = cisSubscribeAdd.ProdGroup,
                                                  PROD_TYPE = cisSubscribeAdd.ProdType,
                                                  SUBSCR_CODE = cisSubscribeAdd.SubscrCode,
                                                  PROVINCE_VALUE = cisSubscribeAdd.ProvinceValue,
                                                  DISTRICT_VALUE = cisSubscribeAdd.DistrictValue,
                                                  SUB_DISTRICT_VALUE = cisSubscribeAdd.SubDistrictValue,
                                                  POSTAL_VALUE = cisSubscribeAdd.PostalValue,
                                                  SYSCUSTSUBSCR_ID = cisSubscribeAdd.SysCustSubscrId
                                              };

                        DataTable dt = DataTableHelpers.ConvertTo(lstSubscribeAdd);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);

                        bc.DestinationTableName = "TB_I_CIS_SUBSCRIPTION_ADDRESS";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    cisSubscribeAdds = null;

                    //foreach (CisSubscribeAddressEntity cisSubscribeAdd in cisSubscribeAdds)
                    //{
                    //    TB_I_CIS_SUBSCRIBE_ADDRESS dbSubscribeAdd = new TB_I_CIS_SUBSCRIBE_ADDRESS();

                    //    dbSubscribeAdd.CUST_ID = cisSubscribeAdd.CustId;
                    //    dbSubscribeAdd.CARD_ID = cisSubscribeAdd.CardId;
                    //    dbSubscribeAdd.CARD_TYPE_CODE = cisSubscribeAdd.CardTypeCode;
                    //    dbSubscribeAdd.ADDRESS_TYPE_CODE = cisSubscribeAdd.AddressTypeCode;
                    //    dbSubscribeAdd.ADDRESS_NUMBER = cisSubscribeAdd.AddressNumber;
                    //    dbSubscribeAdd.VILLAGE = cisSubscribeAdd.Village;
                    //    dbSubscribeAdd.BUILDING = cisSubscribeAdd.Building;
                    //    dbSubscribeAdd.FLOOR_NO = cisSubscribeAdd.FloorNo;
                    //    dbSubscribeAdd.ROOM_NO = cisSubscribeAdd.RoomNo;
                    //    dbSubscribeAdd.MOO = cisSubscribeAdd.Moo;
                    //    dbSubscribeAdd.STREET = cisSubscribeAdd.Street;
                    //    dbSubscribeAdd.SOI = cisSubscribeAdd.Soi;
                    //    dbSubscribeAdd.SUB_DISTRICT_CODE = cisSubscribeAdd.SubDistrictCode;
                    //    dbSubscribeAdd.DISTRICT_CODE = cisSubscribeAdd.DistrictCode;
                    //    dbSubscribeAdd.PROVICE_CODE = cisSubscribeAdd.ProvinceCode;
                    //    dbSubscribeAdd.COUNTRY_CODE = cisSubscribeAdd.CountryCode;
                    //    dbSubscribeAdd.POSTAL_CODE = cisSubscribeAdd.PostalCode;
                    //    dbSubscribeAdd.KKCIS_ID = cisSubscribeAdd.KKCisId;
                    //    dbSubscribeAdd.UPDATE_DATE = cisSubscribeAdd.UpdatedDate;
                    //    dbSubscribeAdd.PROD_GROUP = cisSubscribeAdd.ProdGroup;
                    //    dbSubscribeAdd.PROD_TYPE = cisSubscribeAdd.ProdType;
                    //    dbSubscribeAdd.SUBSC_CODE = cisSubscribeAdd.SubscrCode;
                    //    dbSubscribeAdd.PROVINCE_VALUE = cisSubscribeAdd.ProvinceValue;
                    //    dbSubscribeAdd.DISTRICT_VALUE = cisSubscribeAdd.DistrictValue;
                    //    dbSubscribeAdd.SUB_DISTRICT_VALUE = cisSubscribeAdd.SubDistrictValue;
                    //    dbSubscribeAdd.POSTAL_VALUE = cisSubscribeAdd.PostalValue;

                    //    _context.TB_I_CIS_SUBSCRIBE_ADDRESS.Add(dbSubscribeAdd);
                    //}

                    //this.Save();
                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                //_context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
        }

        public bool SaveCisSubscribePhone(List<CisSubscribePhoneEntity> cisSubscribePhones)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisSubscribePhones != null && cisSubscribePhones.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_PHONE");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisSubscribePhones.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstSubscribePhone = from cisSubscribePhone in cisSubscribePhones.Skip(k * pageSize).Take(pageSize)
                                                select
                                                new TB_I_CIS_SUBSCRIPTION_PHONE
                                                {
                                                    KKCIS_ID = cisSubscribePhone.KKCisId,
                                                    CUST_ID = cisSubscribePhone.CustId,
                                                    CARD_ID = cisSubscribePhone.CardId,
                                                    CARD_TYPE_CODE = cisSubscribePhone.CardTypeCode,
                                                    PROD_GROUP = cisSubscribePhone.ProdGroup,
                                                    PROD_TYPE = cisSubscribePhone.ProdType,
                                                    SUBSCR_CODE = cisSubscribePhone.SubscrCode,
                                                    PHONE_TYPE_CODE = cisSubscribePhone.PhoneTypeCode,
                                                    PHONE_NUM = cisSubscribePhone.PhoneNum,
                                                    PHONE_EXT = cisSubscribePhone.PhoneExt,
                                                    CREATE_DATE = cisSubscribePhone.CreatedDate,
                                                    CREATE_BY = cisSubscribePhone.CreatedBy,
                                                    UPDATE_DATE = cisSubscribePhone.UpdatedDate,
                                                    UPDATE_BY = cisSubscribePhone.UpdatedBy,
                                                    SYSCUSTSUBSCR_ID = cisSubscribePhone.SysCustSubscrId
                                                };

                        DataTable dt = DataTableHelpers.ConvertTo(lstSubscribePhone);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);

                        bc.DestinationTableName = "TB_I_CIS_SUBSCRIPTION_PHONE";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    cisSubscribePhones = null;

                    //foreach (CisSubscribePhoneEntity cisSubscribePhone in cisSubscribePhones)
                    //{
                    //    TB_I_CIS_SUBSCRIBE_PHONE dbSubscibePhone = new TB_I_CIS_SUBSCRIBE_PHONE();

                    //    dbSubscibePhone.KKCIS_ID = cisSubscribePhone.KKCisId;
                    //    dbSubscibePhone.CUST_ID = cisSubscribePhone.CustId;
                    //    dbSubscibePhone.CARD_ID = cisSubscribePhone.CardId;
                    //    dbSubscibePhone.CARD_TYPE_CODE = cisSubscribePhone.CardTypeCode;
                    //    dbSubscibePhone.PROD_GROUP = cisSubscribePhone.ProdGroup;
                    //    dbSubscibePhone.PROD_TYPE = cisSubscribePhone.ProdType;
                    //    dbSubscibePhone.SUBSCR_CODE = cisSubscribePhone.SubscrCode;
                    //    dbSubscibePhone.PHONE_TYPE_CODE = cisSubscribePhone.PhoneTypeCode;
                    //    dbSubscibePhone.PHONE_NUM = cisSubscribePhone.PhoneNum;
                    //    dbSubscibePhone.PHONE_EXT = cisSubscribePhone.PhoneExt;
                    //    dbSubscibePhone.CREATE_DATE = cisSubscribePhone.CreatedDate;
                    //    dbSubscibePhone.CRAATE_BY = cisSubscribePhone.CreatedBy;
                    //    dbSubscibePhone.UPDATE_DATE = cisSubscribePhone.UpdatedDate;
                    //    dbSubscibePhone.UPDATE_BY = cisSubscribePhone.UpdatedBy;

                    //    _context.TB_I_CIS_SUBSCRIBE_PHONE.Add(dbSubscibePhone);
                    //}

                    //this.Save();
                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
            //}
        }
        public bool SaveCisSubscribeEmail(List<CisSubscribeMailEntity> cisSubscribeMails)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisSubscribeMails != null && cisSubscribeMails.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_EMAIL");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisSubscribeMails.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstIndividual = from cisSubscribeMail in cisSubscribeMails.Skip(k * pageSize).Take(pageSize)
                                            select
                                            new TB_I_CIS_SUBSCRIPTION_EMAIL
                                            {
                                                KKCIS_ID = cisSubscribeMail.KKCisId,
                                                CUST_ID = cisSubscribeMail.CustId,
                                                CARD_ID = cisSubscribeMail.CardId,
                                                CARD_TYPE_CODE = cisSubscribeMail.CardTypeCode,
                                                PROD_GROUP = cisSubscribeMail.ProdGroup,
                                                PROD_TYPE = cisSubscribeMail.ProdType,
                                                SUBSCR_CODE = cisSubscribeMail.SubscrCode,
                                                EMAIL_TYPE_CODE = cisSubscribeMail.MailTypeCode,
                                                EMAIL_TYPE_NAME = null,
                                                MAILACCOUNT = cisSubscribeMail.MailAccount,
                                                CREATE_DATE = cisSubscribeMail.CreatedDate,
                                                CREATE_BY = cisSubscribeMail.CreatedBy,
                                                UPDATE_DATE = cisSubscribeMail.UpdatedDate,
                                                UPDATE_BY = cisSubscribeMail.UpdatedBy,
                                                SYSCUSTSUBSCR_ID = cisSubscribeMail.SysCustSubscrId
                                            };

                        DataTable dt = DataTableHelpers.ConvertTo(lstIndividual);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);

                        bc.DestinationTableName = "TB_I_CIS_SUBSCRIPTION_EMAIL";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    cisSubscribeMails = null;

                    //foreach (CisSubscribeMailEntity cisSubscribeMail in cisSubscribeMails)
                    //{
                    //    TB_I_CIS_SUBSCRIBE_EMAIL dbSubscibeMail = new TB_I_CIS_SUBSCRIBE_EMAIL();

                    //    dbSubscibeMail.KKCIS_ID = cisSubscribeMail.KKCisId;
                    //    dbSubscibeMail.CUST_ID = cisSubscribeMail.CustId;
                    //    dbSubscibeMail.CARD_ID = cisSubscribeMail.CardId;
                    //    dbSubscibeMail.CARD_TYPE_CODE = cisSubscribeMail.CardTypeCode;
                    //    dbSubscibeMail.PROD_GROUP = cisSubscribeMail.ProdGroup;
                    //    dbSubscibeMail.PROD_TYPE = cisSubscribeMail.ProdType;
                    //    dbSubscibeMail.SUBSCR_CODE = cisSubscribeMail.SubscrCode;
                    //    dbSubscibeMail.EMAIL_TYPE_CODE = cisSubscribeMail.MailTypeCode;
                    //    dbSubscibeMail.EMAIL_TYPE_NAME = null;
                    //    dbSubscibeMail.MAILACCOUNT = cisSubscribeMail.MailAccount;
                    //    dbSubscibeMail.CREATE_DATE = cisSubscribeMail.CreatedDate;
                    //    dbSubscibeMail.CREATE_BY = cisSubscribeMail.CreatedBy;
                    //    dbSubscibeMail.UPDATE_DATE = cisSubscribeMail.UpdatedDate;
                    //    dbSubscibeMail.UPDATE_BY = cisSubscribeMail.UpdatedBy;
                    //    dbSubscibeMail.CUSTOMER_ID = null;

                    //    _context.TB_I_CIS_SUBSCRIBE_EMAIL.Add(dbSubscibeMail);
                    //}

                    //this.Save();
                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                //_context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
            //}
        }

        public bool SaveCisSubscribeAddressComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_ACCOUNT_ADDRESS(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisSubscribePhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_ACCOUNT_PHONE(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisSubscribeEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_ACCOUNT_EMAIL(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }
        public bool SaveCisAddressType(List<CisAddressTypeEntity> cisAddresstypes)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisAddresstypes != null && cisAddresstypes.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_ADDRESS_TYPE");
                            this.Save();

                            foreach (CisAddressTypeEntity cisAddresstype in cisAddresstypes)
                            {
                                TB_I_CIS_ADDRESS_TYPE dbAddresstype = new TB_I_CIS_ADDRESS_TYPE();

                                dbAddresstype.ADDRESS_TYPE_CODE = cisAddresstype.AddressTypeCode;
                                dbAddresstype.ADDRESS_TYPE_NAME = cisAddresstype.AddressTypeDesc;
                                dbAddresstype.STATUS = cisAddresstype.Status;

                                _context.TB_I_CIS_ADDRESS_TYPE.Add(dbAddresstype);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisCountry(List<CisCountryEntity> cisCountry)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisCountry != null && cisCountry.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_COUNTRY");
                            this.Save();

                            foreach (CisCountryEntity countryEntity in cisCountry)
                            {
                                TB_I_CIS_COUNTRY dbCountry = new TB_I_CIS_COUNTRY();

                                dbCountry.COUNTRY_CODE = countryEntity.CountryCode;
                                dbCountry.COUNTRY_NAME_TH = countryEntity.CountryNameTH;
                                dbCountry.COUNTRY_NAME_EN = countryEntity.CountryNameEN;
                                dbCountry.STATUS = countryEntity.Status;

                                _context.TB_I_CIS_COUNTRY.Add(dbCountry);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public List<CisCorporateEntity> GetCISCorExport()
        {
            var query = (from cor in _context.TB_I_CIS_CORPORATE.AsNoTracking()
                         where !string.IsNullOrEmpty(cor.ERROR)
                         select new CisCorporateEntity
                         {
                             KKCisId = cor.KKCIS_ID,
                             CustId = cor.CUST_ID,
                             CardId = cor.CARD_ID,
                             CardTypeCode = cor.CARD_TYPE_CODE,
                             CustTypeCode = cor.CUST_TYPE_CODE,
                             CustTypeGroup = cor.CUST_TYPE_GROUP,
                             TitleId = cor.TITLE_ID,
                             NameTh = cor.NAME_TH,
                             NameEn = cor.NAME_EN,
                             IsicCode = cor.ISIC_CODE,
                             TaxId = cor.TAX_ID,
                             HostBusinessCountryCode = cor.HOST_BUSINESS_COUNTRY_CODE,
                             ValuePerShare = cor.VALUE_PER_SHARE,
                             AuthorizedShareCapital = cor.AUTHORIZED_SHARE_CAPITAL,
                             RegisterDate = cor.REGISTER_DATE,
                             BusinessCode = cor.BUSINESS_CODE,
                             FixedAsset = cor.FIXED_ASSET,
                             FixedAssetexcludeLand = cor.FIXED_ASSET_EXCLUDE_LAND,
                             NumberOfEmployee = cor.NUMBER_OF_EMPLOYEE,
                             ShareInfoFlag = cor.SHARE_INFO_FLAG,
                             FlgmstApp = cor.FLG_MST_APP,
                             FirstBranch = cor.FIRST_BRANCH,
                             PlaceCustUpdated = cor.PLACE_CUST_UPDATED,
                             DateCustUpdated = cor.DATE_CUST_UPDATED,
                             IdCountryIssue = cor.ID_COUNTRY_ISSUE,
                             BusinessCatCode = cor.BUSINESS_CAT_CODE,
                             MarketingId = cor.MARKETING_ID,
                             Stock = cor.STOCK,
                             CreatedDate = cor.CREATE_DATE,
                             CreatedBy = cor.CREATE_BY,
                             UpdatedDate = cor.UPDATE_DATE,
                             UpdatedBy = cor.UPDATE_BY,
                             Error = cor.ERROR
                         });

            return query.ToList();
        }

        public List<CisIndividualEntity> GetCISIndivExport()
        {
            var query = (from indiv in _context.TB_I_CIS_INDIVIDUAL.AsNoTracking()
                         where !string.IsNullOrEmpty(indiv.ERROR)
                         select new CisIndividualEntity
                         {
                             KKCisId = indiv.KKCIS_ID,
                             CustId = indiv.CUST_ID,
                             CardId = indiv.CARD_ID,
                             CardtypeCode = indiv.CARD_TYPE_CODE,
                             CusttypeCode = indiv.CUST_TYPE_CODE,
                             CusttypeGroup = indiv.CUST_TYPE_GROUP,
                             TitleId = indiv.TITLE_ID,
                             TitlenameCustom = indiv.TITLE_NAME_CUSTOM,
                             FirstnameTh = indiv.FIRST_NAME_TH,
                             MidnameTh = indiv.MID_NAME_TH,
                             LastnameTh = indiv.LAST_NAME_TH,
                             FirstnameEn = indiv.FIRST_NAME_EN,
                             MidnameEn = indiv.MID_NAME_EN,
                             LastnameEn = indiv.LAST_NAME_EN,
                             BirthDate = indiv.BIRTH_DATE,
                             GenderCode = indiv.GENDER_CODE,
                             MaritalCode = indiv.MARITAL_CODE,
                             Nationality1Code = indiv.NATIONALITY1_CODE,
                             Nationality2Code = indiv.NATIONALITY2_CODE,
                             Nationality3Code = indiv.NATIONALITY3_CODE,
                             ReligionCode = indiv.RELIGION_CODE,
                             EducationCode = indiv.EDUCATION_CODE,
                             Position = indiv.POSITION,
                             BusinessCode = indiv.BUSINESS_CODE,
                             CompanyName = indiv.COMPANY_NAME,
                             IsicCode = indiv.ISIC_CODE,
                             AnnualIncome = indiv.ANNUAL_INCOME,
                             SourceIncome = indiv.SOURCE_INCOME,
                             TotalwealthPeriod = indiv.TOTAL_WEALTH_PERIOD,
                             FlgmstApp = indiv.FLG_MST_APP,
                             ChannelHome = indiv.CHANNEL_HOME,
                             FirstBranch = indiv.FIRST_BRANCH,
                             ShareinfoFlag = indiv.SHARE_INFO_FLAG,
                             PlacecustUpdated = indiv.PLACE_CUST_UPDATED,
                             DatecustUpdated = indiv.DATE_CUST_UPDATED,
                             AnnualincomePeriod = indiv.ANNUAL_INCOME_PERIOD,
                             MarketingId = indiv.MARKETING_ID,
                             NumberofEmployee = indiv.NUMBER_OF_EMPLOYEE,
                             FixedAsset = indiv.FIXED_ASSET,
                             FixedassetExcludeland = indiv.FIXED_ASSET_EXCLUDE_LAND,
                             OccupationCode = indiv.OCCUPATION_CODE,
                             OccupationsubtypeCode = indiv.OCCUPATION_SUBTYPE_CODE,
                             CountryIncome = indiv.COUNTRY_INCOME,
                             TotalwealTh = indiv.TOTAL_WEALTH,
                             SourceIncomerem = indiv.SOURCE_INCOME_REM,
                             CreatedDate = indiv.CREATE_DATE,
                             CreatedBy = indiv.CREATE_BY,
                             UpdateDate = indiv.UPDATE_DATE,
                             UpdatedBy = indiv.UPDATE_BY,
                             Error = indiv.ERROR
                         });

            return query.ToList();
        }

        public List<CisSubscriptionEntity> GetCisSubscriptionExport()
        {
            var query = (from sub in _context.TB_I_CIS_SUBSCRIPTION.AsNoTracking()
                         where !string.IsNullOrEmpty(sub.ERROR)
                         select new CisSubscriptionEntity
                         {
                             KKCisId = sub.KKCIS_ID,
                             CustId = sub.CUST_ID,
                             CardId = sub.CARD_ID,
                             CardTypeCode = sub.CARD_TYPE_CODE,
                             ProdGroup = sub.PROD_GROUP,
                             ProdType = sub.PROD_TYPE,
                             SubscrCode = sub.SUBSCR_CODE,
                             RefNo = sub.REF_NO,
                             BranchName = sub.BRANCH_NAME,
                             Text1 = sub.TEXT1,
                             Text2 = sub.TEXT2,
                             Text3 = sub.TEXT3,
                             Text4 = sub.TEXT4,
                             Text5 = sub.TEXT5,
                             Text6 = sub.TEXT6,
                             Text7 = sub.TEXT7,
                             Text8 = sub.TEXT8,
                             Text9 = sub.TEXT9,
                             Text10 = sub.TEXT10,
                             Number1 = sub.NUMBER1,
                             Number2 = sub.NUMBER2,
                             Number3 = sub.NUMBER3,
                             Number4 = sub.NUMBER4,
                             Number5 = sub.NUMBER5,
                             Date1 = sub.DATE1,
                             Date2 = sub.DATE2,
                             Date3 = sub.DATE3,
                             Date4 = sub.DATE4,
                             Date5 = sub.DATE5,
                             SubscrStatus = sub.SUBSCR_STATUS,
                             CreatedDate = sub.CREATE_DATE,
                             CreatedBy = sub.CREATE_BY,
                             CreatedChanel = sub.CREATE_CHANNEL,
                             UpdatedDate = sub.UPDATE_DATE,
                             UpdatedBy = sub.UPDATE_BY,
                             UpdatedChannel = sub.UPDATED_CHANNEL,
                             SysCustSubscrId = sub.SYSCUSTSUBSCR_ID,
                             Error = sub.ERROR
                         });

            return query.ToList();
        }

        public List<CisSubscribeAddressEntity> GetCisSubscribeAddressExport()
        {
            var query = (from address in _context.TB_I_CIS_SUBSCRIPTION_ADDRESS.AsNoTracking()
                         where !string.IsNullOrEmpty(address.ERROR)
                         select new CisSubscribeAddressEntity
                         {
                             KKCisId = address.KKCIS_ID,
                             CustId = address.CUST_ID,
                             CardId = address.CARD_ID,
                             CardTypeCode = address.CARD_TYPE_CODE,
                             ProdGroup = address.PROD_GROUP,
                             ProdType = address.PROD_TYPE,
                             SubscrCode = address.SUBSCR_CODE,
                             AddressTypeCode = address.ADDRESS_TYPE_CODE,
                             AddressNumber = address.ADDRESS_NUMBER,
                             Village = address.VILLAGE,
                             Building = address.BUILDING,
                             FloorNo = address.FLOOR_NO,
                             RoomNo = address.ROOM_NO,
                             Moo = address.MOO,
                             Street = address.STREET,
                             Soi = address.SOI,
                             SubDistrictCode = address.SUB_DISTRICT_CODE,
                             DistrictCode = address.DISTRICT_CODE,
                             ProvinceCode = address.PROVINCE_CODE,
                             CountryCode = address.COUNTRY_CODE,
                             PostalCode = address.POSTAL_CODE,
                             SubDistrictValue = address.SUB_DISTRICT_VALUE,
                             DistrictValue = address.DISTRICT_VALUE,
                             ProvinceValue = address.PROVINCE_VALUE,
                             PostalValue = address.POSTAL_VALUE,
                             CreatedDate = address.CREATE_DATE,
                             CreatedBy = address.CREATE_BY,
                             UpdatedDate = address.UPDATE_DATE,
                             UpdatedBy = address.UPDATE_BY,
                             SysCustSubscrId = address.SYSCUSTSUBSCR_ID,
                             Error = address.ERROR
                         });

            return query.ToList();
        }

        public List<CisSubscribePhoneEntity> GetCisSubscribePhoneExport()
        {
            var query = (from phone in _context.TB_I_CIS_SUBSCRIPTION_PHONE.AsNoTracking()
                         where !string.IsNullOrEmpty(phone.ERROR)
                         select new CisSubscribePhoneEntity
                         {
                             KKCisId = phone.KKCIS_ID,
                             CustId = phone.CUST_ID,
                             CardId = phone.CARD_ID,
                             CardTypeCode = phone.CARD_TYPE_CODE,
                             ProdGroup = phone.PROD_GROUP,
                             ProdType = phone.PROD_TYPE,
                             SubscrCode = phone.SUBSCR_CODE,
                             PhoneTypeCode = phone.PHONE_TYPE_CODE,
                             PhoneNum = phone.PHONE_NUM,
                             PhoneExt = phone.PHONE_EXT,
                             CreatedDate = phone.CREATE_DATE,
                             CreatedBy = phone.CREATE_BY,
                             UpdatedDate = phone.UPDATE_DATE,
                             UpdatedBy = phone.UPDATE_BY,
                             SysCustSubscrId = phone.SYSCUSTSUBSCR_ID,
                             Error = phone.ERROR
                         });

            return query.ToList();
        }

        public List<CisSubscribeMailEntity> GetCisSubscriptEmailExport()
        {
            var query = (from email in _context.TB_I_CIS_SUBSCRIPTION_EMAIL.AsNoTracking()
                         where !string.IsNullOrEmpty(email.ERROR)
                         select new CisSubscribeMailEntity
                         {
                             KKCisId = email.KKCIS_ID,
                             CustId = email.CUST_ID,
                             CardId = email.CARD_ID,
                             CardTypeCode = email.CARD_TYPE_CODE,
                             ProdGroup = email.PROD_GROUP,
                             ProdType = email.PROD_TYPE,
                             SubscrCode = email.SUBSCR_CODE,
                             MailTypeCode = email.EMAIL_TYPE_CODE,
                             MailAccount = email.MAILACCOUNT,
                             CreatedDate = email.CREATE_DATE,
                             CreatedBy = email.CREATE_BY,
                             UpdatedDate = email.UPDATE_DATE,
                             UpdatedBy = email.UPDATE_BY,
                             SysCustSubscrId = email.SYSCUSTSUBSCR_ID,
                             Error = email.ERROR
                         });

            return query.ToList();
        }

        private static string ValidateError<T>(T obj)
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
        private void UpdateMasterTitle()
        {
            var titleTh = _context.TB_I_CIS_TITLE.Where(x => x.TITLE_NAME_TH.Trim() != "").Select(x => x.TITLE_NAME_TH).Distinct().ToList();
            var titleEn = _context.TB_I_CIS_TITLE.Where(x => x.TITLE_NAME_EN.Trim() != "").Select(x => x.TITLE_NAME_EN).Distinct().ToList();

            foreach (var item in titleTh)
            {
                var title = _context.TB_M_TITLE.FirstOrDefault(x => x.TITLE_NAME.Equals(item) && x.LANGUAGE.Equals(Constants.TitleLanguage.TitleTh));

                if (title != null)
                {
                    title.TITLE_NAME = item;
                    title.LANGUAGE = Constants.TitleLanguage.TitleTh;

                    SetEntryStateModified(title);
                    this.Save();
                }
                else
                {
                    TB_M_TITLE dbTitle = new TB_M_TITLE();
                    dbTitle.TITLE_NAME = item;
                    dbTitle.LANGUAGE = Constants.TitleLanguage.TitleTh;

                    _context.TB_M_TITLE.Add(dbTitle);
                    this.Save();
                }
            }

            foreach (var item in titleEn)
            {
                var title = _context.TB_M_TITLE.FirstOrDefault(x => x.TITLE_NAME.ToUpper().Equals(item.ToUpper()) && x.LANGUAGE.Equals(Constants.TitleLanguage.TitleEn));

                if (title != null)
                {
                    title.TITLE_NAME = item;
                    title.LANGUAGE = Constants.TitleLanguage.TitleEn;

                    SetEntryStateModified(title);
                    this.Save();
                }
                else
                {
                    TB_M_TITLE dbTitle = new TB_M_TITLE();
                    dbTitle.TITLE_NAME = item;
                    dbTitle.LANGUAGE = Constants.TitleLanguage.TitleEn;

                    _context.TB_M_TITLE.Add(dbTitle);
                    this.Save();
                }
            }

        }
        private void UpdateMasterPhoneType(CisPhoneTypeEntity cisPhonetype)
        {
            var phonetype = _context.TB_M_PHONE_TYPE.FirstOrDefault(x => x.PHONE_TYPE_CODE == cisPhonetype.PhoneTypecode);
            if (phonetype == null)
            {
                TB_M_PHONE_TYPE dbPhonetype = new TB_M_PHONE_TYPE();
                dbPhonetype.PHONE_TYPE_CODE = cisPhonetype.PhoneTypecode;
                dbPhonetype.PHONE_TYPE_NAME = cisPhonetype.PhoneTypeDesc;
                dbPhonetype.STATUS = 1;

                _context.TB_M_PHONE_TYPE.Add(dbPhonetype);
                this.Save();
            }
            else
            {
                phonetype.PHONE_TYPE_CODE = cisPhonetype.PhoneTypecode;
                phonetype.PHONE_TYPE_NAME = cisPhonetype.PhoneTypeDesc;
                SetEntryStateModified(phonetype);
                this.Save();
            }
        }
        public bool SaveCisSubscriptionType(List<CisSubscriptionTypeEntity> cisSubscriptionTypes)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    ValidationContext vc = null;
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisSubscriptionTypes != null && cisSubscriptionTypes.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_TYPE");
                            this.Save();

                            foreach (CisSubscriptionTypeEntity cisType in cisSubscriptionTypes)
                            {
                                TB_I_CIS_SUBSCRIPTION_TYPE dbType = new TB_I_CIS_SUBSCRIPTION_TYPE();

                                dbType.CUST_TYPE_GROUP = cisType.CustTypeGroup;
                                dbType.CUST_TYPE_CODE = cisType.CustTypeCode;
                                dbType.CUST_TYPE_TH = cisType.CustTypeTh;
                                dbType.CUST_TYPE_EN = cisType.CustTypeEn;
                                dbType.CARD_TYPE_CODE = cisType.CardTypeCode;
                                dbType.CARD_TYPE_EN = cisType.CardTypeEn;
                                dbType.CARD_TYPE_TH = cisType.CardTypeTh;
                                dbType.STATUS = cisType.Status;

                                vc = new ValidationContext(cisType, null, null);
                                var validationResults = new List<ValidationResult>();
                                bool valid = Validator.TryValidateObject(cisType, vc, validationResults, true);
                                if (!valid)
                                {
                                    dbType.ERROR =
                                        validationResults.Select(x => x.ErrorMessage)
                                            .Aggregate((i, j) => i + Environment.NewLine + j);
                                }

                                _context.TB_I_CIS_SUBSCRIPTION_TYPE.Add(dbType);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }
        public bool SaveCisSubscriptionTypeComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_SUBSCRIPTION_TYPE(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }

        public List<CisSubscriptionTypeEntity> GetCisSubscriptionTypeExport()
        {
            var query = (from sub in _context.TB_I_CIS_SUBSCRIPTION_TYPE.AsNoTracking()
                         where !string.IsNullOrEmpty(sub.ERROR)
                         select new CisSubscriptionTypeEntity
                         {
                             CustTypeGroup = sub.CUST_TYPE_GROUP,
                             CustTypeCode = sub.CUST_TYPE_CODE,
                             CustTypeTh = sub.CUST_TYPE_TH,
                             CustTypeEn = sub.CUST_TYPE_EN,
                             CardTypeCode = sub.CARD_TYPE_CODE,
                             CardTypeEn = sub.CARD_TYPE_EN,
                             CardTypeTh = sub.CARD_TYPE_TH,
                             Status = sub.STATUS,
                             Error = sub.ERROR
                         });

            return query.ToList();
        }

        public bool SaveCisCustomerPhone(List<CisCustomerPhoneEntity> cisCustomerPhones)
        {
            //using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{ 
            //_context.Configuration.AutoDetectChangesEnabled = false;
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (cisCustomerPhones != null && cisCustomerPhones.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CUSTOMER_PHONE");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (cisCustomerPhones.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstCustomerPhone = from cisCustomerPhone in cisCustomerPhones.Skip(k * pageSize).Take(pageSize)
                                               select
                                               new TB_I_CIS_CUSTOMER_PHONE
                                               {
                                                   KKCIS_ID = cisCustomerPhone.KKCisId,
                                                   CUST_ID = cisCustomerPhone.CustId,
                                                   CARD_ID = cisCustomerPhone.CardId,
                                                   CARD_TYPE_CODE = cisCustomerPhone.CardTypeCode,
                                                   CUST_TYPE_GROUP = cisCustomerPhone.CustTypeGroup,
                                                   PHONE_TYPE_CODE = cisCustomerPhone.PhoneTypeCode,
                                                   PHONE_NUM = cisCustomerPhone.PhoneNum,
                                                   PHONE_EXT = cisCustomerPhone.PhoneExt,
                                                   CREATE_DATE = cisCustomerPhone.CreateDate,
                                                   CREATE_BY = cisCustomerPhone.CreateBy,
                                                   UPDATE_DATE = cisCustomerPhone.UpdateDate,
                                                   UPDATE_BY = cisCustomerPhone.UpdateBy,
                                                   CUSTOMER_ID = null
                                               };

                        DataTable dt = DataTableHelpers.ConvertTo(lstCustomerPhone);
                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);
                        bc.DestinationTableName = "TB_I_CIS_CUSTOMER_PHONE";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    cisCustomerPhones = null;

                    //foreach (CisCustomerPhoneEntity cisCustomerPhone in cisCustomerPhones)
                    //{
                    //    TB_I_CIS_CUSTOMER_PHONE dbCustomer = new TB_I_CIS_CUSTOMER_PHONE();

                    //    dbCustomer.KKCIS_ID  = cisCustomerPhone.KKCisId;
                    //    dbCustomer.CUST_ID =cisCustomerPhone.CustId;
                    //    dbCustomer.CARD_ID  = cisCustomerPhone.CardId;
                    //    dbCustomer.CARD_TYPE_CODE = cisCustomerPhone.CardTypeCode;
                    //    dbCustomer.CUST_TYPE_GROUP = cisCustomerPhone.CustTypeGroup;
                    //    dbCustomer.PHONE_TYPE_CODE = cisCustomerPhone.PhoneTypeCode;
                    //    dbCustomer.PHONE_NUM = cisCustomerPhone.PhoneNum;
                    //    dbCustomer.PHONE_EXT = cisCustomerPhone.PhoneExt;
                    //    dbCustomer.CREATE_DATE = cisCustomerPhone.CreateDate;
                    //    dbCustomer.CREATE_BY = cisCustomerPhone.CreateBy;
                    //    dbCustomer.UPDATE_DATE = cisCustomerPhone.UpdateDate;
                    //    dbCustomer.UPDATE_BY = cisCustomerPhone.UpdateBy; 

                    //    _context.TB_I_CIS_CUSTOMER_PHONE.Add(dbCustomer);
                    //}

                    this.Save();
                }

                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                //_context.Configuration.AutoDetectChangesEnabled = true;
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
            //}
        }
        public bool SaveCisCustomerPhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_CUSTOMER_PHONE(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }

        public List<CisCustomerPhoneEntity> GetCisCustomerPhoneExport()
        {
            var query = (from cusphone in _context.TB_I_CIS_CUSTOMER_PHONE.AsNoTracking()
                         where !string.IsNullOrEmpty(cusphone.ERROR)
                         select new CisCustomerPhoneEntity
                         {
                             KKCisId = cusphone.KKCIS_ID,
                             CustId = cusphone.CUST_ID,
                             CardId = cusphone.CARD_ID,
                             CardTypeCode = cusphone.CARD_TYPE_CODE,
                             CustTypeGroup = cusphone.CUST_TYPE_GROUP,
                             PhoneTypeCode = cusphone.PHONE_TYPE_CODE,
                             PhoneNum = cusphone.PHONE_NUM,
                             PhoneExt = cusphone.PHONE_EXT,
                             CreateDate = cusphone.CREATE_DATE,
                             CreateBy = cusphone.CREATE_BY,
                             UpdateDate = cusphone.UPDATE_DATE,
                             UpdateBy = cusphone.UPDATE_BY,
                             Error = cusphone.ERROR
                         });

            return query.ToList();
        }

        public bool SaveCisCustomerEmail(List<CisCustomerEmailEntity> cisCustomerEmails)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (cisCustomerEmails != null && cisCustomerEmails.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CUSTOMER_EMAIL");
                            this.Save();

                            foreach (CisCustomerEmailEntity cisCustomerEmail in cisCustomerEmails)
                            {
                                TB_I_CIS_CUSTOMER_EMAIL dbCustomer = new TB_I_CIS_CUSTOMER_EMAIL();

                                dbCustomer.KKCIS_ID = cisCustomerEmail.KKCisId;
                                dbCustomer.CUST_ID = cisCustomerEmail.CustId;
                                dbCustomer.CARD_ID = cisCustomerEmail.CardId;
                                dbCustomer.CARD_TYPE_CODE = cisCustomerEmail.CardTypeCode;
                                dbCustomer.CUST_TYPE_GROUP = cisCustomerEmail.CustTypeGroup;
                                dbCustomer.MAIL_TYPE_CODE = cisCustomerEmail.MailTypeCode;
                                dbCustomer.MAILACCOUNT = cisCustomerEmail.MailAccount;
                                dbCustomer.CREATE_DATE = cisCustomerEmail.CreatedDate;
                                dbCustomer.CREATE_BY = cisCustomerEmail.CreatedBy;
                                dbCustomer.UPDATE_DATE = cisCustomerEmail.UpdatedDate;
                                dbCustomer.UPDATE_BY = cisCustomerEmail.UpdatedBy;

                                _context.TB_I_CIS_CUSTOMER_EMAIL.Add(dbCustomer);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public bool SaveCisCustomerEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.SP_IMPORT_CIS_CUSTOMER_EMAIL(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                if ((int)outputIsError.Value == 0)
                {
                    numOfComplete = (int)outputNumOfComplete.Value;
                    numOfError = (int)outputNumOfError.Value;
                    //messageError = (string)outputMsgError.Value;
                }
                else
                {
                    numOfComplete = 0;
                    numOfError = 0;
                    messageError = (string)outputMsgError.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }

        public List<CisCustomerEmailEntity> GetCisCustomerEmailExport()
        {
            var query = (from customer in _context.TB_I_CIS_CUSTOMER_EMAIL.AsNoTracking()
                         where !string.IsNullOrEmpty(customer.ERROR)
                         select new CisCustomerEmailEntity
                         {
                             KKCisId = customer.KKCIS_ID,
                             CustId = customer.CUST_ID,
                             CardId = customer.CARD_ID,
                             CardTypeCode = customer.CARD_TYPE_CODE,
                             CustTypeGroup = customer.CUST_TYPE_GROUP,
                             MailTypeCode = customer.MAIL_TYPE_CODE,
                             MailAccount = customer.MAILACCOUNT,
                             CreatedDate = customer.CREATE_DATE,
                             CreatedBy = customer.CREATE_BY,
                             UpdatedDate = customer.UPDATE_DATE,
                             UpdatedBy = customer.UPDATE_BY,
                             Error = customer.ERROR
                         });

            return query.ToList();
        }

        public void DeleteCisTitle()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_TITLE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisCorporate()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CORPORATE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisIndividual()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_INDIVIDUAL");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisProductGroup()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PRODUCT_GROUP");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubscription()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisProvince()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PROVINCE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisDistrict()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_DISTRICT");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubDistrict()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBDISTRICT");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisPhoneType()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_PHONE_TYPE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisEmailType()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_EMAIL_TYPE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubscribeAddress()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_ADDRESS");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubscribePhone()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_PHONE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubscribeEmail()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_EMAIL");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisAddressType()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_ADDRESS_TYPE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisSubscriptionType()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_SUBSCRIPTION_TYPE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisCustomerPhone()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CUSTOMER_PHONE");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisCustomerEmail()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_CUSTOMER_EMAIL");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        public void DeleteCisCountry()
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_CIS_COUNTRY");
                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        #region "Persistence"

        private int Save()
        {
            return _context.SaveChanges();
        }

        private void SetEntryCurrentValues(object entityTo, object entityFrom)
        {
            _context.Entry(entityTo).CurrentValues.SetValues(entityFrom);
            // Set state to Modified
            _context.Entry(entityTo).State = System.Data.Entity.EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        #endregion
    }
}
