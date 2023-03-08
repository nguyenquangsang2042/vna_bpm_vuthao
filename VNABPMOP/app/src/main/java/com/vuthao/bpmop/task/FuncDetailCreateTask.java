package com.vuthao.bpmop.task;

import android.annotation.SuppressLint;
import android.graphics.Color;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.CurrentUser;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Calendar;

public class FuncDetailCreateTask {
    public static class FlagActionPermission {
        public static final int NoAction = -1;
        //Tạo mới
        public static final int CreateNew = 0;
        //Người tạo update
        public static final int CreatorUpdate = 1;
        //Người xử lý update
        public static final int HandlerUpdate = 2;
        //Người taọ đồng thời là người xử lý update
        public static final int CreatorHandlerUpdate = 3;
    }

    public static class FlagUserPermission {
        public static final int Viewer = -1;
        public static final int Creator = 1;
        public static final int Handler = 2;
        public static final int CreatorAndHandler = 3;
    }

    public static class ActionStatusID {
        public static final int Cancel = 4;
        public static final int Completed = 2;
        public static final int Hold = 3;
        public static final int InProgress = 1;
        public static final int NoProcess = 0;
    }

    public static void setViewControl_NotEdited(LinearLayout _lnContent, TextView _tvContent) {
        if (_lnContent != null) {
            _lnContent.setEnabled(false);
            _lnContent.setBackgroundColor(Color.TRANSPARENT);
        }

        if (_tvContent != null) {
            _tvContent.setPadding(0, 0, 0, 0);
        }
    }

    public static void setViewControl_Edited(LinearLayout _lnContent, TextView _tvContent) {
        if (_lnContent != null) {
            _lnContent.setEnabled(true);
            _lnContent.setPadding(6, 6, 6, 6);
            _lnContent.setBackgroundResource(R.drawable.edtcornerstrokegray);
        }

        if (_tvContent != null) {
            _tvContent.setPadding(6, 6, 6, 6);
        }
    }

    public static String getStatusNameByID(int status) {
        switch (status) {
            case ActionStatusID.Cancel: {
                return Functions.share.getTitle("TEXT_CANCEL","Hủy");
            }
            case ActionStatusID.Completed: {
                return Functions.share.getTitle("TEXT_COMPLETED","Hoàn tất");
            }
            case ActionStatusID.Hold: {
                return Functions.share.getTitle("TEXT_HOLD","Tạm hoãn");
            }
            case ActionStatusID.InProgress: {
                return Functions.share.getTitle("TEXT_INPROGRESS","Đang thực hiện");
            }
            default: {
                return Functions.share.getTitle("TEXT_NOPROCESS","Chưa thực hiện");
            }
        }
    }

    @SuppressLint("SimpleDateFormat")
    public static String getToDayTask() {
        DateFormat dateFormat = null;
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm");
        } else {
            dateFormat = new SimpleDateFormat("MM/dd/yy HH:mm");
        }
        Calendar cal = Calendar.getInstance();
        return dateFormat.format(cal.getTime());
    }
}
