package com.motiftech.icollection.constants;

import java.util.HashMap;
import java.util.Map;

public final class RuleConstants {
	public static final String RULE_USER 	= "SYSTEM";
	public static final String COMMA 		= ",";
	public static final String IS_ACTIVE 	= "1";
	public static final String NOT_ACTIVE 	= "0";

	public static final String PASS 		= "pass";
	public static final String NOTPASS 		= "not pass";
	
	public static class CONFIG{
		public static final String FLOWASSIGNED_PROCESS			= "flowAssigned";
		public static final String FLOWEMAIL_PROCESS			= "flowEmail";
		public static final String SR_PROCESS					= "SRProcess";
		public static final String SLA_PROCESS					= "SLADecision";
		public static final String CANCEL_SR_PROCESS			= "CancelSRDecision";
		
		public static final String RULE_CONFIG_PATH_PROPERTY = "rule.cfg.path";
		
		public static final String RULE_PROPERTY = "rule";
		public static final String FLOW_PROPERTY = "flow";
		
		public static final String NAME_PROPERTY = "name";
		public static final String TYPE_PROPERTY = "type";
	}
	
	public static class EMAIL{
		public static final String NOT_SEND		= "0";
		public static final String SEND			= "1";
		
		public static class TEMPLATE{
			public static final String ASSIGN			= "assign";
			public static final String CHANGE_STATUS	= "changeStatus";
			public static final String OVER_SLA			= "overSLA";
			public static final String CUSTOMER			= "customer";
			public static final String NOTE				= "note";
		}
	}
	
	public class RULE_TYPE{
		public static final int SYSTEM_ASSIGN 	= 1;
		public static final int USER_ASSIGN 	= 2;
		public static final int GROUP_ASSIGN 	= 3;
		public static final int CONSOLIDATE 	= 4;
		public static final int MAP_PRODUCT 	= 5;
		public static final int AUTO_FORWARD 	= 6;
		
		public static final String CHANGE_STATUS 	= "02";
		public static final String DELEGATE 		= "03";
		public static final String TRANSFER 		= "04";
		public static final String RESET_OWNER 		= "07";
		public static final String UPDATE_OWNER	 	= "08";
		public static final String USER_ERROR 		= "12";
	}
	
	public class RULE_ACTION{
		public static final String SYSTEM_ASSIGN 	= "System Assign";
		public static final String USER_ASSIGN 		= "User Assign";
		public static final String GROUP_ASSIGN 	= "Group Assign";
		public static final String CONSOLIDATE 		= "Consolidate";
		public static final String AUTO_FORWARD_ASSIGN = "Auto Forward Assign";
	}
	
	public static Map<String, Integer> stateMap = new HashMap<String, Integer>();
	static {
		stateMap.put("DRAFT", 1);
		stateMap.put("OPEN", 2);
		stateMap.put("WAITINGCUSTOMER", 3);
		stateMap.put("INPROGRESS", 4);
		stateMap.put("ROUTEBACK", 5);
		stateMap.put("CANCELLED", 6);
		stateMap.put("CLOSED", 7);
	}
	
	public class STATUS{
		public static final String DRAFT 			= "DRAFT";
		public static final String OPEN 			= "OPEN";
		public static final String WAITINGCUSTOMER 	= "WAITINGCUSTOMER";
		public static final String INPROGRESS 		= "INPROGRESS";
		public static final String ROUTEBACK 		= "ROUTEBACK";
		public static final String CANCELLED 		= "CANCELLED";
		public static final String CLOSED 			= "CLOSED";
		public static final String CHANGE_STATUS 	= "Change Status";
		
	}
	
	public static class OPTION_TYPE{
		public static final String SR_STATUS = "sr status";
	}
}
