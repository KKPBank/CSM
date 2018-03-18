using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSM.Entity;
using log4net;
using CSM.Common.Utilities;
using System.Globalization;
using System.Data;

namespace CSM.Data.DataAccess
{
    public class CustomerDataAccess : ICustomerDataAccess
    {
        private object sync = new Object();
        private readonly CSMContext _context;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CustomerDataAccess));

        public CustomerDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "Relationship"
        public IEnumerable<RelationshipEntity> GetAllRelationships(RelationshipSearchFilter searchFilter)
        {
            var query = from rs in _context.TB_M_RELATIONSHIP.AsNoTracking()
                        from cs in _context.TB_R_USER.Where(o => o.USER_ID == rs.CREATE_USER).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == rs.UPDATE_USER).DefaultIfEmpty()
                        where (searchFilter.Status == null || searchFilter.Status == Constants.ApplicationStatus.All || rs.STATUS == searchFilter.Status)
                        && (string.IsNullOrEmpty(searchFilter.RelationshipName) || rs.RELATIONSHIP_NAME.ToUpper().Contains(searchFilter.RelationshipName.ToUpper()))
                        && (string.IsNullOrEmpty(searchFilter.RelationshipDesc) || rs.RELATIONSHIP_DESC.ToUpper().Contains(searchFilter.RelationshipDesc.ToUpper()))
                        select new RelationshipEntity
                        {
                            RelationshipId = rs.RELATIONSHIP_ID,
                            RelationshipName = rs.RELATIONSHIP_NAME,
                            RelationshipDesc = rs.RELATIONSHIP_DESC,
                            Status = rs.STATUS,
                            CreatedDate = rs.CREATE_DATE,
                            Updatedate = rs.UPDATE_DATE,
                            CreateUser = cs != null ? new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            } : null,
                            UpdateUser = us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetRelationshipListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<RelationshipEntity>();
        }

        public RelationshipEntity GetRelationshipByID(int relationshipId)
        {
            var query = from rs in _context.TB_M_RELATIONSHIP
                        from cs in _context.TB_R_USER.Where(o => o.USER_ID == rs.CREATE_USER).DefaultIfEmpty()
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == rs.UPDATE_USER).DefaultIfEmpty()
                        where rs.RELATIONSHIP_ID == relationshipId
                        select new RelationshipEntity
                        {
                            RelationshipId = rs.RELATIONSHIP_ID,
                            RelationshipName = rs.RELATIONSHIP_NAME,
                            RelationshipDesc = rs.RELATIONSHIP_DESC,
                            Status = rs.STATUS,
                            CreatedDate = rs.CREATE_DATE,
                            Updatedate = rs.UPDATE_DATE,
                            CreateUser = cs != null ? new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            } : null,
                            UpdateUser = us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public bool IsDuplicateRelationship(RelationshipEntity relationEntity)
        {
            var cnt = _context.TB_M_RELATIONSHIP.Where(
                            x => x.RELATIONSHIP_NAME == relationEntity.RelationshipName
                                 && x.RELATIONSHIP_ID != relationEntity.RelationshipId
                                 ).Count();
            return cnt > 0;
        }

        public bool SaveContactRelation(RelationshipEntity relationEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                TB_M_RELATIONSHIP relationship = null;

                //Save New
                if (relationEntity.RelationshipId == 0)
                {
                    relationship = new TB_M_RELATIONSHIP();
                    relationship.RELATIONSHIP_NAME = relationEntity.RelationshipName;
                    relationship.RELATIONSHIP_DESC = relationEntity.RelationshipDesc;
                    relationship.STATUS = relationEntity.Status;
                    relationship.CREATE_USER = relationEntity.CreateUser.UserId;
                    relationship.CREATE_DATE = DateTime.Now;
                    relationship.UPDATE_USER = relationEntity.UpdateUser.UserId;
                    relationship.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_RELATIONSHIP.Add(relationship);
                    this.Save();

                }
                else
                {
                    //Edit
                    relationship = _context.TB_M_RELATIONSHIP.FirstOrDefault(x => x.RELATIONSHIP_ID == relationEntity.RelationshipId);
                    if (relationship != null)
                    {
                        relationship.RELATIONSHIP_NAME = relationEntity.RelationshipName;
                        relationship.RELATIONSHIP_DESC = relationEntity.RelationshipDesc;
                        relationship.STATUS = relationEntity.Status;
                        relationship.UPDATE_USER = relationEntity.UpdateUser.UserId;
                        relationship.UPDATE_DATE = DateTime.Now;
                        SetEntryStateModified(relationship);
                        this.Save();
                    }
                    else
                    {
                        Logger.ErrorFormat("RELATIONSHIP ID: {0} does not exist", relationEntity.RelationshipId);
                    }
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
            }

            return false;
        }

        #endregion

        #region "Customer"
        public IEnumerable<CustomerEntity> xxGetCustomerList(CustomerSearchFilter searchFilter)
        {
            int? customerType = searchFilter.CustomerType.ToNullable<int>();
            int? status = searchFilter.Status.ToNullable<int>();

            const string defaultPhone = "-1CSM";
            string phoneNo = string.IsNullOrEmpty(searchFilter.PhoneNo) ? defaultPhone : searchFilter.PhoneNo;
            var lstCustomerId = from p in _context.TB_M_PHONE
                                where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && p.PHONE_NO == phoneNo
                                select new
                                {
                                    CustomerId = p.CUSTOMER_ID
                                };

            var query = from ac in _context.TB_M_ACCOUNT
                        join c in _context.TB_M_CUSTOMER on ac.CUSTOMER_ID equals c.CUSTOMER_ID
                        from l in lstCustomerId.Where(x => x.CustomerId == c.CUSTOMER_ID).DefaultIfEmpty()
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        where (customerType == null || customerType == Constants.ApplicationStatus.All || c.TYPE == customerType)
                            //&& (string.IsNullOrEmpty(searchFilter.AccountNo) || ac.ACCOUNT_NO.Equals(searchFilter.AccountNo))
                            ////&& (string.IsNullOrEmpty(searchFilter.PhoneNo) || c.TB_M_PHONE.Any(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && x.PHONE_NO.Equals(searchFilter.PhoneNo)))
                            //&& (string.IsNullOrEmpty(searchFilter.Product) || ac.PRODUCT.Equals(searchFilter.Product))
                            //&& (string.IsNullOrEmpty(searchFilter.Grade) || ac.GRADE.Equals(searchFilter.Grade))
                            //&& (string.IsNullOrEmpty(searchFilter.BranchName) || ac.BRANCH_NAME.Equals(searchFilter.BranchName))
                            && (phoneNo == defaultPhone || l.CustomerId.HasValue)
                        select new CustomerEntity
                        {
                            CustomerId = c.CUSTOMER_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            AccountNo = ac.ACCOUNT_NO,
                            Account = (ac != null ? new AccountEntity
                            {
                                AccountId = ac.ACCOUNT_ID,
                                AccountNo = ac.ACCOUNT_NO,
                                Product = ac.SUBSCRIPTION_DESC,
                                ProductGroup = ac.PRODUCT_GROUP,
                                Status = ac.STATUS,
                                BranchCode = ac.BRANCH_CODE,
                                BranchName = ac.BRANCH_NAME,
                                AccountDesc = ac.ACCOUNT_DESC,
                                Grade = ac.GRADE
                            } : null),
                            CustomerType = c.TYPE,
                            SubscriptType = (st != null ? new SubscriptTypeEntity
                            {
                                SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            } : null),
                            Registration = ac != null ? ac.CAR_NO : "",
                            //PhoneList = (from p in _context.TB_M_PHONE
                            //             where p.CUSTOMER_ID == c.CUSTOMER_ID && p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                            //             select new PhoneEntity
                            //             {
                            //                 CustomerId = p.CUSTOMER_ID,
                            //                 PhoneTypeId = p.PHONE_TYPE_ID,
                            //                 PhoneId = p.PHONE_ID,
                            //                 PhoneNo = p.PHONE_NO
                            //             }).ToList()
                        };

            // Filter CardNo
            if (!string.IsNullOrEmpty(searchFilter.CardNo))
            {
                query = query.Where(x => x.CardNo.ToUpper().Equals(searchFilter.CardNo.ToUpper()));
            }

            // Filter AccountNo
            if (!string.IsNullOrEmpty(searchFilter.AccountNo))
            {
                query = query.Where(x => x.AccountNo.ToUpper().Equals(searchFilter.AccountNo.ToUpper()));
            }

            // Filter Product
            if (!string.IsNullOrEmpty(searchFilter.Product))
            {
                query = query.Where(x => x.Account.Product.Equals(searchFilter.Product));
            }

            // Filter Grade
            if (!string.IsNullOrEmpty(searchFilter.Grade))
            {
                query = query.Where(x => x.Account.Grade.Equals(searchFilter.Grade));
            }

            // Filter BranchName
            if (!string.IsNullOrEmpty(searchFilter.BranchName))
            {
                query = query.Where(x => x.Account.BranchName.Equals(searchFilter.BranchName));
            }

            // Filter Status
            if (status != null && status != Constants.ApplicationStatus.All)
            {
                if (status == 1)
                {
                    query = query.Where(x => x.Account.Status == Constants.AccountStatus.Active);
                }
                else if (status == 0)
                {
                    query = query.Where(x => x.Account.Status != Constants.AccountStatus.Active);
                }
            }

            // Wildcard filter by
            query = WildcardFilterBy(query, searchFilter);

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetCustomerListSort(query, searchFilter);
            query = query.Skip(startPageIndex).Take(searchFilter.PageSize);
            var customers = query.ToList();

            Task.Factory.StartNew(() => Parallel.ForEach(customers, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                customer =>
                {
                    lock (sync)
                    {
                        var phones = from p in _context.TB_M_PHONE
                                     where p.CUSTOMER_ID == customer.CustomerId && p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                     select new PhoneEntity
                                     {
                                         CustomerId = p.CUSTOMER_ID,
                                         PhoneTypeId = p.PHONE_TYPE_ID,
                                         PhoneId = p.PHONE_ID,
                                         PhoneNo = p.PHONE_NO
                                     };

                        customer.PhoneList = phones.Any() ? phones.ToList() : null;
                    }
                })).Wait();

            return customers;
        }

        public string GetOTPTemplateIdByLang(string iVRLang)
        {
            return (from a in _context.TB_M_OTP_TEMPLATE.AsNoTracking()
                    select (iVRLang == Constants.IVRLang.English ? a.TEMP_ENG : a.TEMP_TH))
                    .FirstOrDefault();
        }

        public IEnumerable<CustomerEntity> GetCustomerList(CustomerSearchFilter searchFilter)
        {

            string strCustomerType = null;
            string strStatus = null;
            string strCardNo = !string.IsNullOrEmpty(searchFilter.CardNo) ? searchFilter.CardNo.EscapeSingleQuotes().ToUpper() : null;
            string strAccountNo = !string.IsNullOrEmpty(searchFilter.AccountNo) ? searchFilter.AccountNo.EscapeSingleQuotes().ToUpper() : null;
            string strProduct = !string.IsNullOrEmpty(searchFilter.Product) ? searchFilter.Product : null;
            string strGrade = !string.IsNullOrEmpty(searchFilter.Grade) ? searchFilter.Grade : null;
            string strBranchName = !string.IsNullOrEmpty(searchFilter.BranchName) ? searchFilter.BranchName : null;
            string strPhoneNo = !string.IsNullOrEmpty(searchFilter.PhoneNo) ? searchFilter.PhoneNo : null;
            string strSortBy = string.Empty;
           
            int refSearchType = 0;
            string strFirstName = string.Empty;
            string strLastName = string.Empty;
            string strCarNo = string.Empty;
            

            #region "Filter by FirstName"

            if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
            {
                string firstName = searchFilter.FirstName.ExtractString(ref refSearchType).EscapeSingleQuotes();
                switch (refSearchType)
                {
                    case 1:
                        strFirstName = string.Format(" AND ( FIRST_NAME_TH LIKE '{0}%' OR UPPER(FIRST_NAME_EN) LIKE '{1}%' ) ", firstName, firstName.ToUpperInvariant());
                        break;
                    case 2:
                        strFirstName = string.Format(" AND ( FIRST_NAME_TH LIKE '%{0}' OR UPPER(FIRST_NAME_EN) LIKE '%{1}' ) ", firstName, firstName.ToUpperInvariant());
                        break;
                    case 3:
                        strFirstName = string.Format(" AND ( FIRST_NAME_TH LIKE '%{0}%' OR UPPER(FIRST_NAME_EN) LIKE '%{1}%' ) ", firstName, firstName.ToUpperInvariant());
                        break;
                    default:
                        strFirstName = string.Format(" AND ( FIRST_NAME_TH = '{0}' OR UPPER(FIRST_NAME_EN) = '{1}' ) ", firstName, firstName.ToUpperInvariant());
                        break;
                }
            }

            #endregion

            #region "Filter by LastName"

            refSearchType = 0;

            if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
            {
                string lastName = searchFilter.LastName.ExtractString(ref refSearchType).EscapeSingleQuotes();
                switch (refSearchType)
                {
                    case 1:
                        strLastName = string.Format(" AND ( LAST_NAME_TH LIKE '{0}%' OR UPPER(cus.LAST_NAME_EN) LIKE '{1}%' ) ", lastName, lastName.ToUpperInvariant());
                        break;
                    case 2:
                        strLastName = string.Format(" AND ( LAST_NAME_TH LIKE '%{0}' OR UPPER(LAST_NAME_EN) LIKE '%{1}' ) ", lastName, lastName.ToUpperInvariant());
                        break;
                    case 3:
                        strLastName = string.Format(" AND ( LAST_NAME_TH LIKE '%{0}%' OR UPPER(LAST_NAME_EN) LIKE '%{1}%' ) ", lastName, lastName.ToUpperInvariant());
                        break;
                    default:
                        strLastName = string.Format(" AND ( LAST_NAME_TH = '{0}' OR UPPER(LAST_NAME_EN) = '{1}' ) ", lastName, lastName.ToUpperInvariant());
                        break;
                }
            }

            #endregion

            #region "Filter by Registration"

            refSearchType = 0;

            if (!string.IsNullOrWhiteSpace(searchFilter.Registration))
            {
                string registration = searchFilter.Registration.ExtractString(ref refSearchType).EscapeSingleQuotes();
                switch (refSearchType)
                {
                    case 1:
                        strCarNo = string.Format(" AND CAR_NO LIKE '{0}%' ", registration);
                        break;
                    case 2:
                        strCarNo = string.Format(" AND CAR_NO LIKE '%{0}' ", registration);
                        break;
                    case 3:
                        strCarNo = string.Format(" AND CAR_NO LIKE '%{0}%' ", registration);
                        break;
                    default:
                        strCarNo = string.Format(" AND CAR_NO = '{0}' ", registration);
                        break;
                }
            }

            #endregion

            #region "Filter by CustomerType"
            int? customerType = searchFilter.CustomerType.ToNullable<int>();
            if (customerType != null && customerType != Constants.ApplicationStatus.All)
            {
                strCustomerType = customerType.Value.ToString();
            }
            #endregion

            #region "Filter by Status"
            int? status = searchFilter.Status.ToNullable<int>();
            if (status != null && status != Constants.ApplicationStatus.All)
            {
                if (status == 1)
                {
                    strStatus = Constants.AccountStatus.Active;
                }
                else if (status == 0)
                {
                    strStatus = "I";
                }
            }
            #endregion

            #region "Sort by"

            var sortList = new Dictionary<string, string>();
            sortList.Add("CardNo", " cus.CARD_NO ");
            sortList.Add("FirstNameThai", " CASE WHEN ISNULL(cus.FIRST_NAME_TH,'') = '' THEN cus.FIRST_NAME_EN ELSE cus.FIRST_NAME_TH END ");
            sortList.Add("LastNameThai", " CASE WHEN ISNULL(cus.FIRST_NAME_TH,'') = '' THEN cus.LAST_NAME_EN ELSE cus.LAST_NAME_TH END ");
            sortList.Add("AccountNo", " acc.ACCOUNT_NO ");
            sortList.Add("Product", " acc.SUBSCRIPTION_DESC ");
            sortList.Add("Registration", " acc.CAR_NO ");
            sortList.Add("Grade", " acc.GRADE ");
            sortList.Add("Status", " CASE WHEN acc.[STATUS] <> 'A' THEN 'I' ELSE acc.[STATUS] END ");
            sortList.Add("BranchName", " acc.BRANCH_NAME ");
            sortList.Add("CustomerType", " cus.TYPE ");
            sortList.Add("SubscriptTypeName", " st.SUBSCRIPT_TYPE_NAME ");

            var objSort = sortList.SingleOrDefault(x => x.Key.Equals(searchFilter.SortField));
            strSortBy = objSort.Value ?? " cus.FIRST_NAME_TH ";
            strSortBy = strSortBy + searchFilter.SortOrder;

            #endregion

            // Call SP_GET_CUSTOMER_COUNT
            var totalRecords = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRecords", typeof(int));
            _context.SP_GET_CUSTOMER_COUNT(strCustomerType, strFirstName, strLastName, strCarNo, strCardNo,
                strAccountNo, strProduct, strGrade, strBranchName, strStatus,
                strPhoneNo, totalRecords);

            // Call SP_GET_CUSTOMER_LIST
            var lstCustomer = _context.SP_GET_CUSTOMER_LIST(strCustomerType, strFirstName, strLastName, strCarNo, strCardNo,
                strAccountNo, strProduct, strGrade, strBranchName, strStatus,
                strPhoneNo, searchFilter.PageNo, searchFilter.PageSize, strSortBy);

            var result = (from c in lstCustomer
                        select new CustomerEntity
                        {
                            RowNum = c.RowNum,
                            CustomerId = c.CUSTOMER_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            AccountNo = c.ACCOUNT_NO,
                            Account = new AccountEntity
                            {
                                AccountNo = c.ACCOUNT_NO,
                                AccountId = c.ACCOUNT_ID,
                                Product = c.PRODUCT,
                                ProductGroup = c.PRODUCT_GROUP,
                                Status = c.STATUS,
                                BranchCode = c.BRANCH_CODE,
                                BranchName = c.BRANCH_NAME,
                                AccountDesc = c.ACCOUNT_DESC,
                                Grade = c.GRADE
                            },
                            CustomerType = c.TYPE,
                            SubscriptType = new SubscriptTypeEntity
                            {
                                SubscriptTypeId = c.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeName = c.SUBSCRIPT_TYPE_NAME,
                            },
                            Registration = c.CAR_NO,
                            StrPhoneNo = c.PhoneList,
                        }).AsQueryable();
           

            searchFilter.TotalRecords = (int)totalRecords.Value;

            return result.OrderBy(ord => ord.RowNum).ToList();
        }

        public CustomerEntity GetCustomerByID(int customerId)
        {
            var query = from c in _context.TB_M_CUSTOMER
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        from titleTH in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_TH_ID).DefaultIfEmpty()
                        from titleEN in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_EN_ID).DefaultIfEmpty()
                        where c.CUSTOMER_ID == customerId
                        select new CustomerEntity
                        {
                            CustomerId = c.CUSTOMER_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            BirthDate = c.BIRTH_DATE,
                            Email = c.EMAIL,
                            Fax = (from p in c.TB_M_PHONE
                                   where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax
                                   select new { p.PHONE_NO }
                                  ).FirstOrDefault().PHONE_NO,
                            SubscriptType = (st != null ? new SubscriptTypeEntity
                            {
                                SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeCode = st.SUBSCRIPT_TYPE_CODE,
                                SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            } : null),
                            TitleThai = (titleTH != null ? new TitleEntity
                            {
                                TitleId = titleTH.TITLE_ID,
                                TitleName = titleTH.TITLE_NAME,
                            } : null),
                            TitleEnglish = (titleEN != null ? new TitleEntity
                            {
                                TitleId = titleEN.TITLE_ID,
                                TitleName = titleEN.TITLE_NAME,
                            } : null),
                            CustomerType = c.TYPE,
                            PhoneList = (from p in c.TB_M_PHONE
                                         where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                         orderby p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE ascending, p.PHONE_ID ascending 
                                         select new PhoneEntity
                                         {
                                             CustomerId = p.CUSTOMER_ID,
                                             PhoneTypeId = p.PHONE_TYPE_ID,
                                             PhoneId = p.PHONE_ID,
                                             PhoneNo = p.PHONE_NO,
                                             PhoneTypeName = p.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                                         }).ToList()
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public List<CustomerEntity> xxxGetCustomerByPhoneNo(int? customerId, List<string> lstPhoneNo)
        {

            List<string> lstMyPhone = new List<string>();
            if (customerId.HasValue)
            {
                lstMyPhone = _context.TB_M_PHONE.Where(x => x.CUSTOMER_ID == customerId
                    && x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax).Select(x => x.PHONE_NO).ToList();
            }
            new TB_M_CUSTOMER().EMPLOYEE_ID = 1;

            var query = from c in _context.TB_M_CUSTOMER
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        from ac in _context.TB_M_ACCOUNT.Where(x => x.CUSTOMER_ID == c.CUSTOMER_ID).DefaultIfEmpty()
                        where (lstPhoneNo.Count == 0 || c.TB_M_PHONE.Count(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && lstPhoneNo.Contains(x.PHONE_NO.ToUpper())) > 0)
                        && (!customerId.HasValue || c.CUSTOMER_ID != customerId)
                        && (lstMyPhone.Count == 0 || c.TB_M_PHONE.Count(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && !lstMyPhone.Contains(x.PHONE_NO.ToUpper())) > 0)
                        select new CustomerEntity
                        {
                            CustomerId = c.CUSTOMER_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            AccountNo = ac.ACCOUNT_NO,
                            Account = (ac != null ? new AccountEntity
                            {
                                AccountId = ac.ACCOUNT_ID,
                                AccountNo = ac.ACCOUNT_NO,
                                Product = ac.SUBSCRIPTION_DESC,
                                ProductGroup = ac.PRODUCT_GROUP,
                                Status = ac.STATUS,
                                BranchCode = ac.BRANCH_CODE,
                                BranchName = ac.BRANCH_NAME
                            } : null),
                            CustomerType = c.TYPE,
                            SubscriptType = (st != null ? new SubscriptTypeEntity
                            {
                                SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            } : null),
                            PhoneList = (from p in c.TB_M_PHONE
                                         where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                         select new PhoneEntity
                                         {
                                             CustomerId = p.CUSTOMER_ID,
                                             PhoneTypeId = p.PHONE_TYPE_ID,
                                             PhoneId = p.PHONE_ID,
                                             PhoneNo = p.PHONE_NO
                                         }).ToList()
                        };

            return query.ToList<CustomerEntity>();
        }

        public List<CustomerEntity> GetCustomerByPhoneNo(int? customerId, List<string> lstPhoneNo)
        {

            string strPhoneNo1 = (lstPhoneNo.Count > 0) ? lstPhoneNo[0] : "";
            string strPhoneNo2 = (lstPhoneNo.Count > 1) ? lstPhoneNo[1] : "";
            string strPhoneNo3 = (lstPhoneNo.Count > 2) ? lstPhoneNo[2] : "";

            // Call SP_GET_CUSTOMER_BY_PHONE
            var lstCustomer = _context.SP_GET_CUSTOMER_BY_PHONE(customerId, strPhoneNo1, strPhoneNo2, strPhoneNo3);

            var result = (from c in lstCustomer
                          select new CustomerEntity
                          {
                              RowNum = c.RowNum,
                              CustomerId = c.CUSTOMER_ID,
                              FirstNameThai = c.FIRST_NAME_TH,
                              FirstNameEnglish = c.FIRST_NAME_EN,
                              LastNameThai = c.LAST_NAME_TH,
                              LastNameEnglish = c.LAST_NAME_EN,
                              FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                              LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                              CardNo = c.CARD_NO,
                              AccountNo = c.ACCOUNT_NO,
                              Account = new AccountEntity
                              {
                                  AccountNo = c.ACCOUNT_NO,
                                  AccountId = c.ACCOUNT_ID,
                                  Product = c.PRODUCT,
                                  ProductGroup = c.PRODUCT_GROUP,
                                  Status = c.STATUS,
                                  BranchCode = c.BRANCH_CODE,
                                  BranchName = c.BRANCH_NAME,
                                  AccountDesc = c.ACCOUNT_DESC,
                                  Grade = c.GRADE
                              },
                              CustomerType = c.TYPE,
                              SubscriptType = new SubscriptTypeEntity
                              {
                                  SubscriptTypeId = c.SUBSCRIPT_TYPE_ID,
                                  SubscriptTypeName = c.SUBSCRIPT_TYPE_NAME,
                              },
                              Registration = c.CAR_NO,
                              StrPhoneNo = c.PhoneList,
                          }).AsQueryable();

            //return result.ToList();
            return result.Take(100).ToList();
        }

        public List<CustomerEntity> GetCustomerByName(string customerTHName)
        {
            var result = from c in _context.TB_M_CUSTOMER
                         from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                         from ac in _context.TB_M_ACCOUNT.Where(x => x.CUSTOMER_ID == c.CUSTOMER_ID).DefaultIfEmpty()
                         where c.FIRST_NAME_TH == customerTHName
                         select new CustomerEntity {
                             CustomerId = c.CUSTOMER_ID,
                             CardNo = c.CARD_NO,
                             FirstNameThai = c.FIRST_NAME_TH,
                             FirstNameEnglish = c.FIRST_NAME_EN,
                             LastNameThai = c.LAST_NAME_TH,
                             LastNameEnglish = c.LAST_NAME_EN,
                             FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                             LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                             AccountNo = ac.ACCOUNT_NO,
                             SubscriptType = new SubscriptTypeEntity
                             {
                                 SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                 SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                             },
                             PhoneList = (from p in c.TB_M_PHONE
                                          where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                          select new PhoneEntity
                                          {
                                              CustomerId = p.CUSTOMER_ID,
                                              PhoneTypeId = p.PHONE_TYPE_ID,
                                              PhoneId = p.PHONE_ID,
                                              PhoneNo = p.PHONE_NO
                                          }).ToList()
                         };

            return result.Take(100).ToList();
        }

        public bool SaveCustomer(CustomerEntity customerEntity)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        TB_M_CUSTOMER customer = null;

                        // Get CardTypeCode
                        string cardTypeCode = null;
                        if (customerEntity.SubscriptType != null && customerEntity.SubscriptType.SubscriptTypeId.HasValue)
                        {
                            var objSubscriptType = _context.TB_M_SUBSCRIPT_TYPE.FirstOrDefault(x => x.SUBSCRIPT_TYPE_ID == customerEntity.SubscriptType.SubscriptTypeId.Value);
                            if (objSubscriptType != null)
                            {
                                cardTypeCode = objSubscriptType.CARD_TYPE_CODE;
                            }
                        }

                        //Save New
                        if (customerEntity.CustomerId == null || customerEntity.CustomerId == 0)
                        {
                            customer = new TB_M_CUSTOMER();

                            customer.TYPE = Convert.ToInt16(customerEntity.CustomerType ?? Constants.CustomerType.Prospect); // กรณีเพิ่มข้อมูลใหม่ Type จะเป็น Prospect
                            customer.SUBSCRIPT_TYPE_ID = customerEntity.SubscriptType.SubscriptTypeId;
                            customer.CARD_TYPE_CODE = cardTypeCode;
                            customer.CARD_NO = customerEntity.CardNo;
                            customer.BIRTH_DATE = customerEntity.BirthDate;
                            customer.TITLE_TH_ID = customerEntity.TitleThai.TitleId;
                            customer.FIRST_NAME_TH = customerEntity.FirstNameThai;
                            customer.LAST_NAME_TH = customerEntity.LastNameThai;
                            customer.TITLE_EN_ID = customerEntity.TitleEnglish.TitleId;
                            customer.FIRST_NAME_EN = customerEntity.FirstNameEnglish;
                            customer.LAST_NAME_EN = customerEntity.LastNameEnglish;
                            customer.EMAIL = customerEntity.Email;
                            customer.EMPLOYEE_ID = customerEntity.EmployeeId;

                            customer.CREATE_USER = customerEntity.CreateUser.UserId;
                            customer.CREATE_DATE = DateTime.Now;
                            customer.UPDATE_USER = customerEntity.UpdateUser.UserId;
                            customer.UPDATE_DATE = DateTime.Now;

                            #region "Add CustomerLog"

                            TB_L_CUSTOMER_LOG custLog = new TB_L_CUSTOMER_LOG();
                            custLog.CUSTOMER_ID = customer.CUSTOMER_ID;
                            custLog.DETAIL = Constants.CustomerLog.AddCustomer;
                            custLog.CREATE_USER = customerEntity.CreateUser.UserId;
                            custLog.CREATE_DATE = DateTime.Now;
                            _context.TB_L_CUSTOMER_LOG.Add(custLog);

                            #endregion

                            _context.TB_M_CUSTOMER.Add(customer);
                            this.Save();

                            #region "Phone & Fax"

                            // Phone & Fax
                            if (customerEntity.PhoneList != null && customerEntity.PhoneList.Count() > 0)
                            {
                                foreach (PhoneEntity phoneEntity in customerEntity.PhoneList)
                                {
                                    TB_M_PHONE phone = new TB_M_PHONE();
                                    phone.CUSTOMER_ID = customer.CUSTOMER_ID; // FK
                                    phone.PHONE_TYPE_ID = phoneEntity.PhoneTypeId;
                                    phone.PHONE_NO = phoneEntity.PhoneNo;
                                    _context.TB_M_PHONE.Add(phone);
                                }

                                this.Save();
                            }

                            #endregion

                            customerEntity.CustomerId = customer.CUSTOMER_ID; // CustomerId ที่ได้จากการ Save

                            #region "Dummy Account"

                            TB_M_ACCOUNT dbAccount = new TB_M_ACCOUNT();
                            dbAccount.CUSTOMER_ID = customerEntity.CustomerId; // FK
                            dbAccount.ACCOUNT_NO = "DUMMY";
                            dbAccount.UPDATE_DATE = DateTime.Now;
                            dbAccount.IS_DEFAULT = true;
                            dbAccount.STATUS = "A";
                            _context.TB_M_ACCOUNT.Add(dbAccount);

                            this.Save();
                            customerEntity.DummyAccountId = dbAccount.ACCOUNT_ID;
                            int accountDummy = dbAccount.ACCOUNT_ID;

                            #endregion

                            #region "Dummy Contact"

                            ContactEntity contactEntity = new ContactEntity();
                            contactEntity.ContactId = null;
                            contactEntity.SubscriptType = new SubscriptTypeEntity
                            {
                                SubscriptTypeId = customerEntity.SubscriptType.SubscriptTypeId
                            };
                            contactEntity.CardNo = customerEntity.CardNo;
                            contactEntity.BirthDate = customerEntity.BirthDate;
                            contactEntity.TitleThai = new TitleEntity { TitleId = customerEntity.TitleThai.TitleId };
                            contactEntity.FirstNameThai = customerEntity.FirstNameThai;
                            contactEntity.LastNameThai = customerEntity.LastNameThai;
                            contactEntity.TitleEnglish = new TitleEntity { TitleId = customerEntity.TitleEnglish.TitleId };
                            contactEntity.FirstNameEnglish = customerEntity.FirstNameEnglish;
                            contactEntity.LastNameEnglish = customerEntity.LastNameEnglish;
                            contactEntity.Email = customerEntity.Email;
                            contactEntity.CreateUser = new UserEntity { UserId = customerEntity.CreateUser.UserId };
                            contactEntity.UpdateUser = new UserEntity { UserId = customerEntity.UpdateUser.UserId };
                            contactEntity.CustomerId = customerEntity.CustomerId; // FK
                            contactEntity.IsDefault = true; // Dummy

                            // Phone & Fax
                            contactEntity.PhoneList = new List<PhoneEntity>();
                            foreach (PhoneEntity phoneEntity in customerEntity.PhoneList)
                            {
                                contactEntity.PhoneList.Add(new PhoneEntity
                                {
                                    PhoneTypeId = phoneEntity.PhoneTypeId,
                                    PhoneNo = phoneEntity.PhoneNo
                                });
                            }

                            // Customer Contact
                            int relationshipIdDummy =
                                _context.TB_M_RELATIONSHIP.FirstOrDefault(x => x.IS_DEFAULT.Value == true)
                                    .RELATIONSHIP_ID;
                            List<CustomerContactEntity> lstCustomerContact = new List<CustomerContactEntity>();
                            lstCustomerContact.Add(new CustomerContactEntity
                            {
                                CustomerId = customerEntity.CustomerId, // FK
                                AccountId = accountDummy,
                                RelationshipId = relationshipIdDummy,
                                IsEdit = false
                            });

                            this.SaveContact(contactEntity, lstCustomerContact, true); // Save DummyContact

                            customerEntity.DummyCustomerContactId = contactEntity.ContactId; // dummy contact id

                            #endregion
                        }
                        else
                        {
                            //Edit
                            customer =
                                _context.TB_M_CUSTOMER.FirstOrDefault(x => x.CUSTOMER_ID == customerEntity.CustomerId);
                            if (customer != null)
                            {
                                customer.SUBSCRIPT_TYPE_ID = customerEntity.SubscriptType.SubscriptTypeId;
                                customer.CARD_TYPE_CODE = cardTypeCode;
                                customer.CARD_NO = customerEntity.CardNo;
                                customer.BIRTH_DATE = customerEntity.BirthDate;
                                customer.TITLE_TH_ID = customerEntity.TitleThai.TitleId;
                                customer.FIRST_NAME_TH = customerEntity.FirstNameThai;
                                customer.LAST_NAME_TH = customerEntity.LastNameThai;
                                customer.TITLE_EN_ID = customerEntity.TitleEnglish.TitleId;
                                customer.FIRST_NAME_EN = customerEntity.FirstNameEnglish;
                                customer.LAST_NAME_EN = customerEntity.LastNameEnglish;
                                customer.EMAIL = customerEntity.Email;
                                customer.UPDATE_USER = customerEntity.UpdateUser.UserId;
                                customer.UPDATE_DATE = DateTime.Now;
                                SetEntryStateModified(customer);

                                // AddCustomerLog
                                this.AddCustomerLog(customer.CUSTOMER_ID, Constants.CustomerLog.EditCustomer,
                                    customerEntity.UpdateUser.UserId);

                                #region "Phone & Fax"

                                // Delete Phone & Fax
                                var lstPhone = _context.TB_M_PHONE.Where(x => x.CUSTOMER_ID == customerEntity.CustomerId);
                                if (lstPhone.Any())
                                {
                                    foreach (var p in lstPhone)
                                    {
                                        _context.TB_M_PHONE.Remove(p);
                                    }
                                }

                                // Save Phone & Fax
                                if (customerEntity.PhoneList != null && customerEntity.PhoneList.Count() > 0)
                                {
                                    foreach (PhoneEntity phoneEntity in customerEntity.PhoneList)
                                    {
                                        TB_M_PHONE phone = new TB_M_PHONE();
                                        phone.CUSTOMER_ID = customer.CUSTOMER_ID; // FK
                                        phone.PHONE_TYPE_ID = phoneEntity.PhoneTypeId;
                                        phone.PHONE_NO = phoneEntity.PhoneNo;
                                        _context.TB_M_PHONE.Add(phone);
                                    }
                                }

                                #endregion

                                this.Save();

                                #region "Update Dummy Contact"

                                int? accountDummy = null;
                                var query = from c in _context.TB_M_CONTACT
                                            //from cc in
                                            //    _context.TB_M_CUSTOMER_CONTACT.Where(x => x.CONTACT_ID == c.CONTACT_ID)
                                            //        .DefaultIfEmpty()
                                            join cc in _context.TB_M_CUSTOMER_CONTACT on c.CONTACT_ID equals cc.CONTACT_ID
                                            where c.IS_DEFAULT == true && cc.CUSTOMER_ID == customerEntity.CustomerId
                                            select new { c.CONTACT_ID };
                                if (query.Any())
                                {
                                    accountDummy = query.FirstOrDefault().CONTACT_ID;
                                }

                                if (accountDummy.HasValue)
                                {
                                    ContactEntity contactEntity = new ContactEntity();
                                    contactEntity.ContactId = accountDummy.Value;
                                    contactEntity.SubscriptType = new SubscriptTypeEntity
                                    {
                                        SubscriptTypeId = customerEntity.SubscriptType.SubscriptTypeId
                                    };
                                    contactEntity.CardNo = customerEntity.CardNo;
                                    contactEntity.BirthDate = customerEntity.BirthDate;
                                    contactEntity.TitleThai = new TitleEntity
                                    {
                                        TitleId = customerEntity.TitleThai.TitleId
                                    };
                                    contactEntity.FirstNameThai = customerEntity.FirstNameThai;
                                    contactEntity.LastNameThai = customerEntity.LastNameThai;
                                    contactEntity.TitleEnglish = new TitleEntity
                                    {
                                        TitleId = customerEntity.TitleEnglish.TitleId
                                    };
                                    contactEntity.FirstNameEnglish = customerEntity.FirstNameEnglish;
                                    contactEntity.LastNameEnglish = customerEntity.LastNameEnglish;
                                    contactEntity.Email = customerEntity.Email;
                                    contactEntity.CreateUser = new UserEntity
                                    {
                                        UserId = customerEntity.CreateUser.UserId
                                    };
                                    contactEntity.UpdateUser = new UserEntity
                                    {
                                        UserId = customerEntity.UpdateUser.UserId
                                    };
                                    contactEntity.CustomerId = customerEntity.CustomerId;
                                    // FK                            

                                    // Phone & Fax
                                    contactEntity.PhoneList = new List<PhoneEntity>();
                                    foreach (PhoneEntity phoneEntity in customerEntity.PhoneList)
                                    {
                                        contactEntity.PhoneList.Add(new PhoneEntity
                                        {
                                            PhoneTypeId = phoneEntity.PhoneTypeId,
                                            PhoneNo = phoneEntity.PhoneNo
                                        });
                                    }

                                    this.SaveContact(contactEntity, null, true); // Update DummyContact without relation
                                }

                                #endregion
                            }
                            else
                            {
                                Logger.ErrorFormat("CUSTOMER ID: {0} does not exist", customerEntity.CustomerId);
                            }
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

        public bool IsDuplicateCardNo(int? customerId, int? subscriptTypeId, string cardNo)
        {
            string cardTypeCode = _context.TB_M_SUBSCRIPT_TYPE.FirstOrDefault(x => x.SUBSCRIPT_TYPE_ID == subscriptTypeId).CARD_TYPE_CODE;
            var cnt = _context.TB_M_CUSTOMER.Where(
                            x => x.CARD_NO == cardNo
                                 && x.CARD_TYPE_CODE == cardTypeCode
                                 && x.CUSTOMER_ID != customerId
                                 ).Count();
            return cnt > 0;
        }

        public List<int?> GetCustomerIdWithCallId(string phoneNo)
        {
            var query = _context.TB_M_PHONE.Where(x => x.PHONE_NO == phoneNo).Select(x => x.CUSTOMER_ID);
            if (query.Any())
            {
                return query.Distinct().ToList();
            }
            return null;
        }

        #endregion

        #region "Note"

        public IEnumerable<NoteEntity> GetNoteList(NoteSearchFilter searchFilter)
        {
            var query = from n in _context.TB_M_NOTE
                        join cs in _context.TB_R_USER on n.CREATE_USER equals cs.USER_ID
                        join us in _context.TB_R_USER on n.UPDATE_USER equals us.USER_ID
                        where n.CUSTOMER_ID == searchFilter.CustomerId
                        && (!searchFilter.EffectiveDate.HasValue || n.EFFECTIVE_DATE <= searchFilter.EffectiveDate.Value)
                        && (!searchFilter.EffectiveDate.HasValue || n.EXPIRY_DATE >= searchFilter.EffectiveDate.Value)
                        select new NoteEntity
                        {
                            CustomerId = n.CUSTOMER_ID,
                            NoteId = n.NOTE_ID,
                            Detail = n.DETAIL,
                            EffectiveDate = n.EFFECTIVE_DATE,
                            ExpiryDate = n.EXPIRY_DATE,
                            CreateDate = n.CREATE_DATE,
                            UpdateDate = n.UPDATE_DATE,
                            CreateUser = new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            },
                            UpdateUser = new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            }

                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetNoteListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<NoteEntity>();
        }

        public NoteEntity GetNoteByID(int noteId)
        {
            var query = from n in _context.TB_M_NOTE
                        join cs in _context.TB_R_USER on n.CREATE_USER equals cs.USER_ID
                        join us in _context.TB_R_USER on n.UPDATE_USER equals us.USER_ID
                        where n.NOTE_ID == noteId
                        select new NoteEntity
                        {
                            CustomerId = n.CUSTOMER_ID,
                            NoteId = n.NOTE_ID,
                            EffectiveDate = n.EFFECTIVE_DATE,
                            ExpiryDate = n.EXPIRY_DATE,
                            Detail = n.DETAIL,
                            CreateDate = n.CREATE_DATE,
                            UpdateDate = n.UPDATE_DATE,
                            CreateUser = new UserEntity
                            {
                                Firstname = cs.FIRST_NAME,
                                Lastname = cs.LAST_NAME,
                                PositionCode = cs.POSITION_CODE
                            },
                            UpdateUser = new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            }

                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public bool SaveNote(NoteEntity noteEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                TB_M_NOTE note = null;

                //Save New
                if (noteEntity.NoteId == null || noteEntity.NoteId == 0)
                {

                    note = new TB_M_NOTE();
                    note.CUSTOMER_ID = noteEntity.CustomerId;
                    note.EFFECTIVE_DATE = noteEntity.EffectiveDate;
                    note.EXPIRY_DATE = noteEntity.ExpiryDate;
                    note.DETAIL = noteEntity.Detail;
                    note.CREATE_USER = noteEntity.CreateUser.UserId;
                    note.CREATE_DATE = DateTime.Now;
                    note.UPDATE_USER = noteEntity.UpdateUser.UserId;
                    note.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_NOTE.Add(note);
                    this.Save();

                }
                else
                {
                    //Edit
                    note = _context.TB_M_NOTE.FirstOrDefault(x => x.NOTE_ID == noteEntity.NoteId);
                    if (note != null)
                    {
                        note.EFFECTIVE_DATE = noteEntity.EffectiveDate;
                        note.EXPIRY_DATE = noteEntity.ExpiryDate;
                        note.DETAIL = noteEntity.Detail;
                        note.UPDATE_USER = noteEntity.UpdateUser.UserId;
                        note.UPDATE_DATE = DateTime.Now;
                        SetEntryStateModified(note);
                        this.Save();
                    }
                    else
                    {
                        Logger.ErrorFormat("NOTE ID: {0} does not exist", noteEntity.NoteId);
                    }
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
            }

            return false;
        }

        #endregion

        #region "Document"

        public IEnumerable<AttachmentEntity> GetAttachmentList(AttachmentSearchFilter searchFilter)
        {
            var query = from at in _context.TB_M_CUSTOMER_ATTACHMENT
                        from usr in _context.TB_R_USER.Where(x => x.USER_ID == at.CREATE_USER).DefaultIfEmpty()
                        where at.CUSTOMER_ID == searchFilter.CustomerId
                        select new AttachmentEntity
                        {
                            AttachmentId = at.CUSTOMER_ATTACHMENT_ID,
                            Filename = at.FILE_NAME,
                            ContentType = at.CONTENT_TYPE,
                            Url = at.URL,
                            Name = at.ATTACHMENT_NAME,
                            Description = at.ATTACHMENT_DESC,
                            CreateDate = at.CREATE_DATE,
                            ExpiryDate = at.EXPIRY_DATE,
                            CreateUserId = at.CREATE_USER,
                            CreateUserFullName = usr != null ? usr.POSITION_CODE + " - " + usr.FIRST_NAME + " " + usr.LAST_NAME : string.Empty,
                            Status = at.STATUS,
                            DocumentLevel = Constants.DocumentLevel.Customer,
                            SrNo = "",

                        };
            var querySR = from at in _context.TB_T_SR_ATTACHMENT
                          from usr in _context.TB_R_USER.Where(x => x.USER_ID == at.CREATE_USER).DefaultIfEmpty()
                          where at.CUSTOMER_ID == searchFilter.CustomerId
                          && at.TB_T_SR.TB_C_SR_STATUS.SR_STATUS_CODE != Constants.SRStatusCode.Draft
                          select new AttachmentEntity
                          {
                              AttachmentId = at.SR_ATTACHMENT_ID,
                              Filename = at.SR_ATTACHMENT_FILE_NAME,
                              ContentType = at.SR_ATTACHMENT_CONTENT_TYPE,
                              Url = at.SR_ATTACHMENT_URL,
                              Name = at.SR_ATTACHMENT_NAME,
                              Description = at.SR_ATTACHMENT_DESC,
                              CreateDate = at.CREATE_DATE,
                              ExpiryDate = at.EXPIRY_DATE,
                              CreateUserId = at.CREATE_USER,
                              CreateUserFullName = usr != null ? usr.POSITION_CODE + " - " + usr.FIRST_NAME + " " + usr.LAST_NAME : string.Empty,
                              Status = at.STATUS,
                              DocumentLevel = Constants.DocumentLevel.Sr,
                              SrNo = at.TB_T_SR != null ? at.TB_T_SR.SR_NO : null,

                          };

            query = query.Concat(querySR); // union all

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetAttachmentListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<AttachmentEntity>();
        }

        public AttachmentEntity GetAttachmentByID(int attachmentId, string documentLevel)
        {
            if (documentLevel == Constants.DocumentLevel.Customer)
            {
                var query = from at in _context.TB_M_CUSTOMER_ATTACHMENT
                            from usr in _context.TB_R_USER.Where(x => x.USER_ID == at.CREATE_USER).DefaultIfEmpty()
                            where at.CUSTOMER_ATTACHMENT_ID == attachmentId
                            select new AttachmentEntity
                            {
                                AttachmentId = at.CUSTOMER_ATTACHMENT_ID,
                                Filename = at.FILE_NAME,
                                ContentType = at.CONTENT_TYPE,
                                Url = at.URL,
                                Name = at.ATTACHMENT_NAME,
                                Description = at.ATTACHMENT_DESC,
                                CreateDate = at.CREATE_DATE,
                                ExpiryDate = at.EXPIRY_DATE,
                                CreateUserFullName = usr != null ? usr.POSITION_CODE + " - " + usr.FIRST_NAME + " " + usr.LAST_NAME : string.Empty,
                                Status = at.STATUS,
                                DocumentLevel = Constants.DocumentLevel.Customer,
                                AttachTypeList = _context.TB_T_ATTACHMENT_TYPE.Where(x => x.CUSTOMER_ATTACHMENT_ID == at.CUSTOMER_ATTACHMENT_ID)
                                                    .Select(x => new AttachmentTypeEntity
                                                    {
                                                        AttachmentId = x.CUSTOMER_ATTACHMENT_ID,
                                                        DocTypeId = x.DOCUMENT_TYPE_ID,
                                                        Name = x.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                                    }).ToList()

                            };
                return query.Any() ? query.FirstOrDefault() : null;
            }
            else
            {
                var query = from at in _context.TB_T_SR_ATTACHMENT
                            from usr in _context.TB_R_USER.Where(x => x.USER_ID == at.CREATE_USER).DefaultIfEmpty()
                            where at.SR_ATTACHMENT_ID == attachmentId
                            select new AttachmentEntity
                            {
                                AttachmentId = at.SR_ATTACHMENT_ID,
                                Filename = at.SR_ATTACHMENT_FILE_NAME,
                                ContentType = at.SR_ATTACHMENT_CONTENT_TYPE,
                                Url = at.SR_ATTACHMENT_URL,
                                Name = at.SR_ATTACHMENT_NAME,
                                Description = at.SR_ATTACHMENT_DESC,
                                CreateDate = at.CREATE_DATE,
                                ExpiryDate = at.EXPIRY_DATE,
                                CreateUserFullName = usr != null ? usr.POSITION_CODE + " - " + usr.FIRST_NAME + " " + usr.LAST_NAME : string.Empty,
                                Status = at.STATUS,
                                DocumentLevel = Constants.DocumentLevel.Sr,
                                AttachTypeList = _context.TB_T_SR_ATTACHMENT_DOCUMENT_TYPE.Where(x => x.SR_ATTACHMENT_ID == at.SR_ATTACHMENT_ID)
                                                  .Select(x => new AttachmentTypeEntity
                                                  {
                                                      AttachmentId = x.SR_ATTACHMENT_ID,
                                                      DocTypeId = x.DOCUMENT_TYPE_ID,
                                                      Name = x.TB_M_DOCUMENT_TYPE.DOCUMENT_TYPE_NAME
                                                  }).ToList()

                            };

                return query.Any() ? query.FirstOrDefault() : null;
            }



        }

        public void SaveCustomerAttachment(AttachmentEntity attachment)
        {

            TB_M_CUSTOMER_ATTACHMENT dbAttach = null;
            var isIDNull = attachment.AttachmentId == 0 ? true : false;

            if (isIDNull)
            {
                dbAttach = new TB_M_CUSTOMER_ATTACHMENT();
                dbAttach.CUSTOMER_ATTACHMENT_ID = attachment.AttachmentId;
                dbAttach.ATTACHMENT_NAME = attachment.Name;
                dbAttach.ATTACHMENT_DESC = attachment.Description;
                dbAttach.EXPIRY_DATE = attachment.ExpiryDate;
                dbAttach.CREATE_DATE = DateTime.Now;
                dbAttach.CREATE_USER = attachment.CreateUserId;
                dbAttach.STATUS = attachment.Status;
                dbAttach.CUSTOMER_ID = attachment.CustomerId; // FK

                // Save new file
                dbAttach.FILE_NAME = attachment.Filename;
                dbAttach.URL = attachment.Url;
                dbAttach.CONTENT_TYPE = attachment.ContentType;
                _context.TB_M_CUSTOMER_ATTACHMENT.Add(dbAttach);

                // AddCustomerLog
                this.AddCustomerLog(attachment.CustomerId.Value, Constants.CustomerLog.AddDocument, attachment.CreateUserId.Value);

            }

            if (!isIDNull)
            {
                // Get previous path file
                dbAttach = _context.TB_M_CUSTOMER_ATTACHMENT.FirstOrDefault(x => x.CUSTOMER_ATTACHMENT_ID == attachment.AttachmentId);
                dbAttach.ATTACHMENT_NAME = attachment.Name;
                dbAttach.ATTACHMENT_DESC = attachment.Description;
                dbAttach.UPDATE_DATE = DateTime.Now;
                dbAttach.EXPIRY_DATE = attachment.ExpiryDate;
                dbAttach.UPDATE_USER = attachment.CreateUserId;
                dbAttach.STATUS = attachment.Status;

                if (!string.IsNullOrWhiteSpace(attachment.Url))
                {
                    string customerDocFolder = _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathCustomer).FirstOrDefault().PARAMETER_VALUE;
                    var prevFile = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", customerDocFolder, dbAttach.URL);

                    if (StreamDataHelpers.TryToDelete(prevFile))
                    {
                        // Save new file
                        dbAttach.FILE_NAME = attachment.Filename;
                        dbAttach.URL = attachment.Url;
                        dbAttach.CONTENT_TYPE = attachment.ContentType;
                    }
                }

                SetEntryStateModified(dbAttach);

                // AddCustomerLog
                this.AddCustomerLog(dbAttach.CUSTOMER_ID.Value, Constants.CustomerLog.EditDocument, attachment.CreateUserId.Value);
            }

            this.Save();

            if (attachment.AttachTypeList != null && attachment.AttachTypeList.Count > 0)
            {
                this.SaveAttachTypes(dbAttach.CUSTOMER_ATTACHMENT_ID, attachment.AttachTypeList);
            }

        }

        private void SaveAttachTypes(int attachmentId, IEnumerable<AttachmentTypeEntity> attachTypes)
        {
            foreach (AttachmentTypeEntity attachType in attachTypes)
            {
                TB_T_ATTACHMENT_TYPE dbAttachType = _context.TB_T_ATTACHMENT_TYPE.FirstOrDefault(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId
                        && x.DOCUMENT_TYPE_ID == attachType.DocTypeId);

                if (dbAttachType == null && attachType.IsDelete == false)
                {
                    dbAttachType = new TB_T_ATTACHMENT_TYPE();
                    dbAttachType.CUSTOMER_ATTACHMENT_ID = attachmentId;
                    dbAttachType.DOCUMENT_TYPE_ID = attachType.DocTypeId;
                    dbAttachType.CREATE_USER = attachType.CreateUserId;
                    dbAttachType.CREATE_DATE = DateTime.Now;
                    _context.TB_T_ATTACHMENT_TYPE.Add(dbAttachType);
                }

                if (dbAttachType != null && attachType.IsDelete == true)
                {
                    _context.TB_T_ATTACHMENT_TYPE.Remove(dbAttachType);
                }
            }

            this.Save();
        }

        public void DeleteCustomerAttachment(int attachmentId, int updateBy)
        {
            try
            {
                #region "comment out"
                //// Delete AttachType
                //var lstAttachType = _context.TB_T_ATTACHMENT_TYPE.Where(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId);
                //if (lstAttachType.Any())
                //{
                //    _context.TB_T_ATTACHMENT_TYPE.RemoveRange(lstAttachType);
                //}

                //string prevFile = string.Empty;
                //// Delete Customer Attachment
                //var custAttach = _context.TB_M_CUSTOMER_ATTACHMENT.Where(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId).FirstOrDefault();
                //if (custAttach != null)
                //{
                //    string customerDocFolder = _context.TB_C_PARAMETER.Where(x => x.PARAMETER_NAME == Constants.ParameterName.AttachmentPathCustomer).FirstOrDefault().PARAMETER_VALUE;
                //    prevFile = string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", customerDocFolder, custAttach.URL); // for delete file

                //    // AddCustomerLog
                //    this.AddCustomerLog(custAttach.CUSTOMER_ID.Value, Constants.CustomerLog.DeleteDocument, updateBy);

                //    _context.TB_M_CUSTOMER_ATTACHMENT.Remove(custAttach);
                //}

                //this.Save();

                //// Delete File
                //StreamDataHelpers.TryToDelete(prevFile);

                #endregion

                var custAttach = _context.TB_M_CUSTOMER_ATTACHMENT.Where(x => x.CUSTOMER_ATTACHMENT_ID == attachmentId).FirstOrDefault();
                if (custAttach != null)
                {
                    custAttach.STATUS = Constants.ApplicationStatus.Inactive;
                    SetEntryStateModified(custAttach);

                    // AddCustomerLog
                    this.AddCustomerLog(custAttach.CUSTOMER_ID.Value, Constants.CustomerLog.DeleteDocument, updateBy);
                }

                this.Save();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
        }

        #endregion

        #region "Existing Product"

        public IEnumerable<AccountEntity> GetAccountList(AccountSearchFilter searchFilter)
        {

            var query = from ac in _context.TB_M_ACCOUNT
                        where (!searchFilter.CustomerId.HasValue || ac.CUSTOMER_ID == searchFilter.CustomerId)
                        select new AccountEntity
                        {
                            CustomerId = ac.CUSTOMER_ID,
                            AccountId = ac.ACCOUNT_ID,
                            AccountNo = ac.ACCOUNT_NO,
                            Product = ac.SUBSCRIPTION_DESC,
                            ProductGroup = ac.PRODUCT_GROUP,
                            Status = ac.STATUS,
                            BranchCode = ac.BRANCH_CODE,
                            BranchName = ac.BRANCH_NAME,
                            EffectiveDate = ac.EFFECTIVE_DATE,
                            ExpiryDate = ac.EXPIRY_DATE,
                            Registration = ac.CAR_NO,
                            Grade = ac.GRADE,
                            CountOfPayment = "XXXXXX", // The field does not used
                            SubscriptionCode = ac.SUBSCRIPTION_CODE,
                            AccountDesc = ac.ACCOUNT_DESC

                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetAccountListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<AccountEntity>();
        }

        private static IQueryable<AccountEntity> SetAccountListSort(IQueryable<AccountEntity> accountList, AccountSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "ProductGroup":
                        return accountList.OrderBy(ord => ord.ProductGroup);
                    case "Product":
                        return accountList.OrderBy(ord => ord.Product);
                    case "Grade":
                        return accountList.OrderBy(ord => ord.Grade);
                    case "AccountNo":
                        return accountList.OrderBy(ord => ord.AccountNo);
                    case "Registration":
                        return accountList.OrderBy(ord => ord.Registration);
                    case "BranchName":
                        return accountList.OrderBy(ord => ord.BranchDisplay);
                    case "EffectiveDate":
                        return accountList.OrderBy(ord => ord.EffectiveDate);
                    case "ExpiryDate":
                        return accountList.OrderBy(ord => ord.ExpiryDate);
                    case "CountOfPayment":
                        return accountList.OrderBy(ord => ord.CountOfPayment);
                    case "Status":
                        return accountList.OrderBy(ord => (ord.Status == "A") ? "A" : "I");
                    default:
                        return accountList.OrderBy(ord => ord.ProductGroup);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "ProductGroup":
                        return accountList.OrderByDescending(ord => ord.ProductGroup);
                    case "Product":
                        return accountList.OrderByDescending(ord => ord.Product);
                    case "Grade":
                        return accountList.OrderByDescending(ord => ord.Grade);
                    case "AccountNo":
                        return accountList.OrderByDescending(ord => ord.AccountNo);
                    case "Registration":
                        return accountList.OrderByDescending(ord => ord.Registration);
                    case "BranchName":
                        return accountList.OrderByDescending(ord => ord.BranchDisplay);
                    case "EffectiveDate":
                        return accountList.OrderByDescending(ord => ord.EffectiveDate);
                    case "ExpiryDate":
                        return accountList.OrderByDescending(ord => ord.ExpiryDate);
                    case "CountOfPayment":
                        return accountList.OrderByDescending(ord => ord.CountOfPayment);
                    case "Status":
                        return accountList.OrderByDescending(ord => (ord.Status == "A") ? "A" : "I");
                    default:
                        return accountList.OrderByDescending(ord => ord.ProductGroup);
                }
            }
        }

        public ExistingProductEntity GetExistingProductDetail(ExistingProductSearchFilter searchFilter)
        {

            ExistingProductEntity productDeatil = new ExistingProductEntity();

            var queryEmail = from accm in _context.TB_M_ACCOUNT_EMAIL
                             where (!searchFilter.AccountId.HasValue || accm.ACCOUNT_ID == searchFilter.AccountId)
                             //where (!searchFilter.CustomerId.HasValue || accm.CUSTOMER_ID == searchFilter.CustomerId)
                             //&& (string.IsNullOrEmpty(searchFilter.ProductType) || accm.PRODUCT_TYPE.ToUpper() == searchFilter.ProductType.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.ProductGroup) || accm.PRODUCT_GROUP.ToUpper() == searchFilter.ProductGroup.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.SubscriptionCode) || accm.SUBSCRIPTION_CODE.ToUpper() == searchFilter.SubscriptionCode.ToUpper())
                             select new AccountEmailEntity
                             {
                                 CustomerId = accm.CUSTOMER_ID,
                                 EmailId = accm.EMAIL_ID,
                                 ProductGroup = accm.PRODUCT_GROUP,
                                 ProductType = accm.PRODUCT_TYPE,
                                 SubscriptionCode = accm.SUBSCRIPTION_CODE,
                                 EmailAccount = accm.EMAIL_ACCOUNT
                             };
            var queryPhone = from accp in _context.TB_M_ACCOUNT_PHONE
                             where (!searchFilter.AccountId.HasValue || accp.ACCOUNT_ID == searchFilter.AccountId)
                             //where (!searchFilter.CustomerId.HasValue || accp.CUSTOMER_ID == searchFilter.CustomerId)
                             //&& (string.IsNullOrEmpty(searchFilter.ProductType) || accp.PRODUCT_TYPE.ToUpper() == searchFilter.ProductType.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.ProductGroup) || accp.PRODUCT_GROUP.ToUpper() == searchFilter.ProductGroup.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.SubscriptionCode) || accp.SUBSCRIPTION_CODE.ToUpper() == searchFilter.SubscriptionCode.ToUpper())
                             orderby accp.PHONE_TYPE_CODE ascending
                             select new AccountPhoneEntity
                             {
                                 CustomerId = accp.CUSTOMER_ID,
                                 ProductType = accp.PRODUCT_TYPE,
                                 ProductGroup = accp.PRODUCT_GROUP,
                                 SubscriptionCode = accp.SUBSCRIPTION_CODE,
                                 PhoneNo = accp.PHONE_NO,
                                 PhoneExt = accp.PHONE_EXT,
                                 PhoneTypeCode = accp.PHONE_TYPE_CODE,
                                 PhoneTypeName = accp.PHONE_TYPE_NAME
                             };
            var queryAddress = from addr in _context.TB_M_ACCOUNT_ADDRESS
                               where (!searchFilter.AccountId.HasValue || addr.ACCOUNT_ID == searchFilter.AccountId)
                             //  where (!searchFilter.CustomerId.HasValue || addr.CUSTOMER_ID == searchFilter.CustomerId)
                             //&& (string.IsNullOrEmpty(searchFilter.ProductType) || addr.PRODUCT_TYPE.ToUpper() == searchFilter.ProductType.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.ProductGroup) || addr.PRODUCT_GROUP.ToUpper() == searchFilter.ProductGroup.ToUpper())
                             //&& (string.IsNullOrEmpty(searchFilter.SubscriptionCode) || addr.SUBSCRIPTION_CODE.ToUpper() == searchFilter.SubscriptionCode.ToUpper())
                               select new AccountAddressEntity
                               {
                                   CustomerId = addr.CUSTOMER_ID,
                                   ProductType = addr.PRODUCT_TYPE,
                                   ProductGroup = addr.PRODUCT_GROUP,
                                   SubscriptionCode = addr.SUBSCRIPTION_CODE,
                                   AddressTypeCode = addr.ADDRESS_TYPE_CODE,
                                   AddressTypeName = addr.ADDRESS_TYPE_NAME,
                                   AddressNo = addr.ADDRESS_NO,
                                   Village = addr.VILLAGE,
                                   Building = addr.BUILDING,
                                   FloorNo = addr.FLOOR_NO,
                                   RoomNo = addr.ROOM_NO,
                                   Moo = addr.MOO,
                                   Street = addr.STREET,
                                   Soi = addr.SOI,
                                   SubDistrict = addr.SUB_DISTRICT,
                                   District = addr.DISTRICT,
                                   Province = addr.PROVINCE,
                                   Country = addr.COUNTRY,
                                   Postcode = addr.POSTCODE,
                                   KkcisId = addr.KKCIS_ID
                               };
            productDeatil.AccountEmailList = queryEmail.Any() ? queryEmail.ToList() : null;
            productDeatil.AccountPhoneList = queryPhone.Any() ? queryPhone.ToList() : null;
            productDeatil.AccountAddressList = queryAddress.Any() ? queryAddress
                .OrderBy(ord => (ord.AddressTypeCode == "04") ? "1" :
                    ((ord.AddressTypeCode == "05") ? "2" :
                    ((ord.AddressTypeCode == "03") ? "3" : 
                    ((ord.AddressTypeCode == "02") ? "4" : 
                    ((ord.AddressTypeCode == "06") ? "5" : "6"))))
                ).ToList() : null;

            return productDeatil;
        }


        #endregion

        #region "Contact"

        public IEnumerable<ContactEntity> GetContactList(ContactSearchFilter searchFilter)
        {
            var query = from cc in _context.TB_M_CUSTOMER_CONTACT
                        from cs in _context.TB_R_USER.Where(x => x.USER_ID == cc.UPDATE_USER).DefaultIfEmpty()
                        from con in _context.TB_M_CONTACT.Where(x => x.CONTACT_ID == cc.CONTACT_ID).DefaultIfEmpty()
                        from ac in _context.TB_M_ACCOUNT.Where(x => x.CUSTOMER_ID == cc.CUSTOMER_ID && x.ACCOUNT_ID == cc.ACCOUNT_ID).DefaultIfEmpty()
                        from rl in _context.TB_M_RELATIONSHIP.Where(x => x.RELATIONSHIP_ID == cc.RELATIONSHIP_ID).DefaultIfEmpty()                        
                        where (!searchFilter.CustomerId.HasValue || cc.CUSTOMER_ID == searchFilter.CustomerId)
                        select new ContactEntity
                        {
                            CustomerId = cc.CUSTOMER_ID,
                            CustomerContactId = cc.CUSTOMER_CONTACT_ID,
                            ContactId = con.CONTACT_ID,
                            FirstNameThai = con.FIRST_NAME_TH,
                            FirstNameEnglish = con.FIRST_NAME_EN,
                            LastNameThai = con.LAST_NAME_TH,
                            LastNameEnglish = con.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.FIRST_NAME_TH : con.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.LAST_NAME_TH : con.LAST_NAME_EN,
                            CardNo = con.CARD_NO,
                            IsEdit = cc.IS_EDIT, //con.IS_EDIT,
                            System = cc.SYSTEM,
                            Account = (ac != null ? new AccountEntity
                            {
                                AccountId = ac.ACCOUNT_ID,
                                AccountNo = ac.ACCOUNT_NO,
                                Product = ac.SUBSCRIPTION_DESC,
                                ProductGroup = ac.PRODUCT_GROUP,
                                Status = ac.STATUS,
                                BranchCode = ac.BRANCH_CODE,
                                BranchName = ac.BRANCH_NAME,
                                Grade = ac.GRADE,
                                AccountDesc = ac.ACCOUNT_DESC
                            } : null),
                            Relationship = (rl != null ? new RelationshipEntity
                            {
                                RelationshipId = rl.RELATIONSHIP_ID,
                                RelationshipName = rl.RELATIONSHIP_NAME
                            } : null),
                            UpdateUser = cs != null ? new UserEntity
                            {
                                Username = cs.USERNAME
                            } : null
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetContactListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<ContactEntity>();
        }

        private static IQueryable<ContactEntity> SetContactListSort(IQueryable<ContactEntity> accountList, ContactSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "CardNo":
                        return accountList.OrderBy(ord => ord.CardNo);
                    case "FirstNameThai":
                        return accountList.OrderBy(ord => ord.FirstNameThaiEng);
                    case "LastNameThai":
                        return accountList.OrderBy(ord => ord.LastNameThaiEng);
                    case "AccountNo":
                        return accountList.OrderBy(ord => ord.Account.AccountNo);
                    case "ProductGroup":
                        return accountList.OrderBy(ord => ord.Account.ProductGroup);
                    case "Product":
                        return accountList.OrderBy(ord => ord.Account.Product);
                    case "Grade":
                        return accountList.OrderBy(ord => ord.Account.Grade);
                    case "RelationshipName":
                        return accountList.OrderBy(ord => ord.Relationship.RelationshipName);
                    case "Status":
                        return accountList.OrderBy(ord => (ord.Account.Status == "A") ? "A" : "I");
                    default:
                        return accountList.OrderBy(ord => ord.CardNo);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "CardNo":
                        return accountList.OrderByDescending(ord => ord.CardNo);
                    case "FirstNameThai":
                        return accountList.OrderByDescending(ord => ord.FirstNameThaiEng);
                    case "LastNameThai":
                        return accountList.OrderByDescending(ord => ord.LastNameThaiEng);
                    case "AccountNo":
                        return accountList.OrderByDescending(ord => ord.Account.AccountNo);
                    case "ProductGroup":
                        return accountList.OrderByDescending(ord => ord.Account.ProductGroup);
                    case "Product":
                        return accountList.OrderByDescending(ord => ord.Account.Product);
                    case "Grade":
                        return accountList.OrderByDescending(ord => ord.Account.Grade);
                    case "RelationshipName":
                        return accountList.OrderByDescending(ord => ord.Relationship.RelationshipName);
                    case "Status":
                        return accountList.OrderByDescending(ord => (ord.Account.Status == "A") ? "A" : "I");
                    default:
                        return accountList.OrderByDescending(ord => ord.CardNo);
                }
            }
        }

        public bool DeleteCustomerContact(int customerContactId, int updateBy)
        {
            bool result = false;
            try
            {
                // Delete Contact
                var objCustomerContact = _context.TB_M_CUSTOMER_CONTACT.FirstOrDefault(x => x.CUSTOMER_CONTACT_ID == customerContactId);
                if (objCustomerContact != null)
                {
                    // AddCustomerLog
                    this.AddCustomerLog(objCustomerContact.CUSTOMER_ID.Value, Constants.CustomerLog.DeleteContact, updateBy);

                    // Get for delete
                    int contactId = objCustomerContact.CONTACT_ID.Value;
                    int cntContact = _context.TB_M_CUSTOMER_CONTACT.Count(x => x.CONTACT_ID == contactId);

                    // Delete CustomerContact
                    _context.TB_M_CUSTOMER_CONTACT.Remove(objCustomerContact);

                    // Delete Contact & ContactPhone  กรณี contact มีความสัมพันธ์เดียวต้องลบ contact ด้วย
                    if (cntContact == 1)
                    {
                        var contactPhone = _context.TB_M_CONTACT_PHONE.Where(x => x.CONTACT_ID == contactId);
                        _context.TB_M_CONTACT_PHONE.RemoveRange(contactPhone);

                        var contact = _context.TB_M_CONTACT.FirstOrDefault(x => x.CONTACT_ID == contactId);
                        _context.TB_M_CONTACT.Remove(contact);
                    }

                }

                // In case FK
                try
                {
                    this.Save();
                    result = true;
                }
                catch
                {
                    result = false;
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return result;
        }

        public ContactEntity GetContactByID(int contactId)
        {
            var query = from c in _context.TB_M_CONTACT
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        from titleTH in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_TH_ID).DefaultIfEmpty()
                        from titleEN in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_EN_ID).DefaultIfEmpty()
                        where c.CONTACT_ID == contactId
                        select new ContactEntity
                        {
                            ContactId = c.CONTACT_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            BirthDate = c.BIRTH_DATE,
                            Email = c.EMAIL,
                            IsEdit = c.IS_EDIT,
                            //System = c.SYSTEM,
                            Fax = (from p in c.TB_M_CONTACT_PHONE
                                   where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax
                                   select new { p.PHONE_NO }
                                  ).FirstOrDefault().PHONE_NO,
                            SubscriptType = (st != null ? new SubscriptTypeEntity
                            {
                                SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            } : null),
                            TitleThai = (titleTH != null ? new TitleEntity
                            {
                                TitleId = titleTH.TITLE_ID,
                                TitleName = titleTH.TITLE_NAME,
                            } : null),
                            TitleEnglish = (titleEN != null ? new TitleEntity
                            {
                                TitleId = titleEN.TITLE_ID,
                                TitleName = titleEN.TITLE_NAME,
                            } : null),
                            PhoneList = (from p in c.TB_M_CONTACT_PHONE
                                         where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                         orderby p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE ascending, p.CONTACT_PHONE_ID ascending 
                                         select new PhoneEntity
                                         {
                                             ContactId = p.CONTACT_ID,
                                             PhoneTypeId = p.PHONE_TYPE_ID,
                                             PhoneId = p.CONTACT_PHONE_ID,
                                             PhoneNo = p.PHONE_NO,
                                             PhoneTypeName = p.TB_M_PHONE_TYPE.PHONE_TYPE_NAME,
                                         }).ToList()

                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        public bool SaveContact(ContactEntity contactEntity, List<CustomerContactEntity> lstCustomerContact)
        {
            return SaveContact(contactEntity, lstCustomerContact, false);
        }
        
        private bool SaveContact(ContactEntity contactEntity, List<CustomerContactEntity> lstCustomerContact, bool isAutoFromCustomer = false)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                TB_M_CONTACT contact = null;

                //Save New
                if (contactEntity.ContactId == null || contactEntity.ContactId == 0)
                {
                    contact = new TB_M_CONTACT();

                    contact.SUBSCRIPT_TYPE_ID = contactEntity.SubscriptType.SubscriptTypeId;
                    contact.CARD_NO = contactEntity.CardNo;
                    contact.BIRTH_DATE = contactEntity.BirthDate;
                    contact.TITLE_TH_ID = contactEntity.TitleThai.TitleId;
                    contact.FIRST_NAME_TH = contactEntity.FirstNameThai;
                    contact.LAST_NAME_TH = contactEntity.LastNameThai;
                    contact.TITLE_EN_ID = contactEntity.TitleEnglish.TitleId;
                    contact.FIRST_NAME_EN = contactEntity.FirstNameEnglish;
                    contact.LAST_NAME_EN = contactEntity.LastNameEnglish;
                    contact.EMAIL = contactEntity.Email;

                    contact.IS_DEFAULT = contactEntity.IsDefault; // Dummy
                    if (isAutoFromCustomer)
                    {
                        contact.IS_EDIT = false; // Dummy
                    }
                    else
                    {
                        contact.IS_EDIT = true;
                    }


                    contact.CREATE_USER = contactEntity.CreateUser.UserId;
                    contact.CREATE_DATE = DateTime.Now;
                    contact.UPDATE_USER = contactEntity.UpdateUser.UserId;
                    contact.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_CONTACT.Add(contact);

                    // AddCustomerLog
                    this.AddCustomerLog(contactEntity.CustomerId.Value, Constants.CustomerLog.AddContact, contactEntity.UpdateUser.UserId);

                    this.Save();

                    #region "Phone & Fax"

                    // Phone & Fax
                    if (contactEntity.PhoneList != null && contactEntity.PhoneList.Count() > 0)
                    {
                        foreach (PhoneEntity phoneEntity in contactEntity.PhoneList)
                        {
                            TB_M_CONTACT_PHONE contactPhone = new TB_M_CONTACT_PHONE();
                            contactPhone.CONTACT_ID = contact.CONTACT_ID; // FK
                            contactPhone.PHONE_TYPE_ID = phoneEntity.PhoneTypeId;
                            contactPhone.PHONE_NO = phoneEntity.PhoneNo;
                            contactPhone.CREATE_USER = contactEntity.CreateUser.UserId;
                            contactPhone.CREATE_DATE = DateTime.Now;
                            contactPhone.UPDATE_USER = contactEntity.UpdateUser.UserId;
                            contactPhone.UPDATE_DATE = DateTime.Now;
                            _context.TB_M_CONTACT_PHONE.Add(contactPhone);
                        }

                        this.Save();
                    }

                    #endregion

                    contactEntity.ContactId = contact.CONTACT_ID; // ContactId ที่ได้จากการ Save

                }
                else
                {
                    //Edit
                    contact = _context.TB_M_CONTACT.FirstOrDefault(x => x.CONTACT_ID == contactEntity.ContactId);
                    if (contact != null && (contact.IS_EDIT == true || isAutoFromCustomer))
                    {
                        contact.SUBSCRIPT_TYPE_ID = contactEntity.SubscriptType.SubscriptTypeId;
                        contact.CARD_NO = contactEntity.CardNo;
                        contact.BIRTH_DATE = contactEntity.BirthDate;
                        contact.TITLE_TH_ID = contactEntity.TitleThai.TitleId;
                        contact.FIRST_NAME_TH = contactEntity.FirstNameThai;
                        contact.LAST_NAME_TH = contactEntity.LastNameThai;
                        contact.TITLE_EN_ID = contactEntity.TitleEnglish.TitleId;
                        contact.FIRST_NAME_EN = contactEntity.FirstNameEnglish;
                        contact.LAST_NAME_EN = contactEntity.LastNameEnglish;
                        contact.EMAIL = contactEntity.Email;

                        contact.UPDATE_USER = contactEntity.UpdateUser.UserId;
                        contact.UPDATE_DATE = DateTime.Now;
                        SetEntryStateModified(contact);

                        // AddCustomerLog
                        this.AddCustomerLog(contactEntity.CustomerId.Value, Constants.CustomerLog.EditContact, contactEntity.UpdateUser.UserId);

                        #region "Phone & Fax"

                        // Delete Phone & Fax
                        var lstPhone = _context.TB_M_CONTACT_PHONE.Where(x => x.CONTACT_ID == contactEntity.ContactId);
                        if (lstPhone.Any())
                        {
                            foreach (var p in lstPhone)
                            {
                                _context.TB_M_CONTACT_PHONE.Remove(p);
                            }

                        }

                        // Save Phone & Fax
                        if (contactEntity.PhoneList != null && contactEntity.PhoneList.Count() > 0)
                        {
                            foreach (PhoneEntity phoneEntity in contactEntity.PhoneList)
                            {
                                TB_M_CONTACT_PHONE contactPhone = new TB_M_CONTACT_PHONE();
                                contactPhone.CONTACT_ID = contactEntity.ContactId.Value; // FK
                                contactPhone.PHONE_TYPE_ID = phoneEntity.PhoneTypeId;
                                contactPhone.PHONE_NO = phoneEntity.PhoneNo;
                                contactPhone.CREATE_USER = contactEntity.CreateUser.UserId;
                                contactPhone.CREATE_DATE = DateTime.Now;
                                contactPhone.UPDATE_USER = contactEntity.UpdateUser.UserId;
                                contactPhone.UPDATE_DATE = DateTime.Now;
                                _context.TB_M_CONTACT_PHONE.Add(contactPhone);
                            }
                        }

                        #endregion

                        this.Save();
                    }
                    else if (contact == null)
                    {
                        Logger.ErrorFormat("CONTACT ID: {0} does not exist", contactEntity.ContactId);
                    }
                }

                if (lstCustomerContact != null && lstCustomerContact.Count > 0)
                {
                    this.SaveContactRelationship(lstCustomerContact, contactEntity.ContactId.Value, contactEntity.UpdateUser.UserId);
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
            }

            return false;
        }

        public bool SaveContactSr(ContactEntity contactEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                TB_M_CONTACT contact = null;

                //Save New
                contact = new TB_M_CONTACT();

                contact.SUBSCRIPT_TYPE_ID = contactEntity.SubscriptType.SubscriptTypeId;
                contact.CARD_NO = contactEntity.CardNo;
                contact.BIRTH_DATE = contactEntity.BirthDate;
                contact.TITLE_TH_ID = contactEntity.TitleThai.TitleId;
                contact.FIRST_NAME_TH = contactEntity.FirstNameThai;
                contact.LAST_NAME_TH = contactEntity.LastNameThai;
                contact.TITLE_EN_ID = contactEntity.TitleEnglish.TitleId;
                contact.FIRST_NAME_EN = contactEntity.FirstNameEnglish;
                contact.LAST_NAME_EN = contactEntity.LastNameEnglish;
                contact.EMAIL = contactEntity.Email;
                contact.IS_DEFAULT = contactEntity.IsDefault; // Dummy
                contact.IS_EDIT = true;

                contact.CREATE_USER = contactEntity.CreateUser.UserId;
                contact.CREATE_DATE = DateTime.Now;
                contact.UPDATE_USER = contactEntity.UpdateUser.UserId;
                contact.UPDATE_DATE = DateTime.Now;
                _context.TB_M_CONTACT.Add(contact);

                // AddCustomerLog
                this.AddCustomerLog(contactEntity.CustomerId.Value, Constants.CustomerLog.AddContact, contactEntity.UpdateUser.UserId);

                this.Save();

                #region "Phone & Fax"

                // Phone & Fax
                if (contactEntity.PhoneList != null && contactEntity.PhoneList.Count() > 0)
                {
                    foreach (PhoneEntity phoneEntity in contactEntity.PhoneList)
                    {
                        TB_M_CONTACT_PHONE contactPhone = new TB_M_CONTACT_PHONE();
                        contactPhone.CONTACT_ID = contact.CONTACT_ID; // FK
                        contactPhone.PHONE_TYPE_ID = phoneEntity.PhoneTypeId;
                        contactPhone.PHONE_NO = phoneEntity.PhoneNo;
                        contactPhone.CREATE_USER = contactEntity.CreateUser.UserId;
                        contactPhone.CREATE_DATE = DateTime.Now;
                        contactPhone.UPDATE_USER = contactEntity.UpdateUser.UserId;
                        contactPhone.UPDATE_DATE = DateTime.Now;
                        _context.TB_M_CONTACT_PHONE.Add(contactPhone);
                    }

                    this.Save();
                }

                #endregion

                contactEntity.ContactId = contact.CONTACT_ID; // ContactId ที่ได้จากการ Save

                //save contact relation
                TB_M_CUSTOMER_CONTACT dbCustContact = null;
                dbCustContact = new TB_M_CUSTOMER_CONTACT();
                dbCustContact.CONTACT_ID = contact.CONTACT_ID;
                dbCustContact.CUSTOMER_ID = contactEntity.CustomerId;
                dbCustContact.ACCOUNT_ID = contactEntity.AccountId;
                dbCustContact.RELATIONSHIP_ID = contactEntity.RelationshipId;
                dbCustContact.IS_EDIT = true;

                dbCustContact.CREATE_USER = contactEntity.CreateUser.UserId;
                dbCustContact.CREATE_DATE = DateTime.Now;
                dbCustContact.UPDATE_USER = contactEntity.UpdateUser.UserId;
                dbCustContact.UPDATE_DATE = DateTime.Now;
                _context.TB_M_CUSTOMER_CONTACT.Add(dbCustContact);

                this.Save();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }

            return false;
        }

        private void SaveContactRelationship(List<CustomerContactEntity> lstCustomerContact, int contactId, int updateUser)
        {
            foreach (CustomerContactEntity customerContactEntity in lstCustomerContact)
            {
                TB_M_CUSTOMER_CONTACT dbCustContact = null;
                var isIDNull = customerContactEntity.CustomerContactId == null || customerContactEntity.CustomerContactId == 0 ? true : false;

                if (isIDNull)
                {
                    dbCustContact = new TB_M_CUSTOMER_CONTACT();
                    dbCustContact.CONTACT_ID = contactId;
                    dbCustContact.CUSTOMER_ID = customerContactEntity.CustomerId;
                    dbCustContact.ACCOUNT_ID = customerContactEntity.AccountId;
                    dbCustContact.RELATIONSHIP_ID = customerContactEntity.RelationshipId;
                    if (customerContactEntity.IsEdit.HasValue && customerContactEntity.IsEdit == false)
                    {
                        dbCustContact.IS_EDIT = false; // Dummy
                    }
                    else
                    {
                        dbCustContact.IS_EDIT = true;
                    }


                    dbCustContact.CREATE_USER = updateUser;
                    dbCustContact.CREATE_DATE = DateTime.Now;
                    dbCustContact.UPDATE_USER = updateUser;
                    dbCustContact.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_CUSTOMER_CONTACT.Add(dbCustContact);
                }
                else
                {
                    dbCustContact = _context.TB_M_CUSTOMER_CONTACT.FirstOrDefault(x => x.CUSTOMER_CONTACT_ID == customerContactEntity.CustomerContactId);
                    if (dbCustContact != null)
                    {
                        dbCustContact.ACCOUNT_ID = customerContactEntity.AccountId;
                        dbCustContact.RELATIONSHIP_ID = customerContactEntity.RelationshipId;
                        dbCustContact.UPDATE_USER = updateUser;
                        dbCustContact.UPDATE_DATE = DateTime.Now;
                        SetEntryStateModified(dbCustContact);
                    }
                    else
                    {
                        Logger.ErrorFormat("CUSTOMER_CONTACT_ID: {0} does not exist", customerContactEntity.CustomerContactId);
                    }
                }

            }

            this.Save();
        }

        public List<CustomerContactEntity> GetContactRelationshipList(int contactId, int currentCustomerId)
        {

            var query = from cc in _context.TB_M_CUSTOMER_CONTACT
                        from cs in _context.TB_R_USER.Where(x => x.USER_ID == cc.UPDATE_USER).DefaultIfEmpty()
                        from con in _context.TB_M_CONTACT.Where(x => x.CONTACT_ID == cc.CONTACT_ID).DefaultIfEmpty()
                        from ac in _context.TB_M_ACCOUNT.Where(x => x.CUSTOMER_ID == cc.CUSTOMER_ID && x.ACCOUNT_ID == cc.ACCOUNT_ID).DefaultIfEmpty()
                        from rl in _context.TB_M_RELATIONSHIP.Where(x => x.RELATIONSHIP_ID == cc.RELATIONSHIP_ID).DefaultIfEmpty()
                        from cust in _context.TB_M_CUSTOMER.Where(x => x.CUSTOMER_ID == cc.CUSTOMER_ID).DefaultIfEmpty()
                        from ttTh in _context.TB_M_TITLE.Where(x => x.TITLE_ID == cust.TITLE_TH_ID).DefaultIfEmpty()
                        from ttEn in _context.TB_M_TITLE.Where(x => x.TITLE_ID == cust.TITLE_EN_ID).DefaultIfEmpty()
                        where cc.CONTACT_ID == contactId
                        // && cc.CUSTOMER_ID == customerId // เปลี่ยนเงื่อนไข ให้ดึงทุกสัญญาที่เกี่ยวข้องกับ Contact
                        select new CustomerContactEntity
                        {
                            CustomerContactId = cc.CUSTOMER_CONTACT_ID,
                            ContactId = con.CONTACT_ID,
                            AccountId = ac.ACCOUNT_ID,
                            AccountNo = ac.ACCOUNT_NO,
                            Product = ac.SUBSCRIPTION_DESC,
                            RelationshipId = rl != null ? rl.RELATIONSHIP_ID : 0,
                            RelationshipName = rl != null ? rl.RELATIONSHIP_NAME : "",
                            IsEdit = (cc.IS_EDIT.Value && cc.CUSTOMER_ID == currentCustomerId) ? true : false,  // IsEdit เป็น true เฉพาะที่สร้างจาก csm เอง และผูกกับลูกค้าที่กำลังแก้ไขเท่านั้น
                            CustomerFullNameThaiEng = cust != null ? (!string.IsNullOrEmpty(cust.FIRST_NAME_TH) ? (cust.FIRST_NAME_TH + " " + cust.LAST_NAME_TH) : (cust.FIRST_NAME_EN + " " + cust.LAST_NAME_EN)) : "",
                            CustomerId = cc.CUSTOMER_ID,
                            AccountDesc = ac.ACCOUNT_DESC,
                            ContactFromSystem = cc.SYSTEM,
                            UpdateUser = cs != null ? new UserEntity
                            {
                                Username = cs.USERNAME
                            } : null,
                        };

            return query.OrderByDescending(o => o.IsEdit).ToList();
        }

        public List<AccountEntity> GetAccountByCustomerId(int customerId)
        {
            var query = from ac in _context.TB_M_ACCOUNT
                        where ac.CUSTOMER_ID == customerId
                        orderby ac.ACCOUNT_NO ascending
                        select new AccountEntity
                        {
                            AccountId = ac.ACCOUNT_ID,
                            AccountNo = ac.ACCOUNT_NO,
                            Product = ac.SUBSCRIPTION_DESC,
                            AccountDesc = ac.ACCOUNT_DESC
                        };

            return query.ToList();
        }

        public List<ContactEntity> xxxGetContactByPhoneNo(int? contactId, string firstNameTh, string lastNameTh, string firstNameEn,
            string lastNameEn, List<string> lstPhoneNo)
        {
            List<string> lstMyPhone = new List<string>();
            if (contactId.HasValue)
            {
                lstMyPhone = _context.TB_M_CONTACT_PHONE.Where(x => x.CONTACT_ID == contactId
                    && x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax).Select(x => x.PHONE_NO).ToList();
            }

            var query = from con in _context.TB_M_CONTACT
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == con.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        where (string.IsNullOrEmpty(firstNameTh) || con.FIRST_NAME_TH.Equals(firstNameTh))
                        && (string.IsNullOrEmpty(lastNameTh) || con.LAST_NAME_TH.Equals(lastNameTh))
                        && (string.IsNullOrEmpty(firstNameEn) || con.FIRST_NAME_EN.Equals(firstNameEn))
                        && (string.IsNullOrEmpty(lastNameEn) || con.LAST_NAME_EN.Equals(lastNameEn))
                        && (lstPhoneNo.Count == 0 || con.TB_M_CONTACT_PHONE.Count(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && lstPhoneNo.Contains(x.PHONE_NO.ToUpper())) > 0)
                        && (!contactId.HasValue || con.CONTACT_ID != contactId)
                        && (lstMyPhone.Count == 0 || con.TB_M_CONTACT_PHONE.Count(x => x.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax && !lstMyPhone.Contains(x.PHONE_NO.ToUpper())) > 0)
                        select new ContactEntity
                        {
                            ContactId = con.CONTACT_ID,
                            FirstNameThai = con.FIRST_NAME_TH,
                            FirstNameEnglish = con.FIRST_NAME_EN,
                            LastNameThai = con.LAST_NAME_TH,
                            LastNameEnglish = con.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.FIRST_NAME_TH : con.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.LAST_NAME_TH : con.LAST_NAME_EN,
                            CardNo = con.CARD_NO,
                            PhoneList = (from p in con.TB_M_CONTACT_PHONE
                                         where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                         select new PhoneEntity
                                         {
                                             PhoneTypeId = p.PHONE_TYPE_ID,
                                             PhoneNo = p.PHONE_NO
                                         }).ToList()
                        };

            return query.ToList<ContactEntity>();
        }

        public List<ContactEntity> GetContactByPhoneNo(int? contactId, string firstNameTh, string lastNameTh, string firstNameEn,
           string lastNameEn, List<string> lstPhoneNo)
        {
            string strPhoneNo1 = (lstPhoneNo.Count > 0) ? lstPhoneNo[0] : "";
            string strPhoneNo2 = (lstPhoneNo.Count > 1) ? lstPhoneNo[1] : "";
            string strPhoneNo3 = (lstPhoneNo.Count > 2) ? lstPhoneNo[2] : "";

            // Call SP_GET_CONTACT_LIST
            var lstContact = _context.SP_GET_CONTACT_LIST(contactId, firstNameTh, lastNameTh, firstNameEn, lastNameEn, strPhoneNo1, strPhoneNo2, strPhoneNo3);

            var result = (from con in lstContact
                          select new ContactEntity
                          {
                              ContactId = con.CONTACT_ID,
                              FirstNameThai = con.FIRST_NAME_TH,
                              FirstNameEnglish = con.FIRST_NAME_EN,
                              LastNameThai = con.LAST_NAME_TH,
                              LastNameEnglish = con.LAST_NAME_EN,
                              FirstNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.FIRST_NAME_TH : con.FIRST_NAME_EN,
                              LastNameThaiEng = !string.IsNullOrEmpty(con.FIRST_NAME_TH) ? con.LAST_NAME_TH : con.LAST_NAME_EN,
                              CardNo = con.CARD_NO,
                              StrPhoneNo = con.PhoneList
                          }).AsQueryable();

            return result.ToList();
        }

        #endregion

        #region "SR"

        public IEnumerable<SrEntity> GetSrList(SrSearchFilter searchFilter)
        {
            List<int> lstFilerOwnerUserId = new List<int>();
            int isNotFilterOwner = 1;
            if (searchFilter.OwnerList != null)
            {
                isNotFilterOwner = 0;
                lstFilerOwnerUserId = searchFilter.OwnerList.Select(x => x.UserId).ToList();
            }

            List<int> pageIds;
            if (!string.IsNullOrEmpty(searchFilter.CanViewSrPageIds))
            {
                try
                {
                    pageIds = searchFilter.CanViewSrPageIds.Split(',').Select(u => Convert.ToInt32(u)).ToList();
                }
                catch (Exception)
                {
                    pageIds = new List<int>();
                    searchFilter.CanViewSrPageIds = string.Empty;
                }
            }
            else
            {
                pageIds = new List<int>();
            }

            List<int> userIds = new List<int>();

            if (!(searchFilter.CanViewAllUsers ?? false) && !string.IsNullOrEmpty(searchFilter.CanViewUserIds))
            {
                try
                {
                    userIds = searchFilter.CanViewUserIds.Split(',').Select(u => Convert.ToInt32(u)).ToList();
                }
                catch (Exception ex)
                {
                    searchFilter.CanViewUserIds = string.Empty;
                    Logger.Error("Exception occur:\n", ex);
                }
            }

            var query = from sr in _context.TB_T_SR.AsNoTracking()
                            //join ownUsr in _context.TB_R_USER on sr.OWNER_USER_ID equals ownUsr.USER_ID
                        from ownUsr in _context.TB_R_USER.Where(x => x.USER_ID == sr.OWNER_USER_ID).DefaultIfEmpty()
                        from dgUsr in _context.TB_R_USER.Where(x => x.USER_ID == sr.DELEGATE_USER_ID).DefaultIfEmpty()
                        from state in _context.TB_C_SR_STATE.Where(x => x.SR_STATE_ID == sr.TB_C_SR_STATUS.SR_STATE_ID).DefaultIfEmpty()
                        where (!searchFilter.CustomerId.HasValue || sr.CUSTOMER_ID == searchFilter.CustomerId)
                        && (isNotFilterOwner == 1 || sr.SR_STATUS_ID == Constants.SRStatusId.Draft ||
                            (
                                (sr.OWNER_USER_ID.HasValue && lstFilerOwnerUserId.Contains(sr.OWNER_USER_ID.Value))
                                    || (sr.DELEGATE_USER_ID.HasValue && lstFilerOwnerUserId.Contains(sr.DELEGATE_USER_ID.Value)
                                )
                            )) // not Draft
                        && (isNotFilterOwner == 1 || sr.SR_STATUS_ID != Constants.SRStatusId.Draft ||
                            (
                                sr.SR_STATUS_ID == Constants.SRStatusId.Draft && lstFilerOwnerUserId.Contains(sr.CREATE_USER.Value)
                            )) // in case Draft
                        && (isNotFilterOwner == 1 || (sr.SR_STATUS_ID != Constants.SRStatusId.Closed && sr.SR_STATUS_ID != Constants.SRStatusId.Cancelled))
                        && ((searchFilter.CanViewAllUsers ?? false) || string.IsNullOrEmpty(searchFilter.CanViewUserIds)
                            || (sr.OWNER_USER_ID.HasValue && userIds.Contains(sr.OWNER_USER_ID.Value)
                                || (sr.DELEGATE_USER_ID.HasValue && userIds.Contains(sr.DELEGATE_USER_ID.Value))
                                || (sr.SR_STATUS_ID.HasValue && sr.SR_STATUS_ID.Value == Constants.SRStatusId.Draft && sr.CREATE_USER == searchFilter.CurrentUserId)
                                || (sr.SR_PAGE_ID.HasValue && pageIds.Contains(sr.SR_PAGE_ID.Value))
                                )
                            )
                        select new SrEntity
                        {
                            SrId = sr.SR_ID,
                            RuleAlertNo = sr.RULE_THIS_ALERT,
                            //RuleTotalWorkingHours = sr.RULE_TOTAL_WORK,
                            RuleTotalWorkingHours = sr.TB_T_SR_ACTIVITY.Sum(x => x.WORKING_MINUTE ?? 0),
                            RuleNextSLA = sr.RULE_NEXT_SLA,
                            AccountNo = sr.TB_M_ACCOUNT.ACCOUNT_NO,
                            SrNo = sr.SR_NO,
                            ChannelName = sr.TB_R_CHANNEL.CHANNEL_NAME,
                            ProductName = sr.TB_R_PRODUCT.PRODUCT_NAME,
                            AreaName = sr.TB_M_AREA.AREA_NAME,
                            SubareaName = sr.TB_M_SUBAREA.SUBAREA_NAME,
                            SrSubject = sr.SR_SUBJECT,
                            SrStateName = state.SR_STATE_NAME,
                            SrStatusName = sr.TB_C_SR_STATUS.SR_STATUS_NAME,
                            SrANo = sr.SR_ANO,
                            CreateUser = sr.CREATE_USER,
                            CreateDate = sr.CREATE_DATE,
                            UpdateDate = sr.UPDATE_DATE,
                            OwnerId = sr.OWNER_USER_ID,
                            OwnerUser = (ownUsr != null ? new UserEntity
                            {
                                Firstname = ownUsr.FIRST_NAME,
                                Lastname = ownUsr.LAST_NAME,
                                PositionCode = ownUsr.POSITION_CODE
                            } : null),
                            DelegateUser = (dgUsr != null ? new UserEntity
                            {
                                Firstname = dgUsr.FIRST_NAME,
                                Lastname = dgUsr.LAST_NAME,
                                PositionCode = dgUsr.POSITION_CODE
                            } : null),
                            Is_Secret = sr.CPN_SECRET,
                            SrPageId = sr.SR_PAGE_ID,
                            OwnerUserId = sr.TB_R_USER3.USER_ID,
                            DelegateUserId = sr.TB_R_USER6.USER_ID,
                            OwnerSupUserId = sr.TB_R_USER3.SUPERVISOR_ID,
                            DelegateSupUserId = sr.TB_R_USER6.SUPERVISOR_ID
                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetSrListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<SrEntity>();
        }

        private static IQueryable<SrEntity> SetSrListSort(IQueryable<SrEntity> accountList, SrSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "RuleAlertNo":
                        return accountList.OrderBy(ord => ord.RuleAlertNo);
                    case "RuleNextSLA":
                        return accountList.OrderBy(ord => ord.RuleNextSLA);
                    case "RuleTotalWorkingHours":
                        return accountList.OrderBy(ord => ord.RuleTotalWorkingHours);
                    case "AccountNo":
                        return accountList.OrderBy(ord => ord.AccountNo);
                    case "SrNo":
                        return accountList.OrderBy(ord => ord.SrNo);
                    case "ChannelName":
                        return accountList.OrderBy(ord => ord.ChannelName);
                    case "ProductName":
                        return accountList.OrderBy(ord => ord.ProductName);
                    case "AreaName":
                        return accountList.OrderBy(ord => ord.AreaName);
                    case "SubareaName":
                        return accountList.OrderBy(ord => ord.SubareaName);
                    case "SrSubject":
                        return accountList.OrderBy(ord => ord.SrSubject);
                    case "SrStatusName":
                        return accountList.OrderBy(ord => ord.SrStatusName);
                    case "CreateDate":
                        return accountList.OrderBy(ord => ord.CreateDate);
                    case "UpdateDate":
                        return accountList.OrderBy(ord => ord.UpdateDate);
                    case "OwnerUser":
                        return accountList.OrderBy(ord => ord.OwnerUser.PositionCode).ThenBy(ord => ord.OwnerUser.Firstname).ThenBy(ord => ord.OwnerUser.Lastname);
                    case "DelegateUser":
                        return accountList.OrderBy(ord => ord.DelegateUser.PositionCode).ThenBy(ord => ord.DelegateUser.Firstname).ThenBy(ord => ord.DelegateUser.Lastname);
                    case "SrANo":
                        return accountList.OrderBy(ord => ord.SrANo);
                    default:
                        return accountList.OrderBy(ord => ord.CreateDate);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "RuleAlertNo":
                        return accountList.OrderByDescending(ord => ord.RuleAlertNo);
                    case "RuleNextSLA":
                        return accountList.OrderByDescending(ord => ord.RuleNextSLA);
                    case "RuleTotalWorkingHours":
                        return accountList.OrderByDescending(ord => ord.RuleTotalWorkingHours);
                    case "AccountNo":
                        return accountList.OrderByDescending(ord => ord.AccountNo);
                    case "SrNo":
                        return accountList.OrderByDescending(ord => ord.SrNo);
                    case "ChannelName":
                        return accountList.OrderByDescending(ord => ord.ChannelName);
                    case "ProductName":
                        return accountList.OrderByDescending(ord => ord.ProductName);
                    case "AreaName":
                        return accountList.OrderByDescending(ord => ord.AreaName);
                    case "SubareaName":
                        return accountList.OrderByDescending(ord => ord.SubareaName);
                    case "SrSubject":
                        return accountList.OrderByDescending(ord => ord.SrSubject);
                    case "SrStatusName":
                        return accountList.OrderByDescending(ord => ord.SrStatusName);
                    case "CreateDate":
                        return accountList.OrderByDescending(ord => ord.CreateDate);
                    case "UpdateDate":
                        return accountList.OrderByDescending(ord => ord.UpdateDate);
                    case "OwnerUser":
                        return accountList.OrderByDescending(ord => ord.OwnerUser.PositionCode).ThenByDescending(ord => ord.OwnerUser.Firstname).ThenByDescending(ord => ord.OwnerUser.Lastname);
                    case "DelegateUser":
                        return accountList.OrderByDescending(ord => ord.DelegateUser.PositionCode).ThenByDescending(ord => ord.DelegateUser.Firstname).ThenByDescending(ord => ord.DelegateUser.Lastname);
                    case "SrANo":
                        return accountList.OrderByDescending(ord => ord.SrANo);
                    default:
                        return accountList.OrderByDescending(ord => ord.CreateDate);
                }
            }
        }

        #endregion

        #region "Customer Logs"

        public IEnumerable<CustomerLogEntity> GetCustomerLogList(CustomerLogSearchFilter searchFilter)
        {
            var query = from l in _context.TB_L_CUSTOMER_LOG
                        from us in _context.TB_R_USER.Where(o => o.USER_ID == l.CREATE_USER).DefaultIfEmpty()
                        where (!searchFilter.CustomerId.HasValue || l.CUSTOMER_ID == searchFilter.CustomerId)
                        select new CustomerLogEntity
                        {
                            LogId = l.LOG_ID,
                            CustomerId = l.CUSTOMER_ID,
                            Detail = l.DETAIL,
                            CreatedDate = l.CREATE_DATE,
                            User = (us != null ? new UserEntity
                            {
                                Firstname = us.FIRST_NAME,
                                Lastname = us.LAST_NAME,
                                PositionCode = us.POSITION_CODE
                            } : null)

                        };

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = query.Count();
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            query = SetCustomerLogListSort(query, searchFilter);
            return query.Skip(startPageIndex).Take(searchFilter.PageSize).ToList<CustomerLogEntity>();
        }

        private static IQueryable<CustomerLogEntity> SetCustomerLogListSort(IQueryable<CustomerLogEntity> customerLogList, CustomerLogSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "CreatedDate":
                        return customerLogList.OrderBy(ord => ord.CreatedDate);
                    case "Detail":
                        return customerLogList.OrderBy(ord => ord.Detail);
                    case "User":
                        return customerLogList.OrderBy(ord => ord.User.PositionCode).ThenBy(ord => ord.User.Firstname).ThenBy(ord => ord.User.Lastname);
                    default:
                        return customerLogList.OrderBy(ord => ord.CreatedDate);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "CreatedDate":
                        return customerLogList.OrderByDescending(ord => ord.CreatedDate);
                    case "Detail":
                        return customerLogList.OrderByDescending(ord => ord.Detail);
                    case "User":
                        return customerLogList.OrderByDescending(ord => ord.User.PositionCode).ThenByDescending(ord => ord.User.Firstname).ThenByDescending(ord => ord.User.Lastname);
                    default:
                        return customerLogList.OrderByDescending(ord => ord.CreatedDate);
                }
            }
        }

        private void AddCustomerLog(int customerId, string detail, int createUser)
        {
            TB_L_CUSTOMER_LOG custLog = new TB_L_CUSTOMER_LOG();
            custLog.CUSTOMER_ID = customerId;
            custLog.DETAIL = detail;

            custLog.CREATE_USER = createUser;
            custLog.CREATE_DATE = DateTime.Now;
            _context.TB_L_CUSTOMER_LOG.Add(custLog);
        }

        #endregion

        public bool SaveCallId(string callId, string phoneNo, string cardNo, string callType, int userId, string iVRLang)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                var duplicated = (from ci in _context.TB_T_CALL_INFO
                                  where ci.CALL_ID == callId && ci.A_NO == phoneNo
                                  select ci.CALL_INFO_ID).Any();

                if (!duplicated)
                {
                    TB_T_CALL_INFO callInfo = new TB_T_CALL_INFO
                    {
                        CALL_ID = callId,
                        A_NO = phoneNo,
                        CARD_NO = cardNo,
                        CALL_TYPE = callType,
                        IVR_LANG = iVRLang,
                        CREATE_USER = userId,
                        CREATE_DATE = DateTime.Now
                    };
                    _context.TB_T_CALL_INFO.Add(callInfo);
                    return this.Save() > 0 ? true : false;
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Duplicate Call Info").Add("CallId", callId).Add("PhoneNo", phoneNo)
                        .Add("CardNo", cardNo.MaskCardNo()).ToInputLogString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }

            return false;
        }

        public CallInfoEntity GetCallInfoByCallId(string callId)
        {
            var query = from ci in _context.TB_T_CALL_INFO
                        where ci.CALL_ID == callId
                        select new CallInfoEntity
                        {
                            CallId = ci.CALL_ID,
                            PhoneNo = ci.A_NO,
                            CardNo = ci.CARD_NO,
                            CallType = ci.CALL_TYPE,
                            IVRLang = ci.IVR_LANG
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        #region "Lookup BranchName of account"
        public List<AccountEntity> GetAccountBranchByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetAccountBranchQueryByName(searchTerm).OrderBy(x => x.BranchCode).ThenBy(x => x.BranchName)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }

        // And the total count of records
        public int GetAccountBranchCountByName(string searchTerm)
        {
            return GetAccountBranchQueryByName(searchTerm).Count();
        }
        private IQueryable<AccountEntity> GetAccountBranchQueryByName(string searchTerm)
        {
            var query = from acc in _context.TB_M_ACCOUNT.AsNoTracking()
                        where !string.IsNullOrEmpty(acc.BRANCH_NAME)
                        && (string.IsNullOrEmpty(searchTerm) || acc.BRANCH_NAME.Contains(searchTerm))
                        select new AccountEntity
                        {
                            BranchCode = acc.BRANCH_CODE,
                            BranchName = acc.BRANCH_NAME
                        };
            return query.Distinct();
        }
        #endregion

        #region "Lookup Prodduct of account"
        public List<AccountEntity> GetAccountProductByName(string searchTerm, int pageSize, int pageNum)
        {
            return GetAccountProductQueryByName(searchTerm).OrderBy(x => x.Product)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }

        // And the total count of records
        public int GetAccountProductCountByName(string searchTerm)
        {
            return GetAccountProductQueryByName(searchTerm).Count();
        }
        private IQueryable<AccountEntity> GetAccountProductQueryByName(string searchTerm)
        {
            var query = from acc in _context.TB_M_ACCOUNT.AsNoTracking()
                        where !string.IsNullOrEmpty(acc.SUBSCRIPTION_DESC)
                        && (string.IsNullOrEmpty(searchTerm) || acc.SUBSCRIPTION_DESC.Contains(searchTerm))
                        select new AccountEntity
                        {
                            Product = acc.SUBSCRIPTION_DESC
                        };
            return query.Distinct();
        }
        #endregion

        #region "Lookup Grade of account"
        public List<AccountEntity> GetAccountGradeByName(string searchTerm, string product, int pageSize, int pageNum)
        {
            return GetAccountGradeQueryByName(searchTerm, product).OrderBy(x => x.Grade)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();
        }

        // And the total count of records
        public int GetAccountGradeCountByName(string searchTerm, string product)
        {
            return GetAccountGradeQueryByName(searchTerm, product).Count();
        }
        private IQueryable<AccountEntity> GetAccountGradeQueryByName(string searchTerm, string product)
        {
            var query = from acc in _context.TB_M_ACCOUNT.AsNoTracking()
                        where !string.IsNullOrEmpty(acc.GRADE)
                        && (string.IsNullOrEmpty(searchTerm) || acc.GRADE.Contains(searchTerm))
                        && (string.IsNullOrEmpty(product) || acc.SUBSCRIPTION_DESC == product)
                        select new AccountEntity
                        {
                            Grade = acc.GRADE
                        };
            return query.Distinct();
        }
        #endregion

        public UserMarketingEntity GetUserMarketing(int customerId)
        {
            var result = (from cust in _context.TB_M_CUSTOMER
                          from mkUser in _context.TB_R_USER.Where(x => x.USER_ID == cust.EMPLOYEE_ID).DefaultIfEmpty()
                          from branch in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == mkUser.BRANCH_ID).DefaultIfEmpty()
                          from upperBranch1 in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == branch.UPPER_BRANCH_ID).DefaultIfEmpty()
                          from upperBranch2 in _context.TB_R_BRANCH.Where(x => x.BRANCH_ID == upperBranch1.UPPER_BRANCH_ID).DefaultIfEmpty()
                          where cust.CUSTOMER_ID == customerId
                                  && cust.EMPLOYEE_ID.HasValue
                                  && cust.TYPE == Constants.CustomerType.Employee
                          select new UserMarketingEntity
                          {
                              UserEntity = mkUser != null ? new UserEntity
                              {
                                  UserId = mkUser.USER_ID,
                                  PositionCode = mkUser.POSITION_CODE,
                                  Firstname = mkUser.FIRST_NAME,
                                  Lastname = mkUser.LAST_NAME,
                              } : null,
                              BranchEntity = branch != null ? new BranchEntity
                              {
                                  BranchId = branch.BRANCH_ID,
                                  BranchCode = branch.BRANCH_CODE,
                                  BranchName = branch.BRANCH_NAME,
                              } : null,
                              UpperBranch1 = upperBranch1 != null ? new BranchEntity
                              {
                                  BranchId = upperBranch1.BRANCH_ID,
                                  BranchCode = upperBranch1.BRANCH_CODE,
                                  BranchName = upperBranch1.BRANCH_NAME,
                              } : null,
                              UpperBranch2 = upperBranch2 != null ? new BranchEntity
                              {
                                  BranchId = upperBranch2.BRANCH_ID,
                                  BranchCode = upperBranch2.BRANCH_CODE,
                                  BranchName = upperBranch2.BRANCH_NAME,
                              } : null,
                          }).FirstOrDefault();

            if (result == null || result.UserEntity == null)
                return null;

            return result;
        }

        public CustomerEntity GetCustomerByEmployeeID(int employeeId)
        {
            var query = from c in _context.TB_M_CUSTOMER
                        from st in _context.TB_M_SUBSCRIPT_TYPE.Where(x => x.SUBSCRIPT_TYPE_ID == c.SUBSCRIPT_TYPE_ID).DefaultIfEmpty()
                        from titleTH in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_TH_ID).DefaultIfEmpty()
                        from titleEN in _context.TB_M_TITLE.Where(x => x.TITLE_ID == c.TITLE_EN_ID).DefaultIfEmpty()
                        where c.EMPLOYEE_ID == employeeId
                        select new CustomerEntity
                        {
                            CustomerId = c.CUSTOMER_ID,
                            FirstNameThai = c.FIRST_NAME_TH,
                            FirstNameEnglish = c.FIRST_NAME_EN,
                            LastNameThai = c.LAST_NAME_TH,
                            LastNameEnglish = c.LAST_NAME_EN,
                            FirstNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                            LastNameThaiEng = !string.IsNullOrEmpty(c.FIRST_NAME_TH) ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                            CardNo = c.CARD_NO,
                            BirthDate = c.BIRTH_DATE,
                            Email = c.EMAIL,
                            Fax = (from p in c.TB_M_PHONE
                                   where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE == Constants.PhoneTypeCode.Fax
                                   select new { p.PHONE_NO }
                                  ).FirstOrDefault().PHONE_NO,
                            SubscriptType = (st != null ? new SubscriptTypeEntity
                            {
                                SubscriptTypeId = st.SUBSCRIPT_TYPE_ID,
                                SubscriptTypeCode = st.SUBSCRIPT_TYPE_CODE,
                                SubscriptTypeName = st.SUBSCRIPT_TYPE_NAME,
                            } : null),
                            TitleThai = (titleTH != null ? new TitleEntity
                            {
                                TitleId = titleTH.TITLE_ID,
                                TitleName = titleTH.TITLE_NAME,
                            } : null),
                            TitleEnglish = (titleEN != null ? new TitleEntity
                            {
                                TitleId = titleEN.TITLE_ID,
                                TitleName = titleEN.TITLE_NAME,
                            } : null),
                            CustomerType = c.TYPE,
                            PhoneList = (from p in c.TB_M_PHONE
                                         where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                         select new PhoneEntity
                                         {
                                             CustomerId = p.CUSTOMER_ID,
                                             PhoneTypeId = p.PHONE_TYPE_ID,
                                             PhoneId = p.PHONE_ID,
                                             PhoneNo = p.PHONE_NO,
                                             PhoneTypeName = p.TB_M_PHONE_TYPE.PHONE_TYPE_NAME
                                         }).ToList()
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        #region "Functions"
        private static IQueryable<RelationshipEntity> SetRelationshipListSort(IQueryable<RelationshipEntity> relationshipList, RelationshipSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "RELATIONSHIPNAME":
                        return relationshipList.OrderBy(ord => ord.RelationshipName);
                    case "RELATIONSHIPDESC":
                        return relationshipList.OrderBy(ord => ord.RelationshipDesc);
                    case "STATUS":
                        return relationshipList.OrderBy(ord => (ord.Status == 1) ? "A" : "I");
                    case "UPDATEUSER":
                        return relationshipList.OrderBy(ord => ord.UpdateUser.PositionCode).ThenBy(ord => ord.UpdateUser.Firstname).ThenBy(ord => ord.UpdateUser.Lastname);
                    case "UPDATEDATE":
                        return relationshipList.OrderBy(ord => ord.Updatedate);
                    default:
                        return relationshipList.OrderBy(ord => ord.RelationshipId);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "RELATIONSHIPNAME":
                        return relationshipList.OrderByDescending(ord => ord.RelationshipName);
                    case "RELATIONSHIPDESC":
                        return relationshipList.OrderByDescending(ord => ord.RelationshipDesc);
                    case "STATUS":
                        return relationshipList.OrderByDescending(ord => (ord.Status == 1) ? "A" : "I");
                    case "UPDATEUSER":
                        return relationshipList.OrderByDescending(ord => ord.UpdateUser.PositionCode).ThenByDescending(ord => ord.UpdateUser.Firstname).ThenByDescending(ord => ord.UpdateUser.Lastname);
                    case "UPDATEDATE":
                        return relationshipList.OrderByDescending(ord => ord.Updatedate);
                    default:
                        return relationshipList.OrderByDescending(ord => ord.RelationshipId);
                }
            }
        }

        private static IQueryable<CustomerEntity> SetCustomerListSort(IQueryable<CustomerEntity> customerList, CustomerSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "CardNo":
                        return customerList.OrderBy(ord => ord.CardNo);
                    case "FirstNameThai":
                        return customerList.OrderBy(ord => ord.FirstNameThaiEng);
                    case "LastNameThai":
                        return customerList.OrderBy(ord => ord.LastNameThaiEng);
                    case "AccountNo":
                        return customerList.OrderBy(ord => ord.AccountNo);
                    case "Product":
                        return customerList.OrderBy(ord => ord.Account != null ? ord.Account.Product : null);
                    case "Registration":
                        return customerList.OrderBy(ord => ord.Registration);
                    case "Grade":
                        return customerList.OrderBy(ord => ord.Account != null ? ord.Account.Grade : null);
                    case "Status":
                        return customerList.OrderBy(ord => (ord.Account.Status == "A") ? "A" : "I");
                    case "BranchName":
                        //return customerList.OrderBy(ord => ord.Account.BranchDisplay);
                        return customerList.OrderBy(ord => ord.Account.BranchCode).ThenBy(ord => ord.Account.BranchName);
                    case "CustomerType":
                        return customerList.OrderBy(ord => ord.CustomerType);
                    case "SubscriptTypeName":
                        return customerList.OrderBy(ord => ord.SubscriptType.SubscriptTypeName);
                    default:
                        return customerList.OrderBy(ord => ord.CustomerId);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "CardNo":
                        return customerList.OrderByDescending(ord => ord.CardNo);
                    case "FirstNameThai":
                        return customerList.OrderByDescending(ord => ord.FirstNameThaiEng);
                    case "LastNameThai":
                        return customerList.OrderByDescending(ord => ord.LastNameThaiEng);
                    case "AccountNo":
                        return customerList.OrderByDescending(ord => ord.AccountNo);
                    case "Product":
                        return customerList.OrderByDescending(ord => ord.Account != null ? ord.Account.Product : null);
                    case "Registration":
                        return customerList.OrderByDescending(ord => ord.Registration);
                    case "Grade":
                        return customerList.OrderByDescending(ord => ord.Account != null ? ord.Account.Grade : null);
                    case "Status":
                        return customerList.OrderByDescending(ord => (ord.Account.Status == "A") ? "A" : "I");
                    case "BranchName":
                        //return customerList.OrderByDescending(ord => ord.Account.BranchDisplay);
                        return customerList.OrderByDescending(ord => ord.Account.BranchCode).ThenByDescending(ord => ord.Account.BranchName);
                    case "CustomerType":
                        return customerList.OrderByDescending(ord => ord.CustomerType);
                    case "SubscriptTypeName":
                        return customerList.OrderByDescending(ord => ord.SubscriptType.SubscriptTypeName);
                    default:
                        return customerList.OrderByDescending(ord => ord.CustomerId);
                }
            }
        }

        private static IQueryable<NoteEntity> SetNoteListSort(IQueryable<NoteEntity> noteList, NoteSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "UpdateUser":
                        return noteList.OrderBy(ord => ord.UpdateUser.PositionCode).ThenBy(ord => ord.UpdateUser.Firstname).ThenBy(ord => ord.UpdateUser.Lastname);
                    case "UpdateDate":
                        return noteList.OrderBy(ord => ord.UpdateDate);
                    default:
                        return noteList.OrderBy(ord => ord.UpdateDate);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "UpdateUser":
                        return noteList.OrderByDescending(ord => ord.UpdateUser.PositionCode).ThenByDescending(ord => ord.UpdateUser.Firstname).ThenByDescending(ord => ord.UpdateUser.Lastname);
                    case "UpdateDate":
                        return noteList.OrderByDescending(ord => ord.UpdateDate);
                    default:
                        return noteList.OrderByDescending(ord => ord.UpdateDate);
                }
            }
        }

        private static IQueryable<AttachmentEntity> SetAttachmentListSort(IQueryable<AttachmentEntity> attachmentList, AttachmentSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField)
                {
                    case "Name":
                        return attachmentList.OrderBy(ord => ord.Name);
                    case "Description":
                        return attachmentList.OrderBy(ord => ord.Description);
                    case "DocumentLevel":
                        return attachmentList.OrderBy(ord => ord.DocumentLevel);
                    case "SrNo":
                        return attachmentList.OrderBy(ord => ord.SrNo);
                    case "CreateDate":
                        return attachmentList.OrderBy(ord => ord.CreateDate);
                    case "ExpiryDate":
                        return attachmentList.OrderBy(ord => ord.ExpiryDate);
                    case "CreateUserId":
                        return attachmentList.OrderBy(ord => ord.CreateUserFullName);
                    case "Status":
                        return attachmentList.OrderBy(ord => (ord.Status == 1) ? "A" : "I");
                    default:
                        return attachmentList.OrderBy(ord => ord.ExpiryDate);
                }
            }
            else
            {
                switch (searchFilter.SortField)
                {
                    case "Name":
                        return attachmentList.OrderByDescending(ord => ord.Name);
                    case "Description":
                        return attachmentList.OrderByDescending(ord => ord.Description);
                    case "DocumentLevel":
                        return attachmentList.OrderByDescending(ord => ord.DocumentLevel);
                    case "SrNo":
                        return attachmentList.OrderByDescending(ord => ord.SrNo);
                    case "CreateDate":
                        return attachmentList.OrderByDescending(ord => ord.CreateDate);
                    case "ExpiryDate":
                        return attachmentList.OrderByDescending(ord => ord.ExpiryDate);
                    case "CreateUserId":
                        return attachmentList.OrderByDescending(ord => ord.CreateUserFullName);
                    case "Status":
                        return attachmentList.OrderByDescending(ord => (ord.Status == 1) ? "A" : "I");
                    default:
                        return attachmentList.OrderByDescending(ord => ord.ExpiryDate);
                }
            }
        }

        private static IQueryable<CustomerEntity> WildcardFilterBy(IQueryable<CustomerEntity> query, CustomerSearchFilter searchFilter)
        {
            int refSearchType = 0;

            #region "Filter by FirstName"

            if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
            {
                string firstName = searchFilter.FirstName.ExtractString(ref refSearchType);
                switch (refSearchType)
                {
                    case 1:
                        query = query.Where(x => x.FirstNameThai.StartsWith(firstName)
                            || x.FirstNameEnglish.ToUpper().StartsWith(firstName.ToUpper()));
                        break;
                    case 2:
                        query = query.Where(x => x.FirstNameThai.EndsWith(firstName)
                            || x.FirstNameEnglish.ToUpper().EndsWith(firstName.ToUpper()));
                        break;
                    case 3:
                        query = query.Where(x => x.FirstNameThai.Contains(firstName)
                            || x.FirstNameEnglish.ToUpper().Contains(firstName.ToUpper()));
                        break;
                    default:
                        query = query.Where(x => x.FirstNameThai.Equals(firstName)
                            || x.FirstNameEnglish.ToUpper().Equals(firstName.ToUpper()));
                        break;
                }
            }

            #endregion

            #region "Filter by LastName"

            refSearchType = 0;

            if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
            {
                string lastName = searchFilter.LastName.ExtractString(ref refSearchType);
                switch (refSearchType)
                {
                    case 1:
                        query = query.Where(x => x.LastNameThai.StartsWith(lastName)
                             || x.LastNameEnglish.ToUpper().StartsWith(lastName.ToUpper()));
                        break;
                    case 2:
                        query = query.Where(x => x.LastNameThai.EndsWith(lastName)
                            || x.LastNameEnglish.ToUpper().EndsWith(lastName.ToUpper()));
                        break;
                    case 3:
                        query = query.Where(x => x.LastNameThai.Contains(lastName)
                            || x.LastNameEnglish.ToUpper().Contains(lastName.ToUpper()));
                        break;
                    default:
                        query = query.Where(x => x.LastNameThai.Equals(lastName)
                            || x.LastNameEnglish.ToUpper().Equals(lastName.ToUpper()));
                        break;
                }
            }

            #endregion

            #region "Filter by Registration"

            refSearchType = 0;

            if (!string.IsNullOrWhiteSpace(searchFilter.Registration))
            {
                string registration = searchFilter.Registration.ExtractString(ref refSearchType);
                switch (refSearchType)
                {
                    case 1:
                        query = query.Where(x => x.Registration.StartsWith(registration));
                        break;
                    case 2:
                        query = query.Where(x => x.Registration.EndsWith(registration));
                        break;
                    case 3:
                        query = query.Where(x => x.Registration.Contains(registration));
                        break;
                    default:
                        query = query.Where(x => x.Registration.Equals(registration));
                        break;
                }
            }

            #endregion

            return query;
        }

        #endregion

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
