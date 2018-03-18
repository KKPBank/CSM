package com.motiftech.icollection.dao.impl;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Restrictions;

import com.icollection.common.entity.AutoApprove;
import com.motiftech.icollection.dao.AutoApproveDao;

public class AutoApproveDaoImpl implements AutoApproveDao {

	private SessionFactory sessionFactory;

	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}

	public AutoApprove getAutoApprove() {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(AutoApprove.class);
		criteria.add(Restrictions.eq("autoApproveFlag", "1"));
		criteria.add(Restrictions.eq("isDeleted", Boolean.FALSE));
		criteria.setMaxResults(1);
		return (AutoApprove)criteria.uniqueResult();
	}
}
