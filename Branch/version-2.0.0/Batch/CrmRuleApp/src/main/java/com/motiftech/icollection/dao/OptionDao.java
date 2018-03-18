package com.motiftech.icollection.dao;

import com.motiftech.icollection.entity.Option;

public interface OptionDao {
	public  Option getOptDescByOption(String type,String code);
}
