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
@Table(name = "TB_L_SR_LOGGING")
public class SRLogging implements Serializable{

	private static final long serialVersionUID = -4010404100660129067L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_LOGGING_ID",  nullable = false, insertable = true, updatable = true)
	private Integer srLoggingId;
	
	@Column(name = "SR_ID", nullable = false)
	private Integer srId;
	
	@Column(name = "SR_ACTIVITY_ID", nullable = true)
	private Integer srActivityId;
	
	@Column(name = "SR_LOGGING_SYSTEM_ACTION", nullable = true, columnDefinition = "varchar(500)")
	private String srLoggingSystemAction;
	
	@Column(name = "SR_LOGGING_ACTION", nullable = true, columnDefinition = "varchar(500)")
	private String srLoggingAction;
	
	@Column(name = "CREATE_USER", nullable = true)
	private Integer createUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;
	
	@Column(name = "SR_STATUS_ID_OLD", nullable = true)
	private Integer srStatusIdOld;
	
	@Column(name = "SR_STATUS_ID_NEW", nullable = true)
	private Integer srStatusIdNew;
	
	@Column(name = "OWNER_USER_ID_OLD", nullable = true)
	private Integer ownerUserIdOld;
	
	@Column(name = "OWNER_USER_ID_NEW", nullable = true)
	private Integer ownerUserIdNew;
	
	@Column(name = "DELEGATE_USER_ID_OLD", nullable = true)
	private Integer delegateUserIdOld;
	
	@Column(name = "DELEGATE_USER_ID_NEW", nullable = true)
	private Integer delegateUserIdNew;
	
	@Column(name = "OVER_SLA_MINUTE", nullable = true)
	private Integer overSlaMinute;
	
	@Column(name = "OVER_SLA_TIMES", nullable = true)
	private Integer overSlaTimes;

	@Column(name = "WORKING_MINUTE", nullable = true)
	private Integer workingMinute;
	
	@Column(name = "WORKING_HOUR", nullable = true)
	private Integer workingHour;

	public Integer getSrLoggingId() {
		return srLoggingId;
	}

	public void setSrLoggingId(Integer srLoggingId) {
		this.srLoggingId = srLoggingId;
	}

	public Integer getSrId() {
		return srId;
	}

	public void setSrId(Integer srId) {
		this.srId = srId;
	}

	public Integer getSrActivityId() {
		return srActivityId;
	}

	public void setSrActivityId(Integer srActivityId) {
		this.srActivityId = srActivityId;
	}

	public String getSrLoggingSystemAction() {
		return srLoggingSystemAction;
	}

	public void setSrLoggingSystemAction(String srLoggingSystemAction) {
		this.srLoggingSystemAction = srLoggingSystemAction;
	}

	public String getSrLoggingAction() {
		return srLoggingAction;
	}

	public void setSrLoggingAction(String srLoggingAction) {
		this.srLoggingAction = srLoggingAction;
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

	public Integer getSrStatusIdOld() {
		return srStatusIdOld;
	}

	public void setSrStatusIdOld(Integer srStatusIdOld) {
		this.srStatusIdOld = srStatusIdOld;
	}

	public Integer getSrStatusIdNew() {
		return srStatusIdNew;
	}

	public void setSrStatusIdNew(Integer srStatusIdNew) {
		this.srStatusIdNew = srStatusIdNew;
	}

	public Integer getOwnerUserIdOld() {
		return ownerUserIdOld;
	}

	public void setOwnerUserIdOld(Integer ownerUserIdOld) {
		this.ownerUserIdOld = ownerUserIdOld;
	}

	public Integer getOwnerUserIdNew() {
		return ownerUserIdNew;
	}

	public void setOwnerUserIdNew(Integer ownerUserIdNew) {
		this.ownerUserIdNew = ownerUserIdNew;
	}

	public Integer getDelegateUserIdOld() {
		return delegateUserIdOld;
	}

	public void setDelegateUserIdOld(Integer delegateUserIdOld) {
		this.delegateUserIdOld = delegateUserIdOld;
	}

	public Integer getDelegateUserIdNew() {
		return delegateUserIdNew;
	}

	public void setDelegateUserIdNew(Integer delegateUserIdNew) {
		this.delegateUserIdNew = delegateUserIdNew;
	}

	public Integer getOverSlaMinute() {
		return overSlaMinute;
	}

	public void setOverSlaMinute(Integer overSlaMinute) {
		this.overSlaMinute = overSlaMinute;
	}

	public Integer getOverSlaTimes() {
		return overSlaTimes;
	}

	public void setOverSlaTimes(Integer overSlaTimes) {
		this.overSlaTimes = overSlaTimes;
	}

	public Integer getWorkingMinute() {
		return workingMinute;
	}

	public void setWorkingMinute(Integer workingMinute) {
		this.workingMinute = workingMinute;
	}

	public Integer getWorkingHour() {
		return workingHour;
	}

	public void setWorkingHour(Integer workingHour) {
		this.workingHour = workingHour;
	}
}
