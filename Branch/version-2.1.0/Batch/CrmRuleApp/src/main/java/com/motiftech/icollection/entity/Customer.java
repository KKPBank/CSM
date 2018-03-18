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
@Table(name = "TB_M_CUSTOMER")
public class Customer implements Serializable{
	
	private static final long serialVersionUID = 1311387872454434112L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "CUSTOMER_ID",  nullable = false, insertable = true, updatable = true)
	private Integer customerId;
	
	@Column(name = "TYPE", nullable = true)
	private short type;
	
	@Column(name = "SUBSCRIPT_TYPE_ID", nullable = true)
	private Integer subscriptTypeId;
	
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
	
	@Column(name = "FIRST_NAME_TH", nullable = true, columnDefinition = "varchar(100)")
	private String firstNameTh;
	
	@Column(name = "FIRST_NAME_EN", nullable = true, columnDefinition = "varchar(100)")
	private String firstNameEn;
	
	@Column(name = "LAST_NAME_TH", nullable = true, columnDefinition = "varchar(100)")
	private String lastNameTh;
	
	@Column(name = "LAST_NAME_EN", nullable = true, columnDefinition = "varchar(100)")
	private String lastNameEn;
	
	@Column(name = "CARD_NO", nullable = true, columnDefinition = "varchar(50)")
	private String cardNo;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "BIRTH_DATE", nullable = true)
	private Date birthDate;
	
	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(100)")
	private String email;
	
	@Column(name = "TITLE_TH_ID", nullable = true)
	private Integer titleThId;
	
	@Column(name = "TITLE_EN_ID", nullable = true)
	private Integer titleEnId;
	
	@Column(name = "EMPLOYEE_ID", nullable = true)
	private Integer employeeID;
	
	@Column(name = "FAX", nullable = true, columnDefinition = "varchar(100)")
	private String fax;

	public Integer getCustomerId() {
		return customerId;
	}

	public void setCustomerId(Integer customerId) {
		this.customerId = customerId;
	}

	public short getType() {
		return type;
	}

	public void setType(short type) {
		this.type = type;
	}

	public Integer getSubscriptTypeId() {
		return subscriptTypeId;
	}

	public void setSubscriptTypeId(Integer subscriptTypeId) {
		this.subscriptTypeId = subscriptTypeId;
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

	public String getFirstNameTh() {
		return firstNameTh;
	}

	public void setFirstNameTh(String firstNameTh) {
		this.firstNameTh = firstNameTh;
	}

	public String getFirstNameEn() {
		return firstNameEn;
	}

	public void setFirstNameEn(String firstNameEn) {
		this.firstNameEn = firstNameEn;
	}

	public String getLastNameTh() {
		return lastNameTh;
	}

	public void setLastNameTh(String lastNameTh) {
		this.lastNameTh = lastNameTh;
	}

	public String getLastNameEn() {
		return lastNameEn;
	}

	public void setLastNameEn(String lastNameEn) {
		this.lastNameEn = lastNameEn;
	}

	public String getCardNo() {
		return cardNo;
	}

	public void setCardNo(String cardNo) {
		this.cardNo = cardNo;
	}

	public Date getBirthDate() {
		return birthDate;
	}

	public void setBirthDate(Date birthDate) {
		this.birthDate = birthDate;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public Integer getTitleThId() {
		return titleThId;
	}

	public void setTitleThId(Integer titleThId) {
		this.titleThId = titleThId;
	}

	public Integer getTitleEnId() {
		return titleEnId;
	}

	public void setTitleEnId(Integer titleEnId) {
		this.titleEnId = titleEnId;
	}

	public Integer getEmployeeID() {
		return employeeID;
	}

	public void setEmployeeID(Integer employeeID) {
		this.employeeID = employeeID;
	}

	public String getFax() {
		return fax;
	}

	public void setFax(String fax) {
		this.fax = fax;
	}

}
