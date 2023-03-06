using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
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
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.SharedView;
using Com.Google.Android.Flexbox;
using Jp.Wasabeef;
using Newtonsoft.Json.Linq;
using static BPMOPMobile.Droid.Presenter.Adapter.AdapterDetailCreateTask_Attachment;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentDetailCreateTask_Child : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private LinearLayout _lnAll, _lnSubToolbar, _lnTieuDeContent, _lnNguoiXuLyContent, _lnHanHoanTatContent, _lnTinhTrangContent, _lnNoiDungContent, _lnNoiDungContentClick, _lnTaoMoi, _lnNguoiGiao;
        private View _rootView, _viewBlurRecycleUseChoose;
        private ImageView _imgTitleName, _imgCreateChildTask, _imgDoneTask;
        private TextView _tvName, _tvChiTiet, _tvCongViecCon, _tvAttachFile, _tvAttachFileTaoMoi, _tvAttachFileChild1, _tvAttachFileChild2, _tvTieuDe, _tvNguoiXuLy, _tvHanHoanTat, _tvTinhTrang, _tvNoiDung,
            _tvTieuDeContent, _tvHanHoanTatContent, _tvNguoiGiao;
        private RichEditor _richEditorNoiDung;
        private Dialog _dialogPopupControl;
        private ImageView _imgBack, _imgSave;
        private BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        private CustomFlexBoxRecyclerView _recyNguoiXuLy;
        private RecyclerView _recyAttachFile;
        private AdapterSelectUserGroupMultiple_Text2 _adapterListUserText;
        private ControllerDetailShare CTRLDetailShare = new ControllerDetailShare();
        private ControllerDetailCreateTask CTRLDetailCreateTask = new ControllerDetailCreateTask();
        private FragmentDetailWorkflow _fragmentDetailWorkflow;
        private FragmentDetailCreateTask _fragmentDetailCreateTask;

        private AdapterDetailCreateTask_Attachment _adapterAttachment;

        // DATA FORM DETAIL
        public JObject _OBJTASKFORM;
        private BeanTask _parentItem;
        //private List<BeanUserAndGroup> _lstAssignTo = new List<BeanUserAndGroup>();
        private List<BeanUserAndGroup> _lstCurrentUserGroup = new List<BeanUserAndGroup>();
        private List<BeanAttachFile> _lstAttachFileFull = new List<BeanAttachFile>();
        private List<string> _lstComment = new List<string>();
        private List<BeanTask> _lstChildTask = new List<BeanTask>();
        //private int _flagViewPermission = (int)ControllerDetailCreateTask.FlagPermission.CreateNew;
        private int _flagUserPermission = (int)ControllerDetailCreateTask.FlagUserPermission.Creator;
        private bool _IsClickFromAction = false; // Check xem là Click từ Action hay Control
        public Java.IO.File _tempfileFromCamera;

        public FragmentDetailCreateTask_Child() { /* Prevent Darkmode */ }

        public FragmentDetailCreateTask_Child(FragmentDetailCreateTask _fragmentDetailCreateTask, BeanTask _parentItem, bool _IsClickFromAction)
        {
            this._fragmentDetailCreateTask = _fragmentDetailCreateTask;
            this._parentItem = _parentItem;
            this._IsClickFromAction = _IsClickFromAction;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewDetailCreateTask, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_All);
                _lnSubToolbar = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_SubToolbar);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_Back);
                _imgSave = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_Done);
                _imgCreateChildTask = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_ChildTask);
                _imgDoneTask = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_DoneTask);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_Name);
                _imgTitleName = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_Name);
                _tvChiTiet = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_ChiTiet);
                _tvCongViecCon = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_CongViecCon);
                // -- TieuDe --
                _tvTieuDe = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TieuDe);
                _lnTieuDeContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_TieuDe_Content);
                _tvTieuDeContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TieuDe_Content);
                // -- NguoiXuLy --
                _tvNguoiXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NguoiXuLy);
                _lnNguoiXuLyContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NguoiXuLy_Content);
                _viewBlurRecycleUseChoose = _rootView.FindViewById<View>(Resource.Id.viewBlurRecycleUseChoose);
                _recyNguoiXuLy = _rootView.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_ViewDetailCreateTask_NguoiXuLy);
                // -- HanHoanTat --
                _tvHanHoanTat = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_HanHoanTat);
                _lnHanHoanTatContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_HanHoanTat_Content);
                _tvHanHoanTatContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_HanHoanTat_Content);
                // -- TinhTrang --
                _tvTinhTrang = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TinhTrang);
                _lnTinhTrangContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_TinhTrang_Content);
                // -- NguoiGiao --
                _lnNguoiGiao = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NguoiGiao);
                _tvNguoiGiao = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NguoiGiao);
                // -- NoiDung --
                _tvNoiDung = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NoiDung);
                _lnNoiDungContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NoiDung_Content);
                _lnNoiDungContentClick = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NoiDung_ContentClick); // Để che lên richeditor click dc
                _richEditorNoiDung = _rootView.FindViewById<RichEditor>(Resource.Id.richEditor_ViewDetailCreateTask_NoiDung_Content);
                // -- DinhKem --
                _recyAttachFile = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewDetailCreateTask_AttachFile);
                _lnTaoMoi = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_AttachFile_TaoMoi);
                _tvAttachFile = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile);
                _tvAttachFileTaoMoi = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_TaoMoi);
                _tvAttachFileChild1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_Child1);
                _tvAttachFileChild2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_Child2);

                _lnTieuDeContent.Click += Click_lnTieuDeContent;
                _lnNguoiXuLyContent.Click += Click_lnNguoiXuLyContent;
                _viewBlurRecycleUseChoose.Click += Click_lnNguoiXuLyContent;
                _lnHanHoanTatContent.Click += Click_lnHanHoanTatContent;
                _tvNoiDung.Click += Click_lnNoiDungContent;
                _lnNoiDungContent.Click += Click_lnNoiDungContent;
                _lnNoiDungContentClick.Click += Click_lnNoiDungContent;
                _lnTaoMoi.Click += Click_lnTaoMoi;

                _richEditorNoiDung.Enabled = false;
                _imgBack.Click += Click_imgBack;
                _imgSave.Click += Click_imgSave;

                _imgTitleName.Visibility = ViewStates.Gone;
                _lnSubToolbar.Visibility = ViewStates.Gone;
                _imgCreateChildTask.Visibility = ViewStates.Gone;
                _lnTinhTrangContent.Visibility = ViewStates.Invisible;
                _tvTinhTrang.Visibility = ViewStates.Invisible;
                _imgDoneTask.Visibility = ViewStates.Gone;
                _lnAll.Click += (sender, e) => { };

                _lnNguoiGiao.Visibility = ViewStates.Gone;
            }
            SetViewByLanguage();
            SetData();

            return _rootView;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if ((requestCode == CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment || requestCode == CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment_Camera) && resultCode == (int)Result.Ok) // đính kèm comment
                {
                    // Comment
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment) // chọn file thường
                    {
                        _beanAttachFile = _mainAct.GetAttachFileFromURI(_mainAct, _rootView.Context, data.Data);
                    }
                    else // chụp từ camera
                    {
                        if (_tempfileFromCamera != null)
                        {
                            Android.Net.Uri contentUri = FileProvider.GetUriForFile(_mainAct, CmmDroidVariable.M_PackageProvider, _tempfileFromCamera);
                            _beanAttachFile = _mainAct.GetAttachFileFromURI_Camera(_mainAct, _rootView.Context, contentUri);
                            _beanAttachFile.Path = _tempfileFromCamera.Path;
                        }
                        else
                        {
                            _beanAttachFile = null;
                        }
                    }

                    if (_beanAttachFile == null || (_beanAttachFile != null && string.IsNullOrEmpty(_beanAttachFile.Path)))
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                          CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        return;
                    }

                    if (CTRLDetailCreateTask.CheckFileExistInList(_lstAttachFileFull, _beanAttachFile) == true)
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                           CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                        return;
                    }

                    _lstAttachFileFull.Add(_beanAttachFile);
                    _adapterAttachment.NotifyDataSetChanged();
                    _recyAttachFile.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(60, _rootView.Context) * _lstAttachFileFull.Count);
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("BANANA", "Không thể chọn tâp tin này."),
                                                           CmmFunction.GetTitle("BANANA", "Can't choose this file."));

