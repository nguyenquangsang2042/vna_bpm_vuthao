using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;

namespace BPMOPMobile.Droid.Core.Common
{
    public static class CmmDroidVariable
    {
        public static string M_DynamicBackgroundURL = CmmVariable.M_Domain + "/_layouts/15/eoffices/Login/imgBg.svg";
        public static string M_SysLangVN = "1066";
        public static string M_SysLangEN = "1033";
        public static float M_DefaultConfig_FontScale = 1f;

        public static int M_SwipeRefreshLayoutDistance = 100;
        public static int M_MyPermissionsRequestReadExternalStorage = 1;
        public static int M_OnActivityResultFileChooserCode = 183; // code để định danh khi import file từ local vào list import của control Detail Create Workflow

        public static long M_MulticlickPreventTime = 500; // milisecond thời gian cho click lại 
        public static long M_ActionDelayTime = 200; // milisecond thời gian để action delay
        public static string M_PackageProvider = Application.Context.ApplicationContext.PackageName + ".provider"; //"com.BPMOPMobile.provider"; // Xem trong manifest 

        public static string M_Camera_Prefix = "AndroidCamera"; // tiền tố ảnh chụp từ camera

        public static string[] M_MimeTypesImage = { "image/jpeg", "image/jpg", "image/png" };
        public static string[] M_MimeTypes = {"application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/pdf", "text/csv", "text/plain",  // .doc & .docx
                                              "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .ppt & .pptx
                                              "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xls & .xlsx
                                              "application/pdf",
                                              "image/jpeg", "image/jpg", "image/png"};

        public static int M_DetailWorkflow_ChooseFileControlAttachment = 1803; // code để định danh khi import file từ local vào list import của control Detail Create Workflow
        public static int M_DetailWorkflow_ChooseFileControlAttachment_Camera = M_DetailWorkflow_ChooseFileControlAttachment + 1; // code để định danh khi import file từ local vào list import của control Detail Create Workflow

        public static int M_DetailWorkflow_ChooseFileComment = 1904; // code để định danh khi import file từ local vào list import phần comment
        public static int M_DetailWorkflow_ChooseFileComment_Camera = M_DetailWorkflow_ChooseFileComment + 1; // code để định danh khi import file từ camera vào list import phần comment

        public static int M_DetailCreateTask_ChooseFileControlAttachment = 1210; // code để định danh khi import file từ local vào list import của control Detail Create Workflow
        public static int M_DetailCreateTask_ChooseFileControlAttachment_Camera = M_DetailCreateTask_ChooseFileControlAttachment + 1; // code để định danh khi import file từ local vào list import của control Detail Create Workflow

        public static int M_DetailCreateTask_ChooseFileComment = 1311; // code để định danh khi import file từ local vào list import phần comment
        public static int M_DetailCreateTask_ChooseFileComment_Camera = M_DetailCreateTask_ChooseFileComment + 1; // code để định danh khi import file từ local vào list import phần comment

        public static int M_DetailCreateTask_Child_ChooseFileControlAttachment = 2802; // code để định danh khi import file từ local vào list import của control Detail Create Workflow
        public static int M_DetailCreateTask_Child_ChooseFileControlAttachment_Camera = M_DetailCreateTask_Child_ChooseFileControlAttachment + 1; // code để định danh khi import file từ local vào list import của control Detail Create Workflow

        public static int M_CreateWorkflowDetail_ChooseFileControlAttachment = 2505; // code để định danh khi import file từ local vào list import của control Detail Create Workflow

        public static int M_ReplyComment_AttachFile = 120; // code để định danh khi import file từ local vào list import của trang Reply Comment
        public static int M_ReplyComment_AttachFile_Camera = M_ReplyComment_AttachFile + 1; // code để định danh khi import file từ local vào list import của trang Reply Comment
    }
}