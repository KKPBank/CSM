package com.motiftech.icollection.dao.impl;

import java.util.List;

import org.apache.log4j.Logger;
import org.hibernate.Criteria;
import org.hibernate.SQLQuery;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Order;
import org.hibernate.criterion.Projections;
import org.hibernate.criterion.Restrictions;
import org.hibernate.transform.Transformers;

import com.icollection.common.constants.InquiryConstants;
import com.motiftech.icollection.bean.ContactBean;
import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.dao.AccountDao;
import com.motiftech.icollection.entity.Activity;
import com.motiftech.icollection.entity.Area;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.Campaignservice;
import com.motiftech.icollection.entity.Channel;
import com.motiftech.icollection.entity.ComplaintIssue;
import com.motiftech.icollection.entity.ComplaintRootCause;
import com.motiftech.icollection.entity.ComplaintSubject;
import com.motiftech.icollection.entity.ComplaintType;
import com.motiftech.icollection.entity.Customer;
import com.motiftech.icollection.entity.HREmployee;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.MasterAccount;
import com.motiftech.icollection.entity.Product;
import com.motiftech.icollection.entity.ProductGroup;
import com.motiftech.icollection.entity.SLA;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.SRLogging;
import com.motiftech.icollection.entity.SRStatus;
import com.motiftech.icollection.entity.State;
import com.motiftech.icollection.entity.Subarea;
import com.motiftech.icollection.entity.VW_SR;

public class AccountDaoImpl implements AccountDao {
	
