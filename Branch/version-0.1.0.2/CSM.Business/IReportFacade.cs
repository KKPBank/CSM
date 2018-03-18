using System;
using System.Collections.Generic;
using System.Data;
using CSM.Entity; 

namespace CSM.Business
{
    public interface IReportFacade : IDisposable
    {
        IDictionary<string, string> GetAttachfileSelectList();
        DataTable GetExportJobs(ExportJobsSearchFilter searchFilter);
        DataTable GetExportSR(ExportSRSearchFilter searchFilter);
        DataTable GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter);
        DataTable GetExportVerify(ExportVerifySearchFilter searchFilter);
        DataTable GetExportNcb(ExportNcbSearchFilter searchFilter);
        string GetParameter(string paramName);
        IDictionary<string, string> GetVerifyResultSelectList();
        IDictionary<string, string> GetSubscriptTypeSelectList();
        IDictionary<string, string> GetSubscriptTypeSelectList(string textName, int textValue = 0);
        IDictionary<string, string> GetSlaSelectList();
        IDictionary<string, string> GetVerifyResultSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetSlaSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetAttachfileSelectList(string textName, int? textValue = null);
    }
}
