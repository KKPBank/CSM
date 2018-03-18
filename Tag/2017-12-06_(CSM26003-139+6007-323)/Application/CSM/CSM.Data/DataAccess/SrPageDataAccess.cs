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

        public IEnumerable<SrPageItemEntity> GetSrPages(int? roleId, int? srStatusId)
        {
            return (from pag in _context.TB_C_SR_PAGE.AsNoTracking()
                    where (!roleId.HasValue || pag.TB_C_ROLE_SR_PAGE.Any(x => x.ROLE_ID == roleId.Value))
                        && (!srStatusId.HasValue || _context.TB_C_SR_PAGE_STATUS
                                                    .Any(x => x.SR_PAGE_ID == pag.SR_PAGE_ID
                                                                && x.SR_STATUS_ID == srStatusId.Value))
                    select new SrPageItemEntity()
                    {
                        SrPageId = pag.SR_PAGE_ID,
                        SrPageCode = pag.SR_PAGE_CODE,
                        SrPageName = pag.SR_PAGE_NAME
                    });
        }
    }
}
