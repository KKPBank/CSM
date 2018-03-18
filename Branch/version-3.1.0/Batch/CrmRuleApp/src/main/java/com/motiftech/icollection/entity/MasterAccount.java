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

@Entity
@Table(name = "TB_M_ACCOUNT")
public class MasterAccount implements Serializable{

	private static final long serialVersionUID = -6889226453876095881L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "ACCOUNT_ID",  nullable = false, insertable = true, updatable = true)
	private Integer accountId;
	
	@Column(name = "CUSTOMER_ID", nullable = true)
	private Integer customerId;
	
	@Column(name = "PRODUCT_GROUP", nullable = true, columnDefinition = "varchar(50)")
	private String productGroup;
	
	@Column(name = "PRODUCT", nullable = true, columnDefinition = "varchar(50)")
	private String product;
	
	@Column(name = "ACCOUNT_NO", nullable = true, columnDefinition = "varchar(255)")
	private String accountNo;
	
	@Column(name = "BRANCH_NAME", nullable = true, columnDefinition = "varchar(255)")
	private String branchName;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "EFFECTIVE_DATE", nullable = true)
	private Date effectiveDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "EXPIRY_DATE", nullable = true)
	private Date expiryDate;
	
	@Column(name = "STATUS", nullable = true, columnDefinition = "varchar(10)")
	private String status;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;
	
	@Column(name = "CAR_NO", nullable = true, columnDefinition = "varchar(255)")
	private String carNo;
	
	@Column(name = "GRADE", nullable = true, columnDefinition = "varchar(255)")
	private String grade;
	
	@Column(name = "KKCIS_ID", nullable = true, columnDefinition = "decimal", precision = 38, scale = 0)
	private BigDecimal kkcisId; 
	
	@Column(name = "PRODUCT_DESC", nullable = true, columnDefinition = "varchar(255)")
	private String productDesc;
	
	@Column(name = "SUBSCRIPTION_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String subscriptionCode;
	
	@Column(name = "SUBSCRIPTION_DESC", nullable = true, columnDefinition = "varchar(255)")
	private String subscriptionDesc;


	public Integer getCustomerId() {
		return customerId;
	}

	public void setCustomerId(Integer customerId) {
		this.customerId = customerId;
	}

	public String getProductGroup() {
		return productGroup;
	}

	public void setProductGroup(String productGroup) {
		this.productGroup = productGroup;
	}

	public String getProduct() {
		return product;
	}

	public void setProduct(String product) {
		this.product = product;
	}

	public String getAccountNo() {
		return accountNo;
	}

	public void setAccountNo(String accountNo) {
		this.accountNo = accountNo;
	}

	public String getBranchName() {
		return branchName;
	}

	public void setBranchName(String branchName) {
		this.branchName = branchName;
	}

	public Date getEffectiveDate() {
		return effectiveDate;
	}

	public void setEffectiveDate(Date effectiveDate) {
		this.effectiveDate = effectiveDate;
	}

	public Date getExpiryDate() {
		return expiryDate;
	}

	public void setExpiryDate(Date expiryDate) {
		this.expiryDate = expiryDate;
	}

	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}

	public Date getUpdateDate() {
		return updateDate;
	}

	public void setUpdateDate(Date updateDate) {
		this.updateDate = updateDate;
	}

	public String getCarNo() {
		return carNo;
	}

	public void setCarNo(String carNo) {
		this.carNo = carNo;
	}

	public String getGrade() {
		return grade;
	}

	public void setGrade(String grade) {
		this.grade = grade;
	}

	public BigDecimal getKkcisId() {
		return kkcisId;
	}

	public void setKkcisId(BigDecimal kkcisId) {
		this.kkcisId = kkcisId;
	}

	public String getProductDesc() {
		return productDesc;
	}

	public void setProductDesc(String productDesc) {
		this.productDesc = productDesc;
	}

	public String getSubscriptionCode() {
		return subscriptionCode;
	}

	public void setSubscriptionCode(String subscriptionCode) {
		this.subscriptionCode = subscriptionCode;
	}

	public String getSubscriptionDesc() {
		return subscriptionDesc;
	}

	public void setSubscriptionDesc(String subscriptionDesc) {
		this.subscriptionDesc = subscriptionDesc;
	}

	public Integer getAccountId() {
		return accountId;
	}

	public void setAccountId(Integer accountId) {
		this.accountId = accountId;
	}

}
