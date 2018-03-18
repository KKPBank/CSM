using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class AfsAssetEntity
    {
        public int? EmployeeId { get; set; }
        public int AssetId { get; set; }
        public string AssetNo { get; set; }
        public string AssetType { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDes { get; set; }
        public string AfsRefNo { get; set; }
        public string Amphur { get; set; }
        public string Province { get; set; }     
        public string SaleName { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }

        public UserEntity SaleOwnerUser { get; set; }
        public BranchEntity SaleOwnerBranch { get; set; }
    }
}
