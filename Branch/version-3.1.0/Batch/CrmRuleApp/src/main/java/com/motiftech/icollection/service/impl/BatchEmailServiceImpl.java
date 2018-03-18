package com.motiftech.icollection.service.impl;

import java.util.ArrayList;
import java.util.List;

import org.apache.log4j.Logger;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.icollection.bean.AccColleMailBean;
import com.icollection.common.constants.InquiryConstants;
import com.motiftech.common.util.CalendarHelper;
import com.motiftech.icollection.constants.RuleConstants;
import com.motiftech.icollection.dao.PrepareEmailDao;
import com.motiftech.icollection.dao.UserDao;
import com.motiftech.icollection.entity.PrepareEmail;
import com.motiftech.icollection.entity.User;
import com.motiftech.icollection.service.IBatchEmailService;
import com.utils.EmailUtil;

@Service(value = "batchEmailService")
public class BatchEmailServiceImpl implements IBatchEmailService {

	private final Logger log 	= Logger.getLogger(getClass());
	private PrepareEmailDao prepareEmailDao;
	private UserDao 		userDao;
	private EmailUtil 		emailUtil;
	
	@Transactional(readOnly = true)
	public List<AccColleMailBean> getListAccount() {
		log.debug("Email : getListAccount");
		List<AccColleMailBean> listBean = new ArrayList<AccColleMailBean>();
		List<PrepareEmail> listEmail = prepareEmailDao.getDataEmail();
		for (PrepareEmail email : listEmail) {
			AccColleMailBean bean = new AccColleMailBean();
			bean.setEmailId(email.getSrPrepareEmailId());
			bean.setSender(email.getEmailSender());
			bean.seteMail(email.getEmailAddress());
			bean.setMessage(email.getEmailContent());
			bean.setSubject(email.getEmailSubject());
			listBean.add(bean);
		}
		return listBean;
	}

	@Transactional
	public void sendEmail(List<AccColleMailBean> listBean) {
		for (AccColleMailBean bean : listBean) {
			String[] to 	= bean.geteMail().split(",");
			String from		= bean.getSender();
			String subject 	= bean.getSubject();
			String message 	= bean.getMessage();
			boolean result 	= false;
			try {
				result = emailUtil.sendSimpleMail(from, to, subject, message);
				log.info("from " + from + " send to " + bean.geteMail() + " success");
				result = true;
			} catch (RuntimeException ex) {
				log.error("send to " + bean.geteMail() + " , fail : ", ex);
			}

			try{
				User systemUser = userDao.getUserByUserName(RuleConstants.RULE_USER);
				PrepareEmail prepareEmail = prepareEmailDao.getPrepareEmail(bean.getEmailId());
				prepareEmail.setUpdateUser((systemUser!=null)?systemUser.getUserName():null);
				prepareEmail.setUpdateDate(CalendarHelper.getCurrentDateTime().getTime());
				if(result)prepareEmail.setExportStatus(InquiryConstants.ExportStatus.EXPORTED);
				else prepareEmail.setExportStatus(InquiryConstants.ExportStatus.FAILED);
				prepareEmailDao.updatePrepareEmail(prepareEmail);
			}catch(Exception e){
				log.error("Could not update ExportStatus with EmailId:"+bean.getEmailId()+"|", e);
			}
		}
	}

	public void setEmailUtil(EmailUtil emailUtil) {
		this.emailUtil = emailUtil;
	}

	public void setPrepareEmailDao(PrepareEmailDao prepareEmailDao) {
		this.prepareEmailDao = prepareEmailDao;
	}

	public void setUserDao(UserDao userDao) {
		this.userDao = userDao;
	}
}
