using System;
using System.Collections.Generic;
using System.Data;
using CSM.Common.Utilities;
using CSM.Entity;
using log4net;
using System.Linq;
using System.Data.Entity.Core.Objects;

namespace CSM.Data.DataAccess
{
    public class AFSDataAccess : IAFSDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AFSDataAccess));

        public AFSDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public bool SaveAFSProperty(List<PropertyEntity> properties)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    //ValidationContext vc = null;
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (properties != null && properties.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_PROPERTY");
                            this.Save();

                            foreach (PropertyEntity prop in properties)
                            {
                                TB_I_PROPERTY dbProp = new TB_I_PROPERTY();
                                dbProp.ROW_ID = prop.RowId;
                                dbProp.IF_ROW_STAT = prop.IfRowStat;
                                dbProp.IF_ROW_BATCH_NUM = prop.IfRowBatchNum;
                                dbProp.AST_ASSET_NUM = prop.AssetNum;
                                dbProp.AST_TYPE_CD = prop.AssetType;
                                dbProp.AST_TRADEINTYPE_CD = prop.AssetTradeInType;
                                dbProp.AST_STATUS_CD = prop.AssetStatus;
                                dbProp.AST_DESC_TEXT = prop.AssetDesc;
                                dbProp.AST_NAME = prop.AssetName;
                                dbProp.AST_COMMENTS = prop.AssetComments;
                                dbProp.AST_REF_NUMBER_1 = prop.AssetRefNo1;
                                dbProp.AST_LOT_NUM = prop.AssetLot;
                                dbProp.AST_PURCH_LOC_DESC = prop.AssetPurch;
                                dbProp.EMPLOYEE_ID = prop.EmployeeId;
                                dbProp.SALE_NAME = prop.SaleName;
                                dbProp.MOBILE_NO = prop.MobileNo;
                                dbProp.PHONE_NO = prop.PhoneNo;
                                dbProp.EMAIL = prop.Email;

                                //vc = new ValidationContext(prop, null, null);
                                //var validationResults = new List<ValidationResult>();
                                //bool valid = Validator.TryValidateObject(prop, vc, validationResults, true);
                                //if (!valid)
                                //{
                                //    dbProp.ERROR =
                                //        validationResults.Select(x => x.ErrorMessage)
                                //            .Aggregate((i, j) => i + Environment.NewLine + j);
                                //}

                                if (prop.EmployeeId == null)
                                {
                                    dbProp.ERROR = "No matching employee";
                                }

                                _context.TB_I_PROPERTY.Add(dbProp);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public bool SaveSaleZone(List<SaleZoneEntity> saleZones)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    //ValidationContext vc = null;
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (saleZones != null && saleZones.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_SALE_ZONE");
                            this.Save();

                            foreach (SaleZoneEntity saleZone in saleZones)
                            {
                                TB_I_SALE_ZONE dbSaleZone = new TB_I_SALE_ZONE();
                                dbSaleZone.AMPHUR = saleZone.District;
                                dbSaleZone.PROVINCE = saleZone.Province;
                                dbSaleZone.SALE_NAME = saleZone.SaleName;
                                dbSaleZone.PHONE_NO = saleZone.PhoneNo;
                                dbSaleZone.EMPLOYEE_NO = saleZone.EmployeeNo;
                                dbSaleZone.MOBILE_NO = saleZone.MobileNo;
                                dbSaleZone.EMAIL = saleZone.Email;
                                dbSaleZone.EMPLOYEE_ID = saleZone.EmployeeId;

                                //vc = new ValidationContext(saleZone, null, null);
                                //var validationResults = new List<ValidationResult>();
                                //bool valid = Validator.TryValidateObject(saleZone, vc, validationResults, true);
                                //if (!valid)
                                //{
                                //    dbSaleZone.ERROR =
                                //        validationResults.Select(x => x.ErrorMessage)
                                //            .Aggregate((i, j) => i + Environment.NewLine + j);
                                //}

                                if (saleZone.EmployeeId == null)
                                {
                                    dbSaleZone.ERROR = "No matching employee";
                                }

                                _context.TB_I_SALE_ZONE.Add(dbSaleZone);
                            }

                            this.Save();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public int? GetUserIdByEmployeeCode(string employeeCode)
        {
            var query = _context.TB_R_USER.Where(x => x.EMPLOYEE_CODE == employeeCode);
            return query.Any() ? query.First().USER_ID : new Nullable<int>();
        }

        public bool SaveAFSAsset(List<AfsAssetEntity> assetList, ref int numOfComplete)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (assetList != null && assetList.Count > 0)
                        {
                            foreach (AfsAssetEntity assetEntity in assetList)
                            {
                                var afsAsset =
                                    _context.TB_M_AFS_ASSET.FirstOrDefault(x => x.ASSET_NO == assetEntity.AssetNo);

                                if (afsAsset != null)
                                {
                                    //Update                             
                                    //afsAsset.EMPLOYEE_ID = assetEntity.EmployeeId;
                                    afsAsset.SALE_USER_ID = assetEntity.EmployeeId;
                                    afsAsset.ASSET_TYPE = assetEntity.AssetType;
                                    afsAsset.STATUS = assetEntity.Status;
                                    afsAsset.STATUS_DESC = assetEntity.StatusDesc;
                                    afsAsset.PROJECT_NAME = assetEntity.ProjectName;
                                    afsAsset.PROJECT_DES = assetEntity.ProjectDes;
                                    afsAsset.AFS_REF_NO = assetEntity.AfsRefNo;
                                    afsAsset.AMPHUR = assetEntity.Amphur;
                                    afsAsset.PROVINCE = assetEntity.Province;
                                    afsAsset.SALE_NAME = assetEntity.SaleName;
                                    afsAsset.PHONE_NO = assetEntity.PhoneNo;
                                    afsAsset.MOBILE_NO = assetEntity.MobileNo;
                                    afsAsset.EMAIL = assetEntity.Email;
                                    SetEntryStateModified(afsAsset);
                                }
                                else
                                {
                                    //Save New
                                    afsAsset = new TB_M_AFS_ASSET();
                                    //afsAsset.EMPLOYEE_ID = assetEntity.EmployeeId;
                                    afsAsset.SALE_USER_ID = assetEntity.EmployeeId;
                                    afsAsset.ASSET_NO = assetEntity.AssetNo;
                                    afsAsset.ASSET_TYPE = assetEntity.AssetType;
                                    afsAsset.STATUS = assetEntity.Status;
                                    afsAsset.STATUS_DESC = assetEntity.StatusDesc;
                                    afsAsset.PROJECT_NAME = assetEntity.ProjectName;
                                    afsAsset.PROJECT_DES = assetEntity.ProjectDes;
                                    afsAsset.AFS_REF_NO = assetEntity.AfsRefNo;
                                    afsAsset.AMPHUR = assetEntity.Amphur;
                                    afsAsset.PROVINCE = assetEntity.Province;
                                    afsAsset.SALE_NAME = assetEntity.SaleName;
                                    afsAsset.PHONE_NO = assetEntity.PhoneNo;
                                    afsAsset.MOBILE_NO = assetEntity.MobileNo;
                                    afsAsset.EMAIL = assetEntity.Email;
                                    _context.TB_M_AFS_ASSET.Add(afsAsset);
                                }
                            }

                            this.Save();

                            //if (this.Save() > 0)
                            //{
                            //    numOfComplete += 1;
                            //}                       

                        }

                        transaction.Commit();

                        #region "Count of Complete"

                        numOfComplete = _context.TB_I_PROPERTY.Count(x => x.ERROR == null);

                        #endregion

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public List<PropertyEntity> GetProperties()
        {
            var query = from pp in _context.TB_I_PROPERTY
                        select new PropertyEntity
                        {
                            EmployeeId = pp.EMPLOYEE_ID,
                            RowId = pp.ROW_ID,
                            IfRowStat = pp.IF_ROW_STAT,
                            IfRowBatchNum = pp.IF_ROW_BATCH_NUM,
                            AssetNum = pp.AST_ASSET_NUM,
                            AssetType = pp.AST_TYPE_CD,
                            AssetTradeInType = pp.AST_TRADEINTYPE_CD,
                            AssetStatus = pp.AST_STATUS_CD,
                            AssetDesc = pp.AST_DESC_TEXT,
                            AssetName = pp.AST_NAME,
                            AssetComments = pp.AST_COMMENTS,
                            AssetRefNo1 = pp.AST_REF_NUMBER_1,
                            AssetLot = pp.AST_LOT_NUM,
                            AssetPurch = pp.AST_PURCH_LOC_DESC,
                            SaleName = pp.SALE_NAME,
                            PhoneNo = pp.PHONE_NO,
                            MobileNo = pp.MOBILE_NO,
                            Email = pp.EMAIL,
                            Error = pp.ERROR
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<AfsAssetEntity> GetCompleteProperties()
        {
            var query = from pp in _context.TB_I_PROPERTY
                        where string.IsNullOrEmpty(pp.ERROR)
                        select new AfsAssetEntity
                        {
                            EmployeeId = pp.EMPLOYEE_ID,
                            AssetNo = pp.AST_ASSET_NUM,
                            AssetType = pp.AST_TYPE_CD,
                            Status = pp.AST_STATUS_CD,
                            StatusDesc = pp.AST_DESC_TEXT,
                            ProjectName = pp.AST_TRADEINTYPE_CD,
                            ProjectDes = pp.AST_TRADEINTYPE_CD, // TODO: Check fields mapping
                            AfsRefNo = pp.AST_REF_NUMBER_1,
                            Amphur = pp.AST_LOT_NUM,
                            Province = pp.AST_PURCH_LOC_DESC,
                            SaleName = pp.SALE_NAME,
                            PhoneNo = pp.PHONE_NO,
                            MobileNo = pp.MOBILE_NO,
                            Email = pp.EMAIL
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<PropertyEntity> GetErrorProperties()
        {
            var query = from pp in _context.TB_I_PROPERTY
                        where !string.IsNullOrEmpty(pp.ERROR)
                        select new PropertyEntity
                        {
                            EmployeeId = pp.EMPLOYEE_ID,
                            RowId = pp.ROW_ID,
                            IfRowStat = pp.IF_ROW_STAT,
                            IfRowBatchNum = pp.IF_ROW_BATCH_NUM,
                            AssetNum = pp.AST_ASSET_NUM,
                            AssetType = pp.AST_TYPE_CD,
                            AssetTradeInType = pp.AST_TRADEINTYPE_CD,
                            AssetStatus = pp.AST_STATUS_CD,
                            AssetDesc = pp.AST_DESC_TEXT,
                            AssetName = pp.AST_NAME,
                            AssetComments = pp.AST_COMMENTS,
                            AssetRefNo1 = pp.AST_REF_NUMBER_1,
                            AssetLot = pp.AST_LOT_NUM,
                            AssetPurch = pp.AST_PURCH_LOC_DESC,
                            SaleName = pp.SALE_NAME,
                            PhoneNo = pp.PHONE_NO,
                            MobileNo = pp.MOBILE_NO,
                            Email = pp.EMAIL,
                            Error = pp.ERROR
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<SaleZoneEntity> GetErrorSaleZones()
        {
            var query = from sz in _context.TB_I_SALE_ZONE
                        where !string.IsNullOrEmpty(sz.ERROR)
                        select new SaleZoneEntity
                        {
                            District = sz.AMPHUR,
                            Province = sz.PROVINCE,
                            SaleName = sz.SALE_NAME,
                            PhoneNo = sz.PHONE_NO,
                            EmployeeNo = sz.EMPLOYEE_NO,
                            MobileNo = sz.MOBILE_NO,
                            Email = sz.EMAIL,
                            Error = sz.ERROR
                        };

            return query.Any() ? query.ToList() : null;
        }

        public List<SaleZoneEntity> GetSaleZones()
        {
            var query = from sz in _context.TB_I_SALE_ZONE
                        select new SaleZoneEntity
                       {
                           District = sz.AMPHUR,
                           Province = sz.PROVINCE,
                           SaleName = sz.SALE_NAME,
                           PhoneNo = sz.PHONE_NO,
                           EmployeeNo = sz.EMPLOYEE_NO,
                           MobileNo = sz.MOBILE_NO,
                           Email = sz.EMAIL,
                           Error = sz.ERROR,
                           EmployeeId = sz.EMPLOYEE_ID
                       };

            return query.Any() ? query.ToList() : null;
        }

        #region Backup GetAFSExport 07/03/2017
        //Comment on 07/03/2017
        //public List<AfsexportEntity> GetAFSExport()
        //{
        //    DateTime minDate = DateTime.Now.Date;
        //    DateTime maxDate = minDate.AddDays(1);

        //    var query = (from a in _context.TB_T_SR_ACTIVITY.AsNoTracking()
        //                 join s in _context.TB_T_SR on a.SR_ID equals s.SR_ID
        //                 join u in _context.TB_R_USER on a.CREATE_USER equals u.USER_ID
        //                 join y in _context.TB_C_SR_ACTIVITY_TYPE on a.SR_ACTIVITY_TYPE_ID equals y.SR_ACTIVITY_TYPE_ID
        //                 join c in _context.TB_M_CONTACT on s.CONTACT_ID equals c.CONTACT_ID
        //                 where a.EXPORT_DATE == null
        //                 && s.SR_PAGE_ID == Constants.SRPage.AFSPageId
        //                 && (s.SR_STATUS_ID != Constants.SRStatusId.Draft && s.SR_STATUS_ID != Constants.SRStatusId.Cancelled)
        //                 select new AfsexportEntity
        //                 {
        //                     CreatedDate = a.CREATE_DATE,
        //                     CreatedBy = u.USERNAME,
        //                     Type = y.SR_ACTIVITY_TYPE_NAME,
        //                     Question = s.SR_SUBJECT,
        //                     Description = a.SR_ACTIVITY_DESC,
        //                     PhoneList = (from p in c.TB_M_CONTACT_PHONE
        //                                  where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
        //                                  select new PhoneEntity
        //                                  {
        //                                      PhoneTypeId = p.PHONE_TYPE_ID,
        //                                      PhoneNo = p.PHONE_NO
        //                                  }).ToList(),
        //                     ContactFirstName = c.FIRST_NAME_TH != null ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
        //                     ContactLastName = c.LAST_NAME_TH != null ? c.LAST_NAME_TH : c.LAST_NAME_EN,
        //                     Property = s.SR_AFS_ASSET_NO,
        //                     AssetInspection = "N",
        //                     CallBackRequest = "N",
        //                     DocumentRequest = "N",
        //                     LocationEnquiry = "N",
        //                     PriceEnquiry = "N",
        //                     PriceIssuedCallBack = "N",
        //                     CallBackRequired = "N",
        //                     MediaSource = "N"
        //                 });

        //    return query.ToList();
        //}
        #endregion

        public List<AfsexportEntity> GetAFSExport()
        {
            DateTime minDate = DateTime.Now.Date;
            DateTime maxDate = minDate.AddDays(1);

            var query = (from s in _context.TB_T_SR
                         join a in _context.TB_T_SR_ACTIVITY on s.SR_ID equals a.SR_ID
                         join u in _context.TB_R_USER on a.CREATE_USER equals u.USER_ID
                         join y in _context.TB_C_SR_ACTIVITY_TYPE on a.SR_ACTIVITY_TYPE_ID equals y.SR_ACTIVITY_TYPE_ID
                         join c in _context.TB_M_CONTACT on s.CONTACT_ID equals c.CONTACT_ID
                         where s.EXPORT_DATE == null
                         && a.SR_ACTIVITY_ID == (
                            _context.TB_T_SR_ACTIVITY.Where(x => x.SR_ID == s.SR_ID).Min(x => x.SR_ACTIVITY_ID)
                         )
                         && s.SR_PAGE_ID == Constants.SRPage.AFSPageId
                         && (s.SR_STATUS_ID != Constants.SRStatusId.Draft && s.SR_STATUS_ID != Constants.SRStatusId.Cancelled)
                         orderby a.CREATE_DATE
                         select new AfsexportEntity
                         {
                             SR_ID = s.SR_ID,
                             CreatedDate = a.CREATE_DATE,
                             CreatedBy = u.USERNAME,
                             Type = y.SR_ACTIVITY_TYPE_NAME,
                             Question = s.SR_SUBJECT,
                             Description = a.SR_ACTIVITY_DESC,
                             PhoneList = (from p in c.TB_M_CONTACT_PHONE
                                          where p.TB_M_PHONE_TYPE.PHONE_TYPE_CODE != Constants.PhoneTypeCode.Fax
                                          select new PhoneEntity
                                          {
                                              PhoneTypeId = p.PHONE_TYPE_ID,
                                              PhoneNo = p.PHONE_NO
                                          }).ToList(),
                             ContactFirstName = c.FIRST_NAME_TH != null ? c.FIRST_NAME_TH : c.FIRST_NAME_EN,
                             ContactLastName = c.LAST_NAME_TH != null ? c.LAST_NAME_TH : c.LAST_NAME_EN,
                             Property = s.SR_AFS_ASSET_NO,
                             AssetInspection = "N",
                             CallBackRequest = "N",
                             DocumentRequest = "N",
                             LocationEnquiry = "N",
                             PriceEnquiry = "N",
                             PriceIssuedCallBack = "N",
                             CallBackRequired = "N",
                             MediaSource = "N"
                         });

            return query.ToList();
        }

        public bool UpdateAFSExportWithExportDate()
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        DateTime today = DateTime.Now;

                        var lstAct =
                            (from a in _context.TB_T_SR_ACTIVITY
                             join s in _context.TB_T_SR on a.SR_ID equals s.SR_ID
                             where a.EXPORT_DATE == null
                                   && s.SR_PAGE_ID == Constants.SRPage.AFSPageId
                                   && (s.SR_STATUS_ID != Constants.SRStatusId.Draft && s.SR_STATUS_ID != Constants.SRStatusId.Cancelled)
                             select new
                             {
                                 a
                             });

                        if (lstAct.Any())
                        {
                            foreach (var act in lstAct)
                            {
                                act.a.EXPORT_DATE = today;
                                SetEntryStateModified(act.a);
                            }
                        }

                        this.Save();
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public bool UpdateAFSExportWithExportDate(List<int> sr_id_list)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        DateTime today = DateTime.Now;

                        var lstAct =
                            (from s in _context.TB_T_SR
                             where sr_id_list.Contains(s.SR_ID)
                             select new
                             {
                                 s
                             });

                        if (lstAct.Any())
                        {
                            foreach (var sr in lstAct)
                            {
                                sr.s.EXPORT_DATE = today;
                                SetEntryStateModified(sr.s);
                            }
                        }

                        this.Save();
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.Error("Exception occur:\n", ex);
                    }
                    finally
                    {
                        _context.Configuration.AutoDetectChangesEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public List<AfsMarketingEntity> GetAFSMarketing()
        {
            var query = (from u in _context.TB_R_USER.AsNoTracking()
                         from p in _context.TB_R_USER_PHONE.Where(x => x.USER_ID == u.USER_ID).DefaultIfEmpty()
                         where u.EXPORT_DATE == null && u.MARKETING_FLAG == true
                         orderby p.ORDER
                         select new AfsMarketingEntity
                         {
                             UserID = u.USER_ID,
                             EmpNum = u.EMPLOYEE_CODE,
                             FstName = u.FIRST_NAME,
                             LastName = u.LAST_NAME,
                             PhoneNo = p.PHONE_NO,
                             CreateDate = u.CREATE_DATE,
                             UpdateDate = u.UPDATE_DATE,
                             PhoneOrder = p.ORDER,
                             Status = u.STATUS,
                             IsNew = (System.Data.Entity.DbFunctions.TruncateTime(u.CREATE_DATE) == System.Data.Entity.DbFunctions.TruncateTime(u.UPDATE_DATE))
                         });

            return query.ToList();
        }

        public bool UpdateExportDate(List<int> userIdList)
        {
            bool ret = false;
            try
            {
                DateTime today = DateTime.Now;
                var users = _context.TB_R_USER.Where(p => userIdList.Contains(p.USER_ID)).ToList();
                foreach (var user in users)
                {
                    user.EXPORT_DATE = today;
                }
                _context.SaveChanges();
                ret = true;
            }
            catch(Exception ex)
            {
                ret = false;
                Logger.Error("Exception occur:\n", ex);
            }

            return ret;
        }

        public List<AfsMarketingEntity> GetAFSMarketingNew()
        {
            //DateTime minDate = DateTime.Now.Date;
            //DateTime maxDate = minDate.AddDays(1);

            var query = (from u in _context.TB_R_USER.AsNoTracking()
                         from p in _context.TB_R_USER_PHONE.Where(x => x.USER_ID == u.USER_ID).DefaultIfEmpty()
                         //where (p.CREATE_DATE >= minDate) && (p.CREATE_DATE <= maxDate)
                         //&& (u.MARKETING_FLAG == true)

                         where (u.MARKETING_FLAG == true)
                         && EntityFunctions.TruncateTime(p.CREATE_DATE) == EntityFunctions.TruncateTime(p.UPDATE_DATE)
                         && u.EXPORT_DATE == null

                         select new AfsMarketingEntity
                         {
                             EmpNum = u.EMPLOYEE_CODE,
                             FstName = u.FIRST_NAME,
                             LastName = u.LAST_NAME,
                             PhoneNo = p.PHONE_NO,
                             CreateDate = p.CREATE_DATE,
                             UpdateDate = p.UPDATE_DATE,
                             PhoneOrder = p.ORDER,
                             Status = u.STATUS
                         });

            return query.ToList();
        }

        public List<AfsMarketingEntity> GetAFSMarketingUpdate()
        {
            //DateTime minDate = DateTime.Now.Date;
            //DateTime maxDate = minDate.AddDays(1);

            var query = (from u in _context.TB_R_USER.AsNoTracking()
                         from p in _context.TB_R_USER_PHONE.Where(x => x.USER_ID == u.USER_ID).DefaultIfEmpty()
                         //where (p.UPDATE_DATE >= minDate) && (p.UPDATE_DATE <= maxDate)
                         //&& p.CREATE_DATE < minDate
                         //&& (u.MARKETING_FLAG == true)
                         where (u.MARKETING_FLAG == true)
                         && EntityFunctions.TruncateTime(p.CREATE_DATE) != EntityFunctions.TruncateTime(p.UPDATE_DATE)
                         && u.EXPORT_DATE == null

                         select new AfsMarketingEntity
                         {
                             EmpNum = u.EMPLOYEE_CODE,
                             FstName = u.FIRST_NAME,
                             LastName = u.LAST_NAME,
                             PhoneNo = p.PHONE_NO,
                             CreateDate = p.CREATE_DATE,
                             UpdateDate = p.UPDATE_DATE,
                             PhoneOrder = p.ORDER,
                             Status = u.STATUS
                         });

            return query.ToList();
        }

        public bool SaveExportDate(bool isUpdate)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var today = DateTime.Now;
                IQueryable<TB_R_USER_PHONE> phoneOwners = _context.TB_R_USER_PHONE;
                if(isUpdate)
                    phoneOwners =  phoneOwners.Where(p => EntityFunctions.TruncateTime(p.CREATE_DATE) != EntityFunctions.TruncateTime(p.UPDATE_DATE));
                else
                    phoneOwners =  phoneOwners.Where(p => EntityFunctions.TruncateTime(p.CREATE_DATE) == EntityFunctions.TruncateTime(p.UPDATE_DATE));


                var UserIds = phoneOwners.Select(o => o.USER_ID).Distinct();

                var users = from u in _context.TB_R_USER
                            where (u.MARKETING_FLAG == true)
                            && u.EXPORT_DATE == null
                            && UserIds.Contains(u.USER_ID)
                            select u;

                foreach (var u in users)
                {
                    u.EXPORT_DATE = today;
                    _context.Entry(u).Property("EXPORT_DATE").IsModified = true;
                }

                return (Save() > 0);
            }
            catch(Exception ex) {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }

            return false;
        }

        #region "Persistence"

        private int Save()
        {
            return _context.SaveChanges();
        }

        private void SetEntryCurrentValues(object entityTo, object entityFrom)
        {
            _context.Entry(entityTo).CurrentValues.SetValues(entityFrom);
            // Set state to Modified
            _context.Entry(entityTo).State = System.Data.Entity.EntityState.Modified;
        }

        private void SetEntryStateModified(object entity)
        {
            if (_context.Configuration.AutoDetectChangesEnabled == false)
            {
                // Set state to Modified
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }

        #endregion
    }
}
