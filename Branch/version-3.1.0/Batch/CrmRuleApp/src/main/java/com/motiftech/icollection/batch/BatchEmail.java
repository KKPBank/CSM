package com.motiftech.icollection.batch;

import java.io.IOException;
import java.util.List;

import com.icollection.batch.AbstractBatch;
import com.icollection.bean.AccColleMailBean;
import com.motiftech.icollection.service.IBatchEmailService;

public class BatchEmail extends AbstractBatch{
	public void run(String... args){
		IBatchEmailService batchEmailService = getBean("batchEmailService",IBatchEmailService.class);
		log.info("START BatchEmail");
		List<AccColleMailBean> listBean = batchEmailService.getListAccount();
		log.debug("listBean.size() : " + listBean.size());
		batchEmailService.sendEmail(listBean);
		log.info("END BatchEmail");
	}
	
	public static void main(String[]args) throws IOException{
		BatchEmail batch = new BatchEmail();
		batch.start(args);
	}
}
