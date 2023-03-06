using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using Com.Telerik.Widget.Calendar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using static BPMOPMobile.Droid.Presenter.Adapter.AdapterBoardDetailGroup;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentBoardDetailGroup_customlibrary : CustomBaseFragment, View.IOnTouchListener, RecyclerView.IOnItemTouchListener
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private PopupWindow _popupFilter;
        private Dialog _dialogAction;
        private View _rootView, _popupViewFilter;
        private EditText _edtSearch;
        private TextView _tvTitle, _tvBoard, _tvList, _tvReport;
        private ImageView _imgBack, _imgBoard, _imgList, _imgReport, _imgDelete;
        private RelativeLayout _relaToolBar, _relaBoard, _relaList, _relaReport, _relaDataBoard, _relaDataList, _relaDataReport;
        private LinearLayout _lnFilter, _lnBlackFilter, _lnNoDataBoard, _lnNoDataList, _lnNoDataReport;
        private View _vwBoard, _vwList, _vwReport;
        private CircleImageView _imgAvatar;
        private SwipeRefreshLayout _swipe;
        private RecyclerView _recyBoard, _recyList, _recyReport;
        private AdapterBoardDetailGroup _adapterRecyBoard;
        //private AdapterHomePageRecyVTBD _adapterRecyList;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerBoard CTRLBoard = new ControllerBoard();

        private List<BeanWorkflowStepDefine> _lstStepDefine = new List<BeanWorkflowStepDefine>(); // Chỉ bao gồm bước của Quy Trình
        private List<BeanWorkflowItem> _lstWorkflowItem_Full = new List<BeanWorkflowItem>();
        private List<BeanBoardStepDefine> _lstStepDefine_Full = new List<BeanBoardStepDefine>();

        private List<BeanWorkflowItem> _lstWorkflowItem_Filter = new List<BeanWorkflowItem>();
        private List<BeanBoardStepDefine> _lstStepDefine_Filter = new List<BeanBoardStepDefine>();

        private BeanWorkflow _beanWorkflow = new BeanWorkflow();
        private int _flagCurrentPage = 1; // 1 = Board, 2 = List, 3 = Report
        private int _firstVisibleListBoard = 0;
        private float OnTouch_x1, OnTouch_x2, OnTouch_y1, OnTouch_y2;
        private float OnIntercept_x1, OnIntercept_x2, OnIntercept_y1, OnIntercept_y2;

        public FragmentBoardDetailGroup_customlibrary() { }
        public FragmentBoardDetailGroup_customlibrary(BeanWorkflow _beanWorkflow, int _flagCurrentPage)
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
                ////_recyBoard = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewBoardDetailGroup_Board);
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
                //_tvNoDataBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Board_NoData);
                //_tvNoDataList = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_List_NoData);
                //_tvNoDataReport = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Report_NoData);
                _vwBoard = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Board);
                _vwList = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_List);
                _vwReport = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Report);

                //_imgHomeBottom = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Bottom_Home);
                //_imgVTBDBottom = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Bottom_VTBD);
                //_imgVDTBottom = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Bottom_VDT);

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                _swipe.Refresh += Swipe_RefreshData;
                _imgBack.Click += Click_imgBack;
                _imgDelete.Click += Click_imgDelete;
                _lnFilter.Click += Click_lnFilter;
                _relaBoard.Click += Click_relaBoard;
                _relaList.Click += Click_relaList;
                _relaReport.Click += Click_relaReport;
                _edtSearch.TextChanged += TextChanged_EdtSearch;

                _recyBoard.AddOnItemTouchListener(this);
                _recyBoard.SetOnTouchListener(this);

                SetViewNavigationByCurrentPage(); // Set View bằng _flagCurrentPage
                _imgDelete.Visibility = ViewStates.Gone;
            }
            SetViewByLanguage();
            GetNewSetData();
            //MainActivity.FlagNavigation = 5; // Board Detail Group
            return _rootView;
        }

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (_beanWorkflow != null)
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _tvTitle.Text = !String.IsNullOrEmpty(_beanWorkflow.Title) ? _beanWorkflow.Title : "";
                    else
                        _tvTitle.Text = !String.IsNullOrEmpty(_beanWorkflow.TitleEN) ? _beanWorkflow.TitleEN : "";

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetViewByLanguage", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetViewByLanguage", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Back", ex);
#endif
            }
        }
        private void Click_imgAvatar(object sender, EventArgs e)
        {
            try
            {
                //MainActivity.FlagNavigation = 4; // Board
                //MinionAction.OnRenewDataAndShowFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
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

                    TextView _tvChon = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_ApDung);
                    TextView _tvBoChon = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupViewBoardDetailGroupFilter_MacDinh);

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
                                    catch (Exception) { }
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
                                    catch (Exception) { }
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
                        if (!String.IsNullOrEmpty(_flagTuNgay))
                        {
                            DateTime _tempTuNgay = DateTime.Now;
                            try
                            {
                                _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                            }
                            catch (Exception)
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
                        if (!String.IsNullOrEmpty(_flagDenNgay))
                        {
                            DateTime _tempDenNgay = DateTime.Now;
                            try
                            {
                                _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                            }
                            catch (Exception)
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

                    _tvBoChon.Click += (sender, e) =>
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
                    _tvChon.Click += (sender, e) =>
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "Click_lnFilter", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "Click_relaBoard", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "Click_relaList", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "Click_relaReport", ex);
#endif
            }
        }
        private void TextChanged_EdtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.Text.ToString())) // Không Search
                {
                    _imgDelete.Visibility = ViewStates.Gone;
                    SetData_Board(_lstStepDefine_Full);
                    SetData_List(_lstWorkflowItem_Full);
                }
                else
                {
                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text).ToLowerInvariant().Trim();
                    _imgDelete.Visibility = ViewStates.Visible;

                    List<BeanWorkflowItem> _lstWorkflowItem_Search = new List<BeanWorkflowItem>();
                    List<BeanBoardStepDefine> _lstStepDefine_Search = new List<BeanBoardStepDefine>();

                    foreach (BeanBoardStepDefine item in _lstStepDefine_Full)
                    {
                        BeanBoardStepDefine _tempItemList = new BeanBoardStepDefine()
                        {
                            itemStepDefine = item.itemStepDefine,
                            lstWorkflowItem = item.lstWorkflowItem.Where(x => CmmFunction.removeSignVietnamese(x.Content).Contains(_content)).ToList()
                        };

                        _lstStepDefine_Search.Add(_tempItemList);
                        _lstWorkflowItem_Search.AddRange(_tempItemList.lstWorkflowItem.ToList());
                    }
                    SetData_Board(_lstStepDefine_Search);
                    SetData_List(_lstWorkflowItem_Search);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoardDetailGroup", "TextChanged_EdtSearch", ex);
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
                        GetNewSetData();
                    });
                });
                _swipe.Refreshing = false;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoardDetailGroup", "Swipe_RefreshData", ex);
