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
@Table(name="TB_R_BRANCH")
public class Branch implements Serializable {
	
	private static final long serialVersionUID = 7470512869154703273L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "BRANCH_ID", nullable = false, insertable = true, updatable = true)
	private Integer branchId;

	@Column(name = "CHANNEL_ID", nullable = true)
	private Integer channelId;
	
	@Column(name = "BRANCH_CODE", nullable = true, columnDefinition = "varchar(100)")
	private String branchCode;

	@Column(name = "BRANCH_NAME", nullable = true, columnDefinition = "varchar(500)")
	private String branchName;

	@Column(name = "STATUS", nullable = true)
	private Short status;

	@Column(name = "UPPER_BRANCH_ID", nullable = true)
	private Integer upperBranchId;
	
	@Column(name = "START_TIME_HOUR", nullable = true)
	private Short startTimeHour;

	@Column(name = "START_TIME_MINUTE", nullable = true)
	private Short startTimeMinute;

	@Column(name = "END_TIME_HOUR", nullable = true)
	private Short endTimeHour;

	@Column(name = "END_TIME_MINUTE", nullable = true)
	private Short endTimeMinute;
	
	@Column(name = "CREATE_USER", nullable = true, columnDefinition = "varchar(50)")
	private String createUser;

	@Column(name = "UPDATE_USER", nullable = true, columnDefinition = "varchar(50)")
	private String updateUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	public Integer getBranchId() {
		return branchId;
	}

	public void setBranchId(Integer branchId) {
		this.branchId = branchId;
	}

	public String getBranchCode() {
		return branchCode;
	}

	public void setBranchCode(String branchCode) {
		this.branchCode = branchCode;
	}

	public String getBranchName() {
		return branchName;
	}

	public void setBranchName(String branchName) {
		this.branchName = branchName;
	}

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Integer getUpperBranchId() {
		return upperBranchId;
	}

	public void setUpperBranchId(Integer upperBranchId) {
		this.upperBranchId = upperBranchId;
	}

	public Short getStartTimeHour() {
		return startTimeHour;
	}

	public void setStartTimeHour(Short startTimeHour) {
		this.startTimeHour = startTimeHour;
	}

	public Short getStartTimeMinute() {
		return startTimeMinute;
	}

	public void setStartTimeMinute(Short startTimeMinute) {
		this.startTimeMinute = startTimeMinute;
	}

	public Short getEndTimeHour() {
		return endTimeHour;
	}

	public void setEndTimeHour(Short endTimeHour) {
		this.endTimeHour = endTimeHour;
	}

	public Short getEndTimeMinute() {
		return endTimeMinute;
	}

	public void setEndTimeMinute(Short endTimeMinute) {
		this.endTimeMinute = endTimeMinute;
	}

	public String getCreateUser() {
		return createUser;
	}

	public void setCreateUser(String createUser) {
		this.createUser = createUser;
	}

	public String getUpdateUser() {
		return updateUser;
	}

	public void setUpdateUser(String updateUser) {
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
