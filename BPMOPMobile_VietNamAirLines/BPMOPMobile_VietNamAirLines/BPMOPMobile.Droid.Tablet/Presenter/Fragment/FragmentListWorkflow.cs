using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
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
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Tablet.Class;
using BPMOPMobile.Droid.Tablet.Presenter.Adapter;
using Newtonsoft.Json;
using Refractored.Controls;
using static Android.App.DatePickerDialog;

namespace BPMOPMobile.Droid.Tablet.Presenter.Fragment
{
    [Obsolete]
    class FragmentListWorkflow : Android.App.Fragment, IOnDateSetListener
    {
        private MainActivity _mainAct;
        private View _rootView, _popupViewFilter;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private PopupWindow _popupFilter;
        private List<BeanNotify> _lstVDT = new List<BeanNotify>();
        private List<BeanNotify> _lstVDT_Filter = new List<BeanNotify>();
        private List<BeanNotify> _lstVDT_Search = new List<BeanNotify>();
        private List<BeanWorkflowItem> _lstVTBD = new List<BeanWorkflowItem>();
        private List<BeanWorkflowItem> _lstVTBD_Filter = new List<BeanWorkflowItem>();
        private List<BeanWorkflowItem> _lstVTBD_Search = new List<BeanWorkflowItem>();
        private BeanWorkflowItem _selectedWorkflowItem = new BeanWorkflowItem();
        private SwipeRefreshLayout _swipe;
        private RelativeLayout _relaToolbar;
        private CircleImageView _imgAvata;
        private RecyclerView _recyDetailAction;
        private ExpandableListView _expandDetailData, _expandDetailProcess;
        private TextView _tvName, _tvNoData, _tvNgayGuiDenTuNgay, _tvNgayGuiDenDenNgay, _tvNgayKhoiTaoTuNgay, _tvNgayKhoiTaoDenNgay, _tvBottomHome, _tvBottomVDT, _tvBottomVTBD, _tvBottomBoard,
           _tvDetailAvatar, _tvDetailTitle1, _tvDetailTitle2, _tvDetailToName, _tvDetailCreatedName, _tvDetailProcessTitle;
        private ImageView _imgFilter, _imgDeleteSearch, _imgBottomHome, _imgBottomVDT, _imgBottomVTBD, _imgBottomBoard, _imgDetailFavorite, _imgDetailShare, _imgDetailProcess, _imgDetailProcessClose;
        private EditText _edtSearch;
        private LinearLayout _lnNoData, _lnContent, _lnBottomHome, _lnBottomVDT, _lnBottomVTBD, _lnBottomBoard, _lnCreateTask, _lnDetailData, _lnDetailProcess;
        private ListView _lvData;
        private AdapterHomePageVDT _NotifyVDTAdapter;
        private AdapterHomePageVTBD _NotifyVTBDAdapter;
        private string _type = "VDT";
        private string _queryVDT = "", _queryVTBD = "";
        private int _flagDatePicker = 0; // 1 = Ngày gửi đến VDT Start | 2 = Ngày gửi đến VDT End | 3 = Ngày khởi tạo VTBD Start | 4 = Ngày khởi tạo VTBD End
        private int _flagIsFiltering = 0;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            MinionAction.ChangeView -= ReloadData;
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            CTRLHomePage.InitListFilterCondition("BOTH");
            base.OnDestroyView();
        }

