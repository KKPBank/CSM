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
@Table(name = "TB_T_SR_ACTIVITY")
public class Activity implements Serializable{

	private static final long serialVersionUID = -8587965130536294766L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_ACTIVITY_ID",  nullable = false, insertable = true, updatable = true)
	private Integer srActivityId;
	
	@Column(name = "SR_ID", nullable = false)
	private Integer srId;
	
	@Column(name = "OWNER_BRANCH_ID", nullable = true)
	private Integer ownerBranchId;
	
	@Column(name = "OWNER_USER_ID", nullable = true)
	private Integer ownerUserId;
	
	@Column(name = "SR_STATUS_ID", nullable = true)
	private Integer srStatusId;
	
	@Column(name = "DELEGATE_BRANCH_ID", nullable = true)
	private Integer delegateBranchId;
	
	@Column(name = "DELEGATE_USER_ID", nullable = true)
	private Integer delegateUserId;
	
	@Column(name = "OLD_OWNER_USER_ID", nullable = true)
	private Integer oldOwnerUserId;
	
	@Column(name = "OLD_DELEGATE_USER_ID", nullable = true)
	private Integer oldDelegateUserId;
	
	@Column(name = "OLD_SR_STATUS_ID", nullable = true)
	private Integer oldSrStatusId;
	
	@Column(name = "IS_SEND_DELEGATE_EMAIL", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean isSendDelegateEmail;
	
	@Column(name = "SR_ACTIVITY_DESC", nullable = true, columnDefinition = "nvarchar(max)")
	private String srActivityDesc;
	
	@Column(name = "SR_EMAIL_TEMPLATE_ID", nullable = true)
	private Integer srEmailTemplateId;
	
	@Column(name = "SR_ACTIVITY_EMAIL_SENDER", nullable = true, columnDefinition = "varchar(500)")
	private String srActivityEmailSender;
	
	@Column(name = "SR_ACTIVITY_EMAIL_TO", nullable = true, columnDefinition = "varchar(2000)")
	private String srActivityEmailTo;
	
	@Column(name = "SR_ACTIVITY_EMAIL_CC", nullable = true, columnDefinition = "varchar(2000)")
	private String srActivityEmailCC;
	
	@Column(name = "SR_ACTIVITY_EMAIL_SUBJECT", nullable = true, columnDefinition = "nvarchar(max)")
	private String srActivityEmailSubject;
	
	@Column(name = "SR_ACTIVITY_EMAIL_BODY", nullable = true, columnDefinition = "nvarchar(max)")
	private String srActivityEmailBody;
	
	@Column(name = "SR_ACTIVITY_TYPE_ID", nullable = true)
	private Integer srActivityTypeId;
	
	@Column(name = "CREATE_BRANCH_ID", nullable = true)
	private Integer createBranchId;
	
	@Column(name = "CREATE_USER", nullable = true)
	private Integer createUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;

	@Column(name = "ACTIVITY_CAR_SUBMIT_STATUS", nullable = true)
	private Short activityCarSubmitStatus;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "ACTIVITY_CAR_SUBMIT_DATE", nullable = true)
	private Date activityCarSubmitDate;
	
	@Column(name = "ACTIVITY_CAR_SUBMIT_ERROR_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String activityCarSubmitErrorCode;
	
	@Column(name = "ACTIVITY_CAR_SUBMIT_ERROR_DESC", nullable = true, columnDefinition = "varchar(4000)")
	private String activityCarSubmitErrorDesc;
	
	@Column(name = "ACTIVITY_HP_SUBMIT_STATUS", nullable = true)
	private Short activityHPSubmitStatus;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "ACTIVITY_HP_SUBMIT_DATE", nullable = true)
	private Date activityHPSubmitDate;
	
