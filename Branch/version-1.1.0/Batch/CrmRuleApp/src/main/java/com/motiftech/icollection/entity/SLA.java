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
@Table(name = "TB_M_SLA")
public class SLA implements Serializable {

	private static final long serialVersionUID = 6215301436769566007L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SLA_ID", nullable = false, insertable = true, updatable = true)
	private Integer slaId;

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

	@Column(name = "CHANNEL_ID", nullable = false)
	private Integer channelId;

	@Column(name = "SR_STATUS_ID", nullable = false)
	private Integer srStatusId;

	@Column(name = "SLA_MINUTE", nullable = false)
	private Integer slaMinute;

	@Column(name = "SLA_TIMES", nullable = false)
	private Integer slaTimes;

	@Column(name = "SLA_DAY", nullable = false)
	private Integer slaDay;
	
	@Column(name = "SLA_IS_ACTIVE", nullable = true, columnDefinition = "numeric(1,0)")
	private Boolean slaIsActive;

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

	public Integer getSlaId() {
		return slaId;
	}

	public void setSlaId(Integer slaId) {
		this.slaId = slaId;
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

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Integer getSrStatusId() {
		return srStatusId;
	}

	public void setSrStatusId(Integer srStatusId) {
		this.srStatusId = srStatusId;
	}

	public Integer getSlaMinute() {
		return slaMinute;
	}

	public void setSlaMinute(Integer slaMinute) {
		this.slaMinute = slaMinute;
	}

	public Integer getSlaTimes() {
		return slaTimes;
	}

	public void setSlaTimes(Integer slaTimes) {
		this.slaTimes = slaTimes;
	}

	public Integer getSlaDay() {
		return slaDay;
	}

	public void setSlaDay(Integer slaDay) {
		this.slaDay = slaDay;
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

	public Boolean getSlaIsActive() {
		return slaIsActive;
	}

	public void setSlaIsActive(Boolean slaIsActive) {
		this.slaIsActive = slaIsActive;
	}

}