	private SessionFactory sessionFactory;
	Logger log = Logger.getLogger(this.getClass());
	
	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}
	
	public Branch getBranchById(Integer branchId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Branch.class);
		criteria.add(Restrictions.eq("branchId", branchId));
		criteria.setMaxResults(1);
		return (Branch)criteria.uniqueResult();
	}
	
	public void updateSR(SR sr) {
		sessionFactory.getCurrentSession().update(sr);
		sessionFactory.getCurrentSession().flush();
		//TODO: check if the c
//		sessionFactory.getCurrentSession().disconnect();
	}
	
	public Long countSRFromView(){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(VW_SR.class);
		criteria.setProjection(Projections.rowCount());		
		return (Long)criteria.uniqueResult();
	}
	/**
	public Long countSRByAssignFlag(String assignFlag){
		Criteria criteria = getCriteriaListSR(assignFlag);
		criteria.setProjection(Projections.rowCount());		
		return (Long)criteria.uniqueResult();
	}
	
	public Criteria getCriteriaListSR(String assignFlag){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SR.class);
		criteria.add(Restrictions.not(Restrictions.in("srStatusId", new Integer[]{RuleConstants.stateMap.get(RuleConstants.STATUS.DRAFT)
																				,RuleConstants.stateMap.get(RuleConstants.STATUS.CLOSED)
																				,RuleConstants.stateMap.get(RuleConstants.STATUS.CANCELLED)})));
		if(assignFlag.equals(InquiryConstants.AssignFlag.UNASSIGN))
			criteria.add(Restrictions.or(Restrictions.eq("ruleAssignFlag",assignFlag), Restrictions.isNull("ruleAssignFlag")));
		else
			criteria.add(Restrictions.eq("ruleAssignFlag",assignFlag));
		return criteria;
	}
	
	@SuppressWarnings("unchecked")
	public List<SR> listSRByAssignFlag(int startIndex,int maxSize, String assignFlag){
		log.info("listSRByAssignFlag assignFlag : " + assignFlag);
		Criteria criteria = getCriteriaListSR(assignFlag);
		criteria.addOrder(Order.desc("srId"));	
		if(startIndex>=0 && maxSize>=0){
			criteria.setFirstResult(startIndex);
			criteria.setMaxResults(maxSize);
		}
		log.info("query size : " + criteria.list().size());
		return criteria.list();
	}
	/**/
	
	public Long countSRByAssignFlag(String assignFlag){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT count(SR.SR_ID) FROM TB_T_SR SR ");
		sql.append(getCriteriaListSR(assignFlag));
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("assignFlag",assignFlag);
		sqlQuery.setMaxResults(1);
		Long result = new Long((Integer)sqlQuery.uniqueResult());
		return result;
	}
	
	public String getCriteriaListSR(String assignFlag){
		StringBuilder sql = new StringBuilder();
		sql.append(" INNER JOIN TB_C_SR_STATUS status ON status.SR_STATUS_ID = SR.SR_STATUS_ID ");
		sql.append(" WHERE status.SR_STATUS_RULE = '1' ");
		sql.append(" AND ( SR.RULE_ASSIGN_FLAG = :assignFlag ");
		if(assignFlag.equals(InquiryConstants.AssignFlag.UNASSIGN))
			sql.append(" OR SR.RULE_ASSIGN_FLAG IS NULL ");
		sql.append(" ) ");
		return sql.toString();
	}
	
	@SuppressWarnings("unchecked")
	public List<SR> listSRByAssignFlag(int startIndex,int maxSize, String assignFlag){
		log.info("listSRByAssignFlag assignFlag : " + assignFlag);
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT SR.SR_ID as srId ");
		sql.append(" ,SR.SR_ANO  as srAno ");
		sql.append(" ,SR.CUSTOMER_ID  as customerId ");
		sql.append(" ,SR.ACCOUNT_ID  as accountId ");
		sql.append(" ,SR.CONTACT_ID  as contactId ");
		sql.append(" ,SR.CONTACT_ACCOUNT_NO  as contactAccountNo ");
		sql.append(" ,SR.CONTACT_RELATIONSHIP_ID  as contactRelationshipId ");
		sql.append(" ,SR.PRODUCTGROUP_ID  as productgroupId ");
		sql.append(" ,SR.PRODUCT_ID  as productId ");
		sql.append(" ,SR.CAMPAIGNSERVICE_ID  as campaignserviceId ");
		sql.append(" ,SR.AREA_ID  as areaId ");
		sql.append(" ,SR.SUBAREA_ID  as subareaId ");
		sql.append(" ,SR.TYPE_ID  as typeId ");
		sql.append(" ,SR.MAP_PRODUCT_ID  as mapProductId ");
		sql.append(" ,SR.CHANNEL_ID  as channelId ");
		sql.append(" ,SR.MEDIA_SOURCE_ID  as mediaSourceId ");
		sql.append(" ,cast(SR.SR_SUBJECT as varchar) as srSubject ");
		sql.append(" ,cast(SR.SR_REMARK as varchar)  as srRemark ");
		sql.append(" ,SR.OWNER_BRANCH_ID  as ownerBranchId ");
		sql.append(" ,SR.OWNER_USER_ID  as ownerUserId ");
		sql.append(" ,SR.DELEGATE_BRANCH_ID  as delegateBranchId ");
		sql.append(" ,SR.DELEGATE_USER_ID  as delegateUserId ");
		sql.append(" ,SR.OLD_OWNER_USER_ID  as oldOwnerUserId ");
		sql.append(" ,SR.OLD_DELEGATE_USER_ID  as oldDelegateUserId ");
		sql.append(" ,SR.SR_PAGE_ID  as srPageId ");
		sql.append(" ,SR.SR_IS_VERIFY  as srIsVerify ");
		sql.append(" ,SR.SR_IS_VERIFY_PASS  as srIsVerifyPass ");
		sql.append(" ,SR.SR_STATUS_ID  as srStatusId ");
		sql.append(" ,SR.OLD_SR_STATUS_ID  as oldSrStatusId ");
		sql.append(" ,SR.SR_DEF_ACCOUNT_ADDRESS_ID  as srDefAccountAddressId ");
		sql.append(" ,SR.SR_DEF_ADDRESS_HOUSE_NO  as srDefAddressHouseNo ");
		sql.append(" ,SR.SR_DEF_ADDRESS_VILLAGE  as srDefAddressVillage ");
		sql.append(" ,SR.SR_DEF_ADDRESS_BUILDING  as srDefAddressBuliding ");
		sql.append(" ,SR.SR_DEF_ADDRESS_FLOOR_NO  as srDefAddressFloorNo ");
		sql.append(" ,SR.SR_DEF_ADDRESS_ROOM_NO  as srDefAddressRoomNo ");
		sql.append(" ,SR.SR_DEF_ADDRESS_MOO  as srDefAddressMoo ");
		sql.append(" ,SR.SR_DEF_ADDRESS_SOI  as srDefAddressSoi ");
		sql.append(" ,SR.SR_DEF_ADDRESS_STREET  as srDefAddressStreet ");
		sql.append(" ,SR.SR_DEF_ADDRESS_TAMBOL  as srDefAddressTambol ");
		sql.append(" ,SR.SR_DEF_ADDRESS_AMPHUR  as srDefAddressAmphur ");
		sql.append(" ,SR.SR_DEF_ADDRESS_PROVINCE  as srDefAddressProvince ");
		sql.append(" ,SR.SR_DEF_ADDRESS_ZIPCODE  as srDefAddressZipcode ");
		sql.append(" ,SR.SR_AFS_ASSET_ID  as srAfsAssetId ");
		sql.append(" ,SR.SR_AFS_ASSET_NO  as srAfsAssetNo ");
		sql.append(" ,SR.SR_AFS_ASSET_DESC  as srAfsAssetDesc ");
		sql.append(" ,SR.SR_NCB_CUSTOMER_BIRTHDATE  as srNcbCustomerBirthdate ");
		sql.append(" ,SR.SR_NCB_CHECK_STATUS  as srNcbCheckStatus ");
		sql.append(" ,SR.SR_NCB_MARKETING_USER_ID  as srNcbMargetingUserId ");
		sql.append(" ,SR.SR_NCB_MARKETING_FULL_NAME  as srNcbMargetingFullName ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_ID  as srNcbMargetingBranchId ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_NAME  as srNcbMargetingBranchName ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_UPPER_1_ID  as srNcbMargetingBranchUpper1Id ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_UPPER_1_NAME  as srNcbMargetingBranchUpper1Name ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_UPPER_2_ID  as srNcbMargetingBranchUpper2Id ");
		sql.append(" ,SR.SR_NCB_MARKETING_BRANCH_UPPER_2_NAME  as srNcbMargetingBranchUpper2Name ");
		sql.append(" ,SR.CREATE_BRANCH_ID  as createBranchId ");
		sql.append(" ,SR.CREATE_USER  as createUser ");
		sql.append(" ,SR.UPDATE_USER  as updateUser ");
		sql.append(" ,SR.CREATE_DATE  as createDate ");
		sql.append(" ,SR.UPDATE_DATE  as updateDate ");
		sql.append(" ,SR.CLOSE_DATE  as closeDate ");
		sql.append(" ,SR.UPDATE_DATE_BY_OWNER  as updateDateByOwner ");
		sql.append(" ,SR.UPDATE_DATE_BY_DELEGATE  as updateDateByDelegate ");
		sql.append(" ,SR.RULE_DELEGATE_FLAG  as ruleDelegateFlag ");
		sql.append(" ,SR.RULE_DELEGATE_BRANCH_ID  as ruleDelegateBranchId ");
		sql.append(" ,SR.RULE_THIS_ALERT  as ruleThisAlert ");
		sql.append(" ,SR.RULE_TOTAL_ALERT  as ruleTotalAlert ");
		sql.append(" ,SR.RULE_THIS_WORK  as ruleThisWork ");
		sql.append(" ,SR.RULE_TOTAL_WORK  as ruleTotalWork ");
		sql.append(" ,SR.RULE_NEXT_SLA  as ruleNextSla ");
		sql.append(" ,SR.RULE_ASSIGN_FLAG  as ruleAssignFlag ");
		sql.append(" ,SR.RULE_EMAIL_FLAG  as ruleEmailFlag ");
		sql.append(" ,SR.RULE_ASSIGN_DATE  as ruleAssignDate ");
		sql.append(" ,SR.RULE_STATUS_DATE  as ruleStatusDate ");
		sql.append(" ,SR.RULE_DELEGATE_DATE  as ruleDelegateDate ");
		sql.append(" ,SR.RULE_CURRENT_SLA  as ruleCurrentSla ");
		sql.append(" ,SR.DRAFT_SR_EMAIL_TEMPLATE_ID  as draftSrEmailTemplateId ");
		sql.append(" ,SR.DRAFT_MAIL_SENDER  as draftMailSender ");
		sql.append(" ,SR.DRAFT_MAIL_TO  as draftMailTo ");
		sql.append(" ,SR.DRAFT_MAIL_CC  as draftMailCC ");
		sql.append(" ,cast(SR.DRAFT_MAIL_SUBJECT as varchar) as draftMailSubject ");
		sql.append(" ,cast(SR.DRAFT_MAIL_BODY as varchar)   as draftMailBody ");
		sql.append(" ,SR.DRAFT_ACTIVITY_TYPE_ID  as draftActivityTypeId ");
		sql.append(" ,cast(SR.DRAFT_ACTIVITY_DESC as varchar)  as draftActivityDesc ");
		sql.append(" ,SR.DRAFT_ACCOUNT_ADDRESS_TEXT  as draftAccountAddressText ");
		sql.append(" ,SR.DRAFT_IS_SEND_EMAIL_FOR_DELEGATE  as draftIsSendEmailForDelegate ");
		sql.append(" ,SR.DRAFT_IS_CLOSE  as draftIsClose ");
		sql.append(" ,SR.DRAFT_ATTACHMENT_JSON  as draftAttachmentJson ");
		sql.append(" ,SR.DRAFT_VERIFY_ANSWER_JSON  as draftVerifyAnswerJson ");
		sql.append(" ,SR.EXPORT_DATE  as exportDate ");
		sql.append(" ,SR.CPN_PRODUCT_GROUP_ID  as cpnProductGroupId ");
		sql.append(" ,SR.CPN_PRODUCT_ID  as cpnProductId ");
		sql.append(" ,SR.CPN_CAMPAIGNSERVICE_ID  as cpnCampaignserviceId ");
		sql.append(" ,SR.CPN_SUBJECT_ID  as cpnSubjectId ");
		sql.append(" ,SR.CPN_TYPE_ID  as cpnTypeId ");
		sql.append(" ,SR.CPN_ROOT_CAUSE_ID  as cpnRootCauseId ");
		sql.append(" ,SR.CPN_ISSUES_ID  as cpnIssueId ");
		sql.append(" ,SR.CPN_MAPPING_ID  as cpnMappingId ");
		sql.append(" ,SR.CPN_BU_GROUP_ID  as cpnBuGroupId ");
		sql.append(" ,SR.CPN_CAUSE_SUMMARY_ID  as cpnCauseSummaryId ");
		sql.append(" ,SR.CPN_SUMMARY_ID  as cpnSummaryId ");
		sql.append(" ,SR.CPN_SECRET  as cpnSecret ");
		sql.append(" ,SR.CPN_CAR  as cpnCar ");
		sql.append(" ,SR.CPN_HPLog100  as cpnHPLog100 ");
		sql.append(" ,SR.CPN_SUMMARY  as cpnSummary ");
		sql.append(" ,SR.CPN_CAUSE_CUSTOMER  as cpnCauseCustomer ");
		sql.append(" ,SR.CPN_CAUSE_STAFF  as cpnCauseStaff ");
		sql.append(" ,SR.CPN_CAUSE_SYSTEM  as cpnCauseSystem ");
		sql.append(" ,SR.CPN_CAUSE_PROCESS  as cpnCauseProcess ");
		sql.append(" ,cast(SR.CPN_CAUSE_CUSTOMER_DETAIL as varchar)  as cpnCauseCustomerDetail ");
		sql.append(" ,cast(SR.CPN_CAUSE_STAFF_DETAIL as varchar)  as cpnCauseStaffDetail ");
		sql.append(" ,cast(SR.CPN_CAUSE_SYSTEM_DETAIL as varchar)  as cpnCauseSystemDetail ");
		sql.append(" ,cast(SR.CPN_CAUSE_PROCESS_DETAIL as varchar)  as cpnCauseProcessDetail ");
		sql.append(" ,cast(SR.CPN_FIXED_DETAIL as varchar)  as cpnFixedDetail ");
		sql.append(" ,SR.DRAFT_MAIL_BCC  as draftMailBCC ");
		sql.append(" ,cast(SR.CLOSE_USER as varchar)  as closeUser ");
		sql.append(" ,cast(SR.CLOSE_USERNAME as varchar)  as closeUsername ");
		sql.append(" ,cast(SR.CPN_BU1_CODE as varchar)  as cpnBU1Code ");
		sql.append(" ,cast(SR.CPN_BU2_CODE as varchar)  as cpnBU2Code ");
		sql.append(" ,cast(SR.CPN_BU3_CODE as varchar)  as cpnBU3Code ");
		sql.append(" ,SR.CPN_MSHBranchId  as cpnMSHBranchId ");
		sql.append(" FROM TB_T_SR SR ");
		sql.append(getCriteriaListSR(assignFlag));
		sql.append(" ORDER BY SR.SR_ID desc ");
		
//		log.debug("sql : " + sql.toString());
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("assignFlag",assignFlag);
		if(startIndex>=0 && maxSize>=0){
			sqlQuery.setFirstResult(startIndex);
			sqlQuery.setFetchSize(maxSize);
		}
		sqlQuery.setResultTransformer(Transformers.aliasToBean(SR.class));
		log.info("query size : " + sqlQuery.list().size());
//		sessionFactory.evictQueries();
//		sqlQuery.setFlushMode(FlushMode.NEVER);
		return sqlQuery.list();
	}
	
	@SuppressWarnings("unchecked")
	public List<VW_SR> listSRFromView(int startIndex,int maxSize){
		log.info("listSRFromView assignFlag");
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(VW_SR.class);
		criteria.addOrder(Order.desc("srId"));	
		if(startIndex>=0 && maxSize>=0){
			criteria.setFirstResult(startIndex);
			criteria.setMaxResults(maxSize);
		}
		log.info("query size : " + criteria.list().size());
		return criteria.list();
	}
	/**/
	public SR getConsolidate(Integer customerId) {
		/**
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SR.class);
		criteria.add(Restrictions.eq("customerId",customerId));
		criteria.add(Restrictions.isNotNull("ownerUserId"));
		criteria.add(Restrictions.not(Restrictions.in("srStatusId", new Integer[]{RuleConstants.stateMap.get(RuleConstants.STATUS.DRAFT)
																				,RuleConstants.stateMap.get(RuleConstants.STATUS.CANCELLED)
																				,RuleConstants.stateMap.get(RuleConstants.STATUS.CLOSED)})));
		criteria.addOrder(Order.desc("updateDate"));
		criteria.setMaxResults(1);
		return (SR)criteria.uniqueResult();
		/**/
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT SR.OWNER_USER_ID as ownerUserId ");
		sql.append(" ,SR.DELEGATE_USER_ID as delegateUserId ");
		sql.append(" FROM TB_T_SR SR ");
		sql.append(" INNER JOIN TB_C_SR_STATUS status ON status.SR_STATUS_ID = SR.SR_STATUS_ID ");
		sql.append(" WHERE status.SR_STATUS_RULE = '1' ");
		sql.append(" AND SR.CUSTOMER_ID = :customerId ");
		sql.append(" AND SR.OWNER_USER_ID IS NOT NULL ");
		sql.append(" ORDER BY SR.UPDATE_DATE desc ");
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("customerId",customerId);
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(SR.class));
		return (SR)sqlQuery.uniqueResult();
	}
	
	public MapProduct getMapProduct(KeysBean keysBean) {
		log.debug("getMapProduct : " + keysBean.toString());
		StringBuilder sql = new StringBuilder();
		sql.append(mapProductKey(keysBean));
		sql.append(" AND mapProduct.CAMPAIGNSERVICE_ID = :campaignserviceId  ");
		sql.append(" AND mapProduct.SUBAREA_ID = :subareaId ");
		sql.append(" ORDER BY mapProduct.UPDATE_DATE DESC");
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("campaignserviceId",keysBean.getCampaignserviceId());
		sqlQuery.setParameter("subareaId",keysBean.getSubareaId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(MapProduct.class));
		MapProduct mapProduct = (MapProduct)sqlQuery.uniqueResult();
		if(mapProduct != null)return mapProduct;
		
		log.debug("find case CampaignserviceId is null in mapProduct");
		//case CampaignserviceId is null in mapProduct
		sql = new StringBuilder();
		sql.append(mapProductKey(keysBean));
		sql.append(" AND mapProduct.SUBAREA_ID = :subareaId ");
		sql.append(" ORDER BY mapProduct.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("subareaId",keysBean.getSubareaId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(MapProduct.class));
		mapProduct = (MapProduct)sqlQuery.uniqueResult();
		if(mapProduct != null)return mapProduct;

		log.debug("find case SubareaId is null in mapProduct");
		//case SubareaId is null in mapProduct
		sql = new StringBuilder();
		sql.append(mapProductKey(keysBean));
		sql.append(" AND mapProduct.CAMPAIGNSERVICE_ID = :campaignserviceId  ");
		sql.append(" ORDER BY mapProduct.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("campaignserviceId",keysBean.getCampaignserviceId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(MapProduct.class));
		mapProduct = (MapProduct)sqlQuery.uniqueResult();
		if(mapProduct != null)return mapProduct;

		log.debug("find case CampaignserviceId and SubareaId are null in mapProduct");
		//case CampaignserviceId and SubareaId are null in mapProduct
		sql = new StringBuilder();
		sql.append(mapProductKey(keysBean));
		sql.append(" ORDER BY mapProduct.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(MapProduct.class));
		mapProduct = (MapProduct)sqlQuery.uniqueResult();
		return mapProduct;
	}
	
	private String mapProductKey(KeysBean keysBean){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ");
		sql.append(" mapProduct.MAP_PRODUCT_ID AS mapProductId");
		sql.append(" ,mapProduct.DEFAULT_OWNER_USER_ID AS defaultOwnerUserId");
		sql.append(" FROM TB_M_MAP_PRODUCT mapProduct ");
		sql.append(" INNER JOIN TB_R_PRODUCT product ON product.PRODUCT_ID = mapProduct.PRODUCT_ID ");
		sql.append(" INNER JOIN TB_R_PRODUCTGROUP productGroup ON productGroup.PRODUCTGROUP_ID = product.PRODUCTGROUP_ID ");
		sql.append(" WHERE mapProduct.PRODUCT_ID = :productId ");
		sql.append(" AND product.PRODUCTGROUP_ID = :productGroupId ");
		sql.append(" AND mapProduct.TYPE_ID = :typeId ");
		sql.append(" AND mapProduct.AREA_ID = :areaId ");
		sql.append(" AND mapProduct.DEFAULT_OWNER_USER_ID IS NOT NULL ");
		return sql.toString();
	}
	
	public AutoForward autoForward(KeysBean keysBean, Integer channelId) {
		StringBuilder sql = new StringBuilder();
		sql.append(autoForwardKey());
		sql.append(" AND autoForward.CAMPAIGNSERVICE_ID = :campaignserviceId ");
		sql.append(" AND autoForward.SUBAREA_ID = :subareaId ");
		sql.append(" ORDER BY autoForward.UPDATE_DATE DESC");
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("channelId",channelId);
		sqlQuery.setParameter("isActive",RuleConstants.IS_ACTIVE);
		sqlQuery.setParameter("campaignserviceId",keysBean.getCampaignserviceId());
		sqlQuery.setParameter("subareaId",keysBean.getSubareaId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(AutoForward.class));
		AutoForward autoForward = (AutoForward)sqlQuery.uniqueResult();
		if(autoForward != null)return autoForward;
		
		//case CampaignserviceId is null in autoForward
		sql = new StringBuilder();
		sql.append(autoForwardKey());
		sql.append(" AND autoForward.SUBAREA_ID = :subareaId ");
		sql.append(" ORDER BY autoForward.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("channelId",channelId);
		sqlQuery.setParameter("isActive",RuleConstants.IS_ACTIVE);
		sqlQuery.setParameter("subareaId",keysBean.getSubareaId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(AutoForward.class));
		autoForward = (AutoForward)sqlQuery.uniqueResult();
		if(autoForward != null)return autoForward;
		
		//case SubareaId is null in autoForward
		sql = new StringBuilder();
		sql.append(autoForwardKey());
		sql.append(" AND autoForward.CAMPAIGNSERVICE_ID = :campaignserviceId ");
		sql.append(" ORDER BY autoForward.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("channelId",channelId);
		sqlQuery.setParameter("isActive",RuleConstants.IS_ACTIVE);
		sqlQuery.setParameter("campaignserviceId",keysBean.getCampaignserviceId());
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(AutoForward.class));
		autoForward = (AutoForward)sqlQuery.uniqueResult();
		if(autoForward != null)return autoForward;
		
		//case CampaignserviceId and SubareaId are null in autoForward
		sql = new StringBuilder();
		sql.append(autoForwardKey());
		sql.append(" ORDER BY autoForward.UPDATE_DATE DESC");
		sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",keysBean.getProductId());
		sqlQuery.setParameter("productGroupId",keysBean.getProductGroupId());
		sqlQuery.setParameter("typeId",keysBean.getTypeId());
		sqlQuery.setParameter("areaId",keysBean.getAreaId());
		sqlQuery.setParameter("channelId",channelId);
		sqlQuery.setParameter("isActive",RuleConstants.IS_ACTIVE);
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(AutoForward.class));
		autoForward = (AutoForward)sqlQuery.uniqueResult();
		return autoForward;
	}
	
	private String autoForwardKey(){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ");
		sql.append(" autoForward.AUTO_FORWARD_ID AS autoForwardId");
		sql.append(" ,autoForward.FORWARD_TO_USER AS forwardToUser");
		sql.append(" FROM TB_M_MAP_AUTO_FORWARD autoForward ");
		sql.append(" INNER JOIN TB_R_PRODUCT product ON product.PRODUCT_ID = autoForward.PRODUCT_ID ");
		sql.append(" INNER JOIN TB_R_PRODUCTGROUP productGroup ON productGroup.PRODUCTGROUP_ID = product.PRODUCTGROUP_ID ");
		sql.append(" WHERE autoForward.PRODUCT_ID = :productId ");
		sql.append(" AND product.PRODUCTGROUP_ID = :productGroupId ");
		sql.append(" AND autoForward.TYPE_ID = :typeId ");
		sql.append(" AND autoForward.AREA_ID = :areaId ");
		sql.append(" AND autoForward.CHANNEL_ID = :channelId ");
		sql.append(" AND autoForward.FORWARD_TO_USER IS NOT NULL ");
		sql.append(" AND autoForward.IS_ACTIVE = :isActive");
		return sql.toString();
	}
	
	public SR getSR(Integer srId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SR.class);
		criteria.add(Restrictions.eq("srId", srId));
		criteria.setMaxResults(1);
		return (SR)criteria.uniqueResult();
	}
	
	public Customer getCustomer(Integer customerId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Customer.class);
		criteria.add(Restrictions.eq("customerId", customerId));
		criteria.setMaxResults(1);
		return (Customer)criteria.uniqueResult();
	}
	
	public Campaignservice getCampaignservice(Integer campaignserviceId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Campaignservice.class);
		criteria.add(Restrictions.eq("campaignserviceId", campaignserviceId));
		criteria.setMaxResults(1);
		return (Campaignservice)criteria.uniqueResult();
	}
	
	public Activity getActivity(Integer srId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Activity.class);
		criteria.add(Restrictions.eq("srId", srId));
		criteria.addOrder(Order.desc("srActivityId"));
		criteria.setMaxResults(1);
		return (Activity)criteria.uniqueResult();
	}
	
	public SRLogging getSRLogging(Integer srId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SRLogging.class);
		criteria.add(Restrictions.eq("srId", srId));
		criteria.addOrder(Order.desc("srLoggingId"));
		criteria.setMaxResults(1);
		return (SRLogging)criteria.uniqueResult();
	}
	
	/**
	@SuppressWarnings("unchecked")
	public List<SLA> listAllSLA(){
		log.debug("listAllSLA");
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.addOrder(Order.asc("typeId"));
		log.debug("query size : " + criteria.list().size());
		return criteria.list();
	}
	
	public SLA getSLA(){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.setMaxResults(1);
		return (SLA)criteria.uniqueResult();
	}
	/**/
	public SLA getSLA(Integer productId, Integer channelId, Integer srStatusId, Integer areaId, Integer typeId, Integer campaignserviceId, Integer subareaId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("productId", productId));
		criteria.add(Restrictions.eq("channelId", channelId));
		criteria.add(Restrictions.eq("srStatusId", srStatusId));
		criteria.add(Restrictions.eq("areaId", areaId));
		criteria.add(Restrictions.eq("typeId", typeId));
		criteria.add(Restrictions.eq("campaignserviceId", campaignserviceId));
		criteria.add(Restrictions.eq("subareaId", subareaId));
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.addOrder(Order.asc("typeId"));
		criteria.setMaxResults(1);
		
		SLA sla = (SLA)criteria.uniqueResult();
		if(sla != null)return sla; 
		
		//campaignserviceId is null
		criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("productId", productId));
		criteria.add(Restrictions.eq("channelId", channelId));
		criteria.add(Restrictions.eq("srStatusId", srStatusId));
		criteria.add(Restrictions.eq("areaId", areaId));
		criteria.add(Restrictions.eq("typeId", typeId));
		criteria.add(Restrictions.eq("campaignserviceId", null));
		criteria.add(Restrictions.eq("subareaId", subareaId));
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.addOrder(Order.asc("typeId"));
		criteria.setMaxResults(1);
		
		sla = (SLA)criteria.uniqueResult();
		if(sla != null)return sla;
		
		//subareaId is null
		criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("productId", productId));
		criteria.add(Restrictions.eq("channelId", channelId));
		criteria.add(Restrictions.eq("srStatusId", srStatusId));
		criteria.add(Restrictions.eq("areaId", areaId));
		criteria.add(Restrictions.eq("typeId", typeId));
		criteria.add(Restrictions.eq("campaignserviceId", campaignserviceId));
		criteria.add(Restrictions.eq("subareaId", null));
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.addOrder(Order.asc("typeId"));
		criteria.setMaxResults(1);
		
		sla = (SLA)criteria.uniqueResult();
		if(sla != null)return sla;
		
		//campaignserviceId and subareaId are null
		criteria = sessionFactory.getCurrentSession().createCriteria(SLA.class);
		criteria.add(Restrictions.eq("productId", productId));
		criteria.add(Restrictions.eq("channelId", channelId));
		criteria.add(Restrictions.eq("srStatusId", srStatusId));
		criteria.add(Restrictions.eq("areaId", areaId));
		criteria.add(Restrictions.eq("typeId", typeId));
		criteria.add(Restrictions.eq("campaignserviceId", null));
		criteria.add(Restrictions.eq("subareaId", null));
		criteria.add(Restrictions.eq("slaIsActive", Boolean.TRUE));
		criteria.addOrder(Order.asc("typeId"));
		criteria.setMaxResults(1);
		
		sla = (SLA)criteria.uniqueResult();
		return sla;
	}
	
	public MasterAccount getMasterAccount(Integer accountId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(MasterAccount.class);
		criteria.add(Restrictions.eq("accountId", accountId));
		criteria.setMaxResults(1);
		return (MasterAccount)criteria.uniqueResult();
	}
	
	public ProductGroup getProductGroup(Integer productGroupId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(ProductGroup.class);
		criteria.add(Restrictions.eq("productGroupId", productGroupId));
		criteria.setMaxResults(1);
		return (ProductGroup)criteria.uniqueResult();
	}
	
	public Product getProduct(Integer productId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Product.class);
		criteria.add(Restrictions.eq("productId", productId));
		criteria.setMaxResults(1);
		return (Product)criteria.uniqueResult();
	}
	
	public Area getArea(Integer areaId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Area.class);
		criteria.add(Restrictions.eq("areaId", areaId));
		criteria.setMaxResults(1);
		return (Area)criteria.uniqueResult();
	}
	
	public Subarea getSubarea(Integer subareaId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Subarea.class);
		criteria.add(Restrictions.eq("subareaId", subareaId));
		criteria.setMaxResults(1);
		return (Subarea)criteria.uniqueResult();
	}
	
	public Channel getChannel(Integer channelId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Channel.class);
		criteria.add(Restrictions.eq("channelId", channelId));
		criteria.setMaxResults(1);
		return (Channel)criteria.uniqueResult();
	}
	
	public SRStatus getSRStatus(Integer srStatusId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SRStatus.class);
		criteria.add(Restrictions.eq("srStatusId", srStatusId));
		criteria.setMaxResults(1);
		return (SRStatus)criteria.uniqueResult();
	}
	
	public HREmployee getHREmployee(String employeeCode){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(HREmployee.class);
		criteria.add(Restrictions.eq("employeeId", employeeCode));
		criteria.setMaxResults(1);
		return (HREmployee)criteria.uniqueResult();
	}
	
	public State getState(Integer srId){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT state.SR_STATE_ID as srStateId ");
		sql.append(" ,state.SR_STATE_NAME as srStateName ");
		sql.append(" FROM TB_C_SR_STATE state ");
		sql.append(" INNER JOIN TB_C_SR_STATUS status ON status.SR_STATE_ID = state.SR_STATE_ID ");
		sql.append(" INNER JOIN TB_T_SR sr ON sr.SR_STATUS_ID = status.SR_STATUS_ID ");
		sql.append(" WHERE sr.SR_ID = :srId ");
		
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("srId", srId);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(State.class));
		State state = (State)sqlQuery.uniqueResult();
		return state;
	}
	
	public ContactBean getContact(Integer contactId){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT contact.FIRST_NAME_TH + ' ' + contact.LAST_NAME_TH as name ");
		sql.append(" ,contactPhone.PHONE_NO as phoneNo ");
		sql.append(" FROM TB_M_CONTACT contact ");
		sql.append(" INNER JOIN TB_M_CONTACT_PHONE contactPhone ON contactPhone.CONTACT_ID = contact.CONTACT_ID ");
		sql.append(" INNER JOIN TB_M_PHONE_TYPE phoneType ON phoneType.PHONE_TYPE_ID = contactPhone.PHONE_TYPE_ID ");
		sql.append(" WHERE contact.CONTACT_ID = :contactId ");
		sql.append(" AND phoneType.PHONE_TYPE_CODE = '02' ");//02 : Mobile phone
		
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("contactId", contactId);
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(ContactBean.class));
		return (ContactBean) sqlQuery.uniqueResult();
	}
	
	public ContactBean getCustomerPhone(Integer customerId){
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ' ' as name ");
		sql.append(" ,phone.PHONE_NO as phoneNo ");
		sql.append(" FROM TB_M_PHONE phone ");
		sql.append(" INNER JOIN TB_M_PHONE_TYPE phoneType ON phoneType.PHONE_TYPE_ID = phone.PHONE_TYPE_ID ");
		sql.append(" WHERE phone.CUSTOMER_ID = :customerId ");
		sql.append(" AND phoneType.PHONE_TYPE_CODE = '02' ");//02 : Mobile phone
		
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("customerId", customerId);
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(ContactBean.class));
		return (ContactBean) sqlQuery.uniqueResult();
	}
	
	public ComplaintIssue getComplaintIssue(Integer complaintIssueId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(ComplaintIssue.class);
		criteria.add(Restrictions.eq("complaintIssueId", complaintIssueId));
		criteria.setMaxResults(1);
		return (ComplaintIssue)criteria.uniqueResult();
	}
	
	public ComplaintType getComplaintType(Integer complaintTypeId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(ComplaintType.class);
		criteria.add(Restrictions.eq("complaintTypeId", complaintTypeId));
		criteria.setMaxResults(1);
		return (ComplaintType)criteria.uniqueResult();
	}
	
	public ComplaintSubject getComplaintSubject(Integer complaintSubjectId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(ComplaintSubject.class);
		criteria.add(Restrictions.eq("complaintSubjectId", complaintSubjectId));
		criteria.setMaxResults(1);
		return (ComplaintSubject)criteria.uniqueResult();
	}
	
	public ComplaintRootCause getComplaintRootCause(Integer rootCauseId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(ComplaintRootCause.class);
		criteria.add(Restrictions.eq("rootCauseId", rootCauseId));
		criteria.setMaxResults(1);
		return (ComplaintRootCause)criteria.uniqueResult();
	}
	
	public void saveSRLogging(SRLogging srLogging){
		sessionFactory.getCurrentSession().save(srLogging);
	}
	
	public void saveActivity(Activity activity){
		sessionFactory.getCurrentSession().save(activity);
	}
}
