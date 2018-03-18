package com.motiftech.icollection.dao;

import java.util.List;

import com.motiftech.icollection.entity.PrepareEmail;

public interface PrepareEmailDao {
	public List<PrepareEmail> getDataEmail();
	public PrepareEmail getPrepareEmail(Integer srPrepareEmailId);
	public void updatePrepareEmail(PrepareEmail prepareEmail);
	public void savePrepareEmail(PrepareEmail prepareEmail);
}
