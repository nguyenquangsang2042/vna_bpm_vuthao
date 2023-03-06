using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using UIKit;
using System.Reflection;
using BPMOPMobile.iPad.Components;
using CoreAnimation;
using System.Drawing;
using Xamarin.iOS;
using BPMOPMobile.iPad.CustomControlClass;
using System.ComponentModel;

namespace BPMOPMobile.iPad.IOSClass
{
    public delegate void updateLoadingMessage(string title1, string title2);

    public class CmmIOSFunction
    {
        public enum FilterStatus
        {
            FROM_NOTIFY = 0,
            VBD_CHOYKIEN = 1,
            VBD_DAGIAIQUYET = 2,
            VBD_DANGGIAIQUYET = 3,
            VBD_TREHAN = 4,

            VBBH_ALL = 5,
            VBBH_CHOPHEDUYET = 6,
            VBBH_DAPHEDUYET = 7,
            VBBH_KHONGPHEDUYET = 8
        }

        public enum DOC_FilterStatus
        {
            NONE = -1,
            FROM_NOTIFY = 0,
            VBD_TRONGNGAY = 1,
            VBD_CUHON = 2,
            VBD_DAGIAIQUYET = 3,
            VBD_TATCA = 4,

            VBBH_TRONGNGAY = 5,
            VBBH_CUHON = 6,
            VBBH_DAGIAIQUYET = 7,
            VBBH_TATCA = 8
        }

        public enum FlagActionPermission
        {
            [Description("Không thao tác")]
            NoAction = -1,
            [Description("Tạo mới")]
            CreateNew = 0,
            [Description("Người tạo update")]
            CreatorUpdate = 1,
            [Description("Người xử lý update")]
            HandlerUpdate = 2,
        }

        public struct DynamicColorAvatar
        {
            public const string A = "#E18D96";
            public const string B = "#F3D1DC";
            public const string C = "#FCF0CF";
            public const string D = "#FDCF76";
            public const string E = "#B16E4B";
            public const string F = "#89AEB2";
            public const string G = "#F1E0B0";
            public const string H = "#F1CDB0";
            public const string I = "#E7CFC8";
            public const string J = "#D2A3A9";
            public const string K = "#E6DCE5";
            public const string L = "#EBC3C1";
            public const string M = "#ECAD8F";
            public const string N = "#AF6E4E";
            public const string O = "#C8B4BA";
            public const string P = "#F3DD83";
            public const string Q = "#C1CD97";
            public const string R = "#38908F";
            public const string S = "#5E96AE";
            public const string T = "#E08963";
            public const string U = "#BC85A3";
            public const string V = "#9799BA";
            public const string W = "#B5DDD1";
            public const string X = "#D7E7A9";
            public const string Y = "#84B4C8";
            public const string Z = "#619169";
        }

        public static event updateLoadingMessage EventUpdateLoading;
        public static void performance_UpdateLoading(string title1, string title2)
        {
            EventUpdateLoading(title1, title2);
        }

