package com.motiftech.icollection.entity;

import java.io.Serializable;
import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

@Entity
@Table(name = "VW_TB_T_SR")
public class VW_SR implements Serializable {

	private static final long serialVersionUID = 415134632363303678L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_ID", nullable = false, insertable = true, updatable = true)
	private Integer srId;

//	@Column(name = "SR_NO", nullable = true, columnDefinition = "varchar(50)")
//	private String srNo;
//
//	@Column(name = "SR_CALL_ID", nullable = true, columnDefinition = "varchar(50)")
//	private String srCallId;
//	
//	@Column(name = "SR_ANO", nullable = true, columnDefinition = "varchar(50)")
//	private String srAno;
//	
	@Column(name = "CUSTOMER_ID", nullable = true)
	private Integer customerId;

//	@Column(name = "ACCOUNT_ID", nullable = true)
//	private Integer accountId;
//
//	@Column(name = "CONTACT_ID", nullable = true)
//	private Integer contactId;
//
//	@Column(name = "CONTACT_ACCOUNT_NO", nullable = true, columnDefinition = "varchar(255)")
//	private String contactAccountNo;
//	
//	@Column(name = "CONTACT_RELATIONSHIP_ID", nullable = true)
//	private Integer contactRelationshipId;
//	
	@Column(name = "PRODUCTGROUP_ID", nullable = true)
	private Integer productgroupId;
	
	@Column(name = "PRODUCT_ID", nullable = true)
	private Integer productId;
	
	@Column(name = "CAMPAIGNSERVICE_ID", nullable = true)
	private Integer campaignserviceId;
	
	@Column(name = "AREA_ID", nullable = true)
	private Integer areaId;

	@Column(name = "SUBAREA_ID", nullable = true)
	private Integer subareaId;

	@Column(name = "TYPE_ID", nullable = true)
	private Integer typeId;

//	@Column(name = "MAP_PRODUCT_ID", nullable = true)
//	private Integer mapProductId;
//	
	@Column(name = "CHANNEL_ID", nullable = true)
	private Integer channelId;

//	@Column(name = "MEDIA_SOURCE_ID", nullable = true)
//	private Integer mediaSourceId;
//	
//	@Column(name = "SR_SUBJECT", nullable = true, columnDefinition = "varchar(8000)")
//	private String srSubject;
//
//	@Column(name = "SR_REMARK", nullable = true, columnDefinition = "varchar(8000)")
//	private String srRamark;
//	
	@Column(name = "OWNER_BRANCH_ID", nullable = true)
	private Integer ownerBranchId;

	@Column(name = "OWNER_USER_ID", nullable = true)
	private Integer ownerUserId;
	
//	@Column(name = "DELEGATE_BRANCH_ID", nullable = true)
//	private Integer delegateBranchId;
//
	@Column(name = "DELEGATE_USER_ID", nullable = true)
	private Integer delegateUserId;

//	@Column(name = "OLD_OWNER_USER_ID", nullable = true)
//	private Integer oldOwnerUserId;
//	
//	@Column(name = "OLD_DELEGATE_USER_ID", nullable = true)
//	private Integer oldDelegateUserId;
//
//	@Column(name = "SR_PAGE_ID", nullable = true)
//	private Integer srPageId;
//
//	@Column(name = "SR_IS_VERIFY", nullable = true, columnDefinition = "numeric(1,0)")
//	private Boolean srIsVerify;
//
//	@Column(name = "SR_IS_VERIFY_PASS", nullable = true, columnDefinition = "varchar(10)")
//	private String srIsVerifyPass;
//
	@Column(name = "SR_STATUS_ID", nullable = true)
	private Integer srStatusId;
	
