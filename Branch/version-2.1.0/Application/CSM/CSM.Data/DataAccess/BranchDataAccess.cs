using System.Data;
using CSM.Common.Utilities;
using CSM.Entity;
using CSM.Service.Messages.Branch;
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
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
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

            var response = new InsertOrUpdateBranchResponse();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Insert or Update Branch").Add("BranchCode", request.BranchCode)
                .Add("BranchName", request.BranchName).Add("ChannelId", channelId).Add("UpperBranchId", upperBranchId).ToInputLogString());

            try
            {
                #region "Comment out"
                //                var result = new InsertOrUpdateBranchResponse();

                ////                var dbBranch = _context.TB_R_BRANCH.Where(x => x.BRANCH_CODE.Trim().ToUpper() == request.BranchCode.Trim().ToUpper()).FirstOrDefault();
                //                TB_R_BRANCH dbBranch;
                ////                if (dbBranch == null)
                //                if (!request.Command)
                //                {
                //                    //add mode
                //                    dbBranch = new TB_R_BRANCH();
                //                    result.IsNewBranch = true;

                //                    //check duplicate code
                //                    var count = _context.TB_R_BRANCH.Count(p => p.BRANCH_CODE == request.BranchCode);
                //                    if (count > 0)
                //                    {
                //                        result.IsSuccess = false;
                //                        result.ErrorCode = "6";
                //                        result.ErrorMessage = string.Format("CSM : รหัสสาขา {0} มีในระบบแล้ว", request.BranchCode);
                //                        return result;
                //                    }

                //                    //check duplicate name
                //                    count = _context.TB_R_BRANCH.Count(p => p.BRANCH_NAME == request.BranchName);
                //                    if (count > 0)
                //                    {
                //                        result.IsSuccess = false;
                //                        result.ErrorCode = "6";
                //                        result.ErrorMessage = string.Format("CSM : {0} มีในระบบ", request.BranchName);
                //                        return result;
                //                    }
                //                }
                //                else
                //                {
                //                    //edit mode
                //                    result.IsNewBranch = false;
                //                    dbBranch = _context.TB_R_BRANCH.FirstOrDefault(x => x.BRANCH_CODE.Trim().ToUpper() == request.BranchCode.Trim().ToUpper());

                //                    #region == Validate Branch Data ==

                //                    if (dbBranch == null)
                //                    {
                //                        //create new branch at slm
                //                        dbBranch = new TB_R_BRANCH();
                //                        result.IsNewBranch = true;
                //                    }
                //                    else
                //                    {
                //                        // IF (Change Branch) Or (Change Status 1 to 0)
                //                        if (dbBranch.STATUS == 1 && request.Status == 0)
                //                        {
                //                            var count = _context.TB_M_POOL_BRANCH.Count(x => x.BRANCH_ID == dbBranch.BRANCH_ID && x.STATUS == 1);

                //                            if (count > 0)
                //                            {
                //                                result.IsSuccess = false;
                //                                result.ErrorCode = "6";
                //                                result.ErrorMessage = "ไม่สามารถอัพเดตข้อมูลเป็นปิดสาขาได้ เนื่องจาก มีการผูกสาขากับ Communication Pool";
                //                                return result;
                //                            }
                //                        }

                //                        //duplicate code and name
                //                        var countDuplicate = _context.TB_R_BRANCH.Count(p => p.BRANCH_NAME == request.BranchName && p.BRANCH_CODE != request.BranchCode);
                //                        if (countDuplicate > 0)
                //                        {
                //                            result.IsSuccess = false;
                //                            result.ErrorCode = "6";
                //                            result.ErrorMessage = string.Format("{0} มีในระบบแล้ว", request.BranchName);
                //                            return result;
                //                        }

                //                        //check recursive upper branch
                //                        if (!CheckRecursiveBranch(dbBranch.BRANCH_CODE, upperBranchId))
                //                        {
                //                            result.IsSuccess = false;
                //                            result.ErrorCode = "6";
                //                            result.ErrorMessage = "CSM : การบันทึกข้อมูลไม่สำเร็จเนื่องจากพบ Recursive Upper Branch";
                //                            return result;
                //                        }
                //                    }
                //                    #endregion
                //                }

                //                dbBranch.CHANNEL_ID = channelId;
                //                dbBranch.BRANCH_CODE = ValueOrDefault(request.BranchCode);
                //                dbBranch.BRANCH_NAME = ValueOrDefault(request.BranchName);
                //                dbBranch.STATUS = Convert.ToInt16(request.Status);
                //                dbBranch.UPPER_BRANCH_ID = upperBranchId;
                //                dbBranch.START_TIME_HOUR = Convert.ToInt16(request.StartTimeHour);
                //                dbBranch.START_TIME_MINUTE = Convert.ToInt16(request.StartTimeMinute);
                //                dbBranch.END_TIME_HOUR = Convert.ToInt16(request.EndTimeHour);
                //                dbBranch.END_TIME_MINUTE = Convert.ToInt16(request.EndTimeMinute);

                //                dbBranch.BRANCH_HOME_NO = ValueOrDefault(request.HomeNo);
                //                dbBranch.BRANCH_MOO = ValueOrDefault(request.Moo);
                //                dbBranch.BRANCH_BUILDING = ValueOrDefault(request.Building);
                //                dbBranch.BRANCH_FLOOR = ValueOrDefault(request.Floor);
                //                dbBranch.BRANCH_SOI = ValueOrDefault(request.Soi);
                //                dbBranch.BRANCH_STREET = ValueOrDefault(request.Street);
                //                dbBranch.BRANCH_PROVINCE = ValueOrDefault(request.Province);
                //                dbBranch.BRANCH_AMPHUR = ValueOrDefault(request.Amphur);
                //                dbBranch.BRANCH_TAMBOL = ValueOrDefault(request.Tambol);
                //                dbBranch.BRANCH_ZIPCODE = ValueOrDefault(request.Zipcode);

                //                var now = DateTime.Now;

                //                if (result.IsNewBranch)
                //                {
                //                    dbBranch.CREATE_USER = ValueOrDefault(request.ActionUsername);
                //                    dbBranch.UPDATE_USER = ValueOrDefault(request.ActionUsername);
                //                    dbBranch.CREATE_DATE = now;
                //                    dbBranch.UPDATE_DATE = now;

                //                    _context.TB_R_BRANCH.Add(dbBranch);
                //                }
                //                else
                //                {
                //                    dbBranch.UPDATE_USER = ValueOrDefault(request.ActionUsername);
                //                    dbBranch.UPDATE_DATE = DateTime.Now;
                //                    SetEntryStateModified(dbBranch);
                //                }

                //                this.Save();

                //                result.IsSuccess = true;
                //                return result;
                #endregion

                string logMsg;
                var today = DateTime.Now;
                string branchCode = request.BranchCode.NullSafeTrim();
                var dbBranch = _context.TB_R_BRANCH.FirstOrDefault(x => x.BRANCH_CODE == branchCode);
                bool isNewBranch = (dbBranch == null);

                if (isNewBranch)
                {
                    dbBranch = new TB_R_BRANCH();
                }
                else
                {
                    // เช็คเงื่อนไขการปิดสาขา
                    if (dbBranch.STATUS == 1 && request.Status == 0)
                    {
                        if (_context.TB_M_POOL_BRANCH.Any(x => x.BRANCH_ID == dbBranch.BRANCH_ID && x.STATUS == Constants.ApplicationStatus.Active))
                        {
                            response.IsSuccess = false;
                            response.ErrorCode = "6";
                            response.ErrorMessage = "ไม่สามารถอัพเดตข้อมูลเป็นปิดสาขาได้ เนื่องจาก มีการผูกสาขากับ Communication Pool";
                            return response;
                        }
                    }

                    // Check recursive upper branch
                    if (!CheckRecursiveBranch(dbBranch.BRANCH_CODE, upperBranchId))
                    {
                        response.IsSuccess = false;
                        response.ErrorCode = "6";
                        response.ErrorMessage = "CSM : การบันทึกข้อมูลไม่สำเร็จเนื่องจากพบ Recursive Upper Branch";
                        return response;
                    }
                }

                dbBranch.CHANNEL_ID = channelId;
                dbBranch.BRANCH_CODE = branchCode;
                dbBranch.BRANCH_NAME = request.BranchName.NullSafeTrim();
                dbBranch.STATUS = request.Status;
                dbBranch.UPPER_BRANCH_ID = upperBranchId;
                dbBranch.START_TIME_HOUR = (short)request.StartTimeHour;
                dbBranch.START_TIME_MINUTE = (short)request.StartTimeMinute;
                dbBranch.END_TIME_HOUR = (short)request.EndTimeHour;
                dbBranch.END_TIME_MINUTE = (short)request.EndTimeMinute;

                dbBranch.BRANCH_HOME_NO = request.HomeNo.NullSafeTrim();
                dbBranch.BRANCH_MOO = request.Moo.NullSafeTrim();
                dbBranch.BRANCH_BUILDING = request.Building.NullSafeTrim();
                dbBranch.BRANCH_FLOOR = request.Floor.NullSafeTrim();
                dbBranch.BRANCH_SOI = request.Soi.NullSafeTrim();
                dbBranch.BRANCH_STREET = request.Street.NullSafeTrim();
                dbBranch.BRANCH_PROVINCE = request.Province.NullSafeTrim();
                dbBranch.BRANCH_AMPHUR = request.Amphur.NullSafeTrim();
                dbBranch.BRANCH_TAMBOL = request.Tambol.NullSafeTrim();
                dbBranch.BRANCH_ZIPCODE = request.Zipcode.NullSafeTrim();
                dbBranch.UPDATE_USER = request.ActionUsername.NullSafeTrim();
                dbBranch.UPDATE_DATE = today;

                if (isNewBranch)
                {
                    logMsg = "Insert Branch";
                    dbBranch.CREATE_USER = request.ActionUsername.NullSafeTrim();
                    dbBranch.CREATE_DATE = today;
                    _context.TB_R_BRANCH.Add(dbBranch);
                }
                else
                {
                    logMsg = "Update Branch";
                    SetEntryStateModified(dbBranch);
                }

                response.IsSuccess = (Save() > 0);

                if (response.IsSuccess)
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg(logMsg).ToSuccessLogString());
                }
                else
                {
                    Logger.Info(_logMsg.Clear().SetPrefixMsg(logMsg).Add("Error Message", "Failed to save data").ToFailLogString());
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                Logger.Info(_logMsg.Clear().SetPrefixMsg("Insert or Update Branch").Add("Error Message", ex.Message).ToFailLogString());

                response = new InsertOrUpdateBranchResponse
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message,
                };

                return response;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = false;
            }
        }

        private bool CheckRecursiveBranch(string branchCode, int? upperBranch)
        {
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Check Recursive Branch").Add("BranchCode", branchCode)
                .Add("UpperBranchId", upperBranch).ToInputLogString());

            TB_R_BRANCH itemUpper;
            if (upperBranch.HasValue)
            {
                itemUpper = _context.TB_R_BRANCH.SingleOrDefault(i => i.BRANCH_ID == upperBranch);

                if (itemUpper.BRANCH_CODE == branchCode)
                {
                    return false;
                }

                return CheckRecursiveBranch(branchCode, itemUpper.UPPER_BRANCH_ID);
            }

            return true;
        }

        public UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request, List<int> branchIds)
        {
            var holidayDate = request.HolidayDate.Date;

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Update BranchCalendar").Add("UpdateMode", request.UpdateMode)
                .Add("HolidayDate", holidayDate).Add("HolidayDesc", request.HolidayDesc).ToInputLogString());

            if (request.UpdateMode == 1)
            {
                try
                {
                    using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            var today = DateTime.Now;

                            var calendars = _context.TB_R_BRANCH_CALENDAR.Where(x => EntityFunctions.TruncateTime(x.HOLIDAY_DATE) == holidayDate).ToList();
                            _context.TB_R_BRANCH_CALENDAR.RemoveRange(calendars);
                            Save();

                            foreach (var branchId in branchIds)
                            {
                                var calendar = new TB_R_BRANCH_CALENDAR();
                                calendar.HOLIDAY_DATE = request.HolidayDate.Date;
                                calendar.HOLIDAY_DESC = request.HolidayDesc;
                                calendar.BRANCH_ID = branchId;
                                calendar.CREATE_USER = request.ActionUsername;
                                calendar.UPDATE_USER = request.ActionUsername;
                                calendar.CREATE_DATE = today;
                                calendar.UPDATE_DATE = today;
                                _context.TB_R_BRANCH_CALENDAR.Add(calendar);
                            }

                            Save();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.Error("Exception occur:\n", ex);

                            return new UpdateBranchCalendarResponse
                            {
                                IsSuccess = false,
                                ErrorMessage = ex.Message
                            };
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

                    return new UpdateBranchCalendarResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }

            if (request.UpdateMode == 2)
            {
                if (branchIds != null && branchIds.Count > 0)
                {
                    var today = DateTime.Now;
                    var existingBranchList = _context.TB_R_BRANCH_CALENDAR.Where(x => EntityFunctions.TruncateTime(x.HOLIDAY_DATE) == holidayDate).ToList();

                    foreach (var branchId in branchIds)
                    {
                        var calendar = existingBranchList.SingleOrDefault(x => x.BRANCH_ID == branchId);
                        if (calendar != null)
                        {
                            // Update
                            calendar.HOLIDAY_DESC = request.HolidayDesc;
                            calendar.UPDATE_USER = request.ActionUsername;
                            calendar.UPDATE_DATE = today;
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
                            calendar.CREATE_DATE = today;
                            calendar.UPDATE_DATE = today;
                            _context.TB_R_BRANCH_CALENDAR.Add(calendar);
                        }
                    }

                    Save();
                }
            }

            return new UpdateBranchCalendarResponse
            {
                IsSuccess = true
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
