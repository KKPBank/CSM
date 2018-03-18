package com.motiftech.icollection.process;

import com.motiftech.common.process.AbstractProcess;
import com.motiftech.icollection.batch.BatchEmail;

public class SendEmailProcess extends AbstractProcess {
	
	public SendEmailProcess() {
		super("");
	}
	
	@Override
	protected void doExecute() {
		try{
			String[] args={};
			try{
				BatchEmail batchEmail = new BatchEmail();
				batchEmail.start(args);
				batchEmail=null;
			}catch(Exception e){
				log.error("Error BatchEmail:",e);
			}
		}catch(RuntimeException ex){
			log.error("SendSmsProcess", ex);
		}
	}
	
}
