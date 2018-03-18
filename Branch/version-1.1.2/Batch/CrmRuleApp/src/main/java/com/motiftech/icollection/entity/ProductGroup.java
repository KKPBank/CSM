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
@Table(name = "TB_R_PRODUCTGROUP")
public class ProductGroup implements Serializable {

	private static final long serialVersionUID = -9162323797232973760L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "PRODUCTGROUP_ID", nullable = false, insertable = true, updatable = true)
	private Integer productGroupId;

	@Column(name = "PRODUCTGROUP_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String productGroupName;

	@Column(name = "PRODUCTGROUP_CODE", nullable = false, columnDefinition = "varchar(50)")
	private String productGroupCode;
	
	@Column(name = "PRODUCTGROUP_IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean productGroupIsActive;

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

	public Integer getProductGroupId() {
		return productGroupId;
	}

	public void setProductGroupId(Integer productGroupId) {
		this.productGroupId = productGroupId;
	}

	public String getProductGroupName() {
		return productGroupName;
	}

	public void setProductGroupName(String productGroupName) {
		this.productGroupName = productGroupName;
	}

	public String getProductGroupCode() {
		return productGroupCode;
	}

	public void setProductGroupCode(String productGroupCode) {
		this.productGroupCode = productGroupCode;
	}

	public Boolean getProductGroupIsActive() {
		return productGroupIsActive;
	}

	public void setProductGroupIsActive(Boolean productGroupIsActive) {
		this.productGroupIsActive = productGroupIsActive;
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
