using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.SchedTask;

namespace CSM.Business
{
    public interface ICisFacade : IDisposable
    {
        string GetParameter(string paramName);
        List<CisCorporateEntity> ReadFileCisCorporate(string filePath, string fiPrefix, ref int numOfCor, ref string fiCor, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisIndividualEntity> ReadFileCisIndividual(string filePath, string fiPrefix, ref int numOfIndiv, ref string fiIndiv, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisProductGroupEntity> ReadFileCisProductGroup(string filePath, string fiPrefix, ref int numOfProd, ref string fiProd, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubscriptionEntity> ReadFileCisSubscription(string filePath, string fiPrefix, ref int numOfSub, ref string fiSub, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisTitleEntity> ReadFileCisTitle(string filePath, string fiPrefix, ref int numOfTi, ref string fiTi, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisProvinceEntity> ReadFileCisProvince(string filePath, string fiPrefix, ref int numOfPro, ref string fiPro, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisDistrictEntity> ReadFileCisDistrict(string filePath, string fiPrefix, ref int numOfDis, ref string fiDis, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubDistrictEntity> ReadFileCisSubDistrict(string filePath, string fiPrefix, ref int numOfSubDis, ref string fiSubDis, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisPhoneTypeEntity> ReadFileCisPhoneType(string filePath, string fiPrefix, ref int numOfPhonetype, ref string fiPhonetype, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisEmailTypeEntity> ReadFileCisEmailType(string filePath, string fiPrefix, ref int numOfEmailtype, ref string fiEmailtype, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubscribeAddressEntity> ReadFileCisSubscribeAddress(string filePath, string fiPrefix, ref int numOfSubAdd, ref string fiSubAdd, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubscribePhoneEntity> ReadFileCisSubscribePhone(string filePath, string fiPrefix, ref int numOfSubPhone, ref string fiSubPhone, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubscribeMailEntity> ReadFileCisSubscribeMail(string filePath, string fiPrefix, ref int numOfSubMail, ref string fiSubMail, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisAddressTypeEntity> ReadFileCisAddressType(string filePath, string fiPrefix, ref int numOfAddtype, ref string fiAddtype, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisSubscriptionTypeEntity> ReadFileCisSubscriptionType(string filePath, string fiPrefix, ref int numOfSubType, ref string fiSubType, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisCustomerPhoneEntity> ReadFileCisCustomerPhone(string filePath, string fiPrefix, ref int numOfCusPhone, ref string fiCusPhone, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisCustomerEmailEntity> ReadFileCisCustomerEmail(string filePath, string fiPrefix, ref int numOfCusEmail, ref string fiCusEmail, ref bool isValidHeader, ref string msgValidateFileError);
        List<CisCountryEntity> ReadFileCisCountry(string filePath, string fiPrefix, ref int numOfCountry, ref string fiCountry, ref bool isValidHeader, ref string msgValidateFileError);
        bool SaveCisCorporate(List<CisCorporateEntity> cisCorporates, string fiCor);
        bool SaveCisCorporateComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisIndividual(List<CisIndividualEntity> cisIndividuals, string fiIndiv);
        bool SaveCisIndividualComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisProductGroup(List<CisProductGroupEntity> cisProductGroup, string fiProd);
        bool SaveCisSubscription(List<CisSubscriptionEntity> cisSubscription, string fiSub);
        bool SaveCisSubscriptionComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisTitle(List<CisTitleEntity> cisTitles,string fiTitle);
        bool SaveCisProvince(List<CisProvinceEntity> cisProvinces,string fiProvince);
        bool SaveCisDistrict(List<CisDistrictEntity> cisDistricts,string fiDistrict);
        bool SaveCisSubDistrict(List<CisSubDistrictEntity> cisSubDistricts,string fiSubDistrict);
        bool SaveCisPhoneType(List<CisPhoneTypeEntity> cisPhones, string fiPhoneType);
        bool SaveCisEmailType(List<CisEmailTypeEntity> cisEmails, string fiEmailType);
        bool SaveCisSubscribeAddress(List<CisSubscribeAddressEntity> cisSubscribeAdds, string fiSubAdds);
        bool SaveCisSubscribePhone(List<CisSubscribePhoneEntity> cisSubscribePhones, string fiSubPhone);
        bool SaveCisSubscribeEmail(List<CisSubscribeMailEntity> cisSubscribeMails, string fiSubEmail);
        bool SaveCisSubscribeAddressComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisSubscribePhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisSubscribeEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisAddressType(List<CisAddressTypeEntity> cisAddresstypes, string fiAddtype);
        bool SaveCisSubscriptionType(List<CisSubscriptionTypeEntity> cisSubscriptionType, string fiSubType);
        bool SaveCisSubscriptionTypeComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCustomerPhone(List<CisCustomerPhoneEntity> cisCustomerPhone, string fiCusPhone);
        bool SaveCisCustomerPhoneComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCustomerEmail(List<CisCustomerEmailEntity> cisCustomerEmail, string fiCusEmail);
        bool SaveCisCustomerEmailComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        bool SaveCisCountry(List<CisCountryEntity> cisCountry, string fiCountry);
        void SaveLogSuccess(ImportCISTaskResponse taskResponse);
        void SaveLogError(ImportCISTaskResponse taskResponse);
        bool ExportIndividualCIS(string filePath, string fileName);
        bool ExportCorporateCIS(string filePath, string fileName);
        bool ExportSubscriptionCIS(string filePath, string fileName);
        bool ExportSubscribeAddressCIS(string filePath, string fileName);
        bool ExportSubscribePhoneCIS(string filePath, string fileName);
        bool ExportSubscribeEmailCIS(string filePath, string fileName);
        bool ExportSubscriptionTypeCIS(string filePath, string fileName);
        bool ExportCustomerPhoneCIS(string filePath, string fileName);
        bool ExportCustomerEmailCIS(string filePath, string fileName);
        void DeleteAllCisTableInterface();
    }
}
