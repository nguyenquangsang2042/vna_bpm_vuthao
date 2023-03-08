package com.vuthao.bpmop.base.realm;

import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;

import io.realm.Realm;

public class AppLanguageController extends RealmController {

    public void updateLanguageUser(int langCode) {
        realm.executeTransaction(realm -> CurrentUser.getInstance().getUser().setLanguage(langCode));
    }
}
