using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Entity;
using log4net;
using System.Globalization;
using CSM.Common.Utilities;

namespace CSM.Data.DataAccess
{
    public class MappingProductTypeDataAccess : IMappingProductTypeDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MappingProductTypeDataAccess));

        public MappingProductTypeDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        #region "MappingProductType"

        public IEnumerable<QuestionGroupTableItemEntity> GetQuestionGroupList(QuestionSelectSearchFilter searchFilter)
        {
            var resultQuery = (from questionGroup in _context.TB_M_QUESTIONGROUP
                from createUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.CREATE_USER).DefaultIfEmpty()
                from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == questionGroup.UPDATE_USER).DefaultIfEmpty()
                where
                    ((searchFilter.QuestionName == null || questionGroup.QUESTIONGROUP_NAME.Contains(searchFilter.QuestionName)) &&
                     (searchFilter.ProductId == null || questionGroup.PRODUCT_ID == searchFilter.ProductId))
                select new QuestionGroupTableItemEntity
                {
                    QuestionGroupId = questionGroup.QUESTIONGROUP_ID,
                    QuestionGroupName = questionGroup.QUESTIONGROUP_NAME,
                    IsActive = questionGroup.QUESTIONGROUP_IS_ACTIVE,
                    UpdateUser = (updateUser != null
                        ? new UserEntity
                        {
                            PositionCode = updateUser.POSITION_CODE,
                            Firstname = updateUser.FIRST_NAME,
                            Lastname = updateUser.LAST_NAME
                        }
                        : null),
                    CreateUser = (createUser != null
                        ? new UserEntity
                        {
                            PositionCode = createUser.POSITION_CODE,
                            Firstname = createUser.FIRST_NAME,
                            Lastname = createUser.LAST_NAME
                        }
                        : null),
                    UpdateDate = questionGroup.UPDATE_DATE,
                    ProductId = questionGroup.PRODUCT_ID,
                    Description = questionGroup.QUESTIONGROUP_DESC,
                    QuestionNo = questionGroup.TB_M_QUESTIONGROUP_QUESTION.Count()
                });

            if (!string.IsNullOrEmpty(searchFilter.QuestionIdList))
            {
                var questionIdArray = searchFilter.QuestionIdList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                resultQuery = resultQuery.Where(q => !questionIdArray.Contains(q.QuestionGroupId));
            }

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetQuestionGroupListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public bool SaveMapProduct(MappingProductTypeItemEntity mappingItemEntity, List<ProductQuestionGroupItemEntity> productQuestionEntityList)
        {
            _context.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var isEdit = mappingItemEntity.MapProductId.HasValue;
                TB_M_MAP_PRODUCT mapProduct;

                if (!isEdit)
                {
                    //add mode
                    mapProduct = new TB_M_MAP_PRODUCT();
                }
                else
                {
                    //edit mode
                    mapProduct = _context.TB_M_MAP_PRODUCT.SingleOrDefault(m => m.MAP_PRODUCT_ID == mappingItemEntity.MapProductId.Value);
                    if (mapProduct == null)
                    {
                        Logger.ErrorFormat("MAP_PRODUCT ID: {0} does not exist", mappingItemEntity.AreaId);
                        return false;
                    }
                }

                mapProduct.PRODUCT_ID = mappingItemEntity.ProductId;
                mapProduct.CAMPAIGNSERVICE_ID = mappingItemEntity.CampaignServiceId;
                mapProduct.AREA_ID = mappingItemEntity.AreaId;
                mapProduct.SUBAREA_ID = mappingItemEntity.SubAreaId;
                mapProduct.TYPE_ID = mappingItemEntity.TypeId;
                mapProduct.DEFAULT_OWNER_USER_ID = mappingItemEntity.OwnerUserId;
                mapProduct.SR_PAGE_ID = mappingItemEntity.SrPageId;
                mapProduct.MAP_PRODUCT_IS_VERIFY = mappingItemEntity.IsVerify;
                mapProduct.MAP_PRODUCT_IS_ACTIVE = mappingItemEntity.IsActive;
                mapProduct.UPDATE_USER = mappingItemEntity.UserId;
                mapProduct.UPDATE_DATE = DateTime.Now;

                mapProduct.IS_VERIFY_OTP = mappingItemEntity.IsVerifyOTP;
                mapProduct.IS_SR_SECRET = mappingItemEntity.IsSRSecret;
                mapProduct.OTP_TEMPLATE_ID = mappingItemEntity.OTPTemplate.OTPTemplateId;
                mapProduct.HP_LANGUAGE_INDEPENDENT_CODE = mappingItemEntity.HPLanguageIndependentCode;
                mapProduct.HP_SUBJECT = mappingItemEntity.HPSubject;

                if (!isEdit)
                {
                    //add
                    mapProduct.CREATE_USER = mappingItemEntity.UserId;
                    mapProduct.CREATE_DATE = DateTime.Now;
                    _context.TB_M_MAP_PRODUCT.Add(mapProduct);
                    Save();

                    //save map product question group
                    if (productQuestionEntityList.Any())
                    {
                        foreach (var productQuestionEntity in productQuestionEntityList)
                        {
                            var mapProductQuestionGroup = new TB_M_MAP_PRODUCT_QUESTIONGROUP();
                            mapProductQuestionGroup.MAP_PRODUCT_ID = mapProduct.MAP_PRODUCT_ID;
                            mapProductQuestionGroup.QUESTIONGROUP_ID = Convert.ToInt32(productQuestionEntity.id, CultureInfo.InvariantCulture);
                            mapProductQuestionGroup.REQUIRE_AMOUNT_PASS = Convert.ToInt32(productQuestionEntity.pass_value, CultureInfo.InvariantCulture);
                            mapProductQuestionGroup.SEQ_NO = Convert.ToInt32(productQuestionEntity.seq, CultureInfo.InvariantCulture);
                            _context.TB_M_MAP_PRODUCT_QUESTIONGROUP.Add(mapProductQuestionGroup);
                            Save();
                        }
                    }
                }
                else
                {
                    SetEntryStateModified(mapProduct);

                    //delete map product question group
                    var mapProductQuestionList = _context.TB_M_MAP_PRODUCT_QUESTIONGROUP.Where(q => q.MAP_PRODUCT_ID == mappingItemEntity.MapProductId);
                    foreach (var mapProductQuestionItem in mapProductQuestionList)
                    {
                        _context.TB_M_MAP_PRODUCT_QUESTIONGROUP.Remove(mapProductQuestionItem);
                    }
                    this.Save();

                    //update map product question group
                    if (productQuestionEntityList.Any())
                    {
                        foreach (var productQuestionEntity in productQuestionEntityList)
                        {
                            var mapProductQuestionGroup = new TB_M_MAP_PRODUCT_QUESTIONGROUP();
                            mapProductQuestionGroup.MAP_PRODUCT_ID = mapProduct.MAP_PRODUCT_ID;
                            mapProductQuestionGroup.QUESTIONGROUP_ID = Convert.ToInt32(productQuestionEntity.id, CultureInfo.InvariantCulture);
                            mapProductQuestionGroup.REQUIRE_AMOUNT_PASS = Convert.ToInt32(productQuestionEntity.pass_value, CultureInfo.InvariantCulture);
                            mapProductQuestionGroup.SEQ_NO = Convert.ToInt32(productQuestionEntity.seq, CultureInfo.InvariantCulture);
                            _context.TB_M_MAP_PRODUCT_QUESTIONGROUP.Add(mapProductQuestionGroup);
                            Save();
                        }
                    }
                }

                Save();
                return true;

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                return false;
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public IEnumerable<MappingProductTypeItemEntity> GetMappingList(MappingProductSearchFilter searchFilter)
        {
            bool isActiveHasNotValueOrAll = false;
            bool mappingIsActive = true;

            if (string.IsNullOrEmpty(searchFilter.IsActive) || searchFilter.IsActive == "all")
            {
                isActiveHasNotValueOrAll = true;
            }
            else
            {
                if (searchFilter.IsActive == "false")
                {
                    mappingIsActive = false;
                }
            }

            bool isVerifyHasNotValueOrAll = false;
            bool mappingIsVerify = true;
            if (string.IsNullOrEmpty(searchFilter.IsVerify) || searchFilter.IsVerify == "all")
            {
                isVerifyHasNotValueOrAll = true;
            }
            else
            {
                if (searchFilter.IsVerify == "false")
                {
                    mappingIsVerify = false;
                }
            }

            var resultQuery =
                (from mapping in _context.TB_M_MAP_PRODUCT
                 from product in _context.TB_R_PRODUCT.Where(x => x.PRODUCT_ID == mapping.PRODUCT_ID).DefaultIfEmpty()
                 from productGroup in _context.TB_R_PRODUCTGROUP.Where(x => x.PRODUCTGROUP_ID == product.PRODUCTGROUP_ID).DefaultIfEmpty()
                 from campaignService in _context.TB_R_CAMPAIGNSERVICE.Where(x => x.CAMPAIGNSERVICE_ID == mapping.CAMPAIGNSERVICE_ID).DefaultIfEmpty()
                 from area in _context.TB_M_AREA.Where(x => x.AREA_ID == mapping.AREA_ID).DefaultIfEmpty()
                 from subArea in _context.TB_M_SUBAREA.Where(x => x.SUBAREA_ID == mapping.SUBAREA_ID).DefaultIfEmpty()
                 from type in _context.TB_M_TYPE.Where(x => x.TYPE_ID == mapping.TYPE_ID).DefaultIfEmpty()
                 from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == mapping.UPDATE_USER).DefaultIfEmpty()
                 from ownerUser in _context.TB_R_USER.Where(x => x.USER_ID == mapping.DEFAULT_OWNER_USER_ID).DefaultIfEmpty()
                 from otp in _context.TB_M_OTP_TEMPLATE.Where(x => x.OTP_TEMP_ID == mapping.OTP_TEMPLATE_ID).DefaultIfEmpty()
                 from hp in _context.TB_M_HP_STATUS.Where(x => x.HP_LANGUAGE_INDEPENDENT_CODE == mapping.HP_LANGUAGE_INDEPENDENT_CODE).DefaultIfEmpty()
                 where ((!searchFilter.ProductGroupId.HasValue || product.PRODUCTGROUP_ID == searchFilter.ProductGroupId.Value) &&
                        (!searchFilter.ProductId.HasValue || mapping.PRODUCT_ID == searchFilter.ProductId.Value) &&
                        (!searchFilter.CampaignId.HasValue || mapping.CAMPAIGNSERVICE_ID == searchFilter.CampaignId.Value) &&
                        (!searchFilter.TypeId.HasValue || mapping.TYPE_ID == searchFilter.TypeId) &&
                        (!searchFilter.AreaId.HasValue || mapping.AREA_ID == searchFilter.AreaId) &&
                        (!searchFilter.SubAreaId.HasValue || mapping.SUBAREA_ID == searchFilter.SubAreaId) &&
                        (!searchFilter.OwnerId.HasValue || ownerUser.USER_ID == searchFilter.OwnerId) &&
                        (isActiveHasNotValueOrAll || mapping.MAP_PRODUCT_IS_ACTIVE == mappingIsActive) &&
                        (isVerifyHasNotValueOrAll || mapping.MAP_PRODUCT_IS_VERIFY == mappingIsVerify) &&
                        (!searchFilter.IsVerifyOTP.HasValue || mapping.IS_VERIFY_OTP == searchFilter.IsVerifyOTP))
                 select new MappingProductTypeItemEntity
                 {
                     MapProductId = mapping.MAP_PRODUCT_ID,
                     ProductGroupName = productGroup.PRODUCTGROUP_NAME,
                     ProductName = product.PRODUCT_NAME,
                     CampaignName = campaignService.CAMPAIGNSERVICE_NAME,
                     AreaName = area.AREA_NAME,
                     SubAreaName = subArea.SUBAREA_NAME,
                     TypeName = type.TYPE_NAME,
                     Verify = mapping.MAP_PRODUCT_IS_VERIFY ? "Yes" : "No",
                     QuestionGroupNameList = mapping.TB_M_MAP_PRODUCT_QUESTIONGROUP.Select(grp => grp.TB_M_QUESTIONGROUP.QUESTIONGROUP_NAME).ToList(),
                     OwnerUser = (ownerUser != null
                         ? new UserEntity
                         {
                             PositionCode = ownerUser.POSITION_CODE,
                             Firstname = ownerUser.FIRST_NAME,
                             Lastname = ownerUser.LAST_NAME
                         } : null),
                     Active = mapping.MAP_PRODUCT_IS_ACTIVE ? "Active" : "Inactive",
                     UpdateUser = (updateUser != null
                         ? new UserEntity
                         {
                             PositionCode = updateUser.POSITION_CODE,
                             Firstname = updateUser.FIRST_NAME,
                             Lastname = updateUser.LAST_NAME
                         }
                         : null),
                     UpdateDate = mapping.UPDATE_DATE,
                     IsVerifyOTP = mapping.IS_VERIFY_OTP,
                     IsSRSecret = mapping.IS_SR_SECRET,
                     OTPTemplate = (otp != null ? new OTPTemplateEntity()
                     {
                         OTPTemplateId = otp.OTP_TEMP_ID,
                         OTPTemplateCode = otp.OTP_TEMP_CODE,
                         OTPTemplateName = otp.OTP_TEMP_NAME
                     } : null),
                     HPLanguageIndependentCode = mapping.HP_LANGUAGE_INDEPENDENT_CODE,
                     HPSubject = mapping.HP_SUBJECT,
                     HpStatus = (hp != null ? new HpStatusEntity()
                     {
                         HpStatusId = hp.HP_ID,
                         HpLangIndeCode = hp.HP_LANGUAGE_INDEPENDENT_CODE,
                         HpSubject = hp.HP_SUBJECT
                     } : null)
                 });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetMappingListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }

        public MappingProductTypeItemEntity GetMappingById(int mapProductId)
        {
            return (from mapping in _context.TB_M_MAP_PRODUCT
                    from createUser in _context.TB_R_USER.Where(x => x.USER_ID == mapping.CREATE_USER).DefaultIfEmpty()
                    from updateUser in _context.TB_R_USER.Where(x => x.USER_ID == mapping.UPDATE_USER).DefaultIfEmpty()
                    from ownerUser in _context.TB_R_USER.Where(x => x.USER_ID == mapping.DEFAULT_OWNER_USER_ID).DefaultIfEmpty()
                    from OTP in _context.TB_M_OTP_TEMPLATE.Where(x => x.OTP_TEMP_ID == mapping.OTP_TEMPLATE_ID).DefaultIfEmpty()
                    from hps in _context.TB_M_HP_STATUS.Where(x => x.HP_LANGUAGE_INDEPENDENT_CODE == mapping.HP_LANGUAGE_INDEPENDENT_CODE).DefaultIfEmpty()
                    where mapping.MAP_PRODUCT_ID == mapProductId
                    select new MappingProductTypeItemEntity
                    {
                        MapProductId = mapping.MAP_PRODUCT_ID,
                        ProductGroupId = mapping.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_ID,
                        ProductGroupName = mapping.TB_R_PRODUCT.TB_R_PRODUCTGROUP.PRODUCTGROUP_NAME,
                        ProductId = mapping.PRODUCT_ID,
                        ProductName = mapping.TB_R_PRODUCT.PRODUCT_NAME,
                        CampaignServiceId = mapping.CAMPAIGNSERVICE_ID,
                        CampaignName = mapping.TB_R_CAMPAIGNSERVICE.CAMPAIGNSERVICE_NAME,
                        AreaId = mapping.AREA_ID,
                        AreaName = mapping.TB_M_AREA.AREA_NAME,
                        SubAreaId = mapping.SUBAREA_ID,
                        SubAreaName = mapping.TB_M_SUBAREA.SUBAREA_NAME,
                        TypeId = mapping.TYPE_ID,
                        TypeName = mapping.TB_M_TYPE.TYPE_NAME,
                        OwnerUserId = mapping.DEFAULT_OWNER_USER_ID,
                        OwnerSrName = mapping.DEFAULT_OWNER_USER_ID.HasValue? (ownerUser.FIRST_NAME + " " + ownerUser.LAST_NAME) : "",
                        OwnerBranchId = mapping.DEFAULT_OWNER_USER_ID.HasValue ? ownerUser.BRANCH_ID : (int?) null,
                        OwnerBranchName = mapping.DEFAULT_OWNER_USER_ID.HasValue ? ownerUser.TB_R_BRANCH.BRANCH_NAME : "",
                        SrPageId = mapping.SR_PAGE_ID,
                        SrPageName = mapping.TB_C_SR_PAGE.SR_PAGE_NAME,
                        IsVerify = mapping.MAP_PRODUCT_IS_VERIFY,
                        IsActive = mapping.MAP_PRODUCT_IS_ACTIVE,
                        HPSubject = mapping.HP_SUBJECT,
                        HPLanguageIndependentCode = mapping.HP_LANGUAGE_INDEPENDENT_CODE,
                        CreateUser = (createUser != null
                            ? new UserEntity
                            {
                                PositionCode = createUser.POSITION_CODE,
                                Firstname = createUser.FIRST_NAME,
                                Lastname = createUser.LAST_NAME
                            }
                            : null),
                        UpdateUser = (updateUser != null
                            ? new UserEntity
                            {
                                PositionCode = updateUser.POSITION_CODE,
                                Firstname = updateUser.FIRST_NAME,
                                Lastname = updateUser.LAST_NAME
                            }
                            : null),
                        CreateDate = mapping.CREATE_DATE,
                        UpdateDate = mapping.UPDATE_DATE,
                        IsVerifyOTP = mapping.IS_VERIFY_OTP,
                        IsSRSecret = mapping.IS_SR_SECRET,
                        OTPTemplate = (OTP != null ? new OTPTemplateEntity
                        {
                            OTPTemplateId = OTP.OTP_TEMP_ID,
                            OTPTemplateCode = OTP.OTP_TEMP_CODE,
                            OTPTemplateName = OTP.OTP_TEMP_NAME
                        } : null),
                        HpStatus = (hps != null ? new HpStatusEntity
                        {
                            HpStatusId = hps.HP_ID,
                            HpLangIndeCode = hps.HP_LANGUAGE_INDEPENDENT_CODE,
                            HpSubject = hps.HP_SUBJECT
                        } : null)
                    }).SingleOrDefault();
        }

        public IEnumerable<QuestionGroupEditTableItemEntity> GetQuestionGroupById(QuestionGroupEditSearchFilter searchFilter)
        {
            var resultQuery = (from productQuestionGroup in _context.TB_M_MAP_PRODUCT_QUESTIONGROUP
                               where (productQuestionGroup.MAP_PRODUCT_ID == searchFilter.MapProductId.Value)
                               select new QuestionGroupEditTableItemEntity
                               {
                                   MapProductQuestionGroupId = productQuestionGroup.MAP_PRODUCT_QUESTIONGROUP_ID,
                                   MapProductId = productQuestionGroup.MAP_PRODUCT_ID,
                                   QuestionGroupId = productQuestionGroup.QUESTIONGROUP_ID,
                                   QuestionGroupName = productQuestionGroup.TB_M_QUESTIONGROUP.QUESTIONGROUP_NAME,
                                   PassAmount = productQuestionGroup.REQUIRE_AMOUNT_PASS,
                                   QuestionNo = productQuestionGroup.TB_M_QUESTIONGROUP.TB_M_QUESTIONGROUP_QUESTION.Count(),
                                   Seq = productQuestionGroup.SEQ_NO
                               });

            int startPageIndex = (searchFilter.PageNo - 1) * searchFilter.PageSize;
            searchFilter.TotalRecords = resultQuery.Count();

            if (startPageIndex >= searchFilter.TotalRecords)
            {
                startPageIndex = 0;
                searchFilter.PageNo = 1;
            }

            resultQuery = SetQuestionGroupByIdListSort(resultQuery, searchFilter);
            return resultQuery.Skip(startPageIndex).Take(searchFilter.PageSize).ToList();
        }
        
        public IEnumerable<QuestionGroupEditTableItemEntity> GetLoadQuestionGroupById(int mapProductId)
        {
            var resultQuery = (from productQuestionGroup in _context.TB_M_MAP_PRODUCT_QUESTIONGROUP
                               where (productQuestionGroup.MAP_PRODUCT_ID == mapProductId)
                               select new QuestionGroupEditTableItemEntity
                               {
                                   MapProductQuestionGroupId = productQuestionGroup.MAP_PRODUCT_QUESTIONGROUP_ID,
                                   MapProductId = productQuestionGroup.MAP_PRODUCT_ID,
                                   QuestionGroupId = productQuestionGroup.QUESTIONGROUP_ID,
                                   QuestionGroupName = productQuestionGroup.TB_M_QUESTIONGROUP.QUESTIONGROUP_NAME,
                                   PassAmount = productQuestionGroup.REQUIRE_AMOUNT_PASS,
                                   QuestionNo = productQuestionGroup.TB_M_QUESTIONGROUP.TB_M_QUESTIONGROUP_QUESTION.Count(),
                                   Seq = productQuestionGroup.SEQ_NO
                               }).OrderBy(q => q.MapProductId).ThenBy(q => q.Seq).ToList();

            return resultQuery;
        }

        public bool CheckDuplicateMappProduct(MappingProductTypeItemEntity mappingItemEntity)
        {
            var isDuplicate = false;

            if (!mappingItemEntity.MapProductId.HasValue)
            {
                //add mode
                if (mappingItemEntity.CampaignServiceId.HasValue)
                {
                    isDuplicate =
                        _context.TB_M_MAP_PRODUCT.Any(
                            q =>
                                q.PRODUCT_ID == mappingItemEntity.ProductId &&
                                q.CAMPAIGNSERVICE_ID == mappingItemEntity.CampaignServiceId.Value &&
                                q.AREA_ID == mappingItemEntity.AreaId &&
                                q.SUBAREA_ID == mappingItemEntity.SubAreaId &&
                                q.TYPE_ID == mappingItemEntity.TypeId);
                }
                else
                {
                    isDuplicate =
                        _context.TB_M_MAP_PRODUCT.Any(
                            q =>
                                q.PRODUCT_ID == mappingItemEntity.ProductId &&
                                q.CAMPAIGNSERVICE_ID == null &&
                                q.AREA_ID == mappingItemEntity.AreaId &&
                                q.SUBAREA_ID == mappingItemEntity.SubAreaId &&
                                q.TYPE_ID == mappingItemEntity.TypeId);
                }
            }
            else
            {
                //edit mode
                if (mappingItemEntity.CampaignServiceId.HasValue)
                {
                    isDuplicate =
                    _context.TB_M_MAP_PRODUCT.Any(
                        q =>
                            q.MAP_PRODUCT_ID != mappingItemEntity.MapProductId.Value &&
                            q.PRODUCT_ID == mappingItemEntity.ProductId &&
                            q.CAMPAIGNSERVICE_ID == mappingItemEntity.CampaignServiceId.Value &&
                            q.AREA_ID == mappingItemEntity.AreaId &&
                            q.SUBAREA_ID == mappingItemEntity.SubAreaId &&
                            q.TYPE_ID == mappingItemEntity.TypeId);
                }
                else
                {
                    isDuplicate =
                    _context.TB_M_MAP_PRODUCT.Any(
                        q =>
                            q.MAP_PRODUCT_ID != mappingItemEntity.MapProductId.Value &&
                            q.PRODUCT_ID == mappingItemEntity.ProductId &&
                            q.CAMPAIGNSERVICE_ID == null &&
                            q.AREA_ID == mappingItemEntity.AreaId &&
                            q.SUBAREA_ID == mappingItemEntity.SubAreaId &&
                            q.TYPE_ID == mappingItemEntity.TypeId);
                }
            }
            return isDuplicate;
        }

        #endregion

        public IEnumerable<OTPTemplateEntity> GetOTPTemplate(int? id = null, short? status = null)
        {
            var res = from o in _context.TB_M_OTP_TEMPLATE
                      where (id == null || o.OTP_TEMP_ID == id)
                        && (status == null || o.STATUS == status)
                      select new OTPTemplateEntity
                      {
                          OTPTemplateId = o.OTP_TEMP_ID,
                          OTPTemplateCode = o.OTP_TEMP_CODE,
                          OTPTemplateName = o.OTP_TEMP_NAME
                      };
            return res;
        }

        #region "Functions"
        private static IQueryable<MappingProductTypeItemEntity> SetMappingListSort(IQueryable<MappingProductTypeItemEntity> slaList, MappingProductSearchFilter searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter.SortOrder))
                searchFilter.SortOrder = "ASC";

            if (string.IsNullOrEmpty(searchFilter.SortField))
                searchFilter.SortField = "ProductGroup";

            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return
                            slaList.OrderBy(a => a.ProductGroupName)
                                .ThenBy(a => a.ProductName)
                                .ThenBy(a => a.CampaignName)
                                .ThenBy(a => a.TypeName)
                                .ThenBy(a => a.AreaName)
                                .ThenBy(a => a.SubAreaName);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return
                            slaList.OrderByDescending(a => a.ProductGroupName)
                                .ThenByDescending(a => a.ProductName)
                                .ThenByDescending(a => a.CampaignName)
                                .ThenByDescending(a => a.TypeName)
                                .ThenByDescending(a => a.AreaName)
                                .ThenByDescending(a => a.SubAreaName);
                }
            }
        }

        private static IQueryable<QuestionGroupTableItemEntity> SetQuestionGroupListSort(IQueryable<QuestionGroupTableItemEntity> areaList, QuestionSelectSearchFilter searchFilter)
        {

            if (!string.IsNullOrEmpty(searchFilter.SortField))
            {
                if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
                {

                    switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                    {
                        default:
                            return areaList.OrderBy(a => a.QuestionGroupName);
                    }
                }
                else
                {

                    switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                    {
                        default:
                            return areaList.OrderByDescending(a => a.QuestionGroupName);
                    }
                }
            }

            return areaList.OrderBy(a => a.QuestionGroupName);
        }

        private static IQueryable<QuestionGroupEditTableItemEntity> SetQuestionGroupByIdListSort(IQueryable<QuestionGroupEditTableItemEntity> mapProductQuestionGroup, QuestionGroupEditSearchFilter searchFilter)
        {
            if (searchFilter.SortOrder.ToUpper(CultureInfo.InvariantCulture).Equals("ASC"))
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return mapProductQuestionGroup.OrderBy(a => a.Seq);
                }
            }
            else
            {
                switch (searchFilter.SortField.ToUpper(CultureInfo.InvariantCulture))
                {
                    default:
                        return mapProductQuestionGroup.OrderByDescending(a => a.Seq);
                }
            }
        }

        #endregion

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
