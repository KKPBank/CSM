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
@Table(name="TB_C_RULE_OPTION")
public class Option implements Serializable {
	
	private static final long serialVersionUID = -6263543854830866122L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "RULE_OPTION_ID",  nullable = false, insertable = true, updatable = true)
	private Integer ruleOptionId;
	
	@Column(name = "OPTION_CODE", nullable = true, columnDefinition="varchar(100)")
	private String optionCode;	
	
	@Column(name = "OPTION_DESC", nullable = true, columnDefinition="varchar(100)")
	private String optionDesc;
	
	@Column(name = "OPTION_TYPE", nullable = true, columnDefinition="varchar(100)")
	private String optionType;
	
	@Column(name = "SEQ", nullable = true)
	private Integer seq;
	
	@Column(name = "OPTION_SUB_CODE", nullable = true, columnDefinition="varchar(100)")
	private String optionSubCode;
	
	@Column(name = "OPTION_CREATE_BY", nullable = true, columnDefinition="varchar(100)")
	private String OptionCreateBy;
	
	@Column(name = "CREATE_BY", nullable = true, columnDefinition="varchar(100)")
	private String createBy;
	
	@Temporal(TemporalType.TIMESTAMP)
    @Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;
	
	@Column(name = "IS_DELETE",nullable = false, columnDefinition="numeric(1,0)")
	private Boolean isDelete;
	
	@Column(name = "UPDATE_BY", nullable = true, columnDefinition="varchar(100)")
	private String updateBy;
	
	@Temporal(TemporalType.TIMESTAMP)
    @Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	@Column(name = "SYSTEM_VIEW", nullable = true, columnDefinition="varchar(50)")
	private String systemView;

	public Integer getRuleOptionId() {
		return ruleOptionId;
	}

	public void setRuleOptionId(Integer ruleOptionId) {
		this.ruleOptionId = ruleOptionId;
	}

	public String getOptionCode() {
		return optionCode;
	}

	public void setOptionCode(String optionCode) {
		this.optionCode = optionCode;
	}

	public String getOptionDesc() {
		return optionDesc;
	}

	public void setOptionDesc(String optionDesc) {
		this.optionDesc = optionDesc;
	}

	public String getOptionType() {
		return optionType;
	}

	public void setOptionType(String optionType) {
		this.optionType = optionType;
	}

	public Integer getSeq() {
		return seq;
	}

	public void setSeq(Integer seq) {
		this.seq = seq;
	}

	public String getOptionSubCode() {
		return optionSubCode;
	}

	public void setOptionSubCode(String optionSubCode) {
		this.optionSubCode = optionSubCode;
	}

	public String getOptionCreateBy() {
		return OptionCreateBy;
	}

	public void setOptionCreateBy(String optionCreateBy) {
		OptionCreateBy = optionCreateBy;
	}

	public String getCreateBy() {
		return createBy;
	}

	public void setCreateBy(String createBy) {
		this.createBy = createBy;
	}

	public Date getCreateDate() {
		return createDate;
	}

	public void setCreateDate(Date createDate) {
		this.createDate = createDate;
	}

	public Boolean getIsDelete() {
		return isDelete;
	}

	public void setIsDelete(Boolean isDelete) {
		this.isDelete = isDelete;
	}

	public String getUpdateBy() {
		return updateBy;
	}

	public void setUpdateBy(String updateBy) {
		this.updateBy = updateBy;
	}

	public Date getUpdateDate() {
		return updateDate;
	}

	public void setUpdateDate(Date updateDate) {
		this.updateDate = updateDate;
	}

	public String getSystemView() {
		return systemView;
	}

	public void setSystemView(String systemView) {
		this.systemView = systemView;
	}
	
}
