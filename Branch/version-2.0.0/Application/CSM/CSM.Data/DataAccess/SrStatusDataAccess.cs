using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CSM.Entity;
using CSM.Common.Utilities;
using System;

namespace CSM.Data.DataAccess
{
    public class SrStatusDataAccess : ISrStatusDataAccess
    {
        private readonly CSMContext _context;

        public SrStatusDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "SrStatus"
        public List<SRStatusEntity> GetSrStatusList()
        {
            List<SRStatusEntity> list =
                _context.TB_C_SR_STATUS.OrderBy(l => l.SR_STATUS_NAME).Select(item => new SRStatusEntity
                {
                    SRStatusId = item.SR_STATUS_ID,
                    SRStatusCode = item.SR_STATUS_CODE,
                    SRStatusName = item.SR_STATUS_NAME
                }).ToList();

            return list;
        }
        #endregion

        public List<SRStatusEntity> AutoCompleteSearchSrStatus()
        {
            var query = _context.TB_C_SR_STATUS.AsQueryable();
            query = query.OrderBy(q => q.SR_STATUS_ID);

            return query.Select(item => new SRStatusEntity
            {
                SRStatusId = item.SR_STATUS_ID,
                SRStatusName = item.SR_STATUS_NAME
            }).ToList();
        }

        public IEnumerable<SRStateEntity> GetSRState(int? id, string name)
        {
            return (from st in _context.TB_C_SR_STATE.AsNoTracking()
                    where (!id.HasValue || st.SR_STATE_ID == id.Value)
                         && ((name ?? "").Trim().Length == 0 || st.SR_STATE_NAME == name)
                    select new SRStateEntity()
                    {
                        SRStateId = st.SR_STATE_ID,
                        SRStateName = st.SR_STATE_NAME
                    });
        }

        public SRStatusEntity GetSrStatus(string code)
        {
            var obj = _context.TB_C_SR_STATUS.FirstOrDefault(s => s.SR_STATUS_CODE.ToUpper() == code.ToUpper());
            if (obj == null)
                return null;

            return new SRStatusEntity
            {
                SRStatusId = obj.SR_STATUS_ID,
                SRStatusCode = obj.SR_STATUS_CODE,
                SRStatusName = obj.SR_STATUS_NAME
            };
        }

        IEnumerable<TB_C_SR_STATUS> getAvailableNextSrStatuses(int srId)
        {
            var sr = _context.TB_T_SR.AsNoTracking().SingleOrDefault(s => s.SR_ID == srId);

            if (sr == null || !sr.SR_STATUS_ID.HasValue
                || !sr.PRODUCTGROUP_ID.HasValue || !sr.PRODUCT_ID.HasValue || !sr.CAMPAIGNSERVICE_ID.HasValue
                || !sr.AREA_ID.HasValue || !sr.SUBAREA_ID.HasValue || !sr.TYPE_ID.HasValue)
            {
                return null;
            }

            return (from c in _context.TB_C_SR_STATUS_CHANGE.AsNoTracking()
                    from s in _context.TB_C_SR_STATUS.Where(x => x.SR_STATUS_ID == c.TO_SR_STATUS_ID).DefaultIfEmpty()
                    where c.FROM_SR_STATUS_ID == sr.SR_STATUS_ID.Value
                        && c.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID.Value
                        && c.PRODUCT_ID == sr.PRODUCT_ID.Value
                        && c.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID.Value
                        && c.AREA_ID == sr.AREA_ID.Value
                        && c.SUBAREA_ID == sr.SUBAREA_ID.Value
                        && c.TYPE_ID == sr.TYPE_ID.Value
                    orderby s.SR_STATUS_ID
                    select s).Distinct();
        }

        public IEnumerable<SRStatusEntity> AutoCompleteStatus(int maxResult, string keyword, int? stateId, int? srId, object isAllStatuss)
        {
            IEnumerable<TB_C_SR_STATUS> toStatus = null;
            if (srId.HasValue)
            { toStatus = getAvailableNextSrStatuses(srId.Value); }
            else
            { toStatus = _context.TB_C_SR_STATUS.AsNoTracking(); }

            return (from a in toStatus
                    where a.SR_STATUS_NAME.Contains(keyword)
                        && (stateId.HasValue == false || a.SR_STATE_ID == stateId.Value)
                    select new SRStatusEntity()
                    {
                        SRStatusId = a.SR_STATUS_ID,
                        SRStatusCode = a.SR_STATUS_CODE,
                        SRStatusName = a.SR_STATUS_NAME
                    }).Take(maxResult);
        }

