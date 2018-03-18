using System;
using System.Collections.Generic;
using System.Linq;

using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.OTP;
using CSM.Service.Messages.Common;

namespace CSM.Business
{
    public class ComplaintFacade : Common.BaseFacade2<ComplaintFacade>
    {
        public List<RootCauseEntity> AutoCompleteRootCause(int maxResult, string keyword, int? subjectId, int? typeId, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteRootCause(maxResult, keyword, subjectId, typeId, isAllStatus).ToList();
        }

        public List<ComplaintSubjectEntity> AutoCompleteSubject(int maxResult, string keyword, int? typeId, int? rootCauseId, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteSubject(maxResult, keyword, typeId, rootCauseId, isAllStatus).ToList();
        }

        public List<ComplaintTypeEntity> AutoCompleteType(int maxResult, string keyword, int? subjectId, int? rootCauseId, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteType(maxResult, keyword, subjectId, rootCauseId, isAllStatus).ToList();
        }

        public List<ComplaintBUEntity> AutoCompleteBU3(int maxResult, string keyword, string bU1, string bU2, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteBU3(maxResult, keyword, bU1, bU2, isAllStatus).ToList();
        }

        public OTPResultSvcResponse OTPRequest(OTPResultSvcRequest req) { return OTPResult(req); }

        public OTPResultSvcResponse OTPResult(OTPResultSvcRequest req)
        {
            bool valid = false;
            string msg = string.Empty;
            OTPResultSvcResponse res = new OTPResultSvcResponse();
            using (var commonFacade = new CommonFacade())
            {
                valid = commonFacade.VerifyServiceRequest<Header>(req.Header);
                res.Header = new Header
                {
                    service_name = req.Header.service_name,
                    channel_id = req.Header.channel_id,
                    user_name = req.Header.user_name,
                    password = req.Header.password,
                    transaction_date = req.Header.transaction_date,
                    reference_no = req.Header.reference_no,
                    system_code = req.Header.system_code
                };
            }
            if (valid)
            {
                using (var srFacade = new ServiceRequestFacade())
                {
                    if (srFacade.UpdateSendSMSVerify(req, out msg))
                    {
                        res.STATUS = "0";
                        res.ERROR_CODE = "0";
                        res.ERROR_DESC = "";
                    }
                    else
                    {
                        res.STATUS = "1";
                        res.ERROR_CODE = "1";
                        res.ERROR_DESC = msg;
                    }
                }
            }
            else
            {
                res.STATUS = "1";
                res.ERROR_CODE = "1";
                res.ERROR_DESC = "User name or password is invalid.";
            }
            return res;
        }

        public List<ComplaintBUEntity> AutoCompleteBU2(int maxResult, string keyword, string bU1, string bU3, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteBU2(maxResult, keyword, bU1, bU3, isAllStatus).ToList();
        }

        public List<ComplaintBUEntity> AutoCompleteBU1(int maxResult, string keyword, string bU2, string bU3, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteBU1(maxResult, keyword, bU2, bU3, isAllStatus).ToList();
        }

        public List<MSHBranchEntity> AutoCompleteHRBranch(int maxResult, string keyword, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).AutoCompleteHRBranch(maxResult, keyword, isAllStatus).ToList();
        }

        public ComplaintSubjectEntity GetSubjectById(int subjectId)
        {
            return (new ComplaintDataAccess(_context)).GetSubject(subjectId).FirstOrDefault();
        }

        public List<ComplaintMappingEntity> GetComplaintMapping(int? subjectId = null, int? typeId = null, int? rootCause = null, bool? isAllStatus = null, int? take = null)
        {
            return (new ComplaintDataAccess(_context)).GetComplaintMapping(null, subjectId, typeId, rootCause, isAllStatus, take).ToList();
        }

        public ComplaintTypeEntity GetTypeById(int complaintTypetId)
        {
            return (new ComplaintDataAccess(_context)).GetType(complaintTypetId).FirstOrDefault();
        }

        public RootCauseEntity GetRootCauseById(int rootCauseId)
        {
            return (new ComplaintDataAccess(_context)).GetRootCause(rootCauseId).FirstOrDefault();
        }

        public List<ComplaintIssuesEntity> GetIssue()
        {
            return (new ComplaintDataAccess(_context)).GetIssue().ToList();
        }

        public ComplaintIssuesEntity GetIssueById(int id)
        {
            return (new ComplaintDataAccess(_context)).GetIssue(id).FirstOrDefault();
        }

        public List<ComplaintBUGroupEntity> GetBUGroup()
        {
            return (new ComplaintDataAccess(_context)).GetBUGroup().ToList();
        }

        public ComplaintBUGroupEntity GetBUGroupById(int id)
        {
            return (new ComplaintDataAccess(_context)).GetBUGroup(id).FirstOrDefault();
        }

        public List<ComplaintCauseSummaryEntity> GetCauseSummary()
        {
            return (new ComplaintDataAccess(_context)).GetCauseSummary().ToList();
        }

        public ComplaintCauseSummaryEntity GetCauseSummaryById(int id)
        {
            return (new ComplaintDataAccess(_context)).GetCauseSummary(id).FirstOrDefault();
        }

        public List<ComplaintSummaryEntity> GetSummary()
        {
            return (new ComplaintDataAccess(_context)).GetSummary().ToList();
        }

        public ProductGroupEntity GetProductGroupById(int? id)
        {
            return (new ProductGroupDataAccess(_context)).GetProductGroup(id).FirstOrDefault();
        }

        public ProductEntity GetProductById(int id)
        {
            return (new ProductDataAccess(_context)).GetProduct(id).FirstOrDefault();
        }

        public CampaignServiceEntity GetCampaignById(int id)
        {
            return (new CampaignServiceDataAccess(_context)).GetCampaign(campaignId: id).FirstOrDefault();
        }

        public ComplaintMappingEntity GetMappingById(int id)
        {
            return (new ComplaintDataAccess(_context)).GetComplaintMapping(id).FirstOrDefault();
        }

        public ComplaintSummaryEntity GetSummaryById(int id)
        {
            return (new ComplaintDataAccess(_context)).GetSummary(id).FirstOrDefault();
        }

        public List<ComplaintHRBUEntity> GetDefaultBU(int maxResult, string bU1, string bU2, string bU3, bool? isAllStatus)
        {
            return (new ComplaintDataAccess(_context)).GetDefaultBU(maxResult, bU1, bU2, bU3, isAllStatus).ToList();
        }

        public ComplaintBUEntity GetBUByCode(string forBU, string buCode)
        {
            return (new ComplaintDataAccess(_context)).GetBUByCode(forBU, buCode);
        }

        public List<MSHBranchEntity> GetHRBranch(int? id = null, string comp = null, string code = null, string name = null, string status = null)
        {
            return (new ComplaintDataAccess(_context)).GetHRBranch(id, comp, code, name, status).ToList();
        }

        public MSHBranchEntity GetHRBranchById(int? id)
        {
            return GetHRBranch(id ?? 0).FirstOrDefault();
        }
    }
}
