package com.motiftech.icollection.service.impl;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.icollection.bean.MailBean;
import com.icollection.common.constants.InquiryConstants;
import com.motiftech.common.util.CalendarHelper;
import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.dao.AccountDao;
import com.motiftech.icollection.dao.CalendarDao;
import com.motiftech.icollection.dao.OptionDao;
import com.motiftech.icollection.dao.PrepareEmailDao;
import com.motiftech.icollection.dao.TypeDao;
import com.motiftech.icollection.dao.UserDao;
import com.motiftech.icollection.entity.Activity;
import com.motiftech.icollection.entity.Area;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.BranchCalendar;
import com.motiftech.icollection.entity.Campaignservice;
import com.motiftech.icollection.entity.Channel;
import com.motiftech.icollection.entity.Contact;
import com.motiftech.icollection.entity.Customer;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.MasterAccount;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.entity.PrepareEmail;
import com.motiftech.icollection.entity.Product;
import com.motiftech.icollection.entity.ProductGroup;
import com.motiftech.icollection.entity.SLA;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.SRLogging;
import com.motiftech.icollection.entity.SRStatus;
import com.motiftech.icollection.entity.Subarea;
import com.motiftech.icollection.entity.Type;
import com.motiftech.icollection.entity.User;
import com.motiftech.icollection.entity.VW_SR;
import com.motiftech.icollection.mapping.AccountMapping;
import com.motiftech.icollection.model.Account;
import com.motiftech.icollection.service.AccountService;
import com.motiftech.icollection.utils.MailUtil;

@Service(value="accountService")
public class AccountServiceImpl implements AccountService {

	private final Logger 	log = Logger.getLogger(getClass());
	private AccountDao 		accountDao;
	private UserDao 		userDao;
	private TypeDao 		typeDao;
	private OptionDao 		optionDao;
	private CalendarDao 	calendarDao;
	private PrepareEmailDao	prepareEmailDao;
	private MailUtil 		emailUtil;
	private Properties 		ruleProperties;
	
	@Transactional
	public SR consolidate(Integer customerId) {
		log.debug("customerId : " + customerId);
		SR newSr = accountDao.getConsolidate(customerId);
		if(newSr != null){
			log.debug("null != newSr owner : " + newSr.getOwnerUserId());
			User user = (newSr.getOwnerUserId()!=null)?userDao.getUser(newSr.getOwnerUserId()):null;
			if(user != null)return newSr;
		}
		log.debug("null == newSr");
		return null;
	}
	
	@Transactional
	public MapProduct checkMapProduct(KeysBean keysBean) {
		MapProduct mapProduct = accountDao.getMapProduct(keysBean);
		if(mapProduct != null){
			log.debug("mapProduct != null id = " + mapProduct.getMapProductId() + " default user : " + mapProduct.getDefaultOwnerUserId());
			User user = (mapProduct.getDefaultOwnerUserId()!=null)?userDao.getUser(mapProduct.getDefaultOwnerUserId()):null;
			if(user != null)return mapProduct;
		}
		log.debug("mapProduct == null");
		return null;
	}
	
	@Transactional
	public AutoForward autoForward(KeysBean keysBean, Integer channelId) {
		AutoForward autoForward = accountDao.autoForward(keysBean, channelId);
		if(autoForward != null){
			User user = (autoForward.getAutoForwardId()!=null)?userDao.getUser(autoForward.getAutoForwardId()):null;
			if(user != null)return autoForward;
		}
		return null;
	}
	
	@Transactional
	public Type getTypeByName(String typeName) {
		return typeDao.getTypeByName(typeName);
	}
	
	private static Integer findBranchHoliday(Integer branchId, List<BranchCalendar> branchCalendars){
		if(branchCalendars.size() == 0)return 0;
		for(BranchCalendar branchCalendar:branchCalendars){
			if(branchCalendar.getBranchId().equals(branchId)){
				int diff = dateDiff(branchCalendar.getHolidayDate());
				if(diff == 0)return -1;
			}
		}
		return 0;
	}
	
	private static Integer dateDiff(Date date){
		if(date == null)return 0;
		Calendar calendar = Calendar.getInstance();
		Calendar currentDate = Calendar.getInstance();
		calendar.setTime(date);
		return CalendarHelper.dateDiff(currentDate, calendar);
	}
	
	private void setAccount(Account account, SR sr, List<BranchCalendar> branchCalendars){
		if(sr.getProductgroupId() != null && sr.getProductId() != null 
				&& sr.getAreaId() != null && sr.getSubareaId() != null 
				&& sr.getTypeId() != null && sr.getCampaignserviceId() != null){
			
//			long startTime = System.currentTimeMillis();
			
			SLA sla = accountDao.getSLA(sr.getProductId(), sr.getChannelId(), sr.getSrStatusId(), sr.getAreaId(), sr.getTypeId(), sr.getCampaignserviceId(), sr.getSubareaId());
			
//			long endTime = System.currentTimeMillis();
//			Long diffTime 		= endTime-startTime;
//			Long diffSecond		= (endTime-startTime)/1000;
//			
//			Long hours			= (diffSecond/60)/60;
//			Long minutes		= (diffSecond/60)%60;
//			Long seconds		= diffSecond%(60*60);
//			Long millisecords	= diffTime%1000;
			
			//log.debug("SR ID : " + sr.getSrId() + ", map SLA time : " + hours + " hour(s), " + minutes + " minute(s), " + seconds + " second(s), " + millisecords + " millisecond(s)");
			
			if(sla != null){
				account.setSLAMinute(sla.getSlaMinute());
				account.setSLATime(sla.getSlaTimes());
				account.setSLADay(sla.getSlaDay());
			}
			
			if(sr.getRuleNextSla() != null){
				Calendar current = Calendar.getInstance();
				Calendar nextSLA = Calendar.getInstance();
				current.setTime(account.getCurrentDate());
				nextSLA.setTime(sr.getRuleNextSla());
				account.setNextSLA(sr.getRuleNextSla());
				account.setNextSLAFlag(timeAfter(current, nextSLA));
			}
		}
		account.setHoliday(findBranchHoliday(sr.getOwnerBranchId(),branchCalendars));
	}
	/**/
	private void setAccountFromView(Account account, VW_SR sr, List<BranchCalendar> branchCalendars){
		if(sr.getProductgroupId() != null && sr.getProductId() != null 
				&& sr.getAreaId() != null && sr.getSubareaId() != null 
				&& sr.getTypeId() != null && sr.getCampaignserviceId() != null){

			//long startTime = System.currentTimeMillis();
			
			SLA sla = accountDao.getSLA(sr.getProductId(), sr.getChannelId(), sr.getSrStatusId(), sr.getAreaId(), sr.getTypeId(), sr.getCampaignserviceId(), sr.getSubareaId());
			
//			long endTime = System.currentTimeMillis();
//			Long diffTime 		= endTime-startTime;
//			Long diffSecond		= (endTime-startTime)/1000;
//			
//			Long hours			= (diffSecond/60)/60;
//			Long minutes		= (diffSecond/60)%60;
//			Long seconds		= diffSecond%(60*60);
//			Long millisecords	= diffTime%1000;
			
			//log.debug("SR ID : " + sr.getSrId() + ", map SLA time : " + hours + " hour(s), " + minutes + " minute(s), " + seconds + " second(s), " + millisecords + " millisecond(s)");
			
			if(sla != null){
				account.setSLAMinute(sla.getSlaMinute());
				account.setSLATime(sla.getSlaTimes());
				account.setSLADay(sla.getSlaDay());
			}
			
			if(sr.getRuleNextSla() != null){
				Calendar current = Calendar.getInstance();
				Calendar nextSLA = Calendar.getInstance();
				current.setTime(account.getCurrentDate());
				nextSLA.setTime(sr.getRuleNextSla());
				account.setNextSLA(sr.getRuleNextSla());
				account.setNextSLAFlag(timeAfter(current, nextSLA));
			}
		}
		account.setHoliday(findBranchHoliday(sr.getOwnerBranchId(),branchCalendars));
	}
	/**/
	@Transactional
	public List<Account> listSR(int startIndex, int maxSize, List<BranchCalendar> branchCalendars, String linkEmail, String assignFlag) {
		log.debug("listSRByAssignFlag listSR");
		List<SR> srList 			= accountDao.listSRByAssignFlag(startIndex, maxSize, assignFlag);
		List<Account> accountList 	= new ArrayList<Account>();
		log.debug("start map SLA");
		for(SR sr: srList){
			Account account = AccountMapping.createAccount(sr);
			if(account!=null){
				setAccount(account, sr, branchCalendars);
				account.setLinkEmail(linkEmail);
				accountList.add(account);
			}
			else log.warn("account : " + sr.getSrId() + " was discarded because some field is missing");
		}
		log.debug("map SLA finish");
		return accountList;
	}
	/**/
	@Transactional
	public List<Account> listSRFromView(int startIndex, int maxSize, List<BranchCalendar> branchCalendars, String linkEmail) {
		log.debug("listSRFromView listSR");
		List<VW_SR> srList 			= accountDao.listSRFromView(startIndex, maxSize);
		List<Account> accountList 	= new ArrayList<Account>();
		log.debug("start map SLA");
		for(VW_SR sr: srList){
			Account account = AccountMapping.createAccountFromView(sr);
			if(account!=null){
				setAccountFromView(account, sr, branchCalendars);
				account.setLinkEmail(linkEmail);
				accountList.add(account);
			}
			else log.warn("account : " + sr.getSrId() + " was discarded because some field is missing");
		}
		log.debug("map SLA finish");
		return accountList;
	}
	/**/
	@Transactional
	public Branch getBranch(Integer branchId){
		return accountDao.getBranchById(branchId);
	}
	
