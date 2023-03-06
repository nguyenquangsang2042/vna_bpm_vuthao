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
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    class FragmentChildAppSingleListVDT_old : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView, _popupViewFilter;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private PopupWindow _popupFilter;

        private SwipeRefreshLayout _swipe;
        private RelativeLayout _relaToolbar;
        private CircleImageView _imgAvata;
        private TextView _tvName, _tvNoData;
        private ImageView _imgFilter, _imgSearch, _imgDeleteSearch;
        private EditText _edtSearch;
        private RecyclerView _recyData;
        private LinearLayout _lnAll, _lnNoData, _lnContent, _lnDisablePager, _lnBottomNavigation;

        private List<JObject> _lstJObjectDynamic = new List<JObject>();
        private AdapterFragmentListStable _adapterRecy;

        private int _resourceViewID = 0;
        private string _type = "VDT";
        private int _flagIsFiltering = 0;
        private bool _allowLoadMore = true;
        public BeanWorkflow _currentWorkflow;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            MinionAction.RenewFragmentSingleVDT -= RenewData;
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
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewVer2ListWorkflow);

                _imgAvata.Click += Click_Menu;
                _imgSearch.Click += Click_imgSearch;
                _swipe.Refresh += Swipe_RefreshData;
                _imgFilter.Click += Click_imgFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };
                _imgDeleteSearch.Visibility = ViewStates.Gone;

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                SetDataTitle();
                SetData();

                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation,true);
                bottomNavigation.InitializeView();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    Action action = new Action(() =>
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
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
            }
            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.RenewFragmentSingleVDT += RenewData;
            SetViewByLanguage(null, null);

            return _rootView;
        }
        public FragmentChildAppSingleListVDT_old(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }

        #region Event
        private void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                }
                else
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_TOME", "To me") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search");
                }

                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");

                if (_adapterRecy != null)
                    _adapterRecy.NotifyDataSetChanged();
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
            if (CmmDroidFunction.PreventMultipleClick(1000) == true)
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                Action action = new Action(() =>
                {
                    _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    string _flagTrangThai = "", _flagHanXuLy = "", _flagTuNgay = "", _flagDenNgay = "";
                    try
                    {
                        DisplayMetrics _displayMetrics = Resources.DisplayMetrics;
                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                        #region Get View
                        _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupVer2FilterVDT, null);
                        _popupFilter = new PopupWindow(_popupViewFilter, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                        TextView _tvTrangThai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_TrangThai);
                        TextView _tvTrangThaiTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_TrangThai_TatCa);
                        TextView _tvTrangThaiChuaXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_TrangThai_CXL);
                        TextView _tvTrangThaiDaXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_TrangThai_DXL);
                        TextView _tvHanXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_HanXuLy);
                        TextView _tvHanXuLyTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_HanXuLy_TatCa);
                        TextView _tvHanXuLyQuaHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_HanXuLy_QuaHan);
                        TextView _tvHanXuLyTrongHan = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_HanXuLy_TrongHan);
                        TextView _tvNgayGuiDen = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_NgayGuiDen);
                        TextView _tvNgayGuiDenTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_NgayGuiDen_TuNgay);
                        TextView _tvNgayGuiDenDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_NgayGuiDen_DenNgay);

                        LinearLayout _lnNgayGuiDenTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterVDT_NgayGuiDen_TuNgay);
                        RadCalendarView _calendarTuNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer2FilterVDT_NgayGuiDen_TuNgay);
                        ImageView _imgNgayGuiDenTuNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterVDT_NgayGuiDen_TuNgay);

                        LinearLayout _lnNgayGuiDenDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterVDT_NgayGuiDen_DenNgay);
                        RadCalendarView _calendarDenNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer2FilterVDT_NgayGuiDen_DenNgay);
                        ImageView _imgNgayGuiDenDenNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterVDT_NgayGuiDen_DenNgay);

                        TextView _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_MacDinh);
                        TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterVDT_NgayGuiDen_ApDung);
                        LinearLayout _lnBlurBottom = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterVDT_BottomBlur);
                        LinearLayout _lnBlurTop = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterVDT_TopBlur);

                        CTRLHomePage.InitRadCalendarView(_calendarTuNgay, _tvNgayGuiDenTuNgay);
                        CTRLHomePage.InitRadCalendarView(_calendarDenNgay, _tvNgayGuiDenDenNgay);
                        #endregion

                        #region Init Data
                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        {
                            _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");
                            _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                            _tvTrangThaiChuaXuLy.Text = CmmFunction.GetTitle("TEXT_INPROCESS", "Chờ xử lý");
                            _tvTrangThaiDaXuLy.Text = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý");
                            _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn xử lý");
                            _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                            _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("TEXT_OVERDUE", "Quá hạn");
                            _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("TEXT_ONTIME", "Trong hạn");
                            _tvNgayGuiDen.Text = CmmFunction.GetTitle("TEXT_DATE_OF_ARRIVAL", "Ngày gửi đến");
                            _tvNgayGuiDenTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                            _tvNgayGuiDenDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
                            _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Thiết lập lại");
                            _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");
                        }
                        else
                        {
                            _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Status");
                            _tvTrangThaiTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "All");
                            _tvTrangThaiChuaXuLy.Text = CmmFunction.GetTitle("TEXT_INPROCESS", "In process");
                            _tvTrangThaiDaXuLy.Text = CmmFunction.GetTitle("TEXT_PROCESSED", "Processed");
                            _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Due date");
                            _tvHanXuLyTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "All");
                            _tvHanXuLyQuaHan.Text = CmmFunction.GetTitle("TEXT_OVERDUE", "Overdue");
                            _tvHanXuLyTrongHan.Text = CmmFunction.GetTitle("TEXT_ONTIME", "On time");
                            _tvNgayGuiDen.Text = CmmFunction.GetTitle("TEXT_DATE_OF_ARRIVAL", "Date of arrival");
                            _tvNgayGuiDenTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "From date");
                            _tvNgayGuiDenDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "To date");
                            _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_SETTING", "Setting");
                            _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Apply");
                        }
                        foreach (var item in CTRLHomePage.LstFilterCondition_VDT)
                        {
                            if (item.Key.Equals("Trạng thái"))
                            {
                                _flagTrangThai = item.Value;
                                if (item.Value.Equals("1"))
                                {
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                                }
                                else if (item.Value.Equals("2"))
                                {
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                                }
                                else if (item.Value.Equals("3"))
                                {
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                                }
                            }
                            else if (item.Key.Equals("Hạn xử lý"))
                            {
                                _flagHanXuLy = item.Value;
                                if (item.Value.Equals("1"))
                                {
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                                }
                                else if (item.Value.Equals("2"))
                                {
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                                }
                                else if (item.Value.Equals("3"))
                                {
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                                    CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                                }
                            }
                            else if (item.Key.Equals("Từ ngày"))
                            {
                                _flagTuNgay = item.Value;
                                if (!String.IsNullOrEmpty(_flagTuNgay))
                                {
                                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                        _tvNgayGuiDenTuNgay.Text = _flagTuNgay;
                                    else
                                    {
                                        DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                                        _tvNgayGuiDenTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                                    }
                                }
                            }
                            else if (item.Key.Equals("Đến ngày"))
                            {
                                _flagDenNgay = item.Value;
                                if (!String.IsNullOrEmpty(_flagDenNgay))
                                {
                                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                        _tvNgayGuiDenDenNgay.Text = _flagDenNgay;
                                    else
                                    {
                                        DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                                        _tvNgayGuiDenDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Event
                        _tvTrangThaiTatCa.Click += (sender, e) =>
                        {
                            _flagTrangThai = "1";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                        };
                        _tvTrangThaiChuaXuLy.Click += (sender, e) =>
                        {
                            _flagTrangThai = "2";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                        };
                        _tvTrangThaiDaXuLy.Click += (sender, e) =>
                        {
                            _flagTrangThai = "3";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                        };

                        _tvHanXuLyTatCa.Click += (sender, e) =>
                        {
                            _flagHanXuLy = "1";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                        };
                        _tvHanXuLyQuaHan.Click += (sender, e) =>
                        {
                            _flagHanXuLy = "2";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                        };
                        _tvHanXuLyTrongHan.Click += (sender, e) =>
                        {
                            _flagHanXuLy = "3";
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                        };

                        _lnNgayGuiDenTuNgay.Click += (sender, e) =>
                        {
                            if (_calendarTuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
                            {
                                _imgNgayGuiDenTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                                _imgNgayGuiDenDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                                _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                                _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                                _calendarTuNgay.Visibility = ViewStates.Gone;
                                _calendarDenNgay.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                _imgNgayGuiDenTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
                                _imgNgayGuiDenDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                                _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                                _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

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
                        _lnNgayGuiDenDenNgay.Click += (sender, e) =>
                        {
                            if (_calendarDenNgay.Visibility == ViewStates.Visible)
                            {
                                _imgNgayGuiDenTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                                _imgNgayGuiDenDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                                _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                                _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                                _calendarTuNgay.Visibility = ViewStates.Gone;
                                _calendarDenNgay.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                _imgNgayGuiDenDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                                _imgNgayGuiDenTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                                _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                                _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

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
                        _tvNgayGuiDenTuNgay.TextChanged += (sender, e) =>
                        {
                            if (!_tvNgayGuiDenTuNgay.Text.Equals("Từ ngày") && !_tvNgayGuiDenTuNgay.Text.Equals("Start date"))
                            {
                                DateTime _temp = DateTime.ParseExact(_tvNgayGuiDenTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                                _flagTuNgay = _temp.ToString("dd/MM/yyyy");
                            }
                        };
                        _tvNgayGuiDenDenNgay.TextChanged += (sender, e) =>
                        {
                            if (!_tvNgayGuiDenDenNgay.Text.Equals("Đến ngày") && !_tvNgayGuiDenDenNgay.Text.Equals("End date"))
                            {
                                DateTime _temp = DateTime.ParseExact(_tvNgayGuiDenDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                                _flagDenNgay = _temp.ToString("dd/MM/yyyy");
                            }
                        };

                        _tvMacDinh.Click += (sender, e) =>
                        {
                            // Default Trạng thái
                            _flagTrangThai = CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái");
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTrangThaiChuaXuLy);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTrangThaiDaXuLy);

                            //Default Hạn xử lý
                            _flagHanXuLy = CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý");
                            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvHanXuLyTatCa);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyQuaHan);
                            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvHanXuLyTrongHan);

                            //Default Ngày gửi đến
                            _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                            _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                            _flagTuNgay = CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày");
                            _flagDenNgay = CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày");


                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                _tvNgayGuiDenTuNgay.Text = _flagTuNgay;
                            else
                            {
                                DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                                _tvNgayGuiDenTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                            }

                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                _tvNgayGuiDenDenNgay.Text = _flagDenNgay;
                            else
                            {
                                DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                                _tvNgayGuiDenDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                            }

                        };
                        _tvApDung.Click += (sender, e) =>
                        {
                            #region Validate Data
                            if (_tvNgayGuiDenTuNgay.Text.Contains("/") && _tvNgayGuiDenDenNgay.Text.Contains("/") &&
                                DateTime.ParseExact(_tvNgayGuiDenTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null) >
                                DateTime.ParseExact(_tvNgayGuiDenDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null))
                            {
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"),
                                    CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Start date cannot be greater than end date, please choose again"));

                                _lnNgayGuiDenDenNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                                _lnNgayGuiDenTuNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                                return;
                            }
                            #endregion

                            #region Check xem có phải là Default Filter không
                            if (_flagTrangThai.Equals(CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái")) && _flagHanXuLy.Equals(CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý")) &&
                                _flagTuNgay.Equals(CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày")) && _flagDenNgay.Equals(CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày")))
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
                            CTRLHomePage.LstFilterCondition_VDT = new List<KeyValuePair<string, string>>()
                            {
                            new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                            new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                            new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                            new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                            };
                            SetData();
                            _edtSearch.Text = _edtSearch.Text; // để trigger lại hàm text changed
                            CmmDroidFunction.HideProcessingDialog();
                            #endregion

                            _popupFilter.Dismiss();
                        };

                        _lnBlurBottom.Click += (sender, e) =>
                        {
                            _popupFilter.Dismiss();
                        };
                        _lnBlurTop.Click += (sender, e) =>
                        {
                            _popupFilter.Dismiss();
                        };
                        #endregion

                        _popupFilter.Focusable = true;
                        _popupFilter.OutsideTouchable = false;
                        _popupFilter.ShowAsDropDown(_relaToolbar);
                    }
                    catch (Exception ex)
                    {
                        CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFilter", ex);
#endif
                    }
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            }
        }
        private void Click_imgSearch(object sender, EventArgs e)
        {

        }
        private void Click_ItemRecy(object sender, JObject e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string _query = String.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", e["ID"].ToString());
                    List<BeanWorkflowItem> _lstWFItem = conn.Query<BeanWorkflowItem>(_query);
                    conn.Close();
                    if (_lstWFItem != null && _lstWFItem.Count > 0)
                    {
                        FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_lstWFItem[0], null, this.GetType().Name);
                        _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemRecy", ex);
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
                if (String.IsNullOrEmpty(_edtSearch.Text)) // empty -> tra lai ban dau -> gio61ngf như Set Data
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

                if (_type.ToLowerInvariant().Equals("vdt"))
                {
                    if (_tempLastVisible == _lstJObjectDynamic.Count - 1 && _allowLoadMore == true)
                    {
                        _adapterRecy.SetAllowLoadMore(_allowLoadMore);

                        ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                        List<JObject> _lstMore = _pConTrolDynamic.GetDynamicWorkflowItem(_resourceViewID, null, CmmDroidVariable.M_LoadMoreLimit, _lstJObjectDynamic.Count);

                        Action action = new Action(() =>
                        {
                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmDroidVariable.M_LoadMoreLimit)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstJObjectDynamic.AddRange(_lstMore);
                                _adapterRecy.SetAllowLoadMore(_allowLoadMore);
                                _adapterRecy.NotifyDataSetChanged();
                            }
                            else
                            {
                                _allowLoadMore = false;
                                _adapterRecy.SetAllowLoadMore(_allowLoadMore);
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
                //var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                //List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                //{
                //    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái")),
                //    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý")),
                //    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày")),
                //    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày")),
                //};

                //string _queryTitleCount = CTRLHomePage.GetQueryStringVDT_ByCondition(LstFilterDefault, false, true);
                //var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);
                //conn.Close();

                //if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                //    SetTitleByListCount(_lstTitleCount[0].Count);
                //else
                //    SetTitleByListCount(0);
                SetTitleByListCount(0);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataTitle", ex);
#endif
            }
        }
        private async void SetData()
        {
            try
            {
                await Task.Run(() =>
                {
                    ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                    _lstJObjectDynamic = _pConTrolDynamic.GetDynamicWorkflowItem(_resourceViewID, null, CmmDroidVariable.M_LoadMoreLimit, 0);
                    _mainAct.RunOnUiThread(() =>
                    {
                        Action action = new Action(() =>
                        {
                            if (_lstJObjectDynamic != null && _lstJObjectDynamic.Count > 0)
                            {
                                _recyData.Visibility = ViewStates.Visible;
                                _lnNoData.Visibility = ViewStates.Gone;

                                _recyData.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);

                                if (_lstJObjectDynamic.Count < CmmDroidVariable.M_LoadMoreLimit)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _adapterRecy = new AdapterFragmentListStable(_rootView.Context, _lstJObjectDynamic, _mainAct);
                                InitRecyclerView(_adapterRecy);
                            }
                            else
                            {
                                _recyData.Visibility = ViewStates.Gone;
                                _lnNoData.Visibility = ViewStates.Visible;
                                _allowLoadMore = false;
                            }

                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
                    });
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }
        private void InitRecyclerView(AdapterFragmentListStable _adapterRecy)
        {
            if (_adapterRecy != null)
            {
                _adapterRecy.SetAllowLoadMore(_allowLoadMore);
                _adapterRecy.CustomItemClick -= Click_ItemRecy;
                _adapterRecy.CustomItemClick += Click_ItemRecy;
                _recyData.SetAdapter(_adapterRecy);
                _recyData.SetLayoutManager(new CustomSpeedLinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
            }
        }
        public void RenewData(object sender, EventArgs e)
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
        #endregion

    }
}