	@Column(name = "OLD_SR_STATUS_ID", nullable = true)
	private Integer oldSrStatusId;
	
//	@Column(name = "SR_DEF_ACCOUNT_ADDRESS_ID", nullable = true)
//	private Integer srDefAccountAddressId;
//	
//	@Column(name = "SR_DEF_ADDRESS_HOUSE_NO", nullable = true, columnDefinition = "varchar(50)")
//	private String srDefAddressHouseNo;
//
//	@Column(name = "SR_DEF_ADDRESS_MOO", nullable = true, columnDefinition = "varchar(50)")
//	private String srDefAddressMoo;
//
//	@Column(name = "SR_DEF_ADDRESS_BUILDING", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressBuliding;
//
//	@Column(name = "SR_DEF_ADDRESS_VILLAGE", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressVillage;
//
//	@Column(name = "SR_DEF_ADDRESS_SOI", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressSoi;
//
//	@Column(name = "SR_DEF_ADDRESS_STREET", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressStreet;
//
//	@Column(name = "SR_DEF_ADDRESS_TAMBOL", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressTambol;
//
//	@Column(name = "SR_DEF_ADDRESS_AMPHUR", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressAmphur;
//
//	@Column(name = "SR_DEF_ADDRESS_PROVINCE", nullable = true, columnDefinition = "varchar(500)")
//	private String srDefAddressProvince;
//
//	@Column(name = "SR_DEF_ADDRESS_ZIPCODE", nullable = true, columnDefinition = "varchar(10)")
//	private String srDefAddressSipcode;
//
//	@Column(name = "SR_AFS_ASSET_ID", nullable = true)
//	private Integer srAfsAssetId;
//
//	@Column(name = "SR_AFS_ASSET_NO", nullable = true, columnDefinition = "varchar(100)")
//	private String srAfsAssetNo;
//	
//	@Column(name = "SR_AFS_ASSET_DESC", nullable = true, columnDefinition = "varchar(1000)")
//	private String srAfsAssetDesc;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "SR_NCB_CUSTOMER_BIRTHDATE", nullable = true)
//	private Date srNcbCustomerBirthdate;
//
//	@Column(name = "SR_NCB_CHECK_STATUS", nullable = true, columnDefinition = "varchar(50)")
//	private String srNcbCheckStatus;
//	
//	@Column(name = "SR_NCB_MARKETING_USER_ID", nullable = true)
//	private Integer srNcbMargetingUserId;
//
//	@Column(name = "SR_NCB_MARKETING_FULL_NAME", nullable = true, columnDefinition = "varchar(10)")
//	private String srNcbMargetingFullName;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_ID", nullable = true)
//	private Integer srNcbMargetingBranchId;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_NAME", nullable = true, columnDefinition = "varchar(200)")
//	private String srNcbMargetingBranchName;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_1_ID", nullable = true)
//	private Integer srNcbMargetingBranchUpper1Id;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_1_NAME", nullable = true, columnDefinition = "varchar(200)")
//	private String srNcbMargetingBranchUpper1Name;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_2_ID", nullable = true)
//	private Integer srNcbMargetingBranchUpper2Id;
//	
//	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_2_NAME", nullable = true, columnDefinition = "varchar(200)")
//	private String srNcbMargetingBranchUpper2Name;
//	
//	@Column(name = "CREATE_BRANCH_ID", nullable = true)
//	private Integer createBranchId;
//	
	@Column(name = "CREATE_USER", nullable = true)
	private Integer createUser;

//	@Column(name = "UPDATE_USER", nullable = true)
//	private Integer updateUser;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "CREATE_DATE", nullable = true)
//	private Date createDate;
//
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "UPDATE_DATE", nullable = true)
//	private Date updateDate;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "CLOSE_DATE", nullable = true)
//	private Date closeDate;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "UPDATE_DATE_BY_OWNER", nullable = true)
//	private Date updateDateByOwner;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "UPDATE_DATE_BY_DELEGATE", nullable = true)
//	private Date updateDateByDelegate;
//	
	@Column(name = "RULE_DELEGATE_FLAG", nullable = true, columnDefinition = "varchar(1)")
	private String ruleDelegateFlag;

//	@Column(name = "RULE_DELEGATE_BRANCH_ID", nullable = true)
//	private Integer ruleDelegateBranchId;
//
	@Column(name = "RULE_THIS_ALERT", nullable = true)
	private Integer ruleThisAlert;

//	@Column(name = "RULE_TOTAL_ALERT", nullable = true)
//	private Integer ruleTotalAlert;
//
	@Column(name = "RULE_THIS_WORK", nullable = true)
	private Integer ruleThisWork;

//	@Column(name = "RULE_TOTAL_WORK", nullable = true)
//	private Integer ruleTotalWork;
//
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_NEXT_SLA", nullable = true)
	private Date ruleNextSla;
//	
//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "RULE_CURRENT_SLA", nullable = true)
//	private Date ruleCurrentSla;

//	@Column(name = "RULE_ASSIGN_FLAG", nullable = true, columnDefinition = "varchar(1)")
//	private String ruleAssignFlag;
//	
	@Column(name = "RULE_EMAIL_FLAG", nullable = true, columnDefinition = "varchar(1)")
	private String ruleEmailFlag;

//	@Temporal(TemporalType.TIMESTAMP)
//	@Column(name = "RULE_DELEGATE_DATE", nullable = true)
//	private Date ruleDelegateDate;
//	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_ASSIGN_DATE", nullable = true)
	private Date ruleAssignDate;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_STATUS_DATE", nullable = true)
	private Date ruleStatusDate;


