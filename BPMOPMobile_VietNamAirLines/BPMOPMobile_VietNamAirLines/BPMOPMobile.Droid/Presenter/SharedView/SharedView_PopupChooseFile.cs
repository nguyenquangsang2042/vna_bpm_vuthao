using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupChooseFile : SharedViewBase
    {
        public int _requestCodeFile;
        public int _requestCodeCamera;
        public int _flagview;

        public enum FlagView
        {
            [Description("DetailWorkflow - ControlInputAttachmentVertical")]
            DetailWorkflow_ControlInputAttachmentVertical = 0,

            [Description("DetailWorkflow - Comment")]
            DetailWorkflow_Comment = 1,

            [Description("DetailCreateTask - ControlInputAttachmentVertical")]
            DetailCreateTask_ControlInputAttachmentVertical = 2,

            [Description("DetailCreateTask - Comment")]
            DetailCreateTask_Comment = 3,

            [Description("DetailCreateTask_Child - ControlInputAttachmentVertical")]
            DetailCreateTask_Child_ControlInputAttachmentVertical = 4,

            [Description("ReplyComment")]
            ReplyComment = 5,
        }

        public SharedView_PopupChooseFile(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView,
            int _requestCodeFile, int _requestCodeCamera, int _flagview) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
            this._requestCodeFile = _requestCodeFile;
            this._requestCodeCamera = _requestCodeCamera;
            this._flagview = _flagview;
        }

        public override void InitializeView()
        {
            ControllerBase CTRLBase = new ControllerBase();
            try
            {
                #region Get View - Init Data
                DisplayMetrics _displayMetrics = _fragment.Resources.DisplayMetrics;
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControlAttachmentChooseFile, null);

                ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControlAttachmentChooseFile_Back);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_title);
                TextView _tvInApp = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_InApp);
                RecyclerView _recyInApp = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControlAttachmentChooseFile_InApp);
                ListView _lvInApp = _viewPopupControl.FindViewById<ListView>(Resource.Id.lv_PopupControlAttachmentChooseFile_InApp);
                TextView _tvOther = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other);
                LinearLayout _lnOtherCloud = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Cloud);
                LinearLayout _lnOtherLibrary = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Library);
                LinearLayout _lnOtherCamera = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Camera);
                TextView _tvOtherCloud = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Cloud);
                TextView _tvOtherLibrary = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Library);
                TextView _tvOtherCamera = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Camera);
                CTRLBase.RequestAppPermission(_mainAct);
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvTitle.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");
                    _tvInApp.Text = CmmFunction.GetTitle("TEXT_FILE_INAPP", "Tập tin trong ứng dụng");
                    _tvOther.Text = CmmFunction.GetTitle("TEXT_OTHER_RESOURCE", "Nguồn khác");
                    _tvOtherCloud.Text = CmmFunction.GetTitle("TEXT_STORAGE_APPLICATION", "Ứng dụng lưu trữ");
                    _tvOtherLibrary.Text = CmmFunction.GetTitle("TEXT_PHOTO_LIBRARY", "Thư viện ảnh");
                    _tvOtherCamera.Text = "Camera";
                }
                else
                {
                    _tvTitle.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Attached file");
                    _tvInApp.Text = CmmFunction.GetTitle("TEXT_FILE_INAPP", "Files in app");
                    _tvOther.Text = CmmFunction.GetTitle("TEXT_OTHER_RESOURCE", "Other resources");
                    _tvOtherCloud.Text = CmmFunction.GetTitle("TEXT_STORAGE_APPLICATION", "Storage application");
                    _tvOtherLibrary.Text = CmmFunction.GetTitle("TEXT_PHOTO_LIBRARY", "Photo library");
                    _tvOtherCamera.Text = "Camera";
                }

                #endregion

                #region Show View
                Dialog _dialogAction = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen_Animation);
                Window window = _dialogAction.Window;
                var dm = _fragment.Resources.DisplayMetrics;
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);

                _dialogAction.RequestWindowFeature(1);
                _dialogAction.SetCanceledOnTouchOutside(false);
                _dialogAction.SetCancelable(true);
                _dialogAction.SetContentView(_viewPopupControl);
                _dialogAction.Show();

                WindowManagerLayoutParams s = window.Attributes;
                s.Width = WindowManagerLayoutParams.MatchParent;
                s.Height = WindowManagerLayoutParams.MatchParent;
                window.Attributes = s;
                #endregion

                #region Event

                _imgBack.Click += (sender, e) =>
                {
                    _dialogAction.Dismiss();
                };
                _lnOtherCloud.Click += (sender, e) =>
                {
                    try
                    {
                        Intent intent = new Intent(Intent.ActionGetContent);
                        intent.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypes);
                        intent.SetFlags(ActivityFlags.ClearTop);
                        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                            intent.SetType("*/*");
                        else
                            intent.SetType("file/*");
                        _fragment.StartActivityForResult(intent, _requestCodeFile);

                        _dialogAction.Dismiss();

                    }
                    catch (Exception) { }
                };
                _lnOtherLibrary.Click += (sender, e) =>
                {
                    try
                    {
                        Intent intent = new Intent(Intent.ActionGetContent);
                        intent.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypesImage);
                        intent.SetFlags(ActivityFlags.ClearTop);
                        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                            intent.SetType("*/*");
                        else
                            intent.SetType("file/*");
                        _fragment.StartActivityForResult(intent, _requestCodeFile);

                        _dialogAction.Dismiss();
                    }
                    catch (Exception) {
                        _mainAct.RunOnUiThread(() =>
                        {
                            Intent intent = new Intent(Settings.ActionApplicationDetailsSettings);
                            Android.Net.Uri uri = Android.Net.Uri.FromParts("package", _mainAct.PackageName, null);
                            intent.SetData(uri);
                            _mainAct.StartActivity(intent);
                        });
                    }
                };
                _lnOtherCamera.Click += (sender, e) =>
                {
                    try
                    {
                        /*if (ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted ||
                        ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted ||
                        ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
                        {
                            ActivityCompat.RequestPermissions(_mainAct,
                                        new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, CmmDroidVariable.M_DetailWorkflow_ChooseFileComment_Camera);
                        }
                        else
                        {*/
                        string filePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", CmmDroidVariable.M_Camera_Prefix + SystemClock.ElapsedRealtime() + ".jpg");
                        Java.IO.File fileFromCamera = null;


                        switch (_flagview)
                        {
                            case (int)FlagView.DetailWorkflow_ControlInputAttachmentVertical:
                            case (int)FlagView.DetailWorkflow_Comment:
                                {
                                    FragmentDetailWorkflow _currentFragment = (FragmentDetailWorkflow)_fragment;
                                    _currentFragment._tempfileFromCamera = new Java.IO.File(filePath);
                                    fileFromCamera = _currentFragment._tempfileFromCamera;
                                    break;
                                }
                            case (int)FlagView.DetailCreateTask_ControlInputAttachmentVertical:
                            case (int)FlagView.DetailCreateTask_Comment:
                                {
                                    FragmentDetailCreateTask _currentFragment = (FragmentDetailCreateTask)_fragment;
                                    _currentFragment._tempfileFromCamera = new Java.IO.File(filePath);
                                    fileFromCamera = _currentFragment._tempfileFromCamera;
                                    break;
                                }
                            case (int)FlagView.DetailCreateTask_Child_ControlInputAttachmentVertical:
                                {
                                    FragmentDetailCreateTask_Child _currentFragment = (FragmentDetailCreateTask_Child)_fragment;
                                    _currentFragment._tempfileFromCamera = new Java.IO.File(filePath);
                                    fileFromCamera = _currentFragment._tempfileFromCamera;
                                    break;
                                }
                            case (int)FlagView.ReplyComment:
                                {
                                    FragmentReplyComment _currentFragment = (FragmentReplyComment)_fragment;
                                    _currentFragment._tempfileFromCamera = new Java.IO.File(filePath);
                                    fileFromCamera = _currentFragment._tempfileFromCamera;
                                    break;
                                }
                        }

                        if (fileFromCamera != null)
                        {
                            try
                            {
                                fileFromCamera.CreateNewFile();
                            }
                            catch (System.IO.IOException) { }
                            Android.Net.Uri outputURI = Android.Net.Uri.FromFile(fileFromCamera);

                            Intent intent = new Intent(MediaStore.ActionImageCapture);
                            intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(_mainAct, CmmDroidVariable.M_PackageProvider, fileFromCamera));
                            _fragment.StartActivityForResult(intent, _requestCodeCamera);
                        }
                        _dialogAction.Dismiss();
                    }
                    //}
                    catch (Exception ex)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            Intent intent = new Intent(Settings.ActionApplicationDetailsSettings);
                            Android.Net.Uri uri = Android.Net.Uri.FromParts("package", _mainAct.PackageName, null);
                            intent.SetData(uri);
                            _mainAct.StartActivity(intent);
                        });
                    }
                };

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }
    }
}