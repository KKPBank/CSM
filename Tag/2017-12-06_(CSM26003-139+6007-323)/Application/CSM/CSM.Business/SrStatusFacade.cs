using System;
using System.Collections.Generic;
using CSM.Data.DataAccess;
using CSM.Entity;
using System.Linq;

namespace CSM.Business
{
    public class SrStatusFacade : ISrStatusFacade
    {
        private readonly CSMContext _context;
        private ISrStatusDataAccess _srStatusDataAccess;

        public SrStatusFacade()
        {
            _context = new CSMContext();
        }

        #region "SrStatus"

        public List<SRStatusEntity> GetSrStatusList()
        {
            _srStatusDataAccess = new SrStatusDataAccess(_context);
            return _srStatusDataAccess.GetSrStatusList();
        }

        public List<SRStatusEntity> GetSrStatus(int? id = null, int? stateId = null, string code = null, string name = null, string status = null, int? pageId = null)
        {
            return (new SrStatusDataAccess(_context)).GetSrStatus(id, stateId, code, name, status, pageId).ToList();
        }

        public List<SRStateEntity> GetSrState(int? id = null, string name = null)
        {
            return (new SrStatusDataAccess(_context)).GetSRState(id, name).ToList();
        }

        public List<SRStatusEntity> SearchSrStatus(SRStatusSearchFilter searchFilter)
        {
            var q = (new SrStatusDataAccess(_context)).GetSrStatus(searchFilter.SRStatusId, searchFilter.SRStateId, null, null, searchFilter.Status, searchFilter.SRPageId);
            searchFilter.TotalRecords = q.Count();

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            switch (searchFilter.SortField)
            {
                case "SRStateName":
                    q = searchFilter.SortOrder.ToUpper().Equals("ASC") ? q.OrderBy(x => x.SRState.SRStateName) : q.OrderByDescending(x => x.SRState.SRStateName);
                    break;
            }
            return q.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public List<SRStatusEntity> AutoCompleteStatus(int maxResult, string keyword, int? stateId, int? srId, bool? isAllStatus)
        {
            return (new SrStatusDataAccess(_context)).AutoCompleteStatus(maxResult, keyword, stateId, srId, isAllStatus).ToList();
        }

        public List<SRStateEntity> AutoCompleteState(int maxResult, string keyword, int? statusId, int? srId, bool? isAllStatus)
        {
            return (new SrStatusDataAccess(_context)).AutoCompleteState(maxResult, keyword, statusId, srId, isAllStatus).ToList();
        }

        public List<SRStatusEntity> GetByState(int stateId)
        {
            return (new SrStatusDataAccess(_context)).GetSrStatus(null, stateId, null, null, null, null).ToList();
        }

        #endregion

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

        public bool Save(SRStatusEntity model, out string msg)
        {
            msg = string.Empty;
            bool succ = false;
            var da = new SrStatusDataAccess(_context);
            succ = da.SaveStatus(model, ref msg);
            return succ;
        }

        #endregion
    }
}
