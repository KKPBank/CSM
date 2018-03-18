package com.motiftech.icollection.batch;

import com.icollection.batch.AbstractBatch;
import com.motiftech.icollection.RuleCancelSR;

public class BatchRuleCancelSR extends AbstractBatch{
	
	public void init(){
	}
	
	public void run(String... args){
		try{
			init();
			log.debug("START BatchRuleCancelSR");
			final RuleCancelSR runner = new RuleCancelSR(args);
			runner.run();
			log.debug("END BatchRuleCancelSR");
		}catch(RuntimeException ex){
			log.error("run BatchRuleCancelSR", ex);
		}
	}

	public static void main(String[] args) {
		BatchRuleCancelSR test = new BatchRuleCancelSR();
		test.run(args);
	}
	
}
