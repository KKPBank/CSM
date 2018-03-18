using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CSM.Common.Utilities;
using CSM.Service.Messages.Common;

namespace CSM.Service.Messages.Master
{
    [DataContract]
    public class CreateProductMasterRequest : IValidatableObject
    {
        [DataMember(IsRequired = true, Order = 1)]
        public Header Header { get; set; }

        [DataMember(Order = 2)]
        public ProductGroupDataItem ProductGroup { get; set; }

        [DataMember(Order = 3)]
        public ProductDataItem Product { get; set; }

        [DataMember(Order = 4)]
        public CampaignDataItem Campaign { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var status = new string[2]
            {
                Constants.ApplicationStatus.Active.ConvertToString(), 
                Constants.ApplicationStatus.Inactive.ConvertToString()
            };

            if (this.ProductGroup == null)
            {
                results.Add(new ValidationResult("ProductGroup object is required."));
            }

            //if (this.Product == null)
            //{
            //    results.Add(new ValidationResult("Product object is required."));
            //}

            //if (this.Campaign == null)
            //{
            //    results.Add(new ValidationResult("Campaign object is required."));
            //}

            #region "Product Group"

            if (this.ProductGroup != null && !string.IsNullOrWhiteSpace(this.ProductGroup.Code) && this.ProductGroup.Code.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "ProductGroup.Code", 50)));
            }

            if (this.ProductGroup != null && !string.IsNullOrWhiteSpace(this.ProductGroup.Name) && this.ProductGroup.Name.Length > 255)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "ProductGroup.Name", 255)));
            }

            if (this.ProductGroup != null && string.IsNullOrWhiteSpace(this.ProductGroup.Status))
            {
                results.Add(new ValidationResult("ProductGroup.Status is required."));
            }

            if (this.ProductGroup != null && !string.IsNullOrWhiteSpace(this.ProductGroup.Status) && !status.Contains(this.ProductGroup.Status))
            {
                results.Add(new ValidationResult(string.Format("{0} accept only 0 or 1.", "ProductGroup.Status")));
            }

            if (this.ProductGroup != null && !string.IsNullOrWhiteSpace(this.ProductGroup.CreateUser) && this.ProductGroup.CreateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "ProductGroup.CreateUser", 50)));
            }

            if (this.ProductGroup != null && this.ProductGroup.CreateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "ProductGroup.CreateDate")));
            }

            if (this.ProductGroup != null && !string.IsNullOrWhiteSpace(this.ProductGroup.UpdateUser) && this.ProductGroup.UpdateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "ProductGroup.UpdateUser", 50)));
            }

            if (this.ProductGroup != null && this.ProductGroup.UpdateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "ProductGroup.UpdateDate")));
            }

            #endregion

            #region "Product"

            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.Code) && this.Product.Code.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Product.Code", 50)));
            }

            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.Name) && this.Product.Name.Length > 255)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Product.Name", 255)));
            }

            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.ProductType) && this.Product.ProductType.Length > 1)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Product.ProductType", 1)));
            }

            if (this.Product != null && string.IsNullOrWhiteSpace(this.Product.Status))
            {
                results.Add(new ValidationResult("Product.Status is required."));
            }

            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.Status) && !status.Contains(this.Product.Status))
            {
                results.Add(new ValidationResult(string.Format("{0} accept only 0 or 1.", "Product.Status")));
            }

            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.CreateUser) && this.Product.CreateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Product.CreateUser", 50)));
            }

            if (this.Product != null && this.Product.CreateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "Product.CreateDate")));
            }
            
            if (this.Product != null && !string.IsNullOrWhiteSpace(this.Product.UpdateUser) && this.Product.UpdateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Product.UpdateUser", 50)));
            }

            if (this.Product != null && this.Product.UpdateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "Product.UpdateDate")));
            }

            #endregion

            #region "Campaign"

            if (this.Campaign != null && !string.IsNullOrWhiteSpace(this.Campaign.Code) && this.Campaign.Code.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "CampaignCode", 50)));
            }

            if (this.Campaign != null && !string.IsNullOrWhiteSpace(this.Campaign.Name) && this.Campaign.Name.Length > 255)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Campaign.Name", 255)));
            }

            if (this.Campaign != null && string.IsNullOrWhiteSpace(this.Campaign.Status))
            {
                results.Add(new ValidationResult("Campaign.Status is required."));
            }

            if (this.Campaign != null && !string.IsNullOrWhiteSpace(this.Campaign.Status) && !status.Contains(this.Campaign.Status))
            {
                results.Add(new ValidationResult(string.Format("{0} accept only 0 or 1.", "Campaign.Status")));
            }

            if (this.Campaign != null && !string.IsNullOrWhiteSpace(this.Campaign.CreateUser) && this.Campaign.CreateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Campaign.CreateUser", 50)));
            }

            if (this.Campaign != null && this.Campaign.CreateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "Campaign.CreateDate")));
            }

            if (this.Campaign != null && !string.IsNullOrWhiteSpace(this.Campaign.UpdateUser) && this.Campaign.UpdateUser.Length > 50)
            {
                results.Add(new ValidationResult(string.Format("{0} cannot exceed {1} characters.", "Campaign.UpdateUser", 50)));
            }

            if (this.Campaign != null && this.Campaign.UpdateDateValue == null)
            {
                results.Add(new ValidationResult(string.Format("{0} is invalid date format.", "Campaign.UpdateDate")));
            }

            #endregion

            return results;
        }
    }

    public class ProductGroupDataItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }

        [IgnoreDataMember]
        public DateTime? CreateDateValue
        {
            get { return this.CreateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }

        [IgnoreDataMember]
        public DateTime? UpdateDateValue
        {
            get { return this.UpdateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
    }

    public class ProductDataItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }

        [IgnoreDataMember]
        public DateTime? CreateDateValue
        {
            get { return this.CreateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }

        [IgnoreDataMember]
        public DateTime? UpdateDateValue
        {
            get { return this.UpdateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
    }

    public class CampaignDataItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }

        [IgnoreDataMember]
        public DateTime? CreateDateValue
        {
            get { return this.CreateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }

        [IgnoreDataMember]
        public DateTime? UpdateDateValue
        {
            get { return this.UpdateDate.ParseDateTime(Constants.DateTimeFormat.ReportDateTime); }
        }
    }
}
