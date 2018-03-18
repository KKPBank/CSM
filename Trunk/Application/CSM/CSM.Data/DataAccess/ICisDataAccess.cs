using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ICisDataAccess
    {
        int? GetCustomerIdByCisCode(string cisCode);        
        bool SaveCisCorporate(List<CisCorporateEntity> cisCorporates);
        bool SaveCisCorporateComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisIndividual(List<CisIndividualEntity> cisIndividuals);
        bool SaveCisIndividualComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisProductGroup(List<CisProductGroupEntity> cisProductGroups);
        bool SaveCisSubscription(List<CisSubscriptionEntity> cisSubscriptions);
        bool SaveCisSubscriptionComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisTitle(List<CisTitleEntity> cisTitles);
        bool SaveCisProvince(List<CisProvinceEntity> cisProvinces);
        bool SaveCisDistrict(List<CisDistrictEntity> cisDistricts);
        bool SaveCisSubDistrict(List<CisSubDistrictEntity> cisSubDistricts);
        bool SaveCisPhoneType(List<CisPhoneTypeEntity> cisPhones);
        bool SaveCisEmailType(List<CisEmailTypeEntity> cisEmails);
        bool SaveCisSubscribeAddress(List<CisSubscribeAddressEntity> cisSubscribeAdds);
        bool SaveCisSubscribePhone(List<CisSubscribePhoneEntity> cisSubscribePhones);
        bool SaveCisSubscribeEmail(List<CisSubscribeMailEntity> cisSubscribeMails);
        bool SaveCisSubscribeAddressComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisSubscribePhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisSubscribeEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisAddressType(List<CisAddressTypeEntity> cisAddresstypes);        
        bool SaveCisSubscriptionType(List<CisSubscriptionTypeEntity> cisSubscriptionTypes);
        bool SaveCisSubscriptionTypeComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCustomerPhone(List<CisCustomerPhoneEntity> cisCustomerPhones);
        bool SaveCisCustomerPhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCustomerEmail(List<CisCustomerEmailEntity> cisCustomerEmails);
        bool SaveCisCustomerEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCountry(List<CisCountryEntity> cisCountry);
        List<CisCorporateEntity> GetCISCorExport();
        List<CisIndividualEntity> GetCISIndivExport();
        List<CisSubscriptionEntity> GetCisSubscriptionExport();
        List<CisSubscribeAddressEntity> GetCisSubscribeAddressExport();
        List<CisSubscribePhoneEntity> GetCisSubscribePhoneExport();
        List<CisSubscribeMailEntity> GetCisSubscriptEmailExport();
        List<CisSubscriptionTypeEntity> GetCisSubscriptionTypeExport();
        List<CisCustomerPhoneEntity> GetCisCustomerPhoneExport();
        List<CisCustomerEmailEntity> GetCisCustomerEmailExport();
        void DeleteCisTitle();
        void DeleteCisCorporate();
        void DeleteCisIndividual();
        void DeleteCisProductGroup();
        void DeleteCisSubscription();
        void DeleteCisProvince();
        void DeleteCisDistrict();
        void DeleteCisSubDistrict();
        void DeleteCisPhoneType();
        void DeleteCisEmailType();
        void DeleteCisSubscribeAddress();
        void DeleteCisSubscribePhone();
        void DeleteCisSubscribeEmail();
        void DeleteCisAddressType();
        void DeleteCisSubscriptionType();
        void DeleteCisCustomerPhone();
        void DeleteCisCustomerEmail();
        void DeleteCisCountry();
    }
}
