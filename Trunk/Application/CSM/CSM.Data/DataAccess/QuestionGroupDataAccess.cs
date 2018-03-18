using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Globalization;

namespace CSM.Data.DataAccess
{
    public class QuestionGroupDataAccess : IQuestionGroupDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QuestionGroupDataAccess));

        public QuestionGroupDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "QuestionGroup"

        public IEnumerable<QuestionGroupQuestionItemEntity> GetQuestionListById(QuestionGroupInQuestionSearchFilter searchFilter)
        {
            var resultQuery = (from questionGroupQuestion in _context.TB_M_QUESTIONGROUP_QUESTION
                               from createUser in
                                   _context.TB_R_USER.Where(x => x.USER_ID == questionGroupQuestion.TB_M_QUESTION.CREATE_USER)
                                       .DefaultIfEmpty()
                               from updateUser in
                                   _context.TB_R_USER.Where(x => x.USER_ID == questionGroupQuestion.TB_M_QUESTION.UPDATE_USER)
                                       .DefaultIfEmpty()
                               where
                                   (questionGroupQuestion == null ||
                                    questionGroupQuestion.QUESTIONGROUP_ID == searchFilter.QuestionGroupId)
                               select new QuestionGroupQuestionItemEntity
                               {
                                   QuestionGroupQuestionId = questionGroupQuestion.QUESTIONGROUP_QUESTION_ID,
                                   QuestionId = questionGroupQuestion.QUESTION_ID,
                                   QuestionName = questionGroupQuestion.TB_M_QUESTION.QUESTION_NAME,
                                   Status = (bool)questionGroupQuestion.TB_M_QUESTION.IS_ACTIVE,
                                   SeqNo = questionGroupQuestion.SEQ_NO,
                                   UpdateUserName = (updateUser != null
                                       ? new UserEntity
                                       {
                                           PositionCode = updateUser.POSITION_CODE,
                                           Firstname = updateUser.FIRST_NAME,
                                           Lastname = updateUser.LAST_NAME
                                       }
                                       : null),
                                   CreateUserName = (createUser != null
                                       ? new UserEntity
                                       {
                                           PositionCode = createUser.POSITION_CODE,
                                           Firstname = createUser.FIRST_NAME,
                                           Lastname = createUser.LAST_NAME
                                       }
                                       : null),
                                   UpdateDate = questionGroupQuestion.TB_M_QUESTION.UPDATE_DATE,
                                   CreateDate = questionGroupQuestion.TB_M_QUESTION.CREATE_DATE
                               });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetQuestionGroupQuesionListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public List<QuestionItemEntity> SearchQuestionGroupQuestion(int offset, int limit, string questionName, string questionIdList, ref int totalCount)
        {
            var resultQuery = (from question in _context.TB_M_QUESTION
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == question.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == question.UPDATE_USER).DefaultIfEmpty()
                               where (questionName == null || question.QUESTION_NAME.Contains(questionName))
                               select new QuestionItemEntity
                               {
                                   QuestionId = question.QUESTION_ID,
                                   QuestionName = question.QUESTION_NAME,
                                   IsActive = question.IS_ACTIVE != null && (bool)question.IS_ACTIVE ? "Active" : "Inactive",
                                   UpdateUserName = (updateUser != null ? new UserEntity()
                                   {
                                       PositionCode = updateUser.POSITION_CODE,
                                       Firstname = updateUser.FIRST_NAME,
                                       Lastname = updateUser.LAST_NAME
                                   } : null),
                                   CreateUserName = (createUser != null ? new UserEntity()
                                   {
                                       PositionCode = createUser.POSITION_CODE,
                                       Firstname = createUser.FIRST_NAME,
                                       Lastname = createUser.LAST_NAME
                                   } : null),
                                   UpdateDate = question.UPDATE_DATE.HasValue ? question.UPDATE_DATE : question.CREATE_DATE,
                                   CreateUser = (int)question.CREATE_USER,
                                   UpdateUser = (int)question.UPDATE_USER
                               });

            return resultQuery.ToList();
        }

        public IEnumerable<QuestionGroupItemEntity> GetQuestionGroupList(QuestionGroupSearchFilter searchFilter)
        {
            var questionGroupListStatus = searchFilter.Status == "all" ? null : searchFilter.Status.ToNullable<bool>();
            var resultQuery = (from questionGroup in _context.TB_M_QUESTIONGROUP
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.UPDATE_USER).DefaultIfEmpty()
                               where ((searchFilter.QuestionGroupName == null || questionGroup.QUESTIONGROUP_NAME.Contains(searchFilter.QuestionGroupName)) &&
                                      (!searchFilter.ProductId.HasValue || questionGroup.PRODUCT_ID == searchFilter.ProductId) &&
                                      (!questionGroupListStatus.HasValue || questionGroup.QUESTIONGROUP_IS_ACTIVE == questionGroupListStatus))
                               select new QuestionGroupItemEntity
                               {
                                   QuestionGroupId = questionGroup.QUESTIONGROUP_ID,
                                   QuestionGroupName = questionGroup.QUESTIONGROUP_NAME,
                                   QuestionNo = questionGroup.TB_M_QUESTIONGROUP_QUESTION.Count(),
                                   ProductName = questionGroup.TB_R_PRODUCT.PRODUCT_NAME,
                                   Status = questionGroup.QUESTIONGROUP_IS_ACTIVE,
                                   UpdateUserName = (updateUser != null ? new UserEntity
                                   {
                                       PositionCode = updateUser.POSITION_CODE,
                                       Firstname = updateUser.FIRST_NAME,
                                       Lastname = updateUser.LAST_NAME
                                   } : null),
                                   UpdateDate = questionGroup.UPDATE_DATE
                               });

            resultQuery = resultQuery.OrderBy(r => r.QuestionGroupName);

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            //            resultQuery = SetQuestionGroupListSort(resultQuery, searchFilter);

            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                TB_M_QUESTIONGROUP questionGroup;

                if (!questionGroupItemEntity.QuestionGroupId.HasValue)
                {
                    //save
                    questionGroup = new TB_M_QUESTIONGROUP();
                    questionGroup.QUESTIONGROUP_NAME = questionGroupItemEntity.QuestionGroupName;
                    questionGroup.QUESTIONGROUP_IS_ACTIVE = questionGroupItemEntity.Status;
                    questionGroup.PRODUCT_ID = questionGroupItemEntity.QuestionGroupProductId;
                    questionGroup.CREATE_USER = questionGroupItemEntity.UserId;
                    questionGroup.CREATE_DATE = DateTime.Now;
                    questionGroup.UPDATE_USER = questionGroupItemEntity.UserId;
                    questionGroup.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_QUESTIONGROUP.Add(questionGroup);
                    Save();
                }
                else
                {
                    //save
                    questionGroup = new TB_M_QUESTIONGROUP();
                    questionGroup.QUESTIONGROUP_NAME = questionGroupItemEntity.QuestionGroupName;
                    questionGroup.QUESTIONGROUP_IS_ACTIVE = questionGroupItemEntity.Status;
                    questionGroup.PRODUCT_ID = questionGroupItemEntity.QuestionGroupProductId;
                    questionGroup.UPDATE_USER = questionGroupItemEntity.UserId;
                    questionGroup.UPDATE_DATE = DateTime.Now;
                    questionGroup.QUESTIONGROUP_ID = (int)questionGroupItemEntity.QuestionGroupId;
                    SetEntryStateModified(questionGroup);
                    Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return false;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public QuestionGroupItemEntity GetQuestionGroupById(int questionGroupId)
        {
            var query = (from questionGroup in _context.TB_M_QUESTIONGROUP
                         from createUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.CREATE_USER).DefaultIfEmpty()
                         from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.UPDATE_USER).DefaultIfEmpty()
                         where questionGroup.QUESTIONGROUP_ID == questionGroupId
                         select new QuestionGroupItemEntity
                         {
                             QuestionGroupId = questionGroup.QUESTIONGROUP_ID,
                             Status = questionGroup.QUESTIONGROUP_IS_ACTIVE,
                             QuestionGroupName = questionGroup.QUESTIONGROUP_NAME,
                             QuestionGroupProduct = questionGroup.TB_R_PRODUCT.PRODUCT_NAME,
                             QuestionGroupProductId = questionGroup.PRODUCT_ID,
                             Description = questionGroup.QUESTIONGROUP_DESC,
                             CreateDate = questionGroup.CREATE_DATE,
                             UpdateDate = questionGroup.UPDATE_DATE,
                             CreateUserName = (createUser != null ? new UserEntity
                             {
                                 PositionCode = createUser.POSITION_CODE,
                                 Firstname = createUser.FIRST_NAME,
                                 Lastname = createUser.LAST_NAME
                             } : null),
                             UpdateUserName = (updateUser != null ? new UserEntity
                             {
                                 PositionCode = updateUser.POSITION_CODE,
                                 Firstname = updateUser.FIRST_NAME,
                                 Lastname = updateUser.LAST_NAME
                             } : null)
                         }).SingleOrDefault();

            return query;
        }

        #endregion

        public List<QuestionGroupDuplicateEntity> GetQuestionGroupDuplicates(List<int> productIds, string questionGroupName, int? questionGroupId)
        {
            return (from grp in _context.TB_M_QUESTIONGROUP
                    from prd in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == grp.PRODUCT_ID).DefaultIfEmpty()
                    where productIds.Contains(grp.PRODUCT_ID)
                        && grp.QUESTIONGROUP_NAME.Trim() == questionGroupName
                        && (!questionGroupId.HasValue || (questionGroupId.HasValue && grp.QUESTIONGROUP_ID != questionGroupId.Value))
                    select new
                    {
                        grp.QUESTIONGROUP_ID,
                        prd.PRODUCT_ID,
                        prd.PRODUCT_NAME
                    }).ToList()
                          .Select(x => new QuestionGroupDuplicateEntity
                          {
                              QuestionGroupId = x.QUESTIONGROUP_ID,
                              QuestionGroupName = questionGroupName,
                              ProductId = x.PRODUCT_ID,
                              ProductName = x.PRODUCT_NAME
                          }).ToList();
        }

        public bool SaveQuestionGroup(QuestionGroupItemEntity questionGroupItemEntity, string idQuestions)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var isEdit = questionGroupItemEntity.QuestionGroupId.HasValue;
                TB_M_QUESTIONGROUP questionGroup;

                if (!isEdit)
                {
                    //add
                    questionGroup = new TB_M_QUESTIONGROUP();
                }
                else
                {
                    questionGroup = _context.TB_M_QUESTIONGROUP.SingleOrDefault(a => a.QUESTIONGROUP_ID == questionGroupItemEntity.QuestionGroupId.Value);
                    if (questionGroup == null)
                    {
                        Logger.ErrorFormat("QUESITON GROUP ID: {0} does not exist", questionGroupItemEntity.QuestionGroupId);
                        return false;
                    }
                }

                questionGroup.QUESTIONGROUP_NAME = questionGroupItemEntity.QuestionGroupName;
                questionGroup.PRODUCT_ID = questionGroupItemEntity.QuestionGroupProductId;
                questionGroup.QUESTIONGROUP_IS_ACTIVE = questionGroupItemEntity.Status;
                questionGroup.QUESTIONGROUP_DESC = questionGroupItemEntity.Description;
                questionGroup.UPDATE_USER = questionGroupItemEntity.UserId;
                questionGroup.UPDATE_DATE = DateTime.Now;

                if (!isEdit)
                {
                    questionGroup.CREATE_USER = questionGroupItemEntity.UserId;
                    questionGroup.CREATE_DATE = DateTime.Now;
                    _context.TB_M_QUESTIONGROUP.Add(questionGroup);
                    this.Save();

                    //save area_subarea
                    if (!string.IsNullOrEmpty(idQuestions))
                    {
                        string[] idQuestionsArray = idQuestions.Split(',');

                        int i = 1;
                        foreach (var idQuestion in idQuestionsArray)
                        {
                            var questionGroupQuestion = new TB_M_QUESTIONGROUP_QUESTION();
                            questionGroupQuestion.QUESTIONGROUP_ID = questionGroup.QUESTIONGROUP_ID;
                            questionGroupQuestion.QUESTION_ID = Convert.ToInt32(idQuestion, CultureInfo.InvariantCulture);
                            questionGroupQuestion.SEQ_NO = i;
                            _context.TB_M_QUESTIONGROUP_QUESTION.Add(questionGroupQuestion);
                            Save();
                            i++;
                        }
                    }
                }
                else
                {
                    SetEntryStateModified(questionGroup);

                    //delete
                    var questionGroupQuestionList = _context.TB_M_QUESTIONGROUP_QUESTION.Where(a => a.QUESTIONGROUP_ID == questionGroupItemEntity.QuestionGroupId);
                    foreach (var questionGroupQuestionItem in questionGroupQuestionList)
                    {
                        _context.TB_M_QUESTIONGROUP_QUESTION.Remove(questionGroupQuestionItem);
                    }
                    Save();

                    //update
                    if (!string.IsNullOrEmpty(idQuestions))
                    {
                        string[] idQuestionsArray = idQuestions.Split(',');
                        int i = 1;
                        foreach (var idQuestion in idQuestionsArray)
                        {
                            var questionGroupQuestion = new TB_M_QUESTIONGROUP_QUESTION();
                            questionGroupQuestion.QUESTIONGROUP_ID = questionGroup.QUESTIONGROUP_ID;
                            questionGroupQuestion.QUESTION_ID = Convert.ToInt32(idQuestion, CultureInfo.InvariantCulture);
                            questionGroupQuestion.SEQ_NO = i;
                            _context.TB_M_QUESTIONGROUP_QUESTION.Add(questionGroupQuestion);
                            Save();
                            i++;
                        }
                    }
                }

                Save();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return false;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public List<QuestionGroupProductEntity> GetProductList(string prefix, ref int totalCount)
        {
            var query = _context.TB_R_PRODUCT.AsQueryable();
            var questionGroupProductEntity = new QuestionGroupProductEntity();

            query = query.Where(q => q.PRODUCT_NAME.Contains(prefix)).OrderBy(q => q.PRODUCT_NAME);

            totalCount = query.Count();

            questionGroupProductEntity.QuestionGroupProductList = query.Select(item => new QuestionGroupProductEntity
            {
                Product = item.PRODUCT_NAME,
                ProductGroup = item.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
            }).ToList();

            return questionGroupProductEntity.QuestionGroupProductList;
        }

        public List<QuestionItemEntity> GetQuestionList(QuestionSelectSearchFilter searchFilter)
        {
            var resultQuery = (from question in _context.TB_M_QUESTION
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == question.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == question.UPDATE_USER).DefaultIfEmpty()
                               //where (searchFilter.QuestionName == null || question.QUESTION_NAME.Contains(searchFilter.QuestionName))
                               where (searchFilter.QuestionName == null || question.QUESTION_NAME.Contains(searchFilter.QuestionName)) && question.IS_ACTIVE == true
                               select new QuestionItemEntity
                               {
                                   QuestionId = question.QUESTION_ID,
                                   QuestionName = question.QUESTION_NAME,
                                   IsActive = question.IS_ACTIVE != null && (bool)question.IS_ACTIVE ? "Active" : "Inactive",
                                   UpdateUserName = (updateUser != null ? new UserEntity
                                   {
                                       PositionCode = updateUser.POSITION_CODE,
                                       Firstname = updateUser.FIRST_NAME,
                                       Lastname = updateUser.LAST_NAME
                                   } : null),
                                   CreateUserName = (createUser != null ? new UserEntity
                                   {
                                       PositionCode = createUser.POSITION_CODE,
                                       Firstname = createUser.FIRST_NAME,
                                       Lastname = createUser.LAST_NAME
                                   } : null),
                                   UpdateDate = question.UPDATE_DATE.HasValue ? question.UPDATE_DATE : question.CREATE_DATE,
                                   CreateUser = (int)question.CREATE_USER,
                                   UpdateUser = (int)question.UPDATE_USER
                               });

            if (!string.IsNullOrEmpty(searchFilter.QuestionIdList))
            {
                var questionIdArray = searchFilter.QuestionIdList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                resultQuery = resultQuery.Where(q => !questionIdArray.Contains(q.QuestionId.Value));
            }

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetQuestionListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public List<QuestionItemEntity> GetQuestionList(int questionGroupId)
        {
            return _context.TB_M_QUESTIONGROUP_QUESTION
                .Where(x => x.QUESTIONGROUP_ID == questionGroupId && (x.TB_M_QUESTION.IS_ACTIVE ?? false))
                .OrderBy(x => x.SEQ_NO)
                .Select(x => new QuestionItemEntity()
                {
                    SeqNo = x.SEQ_NO,
                    QuestionId = x.QUESTION_ID,
                    QuestionName = x.TB_M_QUESTION.QUESTION_NAME
                }).ToList();
        }

        #region "Functions"

        private static IQueryable<QuestionItemEntity> SetQuestionListSort(IQueryable<QuestionItemEntity> areaList, QuestionSelectSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return areaList.OrderBy(a => a.QuestionName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return areaList.OrderByDescending(a => a.QuestionName);
                }
            }
        }

        private static IQueryable<QuestionGroupQuestionItemEntity> SetQuestionGroupQuesionListSort(IQueryable<QuestionGroupQuestionItemEntity> questionGroupList, QuestionGroupInQuestionSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return
                            questionGroupList.OrderBy(a => a.SeqNo);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return
                            questionGroupList.OrderByDescending(a => a.SeqNo);
                }
            }
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
