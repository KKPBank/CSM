using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IAFSDataAccess
    {
        bool SaveAFSProperty(List<PropertyEntity> properties);
        bool SaveSaleZone(List<SaleZoneEntity> saleZones);
        bool SaveAFSAsset(List<AfsAssetEntity> assetList, ref int numOfComplete);
        int? GetUserIdByEmployeeCode(string employeeCode);
        List<AfsAssetEntity> GetCompleteProperties();
        List<PropertyEntity> GetErrorProperties();
        List<SaleZoneEntity> GetErrorSaleZones();
        List<SaleZoneEntity> GetSaleZones();
        List<AfsexportEntity> GetAFSExport();
        bool UpdateAFSExportWithExportDate();
        List<AfsMarketingEntity> GetAFSMarketingNew();
        List<AfsMarketingEntity> GetAFSMarketingUpdate();
    }
}
