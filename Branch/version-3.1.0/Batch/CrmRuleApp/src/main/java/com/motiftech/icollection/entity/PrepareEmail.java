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
@Table(name = "TB_T_SR_PREPARE_EMAIL")
public class PrepareEmail implements Serializable{

	private static final long serialVersionUID = 6243400805345121499L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_PREPARE_EMAIL_ID",  nullable = false, insertable = true, updatable = true)
	private Integer srPrepareEmailId;
	
	@Column(name = "SR_ACTIVITY_ID", nullable = false)
	private Integer srActivityId;
	
	@Column(name = "EMAIL_ADDRESS", nullable = false, columnDefinition = "varchar(255)")
	private String emailAddress;
	
	@Column(name = "EMAIL_CONTENT", nullable = true, columnDefinition = "varchar(max)")
	private String emailContent;
	
	@Column(name = "EMAIL_SENDER", nullable = false, columnDefinition = "varchar(100)")
	private String emailSender;
	
	@Column(name = "EMAIL_SUBJECT", nullable = false, columnDefinition = "varchar(500)")
	private String emailSubject;
	
	@Column(name = "EMAIL_CREATE_BY", nullable = true, columnDefinition = "varchar(100)")
	private String emailCreateBy;
	
	@Column(name = "RULE_ACTIVITY_ID", nullable = true)
	private Integer ruleActivityId;
	
	@Column(name = "EXPORT_STATUS", nullable = true, columnDefinition = "varchar(10)")
	private String exportStatus;
	
	@Column(name = "CREATE_USER", nullable = true, columnDefinition = "varchar(100)")
	private String createUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;
	
	@Column(name = "UPDATE_USER", nullable = true, columnDefinition = "varchar(100)")
	private String updateUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	@Column(name = "IS_DELETE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean isDelete;

	
	@Column(name = "THIS_ALERT", nullable = true)
	private Integer thisAlert;
	
	@Column(name = "WORKING_MINUTE", nullable = true)
	private Integer workingMinute;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_NEXT_SLA", nullable = true)
	private Date ruleNextSLA;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "RULE_CURRENT_SLA", nullable = true)
	private Date ruleCurrentSLA;
	
	
	public Integer getSrPrepareEmailId() {
		return srPrepareEmailId;
	}

	public void setSrPrepareEmailId(Integer srPrepareEmailId) {
		this.srPrepareEmailId = srPrepareEmailId;
	}

	public Integer getSrActivityId() {
		return srActivityId;
	}

	public void setSrActivityId(Integer srActivityId) {
		this.srActivityId = srActivityId;
	}

	public String getEmailAddress() {
		return emailAddress;
	}

	public void setEmailAddress(String emailAddress) {
		this.emailAddress = emailAddress;
	}

	public String getEmailContent() {
		return emailContent;
	}

	public void setEmailContent(String emailContent) {
		this.emailContent = emailContent;
	}

	public String getEmailSender() {
		return emailSender;
	}

	public void setEmailSender(String emailSender) {
		this.emailSender = emailSender;
	}

	public String getEmailSubject() {
		return emailSubject;
	}

	public void setEmailSubject(String emailSubject) {
		this.emailSubject = emailSubject;
	}

	public String getEmailCreateBy() {
		return emailCreateBy;
	}

	public void setEmailCreateBy(String emailCreateBy) {
		this.emailCreateBy = emailCreateBy;
	}

	public Integer getRuleActivityId() {
		return ruleActivityId;
	}

	public void setRuleActivityId(Integer ruleActivityId) {
		this.ruleActivityId = ruleActivityId;
	}

	public String getExportStatus() {
		return exportStatus;
	}

	public void setExportStatus(String exportStatus) {
		this.exportStatus = exportStatus;
	}

	public String getCreateUser() {
		return createUser;
	}

	public void setCreateUser(String createUser) {
		this.createUser = createUser;
	}

	public Date getCreateDate() {
		return createDate;
	}

	public void setCreateDate(Date createDate) {
		this.createDate = createDate;
	}

	public String getUpdateUser() {
		return updateUser;
	}

	public void setUpdateUser(String updateUser) {
		this.updateUser = updateUser;
	}

	public Date getUpdateDate() {
		return updateDate;
	}

	public void setUpdateDate(Date updateDate) {
		this.updateDate = updateDate;
	}

	public Boolean getIsDelete() {
		return isDelete;
	}

	public void setIsDelete(Boolean isDelete) {
		this.isDelete = isDelete;
	}

	public Integer getThisAlert() {
		return thisAlert;
	}

	public void setThisAlert(Integer thisAlert) {
		this.thisAlert = thisAlert;
	}

	public Integer getWorkingMinute() {
		return workingMinute;
	}

	public void setWorkingMinute(Integer workingMinute) {
		this.workingMinute = workingMinute;
	}

	public Date getRuleNextSLA() {
		return ruleNextSLA;
	}

	public void setRuleNextSLA(Date ruleNextSLA) {
		this.ruleNextSLA = ruleNextSLA;
	}

	public Date getRuleCurrentSLA() {
		return ruleCurrentSLA;
	}

	public void setRuleCurrentSLA(Date ruleCurrentSLA) {
		this.ruleCurrentSLA = ruleCurrentSLA;
	}
	
}
