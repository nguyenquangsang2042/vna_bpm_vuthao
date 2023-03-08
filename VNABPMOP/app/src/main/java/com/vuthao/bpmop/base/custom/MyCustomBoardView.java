package com.vuthao.bpmop.base.custom;

import android.content.Context;
import android.util.AttributeSet;

import com.vuthao.bpmop.base.custom.boardview.BoardView;

public class MyCustomBoardView extends BoardView {
    public MyCustomBoardView(Context context) {
        super(context);
    }

    public MyCustomBoardView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public MyCustomBoardView(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    @Override
    public void notifyDataSetChanged() {
        super.notifyDataSetChanged();
    }
}
