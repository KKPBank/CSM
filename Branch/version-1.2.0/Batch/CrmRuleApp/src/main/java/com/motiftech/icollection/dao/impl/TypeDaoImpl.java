package com.motiftech.icollection.dao.impl;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Restrictions;

import com.motiftech.icollection.dao.TypeDao;
import com.motiftech.icollection.entity.Type;

public class TypeDaoImpl implements TypeDao {

	private SessionFactory sessionFactory;

	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}

	public Type getTypeByName(String typeName) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Type.class);
		criteria.add(Restrictions.eq("typeName", typeName));
		criteria.add(Restrictions.eq("typeIsActive", Boolean.FALSE));
		criteria.setMaxResults(1);
		return (Type)criteria.uniqueResult();
	}
	
	public Type getType(Integer typeId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(Type.class);
		criteria.add(Restrictions.eq("typeId", typeId));
		criteria.setMaxResults(1);
		return (Type)criteria.uniqueResult();
	}
}
