package com.motiftech.icollection.model;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import org.apache.log4j.Logger;

import com.motiftech.common.util.CalendarHelper;
import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.service.AccountService;
import com.motiftech.rules.RuleModel;

public class Account implements RuleModel{
	Logger log = Logger.getLogger(this.getClass());
	
	private AccountService service;
	private Short 	hourStartByBranch;
	private Short 	hourEndByBranch;
	private Short 	minuteStartByBranch;
	private Short 	minuteEndByBranch;
	private String 	timeStart;
	private String 	timeOut;
	private Date 	currentDate;
	private int 	SLAMinute 	= 0;
	private int 	SLATime		= 0;
	private int 	alertChiefTimes		= 0;
	private int 	alertChief1Times	= 0;
	private int 	alertChief2Times	= 0;
	private int 	alertChief3Times	= 0;
	private int 	SLADay		= 0;
	private int 	workingTime = 0;
	private int 	remainTime  = 0;
	private int 	holiday 	= 0;
	private Integer	srId;
	private Integer ownerBranchId;
	private Integer ownerUserId;
	private Integer delegateUserId;
	private String 	delegateFlag;
	private Date 	currentSLA;
	private Date 	nextSLA;
	private Date 	ruleStatusDate;
	private boolean nextSLAFlag = false;
	private Integer createId;
	private Integer oldStatusId;
	private Date 	assignDate;
	private String	ruleEmailFlag;
	private Integer customerId;
	private int 	ruleThisAlert;
	private int 	ruleThisWork;
	private String	linkEmail;
	private Integer channelId;
	private Integer statusId;
	private String  updateStatus;

	private KeysBean keysBean = new KeysBean();
	private String 	 actionType;
	private List<String> process = new ArrayList<String>();
	
	public void consolidate(){
		if(service!=null){
			SR newSr = service.consolidate(customerId);
			if(newSr != null && newSr.getOwnerUserId() != null){
				this.ownerUserId	= newSr.getOwnerUserId();
				this.delegateUserId = newSr.getDelegateUserId();
				this.actionType 	= RuleConstants.RULE_ACTION.CONSOLIDATE;
			}
		}
	}
	
	public void checkMapProduct(){
		if(service!=null){
			log.debug("checkMapProduct");
			MapProduct mapProduct = service.checkMapProduct(keysBean);
			if(mapProduct != null && mapProduct.getDefaultOwnerUserId() != null){
				this.ownerUserId	= mapProduct.getDefaultOwnerUserId();
				this.actionType 	= RuleConstants.RULE_ACTION.SYSTEM_ASSIGN;
			}
		}
	}
	
	public void autoForward(){
		if(service!=null){
			log.debug("autoForward");
			AutoForward autoForward = service.autoForward(keysBean, channelId);
			if(autoForward != null && autoForward.getForwardToUser() != null){
				this.ownerUserId	= autoForward.getForwardToUser();
				this.actionType 	= RuleConstants.RULE_ACTION.AUTO_FORWARD_ASSIGN;
			}
		}
	}
	
	public String checkSLADay(){
		if(SLADay > 0){
			Calendar currentDateCalendar = Calendar.getInstance();
			currentDateCalendar.setTime(this.currentDate);
			Calendar statusDate = Calendar.getInstance();
			if(this.ruleStatusDate != null)statusDate.setTime(this.ruleStatusDate);
			if(CalendarHelper.dateDiff(currentDateCalendar, statusDate) >= SLADay)return RuleConstants.NOTPASS;
		}
		return RuleConstants.PASS;
	}
	
	public void updateSR(){
		if(service!=null)service.updateSR(srId,this);
	}
	
	public void updateSRStatus(String status){
		if(service != null){
			actionType 		= RuleConstants.STATUS.CHANGE_STATUS;
			updateStatus 	= status;
			service.updateSRStatus(srId,this);
		}
	}
	
	public void enterProcess(String p) {
		this.process.add(p);
	}
	
	public void debug(String s){
		log.debug("SLA : " + s);
	}
	
	public void sendEmail(String template) {
		if(service!=null)service.sendEmail(srId, template, this);
	}
	
	public void sendDelegateEmail(String template) {
		if(service!=null)service.sendDelegateEmail(srId, template, this);
	}
	
