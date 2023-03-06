using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.OS;
using Android.Provider;
using Android.Renderscripts;
using Android.Runtime;
using Android.Support.Design.Internal;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Webkit;
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
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using BPMOPMobile.Droid.Presenter.SharedView;
using Com.Google.Android.Flexbox;
using Com.Telerik.Widget.Calendar;
using Jp.Wasabeef;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using SQLite;
using TEditor;
using TEditor.Abstractions;
using static BPMOPMobile.Droid.Class.MinionAction;
using static BPMOPMobile.Droid.Core.Class.EnumFormControlView;
using static BPMOPMobile.Droid.Core.Class.MinionActionCore;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentDetailWorkflow : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private Dialog _dialogPopupControl, _dialogPopupControlGrid;
        private View _rootView;
        private LinearLayout _lnAll, _lnActionAll, _lnItemAll, _lnData, _lnNoData, _relaTaskName;
        private TextView _tvTitle, _tvItemAvatar, _tvItemTitle, _tvItemTime, _tvItemDescription, _tvNoData;
        private ImageView _imgBack, _imgProcess, _imgComment, _imgAttachFile, _imgSubcribe, _imgItemAttach, _imgItemFlag, _imgShare;
        private CircleImageView _imgAvatar;
        private RecyclerView _recyData;
        private View _viewDiableActionDraft;

        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();
        private ControllerComment CTRLComment = new ControllerComment();
        public BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        public BeanNotify _notifyItem = new BeanNotify();

        public JObject _OBJFORMACTION;
        private List<ViewSection> _LISTSECTION = new List<ViewSection>();                       // List lưu Section Dynamic Form
        private ViewRow _LISTACTION = new ViewRow();                                            // List lưu Dynamic Action
        private List<BeanTask> _LISTTASK = new List<BeanTask>();                                // List lưu Dynamic Task
        private List<BeanWorkFlowRelated> _LISTFLOWRELATED = new List<BeanWorkFlowRelated>();   // List lưu Dynamic Workflow Related
        private List<ViewElement> _lstEditedElement = new List<ViewElement>();                  // List lưu những Element có edit để send lên API 

        public List<BeanAttachFile> _lstAttFileControl_Deleted = new List<BeanAttachFile>();    // List lưu lại những item nào đã bị xóa ra khỏi Control inputattachmenthorizon
        public List<JObject> _lstGridDetail_Deleted = new List<JObject>();                      // List lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail

        private List<BeanQuaTrinhLuanChuyen> _lstQTLC = new List<BeanQuaTrinhLuanChuyen>();     // List lưu QTLC
        private ComponentButtonBot _componentButtonBot;                                         // Component của Control Button Bot
        public AdapterDetailRecyDynamicControl _adapterDetailExpandControl;
        private ViewElement _clickedElementAttachment;                                          // Lưu lại để nếu có gọi OnActivityResult thì update cho nhanh
        private string _previousFragment = "";

        public Java.IO.File _tempfileFromCamera;

        private AdapterRecyTemplateValueType _adapterTemplateControlGrid;                       // Adapter khi click vào control grid -> ko đụng vào

        // Component Comment
        private List<BeanComment> _LISTCOMMENT = new List<BeanComment>();
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private DateTime _CommentChanged;
        private string _OtherResourceId = "";
        private int _maxLinesTvPeople = 1;

        #region Constructor
        public FragmentDetailWorkflow() { /* Prevent Darkmode */ }
        public FragmentDetailWorkflow(BeanWorkflowItem _workflowItem, BeanNotify _notifyItem, string _previousFragment)
        {
            this._workflowItem = _workflowItem;
            this._notifyItem = _notifyItem;
            this._previousFragment = _previousFragment;
        }
        #endregion

        #region Life Cycle
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainActivity._notificationID = ""; // khởi tạo lại giá trị
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            MinionActionCore.ElementFormClickEvent -= Click_ElementFormEvent; // Event Click vào Element Control Form
            MinionActionCore.ElementFormClickEvent_WithInnerAction -= Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
            MinionActionCore.ElementGridChildActionClickEvent -= Click_ElementGridChildActionClickEvent; // Event Click vào Element trên popup của lưới Detail
            MinionActionCore.FlowRelatedClickEvent_WithInnerAction -= Click_FlowRelated_WithInnerAction; // Event Click vào Item của Flow Related
            MinionActionCore.ElementActionClickEvent -= Click_ElementActionEvent; // Event Click vào Element Control Button Bot
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewDetailWorkflow, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_All);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Back);
                _imgProcess = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Proce);
                _imgAttachFile = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_AttachFile);
                _imgComment = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Comment);
                _imgShare = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Share);
                _imgSubcribe = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Subcribe);
                _lnActionAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_ActionAll);
                //_expandData = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewDetailWorkflow_Data);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewDetailWorkflow_Data);
                _lnData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_Data);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_NoData);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_NoData);
                _viewDiableActionDraft = _rootView.FindViewById<View>(Resource.Id.view_Diable_WorkFlowDraft);
                #region View Item
                _relaTaskName = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_TaskName);
                _lnItemAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailWorkflow_ItemAll);
                _tvItemAvatar = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_Avatar);
                _imgAvatar = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewDetailWorkflow_Avatar);
                _tvItemTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_Title);
                _tvItemTime = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_Time);
                _tvItemDescription = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailWorkflow_Description);
                _imgItemFlag = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_Flag);
                _imgItemAttach = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailWorkflow_ItemAttachFile);
                _lnItemAll.Click += delegate { return; }; // Disable Itemclick cua list truoc do
                _lnAll.Click += delegate { return; };
                #endregion

                _tvTitle.Click += Click_tvTitle;
                _tvItemDescription.Click += Click_tvItemPeople;

                _imgAttachFile.Click += Click_imgAttachFile;
                _imgBack.Click += Click_Back;
                _imgProcess.Click += Click_imgProcess;
                _imgComment.Click += Click_imgComment;
                _imgShare.Click += Click_imgShare;
                _imgSubcribe.Click += Click_imgSubcribe;
                if (_workflowItem != null)
                {
                    if (_workflowItem.StatusGroup == 1)
                    {
                        _viewDiableActionDraft.Visibility = ViewStates.Visible;
                        _viewDiableActionDraft.Click += _viewDiableActionDraft_Click;
                        _imgComment.Enabled = false;
                        _imgAttachFile.Enabled = false;
                    }
                }
                _tvTitle.Text = "";
                _lnActionAll.Visibility = ViewStates.Gone; // ẩn đi -> nếu có action thì hiện lại
                _lnItemAll.Visibility = ViewStates.Gone; // ẩn đi -> gọi api xong load lại
                _relaTaskName.Visibility = ViewStates.Gone; // ẩn đi -> gọi api xong load lại
                if (_workflowItem != null && !String.IsNullOrEmpty(_workflowItem.ID))
                {
                    SetView();
                    GetAndSetDataFromServer();
                }
                else
                {
                    CmmDroidFunction.ShowAlertDialogWithAction(_mainAct,
                    CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                    _actionPositiveButton: new Action(() => { Click_Back(null, null); }),
                    _actionNegativeButton: null,
                    _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                    _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
                }
            }
            MinionActionCore.ElementFormClickEvent += Click_ElementFormEvent; // Event Click vào Element Control Form
            MinionActionCore.ElementFormClickEvent_WithInnerAction += Click_ElementFormEvent_WithInnerAction; // Event Click vào Element Control Form có action bên trong
            MinionActionCore.ElementGridChildActionClickEvent += Click_ElementGridChildActionClickEvent; // Event Click vào Element trên popup của lưới Detail
            MinionActionCore.FlowRelatedClickEvent_WithInnerAction += Click_FlowRelated_WithInnerAction; // Event Click vào Item của Flow Related
            MinionActionCore.ElementActionClickEvent += Click_ElementActionEvent; // Event Click vào Element Control Button Bot
            CmmDroidFunction.HideSoftKeyBoard(_mainAct);
            return _rootView;
        }

        private void _viewDiableActionDraft_Click(object sender, EventArgs e)
        {
            CmmDroidFunction.ShowAlertDialog(_mainAct,
                               CmmFunction.GetTitle("TEXT_ALERT_DRAFT", "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này"),
                               CmmFunction.GetTitle("TEXT_ALERT_DRAFT", "Please use the web version to edit this item!")
                               );
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if ((requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileComment || requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileComment_Camera) && resultCode == (int)Result.Ok)
                {
                    // COMPONENT COMMENT

                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileComment) // chọn file thường
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

                    if (CTRLDetailWorkflow.CheckFileExistInList(_lstAttachComment, _beanAttachFile) == true) // Validate
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                           CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                        return;
                    }

                    if (_adapterDetailExpandControl != null)
                    {
                        _lstAttachComment.Add(_beanAttachFile);
                        _adapterDetailExpandControl.UpdateListAttachComment(_lstAttachComment);
                        _adapterDetailExpandControl.NotifyDataSetChanged();
                    }
                }
                else if ((requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment || requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment_Camera) && resultCode == (int)Result.Ok)
                {
                    // CONTROL ATTACHMENT VERTICAL
                    BeanAttachFile _beanAttachFile = new BeanAttachFile();

                    if (requestCode == CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment) // chọn file thường
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

                    if (CTRLDetailWorkflow.CheckFileExistInList(_lstAttachComment, _beanAttachFile) == true) // Validate
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                           CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                        return;
                    }

                    if (_clickedElementAttachment != null) // Cập nhật lại giá trị cho Control Attachment
                    {
                        List<BeanAttachFile> _lstAttFileControl_Full = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_clickedElementAttachment.Value.Trim());

                        #region Validate
                        if (!String.IsNullOrEmpty(_beanAttachFile.Path) && _lstAttFileControl_Full != null)// Check xem có tồn tại trong list chưa, nếu có rồi -> return
                        {
                            if (CTRLDetailWorkflow.CheckFileExistInList(_lstAttFileControl_Full, _beanAttachFile) == true) // Validate
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"),
                                                   CmmFunction.GetTitle("TEXT_EXISTINGFILE", "File has been existed in list"));
                                return;
                            }
                        }
                        #endregion

                        _lstAttFileControl_Full.Add(_beanAttachFile);
                        UpdateValueForElement(_clickedElementAttachment, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                        _adapterDetailExpandControl.NotifyDataSetChanged();
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
        #endregion

        #region Event
        private void SetView()
        {
            try
            {
                _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");

                CTRLDetailWorkflow.SetTitleByItem(_tvTitle, _workflowItem, _notifyItem);

                #region Line 1
                if (_notifyItem == null) // Click từ VTBD vào -> Avatar của người gửi -> Có gắn lại Avatar sau khi gọi QTLC
                {
                    if (!String.IsNullOrEmpty(_workflowItem.AssignedTo))
                    {
                        string[] valueSearch = _workflowItem.AssignedTo.ToLowerInvariant().Split(",");
                        CmmDroidFunction.SetAvataByBeanUser(_mainAct, _rootView.Context, valueSearch[0], "ID", _imgAvatar, _tvItemAvatar, 100);
                    }
                    else
                        _tvItemAvatar.Visibility = ViewStates.Invisible;
                }
                else // Click từ VDT vào -> Avatar của người hiện hành -> Có gắn lại Avatar sau khi gọi QTLC
                {
                    if (!String.IsNullOrEmpty(_notifyItem.SendUnit))
                        CmmDroidFunction.SetAvataByBeanUser(_mainAct, _rootView.Context, _notifyItem.SendUnit, "FullName", _imgAvatar, _tvItemAvatar, 60);
                    else
                        _tvItemAvatar.Visibility = ViewStates.Invisible;
                }

                if (_notifyItem == null) // Click từ VTBD vào -> Tv in đậm là người khởi tạo
                {
                    if (!String.IsNullOrEmpty(_workflowItem.CreatedByName))
                        _tvItemTitle.Text = _workflowItem.CreatedByName;
                    else
                        _tvItemTitle.Visibility = ViewStates.Invisible;
                }
                else // Click từ VDT vào -> Tv in đậm là giống Avatar - người gửi đến
                {
                    if (!String.IsNullOrEmpty(_notifyItem.SendUnit))
                        _tvItemTitle.Text = _notifyItem.SendUnit;
                    else
                    {
                        if (_workflowItem != null)
                        {
                            if (!String.IsNullOrEmpty(_workflowItem.CreatedByName))
                                _tvItemTitle.Text = _workflowItem.CreatedByName;
                            else
                                _tvItemTitle.Visibility = ViewStates.Invisible;
                        }
                        else
                            _tvItemTitle.Visibility = ViewStates.Invisible;
                    }
                }

                if (_workflowItem.Created.HasValue)
                    _tvItemTime.Text = CTRLDetailWorkflow.GetFormatDateLang(_workflowItem.Created.Value);
                else
                    _tvItemTime.Visibility = ViewStates.Invisible;

                if (_notifyItem == null) // Click từ VTBD vào
                {
                    if (_workflowItem.Created.HasValue)
                        _tvItemTime.Text = CTRLDetailWorkflow.GetFormatDateLang(_workflowItem.Created.Value);
                    else
                        _tvItemTime.Visibility = ViewStates.Invisible;
                }
                else // Click từ VDT vào
                {
                    if (_notifyItem.Created.HasValue) // ưu tiên ngày của to do
                        _tvItemTime.Text = CTRLDetailWorkflow.GetFormatDateLang(_notifyItem.Created.Value);
                }

                #endregion

                #region Line 2
                if (!String.IsNullOrEmpty(_workflowItem.AssignedTo))
                {
                    try
                    {
                        string[] _lstFullName = CTRLHomePage.GetArrayFullNameFromArrayID(_workflowItem.AssignedTo.ToLowerInvariant().Split(","));

                        switch (_workflowItem.StatusGroup)
                        {
                            case (int)CmmFunction.AppStatusID.Completed: // Đã phê duyệt
                                _tvItemDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + _lstFullName[0];
                                break;
                            case (int)CmmFunction.AppStatusID.Canceled: // Đã hủy
                                _tvItemDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + _lstFullName[0];
                                break;
                            case (int)CmmFunction.AppStatusID.Rejected: // Đã từ chối
                                _tvItemDescription.Text = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + _lstFullName[0];
                                break;
                            default:
                                CTRLHomePage.SetTextView_FormatMultiUser_DetailWorkflow(_rootView.Context, _tvItemDescription, _lstFullName);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        _tvItemDescription.Visibility = ViewStates.Invisible;
                    }
                }
                else
                    _tvItemDescription.Visibility = ViewStates.Invisible;

                if (_notifyItem == null) // Click từ VTBD vào
                {
                    _imgItemFlag.Visibility = ViewStates.Invisible;
                }
                else // Click từ VDT vào
                {
                    if (_notifyItem.Priority == 3)
                        _imgItemFlag.Visibility = ViewStates.Visible;
                    else
                        _imgItemFlag.Visibility = ViewStates.Invisible;
                }


                if (_workflowItem.IsFollow == true)
                {
                    _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                    _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clYellow)));
                }
                else
                {
                    _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                    _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                }

                _imgItemAttach.Visibility = ViewStates.Invisible;
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentDetailWorkflow - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Set lại view khi đã lấy ProcessHistory - trường hợp click từ VTBD vào
        /// </summary>
        private void SetViewItem_ByProcessHistory()
        {
            try
            {
                if (_workflowItem.ActionStatusID == 10 || _workflowItem.ActionStatusID == -1 || _workflowItem.ActionStatusID == 6) // da phe duyet - da huy - da tu choi
                {
                    if (!String.IsNullOrEmpty(_workflowItem.CreatedBy))
                    {
                        string[] valueSearch = _workflowItem.CreatedBy.ToLowerInvariant().Split(",");
                        CmmDroidFunction.SetAvataByBeanUser(_mainAct, _rootView.Context, valueSearch[0], "ID", _imgAvatar, _tvItemAvatar, 100);
                    }
                    if (!String.IsNullOrEmpty(_workflowItem.CreatedByName))
                    {
                        _tvItemTitle.Text = _workflowItem.CreatedByName;
                    }
                }
                else if (_lstQTLC != null & _lstQTLC.Count > 0) // gắn lại avatar người gửi bước trước đó
                {
                    // Nếu phiếu đã duyệt - đã hủy - đã từ chối xong thì Avatar + tên in đậm là người tạo

                    if (!String.IsNullOrEmpty(_lstQTLC[0].FromUserId))
                    {
                        string[] valueSearch = _lstQTLC[0].FromUserId.ToLowerInvariant().Split(",");
                        CmmDroidFunction.SetAvataByBeanUser(_mainAct, _rootView.Context, valueSearch[0], "ID", _imgAvatar, _tvItemAvatar, 100);
                    }
                    if (!String.IsNullOrEmpty(_lstQTLC[0].FromUserName))
                    {
                        _tvItemTitle.Visibility = ViewStates.Visible;
                        _tvItemTitle.Text = _lstQTLC[0].FromUserName;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewItem_ByProcessHistory", ex);
#endif
            }
        }

        private void Click_Back(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(1000))
                    _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Back", ex);
#endif
            }
        }

        private void Click_Back_WithRefreshPage(object sender, EventArgs e)
        {
            try
            {
                MinionAction.OnRefreshFragmentHomePage(null, null);
                MinionAction.OnRenewFragmentSingleVDT(null, null);
                MinionAction.OnRenewFragmentSingleVTBD(null, null);


                // App Parent
                /*if (_previousFragment.Equals(typeof(FragmentHomePage).Name))
                {
                    MinionAction.OnRefreshFragmentHomePage(null, null);
                }
                else if (_previousFragment.Equals(typeof(FragmentSingleListVDT).Name))
                {
                    MinionAction.OnRenewFragmentSingleVDT(null, null);
                }
                else*/
                if (_previousFragment.Equals(typeof(FragmentSingleListFollow).Name))
                {
                    MinionAction.OnRenewFragmentSingleFollow(null, null);
                }
                /*else if (_previousFragment.Equals(typeof(FragmentSingleListVTBD).Name))
                {
                    MinionAction.OnRenewFragmentSingleVTBD(null, null);
                }*/
                else if (_previousFragment.Equals(typeof(FragmentBoardDetailGroup).Name))
                {
                    MinionAction.OnRenewFragmentBoardDetailGroup(null, null);
                }
                // Child App
                else if (_previousFragment.Equals(typeof(FragmentChildAppHomePage).Name))
                {
                    MinionAction.OnRefreshFragmentHomePage(null, null);
                }
                else if (_previousFragment.Equals(typeof(FragmentChildAppSingleListVDT).Name))
                {
                    MinionAction.OnRenewFragmentSingleVDT(null, null);
                }
                else if (_previousFragment.Equals(typeof(FragmentChildAppSingleListVTBD).Name))
                {
                    MinionAction.OnRenewFragmentSingleVTBD(null, null);
                }
                else if (_previousFragment.Equals(typeof(FragmentChildAppSingleListVTBD).Name))
                {
                    MinionAction.OnRenewFragmentBoardDetailGroup(null, null);
                }
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Back_WithRefreshPage", ex);
#endif
            }
        }

        private void Click_tvTitle(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true && CmmDroidFunction.CheckTextViewIsEllipsized(_tvTitle) == true)
                {
                    // gọi lại hàm xem full info
                    ViewElement tempElement = new ViewElement();
                    tempElement.Title = "";
                    tempElement.Value = _tvTitle.Text;

                    SharedView_PopupControlViewFullInfo _popupControlViewFullInfo = new SharedView_PopupControlViewFullInfo(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                    _popupControlViewFullInfo.InitializeValue_Master(tempElement);
                    _popupControlViewFullInfo.InitializeView();
                    //ShowPopup_ViewFullInformation(tempElement);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView_Action", ex);
#endif
            }
        }

        private void Click_tvItemPeople(object sender, EventArgs e)
        {
            try
            {
                if (_workflowItem.ActionStatusID == 10 || _workflowItem.ActionStatusID == -1 || _workflowItem.ActionStatusID == 6) // da phe duyet - da huy - Từ chối
                {
                    return;
                }

                _tvItemDescription.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                if (_maxLinesTvPeople == 1)
                {
                    _maxLinesTvPeople = int.MaxValue;
                    _tvItemDescription.SetMaxLines(_maxLinesTvPeople);
                }
                else
                {
                    _maxLinesTvPeople = 1;
                    _tvItemDescription.SetMaxLines(_maxLinesTvPeople);
                }

                if (!_workflowItem.AssignedTo.Contains(",")) // Không phải nhiều người -> ko check
                {
                    return;
                }

                if (!String.IsNullOrEmpty(_tvItemDescription.Text) && _tvItemDescription.Text.Contains("+")) // Đang ở dạng +n người
                {
                    //_tvItemDescription.SetMaxLines(int.MaxValue);
                    string[] _lstFullName = CTRLHomePage.GetArrayFullNameFromArrayID(_workflowItem.AssignedTo.ToLowerInvariant().Split(","));

                    CTRLHomePage.SetTextView_FormatMultiUser_DetailWorkflow(_rootView.Context, _tvItemDescription, _lstFullName, _plusMoreFormat: false);
                }
                else
                {
                    //_tvItemDescription.SetMaxLines(1);
                    string[] _lstFullName = CTRLHomePage.GetArrayFullNameFromArrayID(_workflowItem.AssignedTo.ToLowerInvariant().Split(","));
                    CTRLHomePage.SetTextView_FormatMultiUser_DetailWorkflow(_rootView.Context, _tvItemDescription, _lstFullName);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView_Action", ex);
#endif
            }
        }

        private void Click_imgProcess(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _imgProcess.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                    FragmentDetailProcess fragmentDetailProcess = new FragmentDetailProcess(this, _workflowItem, _notifyItem, _lstQTLC);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailProcess, "FragmentDetailProcess", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgProcess", ex);
#endif
            }
        }

        private void Click_imgComment(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    int _commentPosition = _adapterDetailExpandControl.ItemCount;
                    _recyData.SmoothScrollToPosition(_commentPosition);

                    Action action = new Action(() =>
                    {
                        _recyData.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgProcess", ex);
#endif
            }
        }

        private void Click_imgAttachFile(object sender, EventArgs e)
        {
            try
            {
                _imgAttachFile.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                List<BeanAttachFile> _lstAttFile = new List<BeanAttachFile>(); // List File đính kèm
                ViewElement _elementAttachFile = new ViewElement();
                if (_clickedElementAttachment != null)
                {
                    _elementAttachFile = _clickedElementAttachment;
                    _lstAttFile = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_clickedElementAttachment.Value);
                }
                else
                {
                    _elementAttachFile = CTRLDetailWorkflow.GetElementAttachFromSection(_LISTSECTION);
                    _lstAttFile = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_elementAttachFile.Value);
                }


                if (_elementAttachFile != null)
                {
                    _lstAttFile = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_elementAttachFile.Value);
                }

                if (_lstAttFile != null && _lstAttFile.Count > 0 && _imgAttachFile.Alpha == 1)
                {
                    FragmentDetailAttachFile fragmentDetailAttachFile = new FragmentDetailAttachFile(this, _elementAttachFile, _lstAttFile, _workflowItem, _notifyItem);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailAttachFile, "FragmentDetailAttachFile", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgAttachFile", ex);
#endif
            }
        }

        private void Click_imgShare(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                {
                    _imgShare.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    FragmentDetailShare fragmentDetailShare = new FragmentDetailShare(this, _workflowItem, _notifyItem);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailShare, "FragmentDetailShare", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgProcess", ex);
#endif
            }
        }

        private void Click_imgSubcribe(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _imgSubcribe.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    string _message = "";

                    if (_workflowItem.IsFollow == true) // unfollow
                        _message = CmmFunction.GetTitle("MESS_UNFOLLOW_TASK", "Hủy theo dõi công việc này");
                    else
                        _message = CmmFunction.GetTitle("MESS_FOLLOW_TASK", "Đặt theo dõi công việc này");

                    Action _actionPositiveButton = new Action(async () =>
                    {
                        CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                        if (_workflowItem.IsFollow != true)
                        {
                            _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                            _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clYellow)));
                        }
                        else
                        {
                            _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                            _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                        }
                        await Task.Run(() =>
                        {
                            bool _result = false;
                            ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                            JArray jArrayForm = JArray.Parse(_OBJFORMACTION["form"].ToString());
                            string _formDefineInfo = jArrayForm[0]["FormDefineInfo"].ToString();
                            List<KeyValuePair<string, string>> _lstExt = new List<KeyValuePair<string, string>>();
                            _lstExt.Add(new KeyValuePair<string, string>("status", _workflowItem.IsFollow ? "0" : "1"));

                            string _messageAPI = "";

                            _result = _pControlDynamic.SendControlDynamicAction("Follow", _workflowItem.ID, _formDefineInfo, JsonConvert.SerializeObject(new List<ObjectSubmitAction>()), ref _messageAPI, new List<KeyValuePair<string, string>>(), _lstExt);

                            if (_result)
                            {
                                _pControlDynamic.UpdateMasterData<BeanWorkflowFollow>();
                                _workflowItem.IsFollow = !_workflowItem.IsFollow;
                                _mainAct.RunOnUiThread(() =>
                                {
                                    if (_workflowItem.IsFollow == true)
                                    {
                                        _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                                        _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clYellow)));
                                    }
                                    else
                                    {
                                        _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                                        _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                                    }

                                    // UPDATE lại dưới DB + Tick event Renew lại Item ở trang trước
                                    SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                                    string _queryUpdateFavorite = String.Format(CTRLDetailWorkflow._queryUpdateFavorite, _workflowItem.IsFollow == true ? 1 : 0, _workflowItem.ID);
                                    conn.Execute(_queryUpdateFavorite);
                                    conn.Close();

                                    MinionAction.OnRenewItem_AfterFollow(null, new MinionAction.RenewItem_AfterFollow(_workflowItem.ID, _workflowItem.IsFollow));

                                    CmmDroidFunction.HideProcessingDialog();
                                });
                            }
                            else
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                if (!String.IsNullOrEmpty(_messageAPI))
                                    CmmDroidFunction.ShowAlertDialog(_mainAct, _messageAPI, _messageAPI);
                                else
                                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                       CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                            }
                        });
                    });

                    CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, _message,
                    _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                    _actionNegativeButton: new Action(() => { }),
                    //_title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                    _title: "",
                    _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                    _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgSubcribe", ex);
