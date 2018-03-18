package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_M_COMPLAINT_TYPE")
public class ComplaintType implements Serializable{

	private static final long serialVersionUID = 4521178376521922798L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "COMPLAINT_TYPE_ID", nullable = false, insertable = true, updatable = true)
	private Integer complaintTypeId;
	
	@Column(name = "COMPLAINT_TYPE_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String complaintTypeName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public Integer getComplaintTypeId() {
		return complaintTypeId;
	}

	public void setComplaintTypeId(Integer complaintTypeId) {
		this.complaintTypeId = complaintTypeId;
	}

	public String getComplaintTypeName() {
		return complaintTypeName;
	}

	public void setComplaintTypeName(String complaintTypeName) {
		this.complaintTypeName = complaintTypeName;
	}
	
}