	@Transactional
	public void updateSR(Integer srId, Account account){
		log.debug("updateSR srId : " + srId + ", owner : " + account.getOwnerUserId());
		SR	 sr					= accountDao.getSR(srId);
		User user 				= (account.getOwnerUserId()!=null)?userDao.getUser(account.getOwnerUserId()):null;
		User systemUser 		= userDao.getUserByUserName(RuleConstants.RULE_USER);
		if(user != null && sr != null){
			Branch 	branch 		= accountDao.getBranchById(user.getBranchId());
			sr.setRuleThisAlert(sr.getRuleThisAlert()!=null?sr.getRuleThisAlert():0);
			Calendar current = getNextSLA(account, branch, sr, user);
			account.setNextSLA(current.getTime());

			sr.setOwnerUserId(account.getOwnerUserId());
			sr.setOwnerBranchId(user.getBranchId());
			sr.setDelegateUserId(account.getDelegateUserId());
			
			if(!account.getActionType().equals(RuleConstants.RULE_ACTION.USER_ASSIGN)){
				sr.setUpdateDateByOwner(account.getCurrentDate());
				if(account.getDelegateUserId() != null)sr.setUpdateDateByDelegate(account.getCurrentDate());
			}
			
			sr.setRuleNextSla((account.getSLAMinute() > 0)?current.getTime():null);
			sr.setRuleAssignFlag(InquiryConstants.AssignFlag.ASSIGN);
			sr.setRuleAssignDate(account.getCurrentDate());
			sr.setRuleCurrentSla(account.getCurrentSLA());
			sr.setUpdateDate(account.getCurrentDate());
			sr.setUpdateUser((systemUser!=null)?systemUser.getUserId():null);
			
			accountDao.saveSRLogging(setSRLogging(sr, account, user, systemUser));
			accountDao.updateSR(sr);
			log.debug("update success");
		}else{
			log.error("Update SR fail srId : " + srId + " / " + account.getOwnerUserId() + " / " + user);
		}
	}
	
	@Transactional
	public void updateSRStatus(Integer srId, Account account){
		SR	 sr					= accountDao.getSR(srId);
		User user 				= (account.getOwnerUserId()!=null)?userDao.getUser(account.getOwnerUserId()):null;
		User systemUser 		= userDao.getUserByUserName(RuleConstants.RULE_USER);
		if(user != null && sr != null){
			sr.setSrStatusId(RuleConstants.getStatemap().get(account.getUpdateStatus()));
			sr.setUpdateDate(account.getCurrentDate());
			sr.setUpdateUser((systemUser!=null)?systemUser.getUserId():null);
			accountDao.saveSRLogging(setSRLogging(sr, account, user, systemUser));
			accountDao.updateSR(sr);
		}else{
			log.error("updateSRStatus fail srId : " + srId + " / " + account.getOwnerUserId() + " / " + user);
		}
	}
	
	private SRLogging setSRLogging(SR sr, Account account, User user, User systemUser){
//		SRLogging oldLogging  	= accountDao.getSRLogging(sr.getSrId());
		Activity  activity 		= accountDao.getActivity(sr.getSrId());
		
		SRLogging srLogging = new SRLogging();
		srLogging.setSrId(sr.getSrId());
		srLogging.setSrActivityId(activity!=null?activity.getSrActivityId():null);
		srLogging.setSrLoggingSystemAction("RULE");
		if(account.getActionType().equals(RuleConstants.RULE_ACTION.USER_ASSIGN) && user.getIsGroup())
			srLogging.setSrLoggingAction(RuleConstants.RULE_ACTION.GROUP_ASSIGN);
		else srLogging.setSrLoggingAction(account.getActionType());
		
		srLogging.setSrStatusIdNew(sr.getSrStatusId());
		srLogging.setSrStatusIdOld(sr.getOldSrStatusId());
		srLogging.setOwnerUserIdOld(sr.getOldOwnerUserId());
		srLogging.setOwnerUserIdNew(account.getOwnerUserId());
		srLogging.setDelegateUserIdOld(sr.getOldDelegateUserId());
		srLogging.setDelegateUserIdNew(account.getDelegateUserId());
		srLogging.setOverSlaMinute(account.getRuleThisWork());
		srLogging.setOverSlaTimes(account.getRuleThisAlert());
		
		srLogging.setCreateDate(account.getCurrentDate());
		srLogging.setCreateUser((systemUser!=null)?systemUser.getUserId():null);
		
		return srLogging;
	}
	
