package com.motiftech.icollection;

import java.util.Date;
import java.util.Locale;

import org.apache.log4j.Logger;

import com.icollection.process.RuleProcess;
import com.motiftech.icollection.process.SendEmailProcess;

public class RuleEmailAssign implements Runnable{

	private static final Logger log = Logger.getLogger(RuleEmailAssign.class);
	String[] args;
	public static void main(String[] args){
		RuleEmailAssign rule = new RuleEmailAssign(args);
		rule.run();
	}
	public RuleEmailAssign(String[] args) {	
		this.args = args;
	}
	
	public void run() {
		try{
			Locale.setDefault(Locale.ENGLISH);
			
			log.info("start :"+new Date());
			int threadCount = 1;
			if(args.length>0){
				try{
					threadCount = Integer.parseInt(args[0]);
					if(threadCount<=0){
						threadCount = 1;
					}
				}catch(NumberFormatException ex){
					log.warn("arguments is not a number, using default value of 1 thread");
				}
			}
			
			log.debug("thread no="+threadCount);
			
			RuleProcess[] runners = new RuleProcess[threadCount];
			for(int i=0;i<threadCount;i++){				
				runners[i] = new SendEmailProcess();
				runners[i].execute();
			}
			
			boolean complete = false;
			while(!complete){
				complete=true;
				for(int j=0;j<runners.length;j++){
					if(runners[j]==null)continue;
					if(!runners[j].isFinished()){
						complete=false;
					}else{
						log.info("thread["+j+"] completed");
						runners[j]=null;
					}
				}
			}
			log.info("finish :"+new Date());
		}catch(RuntimeException ex){
			log.error("runtime error(RuleEmailAssign)",ex);
		}catch(Exception ex){
			log.error("error(RuleEmailAssign)",ex);
		}
	}
}
