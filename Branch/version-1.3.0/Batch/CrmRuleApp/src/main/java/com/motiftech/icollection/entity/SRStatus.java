package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_C_SR_STATUS")
public class SRStatus implements Serializable{

	private static final long serialVersionUID = -1241198271092023572L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_STATUS_ID", nullable = false, insertable = true, updatable = true)
	private Integer srStatusId;
	
	@Column(name = "SR_STATUS_CODE", nullable = false, columnDefinition = "varchar(50)")
	private String srStatusCode;
	
	@Column(name = "SR_STATUS_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String srStatusName;

	public Integer getSrStatusId() {
		return srStatusId;
	}

	public void setSrStatusId(Integer srStatusId) {
		this.srStatusId = srStatusId;
	}

	public String getSrStatusCode() {
		return srStatusCode;
	}

	public void setSrStatusCode(String srStatusCode) {
		this.srStatusCode = srStatusCode;
	}

	public String getSrStatusName() {
		return srStatusName;
	}

	public void setSrStatusName(String srStatusName) {
		this.srStatusName = srStatusName;
	}
	

}