	private Calendar getNextSLA(Account account, Branch branch, SR sr, User user){
		if(branch!=null){
			account.setHourStartByBranch(branch.getStartTimeHour());
			account.setMinuteStartByBranch(branch.getStartTimeMinute());
			account.setHourEndByBranch(branch.getEndTimeHour());
			account.setMinuteEndByBranch(branch.getEndTimeMinute());
		}else{
			String[] timeStart = StringUtils.split(account.getTimeStart(),":");
			account.setHourStartByBranch(Short.parseShort(timeStart[0]));
			account.setMinuteStartByBranch(Short.parseShort(timeStart[1]));
//			for(int x=0;x<timeStart.length;x++){
//				if(x==0)account.setHourStartByBranch(Short.parseShort(timeStart[x]));
//				else account.setMinuteStartByBranch(Short.parseShort(timeStart[x]));
//			}
			String[] timeEnd = StringUtils.split(account.getTimeOut(),":");
			account.setHourEndByBranch(Short.parseShort(timeEnd[0]));
			account.setMinuteEndByBranch(Short.parseShort(timeEnd[1]));
//			for(int x=0;x<timeEnd.length;x++){
//				if(x==0)account.setHourEndByBranch(Short.parseShort(timeEnd[x]));
//				else account.setMinuteEndByBranch(Short.parseShort(timeEnd[x]));
//			}
		}
		Calendar currentTime 	= CalendarHelper.getCurrentDateTime();
		Calendar currentSLA 	= CalendarHelper.getCurrentDateTime();
		Calendar calToStartTime = CalendarHelper.getCurrentDateTime();
		Calendar calToEndTime 	= CalendarHelper.getCurrentDateTime();
		
		if(account.getCurrentSLA()!=null)currentSLA.setTime(account.getCurrentSLA());
		else currentSLA.setTime(account.getCurrentDate());
		
		CalendarHelper.setTime(calToStartTime, Integer.parseInt(account.getHourStartByBranch().toString()), Integer.parseInt(account.getMinuteStartByBranch().toString()), 00);
		CalendarHelper.setTime(calToEndTime, Integer.parseInt(account.getHourEndByBranch().toString()), Integer.parseInt(account.getMinuteEndByBranch().toString()), 00);

		account.setWorkingTime(minutesDiff(calToStartTime.getTime(), calToEndTime.getTime()));
		account.setRemainTime(minutesDiff(currentTime.getTime(), calToEndTime.getTime()));
		
		log.debug(account.getOwnerUserId() + " SLAMinute   : " + account.getSLAMinute());
		log.debug(account.getOwnerUserId() + " SLATime     : " + account.getSLATime());
		log.debug(account.getOwnerUserId() + " workingTime : " + account.getWorkingTime());
		log.debug(account.getOwnerUserId() + " remainTime  : " + account.getRemainTime());
		
		if(account.getSLAMinute() > 0){
			log.debug("before calculate SLA : " + currentSLA.getTime());
			
			if(account.getRemainTime() > account.getSLAMinute())currentSLA.add(Calendar.MINUTE, account.getSLAMinute());
			else{
				int SLARemaining = account.getSLAMinute() - account.getRemainTime();
				currentSLA.add(Calendar.DATE, 1);
				CalendarHelper.setTime(currentSLA, Integer.parseInt(account.getHourStartByBranch().toString()), Integer.parseInt(account.getMinuteStartByBranch().toString()), currentSLA.get(Calendar.SECOND));
				while(SLARemaining >= 0){
					if(account.getWorkingTime() > SLARemaining){
						currentSLA.add(Calendar.MINUTE, SLARemaining);
						currentSLA = isHoliday(currentSLA, account, sr, user);
						log.debug("after  calculate SLA : " + currentSLA.getTime());
						return currentSLA;
					}else{
						SLARemaining = SLARemaining - account.getWorkingTime();
						currentSLA.add(Calendar.DATE, 1);
						currentSLA = isHoliday(currentSLA, account, sr, user);
					}
				}
			}
			log.debug("after  calculate SLA : " + currentSLA.getTime());
		}
		return currentSLA;
	}
	
	@Transactional(readOnly=true)
	public Long countSRByAssignFlag(String assignFlag){
		return accountDao.countSRByAssignFlag(assignFlag);
	}
	/**/
	@Transactional(readOnly=true)
	public Long countSRFromView(){
		return accountDao.countSRFromView();
	}
	/**/
	@Transactional(readOnly=true)
	public Option getWorkTime(String type, String code){
		return optionDao.getOptDescByOption(type, code);
	}
	
	private static boolean timeAfter(Calendar cal1, Calendar cal2) {
		if (cal1.get(Calendar.YEAR) 		< cal2.get(Calendar.YEAR))			return false;
		if (cal1.get(Calendar.YEAR) 		> cal2.get(Calendar.YEAR))			return true;
		if (cal1.get(Calendar.DAY_OF_YEAR) 	< cal2.get(Calendar.DAY_OF_YEAR))	return false;
		if (cal1.get(Calendar.DAY_OF_YEAR) 	> cal2.get(Calendar.DAY_OF_YEAR))	return true;
		if (cal1.get(Calendar.HOUR_OF_DAY) 	< cal2.get(Calendar.HOUR_OF_DAY))	return false;
		if (cal1.get(Calendar.HOUR_OF_DAY) 	> cal2.get(Calendar.HOUR_OF_DAY))	return true;
		if (cal1.get(Calendar.MINUTE) 		< cal2.get(Calendar.MINUTE))		return false;
		if (cal1.get(Calendar.MINUTE) 		> cal2.get(Calendar.MINUTE))		return true;
		if (cal1.get(Calendar.SECOND) 		<= cal2.get(Calendar.SECOND))		return false;
		return true;
	}
	
	@Transactional
	private Calendar isHoliday(Calendar nextSLA, Account acc, SR sr, User user){
		Long isHoliday = calendarDao.isHolidayBranch(nextSLA.getTime(),user.getBranchId());
		if(isHoliday.compareTo((long) 0) > 0){
			nextSLA.add(Calendar.DATE, 1);
			return isHoliday(nextSLA, acc, sr, user);
		}else return nextSLA;
		
	}
	
	@Transactional
	public void sendEmail(Integer srId, String template, Account account) {
		SR sr = accountDao.getSR(srId);	
		if(sr != null && sr.getRuleEmailFlag().equals(RuleConstants.EMAIL.SEND)){
			User systemUser = userDao.getUserByUserName(RuleConstants.RULE_USER);
			boolean result 	= sendEmail(sr, template, account, systemUser);
			sr.setRuleThisAlert(account.getRuleThisAlert());
			sr.setRuleThisWork(account.getRuleThisWork());
			sr.setUpdateDate(account.getCurrentDate());
			sr.setUpdateUser((systemUser!=null)?systemUser.getUserId():null);
			if(result)accountDao.updateSR(sr);
		}
	}
	
	@Transactional
	public void sendDelegateEmail(Integer srId, String template, Account account) {
		SR sr = accountDao.getSR(srId);	
		if(sr != null){
			User systemUser = userDao.getUserByUserName(RuleConstants.RULE_USER);
			boolean result 	= sendDelegateEmail(sr, template, account, systemUser);
			sr.setRuleDelegateFlag("0");
			sr.setUpdateDate(account.getCurrentDate());
			sr.setUpdateUser((systemUser!=null)?systemUser.getUserId():null);
			if(result)accountDao.updateSR(sr);
		}
	}
	
