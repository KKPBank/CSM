using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Globalization;

namespace CSM.Data.DataAccess
{
    public class QuestionDataAccess : IQuestionDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QuestionDataAccess));
        public QuestionDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "Question"

        public IEnumerable<QuestionItemEntity> GetQuestionList(QuestionSearchFilter searchFilter)
        {
            var questionStatus = searchFilter.Status == "all" ? null : searchFilter.Status.ToNullable<bool>();
            var resultQuery = (from question in _context.TB_M_QUESTION
                               from createUser in _context.TB_R_USER.Where(x => x.USER_ID == question.CREATE_USER).DefaultIfEmpty()
                               from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == question.UPDATE_USER).DefaultIfEmpty()
                               where ((searchFilter.QuestionName == null || question.QUESTION_NAME.Contains(searchFilter.QuestionName)) &&
                                      (!questionStatus.HasValue || question.IS_ACTIVE == questionStatus ))
                               select new QuestionItemEntity
                               {
                                   QuestionId = question.QUESTION_ID,
                                   QuestionName = question.QUESTION_NAME,
                                   IsActive = question.IS_ACTIVE != null && (bool) question.IS_ACTIVE ? "Active" : "Inactive",
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
                                   CreateUser = (int) question.CREATE_USER,
                                   UpdateUser = (int) question.UPDATE_USER
                               });
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

        public bool SaveQuestion(QuestionItemEntity questionItemEntity)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                TB_M_QUESTION question;

                if (!questionItemEntity.QuestionId.HasValue)
                {
                    //save
                    question = new TB_M_QUESTION();
                    question.QUESTION_NAME = questionItemEntity.QuestionName;
                    question.IS_ACTIVE = questionItemEntity.Status;
                    question.CREATE_USER = questionItemEntity.UserId;
                    question.CREATE_DATE = DateTime.Now;
                    question.UPDATE_USER = questionItemEntity.UserId;
                    question.UPDATE_DATE = DateTime.Now;
                    _context.TB_M_QUESTION.Add(question);
                    Save();
                }
                else
                {
                    //save
                    question = new TB_M_QUESTION();
                    question.QUESTION_NAME = questionItemEntity.QuestionName;
                    question.IS_ACTIVE = questionItemEntity.Status;
                    question.CREATE_USER = questionItemEntity.CreateUser;
                    question.CREATE_DATE = questionItemEntity.CreateDate;
                    question.UPDATE_USER = questionItemEntity.UserId;
                    question.UPDATE_DATE = DateTime.Now;
                    question.QUESTION_ID = (int)questionItemEntity.QuestionId;
                    SetEntryStateModified(question);
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

        #endregion

        public bool CheckQuestionName(QuestionItemEntity questionItemEntity)
        {
            var questionName = questionItemEntity.QuestionName;

            if (!questionItemEntity.QuestionId.HasValue)
            {
                var query = _context.TB_M_QUESTION.Where(x => x.QUESTION_NAME.ToUpper().Equals(questionName.ToUpper()));
                var count = query.Count();
                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var query = _context.TB_M_QUESTION.Where(x => x.QUESTION_NAME.ToUpper().Equals(questionName.ToUpper()) && x.QUESTION_ID != questionItemEntity.QuestionId);
                var count = query.Count();
                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        
        public QuestionItemEntity GetQuestionById(int questionId)
        {
            var query = from ques in _context.TB_M_QUESTION
                        from createUser in _context.TB_R_USER.Where(x => x.USER_ID == ques.CREATE_USER).DefaultIfEmpty()
                        from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == ques.UPDATE_USER).DefaultIfEmpty()
                        where ques.QUESTION_ID == questionId
                        select new QuestionItemEntity
                        {
                            QuestionId = ques.QUESTION_ID,
                            Status = (bool) ques.IS_ACTIVE,
                            QuestionName = ques.QUESTION_NAME,
                            CreateDate = ques.CREATE_DATE,
                            UpdateDate = ques.UPDATE_DATE,
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
                            } : null),
                            CreateUser = (int) ques.CREATE_USER,
                            UpdateUser = (int) ques.UPDATE_USER
                        };

            return query.Any() ? query.FirstOrDefault() : null;
        }

        #region "Functions"

        private static IQueryable<QuestionItemEntity> SetQuestionListSort(IQueryable<QuestionItemEntity> areaList, QuestionSearchFilter searchFilter)
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
