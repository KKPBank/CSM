using CSM.Common.Utilities;
using CSM.Entity.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CSM.Entity
{
    public class ExistingProductEntity
    {
        public List<AccountEmailEntity> AccountEmailList { get; set; }
        public List<AccountPhoneEntity> AccountPhoneList { get; set; }
        public List<AccountAddressEntity> AccountAddressList { get; set; }

        public string EmailDisplay
        {
            get
            {
                if (AccountEmailList != null)
                    return StringHelpers.ConvertListToString(AccountEmailList.Select(x => x.EmailAccount).ToList<object>(), ", ");
                else
                    return "";
            }
        }

        private List<AccountPhoneEntity> AccountPhoneListNotFax
        {
            get
            {
                return AccountPhoneList == null ? null : AccountPhoneList.Where(x => x.PhoneTypeCode != Constants.PhoneTypeCode.Fax).ToList();
            }
        }
        public string PhoneNo1
        {
            get
            {
                string strPhone = "";
                if (AccountPhoneListNotFax != null && AccountPhoneListNotFax.Count() > 0)
                {
                    strPhone = string.Format(CultureInfo.InvariantCulture, "{0}{1}", !string.IsNullOrEmpty(AccountPhoneListNotFax[0].PhoneNo) ? AccountPhoneListNotFax[0].PhoneNo : ""
                        , !string.IsNullOrEmpty(AccountPhoneListNotFax[0].PhoneExt) ? " ต่อ " + AccountPhoneListNotFax[0].PhoneExt : "");
                }

                return strPhone;
            }
        }
        public string PhoneNo2
        {
            get
            {
                string strPhone = "";
                if (AccountPhoneListNotFax != null && AccountPhoneListNotFax.Count() > 1)
                {
                    strPhone = string.Format(CultureInfo.InvariantCulture, "{0}{1}", !string.IsNullOrEmpty(AccountPhoneListNotFax[1].PhoneNo) ? AccountPhoneListNotFax[1].PhoneNo : ""
                        , !string.IsNullOrEmpty(AccountPhoneListNotFax[1].PhoneExt) ? " ต่อ " + AccountPhoneListNotFax[1].PhoneExt : "");
                }

                return strPhone;
            }
        }
        public string PhoneNo3
        {
            get
            {
                string strPhone = "";
                if (AccountPhoneListNotFax != null && AccountPhoneListNotFax.Count() > 2)
                {
                    strPhone = string.Format(CultureInfo.InvariantCulture, "{0}{1}", !string.IsNullOrEmpty(AccountPhoneListNotFax[2].PhoneNo) ? AccountPhoneListNotFax[2].PhoneNo : ""
                        , !string.IsNullOrEmpty(AccountPhoneListNotFax[2].PhoneExt) ? " ต่อ " + AccountPhoneListNotFax[2].PhoneExt : "");
                }

                return strPhone;
            }
        }

        public string FaxDisplay
        {
            get
            {
                if (AccountPhoneList != null)
                {
                    return StringHelpers.ConvertListToString(
                            AccountPhoneList.Where(x => x.PhoneTypeCode == Constants.PhoneTypeCode.Fax)
                                .Select( x =>
                                        string.IsNullOrEmpty(x.PhoneExt)
                                            ? x.PhoneNo
                                            : (x.PhoneNo + " ต่อ " + x.PhoneExt))
                                .ToList<object>(), ", ");
                }
                else
                    return "";
            }
        }
    }

    public class ExistingProductSearchFilter : Pager
    {
        public int? CustomerId { get; set; }
        public string ProductType { get; set; }
        public string ProductGroup { get; set; }
        public string SubscriptionCode { get; set; }
        public int? AccountId { get; set; }
    }
    
}
