using System;
using BPMOPMobile.Bean;

namespace BPMOPMobile.Class
{
    public static class CmmEvent
    {
        public static readonly string errMess = "Failed";
        public class UpdateEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
            public string LangSelected { get; set; }
            public string ErrMess { get; set; }

            public UpdateEventArgs()
            {
                LangSelected = "";
                ErrMess = "";
            }
            public UpdateEventArgs(bool isSuccess, string langSelected = "", string errMess = "")
            {
                IsSuccess = isSuccess;
                LangSelected = langSelected;
                ErrMess = errMess;
            }
        }

        public class LoginEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
            public string UserName { get; set; }
            public string ErrCode { get; set; }
            public string Pass { get; set; }
            public string VerifyOTP { get; set; }
            public BeanUser UserInfo { get; set; }

            public LoginEventArgs()
            {
                UserName = "";
                Pass = "";
            }
            public LoginEventArgs(bool isSuccess, string userName = "", string pass = "", string verifyOTP = "", BeanUser userInfo = null, string errCode = "")
            {
                IsSuccess = isSuccess;
                UserName = userName;
                Pass = pass;
                VerifyOTP = verifyOTP;
                UserInfo = userInfo;
                ErrCode = errCode;
            }
        }

        public class UpdateBackgroundEventArgs : EventArgs
        {
            public string BeanName { get; set; }
            public string ErrMess { get; set; }
            //public UpdateBackgroundEventArgs()
            //{
            //    BeanName = "";
            //    ErrMess = "";
            //}
            public UpdateBackgroundEventArgs(string beanName = "", string errMess = "")
            {
                BeanName = beanName;
                ErrMess = errMess;
            }
        }

        public class SyncDataRequest : EventArgs
        {
            public bool isDone { get; set; }
            public SyncDataRequest(bool _isDone)
            {
                isDone = _isDone;
            }
        }

        public static event EventHandler<UpdateEventArgs> UpdateLangComplete;
        public static event EventHandler<LoginEventArgs> ReloginRequest;
        public static event EventHandler<SyncDataRequest> SyncDataBackgroundResultRequest;
        public static event EventHandler<UpdateBackgroundEventArgs> SyncDataBackGroundRequest;

        public static void UpdateLangComplete_Performence(object sender, UpdateEventArgs e)
        {

            if (UpdateLangComplete != null)
            {
                UpdateLangComplete(sender, e);
            }
        }

        public static void ReloginRequest_Performence(object sender, LoginEventArgs e)
        {

            if (ReloginRequest != null)
            {
                ReloginRequest(sender, e);
            }
        }

        public static void SyncDataBackgroundRequest_Performence(object sender, UpdateBackgroundEventArgs e)
        {
            if (SyncDataBackGroundRequest != null)
            {
                SyncDataBackGroundRequest(sender, e);
            }
        }

        public static void SyncDataRequest_Performence(object sender, SyncDataRequest e)
        {
            if (SyncDataBackgroundResultRequest != null)
            {
                SyncDataBackgroundResultRequest(sender, e);
            }
        }

    }
}
