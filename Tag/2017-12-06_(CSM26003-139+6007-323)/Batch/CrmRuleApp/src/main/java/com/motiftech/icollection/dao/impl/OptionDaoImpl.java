package com.motiftech.icollection.dao.impl;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Restrictions;

import com.motiftech.icollection.dao.OptionDao;
import com.motiftech.icollection.entity.Option;

public class OptionDaoImpl implements OptionDao {
	
	private SessionFactory sessionFactory;
	
	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}
	
	public Option getOptDescByOption(String type, String code) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Option.class);
		criteria.add(Restrictions.eq("isDelete", Boolean.FALSE));
		criteria.add(Restrictions.eq("optionCode", code));
		criteria.add(Restrictions.eq("optionType", type));
		criteria.setMaxResults(1);
		return (Option)criteria.uniqueResult();
	}
}