        public CmmIOSFunction()
        { }
        public static string collectDeviceInfo()
        {
            try
            {
                AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                UIDevice device = new UIDevice();
                DeviceInfo info = new DeviceInfo();
                info.DeviceId = device.IdentifierForVendor.AsString();
                info.DeviceOS = 2; //1: Android   2: IOS  4: WindowPhone
                info.DevicePushToken = appD.pushDeviceToken;
                info.DeviceName = device.Name;
                info.DeviceModel = DeviceHardware.Model;
                info.DeviceOSVersion = device.SystemVersion;
                info.AppVersion = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();

                return JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - CmmIOSFunction - collectDeviceInfo - Err" + ex.ToString());
                return null;
            }
        }
        public static int iosVersion()
        {
            try
            {
                UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
                int version;
                version = Convert.ToInt32(UIDevice.CurrentDevice.SystemVersion.Substring(0, 2));
                return version;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - CmmIOSFunction - iosVersion - Err" + ex.ToString());
                return 0;
            }
        }
        public static void SetLangToView(UIView mainView)
        {
            if (mainView.Subviews.Length == 0 || !string.IsNullOrEmpty(mainView.AccessibilityIdentifier))
            {
                if (!string.IsNullOrEmpty(mainView.AccessibilityIdentifier))
                {
                    string langValue = "";
                    if (mainView.GetType() == typeof(UIButton))
                    {
                        UIButton control = ((UIButton)mainView);
                        langValue = CmmFunction.GetTitle(control.AccessibilityIdentifier.ToString(), control.TitleLabel.Text);
                        control.SetTitle(langValue, UIControlState.Normal);
                    }
                    else if (mainView.GetType() == typeof(UILabel))
                    {
                        UILabel control = ((UILabel)mainView);
                        langValue = CmmFunction.GetTitle(mainView.AccessibilityIdentifier.ToString(), control.Text);
                        control.Text = langValue;
                    }
                    else if (mainView.GetType() == typeof(UITextField))
                    {
                        UITextField control = ((UITextField)mainView);
                        langValue = CmmFunction.GetTitle(mainView.AccessibilityIdentifier.ToString(), control.Placeholder);
                        control.Placeholder = langValue;
                    }
                    return;
                }
            }
            foreach (var v in mainView.Subviews)
            {
                SetLangToView(v);
            }
        }
        public static void CheckFolderExists()
        {
            //Folder data nếu chưa có thì tạo mới
            string dataFolderpath = Path.GetDirectoryName(CmmVariable.M_DataFolder);
            if (!Directory.Exists(CmmVariable.M_DataFolder))
                Directory.CreateDirectory(CmmVariable.M_DataFolder);

            //Folder avatar nếu chưa có thì tạo mới
            string avatar_Folderpath = Path.GetDirectoryName(CmmVariable.M_Folder_Avatar);
            if (!Directory.Exists(CmmVariable.M_Folder_Avatar))
                Directory.CreateDirectory(CmmVariable.M_Folder_Avatar);
        }
        public static UIColor FromHex(UIColor color, int hexValue)
        {
            return UIColor.FromRGB(
                (((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
                (((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
                (((float)(hexValue & 0xFF)) / 255.0f)
            );
        }

        public static UIColor GetColorByAppStatus(int AppStatusID)
        {
            try
            {

                #region old color
                /*switch (AppStatusID)
                {
                    // TASK
                    case 2: // Chưa thực hiện - Task
                        return UIColor.FromRGB(240, 240, 240);
                    case 4: // Đang thực hiện - Task
                        return UIColor.FromRGB(209, 233, 255);
                    case 16: // Từ chối
                    case 64: // Hủy - Task
                    case 128: // Tạm hoãn - Task
                        return UIColor.FromRGB(255, 203, 203);
                    // NOTIFY
                    case 1: // Soạn thảo
                        return UIColor.FromRGB(243, 243, 243);
                    case 8: // Hoàn tất - Task
                        return UIColor.FromRGB(220, 255, 218);
                    case 32: // Thu hồi
                        return UIColor.FromRGB(255, 230, 230);
                    default:
                        return UIColor.FromRGB(197, 221, 249);
                }*/
                #endregion
                #region new color since 24.05.22
                switch (AppStatusID)
                {
                    // TASK
                    case 2: // Chưa thực hiện - Task
                        return UIColor.FromRGB(244, 244, 244);
                    case 4: // Đang thực hiện - Task
                        return UIColor.FromRGB(255, 249, 200);
                    case 16: // Từ chối
                    case 64: // Hủy - Task
                    case 128: // Tạm hoãn - Task
                        return UIColor.FromRGB(255, 225, 225);
                    // NOTIFY
                    case 1: // Soạn thảo
                        return UIColor.FromRGB(244, 244, 244);
                    case 8: // Hoàn tất - Task
                        return UIColor.FromRGB(217, 255, 218);
                    case 32: // Thu hồi
                        return UIColor.FromRGB(255, 230, 230); /// sau khi thu hồi phiếu thì trạng thái của phiếu là đang lưu
                    default:
                        return UIColor.FromRGB(244, 244, 244);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmIOSFunction - GetColorByAppStatus - Err:" + ex.ToString());
            }
            return UIColor.FromRGB(197, 221, 249);
        }

        public static UIAlertController commonAlertMessage(UIViewController controller, string _title, string _mess)
        {
            /*
            UIAlertController alert = UIAlertController.Create(_title, _mess, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CLOSE", "Dismiss"), UIAlertActionStyle.Default, null));
            controller.PresentViewController(alert, true, null);
            return alert;
            */
            string strAction = "";
            if (CmmVariable.SysConfig.LangCode == "1033")
                strAction = "Close";
            else
                strAction = "Đóng";

            UIAlertController alert = UIAlertController.Create("Thông báo", _mess, UIAlertControllerStyle.Alert);//_title
            alert.AddAction(UIAlertAction.Create(strAction, UIAlertActionStyle.Default, null));
            controller.PresentViewController(alert, true, null);
            return alert;
        }

        public static NSDate DateTimeToNSDate(DateTime date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
            return NSDate.FromTimeIntervalSinceReferenceDate((date - reference).TotalSeconds);
        }

        public static string decodeHtml(string param)
        {
            return WebUtility.HtmlDecode(param);
        }

        public static NSAttributedString GetAttributedStringFromHtml(string html)
        {
            NSError error = null;
            NSAttributedString attributedString = new NSAttributedString(NSData.FromString(html),
                new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML, StringEncoding = NSStringEncoding.UTF8 },
                ref error);
            return attributedString;
        }

        public static void processReceivedRemoteNotification(UIStoryboard Storyboard, UINavigationController nav, string id, string listName, string documentID, string taskID, string templateID, string alert, string _appstateFrom)
        {
            try
            {
                switch (templateID)
                {
                    case "201": // đăng nhập từ thiết bị khác - tự signout
                        //kiem tra lai truong hop nay server push ve ko dung cau truc, cu the la ko co title
                        if (!string.IsNullOrEmpty(alert))
                        {
                            alert = alert.Replace("\n", "|");
                            string[] content = alert.Split('|');

                            //ToastMessageView mess = (ToastMessageView)Storyboard.InstantiateViewController("ToastMessageView");
                            //alert = alert.Replace("\n", "|");
                            //string[] mss = alert.Split('|');
                            //mess.SetContent(content[0], content[1], templateID, null);
                            //mess.ModalPresentationStyle = UIModalPresentationStyle.Custom;

                            //RootNavigationController root = Storyboard.InstantiateViewController("RootNavigationController") as RootNavigationController;
                            //UIApplication.SharedApplication.Windows[0].RootViewController = root;
                            //UIApplication.SharedApplication.Windows[0].RootViewController.Add(mess.View);
                        }

                        //ProviderUser p_user = new ProviderUser();
                        //p_user.SignOut();

                        var dataFile = CmmVariable.M_DataPath;
                        if (File.Exists(dataFile))
                            File.Delete(dataFile);

                        var configFile = CmmVariable.M_settingFileName;
                        if (File.Exists(configFile))
                            File.Delete(configFile);

                        CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db";
                        CmmVariable.M_settingFileName = "config.ini";
                        CmmVariable.M_AuthenticatedHttpClient = null;
                        CmmVariable.SysConfig = null;
                        CmmFunction.WriteSetting();

                        TimerResync.Instance.timerReSync.Stop();
                        UIApplication.SharedApplication.CancelAllLocalNotifications();
                        UIApplication.SharedApplication.UnregisterForRemoteNotifications();
                        break;
                    default:
                        if (!string.IsNullOrEmpty(id))
                        {
                            string[] content = null;
                            if (!string.IsNullOrEmpty(alert))
                            {
                                alert = alert.Replace("\n", "|");
                                content = alert.Split('|');
                            }

                            List<BeanNotify> lst_notify = new List<BeanNotify>();
                            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                            string queryVbden = string.Format("SELECT * FROM BeanNotify WHERE ID = ?");
                            lst_notify = conn.Query<BeanNotify>(queryVbden, id);
                            if (lst_notify.Count > 0)
                            {
                                if (_appstateFrom == "Active")
                                {
                                    //ToastMessageView mess = (ToastMessageView)Storyboard.InstantiateViewController("ToastMessageView");
                                    //mess.SetContent(content[0], content[1], templateID, lst_notify[0]);
                                    //mess.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                                    //AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                                    //appD.Window.Add(mess.View);
                                }
                                else if (_appstateFrom == "Inactive")
                                {
                                    //AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                                    //ViewRequestDetails detailView = (ViewRequestDetails)Storyboard.InstantiateViewController("ViewRequestDetails");
                                    //detailView.setContentFromPush(lst_notify[0], true);
                                    //appD.NavController.PushViewController(detailView, true);
                                }
                                else
                                {
                                    //AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                                    //ViewRequestDetails detailView = (ViewRequestDetails)Storyboard.InstantiateViewController("ViewRequestDetails");
                                    //detailView.setContentFromPush(lst_notify[0], true);
                                    //appD.NavController.PushViewController(detailView, true);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmIOSFunction - processReceivedRemoteNotification: " + ex.ToString());
            }
        }

        public async static void processReceivedRemoteNotificationWhenterminal(UIStoryboard Storyboard, UINavigationController nav, string id, string listName, string documentID, string taskID, string templateID, string alert)
        {
            try
            {

                switch (templateID)
                {
                    case "201": // đăng nhập từ thiết bị khác - tự signout
                        //kiem tra lai truong hop nay server push ve ko dung cau truc, cu the la ko co title
                        if (!string.IsNullOrEmpty(alert))
                        {
                            alert = alert.Replace("\n", "|");
                            string[] content = alert.Split('|');

                            //ToastMessageView mess = (ToastMessageView)Storyboard.InstantiateViewController("ToastMessageView");
                            //alert = alert.Replace("\n", "|");
                            //string[] mss = alert.Split('|');
                            //mess.SetContent(content[0], content[1], templateID, null);
                            //mess.ModalPresentationStyle = UIModalPresentationStyle.Custom;

                            //RootNavigationController root = Storyboard.InstantiateViewController("RootNavigationController") as RootNavigationController;
                            //UIApplication.SharedApplication.Windows[0].RootViewController = root;
                            //UIApplication.SharedApplication.Windows[0].RootViewController.Add(mess.View);
                        }

                        //ProviderUser p_user = new ProviderUser();
                        //p_user.SignOut();

                        var dataFile = CmmVariable.M_DataPath;
                        if (File.Exists(dataFile))
                            File.Delete(dataFile);

                        var configFile = CmmVariable.M_settingFileName;
                        if (File.Exists(configFile))
                            File.Delete(configFile);

                        CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db";
                        CmmVariable.M_settingFileName = "config.ini";
                        CmmVariable.M_AuthenticatedHttpClient = null;
                        CmmVariable.SysConfig = null;
                        CmmFunction.WriteSetting();

                        TimerResync.Instance.timerReSync.Stop();
                        UIApplication.SharedApplication.CancelAllLocalNotifications();
                        UIApplication.SharedApplication.UnregisterForRemoteNotifications();
                        break;
                    default:
                        if (!string.IsNullOrEmpty(id))
                        {
                            string[] content = null;
                            if (!string.IsNullOrEmpty(alert))
                            {
                                alert = alert.Replace("\n", "|");
                                content = alert.Split('|');
                            }

                            List<BeanNotify> lst_notify = new List<BeanNotify>();
                            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                            string queryVbden = string.Format("SELECT * FROM BeanNotify WHERE ID = ?");
                            lst_notify = conn.Query<BeanNotify>(queryVbden, id);
                            if (lst_notify.Count > 0)
                            {
                                //AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                                //ViewRequestDetails detailView = (ViewRequestDetails)Storyboard.InstantiateViewController("ViewRequestDetails");
                                //detailView.setContentFromPush(lst_notify[0], true);
                                //appD.NavController.PushViewController(detailView, true);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmIOSFunction - processReceivedRemoteNotification: " + ex.ToString());
            }
        }

        /// <summary>
        /// Set thuộc tính cho component với các value định dạng riêng của iOS
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strPropName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValueByNameCustom(object obj, string strPropName, object value)
        {
            PropertyInfo propInfo = CmmFunction.GetProperty(obj, strPropName);
            if (propInfo != null)
            {
                Type typeObject = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                Type typePropretyBase = typeof(PropretyBase);
                MethodInfo theMethod = typePropretyBase.GetMethod("Get" + typeObject.Name);

                if (theMethod != null)
                {
                    PropretyBase proprety = new PropretyBase();
                    var result = theMethod.Invoke(proprety, new[] { value });
                    propInfo.SetValue(obj, result, null);
                }
                else
                    CmmFunction.SetPropertyValue(obj, propInfo, value);
            }
        }

        public static void ResignFirstResponderOnTap(UIView view)
        {
            var gesture = new UITapGestureRecognizer(() =>
            {
                view.EndEditing(true);
            });

            gesture.CancelsTouchesInView = false; //for iOS5
            view.AddGestureRecognizer(gesture);
        }

        public static async Task<string> CheckFileLocalIsExist(string filepathURL)
        {
            try
            {
                string localDocumentFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), CmmVariable.M_DataFolder);
                string filename = filepathURL.Split('/').Last();
                string localfilePath = Path.Combine(localDocumentFilepath, filename);
                if (!File.Exists(localfilePath))
                {
                    var result = await Task.Run(() =>
                    {
                        ProviderBase provider = new ProviderBase();
                        if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            return filename;
                        else
                            return null;
                    });

                    return result;
                }
                else
                    return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmIOSFunction - checkFileLocalIsExist - Err: " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get image từ đường dẫn file gán cho Imageview
        /// </summary>
        /// <param name="localfilename">đường dẫn file</param>
        /// <param name="defaultFile">image mặc định sẽ hiện khi xuất hiện lỗi get file</param>
        /// <param name="image_view">image view chứa image</param>
        public static async void openFile(string localfilename, string defaultFile, UIImageView image_view)
        {
            try
            {
                NSData data = null;
                await Task.Run(() =>
                {
                    string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                    data = NSData.FromUrl(new NSUrl(localfilePath, false));
                });

                if (data != null)
                {
                    UIImage image = UIImage.LoadFromData(data);
                    if (image != null)
                        image_view.Image = image;
                    else
                        image_view.Image = UIImage.FromFile(defaultFile);
                }
                else
                    image_view.Image = UIImage.FromFile(defaultFile);
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile(defaultFile);
                Console.WriteLine("CmmIOSFunction - openFile - Err: " + ex.ToString());
            }
        }

        public static void UpdateScrollTableView(UIView view, UITableView tableView)
        {
            //UITapGestureRecognizer gesture = new UITapGestureRecognizer();
            //gesture.ShouldReceiveTouch += (UIGestureRecognizer r, UITouch t) =>
            //{

            //    bool isScroll = true;
            //    if (view.GetType() == typeof(ControlAttachmentVerticalWithFormFrame))
            //    {
            //        isScroll = false;
            //    }

            //    if (tableView.ScrollEnabled == isScroll)
            //    {
            //        return false;
            //    }

            //    tableView.ScrollEnabled = isScroll;
            //    return true;
            //};
            //gesture.CancelsTouchesInView = false;
            //view.AddGestureRecognizer(gesture);
        }

        public static void MakeCornerTopLeftRight(UIView _view, int _radius)
        {
            UIBezierPath maskPath = UIBezierPath.FromRoundedRect(_view.Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new SizeF(_radius, _radius));
            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.Frame = _view.Bounds;
            maskLayer.Path = maskPath.CGPath;

            _view.Layer.Mask = maskLayer;
        }

        public static void CreateCircleButton(UIButton _mButton)
        {
            try
            {
                double min = Math.Min(_mButton.Frame.Width, _mButton.Frame.Height);
                _mButton.Layer.CornerRadius = (float)(min / 2.0);
                _mButton.Layer.MasksToBounds = false;
                _mButton.Layer.BorderColor = UIColor.Clear.CGColor;
                _mButton.Layer.BorderWidth = 0;
                _mButton.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkflowDetailView - CreateCircle - Err: " + ex.ToString());
            }
        }

        public static string GetDynamicColorAvatar(string _nameChar)
        {
            switch (_nameChar.ToUpper())
            {
                case "A":
                    return DynamicColorAvatar.A;
                case "B":
                    return DynamicColorAvatar.B;
                case "C":
                    return DynamicColorAvatar.C;
                case "D":
                    return DynamicColorAvatar.D;
                case "E":
                    return DynamicColorAvatar.E;
                case "F":
                    return DynamicColorAvatar.F;
                case "G":
                    return DynamicColorAvatar.G;
                case "H":
                    return DynamicColorAvatar.H;
                case "I":
                    return DynamicColorAvatar.I;
                case "J":
                    return DynamicColorAvatar.J;
                case "K":
                    return DynamicColorAvatar.K;
                case "L":
                    return DynamicColorAvatar.L;
                case "M":
                    return DynamicColorAvatar.M;
                case "N":
                    return DynamicColorAvatar.N;
                case "O":
                    return DynamicColorAvatar.O;
                case "P":
                    return DynamicColorAvatar.P;
                case "Q":
                    return DynamicColorAvatar.Q;
                case "R":
                    return DynamicColorAvatar.R;
                case "S":
                    return DynamicColorAvatar.S;
                case "T":
                    return DynamicColorAvatar.T;
                case "U":
                    return DynamicColorAvatar.U;
                case "V":
                    return DynamicColorAvatar.V;
                case "W":
                    return DynamicColorAvatar.W;
                case "X":
                    return DynamicColorAvatar.X;
                case "Y":
                    return DynamicColorAvatar.Y;
                default:
                    return DynamicColorAvatar.Z;
            }
        }

        public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        public static bool CheckStringTrunCated(UILabel lbl_text)
        {
            UIFont descriptionLabelFont = UIFont.SystemFontOfSize(lbl_text.Font.PointSize);
            NSString textToMeasure = (NSString)lbl_text.Text;

            CGRect labelRect = textToMeasure.GetBoundingRect(
                new CGSize(nfloat.MaxValue, lbl_text.Frame.Height),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = descriptionLabelFont },
                new NSStringDrawingContext()
            );
            if (labelRect.Width > lbl_text.Bounds.Size.Width)
            {
                return true;
            }
            return false;
        }

        public static bool CheckStringTrunCated(string content, CGRect controlSize, nfloat fontSize)
        {
            UIFont descriptionLabelFont = UIFont.SystemFontOfSize(fontSize);
            NSString textToMeasure = (NSString)content;

            CGRect labelRect = textToMeasure.GetBoundingRect(
                new CGSize(nfloat.MaxValue, controlSize.Height),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = descriptionLabelFont },
                new NSStringDrawingContext()
            );
            if (labelRect.Width > controlSize.Width)
            {
                return true;
            }
            return false;
        }

        public static UIImage RotateCameraImageToProperOrientation(UIImage imageSource, nfloat maxResolution)
        {

            var imgRef = imageSource.CGImage;

            var width = (nfloat)imgRef.Width;
            var height = (nfloat)imgRef.Height;

            var bounds = new CGRect(0, 0, width, height);

            nfloat scaleRatio = 1;

            if (width > maxResolution || height > maxResolution)
            {
                scaleRatio = (nfloat)Math.Min(maxResolution / bounds.Width, maxResolution / bounds.Height);
                bounds.Height = bounds.Height * scaleRatio;
                bounds.Width = bounds.Width * scaleRatio;
            }

            var transform = CGAffineTransform.MakeIdentity();
            var orient = imageSource.Orientation;
            var imageSize = new CGSize(imgRef.Width, imgRef.Height);
            nfloat storedHeight;

            switch (imageSource.Orientation)
            {
                case UIImageOrientation.Up:
                    transform = CGAffineTransform.MakeIdentity();
                    break;

                case UIImageOrientation.UpMirrored:
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0.0f);
                    transform = CGAffineTransform.Scale(transform, -1.0f, 1.0f);
                    break;

                case UIImageOrientation.Down:
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)Math.PI);
                    break;

                case UIImageOrientation.DownMirrored:
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Height);
                    transform = CGAffineTransform.Scale(transform, 1.0f, -1.0f);
                    break;

                case UIImageOrientation.Left:
                    storedHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = storedHeight;
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (nfloat)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.LeftMirrored:
                    storedHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = storedHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
                    transform = CGAffineTransform.Scale(transform, -1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (nfloat)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Right:
                    storedHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = storedHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.RightMirrored:
                    storedHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = storedHeight;
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, (nfloat)Math.PI / 2.0f);
                    break;

                default:
                    break;
            }

            UIGraphics.BeginImageContext(bounds.Size);
            var context = UIGraphics.GetCurrentContext();

            if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
            {
                context.ScaleCTM(-scaleRatio, scaleRatio);
                context.TranslateCTM(-height, 0);
            }
            else
            {
                context.ScaleCTM(scaleRatio, -scaleRatio);
                context.TranslateCTM(0, -height);
            }

            context.ConcatCTM(transform);
            context.DrawImage(new CGRect(0, 0, width, height), imgRef);

            var imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }

        public static UIImage ScaleImageFollowHeight(UIImage imageSource, nfloat heightResolution)
        {
            nfloat oldHeight = imageSource.Size.Height;
            nfloat scaleFactor = heightResolution / oldHeight;

            nfloat newHeight = heightResolution;// heightResolution * scaleFactor; 
            nfloat newWidth = imageSource.Size.Width * scaleFactor;

            UIGraphics.BeginImageContext(new CGSize(newWidth, newHeight));
            imageSource.Draw(new CGRect(0, 0, newWidth, newHeight));
            UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return newImage;
        }

        public static void AddBorderView(UIView view)
        {
            view.Layer.BorderWidth = 1;
            view.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view.Layer.CornerRadius = 3;
            view.ClipsToBounds = true;
        }

        public static void AddAttributeTitle(UILabel lable)
        {
            if (lable.Text.Contains('('))
            {
                var indexTitle = lable.Text.IndexOf('(');
                NSMutableAttributedString attTilte = new NSMutableAttributedString(lable.Text);
                attTilte.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(14), new NSRange(0, lable.Text.Length));
                attTilte.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(153, 153, 153), new NSRange(0, lable.Text.Length));
                attTilte.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Red, new NSRange(indexTitle, lable.Text.Length - indexTitle));
                lable.AttributedText = attTilte;
            }
        }

        public static void AddShadowForTopORBotBar(UIView _view, bool _isTopView)
        {
            if (_isTopView)
            {
                _view.Layer.ShadowColor = UIColor.DarkGray.CGColor;
                _view.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(0, 0.1, _view.Frame.Width, _view.Frame.Height)).CGPath;
                _view.Layer.ShadowOffset = new CGSize(0, 0.5);
                _view.Layer.ShadowOpacity = 1;
                _view.ClipsToBounds = false;
            }
            else
            {
                _view.Layer.ShadowColor = UIColor.DarkGray.CGColor;
                _view.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(0, -0.1, _view.Frame.Width, _view.Frame.Height)).CGPath;
                _view.Layer.ShadowOffset = new CGSize(0, 0.5);
                _view.Layer.ShadowOpacity = 1;
                _view.ClipsToBounds = false;
            }
        }

        public static string GetHtmlStyle()
        {
            string htmlStyle = "";
            try
            {
                htmlStyle = string.Format("<style>body{{font-family:'{0}'; font-size:{1}px;}}</style>",
                            "ArialMT",
                            14);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("getHtmlStyle - err: " + ex.ToString());
#endif
            }
            return htmlStyle;
        }

        public static string GetStringDateFilter(DateTime fromDate, DateTime toDate)
        {
            string condition = "AB.Created IS NOT NULL ";
            if (fromDate == default(DateTime) && toDate == default(DateTime))
            {
                return "";
            }
            else
            {
                //from Date
                string str_fromDate = "";
                if (fromDate.Year == 1)
                    str_fromDate = "";
                else
                    str_fromDate = fromDate.ToString("yyyy-MM-dd");

                //to Date
                string str_toDate = "";
                if (toDate.Year == 1)
                    str_toDate = "";
                else
                {
                    toDate = toDate.AddDays(1);
                    str_toDate = toDate.ToString("yyyy-MM-dd");
                }

                //tungay != null, denngay = null
                if (!string.IsNullOrEmpty(str_fromDate) && string.IsNullOrEmpty(str_toDate))
                {
                    condition = string.Format(condition + "AND AB.Created >= '{0}'", str_fromDate);
                }
                //tungay != null, denngay = null
                else if (string.IsNullOrEmpty(str_fromDate) && !string.IsNullOrEmpty(str_toDate))
                {
                    condition = string.Format(condition + "AND AB.Created <= '{0}'", str_toDate);
                }
                //
                else
                {
                    condition = string.Format(condition + "AND (AB.Created >= '{0}' AND AB.Created < '{1}')", str_fromDate, str_toDate);
                }

                return condition;
            }
        }

        /// <summary>
        /// Kiểm tra login vào site DEV hay LIVE (active): login có đuôi @vuthao thì log vào iste DEV
        /// </summary>
        /// <param name="userName"></param>
        public static void CheckDomain(string userName)
        {
            CmmVariable.M_Domain = !string.IsNullOrEmpty(userName.TrimEnd()) && userName.TrimEnd().Contains("@vuthao") ? CmmVariable.M_Domain_develop : CmmVariable.M_Domain_active;
        }

        public static void AlertUnsupportFile(UIViewController controller)
        {
            CmmIOSFunction.commonAlertMessage(controller, "VNA BPM", "Hệ thống không hỗ trợ định dạng tập tin này.");
        }

        /// <summary>
        /// Lưu danh sách phiếu vừa lấy xuống DB
        /// </summary>
        /// <param name="lstItemAppBase"></param>
        /// <param name="conn"></param>
        public static void UpdateNewListDataOnline(List<BeanAppBaseExt> lstItemAppBase, SQLiteConnection conn)
        {
            try
            {
                SQLite3.BusyTimeout(conn.Handle, 60000);
                //conn.BeginTransaction();
                foreach (BeanAppBaseExt item in lstItemAppBase)
                {
                    try
                    {
                        //new ProviderBase().UpdateItemDataNewLoading(item, conn);
                        conn.InsertOrReplace(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CmmIOSFunciton - UpdateNewListDataOnline - UpdateDataNewLoading err " + ex.ToString());
                    }
                }
                //conn.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmIOSFunciton - UpdateNewListDataOnline - Err " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public static string GetQueryStringAppBaseVDT(bool isInprocess)
        {
            return isInprocess ? string.Format(@"SELECT * FROM BeanAppBaseExt WHERE AssignedTo LIKE '%{0}%' AND StatusGroup IN ({1}) Order By Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId, CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY))
                : string.Format(@"SELECT * FROM BeanAppBaseExt WHERE NotifiedUsers LIKE '%{0}%' Order By Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId);
        }

        public static string GetQueryStringAppBaseVTBD(bool isInprocess)
        {
            return string.Format(@"SELECT * FROM BeanAppBaseExt WHERE CreatedBy LIKE '%{0}%' AND StatusGroup IN ({1}) Order By Created DESC LIMIT ? OFFSET ?",
                CmmVariable.SysConfig.UserId,
                CmmFunction.GetAppSettingValue(isInprocess ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY)
                );
        }
    }

    #region custom class get height for text view
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the height of a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="width">The width.</param>
        /// <returns>nfloat.</returns>
        public static nfloat StringHeight(this string text, UIFont font, nfloat width)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            return text.StringRect(font, width).Height;
        }

        /// <summary>
        /// Gets the rectangle of a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="width">The width.</param>
        /// <returns>CGRect.</returns>
        public static CGRect StringRect(this string text, UIFont font, nfloat width)//UsesLineFragmentOrigin
        {
            var nativeString = new NSString(text);

            return nativeString.GetBoundingRect(
                new CGSize(width, float.MaxValue),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes { Font = font },
                null);
        }
        /// <summary>
        /// Gets the rectangle of a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="width">The width.</param>
        /// <returns>CGRect.</returns>
        public static CGSize StringRectHTML(this string text, UIFont font, nfloat width)//UsesLineFragmentOrigin
        {
            string htmlStyle = "";
            try
            {
                htmlStyle = string.Format("<style>body{{font-family:'{0}'; font-size:{1}px;}}</style>",
                            "ArialMT",
                            14);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("getHtmlStyle - err: " + ex.ToString());
#endif
            }

            NSString htmlString = new NSString(text);
            NSData htmlData = NSData.FromString(htmlStyle + htmlString);
            NSAttributedStringDocumentAttributes importParams = new NSAttributedStringDocumentAttributes();
            importParams.DocumentType = NSDocumentType.HTML;
            importParams.StringEncoding = NSStringEncoding.UTF8;

            NSError error = new NSError();
            error = null;
            NSDictionary dict = new NSDictionary();
            if (font != null)
            {
                dict = new NSMutableDictionary()
                            {
                                {
                                    UIStringAttributeKey.Font,
                                    font
                                }
                            };
            }

            var attrString = new NSAttributedString(htmlData, importParams, out dict, ref error);
            return attrString.Size;
        }

        public static CGSize MeasureString(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return CGSize.Empty;

            var firstAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Arial-BoldMT", fontSize)
            };
            return new NSAttributedString(text, firstAttributes).Size;
        }
    }
    #endregion
}