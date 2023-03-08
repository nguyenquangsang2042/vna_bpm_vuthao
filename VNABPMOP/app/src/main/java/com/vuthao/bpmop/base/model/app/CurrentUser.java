package com.vuthao.bpmop.base.model.app;

public class CurrentUser {
    private static CurrentUser instance;
    private User user;

    public static CurrentUser getInstance() {
        if (instance == null) instance = new CurrentUser();
        return instance;
    }

    public User getUser() {
        return user;
    }

    public void setUser(User user) {
        this.user = user;
    }
}
