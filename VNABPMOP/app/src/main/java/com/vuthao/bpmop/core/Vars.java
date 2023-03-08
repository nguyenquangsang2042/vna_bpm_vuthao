package com.vuthao.bpmop.core;

public class Vars {
    public static class FlagViewControlAttachment {
        public static final int DetailWorkflow = 1;
        public static final int DetailAttachFile = 2;
        public static final int DetailCreateTask = 3;
        public static final int DetailCreateTaskChild = 4;
        public static final int CreateWorkflowDetail = 5;
    }

    public static class FlagViewControlAction {
        public static final int DetailWorkflow = 0;
    }

    public static class FlagViewControlDynamic {
        public static final int DetailWorkflow = 0;
        public static final int DetailWorkflow_InputGridDetail = 1;
        public static final int DetailCreateTask = 2;
    }

    public static class ControlInputGridDetails_InnerActionID {
        public static final int Create = 1;
        public static final int Edit = 2;
        public static final int Delete = 3;
        public static final int View = 4;
    }

    public static class EnumDynamicControlCategory {
        public static final int Detail = 0;
        public static final int TemplateValue = 1;
    }

    public static class FlagView {
        public static final int DetailWorkflow_ControlInputAttachmentVertical = 0;
        public static final int DetailWorkflow_Comment = 1;
        public static final int DetailCreateTask_ControlInputAttachmentVertical = 2;
        public static final int DetailCreateTask_Comment = 3;
        public static final int DetailCreateTask_Child_ControlInputAttachmentVertical = 4;
        public static final int ReplyComment = 5;
    }

    public static class ControlInputAttachmentVertical_InnerActionID {
        public static final int Create = 1;
        public static final int Edit = 2;
        public static final int Delete = 3;
        public static final int View = 4;
    }
}
