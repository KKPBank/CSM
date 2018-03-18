package com.motiftech.icollection.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "TB_R_CHANNEL")
public class Channel implements Serializable{

	private static final long serialVersionUID = 5097403699867857553L;

	@Id
	@GeneratedValue(strategy = GenerationType.AUTO)
	@Column(name = "CHANNEL_ID", nullable = false, insertable = true, updatable = true)
	private Integer channelId;
	
	@Column(name = "CHANNEL_NAME", nullable = true, columnDefinition = "varchar(200)")
	private String channelName;
	
	@Column(name = "STATUS", nullable = true)
	private Short status;
	
	@Column(name = "CHANNEL_CODE", nullable = true, columnDefinition = "varchar(10)")
	private String channelCode;
	
	@Column(name = "EMAIL", nullable = true, columnDefinition = "varchar(100)")
	private String email;

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public String getChannelName() {
		return channelName;
	}

	public void setChannelName(String channelName) {
		this.channelName = channelName;
	}

	public Short getStatus() {
		return status;
	}

	public void setStatus(Short status) {
		this.status = status;
	}

	public String getChannelCode() {
		return channelCode;
	}

	public void setChannelCode(String channelCode) {
		this.channelCode = channelCode;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

}
