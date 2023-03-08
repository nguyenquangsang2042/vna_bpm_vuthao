package com.example.vnabpmjava.database.dao;

import androidx.room.Dao;
import androidx.room.Insert;
import androidx.room.OnConflictStrategy;

import com.example.vnabpmjava.database.model.User;

import java.util.List;

@Dao
public interface UserDao {
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    void insert(User user);
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    void insertAll(List<User> lstUser);
}