#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Override_OnActivityResult", ex);
#endif
            }
        }

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                _tvChiTiet.Text = CmmFunction.GetTitle("TEXT_DETAIL", "Chi tiết");
                _tvCongViecCon.Text = CmmFunction.GetTitle("TEXT_CHILDTASK", "Công việc con");
                _tvTieuDe.Text = CmmFunction.GetTitle("TEXT_TITLE", "Tiêu đề") + " (*)";
                _tvNguoiXuLy.Text = CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý") + " (*)";
                _tvHanHoanTat.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn hoàn tất");
                _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                _tvNoiDung.Text = CmmFunction.GetTitle("TEXT_CONTENT", "Nội dung");
                _tvAttachFile.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");
                _tvAttachFileTaoMoi.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới");
                _tvAttachFileChild1.Text = CmmFunction.GetTitle("TEXT_CONTROL_DOCUMENTNAME", "Tên tài liệu");
                _tvAttachFileChild2.Text = CmmFunction.GetTitle("TEXT_ASSIGNORS", "Người giao");
                _tvNguoiGiao.Text = CmmFunction.GetTitle("TEXT_CREATOR", "Người tạo");

                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvTieuDe);
                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvNguoiXuLy);
                //CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvNoiDung);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private async void Click_imgSave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvTieuDeContent.Text))
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập tiêu đề"),
                                                             CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Please enter title"));
                    return;
                }

                if (_lstCurrentUserGroup == null || _lstCurrentUserGroup.Count <= 0)
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện."),
                                                               CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Please choose user to do action."));
                    return;
                }
                //if (String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()))
                //{
                //    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập nội dung"),
                //                                             CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Please enter content"));
                //    return;
                //}
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    bool _result = false;

                    #region Handle List Attach File
                    List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
                    if (_lstAttachFileFull != null && _lstAttachFileFull.Count > 0)
                    {
                        foreach (var item in _lstAttachFileFull)
                        {
                            if (item.ID == "")
                            {
                                string key = item.Title;
                                KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
                                _lstKeyVarAttachmentLocal.Add(_UploadFile);
                            }
                        }
                    }
                    #endregion

                    BeanTask _itemTask = new BeanTask();

                    #region Handle Bean Task
                    _itemTask.ID = 0;
                    _itemTask.Title = _tvTieuDeContent.Text;
                    if (!String.IsNullOrEmpty(_tvHanHoanTatContent.Text))
                    {
                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _itemTask.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                        else
                            _itemTask.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);

                        if (DateTime.Compare(_itemTask.DueDate.Value, DateTime.Now) < 0)
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE2", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại!"),
                                             CmmFunction.GetTitle("TEXT_DATE_COMPARE2", "Start date cannot be greater than date now!"));
                            });
                            return;
                        }
                    }
                    else
                    {
                        _itemTask.DueDate = null;
                    }

                    if (_parentItem != null) // Đã có parent Item
                    {
                        _itemTask.Parent = _parentItem.ID;  // Tạo Task con cần thêm Parent
                        _itemTask.WorkflowId = _parentItem.WorkflowId;
                        _itemTask.SPItemId = _parentItem.SPItemId;
                        _itemTask.Step = _parentItem.Step;
                    }
                    else if (_fragmentDetailWorkflow != null)
                    {
                        _itemTask.WorkflowId = _workflowItem.WorkflowID;
                        _itemTask.SPItemId = int.Parse(_workflowItem.ID);
                        _itemTask.Step = _workflowItem.Step.HasValue ? _workflowItem.Step.Value : -1;
                    }
                    _itemTask.Content = !String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()) ? _richEditorNoiDung.GetHtml() : "";
                    _itemTask.Status = 0; // Tạo mới
                    #endregion

                    _result = _pControlDynamic.SendCreateTaskAction(_itemTask, _lstCurrentUserGroup, new List<ObjectSubmitAction>(), _lstKeyVarAttachmentLocal, (int)ControllerDetailCreateTask.FlagActionPermission.CreateNew);

                    if (_result)
                    {
                        //ProviderBase pBase = new ProviderBase();
                        //pBase.UpdateAllMasterData(true);
                        //pBase.UpdateAllDynamicData(true);
                        _mainAct.RunOnUiThread(() =>
                        {
                            _fragmentDetailCreateTask.RenewCurrentPageAndPrevious(true, false); // ko cần check quyền cho nhẹ
                            Click_imgBack(null, null);
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
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDone", ex);
#endif
            }
        }

        private void Click_lnTieuDeContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlTextInput(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    #region Get View - Init Data
                    View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputText, null);
                    ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Close);
                    TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputText_Title);
                    ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Done);
                    ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Delete);
                    EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_InputText);
                    ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_ClearText);

                    _imgDelete.Visibility = ViewStates.Gone;

                    _tvTitle.Text = _tvTieuDe.Text;
                    CmmDroidFunction.SetTextViewHighlightControl(_rootView.Context, _tvTitle);
                    _edtContent.SetFilters(new Android.Text.IInputFilter[] { new Android.Text.InputFilterLengthFilter(255) }); // Limit lại 255 kí tự

                    if (!String.IsNullOrEmpty(_tvTieuDeContent.Text))
                        _edtContent.Text = CmmDroidFunction.FormatHTMLToString(_tvTieuDeContent.Text);
                    else
                        _edtContent.Text = "";
                    #endregion

                    #region Event
                    _imgBack.Click += (sender, e) =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                        _dialogPopupControl.Dismiss();
                    };
                    _imgClearText.Click += (sender, e) =>
                    {
                        _edtContent.Text = "";
                    };

                    _imgDelete.Click += (sender, e) =>
                    {
                        _edtContent.Text = "";
                        _imgDone.PerformClick();
                    };

                    _imgDone.Click += delegate
                    {
                        _tvTieuDeContent.Text = _edtContent.Text;
                        //string _result = "";
                        //if (!String.IsNullOrEmpty(_edtContent.Text))
                        //{
                        //    _result = _edtContent.Text;
                        //}
                        //UpdateValueForElement(clickedElement, _result);
                        //_adapterDetailExpandControl.NotifyDataSetChanged();
                        CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                        _dialogPopupControl.Dismiss();
                    };
                    #endregion

                    #region Show View                
                    _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                    Window window = _dialogPopupControl.Window;
                    _dialogPopupControl.RequestWindowFeature(1);
                    _dialogPopupControl.SetCanceledOnTouchOutside(false);
                    _dialogPopupControl.SetCancelable(true);
                    window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    window.SetGravity(GravityFlags.Bottom);
                    var dm = _mainAct.Resources.DisplayMetrics;

                    _dialogPopupControl.SetContentView(_viewPopupControl);
                    _dialogPopupControl.Show();
                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = WindowManagerLayoutParams.MatchParent;
                    s.Height = WindowManagerLayoutParams.MatchParent;
                    window.Attributes = s;
                    #endregion
                    _edtContent.RequestFocus();
                    CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnTieuDeContent", ex);
