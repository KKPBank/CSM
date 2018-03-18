using CSM.Common.Utilities;

namespace CSM.Entity.Common
{
    public class SearchFilter
    {
        public SearchFilter()
        {
            order = "asc";
            limit = 15;
            offset = 0;
            total = 0;
        }

        public string sort { get; set; }
        public string order { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public int total { get; set; }

        public void SetPagingParameter(BootstrapParameters p)
        {
            this.sort = p.sort;
            this.order = p.order;
            this.limit = p.limit;
            this.offset = p.offset;
        }
    }
}
