using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allyants.BoardViewLib;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
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
using Com.Telerik.Widget.Calendar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using static BPMOPMobile.Droid.Presenter.Adapter.AdapterBoardDetailGroup;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    public class FragmentBoardDetailGroup_wfitem : CustomBaseFragment, BoardView.IDragItemStartCallback, BoardView.IItemClickListener
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private PopupWindow _popupFilter;
        private Dialog _dialogAction;
        private View _rootView, _popupViewFilter;
        private EditText _edtSearch;
        private TextView _tvTitle, _tvNoDataBoard, _tvNoDataList, _tvNoDataReport, _tvBoard, _tvList, _tvReport;
        private ImageView _imgBack, _imgBoard, _imgList, _imgReport, _imgDelete;
        private RelativeLayout _relaToolBar, _relaBoard, _relaList, _relaReport, _relaDataBoard, _relaDataList, _relaDataReport;
        private LinearLayout _lnFilter, _lnBlackFilter, _lnNoDataBoard, _lnNoDataList, _lnNoDataReport, _lnBottomNavigation;
        private View _vwBoard, _vwList, _vwReport;
        private CircleImageView _imgAvatar;
        private SwipeRefreshLayout _swipe;
        private MyCustomBoardView _boardView;
        private RecyclerView _recyList, _recyReport;

        private AdapterBoardDetailGroupLibrary _adapterViewBoard;
        private AdapterHomePageRecyVTBD_Ver2 _adapterRecyList;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerBoard CTRLBoard = new ControllerBoard();

        private List<BeanWorkflowStepDefine> _lstStepDefine = new List<BeanWorkflowStepDefine>(); // Chỉ bao gồm bước của Quy Trình
        private List<BeanWorkflowItem> _lstWorkflowItem_Full = new List<BeanWorkflowItem>();
        private List<BeanBoardStepDefine> _lstStepDefine_Full = new List<BeanBoardStepDefine>();

        private List<BeanWorkflowItem> _lstWorkflowItem_Filter = new List<BeanWorkflowItem>();
        private List<BeanBoardStepDefine> _lstStepDefine_Filter = new List<BeanBoardStepDefine>();

        private BeanWorkflow _beanWorkflow = new BeanWorkflow();
        private int _flagCurrentPage = 1; // 1 = Board, 2 = List, 3 = Report

        public FragmentBoardDetailGroup_wfitem() { }

        public FragmentBoardDetailGroup_wfitem(BeanWorkflow _beanWorkflow, int _flagCurrentPage)
        {
            this._beanWorkflow = _beanWorkflow;
            this._flagCurrentPage = _flagCurrentPage;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            //_mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent;
            MinionAction.RenewFragmentBoardDetailGroup -= MinionAction_FragmentBoardDetailGroup;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.ViewBoardDetailGroup, null);
            _mainAct = (MainActivity)this.Activity;
            _inflater = inflater;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView != null)
            {
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Back);

                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewBoardDetailGroup);
                _relaToolBar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Toolbar);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_BlackFilter);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Filter);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewBoardDetailGroup_Search);
                _imgDelete = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Search_Delete);
                _tvBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Board);
                _tvList = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_List);
                _tvReport = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Report);
                _imgBoard = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Board);
                _imgList = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_List);
                _imgReport = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Report);
                _boardView = _rootView.FindViewById<MyCustomBoardView>(Resource.Id.boardView_ViewBoardDetailGroup);
                _recyList = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewBoardDetailGroup_List);
                _recyReport = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewBoardDetailGroup_Report);
                _relaBoard = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Board);
                _relaList = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_List);
                _relaReport = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Report);
                _relaDataBoard = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_Board);
                _relaDataList = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_List);
                _relaDataReport = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_Report);
                _lnNoDataBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_Board_NoData);
                _lnNoDataList = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_List_NoData);
                _lnNoDataReport = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_Report_NoData);
                _tvNoDataBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Board_NoData);
                _tvNoDataList = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_List_NoData);
                _tvNoDataReport = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Report_NoData);
                _vwBoard = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Board);
                _vwList = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_List);
                _vwReport = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Report);

                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_BottomNavigation);
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                _swipe.Refresh += Swipe_RefreshData;
                _imgBack.Click += Click_imgBack;
                _imgDelete.Click += Click_imgDelete;
                _lnFilter.Click += Click_lnFilter;
                _relaBoard.Click += Click_relaBoard;
                _relaList.Click += Click_relaList;
                _relaReport.Click += Click_relaReport;
                _edtSearch.TextChanged += TextChanged_EdtSearch;
                _edtSearch.EditorAction += EditorAction_EdtSearch;

                _boardView.SetOnItemClickListener(this);
                _boardView.SetOnDragItemListener(this);

                SetViewNavigationByCurrentPage(); // Set View bằng _flagCurrentPage
                _imgDelete.Visibility = ViewStates.Gone;

                MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board;
                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation);
                bottomNavigation.InitializeView();
            }
            SetViewByLanguage();
            GetNewSetData();
            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent;
            MinionAction.RenewFragmentBoardDetailGroup += MinionAction_FragmentBoardDetailGroup;
            return _rootView;
        }

        private void MinionAction_FragmentBoardDetailGroup(object sender, EventArgs e)
        {
            try
            {
                SetData();
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_FragmentBoardDetailGroup", ex);
#endif
            }
        }

        private void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                if (e != null)
                {
                    //_mainAct.RunOnUiThread(() => 
                    //{
                    //    SetData();
                    //});
                    GetNewSetData(false);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_RenewItem_AfterFollowEvent", ex);
#endif
            }
        }

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (_beanWorkflow != null)
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvTitle.Text = !string.IsNullOrEmpty(_beanWorkflow.Title) ? _beanWorkflow.Title : "";
                        _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                    }
                    else
                    {
                        _tvTitle.Text = !string.IsNullOrEmpty(_beanWorkflow.TitleEN) ? _beanWorkflow.TitleEN : "";
                        _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search");
                    }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void SetViewNavigationByCurrentPage()
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                switch (_flagCurrentPage)
                {
                    case 1:
                        {
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgBoard, _tvBoard, _vwBoard);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgList, _tvList, _vwList, 2);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgReport, _tvReport, _vwReport, 3);

                            _relaDataBoard.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                            _relaDataBoard.Visibility = ViewStates.Visible;
                            _relaDataList.Visibility = ViewStates.Gone;
                            _relaDataReport.Visibility = ViewStates.Gone;
                            break;
                        }
                    case 2:
                        {
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgBoard, _tvBoard, _vwBoard, 1);
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgList, _tvList, _vwList);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgReport, _tvReport, _vwReport, 3);

                            _relaDataList.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                            _relaDataBoard.Visibility = ViewStates.Gone;
                            _relaDataList.Visibility = ViewStates.Visible;
                            _relaDataReport.Visibility = ViewStates.Gone;
                            break;
                        }
                    case 3:
                        {
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgBoard, _tvBoard, _vwBoard, 1);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgList, _tvList, _vwList, 2);
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgReport, _tvReport, _vwReport);

                            _relaDataBoard.Visibility = ViewStates.Gone;
                            _relaDataList.Visibility = ViewStates.Gone;
                            _relaDataReport.Visibility = ViewStates.Visible;
                            break;
                        }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Back", ex);
