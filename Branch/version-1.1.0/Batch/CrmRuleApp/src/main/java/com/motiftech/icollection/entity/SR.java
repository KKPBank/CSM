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
@Table(name = "TB_T_SR")
public class SR implements Serializable {

	private static final long serialVersionUID = 2751446770569246475L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_ID", nullable = false, insertable = true, updatable = true)
	private Integer srId;

	@Column(name = "SR_NO", nullable = true, columnDefinition = "varchar(50)")
	private String srNo;

	@Column(name = "SR_CALL_ID", nullable = true, columnDefinition = "varchar(50)")
	private String srCallId;

	@Column(name = "SR_ANO", nullable = true, columnDefinition = "varchar(50)")
	private String srAno;

	@Column(name = "CUSTOMER_ID", nullable = true)
	private Integer customerId;

	@Column(name = "ACCOUNT_ID", nullable = true)
	private Integer accountId;

	@Column(name = "CONTACT_ID", nullable = true)
	private Integer contactId;

	@Column(name = "CONTACT_ACCOUNT_NO", nullable = true, columnDefinition = "varchar(255)")
	private String contactAccountNo;

	@Column(name = "CONTACT_RELATIONSHIP_ID", nullable = true)
	private Integer contactRelationshipId;

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

	@Column(name = "MAP_PRODUCT_ID", nullable = true)
	private Integer mapProductId;

	@Column(name = "CHANNEL_ID", nullable = true)
	private Integer channelId;

	@Column(name = "MEDIA_SOURCE_ID", nullable = true)
	private Integer mediaSourceId;

	@Column(name = "SR_SUBJECT", nullable = true, columnDefinition = "nvarchar(max)")
	private String srSubject;

	@Column(name = "SR_REMARK", nullable = true, columnDefinition = "nvarchar(max)")
	private String srRemark;

	@Column(name = "OWNER_BRANCH_ID", nullable = true)
	private Integer ownerBranchId;

	@Column(name = "OWNER_USER_ID", nullable = true)
	private Integer ownerUserId;

	@Column(name = "DELEGATE_BRANCH_ID", nullable = true)
	private Integer delegateBranchId;

	@Column(name = "DELEGATE_USER_ID", nullable = true)
	private Integer delegateUserId;

	@Column(name = "OLD_OWNER_USER_ID", nullable = true)
	private Integer oldOwnerUserId;

	@Column(name = "OLD_DELEGATE_USER_ID", nullable = true)
	private Integer oldDelegateUserId;

	@Column(name = "SR_PAGE_ID", nullable = true)
	private Integer srPageId;

