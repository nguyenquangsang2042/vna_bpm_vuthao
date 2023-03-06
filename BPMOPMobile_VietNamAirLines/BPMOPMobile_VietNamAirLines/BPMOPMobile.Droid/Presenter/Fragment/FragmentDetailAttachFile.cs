using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentDetailAttachFile : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private TextView _tvTitle, _tvName;
        private ImageView _imgBack;
        private LinearLayout _lnAll, _lnDynamic;
        private Android.Net.Uri _mUri;
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();
        private FragmentDetailWorkflow _fragmentDetailWorkflow;
        private List<BeanAttachFile> _lstAttachFile = new List<BeanAttachFile>();
        private BeanWorkflowItem _workflowItem;
        private BeanNotify _notifyItem;
        private ViewElement _elementAttachFile = new ViewElement();

        public FragmentDetailAttachFile() { /* Prevent Darkmode */ }

        public FragmentDetailAttachFile(FragmentDetailWorkflow _fragmentDetailWorkflow, ViewElement _elementAttachFile, List<BeanAttachFile> _lstAttachFile, BeanWorkflowItem _workflowItem, BeanNotify _notifyItem)
        {
            this._lstAttachFile = _lstAttachFile;
            this._workflowItem = _workflowItem;
            this._notifyItem = _notifyItem;
            this._fragmentDetailWorkflow = _fragmentDetailWorkflow;
            this._elementAttachFile = _elementAttachFile;
        }
      
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewDetailAttachFile, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailAttachFile_All);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailAttachFile_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailAttachFile_Back);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailAttachFile_Title);
                _lnDynamic = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailAttachFile_Dynamic);
            }
            _lnAll.Click += (sender, e) => { };
            _imgBack.Click += Click_imgBack;

            SetView();

            Action action = new Action(() =>
            {
                SetData();
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);

            MinionActionCore.ElementFormClickEvent_WithInnerAction += Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
            return _rootView;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MinionActionCore.ElementFormClickEvent_WithInnerAction -= Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
        }

        #region Event
        private void SetView()
        {
            try
            {
                CTRLDetailWorkflow.SetTitleByItem(_tvName, _workflowItem, _notifyItem);
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "File đính kèm");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - SetView - Error: " + ex.Message);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - Click_imgBack - Error: " + ex.Message);
#endif
            }
        }

        private void SetData()
        {
            try
            {
                _lnDynamic.RemoveAllViews();
                ControlInputAttachmentVertical controlAttachment = new ControlInputAttachmentVertical(_mainAct, _lnDynamic, _elementAttachFile, -1, (int)EnumFormControlView.FlagViewControlAttachment.DetailAttachFile);
                controlAttachment.InitializeFrameView(_lnDynamic);
                controlAttachment.SetTitle();
                controlAttachment.SetValue();
                controlAttachment.SetEnable();
                controlAttachment.SetProprety();

                _lnDynamic.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void Click_ElementFormEvent_WithInnerAction(object sender, MinionActionCore.ElementFormClick_WithInnerAction e)
        {
            try
            {
                ViewElement _clickedElement = e.element;
                int _actionID = e.actionID;
                int _positonToAction = e.positionToAction;

                switch (_clickedElement.DataType)
                {
                    case "inputattachmenthorizon":
                    case "inputattachmentvertical":
                        if (_actionID == (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.View) // Nếu View file thì không cần check enable
                        {
                            ShowPopup_ControlInputAttachmentVertical_InnerAction(_clickedElement, _actionID, _positonToAction);
                        }
                        else if (_clickedElement.Enable == true) // Action khác -> Enable phải = true mới cho thao tác
                        {
                            ShowPopup_ControlInputAttachmentVertical_InnerAction(_clickedElement, _actionID, _positonToAction);
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ElementFormEvent_WithInnerAction", ex);
#endif
            }
        }

        private void ShowPopup_ControlInputAttachmentVertical_InnerAction(ViewElement clickedElement, int actionID, int positionToAction)
        {
            try
            {
                var data = clickedElement.Value.Trim();
                List<BeanAttachFile> _lstAttFileControl_Full = JsonConvert.DeserializeObject<List<BeanAttachFile>>(data);
                if (_lstAttFileControl_Full == null || _lstAttFileControl_Full.Count == 0)
                {
                    _lstAttFileControl_Full = new List<BeanAttachFile>();
                }

                switch (actionID)
                {
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Delete): // Xóa item trong RecyclerView
                        {
                            break;
                        }
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Edit): // Edit item trong RecyclerView
                        {
                            break;
                        }
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.View): // View item trong RecyclerView
                        {
                            if (!_lstAttFileControl_Full[positionToAction].ID.Equals("")) // mở file từ server
                            {
                                CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + _lstAttFileControl_Full[positionToAction].Path);
                            }
                            else // mở file từ local
                            {
                                if (System.IO.File.Exists(_lstAttFileControl_Full[positionToAction].Path))
                                {
                                    CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, _lstAttFileControl_Full[positionToAction].Path);
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlInputAttachmentVertical_InnerAction", ex);
#endif
            }
        }
        #endregion

        #region Data
        private async void OpenFileDinhkem(string link)
        {
            try
            {
                string pdfFilePath = System.IO.Path.Combine(CmmVariable.M_DataFolder + "/", System.IO.Path.GetFileName(link) ?? throw new InvalidOperationException());
                ProviderUser pUser = new ProviderUser();
                bool result;
                if (!System.IO.File.Exists(pdfFilePath))
                {

                }
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    result = pUser.DownloadFile(link, pdfFilePath, CmmVariable.M_AuthenticatedHttpClient);
                    if (result)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            LoadImage(pdfFilePath);

                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                              CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                    }
                });

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentDetailWorkflow - Error: " + ex.Message);
#endif
            }
        }

        private void LoadImage(string localpath)
        {
            try
            {
                //"/data/user/0/com.vuthao.becamexmobile.becamexmobile/files/data/20_YCCV - 20030011.pdf"
                Java.IO.File file = new Java.IO.File(localpath);
                //_mUri = FileProvider.GetUriForFile(_rootView.Context, "com.BecamexMobile.provider", file);
                _mUri = FileProvider.GetUriForFile(_rootView.Context, CmmDroidVariable.M_PackageProvider, file); // Xem lai
                _rootView.Context.RevokeUriPermission(_mUri, ActivityFlags.GrantReadUriPermission);
                string extension = System.IO.Path.GetExtension(localpath);
                string application = "";
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".doc":
                        case ".docx":
                            application = "application/msword";
                            break;
                        case ".pdf":
                            application = "application/pdf";
                            break;
                        case ".xls":
                        case ".xlsx":
                            application = "application/vnd.ms-excel";
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            application = "image/jpeg";//application = "image/*";
                            break;
                        default:
                            application = "*/*";
                            break;
                    }
                Intent intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetDataAndType(_mUri, application);
                StartActivity(intent);
            }
            catch (Exception)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    Toast.MakeText(_mainAct, "Bạn không có ứng dụng có thể mở loại tệp này.", ToastLength.Long).Show();
                });
            }
            finally
            {
                //Task.Delay(1000);
                //_rootView.Context.RevokeUriPermission("com.EuroWindow", uri, ActivityFlags.GrantReadUriPermission);
            }
        }

        private void UpdateValueFor2Fragment(ViewElement clickedElement, List<BeanAttachFile> _lstAttFileControl_Full)
        {
            // Cập nhật trang Detail Workflow
            _fragmentDetailWorkflow.UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
            // Cập nhật trang Detail Attach file
            this._elementAttachFile.Value = JsonConvert.SerializeObject(_lstAttFileControl_Full);
            SetData();
        }
        #endregion
    }
}