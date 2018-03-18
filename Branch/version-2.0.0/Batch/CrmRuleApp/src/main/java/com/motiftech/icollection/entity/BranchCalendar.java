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
@Table(name="TB_R_BRANCH_CALENDAR")
public class BranchCalendar implements Serializable {
	

	private static final long serialVersionUID = -2939410021173282731L;
	
	@Id
	@GeneratedValue(strategy = GenerationType.AUTO )
	@Column(name = "BRANCH_CALENDAR_ID", nullable = false,insertable = true, updatable = true)
	private Integer branchCalendarId;
	
	@Column(name = "BRANCH_ID", nullable = false)
	private Integer branchId;
	
	@Temporal(TemporalType.DATE)
    @Column(name = "HOLIDAY_DATE", nullable = false)
	private Date holidayDate;
	
	@Column(name = "HOLIDAY_DESC", nullable = true, columnDefinition="varchar(500)")
	private String holidayDesc;
	
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

	public Integer getBranchCalendarId() {
		return branchCalendarId;
	}

	public void setBranchCalendarId(Integer branchCalendarId) {
		this.branchCalendarId = branchCalendarId;
	}

	public Integer getBranchId() {
		return branchId;
	}

	public void setBranchId(Integer branchId) {
		this.branchId = branchId;
	}

	public Date getHolidayDate() {
		return holidayDate;
	}

	public void setHolidayDate(Date holidayDate) {
		this.holidayDate = holidayDate;
	}

	public String getHolidayDesc() {
		return holidayDesc;
	}

	public void setHolidayDesc(String holidayDesc) {
		this.holidayDesc = holidayDesc;
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