        public IEnumerable<SRStateEntity> AutoCompleteState(int maxResult, string keyword, int? statusId, int? srId, bool? isAllStatus)
        {
            IEnumerable<TB_C_SR_STATUS> toStatus = null;
            if (srId.HasValue)
            { toStatus = getAvailableNextSrStatuses(srId.Value); }
            else
            { toStatus = _context.TB_C_SR_STATUS.AsNoTracking(); }

            return (from a in _context.TB_C_SR_STATE.AsNoTracking()
                    from b in toStatus.Where(x => x.SR_STATE_ID == a.SR_STATE_ID)
                    where a.SR_STATE_NAME.Contains(keyword)
                        && (statusId.HasValue == false || b.SR_STATUS_ID == statusId.Value)
                    select new SRStateEntity()
                    {
                        SRStateId = a.SR_STATE_ID,
                        SRStateName = a.SR_STATE_NAME
                    }).GroupBy(g => new { g.SRStateId, g.SRStateName })
                    .Select(x => new SRStateEntity()
                    {
                        SRStateId = x.Key.SRStateId,
                        SRStateName = x.Key.SRStateName
                    }).Take(maxResult);
        }

        public IEnumerable<SRStatusEntity> GetSrStatus(int? id, int? stateId, string code, string name, string status, int? pageId)
        {
            var q = (from s in _context.TB_C_SR_STATUS.AsNoTracking()
                     join t in _context.TB_C_SR_STATE on s.SR_STATE_ID equals t.SR_STATE_ID
                     join create in _context.TB_R_USER.AsNoTracking() on s.CREATE_USER equals create.USER_ID into gcu
                     from cu in gcu.DefaultIfEmpty()
                     join edit in _context.TB_R_USER.AsNoTracking() on s.UPDATE_USER equals edit.USER_ID into guu
                     from uu in guu.DefaultIfEmpty()
                     where (!id.HasValue || s.SR_STATUS_ID == id)
                         && (!stateId.HasValue || s.SR_STATE_ID == stateId)
                         && ((code ?? "").Trim().Length == 0 || s.SR_STATUS_CODE == code)
                         && ((name ?? "").Trim().Length == 0 || s.SR_STATUS_NAME == name)
                         && ((status ?? "").Trim().Length == 0 || s.STATUS == status)
                     select new SRStatusEntity
                     {
                         SRStatusId = s.SR_STATUS_ID,
                         SRStatusCode = s.SR_STATUS_CODE,
                         SRStatusName = s.SR_STATUS_NAME,
                         Status = s.STATUS,
                         SendHP = s.SR_STATUS_SEND_TO_HP,
                         SendRule = s.SR_STATUS_RULE,
                         CreateDate = s.CREATE_DATE,
                         UpdateDate = s.UPDATE_DATE,
                         SRStateId = s.SR_STATE_ID,
                         SRState = new SRStateEntity()
                         {
                             SRStateId = s.SR_STATE_ID,
                             SRStateName = t.SR_STATE_NAME
                         },
                         CreateUser = new UserEntity()
                         {
                             UserId = s.CREATE_USER ?? 0,
                             Firstname = cu.FIRST_NAME,
                             Lastname = cu.LAST_NAME,
                             PositionCode = cu.POSITION_CODE
                         },
                         UpdateUser = new UserEntity()
                         {
                             UserId = s.UPDATE_USER ?? 0,
                             Firstname = uu.FIRST_NAME,
                             Lastname = uu.LAST_NAME,
                             PositionCode = uu.POSITION_CODE
                         },
                         SRPages = (from ps in _context.TB_C_SR_PAGE_STATUS.Where(x => x.SR_STATUS_ID == s.SR_STATUS_ID)
                                    from p in _context.TB_C_SR_PAGE.Where(x => x.SR_PAGE_ID == ps.SR_PAGE_ID)
                                    select new SrPageItemEntity()
                                    {
                                        SrPageId = p.SR_PAGE_ID,
                                        SrPageCode = p.SR_PAGE_CODE,
                                        SrPageName = p.SR_PAGE_NAME
                                    }).ToList()
                     });
            if (pageId.HasValue)
            {
                q = q.Where(x => x.SRPages.Any(y => y.SrPageId == pageId));
            }
            return q;
        }

