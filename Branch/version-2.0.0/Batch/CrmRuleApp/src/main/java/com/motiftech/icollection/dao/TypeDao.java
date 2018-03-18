package com.motiftech.icollection.dao;

import com.motiftech.icollection.entity.Type;

public interface TypeDao {
	public Type getType(Integer typeId);
	public Type getTypeByName(String typeName);
}
