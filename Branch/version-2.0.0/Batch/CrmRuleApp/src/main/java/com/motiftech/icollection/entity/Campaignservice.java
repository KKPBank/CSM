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
@Table(name = "TB_R_CAMPAIGNSERVICE")
public class Campaignservice implements Serializable{
	
	private static final long serialVersionUID = -6342938358973929943L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "CAMPAIGNSERVICE_ID",  nullable = false, insertable = true, updatable = true)
	private Integer campaignserviceId;

	@Column(name = "CAMPAIGNSERVICE_NAME", nullable = false, columnDefinition = "varchar(100)")
	private String campaignserviceName;
	
	@Column(name = "CAMPAIGNSERVICE_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String campaignserviceCode;
	
	@Column(name = "PRODUCT_ID", nullable = false)
	private Integer productId;
	
	@Column(name = "CAMPAIGNSERVICE_IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean campaignserviceIsActive;
	
	@Column(name = "CREATE_USER", nullable = true, columnDefinition = "varchar(100)")
	private String createUser;

	@Column(name = "UPDATE_USER", nullable = true, columnDefinition = "varchar(100)")
	private String updateUser;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	public Integer getCampaignserviceId() {
		return campaignserviceId;
	}

	public void setCampaignserviceId(Integer campaignserviceId) {
		this.campaignserviceId = campaignserviceId;
	}

	public String getCampaignserviceName() {
		return campaignserviceName;
	}

	public void setCampaignserviceName(String campaignserviceName) {
		this.campaignserviceName = campaignserviceName;
	}

	public Integer getProductId() {
		return productId;
	}

	public void setProductId(Integer productId) {
		this.productId = productId;
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

	public String getCampaignserviceCode() {
		return campaignserviceCode;
	}

	public void setCampaignserviceCode(String campaignserviceCode) {
		this.campaignserviceCode = campaignserviceCode;
	}

	public Boolean getCampaignserviceIsActive() {
		return campaignserviceIsActive;
	}

	public void setCampaignserviceIsActive(Boolean campaignserviceIsActive) {
		this.campaignserviceIsActive = campaignserviceIsActive;
	}
	

}
