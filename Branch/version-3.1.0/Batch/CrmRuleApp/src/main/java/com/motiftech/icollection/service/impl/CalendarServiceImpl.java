package com.motiftech.icollection.service.impl;

import java.util.ArrayList;
import java.util.List;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.motiftech.icollection.dao.CalendarDao;
import com.motiftech.icollection.entity.BranchCalendar;
import com.motiftech.icollection.service.CalendarService;

@Service("calendarService")
public class CalendarServiceImpl implements CalendarService {
	
	private CalendarDao calendarDao;

	@Transactional
	public List<BranchCalendar> getAllBranchCalendar() {
		List<BranchCalendar> branchCalendars = calendarDao.getAllBranchCalendar();
		return (branchCalendars==null)?new ArrayList<BranchCalendar>():branchCalendars;
	}
	
	public CalendarDao getCalendarDao() {
		return calendarDao;
	}

	public void setCalendarDao(CalendarDao calendarDao) {
		this.calendarDao = calendarDao;
	}

}
