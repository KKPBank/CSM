package com.motiftech.icollection.dao;

import java.util.Date;
import java.util.List;

import com.motiftech.icollection.entity.BranchCalendar;

public interface CalendarDao {
	public Long isHolidayBranch(Date date,Integer branchId);
	public List<BranchCalendar> getAllBranchCalendar();
}