	@Transactional
	public void sendSLAEmail(Integer srId, String template, Account account) {
		SR sr = accountDao.getSR(srId);	
		if(sr != null){
			User user 				= (account.getOwnerUserId()!=null)?userDao.getUser(account.getOwnerUserId()):null;
			User systemUser 		= userDao.getUserByUserName(RuleConstants.RULE_USER);
			if(user != null){
				Branch 	branch 		= accountDao.getBranchById(user.getBranchId());
				
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.CHANGE_STATUS)){
					sr.setRuleThisAlert(0);
					sr.setRuleThisWork(0);
					sr.setRuleCurrentSla(null);
					account.setCurrentSLA(null);
				}else{
					sr.setRuleThisAlert(account.getRuleThisAlert());
					sr.setRuleThisWork(account.getRuleThisWork());
				}
				
				Calendar current = getNextSLA(account, branch, sr, user);
				account.setNextSLA(current.getTime());
				sr.setRuleNextSla((account.getSLAMinute() > 0)?current.getTime():null);
				sr.setRuleCurrentSla(account.getCurrentSLA());
				sr.setUpdateDate(account.getCurrentDate());
				sr.setUpdateUser((systemUser!=null)?systemUser.getUserId():null);
				
				if(sendEmail(sr, template, account, systemUser)){
					//accountDao.saveSRLogging(setSRLogging(sr, account, user, systemUser));
					sr.setOldSrStatusId(account.getStatusId());
					accountDao.updateSR(sr);
				}
			}else{
				log.error("Update SR fail srId :"+sr.getSrId()+" / "+account.getOwnerUserId()+" / "+user);
			}
		}
	}
	
	@Transactional
	public void sendSuperVisorEmail(Integer srId, String template, Account account) {
		SR sr = accountDao.getSR(srId);	
		if(sr != null){
			User systemUser = userDao.getUserByUserName(RuleConstants.RULE_USER);
			sendSuperVisorEmail(sr, template, account, systemUser);
		}
		
	}
	
	private boolean sendEmail(SR sr, String template, Account account, User systemUser){
		log.debug("SR ID : " + sr.getSrId() + ", sendEmail template : >>" + template + "<<" + ", owner : " + sr.getOwnerUserId());
		User ownerUser 		= (sr.getOwnerUserId()!=null)?userDao.getUser(sr.getOwnerUserId()):null;
		User delegateUser 	= (sr.getDelegateUserId()!=null)?userDao.getUser(sr.getDelegateUserId()):null;
		Activity activity	= accountDao.getActivity(sr.getSrId());
		if(ownerUser != null){
			String ownerUserEmail = ownerUser.getEmail();
			if(isValidateEmail(ownerUserEmail)){
				MailBean mailDetail = new MailBean();
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.ASSIGN))
					mailDetail = emailTemplateAssign(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.CHANGE_STATUS))
					mailDetail = emailTemplateChangeStatus(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.OVER_SLA))
					mailDetail = emailTemplateOverSLA(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.CUSTOMER))
					mailDetail = emailTemplateCustomer(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.NOTE))
					mailDetail = emailTemplateNote(template, sr, account, ownerUser, delegateUser, activity);
				
				PrepareEmail prepareEmail = new PrepareEmail();
				prepareEmail.setEmailAddress(ownerUserEmail);
				prepareEmail.setEmailSender(ownerUser.getUserName());
				prepareEmail.setEmailSubject(mailDetail.getSubject());
				prepareEmail.setEmailContent(mailDetail.getMessage());
				prepareEmail.setExportStatus(InquiryConstants.ExportStatus.NOT_EXPORTED);
				prepareEmail.setIsDelete(Boolean.FALSE);
				prepareEmail.setCreateUser((systemUser!=null)?systemUser.getUserName():"");
				prepareEmail.setCreateDate(account.getCurrentDate());
				prepareEmail.setUpdateUser((systemUser!=null)?systemUser.getUserName():"");
				prepareEmail.setUpdateDate(account.getCurrentDate());
				prepareEmail.setSrActivityId(activity.getSrActivityId());
				prepareEmailDao.savePrepareEmail(prepareEmail);
				
				if(delegateUser != null && isValidateEmail(delegateUser.getEmail())){
					PrepareEmail prepareEmailDelegate = new PrepareEmail();
					prepareEmailDelegate.setEmailAddress(delegateUser.getEmail());
					prepareEmailDelegate.setEmailSender(delegateUser.getUserName());
					prepareEmailDelegate.setEmailSubject(mailDetail.getSubject());
					prepareEmailDelegate.setEmailContent(mailDetail.getMessage());
					prepareEmailDelegate.setExportStatus(InquiryConstants.ExportStatus.NOT_EXPORTED);
					prepareEmailDelegate.setIsDelete(Boolean.FALSE);
					prepareEmailDelegate.setCreateUser((systemUser!=null)?systemUser.getUserName():"");
					prepareEmailDelegate.setCreateDate(account.getCurrentDate());
					prepareEmailDelegate.setUpdateUser((systemUser!=null)?systemUser.getUserName():"");
					prepareEmailDelegate.setUpdateDate(account.getCurrentDate());
					prepareEmailDelegate.setSrActivityId(activity.getSrActivityId());
					prepareEmailDao.savePrepareEmail(prepareEmailDelegate);
				}
			}else log.error("User : " + ownerUser.getFirstName() + " has invalid Email");
			return true;
		}else{
			log.error("User from srId : " + sr.getSrId() + " cannot be found");
		}
		
		return false;
	}
	
	private boolean sendDelegateEmail(SR sr, String template, Account account, User systemUser){
		log.debug("sendDelegateEmail template : >>" + template + "<<" + ", owner : " + sr.getOwnerUserId());
		User ownerUser 		= (sr.getOwnerUserId()!=null)?userDao.getUser(sr.getOwnerUserId()):null;
		User delegateUser 	= (sr.getDelegateUserId()!=null)?userDao.getUser(sr.getDelegateUserId()):null;
		Activity activity	= accountDao.getActivity(sr.getSrId());
		if(delegateUser != null){
			String delegateUserEmail = delegateUser.getEmail();
			if(isValidateEmail(delegateUserEmail)){
				MailBean mailDetail = new MailBean();
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.ASSIGN))
					mailDetail = emailTemplateAssign(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.CHANGE_STATUS))
					mailDetail = emailTemplateChangeStatus(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.OVER_SLA))
					mailDetail = emailTemplateOverSLA(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.CUSTOMER))
					mailDetail = emailTemplateCustomer(template, sr, account, ownerUser, delegateUser);
				if(template.equals(RuleConstants.EMAIL.TEMPLATE.NOTE))
					mailDetail = emailTemplateNote(template, sr, account, ownerUser, delegateUser, activity);
				
				PrepareEmail prepareEmailDelegate = new PrepareEmail();
				prepareEmailDelegate.setEmailAddress(delegateUser.getEmail());
				prepareEmailDelegate.setEmailSender(delegateUser.getUserName());
				prepareEmailDelegate.setEmailSubject(mailDetail.getSubject());
				prepareEmailDelegate.setEmailContent(mailDetail.getMessage());
				prepareEmailDelegate.setExportStatus(InquiryConstants.ExportStatus.NOT_EXPORTED);
				prepareEmailDelegate.setIsDelete(Boolean.FALSE);
				prepareEmailDelegate.setCreateUser((systemUser!=null)?systemUser.getUserName():"");
				prepareEmailDelegate.setCreateDate(account.getCurrentDate());
				prepareEmailDelegate.setUpdateUser((systemUser!=null)?systemUser.getUserName():"");
				prepareEmailDelegate.setUpdateDate(account.getCurrentDate());
				prepareEmailDelegate.setSrActivityId(activity.getSrActivityId());
				prepareEmailDao.savePrepareEmail(prepareEmailDelegate);
				
			}else log.error("delegateUser : " + delegateUser.getFirstName() + " has invalid Email");
			return true;
		}else{
			log.error("delegateUser from srId : " + sr.getSrId() + " cannot be found");
		}
		return false;
	}
	
	private boolean sendSuperVisorEmail(SR sr, String template, Account account, User systemUser){
		log.debug("sendEmail template : >>" + template + "<<");
		User ownerUser 		= (sr.getOwnerUserId()!=null)?userDao.getUser(sr.getOwnerUserId()):null;
		User delegateUser 	= (sr.getDelegateUserId()!=null)?userDao.getUser(sr.getDelegateUserId()):null;
		Activity activity	= accountDao.getActivity(sr.getSrId());
		if(ownerUser != null){
			User supervisor = (ownerUser.getSupervisorId()!=null)?userDao.getUser(ownerUser.getSupervisorId()):null;
			if(supervisor != null){
				String supervisorEmail = supervisor.getEmail();
				if(isValidateEmail(supervisorEmail)){
					MailBean mailDetail = new MailBean();
					if(template.equals(RuleConstants.EMAIL.TEMPLATE.ASSIGN))
						mailDetail = emailTemplateAssign(template, sr, account, ownerUser, delegateUser);
					if(template.equals(RuleConstants.EMAIL.TEMPLATE.CHANGE_STATUS))
						mailDetail = emailTemplateChangeStatus(template, sr, account, ownerUser, delegateUser);
					if(template.equals(RuleConstants.EMAIL.TEMPLATE.OVER_SLA))
						mailDetail = emailTemplateOverSLA(template, sr, account, ownerUser, delegateUser);
					if(template.equals(RuleConstants.EMAIL.TEMPLATE.CUSTOMER))
						mailDetail = emailTemplateCustomer(template, sr, account, ownerUser, delegateUser);
					if(template.equals(RuleConstants.EMAIL.TEMPLATE.NOTE))
						mailDetail = emailTemplateNote(template, sr, account, ownerUser, delegateUser, activity);
				
					PrepareEmail prepareEmailSupervisor = new PrepareEmail();
					prepareEmailSupervisor.setEmailAddress(supervisor.getEmail());
					prepareEmailSupervisor.setEmailSender(supervisor.getUserName());
					prepareEmailSupervisor.setEmailSubject(mailDetail.getSubject());
					prepareEmailSupervisor.setEmailContent(mailDetail.getMessage());
					prepareEmailSupervisor.setExportStatus(InquiryConstants.ExportStatus.NOT_EXPORTED);
					prepareEmailSupervisor.setIsDelete(Boolean.FALSE);
					prepareEmailSupervisor.setCreateUser((systemUser!=null)?systemUser.getUserName():"");
					prepareEmailSupervisor.setCreateDate(account.getCurrentDate());
					prepareEmailSupervisor.setUpdateUser((systemUser!=null)?systemUser.getUserName():"");
					prepareEmailSupervisor.setUpdateDate(account.getCurrentDate());
					prepareEmailSupervisor.setSrActivityId(activity.getSrActivityId());
					prepareEmailDao.savePrepareEmail(prepareEmailSupervisor);
				}else log.error("Supervisor : " + supervisor.getFirstName() + " has invalid Email");
			}else{
				log.error("Supervisor from srId : " + sr.getSrId() + " cannot be found");
			}
			return true;
		}else{
			log.error("User from srId : " + sr.getSrId() + " cannot be found");
		}
		return false;
	}
	
	private MailBean emailTemplateAssign(String template, SR sr, Account account, User ownerUser, User delegateUser){
		Customer 		customer			= accountDao.getCustomer(sr.getCustomerId());
		Campaignservice campaignservice		= accountDao.getCampaignservice(sr.getCampaignserviceId());
		Branch 			branch 				= accountDao.getBranchById(ownerUser.getBranchId());
		MasterAccount 	masterAccount		= accountDao.getMasterAccount(sr.getAccountId());
		ProductGroup 	productGroup		= accountDao.getProductGroup(sr.getProductgroupId());
		Product 		product				= accountDao.getProduct(sr.getProductId());
		Type	 		type				= typeDao.getType(sr.getTypeId());
		Area	 		area				= accountDao.getArea(sr.getAreaId());
		Subarea	 		subarea				= accountDao.getSubarea(sr.getSubareaId());
		Channel	 		channel				= accountDao.getChannel(sr.getChannelId());
		SRStatus	 	srStatus			= accountDao.getSRStatus(sr.getSrStatusId());
		User 			createUser 			= (sr.getCreateUser()!=null)?userDao.getUser(sr.getCreateUser()):null;
		Contact			contact 			= accountDao.getContact(sr.getContactId());
		Activity 		activity			= accountDao.getActivity(sr.getSrId());
		List<String>	customerPhoneList 	= accountDao.getCustomerPhoneList(sr.getCustomerId());
		List<String>	contactPhoneList	= accountDao.getContactPhoneList(sr.getContactId());
		
		String customerName = " ";
		if(customer != null){
			if(StringUtils.isNotBlank(customer.getFirstNameTh()))customerName = customer.getFirstNameTh();
			if(StringUtils.isNotBlank(customer.getLastNameTh()))customerName = customerName + " " + customer.getLastNameTh();
		}
		
		String contactNameTh = "", contactNameEn = "";
		if(contact != null){
			if(StringUtils.isNotBlank(contact.getFirstNameTh()))contactNameTh = contact.getFirstNameTh();
			if(StringUtils.isNotBlank(contact.getLastNameTh()))contactNameTh = contactNameTh + " " + contact.getLastNameTh();
			if(StringUtils.isNotBlank(contact.getFirstNameEn()))contactNameEn = contact.getFirstNameEn();
			if(StringUtils.isNotBlank(contact.getLastNameEn()))contactNameEn = contactNameEn + " " + contact.getLastNameEn();
		}
		
		String customerPhoneNo = " ";
		if(customerPhoneList != null && customerPhoneList.size() > 0){
			customerPhoneNo = StringUtils.join(customerPhoneList, ",");
		}
		
		String contactPhoneNo = " ";
		if(contactPhoneList != null && contactPhoneList.size() > 0){
			contactPhoneNo = StringUtils.join(contactPhoneList, ",");
		}		
		
		String remark = StringUtils.isNotBlank(sr.getSrRemark()) ? sr.getSrRemark() : " ";
		log.debug("sr.getSrAfsAssetNo() ====> " + sr.getSrAfsAssetNo());
		if(StringUtils.isNotBlank(sr.getSrAfsAssetNo())){
			remark = String.format("%s %s : %s", ruleProperties.get("label.asset.code"), sr.getSrAfsAssetNo(), remark);
		}
		
		Map<String,String> varMap 	= new HashMap<String,String>();
		varMap.put("SR_ID", 			sr.getSrNo());
		varMap.put("BRANCH_CODE", 		(branch!=null)?branch.getBranchCode():" ");
		varMap.put("BRANCH_NAME", 		(branch!=null)?branch.getBranchName():" ");
		varMap.put("ACCOUNT_NO", 		(masterAccount!=null)?masterAccount.getAccountNo():" ");
		varMap.put("PRODUCT_GROUP_NAME", (productGroup!=null)?productGroup.getProductGroupName():" ");
		varMap.put("PRODUCT_NAME", 		(product!=null)?product.getProductName():" ");
		varMap.put("CAMPAIGN_NAME", 	(campaignservice!=null)?campaignservice.getCampaignserviceName():" ");
		varMap.put("TYPE_NAME",			(type!=null)?type.getTypeName():" ");
		varMap.put("AREA_NAME",			(area!=null)?area.getAreaName():" ");
		varMap.put("SUBAREA_NAME", 		(subarea!=null)?subarea.getSubareaName():" ");
		varMap.put("CHANNEL_NAME", 		(channel!=null)?channel.getChannelName():" ");
		varMap.put("STATUS", 			(srStatus!=null)?srStatus.getSrStatusName():" ");
		varMap.put("CUSTOMER_NAME",		customerName);
		varMap.put("SUBSCRIPTION_ID", 	(customer!=null)?customer.getCardNo():" ");
		varMap.put("OWNER", 			ownerUser.getFirstName() + " " + ownerUser.getLastName());
		varMap.put("ASSIGNED_DATE", 	sr.getRuleAssignDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleAssignDate()):" ");
		varMap.put("DELEGATE", 			(delegateUser!=null)?delegateUser.getFirstName() + " " + delegateUser.getLastName():" ");
		varMap.put("DELEGATE_DATE", 	sr.getRuleDelegateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleDelegateDate()):" ");
		varMap.put("CREATE_BY", 		(createUser!=null)?createUser.getFirstName() + " " + createUser.getLastName():" ");
		varMap.put("CREATE_DATE", 		sr.getCreateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getCreateDate()):" ");
		varMap.put("NEXTSLA", 			sr.getRuleNextSla()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleNextSla()):" ");
		varMap.put("LINK", 				account.getLinkEmail() + sr.getSrNo());
		varMap.put("CUSTOMER_PHONE_NO", customerPhoneNo);
		varMap.put("CONTACT_NAME",	 	StringUtils.isNotBlank(contactNameTh) ? contactNameTh : contactNameEn);
		varMap.put("CONTACT_PHONE_NO", 	contactPhoneNo);
		varMap.put("SUBJECT", 			sr.getSrSubject());
		varMap.put("REMARK", 			remark);
		varMap.put("ACTIVITY_DESC", 	(activity!=null)?activity.getSrActivityDesc():" ");
		
		return emailUtil.createMailMessage(template, varMap);
	}
	
	private MailBean emailTemplateChangeStatus(String template, SR sr, Account account, User ownerUser, User delegateUser){
		Customer 		customer			= accountDao.getCustomer(sr.getCustomerId());
		Campaignservice campaignservice		= accountDao.getCampaignservice(sr.getCampaignserviceId());
		Branch 			branch 				= accountDao.getBranchById(ownerUser.getBranchId());
		MasterAccount 	masterAccount		= accountDao.getMasterAccount(sr.getAccountId());
		ProductGroup 	productGroup		= accountDao.getProductGroup(sr.getProductgroupId());
		Product 		product				= accountDao.getProduct(sr.getProductId());
		Type	 		type				= typeDao.getType(sr.getTypeId());
		Area	 		area				= accountDao.getArea(sr.getAreaId());
		Subarea	 		subarea				= accountDao.getSubarea(sr.getSubareaId());
		Channel	 		channel				= accountDao.getChannel(sr.getChannelId());
		SRStatus	 	srStatus			= accountDao.getSRStatus(sr.getSrStatusId());
		SRStatus	 	oldSrStatus			= accountDao.getSRStatus(sr.getOldSrStatusId());
		User 			createUser 			= (sr.getCreateUser()!=null)?userDao.getUser(sr.getCreateUser()):null;
		Contact			contact 			= accountDao.getContact(sr.getContactId());
		Activity 		activity			= accountDao.getActivity(sr.getSrId());
		List<String>	customerPhoneList 	= accountDao.getCustomerPhoneList(sr.getCustomerId());
		List<String>	contactPhoneList	= accountDao.getContactPhoneList(sr.getContactId());
		
		String customerName = " ";
		if(customer != null){
			if(StringUtils.isNotBlank(customer.getFirstNameTh()))customerName = customer.getFirstNameTh();
			if(StringUtils.isNotBlank(customer.getLastNameTh()))customerName = customerName + " " + customer.getLastNameTh();
		}
		
		String contactNameTh = "", contactNameEn = "";
		if(contact != null){
			if(StringUtils.isNotBlank(contact.getFirstNameTh()))contactNameTh = contact.getFirstNameTh();
			if(StringUtils.isNotBlank(contact.getLastNameTh()))contactNameTh = contactNameTh + " " + contact.getLastNameTh();
			if(StringUtils.isNotBlank(contact.getFirstNameEn()))contactNameEn = contact.getFirstNameEn();
			if(StringUtils.isNotBlank(contact.getLastNameEn()))contactNameEn = contactNameEn + " " + contact.getLastNameEn();
		}
		
		String customerPhoneNo = " ";
		if(customerPhoneList != null && customerPhoneList.size() > 0){
			customerPhoneNo = StringUtils.join(customerPhoneList, ",");
		}
		
		String contactPhoneNo = " ";
		if(contactPhoneList != null && contactPhoneList.size() > 0){
			contactPhoneNo = StringUtils.join(contactPhoneList, ",");
		}
		
		String remark = StringUtils.isNotBlank(sr.getSrRemark()) ? sr.getSrRemark() : " ";
		log.debug("sr.getSrAfsAssetNo() ====> " + sr.getSrAfsAssetNo());
		if(StringUtils.isNotBlank(sr.getSrAfsAssetNo())){
			remark = String.format("%s %s : %s", ruleProperties.get("label.asset.code"), sr.getSrAfsAssetNo(), remark);
		}
		
		Map<String,String> varMap 	= new HashMap<String,String>();
		varMap.put("SR_ID", 			sr.getSrNo());
		varMap.put("OLD_STATUS", 		(oldSrStatus!=null)?oldSrStatus.getSrStatusName():" ");
		varMap.put("NEW_STATUS", 		(srStatus!=null)?srStatus.getSrStatusName():" ");
		varMap.put("BRANCH_CODE", 		(branch!=null)?branch.getBranchCode():" ");
		varMap.put("BRANCH_NAME", 		(branch!=null)?branch.getBranchName():" ");
		varMap.put("CONTRACT_NO", 		sr.getSrNo());
		varMap.put("ACCOUNT_NO", 		(masterAccount!=null)?masterAccount.getAccountNo():" ");
		varMap.put("PRODUCT_GROUP_NAME", (productGroup!=null)?productGroup.getProductGroupName():" ");
		varMap.put("PRODUCT_NAME", 		(product!=null)?product.getProductName():" ");
		varMap.put("CAMPAIGN_NAME", 	(campaignservice!=null)?campaignservice.getCampaignserviceName():" ");
		varMap.put("TYPE_NAME",			(type!=null)?type.getTypeName():" ");
		varMap.put("AREA_NAME",			(area!=null)?area.getAreaName():" ");
		varMap.put("SUBAREA_NAME", 		(subarea!=null)?subarea.getSubareaName():" ");
		varMap.put("CHANNEL_NAME", 		(channel!=null)?channel.getChannelName():" ");
		varMap.put("STATUS", 			(srStatus!=null)?srStatus.getSrStatusName():" ");
		varMap.put("CUSTOMER_NAME",		customerName);
		varMap.put("SUBSCRIPTION_ID", 	(customer!=null)?customer.getCardNo():" ");
		varMap.put("OWNER", 			ownerUser.getFirstName() + " " + ownerUser.getLastName());
		varMap.put("ASSIGNED_DATE", 	sr.getRuleAssignDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleAssignDate()):" ");
		varMap.put("DELEGATE", 			(delegateUser!=null)?delegateUser.getFirstName() + " " + delegateUser.getLastName():" ");
		varMap.put("DELEGATE_DATE", 	sr.getRuleDelegateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleDelegateDate()):" ");
		varMap.put("CREATE_BY", 		(createUser!=null)?createUser.getFirstName() + " " + createUser.getLastName():" ");
		varMap.put("CREATE_DATE", 		sr.getCreateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getCreateDate()):" ");
		varMap.put("NEXTSLA", 			sr.getRuleNextSla()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleNextSla()):" ");
		varMap.put("LINK", 				account.getLinkEmail() + sr.getSrNo());
		varMap.put("CUSTOMER_PHONE_NO", customerPhoneNo);
		varMap.put("CONTACT_NAME",	 	StringUtils.isNotBlank(contactNameTh) ? contactNameTh : contactNameEn);
		varMap.put("CONTACT_PHONE_NO", 	contactPhoneNo);
		varMap.put("SUBJECT", 			sr.getSrSubject());
		varMap.put("REMARK", 			remark);
		varMap.put("ACTIVITY_DESC", 	(activity!=null)?activity.getSrActivityDesc():" ");
		
		return emailUtil.createMailMessage(template, varMap);
	}
	
	private MailBean emailTemplateOverSLA(String template, SR sr, Account account, User ownerUser, User delegateUser){
		Customer 		customer			= accountDao.getCustomer(sr.getCustomerId());
		Campaignservice campaignservice		= accountDao.getCampaignservice(sr.getCampaignserviceId());
		Branch 			branch 				= accountDao.getBranchById(ownerUser.getBranchId());
		MasterAccount 	masterAccount		= accountDao.getMasterAccount(sr.getAccountId());
		ProductGroup 	productGroup		= accountDao.getProductGroup(sr.getProductgroupId());
		Product 		product				= accountDao.getProduct(sr.getProductId());
		Type	 		type				= typeDao.getType(sr.getTypeId());
		Area	 		area				= accountDao.getArea(sr.getAreaId());
		Subarea	 		subarea				= accountDao.getSubarea(sr.getSubareaId());
		Channel	 		channel				= accountDao.getChannel(sr.getChannelId());
		SRStatus	 	srStatus			= accountDao.getSRStatus(sr.getSrStatusId());
		User 			createUser 			= (sr.getCreateUser()!=null)?userDao.getUser(sr.getCreateUser()):null;
		Contact			contact 			= accountDao.getContact(sr.getContactId());
		Activity 		activity			= accountDao.getActivity(sr.getSrId());
		List<String>	customerPhoneList 	= accountDao.getCustomerPhoneList(sr.getCustomerId());
		List<String>	contactPhoneList	= accountDao.getContactPhoneList(sr.getContactId());
		
		String customerName = " ";
		if(customer != null){
			if(StringUtils.isNotBlank(customer.getFirstNameTh()))customerName = customer.getFirstNameTh();
			if(StringUtils.isNotBlank(customer.getLastNameTh()))customerName = customerName + " " + customer.getLastNameTh();
		}
		
		String contactNameTh = "", contactNameEn = "";
		if(contact != null){
			if(StringUtils.isNotBlank(contact.getFirstNameTh()))contactNameTh = contact.getFirstNameTh();
			if(StringUtils.isNotBlank(contact.getLastNameTh()))contactNameTh = contactNameTh + " " + contact.getLastNameTh();
			if(StringUtils.isNotBlank(contact.getFirstNameEn()))contactNameEn = contact.getFirstNameEn();
			if(StringUtils.isNotBlank(contact.getLastNameEn()))contactNameEn = contactNameEn + " " + contact.getLastNameEn();
		}
		
		String customerPhoneNo = " ";
		if(customerPhoneList != null && customerPhoneList.size() > 0){
			customerPhoneNo = StringUtils.join(customerPhoneList, ",");
		}
		
		String contactPhoneNo = " ";
		if(contactPhoneList != null && contactPhoneList.size() > 0){
			contactPhoneNo = StringUtils.join(contactPhoneList, ",");
		}
		
		String remark = StringUtils.isNotBlank(sr.getSrRemark()) ? sr.getSrRemark() : " ";
		log.debug("sr.getSrAfsAssetNo() ====> " + sr.getSrAfsAssetNo());
		if(StringUtils.isNotBlank(sr.getSrAfsAssetNo())){
			remark = String.format("%s %s : %s", ruleProperties.get("label.asset.code"), sr.getSrAfsAssetNo(), remark);
		}
		
		Map<String,String> varMap 	= new HashMap<String,String>();
		varMap.put("SR_ID", 			sr.getSrNo());
		varMap.put("OVER_SLA", 			"" + account.getRuleThisAlert());
		varMap.put("BRANCH_CODE", 		(branch!=null)?branch.getBranchCode():" ");
		varMap.put("BRANCH_NAME", 		(branch!=null)?branch.getBranchName():" ");
		varMap.put("ACCOUNT_NO", 		(masterAccount!=null)?masterAccount.getAccountNo():" ");
		varMap.put("PRODUCT_GROUP_NAME", (productGroup!=null)?productGroup.getProductGroupName():" ");
		varMap.put("PRODUCT_NAME", 		(product!=null)?product.getProductName():" ");
		varMap.put("CAMPAIGN_NAME", 	(campaignservice!=null)?campaignservice.getCampaignserviceName():" ");
		varMap.put("TYPE_NAME",			(type!=null)?type.getTypeName():" ");
		varMap.put("AREA_NAME",			(area!=null)?area.getAreaName():" ");
		varMap.put("SUBAREA_NAME", 		(subarea!=null)?subarea.getSubareaName():" ");
		varMap.put("CHANNEL_NAME", 		(channel!=null)?channel.getChannelName():" ");
		varMap.put("STATUS", 			(srStatus!=null)?srStatus.getSrStatusName():" ");
		varMap.put("CUSTOMER_NAME",		customerName);
		varMap.put("SUBSCRIPTION_ID", 	(customer!=null)?customer.getCardNo():" ");
		varMap.put("OWNER", 			ownerUser.getFirstName() + " " + ownerUser.getLastName());
		varMap.put("ASSIGNED_DATE", 	sr.getRuleAssignDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleAssignDate()):" ");
		varMap.put("DELEGATE", 			(delegateUser!=null)?delegateUser.getFirstName() + " " + delegateUser.getLastName():" ");
		varMap.put("DELEGATE_DATE", 	sr.getRuleDelegateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleDelegateDate()):" ");
		varMap.put("CREATE_BY", 		(createUser!=null)?createUser.getFirstName() + " " + createUser.getLastName():" ");
		varMap.put("CREATE_DATE", 		sr.getCreateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getCreateDate()):" ");
		varMap.put("NEXTSLA", 			sr.getRuleNextSla()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleNextSla()):" ");
		varMap.put("LINK", 				account.getLinkEmail() + sr.getSrNo());
		varMap.put("CUSTOMER_PHONE_NO", customerPhoneNo);
		varMap.put("CONTACT_NAME",	 	StringUtils.isNotBlank(contactNameTh) ? contactNameTh : contactNameEn);
		varMap.put("CONTACT_PHONE_NO", 	contactPhoneNo);
		varMap.put("SUBJECT", 			sr.getSrSubject());
		varMap.put("REMARK", 			remark);
		varMap.put("ACTIVITY_DESC", 	(activity!=null)?activity.getSrActivityDesc():" ");
		
		return emailUtil.createMailMessage(template, varMap);
	}
	
	private MailBean emailTemplateCustomer(String template, SR sr, Account account, User ownerUser, User delegateUser){
		Customer 		customer			= accountDao.getCustomer(sr.getCustomerId());
		User 			createUser 			= (sr.getCreateUser()!=null)?userDao.getUser(sr.getCreateUser()):null;
		
		String customerName = " ";
		if(customer != null){
			if(StringUtils.isNotBlank(customer.getFirstNameTh()))customerName = customer.getFirstNameTh();
			if(StringUtils.isNotBlank(customer.getLastNameTh()))customerName = customerName + " " + customer.getLastNameTh();
		}
		Map<String,String> varMap 	= new HashMap<String,String>();
		varMap.put("SR_ID", 			sr.getSrNo());
		varMap.put("CUSTOMER_NAME",		customerName);
		varMap.put("SUBSCRIPTION_ID", 	(customer!=null)?customer.getCardNo():" ");
		varMap.put("CREATE_BY", 		(createUser!=null)?createUser.getFirstName() + " " + createUser.getLastName():" ");
		varMap.put("CREATE_DATE", 		sr.getCreateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getCreateDate()):" ");

		return emailUtil.createMailMessage(template, varMap);
	}
	
	private MailBean emailTemplateNote(String template, SR sr, Account account, User ownerUser, User delegateUser, Activity activity){
		Customer 		customer			= accountDao.getCustomer(sr.getCustomerId());
		Campaignservice campaignservice		= accountDao.getCampaignservice(sr.getCampaignserviceId());
		Branch 			branch 				= accountDao.getBranchById(ownerUser.getBranchId());
		MasterAccount 	masterAccount		= accountDao.getMasterAccount(sr.getAccountId());
		ProductGroup 	productGroup		= accountDao.getProductGroup(sr.getProductgroupId());
		Product 		product				= accountDao.getProduct(sr.getProductId());
		Type	 		type				= typeDao.getType(sr.getTypeId());
		Area	 		area				= accountDao.getArea(sr.getAreaId());
		Subarea	 		subarea				= accountDao.getSubarea(sr.getSubareaId());
		Channel	 		channel				= accountDao.getChannel(sr.getChannelId());
		SRStatus	 	srStatus			= accountDao.getSRStatus(sr.getSrStatusId());
		User 			createUser 			= (sr.getCreateUser()!=null)?userDao.getUser(sr.getCreateUser()):null;
		
		String customerName = " ";
		if(customer != null){
			if(StringUtils.isNotBlank(customer.getFirstNameTh()))customerName = customer.getFirstNameTh();
			if(StringUtils.isNotBlank(customer.getLastNameTh()))customerName = customerName + " " + customer.getLastNameTh();
		}
		Map<String,String> varMap 	= new HashMap<String,String>();
		varMap.put("SR_ID", 			sr.getSrNo());
		varMap.put("SUBJECT_ACTIVITY", 	activity.getSrActivityId().toString());
		varMap.put("BRANCH_CODE", 		(branch!=null)?branch.getBranchCode():" ");
		varMap.put("BRANCH_NAME", 		(branch!=null)?branch.getBranchName():" ");
		varMap.put("ACCOUNT_NO", 		(masterAccount!=null)?masterAccount.getAccountNo():" ");
		varMap.put("PRODUCT_GROUP_NAME", (productGroup!=null)?productGroup.getProductGroupName():" ");
		varMap.put("PRODUCT_NAME", 		(product!=null)?product.getProductName():" ");
		varMap.put("CAMPAIGN_NAME", 	(campaignservice!=null)?campaignservice.getCampaignserviceName():" ");
		varMap.put("TYPE_NAME",			(type!=null)?type.getTypeName():" ");
		varMap.put("AREA_NAME",			(area!=null)?area.getAreaName():" ");
		varMap.put("SUBAREA_NAME", 		(subarea!=null)?subarea.getSubareaName():" ");
		varMap.put("CHANNEL_NAME", 		(channel!=null)?channel.getChannelName():" ");
		varMap.put("STATUS", 			(srStatus!=null)?srStatus.getSrStatusName():" ");
		varMap.put("CUSTOMER_NAME",		customerName);
		varMap.put("SUBSCRIPTION_ID", 	(customer!=null)?customer.getCardNo():" ");
		varMap.put("OWNER", 			ownerUser.getFirstName() + " " + ownerUser.getLastName());
		varMap.put("ASSIGNED_DATE", 	sr.getRuleAssignDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleAssignDate()):" ");
		varMap.put("DELEGATE", 			(delegateUser!=null)?delegateUser.getFirstName() + " " + delegateUser.getLastName():" ");
		varMap.put("DELEGATE_DATE", 	sr.getRuleDelegateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleDelegateDate()):" ");
		varMap.put("CREATE_BY", 		(createUser!=null)?createUser.getFirstName() + " " + createUser.getLastName():" ");
		varMap.put("CREATE_DATE", 		sr.getCreateDate()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getCreateDate()):" ");
		varMap.put("NEXTSLA", 			sr.getRuleNextSla()!=null?CalendarHelper.DATETIME_FORMAT.format(sr.getRuleNextSla()):" ");

		return emailUtil.createMailMessage(template, varMap);
	}
	
	private boolean isValidateEmail(String email){
		if(StringUtils.isNotBlank(email)){
			Pattern pattern = Pattern.compile("^[\\w\\-]([\\.\\w])+[\\w]+@([\\w\\-]+\\.)+[A-Z]{2,4}$",Pattern.CASE_INSENSITIVE);
		    Matcher matcher = pattern.matcher(email);
		    return matcher.matches();
		}
	    return false;
	}
	
	private int minutesDiff(Date earlierDate, Date laterDate){
	    if( earlierDate == null || laterDate == null ) return 0;
	    return (int)((laterDate.getTime()/60000) - (earlierDate.getTime()/60000));
	}
	
	public void setOptionDao(OptionDao optionDao) {
		this.optionDao = optionDao;
	}

	public void setCalendarDao(CalendarDao calendarDao) {
		this.calendarDao = calendarDao;
	}

	public void setUserDao(UserDao userDao) {
		this.userDao = userDao;
	}
	public void setAccountDao(AccountDao accountDao) {
		this.accountDao = accountDao;
	}
	public void setTypeDao(TypeDao typeDao) {
		this.typeDao = typeDao;
	}

	public void setEmailUtil(MailUtil emailUtil) {
		this.emailUtil = emailUtil;
	}

	public void setPrepareEmailDao(PrepareEmailDao prepareEmailDao) {
		this.prepareEmailDao = prepareEmailDao;
	}

	public void setRuleProperties(Properties ruleProperties) {
		this.ruleProperties = ruleProperties;
	}
}
