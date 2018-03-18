using System;
using System.Collections.Generic;
using CSM.Entity;
using log4net;
using System.Linq;
using CSM.Common.Utilities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CSM.Data.DataAccess
{
    public class BdwDataAccess : IBdwDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BdwDataAccess));

        public BdwDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public bool SaveBdwContact(List<BdwContactEntity> bdwContacts)
        {
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (bdwContacts != null && bdwContacts.Count > 0)
                {
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_BDW_CONTACT");
                    this.Save();

                    int pageSize = 5000;
                    int totalPage = (bdwContacts.Count + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lstContact = from contact in bdwContacts.Skip(k * pageSize).Take(pageSize)
                                         select
                                         new TB_I_BDW_CONTACT
                                         {
                                             DATA_TYPE = contact.DataType,
                                             DATA_DATE = contact.DataDate,
                                             DATA_SOURCE = contact.DataSource,
                                             CARD_NO = contact.CardNo,
                                             TITILE_TH = contact.TitileTh,
                                             NAME_TH = contact.NameTh,
                                             SURNAME_TH = contact.SurnameTh,
                                             TITILE_EN = contact.TitileEn,
                                             NAME_EN = contact.NameEn,
                                             SURNAME_EN = contact.SurnameEn,
                                             ACCOUNT_NO = contact.AccountNo,
                                             LOAN_MAIN = contact.LoanMain,
                                             PRODUCT_GROUP = contact.ProductGroup,
                                             PRODUCT = contact.Product,
                                             RELATIONSHIP = contact.Relationship,
                                             PHONE = contact.Phone,
                                             ACCOUNT_STATUS = contact.AccountStatus,
                                             CARD_TYPE_CODE = contact.CardTypeCode,
                                             CAMPAIGN = contact.Campaign
                                         };

                        DataTable dt = DataTableHelpers.ConvertTo(lstContact);
                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);
                        bc.DestinationTableName = "TB_I_BDW_CONTACT";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    bdwContacts = null; // clear
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
        }

        public bool SaveCompleteBdwContact(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            bool result = false;
            try
            {
                #region "Comment out"

                //var query = _context.TB_I_BDW_CONTACT.Where(x => string.IsNullOrEmpty(x.ERROR));
                //if (query.Any())
                //{
                //    query = from bdw in _context.TB_I_BDW_CONTACT.AsNoTracking()
                //            from acc in _context.TB_M_ACCOUNT.Where(x => x.ACCOUNT_NO == bdw.ACCOUNT_NO).DefaultIfEmpty()
                //            where bdw.ERROR == null && acc.ACCOUNT_NO == null
                //            select bdw;

                //    Task.Factory.StartNew(() => Parallel.ForEach(query, i =>
                //    {
                //        lock (sync)
                //        {
                //            i.ERROR = "Not found accountNo";
                //            _context.Entry(i.ERROR).State = System.Data.Entity.EntityState.Modified; // set modified state
                //        }
                //    })).Wait();

                //    this.Save();
                //}

                #endregion

                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                _context.Database.CommandTimeout = Constants.BatchCommandTimeout;

                _context.SP_IMPORT_BDW_CONTACT(outputNumOfComplete, outputNumOfError, outputIsError, outputMsgError);               

                if ((int)outputIsError.Value == 0)
                {
                    result = true;
                }

                numOfComplete = (int)outputNumOfComplete.Value;
                numOfError = (int)outputNumOfError.Value;
                messageError = (string)outputMsgError.Value;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return result;
        }

        public List<BdwContactEntity> GetErrorBdwContact(int pageIndex, int pageSize)
        {
            var query = _context.TB_I_BDW_CONTACT.Where(x => !string.IsNullOrEmpty(x.ERROR) && !x.ERROR.Equals("Duplicate Record"))
                .OrderBy(x => x.BDW_CONTACT_ID).Skip(pageIndex * pageSize).Take(pageSize)
                .Select(bdw => new BdwContactEntity
                {
                    DataType = bdw.DATA_TYPE,
                    DataDate = bdw.DATA_DATE,
                    DataSource = bdw.DATA_SOURCE,
                    CardNo = bdw.CARD_NO,
                    TitileTh = bdw.TITILE_TH,
                    NameTh = bdw.NAME_TH,
                    SurnameTh = bdw.SURNAME_TH,
                    TitileEn = bdw.TITILE_EN,
                    NameEn = bdw.NAME_EN,
                    SurnameEn = bdw.SURNAME_EN,
                    AccountNo = bdw.ACCOUNT_NO,
                    LoanMain = bdw.LOAN_MAIN,
                    ProductGroup = bdw.PRODUCT_GROUP,
                    Product = bdw.PRODUCT,
                    Campaign = bdw.CAMPAIGN,                   
                    AccountStatus = bdw.ACCOUNT_STATUS,
                    CardTypeCode = bdw.CARD_TYPE_CODE,
                    Relationship = bdw.RELATIONSHIP,
                    Phone = bdw.PHONE,
                    Error = bdw.ERROR
                });

            return query.Any() ? query.ToList() : null;
        }

        public int GetCountErrorBdwContact()
        {
            int count = 0;
            string stmt = "SELECT COUNT(*) FROM TB_I_BDW_CONTACT WHERE ERROR IS NOT NULL AND ERROR <> @inError";
            string connStr = WebConfig.GetConnectionString("CSMConnectionString");

            try
            {
                using (SqlConnection thisConnection = new SqlConnection(connStr))
                {
                    using (SqlCommand cmdCount = new SqlCommand(stmt, thisConnection))
                    {
                        thisConnection.Open();
                        cmdCount.Parameters.Add(new SqlParameter("@inError", "Duplicate Record"));
                        count = (int)cmdCount.ExecuteScalar();
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return 0;
            }
        }

        public BdwContactEntity GetErrorHeaderBdwContact()
        {
            var query = from bdw in _context.TB_I_BDW_CONTACT
                        where bdw.DATA_TYPE == Constants.ImportBDWContact.DataTypeHeader
                        select new BdwContactEntity
                        {
                            DataType = bdw.DATA_TYPE,
                            DataDate = bdw.DATA_DATE,
                            DataSource = bdw.DATA_SOURCE
                        };

            return query.FirstOrDefault();
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
