using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Data.DataAccess
{
    public interface IReportDataAccess
    {
        IList<ExportJobsEntity> GetExportJobs(ExportJobsSearchFilter searchFilter);
        IList<ExportSREntity> GetExportSR(ExportSRSearchFilter searchFilter);
        IList<ExportSREntity> GetExportSRMonthly(ExportSRSearchFilter searchFilter);
        IList<ExportSRComplaintEntity> GetExportComplaint(ExportSRSearchFilter searchFilter);
        IList<ExportVerifyDetailEntity> GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter);
        IList<ExportVerifyEntity> GetExportVerify(ExportVerifySearchFilter searchFilter);
        IList<ExportNcbEntity> GetExportNcb(ExportNcbSearchFilter searchFilter);
        List<SubscriptTypeEntity> GetActiveSubscriptType();
        string GetReportPath();
        string GetTriggerDays();
        string GetNumOfDaySRReport();
    }
}
