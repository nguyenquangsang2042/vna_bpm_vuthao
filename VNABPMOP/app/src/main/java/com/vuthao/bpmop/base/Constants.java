package com.vuthao.bpmop.base;

public class Constants {
    //public static final String BASE_URL = "https://vnabpm.vuthao.com"; // Dev
    public static final String BASE_URL = "https://bpm.vietnamairlines.com"; // Live
    public static boolean isInBackground = false;
    public static int asyncCallApi = 5;
    public static int dataLimitDay = 30;
    public static String mLangVN = "1066";
    public static String mLangEN = "1033";
    public static String mDateApi = "yyyy-MM-dd'T'HH:mm:ss";
    public static int mSwipeDistance = 100;
    public static String mResourceIdToMe = "221";
    public static String mResourceIdFromMe = "221";
    public static int mFilterLimit = 60;
    public static int delayTimes = 500;
    public static String deviceToken;
    public static String mResourceId; // danh de luu lai WorkflowItemId khi pushnotification kill app
    public static int mFileCode = 183;
    public static String APPSTATUS = "MOBILE_APPSTATUS_ENABLE";                              // ID AppStatus Mobile được xài
    public static String APPSTATUS_FROMME = "MOBILE_APPSTATUS_FROMME";                       // ID AppStatus Default VTBD
    public static String APPSTATUS_TOME_DAXULY = "MOBILE_APPSTATUS_TOME_DAXULY";             // ID AppStatus Default VDT - Đã xử lý
    public static String APPSTATUS_TOME_DANGXULY = "MOBILE_APPSTATUS_TOME_DANGXULY";         // ID AppStatus Default VDT - Đang xử lý
    public static String[] mimeTypesImage = {"image/jpeg", "image/jpg", "image/png"};
    public static String[] mimeTypes = {"application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/pdf", "text/csv", "text/plain",  // .doc & .docx
            "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .ppt & .pptx
            "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xls & .xlsx
            "application/pdf",
            "image/jpeg", "image/jpg", "image/png"};
    public static final String TAG = "xxxxxxxxxxxxxxx";
    public static int mDetailAttachment = 1803;
    public static int mDetailAttachmentCamera = mDetailAttachment + 1;
    public static int mDetailComment = 1904;
    public static int mDetailCommentCamera = mDetailComment + 1;
    public static int mTaskAttachment = 1210;
    public static int mTaskAttachmentCamera = mTaskAttachment + 1;
    public static int mTaskComment = 1311;
    public static int mTaskCommentCamera = mTaskComment + 1;
    public static int mTaskChildAttachment = 2802;
    public static int mTaskChildAttachmentCamera = mTaskChildAttachment + 1;
    public static int mReplyCommentAttachFile = 120;
    public static int mReplyCommentAttachFileCamera = mReplyCommentAttachFile + 1;
}
