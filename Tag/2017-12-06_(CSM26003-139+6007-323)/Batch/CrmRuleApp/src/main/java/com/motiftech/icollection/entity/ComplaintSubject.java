package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_M_COMPLAINT_SUBJECT")
public class ComplaintSubject implements Serializable{

	private static final long serialVersionUID = -6505517913058005890L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "COMPLAINT_SUBJECT_ID", nullable = false, insertable = true, updatable = true)
	private Integer complaintSubjectId;
	
	@Column(name = "COMPLAINT_SUBJECT_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String complaintSubjectName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public Integer getComplaintSubjectId() {
		return complaintSubjectId;
	}

	public void setComplaintSubjectId(Integer complaintSubjectId) {
		this.complaintSubjectId = complaintSubjectId;
	}

	public String getComplaintSubjectName() {
		return complaintSubjectName;
	}

	public void setComplaintSubjectName(String complaintSubjectName) {
		this.complaintSubjectName = complaintSubjectName;
	}
	
}
