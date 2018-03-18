using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;
using log4net;
using CSM.Common.Utilities;
using System.Globalization;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.Branch;
using CSM.Service.Messages.Calendar;

namespace CSM.Business
{
    public interface IBranchFacade : IDisposable
    {
        List<BranchEntity> GetBranchByBranchIds(string keyword, List<int> branchIds, int limit);

        CreateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request);

        UpdateCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request);
    }

    public class BranchFacade : IBranchFacade
    {
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BranchFacade));

        public BranchFacade()
        {
            _context = new CSMContext();
        }

        public List<BranchEntity> GetBranchByBranchIds(string keyword, List<int> branchIds, int limit)
        {
            var branchDataAccess = new BranchDataAccess(_context);
            return branchDataAccess.GetBranchByBranchIds(keyword, branchIds, limit);
        }

        public CreateBranchResponse InsertOrUpdateBranch(InsertOrUpdateBranchRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            CreateBranchResponse response = new CreateBranchResponse();

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call BranchService.InsertOrUpdateBranch").ToInputLogString());
            Logger.Debug("I:--START--:--BranchService.InsertOrUpdateBranch--");

            try
            {
                bool valid = false;
                _commonFacade = new CommonFacade();

                if (request.Header != null)
                {
                    valid = _commonFacade.VerifyServiceRequest<Header>(request.Header);
                    response.Header = new Header
                    {
                        reference_no = request.Header.reference_no,
                        service_name = request.Header.service_name,
                        system_code = request.Header.system_code,
                        transaction_date = request.Header.transaction_date
                    };
                }

                Logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());
                var auditLog = new AuditLogEntity();
                auditLog.Module = Constants.Module.WebService;
                auditLog.Action = Constants.AuditAction.CreateBranch;
                auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                #region == Validate Require Field ==

                if (!valid)
                {
                    response.StatusResponse = new StatusResponse()
                    {
                        ErrorCode = Constants.ErrorCode.CSMBranch001,
                        Status = Constants.StatusResponse.Failed,
                        Description = "Bad Request, the header is not valid"
                    };

                    return response;
                }
                else
                {
                    if (string.IsNullOrEmpty(request.BranchCode))
                        return GetReturnErrorRequireField("BranchCode", response);

                    if (string.IsNullOrEmpty(request.BranchName))
                        return GetReturnErrorRequireField("BranchName", response);

                    if (string.IsNullOrEmpty(request.ChannelCode))
                        return GetReturnErrorRequireField("ChannelCode", response);

                    if (request.StartTimeHour < 0 || request.StartTimeHour > 23)
                        return GetReturnErrorInvalidFormat("StartTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 23", "5", response);

                    if (request.StartTimeMinute < 0 || request.StartTimeMinute > 59)
                        return GetReturnErrorInvalidFormat("StartTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 59", "5", response);

                    if (request.EndTimeHour < 0 || request.EndTimeHour > 23)
                        return GetReturnErrorInvalidFormat("EndTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 23", "5", response);

                    if (request.EndTimeMinute < 0 || request.EndTimeMinute > 59)
                        return GetReturnErrorInvalidFormat("EndTimeHour", "ต้องมีค่าระหว่าง 0 ถึง 59", "5", response);

                    if (request.Status != 0 && request.Status != 1)
                        return GetReturnErrorInvalidFormat("Status", "ต้องมีค่าระหว่าง 0 ถึง 1", "5", response);
                }

                #endregion

                #region == Validate Code ==

                var channelDataAccess = new ChannelDataAccess(_context);

                int? channelId = channelDataAccess.GetChannelIdByChannelCode(request.ChannelCode);
                if (!channelId.HasValue)
                {
                    response.StatusResponse = new StatusResponse()
                    {
                        ErrorCode = Constants.ErrorCode.CSMBranch003,
                        Status = Constants.StatusResponse.Failed,
                        Description = "Fail to save branch:\r\nไม่พบ Channel Code ในฐานข้อมูล CSM"
                    };

                    return response;

                }

                var branchDataAccess = new BranchDataAccess(_context);

                int? upperBranchId = null;

                if (!string.IsNullOrEmpty(request.UpperBranchCode))
                {
                    upperBranchId = branchDataAccess.GetBranchIdByBranchCode(request.UpperBranchCode);
                    if (!upperBranchId.HasValue)
                    {
                        response.StatusResponse = new StatusResponse()
                        {
                            ErrorCode = Constants.ErrorCode.CSMBranch003,
                            Status = Constants.StatusResponse.Failed,
                            Description = "Fail to save branch:\r\nไม่พบ Upper Branch Code ในฐานข้อมูล CSM"
                        };

                        return response;
                    }
                }

                #endregion

                var result = branchDataAccess.InsertOrUpdateBranch(request, channelId.Value, upperBranchId);
                if (result.IsSuccess)
                {
                    response.StatusResponse = new StatusResponse
                    {
                        ErrorCode = string.Empty,
                        Status = Constants.StatusResponse.Success,
                        Description = "Save successful"
                    };
                    AppLog.AuditLog(auditLog, LogStatus.Success, response.StatusResponse.Description);
                }
                else
                {

                    response.StatusResponse = new StatusResponse
                    {
                        ErrorCode = Constants.ErrorCode.CSMBranch003,
                        Status = Constants.StatusResponse.Failed,
                        Description = result.ErrorMessage
                    };
                    AppLog.AuditLog(auditLog, LogStatus.Fail, response.StatusResponse.Description);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                response.StatusResponse = new StatusResponse
                {
                    ErrorCode = Constants.ErrorCode.CSMBranch004,
                    Status = Constants.StatusResponse.Failed,
                    Description = ex.ToString()
                };
            }
            finally
            {
                Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return response;
        }

        private static CreateBranchResponse GetReturnErrorRequireField(string fieldName, CreateBranchResponse response)
        {
            response.StatusResponse = new StatusResponse()
            {
                ErrorCode = Constants.ErrorCode.CSMBranch002,
                Status = Constants.StatusResponse.Failed,
                Description = string.Format("Bad Request, the body is not valid:\r\nข้อมูลที่ส่งมาไม่ครบถ้วน ไม่สามารถบันทึกรายการได้ (Field={0})", fieldName)
            };

            return response;
        }

        private static CreateBranchResponse GetReturnErrorInvalidFormat(string fieldName, string errorCode, string remark, CreateBranchResponse response)
        {
            response.StatusResponse = new StatusResponse()
            {
                ErrorCode = Constants.ErrorCode.CSMBranch002,
                Status = Constants.StatusResponse.Failed,
                Description = string.Format("Bad Request, the body is not valid:\r\nไม่สามารถบันทึกรายการได้ เนื่องจากข้อมูลที่ส่งมาอยู่ในรูปแบบไม่ถูกต้อง ({0} {1})", fieldName, remark)
            };

            return response;
        }

        public UpdateCalendarResponse UpdateBranchCalendar(UpdateBranchCalendarRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            UpdateCalendarResponse response = new UpdateCalendarResponse();

            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call BranchService.UpdateBranchCalendar").ToInputLogString());
            Logger.Debug("I:--START--:--BranchService.UpdateBranchCalendar--");

            try
            {
                bool valid = false;
                _commonFacade = new CommonFacade();

                if (request.Header != null)
                {
                    valid = _commonFacade.VerifyServiceRequest<Header>(request.Header);
                    response.Header = new Header
                    {
                        reference_no = request.Header.reference_no,
                        service_name = request.Header.service_name,
                        system_code = request.Header.system_code,
                        transaction_date = request.Header.transaction_date
                    };
                }

                Logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());
                var auditLog = new AuditLogEntity();
                auditLog.Module = Constants.Module.WebService;
                auditLog.Action = Constants.AuditAction.CreateBranch;
                auditLog.IpAddress = ApplicationHelpers.GetClientIP();

                if (!valid)
                {
                    response.StatusResponse = new StatusResponse()
                    {
                        ErrorCode = Constants.ErrorCode.CSMCalendar001,
                        Status = Constants.StatusResponse.Failed,
                        Description = "Bad Request, the header is not valid"
                    };

                    return response;
                }
                else
                {
                    #region == Validate Require Field ==

                    if (string.IsNullOrEmpty(request.HolidayDesc))
                    {
                        response.StatusResponse = new StatusResponse()
                        {
                            ErrorCode = Constants.ErrorCode.CSMCalendar002,
                            Status = Constants.StatusResponse.Failed,
                            Description = "ข้อมูลที่ส่งมาไม่ครบถ้วน ไม่สามารถบันทึกรายการได้ (Field=HolidateDesc)"
                        };

                        return response;
                    }

                    if (request.UpdateMode != 1 && request.UpdateMode != 2)
                    {
                        response.StatusResponse = new StatusResponse()
                        {
                            ErrorCode = Constants.ErrorCode.CSMCalendar002,
                            Status = Constants.StatusResponse.Failed,
                            Description = "ข้อมูลที่ไม่สามารถบันทึกรายการได้ เนื่องจากข้อมูลที่ส่งมาอยู่ในรูปแบบไม่ถูกต้อง (UpdateMode ต้องมีค่า 1 (Delete and Insert) หรือ 2 (Merge) เท่านั้น)"
                        };

                        return response;
                    }

                    #endregion

                    #region == Validate Code ==

                    var branchDataAccess = new BranchDataAccess(_context);

                    var branchCodes = request.BranchCodeList;
                    var branchIds = new List<int>();

                    if (request.BranchCodeList != null)
                    {
                        var branchCodeNotFoundList = new List<string>();

                        foreach (var code in branchCodes)
                        {
                            var branchId = branchDataAccess.GetBranchIdByBranchCode(code);
                            
                            if (branchId == null)
                            {
                                branchCodeNotFoundList.Add(code);
                            }
                            else
                            {
                                branchIds.Add(branchId.Value);
                            }
                        }

                        //ถ้าlist ของ branch code ที่ไม่มีเบส CSM มากกว่า 0 ให้ส่ง code list กลับไป
                        if (branchCodeNotFoundList.Count > 0)
                        {
                            response.StatusResponse = new StatusResponse()
                            {
                                ErrorCode = Constants.ErrorCode.CSMCalendar004,
                                Status = Constants.StatusResponse.Failed,
                                BranchCodeNotFoundList = branchCodeNotFoundList,
                                Description = "ข้อมูลที่ไม่สามารถบันทึกรายการได้ เนื่องจากไม่พบ Branch Code ในฐานข้อมูล"
                            };

                            return response;
                        }
                    }
                    else
                    {
                        request.BranchCodeList = new List<string>();
                    }

                    #endregion

                    var result = branchDataAccess.UpdateBranchCalendar(request, branchIds);

                    if (result.IsSuccess)
                    {
                        response.StatusResponse = new StatusResponse
                        {
                            ErrorCode = string.Empty,
                            Status = Constants.StatusResponse.Success,
                            Description = "Save successful"
                        };
                    }
                    else
                    {
                        response.StatusResponse = new StatusResponse
                        {
                            ErrorCode = Constants.ErrorCode.CSMCalendar003,
                            Status = Constants.StatusResponse.Failed,
                            Description = result.ErrorMessage
                        };
                        AppLog.AuditLog(auditLog, LogStatus.Fail, response.StatusResponse.Description);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                response.StatusResponse = new StatusResponse
                {
                    ErrorCode = Constants.ErrorCode.CSMBranch004,
                    Status = Constants.StatusResponse.Failed,
                    Description = ex.ToString()
                };
            }
            finally
            {
                Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);
            }

            return response;
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
                    if (_commonFacade != null) { _commonFacade.Dispose(); _commonFacade = null; }
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
