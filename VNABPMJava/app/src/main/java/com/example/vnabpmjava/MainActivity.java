package com.example.vnabpmjava;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import com.example.vnabpmjava.database.model.User;
import com.example.vnabpmjava.database.repository.UserRepository;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        UserRepository repository = new UserRepository(getApplication());
        List<User> users = new ArrayList<>();
        for (int i = 0; i < 10000; i++) {
            User user = new User();
            user.setID(String.valueOf(i + 1));
            user.setUserId("note" +i);
            users.add(user);
        }
        repository.insertAll(users);
    }
}