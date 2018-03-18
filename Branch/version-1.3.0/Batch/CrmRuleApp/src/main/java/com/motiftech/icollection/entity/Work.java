package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_T_SR_RULE_WORK")
public class Work implements Serializable{

	private static final long serialVersionUID = -435932751286333216L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "RULE_WORK_ID",  nullable = false, insertable = true, updatable = true)
	private Integer ruleWorkId;
	
	@Column(name = "RULE_WORK_CODE", nullable = true, columnDefinition = "varchar(50)")
	private String ruleWorkCode;
	
	@Column(name = "RULE_WORK_TEMPLATE", nullable = true, columnDefinition = "varchar(50)")
	private String ruleWorkTemplate;
	
	@Column(name = "RULE_WORK_DESC", nullable = false, columnDefinition = "varchar(500)")
	private String ruleWorkDesc;

	public Integer getRuleWorkId() {
		return ruleWorkId;
	}

	public void setRuleWorkId(Integer ruleWorkId) {
		this.ruleWorkId = ruleWorkId;
	}

	public String getRuleWorkCode() {
		return ruleWorkCode;
	}

	public void setRuleWorkCode(String ruleWorkCode) {
		this.ruleWorkCode = ruleWorkCode;
	}

	public String getRuleWorkTemplate() {
		return ruleWorkTemplate;
	}

	public void setRuleWorkTemplate(String ruleWorkTemplate) {
		this.ruleWorkTemplate = ruleWorkTemplate;
	}

	public String getRuleWorkDesc() {
		return ruleWorkDesc;
	}

	public void setRuleWorkDesc(String ruleWorkDesc) {
		this.ruleWorkDesc = ruleWorkDesc;
	}
	
}
