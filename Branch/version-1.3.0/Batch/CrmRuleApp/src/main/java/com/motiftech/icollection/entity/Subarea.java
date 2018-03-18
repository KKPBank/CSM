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
@Table(name = "TB_M_SUBAREA")
public class Subarea implements Serializable{

	private static final long serialVersionUID = 2909294981952259410L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SUBAREA_ID", nullable = false, insertable = true, updatable = true)
	private Integer subareaId;
	
	@Column(name = "SUBAREA_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String subareaName;

	@Column(name = "SUBAREA_IS_ACTIVE", nullable = false, columnDefinition = "numeric(1,0)")
	private Boolean subareaIsActive;
	
	@Column(name = "CREATE_USER", nullable = true)
	private Integer createUser;

	@Column(name = "UPDATE_USER", nullable = true)
	private Integer updateUser;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "CREATE_DATE", nullable = true)
	private Date createDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "UPDATE_DATE", nullable = true)
	private Date updateDate;

	public Integer getSubareaId() {
		return subareaId;
	}

	public void setSubareaId(Integer subareaId) {
		this.subareaId = subareaId;
	}

	public String getSubareaName() {
		return subareaName;
	}

	public void setSubareaName(String subareaName) {
		this.subareaName = subareaName;
	}

	public Boolean getSubareaIsActive() {
		return subareaIsActive;
	}

	public void setSubareaIsActive(Boolean subareaIsActive) {
		this.subareaIsActive = subareaIsActive;
	}

	public Integer getCreateUser() {
		return createUser;
	}

	public void setCreateUser(Integer createUser) {
		this.createUser = createUser;
	}

	public Integer getUpdateUser() {
		return updateUser;
	}

	public void setUpdateUser(Integer updateUser) {
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
