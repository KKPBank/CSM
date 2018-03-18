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
@Table(name = "TB_M_MAP_PRODUCT")
public class MapProduct implements Serializable {

	private static final long serialVersionUID = 4956711952092057764L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "MAP_PRODUCT_ID", nullable = false, insertable = true, updatable = true)
	private Integer mapProductId;

	@Column(name = "PRODUCT_ID", nullable = false)
	private Integer productId;

	@Column(name = "CAMPAIGNSERVICE_ID", nullable = true)
	private Integer campaignserviceId;

	@Column(name = "AREA_ID", nullable = false)
	private Integer areaId;

	@Column(name = "SUBAREA_ID", nullable = false)
	private Integer subareaId;

	@Column(name = "TYPE_ID", nullable = false)
	private Integer typeId;

	@Column(name = "DEFAULT_OWNER_USER_ID", nullable = true)
	private Integer defaultOwnerUserId;

	@Column(name = "SR_PAGE_ID", nullable = false)
	private Integer srPageId;

	@Column(name = "MAP_PRODUCT_IS_VERIFY", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean mapProductIsVerify;

	@Column(name = "MAP_PRODUCT_IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean mapProductIsActive;

	@Column(name = "HP_SUBJECT", nullable = true, columnDefinition = "varchar(500)")
	private String hpSubject;
	
	@Column(name = "HP_LANGUAGE_INDEPENDENT_CODE", nullable = true, columnDefinition = "varchar(500)")
	private String hpLanguageIndependentCode;
	
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

	public Integer getMapProductId() {
		return mapProductId;
	}

	public void setMapProductId(Integer mapProductId) {
		this.mapProductId = mapProductId;
	}

	public Integer getProductId() {
		return productId;
	}

	public void setProductId(Integer productId) {
		this.productId = productId;
	}

	public Integer getCampaignserviceId() {
		return campaignserviceId;
	}

	public void setCampaignserviceId(Integer campaignserviceId) {
		this.campaignserviceId = campaignserviceId;
	}

	public Integer getAreaId() {
		return areaId;
	}

	public void setAreaId(Integer areaId) {
		this.areaId = areaId;
	}

	public Integer getSubareaId() {
		return subareaId;
	}

	public void setSubareaId(Integer subareaId) {
		this.subareaId = subareaId;
	}

	public Integer getTypeId() {
		return typeId;
	}

	public void setTypeId(Integer typeId) {
		this.typeId = typeId;
	}

	public Integer getDefaultOwnerUserId() {
		return defaultOwnerUserId;
	}

	public void setDefaultOwnerUserId(Integer defaultOwnerUserId) {
		this.defaultOwnerUserId = defaultOwnerUserId;
	}

	public Integer getSrPageId() {
		return srPageId;
	}

	public void setSrPageId(Integer srPageId) {
		this.srPageId = srPageId;
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

	public Boolean getMapProductIsVerify() {
		return mapProductIsVerify;
	}

	public void setMapProductIsVerify(Boolean mapProductIsVerify) {
		this.mapProductIsVerify = mapProductIsVerify;
	}

	public Boolean getMapProductIsActive() {
		return mapProductIsActive;
	}

	public void setMapProductIsActive(Boolean mapProductIsActive) {
		this.mapProductIsActive = mapProductIsActive;
	}

	public String getHpSubject() {
		return hpSubject;
	}

	public void setHpSubject(String hpSubject) {
		this.hpSubject = hpSubject;
	}

	public String getHpLanguageIndependentCode() {
		return hpLanguageIndependentCode;
	}

	public void setHpLanguageIndependentCode(String hpLanguageIndependentCode) {
		this.hpLanguageIndependentCode = hpLanguageIndependentCode;
	}

}
