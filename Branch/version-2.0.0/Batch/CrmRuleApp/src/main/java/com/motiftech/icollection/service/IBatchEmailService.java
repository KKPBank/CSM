package com.motiftech.icollection.service;

import java.util.List;

import com.icollection.bean.AccColleMailBean;

public interface IBatchEmailService {
	public List<AccColleMailBean> getListAccount();
	public void sendEmail(List<AccColleMailBean> listBean);
}
