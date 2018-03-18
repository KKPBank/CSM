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
@Table(name = "TB_R_USER")
public class User implements Serializable{

	private static final long serialVersionUID = -8559346261413148030L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "USER_ID", nullable = false, insertable = true, updatable = true)
	private Integer userId;
	
	@Column(name = "BRANCH_ID", nullable = true)
	private Integer branchId;

	@Column(name = "SUPERVISOR_ID", nullable = true)
	private Integer supervisorId;
	
	@Column(name = "FIRST_NAME", nullable = true, columnDefinition = "varchar(50)")
	private String firstName;
	
	@Column(name = "LAST_NAME", nullable = true, columnDefinition = "varchar(50)")
	private String lastName;
	
	@Column(name = "USERNAME", nullable = true, columnDefinition = "varchar(50)")
	private String userName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;
	
	@Column(name = "ROLE_ID", nullable = true)
	private Integer roleId;
	
	@Column(name = "EMPLOYEE_CODE", nullable = true, columnDefinition = "char(10)")
	private String employeeCode;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;
	
	@Column(name = "IS_GROUP", nullable = true)
	private Boolean isGroup;
	
	@Column(name = "POSITION_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String positionCode;
	
	@Column(name = "MARKETING_CODE", nullable = true, columnDefinition = "varchar(100)")
	private String margetingCode;

	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(100)")
	private String email;
	
	@Column(name = "ROLE_SALE", nullable = true, columnDefinition = "varchar(100)")
	private String roleSale;
	
	@Column(name = "MARKETING_TEAM", nullable = true, columnDefinition = "varchar(100)")
	private String margetingTeam;
	
	@Column(name = "LINE", nullable = true, columnDefinition = "varchar(100)")
	private String line;
	
	@Column(name = "RANK", nullable = true, columnDefinition = "varchar(100)")
	private String rank;
	
	@Column(name = "EMPLOYEE_TYPE", nullable = true, columnDefinition = "varchar(100)")
	private String employeeType;
	
	@Column(name = "COMPANY_NAME", nullable = true, columnDefinition = "varchar(100)")
	private String companyName;
	
	@Column(name = "TELESALE_TEAM", nullable = true, columnDefinition = "varchar(100)")
	private String telesaleTeam;

	public Integer getUserId() {
		return userId;
	}

	public void setUserId(Integer userId) {
		this.userId = userId;
	}

	public Integer getBranchId() {
		return branchId;
	}

	public void setBranchId(Integer branchId) {
		this.branchId = branchId;
	}

	public Integer getSupervisorId() {
		return supervisorId;
	}

	public void setSupervisorId(Integer supervisorId) {
		this.supervisorId = supervisorId;
	}

	public String getFirstName() {
		return firstName;
	}

	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	public String getLastName() {
		return lastName;
	}

	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	public String getUserName() {
		return userName;
	}

	public void setUserName(String userName) {
		this.userName = userName;
	}

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public Integer getRoleId() {
		return roleId;
	}

	public void setRoleId(Integer roleId) {
		this.roleId = roleId;
	}

	public String getEmployeeCode() {
		return employeeCode;
	}

	public void setEmployeeCode(String employeeCode) {
		this.employeeCode = employeeCode;
	}

	public Date getUpdateDate() {
		return updateDate;
	}

	public void setUpdateDate(Date updateDate) {
		this.updateDate = updateDate;
	}

	public Boolean getIsGroup() {
		return isGroup;
	}

	public void setIsGroup(Boolean isGroup) {
		this.isGroup = isGroup;
	}

	public String getPositionCode() {
		return positionCode;
	}

	public void setPositionCode(String positionCode) {
		this.positionCode = positionCode;
	}

	public String getMargetingCode() {
		return margetingCode;
	}

	public void setMargetingCode(String margetingCode) {
		this.margetingCode = margetingCode;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getRoleSale() {
		return roleSale;
	}

	public void setRoleSale(String roleSale) {
		this.roleSale = roleSale;
	}

	public String getMargetingTeam() {
		return margetingTeam;
	}

	public void setMargetingTeam(String margetingTeam) {
		this.margetingTeam = margetingTeam;
	}

	public String getLine() {
		return line;
	}

	public void setLine(String line) {
		this.line = line;
	}

	public String getRank() {
		return rank;
	}

	public void setRank(String rank) {
		this.rank = rank;
	}

	public String getEmployeeType() {
		return employeeType;
	}

	public void setEmployeeType(String employeeType) {
		this.employeeType = employeeType;
	}

	public String getCompanyName() {
		return companyName;
	}

	public void setCompanyName(String companyName) {
		this.companyName = companyName;
	}

	public String getTelesaleTeam() {
		return telesaleTeam;
	}

	public void setTelesaleTeam(String telesaleTeam) {
		this.telesaleTeam = telesaleTeam;
	}
}
