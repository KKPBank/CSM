using System.Data;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using System;
using System.Collections.Generic;
using CSM.Common.Resources;
using System.Linq;
using System.Globalization;

namespace CSM.Business
{
    public class ReportFacade : IReportFacade
    {
        private readonly CSMContext _context;
        private ICommonFacade _commonFacade;
        private IReportDataAccess _reportDataAccess;

        public ReportFacade()
        {
            _context = new CSMContext();
        }

        public IDictionary<string, string> GetAttachfileSelectList()
        {
            return this.GetAttachfileSelectList(null);
        }

        public IDictionary<string, string> GetAttachfileSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.AttachFile.Yes.ToString(CultureInfo.InvariantCulture), Resource.Ddl_AttachFile_Yes);
            dic.Add(Constants.AttachFile.No.ToString(CultureInfo.InvariantCulture), Resource.Ddl_AttachFile_No);
            return dic;
        }

        public DataTable GetExportJobs(ExportJobsSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportJobsEntity> exportJobs = _reportDataAccess.GetExportJobs(searchFilter);
            DataTable dt = DataTableHelpers.ConvertTo(exportJobs);
            return dt;
        }

        public DataTable GetExportSR(ExportSRSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSR(searchFilter);
            DataTable dt = DataTableHelpers.ConvertTo(exportSR);
            return dt;
        }

        public DataTable GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportVerifyDetailEntity> exportVerifyDetail = _reportDataAccess.GetExportVerifyDetail(searchFilter);
            DataTable dt = DataTableHelpers.ConvertTo(exportVerifyDetail);
            return dt;
        }

        public DataTable GetExportVerify(ExportVerifySearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportVerifyEntity> exportVerify = _reportDataAccess.GetExportVerify(searchFilter);
            DataTable dt = DataTableHelpers.ConvertTo(exportVerify);
            return dt;
        }

        public DataTable GetExportNcb(ExportNcbSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportNcbEntity> exportNcb = _reportDataAccess.GetExportNcb(searchFilter);
            DataTable dt = DataTableHelpers.ConvertTo(exportNcb);
            return dt;
        }

        public string GetParameter(string paramName)
        {
            _commonFacade = new CommonFacade();
            ParameterEntity param = _commonFacade.GetCacheParamByName(paramName);
            return param != null ? param.ParamValue : string.Empty;
        }

        public IDictionary<string, string> GetVerifyResultSelectList()
        {
            return this.GetVerifyResultSelectList(null);
        }

        public IDictionary<string, string> GetVerifyResultSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.VerifyResultStatus.Pass.ToString(), Resource.Ddl_VerifyResult_Pass);
            dic.Add(Constants.VerifyResultStatus.Fail.ToString(), Resource.Ddl_VerifyResult_Fail);
            dic.Add(Constants.VerifyResultStatus.Skip.ToString(), Resource.Ddl_VerifyResult_Skip);
            return dic;
        }

        public IDictionary<string, string> GetSubscriptTypeSelectList()
        {
            return this.GetSubscriptTypeSelectList(null);
        }

        public IDictionary<string, string> GetSubscriptTypeSelectList(string textName, int textValue = 0)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            var list = _reportDataAccess.GetActiveSubscriptType();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                list.Insert(0, new SubscriptTypeEntity { SubscriptTypeId = textValue, SubscriptTypeName = textName });
            }

            return (from x in list
                    select new
                    {
                        key = x.SubscriptTypeId.ToString(),
                        value = x.SubscriptTypeName
                    }).ToDictionary(t => t.key, t => t.value);
        }

        public IDictionary<string, string> GetSlaSelectList()
        {
            return this.GetSlaSelectList(null);
        }

        public IDictionary<string, string> GetSlaSelectList(string textName, int? textValue = null)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(textName))
            {
                dic.Add(textValue.ConvertToString(), textName);
            }

            dic.Add(Constants.Sla.Due.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Sla_Due);
            dic.Add(Constants.Sla.OverDue.ToString(CultureInfo.InvariantCulture), Resource.Ddl_Sla_OverDue);
            return dic;
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
