using System;
using System.Collections.Generic;
using CSM.Entity;
using CSM.Service.Messages.Master;

namespace CSM.Business
{
    public interface IProductFacade : IDisposable
    {
        IEnumerable<ProductSREntity> SearchProducts(ProductSearchFilter searchFilter);
        ProductSREntity GetProduct(ProductSearchFilter searchFilter);
        bool IsDuplicateSRStatus(ProductSREntity product);
        bool SaveSRStatus(ProductSREntity productEntity);
        bool DeleteSRStatus(ProductSearchFilter searchFilter);
        List<ProductGroupEntity> GetProductGroupByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId);
        int GetProductGroupCountByName(string searchTerm, int pageSize, int pageNum, int? productId, int? campaignId);
        List<ProductEntity> GetProductByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId);
        int GetProductCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? campaignId);
        List<CampaignServiceEntity> GetCampaignServiceByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId);
        int GetCampaignServiceCountByName(string searchTerm, int pageSize, int pageNum, int? productGroupId, int? productId);
        List<TypeEntity> GetTypeByName(string searchTerm, int pageSize, int pageNum);
        int GetTypeCountByName(string searchTerm, int pageSize, int pageNum);
        List<AreaEntity> GetAreaByName(string searchTerm, int pageSize, int pageNum, int? subAreaId);
        int GetAreaCountByName(string searchTerm, int pageSize, int pageNum, int? subAreaId);
        List<SubAreaEntity> GetSubAreaByName(string searchTerm, int pageSize, int pageNum, int? areaId);
        int GetSubAreaCountByName(string searchTerm, int pageSize, int pageNum, int? areaId);
        CreateProductMasterResponse CreateProductMaster(CreateProductMasterRequest request);

        List<ProductEntity> GetProductList(int? productGroupId = null);
    }
}