#endif
            }
        }

        private void Click_GroupExpandDetail(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            try
            {
                ////if (_LISTSECTION[e.GroupPosition].ShowType == true)
                ////{
                ////    _LISTSECTION[e.GroupPosition].ShowType = false;
                ////}
                ////else
                ////{
                ////    _LISTSECTION[e.GroupPosition].ShowType = true;
                ////}
                ////_adapterDetailExpandControl.NotifyDataSetChanged();

                ////for (int i = 0; i < _adapterDetailExpandControl.GroupCount; i++)
                ////{
                ////    if (_LISTSECTION[i].ShowType)
                ////    {
                ////        _expandData.ExpandGroup(i);
                ////    }
                ////    else
                ////    {
                ////        _expandData.CollapseGroup(i);
                ////    }
                ////}


            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_GroupExpandDetail", ex);
#endif
            }
        }

        public void SetEnableView(bool enable)
        {
            _imgBack.Clickable = enable;
            _imgSubcribe.Clickable = enable;
            _imgAttachFile.Clickable = enable;
            _imgProcess.Clickable = enable;
        }

        #endregion

        #region Data
        public async void GetAndSetDataFromServer(bool showdialog = true)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (showdialog == true)
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                SetEnableView(false);
                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    string _resultString = "";

                    _resultString = _pControlDynamic.GetTicketRequestControlDynamicForm(_workflowItem, CmmVariable.SysConfig.LangCode); // List Form control

                    _lstQTLC = _pControlDynamic.GetListProcessHistory(_workflowItem);

                    if (String.IsNullOrEmpty(_resultString))
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            if (showdialog == true)
                                CmmDroidFunction.HideProcessingDialog();

                            _lnData.Visibility = ViewStates.Gone;
                            _lnNoData.Visibility = ViewStates.Visible;
                            SetEnableView(true);

                            CmmDroidFunction.ShowAlertDialogWithAction(_mainAct,
                            CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"),
                            _actionPositiveButton: new Action(() => { Click_Back(null, null); }),
                            _actionNegativeButton: null,
                            _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                            _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"),
                            _cancelable: false);
                        });
                        return;
                    }
                    _OBJFORMACTION = JObject.Parse(_resultString);

                    try { _LISTSECTION = JArray.Parse(_OBJFORMACTION["form"].ToString()).ToObject<List<ViewSection>>(); }
                    catch (Exception) { _LISTSECTION = new List<ViewSection>(); }

                    try { _LISTACTION = JObject.Parse(_OBJFORMACTION["action"].ToString()).ToObject<ViewRow>(); }
                    catch (Exception) { _LISTACTION = new ViewRow(); }

                    try { _LISTFLOWRELATED = JArray.Parse(_OBJFORMACTION["related"].ToString()).ToObject<List<BeanWorkFlowRelated>>(); }
                    catch (Exception) { _LISTFLOWRELATED = new List<BeanWorkFlowRelated>(); }

                    try { _LISTTASK = JArray.Parse(_OBJFORMACTION["task"].ToString()).ToObject<List<BeanTask>>(); }
                    catch (Exception) { _LISTTASK = new List<BeanTask>(); }

                    try { _CommentChanged = DateTime.Parse(_OBJFORMACTION["moreInfo"]["CommentChanged"].ToString()); }
                    catch (Exception) { _CommentChanged = new DateTime(); }

                    try { _OtherResourceId = _OBJFORMACTION["moreInfo"]["OtherResourceId"].ToString(); }
                    catch (Exception) { _OtherResourceId = ""; }

                    try // Authen View Comment -> để sau này gọi API comment
                    {
                        //tracking
                        ObjectSubmitDetailComment _objSubmitDetailComment = CTRLDetailWorkflow.InitTrackingObjectSubmitDetail(_mainAct);
                        // comment
                        _objSubmitDetailComment.ID = _OtherResourceId; // empty or result
                        _objSubmitDetailComment.ResourceCategoryId = ((int)CmmFunction.CommentResourceCategoryID.WorkflowItem).ToString();
                        _objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment((int)CmmFunction.CommentResourceCategoryID.WorkflowItem), _workflowItem.ID); // lấy trong beansetting
                        _objSubmitDetailComment.ItemId = _workflowItem.ID;
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

                    #region Get List Comment

                    if (_workflowItem.CommentChanged == null) // Lấy lần đầu
                    {
                        string _APIdatenow = "";
                        _LISTCOMMENT = _pControlDynamic.GetListComment(_OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.WorkflowItem, null, ref _APIdatenow);
                        if (_LISTCOMMENT != null && _LISTCOMMENT.Count > 0)
                        {
                            _workflowItem.CommentChanged = DateTime.Parse(_APIdatenow);
                            _workflowItem.IsChange = false;
                            conn.Update(_workflowItem);
                        }
                    }
                    else // ko phải lần đầu -> so sánh với local
                    {
                        if (_workflowItem.CommentChanged < _CommentChanged || _CommentChanged == new DateTime())
                        {
                            _workflowItem.IsChange = true;
                        }

                        if (_workflowItem.IsChange == true) // trên server có thay đổi -> update list cũ
                        {
                            string _APIdatenow = "";

                            _pControlDynamic.GetListComment(_OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.WorkflowItem, _workflowItem.CommentChanged, ref _APIdatenow);

                            _workflowItem.CommentChanged = DateTime.Parse(_APIdatenow);
                            _workflowItem.IsChange = false;
                            conn.Update(_workflowItem);

                            // GetListComment Đã update Sqlite -> gọi Local lên
                            string _queryComment = string.Format(CTRLComment._queryComment, _OtherResourceId);
                            List<BeanComment> _lstCommentLocal = conn.Query<BeanComment>(_queryComment); // List Local
                            _LISTCOMMENT = _lstCommentLocal;
                        }
                        else // ko có thay đổi -> lấy từ local
                        {
                            string _queryComment = string.Format(CTRLComment._queryComment, _OtherResourceId);
                            List<BeanComment> _lstCommentLocal = conn.Query<BeanComment>(_queryComment); // List Local
                            _LISTCOMMENT = _lstCommentLocal;
                        }
                    }

                    #endregion

                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnItemAll.Visibility = ViewStates.Visible;
                        _relaTaskName.Visibility = ViewStates.Visible;
                        SetViewItem_ByProcessHistory(); // Renew view item
                        SetData_Toolbar();
                        SetData_DynamicControlForm();
                        SetData_DynamiControlAction();
                        SetEnableView(true);
                        if (showdialog == true)
                            CmmDroidFunction.HideProcessingDialog();
                        //CmmDroidFunction.HideProcessingDialogFullScreen();
                        if (_workflowItem.StatusGroup != 1)
                        {
                            _lnActionAll.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            CmmDroidFunction.ShowAlertDialog(_mainAct,
                                CmmFunction.GetTitle("TEXT_ALERT_DRAFT", "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này"),
                                CmmFunction.GetTitle("TEXT_ALERT_DRAFT", "Please use the web version to edit this item!")
                                );

                        }
                    });
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    _lnData.Visibility = ViewStates.Gone;
                    _lnNoData.Visibility = ViewStates.Visible;
                    SetEnableView(true);
                    if (showdialog == true)
                        CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetAndSetDataFromServer", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void SetData_Toolbar()
        {
            try
            {
                // Favorite
                _workflowItem.IsFollow = _LISTSECTION[0].IsFollow;
                if (_workflowItem.IsFollow == true)
                {
                    _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_checked);
                    _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clYellow)));
                }
                else
                {
                    _imgSubcribe.SetImageResource(Resource.Drawable.icon_ver2_star_unchecked);
                    _imgSubcribe.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                }

                List<BeanAttachFile> _lstAttFile = new List<BeanAttachFile>(); // List File đính kèm
                ViewElement _elementAttachFile = CTRLDetailWorkflow.GetElementAttachFromSection(_LISTSECTION);
                if (_elementAttachFile != null)
                {
                    _lstAttFile = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_elementAttachFile.Value);
                    if (_lstAttFile == null || _lstAttFile.Count == 0)
                    {
                        _imgAttachFile.Alpha = (float)0.5;
                        _imgAttachFile.Click += (sender, e) => { }; // Disable Event Đã set trước đó
                    }
                }
                else
                {
                    _imgAttachFile.Alpha = (float)0.5;
                    _imgAttachFile.Click += (sender, e) => { }; // Disable Event Đã set trước đó
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_Toolbar", ex);
#endif
            }
        }

        private void SetData_DynamicControlForm()
        {
            try
            {
                if (_LISTSECTION != null && _LISTSECTION.Count > 0)
                {
                    for (int i = 0; i < _LISTSECTION.Count; i++)
                        if (_LISTSECTION[i].ViewRows == null || _LISTSECTION[0].ViewRows.Count <= 0)
                            _LISTSECTION.Remove(_LISTSECTION[i]);

                    _adapterDetailExpandControl = new AdapterDetailRecyDynamicControl(_mainAct, _rootView.Context, _workflowItem, _notifyItem, _LISTSECTION, _LISTFLOWRELATED, _LISTTASK, _LISTCOMMENT, (int)FlagViewControlAttachment.DetailWorkflow, _OtherResourceId);
                    _adapterDetailExpandControl.UpdateFragmentName(this, this.GetType().Name);

                    // Component Task
                    _adapterDetailExpandControl.CustomItemClick_TaskListItem += Click_ComponentTaskList_Item;
                    // Component Comment
                    _adapterDetailExpandControl.CustomItemClick_CommentParent_ImgComment += Click_ComponentComment_ParentComment_ImgComment;
                    _adapterDetailExpandControl.CustomItemClick_CommentParent_ImgAttach += Click_ComponentComment_ParentComment_ImgAttach;
                    _adapterDetailExpandControl.CustomItemClick_Attach_Detail += Click_ComponentComment_ParentAttach_Detail;
                    _adapterDetailExpandControl.CustomItemClick_Attach_Delete += Click_ComponentComment_ParentAttach_Delete;

                    _recyData.SetAdapter(_adapterDetailExpandControl);
                    _recyData.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                    _recyData.SetItemViewCacheSize(_adapterDetailExpandControl.ItemCount);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_DynamicControlForm", ex);
#endif
            }
        }

        private void SetData_DynamiControlAction()
        {
            try
            {
                if (_LISTACTION.Elements != null && _LISTACTION.Elements.Count > 0)
                {
                    if (_workflowItem.StatusGroup != 1)
                    {
                        _lnActionAll.Visibility = ViewStates.Visible;
                    }

                    _lnActionAll.RemoveAllViews();
                    _componentButtonBot = new ComponentButtonBot(_mainAct, _lnActionAll, _LISTACTION, Resources);
                    _componentButtonBot.InitializeFrameView(_lnActionAll);
                    _componentButtonBot.SetTitle();
                    _componentButtonBot.SetValue();
                    _componentButtonBot.SetEnable();
                    _componentButtonBot.SetProprety();
                }
                else
                {
                    _lnActionAll.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_DynamiControlAction", ex);
#endif
            }
        }

        private List<BeanUser> GetListUserFromQTLC(List<BeanQuaTrinhLuanChuyen> _lstQTLC)
        {
            if (_workflowItem.Step.HasValue) // chỉ lấy những user bước trước đó
            {
                _lstQTLC = _lstQTLC.Where(x => x.Step < _workflowItem.Step).ToList();
            }

            List<BeanUser> _result = new List<BeanUser>();
            string _strSearchUser = "";
            try
            {
                if (_lstQTLC != null && _lstQTLC.Count > 0)
                {
                    List<string> lstEmail = new List<string>();
                    foreach (BeanQuaTrinhLuanChuyen item in _lstQTLC)
                    {
                        if (!lstEmail.Any(temp => temp == item.AssignUserId) && item.AssignUserId != CmmVariable.SysConfig.UserId)
                        {
                            lstEmail.Add(item.AssignUserId);
                        }
                    }
                    _strSearchUser = "'" + String.Join("','", lstEmail.ToArray()) + "'";
                    if (!String.IsNullOrEmpty(_strSearchUser))
                    {
                        SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                        ////string query = string.Format("SELECT * FROM BeanUser WHERE AccountName IN({0}) ORDER BY Name ", _strSearchUser);
                        string query = string.Format("SELECT * FROM BeanUser WHERE ID IN({0}) ORDER BY Name ", _strSearchUser);
                        _result = conn.Query<BeanUser>(query);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetListUserFromQTLC", ex);
#endif
            }
            return _result;
        }

        public List<BeanQuaTrinhLuanChuyen> CloneLst(List<BeanQuaTrinhLuanChuyen> sourceList)
        {
            List<BeanQuaTrinhLuanChuyen> retValue = new List<BeanQuaTrinhLuanChuyen>();
            foreach (BeanQuaTrinhLuanChuyen item in sourceList)
            {
                BeanQuaTrinhLuanChuyen newItem = new BeanQuaTrinhLuanChuyen();
                newItem = (BeanQuaTrinhLuanChuyen)CmmFunction.MapData(item, newItem);
                retValue.Add(newItem);
            }

            return retValue;
        }
        #endregion

        #region Custom Control

        #region Dynamic Form
        private void Click_ElementFormEvent(object sender, MinionActionCore.ElementFormClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;

                    switch (_clickedElement.DataType)
                    {
                        case "selectuser":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlSelectUserGroup _popupControlDate = new SharedView_PopupControlSelectUserGroup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: false);
                                _popupControlDate.InitializeValue_Master(_clickedElement);
                                _popupControlDate.InitializeView();
                                break;
                            }
                        case "selectusergroup":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlSelectUserGroup _popupControlDate = new SharedView_PopupControlSelectUserGroup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: true);
                                _popupControlDate.InitializeValue_Master(_clickedElement);
                                _popupControlDate.InitializeView();
                                break;
                            }
                        case "selectusermulti":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlSelectUserGroupMulti _popupControlSelectUserGroupMulti = new SharedView_PopupControlSelectUserGroupMulti(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: false);
                                _popupControlSelectUserGroupMulti.InitializeValue_Master(_clickedElement);
                                _popupControlSelectUserGroupMulti.InitializeView();
                                break;
                            }
                        case "selectusergroupmulti":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlSelectUserGroupMulti _popupControlSelectUserGroupMulti = new SharedView_PopupControlSelectUserGroupMulti(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: true);
                                _popupControlSelectUserGroupMulti.InitializeValue_Master(_clickedElement);
                                _popupControlSelectUserGroupMulti.InitializeView();
                                break;
                            }
                        case "date":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlDate _popupControlDate = new SharedView_PopupControlDate(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                _popupControlDate.InitializeValue_Master(_clickedElement);
                                _popupControlDate.InitializeView();
                                break;
                            }
                        case "datetime":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlDateTime _popupControlDateTime = new SharedView_PopupControlDateTime(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                _popupControlDateTime.InitializeValue_Master(_clickedElement);
                                _popupControlDateTime.InitializeView();
                                break;
                            }
                        case "time":
                            break;
                        case "singlechoice":
                            {
                                // Có trường hợp Enable + Disable
                                SharedView_PopupControlSingleChoice _popupControlSingleChoice = new SharedView_PopupControlSingleChoice(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                _popupControlSingleChoice.InitializeValue_Master(_clickedElement);
                                _popupControlSingleChoice.InitializeView();
                                break;
                            }
                        case "singlelookup":
                            {
                                if (_clickedElement.Enable)
                                {
                                    SharedView_PopupControlSingleLookup _popupControlSingleLookup = new SharedView_PopupControlSingleLookup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlSingleLookup.InitializeValue_Master(_clickedElement);
                                    _popupControlSingleLookup.InitializeView();
                                }
                                break;
                            }
                        case "multiplechoice":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                // Có trường hợp Enable hoặc Disable có Data
                                SharedView_PopupControlMultiChoice _popupControlMultiChoice = new SharedView_PopupControlMultiChoice(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                _popupControlMultiChoice.InitializeValue_Master(_clickedElement);
                                _popupControlMultiChoice.InitializeView();
                                break;
                            }
                        case "multiplelookup":
                            {
                                if (_clickedElement.Enable)
                                {
                                    SharedView_PopupControlMultiLookup _popupControlMultiLookup = new SharedView_PopupControlMultiLookup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlMultiLookup.InitializeValue_Master(_clickedElement);
                                    _popupControlMultiLookup.InitializeView();
                                }
                                break;
                            }
                        case "number":
                            {
                                if (_clickedElement.Enable == false && String.IsNullOrEmpty(_clickedElement.Value))
                                    return;

                                if (_clickedElement.Enable)
                                {
                                    SharedView_PopupControlNumber _popupControlNumber = new SharedView_PopupControlNumber(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlNumber.InitializeValue_Master(_clickedElement);
                                    _popupControlNumber.InitializeView();
                                }
                                else
                                {
                                    SharedView_PopupControlViewFullInfo _popupControlViewFullInfo = new SharedView_PopupControlViewFullInfo(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlViewFullInfo.InitializeValue_Master(_clickedElement);
                                    _popupControlViewFullInfo.InitializeView();
                                }
                                break;
                            }
                        case "tabs":
                            {
                                break;
                            }
                        case "attachment":
                            break;
                        case "attachmentvertical":
                            break;
                        case "yesno":
                            {
                                if (_clickedElement.Enable)
                                    Handle_ControlYesNo(_clickedElement);
                                break;
                            }
                        case "tree":
                            break;
                        case "attachmentverticalformframe":
                            break;
                        case "textinput":
                        case "textinputmultiline":
                            {
                                if (_clickedElement.Enable)
                                {
                                    SharedView_PopupControlTextInput _popupControlTextInput = new SharedView_PopupControlTextInput(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlTextInput.InitializeValue_Master(_clickedElement);
                                    _popupControlTextInput.InitializeView();
                                }
                                else
                                {
                                    SharedView_PopupControlViewFullInfo _popupControlViewFullInfo = new SharedView_PopupControlViewFullInfo(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlViewFullInfo.InitializeValue_Master(_clickedElement);
                                    _popupControlViewFullInfo.InitializeView();
                                }
                                break;
                            }
                        case "textinputformat": // Text Editor
                            {
                                if (_clickedElement.Enable)
                                {
                                    SharedView_PopupControlTextInputFormat _popupControlTextInputFormat = new SharedView_PopupControlTextInputFormat(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlTextInputFormat.InitializeValue_Master(_clickedElement);
                                    _popupControlTextInputFormat.InitializeView();
                                }
                                break;
                            }
                        case "inputattachmenthorizon":
                        case "inputattachmentvertical": // Đã chuyển qua inneraction
                            {
                                //if (_clickedElement.Enable)
                                //    ShowPopup_ControlInputAttachmentVertical(_clickedElement);
                                break;
                            }
                        case "inputworkrelated": // quy trình liên kết
                                                 //ControlLinkedWorkflow_ShowPopupChooseWorkflow();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ElementChildExpandDetail", ex);
#endif
            }
        }

        private void Click_ElementFormEvent_WithInnerAction(object sender, MinionActionCore.ElementFormClick_WithInnerAction e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    if (e.flagViewID != (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow) // Check xem đúng view không mới thao tác
                    {
                        return;
                    }
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
                                if (_actionID == (int)EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Create)
                                {
                                    _clickedElementAttachment = _clickedElement;

                                    SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView,
                                        CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment, CmmDroidVariable.M_DetailWorkflow_ChooseFileControlAttachment_Camera, (int)SharedView_PopupChooseFile.FlagView.DetailWorkflow_ControlInputAttachmentVertical);
                                    SharedPopUpChooseFile.InitializeView();
                                    //ShowPopup_ControlInputAttachmentVertical(_clickedElement);
                                    break;
                                }
                                else
                                {
                                    ShowPopup_ControlInputAttachmentVertical_InnerAction(_clickedElement, _actionID, _positonToAction);
                                    break;
                                }
                            }
                            break;
                        case "inputgriddetails":
                            {
                                ShowPopup_ControlInputGridDetail_InnerAction(_clickedElement, _actionID, _positonToAction);
                                break;
                            }
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ElementFormEvent_WithInnerAction", ex);
#endif
            }
        }

        private void Click_FlowRelated_WithInnerAction(object sender, MinionActionCore.FlowRelatedClick_WithInnerAction e)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    List<BeanWorkFlowRelated> _lstWorkflowRelated = e.lstWorkflowRelated;
                    int _actionID = e.actionID;
                    int _positonToAction = e.positionToAction;

                    switch (_actionID)
                    {
                        case (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.View: // View Full Item -> mở ra trang Detail mới
                            {
                                BeanWorkFlowRelated _currentRelatedItem = _lstWorkflowRelated[_positonToAction];
                                BeanWorkflowItem _currentWFItem = new BeanWorkflowItem();

                                string queryWorkflow = "SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0";
                                if (_currentRelatedItem.ItemID.ToString().Equals(_workflowItem.ID))
                                {
                                    List<BeanWorkflowItem> lstWFItem = conn.Query<BeanWorkflowItem>(String.Format(queryWorkflow, _currentRelatedItem.ItemRLID));
                                    if (lstWFItem == null || lstWFItem.Count == 0)
                                    {
                                        ProviderControlDynamic p_Dynamic = new ProviderControlDynamic();
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(_workflowItem.ID))
                                            {
                                                lstWFItem = p_Dynamic.getWorkFlowItemByRID(_workflowItem.ID);
                                            }
                                            if (lstWFItem != null || lstWFItem.Count != 0)
                                            {
                                                conn.InsertAll(lstWFItem);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
#if DEBUG
                                            CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleAppBaseItemClick getWorkFlowItemByRID", ex);
#endif
                                        }
                                    }
                                    if (lstWFItem != null && lstWFItem.Count > 0)
                                        _currentWFItem = lstWFItem[0];
                                }
                                else
                                {
                                    List<BeanWorkflowItem> lstWFItem = conn.Query<BeanWorkflowItem>(String.Format(queryWorkflow, _currentRelatedItem.ItemID));
                                    if (lstWFItem == null || lstWFItem.Count == 0)
                                    {
                                        ProviderControlDynamic p_Dynamic = new ProviderControlDynamic();
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(_currentRelatedItem.ItemID.ToString()))
                                            {
                                                lstWFItem = p_Dynamic.getWorkFlowItemByRID(_currentRelatedItem.ItemID.ToString());
                                            }
                                            if (lstWFItem != null || lstWFItem.Count != 0)
                                            {
                                                conn.InsertAll(lstWFItem);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
#if DEBUG
                                            CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleAppBaseItemClick getWorkFlowItemByRID", ex);
#endif
                                        }
                                    }
                                    if (lstWFItem != null && lstWFItem.Count > 0)
                                        _currentWFItem = lstWFItem[0];
                                }


                                FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_currentWFItem, null, "FragmentDetailWorkflow");
                                _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);

                                break;
                            }
                        case (int)EnumFormControlInnerAction.FlowRelated_InnerActionID.Delete: // Xóa file đi
                            {
                                // Nếu là người tạo phiếu cha và phiếu đó là soạn thảo -> mới được xóa
                                // Đã kiểm tra trong Component FlowRelated
                                if (_workflowItem.CreatedBy.Equals(CmmVariable.SysConfig.UserId) && _workflowItem.Status.ToLowerInvariant().Equals("soạn thảo"))
                                {
                                    _lstWorkflowRelated.RemoveAt(_positonToAction);
                                    _LISTFLOWRELATED = _lstWorkflowRelated.ToList();
                                    _adapterDetailExpandControl.NotifyDataSetChanged();
                                }
                                break;
                            }
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_FlowRelated_WithInnerAction", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Tick event khi click vào control trên Popup ControlInputGridDetail
        /// </summary>
        private void Click_ElementGridChildActionClickEvent(object sender, MinionActionCore.ElementGridChildActionClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    if (e.flagView != (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow) // Check xem đúng view không mới thao tác
                    {
                        return;
                    }
                    ViewElement _clickedElement_Parent = e.elementParent;
                    ViewElement _clickedElement_Child = e.elementChild;
                    JObject _jObjectElementChild = e.jObjectChild;

                    switch (_clickedElement_Child.DataType)
                    {
                        case "selectuser":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSelectUserGroup _popupControlDate = new SharedView_PopupControlSelectUserGroup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: false);
                                    _popupControlDate.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlDate.InitializeView();
                                }
                                break;
                            }
                        case "selectusergroup":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSelectUserGroup _popupControlDate = new SharedView_PopupControlSelectUserGroup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: true);
                                    _popupControlDate.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlDate.InitializeView();
                                }
                                break;
                            }
                        case "selectusermulti":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSelectUserGroupMulti _popupControlSelectUserGroupMulti = new SharedView_PopupControlSelectUserGroupMulti(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: false);
                                    _popupControlSelectUserGroupMulti.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlSelectUserGroupMulti.InitializeView();
                                }
                                break;
                            }
                        case "selectusergroupmulti":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSelectUserGroupMulti _popupControlSelectUserGroupMulti = new SharedView_PopupControlSelectUserGroupMulti(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, _isUserAndGroup: true);
                                    _popupControlSelectUserGroupMulti.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlSelectUserGroupMulti.InitializeView();
                                }
                                break;
                            }
                        case "date":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlDate _popupControlDate = new SharedView_PopupControlDate(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlDate.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlDate.InitializeView();
                                }
                                break;
                            }
                        case "datetime":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlDateTime _popupControlDateTime = new SharedView_PopupControlDateTime(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlDateTime.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlDateTime.InitializeView();
                                }
                                break;
                            }
                        case "time":
                            break;
                        case "singlechoice":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSingleChoice _popupControlSingleChoice = new SharedView_PopupControlSingleChoice(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlSingleChoice.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlSingleChoice.InitializeView();
                                }
                                break;
                            }
                        case "singlelookup":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlSingleLookup _popupControlSingleLookup = new SharedView_PopupControlSingleLookup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlSingleLookup.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlSingleLookup.InitializeView();
                                }
                                break;
                            }
                        case "multiplechoice":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlMultiChoice _popupControlMultiChoice = new SharedView_PopupControlMultiChoice(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlMultiChoice.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlMultiChoice.InitializeView();
                                }
                                break;
                            }
                        case "multiplelookup":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlMultiLookup _popupControlMultiLookup = new SharedView_PopupControlMultiLookup(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlMultiLookup.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlMultiLookup.InitializeView();
                                }
                                break;
                            }
                        case "number":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlNumber _popupControlNumber = new SharedView_PopupControlNumber(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlNumber.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlNumber.InitializeView();
                                }
                                break;
                            }
                        case "tabs":
                            {
                                break;
                            }
                        case "attachment":
                        case "attachmentvertical":
                            break;
                        case "yesno":
                            {
                                if (_clickedElement_Child.Enable)
                                    UpdateValueForPopupGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild, _clickedElement_Child.Value); ;
                                break;
                            }
                        case "tree":
                            break;
                        case "attachmentverticalformframe":
                            break;
                        case "textinput":
                        case "textinputmultiline":
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlTextInput _popupControlTextInput = new SharedView_PopupControlTextInput(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlTextInput.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlTextInput.InitializeView();
                                }
                                else
                                {
                                    SharedView_PopupControlViewFullInfo _popupControlViewFullInfo = new SharedView_PopupControlViewFullInfo(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlViewFullInfo.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlViewFullInfo.InitializeView();
                                }
                                break;
                            }
                        case "textinputformat": // Text Editor
                            {
                                if (_clickedElement_Child.Enable)
                                {
                                    SharedView_PopupControlTextInputFormat _popupControlTextInputFormat = new SharedView_PopupControlTextInputFormat(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                                    _popupControlTextInputFormat.InitializeValue_InputGridDetail(_clickedElement_Parent, _clickedElement_Child, _jObjectElementChild);
                                    _popupControlTextInputFormat.InitializeView();
                                    //ShowPopup_ControlTextInputFormat(_clickedElement);
                                }
                                break;
                            }
                        case "inputattachmenthorizon":
                        case "inputattachmentvertical":
                        case "inputworkrelated":
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ElementFormEvent_WithInnerAction", ex);
#endif
            }
        }

        /// <summary>
        /// Event khi click vào file đính kèm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_ElementChildAttachmentExpandDetail(object sender, MinionActionCore.ElementAttachFileClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;
                    KeyValuePair<string, string> _clickedAttachment = e.attachment;
                    switch (_clickedElement.DataType)
                    {
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - Click_ElementChildAttachmentExpandDetail - Error: " + ex.Message);
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
                            Action _actionPositiveButton = new Action(() =>
                            {
                                if (!_lstAttFileControl_Full[positionToAction].ID.Equals("")) // file mới thêm local thì không cần add vào list Remove
                                {
                                    _lstAttFileControl_Deleted.Add(_lstAttFileControl_Full[positionToAction]);
                                }
                                _lstAttFileControl_Full.RemoveAt(positionToAction);
                                UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                            });

                            Action _actionNegativeButton = new Action(() =>
                            {
                                // đã có Dispose trong hàm
                            });

                            CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không ?"),
                            _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                            _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                            _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                            _positive: CmmFunction.GetTitle("TEXT_AGREE", "Agree"),
                            _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
                            break;
                        }
                    case (int)(EnumFormControlInnerAction.ControlInputAttachmentVertical_InnerActionID.Edit): // Edit item trong RecyclerView
                        {
                            List<BeanAttachFileCategory> _lstCategory = JsonConvert.DeserializeObject<List<BeanAttachFileCategory>>(clickedElement.DataSource);

                            string _selectedCategory = "";
                            if (!String.IsNullOrEmpty(_lstAttFileControl_Full[positionToAction].AttachTypeName))
                            {
                                //_selectedCategory = _lstAttFileControl_Full[positionToAction].Category.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                _selectedCategory = _lstAttFileControl_Full[positionToAction].AttachTypeName;
                            }

                            if (_lstCategory != null && _lstCategory.Count > 0)
                            {
                                if (!String.IsNullOrEmpty(_selectedCategory))
                                    for (int i = 0; i < _lstCategory.Count; i++)
                                    {
                                        if (_lstCategory[i].Title.Contains(_selectedCategory))
                                            _lstCategory[i].IsSelected = true;
                                    }
                            }
                            else
                            {
                                _lstCategory = new List<BeanAttachFileCategory>();
                            }

                            #region Get View - Init Data

                            View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                            ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                            TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                            RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                            ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                            _imgDone.Visibility = ViewStates.Invisible;

                            if (!String.IsNullOrEmpty(clickedElement.Title))
                            {
                                _tvTitle.Text = clickedElement.Title;
                            }

                            AdapterFormControlInputAttachmentHorizontal _adapterFormControlSingleChoice = new AdapterFormControlInputAttachmentHorizontal(_mainAct, _rootView.Context, _lstCategory);
                            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                            _recyData.SetAdapter(_adapterFormControlSingleChoice);
                            _recyData.SetLayoutManager(staggeredGridLayoutManager);
                            _adapterFormControlSingleChoice.CustomItemClick += (sender, e) =>
                            {
                                if (!String.IsNullOrEmpty(e.Title))
                                {
                                    //// _lstAttFileControl_Full[positionToAction].Category = e.DocumentTypeID + ";#" + e.DocumentTypeValue;
                                    _lstAttFileControl_Full[positionToAction].AttachTypeId = e.ID;
                                    _lstAttFileControl_Full[positionToAction].AttachTypeName = e.Title;
                                    UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(_lstAttFileControl_Full));
                                    _dialogPopupControl.Dismiss();
                                    _adapterDetailExpandControl.NotifyDataSetChanged();
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

        private void ShowPopup_ControlInputGridDetail_InnerAction(ViewElement clickedElement, int actionID, int positionToAction)
        {
            try
            {
                #region Get View - Event
                View scheduleDetail = _inflater.Inflate(Resource.Layout.PopupControl_InputGridDetail, null);
                TextView _tvTitle = scheduleDetail.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputGridDetail_Title);
                RecyclerView _recyContent = scheduleDetail.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_InputGridDetail_Content);

                ImageView _imgDone = scheduleDetail.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputGridDetail_Done);
                ImageView _imgDelete = scheduleDetail.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputGridDetail_Delete);
                ImageView _imgBack = scheduleDetail.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputGridDetail_Back);

                _tvTitle.Text = clickedElement.Title;

                if (clickedElement.Enable)
                {
                    _imgDelete.Visibility = ViewStates.Visible;
                    _imgDone.Visibility = ViewStates.Visible;
                }
                else
                {
                    _imgDelete.Visibility = ViewStates.Gone;
                    _imgDone.Visibility = ViewStates.Invisible;
                }
                #endregion

                #region Show View
                ////_dialogPopupControlGrid = new Dialog(_rootView.Context);
                _dialogPopupControlGrid = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                Window window = _dialogPopupControlGrid.Window;
                _dialogPopupControlGrid.RequestWindowFeature(1);
                _dialogPopupControlGrid.SetCanceledOnTouchOutside(false);
                _dialogPopupControlGrid.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                var dm = Resources.DisplayMetrics;

                _dialogPopupControlGrid.SetContentView(scheduleDetail);
                _dialogPopupControlGrid.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels;
                s.Height = WindowManagerLayoutParams.MatchParent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Data

                _imgBack.Click += delegate
                {
                    _dialogPopupControlGrid.Dismiss();
                };

                if (actionID == (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Edit)
                {
                    List<JObject> ListJObjectRow = new List<JObject>();
                    try
                    {
                        if (!String.IsNullOrEmpty(clickedElement.Value))
                            ListJObjectRow = JsonConvert.DeserializeObject<List<JObject>>(clickedElement.Value);
                    }
                    catch (Exception)
                    {
                        ListJObjectRow = new List<JObject>();
                    }

                    _adapterTemplateControlGrid = new AdapterRecyTemplateValueType(_mainAct, _rootView.Context, clickedElement, ListJObjectRow[positionToAction], (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow);
                    _recyContent.SetAdapter(_adapterTemplateControlGrid);
                    _recyContent.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));

                    _imgDelete.Click += (sender, e) => // edit mới có xóa
                    {
                        try
                        {
                            Action _actionPositiveButton = new Action(() =>
                            {
                                _lstGridDetail_Deleted.Add(ListJObjectRow[positionToAction]);

                                ListJObjectRow.RemoveAt(positionToAction);
                                UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(ListJObjectRow));
                                _dialogPopupControlGrid.Dismiss();
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
                        catch (Exception) { }
                    };

                    _imgDone.Click += (sender, e) =>
                    {
                        try
                        {
                            JObject _currentJObject = _adapterTemplateControlGrid.GetCurrentJObject();

                            foreach (BeanWFDetailsHeader item in _adapterTemplateControlGrid._lstHeader)
                            {
                                if (item.require == true && String.IsNullOrEmpty(_currentJObject[item.internalName.ToString()].ToString()))
                                {
                                    // kiểm tra xem có thằng nào Require mà chưa nhập không
                                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Vui lòng nhập đầy đủ thông tin."),
                                                             CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Please fill all data."));
                                    return;
                                }
                            }

                            ListJObjectRow[positionToAction] = _currentJObject;
                            UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(ListJObjectRow));
                            _dialogPopupControlGrid.Dismiss();
                        }
                        catch (Exception) { }
                    };
                }
                else if (actionID == (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Create)
                {
                    _imgDelete.Visibility = ViewStates.Gone;

                    List<JObject> ListJObjectRow = new List<JObject>();
                    try
                    {
                        if (!String.IsNullOrEmpty(clickedElement.Value))
                            ListJObjectRow = JsonConvert.DeserializeObject<List<JObject>>(clickedElement.Value);
                    }
                    catch (Exception)
                    {
                        ListJObjectRow = new List<JObject>();
                    }

                    _adapterTemplateControlGrid = new AdapterRecyTemplateValueType(_mainAct, _rootView.Context, clickedElement, (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow);
                    _recyContent.SetAdapter(_adapterTemplateControlGrid);
                    _recyContent.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));

                    _imgDone.Click += (sender, e) =>
                    {
                        JObject _currentJObject = _adapterTemplateControlGrid.GetCurrentJObject();

                        foreach (BeanWFDetailsHeader item in _adapterTemplateControlGrid._lstHeader)
                        {
                            if (item.require == true && String.IsNullOrEmpty(_currentJObject[item.internalName.ToString()].ToString()))
                            {
                                // kiểm tra xem có thằng nào Require mà chưa nhập không
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Vui lòng nhập đầy đủ thông tin."),
                                                         CmmFunction.GetTitle("K_Action_PleaseChooseUser", "Please fill all data."));
                                return;
                            }
                        }
                        ListJObjectRow.Add(_currentJObject);

                        UpdateValueForElement(clickedElement, JsonConvert.SerializeObject(ListJObjectRow));
                        _dialogPopupControlGrid.Dismiss();
                    };
                }
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlInputGridDetail_InnerAction", ex);
#endif
            }
        }

        private void Handle_ControlYesNo(ViewElement clickedElement)
        {
            try
            {
                UpdateValueForElement(clickedElement, clickedElement.Value.ToString(), false);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Handle_ControlYesNo", ex);
#endif
            }
        }

        public void UpdateValueForElement(ViewElement clickedElement, string _newValue, bool _NotifyDataSetChanged = true)
        {
            try
            {
                clickedElement.Value = _newValue;
                CTRLDetailWorkflow.UpdateValueElement_InListSection(ref _LISTSECTION, clickedElement, _reCalculated: true);
                UpdateItemForEditedElement(clickedElement);
                _adapterDetailExpandControl.UpdateCurrentListSection(_LISTSECTION);
                if (_NotifyDataSetChanged)
                    _adapterDetailExpandControl.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateValueForElement", ex);
#endif
            }
        }

        /// <summary>
        /// Update giá trị JObject cho popup item dc click vào. => chưa cập nhật vào list chính
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="childElement"></param>
        /// <param name="jObjectChild"></param>
        /// <param name="_newValue"></param>
        public void UpdateValueForPopupGridDetail(ViewElement parentElement, ViewElement childElement, JObject jObjectChild, string _newValue)
        {
            try
            {
                if (_adapterTemplateControlGrid != null) // Trường hợp edit
                {
                    int _flagAction = _adapterTemplateControlGrid.GetCurrentFlagAction();

                    switch (_flagAction)
                    {
                        case (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Create: // Create ko trả ra item theo position -> lấy từ adapter ra
                            {
                                jObjectChild = _adapterTemplateControlGrid.GetCurrentJObject();
                                break;
                            }
                        case (int)EnumFormControlInnerAction.ControlInputGridDetails_InnerActionID.Edit:
                            {
                                jObjectChild = _adapterTemplateControlGrid.GetCurrentJObject();
                                break;
                            }
                    }
                    try
                    {
                        switch (jObjectChild[childElement.InternalName.ToString()].Type) // kiểm tra type của Jtoken cần Update
                        {
                            case JTokenType.Integer:
                            case JTokenType.Float:
                                {
                                    jObjectChild[childElement.InternalName.ToString()] = double.Parse(_newValue);
                                    break;
                                }
                            case JTokenType.String:
                            default: // default = string
                                {
                                    jObjectChild[childElement.InternalName.ToString()] = _newValue;
                                    break;
                                }
                        }
                    }
                    catch (Exception)
                    {
                        jObjectChild[childElement.InternalName.ToString()] = _newValue;
                    }

                    _adapterTemplateControlGrid.UpdateCurrentJObject(jObjectChild);
                    _adapterTemplateControlGrid.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateValueForElement", ex);
#endif
            }
        }

        /// <summary>
        /// Update Item vào List đã được Edit để send Action đi
        /// </summary>
        /// <param name="EditedElement"></param>
        private void UpdateItemForEditedElement(ViewElement editedElement)
        {
            try
            {
                if (_lstEditedElement == null || _lstEditedElement.Count <= 0)
                    _lstEditedElement = new List<ViewElement>();

                // Nếu Element này đã có trong List Edited -> Update
                for (int i = 0; i < _lstEditedElement.Count; i++)
                {
                    if (_lstEditedElement[i].ID.Equals(editedElement.ID)) // Đã có trong List
                    {
                        _lstEditedElement[i] = editedElement;
                        return;
                    }
                }
                _lstEditedElement.Add(editedElement); // chưa có trong List

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateItemForEditedElement", ex);
#endif
            }
        }
        #endregion

        #region Dynamic Action
        private void Click_ElementActionEvent(object sender, MinionActionCore.ElementActionClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true) // không cho click nhiều lần
                {
                    ViewElement _clickedElement = e.element;

                    if (_clickedElement.ID.ToLowerInvariant().Equals("more") || _clickedElement.Value.ToLowerInvariant().Equals("more")) // Action More
                    {
                        List<ButtonAction> _lstActionMore = _componentButtonBot._lstActionMore;
                        if (_lstActionMore != null && _lstActionMore.Count > 0)
                        {
                            SharedView_PopupActionMore _popupActionMore = new SharedView_PopupActionMore(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                            _popupActionMore.InitializeValue_DetailWorkflow_ActionMore(_lstActionMore);
                            _popupActionMore.InitializeView();
                            //ShowPopup_ControlActionMore(_lstActionMore);
                        }
                    }
                    else // Các Action khác
                    {
                        ButtonAction _clickedBtnAction = new ButtonAction { ID = Convert.ToInt32(_clickedElement.ID), Title = _clickedElement.Title, Value = _clickedElement.Value, Notes = _clickedElement.Notes };
                        Click_Action(_clickedBtnAction);
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ElementAction", ex);
#endif
            }
        }

        public void Click_Action(ButtonAction buttonAction)
        {
            try
            {
                if (CmmFunction.ValidateRequiredForm(_LISTSECTION) == false) // có trường chưa nhập
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_FIELD_REQUIRE", "Vui lòng nhập đầy đủ thông tin."),
                                             CmmFunction.GetTitle("MESS_FIELD_REQUIRE", "Please enter all fill required"));

                    return;
                }
                switch (buttonAction.ID)
                {
                    case (int)(WorkflowAction.Action.Next):  // 1 - Action Duyệt
                    case (int)(WorkflowAction.Action.Approve): // 2 - Action Phê duyệt
                        {
                            SharedView_PopupActionAccept _popupActionAccept = new SharedView_PopupActionAccept(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                            _popupActionAccept.InitializeValue_DetailWorkflow(buttonAction);
                            _popupActionAccept.InitializeView();
                            break;
                        }
                    case (int)(WorkflowAction.Action.Forward): // 4 - Action Chuyển xử lý
                    case (int)(WorkflowAction.Action.RequestIdea): // 9 tham vấn ý kiến
                        {
                            SharedView_PopupActionForward _popupActionForward = new SharedView_PopupActionForward(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, 0.9f);
                            _popupActionForward.InitializeValue_DetailWorkflow(buttonAction);
                            _popupActionForward.InitializeView();
                            break;
                        }
                    case (int)(WorkflowAction.Action.Return): // 8 - Action Yêu cầu hiệu chỉnh
                    case (int)(WorkflowAction.Action.Reject): // 16 - Action Từ chối
                    case (int)(WorkflowAction.Action.Cancel): // 51 - Action Hủy
                    case (int)(WorkflowAction.Action.Idea): // 10 cho ý kiến -> bắt buộc có ý kiến -> giống từ chối
                        {
                            SharedView_PopupActionReject _popupActionReject = new SharedView_PopupActionReject(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView);
                            _popupActionReject.InitializeValue_DetailWorkflow(buttonAction);
                            _popupActionReject.InitializeView();
                            break;
                        }
                    case (int)(WorkflowAction.Action.RequestInformation):// 7 - Action bổ sung thông tin
                        {
                            List<BeanUser> _lstUserAll = GetListUserFromQTLC(_lstQTLC); // Phải lấy những User từ bước trước -> Khác các Action khác

                            SharedView_PopupActionRequestInfo _popupActionRequestInfo = new SharedView_PopupActionRequestInfo(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView, 0.9f, _lstUserAll);
                            _popupActionRequestInfo.InitializeValue_DetailWorkflow(buttonAction);
                            _popupActionRequestInfo.InitializeView();
                            break;
                        }
                    case (int)(WorkflowAction.Action.CreateTask): // 54 - tạo task
                        {
                            FragmentDetailCreateTask fragmentDetailCreateTask = new FragmentDetailCreateTask(this, -1, true);
                            _mainAct.AddFragment(FragmentManager, fragmentDetailCreateTask, "FragmentDetailCreateTask", 0);
                            break;
                        }
                    default: // 32 - Action Thu hồi
                        {
                            Action_SendAPI(buttonAction, "", null);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Action", ex);
#endif
            }
        }

        /// <summary>
        /// Hàm send API lên để xử lý trên Server
        /// </summary>
        /// <param name="buttonAction">ButtonAction tương ứng</param>
        /// <param name="comment">ý kiến của Action nếu có</param>
        /// <param name="_lstExtent">List các column thêm nếu cần như: uservalues, ...</param>
        /// <param name="IsFragmentDetailWorkflow">check xem trang thực hiện API là trang nào</param>
        public async void Action_SendAPI(ButtonAction buttonAction, string comment, List<KeyValuePair<string, string>> _lstExtent)
        {
            try
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(async () =>
                {
                    bool _result = false;
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    List<BeanAttachFile> _lstAttachmentLocal = new List<BeanAttachFile>();

                    #region Get FormDefineInfo
                    JArray jArrayForm = JArray.Parse(_OBJFORMACTION["form"].ToString());
                    string _formDefineInfo = jArrayForm[0]["FormDefineInfo"].ToString();

                    // Nếu Action Có comment -> Add thêm cột idea
                    if (!string.IsNullOrEmpty(comment))
                    {
                        KeyValuePair<string, string> _KeyValueComment = new KeyValuePair<string, string>("idea", comment);
                        if (_lstExtent == null) _lstExtent = new List<KeyValuePair<string, string>>();
                        _lstExtent.Add(_KeyValueComment);
                    }
                    #endregion

                    #region Get Edited Json
                    List<ObjectSubmitAction> _lstSubmitActionData = new List<ObjectSubmitAction>();
                    for (int i = 0; i < _lstEditedElement.Count; i++)
                    {
                        if (_lstEditedElement[i].DataType.Equals("inputattachmenthorizon"))
                        {
                            _lstAttachmentLocal = JsonConvert.DeserializeObject<List<BeanAttachFile>>(_lstEditedElement[i].Value).Where(x => x.ID.Equals("")).ToList();

                            // Nếu inputattachmenthorizon -> tạo 2 Object: Object current List và Object Deleted
                            ObjectSubmitAction _beanSubmitCurrent = new ObjectSubmitAction() // Object của những Attach File còn lại trong List.
                            {
                                ID = _lstEditedElement[i].ID,
                                Value = _lstEditedElement[i].Value,
                                TypeSP = "Attachment",
                                DataType = _lstEditedElement[i].DataType
                            };
                            _lstSubmitActionData.Add(_beanSubmitCurrent);

                            if (_lstAttFileControl_Deleted != null && _lstAttFileControl_Deleted.Count > 0) // Nếu có xóa mới có list này
                            {
                                // Loại bỏ những item Local ra
                                _lstAttFileControl_Deleted = _lstAttFileControl_Deleted.Where(x => !x.ID.Equals("")).ToList();

                                ObjectSubmitAction _beanSubmitDeleted = new ObjectSubmitAction() // Object của những Attach File đã bị xóa khỏi List.
                                {
                                    ID = _lstEditedElement[i].ID,
                                    Value = JsonConvert.SerializeObject(_lstAttFileControl_Deleted),
                                    TypeSP = "RemoveAttachment",
                                    DataType = _lstEditedElement[i].DataType
                                };
                                _lstSubmitActionData.Add(_beanSubmitDeleted);
                            }
                        }
                        else if (_lstEditedElement[i].DataType.Equals("inputgriddetails"))
                        {
                            // Nếu inputgriddetails -> tạo 2 Object: Object current List và Object Deleted
                            ObjectSubmitAction _beanSubmitCurrent = new ObjectSubmitAction() // Object của những Attach File còn lại trong List.
                            {
                                ID = _lstEditedElement[i].ID,
                                Value = _lstEditedElement[i].Value,
                                TypeSP = "GridDetails",
                                DataType = _lstEditedElement[i].DataType // inputgriddetails
                            };
                            _lstSubmitActionData.Add(_beanSubmitCurrent);

                            if (_lstGridDetail_Deleted != null && _lstGridDetail_Deleted.Count > 0) // Nếu có xóa mới có list này
                            {
                                // Loại bỏ những item Local ra
                                _lstGridDetail_Deleted = _lstGridDetail_Deleted.Where(x => int.Parse(x["ID"].ToString()) != 0).ToList(); // Add ID = 0 trong AdapterRecyTemplateValueType()

                                ObjectSubmitAction _beanSubmitDeleted = new ObjectSubmitAction() // Object của những Row đã bị xóa khỏi List.
                                {
                                    ID = _lstEditedElement[i].ID,
                                    Value = JsonConvert.SerializeObject(_lstGridDetail_Deleted),
                                    TypeSP = "RemoveGridDetails",
                                    DataType = _lstEditedElement[i].DataType
                                };
                                _lstSubmitActionData.Add(_beanSubmitDeleted);
                            }
                        }
                        else // Cac control khác -> tạo bình thường
                        {
                            ObjectSubmitAction _beanSubmitActionData = new ObjectSubmitAction()
                            {
                                ID = _lstEditedElement[i].ID,
                                Value = _lstEditedElement[i].Value,
                                TypeSP = _lstEditedElement[i].TypeSP,
                                DataType = _lstEditedElement[i].DataType
                            };
                            _lstSubmitActionData.Add(_beanSubmitActionData);
                        }
                    }
                    #endregion

                    #region Get Import Attach File
                    List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();

                    if (_lstAttachmentLocal != null && _lstAttachmentLocal.Count > 0)
                    {
                        foreach (var item in _lstAttachmentLocal)
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

                    string _messageAPI = "";

                    _result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _workflowItem.ID, _formDefineInfo, JsonConvert.SerializeObject(_lstSubmitActionData), ref _messageAPI, _lstKeyVarAttachmentLocal, _lstExtent);

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
                            if (!String.IsNullOrEmpty(_messageAPI))
                                CmmDroidFunction.ShowAlertDialog(_mainAct, _messageAPI, _messageAPI);
                            else
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Action_SendAPI", ex);
#endif
            }
        }
        #endregion

        #region Component Task List
        private void Click_ComponentTaskList_Item(object sender, MinionActionCore.TaskListItemClick e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true && e._viewID == 1) // View Chi tiết
                {
                    FragmentDetailCreateTask fragmentDetailCreateTask = new FragmentDetailCreateTask(this, e._clickedItem.groupItem.ID, false);
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

        #region Component Comment

        /// <summary>
        /// Xem chi tiết file đính kèm list hiện hành ở phần Parent Comment
        /// </summary>
        private void Click_ComponentComment_ParentAttach_Detail(object sender, BeanAttachFile e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (!String.IsNullOrEmpty(e.Url)) // check live
                    {
                        CmmDroidFunction.DownloadAndOpenFile(_mainAct, _rootView.Context, CmmVariable.M_Domain + e.Url); // trường hợp đặc biệt xài URL
                    }
                    else if (System.IO.File.Exists(e.Path))  // check local
                        CmmDroidFunction.OpenFile(_mainAct, _rootView.Context, e.Path);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Detail", ex);
#endif
            }
        }

        /// <summary>
        /// bấm vào nút xóa file đính kèm list hiện hành ở phần Parent Comment
        /// </summary>
        private void Click_ComponentComment_ParentAttach_Delete(object sender, BeanAttachFile e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (e != null)
                    {
                        _lstAttachComment = _lstAttachComment.Where(x => !x.Path.Equals(e.Path)).ToList();
                        if (_adapterDetailExpandControl != null)
                        {
                            _adapterDetailExpandControl.UpdateListAttachComment(_lstAttachComment);
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

        /// <summary>
        /// bấm vào nút đính kèm file ở phần Parent Comment
        /// </summary>
        private void Click_ComponentComment_ParentComment_ImgAttach(object sender, CommentEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    SharedView_PopupChooseFile SharedPopUpChooseFile = new SharedView_PopupChooseFile(_inflater, _mainAct, this, "FragmentDetailWorkflow", _rootView,
                        CmmDroidVariable.M_DetailWorkflow_ChooseFileComment, CmmDroidVariable.M_DetailWorkflow_ChooseFileComment_Camera, (int)SharedView_PopupChooseFile.FlagView.DetailWorkflow_Comment);
                    SharedPopUpChooseFile.InitializeView();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemExpandComment_Attach_Delete", ex);
#endif
            }
        }

        /// <summary>
        /// bấm vào nút Comment Parent -> gọi API
        /// </summary>
        private async void Click_ComponentComment_ParentComment_ImgComment(object sender, CommentEventArgs e)
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
                        beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.WorkflowItem;
                        beanOtherResource.ResourceSubCategoryId = 0;
                        beanOtherResource.Image = "";
                        beanOtherResource.ParentCommentId = null; // cmt mới nên ko có parent

                        List<BeanAttachFile> _lstAttachCommentParent = _adapterDetailExpandControl.GetListAttachComment();
                        List<KeyValuePair<string, string>> _lstKeyValueAttach = CTRLDetailWorkflow.CloneKeyValueFromListAtt(_lstAttachCommentParent);

                        bool _result = _pControlDynamic.AddComment(beanOtherResource, _lstKeyValueAttach);
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            if (_result == true)
                            {
                                _lstAttachComment = new List<BeanAttachFile>();
                                GetAndSetDataFromServer();
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
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SendAPI_CommentParent", ex);
#endif
            }
        }
        #endregion

        #endregion
    }
}