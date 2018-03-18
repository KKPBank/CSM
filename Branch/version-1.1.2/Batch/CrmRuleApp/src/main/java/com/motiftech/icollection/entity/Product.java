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
@Table(name = "TB_R_PRODUCT")
public class Product implements Serializable {

	private static final long serialVersionUID = 4621776116740270738L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "PRODUCT_ID", nullable = false, insertable = true, updatable = true)
	private Integer productId;

	@Column(name = "PRODUCT_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String productName;
	
	@Column(name = "PRODUCTGROUP_ID", nullable = false)
	private Integer productGroupId;
	
	@Column(name = "PRODUCT_IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean productIsActive;
	
	@Column(name = "PRODUCT_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String productCode;
	
	@Column(name = "PRODUCT_TYPE", nullable = true, columnDefinition = "varchar(1)")
	private String productType;
	
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

	public Integer getProductId() {
		return productId;
	}

	public void setProductId(Integer productId) {
		this.productId = productId;
	}

	public String getProductName() {
		return productName;
	}

	public void setProductName(String productName) {
		this.productName = productName;
	}

	public Integer getProductGroupId() {
		return productGroupId;
	}

	public void setProductGroupId(Integer productGroupId) {
		this.productGroupId = productGroupId;
	}

	public Boolean getProductIsActive() {
		return productIsActive;
	}

	public void setProductIsActive(Boolean productIsActive) {
		this.productIsActive = productIsActive;
	}

	public String getProductCode() {
		return productCode;
	}

	public void setProductCode(String productCode) {
		this.productCode = productCode;
	}

	public String getProductType() {
		return productType;
	}

	public void setProductType(String productType) {
		this.productType = productType;
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
