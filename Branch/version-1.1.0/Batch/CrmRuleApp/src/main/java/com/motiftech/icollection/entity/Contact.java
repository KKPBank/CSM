package com.motiftech.icollection.entity;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

/**
 * 
 * @author Neda Peyrone
 *
 */
@Entity
@Table(name = "TB_M_CONTACT")
public class Contact implements Serializable {

	private static final long serialVersionUID = -5181565681170724665L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "CONTACT_ID", nullable = false, insertable = true, updatable = true)
	private Integer contactId;

	@Column(name = "FIRST_NAME_TH", nullable = true, columnDefinition = "varchar(255)")
	private String firstNameTh;

	@Column(name = "LAST_NAME_TH", nullable = true, columnDefinition = "varchar(255)")
	private String lastNameTh;

	@Column(name = "FIRST_NAME_EN", nullable = true, columnDefinition = "varchar(255)")
	private String firstNameEn;

	@Column(name = "LAST_NAME_EN", nullable = true, columnDefinition = "varchar(255)")
	private String lastNameEn;

	@Column(name = "CARD_NO", nullable = true, columnDefinition = "varchar(100)")
	private String cardNo;

	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(100)")
	private String email;

	@Column(name = "SUBSCRIPT_TYPE_ID", nullable = true)
	private Integer subscriptTypeId;

	@Column(name = "TITLE_TH_ID", nullable = true)
	private Integer titleThId;

	@Column(name = "TITLE_EN_ID", nullable = true)
	private Integer titleEnId;

	@Temporal(TemporalType.DATE)
	@Column(name = "BIRTH_DATE", nullable = true)
	private Date birthDate;

	@Column(name = "IS_DEFAULT", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean isDefault;

	@Column(name = "IS_EDIT", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean isEdit;

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

	@Column(name = "KKCIS_ID", nullable = true, columnDefinition = "decimal", precision = 38, scale = 0)
	private BigDecimal kkcisId;

	@Column(name = "CARD_TYPE_CODE", nullable = true, columnDefinition = "varchar(10)")
	private String cardTypeCode;

	public Integer getContactId() {
		return contactId;
	}

	public void setContactId(Integer contactId) {
		this.contactId = contactId;
	}

	public String getFirstNameTh() {
		return firstNameTh;
	}

	public void setFirstNameTh(String firstNameTh) {
		this.firstNameTh = firstNameTh;
	}

	public String getLastNameTh() {
		return lastNameTh;
	}

	public void setLastNameTh(String lastNameTh) {
		this.lastNameTh = lastNameTh;
	}

	public String getFirstNameEn() {
		return firstNameEn;
	}

	public void setFirstNameEn(String firstNameEn) {
		this.firstNameEn = firstNameEn;
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

	public Date getBirthDate() {
		return birthDate;
	}

	public void setBirthDate(Date birthDate) {
		this.birthDate = birthDate;
	}

	public Boolean getIsDefault() {
		return isDefault;
	}

	public void setIsDefault(Boolean isDefault) {
		this.isDefault = isDefault;
	}

	public Boolean getIsEdit() {
		return isEdit;
	}

	public void setIsEdit(Boolean isEdit) {
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

	public BigDecimal getKkcisId() {
		return kkcisId;
	}

	public void setKkcisId(BigDecimal kkcisId) {
		this.kkcisId = kkcisId;
	}

	public String getCardTypeCode() {
		return cardTypeCode;
	}

	public void setCardTypeCode(String cardTypeCode) {
		this.cardTypeCode = cardTypeCode;
	}
}
