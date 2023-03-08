package com.vuthao.bpmop.base.custom;

import android.content.Context;
import android.util.AttributeSet;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.recyclerview.widget.RecyclerView;

public class CustomFlexBoxRecyclerView extends RecyclerView {
    int _rowHeight = -1, _maxRow = -1;

    public CustomFlexBoxRecyclerView(@NonNull Context context) {
        super(context);
    }

    public CustomFlexBoxRecyclerView(@NonNull Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public CustomFlexBoxRecyclerView(@NonNull Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public void setMaxRowAndRowHeight(int _rowHeight, int _maxRow) {
        this._rowHeight = _rowHeight;
        this._maxRow = _maxRow;
    }

    @Override
    protected void onMeasure(int widthSpec, int heightSpec) {
        if (_rowHeight != -1 && _maxRow != -1) {
            heightSpec = MeasureSpec.makeMeasureSpec(_rowHeight * _maxRow, MeasureSpec.AT_MOST);
        } else {
            heightSpec = MeasureSpec.makeMeasureSpec(95 * 3, MeasureSpec.AT_MOST);
        }
        super.onMeasure(widthSpec, heightSpec);
    }
}
