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
@Table(name = "TB_M_MAP_AUTO_FORWARD")
public class AutoForward implements Serializable {

	private static final long serialVersionUID = 7401602121052267828L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "AUTO_FORWARD_ID", nullable = false, insertable = true, updatable = true)
	private Integer autoForwardId;

	@Column(name = "CHANNEL_ID", nullable = true)
	private Integer channelId;

	@Column(name = "FORWARD_TO_USER", nullable = false)
	private Integer forwardToUser;
	
	@Column(name = "PRODUCT_ID", nullable = true)
	private Integer productId;
	
	@Column(name = "CAMPAIGNSERVICE_ID", nullable = true)
	private Integer campaignserviceId;

	@Column(name = "AREA_ID", nullable = true)
	private Integer areaId;

	@Column(name = "SUBAREA_ID", nullable = true)
	private Integer subareaId;

	@Column(name = "TYPE_ID", nullable = true)
	private Integer typeId;

	@Column(name = "IS_VERIFY", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean isVerify;

	@Column(name = "IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean isActive;

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

	public Integer getAutoForwardId() {
		return autoForwardId;
	}

	public void setAutoForwardId(Integer autoForwardId) {
		this.autoForwardId = autoForwardId;
	}

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Integer getForwardToUser() {
		return forwardToUser;
	}

	public void setForwardToUser(Integer forwardToUser) {
		this.forwardToUser = forwardToUser;
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

	public Boolean getIsVerify() {
		return isVerify;
	}

	public void setIsVerify(Boolean isVerify) {
		this.isVerify = isVerify;
	}

	public Boolean getIsActive() {
		return isActive;
	}

	public void setIsActive(Boolean isActive) {
		this.isActive = isActive;
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