#endif
            }
        }

        private void Click_imgDelete(object sender, EventArgs e)
        {
            try
            {
                _edtSearch.Text = "";
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Back", ex);
#endif
            }
        }

        private void Click_lnFilter(object sender, EventArgs e)
        {
            try
            {
                string _flagTuNgay = "", _flagDenNgay = "", _flagTrangThai = "";
                List<KeyValuePair<string, string>> _lstCurrentValueFilter = CTRLBoard.GetCurrentValue_Filter();
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                    LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                    #region Get View - Init Data
                    _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupViewBoardDetailGroupFilter, null);
                    _popupFilter = new PopupWindow(_popupViewFilter, _displayMetrics.WidthPixels, WindowManagerLayoutParams.WrapContent);

                    TextView _tvToday = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_Today);
                    TextView _tvYesterday = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_Yesterday);
                    TextView _tv7Days = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_7Days);
                    TextView _tv30Days = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_30Days);
                    TextView _tvNgayTao = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_NgayTao);

                    LinearLayout _lnNgayTao_TuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupViewBoardDetailGroupFilter_NgayTao_TuNgay);
                    RadCalendarView _calendarTuNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupViewBoardDetailGroupFilter_TuNgay);
                    ImageView _imgNgayTao_TuNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupViewBoardDetailGroupFilter_NgayTao_TuNgay);
                    TextView _tvNgayTao_TuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_NgayTao_TuNgay);

                    LinearLayout _lnNgayTao_DenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupViewBoardDetailGroupFilter_NgayTao_DenNgay);
                    RadCalendarView _calendarDenNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupViewBoardDetailGroupFilter_DenNgay);
                    ImageView _imgNgayTao_DenNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupViewBoardDetailGroupFilter_NgayTao_DenNgay);
                    TextView _tvNgayTao_DenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_NgayTao_DenNgay);

                    TextView _tvTinhTrang = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_TinhTrang);
                    TextView _tvTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_TinhTrang_TatCa);
                    TextView _tvChoPheDuyet = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_TinhTrang_ChoPheDuyet);
                    TextView _tvDaPheDuyet = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_TinhTrang_DaPheDuyet);
                    TextView _tvTuChoi = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_TinhTrang_TuChoi);

                    TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_ApDung);
                    TextView _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_MacDinh);

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvToday.Text = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
                        _tv7Days.Text = CmmFunction.GetTitle("TEXT_7DAYS", "7 ngày");
                        _tv30Days.Text = CmmFunction.GetTitle("TEXT_30DAYS", "30 ngày");
                        _tvYesterday.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                        _tvNgayTao.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Ngày khởi tạo");
                        _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");
                        _tvTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                        _tvChoPheDuyet.Text = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Chờ duyệt");
                        _tvDaPheDuyet.Text = CmmFunction.GetTitle("TEXT_APPROVED", "Đã duyệt");
                        _tvTuChoi.Text = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
                        _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Thiết lập lại");
                        _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");
                    }
                    else
                    {
                        _tvToday.Text = CmmFunction.GetTitle("TEXT_TODAY", "Today");
                        _tv7Days.Text = CmmFunction.GetTitle("TEXT_7DAYS", "7 days");
                        _tv30Days.Text = CmmFunction.GetTitle("TEXT_30DAYS", "30 days");
                        _tvYesterday.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Yesterday");
                        _tvNgayTao.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Created date");
                        _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Status");
                        _tvTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "All");
                        _tvChoPheDuyet.Text = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Waiting");
                        _tvDaPheDuyet.Text = CmmFunction.GetTitle("TEXT_APPROVED", "Approved");
                        _tvTuChoi.Text = CmmFunction.GetTitle("TEXT_REJECT", "Rejected");
                        _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Setting");
                        _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Apply");
                    }


                    DateTime _tempTuNgay = DateTime.Now, _tempDenNgay = DateTime.Now;
                    foreach (KeyValuePair<string, string> _item in _lstCurrentValueFilter)
                    {
                        switch (_item.Key)
                        {
                            case "Trạng thái":
                                {
                                    _flagTrangThai = _item.Value;

                                    if (_item.Value.Equals("1"))
                                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTatCa);
                                    else if (_item.Value.Equals("2"))
                                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvChoPheDuyet);
                                    else if (_item.Value.Equals("3"))
                                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvDaPheDuyet);
                                    else if (_item.Value.Equals("4"))
                                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTuChoi);
                                    break;
                                }
                            case "Từ ngày":
                                {
                                    try
                                    {
                                        _flagTuNgay = _item.Value.ToString();
                                        _tempTuNgay = DateTime.ParseExact(_item.Value, "dd/MM/yyyy", null);
                                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(_tempTuNgay);
                                    }
                                    catch (System.Exception) { }
                                    break;
                                }
                            case "Đến ngày":
                                {
                                    try
                                    {
                                        _flagDenNgay = _item.Value.ToString();
                                        _tempDenNgay = DateTime.ParseExact(_item.Value, "dd/MM/yyyy", null);
                                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(_tempDenNgay);
                                    }
                                    catch (System.Exception) { }
                                    break;
                                }
                        }
                    }
                    CTRLBoard.InitTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, _tempTuNgay, _tempDenNgay); // Để init màu Text
                    CTRLBoard.InitRadCalendarView(_calendarTuNgay, _tvNgayTao_TuNgay);
                    CTRLBoard.InitRadCalendarView(_calendarDenNgay, _tvNgayTao_DenNgay);
                    #endregion

                    #region Event
                    _tvToday.Click += (sender, e) =>
                    {
                        _flagTuNgay = DateTime.Now.ToString("dd/MM/yyyy");
                        _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 1);
                    };
                    _tvYesterday.Click += (sender, e) =>
                    {
                        _flagTuNgay = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                        _flagDenNgay = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");

                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-1));
                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-1));
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 2);
                    };
                    _tv7Days.Click += (sender, e) =>
                    {
                        _flagTuNgay = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                        _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-7));
                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 3);
                    };
                    _tv30Days.Click += (sender, e) =>
                    {
                        _flagTuNgay = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                        _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddMonths(-1));
                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 4);
                    };

                    _lnNgayTao_TuNgay.Click += (sender, e) =>
                    {
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 0);
                        CTRLBoard.SetLinearCalendar(_mainAct, _calendarTuNgay, _lnNgayTao_TuNgay, _imgNgayTao_TuNgay, _calendarDenNgay, _lnNgayTao_DenNgay, _imgNgayTao_DenNgay, 1);
                        if (!string.IsNullOrEmpty(_flagTuNgay))
                        {
                            DateTime _tempTuNgay = DateTime.Now;
                            try
                            {
                                _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                            }
                            catch (System.Exception)
                            {
                                _tempTuNgay = DateTime.Now;
                            }
                            Calendar calendar = new GregorianCalendar(_tempTuNgay.Year, _tempTuNgay.Month - 1, _tempTuNgay.Day);
                            _calendarTuNgay.DisplayDate = calendar.TimeInMillis;
                        }
                    };
                    _lnNgayTao_DenNgay.Click += (sender, e) =>
                    {
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 0);
                        CTRLBoard.SetLinearCalendar(_mainAct, _calendarTuNgay, _lnNgayTao_TuNgay, _imgNgayTao_TuNgay, _calendarDenNgay, _lnNgayTao_DenNgay, _imgNgayTao_DenNgay, 2);
                        if (!string.IsNullOrEmpty(_flagDenNgay))
                        {
                            DateTime _tempDenNgay = DateTime.Now;
                            try
                            {
                                _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                            }
                            catch (System.Exception)
                            {
                                _tempDenNgay = DateTime.Now;
                            }

                            Calendar calendar = new GregorianCalendar(_tempDenNgay.Year, _tempDenNgay.Month - 1, _tempDenNgay.Day);
                            _calendarDenNgay.DisplayDate = calendar.TimeInMillis;
                        }
                    };
                    _tvNgayTao_TuNgay.TextChanged += (sender, e) =>
                    {
                        DateTime _temp = DateTime.ParseExact(_tvNgayTao_TuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                        _flagTuNgay = _temp.ToString("dd/MM/yyyy");
                    };
                    _tvNgayTao_DenNgay.TextChanged += (sender, e) =>
                    {
                        DateTime _temp = DateTime.ParseExact(_tvNgayTao_DenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                        _flagDenNgay = _temp.ToString("dd/MM/yyyy");
                    };

                    _tvTatCa.Click += (sender, e) =>
                    {
                        _flagTrangThai = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvChoPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvDaPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTuChoi);
                    };
                    _tvChoPheDuyet.Click += (sender, e) =>
                    {
                        _flagTrangThai = "2";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvChoPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvDaPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTuChoi);
                    };
                    _tvDaPheDuyet.Click += (sender, e) =>
                    {
                        _flagTrangThai = "3";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvChoPheDuyet);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvDaPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTuChoi);
                    };
                    _tvTuChoi.Click += (sender, e) =>
                    {
                        _flagTrangThai = "4";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvChoPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvDaPheDuyet);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTuChoi);
                    };

                    _tvMacDinh.Click += (sender, e) =>
                    {
                        // 30 ngày 
                        _flagTuNgay = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                        _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

                        _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddMonths(-1));
                        _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
                        CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 4);

                        // Trạng thái
                        _flagTrangThai = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvChoPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvDaPheDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTuChoi);
                    };
                    _tvApDung.Click += (sender, e) =>
                    {
                        #region Validate Data
                        if (_tvNgayTao_TuNgay.Text.Contains("/") && _tvNgayTao_DenNgay.Text.Contains("/") &&
                        DateTime.ParseExact(_tvNgayTao_TuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null) >
                        DateTime.ParseExact(_tvNgayTao_DenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null))
                        {
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"),
                                                                       CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Start date cannot be greater than end date, please choose again"));

                            _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                            _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                            return;
                        }
                        #endregion

                        _lstCurrentValueFilter = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("Trạng thái", _flagTrangThai),
                            new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                            new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                        };
                        CTRLBoard.LstFilterCondition = _lstCurrentValueFilter;
                        FilterData();
                        _popupFilter.Dismiss();
                    };
                    #endregion
                    _popupFilter.Focusable = true;
                    _popupFilter.OutsideTouchable = false;
                    _popupFilter.ShowAsDropDown(_relaToolBar);
                    _lnBlackFilter.Visibility = ViewStates.Visible; // Tô nền đen dưới Popup
                    _popupFilter.DismissEvent += (sender, e) =>
                    {
                        _lnBlackFilter.Visibility = ViewStates.Gone; // Tô nền đen dưới Popup
                    };
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
            }
        }

        private void Click_relaBoard(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 1;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaBoard", ex);
#endif
            }
        }

        private void Click_relaList(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 2;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaList", ex);
#endif
            }
        }

        private void Click_relaReport(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 3;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaReport", ex);
#endif
            }
        }

        private void Click_ItemList(object sender, BeanWorkflowItem e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    BeanWorkflowItem _temp = e;
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_temp, null, "FragmentBoardDetailGroup");
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentListWorkflow", "Click_ItemWorkflowAdapter", ex);
#endif
            }
        }

        private void TextChanged_EdtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Text.ToString())) // Không Search
                {
                    _imgDelete.Visibility = ViewStates.Gone;
                    //SetData_Board(_lstStepDefine_Filter);
                    //SetData_List(_lstWorkflowItem_Filter);

                    ////  _adapterViewBoard.UpdateListData(_lstStepDefine_Filter);
                    ////  _boardView.CustomNotifyDataSetChanged(_adapterViewBoard);
                    ////new SetAdapterBoardTask(_mainAct, _rootView.Context, _boardView, _adapterViewBoard, _lstStepDefine_Filter).Execute();
                }
                else
                {
                    _imgDelete.Visibility = ViewStates.Visible;

                    ////string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text).ToLowerInvariant().Trim();
                    ////List<BeanWorkflowItem> _lstWorkflowItem_Search = new List<BeanWorkflowItem>();
                    ////List<BeanBoardStepDefine> _lstStepDefine_Search = new List<BeanBoardStepDefine>();

                    ////foreach (BeanBoardStepDefine item in _lstStepDefine_Filter)
                    ////{
                    ////    BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine()
                    ////    {
                    ////        itemStepDefine = item.itemStepDefine,
                    ////        lstWorkflowItem = item.lstWorkflowItem.Where(x => CmmFunction.removeSignVietnamese(x.Content).ToLowerInvariant().Contains(_content)).ToList()
                    ////    };

                    ////    _lstStepDefine_Search.Add(_tempItemList);
                    ////    _lstWorkflowItem_Search.AddRange(_tempItemList.lstWorkflowItem.ToList());
                    ////}
                    ////SetData_Board(_lstStepDefine_Search);
                    ////SetData_List(_lstWorkflowItem_Search);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_EdtSearch", ex);
#endif
            }
        }

        private void EditorAction_EdtSearch(object sender, TextView.EditorActionEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next) // Bấm nút Done trên bàn phím
                    {
                        string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text).ToLowerInvariant().Trim();
                        if (_content.Length > 0)
                            _imgDelete.Visibility = ViewStates.Visible;

                        List<BeanWorkflowItem> _lstWorkflowItem_Search = new List<BeanWorkflowItem>();
                        List<BeanBoardStepDefine> _lstStepDefine_Search = new List<BeanBoardStepDefine>();

                        foreach (BeanBoardStepDefine item in _lstStepDefine_Filter)
                        {
                            BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine()
                            {
                                itemStepDefine = item.itemStepDefine,
                                lstWorkflowItem = item.lstWorkflowItem.Where(x => CmmFunction.removeSignVietnamese(x.Content).ToLowerInvariant().Contains(_content)).ToList()
                            };

                            _lstStepDefine_Search.Add(_tempItemList);
                            _lstWorkflowItem_Search.AddRange(_tempItemList.lstWorkflowItem.ToList());
                        }
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                        SetData_Board(_lstStepDefine_Search);
                        SetData_List(_lstWorkflowItem_Search);
                    }
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "EditorAction_EdtSearch", ex);
#endif
            }
        }

        private async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                await Task.Run(() =>
                {
                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);

                    string _preValueLang = CmmVariable.SysConfig.LangCode;
                    ProviderUser pUser = new ProviderUser();
                    pUser.UpdateCurrentUserInfo(CmmVariable.M_Avatar);

                    // Check xem có bị thay đổi giá trị LangCode không
                    if (!_preValueLang.Equals(CmmVariable.SysConfig.LangCode))
                    {
                        pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    _mainAct.RunOnUiThread(() =>
                    {
                        _flagCurrentPage = 1;
                        SetViewByLanguage();
                        SetViewNavigationByCurrentPage();
                        SetData();
                    });
                });
                _swipe.Refreshing = false;
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Swipe_RefreshData", ex);
#endif
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                });
            }
        }
        #endregion

        #region Data
        private async void GetNewSetData(bool _IsShowDialog = true)
        {
            try
            {
                if (_IsShowDialog)
                {
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                }
                _lstStepDefine.Clear();
                _lstStepDefine_Full.Clear();
                _lstWorkflowItem_Full.Clear();

                ProviderBase pBase = new ProviderBase();
                await Task.Run(() =>
                {
                    pBase.UpdateMasterData<BeanWorkflowItem>();

                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    string _queryStepDefine = string.Format(CTRLBoard._QueryStepDefine, _beanWorkflow.WorkflowID);
                    _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                    _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -1, Title = "Phê duyệt" });
                    _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -2, Title = "Từ chối" });

                    string _queryTemp = "";
                    foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                    {
                        ////// TH đặc biệt: "Phê duyệt" - "Từ chối"
                        ////if (item.WorkflowStepDefineID == -1) // Phê duyệt
                        ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.ApprovedID);
                        ////else if (item.WorkflowStepDefineID == -2) //"Từ chối"
                        ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.RejectedID);
                        ////else
                        ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep, _beanWorkflow.WorkflowID, item.Step);

                        BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine { itemStepDefine = item, lstWorkflowItem = new List<BeanWorkflowItem>() };

                        List<BeanWorkflowItem> _lstWFItemStep = conn.Query<BeanWorkflowItem>(_queryTemp);
                        _lstWFItemStep = CTRLBoard.FilterListByCondition(_lstWFItemStep, CTRLBoard.GetCurrentValue_Filter());
                        if (_lstWFItemStep != null && _lstWFItemStep.Count > 0)
                        {
                            for (int i = 0; i < _lstWFItemStep.Count; i++) // Gán Favorite cho từng item
                            {
                                string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _lstWFItemStep[i].ID);
                                List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                                if (_lstFollow != null && _lstFollow.Count > 0)
                                    _lstWFItemStep[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                            }

                            _tempItemList.lstWorkflowItem.AddRange(_lstWFItemStep);
                            _lstWorkflowItem_Full.AddRange(_lstWFItemStep);
                        }

                        _lstStepDefine_Full.Add(_tempItemList);
                    }
                    conn.Close();

                    _lstWorkflowItem_Full = _lstWorkflowItem_Full.OrderByDescending(x => x.Created).ToList(); // phải sắp xếp theo ngày
                    _lstStepDefine_Filter = _lstStepDefine_Full.ToList();
                    _lstWorkflowItem_Filter = _lstWorkflowItem_Full.ToList();
                    _mainAct.RunOnUiThread(() =>
                    {
                        SetData_Board(_lstStepDefine_Filter);
                        SetData_List(_lstWorkflowItem_Filter);
                        if (_IsShowDialog)
                        {
                            CmmDroidFunction.HideProcessingDialog();
                        }
                    });
                });
            }
            catch (System.Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    if (_IsShowDialog)
                    {
                        CmmDroidFunction.HideProcessingDialog();
                    }
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        /// <summary>
        /// giống GetNewSetData nhưng ko gọi API - chỉ set data
        /// </summary>
        private void SetData(bool _isRenewAdapter = true)
        {
            try
            {
                _lstStepDefine.Clear();
                _lstStepDefine_Full.Clear();
                _lstWorkflowItem_Full.Clear();

                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string _queryStepDefine = string.Format(CTRLBoard._QueryStepDefine, _beanWorkflow.WorkflowID);
                _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -1, Title = "Phê duyệt" });
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -2, Title = "Từ chối" });


                string _queryTemp = "";
                foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                {
                    // TH đặc biệt: "Phê duyệt" - "Từ chối"
                    ////if (item.WorkflowStepDefineID == -1) // Phê duyệt
                    ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.ApprovedID);
                    ////else if (item.WorkflowStepDefineID == -2) //"Từ chối"
                    ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.RejectedID);
                    ////else
                    ////    _queryTemp = string.Format(CTRLBoard._QueryWorkflowItemByStep, _beanWorkflow.WorkflowID, item.Step);

                    BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine { itemStepDefine = item, lstWorkflowItem = new List<BeanWorkflowItem>() };

                    List<BeanWorkflowItem> _lstWFItemStep = conn.Query<BeanWorkflowItem>(_queryTemp);
                    _lstWFItemStep = CTRLBoard.FilterListByCondition(_lstWFItemStep, CTRLBoard.GetCurrentValue_Filter());
                    if (_lstWFItemStep != null && _lstWFItemStep.Count > 0)
                    {
                        _tempItemList.lstWorkflowItem.AddRange(_lstWFItemStep);
                        _lstWorkflowItem_Full.AddRange(_lstWFItemStep);
                    }

                    _lstStepDefine_Full.Add(_tempItemList);
                }
                _lstWorkflowItem_Full = _lstWorkflowItem_Full.OrderByDescending(x => x.Created).ToList(); // phải sắp xếp theo ngày

                _lstStepDefine_Filter = _lstStepDefine_Full.ToList();
                _lstWorkflowItem_Filter = _lstWorkflowItem_Full.ToList();

                if (_isRenewAdapter == true)
                {
                    SetData_Board(_lstStepDefine_Filter);
                    SetData_List(_lstWorkflowItem_Filter);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void FilterData()
        {
            try
            {
                _lstStepDefine_Filter.Clear();
                _lstWorkflowItem_Filter.Clear();
                foreach (BeanBoardStepDefine item in _lstStepDefine_Full)
                {
                    BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine
                    {
                        itemStepDefine = item.itemStepDefine,
                        lstWorkflowItem = CTRLBoard.FilterListByCondition(item.lstWorkflowItem, CTRLBoard.GetCurrentValue_Filter())
                    };

                    _lstStepDefine_Filter.Add(_tempItemList);
                    _lstWorkflowItem_Filter.AddRange(_tempItemList.lstWorkflowItem);
                }
                SetData_Board(_lstStepDefine_Filter);
                SetData_List(_lstWorkflowItem_Filter);
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "FilterData", ex);
#endif
            }
        }

        private void SetData_Board(List<BeanBoardStepDefine> _lstBoard)
        {
            try
            {
                if (_lstBoard != null && _lstBoard.Count > 0)
                {
                    _adapterViewBoard = new AdapterBoardDetailGroupLibrary(_mainAct, _rootView.Context, _lstBoard);
                    _boardView.SetAdapter(_adapterViewBoard);
                }
                else
                {
                    _boardView.Visibility = ViewStates.Gone;
                    _lnNoDataBoard.Visibility = ViewStates.Visible;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_Board", ex);
#endif
            }
        }

        private void SetData_List(List<BeanWorkflowItem> _lstWorkflowItem)
        {
            try
            {
                if (_lstWorkflowItem != null && _lstWorkflowItem.Count > 0)
                {
                    _recyList.Visibility = ViewStates.Visible;
                    _lnNoDataList.Visibility = ViewStates.Gone;

                    /// _adapterRecyList = new AdapterHomePageRecyVTBD(_rootView.Context, _lstWorkflowItem, _mainAct);
                    /// _adapterRecyList.CustomItemClick -= Click_ItemList;
                    /// _adapterRecyList.CustomItemClick += Click_ItemList;
                    _recyList.SetAdapter(_adapterRecyList);
                    _recyList.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                }
                else
                {
                    _recyList.Visibility = ViewStates.Gone;
                    _lnNoDataList.Visibility = ViewStates.Visible;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_List", ex);
#endif
            }
        }


        public void OnClick(View view, int columnPosition, int itemPosition)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    BeanWorkflowItem _clickedItem = _adapterViewBoard.GetItemByPostion(columnPosition, itemPosition);
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_clickedItem, null, "FragmentBoardDetailGroup");
                    _mainAct.AddFragment(_mainAct.SupportFragmentManager, detailWorkFlow, "FragmentBoardDetailGroup", 0);
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnClick", ex);
#endif
            }

        }

        public void ChangedPosition(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn)
        {

        }

        public void StartDrag(View itemView, int originalPosition, int originalColumn)
        {

        }

        public void Dragging(View itemView, MotionEvent @event)
        {
            try
            {

            }
            catch (System.Exception)
            {

            }
        }

        public void EndDrag(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn)
        {
            try
            {
                ControllerBase CTRLBase = new ControllerBase();
                if (CTRLBase.CheckAppHasConnection() == false)
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_OFFLINE", "Bạn đang ở chế độ offline"),
                                            CmmFunction.GetTitle("TEXT_OFFLINE", "You are in offline mode"));
                    RollbackData_ViewBoard();
                    return;
                }
                if (originalColumn >= _lstStepDefine_Filter.Count - 2) // Không được thao tác 2 cột cuối: Từ chối - Phê duyệt
                {
                    RollbackData_ViewBoard();
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("BANANA", "Không được thao tác trên bước Phê duyệt và Từ chối"),
                                                               CmmFunction.GetTitle("BANANA", "Không được thao tác trên bước Phê duyệt và Từ chối"));
                }
                else
                {
                    if (System.Math.Abs(newColumn - originalColumn) >= 2) // không cho skip - back quá 1 bước
                    {
                        RollbackData_ViewBoard();
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("BANANA", "Chỉ được thao tác trên hai bước liền kề"),
                        CmmFunction.GetTitle("BANANA", "Can only do action on two adjacent steps"));
                        return;
                    }

                    if (newColumn > originalColumn) // Drag qua phải
                        SetDragAction_Library(originalPosition, originalColumn, newPosition, newColumn, true);
                    else if (newColumn < originalColumn) // Drag qua trái
                        SetDragAction_Library(originalPosition, originalColumn, newPosition, newColumn, false);
                    else if (newColumn == originalColumn) // Drag tại chỗ -> rollback
                        RollbackData_ViewBoard();
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "EndDrag", ex);
#endif
            }
        }

        private void RollbackData_ViewBoard()
        {
            try
            {
                _boardView.Enabled = false;
                List<BeanBoardStepDefine> _lstCurrentData = _adapterViewBoard.GetListData().ToList();
                SetData_Board(_lstCurrentData);
                _boardView.Enabled = true;
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Rollback_ViewBoard", ex);
#endif
            }
        }

        public void SetDragAction_Library(int originalPosition, int originalColumn, int newPosition, int newColumn, bool IsNextAction)
        {
            try
            {
                BeanWorkflowItem _itemAction = _lstStepDefine_Filter[originalColumn].lstWorkflowItem[originalPosition];

                #region Prepare Data
                string _resultString = "";
                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _resultString = _pControlDynamic.GetTicketRequestControlDynamicForm(_itemAction); // List Form control
                else
                    _resultString = _pControlDynamic.GetTicketRequestControlDynamicForm(_itemAction, "1033"); // List Form control

                JObject _OBJFORMACTION = JObject.Parse(_resultString);
                JArray jArrayForm = JArray.Parse(_OBJFORMACTION["form"].ToString());

                ViewRow _LISTACTION = JsonConvert.DeserializeObject<ViewRow>(_OBJFORMACTION["action"].ToString());
                string _formDefineInfo = jArrayForm[0]["FormDefineInfo"].ToString();
                #endregion

                #region Check Validate Button Action
                ButtonAction buttonAction = null;
                foreach (ViewElement item in _LISTACTION.Elements)
                {
                    if (IsNextAction == true) // kiểm tra xem có action tới không
                    {
                        if (item.Value.ToLowerInvariant().Equals("next"))
                            buttonAction = new ButtonAction { ID = Convert.ToInt32(item.ID), Title = item.Title, Value = item.Value, Notes = item.Notes };
                    }
                    else // kiểm tra xem có action lui không
                    {
                        if (item.Value.ToLowerInvariant().Equals("return"))
                            buttonAction = new ButtonAction { ID = Convert.ToInt32(item.ID), Title = item.Title, Value = item.Value, Notes = item.Notes };
                    }
                }
                #endregion

                #region Handle Button Action
                if (buttonAction != null)
                {

                    if (IsNextAction == true)
                    {
                        Action_SendAPI(originalPosition, originalColumn, newPosition, newColumn, _itemAction, buttonAction, _formDefineInfo, "");
                    }
                    else
                    {
                        //Update View trước - Lát rollback sau
                        _boardView.NotifyDataSetChanged(); // Notify lại boardView nếu trường hợp listChild.count = 0

                        bool _flagRollback = true;  // nếu tắt dialog - hủy thì rollback, send action thì ko cần
                        #region Get View - Init Data
                        View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_Accept, null);
                        TextView _tvTitle = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Title);
                        ImageView _imgAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_Accept);
                        EditText _edtComment = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_Accept_YKien);
                        TextView _tvCancel = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Huy);
                        TextView _tvAccept = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_HoanTat);

                        _tvTitle.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Hoàn tất");

                        _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                        _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                        _tvAccept.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Hoàn tất");

                        string _imageName = "icon_bpm_Btn_action_" + buttonAction.ID.ToString();
                        int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                        _imgAction.SetImageResource(resId);

                        #endregion

                        #region Event
                        _tvAccept.Click += (sender, e) =>
                        {
                            if (new ControllerDetailWorkflow().CheckActionHasComment(_mainAct, _edtComment) == true)
                            {
                                CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                                _flagRollback = false; // ko cần rollback vì đã có Refresh
                                _dialogAction.Dismiss();
                                Action_SendAPI(originalPosition, originalColumn, newPosition, newColumn, _itemAction, buttonAction, _formDefineInfo, _edtComment.Text);
                            }
                        };
                        _tvCancel.Click += (sender, e) =>
                        {
                            CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                            _dialogAction.Dismiss();
                        };
                        _edtComment.TextChanged += (sender, e) =>
                        {
                            if (string.IsNullOrEmpty(_edtComment.Text))
                                _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                            else
                                _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                        };

                        #endregion

                        #region Show View
                        _dialogAction = new Dialog(_rootView.Context);
                        Window window = _dialogAction.Window;
                        _dialogAction.RequestWindowFeature(1);
                        _dialogAction.SetCanceledOnTouchOutside(false);
                        _dialogAction.SetCancelable(true);
                        window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                        window.SetGravity(GravityFlags.Center);
                        var dm = Resources.DisplayMetrics;

                        _dialogAction.SetContentView(_viewPopupAction);
                        _dialogAction.Show();
                        WindowManagerLayoutParams s = window.Attributes;
                        s.Width = dm.WidthPixels;
                        window.Attributes = s;
                        window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                        _dialogAction.DismissEvent += (sender, e) =>
                        {
                            if (_flagRollback)
                                RollbackData_ViewBoard();
                        };
                        #endregion

                        _edtComment.Text = "";
                    }
                }
                else
                {
                    RollbackData_ViewBoard();
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Mess_ActionFalse", "Phiếu không có hành động tương ứng!"),
                                                               CmmFunction.GetTitle("K_Mess_ActionFalse", "This workflow item not have corresponding action!"));
                }
                #endregion
            }
            catch (System.Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoardDetailGroup_Library", "SetDragAction_NextPrevious", ex);
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
        private async void Action_SendAPI(int originalPosition, int originalColumn, int newPosition, int newColumn, BeanWorkflowItem _itemAction, ButtonAction buttonAction, string _formDefineInfo, string comment)
        {
            bool _result = false;
            try
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    #region Comment
                    // Nếu Action Có comment -> Add thêm cột idea
                    List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
                    if (!string.IsNullOrEmpty(comment))
                    {
                        KeyValuePair<string, string> _KeyValueComment = new KeyValuePair<string, string>("idea", comment);
                        if (_lstExtent == null) _lstExtent = new List<KeyValuePair<string, string>>();
                        _lstExtent.Add(_KeyValueComment);
                    }
                    #endregion

                    string _messageAPI = "";

                    _result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _itemAction.ID, _formDefineInfo, JsonConvert.SerializeObject(new List<ObjectSubmitAction>()), ref _messageAPI, new List<KeyValuePair<string, string>>(), _lstExtent);

                    if (_result)
                    {
                        _pControlDynamic.UpdateAllMasterData(true);
                        _pControlDynamic.UpdateAllDynamicData(true);

                        _mainAct.RunOnUiThread(() =>
                        {
                            SetData();
                            CmmDroidFunction.HideProcessingDialog();
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            SetData_Board(_lstStepDefine_Filter);
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
            catch (System.Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Action_SendAPI", ex);
#endif
            }
        }

        #endregion

    }
}