#endif
            }
        }

        private void Click_lnNguoiXuLyContent(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                    List<BeanUserAndGroup> _lstSelected = _lstCurrentUserGroup.ToList();

                    #region Get View - Init Data
                    View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                    LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                    ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                    ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                    ImageView _imgDeleteChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Delete);
                    TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                    EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                    RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);
                    CustomFlexBoxRecyclerView _recySelectedUser = _viewPopupControl.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_PopupControl_SelectedUser);

                    _recySelectedUser.Visibility = ViewStates.Visible;
                    _imgDeleteChooseUser.Visibility = ViewStates.Visible;
                    _recySelectedUser.SetMaxRowAndRowHeight((int)CmmDroidFunction.ConvertDpToPixel(35, _rootView.Context), 3); // 95 *3

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvTitleChooseUser.Text = CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm");
                        _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email");
                    }
                    else
                    {
                        _tvTitleChooseUser.Text = CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Choose users or groups");
                        _edtSearchChooseUser.Hint = CmmFunction.GetTitle("MESS_REQUIRE_EMAIL", "Leave a name or email here");
                    }

                    SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    _lstUserAndGroupAll = conn.Query<BeanUserAndGroup>(CTRLDetailShare._queryShareUserGroup);
                    conn.Close();

                    if (_lstUserAndGroupAll != null && _lstUserAndGroupAll.Count > 0 && _lstSelected != null && _lstSelected.Count > 0) // Người đã được chọn sẽ không hiển thị vào list     
                    {
                        for (int i = 0; i < _lstSelected.Count; i++)
                        {
                            _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_lstSelected[i].ID)).ToList();
                        }
                    }

                    if (_lstUserAndGroupAll == null) _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                    if (_lstSelected == null) _lstSelected = new List<BeanUserAndGroup>();

                    AdapterSelectUserGroupMultiple _adapterListUser = new AdapterSelectUserGroupMultiple(_mainAct, _rootView.Context, _lstUserAndGroupAll, _lstSelected);
                    AdapterSelectUserGroupMultiple_Text _adapterListUserSelected = new AdapterSelectUserGroupMultiple_Text(_mainAct, _rootView.Context, _lstSelected);
                    FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(_rootView.Context);
                    flexboxLayoutManager.FlexDirection = FlexDirection.Row;
                    flexboxLayoutManager.JustifyContent = JustifyContent.FlexStart;

                    _recySelectedUser.SetAdapter(_adapterListUserSelected);
                    _recySelectedUser.SetLayoutManager(flexboxLayoutManager);
                    #endregion

                    #region Event
                    _edtSearchChooseUser.TextChanged += async (sender, e) =>
                    {
                        await Task.Run(() =>
                        {
                            List<BeanUserAndGroup> _lstSearch = new List<BeanUserAndGroup>();
                            if (!String.IsNullOrEmpty(_edtSearchChooseUser.Text))
                            {
                                try
                                {
                                    string _content = CmmFunction.removeSignVietnamese(_edtSearchChooseUser.Text).ToLowerInvariant();
                                    _lstSearch = _lstUserAndGroupAll.Where(x => x.Email != null /* && x.Email != CmmVariable.SysConfig.Email*/).ToList();
                                    _lstSearch = _lstSearch.Where(x => CmmFunction.removeSignVietnamese(x.Name).ToLowerInvariant().Contains(_content)
                                                                    || CmmFunction.removeSignVietnamese(x.Email).ToLowerInvariant().Contains(_content)).ToList();
                                }
                                catch (Exception)
                                {
                                    _lstSearch = new List<BeanUserAndGroup>();
                                }
                            }
                            else
                            {
                                _lstSearch = new List<BeanUserAndGroup>();
                            }

                            _adapterListUser = new AdapterSelectUserGroupMultiple(_mainAct, _rootView.Context, _lstSearch, _lstSelected);
                            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);

                            _mainAct.RunOnUiThread(() =>
                            {
                                _recyChooseUser.SetAdapter(_adapterListUser);
                                _recyChooseUser.SetLayoutManager(staggeredGridLayoutManager);
                            });
                            _adapterListUser.CustomItemClick += (sender, e) =>
                            {
                                BeanUserAndGroup _clickedItem = e;
                                if (_clickedItem != null)
                                {
                                    _lstSelected.Add(e);
                                    _lstUserAndGroupAll = _lstUserAndGroupAll.Where(x => !x.ID.Equals(_clickedItem.ID)).ToList();
                                    _mainAct.RunOnUiThread(() =>
                                    {
                                        _adapterListUser.UpdateCurrentList(_lstUserAndGroupAll);
                                        _adapterListUser.NotifyDataSetChanged();
                                        _adapterListUserSelected.UpdateItemListIsClicked(_lstSelected);
                                        _adapterListUserSelected.NotifyDataSetChanged();
                                        _edtSearchChooseUser.Text = _edtSearchChooseUser.Text; // để set Adapter lại
                                        _edtSearchChooseUser.SetSelection(_edtSearchChooseUser.Text.Length);

                                        _recySelectedUser.SmoothScrollToPosition(_lstSelected.Count); // focus lại vi trí cuối cùng
                                    });
                                }
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _edtSearchChooseUser.Text = "";
                                });
                            };
                        });
                    };
                    _imgCloseChooseUser.Click += (sender, e) =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                        _dialogPopupControl.Dismiss();

                    };
                    _adapterListUserSelected.CustomItemClick += (sender, e) =>
                    {
                        BeanUserAndGroup _clickedItem = e;
                        if (_clickedItem != null)
                        {
                            _lstUserAndGroupAll.Add(e);
                            _lstSelected = _lstSelected.Where(x => !x.ID.Equals(_clickedItem.ID)).ToList(); // Loại Item vừa click ra
                            _mainAct.RunOnUiThread(() =>
                            {
                                _adapterListUser.UpdateCurrentList(_lstUserAndGroupAll);
                                _adapterListUser.NotifyDataSetChanged();
                                _adapterListUserSelected.UpdateItemListIsClicked(_lstSelected);
                                _adapterListUserSelected.NotifyDataSetChanged();
                                _edtSearchChooseUser.Text = _edtSearchChooseUser.Text; // để set Adapter lại
                                _edtSearchChooseUser.SetSelection(_edtSearchChooseUser.Text.Length);
                            });
                        }
                    };

                    _imgDeleteChooseUser.Click += (sender, e) =>
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            // Update cho Data bên ngoài
                            _lstCurrentUserGroup = new List<BeanUserAndGroup>();
                            _adapterListUserText.UpdateItemListIsClicked(_lstCurrentUserGroup);
                            _adapterListUserText.NotifyDataSetChanged(); // notify list ở ngoài    
                            SetDataItemTask(_parentItem, _lstCurrentUserGroup);

                            CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                            _dialogPopupControl.Dismiss();
                        });
                    };

                    _imgAcceptChooseUser.Click += (sender, e) =>
                    {
                        List<BeanUserAndGroup> _lstResult = new List<BeanUserAndGroup>();
                        if (_adapterListUserSelected != null)
                        {
                            _lstResult = _adapterListUserSelected.GetListIsclicked();
                        }
                        if (_lstResult == null) _lstResult = new List<BeanUserAndGroup>();
                        _mainAct.RunOnUiThread(() =>
                        {
                            // Update cho Data bên ngoài
                            _lstCurrentUserGroup = _lstResult;
                            _adapterListUserText.UpdateItemListIsClicked(_lstCurrentUserGroup);
                            _adapterListUserText.NotifyDataSetChanged(); // notify list ở ngoài    
                                                                         //SetDataItemTask(null, _lstCurrentUserGroup);

                            CmmDroidFunction.HideSoftKeyBoard(_edtSearchChooseUser, _mainAct);
                            _dialogPopupControl.Dismiss();
                        });
                    };
                    #endregion
                    _mainAct.RunOnUiThread(() =>
                    {
                        #region Show View                
                        _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                        Window window = _dialogPopupControl.Window;
                        _dialogPopupControl.RequestWindowFeature(1);
                        _dialogPopupControl.SetCanceledOnTouchOutside(false);
                        _dialogPopupControl.SetCancelable(true);
                        window.SetGravity(GravityFlags.Bottom);
                        window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                        var dm = _mainAct.Resources.DisplayMetrics;

                        _dialogPopupControl.SetContentView(_viewPopupControl);
                        _dialogPopupControl.Show();
                        WindowManagerLayoutParams s = window.Attributes;
                        s.Width = WindowManagerLayoutParams.MatchParent;
                        s.Height = WindowManagerLayoutParams.MatchParent;
                        window.Attributes = s;
                        #endregion
                        _edtSearchChooseUser.Text = "";
                        _edtSearchChooseUser.RequestFocus();

                        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnNguoiXuLyContent", ex);
#endif
            }
        }

        private void Click_lnHanHoanTatContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlDateTime(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    #region Get View - Init Data
                    View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DateTimePicker, null);
                    DatePicker _datePicker = _viewPopupControl.FindViewById<DatePicker>(Resource.Id.dp_PopupControl_DateTimePicker);
                    TimePicker _timePicker = _viewPopupControl.FindViewById<TimePicker>(Resource.Id.tp_PopupControl_DateTimePicker);
                    ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Delete);
                    ImageView _imgToday = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Today);
                    ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Close);
                    TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DateTimePicker_Title);
                    LinearLayout _lnApply = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_DateTimePicker_Clear);
                    TextView _tvApply = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DateTimePicker_Apply);

                    _timePicker.SetIs24HourView(Java.Lang.Boolean.True);

                    _tvApply.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");
                    DateTime _initDate = new DateTime();
                    try
                    {
                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _initDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                        else
                            _initDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);

                        _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                        _timePicker.Hour = _initDate.Hour;
                        _timePicker.Minute = _initDate.Minute;
                    }
                    catch
                    {
                        _initDate = DateTime.Now;
                    }

                    _tvTitle.Text = CTRLDetailShare.GetFormatDateLang(_initDate);
                    #endregion

                    #region Event
                    _imgClose.Click += (sender, e) =>
                    {
                        _dialogPopupControl.Dismiss();
                    };

                    _imgDelete.Click += delegate
                    {
                        _tvHanHoanTatContent.Text = "";
                        _dialogPopupControl.Dismiss();
                    };

                    _imgToday.Click += delegate
                    {
                        DateTime _initDate = DateTime.Now;
                        _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                        _timePicker.Hour = _initDate.Hour;
                        _timePicker.Minute = _initDate.Minute;
                    };

                    _lnApply.Click += delegate
                    {
                        DateTime _resultDate = new DateTime(_datePicker.Year, _datePicker.Month + 1, _datePicker.DayOfMonth, _timePicker.Hour, _timePicker.Minute, 0);
                        _tvHanHoanTatContent.Text = CTRLDetailShare.GetFormatDateLang(_resultDate);
                        _dialogPopupControl.Dismiss();
                    };
                    #endregion

                    #region Show View                
                    _dialogPopupControl = new Dialog(_rootView.Context);
                    Window window = _dialogPopupControl.Window;
                    _dialogPopupControl.RequestWindowFeature(1);
                    _dialogPopupControl.SetCanceledOnTouchOutside(false);
                    _dialogPopupControl.SetCancelable(true);
                    window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    window.SetGravity(GravityFlags.Center);
                    var dm = _mainAct.Resources.DisplayMetrics;

                    _dialogPopupControl.SetContentView(_viewPopupControl);
                    _dialogPopupControl.Show();
                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = dm.WidthPixels;
                    s.Height = WindowManagerLayoutParams.WrapContent;
                    window.Attributes = s;
                    window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                    #endregion
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnHanHoanTatContent", ex);
#endif
            }
        }

        private void Click_lnNoiDungContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlTextInputFormat(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    bool isChanged = true;
                    #region Get View - Init Data
                    View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputTextFormat, null);
                    ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Close);
                    TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputTextFormat_Title);
                    ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Done);
                    ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Delete);
                    RichEditor mEditor = _viewPopupControl.FindViewById<RichEditor>(Resource.Id.editor_PopupControl_InputTextFormat);

                    _imgDelete.Visibility = ViewStates.Gone;

                    if (!String.IsNullOrEmpty(_tvNoiDung.Text))
                    {
                        _tvTitle.Text = _tvNoiDung.Text;
                    }
                    else
                    {
                        _tvTitle.Text = "";
                    }

                    if (!String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()))
                    {
                        mEditor.SetHtml(_richEditorNoiDung.GetHtml());
                    }
                    #endregion

                    #region Event
                    _imgBack.Click += (sender, e) =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                        _dialogPopupControl.Dismiss();
                    };

                    _imgDelete.Click += (sender, e) =>
                    {
                        mEditor.SetHtml("");
                        _imgDone.PerformClick();
                    };

                    _imgDone.Click += delegate
                    {
                        string _result = "";
                        if (!String.IsNullOrEmpty(mEditor.GetHtml()))
                        {
                            _result = mEditor.GetHtml().ToString();
                        }
                        _richEditorNoiDung.SetHtml(_result);
                        CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                        _dialogPopupControl.Dismiss();
                    };

                    #region Editor Library
                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_undo).Click += delegate { mEditor.Undo(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_redo).Click += delegate { mEditor.Redo(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bold).Click += delegate { mEditor.SetBold(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_italic).Click += delegate { mEditor.SetItalic(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_subscript).Click += delegate { mEditor.SetSubscript(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_superscript).Click += delegate { mEditor.SetSuperscript(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_strikethrough).Click += delegate { mEditor.SetStrikeThrough(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_underline).Click += delegate { mEditor.SetUnderline(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading1).Click += delegate { mEditor.SetHeading(1); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading2).Click += delegate { mEditor.SetHeading(2); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading3).Click += delegate { mEditor.SetHeading(3); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading4).Click += delegate { mEditor.SetHeading(4); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading5).Click += delegate { mEditor.SetHeading(5); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading6).Click += delegate { mEditor.SetHeading(6); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_txt_color).Click += delegate { mEditor.SetTextColor(isChanged ? Color.Black : Color.Red); isChanged = !isChanged; };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bg_color).Click += delegate { mEditor.SetTextBackgroundColor(isChanged ? Color.Transparent : Color.Yellow); isChanged = !isChanged; };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_indent).Click += delegate { mEditor.SetIndent(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_outdent).Click += delegate { mEditor.SetOutdent(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_left).Click += delegate { mEditor.SetAlignLeft(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_center).Click += delegate { mEditor.SetAlignCenter(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_right).Click += delegate { mEditor.SetAlignRight(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_blockquote).Click += delegate { mEditor.SetBlockquote(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_numbers).Click += delegate { mEditor.SetNumbers(); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_image).Click += delegate { mEditor.InsertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_link).Click += delegate { mEditor.InsertLink("https://github.com/wasabeef", "wasabeef"); };

                    _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_checkbox).Click += delegate { mEditor.InsertTodo(); };
                    #endregion

                    #endregion

                    #region Show View                
                    _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                    Window window = _dialogPopupControl.Window;
                    _dialogPopupControl.RequestWindowFeature(1);
                    _dialogPopupControl.SetCanceledOnTouchOutside(false);
                    _dialogPopupControl.SetCancelable(true);
                    window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    window.SetGravity(GravityFlags.Bottom);
                    var dm = _mainAct.Resources.DisplayMetrics;

                    _dialogPopupControl.SetContentView(_viewPopupControl);
                    _dialogPopupControl.Show();
                    WindowManagerLayoutParams s = window.Attributes;
                    s.Width = WindowManagerLayoutParams.MatchParent;
                    s.Height = WindowManagerLayoutParams.MatchParent;
                    window.Attributes = s;
                    #endregion
                    //_richEditorNoiDung.RequestFocus();
                    //CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnTieuDeContent", ex);
#endif
            }
        }

        private void Click_lnTaoMoi(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentDetailCreateTask_Child", _rootView,
                        CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment,
                        CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment_Camera,
                        (int)SharedView_PopupChooseFile.FlagView.DetailCreateTask_Comment);
                    SharedPopUpChooseFile.InitializeView();

                    #region Old

                    //#region Get View - Init Data
                    //DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                    //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControlAttachmentChooseFile, null);
                    //ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControlAttachmentChooseFile_Back);
                    //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_title);
                    //TextView _tvInApp = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_InApp);
                    //RecyclerView _recyInApp = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControlAttachmentChooseFile_InApp);
                    //ListView _lvInApp = _viewPopupControl.FindViewById<ListView>(Resource.Id.lv_PopupControlAttachmentChooseFile_InApp);
                    //TextView _tvOther = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other);
                    //LinearLayout _lnOtherCloud = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Cloud);
                    //LinearLayout _lnOtherLibrary = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Library);
                    //LinearLayout _lnOtherCamera = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControlAttachmentChooseFile_Other_Camera);
                    //TextView _tvOtherCloud = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Cloud);
                    //TextView _tvOtherLibrary = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Library);
                    //TextView _tvOtherCamera = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControlAttachmentChooseFile_Other_Camera);

                    //if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    //{
                    //    _tvTitle.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");
                    //    _tvInApp.Text = CmmFunction.GetTitle("TEXT_FILE_INAPP", "Tập tin trong ứng dụng");
                    //    _tvOther.Text = CmmFunction.GetTitle("TEXT_OTHER_RESOURCE", "Nguồn khác");
                    //    _tvOtherCloud.Text = CmmFunction.GetTitle("TEXT_STORAGE_APPLICATION", "Ứng dụng lưu trữ");
                    //    _tvOtherLibrary.Text = CmmFunction.GetTitle("TEXT_PHOTO_LIBRARY", "Thư viện ảnh");
                    //    _tvOtherCamera.Text = "Camera";
                    //}
                    //else
                    //{
                    //    _tvTitle.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Attached file");
                    //    _tvInApp.Text = CmmFunction.GetTitle("TEXT_FILE_INAPP", "Files in app");
                    //    _tvOther.Text = CmmFunction.GetTitle("TEXT_OTHER_RESOURCE", "Other resources");
                    //    _tvOtherCloud.Text = CmmFunction.GetTitle("TEXT_STORAGE_APPLICATION", "Storage application");
                    //    _tvOtherLibrary.Text = CmmFunction.GetTitle("TEXT_PHOTO_LIBRARY", "Photo library");
                    //    _tvOtherCamera.Text = "Camera";
                    //}

                    //#endregion

                    //#region Show View
                    //_dialogAction = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen_Animation);
                    //Window window = _dialogAction.Window;
                    //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    //window.SetGravity(GravityFlags.Center);

                    //_dialogAction.RequestWindowFeature(1);
                    //_dialogAction.SetCanceledOnTouchOutside(false);
                    //_dialogAction.SetCancelable(true);
                    //_dialogAction.SetContentView(_viewPopupControl);
                    //_dialogAction.Show();

                    //WindowManagerLayoutParams s = window.Attributes;
                    //s.Width = WindowManagerLayoutParams.MatchParent;
                    //s.Height = WindowManagerLayoutParams.MatchParent;
                    //window.Attributes = s;
                    //#endregion

                    //List<BeanAttachFileControl> _lstInApp = new List<BeanAttachFileControl>();
                    //_imgBack.Click += (sender, e) =>
                    //{
                    //    _dialogAction.Dismiss();
                    //};
                    //_tvOtherCloud.Click += (sender, e) =>
                    //{
                    //    Intent i = new Intent(Intent.ActionGetContent);
                    //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                    //    {
                    //        i.SetType("*/*");
                    //    }
                    //    else
                    //    {
                    //        i.SetType("file/*");
                    //    }
                    //    i.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypes);
                    //    i.AddCategory(Intent.CategoryOpenable);
                    //    i.PutExtra(Intent.ExtraLocalOnly, true);
                    //    StartActivityForResult(i, CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment);

                    //    _dialogAction.Dismiss();
                    //};
                    //_tvOtherLibrary.Click += (sender, e) =>
                    //{
                    //    Intent i = new Intent(Intent.ActionGetContent);
                    //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                    //    {
                    //        i.SetType("*/*");
                    //    }
                    //    else
                    //    {
                    //        i.SetType("file/*");
                    //    }
                    //    i.PutExtra(Intent.ExtraMimeTypes, CmmDroidVariable.M_MimeTypes);
                    //    i.AddCategory(Intent.CategoryOpenable);
                    //    i.PutExtra(Intent.ExtraLocalOnly, true);
                    //    StartActivityForResult(i, CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment);

                    //    _dialogAction.Dismiss();
                    //};
                    //_lnOtherCamera.Click += (sender, e) =>
                    //{
                    //    try
                    //    {
                    //        if (ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted ||
                    //        ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted ||
                    //        ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted)
                    //        {
                    //            ActivityCompat.RequestPermissions(_mainAct,
                    //                        new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, CmmDroidVariable.M_DetailWorkflow_ChooseFileComment_Camera);
                    //        }
                    //        else
                    //        {
                    //            string filePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", CmmDroidVariable.M_Camera_Prefix + (Int32)DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + ".jpg");
                    //            _tempfileFromCamera = new Java.IO.File(filePath);
                    //            try
                    //            {
                    //                _tempfileFromCamera.CreateNewFile();
                    //            }
                    //            catch (System.IO.IOException ex) { }

                    //            Android.Net.Uri outputURI = Android.Net.Uri.FromFile(_tempfileFromCamera);

                    //            Intent intent = new Intent(MediaStore.ActionImageCapture);
                    //            intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(_mainAct, CmmDroidVariable.M_PackageProvider, _tempfileFromCamera));
                    //            StartActivityForResult(intent, CmmDroidVariable.M_DetailCreateTask_Child_ChooseFileControlAttachment_Camera);

                    //            _dialogAction.Dismiss();
                    //        }
                    //    }
                    //    catch (Exception ex) { }
                    //};

                    #endregion
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlInputAttachmentVertical", ex);
#endif
            }
        }

        private void Click_ItemReyAttachment_View(object sender, BeanAttachFile e)
        {
            try
            {
                if (!e.ID.Equals("")) // mở file từ server
                {
                    CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + e.Path);
                }
                else // mở file từ local
                {
                    if (System.IO.File.Exists(e.Path))
                    {
                        CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, e.Path);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemReyAttachment_View", ex);
#endif
            }
        }

        private void Click_ItemReyAttachment_Save(object sender, BeanAttachFile e)
        {
            try
            {

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemReyAttachment_Save", ex);
#endif
            }
        }

        private void Click_ItemReyAttachment_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                Action _actionPositiveButton = new Action(() =>
                {
                    _lstAttachFileFull = _lstAttachFileFull.Where(x => x.Title != e.Title).ToList();
                    _adapterAttachment.UpdateListData(_lstAttachFileFull);
                    _adapterAttachment.NotifyDataSetChanged();
                    _recyAttachFile.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(60, _rootView.Context) * _lstAttachFileFull.Count);
                });

                Action _actionNegativeButton = new Action(() =>
                {
                    // đã có Dispose trong hàm
                });

                CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không ?"),
                _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemReyAttachment_Delete", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                SetDataItemTask(_parentItem, _lstCurrentUserGroup); // ko cần truyền parent Item
                SetDataRecyAttachFile(_lstAttachFileFull);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void SetDataRecyAttachFile(List<BeanAttachFile> _lastAttachFile)
        {
            try
            {
                if (_lastAttachFile != null && _lastAttachFile.Count >= 0)
                {
                    _adapterAttachment = new AdapterDetailCreateTask_Attachment(_mainAct, _rootView.Context, _lastAttachFile, _parentItem, _flagUserPermission, _IsClickFromAction);
                    _adapterAttachment.CustomItemClick_ViewItem += Click_ItemReyAttachment_View;
                    _adapterAttachment.CustomItemClick_SaveItem += Click_ItemReyAttachment_Save;
                    _adapterAttachment.CustomItemClick_DeleteItem += Click_ItemReyAttachment_Delete;

                    _recyAttachFile.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                    _recyAttachFile.SetAdapter(_adapterAttachment);
                    _recyAttachFile.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(60, _rootView.Context) * _lastAttachFile.Count);

                    MySwipeHelper mySwipeHelper = new AdapterDetailCreateTask_Attachment_SwipeHelper(_rootView.Context, _recyAttachFile, (int)(_mainAct.Resources.DisplayMetrics.WidthPixels * 0.15), _lastAttachFile);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataRecyAttachFile", ex);
#endif
            }
        }

        private void SetDataItemTask(BeanTask _item, List<BeanUserAndGroup> _lstAssignTo)
        {
            try
            {
                if (_item != null)
                {
                    if (!String.IsNullOrEmpty(_item.Title))
                    {
                        _tvName.Text = _item.Title;
                        _tvTieuDeContent.Text = _item.Title;
                    }
                    else
                        _tvTieuDeContent.Text = "";

                    //if (_item.DueDate.HasValue)
                    //    _tvHanHoanTatContent.Text = CTRLDetailShare.GetFormatDateLang(_item.DueDate.Value);
                    //else
                    //    _tvHanHoanTatContent.Text = "";

                    if (!String.IsNullOrEmpty(_item.Content))
                        _richEditorNoiDung.SetHtml(_item.Content);
                    else
                    {

                    }
                }

                if (_lstAssignTo != null && _lstAssignTo.Count >= 0)
                {
                    _adapterListUserText = new AdapterSelectUserGroupMultiple_Text2(_mainAct, _rootView.Context, _lstAssignTo);
                    FlexboxLayoutManager layoutManager = new FlexboxLayoutManager(_rootView.Context);
                    layoutManager.FlexDirection = FlexDirection.Row;
                    layoutManager.JustifyContent = JustifyContent.FlexStart;

                    _adapterListUserText.CustomItemClick += (sender, e) =>
                    {
                        _lstAssignTo = _lstAssignTo.Where(x => x.ID != e.ID).ToList();
                        _adapterListUserText.UpdateItemListIsClicked(_lstAssignTo);
                        _adapterListUserText.NotifyDataSetChanged();

                        _lstCurrentUserGroup = _lstAssignTo.ToList();
                    };
                    _recyNguoiXuLy.SetAdapter(_adapterListUserText);
                    _recyNguoiXuLy.SetLayoutManager(layoutManager);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataItem", ex);
#endif
            }
        }
        #endregion
    }
}