        public bool SaveStatus(SRStatusEntity model, ref string msg)
        {
            bool succ = chkDuplicate(model, ref msg);
            if (succ)
            {
                using (DbContextTransaction trans = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    bool isEdit = false;
                    DateTime now = DateTime.Now;
                    TB_C_SR_STATUS st = null;
                    succ = false;
                    try
                    {
                        if (model.SRStatusId == 0)
                        {
                            st = new TB_C_SR_STATUS();
                            st.CREATE_DATE = now;
                            st.CREATE_USER = model.CreateUser.UserId;
                        }
                        else
                        {
                            isEdit = true;
                            st = _context.TB_C_SR_STATUS.Where(r => r.SR_STATUS_ID == model.SRStatusId).Single();
                            if (st == null)
                            {
                                msg = $"SR Status ID: {model.SRStatusId} does not exist in database.";
                                trans.Rollback();
                                return false;
                            }
                        }

                        st.SR_STATUS_CODE = model.SRStatusCode;
                        st.SR_STATUS_NAME = model.SRStatusName;
                        st.SR_STATE_ID = model.SRStateId;
                        st.STATUS = model.Status;
                        st.SR_STATUS_SEND_TO_HP = model.SendHP;
                        st.SR_STATUS_RULE = model.SendRule;

                        st.UPDATE_DATE = now;
                        st.UPDATE_USER = model.UpdateUser.UserId;

                        if (!isEdit)
                        {
                            _context.TB_C_SR_STATUS.Add(st);
                        }
                        else
                        {
                            SetEntryStateModified(st);
                        }

                        Save();

                        #region ยกเลิก Page_Status
                        
                        ////Save Page Item
                        //List<int> delList = null;
                        //List<int> updList = null;
                        //List<int> addList = null;
                        //if (model.Old_SRPageIdList != null)
                        //{
                        //    delList = (from o in model.Old_SRPageIdList
                        //              join n in model.SRPageIdList on o equals n into grp
                        //              from d in grp.DefaultIfEmpty()
                        //              where d == 0
                        //              select o).ToList();

                        //    updList = (from o in model.Old_SRPageIdList
                        //              join n in model.SRPageIdList on o equals n
                        //              select o).ToList();

                        //    addList = (from n in model.SRPageIdList
                        //               join o in model.Old_SRPageIdList on n equals o into grp
                        //               from a in grp.DefaultIfEmpty()
                        //               where a == 0
                        //               select n).ToList();
                        //}
                        //else
                        //{
                        //    delList = new List<int>() { };
                        //    updList = new List<int>() { };
                        //    addList = model.SRPageIdList.AsEnumerable().ToList();
                        //}

                        //foreach (var del in delList)
                        //{
                        //    var d = _context.TB_C_SR_PAGE_STATUS.SingleOrDefault(x => x.SR_STATUS_ID == st.SR_STATUS_ID && x.SR_PAGE_ID == del);
                        //    if (d == null)
                        //    {
                        //        msg = $"Can not delete TB_C_SR_PAGE_STATUS ID [{del}] does not exist in database.";
                        //        trans.Rollback();
                        //        return false;
                        //    }
                        //    _context.TB_C_SR_PAGE_STATUS.Remove(d);
                        //}

                        //foreach (var upd in updList)
                        //{
                        //    var u = _context.TB_C_SR_PAGE_STATUS.SingleOrDefault(x => x.SR_STATUS_ID == st.SR_STATUS_ID && x.SR_PAGE_ID == upd);
                        //    if (u == null)
                        //    {
                        //        msg = $"Can not update TB_C_SR_PAGE_STATUS ID [{upd}] does not exist in database.";
                        //        trans.Rollback();
                        //        return false;
                        //    }
                        //    u.UPDATE_DATE = now;
                        //    u.UPDATE_USER = model.UpdateUser.UserId;
                        //    SetEntryStateModified(u);
                        //}

                        //foreach (var add in addList)
                        //{
                        //    var a = new TB_C_SR_PAGE_STATUS();
                        //    a.CREATE_DATE = now;
                        //    a.CREATE_USER = model.CreateUser.UserId;
                        //    a.UPDATE_DATE = now;
                        //    a.UPDATE_USER = model.UpdateUser.UserId;

                        //    a.SR_STATUS_ID = st.SR_STATUS_ID;
                        //    a.SR_PAGE_ID = add;
                        //    _context.TB_C_SR_PAGE_STATUS.Add(a);
                        //}

                        //Save();
                        
                        #endregion

                        trans.Commit();
                        succ = true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        msg = ex.ToString();
                    }
                }
            }
            return succ;
        }

        private bool chkDuplicate(SRStatusEntity model, ref string msg)
        {
            msg = string.Empty;
            var x = from a in _context.TB_C_SR_STATUS.AsNoTracking()
                    join b in _context.TB_C_SR_PAGE_STATUS on a.SR_STATUS_ID equals b.SR_STATUS_ID
                    where (model.SRStatusId <= 0 || a.SR_STATUS_ID != model.SRStatusId)
                        && a.SR_STATUS_CODE == model.SRStatusCode && a.SR_STATUS_NAME == model.SRStatusName
                    select a;
            if (x.Any())
            {
                msg += $"Duplicate SR Status: Code [{model.SRStatusCode}] Name [{model.SRStatusName}].";
            }
            return msg == string.Empty;
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
            _context.Entry(entityTo).State = EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion
    }
}
