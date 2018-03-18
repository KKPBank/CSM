using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class SrPageDataAccess : ISrPageDataAccess
    {
        private readonly CSMContext _context;

        public SrPageDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public List<SrPageItemEntity> GetSrPageList()
        {
            List<SrPageItemEntity> list =
                _context.TB_C_SR_PAGE.OrderBy(l => l.SR_PAGE_ID).Select(item => new SrPageItemEntity
                {
                    SrPageId = item.SR_PAGE_ID,
                    SrPageCode = item.SR_PAGE_CODE,
                    SrPageName = item.SR_PAGE_NAME
                }).ToList();

            return list;
        }
    }
}
