using System;
using System.Collections.Generic;
using System.Data;
using CSM.Entity; 

namespace CSM.Business
{
    public interface IReportFacade : IDisposable
    {
        IDictionary<string, string> GetAttachfileSelectList();
        //DataTable GetExportJobs(ExportJobsSearchFilter searchFilter);
        //DataTable GetExportSR(ExportSRSearchFilter searchFilter);
        //DataTable GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter);
        //DataTable GetExportVerify(ExportVerifySearchFilter searchFilter);
        //DataTable GetExportNcb(ExportNcbSearchFilter searchFilter);
        DataTable GetExportComplaint(ExportSRSearchFilter searchFilter);
        IList<ExportJobsEntity> GetExportJobs(ExportJobsSearchFilter searchFilter);
        IList<ExportNcbEntity> GetExportNcb(ExportNcbSearchFilter searchFilter);
        IList<ExportSREntity> GetExportSR(ExportSRSearchFilter searchFilter);
        IList<ExportSREntity> GetExportSRMonthly(ExportSRSearchFilter searchFilter);
        IList<ExportVerifyEntity> GetExportVerify(ExportVerifySearchFilter searchFilter);
        IList<ExportVerifyDetailEntity> GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter);
        string GetParameter(string paramName);
        IDictionary<string, string> GetVerifyResultSelectList();
        IDictionary<string, string> GetSubscriptTypeSelectList();
        IDictionary<string, string> GetSubscriptTypeSelectList(string textName, int textValue = 0);
        IDictionary<string, string> GetSlaSelectList();
        IDictionary<string, string> GetVerifyResultSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetSlaSelectList(string textName, int? textValue = null);
        IDictionary<string, string> GetAttachfileSelectList(string textName, int? textValue = null);
        byte[] CreateReportCommPool(IList<ExportJobsEntity> jobList, ExportJobsSearchFilter searchFilter);
        byte[] CreateReportSR(IList<ExportSREntity> srList, ExportSRSearchFilter searchFilter);
        byte[] CreateReportNCB(IList<ExportNcbEntity> ncbList, ExportNcbSearchFilter searchFilter);
        byte[] CreateReportVerify(IList<ExportVerifyEntity> vfList, ExportVerifySearchFilter searchFilter);
        byte[] CreateReportVerifyDetail(IList<ExportVerifyDetailEntity> vfdList, ExportVerifyDetailSearchFilter searchFilter);
        string GetReportPath();
        string GetTriggerDays();
        string GetNumOfDaySRReport();
    }
}
