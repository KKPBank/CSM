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
        List<PropertyEntity> GetProperties();
        List<AfsAssetEntity> GetCompleteProperties();
        List<PropertyEntity> GetErrorProperties();
        List<SaleZoneEntity> GetErrorSaleZones();
        List<SaleZoneEntity> GetSaleZones();
        List<AfsexportEntity> GetAFSExport();
        bool UpdateAFSExportWithExportDate();
        bool UpdateAFSExportWithExportDate(List<int> sr_id_list);
        List<AfsMarketingEntity> GetAFSMarketing();
        List<AfsMarketingEntity> GetAFSMarketingNew();
        List<AfsMarketingEntity> GetAFSMarketingUpdate();
        bool SaveExportDate(bool isUpdate);
        bool UpdateExportDate(List<int> userIdList);
    }
}
