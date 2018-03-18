package com.motiftech.icollection.dao;

import com.motiftech.icollection.entity.User;

public interface UserDao {
	public User getUser(Integer userId);
	public User getUser(String employeeCode);
	public User getUserByUserName(String userName);
	public User getUserBranch(Integer branchId);
}