	public void sendSLAEmail(String template) {
		if(service!=null)service.sendSLAEmail(srId, template, this);
	}
	
	public void sendSuperVisorEmail(String template) {
		if(service!=null)service.sendSuperVisorEmail(srId, template, this);
	}
	
	public void increaseCounting(){
		ruleThisAlert++;
		ruleThisWork += SLAMinute;
	}
	
	public String checkOldStatus(){
		if(oldStatusId != null && oldStatusId.compareTo(statusId) != 0)return RuleConstants.PASS;
		return RuleConstants.NOTPASS;
	}
	
	public String checkNextSLAFlag(){
		return (nextSLAFlag)?RuleConstants.PASS:RuleConstants.NOTPASS;
	}
	
	public String checkSLAMapping(){
		return (nextSLA == null && SLAMinute > 0)?RuleConstants.PASS:RuleConstants.NOTPASS;
	}
	
	public String checkRuleThisAlert(){
		if(ruleThisAlert >= SLATime)return RuleConstants.PASS;
		if(alertChiefTimes  > 0 && ruleThisAlert >= alertChiefTimes)return RuleConstants.PASS;
		if(alertChief1Times > 0 && ruleThisAlert >= alertChief1Times)return RuleConstants.PASS;
		if(alertChief2Times > 0 && ruleThisAlert >= alertChief2Times)return RuleConstants.PASS;
		if(alertChief3Times > 0 && ruleThisAlert >= alertChief3Times)return RuleConstants.PASS;
		
		return RuleConstants.NOTPASS;
		
		//return (ruleThisAlert >= SLATime)?RuleConstants.PASS:RuleConstants.NOTPASS;
	}

	public Integer getCustomerId() {
		return customerId;
	}

	public void setCustomerId(Integer customerId) {
		this.customerId = customerId;
	}

	public AccountService getService() {
		return service;
	}

	public void setService(AccountService service) {
		this.service = service;
	}

	public Short getHourStartByBranch() {
		return hourStartByBranch;
	}

	public void setHourStartByBranch(Short hourStartByBranch) {
		this.hourStartByBranch = hourStartByBranch;
	}

	public Short getHourEndByBranch() {
		return hourEndByBranch;
	}

	public void setHourEndByBranch(Short hourEndByBranch) {
		this.hourEndByBranch = hourEndByBranch;
	}

	public Short getMinuteStartByBranch() {
		return minuteStartByBranch;
	}

	public void setMinuteStartByBranch(Short minuteStartByBranch) {
		this.minuteStartByBranch = minuteStartByBranch;
	}

	public Short getMinuteEndByBranch() {
		return minuteEndByBranch;
	}

	public void setMinuteEndByBranch(Short minuteEndByBranch) {
		this.minuteEndByBranch = minuteEndByBranch;
	}

	public String getTimeStart() {
		return timeStart;
	}

	public void setTimeStart(String timeStart) {
		this.timeStart = timeStart;
	}

	public String getTimeOut() {
		return timeOut;
	}

	public void setTimeOut(String timeOut) {
		this.timeOut = timeOut;
	}

	public int getSLAMinute() {
		return SLAMinute;
	}

	public void setSLAMinute(int sLAMinute) {
		SLAMinute = sLAMinute;
	}

	public Date getNextSLA() {
		return nextSLA;
	}

	public void setNextSLA(Date nextSLA) {
		this.nextSLA = nextSLA;
	}

	public Integer getOwnerUserId() {
		return ownerUserId;
	}

	public void setOwnerUserId(Integer ownerUserId) {
		this.ownerUserId = ownerUserId;
	}

	public Integer getSrId() {
		return srId;
	}

	public void setSrId(Integer srId) {
		this.srId = srId;
	}

	public Integer getDelegateUserId() {
		return delegateUserId;
	}

	public void setDelegateUserId(Integer delegateUserId) {
		this.delegateUserId = delegateUserId;
	}

	public Integer getCreateId() {
		return createId;
	}

	public void setCreateId(Integer createId) {
		this.createId = createId;
	}

	public Integer getStatusId() {
		return statusId;
	}

	public void setStatusId(Integer statusId) {
		this.statusId = statusId;
	}

	public Integer getChannelId() {
		return channelId;
	}

	public void setChannelId(Integer channelId) {
		this.channelId = channelId;
	}

