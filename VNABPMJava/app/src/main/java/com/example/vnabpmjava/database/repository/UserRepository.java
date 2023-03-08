package com.example.vnabpmjava.database.repository;

import android.app.Application;

import androidx.lifecycle.LiveData;

import com.example.vnabpmjava.database.Database;
import com.example.vnabpmjava.database.dao.UserDao;
import com.example.vnabpmjava.database.model.User;

import java.util.List;

public class UserRepository {
    private UserDao mUserDao;
    private LiveData<List<User>> mAllUser;

    // Note that in order to unit test the WordRepository, you have to remove the Application
    // dependency. This adds complexity and much more code, and this sample is not about testing.
    // See the BasicSample in the android-architecture-components repository at
    // https://github.com/googlesamples
    public UserRepository(Application application) {
        Database db = Database.getDatabase(application);
        mUserDao = db.userDao();
    }

    // Room executes all queries on a separate thread.
    // Observed LiveData will notify the observer when the data has changed.
    LiveData<List<User>> getAllWords() {
        return mAllUser;
    }

    // You must call this on a non-UI thread or your app will throw an exception. Room ensures
    // that you're not doing any long running operations on the main thread, blocking the UI.
    void insert(User user) {
        Database.databaseWriteExecutor.execute(() -> {
            mUserDao.insert(user);
        });
    }
    public void insertAll(List<User> lstUser) {
        Database.databaseWriteExecutor.execute(() -> {
            mUserDao.insertAll(lstUser);
        });
    }
}
