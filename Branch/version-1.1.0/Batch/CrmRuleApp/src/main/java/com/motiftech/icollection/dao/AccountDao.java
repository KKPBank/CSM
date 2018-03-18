package com.motiftech.icollection.dao;

import java.util.List;

import com.motiftech.icollection.bean.KeysBean;
import com.motiftech.icollection.entity.Activity;
import com.motiftech.icollection.entity.Area;
import com.motiftech.icollection.entity.AutoForward;
import com.motiftech.icollection.entity.Branch;
import com.motiftech.icollection.entity.Campaignservice;
import com.motiftech.icollection.entity.Channel;
import com.motiftech.icollection.entity.Contact;
import com.motiftech.icollection.entity.Customer;
import com.motiftech.icollection.entity.MapProduct;
import com.motiftech.icollection.entity.MasterAccount;
import com.motiftech.icollection.entity.Product;
import com.motiftech.icollection.entity.ProductGroup;
import com.motiftech.icollection.entity.SLA;
import com.motiftech.icollection.entity.SR;
import com.motiftech.icollection.entity.SRLogging;
import com.motiftech.icollection.entity.SRStatus;
import com.motiftech.icollection.entity.Subarea;
import com.motiftech.icollection.entity.VW_SR;

public interface AccountDao {
	public SR 					getConsolidate(Integer customerId);
	public SR 					getSR(Integer srId);
	public Branch 				getBranchById(Integer branchId);
	public void 				updateSR(SR sr);
	public MapProduct 			getMapProduct(KeysBean keysBean);
	public AutoForward 			autoForward(KeysBean keysBean, Integer channelId);
	public Long 				countSRByAssignFlag(String assignedFlag);
	public Long 				countSRFromView();
	public List<SR> 			listSRByAssignFlag(int startIndex,int maxSize, String assignFlag);
	public List<VW_SR> 			listSRFromView(int startIndex,int maxSize);
	public void	 				saveSRLogging(SRLogging srLogging);
	public void 				saveActivity(Activity activity);
	public Customer 			getCustomer(Integer customerId);
	public Activity 			getActivity(Integer srId);
	public SRLogging 			getSRLogging(Integer srId);
	/**
	public List<SLA> 			listAllSLA();
	public SLA 					getSLA();
	/**/
	public SLA 					getSLA(Integer productId, Integer channelId, Integer srStatusId, Integer areaId, Integer typeId, Integer campaignserviceId, Integer subareaId);
	public Product 				getProduct(Integer productId);
	public Area 				getArea(Integer areaId);
	public Subarea 				getSubarea(Integer subareaId);
	public Channel 				getChannel(Integer channelId);
	public SRStatus 			getSRStatus(Integer srStatusIds);
//	public Integer 				getProductGroupId(Integer productId);
	public MasterAccount 		getMasterAccount(Integer accountId);
	public ProductGroup 		getProductGroup(Integer productGroupId);
	public Campaignservice 		getCampaignservice(Integer campaignserviceId);
	public Contact 				getContact(Integer contactId);
	public List<String> 		getCustomerPhoneList(Integer customerId);
	public List<String> 		getContactPhoneList(Integer contactId);
}
