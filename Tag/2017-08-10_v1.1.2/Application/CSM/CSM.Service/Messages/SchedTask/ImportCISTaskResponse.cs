using System;
using System.Collections.Generic;
using System.Text;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.SchedTask
{
    public class ImportCISTaskResponse
    {
        public DateTime SchedDateTime { get; set; }
        public long ElapsedTime { get; set; }
        public List<object> FileList { get; set; }

        #region "comment out"

        //public int numOfCor { get; set; }
        //public int numOfIndiv { get; set; }
        //public int numOfProd { get; set; }
        //public int numOfSub { get; set; }
        //public int numOfTitle { get; set; }
        //public int numOfPro { get; set; }
        //public int numOfDis { get; set; }
        //public int numOfSubDis { get; set; }
        //public int numOfPhonetype { get; set; }
        //public int numOfEmailtype { get; set; }
        //public int numOfSubAdd { get; set; }
        //public int numOfSubPhone { get; set; }
        //public int numOfSubMail { get; set; }
        //public int numOfAddresstype { get; set; }
        //public int numOfSubtype { get; set; }
        //public int numOfCusphone { get; set; }
        //public int numOfCusEmail { get; set; }
        //public int numOfErrCor { get; set; }
        //public int numOfErrIndiv { get; set; }
        //public int numOfErrSub { get; set; }

        #endregion

        // Corperate
        public int NumOfCor { get; set; }
        public int NumOfErrCor { get; set; }
        public int NumOfCorComplete { get; set; }

        // Individual
        public int NumOfIndiv { get; set; }
        public int NumOfErrIndiv { get; set; }
        public int NumOfIndivComplete { get; set; }

        // Subscription
        public int NumOfSub { get; set; }
        public int NumOfSubComplete { get; set; }
        public int NumOfErrSub { get; set; }

        // SubscribeAddress
        public int NumOfSubAdd { get; set; }
        public int NumOfAddressComplete { get; set; }
        public int numOfErrAddress { get; set; }

        // SubscribePhone
        public int NumOfSubPhone { get; set; }
        public int NumOfPhoneComplete { get; set; }
        public int NumOfErrPhone { get; set; }

        // SubscribeMail
        public int NumOfSubMail { get; set; }
        public int NumOfEmailComplete { get; set; }
        public int NumOfErrEmail { get; set; }

        // SubscriptionType
        public int NumOfSubType { get; set; }
        public int NumOfSubTypeComplete { get; set; }
        public int NumOfSubTypeError { get; set; }

        // CusPhone
        public int NumOfCusPhone { get; set; }
        public int NumOfCusPhoneComplete { get; set; }
        public int NumOfCusPhoneError { get; set; }

        // CusEmail
        public int NumOfCusEmail { get; set; }
        public int NumOfCusEmailComplete { get; set; }
        public int NumOfCusEmailError { get; set; }

        // ProductGroup
        public int NumOfProd { get; set; }
        public int NumOfProdComplete { get; set; }
        public int NumOfProdError { get; set; }

        // Title
        public int NumOfTitle { get; set; }
        public int NumOfTitleComplete { get; set; }
        public int NumOfTitleError { get; set; }

        // Country
        public int NumOfCountry { get; set; }
        public int NumOfCountryComplete { get; set; }
        public int NumOfCountryError { get; set; }

        // Province
        public int NumOfPro { get; set; }
        public int NumOfProvinceComplete { get; set; }
        public int NumOfProvinceError { get; set; }

        // District
        public int NumOfDis { get; set; }
        public int NumOfDistrictComplete { get; set; }
        public int NumOfDistrictError { get; set; }

        // SubDistrict
        public int NumOfSubDis { get; set; }
        public int NumOfSubDistrictComplete { get; set; }
        public int NumOfSubDistrictError { get; set; }

        // AddressType
        public int NumOfAddressType { get; set; }
        public int NumOfAddressTypeComplete { get; set; }
        public int NumOfAddressTypeError { get; set; }

        // PhoneType
        public int NumOfPhonetype { get; set; }
        public int NumOfPhonetypeComplete { get; set; }
        public int NumOfPhonetypeError { get; set; }

        // Emailtype
        public int NumOfEmailtype { get; set; }
        public int NumOfEmailtypeComplete { get; set; }
        public int NumOfEmailtypeError { get; set; }
        public List<object> FileErrorList { get; set; }
        public StatusResponse StatusResponse { get; set; }

        public override string ToString()
        {
            #region "comment out"

            //StringBuilder sb = new StringBuilder("");
            //sb.Append(string.Format("Reading files = {0}\n", StringHelpers.ConvertListToString(FileList, "/")));
            //sb.Append(string.Format("Error files = {0}\n", FileErrorList.Count > 0 ? StringHelpers.ConvertListToString(FileErrorList, "/") : "0"));
            //sb.Append(string.Format("Total CIS Corporate = {0} records\n", NumOfCor));
            //sb.Append(string.Format("Total CIS Individual = {0} records\n", NumOfIndiv));
            //sb.Append(string.Format("Total CIS ProductGroup = {0} records\n", NumOfProd));
            //sb.Append(string.Format("Total CIS Subscription = {0} records\n", NumOfSub));
            //sb.Append(string.Format("Total CIS Title = {0} records\n", NumOfTitle));
            //sb.Append(string.Format("Total CIS Country = {0} records\n", NumOfCountry));
            //sb.Append(string.Format("Total CIS Province = {0} records\n", NumOfPro));
            //sb.Append(string.Format("Total CIS District = {0} records\n", NumOfDis));
            //sb.Append(string.Format("Total CIS SubDistrict = {0} records\n", NumOfSubDis));
            //sb.Append(string.Format("Total CIS Phone Type = {0} records\n", NumOfPhonetype));
            //sb.Append(string.Format("Total CIS Email Type = {0} records\n", NumOfEmailtype));
            //sb.Append(string.Format("Total CIS Subscribe Address = {0} records\n", NumOfSubAdd));
            //sb.Append(string.Format("Total CIS Subscribe Phone = {0} records\n", NumOfSubPhone));
            //sb.Append(string.Format("Total CIS Subscribe Mail = {0} records\n", NumOfSubMail));
            //sb.Append(string.Format("Total CIS Address Type = {0} records\n", NumOfAddressType));
            //sb.Append(string.Format("Total CIS Subscript Type = {0} records\n", NumOfSubType));
            //sb.Append(string.Format("Total CIS Customer Phone = {0} records\n", NumOfCusPhone));
            //sb.Append(string.Format("Total CIS Customer Email = {0} records\n", NumOfCusEmail));
            //sb.Append(string.Format("Total Corporate error records = {0} records\n", NumOfErrCor));
            //sb.Append(string.Format("Total Individual error records = {0} records\n", NumOfErrIndiv));
            //sb.Append(string.Format("Total Subscription error records = {0} records\n", NumOfErrSub));

            #endregion

            var fileList = FileList;
            var numOfTotalList = new List<object> { NumOfTitle, NumOfCor, NumOfIndiv, NumOfProd, NumOfSub, NumOfCountry, NumOfPro, NumOfDis, NumOfSubDis, NumOfPhonetype, NumOfEmailtype, NumOfAddressType, NumOfSubAdd, NumOfSubPhone, NumOfSubMail, NumOfSubType, NumOfCusPhone, NumOfCusEmail };
            var numOfCompleteList = new List<object> { NumOfTitleComplete, NumOfCorComplete, NumOfIndivComplete, NumOfProdComplete, NumOfSubComplete, NumOfCountryComplete, NumOfProvinceComplete, NumOfDistrictComplete, NumOfSubDistrictComplete, NumOfPhonetypeComplete, NumOfEmailtypeComplete, NumOfAddressTypeComplete, NumOfAddressComplete, NumOfPhoneComplete, NumOfEmailComplete, NumOfSubTypeComplete, NumOfCusPhoneComplete, NumOfCusEmailComplete };
            var numOfErrorList = new List<object> { NumOfTitleError, NumOfErrCor, NumOfErrIndiv, NumOfProdError, NumOfErrSub, NumOfCountryError, NumOfProvinceError, NumOfDistrictError, NumOfSubDistrictError, NumOfPhonetypeError, NumOfEmailtypeError, NumOfAddressTypeError, numOfErrAddress, NumOfErrPhone, NumOfErrEmail, NumOfSubTypeError, NumOfCusPhoneError, NumOfCusEmailError };

            var sb = new StringBuilder();
            sb.Append("Reading files : Total file/Import data/Error data \n");

            if (fileList != null && fileList.Count > 0)
            {
                for (int i = 0; i <= fileList.Count - 1; i++)
                {

                    if (!string.IsNullOrWhiteSpace(FileList[i].ConvertToString()))
                    {
                        sb.AppendFormat("{0} : {1}/{2}/{3} \n", FileList[i], numOfTotalList[i], numOfCompleteList[i], numOfErrorList[i]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
