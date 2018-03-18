using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace CSM.Data.DataAccess
{
    public class BranchDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BranchDataAccess));
        public BranchDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<BranchEntity> AutoCompleteSearchBranch(string keyword, int limit)
        {
            var query = _context.TB_R_BRANCH.AsQueryable();

            query = query.Where(q => q.STATUS == 1);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.BRANCH_NAME.Contains(keyword));
            }

            query = query.OrderBy(q => q.BRANCH_NAME);

            return query.Take(limit).Select(item => new BranchEntity
            {
                BranchId = item.BRANCH_ID,
                BranchName = item.BRANCH_NAME,
            }).ToList();
        }
        public List<BranchEntity> GetBranchByBranchIds(string keyword, List<int> branchIds, int limit)
        {
            var query = _context.TB_R_BRANCH.AsQueryable();

            query = query.Where(q => q.STATUS == 1);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.BRANCH_NAME.Contains(keyword));
            }

            if (branchIds != null && branchIds.Count > 0)
            {
                query = query.Where(q => branchIds.Contains(q.BRANCH_ID));
            }

            query = query.OrderBy(q => q.BRANCH_NAME);

            return query.Take(limit).Select(item => new BranchEntity
            {
                BranchId = item.BRANCH_ID,
                BranchName = item.BRANCH_NAME,
            }).ToList();
        }
        
        public int? GetBranchIdByBranchCode(string branchCode)
        {
            var item = _context.TB_R_BRANCH.Where(x => x.BRANCH_CODE.Trim().ToUpper() == branchCode.Trim().ToUpper()).Select(x => new { x.BRANCH_ID, x.BRANCH_CODE }).FirstOrDefault();
            if (item == null)
                return null;
            else
                return item.BRANCH_ID;
        }

        public InsertOrUpdateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request, int channelId, int? upperBranchId)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var result = new InsertOrUpdateBranchResponse();

                var dbBranch = _context.TB_R_BRANCH.Where(x => x.BRANCH_CODE.Trim().ToUpper() == request.BranchCode.Trim().ToUpper()).FirstOrDefault();

                if (dbBranch == null)
                {
                    dbBranch = new TB_R_BRANCH();
                    result.IsNewBranch = true;
                }
                else
                {
                    result.IsNewBranch = false;

                    // IF (Change Branch) Or (Change Status 1 to 0)
                    if (dbBranch.STATUS == 1 && request.Status == 0)
                    {
                        #region == Validate Relation Between Branch to Pool ==

                        var count = _context.TB_M_POOL_BRANCH.Count(x => x.BRANCH_ID == dbBranch.BRANCH_ID && x.STATUS == 1);

                        if (count > 0)
                        {
                            result.IsSuccess = false;
                            result.ErrorCode = "6";
                            result.ErrorMessage = "ไม่สามารถอัพเดตข้อมูลเป็นปิดสาขาได้ เนื่องจาก มีการผูกสาขากับ Communication Pool";
                            return result;
                        }

                        #endregion
                    }
                }

                dbBranch.CHANNEL_ID = channelId;
                dbBranch.BRANCH_CODE = ValueOrDefault(request.BranchCode);
                dbBranch.BRANCH_NAME = ValueOrDefault(request.BranchName);
                dbBranch.STATUS = Convert.ToInt16(request.Status);
                dbBranch.UPPER_BRANCH_ID = upperBranchId;
                dbBranch.START_TIME_HOUR = Convert.ToInt16(request.StartTimeHour);
                dbBranch.START_TIME_MINUTE = Convert.ToInt16(request.StartTimeMinute);
                dbBranch.END_TIME_HOUR = Convert.ToInt16(request.EndTimeHour);
                dbBranch.END_TIME_MINUTE = Convert.ToInt16(request.EndTimeMinute);

                dbBranch.BRANCH_HOME_NO = ValueOrDefault(request.HomeNo);
                dbBranch.BRANCH_MOO = ValueOrDefault(request.Moo);
                dbBranch.BRANCH_BUILDING = ValueOrDefault(request.Building);
                dbBranch.BRANCH_FLOOR = ValueOrDefault(request.Floor);
                dbBranch.BRANCH_SOI = ValueOrDefault(request.Soi);
                dbBranch.BRANCH_STREET = ValueOrDefault(request.Street);
                dbBranch.BRANCH_PROVINCE = ValueOrDefault(request.Province);
                dbBranch.BRANCH_AMPHUR = ValueOrDefault(request.Amphur);
                dbBranch.BRANCH_TAMBOL = ValueOrDefault(request.Tambol);
                dbBranch.BRANCH_ZIPCODE = ValueOrDefault(request.Zipcode);

                var now = DateTime.Now;

                if (result.IsNewBranch)
                {
                    dbBranch.CREATE_USER = ValueOrDefault(request.ActionUsername);
                    dbBranch.UPDATE_USER = ValueOrDefault(request.ActionUsername);
                    dbBranch.CREATE_DATE = now;
                    dbBranch.UPDATE_DATE = now;

                    _context.TB_R_BRANCH.Add(dbBranch);
                }
                else
                {
                    dbBranch.UPDATE_USER = ValueOrDefault(request.ActionUsername);
                    dbBranch.UPDATE_DATE = DateTime.Now;
                    SetEntryStateModified(dbBranch);
                }

                this.Save();

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);

                return new InsertOrUpdateBranchResponse()
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message,
                };
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = false;
            }
        }

        private static string ValueOrDefault(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.Trim();
        }

        public UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request, List<int> branchIds)
        {
            var holidayDate = request.HolidayDate.Date;

            if (request.UpdateMode == 1)
            {

                var calendars = _context.TB_R_BRANCH_CALENDAR.Where(x => EntityFunctions.TruncateTime(x.HOLIDAY_DATE) == holidayDate).ToList();
                _context.TB_R_BRANCH_CALENDAR.RemoveRange(calendars);

                Save();

                var now = DateTime.Now;

                foreach (var branchId in branchIds)
                {
                    var calendar = new TB_R_BRANCH_CALENDAR();
                    calendar.HOLIDAY_DATE = request.HolidayDate.Date;
                    calendar.HOLIDAY_DESC = request.HolidayDesc;
                    calendar.BRANCH_ID = branchId;
                    calendar.CREATE_USER = request.ActionUsername;
                    calendar.UPDATE_USER = request.ActionUsername;
                    calendar.CREATE_DATE = now;
                    calendar.UPDATE_DATE = now;

                    _context.TB_R_BRANCH_CALENDAR.Add(calendar);
                }

                Save();
            }

            if (request.UpdateMode == 2)
            {
                var existingBranchList = _context.TB_R_BRANCH_CALENDAR.Where(x => EntityFunctions.TruncateTime(x.HOLIDAY_DATE) == holidayDate).ToList();

                var now = DateTime.Now;

                foreach (var branchId in branchIds)
                {
                    var calendar = existingBranchList.SingleOrDefault(x => x.BRANCH_ID == branchId);

                    if (calendar != null)
                    {
                        // Update
                        calendar.HOLIDAY_DESC = request.HolidayDesc;
                        calendar.UPDATE_USER = request.ActionUsername;
                        calendar.UPDATE_DATE = now;

                        SetEntryStateModified(calendar);
                    }
                    else
                    {
                        calendar = new TB_R_BRANCH_CALENDAR();
                        calendar.HOLIDAY_DATE = request.HolidayDate.Date;
                        calendar.HOLIDAY_DESC = request.HolidayDesc;
                        calendar.BRANCH_ID = branchId;
                        calendar.CREATE_USER = request.ActionUsername;
                        calendar.UPDATE_USER = request.ActionUsername;
                        calendar.CREATE_DATE = now;
                        calendar.UPDATE_DATE = now;

                        _context.TB_R_BRANCH_CALENDAR.Add(calendar);
                    }
                }

                Save();
            }

            return new UpdateBranchCalendarResponse()
            {
                IsSuccess = true,
            };
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
