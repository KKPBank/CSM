package com.motiftech.icollection.service.impl;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.motiftech.icollection.dao.OptionDao;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.service.OptionService;

@Service("optionService")
public class OptionServiceImpl implements OptionService {
	
	private OptionDao optDao;
	
	@Transactional(readOnly=true)
	public Option getOptDescByOption(String type,String code) {
		return optDao.getOptDescByOption(type, code);
	}

	public void setOptDao(OptionDao optDao) {
		this.optDao = optDao;
	}
}
