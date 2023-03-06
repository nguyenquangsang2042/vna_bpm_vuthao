using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
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
using BPMOPMobile.Droid.Presenter.SharedView;
using Com.Google.Android.Flexbox;
using Jp.Wasabeef;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using static BPMOPMobile.Droid.Class.MinionAction;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;
using static BPMOPMobile.Droid.Presenter.Adapter.AdapterDetailCreateTask_Attachment;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentDetailCreateTask : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private LinearLayout _lnAll, _lnChiTiet, _lnCongViecCon, _lnChiTietContent, _lnCongViecConContent, _lnSubToolbar, _lnTieuDeContent, _lnNguoiGiao, _lnNguoiXuLyContent, _lnHanHoanTatContent, _lnTinhTrangContent, _lnNoiDungContent, _lnNoiDungContentClick, _lnAttachFile, _lnTaoMoi, _lnKeyBoard, _lnComment, _lnData, _lnNoData;
        private View _rootView, _vwChiTiet, _vwCongViecCon, _viewBlurRecycleUseChoose;
        private ImageView _imgNguoiXuLy, _imgHanHoanTat, _imgTitleName, _imgCreateChildTask, _imgDoneTask, _imgDeleteTask;
        private TextView _tvName, _tvChiTiet, _tvCongViecCon, _tvAttachFile, _tvAttachFileTaoMoi, _tvAttachFileChild1, _tvAttachFileChild2, _tvTieuDe, _tvNguoiXuLy, _tvHanHoanTat, _tvTinhTrang, _tvNoiDung,
            _tvTieuDeContent, _tvNguoiXuLyContent, _tvHanHoanTatContent, _tvTinhTrangContent, _tvNguoiGiao, _tvNguoiGiaoContent, _tvNoData;
        private RichEditor _richEditorNoiDung;
        private Dialog _dialogPopupControl, _dialogPopupControl_disable;
        private ImageView _imgBack, _imgSave, _imgUser;
        private BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        private BeanNotify _notifyItem = new BeanNotify();
        private CustomFlexBoxRecyclerView _recyNguoiXuLy;
        private RecyclerView _recyAttachFile;
        private AdapterSelectUserGroupMultiple_Text2 _adapterListUserText;
        private ControllerDetailShare CTRLDetailShare = new ControllerDetailShare();
        private ControllerDetailCreateTask CTRLDetailCreateTask = new ControllerDetailCreateTask();
        private ControllerComment CTRLComment = new ControllerComment();
        public CustomBaseFragment _previousFragment;

        private AdapterDetailCreateTask_Attachment _adapterAttachment;
        private MySwipeHelper swipeHelperAttachment;

        // DATA FORM DETAIL
        private int _taskID = -1;
        public JObject _OBJTASKFORM;
        private BeanTask _parentItem;
        private List<BeanUserAndGroup> _lstCurrentUserGroup = new List<BeanUserAndGroup>();
        private List<BeanAttachFile> _lstAttachFileFull = new List<BeanAttachFile>();
        private List<BeanAttachFile> _lstAttFileControl_Deleted = new List<BeanAttachFile>(); // lưu lại những item nào đã bị xóa ra khỏi Control inputattachmenthorizon
        private List<BeanComment> _lstComment = new List<BeanComment>();
        private List<BeanTask> _lstChildTask = new List<BeanTask>();
        private int _flagUserPermission = (int)ControllerDetailCreateTask.FlagUserPermission.Viewer;
        private bool _IsClickFromAction = false; // Check xem là Click từ Action hay Control

        string[] _lstFragmentCheckPermission = new string[] { typeof(FragmentHomePage).Name, typeof(FragmentSingleListVDT).Name, typeof(FragmentSingleListVTBD).Name };

        public int _currentFlagPage = 0; // 0 la chi tiet, 1 la cong viec con

        private string _OtherResourceId = "";
        private DateTime? _CommentChanged;
        // Comment
        private List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private ComponentComment componentComment;
        public Java.IO.File _tempfileFromCamera;

        public FragmentDetailCreateTask() { /* Prevent Darkmode */ }

        public FragmentDetailCreateTask(CustomBaseFragment _previousFragment, int _taskID, bool _IsClickFromAction, BeanWorkflowItem _workflowItem = null)
        {
            this._previousFragment = _previousFragment;
            this._taskID = _taskID;
            this._IsClickFromAction = _IsClickFromAction;

            if (_workflowItem == null)
            {
                if (_previousFragment is FragmentDetailWorkflow)
                {
                    this._workflowItem = ((FragmentDetailWorkflow)_previousFragment)._workflowItem;
                    this._notifyItem = ((FragmentDetailWorkflow)_previousFragment)._notifyItem;
                }
            }
            else
            {
                this._workflowItem = _workflowItem;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct.Window.SetSoftInputMode(SoftInput.AdjustPan);
            //if (_previousFragment is FragmentDetailWorkflow) // Chưa có event -> Add
            //    MinionActionCore.TaskListItemClickEvent -= Click_TaskListItem; // Click vào Item của List Task
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
                _imgDeleteTask = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_DeleteTask);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_Name);
                _imgTitleName = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_Name);
                _lnData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_Data);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NoData);
                _lnChiTiet = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_ChiTiet);
                _lnCongViecCon = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_CongViecCon);
                _lnChiTietContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_ChiTiet_Content);
                _lnCongViecConContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_CongViecCon_Content);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NoData);
                _tvChiTiet = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_ChiTiet);
                _tvCongViecCon = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_CongViecCon);
                _vwChiTiet = _rootView.FindViewById<View>(Resource.Id.vw_ViewDetailCreateTask_ChiTiet);
                _vwCongViecCon = _rootView.FindViewById<View>(Resource.Id.vw_ViewDetailCreateTask_CongViecCon);
                // -- TieuDe --
                _tvTieuDe = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TieuDe);
                _lnTieuDeContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_TieuDe_Content);
                _tvTieuDeContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TieuDe_Content);
                // -- NguoiXuLy --
                _tvNguoiXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NguoiXuLy);
                _lnNguoiXuLyContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NguoiXuLy_Content);
                _viewBlurRecycleUseChoose = _rootView.FindViewById<View>(Resource.Id.viewBlurRecycleUseChoose);
                _recyNguoiXuLy = _rootView.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_ViewDetailCreateTask_NguoiXuLy);
                _imgNguoiXuLy = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_NguoiXuLy);
                // -- HanHoanTat --
                _tvHanHoanTat = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_HanHoanTat);
                _imgHanHoanTat = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailCreateTask_HanHoanTat_Content);
                _lnHanHoanTatContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_HanHoanTat_Content);
                _tvHanHoanTatContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_HanHoanTat_Content);
                // -- TinhTrang --
                _tvTinhTrang = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TinhTrang);
                _lnTinhTrangContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_TinhTrang_Content);
                _tvTinhTrangContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_TinhTrang_Content);
                // -- NguoiGiao --
                _lnNguoiGiao = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NguoiGiao);
                _tvNguoiGiao = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NguoiGiao);
                _tvNguoiGiaoContent = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NguoiGiao_Content);
                // -- NoiDung --
                _tvNoiDung = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_NoiDung);
                _lnNoiDungContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NoiDung_Content);
                _lnNoiDungContentClick = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_NoiDung_ContentClick); // Để che lên richeditor click dc
                _richEditorNoiDung = _rootView.FindViewById<RichEditor>(Resource.Id.richEditor_ViewDetailCreateTask_NoiDung_Content);
                // -- DinhKem --
                _recyAttachFile = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewDetailCreateTask_AttachFile);
                _lnAttachFile = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_AttachFile);
                _lnTaoMoi = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_AttachFile_TaoMoi);
                _tvAttachFile = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile);
                _tvAttachFileTaoMoi = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_TaoMoi);
                _tvAttachFileChild1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_Child1);
                _tvAttachFileChild2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailCreateTask_AttachFile_Child2);
                //Comment
                _lnComment = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_Comment);
                //KeyBoard
                _lnKeyBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailCreateTask_KeyBoard);

                _lnChiTiet.Click += Click_lnChiTiet;

                _lnTieuDeContent.Click += Click_lnTieuDeContent;
                _lnNguoiXuLyContent.Click += Click_lnNguoiXuLyContent;
                _viewBlurRecycleUseChoose.Click += Click_lnNguoiXuLyContent;
                _lnHanHoanTatContent.Click += Click_lnHanHoanTatContent;
                _lnTinhTrangContent.Click += Click_lnTinhTrangContent;
                _tvNoiDung.Click += Click_lnNoiDungContent;
                _lnNoiDungContent.Click += Click_lnNoiDungContent;
                _lnNoiDungContentClick.Click += Click_lnNoiDungContent;
                _lnTaoMoi.Click += Click_lnTaoMoi;

                _richEditorNoiDung.Enabled = false;
                _imgBack.Click += Click_imgBack;
                _imgSave.Click += Click_imgSave;
                _imgCreateChildTask.Click += Click_imgChildTask;
                _imgDoneTask.Click += Click_imgDoneTask;
                _imgDeleteTask.Click += Click_imgDeleteTask;
                _lnAll.Click += (sender, e) => { };

                _lnNguoiGiao.Visibility = ViewStates.Gone;

            }
            SetViewByLanguage();
            SetData();

            //if (_previousFragment is FragmentDetailWorkflow) // Chưa có event -> Add
            //    MinionActionCore.TaskListItemClickEvent += Click_TaskListItem; // Click vào Item của List Task
            CmmDroidFunction.HideSoftKeyBoard(_mainAct);
            return _rootView;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if ((requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment || requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment_Camera) && resultCode == (int)Result.Ok) // đính kèm comment
                {
                    // Comment
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment) // chọn file thường
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
                    //SetDataRecyAttachFile(_lstAttachFileFull);
                }
                else if ((requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileComment || requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileComment_Camera) && resultCode == (int)Result.Ok) // đính kèm comment
                {
                    // Comment
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_DetailCreateTask_ChooseFileComment) // chọn file thường
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

                    if (CTRLDetailCreateTask.CheckFileExistInList(_lstAttachComment, _beanAttachFile) == true)
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                           CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                        return;
                    }


                    if (componentComment != null)
                    {
                        _lstAttachComment.Add(_beanAttachFile);
                        componentComment.UpdateListParentAttach(this._lstAttachComment);
                    }


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
                _tvName.Text = CmmFunction.GetTitle("TEXT_ASSIGNTASK", "Phân công công việc");
                _tvChiTiet.Text = CmmFunction.GetTitle("TEXT_DETAIL", "Chi tiết");
                _tvCongViecCon.Text = CmmFunction.GetTitle("TEXT_CHILDTASK", "Công việc con");
                _tvTieuDe.Text = CmmFunction.GetTitle("TEXT_TITLE_REQUIRE", "Tiêu đề (*)");
                _tvNguoiXuLy.Text = CmmFunction.GetTitle("TEXT_USER_PROCESS_REQUIRE", "Người xử lý (*)");
                _tvHanHoanTat.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn hoàn tất");
                _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                _tvNoiDung.Text = CmmFunction.GetTitle("TEXT_CONTENT", "Nội dung");
                _tvAttachFile.Text = CmmFunction.GetTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm");
                _tvAttachFileTaoMoi.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới");
                _tvAttachFileChild1.Text = CmmFunction.GetTitle("TEXT_CONTROL_DOCUMENTNAME", "Tên tài liệu");
                _tvAttachFileChild2.Text = CmmFunction.GetTitle("TEXT_ASSIGNORS", "Người giao");
                _tvNguoiGiao.Text = CmmFunction.GetTitle("TEXT_CREATOR", "Người tạo");
                _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");

                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvTieuDe);
                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvNguoiXuLy);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - SetView - Error: " + ex.Message);
