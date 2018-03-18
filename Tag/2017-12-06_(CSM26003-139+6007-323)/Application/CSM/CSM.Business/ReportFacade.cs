using System.Data;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using System;
using System.Collections.Generic;
using CSM.Common.Resources;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using System.Web;
using log4net;
using Renci.SshNet;
using System.IO;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public class ReportFacade : IReportFacade
    {
        private readonly CSMContext _context;
        private ICommonFacade _commonFacade;
        private IReportDataAccess _reportDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportFacade));

        public ReportFacade()
        {
            _context = new CSMContext();
        }

        private string addSingleQuote(string str) { return (string.IsNullOrWhiteSpace(str) ? "" : "'") + str; }

        public byte[] CreateReportCommPool(IList<ExportJobsEntity> jobList, ExportJobsSearchFilter searchFilter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> search_criteria = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
                {
                    search_criteria.Add(SetReportData("ชื่อ:" + searchFilter.FirstName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
                {
                    search_criteria.Add(SetReportData("นามสกุล:" + searchFilter.LastName));
                }
                if (searchFilter.JobStatus != null)
                {
                    search_criteria.Add(SetReportData("Job Status:" + searchFilter.JobStatusDesc));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.FromValue))
                {
                    search_criteria.Add(SetReportData("From:" + searchFilter.FromValue));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.AttachFile))
                {
                    search_criteria.Add(SetReportData("Attach File:" + searchFilter.AttachFileDesc));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Subject))
                {
                    search_criteria.Add(SetReportData("Subject:" + searchFilter.Subject));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRId))
                {
                    search_criteria.Add(SetReportData("SR ID:" + searchFilter.SRId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionBranch))
                {
                    search_criteria.Add(SetReportData("Action Branch:" + searchFilter.ActionBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionBy))
                {
                    search_criteria.Add(SetReportData("Action By:" + searchFilter.ActionByName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerBranch))
                {
                    search_criteria.Add(SetReportData("Owner Branch:" + searchFilter.OwnerBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerSR))
                {
                    search_criteria.Add(SetReportData("Owner SR:" + searchFilter.OwnerSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorBranch))
                {
                    search_criteria.Add(SetReportData("Creator Branch:" + searchFilter.CreatorBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorSR))
                {
                    search_criteria.Add(SetReportData("Creator SR:" + searchFilter.CreatorSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.JobDateFrom))
                {
                    search_criteria.Add(SetReportData("Job Date From:" + searchFilter.JobDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.JobDateTo))
                {
                    search_criteria.Add(SetReportData("Job Date To:" + searchFilter.JobDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.JobTimeFrom))
                {
                    search_criteria.Add(SetReportData("Job Time From:" + searchFilter.JobTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.JobTimeTo))
                {
                    search_criteria.Add(SetReportData("Job Time To:" + searchFilter.JobTimeTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionDateFrom))
                {
                    search_criteria.Add(SetReportData("Action Date From:" + searchFilter.ActionDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionDateTo))
                {
                    search_criteria.Add(SetReportData("Action Date To:" + searchFilter.ActionDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionTimeFrom))
                {
                    search_criteria.Add(SetReportData("Action Time From:" + searchFilter.ActionTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ActionTimeTo))
                {
                    search_criteria.Add(SetReportData("Action Time To:" + searchFilter.ActionTimeTo));
                }

                sb.AppendLine(string.Join(",", search_criteria));
                sb.AppendLine("");

                string[] headers = new string[]
                {
                    SetReportData("No"),
                    SetReportData("ชื่อลูกค้า"),
                    SetReportData("นามสกุลลูกค้า"),
                    SetReportData("Channel"),
                    SetReportData("Job Status"),
                    SetReportData("Job Date Time"),
                    SetReportData("From"),
                    SetReportData("Subject"),
                    SetReportData("SR ID."),
                    SetReportData("SR Creator"),

                    SetReportData("SR Owner"),
                    SetReportData("SR State"),
                    SetReportData("SR Status"),
                    SetReportData("Attach File"),
                    SetReportData("Remark"),
                    SetReportData("Pool Name")
                };
                sb.AppendLine(string.Join(",", headers));

                int i = 0;
                foreach (var item in jobList)
                {
                    i += 1;
                    string[] fields = new string[]
                    {
                        SetReportData(i.ToString()),
                        SetReportData(item.FirstName),
                        SetReportData(item.LastName),
                        SetReportData(item.JobType),
                        SetReportData(item.JobStatus),
                        SetReportData(item.JobDateDisplay),
                        SetReportData(item.From),
                        SetReportData(ReplaceHTML(item.Subject)),
                        SetReportData(addSingleQuote(item.SRID)),
                        SetReportData(item.SRCreator),

                        SetReportData(item.SROwner),
                        SetReportData(item.SRState),
                        SetReportData(item.SRStatus),
                        SetReportData(item.AttachFile),
                        SetReportData(ReplaceHTML(item.Remark)),
                        SetReportData(item.PoolName)
                    };
                    sb.AppendLine(string.Join(",", fields));
                }

                return Encoding.GetEncoding(874).GetBytes(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public byte[] CreateReportNCB(IList<ExportNcbEntity> ncbList, ExportNcbSearchFilter searchFilter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> search_criteria = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
                {
                    search_criteria.Add(SetReportData("ชื่อ:" + searchFilter.FirstName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
                {
                    search_criteria.Add(SetReportData("นามสกุล:" + searchFilter.LastName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CardId))
                {
                    search_criteria.Add(SetReportData("Subscription ID:" + searchFilter.CardId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ProductGroup))
                {
                    search_criteria.Add(SetReportData("Product Group:" + searchFilter.ProductGroupName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.BirthDate))
                {
                    search_criteria.Add(SetReportData("Date of Birth/วันที่จดทะเบียน: " + searchFilter.BirthDate));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Product))
                {
                    search_criteria.Add(SetReportData("Product: " + searchFilter.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Sla))
                {
                    search_criteria.Add(SetReportData("SLA: " + searchFilter.SlaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Campaign))
                {
                    search_criteria.Add(SetReportData("Campaign/Service: " + searchFilter.CampaignName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRStatus))
                {
                    search_criteria.Add(SetReportData("SR Status: " + searchFilter.SRStatusName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Type))
                {
                    search_criteria.Add(SetReportData("Type: " + searchFilter.TypeName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Area))
                {
                    search_criteria.Add(SetReportData("Area: " + searchFilter.AreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SubArea))
                {
                    search_criteria.Add(SetReportData("Sub Area: " + searchFilter.SubAreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.UpperBranch))
                {
                    search_criteria.Add(SetReportData("Marketing Branch: " + searchFilter.UpperBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerBranch))
                {
                    search_criteria.Add(SetReportData("Owner Branch: " + searchFilter.OwnerBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerSR))
                {
                    search_criteria.Add(SetReportData("Owner SR: " + searchFilter.OwnerSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorBranch))
                {
                    search_criteria.Add(SetReportData("Creator Branch: " + searchFilter.CreatorBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorSR))
                {
                    search_criteria.Add(SetReportData("Creator SR: " + searchFilter.CreatorSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.DelegateBranch))
                {
                    search_criteria.Add(SetReportData("Delegate Branch: " + searchFilter.DelegateBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.DelegateSR))
                {
                    search_criteria.Add(SetReportData("Deletegate SR: " + searchFilter.DelegateSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRId))
                {
                    search_criteria.Add(SetReportData("SR ID: " + searchFilter.SRId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Date From: " + searchFilter.SRDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateTo))
                {
                    search_criteria.Add(SetReportData("SR Created Date To: " + searchFilter.SRDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Time From: " + searchFilter.SRTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeTo))
                {
                    search_criteria.Add(SetReportData("SR Created Time To: " + searchFilter.SRTimeTo));
                }

                sb.AppendLine(string.Join(",", search_criteria));
                sb.AppendLine("");

                string[] headers = new string[]
                {
                    SetReportData("No"),
                    SetReportData("จำนวนครั้งที่เกิน SLA ทั้งหมด"),
                    SetReportData("ชื่อลูกค้า"),
                    SetReportData("นามสกุลลูกค้า"),
                    SetReportData("Subscription ID"),
                    SetReportData("วันเกิด/วันจดทะเบียน"),
                    SetReportData("NCB Check Status"),
                    SetReportData("SR ID"),
                    SetReportData("SR State"),
                    SetReportData("SR Status"),
                    SetReportData("Product Group"),

                    SetReportData("Product"),
                    SetReportData("Campaign"),
                    SetReportData("Type"),
                    SetReportData("Area"),
                    SetReportData("Sub-Area"),
                    SetReportData("SR Creator"),
                    SetReportData("SR Created Date Time"),
                    SetReportData("SR Owner"),
                    SetReportData("Owner Updated Date Time"),
                    SetReportData("SR Delegate"),

                    SetReportData("Delegate Updated Date Time"),
                    SetReportData("Marketing Upper Branch1"),
                    SetReportData("Marketing Upper Branch2"),
                    SetReportData("MKT Employee Branch"),
                    SetReportData("MKT Employee Name"),
                };
                sb.AppendLine(string.Join(",", headers));

                int i = 0;
                foreach (var item in ncbList)
                {
                    i += 1;
                    string[] fields = new string[]
                    {
                        SetReportData(i.ToString()),
                        SetReportData(item.Sla),
                        SetReportData(item.CustomerFistname),
                        SetReportData(item.CustomerLastname),
                        SetReportData(addSingleQuote(item.CardNo)),
                        SetReportData(item.CustomerBirthDateDisplay),
                        SetReportData(item.NcbCheckStatus),
                        SetReportData(addSingleQuote(item.SRId)),
                        SetReportData(item.SRState),
                        SetReportData(item.SRStatus),
                        SetReportData(item.ProductGroupName),

                        SetReportData(item.ProductName),
                        SetReportData(item.CampaignName),
                        SetReportData(item.TypeName),
                        SetReportData(item.AreaName),
                        SetReportData(item.SubAreaName),
                        SetReportData(item.SRCreator),
                        SetReportData(item.SRCreateDateDisplay),
                        SetReportData(item.SROwner),
                        SetReportData(item.OwnerUpdateDisplay),
                        SetReportData(item.SRDelegate),

                        SetReportData(item.SRDelegateUpdateDisplay),
                        SetReportData(item.MKTUpperBranch1),
                        SetReportData(item.MKTUpperBranch2),
                        SetReportData(item.MKTEmployeeBranch),
                        SetReportData(item.MKTEmployeeName)
                    };
                    sb.AppendLine(string.Join(",", fields));
                }

                return Encoding.GetEncoding(874).GetBytes(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public byte[] CreateReportSR(IList<ExportSREntity> srList, ExportSRSearchFilter searchFilter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> search_criteria = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchFilter.FirstName))
                {
                    search_criteria.Add(SetReportData("ชื่อ:" + searchFilter.FirstName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.LastName))
                {
                    search_criteria.Add(SetReportData("นามสกุล:" + searchFilter.LastName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CardId))
                {
                    search_criteria.Add(SetReportData("Subscription ID:" + searchFilter.CardId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.ProductGroup))
                {
                    search_criteria.Add(SetReportData("Product Group:" + searchFilter.ProductGroupName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.AccountNo))
                {
                    search_criteria.Add(SetReportData("เลขที่บัญชี/สัญญา:" + searchFilter.AccountNo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Product))
                {
                    search_criteria.Add(SetReportData("Product:" + searchFilter.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Type))
                {
                    search_criteria.Add(SetReportData("Type:" + searchFilter.TypeName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Campaign))
                {
                    search_criteria.Add(SetReportData("Campaign/Service:" + searchFilter.CampaignName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Area))
                {
                    search_criteria.Add(SetReportData("Area:" + searchFilter.AreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRId))
                {
                    search_criteria.Add(SetReportData("SR ID:" + searchFilter.SRId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SubArea))
                {
                    search_criteria.Add(SetReportData("Sub Area:" + searchFilter.SubAreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Subject))
                {
                    search_criteria.Add(SetReportData("Subject:" + searchFilter.Subject));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Sla))
                {
                    search_criteria.Add(SetReportData("SLA:" + searchFilter.SlaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Description))
                {
                    search_criteria.Add(SetReportData("Description:" + searchFilter.Description));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRChannel))
                {
                    search_criteria.Add(SetReportData("SR Channel:" + searchFilter.SRChannelName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRStatus))
                {
                    search_criteria.Add(SetReportData("SR Status:" + searchFilter.SRStatusName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Date From:" + searchFilter.SRDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateTo))
                {
                    search_criteria.Add(SetReportData("SR Created Date To:" + searchFilter.SRDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Time From:" + searchFilter.SRTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeTo))
                {
                    search_criteria.Add(SetReportData("SR Created Time To:" + searchFilter.SRTimeTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerBranch))
                {
                    search_criteria.Add(SetReportData("Owner Branch:" + searchFilter.OwnerBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerSR))
                {
                    search_criteria.Add(SetReportData("Owner SR:" + searchFilter.OwnerSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorBranch))
                {
                    search_criteria.Add(SetReportData("Creator Branch:" + searchFilter.CreatorBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.CreatorSR))
                {
                    search_criteria.Add(SetReportData("Creator SR:" + searchFilter.CreatorSRName));
                }

                sb.AppendLine(string.Join(",", search_criteria));
                sb.AppendLine("");

                string[] headers = new string[]
                {
                    SetReportData("No"),
                    SetReportData("จำนวนครั้งที่เกิน SLA ทั้งหมด"),
                    SetReportData("แจ้งเตือนครั้งที่"),
                    SetReportData("ชื่อลูกค้า"),
                    SetReportData("นามสกุล"),
                    SetReportData("Subscription ID"),
                    SetReportData("เลขที่สัญญา / เลขที่บัญชี"),
                    SetReportData("ทะเบียนรถ"),
                    SetReportData("SR ID"),
                    SetReportData("SR Creator Branch"),

                    SetReportData("Channel"),
                    SetReportData("Call ID"),
                    SetReportData("Tell (A-Number)"),
                    SetReportData("Product Group"),
                    SetReportData("Product"),
                    SetReportData("Campaign/Service"),
                    SetReportData("Type"),
                    SetReportData("Area"),
                    SetReportData("Sub-Area"),
                    SetReportData("SR State"),
                    SetReportData("SR Status"),

                    SetReportData("Closed By"),
                    SetReportData("Closed Date Time"),
                    SetReportData("Verify Result"),
                    SetReportData("SR Creator"),
                    SetReportData("SR Created Date Time"),
                    SetReportData("Updated By"),
                    SetReportData("Updated Date Time"),
                    SetReportData("SR Owner"),
                    SetReportData("Owner Updated Date Time"),
                    SetReportData("SR Delegate"),
                    SetReportData("Delegate Updated Date Time"),
                    SetReportData("Subject"),
                    SetReportData("SR Description"),

                    SetReportData("ชื่อผู้ติดต่อ"),
                    SetReportData("นามสกุลผู้ติดต่อ"),
                    SetReportData("ความสัมพันธ์"),
                    SetReportData("เบอร์ติดต่อ"),
                    SetReportData("media source")
                };
                sb.AppendLine(string.Join(",", headers));

                int i = 0;
                foreach (var item in srList)
                {
                    i += 1;
                    string[] fields = new string[]
                    {
                        SetReportData(i.ToString()),
                        SetReportData(item.TotalSla != null ? item.TotalSla.ToString() : ""),
                        SetReportData(item.CurrentAlert != null ? item.CurrentAlert.ToString() : ""),
                        SetReportData(item.CustomerFistname),
                        SetReportData(item.CustomerLastname),
                        SetReportData(addSingleQuote(item.CardNo)),
                        SetReportData(addSingleQuote(item.AccountNo)),
                        SetReportData(item.CarRegisNo),
                        SetReportData(addSingleQuote(item.SRNo)),
                        SetReportData(item.CreatorBranch),

                        SetReportData(item.ChannelName),
                        SetReportData(addSingleQuote(item.CallId)),
                        SetReportData(addSingleQuote(item.ANo)),
                        SetReportData(item.ProductGroupName),
                        SetReportData(item.ProductName),
                        SetReportData(item.CampaignServiceName),
                        SetReportData(item.TypeName),
                        SetReportData(item.AreaName),
                        SetReportData(item.SubAreaName),
                        SetReportData(item.SRStateName),
                        SetReportData(item.SRStatusName),

                        SetReportData(item.CloseUserName),
                        SetReportData(item.CloseDateDisplay),
                        SetReportData(item.SRIsverifyPassDisplay),
                        SetReportData(item.CreatorName),
                        SetReportData(item.CreateDateDisplay),
                        SetReportData(item.UpdaterName),
                        SetReportData(item.UpdateDate.ToDisplay()),
                        SetReportData(item.OwnerName),
                        SetReportData(item.UpdateDateOwnerDisplay),
                        SetReportData(item.DelegatorName),
                        SetReportData(item.UpdateDelegateDisplay),
                        SetReportData(ReplaceHTML(item.SRSubject)),
                        SetReportData(ReplaceHTML(item.SRRemarkDisplay)),

                        SetReportData(item.ContactName),
                        SetReportData(item.ContactSurname),
                        SetReportData(item.Relationship),
                        SetReportData(addSingleQuote(item.ContactNo)),
                        SetReportData(item.MediaSourceName)
                    };
                    sb.AppendLine(string.Join(",", fields));
                }

                return Encoding.GetEncoding(874).GetBytes(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public byte[] CreateReportVerify(IList<ExportVerifyEntity> vfList, ExportVerifySearchFilter searchFilter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> search_criteria = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchFilter.ProductGroup))
                {
                    search_criteria.Add(SetReportData("ProductGroup:" + searchFilter.ProductGroupName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Product))
                {
                    search_criteria.Add(SetReportData("Product:" + searchFilter.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Campaign))
                {
                    search_criteria.Add(SetReportData("Campaign/Service:" + searchFilter.CampaignName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Type))
                {
                    search_criteria.Add(SetReportData("Type:" + searchFilter.TypeName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Area))
                {
                    search_criteria.Add(SetReportData("Area:" + searchFilter.AreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SubArea))
                {
                    search_criteria.Add(SetReportData("Sub Area:" + searchFilter.SubAreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerBranch))
                {
                    search_criteria.Add(SetReportData("Owner Branch:" + searchFilter.OwnerBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerSR))
                {
                    search_criteria.Add(SetReportData("Owner SR:" + searchFilter.OwnerSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRId))
                {
                    search_criteria.Add(SetReportData("SR ID:" + searchFilter.SRId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRIsverify))
                {
                    search_criteria.Add(SetReportData("Verify Result:" + searchFilter.SRIsverifyName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Description))
                {
                    search_criteria.Add(SetReportData("Description:" + searchFilter.Description));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Date From:" + searchFilter.SRDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateTo))
                {
                    search_criteria.Add(SetReportData("SR Created Date To:" + searchFilter.SRDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Time From:" + searchFilter.SRTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeTo))
                {
                    search_criteria.Add(SetReportData("SR Created Time To:" + searchFilter.SRTimeTo));
                }

                sb.AppendLine(string.Join(",", search_criteria));
                sb.AppendLine("");

                string[] headers = new string[]
                {
                    SetReportData("No"),
                    SetReportData("SR ID"),
                    SetReportData("เลขที่สัญญา / เลขที่บัญชีชื่อลูกค้า"),
                    SetReportData("ชื่อลูกค้า"),
                    SetReportData("นามสกุลลูกค้า"),
                    SetReportData("SR Owner"),
                    SetReportData("SR Created Date Time"),
                    SetReportData("SR Creator Branch"),
                    SetReportData("Product Group"),
                    SetReportData("Product"),

                    SetReportData("Campaign/Service"),
                    SetReportData("Type"),
                    SetReportData("Area"),
                    SetReportData("Sub-Area"),
                    SetReportData("Subject"),
                    SetReportData("SR Description"),
                    SetReportData("SR State"),
                    SetReportData("SR Status"),
                    SetReportData("Verify Result"),
                    SetReportData("Total Question"),
                    SetReportData("Total Passed"),

                    SetReportData("Total Failed"),
                    SetReportData("Total disregarded")
                };
                sb.AppendLine(string.Join(",", headers));

                int i = 0;
                foreach (var item in vfList)
                {
                    i += 1;
                    string[] fields = new string[]
                    {
                        SetReportData(i.ToString()),
                        SetReportData(addSingleQuote(item.SRId)),
                        SetReportData(addSingleQuote(item.AccountNo)),
                        SetReportData(item.CustomerFistname),
                        SetReportData(item.CustomerLastname),
                        SetReportData(item.SROwnerName),
                        SetReportData(item.SRCreateDateDisplay),
                        SetReportData(item.SRCreatorBranch),
                        SetReportData(item.ProductGroupName),
                        SetReportData(item.ProductName),

                        SetReportData(item.CampaignServiceName),
                        SetReportData(item.TypeName),
                        SetReportData(item.AreaName),
                        SetReportData(item.SubAreaName),
                        SetReportData(ReplaceHTML(item.SRSubject)),
                        SetReportData(ReplaceHTML(item.SRDescDisplay)),
                        SetReportData(item.SRState),
                        SetReportData(item.SRStatus),
                        SetReportData(item.IsVerifyResultDisplay),
                        SetReportData(item.TotalQuestion.ToString()),
                        SetReportData(item.TotalPass.ToString()),

                        SetReportData(item.TotalFailed.ToString()),
                        SetReportData(item.TotalDisregard.ToString())
                    };
                    sb.AppendLine(string.Join(",", fields));
                }

                return Encoding.GetEncoding(874).GetBytes(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public byte[] CreateReportVerifyDetail(IList<ExportVerifyDetailEntity> vfdList, ExportVerifyDetailSearchFilter searchFilter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> search_criteria = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchFilter.ProductGroup))
                {
                    search_criteria.Add(SetReportData("ProductGroup:" + searchFilter.ProductGroupName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Product))
                {
                    search_criteria.Add(SetReportData("Product:" + searchFilter.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Campaign))
                {
                    search_criteria.Add(SetReportData("Campaign/Service:" + searchFilter.CampaignName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Type))
                {
                    search_criteria.Add(SetReportData("Type:" + searchFilter.TypeName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.Area))
                {
                    search_criteria.Add(SetReportData("Area:" + searchFilter.AreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SubArea))
                {
                    search_criteria.Add(SetReportData("Sub Area:" + searchFilter.SubAreaName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerBranch))
                {
                    search_criteria.Add(SetReportData("Owner Branch:" + searchFilter.OwnerBranchName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.OwnerSR))
                {
                    search_criteria.Add(SetReportData("Owner SR:" + searchFilter.OwnerSRName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRId))
                {
                    search_criteria.Add(SetReportData("SR ID:" + searchFilter.SRId));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRIsverify))
                {
                    search_criteria.Add(SetReportData("Verify Result:" + searchFilter.SRIsverifyName));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Date From:" + searchFilter.SRDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRDateTo))
                {
                    search_criteria.Add(SetReportData("SR Created Date To:" + searchFilter.SRDateTo));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeFrom))
                {
                    search_criteria.Add(SetReportData("SR Created Time From:" + searchFilter.SRTimeFrom));
                }
                if (!string.IsNullOrWhiteSpace(searchFilter.SRTimeTo))
                {
                    search_criteria.Add(SetReportData("SR Created Time To:" + searchFilter.SRTimeTo));
                }

                sb.AppendLine(string.Join(",", search_criteria));
                sb.AppendLine("");

                string[] headers = new string[]
                {
                    SetReportData("No"),
                    SetReportData("SR ID"),
                    SetReportData("เลขที่สัญญา / เลขที่บัญชีชื่อลูกค้า"),
                    SetReportData("ชื่อลูกค้า"),
                    SetReportData("นามสกุลลูกค้า"),
                    SetReportData("Product Group"),
                    SetReportData("Product"),
                    SetReportData("Campaign/Service"),
                    SetReportData("Type"),
                    SetReportData("Area"),

                    SetReportData("Sub-Area"),
                    SetReportData("SR Owner"),
                    SetReportData("Verify Result"),
                    SetReportData("Question Group"),
                    SetReportData("Question"),
                    SetReportData("Answer")
                };
                sb.AppendLine(string.Join(",", headers));

                int i = 0;
                foreach (var item in vfdList)
                {
                    i += 1;
                    string[] fields = new string[]
                    {
                        SetReportData(i.ToString()),
                        SetReportData(addSingleQuote(item.SRId)),
                        SetReportData(addSingleQuote(item.AccountNo)),
                        SetReportData(item.CustomerFistname),
                        SetReportData(item.CustomerLastname),
                        SetReportData(item.ProductGroupName),
                        SetReportData(item.ProductName),
                        SetReportData(item.CampaignServiceName),
                        SetReportData(item.TypeName),
                        SetReportData(item.AreaName),

                        SetReportData(item.SubAreaName),
                        SetReportData(item.SROwnerName),
                        SetReportData(item.IsVerifyPassDisplay),
                        SetReportData(item.QuestionGroupName),
                        SetReportData(item.QuestionName),
                        SetReportData(item.VerifyResultDisplay)
                    };
                    sb.AppendLine(string.Join(",", fields));
                }

                return Encoding.GetEncoding(874).GetBytes(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public string GetReportPath()
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetReportPath();
        }

        public string GetTriggerDays()
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetTriggerDays();
        }

        public string GetNumOfDaySRReport()
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetNumOfDaySRReport();
        }

        private string ReplaceHTML(string fieldStr)
        {
            if (!string.IsNullOrWhiteSpace(fieldStr))
            {
                return ApplicationHelpers.StripHTML(HttpUtility.HtmlDecode(ApplicationHelpers.StripNewLine(fieldStr.Replace(",", string.Empty))));
            }
            else
            {
                return "";
            }
        }

        private string SetReportData(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return "\"" + str.Trim() + "\"";
            }
            else
            {
                return "\"\"";
            }
        }

        public void SaveAuditLogSRReport(ExportSRTaskResponse taskResponse)
        {
            if (taskResponse != null)
            {
                var _auditLog = new AuditLogEntity();
                _auditLog.Module = Constants.Module.Batch;
                _auditLog.Action = taskResponse.TaskAction;
                _auditLog.IpAddress = ApplicationHelpers.GetClientIP();
                if (taskResponse.StatusResponse.Status == Constants.StatusResponse.Success)
                { _auditLog.Status = LogStatus.Success; }
                else
                { _auditLog.Status = LogStatus.Fail; }
                _auditLog.Detail = taskResponse.ToString();
                _auditLog.CreateUserId = null;
                _auditLog.CreatedDate = taskResponse.SchedDateTime;
                AppLog.AuditLog(_auditLog);
            }
        }

        public bool UploadFileSRReport(SRReportSFtpConfig cfg)
        {
            bool succ = false;
            using (var sftp = new SftpClient(cfg.SFtp_Host, cfg.SFtp_Port, cfg.SFtp_UserName, cfg.SFtp_Password))
            {
                try
                {
                    sftp.Connect();
                    string fileName;
                    using (var fs = new FileStream(cfg.UploadFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fileName = Path.GetFileName(fs.Name);
                        string uploadPath = $"{cfg.RemoteDir}/{fileName}";
                        Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("FileName", fileName).ToInputLogString());
                        sftp.BufferSize = 4 * 1024;
                        sftp.UploadFile(fs, uploadPath, null);
                    }
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("FileName", fileName).ToSuccessLogString());
                    succ = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception occur:\n", ex);
                    Logger.Info(_logMsg.Clear().SetPrefixMsg("Upload File").Add("Error Message", ex.Message).ToFailLogString());
                    throw;
                }
                finally
                { sftp.Disconnect(); }
            }
            return succ;
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

        //public DataTable GetExportJobs(ExportJobsSearchFilter searchFilter)
        //{
        //    _reportDataAccess = new ReportDataAccess(_context);
        //    IList<ExportJobsEntity> exportJobs = _reportDataAccess.GetExportJobs(searchFilter);
        //    DataTable dt = DataTableHelpers.ConvertTo(exportJobs);
        //    return dt;
        //}

        public IList<ExportJobsEntity> GetExportJobs(ExportJobsSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportJobs(searchFilter);
        }

        //public DataTable GetExportSR(ExportSRSearchFilter searchFilter)
        //{
        //    _reportDataAccess = new ReportDataAccess(_context);
        //    DataTable dt = null;
        //    if (searchFilter.ReportType == "Complaint")
        //    {
        //        IList<ExportSRComplaintEntity> exportCpn = _reportDataAccess.GetExportComplaint(searchFilter);
        //        Logger.Debug("Select Report 3");

        //        exportCpn.ToList().ForEach(x =>
        //        {
        //            x.SendMailSubject = ApplicationHelpers.RemoveAllHtmlTags(x.SendMailSubject);
        //            x.SendMailBody = ApplicationHelpers.RemoveAllHtmlTags(x.SendMailBody);
        //        });
        //        Logger.Debug("Select Report 4");
        //        dt = DataTableHelpers.ConvertToEx(exportCpn);
        //        Logger.Debug("Select Report 5");
        //    }
        //    else
        //    {
        //        IList<ExportSREntity> exportSR = _reportDataAccess.GetExportSR(searchFilter);
        //        dt = DataTableHelpers.ConvertTo(exportSR);
        //    }
        //    return dt;
        //}

        public IList<ExportSREntity> GetExportSR(ExportSRSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportSR(searchFilter);
        }

        public DataTable GetExportComplaint(ExportSRSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            IList<ExportSRComplaintEntity> exportCpn = _reportDataAccess.GetExportComplaint(searchFilter);
            DataTable dt = DataTableHelpers.ConvertToEx(exportCpn);
            if (dt.Columns.Contains("ลำดับ"))
            {
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                    dr["ลำดับ"] = ++i;
            }
            return dt;
        }

        //public DataTable GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter)
        //{
        //    _reportDataAccess = new ReportDataAccess(_context);
        //    IList<ExportVerifyDetailEntity> exportVerifyDetail = _reportDataAccess.GetExportVerifyDetail(searchFilter);
        //    DataTable dt = DataTableHelpers.ConvertTo(exportVerifyDetail);
        //    return dt;
        //}

        public IList<ExportVerifyDetailEntity> GetExportVerifyDetail(ExportVerifyDetailSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportVerifyDetail(searchFilter);
        }

        //public DataTable GetExportVerify(ExportVerifySearchFilter searchFilter)
        //{
        //    _reportDataAccess = new ReportDataAccess(_context);
        //    IList<ExportVerifyEntity> exportVerify = _reportDataAccess.GetExportVerify(searchFilter);
        //    DataTable dt = DataTableHelpers.ConvertTo(exportVerify);
        //    return dt;
        //}

        public IList<ExportVerifyEntity> GetExportVerify(ExportVerifySearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportVerify(searchFilter);
        }

        //public DataTable GetExportNcb(ExportNcbSearchFilter searchFilter)
        //{
        //    _reportDataAccess = new ReportDataAccess(_context);
        //    IList<ExportNcbEntity> exportNcb = _reportDataAccess.GetExportNcb(searchFilter);
        //    DataTable dt = DataTableHelpers.ConvertTo(exportNcb);
        //    return dt;
        //}

        public IList<ExportNcbEntity> GetExportNcb(ExportNcbSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportNcb(searchFilter);
        }

        public IList<ExportSREntity> GetExportSRMonthly(ExportSRSearchFilter searchFilter)
        {
            _reportDataAccess = new ReportDataAccess(_context);
            return _reportDataAccess.GetExportSRMonthly(searchFilter);
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
