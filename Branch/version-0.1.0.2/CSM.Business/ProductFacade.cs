using System.ComponentModel.DataAnnotations;
using System.Linq;
using CSM.Business.Common;
using CSM.Common.Utilities;
using CSM.Data.DataAccess;
using CSM.Entity;
using CSM.Service.Messages.Common;
using CSM.Service.Messages.Master;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CSM.Business
{
    public class ProductFacade : BaseFacade, IProductFacade
    {
        const string Delimeter = "\n";
        private ICommonFacade _commonFacade;
        private readonly CSMContext _context;
        private IProductDataAccess _productDataAccess;
        private LogMessageBuilder _logMsg = new LogMessageBuilder();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductFacade));

        public ProductFacade()
        {
            _context = new CSMContext();
        }

        public IEnumerable<ProductSREntity> SearchProducts(ProductSearchFilter searchFilter)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.SearchProducts(searchFilter);
        }

        public ProductSREntity GetProduct(ProductSearchFilter searchFilter)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProduct(searchFilter);
        }
        public bool IsDuplicateSRStatus(ProductSREntity product)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.IsDuplicateSRStatus(product);
        }

        public bool SaveSRStatus(ProductSREntity productEntity)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.SaveSRStatus(productEntity);
        }

        public bool DeleteSRStatus(ProductSearchFilter searchFilter)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.DeleteSRStatus(searchFilter);
        }

        public List<ProductGroupEntity> GetProductGroupByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProductGroupByName(searchTerm, pageSize, pageNum, productId, campaignId);
        }

        public int GetProductGroupCountByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProductGroupCountByName(searchTerm, pageSize, pageNum, productId, campaignId);
        }

        public List<ProductEntity> GetProductByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProductByName(searchTerm, pageSize, pageNum, productGroupId, campaignId);
        }

        public int GetProductCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProductCountByName(searchTerm, pageSize, pageNum, productGroupId, campaignId);
        }

        public List<CampaignServiceEntity> GetCampaignServiceByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetCampaignServiceByName(searchTerm, pageSize, pageNum, productGroupId, productId);
        }

        public int GetCampaignServiceCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetCampaignServiceCountByName(searchTerm, pageSize, pageNum, productGroupId, productId);
        }

        public List<TypeEntity> GetTypeByName(string searchTerm, int pageSize, int pageNum)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetTypeByName(searchTerm, pageSize, pageNum);
        }

        public int GetTypeCountByName(string searchTerm, int pageSize, int pageNum)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetTypeCountByName(searchTerm, pageSize, pageNum);
        }

        public List<AreaEntity> GetAreaByName(string searchTerm, int pageSize, int pageNum, int? subAreaId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetAreaByName(searchTerm, pageSize, pageNum, subAreaId);
        }
        public int GetAreaCountByName(string searchTerm, int pageSize, int pageNum, int? subAreaId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetAreaCountByName(searchTerm, pageSize, pageNum, subAreaId);
        }

        public List<SubAreaEntity> GetSubAreaByName(string searchTerm, int pageSize, int pageNum, int? areaId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetSubAreaByName(searchTerm, pageSize, pageNum, areaId);
        }

        public int GetSubAreaCountByName(string searchTerm, int pageSize, int pageNum, int? areaId)
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetSubAreaCountByName(searchTerm, pageSize, pageNum, areaId);
        }

        public CreateProductMasterResponse CreateProductMaster(CreateProductMasterRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Logger.Info(_logMsg.Clear().SetPrefixMsg("Call MasterService.CreateProductMaster").ToInputLogString());
            Logger.Debug("I:--START--:--MasterService.CreateProductMaster--");

            bool valid = false;
            _commonFacade = new CommonFacade();
            CreateProductMasterResponse response = new CreateProductMasterResponse();

            if (request.Header != null)
            {
                valid = _commonFacade.VerifyServiceRequest<Header>(request.Header);
                response.Header = new Header
                {
                    reference_no = request.Header.reference_no,
                    service_name = request.Header.service_name,
                    system_code = request.Header.system_code,
                    transaction_date = request.Header.transaction_date
                };
            }

            Logger.DebugFormat("-- XMLRequest --\n{0}", request.SerializeObject());

            var auditLog = new AuditLogEntity();
            auditLog.Module = Constants.Module.Customer;
            auditLog.Action = Constants.AuditAction.CreateProductMaster;
            auditLog.IpAddress = ApplicationHelpers.GetClientIP();

            #region "Call Validator"

            if (!valid)
            {
                response.StatusResponse = new StatusResponse
                {
                    ErrorCode = Constants.ErrorCode.CSM_PROD001,
                    Status = Constants.StatusResponse.Failed,
                    Description = "Bad Request, the header is not valid"
                };

                AppLog.AuditLog(auditLog, LogStatus.Fail, response.StatusResponse.Description);
                Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());

                stopwatch.Stop();
                Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);

                return response;
            }
            else
            {
                dynamic[] results = DoValidation(request);
                valid = (bool)results[0];
                List<ValidationResult> validationResults = (List<ValidationResult>)results[1];

                if (!valid)
                {
                    response.StatusResponse = new StatusResponse();
                    response.StatusResponse.ErrorCode = Constants.ErrorCode.CSM_PROD002;
                    response.StatusResponse.Status = Constants.StatusResponse.Failed;

                    if (validationResults != null && validationResults.Count > 0)
                    {
                        string msg = "Bad Request, the body is not valid:\n{0}";
                        response.StatusResponse.Description = string.Format(CultureInfo.InvariantCulture, msg,
                            validationResults.Select(x => x.ErrorMessage).Aggregate((i, j) => i + Delimeter + j));
                    }

                    AppLog.AuditLog(auditLog, LogStatus.Fail, response.StatusResponse.Description);
                    Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());

                    stopwatch.Stop();
                    Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);

                    return response;
                }
            }

            #endregion

            ProductEntity product = null;
            CampaignServiceEntity campaign = null;
            ProductGroupEntity productGroup = null;

            if (!string.IsNullOrWhiteSpace(request.ProductGroup.Code) &&
                !string.IsNullOrWhiteSpace(request.ProductGroup.Name))
            {
                productGroup = new ProductGroupEntity
                {
                    ProductGroupCode = request.ProductGroup.Code,
                    ProductGroupName = request.ProductGroup.Name,
                    IsActive = Constants.ApplicationStatus.Active.ConvertToString().Equals(request.ProductGroup.Status) ? true : false,
                    CreateUser = request.ProductGroup.CreateUser,
                    CreateDate = request.ProductGroup.CreateDateValue,
                    UpdateUser = request.ProductGroup.UpdateUser,
                    UpdateDate = request.ProductGroup.UpdateDateValue
                };
            }

            if (request.Product != null && !string.IsNullOrWhiteSpace(request.Product.Code)
                && !string.IsNullOrWhiteSpace(request.Product.Name))
            {
                product = new ProductEntity
                {
                    ProductCode = request.Product.Code,
                    ProductName = request.Product.Name,
                    ProductType = request.Product.ProductType,
                    IsActive = Constants.ApplicationStatus.Active.ConvertToString().Equals(request.Product.Status) ? true : false,
                    CreateUser = request.Product.CreateUser,
                    CreateDate = request.Product.CreateDateValue,
                    UpdateUser = request.Product.UpdateUser,
                    UpdateDate = request.Product.UpdateDateValue
                };
            }

            if (request.Campaign != null && !string.IsNullOrWhiteSpace(request.Campaign.Code) &&
                !string.IsNullOrWhiteSpace(request.Campaign.Name))
            {
                campaign = new CampaignServiceEntity
                {
                    CampaignServiceCode = request.Campaign.Code,
                    CampaignServiceName = request.Campaign.Name,
                    IsActive = Constants.ApplicationStatus.Active.ConvertToString().Equals(request.Campaign.Status) ? true : false,
                    CreateUser = request.Campaign.CreateUser,
                    CreateDate = request.Campaign.CreateDateValue,
                    UpdateUser = request.Campaign.UpdateUser,
                    UpdateDate = request.Campaign.UpdateDateValue
                };
            }

            if (productGroup != null)
            {
                _productDataAccess = new ProductDataAccess(_context);
                bool success = _productDataAccess.SaveProductMaster(productGroup, product, campaign);

                if (success)
                {
                    response.StatusResponse = new StatusResponse
                    {
                        ErrorCode = string.Empty,
                        Status = Constants.StatusResponse.Success,
                        Description = "Create successful data"
                    };

                    AppLog.AuditLog(auditLog, LogStatus.Success, response.StatusResponse.Description);

                    Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
                    stopwatch.Stop();
                    Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);

                    return response;
                }
            }

            response.StatusResponse = new StatusResponse
            {
                ErrorCode = Constants.ErrorCode.CSM_PROD003,
                Status = Constants.StatusResponse.Failed,
                Description = "Fail to create data"
            };

            AppLog.AuditLog(auditLog, LogStatus.Fail, response.StatusResponse.Description);
            Logger.DebugFormat("-- XMLResponse --\n{0}", response.SerializeObject());
            stopwatch.Stop();
            Logger.DebugFormat("O:--Finish--:ElapsedMilliseconds/{0}", stopwatch.ElapsedMilliseconds);

            return response;
        }

        public List<ProductEntity> GetProductList()
        {
            _productDataAccess = new ProductDataAccess(_context);
            return _productDataAccess.GetProductList();
        }

        #region "IDisposable"

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_context != null) { _context.Dispose(); }
                    if (_commonFacade != null) { _commonFacade.Dispose(); }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
