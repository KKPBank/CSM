package com.motiftech.icollection.dao.impl;

import java.util.Calendar;
import java.util.Date;
import java.util.List;

import org.hibernate.Criteria;
import org.hibernate.SessionFactory;
import org.hibernate.criterion.Projections;
import org.hibernate.criterion.Restrictions;

import com.motiftech.icollection.dao.CalendarDao;
import com.motiftech.icollection.entity.BranchCalendar;

public class CalendarDaoImpl implements CalendarDao {
	
	private SessionFactory sessionFactory;

	public void setSessionFactory(SessionFactory sessionFactory) {
		this.sessionFactory = sessionFactory;
	}

	public Long isHolidayBranch(Date next,Integer branchId) {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(BranchCalendar.class);
		criteria.add(Restrictions.eq("branchId",branchId));
		criteria.add(Restrictions.and(Restrictions.ge("holidayDate",setDay(next,0)), Restrictions.lt("holidayDate",setDay(next,1))));
		criteria.setProjection(Projections.rowCount());	
		return (Long)criteria.uniqueResult();
	}
	
	@SuppressWarnings("unchecked")
	public List<BranchCalendar> getAllBranchCalendar() {
		Criteria criteria = sessionFactory.getCurrentSession().createCriteria(BranchCalendar.class);
		return criteria.list();
	}
	
	private Date setDay(Date date,int day){
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		calendar.set(calendar.get(Calendar.YEAR), calendar.get(Calendar.MONTH), calendar.get(Calendar.DATE) + day, 0, 0, 0);
		calendar.set(Calendar.MILLISECOND, 0);
		return calendar.getTime();
	}
}
