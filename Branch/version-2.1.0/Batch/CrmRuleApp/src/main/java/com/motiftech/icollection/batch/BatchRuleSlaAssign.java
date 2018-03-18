package com.motiftech.icollection.batch;

import java.util.Calendar;
import java.util.Properties;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

import org.apache.commons.lang.StringUtils;

import com.icollection.batch.AbstractBatch;
import com.icollection.common.constants.InquiryConstants;
import com.motiftech.common.util.CalendarHelper;
import com.motiftech.icollection.RuleSlaAssign;
import com.motiftech.icollection.entity.Option;
import com.motiftech.icollection.mapping.AccountMapping;
import com.motiftech.icollection.service.AccountService;
import com.utils.ApplicationContextHolder;


public class BatchRuleSlaAssign extends AbstractBatch{
	private final ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(1);
	private long INITIAL_DELAY		= 0;
	private long PERIOD				= 30;
	private TimeUnit TIMEUNIT 		= TimeUnit.SECONDS;
	private TimeUnit END_TIMEUNIT 	= TimeUnit.SECONDS;
	private long END_CURRENT_TIME 	= 0;
	private int HOUR 				= 20;
	private int MIN 				= 0;
	
	public void init(){
		try{
			Properties ruleProperties 	= getBean("ruleProperties",Properties.class);
			String initDelay 			= ruleProperties.getProperty("rule.scheduled.initdelay");
			String period 				= ruleProperties.getProperty("rule.scheduled.period");
			String timeunit 			= ruleProperties.getProperty("rule.scheduled.timeunit");
			String endTimeUnit 			= ruleProperties.getProperty("rule.scheduled.timeunit.end");
			
			AccountService accountService = ApplicationContextHolder.getContext().getBean(AccountService.class);
			Option workTime 			  = accountService.getWorkTime(InquiryConstants.Option.OPTION_TYPE, InquiryConstants.Option.OPTION_CODE_END);
			if(workTime != null){
				String[] times = StringUtils.split(workTime.getOptionDesc()!=null?workTime.getOptionDesc():InquiryConstants.Time.START_TIME,":");
				log.debug("workTime : " + workTime.getOptionDesc());
				for(int x=0;x<times.length;x++){
					if(x == 0)this.HOUR = Integer.parseInt(times[x]);
					else this.MIN = Integer.parseInt(times[x]);
				}
			}
			
			if(StringUtils.isNotBlank(initDelay))this.INITIAL_DELAY = Long.parseLong(initDelay);
			if(StringUtils.isNotBlank(period))this.PERIOD = Long.parseLong(period);
			if(StringUtils.isNotBlank(timeunit)){
				TimeUnit tUnit = getTimeUnit(timeunit);	
				if(tUnit!=null)this.TIMEUNIT = tUnit;
			}
			if(StringUtils.isNotBlank(endTimeUnit)){
				TimeUnit tUnit = getTimeUnit(endTimeUnit);	
				if(tUnit!=null)this.END_TIMEUNIT = tUnit;
			}
			Calendar calToEndTime 	= CalendarHelper.getCurrentDateTime();
			Calendar calCurrent 	= CalendarHelper.getCurrentDateTime();
			CalendarHelper.setTime(calToEndTime, HOUR, MIN, 00);
			this.END_CURRENT_TIME 	= AccountMapping.minutesDiff(calCurrent.getTime(), calToEndTime.getTime());
			log.info("INITIAL_DELAY : " + INITIAL_DELAY);
			log.info("PERIOD        : " + PERIOD);
			log.info("TIMEUNIT      : " + TIMEUNIT);
			log.info("END_TIME      : " + END_CURRENT_TIME);
			log.info("END_TIMEUNIT  : " + END_TIMEUNIT);
			log.info("END-ASSIGNED	: " + calToEndTime.getTime());
		}catch(RuntimeException ex){
			log.error("init BatchRuleSlaAssign", ex);
			init();
		}
	}
	
	private TimeUnit getTimeUnit(String timeunit){
		if("s".equalsIgnoreCase(timeunit))return TimeUnit.SECONDS;
		else if("m".equalsIgnoreCase(timeunit))return TimeUnit.MINUTES;
		else if("h".equalsIgnoreCase(timeunit))return TimeUnit.HOURS;
		else if("d".equalsIgnoreCase(timeunit))return TimeUnit.DAYS;
		return null;
	}
	
	public void run(String... args){
		try{
			init();
			final RuleSlaAssign runner= new RuleSlaAssign(args);
			final ScheduledFuture<?> scheduledHandle = scheduler.scheduleAtFixedRate(runner, INITIAL_DELAY, PERIOD, TIMEUNIT );
			final Runnable STOP_RUNABLE = new Runnable() { public void run() { scheduledHandle.cancel(true); scheduler.shutdown(); }};
			//Incase you want to kill this after some time like 24 hours
			scheduler.schedule(STOP_RUNABLE, END_CURRENT_TIME, END_TIMEUNIT );
		}catch(RuntimeException ex){
			log.error("run BatchRuleSlaAssign", ex);
		}
	}

	public static void main(String[] args) {
		System.out.println("=========== START Initial BatchRuleSlaAssign ============");
		BatchRuleSlaAssign test = new BatchRuleSlaAssign();
		test.run(args);
		System.out.println("=========== END Initial BatchRuleSlaAssign ============");
	}
}
