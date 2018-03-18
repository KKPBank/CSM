package com.motiftech.icollection.dao;

import com.motiftech.icollection.entity.User;

public interface UserDao {
	public User getUser(Integer userId);
	public User getUserByUserName(String userName);
}
