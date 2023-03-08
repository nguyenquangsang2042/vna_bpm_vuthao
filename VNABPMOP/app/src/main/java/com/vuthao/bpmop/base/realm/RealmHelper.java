package com.vuthao.bpmop.base.realm;

import java.util.ArrayList;

import io.realm.Realm;
import io.realm.RealmModel;
import io.realm.RealmResults;

public class RealmHelper<T extends RealmModel> extends RealmController{

    public void addNewItem(T obj) {
        realm.executeTransaction(realm -> realm.copyToRealmOrUpdate(obj));
    }

    public void addNewItems(ArrayList<T> items) {
        realm.executeTransaction(realm -> realm.copyToRealmOrUpdate(items));
    }

    public T getItemById(Class<T> cls,String fieldName, String id) {
        if (realm.where(cls).equalTo(fieldName, id).findFirst() != null) {
            return realm.where(cls).equalTo(fieldName, id).findFirst();
        }
        return null;
    }

    public T getItemByIntId(Class<T> cls,String fieldName, int id) {
        if (realm.where(cls).equalTo(fieldName, id).findFirst() != null) {
            return realm.where(cls).equalTo(fieldName, id).findFirst();
        }
        return null;
    }

    public ArrayList<T> getItems(Class<T> cls) {
        ArrayList<T> arrayList = new ArrayList<>();
        if (realm.where(cls).findAll() != null) {
            RealmResults<T> results = realm.where(cls).findAll();
            arrayList.addAll(results);
            return arrayList;
        }
        return arrayList;
    }
    public void clearAll(Class<T> cls) {
        realm.executeTransaction(realm -> {
            RealmResults<T> results = realm.where(cls).findAll();
            results.deleteAllFromRealm();
        });
    }
}
