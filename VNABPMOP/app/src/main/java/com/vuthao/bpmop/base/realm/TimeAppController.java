package com.vuthao.bpmop.base.realm;

import com.vuthao.bpmop.base.model.app.TimeLanguage;

import java.util.ArrayList;

import io.realm.RealmResults;
import io.realm.Sort;

public class TimeAppController extends RealmController {

    public ArrayList<TimeLanguage> getTimeLanguage(int type) {
        RealmResults<TimeLanguage> results = realm.where(TimeLanguage.class)
                .equalTo("Type", type)
                .notEqualTo("Devices", 2)
                .sort("Index", Sort.ASCENDING)
                .findAll();

        return new ArrayList<>(results);
    }
}
