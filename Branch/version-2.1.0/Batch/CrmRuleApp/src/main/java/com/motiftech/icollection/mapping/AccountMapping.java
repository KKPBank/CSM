package com.motiftech.icollection.mapping;

import java.util.Calendar;
import java.util.Date;

import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.VW_SR;
import com.motiftech.icollection.model.Account;
import com.motiftech.icollection.service.AccountService;
import com.utils.ApplicationContextHolder;

public class AccountMapping {

	public static Account createAccount(SR sr){
		if(sr != null){
			Calendar calendar = Calendar.getInstance();
			Account account = new Account();
			account.setService(ApplicationContextHolder.getContext().getBean(AccountService.class));	
			account.setSrId(sr.getSrId());
			account.setOwnerUserId(sr.getOwnerUserId());
			account.setOwnerBranchId(sr.getOwnerBranchId());
			account.setCreateId(sr.getCreateUser());
			account.setStatusId(sr.getSrStatusId());
			account.setOldStatusId(sr.getOldSrStatusId());
			account.setChannelId(sr.getChannelId());
			account.setRuleStatusDate(sr.getRuleStatusDate());
			account.setCustomerId(sr.getCustomerId());
			
			KeysBean keysBean = new KeysBean();
			keysBean.setProductGroupId(sr.getProductgroupId());
			keysBean.setProductId(sr.getProductId());
			keysBean.setCampaignserviceId(sr.getCampaignserviceId());
			keysBean.setTypeId(sr.getTypeId());
			keysBean.setAreaId(sr.getAreaId());
			keysBean.setSubareaId(sr.getSubareaId());
			
			account.setKeysBean(keysBean);
			
			if(sr.getOwnerUserId() != null)account.setActionType(RuleConstants.RULE_ACTION.USER_ASSIGN);
			
			account.setCurrentDate(calendar.getTime());
			account.setAssignDate(sr.getRuleAssignDate());
			account.setCurrentSLA(sr.getRuleNextSla());
			account.setRuleEmailFlag(sr.getRuleEmailFlag());
			account.setDelegateUserId(sr.getDelegateUserId()!=null?sr.getDelegateUserId():null);
			account.setDelegateFlag(sr.getRuleDelegateFlag());
			account.setRuleThisAlert(sr.getRuleThisAlert()!=null?sr.getRuleThisAlert():0);
			account.setRuleThisWork(sr.getRuleThisWork()!=null?sr.getRuleThisWork():0);
			
			return account;
		}
		return null;
	}
	/**/
	public static Account createAccountFromView(VW_SR sr){
		if(sr != null){
			Calendar calendar = Calendar.getInstance();
			Account account = new Account();
			account.setService(ApplicationContextHolder.getContext().getBean(AccountService.class));	
			account.setSrId(sr.getSrId());
			account.setOwnerUserId(sr.getOwnerUserId());
			account.setOwnerBranchId(sr.getOwnerBranchId());
			account.setCreateId(sr.getCreateUser());
			account.setStatusId(sr.getSrStatusId());
			account.setOldStatusId(sr.getOldSrStatusId());
			account.setChannelId(sr.getChannelId());
			account.setRuleStatusDate(sr.getRuleStatusDate());
			account.setCustomerId(sr.getCustomerId());
			
			KeysBean keysBean = new KeysBean();
			keysBean.setProductGroupId(sr.getProductgroupId());
			keysBean.setProductId(sr.getProductId());
			keysBean.setCampaignserviceId(sr.getCampaignserviceId());
			keysBean.setTypeId(sr.getTypeId());
			keysBean.setAreaId(sr.getAreaId());
			keysBean.setSubareaId(sr.getSubareaId());
			
			account.setKeysBean(keysBean);
			
			if(sr.getOwnerUserId() != null)account.setActionType(RuleConstants.RULE_ACTION.USER_ASSIGN);
			
			account.setCurrentDate(calendar.getTime());
			account.setAssignDate(sr.getRuleAssignDate());
			account.setCurrentSLA(sr.getRuleNextSla());
			account.setRuleEmailFlag(sr.getRuleEmailFlag());
			account.setDelegateUserId(sr.getDelegateUserId()!=null?sr.getDelegateUserId():null);
			account.setDelegateFlag(sr.getRuleDelegateFlag());
			account.setRuleThisAlert(sr.getRuleThisAlert()!=null?sr.getRuleThisAlert():0);
			account.setRuleThisWork(sr.getRuleThisWork()!=null?sr.getRuleThisWork():0);
			
			return account;
		}
		return null;
	}
	/**/
	public static int minutesDiff(Date earlierDate, Date laterDate){
	    if( earlierDate == null || laterDate == null ) return 0;
	    return (int)((laterDate.getTime()/60000) - (earlierDate.getTime()/60000));
	}
}
