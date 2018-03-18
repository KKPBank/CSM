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
@Table(name = "TB_M_HR_EMPLOYEE")
public class HREmployee implements Serializable{

	private static final long serialVersionUID = -1668459275724633860L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "EMP_ID", nullable = false, insertable = true, updatable = true)
	private Integer empId;
	
	@Column(name = "EMPLOYEEID", nullable = true, columnDefinition = "varchar(10)")
	private String employeeId;
	
	@Column(name = "TFNAME", nullable = true, columnDefinition = "varchar(30)")
	private String tfName;
	
	@Column(name = "TLNAME", nullable = true, columnDefinition = "varchar(40)")
	private String tlName;
	
	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(50)")
	private String email;
	
	@Column(name = "BOSS", nullable = true, columnDefinition = "varchar(20)")
	private String boss;
	
	@Column(name = "BOSSNAME", nullable = true, columnDefinition = "varchar(71)")
	private String bossName;
	
	@Column(name = "BOSS_EMAIL", nullable = true, columnDefinition = "varchar(50)")
	private String bossEmail;
	
	@Column(name = "ASSESSOR1", nullable = true, columnDefinition = "varchar(10)")
	private String assessor1;
	
	@Column(name = "ASSESSOR1NAME", nullable = true, columnDefinition = "varchar(71)")
	private String assessor1Name;
	
	@Column(name = "ASSESSOR1_EMAIL", nullable = true, columnDefinition = "varchar(50)")
	private String assessor1Email;
	
	@Column(name = "ASSESSOR2", nullable = true, columnDefinition = "varchar(10)")
	private String assessor2;
	
	@Column(name = "ASSESSOR2NAME", nullable = true, columnDefinition = "varchar(71)")
	private String assessor2Name;
	
	@Column(name = "ASSESSOR2_EMAIL", nullable = true, columnDefinition = "varchar(50)")
	private String assessor2Email;
	
	@Column(name = "ASSESSOR3", nullable = true, columnDefinition = "varchar(10)")
	private String assessor3;
	
	@Column(name = "ASSESSOR3NAME", nullable = true, columnDefinition = "varchar(71)")
	private String assessor3Name;
	
	@Column(name = "ASSESSOR3_EMAIL", nullable = true, columnDefinition = "varchar(50)")
	private String assessor3Email;
	
	@Column(name = "ADDITIONJOB", nullable = true, columnDefinition = "varchar(15)")
	private String additionJob;
	
	@Column(name = "STATUS", nullable = true, columnDefinition = "varchar(1)")
	private String status;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;
	
	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;
	
	@Column(name = "IS_DELETE",nullable = false, columnDefinition="numeric(1,0)")
	private boolean isDelete;

	public Integer getEmpId() {
		return empId;
	}

	public void setEmpId(Integer empId) {
		this.empId = empId;
	}

	public String getEmployeeId() {
		return employeeId;
	}

	public void setEmployeeId(String employeeId) {
		this.employeeId = employeeId;
	}

	public String getTfName() {
		return tfName;
	}

	public void setTfName(String tfName) {
		this.tfName = tfName;
	}

	public String getTlName() {
		return tlName;
	}

	public void setTlName(String tlName) {
		this.tlName = tlName;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getBoss() {
		return boss;
	}

	public void setBoss(String boss) {
		this.boss = boss;
	}

	public String getBossName() {
		return bossName;
	}

	public void setBossName(String bossName) {
		this.bossName = bossName;
	}

	public String getBossEmail() {
		return bossEmail;
	}

	public void setBossEmail(String bossEmail) {
		this.bossEmail = bossEmail;
	}

	public String getAssessor1() {
		return assessor1;
	}

	public void setAssessor1(String assessor1) {
		this.assessor1 = assessor1;
	}

	public String getAssessor1Name() {
		return assessor1Name;
	}

	public void setAssessor1Name(String assessor1Name) {
		this.assessor1Name = assessor1Name;
	}

	public String getAssessor1Email() {
		return assessor1Email;
	}

	public void setAssessor1Email(String assessor1Email) {
		this.assessor1Email = assessor1Email;
	}

	public String getAssessor2() {
		return assessor2;
	}

	public void setAssessor2(String assessor2) {
		this.assessor2 = assessor2;
	}

	public String getAssessor2Name() {
		return assessor2Name;
	}

	public void setAssessor2Name(String assessor2Name) {
		this.assessor2Name = assessor2Name;
	}

	public String getAssessor2Email() {
		return assessor2Email;
	}

	public void setAssessor2Email(String assessor2Email) {
		this.assessor2Email = assessor2Email;
	}

	public String getAssessor3() {
		return assessor3;
	}

	public void setAssessor3(String assessor3) {
		this.assessor3 = assessor3;
	}

	public String getAssessor3Name() {
		return assessor3Name;
	}

	public void setAssessor3Name(String assessor3Name) {
		this.assessor3Name = assessor3Name;
	}

	public String getAssessor3Email() {
		return assessor3Email;
	}

	public void setAssessor3Email(String assessor3Email) {
		this.assessor3Email = assessor3Email;
	}

	public String getAdditionJob() {
		return additionJob;
	}

	public void setAdditionJob(String additionJob) {
		this.additionJob = additionJob;
	}

	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
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

	public boolean isDelete() {
		return isDelete;
	}

	public void setDelete(boolean isDelete) {
		this.isDelete = isDelete;
	}
}
