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
import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.dao.AccountDao;
import com.motiftech.icollection.entity.Activity;
import com.motiftech.icollection.entity.Area;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.Campaignservice;
import com.motiftech.icollection.entity.Channel;
import com.motiftech.icollection.entity.Contact;
import com.motiftech.icollection.entity.Customer;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.MasterAccount;
import com.motiftech.icollection.entity.Product;
import com.motiftech.icollection.entity.ProductGroup;
import com.motiftech.icollection.entity.SLA;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.SRLogging;
import com.motiftech.icollection.entity.SRStatus;
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
	
	public Long countSRByAssignFlag(String assignFlag){
		Criteria criteria = getCriteriaListSR(assignFlag);
		criteria.setProjection(Projections.rowCount());		
		return (Long)criteria.uniqueResult();
	}
	
	public Long countSRFromView(){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(VW_SR.class);
		criteria.setProjection(Projections.rowCount());		
		return (Long)criteria.uniqueResult();
	}
	
	public Criteria getCriteriaListSR(String assignFlag){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SR.class);
		criteria.add(Restrictions.not(Restrictions.in("srStatusId", new Integer[]{RuleConstants.getStatemap().get(RuleConstants.STATUS.DRAFT)
																				,RuleConstants.getStatemap().get(RuleConstants.STATUS.CLOSED)
																				,RuleConstants.getStatemap().get(RuleConstants.STATUS.CANCELLED)})));
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

	public SR getConsolidate(Integer customerId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(SR.class);
		criteria.add(Restrictions.eq("customerId",customerId));
		criteria.add(Restrictions.isNotNull("ownerUserId"));
		criteria.add(Restrictions.not(Restrictions.in("srStatusId", new Integer[]{RuleConstants.getStatemap().get(RuleConstants.STATUS.DRAFT)
																				,RuleConstants.getStatemap().get(RuleConstants.STATUS.CANCELLED)
																				,RuleConstants.getStatemap().get(RuleConstants.STATUS.CLOSED)})));
		criteria.addOrder(Order.desc("updateDate"));
		criteria.setMaxResults(1);
		return (SR)criteria.uniqueResult();
	}
	
	/*public Integer getProductGroupId(Integer productId){
		if(productId == null)return null;
		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ");
		sql.append(" productGroup.PRODUCTGROUP_ID AS productGroupId ");
		sql.append(" ,productGroup.PRODUCTGROUP_CODE AS productGroupCode ");
		sql.append(" FROM TB_R_PRODUCTGROUP productGroup ");
		sql.append(" INNER JOIN TB_R_PRODUCT product ON product.PRODUCTGROUP_ID = productGroup.PRODUCTGROUP_ID ");
		sql.append(" WHERE product.PRODUCT_ID = :productId ");
		
		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("productId",productId);
		sqlQuery.setMaxResults(1);
		sqlQuery.setResultTransformer(Transformers.aliasToBean(ProductGroup.class));
		ProductGroup productGroup = (ProductGroup)sqlQuery.uniqueResult();
		if(productGroup != null)return productGroup.getProductGroupId();
		return null;
	}*/
	
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
	
	/*
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
	*/
	
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
	
	public void saveSRLogging(SRLogging srLogging){
		sessionFactory.getCurrentSession().save(srLogging);
	}
	
	public void saveActivity(Activity activity){
		sessionFactory.getCurrentSession().save(activity);
	}

	public Contact getContact(Integer contactId) {
		log.debug(String.format("I:--START--:--Get Contact--:contactId/%s", contactId));

		try {

			Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Contact.class);
			criteria.add(Restrictions.eq("contactId", contactId));
			criteria.setMaxResults(1);

			Contact contact = (Contact) criteria.uniqueResult();
			log.debug("O:--SUCCESS--:--Get Contact--");
			return contact;
		} catch (Exception ex) {
			ex.printStackTrace();
			log.error("Exception occur:\n", ex);
		}

		return null;
	}

	@SuppressWarnings("unchecked")
	public List<String> getCustomerPhoneList(Integer customerId) {
		log.debug(String.format("I:--START--:--Get Customer Phone List--:CustomerId/%s", customerId));

		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ph.PHONE_NO FROM TB_M_PHONE ph ");
		sql.append(" INNER JOIN TB_M_CUSTOMER cs ON cs.CUSTOMER_ID = ph.CUSTOMER_ID ");
		sql.append(" INNER JOIN TB_M_PHONE_TYPE pt ON pt.PHONE_TYPE_ID = ph.PHONE_TYPE_ID ");
		sql.append(" WHERE 1=1 ");
		sql.append(" AND ph.CUSTOMER_ID = :customerId ");
		// sql.append(" AND pt.PHONE_TYPE_CODE != '05' ");
		sql.append(" ORDER BY pt.PHONE_TYPE_CODE ASC ");

		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("customerId", customerId);
		List<String> results = sqlQuery.list();

		log.debug(String.format("O:--SUCCESS--:--Get Customer Phone List--:ListSize/%s:PhoneNo/", results.size(),
				results != null && results.size() > 0 ? results.get(0) : ""));
		return results;
	}

	@SuppressWarnings("unchecked")
	public List<String> getContactPhoneList(Integer contactId) {
		log.debug(String.format("I:--START--:--Get Contact Phone List--:ContactId/%s", contactId));

		StringBuilder sql = new StringBuilder();
		sql.append(" SELECT ph.PHONE_NO FROM TB_M_CONTACT_PHONE ph ");
		sql.append(" INNER JOIN TB_M_CONTACT cs ON cs.CONTACT_ID = ph.CONTACT_ID ");
		sql.append(" INNER JOIN TB_M_PHONE_TYPE pt ON pt.PHONE_TYPE_ID = ph.PHONE_TYPE_ID ");
		sql.append(" WHERE 1=1 ");
		sql.append(" AND ph.CONTACT_ID = :contactId ");
		// sql.append(" AND pt.PHONE_TYPE_CODE != '05' ");
		sql.append(" ORDER BY pt.PHONE_TYPE_CODE ASC ");

		SQLQuery sqlQuery = sessionFactory.getCurrentSession().createSQLQuery(sql.toString());
		sqlQuery.setParameter("contactId", contactId);
		List<String> results = sqlQuery.list();

		log.debug(String.format("O:--SUCCESS--:--Get Contact Phone List--:ListSize/%s:PhoneNo/%s", results.size(),
				results != null && results.size() > 0 ? results.get(0) : ""));
		return results;
	}
}
