package com.motiftech.icollection;

import java.util.Date;
import java.util.Locale;

import org.apache.log4j.Logger;

import com.icollection.process.RuleProcess;
import com.motiftech.icollection.process.SlaProcess;
import com.motiftech.icollection.service.AccountService;
import com.utils.ApplicationContextHolder;

public class RuleSlaAssign implements Runnable{

	private static final Logger log = Logger.getLogger(RuleSlaAssign.class);
	String[] args;
	
	public static void main(String[] args){
		RuleSlaAssign rule = new RuleSlaAssign(args);
		rule.run();
	}
	
	public RuleSlaAssign(String[] args) {	
		this.args = args;
	}
	
	public void run() {
		try{
			Locale.setDefault(Locale.ENGLISH);
			log.info("start : " + new Date());
			AccountService accountService 	= ApplicationContextHolder.getContext().getBean(AccountService.class);
			long accountCount 				= accountService.countSRFromView();
			int threadCount 				= 1;
			if(args.length>0){
				try{
					threadCount = Integer.parseInt(args[0]);
					if(threadCount<=0)threadCount = 1;
				}catch(NumberFormatException ex){
					log.warn("arguments is not a number, using default value of 1 thread");
				}catch(Exception ex){
					log.error("Exception", ex);
				}
			}
			
			log.debug("thread no    : " + threadCount);
			log.debug("account size : " + accountCount);
			
			int startIndex = 0;
			int size = (int)(accountCount/threadCount);
			int remainder = (int)accountCount%threadCount;
			RuleProcess[] runners = new RuleProcess[threadCount];
			for(int i=0;i<threadCount;i++){
				int fetchSize = size;
				if(remainder>0){
					fetchSize++;
					remainder--;
				}
				log.debug("thread : " + i + " startIndex = "+startIndex+" fetchSize = "+fetchSize);
				runners[i] = new SlaProcess(startIndex,fetchSize);
				runners[i].execute();
				
				startIndex += fetchSize;
			}
			
			boolean complete = false;
			while(!complete){
				complete = true;
				for(int j=0;j<runners.length;j++){
					if(runners[j] == null)continue;
					if(!runners[j].isFinished())complete = false;
					else{
						log.info("thread["+j+"] completed");
						runners[j]=null;
					}
				}
			}
			log.info("finish : " + new Date());
		}catch(RuntimeException ex){
			log.error("run RuleSlaAssign", ex);
		}catch(Exception ex){
			log.error("run RuleSlaAssign", ex);
		}
	}
}