//	@Column(name = "DRAFT_SR_EMAIL_TEMPLATE_ID", nullable = true)
//	private Integer draftSrEmailTemplateId;
//	
//	@Column(name = "DRAFT_MAIL_SENDER", nullable = true, columnDefinition = "varchar(255)")
//	private String draftMailSender;
//	
//	@Column(name = "DRAFT_MAIL_TO", nullable = true, columnDefinition = "varchar(500)")
//	private String draftMailTo;
//	
//	@Column(name = "DRAFT_MAIL_CC", nullable = true, columnDefinition = "varchar(500)")
//	private String draftMailCC;
//	
//	@Column(name = "DRAFT_MAIL_SUBJECT", nullable = true, columnDefinition = "varchar(2000)")
//	private String draftMailSubject;
//
//	@Column(name = "DRAFT_MAIL_BODY", nullable = true, columnDefinition = "varchar(8000)")
//	private String draftMailBody;
//	
//	@Column(name = "DRAFT_ACTIVITY_TYPE_ID", nullable = true)
//	private Integer draftActivityTypeId;
//	
//	@Column(name = "DRAFT_ACTIVITY_DESC", nullable = true, columnDefinition = "varchar(8000)")
//	private String draftActivityDesc;
//	
//	@Column(name = "DRAFT_ACCOUNT_ADDRESS_TEXT", nullable = true, columnDefinition = "varchar(8000)")
//	private String draftAccountAddressText;
//	
//	@Column(name = "DRAFT_IS_SEND_EMAIL_FOR_DELEGATE", nullable = true)
//	private Boolean draftIsSendEmailForDelegate;
//	
//	@Column(name = "DRAFT_IS_CLOSE", nullable = true)
//	private Boolean draftIsClose;
//	
//	@Column(name = "DRAFT_ATTACHMENT_JSON", nullable = true, columnDefinition = "text")
//	private String draftAttachmentJson;
//	
//	@Column(name = "DRAFT_VERIFY_ANSWER_JSON", nullable = true, columnDefinition = "text")
//	private String draftVerifyAnswerJson;
	
	public Integer getSrId() {
		return srId;
	}

	public void setSrId(Integer srId) {
		this.srId = srId;
	}

	public Integer getCustomerId() {
		return customerId;
	}

	public void setCustomerId(Integer customerId) {
		this.customerId = customerId;
	}

	public Integer getProductgroupId() {
		return productgroupId;
	}

	public void setProductgroupId(Integer productgroupId) {
		this.productgroupId = productgroupId;
	}

	public Integer getProductId() {
		return productId;
	}

	public void setProductId(Integer productId) {
		this.productId = productId;
	}

	public Integer getCampaignserviceId() {
		return campaignserviceId;
	}

	public void setCampaignserviceId(Integer campaignserviceId) {
		this.campaignserviceId = campaignserviceId;
	}

	public Integer getAreaId() {
		return areaId;
	}

	public void setAreaId(Integer areaId) {
		this.areaId = areaId;
	}

	public Integer getSubareaId() {
		return subareaId;
	}

	public void setSubareaId(Integer subareaId) {
		this.subareaId = subareaId;
	}

	public Integer getTypeId() {
		return typeId;
	}

	public void setTypeId(Integer typeId) {
		this.typeId = typeId;
	}

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Integer getOwnerBranchId() {
		return ownerBranchId;
	}

	public void setOwnerBranchId(Integer ownerBranchId) {
		this.ownerBranchId = ownerBranchId;
	}

	public Integer getOwnerUserId() {
		return ownerUserId;
	}

	public void setOwnerUserId(Integer ownerUserId) {
		this.ownerUserId = ownerUserId;
	}

	public Integer getDelegateUserId() {
		return delegateUserId;
	}

	public void setDelegateUserId(Integer delegateUserId) {
		this.delegateUserId = delegateUserId;
	}

	public Integer getSrStatusId() {
		return srStatusId;
	}

	public void setSrStatusId(Integer srStatusId) {
		this.srStatusId = srStatusId;
	}

	public Integer getOldSrStatusId() {
		return oldSrStatusId;
	}

	public void setOldSrStatusId(Integer oldSrStatusId) {
		this.oldSrStatusId = oldSrStatusId;
	}

	public Integer getCreateUser() {
		return createUser;
	}

	public void setCreateUser(Integer createUser) {
		this.createUser = createUser;
	}

	public String getRuleDelegateFlag() {
		return ruleDelegateFlag;
	}

	public void setRuleDelegateFlag(String ruleDelegateFlag) {
		this.ruleDelegateFlag = ruleDelegateFlag;
	}

	public Integer getRuleThisAlert() {
		return ruleThisAlert;
	}

	public void setRuleThisAlert(Integer ruleThisAlert) {
		this.ruleThisAlert = ruleThisAlert;
	}

	public Integer getRuleThisWork() {
		return ruleThisWork;
	}

	public void setRuleThisWork(Integer ruleThisWork) {
		this.ruleThisWork = ruleThisWork;
	}

	public Date getRuleNextSla() {
		return ruleNextSla;
	}

	public void setRuleNextSla(Date ruleNextSla) {
		this.ruleNextSla = ruleNextSla;
	}

	public String getRuleEmailFlag() {
		return ruleEmailFlag;
	}

	public void setRuleEmailFlag(String ruleEmailFlag) {
		this.ruleEmailFlag = ruleEmailFlag;
	}

	public Date getRuleAssignDate() {
		return ruleAssignDate;
	}

	public void setRuleAssignDate(Date ruleAssignDate) {
		this.ruleAssignDate = ruleAssignDate;
	}

	public Date getRuleStatusDate() {
		return ruleStatusDate;
	}

	public void setRuleStatusDate(Date ruleStatusDate) {
		this.ruleStatusDate = ruleStatusDate;
	}

}
