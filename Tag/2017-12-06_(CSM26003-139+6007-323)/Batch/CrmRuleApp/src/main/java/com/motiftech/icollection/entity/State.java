package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_C_SR_STATE")
public class State implements Serializable{

	private static final long serialVersionUID = -2090792609726116580L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "SR_STATE_ID", nullable = false, insertable = true, updatable = true)
	private Integer srStateId;
	
	@Column(name = "SR_STATE_NAME", nullable = false, columnDefinition = "varchar(500)")
	private String srStateName;

	public Integer getSrStateId() {
		return srStateId;
	}

	public void setSrStateId(Integer srStateId) {
		this.srStateId = srStateId;
	}

	public String getSrStateName() {
		return srStateName;
	}

	public void setSrStateName(String srStateName) {
		this.srStateName = srStateName;
	}

}
