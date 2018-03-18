package com.motiftech.icollection.process;

import java.util.Collection;
import java.util.Date;

import com.motiftech.common.hibernate.AbstractHibernateProcess;
import com.motiftech.icollection.model.Account;
import com.motiftech.rules.RuleBase;
import com.motiftech.rules.RuleModel;
import com.motiftech.rules.RuleSession;

@Deprecated
public class CollectionRuleProcess extends AbstractHibernateProcess {

	private Collection<Account> facts ;
	
	public CollectionRuleProcess(String processName ,Collection<Account> normalProcessAccounts){
		super(processName);
		this.facts=normalProcessAccounts;
	}
	
	@Override
	protected void doExecute() {
		RuleBase ruleBase = createRuleBase();
		
		int i=1;
		if(ruleBase!=null){
			
			for(RuleModel fact : facts){
				Date c = new Date();
				
				//create RuleSession
				RuleSession ruleSession = ruleBase.newSession();
				
				//set global variable
	//			ruleSession.setGlobal("currentDate", currentDate);
				
				//add facts to RuleBase
				ruleSession.insert(fact);
			
				//execute Rule
				try{
					ruleSession.executeRules();
				}finally{
					ruleSession.dispose();
				}
				
				//flush to DB
				if(i%100==0){
					log.debug("flushing");
					flushSession(); 
				}
				
				Date d = new Date();
				log.debug("account count : "+i++ +" time="+(d.getTime()-c.getTime()));
			}
			
			postProcess(facts);
		}else{
			log.error("cannot find config or rule file(s) for process : "+getProcessName());
		}
	}

	private void postProcess(Collection<Account> facts){
		if(log.isDebugEnabled()){
			for(Account acc : facts)log.debug("account : " + acc.getSrId() + " was executed");
		}
	}
}