#endif
            }
        }

        private void SetViewByPermission()
        {
            try
            {
                if (_parentItem != null)
                    _lnNguoiGiao.Visibility = ViewStates.Visible;
                else
                    _lnNguoiGiao.Visibility = ViewStates.Gone;

                if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Viewer) // Người xem: chỉ dc xem và ko dc thao tác gì thêm
                {
                    #region Permission - Viewer - Chỉ được xem, không được thao tác gì thêm
                    _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASK", "Công việc") : CmmFunction.GetTitle("TEXT_TASK", "Task");

                    // Toolbar
                    _imgTitleName.Visibility = ViewStates.Gone;
                    _imgSave.Visibility = ViewStates.Invisible;
                    _imgDoneTask.Visibility = ViewStates.Gone;
                    _imgCreateChildTask.Visibility = ViewStates.Gone;
                    _imgDeleteTask.Visibility = ViewStates.Gone;

                    _lnSubToolbar.Visibility = ViewStates.Visible;
                    _lnSubToolbar.Enabled = false;
                    _lnSubToolbar.SetBackgroundColor(Color.Transparent);

                    // Được xem tình trạng - ko dc Edit
                    _tvTinhTrang.Visibility = ViewStates.Visible;
                    _lnTinhTrangContent.Visibility = ViewStates.Visible;

                    // Tiêu đề
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTieuDeContent, _tvTieuDeContent);

                    // Người xử lý
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNguoiXuLyContent, null);
                    _recyNguoiXuLy.SetPadding(0, 0, 0, 0);
                    _imgNguoiXuLy.Visibility = ViewStates.Gone;

                    // Hạn hoàn tất
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnHanHoanTatContent, _tvHanHoanTatContent);
                    _imgHanHoanTat.Visibility = ViewStates.Gone;

                    // Tình trạng
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTinhTrangContent, _tvTinhTrangContent);

                    // Nội dung
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNoiDungContent, null);
                    _richEditorNoiDung.SetBackgroundColor(Color.Transparent);

                    // Đính kèm -> ko dc đính kèm, chỉ dc down
                    _lnTaoMoi.Visibility = ViewStates.Invisible;

                    // Bỏ hết (*) của TextView
                    removeIsRequiredAllTextView();
                    #endregion
                }
                else if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Handler)
                {
                    _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASK", "Công việc") : CmmFunction.GetTitle("TEXT_TASK", "Task");

                    #region Permission - Handler - được chỉnh tình trạng, file đính kèm, tạo Task con

                    _imgTitleName.Visibility = ViewStates.Gone;

                    // Cho chỉnh sửa tình trạng
                    _tvTinhTrang.Visibility = ViewStates.Visible;
                    _lnTinhTrangContent.Visibility = ViewStates.Visible;
                    _lnSubToolbar.Visibility = ViewStates.Visible;

                    // Tiêu đề
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTieuDeContent, _tvTieuDeContent);

                    // Người xử lý
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNguoiXuLyContent, null);
                    _recyNguoiXuLy.SetPadding(0, 0, 0, 0);
                    _imgNguoiXuLy.Visibility = ViewStates.Gone;

                    // Hạn hoàn tất
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnHanHoanTatContent, _tvHanHoanTatContent);
                    _imgHanHoanTat.Visibility = ViewStates.Gone;

                    // Nội dung
                    CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNoiDungContent, null);
                    _richEditorNoiDung.SetBackgroundColor(Color.Transparent);

                    // Đính kèm -> đủ quyền
                    _lnTaoMoi.Visibility = ViewStates.Visible;

                    // Bỏ hết (*) của TextView
                    removeIsRequiredAllTextView();
                    #endregion

                    // do xài relative nên chỉnh lại theo kiểu này chứ ko xài Voisibility
                    _imgDoneTask.Visibility = ViewStates.Visible;
                    _imgSave.Visibility = ViewStates.Visible;
                    _imgCreateChildTask.Visibility = ViewStates.Visible;
                    _imgDeleteTask.Visibility = ViewStates.Gone;

                    if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Completed)
                    {
                        #region Check Status "Hoàn tất" - không cho Update + Hoàn tất + Save

                        RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                        _params.AddRule(LayoutRules.AlignParentRight);

                        // do xài relative nên chỉnh lại theo kiểu này chứ ko xài Voisibility
                        _imgDoneTask.Visibility = ViewStates.Gone;
                        _imgSave.Visibility = ViewStates.Invisible;
                        _imgSave.LayoutParameters = _params;

                        // Tình trạng ko dc Edit
                        CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTinhTrangContent, _tvTinhTrangContent);

                        // Không được thêm mới file
                        _lnTaoMoi.Visibility = ViewStates.Invisible;

                        #endregion
                    }
                    else if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Hold)
                    {
                        #region Check status "Tạm hoãn" - không được tạo Task con, đính kèm

                        _imgCreateChildTask.Visibility = ViewStates.Gone;

                        _lnTaoMoi.Visibility = ViewStates.Invisible;

                        #endregion
                    }
                }
                else if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Creator)
                {
                    #region Permission - Creator
                    if (_IsClickFromAction == true) // Click từ Action vào -> tạo mới: ẩn tình trạng đi
                    {
                        _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_ASSIGNTASK", "Phân công công việc") : CmmFunction.GetTitle("TEXT_ASSIGNTASK", "Asign task");

                        #region Tạo mới Task

                        _imgSave.Visibility = ViewStates.Visible;
                        _imgDoneTask.Visibility = ViewStates.Gone;
                        _imgCreateChildTask.Visibility = ViewStates.Gone;
                        _lnSubToolbar.Visibility = ViewStates.Gone;

                        _tvTinhTrang.Visibility = ViewStates.Invisible;
                        _lnTinhTrangContent.Visibility = ViewStates.Invisible;
                        #endregion
                    }
                    else // Click từ Task vào -> Update lại Task đã có
                    {
                        _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASK", "Công việc") : CmmFunction.GetTitle("TEXT_TASK", "Task");

                        if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Hold ||
                            (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Completed))
                        {
                            // check xem nếu phiếu Hold thì ko cho làm gì -> giống viewer
                            // check xem nếu phiếu Done -> chỉ dc tạo Task con

                            if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Completed)
                                _imgCreateChildTask.Visibility = ViewStates.Visible;
                            else
                                _imgCreateChildTask.Visibility = ViewStates.Gone;

                            #region Permission - Viewer - Chỉ được xem, không được thao tác gì thêm

                            // Toolbar
                            _imgTitleName.Visibility = ViewStates.Gone;

                            // do xài relative nên chỉnh lại theo kiểu này chứ ko xài Visibility
                            RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                            _params.AddRule(LayoutRules.AlignParentRight);
                            _imgSave.LayoutParameters = _params;

                            _imgDoneTask.Visibility = ViewStates.Gone;
                            ////_imgCreateChildTask.Visibility = ViewStates.Gone;
                            _imgDeleteTask.Visibility = ViewStates.Gone;

                            _lnSubToolbar.Visibility = ViewStates.Visible;
                            _lnSubToolbar.Enabled = false;
                            _lnSubToolbar.SetBackgroundColor(Color.Transparent);

                            // Được xem tình trạng - ko dc Edit
                            _tvTinhTrang.Visibility = ViewStates.Visible;
                            _lnTinhTrangContent.Visibility = ViewStates.Visible;

                            // Tiêu đề
                            CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTieuDeContent, _tvTieuDeContent);

                            // Người xử lý
                            CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNguoiXuLyContent, null);
                            _recyNguoiXuLy.SetPadding(0, 0, 0, 0);
                            _imgNguoiXuLy.Visibility = ViewStates.Gone;

                            // Hạn hoàn tất
                            CTRLDetailCreateTask.SetViewControl_NotEdited(_lnHanHoanTatContent, _tvHanHoanTatContent);
                            _imgHanHoanTat.Visibility = ViewStates.Gone;

                            // Tình trạng
                            CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTinhTrangContent, _tvTinhTrangContent);

                            // Nội dung
                            CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNoiDungContent, null);
                            _richEditorNoiDung.SetBackgroundColor(Color.Transparent);

                            // Đính kèm -> ko dc đính kèm, chỉ dc down
                            _lnTaoMoi.Visibility = ViewStates.Invisible;

                            // Bỏ hết (*) của TextView
                            removeIsRequiredAllTextView();
                            #endregion
                        }
                        else
                        {
                            if (_parentItem != null && _parentItem.Status != 2) // trạng thái khác hoàn tất và là ng tạo -> dc xóa
                                _imgDeleteTask.Visibility = ViewStates.Visible; // Là người tạo và xem lại phiếu -> được quyền xóa phiếu

                            _lnSubToolbar.Visibility = ViewStates.Visible;

                            /*// Không được chọn tình trạng
                            _tvTinhTrang.Visibility = ViewStates.Invisible;
                            _lnTinhTrangContent.Visibility = ViewStates.Invisible;*/
                            _tvTinhTrang.Visibility = ViewStates.Visible;
                            _lnTinhTrangContent.Visibility = ViewStates.Visible;
                            _lnTinhTrangContent.Enabled = false;
                            // Cácc button được hiện: DeleteTask, TaskChild, Save
                            _imgSave.Visibility = ViewStates.Visible;
                            _imgDoneTask.Visibility = ViewStates.Gone;
                            _imgCreateChildTask.Visibility = ViewStates.Visible;
                            _imgDeleteTask.Visibility = ViewStates.Visible;
                        }
                    }
                    #endregion
                }
                else if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.CreatorAndHandler)
                {
                    _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASK", "Công việc") : CmmFunction.GetTitle("TEXT_TASK", "Task");

                    if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Completed)
                    {
                        #region Check Status "Hoàn tất" - đã hoàn tất thì ko dc làm gì
                        _imgTitleName.Visibility = ViewStates.Gone;
                        // Cho chỉnh sửa tình trạng
                        _tvTinhTrang.Visibility = ViewStates.Visible;
                        _lnTinhTrangContent.Visibility = ViewStates.Visible;
                        _lnSubToolbar.Visibility = ViewStates.Visible;

                        // Tiêu đề
                        _lnTieuDeContent.Enabled = false;
                        _lnTieuDeContent.SetBackgroundColor(Color.Transparent);
                        _tvTieuDeContent.SetPadding(0, 0, 0, 0);

                        // Người xử lý
                        _lnNguoiXuLyContent.Enabled = false;
                        _lnNguoiXuLyContent.SetBackgroundColor(Color.Transparent);
                        _recyNguoiXuLy.SetPadding(0, 0, 0, 0);
                        _imgNguoiXuLy.Visibility = ViewStates.Gone;

                        // Hạn hoàn tất
                        _lnHanHoanTatContent.Enabled = false;
                        _lnHanHoanTatContent.SetBackgroundColor(Color.Transparent);
                        _tvHanHoanTatContent.SetPadding(0, 0, 0, 0);
                        _imgHanHoanTat.Visibility = ViewStates.Gone;

                        // Nội dung
                        _lnNoiDungContent.Enabled = false;
                        _lnNoiDungContent.SetBackgroundColor(Color.Transparent);
                        _richEditorNoiDung.SetBackgroundColor(Color.Transparent);
                        _tvHanHoanTatContent.SetPadding(0, 0, 0, 0);

                        // Đính kèm -> đủ quyền
                        _lnTaoMoi.Visibility = ViewStates.Visible;

                        // Bỏ hết (*) của TextView
                        removeIsRequiredAllTextView();

                        RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                        _params.AddRule(LayoutRules.AlignParentRight);

                        // do xài relative nên chỉnh lại theo kiểu này chứ ko xài Voisibility
                        _imgDoneTask.Visibility = ViewStates.Gone;
                        _imgSave.Visibility = ViewStates.Invisible;
                        _imgSave.LayoutParameters = _params;

                        // Tình trạng ko dc Edit
                        _lnTinhTrangContent.Enabled = false;
                        _lnTinhTrangContent.SetBackgroundColor(Color.Transparent);
                        _tvTinhTrangContent.SetPadding(0, 0, 0, 0);

                        _lnTaoMoi.Visibility = ViewStates.Invisible;
                        #endregion
                    }
                    else if (_parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Hold)
                    {
                        // Người xử lý đồng thời là người tạo và phiếu đang tạm hoãn -> quyền giống người xử lý

                        #region Permission - Handler - được chỉnh tình trạng, file đính kèm, tạo Task con

                        _imgTitleName.Visibility = ViewStates.Gone;

                        // Cho chỉnh sửa tình trạng
                        _tvTinhTrang.Visibility = ViewStates.Visible;
                        _lnTinhTrangContent.Visibility = ViewStates.Visible;
                        _lnSubToolbar.Visibility = ViewStates.Visible;

                        // Tiêu đề
                        CTRLDetailCreateTask.SetViewControl_NotEdited(_lnTieuDeContent, _tvTieuDeContent);

                        // Người xử lý
                        CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNguoiXuLyContent, null);
                        _recyNguoiXuLy.SetPadding(0, 0, 0, 0);
                        _imgNguoiXuLy.Visibility = ViewStates.Gone;

                        // Hạn hoàn tất
                        CTRLDetailCreateTask.SetViewControl_NotEdited(_lnHanHoanTatContent, _tvHanHoanTatContent);
                        _imgHanHoanTat.Visibility = ViewStates.Gone;

                        // Tình trạng
                        CTRLDetailCreateTask.SetViewControl_Edited(_lnTinhTrangContent, _tvTinhTrangContent);

                        // Nội dung
                        CTRLDetailCreateTask.SetViewControl_NotEdited(_lnNoiDungContent, _tvTinhTrangContent);
                        _richEditorNoiDung.SetBackgroundColor(Color.Transparent);

                        // Đính kèm -> đủ quyền
                        _lnTaoMoi.Visibility = ViewStates.Visible;

                        // Bỏ hết (*) của TextView
                        removeIsRequiredAllTextView();
                        #endregion

                        #region Check status "Tạm hoãn" - không được tạo Task con, không dc cập nhật file

                        _lnTaoMoi.Visibility = ViewStates.Invisible;

                        _imgCreateChildTask.Visibility = ViewStates.Gone;

                        #endregion
                    }
                    else // Các tình trạng còn lại -> dc làm tất cả
                    {
                        #region Permission - Creator + Handler - All quyền
                        // Cho chỉnh sửa tình trạng
                        _tvTinhTrang.Visibility = ViewStates.Visible;
                        _lnTinhTrangContent.Visibility = ViewStates.Visible;

                        _imgSave.Visibility = ViewStates.Visible;
                        _imgDoneTask.Visibility = ViewStates.Visible;
                        _imgCreateChildTask.Visibility = ViewStates.Visible;

                        _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? CmmFunction.GetTitle("TEXT_TASK", "Công việc") : CmmFunction.GetTitle("TEXT_TASK", "Task");
                        _lnSubToolbar.Visibility = ViewStates.Visible;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByPermission", ex);
#endif
            }
        }

        private void Click_lnChiTiet(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _currentFlagPage = 0;

                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvCongViecCon, _tvCongViecCon.Text, "(", ")");

                    CTRLDetailCreateTask.SetToolbarItem_Selected(_mainAct, _tvChiTiet, _vwChiTiet);
                    CTRLDetailCreateTask.SetToolbarItem_NotSelected(_mainAct, _tvCongViecCon, _vwCongViecCon);

                    _lnChiTietContent.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                    _lnChiTietContent.Visibility = ViewStates.Visible;
                    _lnCongViecConContent.Visibility = ViewStates.Gone;

                    //ISpannable spannable = new SpannableString(_tvCongViecCon.Text.Trim());
                    //ColorStateList White = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)) });
                    //TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, White, null);
                    //spannable.SetSpan(highlightSpan, 0, _tvCongViecCon.Text.Length - 1, SpanTypes.ExclusiveExclusive);
                    //_tvCongViecCon.SetText(spannable, TextView.BufferType.Spannable);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnChiTiet", ex);
