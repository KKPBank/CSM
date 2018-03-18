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
@Table(name = "TB_C_SR_STATUS")
public class SRStatus implements Serializable{

	private static final long serialVersionUID = -1241198271092023572L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_STATUS_ID", nullable = false, insertable = true, updatable = true)
	private Integer srStatusId;
	
	@Column(name = "SR_STATUS_CODE", nullable = false, columnDefinition = "varchar(50)")
	private String srStatusCode;
	
	@Column(name = "SR_STATUS_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String srStatusName;

	@Column(name = "SR_STATE_ID", nullable = true)
	private Integer srStateId;
	
	@Column(name = "SR_STATUS_SEND_TO_HP", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean srStatusSendToHP;
	
	@Column(name = "SR_STATUS_RULE", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean srStatusRule;
	
	@Column(name = "STATUS", nullable = false, columnDefinition = "nvarchar(1)")
	private String status;
	
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
	
	public Integer getSrStatusId() {
		return srStatusId;
	}

	public void setSrStatusId(Integer srStatusId) {
		this.srStatusId = srStatusId;
	}

	public String getSrStatusCode() {
		return srStatusCode;
	}

	public void setSrStatusCode(String srStatusCode) {
		this.srStatusCode = srStatusCode;
	}

	public String getSrStatusName() {
		return srStatusName;
	}

	public void setSrStatusName(String srStatusName) {
		this.srStatusName = srStatusName;
	}

	public Integer getSrStateId() {
		return srStateId;
	}

	public void setSrStateId(Integer srStateId) {
		this.srStateId = srStateId;
	}

	public Boolean getSrStatusSendToHP() {
		return srStatusSendToHP;
	}

	public void setSrStatusSendToHP(Boolean srStatusSendToHP) {
		this.srStatusSendToHP = srStatusSendToHP;
	}

	public Boolean getSrStatusRule() {
		return srStatusRule;
	}

	public void setSrStatusRule(Boolean srStatusRule) {
		this.srStatusRule = srStatusRule;
	}

	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
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
	

}
