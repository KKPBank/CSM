package com.motiftech.icollection.service;

import java.util.List;

import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.BranchCalendar;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.Type;
import com.motiftech.icollection.model.Account;

public interface AccountService {	
	public SR 		consolidate(Integer customerId);
	public Type 	getTypeByName(String typeName);
	public void 	updateSR(Integer srId, Account acc);
	public void 	updateSRStatus(Integer srId, Account acc);
	public Option 	getWorkTime(String type, String code);
	public Long 	countSRByAssignFlag(String assignFlag);
	public Long 	countSRFromView();
	public void 	sendEmail(Integer srId, String template, Account account);
	public void 	sendDelegateEmail(Integer srId, String template, Account account);
	public void 	sendSLAEmail(Integer srId, String template, Account account);
	public Branch 	getBranch(Integer branchId);
	public void 	sendSuperVisorEmail(Integer srId, String template, Account account);
	public MapProduct 	checkMapProduct(KeysBean keysBean);
	public AutoForward 	autoForward(KeysBean keysBean, Integer channelId);
	public List<Account> listSR(int startIndex, int maxSize, List<BranchCalendar> branchCalendars, String linkEmail, String assignFlag);
	public List<Account> listSRFromView(int startIndex, int maxSize, List<BranchCalendar> branchCalendars, String linkEmail);
}
