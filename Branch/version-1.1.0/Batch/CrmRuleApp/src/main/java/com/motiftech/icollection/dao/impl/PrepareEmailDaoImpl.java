package com.motiftech.icollection.dao.impl;

import java.util.List;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Restrictions;

import com.icollection.common.constants.InquiryConstants;
import com.motiftech.icollection.dao.PrepareEmailDao;
import com.motiftech.icollection.entity.PrepareEmail;

public class PrepareEmailDaoImpl implements PrepareEmailDao{

	private SessionFactory sessionFactory;
	
	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}

	@SuppressWarnings("unchecked")
	public List<PrepareEmail> getDataEmail() {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(PrepareEmail.class);		
		criteria.add(Restrictions.eq("isDelete", Boolean.FALSE));
		criteria.add(Restrictions.eq("exportStatus", InquiryConstants.ExportStatus.NOT_EXPORTED));		
		return criteria.list();
	}
	
	public PrepareEmail getPrepareEmail(Integer srPrepareEmailId){
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(PrepareEmail.class);
		criteria.add(Restrictions.eq("srPrepareEmailId", srPrepareEmailId));
		criteria.setMaxResults(1);
		return (PrepareEmail)criteria.uniqueResult();
	}
	
	public void updatePrepareEmail(PrepareEmail prepareEmail){
		sessionFactory.getCurrentSession().update(prepareEmail);
	}

	public void savePrepareEmail(PrepareEmail prepareEmail){
		sessionFactory.getCurrentSession().save(prepareEmail);
	}
}
