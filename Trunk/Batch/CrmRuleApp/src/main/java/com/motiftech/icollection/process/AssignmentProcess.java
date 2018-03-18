package com.motiftech.icollection.process;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.regex.Pattern;

import org.apache.commons.lang.StringUtils;

import com.icollection.common.constants.InquiryConstants;
import com.motiftech.common.process.AbstractProcess;
import com.motiftech.icollection.batch.BatchEmail;
import com.motiftech.icollection.bean.ConfigCRMBean;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.BranchCalendar;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.model.Account;
import com.motiftech.icollection.service.AccountService;
import com.motiftech.icollection.service.CalendarService;
import com.motiftech.rules.RuleBase;
import com.motiftech.rules.RuleSession;

public class AssignmentProcess extends AbstractProcess {

	private Map<String,RuleBase> ruleMap = new HashMap<String,RuleBase>();
	private static final Pattern FLOW_PATTERN = Pattern.compile("^flow.*");
	private int startIndex;
	private int maxSize;
	
	public AssignmentProcess(int startIndex,int maxSize) {
		super("");
		this.startIndex = startIndex;
		this.maxSize = maxSize;
	}
	
	@Override
	protected void doExecute() {
		try{
			AccountService accountService 			= getBean("accountService",AccountService.class);
			CalendarService calendarService 		= getBean("calendarService", CalendarService.class);
			Properties ruleProperties 				= getBean("ruleProperties",Properties.class);
			String linkEmail 						= ruleProperties.getProperty("link.email");
			List<BranchCalendar> branchCalendars 	= calendarService.getAllBranchCalendar();
			
			Option workTimeStart 		= accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, InquiryConstants.Option.OPTION_CODE_START);
			Option workTimeEnd 			= accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, InquiryConstants.Option.OPTION_CODE_END);
			ConfigCRMBean config 		= new ConfigCRMBean();
			if(workTimeEnd!=null && workTimeStart!=null){
				config.setWorkTimeStart(workTimeStart.getOptionDesc());
				config.setWorkTimeEnd(workTimeEnd.getOptionDesc());
			}else{
				config.setWorkTimeStart(InquiryConstants.Time.START_TIME);
				config.setWorkTimeEnd(InquiryConstants.Time.END_TIME);
			}
			List<Account> accountList = accountService.listSR(startIndex, maxSize, branchCalendars, linkEmail, InquiryConstants.AssignFlag.UNASSIGN);
			log.info("accountList size : " + accountList.size());
			int countList = 1;
			String flowId = null;
			for(Account account : accountList){
				Branch branch = accountService.getBranch(account.getOwnerBranchId());
				if(branch!=null){
					config.setWorkTimeStart(branch.getStartTimeHour() + ":" + branch.getStartTimeMinute());
					config.setWorkTimeEnd(branch.getEndTimeHour() + ":" + branch.getEndTimeMinute());
				}
				
				account.setTimeStart(config.getWorkTimeStart());
				account.setTimeOut(config.getWorkTimeEnd());
				String[] timeStart = StringUtils.split(account.getTimeStart(),":");
				for(int x=0;x<timeStart.length;x++){
					if(x==0)account.setHourStartByBranch(Short.parseShort(timeStart[x]));
					else account.setMinuteStartByBranch(Short.parseShort(timeStart[x]));
				}
				String[] timeEnd = StringUtils.split(account.getTimeOut(),":");
				for(int x=0;x<timeEnd.length;x++){
					if(x==0)account.setHourEndByBranch(Short.parseShort(timeEnd[x]));
					else account.setMinuteEndByBranch(Short.parseShort(timeEnd[x]));
				}
				//remove time start
				//if(CalendarHelper.timeBetween(CalendarHelper.getCurrentDateTime(), config.getWorkTimeStart(), config.getWorkTimeEnd())){
				/**	
				RuleBase enterProcess = getRuleBase(RuleConstants.CONFIG.SR_PROCESS);
					if(enterProcess!=null){
						RuleSession sessionProcess = enterProcess.newSession();
						sessionProcess.insert(account);				
						try{
							sessionProcess.executeRules();
						}finally{
							sessionProcess.dispose();
						}
					}
				/**/
					account.enterProcess("flowAssigned");
					account.enterProcess("flowEmail");
					for(String p : account.getProcess()){
						if(StringUtils.isNotBlank(p)){
							if(FLOW_PATTERN.matcher(p).matches()){
								flowId = getProcessFlowId(p);
								if(StringUtils.isBlank(flowId))log.warn("Could not found flowId by :"+p);
								RuleBase RuleId = getRuleBase(p);
								if(RuleId != null){
									RuleSession session 			= RuleId.newSession();
									Map<String, Object> paramMap 	= new HashMap<String,Object>();
									paramMap.put("account", account);					
									try{
										session.executeProcess(flowId,paramMap);
										session.executeRules();
									}catch(Exception e){
										log.error("Could not complete execute flow process", e);
									}finally{
										session.dispose();
									}
								}else{
									log.warn("Could not found excel RuleBase by :"+p);
								}					
							}
						}
						countList++;
					}
					if(countList%1000 == 0){
						log.info("======================>processing account : " + countList + " unit.");
					}
				//}
			}
			
			String[] args = {};
			BatchEmail batchEmail = new BatchEmail();
			batchEmail.run(args);
			
		}catch(RuntimeException ex){
			log.error("AssignmentProcess", ex);
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
