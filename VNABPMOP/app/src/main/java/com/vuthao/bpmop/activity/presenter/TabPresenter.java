package com.vuthao.bpmop.activity.presenter;

public class TabPresenter {
    public interface TabSelectedListener {
        void OnTabSelected(int position, int lastPage);
        void OnTabReselected(int position);
    }
}
