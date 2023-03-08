package com.vuthao.bpmop.base.vars;

public class Variable {

    public static class AppStatusID {
        public static final int Pending = 128;
        public static final int Canceled = 64;
        public static final int Recalled = 32;
        public static final int Rejected = 16;
        public static final int Completed = 8;
        public static final int InProgress = 4;
        public static final int NotStart = 2;
        public static final int Draft = 1;
    }

    public static class Home {
        public static final int InProcess = 0;
        public static final int Processed = 1;
    }

    public static class BottomNavigation {
        public static final int HomePage = 0;
        public static final int SingleListVDT = 1;
        public static final int SingleListVTBD = 2;
        public static final int Follow = 3;
        public static final int Search = 4;
        public static final int Board = 5;
        public static final int Filter = 6;
    }

    public static class WorkflowAction {
        public static final int Next = 1;
        public static final int Approve = 2;
        public static final int Forward = 3;
        public static final int Return = 4;
        public static final int Reject = 5;
        public static final int Recall = 6;
        public static final int RequestInformation = 7;
        public static final int RecallAfterApproved = 8;
        public static final int RequestIdea = 9;
        public static final int Idea = 10;
        public static final int Save = 11;
        public static final int Submit = 12;
        public static final int Login = 13;
        public static final int Share = 14;
        public static final int Additional = 15;
        public static final int View = 16;
        public static final int Download = 17;
        public static final int Add = 18;
        public static final int Update = 19;
        public static final int Delete = 20;
        public static final int Search = 21;
        public static final int Cancel = 51;
        public static final int CreateTask = 54;
    }
}
