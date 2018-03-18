package com.motiftech.icollection.process;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import org.apache.commons.lang.StringUtils;

import com.icollection.common.constants.InquiryConstants;
import com.motiftech.common.process.AbstractProcess;
import com.motiftech.common.util.CalendarHelper;
import com.motiftech.icollection.bean.ConfigCRMBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.BranchCalendar;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.model.Account;
import com.motiftech.icollection.service.AccountService;
import com.motiftech.icollection.service.CalendarService;
import com.motiftech.rules.RuleBase;
import com.motiftech.rules.RuleSession;

public class SlaProcess extends AbstractProcess {

	private Map<String,RuleBase> ruleMap = new HashMap<String,RuleBase>();
	private int startIndex;
	private int maxSize;
	
	public SlaProcess(int startIndex, int maxSize) {
		super("");
		this.startIndex	= startIndex;
		this.maxSize	= maxSize;
	}
	
	@Override
	protected void doExecute() {
		try{
			AccountService accountService 			= getBean("accountService",AccountService.class);
			CalendarService calendarService 		= getBean("calendarService", CalendarService.class);
			Properties ruleProperties 				= getBean("ruleProperties",Properties.class);
			String linkEmail 						= ruleProperties.getProperty("link.email");
			List<BranchCalendar> branchCalendars 	= calendarService.getAllBranchCalendar();
			Option workTimeStart 	= accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, InquiryConstants.Option.OPTION_CODE_START);
			Option workTimeEnd 		= accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, InquiryConstants.Option.OPTION_CODE_END);
			Option nextTime 		= accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, "nextTime");
			ConfigCRMBean config 	= new ConfigCRMBean();
			if(workTimeEnd != null && workTimeStart != null){
				config.setWorkTimeStart(workTimeStart.getOptionDesc());
				config.setWorkTimeEnd(workTimeEnd.getOptionDesc());
				config.setNextTime(nextTime != null?nextTime.getOptionDesc():String.valueOf(12));
			}else{
				config.setWorkTimeStart(InquiryConstants.Time.START_TIME);
				config.setWorkTimeEnd(InquiryConstants.Time.END_TIME);
				config.setNextTime(String.valueOf(12));
			}
			List<Account> accountList = accountService.listSRFromView(startIndex, maxSize, branchCalendars, linkEmail);
			log.info("accountList size : " + accountList.size());
			int i = 1;
			for(Account account : accountList){
				Branch branch = accountService.getBranch(account.getOwnerBranchId());
				if(branch!=null){
					config.setWorkTimeStart(branch.getStartTimeHour() + ":" + branch.getStartTimeMinute());
					config.setWorkTimeEnd(branch.getEndTimeHour() + ":" + branch.getEndTimeMinute());
				}
				account.setTimeStart(config.getWorkTimeStart());
				account.setTimeOut(config.getWorkTimeEnd());
				String[] timeStart = StringUtils.split(account.getTimeStart(),":");
				account.setHourStartByBranch(Short.parseShort(timeStart[0]));
				account.setMinuteStartByBranch(Short.parseShort(timeStart[1]));
//				for(int x=0;x<timeStart.length;x++){
//					if(x==0)account.setHourStartByBranch(Short.parseShort(timeStart[x]));
//					else account.setMinuteStartByBranch(Short.parseShort(timeStart[x]));
//				}
				String[] timeEnd = StringUtils.split(account.getTimeOut(),":");
				account.setHourEndByBranch(Short.parseShort(timeEnd[0]));
				account.setMinuteEndByBranch(Short.parseShort(timeEnd[1]));
//				for(int x=0;x<timeEnd.length;x++){
//					if(x==0)account.setHourEndByBranch(Short.parseShort(timeEnd[x]));
//					else account.setMinuteEndByBranch(Short.parseShort(timeEnd[x]));
//				}
				
				if(CalendarHelper.timeBetween(CalendarHelper.getCurrentDateTime(), config.getWorkTimeStart(), config.getWorkTimeEnd())){
					RuleBase slaProcess = getRuleBase(RuleConstants.CONFIG.SLA_PROCESS);
					if(slaProcess!=null){
						RuleSession excel = slaProcess.newSession();
						excel.insert(account);	
						try{
							excel.executeRules();
						}catch (Exception e) {
							log.error("Could not complete execute excel process", e);
						}finally{
							excel.dispose();
						}
					}else{
						log.warn("Could not found excel RuleBase by :"+RuleConstants.CONFIG.SLA_PROCESS);
					}					
					if(i%1000==0)log.info("======================>processing account : " + i + " unit.");
				}
			}
		}catch(RuntimeException ex){
			log.error("SlaProcess", ex);
		}
	}
	
	private RuleBase getRuleBase(String processName){
		if(ruleMap.containsKey(processName))return ruleMap.get(processName);
		else{
			RuleBase ruleBase = createRuleBase(processName);
			if(ruleBase!=null)ruleMap.put(processName, ruleBase);
			return ruleBase;
		}
	}
}
