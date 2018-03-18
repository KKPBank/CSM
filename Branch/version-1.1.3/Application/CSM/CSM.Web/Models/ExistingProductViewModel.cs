using System;
using CSM.Entity;
using System.Collections.Generic;

namespace CSM.Web.Models
{
    [Serializable]
    public class ExistingProductViewModel
    {
        public CustomerInfoViewModel CustomerInfo { get; set; }
        public AccountSearchFilter SearchFilter { get; set; }
        public IEnumerable<AccountEntity> AccountList { get; set; }
        public ExistingProductEntity DetailProduct { get; set; }
    }    
}