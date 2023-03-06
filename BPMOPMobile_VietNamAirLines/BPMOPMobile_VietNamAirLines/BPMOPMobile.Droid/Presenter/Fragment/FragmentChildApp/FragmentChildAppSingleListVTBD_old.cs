using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
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
using BPMOPMobile.Droid.Presenter.SharedView;
using Com.Telerik.Widget.Calendar;
using Com.Telerik.Widget.Calendar.Events;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppSingleListVTBD_old : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView, _popupViewFilter;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerBase CTRLBase = new ControllerBase();
        private PopupWindow _popupFilter;

        private List<BeanWorkflowItem> _lstVTBD = new List<BeanWorkflowItem>();
        private SwipeRefreshLayout _swipe;
        private RelativeLayout _relaToolbar;
        private CircleImageView _imgAvata;
        private TextView _tvName, _tvNoData;
        private ImageView _imgFilter, _imgSearch, _imgDeleteSearch;
        private EditText _edtSearch;
        private RecyclerView _recyData;
        private LinearLayout _lnAll, _lnNoData, _lnContent, _lnBlackFilter, _lnDisablePager, _lnBottomNavigation;
        private AdapterHomePageRecyVTBD_Ver2 _adapterHomePageRecyVTBD;
        private string _type = "VTBD";
        private string _queryVTBD = "";
        private int _flagIsFiltering = 0;
        private bool _allowLoadMore = true;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            MinionAction.RenewFragmentSingleVTBD -= RenewData;
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra
            CTRLHomePage.InitListFilterCondition("BOTH");
            base.OnDestroyView();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            if (_rootView == null)
            {
                _mainAct.Window.SetNavigationBarColor(Color.Black);
                _rootView = inflater.Inflate(Resource.Layout.ViewVer2ListWorkflow, null);

                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_All);
                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewVer2ListWorkflow_Toolbar);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewVer2ListWorkflow_Avata);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewVer2ListWorkflow_Name);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewVer2ListWorkflow_Filter);
                _imgSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewVer2ListWorkflow_Search);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewVer2ListWorkflow_Search_Delete);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewVer2ListWorkflow_Search);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewVer2ListWorkflow);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_NoData);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_Content);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewVer2ListWorkflow_NoData);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_BottomNavigation);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_DisablePager);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewVer2ListWorkflow_BlackFilter);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewVer2ListWorkflow);

                _imgAvata.Click += Click_Menu;
                _imgSearch.Click += Click_imgSearch;
                _swipe.Refresh += Swipe_RefreshData;
                _imgFilter.Click += Click_imgFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                SetDataTitle();
                SetData();
                _imgDeleteSearch.Visibility = ViewStates.Gone;

                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation, true);
                bottomNavigation.InitializeView();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLHomePage.InitListFilterCondition("BOTH");
                    CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                    SetDataTitle();
                    SetLinearFilter_ByFlag(0);

                    if (!String.IsNullOrEmpty(_edtSearch.Text))
                    {
                        _edtSearch.Text = "";
                        _edtSearch.TextChanged -= TextChanged_edtSearch;
                        Action action = new Action(() =>
                        {
                            _edtSearch.Text = "";
                            _edtSearch.TextChanged += TextChanged_edtSearch;
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                    }
                }
            }
            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.RenewFragmentSingleVTBD += RenewData;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            SetViewByLanguage(null, null);

            return _rootView;
        }
        public FragmentChildAppSingleListVTBD_old()
        {

        }

        #region Event
        private void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");

                }
                else
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_FROMME", "From me") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search");
                }
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");

                if (_adapterHomePageRecyVTBD != null)
                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }
        private void SetTitleByListCount(int _count)
        {
            if (_type == "VDT")
            {
                CTRLHomePage.SetTextview_FormatItemCount(_tvName, _count, "VDT");
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");
            }
            else if (_type == "VTBD")
            {
                CTRLHomePage.SetTextview_FormatItemCount(_tvName, _count, "VTBD");
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");
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
                        _swipe.Refreshing = false;
                        _swipe.Enabled = true;
                        Action action = new Action(() =>
                        {
                            SetLinearFilter_ByFlag(0);
                            CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                            SetViewByLanguage(null, null);
                            SetData();
                            _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                            _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 50);
                    });
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Swipe_RefreshData", ex);
#endif
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                    _swipe.Enabled = true;
                });
            }
        }
        private async void RefreshData(object sender, EventArgs e)
        {
            try // giống Swipe nhưng ko có event xoay
            {
                await Task.Run(() =>
                {
                    _flagIsFiltering = 0;
                    CTRLHomePage.InitListFilterCondition("BOTH");

                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);
                    _mainAct.RunOnUiThread(() =>
                    {
                        SetLinearFilter_ByFlag(0);
                        SetViewByLanguage(null, null);
                        SetData();
                    });
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Swipe_RefreshData", ex);
#endif
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
                if (_type == "VDT")
                    MinionAction._FlagNavigation = (int)EnumBottomNavigationView.SingleListVDT;
                else
                    MinionAction._FlagNavigation = (int)EnumBottomNavigationView.SingleListVTBD;

                MinionAction.OnRenewDataAndShowFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Menu", ex);
#endif
            }
        }
        private void Click_imgFilter(object sender, EventArgs e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == true)
            {
                _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                string _flagTrangThai = "", _flagHanXuLy = "", _flagTuNgay = "", _flagDenNgay = "";
                try
                {
                    _lnBlackFilter.Visibility = ViewStates.Visible;
                    DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                    LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);


                    #region Get View
                    _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupHomePageFilterVTBD, null);
                    _popupFilter = new PopupWindow(_popupViewFilter, _displayMetrics.WidthPixels, WindowManagerLayoutParams.WrapContent);

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
                    TextView _tvNgayKhoiTaoTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);
                    TextView _tvNgayKhoiTaoDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);

                    LinearLayout _lnHanXuLy = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVTBD_HanXuLy);

                    LinearLayout _lnNgayKhoiTaoTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);
                    RadCalendarView _calendarTuNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);
                    ImageView _imgNgayKhoiTaoTuNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupHomePageFilterVTBD_NgayKhoiTao_TuNgay);

                    LinearLayout _lnNgayKhoiTaoDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);
                    RadCalendarView _calendarDenNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);
                    ImageView _imgNgayKhoiTaoDenNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupHomePageFilterVTBD_NgayKhoiTao_DenNgay);

                    TextView _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_MacDinh);
                    TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupHomePageFilterVTBD_NgayKhoiTao_ApDung);

                    CTRLBase.InitRadCalendarView(_calendarTuNgay, _tvNgayKhoiTaoTuNgay);
                    CTRLBase.InitRadCalendarView(_calendarDenNgay, _tvNgayKhoiTaoDenNgay);
                    #endregion

                    #region Init Data
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                        _tvTrangThaiChuaKetThuc.Text = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Chờ duyệt");
                        _tvTrangThaiDaDuyet.Text = CmmFunction.GetTitle("TEXT_APPROVED", "Đã duyệt");
                        _tvTrangThaiTuChoi.Text = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
                        _tvTrangThaiDaHuy.Text = CmmFunction.GetTitle("TEXT_CANCELED", "Đã hủy");
                        _tvTrangThaiNhap.Text = CmmFunction.GetTitle("TEXT_DRAFT", "Nháp");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn xử lý");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("TEXT_OVERDUE", "Quá hạn");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("TEXT_ONTIME", "Trong hạn");
                        _tvNgayKhoiTao.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Ngày khởi tạo");
                        _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                        _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
                        _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Thiết lập lại");
                        _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");
                    }
                    else
                    {
                        _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Status");
                        _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "All");
                        _tvTrangThaiChuaKetThuc.Text = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Waiting");
                        _tvTrangThaiDaDuyet.Text = CmmFunction.GetTitle("TEXT_APPROVED", "Approved");
                        _tvTrangThaiTuChoi.Text = CmmFunction.GetTitle("TEXT_REJECT", "Rejected");
                        _tvTrangThaiDaHuy.Text = CmmFunction.GetTitle("TEXT_CANCELED", "Cancelled");
                        _tvTrangThaiNhap.Text = CmmFunction.GetTitle("TEXT_DRAFT", "Draft");
                        _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Due date");
                        _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "All");
                        _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("TEXT_OVERDUE", "Overdue");
                        _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("TEXT_ONTIME", "On time");
                        _tvNgayKhoiTao.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Created date");
                        _tvNgayKhoiTaoTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "From date");
                        _tvNgayKhoiTaoDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "To date");
                        _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Setting");
                        _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Apply");
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
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("2"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("3"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("4"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTuChoi);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("5"))
                            {
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                                CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaHuy);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                            }
                            else if (item.Value.Equals("6"))
                            {
                                CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, false); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                                CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
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
                            {
                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                    _tvNgayKhoiTaoTuNgay.Text = _flagTuNgay;
                                else
                                {
                                    DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                                    _tvNgayKhoiTaoTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                                }
                            }

                        }
                        else if (item.Key.Equals("Đến ngày"))
                        {
                            _flagDenNgay = item.Value;
                            if (!String.IsNullOrEmpty(_flagDenNgay))
                            {
                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                    _tvNgayKhoiTaoDenNgay.Text = _flagDenNgay;
                                else
                                {
                                    DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                                    _tvNgayKhoiTaoDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                                }
                            }
                        }
                    }

                    _calendarTuNgay.SelectionMode = CalendarSelectionMode.Single;
                    _calendarTuNgay.DisplayMode = CalendarDisplayMode.Month;
                    _calendarTuNgay.EventsDisplayMode = EventsDisplayMode.Inline;
                    _calendarTuNgay.HorizontalScroll = true; //xem ngang 

                    _calendarDenNgay.SelectionMode = CalendarSelectionMode.Single;
                    _calendarDenNgay.DisplayMode = CalendarDisplayMode.Month;
                    _calendarDenNgay.EventsDisplayMode = EventsDisplayMode.Inline;
                    _calendarDenNgay.HorizontalScroll = true; //xem ngang 

                    _calendarTuNgay.OnSelectedDatesChangedListener = new RadCalendar_SelectedDatesChangedListener(_tvNgayKhoiTaoTuNgay);
                    _calendarDenNgay.OnSelectedDatesChangedListener = new RadCalendar_SelectedDatesChangedListener(_tvNgayKhoiTaoDenNgay);
                    #endregion

                    #region Event
                    _tvTrangThaiTatCa.Click += (sender, e) =>
                    {
                        _flagTrangThai = "1";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiChuaKetThuc.Click += (sender, e) =>
                    {
                        _flagTrangThai = "2";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiDaDuyet.Click += (sender, e) =>
                    {
                        _flagTrangThai = "3";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiTuChoi.Click += (sender, e) =>
                    {
                        _flagTrangThai = "4";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiDaHuy.Click += (sender, e) =>
                    {
                        _flagTrangThai = "5";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);
                    };
                    _tvTrangThaiNhap.Click += (sender, e) =>
                    {
                        _flagTrangThai = "6";
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, false); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiNhap);
                    };

                    _tvHanXuLyTatCa.Click += (sender, e) =>
                    {
                        _flagHanXuLy = "1";
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyQuaHan.Click += (sender, e) =>
                    {
                        _flagHanXuLy = "2";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };
                    _tvHanXuLyTrongHan.Click += (sender, e) =>
                    {
                        _flagHanXuLy = "3";
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTrongHan);
                    };

                    _lnNgayKhoiTaoTuNgay.Click += (sender, e) =>
                    {
                        if (_calendarTuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
                        {
                            _imgNgayKhoiTaoTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                            _imgNgayKhoiTaoDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                            _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                            _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                            _calendarTuNgay.Visibility = ViewStates.Gone;
                            _calendarDenNgay.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            _imgNgayKhoiTaoTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                            _imgNgayKhoiTaoDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                            _imgNgayKhoiTaoDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                            _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                            _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                            if (_calendarDenNgay.Visibility == ViewStates.Visible)
                                _calendarDenNgay.Visibility = ViewStates.Gone;

                            _calendarTuNgay.Animation = ControllerAnimation.GetAnimationSwipe_TopToBot(_calendarTuNgay);
                            _calendarTuNgay.Visibility = ViewStates.Visible;

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
                        }
                    };
                    _lnNgayKhoiTaoDenNgay.Click += (sender, e) =>
                    {
                        if (_calendarDenNgay.Visibility == ViewStates.Visible)
                        {
                            _imgNgayKhoiTaoTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                            _imgNgayKhoiTaoDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                            _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                            _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                            _calendarTuNgay.Visibility = ViewStates.Gone;
                            _calendarDenNgay.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            _imgNgayKhoiTaoDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                            _imgNgayKhoiTaoTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                            _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                            _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                            if (_calendarTuNgay.Visibility == ViewStates.Visible)
                                _calendarTuNgay.Visibility = ViewStates.Gone;

                            _calendarDenNgay.Animation = ControllerAnimation.GetAnimationSwipe_TopToBot(_calendarDenNgay);
                            _calendarDenNgay.Visibility = ViewStates.Visible;


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
                        }
                    };
                    _tvNgayKhoiTaoTuNgay.TextChanged += (sender, e) =>
                    {
                        if (!_tvNgayKhoiTaoTuNgay.Text.Equals("Từ ngày") && !_tvNgayKhoiTaoTuNgay.Text.Equals("Start date"))
                        {
                            DateTime _temp = DateTime.ParseExact(_tvNgayKhoiTaoTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                            _flagTuNgay = _temp.ToString("dd/MM/yyyy");
                        }
                    };
                    _tvNgayKhoiTaoDenNgay.TextChanged += (sender, e) =>
                    {
                        if (!_tvNgayKhoiTaoDenNgay.Text.Equals("Đến ngày") && !_tvNgayKhoiTaoDenNgay.Text.Equals("End date"))
                        {
                            DateTime _temp = DateTime.ParseExact(_tvNgayKhoiTaoDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                            _flagDenNgay = _temp.ToString("dd/MM/yyyy");
                        }
                    };

                    _tvMacDinh.Click += (sender, e) =>
                    {
                        // Default Trạng thái
                        _flagTrangThai = CTRLHomePage.GetDefaultValue_FilterVTBD("Trạng thái");
                        CTRLHomePage.SetVisibleLinearDueDate(_lnHanXuLy, _tvHanXuLyTatCa, _tvHanXuLyQuaHan, _tvHanXuLyTrongHan, true); // Set Theo trạng thái Nháp -> disable / enable linear duedate
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTatCa);
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvTrangThaiChuaKetThuc);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaDuyet);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiTuChoi);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiDaHuy);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvTrangThaiNhap);

                        //Default Hạn xử lý
                        _flagHanXuLy = CTRLHomePage.GetDefaultValue_FilterVTBD("Hạn xử lý");
                        CTRLHomePage.SetTextview_Selected_Filter(_mainAct, _tvHanXuLyTatCa);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyQuaHan);
                        CTRLHomePage.SetTextview_NotSelected_Filter(_mainAct, _tvHanXuLyTrongHan);

                        //Default Ngày gửi đến
                        _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                        _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                        _flagTuNgay = CTRLHomePage.GetDefaultValue_FilterVTBD("Từ ngày");
                        _flagDenNgay = CTRLHomePage.GetDefaultValue_FilterVTBD("Đến ngày");

                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                            _tvNgayKhoiTaoTuNgay.Text = _flagTuNgay;
                        else
                        {
                            DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                            _tvNgayKhoiTaoTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                        }

                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                            _tvNgayKhoiTaoDenNgay.Text = _flagDenNgay;
                        else
                        {
                            DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                            _tvNgayKhoiTaoDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                        }

                    };
                    _tvApDung.Click += (sender, e) =>
                    {
                        #region Validate Data
                        if (_tvNgayKhoiTaoTuNgay.Text.Contains("/") && _tvNgayKhoiTaoDenNgay.Text.Contains("/") &&
                        DateTime.ParseExact(_tvNgayKhoiTaoTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null) >
                        DateTime.ParseExact(_tvNgayKhoiTaoDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null))
                        {
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"),
                                                                       CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Start date cannot be greater than end date, please choose again"));

                            _lnNgayKhoiTaoTuNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                            _lnNgayKhoiTaoDenNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                            return;
                        }
                        #endregion

                        #region Check xem có phải là Default Filter không
                        if (_flagTrangThai.Equals(CTRLHomePage.GetDefaultValue_FilterVTBD("Trạng thái")) && _flagHanXuLy.Equals(CTRLHomePage.GetDefaultValue_FilterVTBD("Hạn xử lý")) &&
                        _flagTuNgay.Equals(CTRLHomePage.GetDefaultValue_FilterVTBD("Từ ngày")) && _flagDenNgay.Equals(CTRLHomePage.GetDefaultValue_FilterVTBD("Đến ngày")))
                        {
                            _flagIsFiltering = 0;
                        }
                        else // Filter khác trạng thái Default
                        {
                            _flagIsFiltering = 1;
                        }
                        #endregion

                        #region Set giá trị và Filter
                        SetLinearFilter_ByFlag(_flagIsFiltering);
                        CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                            new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                            new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                            new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                        };
                        SetData();
                        CmmDroidFunction.HideProcessingDialog();
                        _edtSearch.Text = _edtSearch.Text; // để trigger lại hàm text changed
                        _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
                        #endregion

                        _popupFilter.Dismiss();
                        _lnBlackFilter.Visibility = ViewStates.Gone; // Tô nền đen dưới Popup
                    };
                    #endregion

                    _popupFilter.Focusable = true;
                    _popupFilter.OutsideTouchable = false;
                    _popupFilter.ShowAsDropDown(_relaToolbar);
                    _popupFilter.DismissEvent += (sender, e) =>
                    {
                        _lnBlackFilter.Visibility = ViewStates.Gone;
                    };
                }
                catch (Exception ex)
                {
                    CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
                }
            }
        }
        private void Click_imgSearch(object sender, EventArgs e)
        {

        }
        private void Click_ItemWorkflowAdapter(object sender, BeanWorkflowItem e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    BeanWorkflowItem _temp = e;
                    //SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    //string queryNotify = string.Format("SELECT * FROM BeanNotify WHERE SPItemId = {0} AND ListName = '{1}'", e.ItemID, e.ListName);
                    //var lstNotify = conn.Query<BeanNotify>(queryNotify);
                    //if (lstNotify != null && lstNotify.Count > 0)
                    //{
                    //    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_temp, lstNotify[0], "FragmentSingleListVTBD");
                    //    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                    //}
                    //else
                    //{
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_temp, null, "FragmentSingleListVTBD");
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                    //}
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemWorkflowAdapter", ex);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_DeleteSearch", ex);
#endif
            }
        }
        private void TextChanged_edtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtSearch.Text)) // empty -> tra lai ban dau -> giống như Set Data
                {
                    _imgDeleteSearch.Visibility = ViewStates.Gone;
                    SetData();
                }
                else
                {
                    _imgDeleteSearch.Visibility = ViewStates.Visible;
                    SetData();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_edtSearch", ex);
#endif
            }
        }
        private void SetLinearFilter_ByFlag(int flag)
        {
            if (flag == 1)
            {
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
            }
            else
            {
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
            }
        }
        private void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                CustomSpeedLinearLayoutManager _customLNM = (CustomSpeedLinearLayoutManager)_recyData.GetLayoutManager();
                int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();

                if (_type.ToLowerInvariant().Equals("vtbd"))
                {
                    if (_tempLastVisible == _lstVTBD.Count - 1 && _allowLoadMore == true)
                    {
                        _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        Action action = new Action(() =>
                        {
                            ProviderBase pBase = new ProviderBase();
                            List<BeanWorkflowItem> _lstMore = pBase.LoadMoreDataT<BeanWorkflowItem>(_queryVTBD, CmmDroidVariable.M_LoadMoreLimit, CmmDroidVariable.M_LoadMoreLimit, _lstVTBD.Count);

                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmDroidVariable.M_LoadMoreLimit)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstVTBD.AddRange(_lstMore);
                                _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                ////  _adapterHomePageRecyVTBD.LoadMore(_lstMore);
                                _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                            }
                            else
                            {
                                _allowLoadMore = false;
                                _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                            }
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_RecyData", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void SetDataTitle()
        {
            try
            {
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVTBD("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVTBD("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Đến ngày")),
                };

                string _queryTitleCount = CTRLHomePage.GetQueryStringVTBD_ByCondition(LstFilterDefault, false, true);
                var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);
                conn.Close();

                if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                    SetTitleByListCount(_lstTitleCount[0].Count);
                else
                    SetTitleByListCount(0);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataTitle", ex);
#endif
            }
        }
        private void SetData()
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                _lstVTBD.Clear();
                _queryVTBD = CTRLHomePage.GetQueryStringVTBD_ByCondition(CTRLHomePage.LstFilterCondition_VTBD, true, _searchString: _edtSearch.Text.ToString());
                _lstVTBD = pBase.LoadMoreDataT<BeanWorkflowItem>(_queryVTBD, CmmDroidVariable.M_LoadMoreLimit, CmmDroidVariable.M_LoadMoreLimit, _lstVTBD.Count);
                conn.Close();

                if (_lstVTBD != null && _lstVTBD.Count > 0)
                {
                    _recyData.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);

                    if (_lstVTBD.Count < CmmDroidVariable.M_LoadMoreLimit)
                        _allowLoadMore = false;
                    else
                        _allowLoadMore = true;

                    // _adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD(_rootView.Context, _lstVTBD, _mainAct);
                    InitRecyclerView(_adapterHomePageRecyVTBD);

                    _lnNoData.Visibility = ViewStates.Gone;
                    _recyData.Visibility = ViewStates.Visible;
                }
                else
                {
                    _lnNoData.Visibility = ViewStates.Visible;
                    _recyData.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }
        private void InitRecyclerView(AdapterHomePageRecyVTBD_Ver2 adapterHomePageRecyVTBD)
        {
            if (adapterHomePageRecyVTBD != null)
            {
                //adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                //adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemWorkflowAdapter;
                //adapterHomePageRecyVTBD.CustomItemClick += Click_ItemWorkflowAdapter;
                //_recyData.SetAdapter(adapterHomePageRecyVTBD);
                //_recyData.SetLayoutManager(new CustomSpeedLinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
            }
        }
        private void RenewData(object sender, EventArgs e)
        {
            try
            {
                SetLinearFilter_ByFlag(0);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                SetViewByLanguage(null, null);
                SetData();
                _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "RenewData", ex);
#endif
            }
        }
        private void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                if (e != null && _type.ToLowerInvariant().Equals("vtbd"))
                {
                    _adapterHomePageRecyVTBD.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_RenewItemVTBD_AfterFollowEvent", ex);
#endif
            }
        }
        #endregion

    }
}