#endif
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                });
            }
        }
        private void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                StaggeredGridLayoutManager _temp = (StaggeredGridLayoutManager)_recyBoard.GetLayoutManager();
                int[] tempFirstVisible = new int[5];
                _temp.FindFirstVisibleItemPositions(tempFirstVisible);
                if (tempFirstVisible[0] < _firstVisibleListBoard)
                {
                    _recyBoard.SmoothScrollToPosition(tempFirstVisible[0] - 2);

                }
                else if (tempFirstVisible[0] > _firstVisibleListBoard)
                {
                    _recyBoard.SmoothScrollToPosition(tempFirstVisible[0] + 2);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoardDetailGroup", "ScrollChange_RecyData", ex);
#endif
            }
        }
        #endregion

        #region Data
        private async void GetNewSetData()
        {
            try
            {
                _lstStepDefine.Clear();
                _lstStepDefine_Full.Clear();
                _lstWorkflowItem_Full.Clear();

                ProviderBase pBase = new ProviderBase();
                await Task.Run(() =>
                {
                    pBase.UpdateAllDynamicData(true);

                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    string _queryStepDefine = String.Format(CTRLBoard._QueryStepDefine, _beanWorkflow.WorkflowID);
                    _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                    _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -1, Title = "Phê duyệt" });
                    _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -2, Title = "Từ chối" });


                    string _queryTemp = "";
                    foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                    {
                        // TH đặc biệt: "Phê duyệt" - "Từ chối"
                        ////if (item.WorkflowStepDefineID == -1) // Phê duyệt
                        ////    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.ApprovedID);
                        ////else if (item.WorkflowStepDefineID == -2) //"Từ chối"
                        ////    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.RejectedID);
                        ////else
                        ////    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep, _beanWorkflow.WorkflowID, item.Step);

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
                    _mainAct.RunOnUiThread(() =>
                    {
                        SetData_Board(_lstStepDefine_Filter);
                        SetData_List(_lstWorkflowItem_Filter);
                    });
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData", ex);
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
                string _queryStepDefine = String.Format(CTRLBoard._QueryStepDefine, _beanWorkflow.WorkflowID);
                _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -1, Title = "Phê duyệt" });
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = -2, Title = "Từ chối" });


                string _queryTemp = "";
                foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                {
                    //// TH đặc biệt: "Phê duyệt" - "Từ chối"
                    //if (item.WorkflowStepDefineID == -1) // Phê duyệt
                    //    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.ApprovedID);
                    //else if (item.WorkflowStepDefineID == -2) //"Từ chối"
                    //    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep_Exception, _beanWorkflow.WorkflowID, CTRLBoard.RejectedID);
                    //else
                    //    _queryTemp = String.Format(CTRLBoard._QueryWorkflowItemByStep, _beanWorkflow.WorkflowID, item.Step);

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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData", ex);
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
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "FilterData", ex);
#endif
            }
        }
        private void SetData_Board(List<BeanBoardStepDefine> _lstBoard)
        {
            try
            {
                if (_lstBoard != null && _lstBoard.Count > 0)
                {
                    _recyBoard.Visibility = ViewStates.Visible;
                    _lnNoDataBoard.Visibility = ViewStates.Gone;

                    _adapterRecyBoard = new AdapterBoardDetailGroup(_mainAct, _rootView.Context, this, _recyBoard, _lstBoard);
                    _recyBoard.SetAdapter(_adapterRecyBoard);
                    ////_recyBoard.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Horizontal));
                    _recyBoard.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Horizontal, false));
                }
                else
                {
                    _recyBoard.Visibility = ViewStates.Gone;
                    _lnNoDataBoard.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData_Board", ex);
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

                  //  _adapterRecyList = new AdapterHomePageRecyVTBD(_rootView.Context, _lstWorkflowItem, _mainAct);
                  //  _adapterRecyList.CustomItemClick += delegate { }; //Click_ItemNotifyAdapter;
                  //  _recyList.SetAdapter(_adapterRecyList);
                  //  _recyList.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                }
                else
                {
                    _recyList.Visibility = ViewStates.Gone;
                    _lnNoDataList.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData_List", ex);
#endif
            }
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            try
            {
                switch (e.Action)
                {
                    case MotionEventActions.Move:
                        {
                            break;
                        }
                    case MotionEventActions.Down:
                        {
                            OnTouch_x1 = e.GetX();
                            OnTouch_y1 = e.GetY();
                            break;
                        }
                    case MotionEventActions.Up:
                        {
                            OnTouch_x2 = e.GetX();
                            OnTouch_y2 = e.GetY();

                            float deltaX = OnTouch_x2 - OnTouch_x1;
                            float deltaY = OnTouch_y2 - OnTouch_y1;

                            if (OnTouch_x1 == 0 && OnTouch_y1 == 0)
                            {
                                return true;
                            }

                            if (Math.Abs(deltaY) > 700 && (Math.Abs(deltaY) - Math.Abs(deltaX)) > 100) // Scroll dọc tên recycler View
                            {
                                return false;
                            }

                            if (Math.Abs(deltaX) > 200) // Scroll ngang trên Recyclerview
                            {
                                LinearLayoutManager _temp = (LinearLayoutManager)_recyBoard.GetLayoutManager();
                                int[] firstVisible = new int[5];
                                int[] lastVisible = new int[5];
                                //_temp.FindFirstCompletelyVisibleItemPositions(firstVisible);
                                //_temp.FindLastCompletelyVisibleItemPositions(lastVisible);

                                firstVisible[0] = _temp.FindFirstCompletelyVisibleItemPosition();
                                lastVisible[0] = _temp.FindLastCompletelyVisibleItemPosition();

                                DisplayMetrics displaymetrics = _mainAct.Resources.DisplayMetrics;
                                int _widthScreen = (displaymetrics.WidthPixels / 10); // Item chiếm 8/10 -> scroll thêm 1/10

                                if (OnTouch_x2 < OnTouch_x1) // Right to Left  swipe
                                {
                                    AdapterBoardDetailGroup adapterBoardDetailGroup = (AdapterBoardDetailGroup)_recyBoard.GetAdapter();
                                    int _lstCount = adapterBoardDetailGroup.GetListDataCount() - 1; // -1 vì postion từ 0 lên
                                    if (lastVisible[0] + 1 == _lstCount) // Position cuối cùng -> Smooth scroll là đủ
                                    {
                                        _recyBoard.SmoothScrollToPosition(lastVisible[0] + 1);
                                    }
                                    else
                                    {
                                        _recyBoard.ScrollToPosition(lastVisible[0] + 1);
                                        _recyBoard.SmoothScrollBy(_widthScreen, 0); // scroll cho vào giữa màn hình
                                    }
                                }
                                else // left to  Right swipe    
                                {
                                    if (firstVisible[0] - 1 == 0) // Position đầu tiên -> Smooth scroll là đủ
                                        _recyBoard.SmoothScrollToPosition(firstVisible[0] - 1);
                                    else
                                    {
                                        _recyBoard.ScrollToPosition(firstVisible[0] - 1);
                                        _recyBoard.SmoothScrollBy(_widthScreen * -1, 0); // scroll cho vào giữa màn hình
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception)
            {

            }
            return true;
        }
        public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
        {
            //If you return true touch events are disabled.
            //if Return false touch events are enable.


            MotionEvent e = @event;
            try
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        {
                            OnIntercept_x1 = e.GetX();
                            OnIntercept_y1 = e.GetY();
                            break;
                        }
                    case MotionEventActions.Up:
                        {
                            OnIntercept_x2 = e.GetX();
                            OnIntercept_y2 = e.GetX();

                            float deltaX = OnIntercept_x2 - OnIntercept_x1;
                            float deltaY = OnIntercept_y2 - OnIntercept_y1;

                            if (Math.Abs(deltaY) > 700 && (Math.Abs(deltaY) - Math.Abs(deltaX)) > 100)
                            {
                                return true;
                            }
                            break;
                        }
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        public void OnRequestDisallowInterceptTouchEvent(bool disallow)
        {

        }
        public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
        {

        }

        #region Drag And Drop Action
        public void SetDragAction_NextPrevious(RecyclerView _RecySource, RecyclerView _RecyTarget, int _positionSource, bool IsNextAction)
        {
            try
            {
                #region Prepare Data
                AdapterBoardDetailGroup_Child adapterSource = (AdapterBoardDetailGroup_Child)_RecySource.GetAdapter();
                AdapterBoardDetailGroup_Child adapterTarget = (AdapterBoardDetailGroup_Child)_RecyTarget.GetAdapter();
                List<BeanWorkflowItem> listSource = adapterSource.GetListDataSouce().ToList();
                List<BeanWorkflowItem> listTarget = adapterTarget.GetListDataSouce().ToList();
                BeanWorkflowItem _itemAction = listSource[_positionSource];

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
                        Action_SendAPI(listSource, listTarget, _RecySource.Id, _RecyTarget.Id, _positionSource, buttonAction, _formDefineInfo, "");
                    }
                    else
                    {
                        #region Get View - Init Data
                        View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_Accept, null);
                        TextView _tvTitle = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Title);
                        ImageView _imgAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_Accept);
                        EditText _edtComment = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_Accept_YKien);
                        TextView _tvCancel = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Huy);
                        TextView _tvAccept = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_HoanTat);

                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        {
                            if (buttonAction.ID == (int)WorkflowAction.Action.Reject)
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Accept", "Hủy phê duyệt yêu cầu");
                            }
                            else if (buttonAction.ID == (int)WorkflowAction.Action.Idea)
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Idea", "Cho ý kiến");
                            }
                            else
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("TEXT_REJECT_REQUEST", "Từ chối phê duyệt yêu cầu");
                            }
                            _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                            _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                            _tvAccept.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Hoàn tất");
                        }
                        else
                        {
                            if (buttonAction.ID == (int)WorkflowAction.Action.Reject)
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Accept", "Cancel request");
                            }
                            else if (buttonAction.ID == (int)WorkflowAction.Action.Idea)
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("K_Mess_Action_Idea", "Idea");
                            }
                            else
                            {
                                _tvTitle.Text = CmmFunction.GetTitle("TEXT_REJECT_REQUEST", "Reject request");
                            }
                            _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Leave a comment/ opinion here");
                            _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Exit");
                            _tvAccept.Text = buttonAction.Title;// CmmFunction.GetTitle("TEXT_DONE", "Done");
                        }

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
                                _dialogAction.Dismiss();
                                Action_SendAPI(listSource, listTarget, _RecySource.Id, _RecyTarget.Id, _positionSource, buttonAction, _formDefineInfo, _edtComment.Text);
                            }
                        };
                        _tvCancel.Click += (sender, e) =>
                        {
                            CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                            _dialogAction.Dismiss();
                        };
                        _edtComment.TextChanged += (sender, e) =>
                        {
                            if (String.IsNullOrEmpty(_edtComment.Text))
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
                        #endregion

                        _edtComment.Text = "";
                    }
                }
                else
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Mess_ActionFalse", "Phiếu không có hành động tương ứng!"),
                                                               CmmFunction.GetTitle("K_Mess_ActionFalse", "This workflow item not have corresponding action!"));
                }
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoardDetailGroup", "SetDragAction_NextPrevious", ex);
#endif
            }
        }
        public void SetDragAction_(RecyclerView _RecySource, RecyclerView _RecyTarget, int _positionSource)
        {
            try
            {
                ButtonAction buttonAction = new ButtonAction() { ID = (int)WorkflowAction.Action.Return, Title = "Yêu cầu hiệu chỉnh" };


            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Action_Reject", ex);
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
        private async void Action_SendAPI(List<BeanWorkflowItem> listSource, List<BeanWorkflowItem> listTarget, int _RecySourceId, int _RecyTargetId, int _positionSource, ButtonAction buttonAction, string _formDefineInfo, string comment)
        {
            bool _result = false;
            try
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);


                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    BeanWorkflowItem _itemAction = listSource[_positionSource];

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

                    if (_lstExtent != null && _lstExtent.Count > 0)
                    {
                       //// _result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _itemAction.ID, _formDefineInfo, JsonConvert.SerializeObject(new List<ObjectSubmitAction>()), new List<KeyValuePair<string, string>>(), _lstExtent);
                    }
                    else
                    {
                        ////_result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _itemAction.ID, _formDefineInfo, JsonConvert.SerializeObject(new List<ObjectSubmitAction>()), new List<KeyValuePair<string, string>>(), null);
                    }
                    if (_result)
                    {
                        _pControlDynamic.UpdateAllMasterData(true);
                        _pControlDynamic.UpdateAllDynamicData(true);

                        _mainAct.RunOnUiThread(() =>
                        {
                            //listSource.RemoveAt(_positionSource);
                            //listTarget.Add(_itemAction);

                            //AdapterBoardDetailGroup _parentAdapter = (AdapterBoardDetailGroup)_recyBoard.GetAdapter(); // Cập nhật list parent 
                            //List<BeanBoardStepDefine> _lstStepDefine = _parentAdapter.GetListData();

                            //_lstStepDefine[_RecySourceId].lstWorkflowItem = listSource; // Id start từ 1
                            //_lstStepDefine[_RecyTargetId].lstWorkflowItem = listTarget;  // Id start từ 1
                            //_parentAdapter.UpdateListData(_lstStepDefine);
                            //_parentAdapter.NotifyDataSetChanged();

                            #region Handle Refocus current Item
                            LinearLayoutManager _temp = (LinearLayoutManager)_recyBoard.GetLayoutManager();
                            int firstVisible = _temp.FindFirstCompletelyVisibleItemPosition();
                            int lastVisible = _temp.FindLastCompletelyVisibleItemPosition();

                            DisplayMetrics displaymetrics = _mainAct.Resources.DisplayMetrics;
                            int _widthScreen = (displaymetrics.WidthPixels / 10); // Item chiếm 8/10 -> scroll thêm 1/10
                            AdapterBoardDetailGroup adapterBoardDetailGroup = (AdapterBoardDetailGroup)_recyBoard.GetAdapter();
                            int _lstCount = adapterBoardDetailGroup.GetListDataCount() - 1; // -1 vì postion từ 0 lên

                            SetData();

                            if (firstVisible == 0) // Đang ở vị trí đầu tiên -> khỏi scroll
                            {

                            }
                            else if (lastVisible == _lstCount) // Position cuối cùng 
                            {
                                _recyBoard.ScrollToPosition(lastVisible);
                            }
                            else // Còn lại -> scroll cho vào giữa màn hình
                            {
                                _recyBoard.ScrollToPosition(firstVisible);
                                _recyBoard.SmoothScrollBy(_widthScreen * -1, 0);
                            }
                            #endregion
                            CmmDroidFunction.HideProcessingDialog();
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                          CmmFunction.GetTitle("K_Mess_ActionFalse", "Operation cannot be performed. Please try again!"));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Action_SendAPI", ex);
#endif
            }
        }
        #endregion

        #endregion

    }
}