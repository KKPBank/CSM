package com.motiftech.icollection.dao.impl;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Restrictions;

import com.motiftech.icollection.dao.UserDao;
import com.motiftech.icollection.entity.User;

public class UserDaoImpl implements UserDao {

	private SessionFactory sessionFactory;

	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}

	public User getUser(Integer userId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(User.class);
		criteria.add(Restrictions.eq("userId", userId));
		criteria.setMaxResults(1);
		return (User)criteria.uniqueResult();
	}
	
	public User getUser(String employeeCode) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(User.class);
		criteria.add(Restrictions.eq("employeeCode", employeeCode));
		criteria.setMaxResults(1);
		return (User)criteria.uniqueResult();
	}
	
	public User getUserByUserName(String userName) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(User.class);
		criteria.add(Restrictions.eq("userName", userName));
		criteria.setMaxResults(1);
		return (User)criteria.uniqueResult();
	}
	
	public User getUserBranch(Integer branchId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(User.class);
		criteria.add(Restrictions.eq("branchId", branchId));
		criteria.add(Restrictions.eq("isGroup", true));
		criteria.add(Restrictions.eq("status", ((short)1)));
		criteria.setMaxResults(1);
		return (User)criteria.uniqueResult();
	}
}
