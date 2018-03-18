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
@Table(name = "TB_M_CONTACT")
public class Contact implements Serializable{

	private static final long serialVersionUID = -4812997470636812565L;
	
	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "CONTACT_ID", nullable = false, insertable = true, updatable = true)
	private Integer contactId;
	
	@Column(name = "FIRST_NAME_TH", nullable = true, columnDefinition = "varchar(100)")
	private String firstNameTH;
	
	@Column(name = "LAST_NAME_TH", nullable = true, columnDefinition = "varchar(100)")
	private String lastNameTH;
	
	@Column(name = "FIRST_NAME_EN", nullable = true, columnDefinition = "varchar(100)")
	private String firstNameRN;
	
	@Column(name = "LAST_NAME_EN", nullable = true, columnDefinition = "varchar(100)")
	private String lastNameEN;
	
	@Column(name = "CARD_NO", nullable = true, columnDefinition = "varchar(100)")
	private String cardNo;
	
	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(100)")
	private String email;
	
	@Column(name = "SUBSCRIPT_TYPE_ID", nullable = true)
	private Integer subscriptTypeId;
	
	@Column(name = "TITLE_EN_ID", nullable = true)
	private Integer titleENId;
	
	@Column(name = "TITLE_TH_ID", nullable = true)
	private Integer titleTHId;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "BIRTH_DATE", nullable = true)
	private Date birthDate;
	
	@Column(name = "IS_DEFAULT",nullable = false, columnDefinition="numeric(1,0)")
	private boolean isDefault;
	
	@Column(name = "IS_EDIT",nullable = false, columnDefinition="numeric(1,0)")
	private boolean isEdit;
	
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

	public Integer getContactId() {
		return contactId;
	}

	public void setContactId(Integer contactId) {
		this.contactId = contactId;
	}

	public String getFirstNameTH() {
		return firstNameTH;
	}

	public void setFirstNameTH(String firstNameTH) {
		this.firstNameTH = firstNameTH;
	}

	public String getLastNameTH() {
		return lastNameTH;
	}

	public void setLastNameTH(String lastNameTH) {
		this.lastNameTH = lastNameTH;
	}

	public String getFirstNameRN() {
		return firstNameRN;
	}

	public void setFirstNameRN(String firstNameRN) {
		this.firstNameRN = firstNameRN;
	}

	public String getLastNameEN() {
		return lastNameEN;
	}

	public void setLastNameEN(String lastNameEN) {
		this.lastNameEN = lastNameEN;
	}

	public String getCardNo() {
		return cardNo;
	}

	public void setCardNo(String cardNo) {
		this.cardNo = cardNo;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public Integer getSubscriptTypeId() {
		return subscriptTypeId;
	}

	public void setSubscriptTypeId(Integer subscriptTypeId) {
		this.subscriptTypeId = subscriptTypeId;
	}

	public Integer getTitleENId() {
		return titleENId;
	}

	public void setTitleENId(Integer titleENId) {
		this.titleENId = titleENId;
	}

	public Integer getTitleTHId() {
		return titleTHId;
	}

	public void setTitleTHId(Integer titleTHId) {
		this.titleTHId = titleTHId;
	}

	public Date getBirthDate() {
		return birthDate;
	}

	public void setBirthDate(Date birthDate) {
		this.birthDate = birthDate;
	}

	public boolean isDefault() {
		return isDefault;
	}

	public void setDefault(boolean isDefault) {
		this.isDefault = isDefault;
	}

	public boolean isEdit() {
		return isEdit;
	}

	public void setEdit(boolean isEdit) {
		this.isEdit = isEdit;
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