        public FragmentListWorkflow(string _type, BeanWorkflowItem _selectedWorkflowItem)
        {
            this._type = _type;
            this._selectedWorkflowItem = _selectedWorkflowItem;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            if (_rootView == null)
            {
                _mainAct.Window.SetNavigationBarColor(Color.Black);
                _rootView = inflater.Inflate(Resource.Layout.ViewListWorkflow, null);

                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewListWorkflow_Toolbar);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewListWorkflow_Avata);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Title2);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Filter);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Search_Delete);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewListWorkflow_Search);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewListWorkflow);
                _lvData = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewListWorkflow_Data);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_NoData);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_NoData);

                _lnDetailData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Detail_Content);
                _tvDetailTitle1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_WorkflowTitle);
                _tvDetailTitle2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_WorkflowTitle2);
                _tvDetailAvatar = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_Avatar);
                _tvDetailCreatedName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_CreatedName);
                _tvDetailToName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_ToName);
                _recyDetailAction = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewListWorkflow_Detail_Action);
                _imgDetailFavorite = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Detail_Favorite);
                _imgDetailShare = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Detail_Share);
                _imgDetailProcess = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Detail_Process);
                _expandDetailData = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewListWorkflow_Detail_Data);
                _lnDetailProcess = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Detail_Process);
                _expandDetailProcess = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewListWorkflow_Detail_Process);
                _tvDetailProcessTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Detail_Process_WorkflowTitle);
                _imgDetailProcessClose = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Detail_Process_Back);

                _lnBottomHome = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Bottom_Home);
                _lnBottomVDT = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Bottom_VDT);
                _lnBottomVTBD = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Bottom_VTBD);
                _lnBottomBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Bottom_Board);
                _imgBottomHome = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Bottom_Home);
                _imgBottomVDT = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Bottom_VDT);
                _imgBottomVTBD = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Bottom_VTBD);
                _imgBottomBoard = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Bottom_Board);
                _tvBottomHome = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Bottom_Home);
                _tvBottomVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Bottom_VDT);
                _tvBottomVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Bottom_VTBD);
                _tvBottomBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Bottom_Board);

                _expandDetailProcess.SetGroupIndicator(null);
                _expandDetailProcess.SetChildIndicator(null);
                _expandDetailProcess.DividerHeight = 0;

                _expandDetailData.SetGroupIndicator(null);
                _expandDetailData.SetChildIndicator(null);
                _expandDetailData.DividerHeight = 0;

                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);

                _imgDetailProcess.Click += Click_ShowDetailProcess;
                _imgDetailProcessClose.Click += Click_HideDetailProcess;
                _imgAvata.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _imgFilter.Click += Click_lnFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _lnBottomHome.Click += Click_BottomHome;
                _lnBottomVTBD.Click += Click_BottomVTBD;
                _lnBottomVDT.Click += Click_BottomVDT;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _imgDeleteSearch.Visibility = ViewStates.Gone;
                SetData();
            }
            else
            {
                //if (_NotifyVDTAdapter != null)
                //    _NotifyVDTAdapter.NotifyDataSetChanged();
                //if (_NotifyVTBDAdapter != null)
                //    _NotifyVTBDAdapter.NotifyDataSetChanged();
            }
            CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.ChangeView += ReloadData;
            SetViewByLanguage(null, null);

            return _rootView;
        }


        #region Event
        private void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                if (_type.Equals("VDT"))
                {
                    _imgBottomVTBD.SetColorFilter(Resources.GetColor(Resource.Color.clBottomDisable));
                    _imgBottomVDT.SetColorFilter(Resources.GetColor(Resource.Color.clBottomEnable));
                    _tvBottomVTBD.Visibility = ViewStates.Gone;
                    _tvBottomVDT.Visibility = ViewStates.Visible;
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvName.Text = CmmFunction.GetTitle("K_Mess_VDT", "Đến tôi " + CmmDroidFunction.GetCountNumOfText(_tvName.Text));
                        _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                        _edtSearch.Hint = CmmFunction.GetTitle("K_Search", "Tìm kiếm...");

                    }
                    else
                    {
                        _tvName.Text = CmmFunction.GetTitle("K_Mess_VDT", "To me " + CmmDroidFunction.GetCountNumOfText(_tvName.Text));
                        _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                        _edtSearch.Hint = CmmFunction.GetTitle("K_Search", "Search...");
                    }
                }
                else // VTBD
                {
                    _imgBottomVTBD.SetColorFilter(Resources.GetColor(Resource.Color.clBottomEnable));
                    _imgBottomVDT.SetColorFilter(Resources.GetColor(Resource.Color.clBottomDisable));
                    _tvBottomVTBD.Visibility = ViewStates.Visible;
                    _tvBottomVDT.Visibility = ViewStates.Gone;
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvName.Text = CmmFunction.GetTitle("K_Mess_VTBD", "Tôi bắt đầu " + CmmDroidFunction.GetCountNumOfText(_tvName.Text));
                        _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                        _edtSearch.Hint = CmmFunction.GetTitle("K_Search", "Tìm kiếm...");

                    }
                    else
                    {
                        _tvName.Text = CmmFunction.GetTitle("K_Mess_VDT", "From me " + CmmDroidFunction.GetCountNumOfText(_tvName.Text));
                        _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                        _edtSearch.Hint = CmmFunction.GetTitle("K_Search", "Search...");
                    }
                }
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");
                _lvData.InvalidateViews();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - FragmentListWorkflow - Error: " + ex.Message);
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
                    _flagIsFiltering = 0;
                    CTRLHomePage.InitListFilterCondition("BOTH");
                    SQLite.SQLiteConnection con = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    con.Execute(CTRLHomePage._QueryDeleteNotify);

                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateMasterData<BeanNotify>(null, false, 30, true);
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);
                    _mainAct.RunOnUiThread(() =>
                    {
                        SetViewByLanguage(null, null);
                        SetData();
                    });
                });
                _swipe.Refreshing = false;
                _swipe.Enabled = true;
            }
            catch (Exception ex)
            {
                CmmDroidFunction.LogErr(ex, "MainView - refesh_data - Er: " + ex);
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                    _swipe.Enabled = true;
                });
            }
        }
        private void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                _mainAct.OpenDawer();
                //MinionAction.UpDate_Push(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentListWorkflow - Click_Menu - Error: " + ex.Message);
