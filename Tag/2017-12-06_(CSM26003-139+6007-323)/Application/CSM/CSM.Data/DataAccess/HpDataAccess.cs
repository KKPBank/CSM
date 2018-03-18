using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using CSM.Entity;
using log4net;
using System.Linq;
using CSM.Common.Utilities;
using System.Text.RegularExpressions;

namespace CSM.Data.DataAccess
{
    public class HpDataAccess : IHpDataAccess
    {
        private readonly CSMContext _context;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HpDataAccess));

        public HpDataAccess(CSMContext context)
        {
            _context = context;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Database.CommandTimeout = Constants.CommandTimeout;
        }

        public bool SaveHpActivity(List<HpActivityEntity> hpActivities)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    ValidationContext vc = null;
                    _context.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        if (hpActivities != null && hpActivities.Count > 0)
                        {
                            _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_HP_ACTIVITY");
                            this.Save();

                            foreach (HpActivityEntity hpActivity in hpActivities)
                            {
                                TB_I_HP_ACTIVITY dbHpActivity = new TB_I_HP_ACTIVITY();

                                dbHpActivity.CHANNEL = hpActivity.Channel;
                                dbHpActivity.TYPE = hpActivity.Type;
                                dbHpActivity.AREA = hpActivity.Area;
                                dbHpActivity.STATUS = hpActivity.Status;
                                dbHpActivity.DESCRIPTION = hpActivity.Description;
                                dbHpActivity.COMMENT = hpActivity.Comment;
                                dbHpActivity.ASSET_INFO = hpActivity.AssetInfo;
                                dbHpActivity.CONTACT_INFO = hpActivity.ContactInfo;
                                dbHpActivity.A_NO = hpActivity.Ano;
                                dbHpActivity.CALL_ID = hpActivity.CallId;
                                dbHpActivity.CONTACT_NAME = hpActivity.ContactName;
                                dbHpActivity.CONTACT_LAST_NAME = hpActivity.ContactLastName;
                                dbHpActivity.CONTACT_PHONE = hpActivity.ContactPhone;
                                dbHpActivity.DONE_FLG = hpActivity.DoneFlg;
                                dbHpActivity.CREATE_DATE = hpActivity.CreateDate;
                                dbHpActivity.CREATE_BY = hpActivity.CreateBy;
                                dbHpActivity.START_DATE = hpActivity.StartDate;
                                dbHpActivity.END_DATE = hpActivity.EndDate;
                                dbHpActivity.OWNER_LOGIN = hpActivity.OwnerLogin;
                                dbHpActivity.OWNER_PER_ID = hpActivity.OwnerPerId;
                                dbHpActivity.UPDATE_DATE = hpActivity.UpdateDate;
                                dbHpActivity.UPDATE_BY = hpActivity.UpdateBy;
                                dbHpActivity.SR_NO = hpActivity.SrNo;
                                dbHpActivity.CALL_FLG = hpActivity.CallFlg;
                                dbHpActivity.ENQ_FLG = hpActivity.EnqFlg;
                                dbHpActivity.LOC_ENQ_FLG = hpActivity.LocEnqFlg;
                                dbHpActivity.DOC_REQ_FLG = hpActivity.DocReqFlg;
                                dbHpActivity.PRI_ISSUED_FLG = hpActivity.PriIssuedFlg;
                                dbHpActivity.ASSET_INSPECT_FLG = hpActivity.AssetInspectFlg;
                                dbHpActivity.PLAN_START_DATE = hpActivity.PlanstartDate;
                                dbHpActivity.CONTACT_FAX = hpActivity.ContactFax;
                                dbHpActivity.CONTACT_EMAIL = hpActivity.ContactEmail;

                                vc = new ValidationContext(hpActivity, null, null);
                                var validationResults = new List<ValidationResult>();
                                bool valid = Validator.TryValidateObject(hpActivity, vc, validationResults, true);
                                if (!valid)
                                {
                                    dbHpActivity.ERROR =
                                        validationResults.Select(x => x.ErrorMessage)
                                            .Aggregate((i, j) => i + Environment.NewLine + j);
                                }

                                _context.TB_I_HP_ACTIVITY.Add(dbHpActivity);
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

        public bool SaveHpActivityComplete(ref int numOfComplete, ref int numOfError, ref string messageError)
        {
            try
            {
                var query = (from activity in _context.TB_I_HP_ACTIVITY.AsNoTracking()
                             where string.IsNullOrEmpty(activity.ERROR)
                             select new HpActivityEntity
                             {                          
                                 HpActivityId = activity.HP_ACTIVITY_ID,
                                 Comment = activity.COMMENT,
                                 Status = activity.STATUS
                             });

                if(query.Any())
                {
                    var hpList = query.ToList();
                    foreach (var item in hpList)
                    {
                        string comment = item.Comment.Replace(" ", String.Empty);
                        Match match = Regex.Match(comment, @";\s*[0-9]{1,}\s*;", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            string srNo = comment.ExtractSRNo();
                            string srStatusCode = comment.ExtractSRStatus();

                            var objSr = _context.TB_T_SR.FirstOrDefault(x => !string.IsNullOrEmpty(srNo) && x.SR_NO == srNo);
                            var objSrStatus = _context.TB_C_SR_STATUS.FirstOrDefault(x => !string.IsNullOrEmpty(srStatusCode) && x.SR_STATUS_CODE == srStatusCode);

                            if (objSr != null)
                            {
                                // SrId
                                item.SrId = objSr.SR_ID;

                                // SrStatus
                                if(objSrStatus != null)
                                {
                                    item.SrStatusId = objSrStatus.SR_STATUS_ID;
                                }
                                else
                                {
                                    if (item.Status == "เรียบร้อย" )
                                    {
                                        item.SrStatusId = Constants.SRStatusId.Closed;
                                    }
                                    else
                                    {
                                        item.SrStatusId = objSr.SR_STATUS_ID;
                                    }                                    
                                }
                            }
                            else
                            {
                                item.Error = "No found SrNo";
                            }
                        }
                        else
                        {
                            item.Error = "No found SrNo";
                        }
                    }

                    #region Update Tale Interface

                    int cntComplete = 0;
                    int cntError = 0;

                    foreach (var item in hpList)
                    {
                        TB_I_HP_ACTIVITY dbHp = _context.TB_I_HP_ACTIVITY.FirstOrDefault(x => x.HP_ACTIVITY_ID == item.HpActivityId);
                        if (!string.IsNullOrEmpty(item.Error))
                        {
                            dbHp.ERROR = item.Error;
                            cntError++;
                        }
                        else
                        {
                            dbHp.SR_ID = item.SrId;
                            dbHp.SR_STATUS_ID = item.SrStatusId;
                            cntComplete++;
                        }

                        SetEntryStateModified(dbHp);
                    }

                    this.Save();

                    #endregion

                    numOfComplete = cntComplete;
                    numOfError = cntError;
                }

                #region "comment out"

                //System.Data.Entity.Core.Objects.ObjectParameter outputNumOfComplete = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfComplete", typeof(int));
                //System.Data.Entity.Core.Objects.ObjectParameter outputNumOfError = new System.Data.Entity.Core.Objects.ObjectParameter("NumOfError", typeof(int));
                //System.Data.Entity.Core.Objects.ObjectParameter outputIsError = new System.Data.Entity.Core.Objects.ObjectParameter("IsError", typeof(int));
                //System.Data.Entity.Core.Objects.ObjectParameter outputMsgError = new System.Data.Entity.Core.Objects.ObjectParameter("MessageError", typeof(string));

                //_context.SP_IMPORT_HP_ACTIVITY(outputNumOfComplete, outputIsError, outputNumOfError, outputMsgError);

                //if ((int)outputIsError.Value == 0)
                //{
                //    numOfComplete = (int)outputNumOfComplete.Value;
                //    numOfError = (int)outputNumOfError.Value;
                //    //messageError = (string)outputMsgError.Value;
                //}
                //else
                //{
                //    numOfComplete = 0;
                //    numOfError = 0;
                //    messageError = (string)outputMsgError.Value;
                //}

                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            return true;
        }     

        public List<HpActivityEntity> GetHpActivityExport()
        {
            var query = (from activity in _context.TB_I_HP_ACTIVITY.AsNoTracking()
                         where !string.IsNullOrEmpty(activity.ERROR)
                         select new HpActivityEntity
                         {
                                Channel = activity.CHANNEL,
                                Type = activity.TYPE,
                                Area = activity.AREA,
                                Status = activity.STATUS,
                                Description = activity.DESCRIPTION,
                                Comment = activity.COMMENT,
                                AssetInfo = activity.ASSET_INFO,
                                ContactInfo = activity.CONTACT_INFO,
                                Ano = activity.A_NO,
                                CallId = activity.CALL_ID,
                                ContactName = activity.CONTACT_NAME,
                                ContactLastName = activity.CONTACT_LAST_NAME,
                                ContactPhone = activity.CONTACT_PHONE,
                                DoneFlg = activity.DONE_FLG,
                                CreateDate = activity.CREATE_DATE,
                                CreateBy = activity.CREATE_BY,
                                StartDate = activity.START_DATE,
                                EndDate = activity.END_DATE,
                                OwnerLogin = activity.OWNER_LOGIN,
                                OwnerPerId = activity.OWNER_PER_ID,
                                UpdateDate = activity.UPDATE_DATE,
                                UpdateBy = activity.UPDATE_BY,
                                SrNo = activity.SR_NO,
                                CallFlg = activity.CALL_FLG,
                                EnqFlg = activity.ENQ_FLG,
                                LocEnqFlg = activity.LOC_ENQ_FLG,
                                DocReqFlg = activity.DOC_REQ_FLG,
                                PriIssuedFlg = activity.PRI_ISSUED_FLG,
                                AssetInspectFlg = activity.ASSET_INSPECT_FLG,                   
                                PlanstartDate = activity.PLAN_START_DATE,
                                ContactFax = activity.CONTACT_FAX,
                                ContactEmail = activity.CONTACT_EMAIL,
                                Error = activity.ERROR
                         });

            return query.ToList();
        }

        public IEnumerable<HpStatusEntity> GetHpStatus(int? id = null)
        {
            return (from h in _context.TB_M_HP_STATUS.AsNoTracking()
                    where (id == null || h.HP_ID == id)
                    select new HpStatusEntity
                    {
                        HpStatusId = h.HP_ID,
                        HpLangIndeCode = h.HP_LANGUAGE_INDEPENDENT_CODE,
                        HpSubject = h.HP_SUBJECT
                    });
        }

        public List<ServiceRequestForSaveEntity> GetSrWithHpActivity()
        {
            int? nullValue = null;
            var query = from ac in _context.TB_I_HP_ACTIVITY
                        from usr in _context.TB_R_USER.Where(x => x.USERNAME == ac.CREATE_BY).DefaultIfEmpty()
                        from sr in _context.TB_T_SR.Where(x => x.SR_ID == ac.SR_ID).DefaultIfEmpty()
                        from sts in _context.TB_C_SR_STATUS_CHANGE.Where(x =>
                            x.PRODUCTGROUP_ID == sr.PRODUCTGROUP_ID 
                            && x.PRODUCT_ID == sr.PRODUCT_ID 
                            && x.CAMPAIGNSERVICE_ID == sr.CAMPAIGNSERVICE_ID
                            && x.TYPE_ID == sr.TYPE_ID 
                            && x.AREA_ID == sr.AREA_ID 
                            && x.SUBAREA_ID == sr.SUBAREA_ID
                            && x.FROM_SR_STATUS_ID == sr.SR_STATUS_ID 
                            && x.TO_SR_STATUS_ID == ac.SR_STATUS_ID
                        ).DefaultIfEmpty()
                        where (ac.SR_ID.HasValue)
                        select new ServiceRequestForSaveEntity
                        {
                            SrId = sr.SR_ID,
                            OwnerBranchId = sr.OWNER_BRANCH_ID ?? 0,
                            OwnerUserId = sr.OWNER_USER_ID ?? 0,
                            DelegateBranchId = sr.DELEGATE_BRANCH_ID,
                            DelegateUserId = sr.DELEGATE_USER_ID,
                            SrStatusId = sts  != null ? (ac.SR_STATUS_ID ?? 0) : sr.SR_STATUS_ID ?? 0,
                            ActivityDescription = ac.COMMENT,
                            ActivityTypeId = Constants.BatchInboundActivityTypeId,
                            CreateUserId = usr != null ? usr.USER_ID : nullValue,
                            CreateUsername = usr == null ? ac.CREATE_BY : null,
                            //SrEmailTemplateId                            
                            //CreateActivityUserId
                            CPN_IsSecret = sr.CPN_SECRET,
                            CPN_IsCAR = sr.CPN_SECRET
                        };

            return query.ToList<ServiceRequestForSaveEntity>();
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
