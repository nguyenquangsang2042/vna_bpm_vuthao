package com.vuthao.bpmop.base.realm;

import io.realm.Realm;
import io.realm.RealmConfiguration;

public class RealmController {
    protected final Realm realm;
    protected final RealmConfiguration realmConfiguration =
            new RealmConfiguration.Builder()
            .schemaVersion(0)
            .migration(new Migration())
            .build();

    final String LOG_TAG = "RealmController";

    public RealmController() {
        Realm.removeDefaultConfiguration();
        Realm.setDefaultConfiguration(realmConfiguration);
        this.realm = Realm.getInstance(realmConfiguration);
    }

    public Realm getRealm() { return  realm; }
}
