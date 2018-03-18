package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_M_COMPLAINT_ROOT_CAUSE")
public class ComplaintRootCause implements Serializable{

	private static final long serialVersionUID = 4277306026312450341L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "ROOT_CAUSE_ID", nullable = false, insertable = true, updatable = true)
	private Integer rootCauseId;
	
	@Column(name = "ROOT_CAUSE_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String  rootCauseName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public Integer getRootCauseId() {
		return rootCauseId;
	}

	public void setRootCauseId(Integer rootCauseId) {
		this.rootCauseId = rootCauseId;
	}

	public String getRootCauseName() {
		return rootCauseName;
	}

	public void setRootCauseName(String rootCauseName) {
		this.rootCauseName = rootCauseName;
	}

}