	public Date getAssignDate() {
		return assignDate;
	}

	public void setAssignDate(Date assignDate) {
		this.assignDate = assignDate;
	}

	public String getRuleEmailFlag() {
		return ruleEmailFlag;
	}

	public void setRuleEmailFlag(String ruleEmailFlag) {
		this.ruleEmailFlag = ruleEmailFlag;
	}

	public boolean isNextSLAFlag() {
		return nextSLAFlag;
	}

	public void setNextSLAFlag(boolean nextSLAFlag) {
		this.nextSLAFlag = nextSLAFlag;
	}

	public int getHoliday() {
		return holiday;
	}

	public void setHoliday(int holiday) {
		this.holiday = holiday;
	}

	public List<String> getProcess() {
		return process;
	}

	public void setProcess(List<String> process) {
		this.process = process;
	}

	public int getSLATime() {
		return SLATime;
	}

	public void setSLATime(int sLATime) {
		SLATime = sLATime;
	}

	public int getSLADay() {
		return SLADay;
	}

	public void setSLADay(int sLADay) {
		SLADay = sLADay;
	}

	public Integer getOldStatusId() {
		return oldStatusId;
	}

	public void setOldStatusId(Integer oldStatusId) {
		this.oldStatusId = oldStatusId;
	}

	public int getRuleThisAlert() {
		return ruleThisAlert;
	}

	public void setRuleThisAlert(int ruleThisAlert) {
		this.ruleThisAlert = ruleThisAlert;
	}

	public Integer getOwnerBranchId() {
		return ownerBranchId;
	}

	public void setOwnerBranchId(Integer ownerBranchId) {
		this.ownerBranchId = ownerBranchId;
	}

	public int getRuleThisWork() {
		return ruleThisWork;
	}

	public void setRuleThisWork(int ruleThisWork) {
		this.ruleThisWork = ruleThisWork;
	}

	public int getWorkingTime() {
		return workingTime;
	}

	public void setWorkingTime(int workingTime) {
		this.workingTime = workingTime;
	}

	public int getRemainTime() {
		return remainTime;
	}

	public void setRemainTime(int remainTime) {
		this.remainTime = remainTime;
	}

	public Date getCurrentSLA() {
		return currentSLA;
	}

	public void setCurrentSLA(Date currentSLA) {
		this.currentSLA = currentSLA;
	}

	public Date getCurrentDate() {
		return currentDate;
	}

	public void setCurrentDate(Date currentDate) {
		this.currentDate = currentDate;
	}

	public String getActionType() {
		return actionType;
	}

	public void setActionType(String actionType) {
		this.actionType = actionType;
	}

	public KeysBean getKeysBean() {
		return keysBean;
	}

	public void setKeysBean(KeysBean keysBean) {
		this.keysBean = keysBean;
	}

	public String getLinkEmail() {
		return linkEmail;
	}

	public void setLinkEmail(String linkEmail) {
		this.linkEmail = linkEmail;
	}

	public Date getRuleStatusDate() {
		return ruleStatusDate;
	}

	public void setRuleStatusDate(Date ruleStatusDate) {
		this.ruleStatusDate = ruleStatusDate;
	}

	public String getUpdateStatus() {
		return updateStatus;
	}

	public void setUpdateStatus(String updateStatus) {
		this.updateStatus = updateStatus;
	}

	public String getDelegateFlag() {
		return delegateFlag;
	}

	public void setDelegateFlag(String delegateFlag) {
		this.delegateFlag = delegateFlag;
	}

	public int getAlertChiefTimes() {
		return alertChiefTimes;
	}

	public void setAlertChiefTimes(int alertChiefTimes) {
		this.alertChiefTimes = alertChiefTimes;
	}

	public int getAlertChief1Times() {
		return alertChief1Times;
	}

	public void setAlertChief1Times(int alertChief1Times) {
		this.alertChief1Times = alertChief1Times;
	}

	public int getAlertChief2Times() {
		return alertChief2Times;
	}

	public void setAlertChief2Times(int alertChief2Times) {
		this.alertChief2Times = alertChief2Times;
	}

	public int getAlertChief3Times() {
		return alertChief3Times;
	}

	public void setAlertChief3Times(int alertChief3Times) {
		this.alertChief3Times = alertChief3Times;
	}
}
