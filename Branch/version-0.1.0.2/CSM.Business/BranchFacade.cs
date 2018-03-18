using CSM.Entity;
using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using log4net;
using CSM.Common.Utilities;
using System.Globalization;

namespace CSM.Business
{
    public interface IBranchFacade : IDisposable
    {
        List<BranchEntity> GetBranchByBranchIds(string keyword, List<int> branchIds, int limit);

        InsertOrUpdateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request);

        UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request);
    }

    public class BranchFacade : IBranchFacade
    {
        private readonly CSMContext _context;
        //private LogMessageBuilder _logMsg = new LogMessageBuilder();
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(BranchFacade));

        public BranchFacade()
        {
            _context = new CSMContext();
        }

        public List<BranchEntity> GetBranchByBranchIds(string keyword, List<int> branchIds, int limit)
        {
            var branchDataAccess = new BranchDataAccess(_context);
            return branchDataAccess.GetBranchByBranchIds(keyword, branchIds, limit);
        }

        public InsertOrUpdateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request)
        {
            try
            {
                #region == Validate Require Field ==

                if (string.IsNullOrEmpty(request.BranchCode))
                    return GetReturnErrorRequireField("BranchCode");

                if (string.IsNullOrEmpty(request.BranchName))
                    return GetReturnErrorRequireField("BranchName");

                if (string.IsNullOrEmpty(request.ChannelCode))
                    return GetReturnErrorRequireField("ChannelCode");

                if (request.StartTimeHour < 0 || request.StartTimeHour > 23)
                    return GetReturnErrorInvalidFormat("StartTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 23", "5");

                if (request.StartTimeMinute < 0 || request.StartTimeMinute > 59)
                    return GetReturnErrorInvalidFormat("StartTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 59", "5");

                if (request.EndTimeHour < 0 || request.EndTimeHour > 23)
                    return GetReturnErrorInvalidFormat("EndTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 23", "5");

                if (request.EndTimeMinute < 0 || request.EndTimeMinute > 59)
                    return GetReturnErrorInvalidFormat("EndTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 59", "5");

                if (request.Status != 0 && request.Status != 1)
                    return GetReturnErrorInvalidFormat("Status", "ต้องมีค่าระหว่าง 0 ถึง 1", "5");

                #endregion

                #region == Validate Code ==

                var channelDataAccess = new ChannelDataAccess(_context);

                int? channelId = channelDataAccess.GetChannelIdByChannelCode(request.ChannelCode);
                if (!channelId.HasValue)
                {
                    return new InsertOrUpdateBranchResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "3",
                        ErrorMessage = "ไม่พบ Channel Code ในฐานข้อมูล CSM",
                    };
                }

                var branchDataAccess = new BranchDataAccess(_context);

                int? upperBranchId = null;

                if (!string.IsNullOrEmpty(request.UpperBranchCode))
                {
                    upperBranchId = branchDataAccess.GetBranchIdByBranchCode(request.UpperBranchCode);
                    if (!upperBranchId.HasValue)
                    {
                        return new InsertOrUpdateBranchResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = "4",
                            ErrorMessage = "ไม่พบ Upper Branch Code ในฐานข้อมูล CSM",
                        };
                    }
                }

                #endregion

                return branchDataAccess.InsertOrUpdateBranch(request, channelId.Value, upperBranchId);
            }
            catch (Exception ex)
            {
                return new InsertOrUpdateBranchResponse()
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
            }
        }

        private static InsertOrUpdateBranchResponse GetReturnErrorRequireField(string fieldName, string remark = "")
        {
            return new InsertOrUpdateBranchResponse()
            {
                IsSuccess = false,
                ErrorCode = "2",
                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "ข้อมูลที่ส่งมาไม่ครบถ้วน ไม่สามารถบันทึกรายการได้ (Field={0}){1}", fieldName, remark)
            };
        }

        private static InsertOrUpdateBranchResponse GetReturnErrorInvalidFormat(string fieldName, string errorCode, string remark)
        {
            return new InsertOrUpdateBranchResponse()
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "ไม่สามารถบันทึกรายการได้ เนื่องจากข้อมูลที่ส่งมาอยู่ในรูปแบบไม่ถูกต้อง ({0} {1})", fieldName, remark)
            };
        }

        public UpdateBranchCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request)
        {
            try
            {
                #region == Validate Require Field ==

                if (string.IsNullOrEmpty(request.HolidayDesc))
                {
                    return new UpdateBranchCalendarResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "2",
                        ErrorMessage = "ข้อมูลที่ส่งมาไม่ครบถ้วน ไม่สามารถบันทึกรายการได้ (Field=HolidateDesc)"
                    };
                }

                if (request.UpdateMode != 1 && request.UpdateMode != 2)
                {
                    return new UpdateBranchCalendarResponse()
                    {
                        IsSuccess = false,
                        ErrorCode = "3",
                        ErrorMessage = "ข้อมูลที่ไม่สามารถบันทึกรายการได้ เนื่องจากข้อมูลที่ส่งมาอยู่ในรูปแบบไม่ถูกต้อง (UpdateMode ต้องมีค่า 1 (Delete and Insert) หรือ 2 (Merge) เท่านั้น)"
                    };
                }

                #endregion

                #region == Validate Code ==

                var branchDataAccess = new BranchDataAccess(_context);

                var branchCodes = request.BranchCodeList;
                var branchIds = new List<int>();

                if (request.BranchCodeList != null)
                {
                    foreach (var code in branchCodes)
                    {
                        var branchId = branchDataAccess.GetBranchIdByBranchCode(code);
                        if (branchId == null)
                        {
                            return new UpdateBranchCalendarResponse
                            {
                                IsSuccess = false,
                                ErrorCode = "4",
                                ErrorMessage = string.Format(CultureInfo.InvariantCulture, "ข้อมูลที่ไม่สามารถบันทึกรายการได้ เนื่องจากไม่พบ Branch Code ในฐานข้อมูล (Code={0})", code),
                            };
                        }
                        branchIds.Add(branchId.Value);
                    }
                }
                else
                {
                    request.BranchCodeList = new List<string>();
                }

                #endregion

                return branchDataAccess.UpdateBranchCalendar(request, branchIds);
            }
            catch (Exception ex)
            {
                return new UpdateBranchCalendarResponse()
                {
                    IsSuccess = false,
                    ErrorCode = "1",
                    ErrorMessage = ex.Message
                };
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