#endif
            }
        }

        public void Click_lnCongViecCon(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _currentFlagPage = 1;

                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvCongViecCon, _tvCongViecCon.Text, "(", ")");

                    CTRLDetailCreateTask.SetToolbarItem_NotSelected(_mainAct, _tvChiTiet, _vwChiTiet);
                    CTRLDetailCreateTask.SetToolbarItem_Selected(_mainAct, _tvCongViecCon, _vwCongViecCon);

                    _lnCongViecConContent.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                    _lnChiTietContent.Visibility = ViewStates.Gone;
                    _lnCongViecConContent.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnCongViecCon", ex);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(1000))
                    _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private void Click_imgBack_WithRefreshPage(object sender, EventArgs e)
        {
            try
            {
                RenewPreviousFragment();
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
                if (CmmDroidFunction.PreventMultipleClick() == false)
                {
                    return;
                }
                #region Validate Data
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
                #endregion

                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                await Task.Run(async () =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    bool _result = false;

                    #region Handle List Attach File
                    List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
                    if (_lstAttachFileFull != null && _lstAttachFileFull.Count > 0) // Lấy những file thêm mới từ App ra
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

                    // Xử lý file bị xóa đi
                    List<ObjectSubmitAction> _lstSubmitActionData = new List<ObjectSubmitAction>();

                    if (_lstAttFileControl_Deleted != null && _lstAttFileControl_Deleted.Count > 0) // Nếu có xóa mới có list này
                    {
                        // Loại bỏ những item Local ra
                        //_lstAttFileControl_Deleted = _lstAttFileControl_Deleted.Where(x => !x.ID.Equals("")).ToList();

                        ObjectSubmitAction _beanSubmitDeleted = new ObjectSubmitAction() // Object của những Attach File đã bị xóa khỏi List.
                        {
                            ID = "",
                            Value = JsonConvert.SerializeObject(_lstAttFileControl_Deleted),
                            TypeSP = "RemoveAttachment",
                            DataType = ""
                        };
                        _lstSubmitActionData.Add(_beanSubmitDeleted);
                    }


                    #endregion

                    if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Creator) // Người tạo -> tạo mới hoặc Update Item cũ
                    {
                        if (_IsClickFromAction == true) // Click từ Action -> Tạo mới Item
                        {
                            if (!String.IsNullOrEmpty(_tvHanHoanTatContent.Text))
                            {
                                DateTime _validateItem = DateTime.Now;
                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                    _validateItem = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                                else
                                    _validateItem = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);

                                if (DateTime.Compare(_validateItem, DateTime.Now) < 0)
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

                            #region Handle Bean Task
                            BeanTask _itemTask = new BeanTask();
                            _itemTask.ID = 0;
                            _itemTask.WorkflowId = _workflowItem.WorkflowID;
                            _itemTask.SPItemId = int.Parse(_workflowItem.ID);
                            _itemTask.Step = _workflowItem.Step.HasValue ? _workflowItem.Step.Value : -1;
                            _itemTask.Title = _tvTieuDeContent.Text;

                            if (!String.IsNullOrEmpty(_tvHanHoanTatContent.Text))
                            {
                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                    _itemTask.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                                else
                                    _itemTask.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);
                            }
                            else
                            {
                                _itemTask.DueDate = null;
                            }
                            _itemTask.Parent = 0;
                            _itemTask.Content = !String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()) ? _richEditorNoiDung.GetHtml() : "";
                            _itemTask.Status = 0;
                            #endregion

                            _result = _pControlDynamic.SendCreateTaskAction(_itemTask, _lstCurrentUserGroup, _lstSubmitActionData, _lstKeyVarAttachmentLocal, (int)ControllerDetailCreateTask.FlagActionPermission.CreateNew);
                        }
                        else // Click từ Form -> Update parent Item
                        {
                            #region Handle Bean Task
                            if (!String.IsNullOrEmpty(_tvHanHoanTatContent.Text))
                            {
                                DateTime _validateItem = DateTime.Now;
                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                    _validateItem = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                                else
                                    _validateItem = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);

                                if (DateTime.Compare(_validateItem, DateTime.Now) < 0)
                                {
                                    _mainAct.RunOnUiThread(() =>
                                    {
                                        CmmDroidFunction.HideProcessingDialog();
                                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE2", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại!"),
                                                     CmmFunction.GetTitle("TEXT_DATE_COMPARE2", "Start date cannot be greater than date now!"));
                                    });
                                    return;
                                }
                                _parentItem.DueDate = _validateItem;
                            }

                            _parentItem.Title = _tvTieuDeContent.Text;
                            _parentItem.Content = !String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()) ? _richEditorNoiDung.GetHtml() : "";

                            #endregion

                            _result = _pControlDynamic.SendCreateTaskAction(_parentItem, _lstCurrentUserGroup, _lstSubmitActionData, _lstKeyVarAttachmentLocal, (int)ControllerDetailCreateTask.FlagActionPermission.CreatorUpdate);
                        }
                    }
                    else if (_flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Handler || _flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.CreatorAndHandler) // Người xử lý -> Update Item
                    {
                        #region Handle Bean Task
                        _parentItem.Title = _tvTieuDeContent.Text;
                        if (!String.IsNullOrEmpty(_tvHanHoanTatContent.Text))
                        {
                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                _parentItem.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "dd/MM/yy HH:mm", null);
                            else
                                _parentItem.DueDate = DateTime.ParseExact(_tvHanHoanTatContent.Text, "MM/dd/yy HH:mm", null);
                        }
                        _parentItem.Content = !String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()) ? _richEditorNoiDung.GetHtml() : "";
                        #endregion

                        _result = _pControlDynamic.SendCreateTaskAction(_parentItem, _lstCurrentUserGroup, _lstSubmitActionData, _lstKeyVarAttachmentLocal,
                           _flagUserPermission == (int)ControllerDetailCreateTask.FlagUserPermission.Handler ?
                           (int)ControllerDetailCreateTask.FlagActionPermission.HandlerUpdate :
                           (int)ControllerDetailCreateTask.FlagActionPermission.CreatorHandlerUpdate);

                    }

                    if (_result)
                    {

                        ProviderBase pBase = new ProviderBase();
                        await Task.Run(() =>
                        {
                            pBase.UpdateAllDynamicData(true);
                        });
                        _mainAct.RunOnUiThread(() =>
                        {
                            MinionAction.OnRefreshFragmentParent(null, null);
                            _mainAct.HideFragment();
                            CmmDroidFunction.HideProcessingDialog();

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

        private void Click_imgChildTask(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    FragmentDetailCreateTask_Child fragmentDetailCreateTask_Child = new FragmentDetailCreateTask_Child(this, _parentItem, _IsClickFromAction);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailCreateTask_Child, "FragmentDetailCreateTask_Child", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgChildTask", ex);
#endif
            }
        }

        private void Click_imgDoneTask(object sender, EventArgs e)
        {
            try
            {
                _parentItem.Status = 2; // Hoàn tất
                _tvTinhTrangContent.Text = CTRLDetailCreateTask.GetStatusNameByID(2);
                Click_imgSave(null, null); // Đã có prevent multi click
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDoneTask", ex);
#endif
            }
        }

        private void Click_imgDeleteTask(object sender, EventArgs e)
        {
            try
            {
                Action _actionPositiveButton = new Action(() =>
                {
                    SendAPIDeleteTask();
                });

                Action _actionNegativeButton = new Action(() =>
                {
                    // đã có Dispose trong hàm
                });

                CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmFunction.GetTitle("TEXT_DELETE_CONFIRM_TASK", "Bạn có chắc muốn xóa task này không?"),
                _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDoneTask", ex);
#endif
            }
        }

        private void Click_lnTieuDeContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlTextInput(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == false)
                {
                    return;
                }

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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnTieuDeContent", ex);
#endif
            }
        }

        private async void Click_lnNguoiXuLyContent(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                    {



                        List<BeanUserAndGroup> _lstUserAndGroupAll = new List<BeanUserAndGroup>();
                        List<BeanUserAndGroup> _lstSelected = _lstCurrentUserGroup.ToList();

                        #region Get View - Init Data
                        View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_ChooseUser, null);
                        LinearLayout _lnChooseUser = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_ChooseUser);
                        ImageView _imgCloseChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Close);
                        ImageView _imgDeleteChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Delete);
                        ImageView _imgAcceptChooseUser = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_ChooseUser_Accept);
                        TextView _tvTitleChooseUser = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_ChooseUser_Title);
                        EditText _edtSearchChooseUser = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_ChooseUser_Search);
                        RecyclerView _recyChooseUser = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_ChooseUser);
                        CustomFlexBoxRecyclerView _recySelectedUser = _viewPopupControl.FindViewById<CustomFlexBoxRecyclerView>(Resource.Id.recy_PopupControl_SelectedUser);

                        _recySelectedUser.Visibility = ViewStates.Visible;
                        _imgAcceptChooseUser.Visibility = ViewStates.Visible;
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
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnNguoiXuLyContent", ex);
#endif
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
                                SetDataItemTask(_parentItem, _lstCurrentUserGroup);

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
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnNguoiXuLyContent", ex);
#endif
                }
            });
        }

        private void Click_lnHanHoanTatContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlDateTime(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == false)
                {
                    return;
                }

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
                    if (_resultDate < DateTime.Now)
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE2"), CmmFunction.GetTitle("TEXT_DATE_COMPARE2"));
                    }
                    else
                    {
                        _tvHanHoanTatContent.Text = CTRLDetailShare.GetFormatDateLang(_resultDate);
                        _dialogPopupControl.Dismiss();
                    }

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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnHanHoanTatContent", ex);
#endif
            }
        }

        private void Click_lnTinhTrangContent(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false)
                {
                    return;
                }

                List<BeanLookupData> _lstLookupData = CTRLDetailCreateTask.GetListLookUpTinhTrang().ToList();
                for (int i = 0; i < _lstLookupData.Count; i++)
                {
                    if (_lstLookupData[i].Title.Contains(_tvTinhTrangContent.Text))
                    {
                        _lstLookupData[i].IsSelected = true;
                        break; // vì chỉ có 1 thằng dc chọn nên break luôn
                    }
                }

                #region Get View - Init Data

                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                _imgDone.Visibility = ViewStates.Invisible;

                if (!String.IsNullOrEmpty(_tvTinhTrangContent.Text))
                {
                    _tvTitle.Text = _tvTinhTrangContent.Text;
                }

                AdapterFormControlSingleChoice _adapterFormControlSingleChoice = new AdapterFormControlSingleChoice(_mainAct, _rootView.Context, _lstLookupData);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterFormControlSingleChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);
                _adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                {
                    BeanLookupData _selectedLookupItem = e;
                    if (_selectedLookupItem != null)
                    {
                        try
                        {
                            _parentItem.Status = int.Parse(_selectedLookupItem.ID);
                            _tvTinhTrangContent.Text = _selectedLookupItem.Title;
                        }
                        catch (Exception)
                        {

                        }
                        _dialogPopupControl.Dismiss();
                    }
                };

                #endregion

                #region Event
                _imgClose.Click += (sender, e) =>
                {
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
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void Click_lnNoiDungContent(object sender, EventArgs e)
        {
            try // giống hàm private void ShowPopup_ControlTextInputFormat(ViewElement clickedElement)
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (_lnNoiDungContent.Enabled == true)
                    {

                        bool isChanged = true;
                        #region Get View - Init Data
                        View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputTextFormat, null);
                        LinearLayout _lnContentClick = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_InputTextFormat_ContentClick);
                        ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Close);
                        TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputTextFormat_Title);
                        ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Done);
                        ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Delete);
                        RichEditor mEditor = _viewPopupControl.FindViewById<RichEditor>(Resource.Id.editor_PopupControl_InputTextFormat);
                        HorizontalScrollView _scrollAction = _viewPopupControl.FindViewById<HorizontalScrollView>(Resource.Id.scroll_PopupControl_InputTextFormat_Action);


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

                        //if (_lnNoiDungContent.Enabled == false)// Không cho edit
                        //{
                        //    _lnContent.Enabled = false;
                        //    _imgDone.Visibility = ViewStates.Invisible;
                        //    _scrollAction.Visibility = ViewStates.Invisible;
                        //}

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
                    else
                    {
                        View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputTextFormat_inDisableEdit, null);
                        TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputTextFormat_Title);
                        TextView _txtDetailInDisable = _viewPopupControl.FindViewById<TextView>(Resource.Id.txtDetailInDisable);
                        ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Close);

                        if (!String.IsNullOrEmpty(_tvNoiDung.Text))
                        {
                            _tvTitle.Text = _tvNoiDung.Text;
                        }
                        else
                        {
                            _tvTitle.Text = "";
                        }
                        _imgBack.Click += (sender, e) =>
                        {
                            _dialogPopupControl_disable.Dismiss();
                        };
                        _txtDetailInDisable.Text = Html.FromHtml(_richEditorNoiDung.GetHtml()).ToString();
                        _dialogPopupControl_disable = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                        Window window = _dialogPopupControl_disable.Window;
                        _dialogPopupControl_disable.RequestWindowFeature(1);
                        _dialogPopupControl_disable.SetCanceledOnTouchOutside(false);
                        _dialogPopupControl_disable.SetCancelable(true);
                        window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                        window.SetGravity(GravityFlags.Bottom);
                        var dm = _mainAct.Resources.DisplayMetrics;

                        _dialogPopupControl_disable.SetContentView(_viewPopupControl);
                        _dialogPopupControl_disable.Show();
                        WindowManagerLayoutParams s = window.Attributes;
                        s.Width = WindowManagerLayoutParams.MatchParent;
                        s.Height = WindowManagerLayoutParams.MatchParent;
                        window.Attributes = s;
                    }
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
                if (CmmDroidFunction.PreventMultipleClick() == false)
                {
                    return;
                }

                SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentDetailCreateTask", _rootView,
                    CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment,
                    CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment_Camera,
                    (int)SharedView_PopupChooseFile.FlagView.DetailCreateTask_ControlInputAttachmentVertical);
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

                //#region Event
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
                //    StartActivityForResult(i, CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment);

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
                //    StartActivityForResult(i, CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment);

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
                //            StartActivityForResult(intent, CmmDroidVariable.M_DetailCreateTask_ChooseFileControlAttachment_Camera);

                //            _dialogAction.Dismiss();
                //        }
                //    }
                //    catch (Exception ex) { }
                //};

                //#endregion

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlInputAttachmentVertical", ex);
#endif
            }
        }

        private void Click_ItemRecyAttachment_View(object sender, BeanAttachFile e)
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

        private void Click_ItemRecyAttachment_Save(object sender, BeanAttachFile e)
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemReyAttachment_Save", ex);
#endif
            }
        }

        private void Click_ItemRecyAttachment_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (e.CreatedBy.ToLowerInvariant().Equals(CmmVariable.SysConfig.UserId.ToLowerInvariant()))
                {
                    Action _actionPositiveButton = new Action(() =>
                    {
                        if (!e.ID.Equals("")) // file mới thêm local thì không cần add vào list Remove
                        {
                            _lstAttFileControl_Deleted.Add(e);
                        }

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
                else
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                      CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemReyAttachment_Delete", ex);
#endif
            }
        }

        private async void Click_ItemListComment_Like(object sender, BeanComment e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                    await Task.Run(() =>
                    {
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                        bool _result = _pControlDynamic.SetLikeComment(e.ID, !e.IsLiked);

                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_result == true)
                            {
                                #region Update View Value
                                e.IsLiked = !e.IsLiked;
                                if (e.IsLiked == true)
                                    e.LikeCount = e.LikeCount + 1;
                                else
                                    e.LikeCount = e.LikeCount - 1 < 0 ? 0 : e.LikeCount - 1; // nếu <0 thì gán = 0
                                #endregion

                                CTRLComment.UpdateIsLikedComment(_OtherResourceId, e.IsLiked, e.LikeCount); // Update Sqlite

                                SetDataComment();

                                CmmDroidFunction.HideProcessingDialog();
                            }
                            else
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                                  CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }

        private void Click_ItemListComment_Reply(object sender, BeanComment e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    FragmentReplyComment _fragmentReplyComment = new FragmentReplyComment(this, this.GetType().Name, e, _lstComment, _OtherResourceId, ((int)CmmFunction.CommentResourceCategoryID.Task).ToString());
                    _mainAct.AddFragment(_mainAct.SupportFragmentManager, _fragmentReplyComment, "FragmentReplyComment", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_itemExpandComment_Reply", ex);
#endif
            }
        }

        private async void Click_CommentParent_ImgComment(object sender, CommentEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                    await Task.Run(() =>
                    {
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                        BeanOtherResource beanOtherResource = new BeanOtherResource();
                        beanOtherResource.Content = e._content;
                        beanOtherResource.ResourceId = _OtherResourceId;
                        beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.Task; // Task = 16
                        beanOtherResource.ResourceSubCategoryId = 0;
                        beanOtherResource.Image = "";
                        beanOtherResource.ParentCommentId = null; // cmt mới nên ko có parent

                        List<KeyValuePair<string, string>> _lstKeyValueAttach = CTRLDetailCreateTask.CloneKeyValueFromListAtt(_lstAttachComment);

                        bool _result = _pControlDynamic.AddComment(beanOtherResource, _lstKeyValueAttach);

                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            if (_result == true)
                            {
                                _lstAttachComment = new List<BeanAttachFile>();
                                SetData();
                            }
                            else
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                                  CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SendAPI_CommentParent", ex);
#endif
            }
        }

        private void Click_CommentParent_ImgAttach(object sender, EventArgs e)
        {
            try
            {
                SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentDetailCreateTask", _rootView,
                    CmmDroidVariable.M_DetailCreateTask_ChooseFileComment,
                    CmmDroidVariable.M_DetailCreateTask_ChooseFileComment_Camera,
                    (int)SharedView_PopupChooseFile.FlagView.DetailCreateTask_Comment);
                SharedPopUpChooseFile.InitializeView();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_CommentParent_ImgAttach", ex);
#endif
            }
        }

        private void Click_ItemListAttachParent_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (!e.ID.Equals("")) // mở file từ server
                    {
                        CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + e.Url); // trường hợp đặc biệt xài URL
                    }
                    else // mở file từ local
                    {
                        if (System.IO.File.Exists(e.Path))
                        {
                            CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, e.Path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Detail", ex);
#endif
            }
        }

        private void Click_ItemListAttachParent_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (e != null)
                    {
                        _lstAttachComment = _lstAttachComment.Where(x => !x.Path.Equals(e.Path)).ToList();
                        ////if (_adapterComment != null)
                        ////{
                        ////    _adapterComment.UpdateListAttachParent(_lstAttachComment);
                        ////    _adapterComment.NotifyDataSetChanged();
                        ////}

                        if (componentComment != null)
                        {
                            componentComment.UpdateListParentAttach(_lstAttachComment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Delete", ex);
#endif
            }
        }
        #endregion

        #region Data
        public void SetData()
        {
            try
            {
                if (_IsClickFromAction == true) // trường hợp tạo mới
                {
                    _flagUserPermission = (int)ControllerDetailCreateTask.FlagUserPermission.Creator; // gán quyền tạo
                    SetDataItemTask(null, _lstCurrentUserGroup); // ko cần truyền parent Item
                    SetDataRecyAttachFile(_lstAttachFileFull);
                    SetViewByPermission(); // Ẩn hiện View theo quyền User
                }
                else // Click từ form
                {
                    if (CTRLDetailCreateTask.CheckAppHasConnection() == false) // không có mạng -> ẩn hết
                    {
                        _lnData.Visibility = ViewStates.Gone;
                        _lnNoData.Visibility = ViewStates.Visible;

                        _imgSave.Visibility = ViewStates.Invisible;
                        _imgCreateChildTask.Visibility = ViewStates.Gone;
                        _imgDoneTask.Visibility = ViewStates.Gone;
                        _imgDeleteTask.Visibility = ViewStates.Gone;
                    }
                    else // có mạng -> gọi API
                    {
                        // Gone 2 view để hàm GetDetailTaskAndSetData() xử lý.
                        _lnData.Visibility = ViewStates.Gone;
                        _lnNoData.Visibility = ViewStates.Gone;

                        _imgSave.Visibility = ViewStates.Invisible;
                        _imgCreateChildTask.Visibility = ViewStates.Gone;
                        _imgDoneTask.Visibility = ViewStates.Gone;
                        _imgDeleteTask.Visibility = ViewStates.Gone;

                        GetDetailTaskAndSetData(); // Đã gắn lại _flagUserPermission mới
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        /// <summary>
        /// Để trang create Task child khi tạo Task xong thì refresh lại trang Task parent và previous
        /// </summary>
        public void RenewCurrentPageAndPrevious(bool _IsShowDialog = true, bool _IsCheckPermission = true)
        {
            try
            {
                GetDetailTaskAndSetData(_IsShowDialog, _IsCheckPermission);
                RenewPreviousFragment(); // renew trang trc đó
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        public async void GetDetailTaskAndSetData(bool _IsShowDialog = true, bool _IsCheckPermission = true)
        {
            try
            {
                if (_IsShowDialog)
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    if (_IsCheckPermission)
                    {
                        if (_lstFragmentCheckPermission.Contains(_previousFragment.GetType().Name)) // nằm trong list cần kiểm tra Permission
                        {
                            // Kiểm tra xem có quyền trên phiếu ko
                            string _resultStringTicketRequest = _pControlDynamic.GetTicketRequestControlDynamicForm(_workflowItem, CmmVariable.SysConfig.LangCode); // List Form control

                            if (String.IsNullOrEmpty(_resultStringTicketRequest))
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _lnData.Visibility = ViewStates.Gone;
                                    _lnNoData.Visibility = ViewStates.Visible;

                                    _imgSave.Visibility = ViewStates.Invisible;
                                    _imgCreateChildTask.Visibility = ViewStates.Gone;
                                    _imgDoneTask.Visibility = ViewStates.Gone;
                                    _imgDeleteTask.Visibility = ViewStates.Gone;
                                    if (_IsShowDialog)
                                        CmmDroidFunction.HideProcessingDialog();

                                    CmmDroidFunction.ShowAlertDialogWithAction(_mainAct,
                                    CmmFunction.GetTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"),
                                    _actionPositiveButton: new Action(() => { Click_imgBack(null, null); }),
                                    _actionNegativeButton: null,
                                    _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                                    _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"),
                                    _cancelable: false);
                                });
                                return;
                            }
                        }
                    }
                    // Gọi API Task
                    string _resultString = _pControlDynamic.GetDetailTaskForm(_taskID);
                    if (String.IsNullOrEmpty(_resultString))
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            _lnData.Visibility = ViewStates.Gone;
                            _lnNoData.Visibility = ViewStates.Visible;

                            _imgSave.Visibility = ViewStates.Invisible;
                            _imgCreateChildTask.Visibility = ViewStates.Gone;
                            _imgDoneTask.Visibility = ViewStates.Gone;
                            _imgDeleteTask.Visibility = ViewStates.Gone;
                            //SetDataItemTask(null, _lstCurrentUserGroup);
                            if (_IsShowDialog)
                                CmmDroidFunction.HideProcessingDialog();
                        });
                        return;
                    }

                    _OBJTASKFORM = JObject.Parse(_resultString);

                    try { _parentItem = JsonConvert.DeserializeObject<BeanTask>(_OBJTASKFORM["parentItem"].ToString()); }
                    catch (Exception) { _parentItem = new BeanTask(); }

                    try
                    {
                        _lstCurrentUserGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(_OBJTASKFORM["assignTo"].ToString());
                        _lstCurrentUserGroup = CTRLDetailCreateTask.QueryInfoListAssign(_lstCurrentUserGroup); // Do thiếu info nên phải query lên
                    }
                    catch (Exception) { _lstCurrentUserGroup = new List<BeanUserAndGroup>(); }

                    try { _lstAttachFileFull = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_OBJTASKFORM["attachment"].ToString()); }
                    catch (Exception) { _lstAttachFileFull = new List<BeanAttachFile>(); }

                    //try { _LISTTASK = JsonConvert.DeserializeObject<List<BeanTask>>(_OBJFORMACTION["task"].ToString()); }
                    //catch (Exception ex) { _LISTTASK = new List<BeanTask>(); }

                    try { _lstChildTask = JsonConvert.DeserializeObject<List<BeanTask>>(_OBJTASKFORM["childTask"].ToString()); }
                    catch (Exception) { _lstChildTask = new List<BeanTask>(); }


                    try { _flagUserPermission = (int)_OBJTASKFORM["userPermission"]; }
                    catch (Exception) { _flagUserPermission = (int)ControllerDetailCreateTask.FlagUserPermission.Viewer; }


                    try { _CommentChanged = _parentItem.CommentChanged; }
                    catch (Exception) { _CommentChanged = null; }


                    try { _OtherResourceId = _parentItem.OtherResourceId; }
                    catch (Exception) { _OtherResourceId = ""; }

                    try // Authen View Comment -> để sau này gọi API comment
                    {
                        // tracking
                        ObjectSubmitDetailComment _objSubmitDetailComment = CTRLDetailCreateTask.InitTrackingObjectSubmitDetail(_mainAct);

                        // Comment
                        _objSubmitDetailComment.ID = _OtherResourceId; // empty or result
                        _objSubmitDetailComment.ResourceCategoryId = ((int)CmmFunction.CommentResourceCategoryID.Task).ToString();
                        _objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment((int)CmmFunction.CommentResourceCategoryID.Task), _parentItem.ID); // lấy trong beansetting
                        _objSubmitDetailComment.ItemId = _parentItem.ID.ToString();
                        _objSubmitDetailComment.Author = CmmVariable.SysConfig.UserId;
                        _objSubmitDetailComment.AuthorName = CmmVariable.SysConfig.DisplayName;

                        if (String.IsNullOrEmpty(_OtherResourceId))
                        {
                            _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                        }
                        else
                        {
                            _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                        }
                    }
                    catch (Exception)
                    {
                        //_OtherResourceId = "";
                    }

                    #region Get List Comment - chưa lưu dc SQLite do beanTask lấy live

                    string _APIdatenow = "";
                    _lstComment = _pControlDynamic.GetListComment(_OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.Task, null, ref _APIdatenow);
                    if (_lstComment != null && _lstComment.Count > 0)
                    {
                        _parentItem.CommentChanged = DateTime.Parse(_APIdatenow);
                    }

                    #endregion

                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnData.Visibility = ViewStates.Visible;
                        _lnNoData.Visibility = ViewStates.Gone;
                        SetDataItemTask(_parentItem, _lstCurrentUserGroup);
                        SetDataRecyAttachFile(_lstAttachFileFull);
                        SetDataChildTask(_lstChildTask);
                        SetDataComment();
                        SetViewByPermission(); // Ẩn hiện View theo quyền User
                        if (_IsShowDialog)
                            CmmDroidFunction.HideProcessingDialog();
                    });

                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    if (_IsShowDialog)
                        CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDetailTaskAndSetData", ex);
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
                        _tvTieuDeContent.Text = _item.Title;
                    else
                        _tvTieuDeContent.Text = "";

                    if (_item.DueDate.HasValue)
                        _tvHanHoanTatContent.Text = CTRLDetailShare.GetFormatDateLang(_item.DueDate.Value);
                    else
                        _tvHanHoanTatContent.Text = "";

                    _tvTinhTrangContent.Text = CTRLDetailCreateTask.GetStatusNameByID(_item.Status);

                    if (!String.IsNullOrEmpty(_item.Content))
                        _richEditorNoiDung.SetHtml(_item.Content);

                    if (!String.IsNullOrEmpty(_item.CreatedBy))
                    {
                        SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                        List<BeanUser> _lstUser = conn.Query<BeanUser>(String.Format("SELECT * FROM BeanUser WHERE ID = '{0}'", _item.CreatedBy.ToString()));
                        conn.Close();
                        if (_lstUser != null && _lstUser.Count > 0)
                            _tvNguoiGiaoContent.Text = _lstUser[0].FullName;
                    }
                    else
                        _tvNguoiGiaoContent.Text = "";

                }

                if (_lstAssignTo == null) // để tránh trường hợp ko có mạng
                    _lstAssignTo = new List<BeanUserAndGroup>();

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

        private void SetDataChildTask(List<BeanTask> _lstTask)
        {
            try
            {
                if (_lstTask != null && _lstTask.Count > 0) // check list child
                {
                    _lnCongViecConContent.RemoveAllViews();

                    ComponentTaskList _taskList = new ComponentTaskList(_mainAct, _lnCongViecConContent, _lstTask, true);

                    _taskList.InitializeFrameView(_lnCongViecConContent);
                    _taskList.SetTitle();
                    _taskList.SetValue();
                    _taskList.SetEnable();
                    _taskList.SetProprety();

                    _taskList.TaskListItemClickEvent += Click_TaskListItem;

                    _tvCongViecCon.Text = CmmFunction.GetTitle("TEXT_CHILDTASK", "Công việc con") + String.Format(" ({0})", _lstTask.Where(x => x.Parent == _parentItem.ID).ToList().Count.ToString());
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvCongViecCon, _tvCongViecCon.Text, "(", ")");
                    if (_currentFlagPage == 1)
                    {
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvCongViecCon, _tvCongViecCon.Text, "(", ")");
                    }

                    _lnCongViecCon.Click -= Click_lnCongViecCon;
                    _lnCongViecCon.Click += Click_lnCongViecCon;
                }
                else // không có task -> ko cho click
                {
                    _tvCongViecCon.Text = CmmFunction.GetTitle("TEXT_CHILDTASK", "Công việc con");
                    _lnCongViecConContent.RemoveAllViews();
                    _lnCongViecCon.Click -= Click_lnCongViecCon;
                    if (_currentFlagPage == 1) // đang ở trang công việc con -> focus lại phần chi tiết;
                    {
                        Click_lnChiTiet(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataChildTask", ex);
#endif
            }
        }

        public void SetDataComment()
        {
            try
            {
                if (_lstComment != null && _lstComment.Count >= 0) // check list child
                {
                    _lnComment.RemoveAllViews();
                    componentComment = new ComponentComment(_mainAct, _lnComment, _lstComment);
                    componentComment.InitFlagRecalculateView(true);
                    componentComment.InitializeFrameView(_lnComment);
                    componentComment.SetTitle();
                    componentComment.SetValue();
                    componentComment.SetEnable();
                    componentComment.SetProprety();

                    //Event Component
                    componentComment.CustomClick_CommentParent_ImgComment += Click_CommentParent_ImgComment;
                    componentComment.CustomClick_CommentParent_ImgAttach += Click_CommentParent_ImgAttach;

                    componentComment.CustomItemClick_ItemListAttachParent_Detail += Click_ItemListAttachParent_Detail;
                    componentComment.CustomItemClick_ItemListAttachParent_Delete += Click_ItemListAttachParent_Delete;

                    componentComment.CustomItemClick_ItemListComment_tvLike += Click_ItemListComment_Like;
                    componentComment.CustomItemClick_ItemListComment_tvReply += Click_ItemListComment_Reply;
                    componentComment.CustomItemClick_ItemListComment_AttachDetail += Click_ItemListAttachParent_Detail;

                    if (_lstAttachComment != null && _lstAttachComment.Count > 0) // lưu lại trạng thái
                    {
                        componentComment.UpdateListParentAttach(this._lstAttachComment);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataChildTask", ex);
#endif
            }
        }

        private void SetDataRecyAttachFile(List<BeanAttachFile> _lstAttachFile)
        {
            try
            {
                if (_lstAttachFile != null && _lstAttachFile.Count >= 0)
                {
                    _adapterAttachment = new AdapterDetailCreateTask_Attachment(_mainAct, _rootView.Context, _lstAttachFile, _parentItem, _flagUserPermission, _IsClickFromAction);
                    _adapterAttachment.CustomItemClick_ViewItem += Click_ItemRecyAttachment_View;
                    _adapterAttachment.CustomItemClick_SaveItem += Click_ItemRecyAttachment_Save;
                    _adapterAttachment.CustomItemClick_DeleteItem += Click_ItemRecyAttachment_Delete;

                    _recyAttachFile.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                    _recyAttachFile.SetAdapter(_adapterAttachment);
                    _recyAttachFile.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(60, _rootView.Context) * _lstAttachFile.Count);

                    if (_parentItem != null && _parentItem.Status == (int)ControllerDetailCreateTask.ActionStatusID.Hold) // task tạm hoãn ko cho kéo 
                    {
                        swipeHelperAttachment = null;
                    }
                    else
                    {
                        swipeHelperAttachment = new AdapterDetailCreateTask_Attachment_SwipeHelper(_rootView.Context, _recyAttachFile, (int)(_mainAct.Resources.DisplayMetrics.WidthPixels * 0.15), _lstAttachFile);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataRecyAttachFile", ex);
#endif
            }
        }

        private void RenewPreviousFragment()
        {
            try
            {
                MinionAction.OnRefreshFragmentHomePage(null, null);
                MinionAction.OnRenewFragmentSingleVDT(null, null);
                MinionAction.OnRenewFragmentSingleVTBD(null, null);
                /*if (_previousFragment is FragmentHomePage)
                {
                    ((FragmentHomePage)_previousFragment).EventHandler_RefreshFragmentHomePage(null, null);
                }
                else if (_previousFragment is FragmentSingleListVDT)
                {
                    ((FragmentSingleListVDT)_previousFragment).RenewData(null, null);
                }
                else if (_previousFragment is FragmentSingleListVTBD)
                {
                    ((FragmentSingleListVTBD)_previousFragment).RenewData(null, null);
                }
                else*/
                if (_previousFragment is FragmentDetailWorkflow)
                {
                    ((FragmentDetailWorkflow)_previousFragment).GetAndSetDataFromServer();
                }
                else if (_previousFragment is FragmentDetailCreateTask)
                {
                    ((FragmentDetailCreateTask)_previousFragment).RenewCurrentPageAndPrevious(true, false);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "RenewPreviousFragment", ex);
#endif
            }
        }

        private async void SendAPIDeleteTask()
        {
            try
            {
                if (_parentItem != null)
                {
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                    await Task.Run(async () =>
                    {
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                        bool _result = _pControlDynamic.DeleteDetailTaskForm(_parentItem.ID);

                        if (_result == true)
                        {

                            ProviderBase pBase = new ProviderBase();
                            await Task.Run(() =>
                            {
                                pBase.UpdateAllDynamicData(true);
                            });
                            _mainAct.RunOnUiThread(() =>
                            {
                                MinionAction.OnRefreshFragmentParent(null, null);
                                _mainAct.HideFragment();
                                CmmDroidFunction.HideProcessingDialog();
                                /*CmmDroidFunction.HideProcessingDialog();
                                Click_imgBack_WithRefreshPage(null, null);*/
                                //if (_fragmentDetailWorkflow != null) // Click từ trang chi tiết vào -> refresh trang
                                //    _fragmentDetailWorkflow.GetAndSetDataFromServer();
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
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SendAPIDeleteTask", ex);
#endif
            }
        }

        /// <summary>
        /// Remove text (*) trong TextView nếu có
        /// </summary>
        /// <param name="_textView"></param>
        private void removeIsRequiredAllTextView()
        {
            try
            {
                if (_tvTieuDe.Text.Contains("(*)"))
                    _tvTieuDe.Text = _tvTieuDe.Text.Replace("(*)", "");

                if (_tvNguoiXuLy.Text.Contains("(*)"))
                    _tvNguoiXuLy.Text = _tvNguoiXuLy.Text.Replace("(*)", "");

                //if (_tvNoiDung.Text.Contains("(*)"))
                //    _tvNoiDung.Text = _tvNoiDung.Text.Replace("(*)", "");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }
        #endregion

        #region Task List
        private void Click_TaskListItem(object sender, MinionActionCore.TaskListItemClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true && e._viewID == 1) // View Chi tiết
                {
                    FragmentDetailCreateTask fragmentDetailCreateTask = new FragmentDetailCreateTask(this, e._clickedItem.groupItem.ID, false, this._workflowItem);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailCreateTask, "FragmentDetailCreateTask", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_TaskListItem", ex);
#endif
            }
        }
        #endregion
    }
}