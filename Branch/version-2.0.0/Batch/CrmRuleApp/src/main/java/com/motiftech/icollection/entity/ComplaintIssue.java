package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_M_COMPLAINT_ISSUES")
public class ComplaintIssue implements Serializable{

	private static final long serialVersionUID = 6487987800901910784L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "COMPLAINT_ISSUES_ID", nullable = false, insertable = true, updatable = true)
	private Integer complaintIssueId;
	
	@Column(name = "COMPLAINT_ISSUES_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String complaintIssueName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;

	public Integer getComplaintIssueId() {
		return complaintIssueId;
	}

	public void setComplaintIssueId(Integer complaintIssueId) {
		this.complaintIssueId = complaintIssueId;
	}

	public String getComplaintIssueName() {
		return complaintIssueName;
	}

	public void setComplaintIssueName(String complaintIssueName) {
		this.complaintIssueName = complaintIssueName;
	}

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}
	
}