	@Column(name = "ACTIVITY_HP_SUBMIT_ERROR_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String activityHPSubmitErrorCode;
	
	@Column(name = "ACTIVITY_HP_SUBMIT_ERROR_DESC", nullable = true, columnDefinition = "varchar(4000)")
	private String activityHPSubmitErrorDesc;

	@Column(name = "WORKING_MINUTE", nullable = true)
	private Integer workingMinute;
	
	public Integer getSrActivityId() {
		return srActivityId;
	}

	public void setSrActivityId(Integer srActivityId) {
		this.srActivityId = srActivityId;
	}

	public Integer getSrId() {
		return srId;
	}

	public void setSrId(Integer srId) {
		this.srId = srId;
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

	public Integer getSrStatusId() {
		return srStatusId;
	}

	public void setSrStatusId(Integer srStatusId) {
		this.srStatusId = srStatusId;
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

	public Integer getOldSrStatusId() {
		return oldSrStatusId;
	}

	public void setOldSrStatusId(Integer oldSrStatusId) {
		this.oldSrStatusId = oldSrStatusId;
	}

	public Boolean getIsSendDelegateEmail() {
		return isSendDelegateEmail;
	}

	public void setIsSendDelegateEmail(Boolean isSendDelegateEmail) {
		this.isSendDelegateEmail = isSendDelegateEmail;
	}

	public String getSrActivityDesc() {
		return srActivityDesc;
	}

	public void setSrActivityDesc(String srActivityDesc) {
		this.srActivityDesc = srActivityDesc;
	}

	public Integer getSrEmailTemplateId() {
		return srEmailTemplateId;
	}

	public void setSrEmailTemplateId(Integer srEmailTemplateId) {
		this.srEmailTemplateId = srEmailTemplateId;
	}

	public String getSrActivityEmailSender() {
		return srActivityEmailSender;
	}

	public void setSrActivityEmailSender(String srActivityEmailSender) {
		this.srActivityEmailSender = srActivityEmailSender;
	}

	public String getSrActivityEmailTo() {
		return srActivityEmailTo;
	}

	public void setSrActivityEmailTo(String srActivityEmailTo) {
		this.srActivityEmailTo = srActivityEmailTo;
	}

	public String getSrActivityEmailCC() {
		return srActivityEmailCC;
	}

	public void setSrActivityEmailCC(String srActivityEmailCC) {
		this.srActivityEmailCC = srActivityEmailCC;
	}

	public String getSrActivityEmailSubject() {
		return srActivityEmailSubject;
	}

	public void setSrActivityEmailSubject(String srActivityEmailSubject) {
		this.srActivityEmailSubject = srActivityEmailSubject;
	}

	public String getSrActivityEmailBody() {
		return srActivityEmailBody;
	}

	public void setSrActivityEmailBody(String srActivityEmailBody) {
		this.srActivityEmailBody = srActivityEmailBody;
	}

	public Integer getSrActivityTypeId() {
		return srActivityTypeId;
	}

	public void setSrActivityTypeId(Integer srActivityTypeId) {
		this.srActivityTypeId = srActivityTypeId;
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

	public Date getCreateDate() {
		return createDate;
	}

	public void setCreateDate(Date createDate) {
		this.createDate = createDate;
	}

	public Short getActivityCarSubmitStatus() {
		return activityCarSubmitStatus;
	}

	public void setActivityCarSubmitStatus(Short activityCarSubmitStatus) {
		this.activityCarSubmitStatus = activityCarSubmitStatus;
	}

	public Date getActivityCarSubmitDate() {
		return activityCarSubmitDate;
	}

	public void setActivityCarSubmitDate(Date activityCarSubmitDate) {
		this.activityCarSubmitDate = activityCarSubmitDate;
	}

	public String getActivityCarSubmitErrorCode() {
		return activityCarSubmitErrorCode;
	}

	public void setActivityCarSubmitErrorCode(String activityCarSubmitErrorCode) {
		this.activityCarSubmitErrorCode = activityCarSubmitErrorCode;
	}

	public String getActivityCarSubmitErrorDesc() {
		return activityCarSubmitErrorDesc;
	}

	public void setActivityCarSubmitErrorDesc(String activityCarSubmitErrorDesc) {
		this.activityCarSubmitErrorDesc = activityCarSubmitErrorDesc;
	}

	public Short getActivityHPSubmitStatus() {
		return activityHPSubmitStatus;
	}

	public void setActivityHPSubmitStatus(Short activityHPSubmitStatus) {
		this.activityHPSubmitStatus = activityHPSubmitStatus;
	}

	public Date getActivityHPSubmitDate() {
		return activityHPSubmitDate;
	}

	public void setActivityHPSubmitDate(Date activityHPSubmitDate) {
		this.activityHPSubmitDate = activityHPSubmitDate;
	}

	public String getActivityHPSubmitErrorCode() {
		return activityHPSubmitErrorCode;
	}

	public void setActivityHPSubmitErrorCode(String activityHPSubmitErrorCode) {
		this.activityHPSubmitErrorCode = activityHPSubmitErrorCode;
	}

	public String getActivityHPSubmitErrorDesc() {
		return activityHPSubmitErrorDesc;
	}

	public void setActivityHPSubmitErrorDesc(String activityHPSubmitErrorDesc) {
		this.activityHPSubmitErrorDesc = activityHPSubmitErrorDesc;
	}

	public Integer getWorkingMinute() {
		return workingMinute;
	}

	public void setWorkingMinute(Integer workingMinute) {
		this.workingMinute = workingMinute;
	}
	
}