#endif
            }
        }
        private void Click_lnFilter(object sender, EventArgs e)
        {/*
            string _flagTrangThai = "", _flagHanXuLy = "", _flagTuNgay = "", _flagDenNgay = "";
            try
            {
                _lnContent.Alpha = (float)0.2;

                DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                if (_type.Equals("VDT")) // Việc đến tôi
                {
                    #region Init Flag and View
                    _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupHomePageFilterVDT, null);
                    _popupFilter = new PopupWindow(_popupViewFilter, _displayMetrics.WidthPixels, (int)(_lnContent.MeasuredHeight * 0.75));
                    TextView _tvTrangThai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_TrangThai);
                    TextView _tvTrangThaiTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_TrangThai_TatCa);
                    TextView _tvTrangThaiChuaXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_TrangThai_CXL);
                    TextView _tvTrangThaiDaXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_TrangThai_DXL);
                    TextView _tvHanXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_HanXuLy);
                    TextView _tvHanXuLyTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_HanXuLy_TatCa);
                    TextView _tvHanXuLyQuaHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_HanXuLy_QuaHan);
                    TextView _tvHanXuLyTrongHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_HanXuLy_TrongHan);
                    TextView _tvNgayGuiDen = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_NgayGuiDen);
                    _tvNgayGuiDenTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_NgayGuiDen_TuNgay);
                    _tvNgayGuiDenDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_NgayGuiDen_DenNgay);
                    LinearLayout _lnNgayGuiDenTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVDT_NgayGuiDen_TuNgay);
                    LinearLayout _lnNgayGuiDenDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVDT_NgayGuiDen_DenNgay);
                    TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVDT_NgayGuiDen_ApDung);

                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Trạng thái");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Tất cả");
                        _tvTrangThaiChuaXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Chưa xử lý");
                        _tvTrangThaiDaXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đã xử lý");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Hạn xử lý");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Tất cả");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Quá hạn");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Trong hạn");
                        _tvNgayGuiDen.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Ngày gửi đến");
                        _tvNgayGuiDenTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Từ ngày");
                        _tvNgayGuiDenDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đến ngày");
                        _tvApDung.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Áp dụng");
                    }
                    else
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Status");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "All");
                        _tvTrangThaiChuaXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Not process");
                        _tvTrangThaiDaXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Processed");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Due date");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "All");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Overdue");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "In due");
                        _tvNgayGuiDen.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Arrival day");
                        _tvNgayGuiDenTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Start date");
                        _tvNgayGuiDenDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "End date");
                        _tvApDung.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Apply");
                    }

                    foreach (var item in CTRLHomePage.LstFilterCondition_VDT)
                    {
                        if (item.Key.Equals("Trạng thái"))
                        {
                            _flagTrangThai = item.Value;
                            if (item.Value.Equals("1"))
                            {
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                            }
                            else if (item.Value.Equals("2"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                            }
                            else if (item.Value.Equals("3"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                            }
                        }
                        else if (item.Key.Equals("Hạn xử lý"))
                        {
                            _flagHanXuLy = item.Value;
                            if (item.Value.Equals("1"))
                            {
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                            else if (item.Value.Equals("2"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                            else if (item.Value.Equals("3"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                        }
                        else if (item.Key.Equals("Từ ngày"))
                        {
                            _flagTuNgay = item.Value;
                            if (!String.IsNullOrEmpty(_flagTuNgay))
                                _tvNgayGuiDenTuNgay.Text = _flagTuNgay;
                        }
                        else if (item.Key.Equals("Đến ngày"))
                        {
                            _flagDenNgay = item.Value;
                            if (!String.IsNullOrEmpty(_flagDenNgay))
                                _tvNgayGuiDenDenNgay.Text = _flagDenNgay;
                        }
                    }
                    #endregion

                    #region Event
                    _tvTrangThaiTatCa.Click += delegate
                    {
                        _flagTrangThai = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                    };
                    _tvTrangThaiChuaXuLy.Click += delegate
                    {
                        _flagTrangThai = "2";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                    };
                    _tvTrangThaiDaXuLy.Click += delegate
                    {
                        _flagTrangThai = "3";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaXuLy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaXuLy);
                    };

                    _tvHanXuLyTatCa.Click += delegate
                    {
                        _flagHanXuLy = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyQuaHan.Click += delegate
                    {
                        _flagHanXuLy = "2";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyTrongHan.Click += delegate
                    {
                        _flagHanXuLy = "3";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTrongHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                    };

                    _lnNgayGuiDenTuNgay.Click += delegate
                    {
                        _flagDatePicker = 1;
                        ShowDatePickerDialog();
                    };
                    _lnNgayGuiDenDenNgay.Click += delegate
                    {
                        _flagDatePicker = 2;
                        ShowDatePickerDialog();
                    };

                    _tvApDung.Click += delegate
                    {
                        if (_tvNgayGuiDenTuNgay.Text.Contains("/") && _tvNgayGuiDenDenNgay.Text.Contains("/") &&
                        DateTime.ParseExact(_tvNgayGuiDenTuNgay.Text, "dd/MM/yyyy", null) > DateTime.ParseExact(_tvNgayGuiDenDenNgay.Text, "dd/MM/yyyy", null))

                        {
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, "Thông báo", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", null, "Đóng");
                            }
                            else
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, "Alert", "End date must be greater or equal start date.", null, "Close");
                            }
                            return;
                        }
                        if (!_tvNgayGuiDenTuNgay.Text.Equals("Từ ngày") && !_tvNgayGuiDenTuNgay.Text.Equals("Start date"))
                        {
                            _flagTuNgay = _tvNgayGuiDenTuNgay.Text;
                        }
                        if (!_tvNgayGuiDenDenNgay.Text.Equals("Đến ngày") && !_tvNgayGuiDenDenNgay.Text.Equals("End date"))
                        {
                            _flagDenNgay = _tvNgayGuiDenDenNgay.Text;
                        }
                        _flagIsFiltering = 1;
                        SetLinearFilter_ByFlag(1);
                        CTRLHomePage.LstFilterCondition_VDT = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                            new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                            new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                            new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                        };
                        FilterData();
                        CmmDroidFunction.HideProcessingDialog();
                        _popupFilter.Dismiss();
                    };
                    #endregion
                }
                else // việc tôi bắt đầu
                {
                    #region Init Flag and View
                    _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupHomePageFilterVTBD, null);
                    _popupFilter = new PopupWindow(_popupViewFilter, _displayMetrics.WidthPixels, (int)(_lnContent.MeasuredHeight * 0.75));

                    TextView _tvTrangThai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai);
                    TextView _tvTrangThaiTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_TatCa);
                    TextView _tvTrangThaiChuaKetThuc = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_ChuaKetThuc);
                    TextView _tvTrangThaiDaDuyet = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_DaDuyet);
                    TextView _tvTrangThaiTuChoi = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_TuChoi);
                    TextView _tvTrangThaiDaHuy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_DaHuy);
                    TextView _tvTrangThaiNhap = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_TrangThai_Nhap);
                    TextView _tvHanXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_HanXuLy);
                    TextView _tvHanXuLyTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_HanXuLy_TatCa);
                    TextView _tvHanXuLyQuaHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_HanXuLy_QuaHan);
                    TextView _tvHanXuLyTrongHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_HanXuLy_TrongHan);
                    TextView _tvNgayKhoiTao = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao);
                    _tvNgayKhoiTaoTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);
                    _tvNgayKhoiTaoDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);
                    LinearLayout _lnNgayKhoiTaoTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);
                    LinearLayout _lnNgayKhoiTaoDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);
                    TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_ApDung);
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Trạng thái");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Tất cả");
                        _tvTrangThaiChuaKetThuc.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Chưa kết thúc");
                        _tvTrangThaiDaDuyet.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đã duyệt");
                        _tvTrangThaiTuChoi.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Từ chối");
                        _tvTrangThaiDaHuy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đã hủy");
                        _tvTrangThaiNhap.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Nháp");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Hạn xử lý");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Tất cả");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Quá hạn");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Trong hạn");
                        _tvNgayKhoiTao.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Ngày khởi tạo");
                        _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Từ ngày");
                        _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đến ngày");
                        _tvApDung.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Áp dụng");
                    }
                    else
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Status");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "All");
                        _tvTrangThaiChuaKetThuc.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Not end");
                        _tvTrangThaiDaDuyet.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Approved");
                        _tvTrangThaiTuChoi.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Reject");
                        _tvTrangThaiDaHuy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Cancelled");
                        _tvTrangThaiNhap.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Draft");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Due date");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "All");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Overdue");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "In due");
                        _tvNgayKhoiTao.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Created day");
                        _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Start date");
                        _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "End date");
                        _tvApDung.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Apply");
                    }
                    foreach (var item in CTRLHomePage.LstFilterCondition_VTBD)
                    {
                        if (item.Key.Equals("Trạng thái"))
                        {
                            _flagTrangThai = item.Value;
                            if (item.Value.Equals("1"))
                            {
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("2"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("3"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("4"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("5"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("6"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                        }
                        else if (item.Key.Equals("Hạn xử lý"))
                        {
                            _flagHanXuLy = item.Value;
                            if (item.Value.Equals("1"))
                            {
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                            else if (item.Value.Equals("2"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                            else if (item.Value.Equals("3"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTrongHan);
                            }
                        }
                        else if (item.Key.Equals("Từ ngày"))
                        {
                            _flagTuNgay = item.Value;
                            if (!String.IsNullOrEmpty(_flagTuNgay))
                                _tvNgayKhoiTaoTuNgay.Text = _flagTuNgay;
                        }
                        else if (item.Key.Equals("Đến ngày"))
                        {
                            _flagDenNgay = item.Value;
                            if (!String.IsNullOrEmpty(_flagDenNgay))
                                _tvNgayKhoiTaoDenNgay.Text = _flagDenNgay;
                        }
                    }
                    #endregion

                    #region Event
                    _tvTrangThaiTatCa.Click += delegate
                    {
                        _flagTrangThai = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiChuaKetThuc.Click += delegate
                    {
                        _flagTrangThai = "2";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiDaDuyet.Click += delegate
                    {
                        _flagTrangThai = "3";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiTuChoi.Click += delegate
                    {
                        _flagTrangThai = "4";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiDaHuy.Click += delegate
                    {
                        _flagTrangThai = "5";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiNhap.Click += delegate
                    {
                        _flagTrangThai = "6";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi, false);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiNhap);
                        if (CmmVariable.SysConfig.LangCode == "VN")
                        {
                            _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Từ ngày");
                            _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Đến ngày");
                        }
                        else
                        {
                            _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "Start date");
                            _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("K_Mess_PopupFilter", "End date");
                        }
                    };

                    _tvHanXuLyTatCa.Click += delegate
                    {
                        _flagHanXuLy = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyQuaHan.Click += delegate
                    {
                        _flagHanXuLy = "2";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyTrongHan.Click += delegate
                    {
                        _flagHanXuLy = "3";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };

                    _lnNgayKhoiTaoTuNgay.Click += delegate
                    {
                        if (!_flagTrangThai.Equals("6")) // Nháp ko cho hiện lên
                        {
                            _flagDatePicker = 3;
                            ShowDatePickerDialog();
                        }
                    };
                    _lnNgayKhoiTaoDenNgay.Click += delegate
                    {
                        if (!_flagTrangThai.Equals("6")) // Nháp ko cho hiện lên
                        {
                            _flagDatePicker = 4;
                            ShowDatePickerDialog();
                        }
                    };

                    _tvApDung.Click += delegate
                    {
                        if (_tvNgayKhoiTaoTuNgay.Text.Contains("/") && _tvNgayKhoiTaoDenNgay.Text.Contains("/") &&
                        DateTime.ParseExact(_tvNgayKhoiTaoTuNgay.Text, "dd/MM/yyyy", null) > DateTime.ParseExact(_tvNgayKhoiTaoDenNgay.Text, "dd/MM/yyyy", null))
                        {
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, "Thông báo", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.", null, "Đóng");
                            }
                            else
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, "Alert", "End date must be greater or equal start date.", null, "Close");
                            }
                            return;
                        }
                        if (!_tvNgayKhoiTaoTuNgay.Text.Equals("Từ ngày") && !_tvNgayKhoiTaoTuNgay.Text.Equals("Start date"))
                        {
                            _flagTuNgay = _tvNgayKhoiTaoTuNgay.Text;
                        }
                        if (!_tvNgayKhoiTaoDenNgay.Text.Equals("Đến ngày") && !_tvNgayKhoiTaoDenNgay.Text.Equals("End date"))
                        {
                            _flagDenNgay = _tvNgayKhoiTaoDenNgay.Text;
                        }
                        _flagIsFiltering = 1;
                        SetLinearFilter_ByFlag(1);
                        CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                            new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                            new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                            new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                        };
                        FilterData();
                        CmmDroidFunction.HideProcessingDialog();
                        _popupFilter.Dismiss();
                    };
                    #endregion
                }
                _popupFilter.Focusable = true;
                _popupFilter.OutsideTouchable = false;
                _popupFilter.ShowAsDropDown(_relaToolbar);
                _popupFilter.DismissEvent += (sender, e) =>
                {
                    _lnContent.Alpha = 1;
                };
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_lnFilter - Error: " + ex.Message);
#endif
            }
            */
        }
        private void Click_imgSearch(object sender, EventArgs e)
        {

        }
        private void Click_ItemNotifyAdapter(object sender, BeanNotify e)
        {
            try
            {
                // Update itemView ListView
                _NotifyVDTAdapter.UpdateSelectedPosition(e);
                _NotifyVDTAdapter.NotifyDataSetChanged();
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanWorkflowItem WHERE ItemID = {0} AND ListName = '{1}'", e.DocumentID, e.ListName);
                var lstWorkflowItem = conn.Query<BeanWorkflowItem>(queryNotify);
                if (lstWorkflowItem != null & lstWorkflowItem.Count > 0)
                {
                    LoadDetailView(lstWorkflowItem[0]);
                }
                else
                {
                    LoadDetailView(null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemNotifyAdapter - Error: " + ex.Message);
#endif
            }
        }
        private void Click_ItemWorkflowAdapter(object sender, BeanWorkflowItem e)
        {
            try
            {
                // Update itemView ListView
                _NotifyVTBDAdapter.UpdateSelectedPosition(e);
                _NotifyVTBDAdapter.NotifyDataSetChanged();
                LoadDetailView(e);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemWorkflowAdapter - Error: " + ex.Message);
#endif
            }
        }
        private void Click_DeleteSearch(object sender, EventArgs e)
        {
            try
            {
                _edtSearch.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentListWorkflow - Click_DeleteSearch - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomHome(object sender, EventArgs e)
        {
            try
            {
                //_mainAct.HideFragment();
                MinionAction._FlagNavigation = 1; // Home
                MinionAction.OnRefreshFragmentLeftMenu(null, null);
                //Android.App.Fragment frg = FragmentManager.FindFragmentByTag("FragmentHomePage");
                //if (frg != null)
                //{
                //    _mainAct.FragmentManager.BeginTransaction().Remove(frg).Commit();
                //    FragmentHomePage home = new FragmentHomePage(true, false);
                //    _mainAct.ShowFragment(FragmentManager, home, "FragmentHomePage", 1);
                //}
                //else
                //{
                //    FragmentHomePage home = new FragmentHomePage(true, false);
                //    _mainAct.AddFragment(FragmentManager, home, "FragmentHomePage", 1);

                //MinionAction.OnRefreshFragmentHomePage(null, null);

                //_mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomVDT(object sender, EventArgs e)
        {
            try
            {
                if (_type.Equals("VTBD"))
                {
                    _imgBottomVTBD.SetColorFilter(Resources.GetColor(Resource.Color.clBottomDisable));
                    _imgBottomVDT.SetColorFilter(Resources.GetColor(Resource.Color.clBottomEnable));
                    _tvBottomVTBD.Visibility = ViewStates.Gone;
                    _tvBottomVDT.Visibility = ViewStates.Visible;

                    _type = "VDT";
                    ReloadDataNavigation();
                    MinionAction._FlagNavigation = 2; // Home
                    MinionAction.OnRefreshFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomVTBD(object sender, EventArgs e)
        {
            try
            {
                if (_type.Equals("VDT"))
                {
                    _imgBottomVTBD.SetColorFilter(Resources.GetColor(Resource.Color.clBottomEnable));
                    _imgBottomVDT.SetColorFilter(Resources.GetColor(Resource.Color.clBottomDisable));
                    _tvBottomVTBD.Visibility = ViewStates.Visible;
                    _tvBottomVDT.Visibility = ViewStates.Gone;

                    _type = "VTBD";
                    ReloadDataNavigation();
                    MinionAction._FlagNavigation = 3; // VTBD
                    MinionAction.OnRefreshFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }
        private void TextChanged_edtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtSearch.Text)) // empty -> tra lai ban dau
                {
                    _imgDeleteSearch.Visibility = ViewStates.Gone;

                    if (_flagIsFiltering == 0) // search tren list all
                    {
                        if (_type == "VDT") _lstVDT_Search = _lstVDT.ToList();
                        else if (_type == "VTBD") _lstVTBD_Search = _lstVTBD.ToList();
                    }
                    else if (_flagIsFiltering == 1) // search tren list đã filter
                    {
                        if (_type == "VDT") _lstVDT_Search = _lstVDT_Filter.ToList();
                        else if (_type == "VTBD") _lstVTBD_Search = _lstVTBD_Filter.ToList();
                    }
                    #region Set List
                    if (_type == "VDT")
                    {
                        if (_lstVDT_Search != null && _lstVDT_Search.Count > 0)
                        {
                            SetTitleByListCount(_lstVDT_Search.Count);
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvData.Visibility = ViewStates.Visible;
                            _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT_Search, _mainAct);
                            _NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                            _lvData.Adapter = _NotifyVDTAdapter;
                        }
                        else
                        {
                            SetTitleByListCount(0);
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvData.Visibility = ViewStates.Gone;
                        }
                    }
                    else if (_type == "VTBD")
                    {
                        if (_lstVTBD_Search != null && _lstVTBD_Search.Count > 0)
                        {
                            SetTitleByListCount(_lstVTBD_Search.Count);
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvData.Visibility = ViewStates.Visible;
                            _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD_Search, _mainAct);
                            _NotifyVTBDAdapter.CustomItemClick += Click_ItemWorkflowAdapter;
                            _lvData.Adapter = _NotifyVTBDAdapter;
                        }
                        else
                        {
                            SetTitleByListCount(0);
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvData.Visibility = ViewStates.Gone;
                        }
                    }
                    #endregion
                }
                else
                {
                    _imgDeleteSearch.Visibility = ViewStates.Visible;
                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text).ToLowerInvariant().Trim();
                    if (_flagIsFiltering == 0) // search tren list all
                    {
                        if (_type == "VDT")
                        {
                            _lstVDT_Search.Clear();
                            _lstVDT_Search = _lstVDT.Where(x => CmmFunction.removeSignVietnamese(x.Title).ToLowerInvariant().Contains(_content) ||
                                                                CmmFunction.removeSignVietnamese(x.TitleEN).ToLowerInvariant().Contains(_content)).ToList();
                        }
                        else if (_type == "VTBD")
                        {
                            _lstVTBD_Search.Clear();
                            _lstVTBD_Search = _lstVTBD.Where(x => CmmFunction.removeSignVietnamese(x.WorkflowTitle).ToLowerInvariant().Contains(_content)).ToList();
                        }
                    }
                    else if (_flagIsFiltering == 1) // search tren list đã filter
                    {
                        if (_type == "VDT")
                        {
                            _lstVDT_Search.Clear();
                            _lstVDT_Search = _lstVDT_Filter.Where(x => CmmFunction.removeSignVietnamese(x.Title).ToLowerInvariant().Contains(_content) ||
                                                                       CmmFunction.removeSignVietnamese(x.TitleEN).ToLowerInvariant().Contains(_content)).ToList();
                        }
                        else if (_type == "VTBD")
                        {
                            _lstVTBD_Search.Clear();
                            _lstVTBD_Search = _lstVTBD_Filter.Where(x => CmmFunction.removeSignVietnamese(x.WorkflowTitle).ToLowerInvariant().Contains(_content)).ToList();
                        }
                    }
                    if (_type == "VDT")
                    {
                        if (_lstVDT_Search != null && _lstVDT_Search.Count > 0)
                        {
                            SetTitleByListCount(_lstVDT_Search.Count);
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvData.Visibility = ViewStates.Visible;
                            _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT_Search, _mainAct);
                            _NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                            _lvData.Adapter = _NotifyVDTAdapter;
                        }
                        else
                        {
                            SetTitleByListCount(0);
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvData.Visibility = ViewStates.Gone;
                        }
                    }
                    else if (_type == "VTBD")
                    {
                        if (_lstVTBD_Search != null && _lstVTBD_Search.Count > 0)
                        {
                            SetTitleByListCount(_lstVTBD_Search.Count);
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvData.Visibility = ViewStates.Visible;
                            _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD_Search, _mainAct);
                            _NotifyVTBDAdapter.CustomItemClick += Click_ItemWorkflowAdapter;
                            _lvData.Adapter = _NotifyVTBDAdapter;
                        }
                        else
                        {
                            SetTitleByListCount(0);
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvData.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void Click_ShowDetailProcess(object sender, EventArgs e)
        {
            try
            {
                TranslateAnimation anim = new TranslateAnimation(100f, 0f, 0f, 0f);
                anim.Duration = 500;
                _lnDetailProcess.Animation = anim; // side left to right
                _lnDetailProcess.Visibility = ViewStates.Visible;
                _lnDetailData.Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_DetailProcess - Error: " + ex.Message);
#endif
            }
        }
        private void Click_HideDetailProcess(object sender, EventArgs e)
        {
            try
            {
                //TranslateAnimation anim = new TranslateAnimation(0f, 100f, 0f, 0f);
                //anim.Duration = 500;
                //_lnDetailProcess.Animation = anim; // side left to right
                _lnDetailProcess.Visibility = ViewStates.Gone;
                _lnDetailData.Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_DetailProcess - Error: " + ex.Message);
#endif
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                if (_type == "VDT")
                {
                    _lstVDT.Clear();
                    _queryVDT = CTRLHomePage._queryVDT_Count;
                    _lstVDT = conn.Query<BeanNotify>(_queryVDT);

                    if (_lstVDT != null && _lstVDT.Count > 0)
                    {
                        SetTitleByListCount(_lstVDT.Count);
                        _lnNoData.Visibility = ViewStates.Gone;
                        _lvData.Visibility = ViewStates.Visible;
                        _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT, _mainAct);
                        _NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                        _lvData.Adapter = _NotifyVDTAdapter;

                        if (_selectedWorkflowItem != null)
                        {
                            LoadDetailView(_selectedWorkflowItem);
                        }
                        else
                        {
                            Click_ItemNotifyAdapter(null, _lstVDT[0]);
                        }

                    }
                    else
                    {
                        LoadDetailView(_selectedWorkflowItem);
                        SetTitleByListCount(0);
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvData.Visibility = ViewStates.Gone;
                    }
                }
                else if (_type == "VTBD")
                {
                    _lstVTBD.Clear();
                    _queryVTBD = CTRLHomePage._queryVTBD_Count;
                    _lstVTBD = conn.Query<BeanWorkflowItem>(_queryVTBD);

                    if (_lstVTBD != null && _lstVTBD.Count > 0)
                    {
                        SetTitleByListCount(_lstVTBD.Count);
                        _lnNoData.Visibility = ViewStates.Gone;
                        _lvData.Visibility = ViewStates.Visible;
                        _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD, _mainAct);
                        _NotifyVTBDAdapter.CustomItemClick += Click_ItemWorkflowAdapter;
                        _lvData.Adapter = _NotifyVTBDAdapter;

                        if (_selectedWorkflowItem != null)
                        {
                            LoadDetailView(_selectedWorkflowItem);
                        }
                        else
                        {
                            Click_ItemWorkflowAdapter(null, _lstVTBD[0]);
                        }
                    }
                    else
                    {
                        LoadDetailView(_selectedWorkflowItem);
                        SetTitleByListCount(0);
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvData.Visibility = ViewStates.Gone;
                    }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - FragmentListWorkflow - Error: " + ex.Message);
#endif
            }
        }
        private void LoadDetailView(BeanWorkflowItem _item)
        {
            try
            {
                if (_item != null)
                {
                    _lnDetailData.Visibility = ViewStates.Visible;
                    _lnDetailProcess.Visibility = ViewStates.Gone;
                    if (!String.IsNullOrEmpty(_item.CreatedBy)) _tvDetailAvatar.Text = _item.CreatedBy.Substring(0, 2).ToUpper();
                    else _tvDetailAvatar.Text = "";

                    if (!String.IsNullOrEmpty(_item.Title)) _tvDetailTitle1.Text = _item.Title;
                    else _tvDetailTitle1.Text = "";

                    if (!String.IsNullOrEmpty(_item.WorkflowTitle)) _tvDetailTitle2.Text = _item.WorkflowTitle;
                    else _tvDetailTitle2.Text = "";

                    if (!String.IsNullOrEmpty(_item.CreatedByName)) _tvDetailCreatedName.Text = _item.CreatedByName;
                    else _tvDetailCreatedName.Text = "";

                    if (!String.IsNullOrEmpty(_item.CreatedByName)) _tvDetailToName.Text = "Đến: Bạn"; //// chưa rõ
                    else _tvDetailToName.Text = "Đến: Bạn";

                    #region Load Process

                    if (!String.IsNullOrEmpty(_item.Title)) _tvDetailProcessTitle.Text = _item.Title;
                    else _tvDetailProcessTitle.Text = "";

                    List<BeanQuaTrinhLuanChuyen> _lstQtlc = new List<BeanQuaTrinhLuanChuyen>();
                    List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                    BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                    BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", _item.ItemID.ToString());
                    BeanSearchProperty listName = new BeanSearchProperty("ListName", _item.ListName);

                    arrSearchProperty.Add(filterType);
                    arrSearchProperty.Add(keyWord);
                    arrSearchProperty.Add(listName);
                    string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                    ProviderControlDynamic pTicketRequest = new ProviderControlDynamic();
                    _lstQtlc = pTicketRequest.GetListUserWithFilter(searchProperty, 100, 0);
                    List<SupTitleQtlc> _lstProcess = GetSupList(_lstQtlc);

                    if (_lstProcess != null && _lstProcess.Count > 0)
                    {
                        AdapterExpandListProcess _processAdapter = new AdapterExpandListProcess(_rootView.Context, _lstProcess, _mainAct);
                        _expandDetailProcess.SetAdapter(_processAdapter);
                        for (int i = 0; i < _lstProcess.Count; i++)
                        {
                            _expandDetailProcess.ExpandGroup(i);
                        }
                    }
                    #endregion
                }
                else
                {
                    _lnDetailData.Visibility = ViewStates.Gone;
                    _lnDetailProcess.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - LoadDetailView - Error: " + ex.Message);
#endif
            }
        }
        private void FilterData()
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                if (_type == "VDT")
                {
                    _lstVDT_Filter.Clear();
                    _lstVDT_Filter = CTRLHomePage.FilterListVDT_ByCondition(_lstVDT, CTRLHomePage.LstFilterCondition_VDT);

                    if (_lstVDT_Filter != null && _lstVDT_Filter.Count > 0)
                    {
                        SetTitleByListCount(_lstVDT_Filter.Count);
                        _lnNoData.Visibility = ViewStates.Gone;
                        _lvData.Visibility = ViewStates.Visible;
                        _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT_Filter, _mainAct);
                        _NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                        _lvData.Adapter = _NotifyVDTAdapter;
                    }
                    else
                    {
                        SetTitleByListCount(0);
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvData.Visibility = ViewStates.Gone;
                    }
                }
                else if (_type == "VTBD")
                {
                    _lstVTBD_Filter.Clear();
                    _lstVTBD_Filter = CTRLHomePage.FilterListVTBD_ByCondition(_lstVTBD, CTRLHomePage.LstFilterCondition_VTBD);

                    if (_lstVTBD_Filter != null && _lstVTBD_Filter.Count > 0)
                    {
                        SetTitleByListCount(_lstVTBD_Filter.Count);
                        _lnNoData.Visibility = ViewStates.Gone;
                        _lvData.Visibility = ViewStates.Visible;
                        _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD_Filter, _mainAct);
                        _NotifyVTBDAdapter.CustomItemClick += Click_ItemWorkflowAdapter;
                        _lvData.Adapter = _NotifyVTBDAdapter;
                    }
                    else
                    {
                        SetTitleByListCount(0);
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvData.Visibility = ViewStates.Gone;
                    }
                }


            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - FragmentListWorkflow - Error: " + ex.Message);
#endif
            }
        }
        private void SetTitleByListCount(int _count)
        {
            if (_type == "VDT")
            {
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, "Đến tôi (" + _count.ToString() + ")", "(", ")");
                }
                else
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, "To me (" + _count.ToString() + ")", "(", ")");
                }
            }
            else if (_type == "VTBD")
            {
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, "Tôi bắt đầu (" + _count.ToString() + ")", "(", ")");
                }
                else
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, "From me (" + _count.ToString() + ")", "(", ")");
                }
            }
        }
        private void ReloadData(object sender, MinionAction.ChangeViewListWorkflow e)
        {
            try
            {
                if (e.IsSuccess == 1)
                {
                    _type = "VDT";
                    _selectedWorkflowItem = null;
                    SetViewByLanguage(null, null);
                    SetData();
                }
                else if (e.IsSuccess == 2)
                {
                    _type = "VTBD";
                    _selectedWorkflowItem = null;
                    SetViewByLanguage(null, null);
                    SetData();
                }
                else if (e.IsSuccess == 0)// refresh lai trang
                {
                    Swipe_RefreshData(null, null);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentListWorkflow - ReloadData - Error: " + ex.Message);
#endif
            }
        }
        private void ReloadDataNavigation()
        {
            try
            {
                _edtSearch.Text = "";
                SetViewByLanguage(null, null);
                SetData();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentListWorkflow - ReloadData - Error: " + ex.Message);
#endif
            }
        }
        private void ShowDatePickerDialog()
        {
            try
            {
                DateTime _time = DateTime.Now;
                switch (_flagDatePicker)
                {
                    case 1:
                        {
                            KeyValuePair<string, string> _startVDT = CTRLHomePage.LstFilterCondition_VDT[2];
                            if (!String.IsNullOrEmpty(_startVDT.Value)) _time = DateTime.ParseExact(_startVDT.Value, "dd/MM/yyyy", null);
                            break;
                        }
                    case 2:
                        {
                            KeyValuePair<string, string> _endVDT = CTRLHomePage.LstFilterCondition_VDT[3];
                            if (!String.IsNullOrEmpty(_endVDT.Value)) _time = DateTime.ParseExact(_endVDT.Value, "dd/MM/yyyy", null);
                            break;
                        }
                    case 3:
                        {
                            KeyValuePair<string, string> _startVTBD = CTRLHomePage.LstFilterCondition_VTBD[2];
                            if (!String.IsNullOrEmpty(_startVTBD.Value)) _time = DateTime.ParseExact(_startVTBD.Value, "dd/MM/yyyy", null);
                            break;
                        }
                    case 4:
                        {
                            KeyValuePair<string, string> _endVTBD = CTRLHomePage.LstFilterCondition_VTBD[3];
                            if (!String.IsNullOrEmpty(_endVTBD.Value)) _time = DateTime.ParseExact(_endVTBD.Value, "dd/MM/yyyy", null);
                            break;
                        }
                }
                DatePickerDialog datePicker = new DatePickerDialog(_mainAct, this, _time.Year, _time.Month - 1, _time.Day);
                datePicker.Show();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - ShowDatePickerDialog - Error: " + ex.Message);
#endif
            }
        }
        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            try
            {
                DateTime _res = new DateTime(year, month + 1, dayOfMonth, 0, 0, 0);
                switch (_flagDatePicker)
                {
                    case 1:
                        {
                            _tvNgayGuiDenTuNgay.Text = _res.ToString("dd/MM/yyyy");
                            break;
                        }
                    case 2:
                        {
                            _tvNgayGuiDenDenNgay.Text = _res.ToString("dd/MM/yyyy");
                            break;
                        }
                    case 3:
                        {
                            _tvNgayKhoiTaoTuNgay.Text = _res.ToString("dd/MM/yyyy");
                            break;
                        }
                    case 4:
                        {
                            _tvNgayKhoiTaoDenNgay.Text = _res.ToString("dd/MM/yyyy");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - OnDateSet - Error: " + ex.Message);
#endif
            }
        }
        private List<SupTitleQtlc> GetSupList(List<BeanQuaTrinhLuanChuyen> lst)
        {
            try
            {
                List<SupTitleQtlc> supTitleQtlCs = new List<SupTitleQtlc>();
                SupTitleQtlc supTitleQtlc = new SupTitleQtlc();
                List<BeanQuaTrinhLuanChuyen> lstFilter = new List<BeanQuaTrinhLuanChuyen>();
                string step = lst[0].Action;
                for (int i = 0; i < lst.Count; i++)
                {
                    if (step != lst[i].Action)
                    {
                        supTitleQtlc.ListItemMenu = lstFilter;
                        supTitleQtlCs.Add(supTitleQtlc);
                        supTitleQtlc = new SupTitleQtlc();
                        lstFilter = new List<BeanQuaTrinhLuanChuyen>();
                    }
                    supTitleQtlc.TitleName = lst[i].Action;
                    lstFilter.Add(lst[i]);
                    step = lst[i].Action;
                }
                supTitleQtlc.ListItemMenu = lstFilter;
                supTitleQtlCs.Add(supTitleQtlc);

                return supTitleQtlCs;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentDetailWorkflow - Error: " + ex.Message);
#endif
            }
            return null;
        }
        #endregion
    }
}