	@Column(name = "SR_IS_VERIFY", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean srIsVerify;

	@Column(name = "SR_IS_VERIFY_PASS", nullable = true, columnDefinition = "varchar(10)")
	private String srIsVerifyPass;

	@Column(name = "SR_STATUS_ID", nullable = true)
	private Integer srStatusId;

	@Column(name = "OLD_SR_STATUS_ID", nullable = true)
	private Integer oldSrStatusId;

	@Column(name = "SR_DEF_ACCOUNT_ADDRESS_ID", nullable = true)
	private Integer srDefAccountAddressId;

	@Column(name = "SR_DEF_ADDRESS_HOUSE_NO", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressHouseNo;

	@Column(name = "SR_DEF_ADDRESS_MOO", nullable = true, columnDefinition = "varchar(50)")
	private String srDefAddressMoo;

	@Column(name = "SR_DEF_ADDRESS_BUILDING", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressBuliding;

	@Column(name = "SR_DEF_ADDRESS_VILLAGE", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressVillage;

	@Column(name = "SR_DEF_ADDRESS_SOI", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressSoi;

	@Column(name = "SR_DEF_ADDRESS_STREET", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressStreet;

	@Column(name = "SR_DEF_ADDRESS_TAMBOL", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressTambol;

	@Column(name = "SR_DEF_ADDRESS_AMPHUR", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressAmphur;

	@Column(name = "SR_DEF_ADDRESS_PROVINCE", nullable = true, columnDefinition = "varchar(500)")
	private String srDefAddressProvince;

	@Column(name = "SR_DEF_ADDRESS_ZIPCODE", nullable = true, columnDefinition = "varchar(10)")
	private String srDefAddressSipcode;

	@Column(name = "SR_AFS_ASSET_ID", nullable = true)
	private Integer srAfsAssetId;

	@Column(name = "SR_AFS_ASSET_NO", nullable = true, columnDefinition = "varchar(100)")
	private String srAfsAssetNo;

	@Column(name = "SR_AFS_ASSET_DESC", nullable = true, columnDefinition = "varchar(1000)")
	private String srAfsAssetDesc;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "SR_NCB_CUSTOMER_BIRTHDATE", nullable = true)
	private Date srNcbCustomerBirthdate;

	@Column(name = "SR_NCB_CHECK_STATUS", nullable = true, columnDefinition = "varchar(50)")
	private String srNcbCheckStatus;

	@Column(name = "SR_NCB_MARKETING_USER_ID", nullable = true)
	private Integer srNcbMargetingUserId;

	@Column(name = "SR_NCB_MARKETING_FULL_NAME", nullable = true, columnDefinition = "varchar(10)")
	private String srNcbMargetingFullName;

	@Column(name = "SR_NCB_MARKETING_BRANCH_ID", nullable = true)
	private Integer srNcbMargetingBranchId;

	@Column(name = "SR_NCB_MARKETING_BRANCH_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String srNcbMargetingBranchName;

	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_1_ID", nullable = true)
	private Integer srNcbMargetingBranchUpper1Id;

	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_1_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String srNcbMargetingBranchUpper1Name;

	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_2_ID", nullable = true)
	private Integer srNcbMargetingBranchUpper2Id;

	@Column(name = "SR_NCB_MARKETING_BRANCH_UPPER_2_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String srNcbMargetingBranchUpper2Name;

	@Column(name = "CREATE_BRANCH_ID", nullable = true)
	private Integer createBranchId;

	@Column(name = "CREATE_USER", nullable = true)
	private Integer createUser;

	@Column(name = "UPDATE_USER", nullable = true)
	private Integer updateUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CLOSE_DATE", nullable = true)
	private Date closeDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE_BY_OWNER", nullable = true)
	private Date updateDateByOwner;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE_BY_DELEGATE", nullable = true)
	private Date updateDateByDelegate;

	@Column(name = "RULE_DELEGATE_FLAG", nullable = true, columnDefinition = "varchar(1)")
	private String ruleDelegateFlag;

	@Column(name = "RULE_DELEGATE_BRANCH_ID", nullable = true)
	private Integer ruleDelegateBranchId;

	@Column(name = "RULE_THIS_ALERT", nullable = true)
	private Integer ruleThisAlert;

	@Column(name = "RULE_TOTAL_ALERT", nullable = true)
	private Integer ruleTotalAlert;

	@Column(name = "RULE_THIS_WORK", nullable = true)
	private Integer ruleThisWork;

	@Column(name = "RULE_TOTAL_WORK", nullable = true)
	private Integer ruleTotalWork;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_NEXT_SLA", nullable = true)
	private Date ruleNextSla;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_CURRENT_SLA", nullable = true)
	private Date ruleCurrentSla;

	@Column(name = "RULE_ASSIGN_FLAG", nullable = true, columnDefinition = "varchar(1)")
	private String ruleAssignFlag;

	@Column(name = "RULE_EMAIL_FLAG", nullable = true, columnDefinition = "varchar(1)")
	private String ruleEmailFlag;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_DELEGATE_DATE", nullable = true)
	private Date ruleDelegateDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_ASSIGN_DATE", nullable = true)
	private Date ruleAssignDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_STATUS_DATE", nullable = true)
	private Date ruleStatusDate;

	@Column(name = "DRAFT_SR_EMAIL_TEMPLATE_ID", nullable = true)
	private Integer draftSrEmailTemplateId;

	@Column(name = "DRAFT_MAIL_SENDER", nullable = true, columnDefinition = "varchar(255)")
	private String draftMailSender;

	@Column(name = "DRAFT_MAIL_TO", nullable = true, columnDefinition = "varchar(500)")
	private String draftMailTo;

	@Column(name = "DRAFT_MAIL_CC", nullable = true, columnDefinition = "varchar(500)")
	private String draftMailCC;

	@Column(name = "DRAFT_MAIL_SUBJECT", nullable = true, columnDefinition = "varchar(2000)")
	private String draftMailSubject;

	@Column(name = "DRAFT_MAIL_BODY", nullable = true, columnDefinition = "varchar(8000)")
	private String draftMailBody;

	@Column(name = "DRAFT_ACTIVITY_TYPE_ID", nullable = true)
	private Integer draftActivityTypeId;

	@Column(name = "DRAFT_ACTIVITY_DESC", nullable = true, columnDefinition = "varchar(8000)")
	private String draftActivityDesc;

	@Column(name = "DRAFT_ACCOUNT_ADDRESS_TEXT", nullable = true, columnDefinition = "varchar(8000)")
	private String draftAccountAddressText;

	@Column(name = "DRAFT_IS_SEND_EMAIL_FOR_DELEGATE", nullable = true)
	private Boolean draftIsSendEmailForDelegate;

	@Column(name = "DRAFT_IS_CLOSE", nullable = true)
	private Boolean draftIsClose;

	@Column(name = "DRAFT_ATTACHMENT_JSON", nullable = true, columnDefinition = "text")
	private String draftAttachmentJson;

	@Column(name = "DRAFT_VERIFY_ANSWER_JSON", nullable = true, columnDefinition = "text")
	private String draftVerifyAnswerJson;

	public Integer getSrId() {
		return srId;
	}

	public void setSrId(Integer srId) {
		this.srId = srId;
	}

	public String getSrNo() {
		return srNo;
	}

	public void setSrNo(String srNo) {
		this.srNo = srNo;
	}

	public String getSrCallId() {
		return srCallId;
	}

	public void setSrCallId(String srCallId) {
		this.srCallId = srCallId;
	}

	public String getSrAno() {
		return srAno;
	}

	public void setSrAno(String srAno) {
		this.srAno = srAno;
	}

	public Integer getCustomerId() {
		return customerId;
	}

	public void setCustomerId(Integer customerId) {
		this.customerId = customerId;
	}

	public Integer getAccountId() {
		return accountId;
	}

	public void setAccountId(Integer accountId) {
		this.accountId = accountId;
	}

	public Integer getContactId() {
		return contactId;
	}

	public void setContactId(Integer contactId) {
		this.contactId = contactId;
	}

	public String getContactAccountNo() {
		return contactAccountNo;
	}

	public void setContactAccountNo(String contactAccountNo) {
		this.contactAccountNo = contactAccountNo;
	}

	public Integer getContactRelationshipId() {
		return contactRelationshipId;
	}

	public void setContactRelationshipId(Integer contactRelationshipId) {
		this.contactRelationshipId = contactRelationshipId;
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

	public Integer getMapProductId() {
		return mapProductId;
	}

	public void setMapProductId(Integer mapProductId) {
		this.mapProductId = mapProductId;
	}

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Integer getMediaSourceId() {
		return mediaSourceId;
	}

	public void setMediaSourceId(Integer mediaSourceId) {
		this.mediaSourceId = mediaSourceId;
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

	public Integer getDelegateBranchId() {
		return delegateBranchId;
	}

	public void setDelegateBranchId(Integer delegateBranchId) {
		this.delegateBranchId = delegateBranchId;
	}

	public Integer getDelegateUserId() {
		return delegateUserId;
	}

	public void setDelegateUserId(Integer delegateUserId) {
		this.delegateUserId = delegateUserId;
	}

	public Integer getOldOwnerUserId() {
		return oldOwnerUserId;
	}

	public void setOldOwnerUserId(Integer oldOwnerUserId) {
		this.oldOwnerUserId = oldOwnerUserId;
	}

	public Integer getOldDelegateUserId() {
		return oldDelegateUserId;
	}

	public void setOldDelegateUserId(Integer oldDelegateUserId) {
		this.oldDelegateUserId = oldDelegateUserId;
	}

	public Integer getSrPageId() {
		return srPageId;
	}

	public void setSrPageId(Integer srPageId) {
		this.srPageId = srPageId;
	}

	public Boolean getSrIsVerify() {
		return srIsVerify;
	}

	public void setSrIsVerify(Boolean srIsVerify) {
		this.srIsVerify = srIsVerify;
	}

	public String getSrIsVerifyPass() {
		return srIsVerifyPass;
	}

	public void setSrIsVerifyPass(String srIsVerifyPass) {
		this.srIsVerifyPass = srIsVerifyPass;
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

	public Integer getSrDefAccountAddressId() {
		return srDefAccountAddressId;
	}

	public void setSrDefAccountAddressId(Integer srDefAccountAddressId) {
		this.srDefAccountAddressId = srDefAccountAddressId;
	}

	public String getSrDefAddressHouseNo() {
		return srDefAddressHouseNo;
	}

	public void setSrDefAddressHouseNo(String srDefAddressHouseNo) {
		this.srDefAddressHouseNo = srDefAddressHouseNo;
	}

	public String getSrDefAddressMoo() {
		return srDefAddressMoo;
	}

	public void setSrDefAddressMoo(String srDefAddressMoo) {
		this.srDefAddressMoo = srDefAddressMoo;
	}

	public String getSrDefAddressBuliding() {
		return srDefAddressBuliding;
	}

	public void setSrDefAddressBuliding(String srDefAddressBuliding) {
		this.srDefAddressBuliding = srDefAddressBuliding;
	}

	public String getSrDefAddressVillage() {
		return srDefAddressVillage;
	}

	public void setSrDefAddressVillage(String srDefAddressVillage) {
		this.srDefAddressVillage = srDefAddressVillage;
	}

	public String getSrDefAddressSoi() {
		return srDefAddressSoi;
	}

	public void setSrDefAddressSoi(String srDefAddressSoi) {
		this.srDefAddressSoi = srDefAddressSoi;
	}

	public String getSrDefAddressStreet() {
		return srDefAddressStreet;
	}

	public void setSrDefAddressStreet(String srDefAddressStreet) {
		this.srDefAddressStreet = srDefAddressStreet;
	}

	public String getSrDefAddressTambol() {
		return srDefAddressTambol;
	}

	public void setSrDefAddressTambol(String srDefAddressTambol) {
		this.srDefAddressTambol = srDefAddressTambol;
	}

	public String getSrDefAddressAmphur() {
		return srDefAddressAmphur;
	}

	public void setSrDefAddressAmphur(String srDefAddressAmphur) {
		this.srDefAddressAmphur = srDefAddressAmphur;
	}

	public String getSrDefAddressProvince() {
		return srDefAddressProvince;
	}

	public void setSrDefAddressProvince(String srDefAddressProvince) {
		this.srDefAddressProvince = srDefAddressProvince;
	}

	public String getSrDefAddressSipcode() {
		return srDefAddressSipcode;
	}

	public void setSrDefAddressSipcode(String srDefAddressSipcode) {
		this.srDefAddressSipcode = srDefAddressSipcode;
	}

	public Integer getSrAfsAssetId() {
		return srAfsAssetId;
	}

	public void setSrAfsAssetId(Integer srAfsAssetId) {
		this.srAfsAssetId = srAfsAssetId;
	}

	public String getSrAfsAssetNo() {
		return srAfsAssetNo;
	}

	public void setSrAfsAssetNo(String srAfsAssetNo) {
		this.srAfsAssetNo = srAfsAssetNo;
	}

	public String getSrAfsAssetDesc() {
		return srAfsAssetDesc;
	}

	public void setSrAfsAssetDesc(String srAfsAssetDesc) {
		this.srAfsAssetDesc = srAfsAssetDesc;
	}

	public Date getSrNcbCustomerBirthdate() {
		return srNcbCustomerBirthdate;
	}

	public void setSrNcbCustomerBirthdate(Date srNcbCustomerBirthdate) {
		this.srNcbCustomerBirthdate = srNcbCustomerBirthdate;
	}

	public String getSrNcbCheckStatus() {
		return srNcbCheckStatus;
	}

	public void setSrNcbCheckStatus(String srNcbCheckStatus) {
		this.srNcbCheckStatus = srNcbCheckStatus;
	}

	public Integer getSrNcbMargetingUserId() {
		return srNcbMargetingUserId;
	}

	public void setSrNcbMargetingUserId(Integer srNcbMargetingUserId) {
		this.srNcbMargetingUserId = srNcbMargetingUserId;
	}

	public String getSrNcbMargetingFullName() {
		return srNcbMargetingFullName;
	}

	public void setSrNcbMargetingFullName(String srNcbMargetingFullName) {
		this.srNcbMargetingFullName = srNcbMargetingFullName;
	}

	public Integer getSrNcbMargetingBranchId() {
		return srNcbMargetingBranchId;
	}

	public void setSrNcbMargetingBranchId(Integer srNcbMargetingBranchId) {
		this.srNcbMargetingBranchId = srNcbMargetingBranchId;
	}

	public String getSrNcbMargetingBranchName() {
		return srNcbMargetingBranchName;
	}

	public void setSrNcbMargetingBranchName(String srNcbMargetingBranchName) {
		this.srNcbMargetingBranchName = srNcbMargetingBranchName;
	}

	public Integer getSrNcbMargetingBranchUpper1Id() {
		return srNcbMargetingBranchUpper1Id;
	}

	public void setSrNcbMargetingBranchUpper1Id(Integer srNcbMargetingBranchUpper1Id) {
		this.srNcbMargetingBranchUpper1Id = srNcbMargetingBranchUpper1Id;
	}

	public String getSrNcbMargetingBranchUpper1Name() {
		return srNcbMargetingBranchUpper1Name;
	}

	public void setSrNcbMargetingBranchUpper1Name(String srNcbMargetingBranchUpper1Name) {
		this.srNcbMargetingBranchUpper1Name = srNcbMargetingBranchUpper1Name;
	}

	public Integer getSrNcbMargetingBranchUpper2Id() {
		return srNcbMargetingBranchUpper2Id;
	}

	public void setSrNcbMargetingBranchUpper2Id(Integer srNcbMargetingBranchUpper2Id) {
		this.srNcbMargetingBranchUpper2Id = srNcbMargetingBranchUpper2Id;
	}

	public String getSrNcbMargetingBranchUpper2Name() {
		return srNcbMargetingBranchUpper2Name;
	}

	public void setSrNcbMargetingBranchUpper2Name(String srNcbMargetingBranchUpper2Name) {
		this.srNcbMargetingBranchUpper2Name = srNcbMargetingBranchUpper2Name;
	}

	public Integer getCreateBranchId() {
		return createBranchId;
	}

	public void setCreateBranchId(Integer createBranchId) {
		this.createBranchId = createBranchId;
	}

	public Integer getCreateUser() {
		return createUser;
	}

	public void setCreateUser(Integer createUser) {
		this.createUser = createUser;
	}

	public Integer getUpdateUser() {
		return updateUser;
	}

	public void setUpdateUser(Integer updateUser) {
		this.updateUser = updateUser;
	}

	public Date getCreateDate() {
		return createDate;
	}

	public void setCreateDate(Date createDate) {
		this.createDate = createDate;
	}

	public Date getUpdateDate() {
		return updateDate;
	}

	public void setUpdateDate(Date updateDate) {
		this.updateDate = updateDate;
	}

	public Date getCloseDate() {
		return closeDate;
	}

	public void setCloseDate(Date closeDate) {
		this.closeDate = closeDate;
	}

	public Date getUpdateDateByOwner() {
		return updateDateByOwner;
	}

	public void setUpdateDateByOwner(Date updateDateByOwner) {
		this.updateDateByOwner = updateDateByOwner;
	}

	public Date getUpdateDateByDelegate() {
		return updateDateByDelegate;
	}

	public void setUpdateDateByDelegate(Date updateDateByDelegate) {
		this.updateDateByDelegate = updateDateByDelegate;
	}

	public String getRuleDelegateFlag() {
		return ruleDelegateFlag;
	}

	public void setRuleDelegateFlag(String ruleDelegateFlag) {
		this.ruleDelegateFlag = ruleDelegateFlag;
	}

	public Integer getRuleDelegateBranchId() {
		return ruleDelegateBranchId;
	}

	public void setRuleDelegateBranchId(Integer ruleDelegateBranchId) {
		this.ruleDelegateBranchId = ruleDelegateBranchId;
	}

	public Integer getRuleThisAlert() {
		return ruleThisAlert;
	}

	public void setRuleThisAlert(Integer ruleThisAlert) {
		this.ruleThisAlert = ruleThisAlert;
	}

	public Integer getRuleTotalAlert() {
		return ruleTotalAlert;
	}

	public void setRuleTotalAlert(Integer ruleTotalAlert) {
		this.ruleTotalAlert = ruleTotalAlert;
	}

	public Integer getRuleThisWork() {
		return ruleThisWork;
	}

	public void setRuleThisWork(Integer ruleThisWork) {
		this.ruleThisWork = ruleThisWork;
	}

	public Integer getRuleTotalWork() {
		return ruleTotalWork;
	}

	public void setRuleTotalWork(Integer ruleTotalWork) {
		this.ruleTotalWork = ruleTotalWork;
	}

	public Date getRuleNextSla() {
		return ruleNextSla;
	}

	public void setRuleNextSla(Date ruleNextSla) {
		this.ruleNextSla = ruleNextSla;
	}

	public String getRuleAssignFlag() {
		return ruleAssignFlag;
	}

	public void setRuleAssignFlag(String ruleAssignFlag) {
		this.ruleAssignFlag = ruleAssignFlag;
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

	public Integer getDraftSrEmailTemplateId() {
		return draftSrEmailTemplateId;
	}

	public void setDraftSrEmailTemplateId(Integer draftSrEmailTemplateId) {
		this.draftSrEmailTemplateId = draftSrEmailTemplateId;
	}

	public String getDraftMailSender() {
		return draftMailSender;
	}

	public void setDraftMailSender(String draftMailSender) {
		this.draftMailSender = draftMailSender;
	}

	public String getDraftMailTo() {
		return draftMailTo;
	}

	public void setDraftMailTo(String draftMailTo) {
		this.draftMailTo = draftMailTo;
	}

	public String getDraftMailCC() {
		return draftMailCC;
	}

	public void setDraftMailCC(String draftMailCC) {
		this.draftMailCC = draftMailCC;
	}

	public String getDraftMailSubject() {
		return draftMailSubject;
	}

	public void setDraftMailSubject(String draftMailSubject) {
		this.draftMailSubject = draftMailSubject;
	}

	public String getDraftMailBody() {
		return draftMailBody;
	}

	public void setDraftMailBody(String draftMailBody) {
		this.draftMailBody = draftMailBody;
	}

	public Integer getDraftActivityTypeId() {
		return draftActivityTypeId;
	}

	public void setDraftActivityTypeId(Integer draftActivityTypeId) {
		this.draftActivityTypeId = draftActivityTypeId;
	}

	public String getDraftActivityDesc() {
		return draftActivityDesc;
	}

	public void setDraftActivityDesc(String draftActivityDesc) {
		this.draftActivityDesc = draftActivityDesc;
	}

	public String getDraftAccountAddressText() {
		return draftAccountAddressText;
	}

	public void setDraftAccountAddressText(String draftAccountAddressText) {
		this.draftAccountAddressText = draftAccountAddressText;
	}

	public Boolean getDraftIsSendEmailForDelegate() {
		return draftIsSendEmailForDelegate;
	}

	public void setDraftIsSendEmailForDelegate(Boolean draftIsSendEmailForDelegate) {
		this.draftIsSendEmailForDelegate = draftIsSendEmailForDelegate;
	}

	public Boolean getDraftIsClose() {
		return draftIsClose;
	}

	public void setDraftIsClose(Boolean draftIsClose) {
		this.draftIsClose = draftIsClose;
	}

	public String getDraftAttachmentJson() {
		return draftAttachmentJson;
	}

	public void setDraftAttachmentJson(String draftAttachmentJson) {
		this.draftAttachmentJson = draftAttachmentJson;
	}

	public String getDraftVerifyAnswerJson() {
		return draftVerifyAnswerJson;
	}

	public void setDraftVerifyAnswerJson(String draftVerifyAnswerJson) {
		this.draftVerifyAnswerJson = draftVerifyAnswerJson;
	}

	public Date getRuleDelegateDate() {
		return ruleDelegateDate;
	}

	public void setRuleDelegateDate(Date ruleDelegateDate) {
		this.ruleDelegateDate = ruleDelegateDate;
	}

	public Date getRuleCurrentSla() {
		return ruleCurrentSla;
	}

	public void setRuleCurrentSla(Date ruleCurrentSla) {
		this.ruleCurrentSla = ruleCurrentSla;
	}

	public String getSrSubject() {
		return srSubject;
	}

	public void setSrSubject(String srSubject) {
		this.srSubject = srSubject;
	}

	public String getSrRemark() {
		return srRemark;
	}

	public void setSrRemark(String srRemark) {
		this.srRemark = srRemark;
	}
}
