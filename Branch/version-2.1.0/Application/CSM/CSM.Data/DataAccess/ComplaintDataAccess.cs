using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class ComplaintDataAccess : BaseDA<ComplaintDataAccess>
    {
        public ComplaintDataAccess(CSMContext context) : base(context) { }

        public IEnumerable<ComplaintSubjectEntity> AutoCompleteSubject(int maxResult, string keyword, int? typeId = null, int? rootCauseId = null, bool? isAllStatus = null)
        {
            var res = from s in _context.TB_M_COMPLAINT_SUBJECT.AsNoTracking()
                          join m in _context.TB_M_COMPLAINT_MAPPING.AsNoTracking()
                              on s.COMPLAINT_SUBJECT_ID equals m.COMPLAINT_SUBJECT_ID into gsm
                      from sm in gsm.DefaultIfEmpty()
                          join t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                              on sm.COMPLAINT_TYPE_ID equals t.COMPLAINT_TYPE_ID into gsmt
                      from smt in gsmt.DefaultIfEmpty()
                          join r in _context.TB_M_COMPLAINT_ROOT_CAUSE.AsNoTracking()
                              on sm.ROOT_CAUSE_ID equals r.ROOT_CAUSE_ID into gsmr
                      from smr in gsmr.DefaultIfEmpty()
                      where s.COMPLAINT_SUBJECT_NAME.Contains(keyword)
                        && (!typeId.HasValue || smt.COMPLAINT_TYPE_ID == typeId)
                        && (!rootCauseId.HasValue || smr.ROOT_CAUSE_ID == rootCauseId)
                        && (!isAllStatus.HasValue || s.STATUS == Constants.ApplicationStatus.Active)
                      select new ComplaintSubjectEntity
                      {
                          ComplaintSubjectId = s.COMPLAINT_SUBJECT_ID,
                          ComplaintSubjectName = s.COMPLAINT_SUBJECT_NAME,
                          ComplaintSubjectStatus = s.STATUS
                      };
            return res.Distinct().OrderBy(s => s.ComplaintSubjectName).Take(maxResult);
        }

        public IEnumerable<ComplaintTypeEntity> AutoCompleteType(int maxResult, string keyword, int? subjectId, int? rootCauseId, bool? isAllStatus)
        {
            var res = from t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                      join m in _context.TB_M_COMPLAINT_MAPPING.AsNoTracking()
                          on t.COMPLAINT_TYPE_ID equals m.COMPLAINT_TYPE_ID into gtm
                      from tm in gtm.DefaultIfEmpty()
                      join s in _context.TB_M_COMPLAINT_SUBJECT.AsNoTracking()
                          on tm.COMPLAINT_SUBJECT_ID equals s.COMPLAINT_SUBJECT_ID into gtms
                      from tms in gtms.DefaultIfEmpty()
                      join r in _context.TB_M_COMPLAINT_ROOT_CAUSE.AsNoTracking()
                          on tm.ROOT_CAUSE_ID equals r.ROOT_CAUSE_ID into gtmr
                      from tmr in gtmr.DefaultIfEmpty()
                      where t.COMPLAINT_TYPE_NAME.Contains(keyword)
                        && (!subjectId.HasValue || tms.COMPLAINT_SUBJECT_ID == subjectId)
                        && (!rootCauseId.HasValue || tmr.ROOT_CAUSE_ID == rootCauseId)
                        && (!isAllStatus.HasValue || t.STATUS == Constants.ApplicationStatus.Active)
                      select new ComplaintTypeEntity
                      {
                          ComplaintTypeId = t.COMPLAINT_TYPE_ID,
                          ComplaintTypeName = t.COMPLAINT_TYPE_NAME,
                          ComplaintTypeStatus = t.STATUS
                      };
            return res.Distinct().OrderBy(t => t.ComplaintTypeName).Take(maxResult);
        }

        public IEnumerable<RootCauseEntity> AutoCompleteRootCause(int maxResult, string keyword, int? subjectId, int? typeId, bool? isAllStatus)
        {
            var res = from r in _context.TB_M_COMPLAINT_ROOT_CAUSE.AsNoTracking()
                      join m in _context.TB_M_COMPLAINT_MAPPING.AsNoTracking()
                          on r.ROOT_CAUSE_ID equals m.ROOT_CAUSE_ID into grm
                      from rm in grm.DefaultIfEmpty()
                      join s in _context.TB_M_COMPLAINT_SUBJECT.AsNoTracking()
                          on rm.COMPLAINT_SUBJECT_ID equals s.COMPLAINT_SUBJECT_ID into grms
                      from rms in grms.DefaultIfEmpty()
                      join t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                          on rm.COMPLAINT_TYPE_ID equals t.COMPLAINT_TYPE_ID into grmt
                      from rmt in grmt.DefaultIfEmpty()
                      where r.ROOT_CAUSE_NAME.Contains(keyword)
                        && (!subjectId.HasValue || rms.COMPLAINT_SUBJECT_ID == subjectId)
                        && (!typeId.HasValue || rmt.COMPLAINT_TYPE_ID == typeId)
                        && (!isAllStatus.HasValue || r.STATUS == Constants.ApplicationStatus.Active)
                      select new RootCauseEntity
                      {
                          RootCauseId = r.ROOT_CAUSE_ID,
                          RootCauseName = r.ROOT_CAUSE_NAME,
                          RootCauseStatus = r.STATUS
                      };
            return res.Distinct().OrderBy(r => r.RootCauseName).Take(maxResult);
        }

        public IEnumerable<MSHBranchEntity> AutoCompleteHRBranch(int maxResult, string keyword, bool? isAllStatus)
        {
            var q = from b in _context.TB_M_MSH_BRANCH.AsNoTracking()
                    where b.BRANCH_NAME.Contains(keyword) && ((isAllStatus ?? false) || b.STATUS == "A")
                    select new MSHBranchEntity()
                    {
                        Branch_Id = b.BRANCH_ID,
                        Branch_Code = b.BRANCH_CODE,
                        Branch_Name = b.BRANCH_NAME
                    };
            return q.OrderBy(x => x.Branch_Name).Take(maxResult);
        }

        public IEnumerable<ComplaintHRBUEntity> GetDefaultBU(int maxResult, string bU1, string bU2, string bU3, bool? isAllStatus)
        {
            var q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                     where ((bU1 ?? "") == "" || bu.BU1 == bU1)
                         && ((bU2 ?? "") == "" || bu.BU2 == bU2)
                         && ((bU3 ?? "") == "" || bu.BU3 == bU3)
                         && (!isAllStatus.HasValue || !(bu.IS_DELETE ?? false))
                     select new ComplaintHRBUEntity()
                     {
                         BU1 = bu.BU1,
                         BU1Desc = bu.BU1DESC,
                         BU2 = bu.BU2,
                         BU2Desc = bu.BU2DESC,
                         BU3 = bu.BU3,
                         BU3Desc = bu.BU2DESC,
                         BU_Status = !(bu.IS_DELETE ?? false)
                     }).Distinct();
            return q.OrderBy(x => x.BU1Desc).ThenBy(x => x.BU2Desc).ThenBy(x => x.BU3Desc);
        }

        public ComplaintBUEntity GetBUByCode(string forBU, string buCode)
        {
            IEnumerable<ComplaintBUEntity> q = null;
            switch (forBU)
            {
                case "BU3":
                    {
                        q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                             where bu.BU3 == buCode
                             select new ComplaintBUEntity()
                             {
                                 BU_Code = bu.BU3,
                                 BU_Desc = bu.BU3DESC,
                                 BU_Status = !(bu.IS_DELETE ?? false)
                             });
                    }
                    break;
                case "BU2":
                    {
                        q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                             where bu.BU2 == buCode
                             select new ComplaintBUEntity()
                             {
                                 BU_Code = bu.BU2,
                                 BU_Desc = bu.BU2DESC,
                                 BU_Status = !(bu.IS_DELETE ?? false)
                             });
                    }
                    break;
                case "BU1":
                    {
                        q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                             where bu.BU1 == buCode
                             select new ComplaintBUEntity()
                             {
                                 BU_Code = bu.BU1,
                                 BU_Desc = bu.BU1DESC,
                                 BU_Status = !(bu.IS_DELETE ?? false)
                             });
                    }
                    break;
            }
            return (q?.FirstOrDefault());
        }

        public IEnumerable<ComplaintBUEntity> AutoCompleteBU3(int maxResult, string keyword, string bU1, string bU2, bool? isAllStatus)
        {
            var q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                     where ((bU1 ?? "") == "" || bu.BU1 == bU1)
                        && ((bU2 ?? "") == "" || bu.BU2 == bU2)
                        && (!isAllStatus.HasValue || !(bu.IS_DELETE ?? false))
                        && bu.BU3DESC.Contains(keyword)
                     select new ComplaintBUEntity()
                     {
                         BU_Code = bu.BU3,
                         BU_Desc = bu.BU3DESC,
                         BU_Status = !(bu.IS_DELETE ?? false)
                     }
                    ).Distinct();
            return q.OrderBy(s => s.BU_Desc).Take(maxResult);
        }

        public IEnumerable<ComplaintBUEntity> AutoCompleteBU2(int maxResult, string keyword, string bU1, string bU3, bool? isAllStatus)
        {
            var q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                     where ((bU1 ?? "") == "" || bu.BU1 == bU1)
                        && ((bU3 ?? "") == "" || bu.BU3 == bU3)
                        && (!isAllStatus.HasValue || !(bu.IS_DELETE ?? false))
                        && bu.BU2DESC.Contains(keyword)
                     select new ComplaintBUEntity()
                     {
                         BU_Code = bu.BU2,
                         BU_Desc = bu.BU2DESC,
                         BU_Status = !(bu.IS_DELETE ?? false)
                     }
                    ).Distinct();
            return q.OrderBy(s => s.BU_Desc).Take(maxResult);
        }

        public IEnumerable<ComplaintBUEntity> AutoCompleteBU1(int maxResult, string keyword, string bU2, string bU3, bool? isAllStatus)
        {
            var q = (from bu in _context.TB_M_HR_BU.AsNoTracking()
                     where ((bU2 ?? "") == "" || bu.BU2 == bU2) && ((bU3 ?? "") == "" || bu.BU3 == bU3)
                        && (!isAllStatus.HasValue || !(bu.IS_DELETE ?? false))
                        && bu.BU1DESC.Contains(keyword)
                     select new ComplaintBUEntity()
                     {
                         BU_Code = bu.BU1,
                         BU_Desc = bu.BU1DESC,
                         BU_Status = !(bu.IS_DELETE ?? false)
                     }
                    ).Distinct();
            return q.OrderBy(s => s.BU_Desc).Take(maxResult);
        }

        public IEnumerable<ComplaintTypeEntity> GetType(int? typetId)
        {
            return (from t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                    where t.COMPLAINT_TYPE_ID == typetId
                    select new ComplaintTypeEntity
                    {
                        ComplaintTypeId = t.COMPLAINT_TYPE_ID,
                        ComplaintTypeName = t.COMPLAINT_TYPE_NAME,
                        ComplaintTypeStatus = t.STATUS
                    }).OrderBy(x => x.ComplaintTypeName);
        }

        public IEnumerable<ComplaintBUGroupEntity> GetBUGroup(int? id = null)
        {
            return (from b in _context.TB_M_COMPLAINT_BU_GROUP.AsNoTracking()
                    where (!id.HasValue || b.COMPLAINT_BU_GROUP_ID == id)
                    select new ComplaintBUGroupEntity
                    {
                        ComplaintBUGroupId = b.COMPLAINT_BU_GROUP_ID,
                        ComplaintBUGroupName = b.COMPLAINT_BU_GROUP_NAME,
                        ComplaintBUGroupStatus = b.STATUS
                    }).OrderBy(x => x.ComplaintBUGroupName);
        }

        public IEnumerable<ComplaintCauseSummaryEntity> GetCauseSummary(int? id = null)
        {
            return (from c in _context.TB_M_COMPLAINT_CAUSE_SUMMARY.AsNoTracking()
                    where (!id.HasValue || c.FCAUSE_ID == id)
                    select new ComplaintCauseSummaryEntity
                    {
                        ComplaintCauseSummaryId = c.FCAUSE_ID,
                        ComplaintCauseSummaryName = c.FCAUSE_NAME,
                        ComplaintCauseSummaryStatus = c.STATUS
                    }).OrderBy(x => x.ComplaintCauseSummaryName);
        }

        public IEnumerable<ComplaintSummaryEntity> GetSummary(int? id = null)
        {
            return (from s in _context.TB_M_COMPLAINT_SUMMARY.AsNoTracking()
                    where (!id.HasValue || s.COMPLAINT_SUMMARY_ID == id)
                    select new ComplaintSummaryEntity
                    {
                        ComplaintSummaryId = s.COMPLAINT_SUMMARY_ID,
                        ComplaintSummaryName = s.COMPLAINT_SUMMARY_NAME,
                        ComplaintSummaryStatus = s.STATUS
                    }).OrderBy(x => x.ComplaintSummaryName);
        }

        public IEnumerable<ComplaintIssuesEntity> GetIssue(int? id = null)
        {
            return (from i in _context.TB_M_COMPLAINT_ISSUES.AsNoTracking()
                    where (!id.HasValue || i.COMPLAINT_ISSUES_ID == id)
                    select new ComplaintIssuesEntity
                    {
                        ComplaintIssuesId = i.COMPLAINT_ISSUES_ID,
                        ComplaintIssuesName = i.COMPLAINT_ISSUES_NAME,
                        ComplaintIssuesStatus = i.STATUS
                    }).OrderBy(x => x.ComplaintIssuesName);
        }

        public IEnumerable<ComplaintMappingEntity> GetMapping()
        {
            return from m in _context.TB_M_COMPLAINT_MAPPING.AsNoTracking()
                   select new ComplaintMappingEntity
                   {
                       ComplaintMappingId = m.MAPPING_ID,
                       ComplaintSubjectId = m.COMPLAINT_SUBJECT_ID,
                       ComplaintTypetId = m.COMPLAINT_TYPE_ID,
                       ComplaintMappingStatus = m.STATUS
                   };
        }

        public IEnumerable<RootCauseEntity> GetRootCause(int? id)
        {
            return (from r in _context.TB_M_COMPLAINT_ROOT_CAUSE.AsNoTracking()
                    where (!id.HasValue || r.ROOT_CAUSE_ID == id)
                    select new RootCauseEntity
                    {
                        RootCauseId = r.ROOT_CAUSE_ID,
                        RootCauseName = r.ROOT_CAUSE_NAME,
                        RootCauseStatus = r.STATUS
                    }).OrderBy(x => x.RootCauseName);
        }

        public IEnumerable<ComplaintSubjectEntity> GetSubject(int? subjectId)
        {
            return (from s in _context.TB_M_COMPLAINT_SUBJECT.AsNoTracking()
                    where (!subjectId.HasValue || s.COMPLAINT_SUBJECT_ID == subjectId)
                    select new ComplaintSubjectEntity
                    {
                        ComplaintSubjectId = s.COMPLAINT_SUBJECT_ID,
                        ComplaintSubjectName = s.COMPLAINT_SUBJECT_NAME,
                        ComplaintSubjectStatus = s.STATUS
                    }).OrderBy(x => x.ComplaintSubjectName);
        }

        public IEnumerable<ComplaintTypeEntity> GetComplaintType()
        {
            return (from t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                    select new ComplaintTypeEntity
                    {
                        ComplaintTypeId = t.COMPLAINT_TYPE_ID,
                        ComplaintTypeName = t.COMPLAINT_TYPE_NAME,
                        ComplaintTypeStatus = t.STATUS
                    }).OrderBy(x => x.ComplaintTypeName);
        }

        public IEnumerable<ComplaintMappingEntity> GetComplaintMapping(int? id = null, int ? subjectId = null, int? typeId = null, int? rootCause = null, bool? isAllStatus = null, int? take = null)
        {
            var res = from m in _context.TB_M_COMPLAINT_MAPPING.AsNoTracking()
                        join s in _context.TB_M_COMPLAINT_SUBJECT.AsNoTracking()
                            on m.COMPLAINT_SUBJECT_ID equals s.COMPLAINT_SUBJECT_ID
                        join t in _context.TB_M_COMPLAINT_TYPE.AsNoTracking()
                            on m.COMPLAINT_TYPE_ID equals t.COMPLAINT_TYPE_ID
                        join r in _context.TB_M_COMPLAINT_ROOT_CAUSE.AsNoTracking()
                            on m.ROOT_CAUSE_ID equals r.ROOT_CAUSE_ID
                   where (!subjectId.HasValue || m.COMPLAINT_SUBJECT_ID == subjectId)
                    && (!typeId.HasValue || m.COMPLAINT_TYPE_ID == typeId)
                    && (!rootCause.HasValue || m.ROOT_CAUSE_ID == rootCause)
                    && (!isAllStatus.HasValue || m.STATUS == Constants.ApplicationStatus.Active)
                   orderby s.COMPLAINT_SUBJECT_NAME, t.COMPLAINT_TYPE_NAME, r.ROOT_CAUSE_NAME
                   select new ComplaintMappingEntity
                   {
                       ComplaintMappingId = m.MAPPING_ID,
                       ComplaintSubjectId = m.COMPLAINT_SUBJECT_ID,
                       ComplaintTypetId = m.COMPLAINT_TYPE_ID,
                       RootCauseId = m.ROOT_CAUSE_ID
                   };
            if (take.HasValue)
            {
                return res.Take(take.Value);
            }
            return res;
        }

        public IEnumerable<MSHBranchEntity> GetHRBranch(int? id, string comp, string code, string name, string status)
        {
            return (from s in _context.TB_M_MSH_BRANCH.AsNoTracking()
                    where (!id.HasValue || s.BRANCH_ID == id)
                         && ((comp ?? "").Trim().Length == 0 || s.COMP_CODE == comp)
                         && ((code ?? "").Trim().Length == 0 || s.BRANCH_CODE == code)
                         && ((name ?? "").Trim().Length == 0 || s.BRANCH_NAME == name)
                         && ((status ?? "").Trim().Length == 0 || s.STATUS == status)
                    select new MSHBranchEntity
                    {
                        Branch_Id = s.BRANCH_ID,
                        Comp_Code = s.COMP_CODE,
                        Branch_Code = s.BRANCH_CODE,
                        Branch_Name = s.BRANCH_NAME,
                        Status = s.STATUS
                    }).OrderBy(x => x.Branch_Name);
        }
    }
}
