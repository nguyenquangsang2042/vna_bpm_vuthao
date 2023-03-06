using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using Com.Telerik.Widget.Calendar;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupFilterVTBD : SharedViewBase
    {
        public enum FlagViewFilterVTBD
        {
            [Description("HomePage")]
            HomePage = 0,

            [Description("SingleVTBD")]
            SingleListVTBD = 1,


            [Description("ChildAppHomePage")]
            ChildAppHomePage = 3,

            [Description("ChildAppSingleListVTBD")]
            ChildAppSingleListVTBD = 4,
        }
        public ControllerHomePage CTRLHomePage { get; set; }
        public int _flagview { get; set; }
        private List<BeanAppStatus> _lstTrangThai { get; set; }  // Lưu Flag Trạng thái hiện hành
        private List<BeanLookupData> _lstHanXuLy { get; set; }   // Lưu Flag hạn xử lý hiện hành
        private string _flagTuNgay { get; set; }                 // Lưu Flag ngày gửi đến - từ ngày lý hiện hành
        private string _flagDenNgay { get; set; }                // Lưu Flag ngày gửi đến - đến ngày lý hiện hành

        private View _vwTop;
        // Tình Trạng
        private TextView _tvTinhTrang;
        private TextView _tvTinhTrang_Content;
        private LinearLayout _lnTinhTrang_Content;
        // Trạng Thái
        private TextView _tvTrangThai;
        private TextView _tvTrangThai_Content;
        private LinearLayout _lnTrangThai_Content;
        // Hạn xử lý
        private TextView _tvHanXuLy;
        private TextView _tvHanXuLy_Content;
        private LinearLayout _lnHanXuLy_Content;
        // Ngày gửi đến
        private TextView _tvNgay;

        private LinearLayout _lnNgayDenNgay;
        private TextView _tvNgayDenNgay;
        private TextView _lblNgayDenNgay;

        private LinearLayout _lnNgayTuNgay;
        private TextView _tvNgayTuNgay;
        private TextView _lblNgayTuNgay;
        // Default
        private TextView _tvMacDinh;
        private TextView _tvApDung;
        private LinearLayout _lnBlurTop;

        private int IDStatusAll = -99;

        public SharedView_PopupFilterVTBD(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
        }

        public void InitializeValue(ControllerHomePage CTRLHomePage, int _flagview)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                this.CTRLHomePage = CTRLHomePage;
                this._flagview = _flagview;

                #region Init Flag Data

                // Trạng thái
                _lstTrangThai = conn.Query<BeanAppStatus>(String.Format("SELECT ID, Title, TitleEN FROM BeanAppStatus WHERE ID IN ({0})", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS)));
                if (_lstTrangThai != null && _lstTrangThai.Count > 0)
                {
                    KeyValuePair<string, string> _KeyVarStatus = CTRLHomePage.LstFilterCondition_VTBD.Where(x => x.Key.ToLowerInvariant().Contains("trạng thái")).First();
                    string[] arrayStatus = _KeyVarStatus.Value.Split(",");

                    for (int i = 0; i < _lstTrangThai.Count; i++)
                    {
                        if (arrayStatus.Contains(_lstTrangThai[i].ID.ToString()))
                            _lstTrangThai[i].IsSelected = true;
                    }
                }

                // Hạn hoàn tất
                KeyValuePair<string, string> _KeyVarDueDate = CTRLHomePage.LstFilterCondition_VTBD.Where(x => x.Key.ToLowerInvariant().Contains("hạn xử lý")).First();
                _lstHanXuLy = new List<BeanLookupData>(); // phải add đúng thứ tự
                _lstHanXuLy.Add(new BeanLookupData() { ID = "1", Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") });
                _lstHanXuLy.Add(new BeanLookupData() { ID = "2", Title = CmmFunction.GetTitle("TEXT_TODAY1", "Trong ngày") });
                _lstHanXuLy.Add(new BeanLookupData() { ID = "3", Title = CmmFunction.GetTitle("TEXT_OVERDUE", "Trễ hạn") });

                for (int i = 0; i < _lstHanXuLy.Count; i++)
                {
                    if (_lstHanXuLy[i].ID == _KeyVarDueDate.Value)
                    {
                        _lstHanXuLy[i].IsSelected = true;
                        break; // Vì là single choice nên break luôn
                    }
                }

                // Ngày gửi đến
                _flagTuNgay = CTRLHomePage.LstFilterCondition_VTBD.Where(x => x.Key.ToLowerInvariant().Contains("từ ngày")).First().Value;
                _flagDenNgay = CTRLHomePage.LstFilterCondition_VTBD.Where(x => x.Key.ToLowerInvariant().Contains("đến ngày")).First().Value;
                #endregion
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Show view
                View _popupViewFilter = _inflater.Inflate(Resource.Layout.PopupVer4FilterAppBase, null);
                PopupWindow _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);

                _vwTop = _popupViewFilter.FindViewById<View>(Resource.Id.vw_PopupVer4FilterAppBase_Top);
                // Trạng thái
                _tvTinhTrang = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_TinhTrang);
                _lnTinhTrang_Content = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_TinhTrang_Content);
                _tvTinhTrang_Content = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_TinhTrang_Content);

                // Trạng thái
                _tvTrangThai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_TrangThai);
                _lnTrangThai_Content = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_TrangThai_Content);
                _tvTrangThai_Content = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_TrangThai_Content);

                // Hạn hoàn tất
                _tvHanXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_HanXuLy);
                _lnHanXuLy_Content = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_HanXuLy_Content);
                _tvHanXuLy_Content = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_HanXuLy_Content);

                // Ngày khởi tạo
                _tvNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_Ngay);
                _tvNgayTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_Ngay_TuNgay);
                _tvNgayDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_Ngay_DenNgay);
                _lblNgayTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.lbl_PopupVer4FilterAppBase_Ngay_TuNgay);
                _lblNgayDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.lbl_PopupVer4FilterAppBase_Ngay_DenNgay);

                _lnNgayTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_Ngay_TuNgay);
                _lnNgayDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_Ngay_DenNgay);

                _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_MacDinh);
                _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer4FilterAppBase_Ngay_ApDung);
                _lnBlurTop = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer4FilterAppBase_TopBlur);

                _vwTop.Visibility = ViewStates.Gone;
                _lnTinhTrang_Content.Visibility = ViewStates.Gone;

                switch (_flagview)
                {
                    case (int)FlagViewFilterVTBD.HomePage:
                        {
                            FragmentHomePage _currentFrag = (FragmentHomePage)_fragment;

                            if (_flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate))
                                _flagTuNgay = ControllerHomePage.Hint_StartDate;

                            if (_flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                                _flagDenNgay = ControllerHomePage.Hint_EndDate;

                            _popupFilter.DismissEvent += (sender, e) => // dismiss sẽ trả lại trạng thái ban đầu
                            {
                                _currentFrag.SetLinearFilter_ByFlag(_currentFrag._flagIsFiltering_VTBD);
                            };

                            _lnBlurTop.LayoutParameters.Height = _currentFrag._lnToolbar.Height; // 90dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(_currentFrag._lnToolbar);
                            break;
                        }
                    case (int)FlagViewFilterVTBD.SingleListVTBD:
                        {
                            FragmentSingleListVTBD _currentFrag = (FragmentSingleListVTBD)_fragment;

                            if (_flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate))
                                _flagTuNgay = ControllerHomePage.Hint_StartDate;

                            if (_flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                                _flagDenNgay = ControllerHomePage.Hint_EndDate;

                            _popupFilter.DismissEvent += (sender, e) => // dismiss sẽ trả lại trạng thái ban đầu
                            {
                                _currentFrag.SetLinearFilter_ByFlag(_currentFrag._flagIsFiltering);
                            };

                            _lnBlurTop.LayoutParameters.Height = _currentFrag._relaToolbar.Height; // 45dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(_currentFrag._relaToolbar);
                            break;
                        }
                    case (int)FlagViewFilterVTBD.ChildAppHomePage:
                        {
                            FragmentChildAppHomePage _currentFrag = (FragmentChildAppHomePage)_fragment;

                            if (_flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate))
                                _flagTuNgay = ControllerHomePage.Hint_StartDate;

                            if (_flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                                _flagDenNgay = ControllerHomePage.Hint_EndDate;

                            _popupFilter.DismissEvent += (sender, e) => // dismiss sẽ trả lại trạng thái ban đầu
                            {
                                _currentFrag.SetLinearFilter_ByFlag(_currentFrag._flagIsFiltering_VTBD);
                            };

                            _lnBlurTop.LayoutParameters.Height = _currentFrag._lnToolbar.Height; // 90dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(_currentFrag._lnToolbar);
                            break;
                        }
                    case (int)FlagViewFilterVTBD.ChildAppSingleListVTBD:
                        {
                            FragmentChildAppSingleListVTBD _currentFrag = (FragmentChildAppSingleListVTBD)_fragment;

                            if (_flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate))
                                _flagTuNgay = ControllerHomePage.Hint_StartDate;

                            if (_flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                                _flagDenNgay = ControllerHomePage.Hint_EndDate;

                            _popupFilter.DismissEvent += (sender, e) => // dismiss sẽ trả lại trạng thái ban đầu
                            {
                                _currentFrag.SetLinearFilter_ByFlag(_currentFrag._flagIsFiltering);
                            };

                            _lnBlurTop.LayoutParameters.Height = _currentFrag._relaToolbar.Height; // 45dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(_currentFrag._relaToolbar);
                            break;
                        }
                }
                #endregion

                #region Init Data
                // LANGUAGE
                _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATE", "Trạng thái");
                _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                _tvTrangThai_Content.Hint = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn xử lý");
                _tvNgay.Text = CmmFunction.GetTitle("TEXT_DATE_OF_ARRIVAL", "Ngày gửi đến");
                _lblNgayTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                _tvNgayTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                _lblNgayDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
                _tvNgayDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
                _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_RESET_FILTER", "Thiết lập lại");
                _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");

                // FLAG
                foreach (var item in CTRLHomePage.LstFilterCondition_VTBD)
                {
                    if (item.Key.ToLowerInvariant().Equals("trạng thái"))
                    {
                        CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);
                    }
                    else if (item.Key.ToLowerInvariant().Equals("hạn xử lý"))
                    {
                        _tvHanXuLy_Content.Text = _lstHanXuLy.Where(x => x.IsSelected == true).First().Title;
                    }
                    else if (item.Key.ToLowerInvariant().Equals("từ ngày"))
                    {
                        if (!String.IsNullOrEmpty(_flagTuNgay))
                        {
                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                _tvNgayTuNgay.Text = _flagTuNgay;
                            else
                            {
                                DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                                _tvNgayTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                            }
                        }
                        else
                            _tvNgayTuNgay.Text = "";
                    }
                    else if (item.Key.ToLowerInvariant().Equals("đến ngày"))
                    {
                        if (!String.IsNullOrEmpty(_flagDenNgay))
                        {
                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                                _tvNgayDenNgay.Text = _flagDenNgay;
                            else
                            {
                                DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                                _tvNgayDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                            }
                        }
                        else
                            _tvNgayDenNgay.Text = "";
                    }
                }
                #endregion

                #region Event
                _tvTrangThai_Content.TextChanged += TextChanged_tvTrangThaiContent;

                _lnTrangThai_Content.Click += Click_lnTrangThaiContent;

                _lnHanXuLy_Content.Click += Click_lnHanXuLyContent;

                _lnNgayTuNgay.Click += Click_lnNgayTuNgay;

                _lnNgayDenNgay.Click += Click_lnNgayDenNgay;

                _tvNgayTuNgay.TextChanged += TextChanged_tvNgayTuNgay;

                _tvNgayDenNgay.TextChanged += TextChanged_tvNgayDenNgay;

                _tvMacDinh.Click += Click_tvMacDinh;

                #region Confirm - Other

                _lnBlurTop.Click += (sender, e) =>
                {
                    if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                        _popupFilter.Dismiss();
                };

                _tvApDung.Click += (sender, e) =>
                {
                    #region Validate Data
                    if (_flagTuNgay.Contains("/") && _flagDenNgay.Contains("/") &&
                            DateTime.ParseExact(_tvNgayTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null) >
                            DateTime.ParseExact(_tvNgayDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null))
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"),
                                                                   CmmFunction.GetTitle("TEXT_DATE_COMPARE1", "Start date cannot be greater than end date, please choose again"));

                        _tvNgayTuNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clRed)));
                        _tvNgayDenNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clRed)));
                        return;
                    }
                    #endregion

                    switch (_flagview)
                    {
                        case (int)FlagViewFilterVTBD.HomePage:
                            {
                                FragmentHomePage _currentFragment = (FragmentHomePage)_fragment;

                                string _flagTrangThai = String.Join(",", _lstTrangThai.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());
                                string _flagHanXuLy = String.Join(",", _lstHanXuLy.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());

                                #region Check xem có phải là Default Filter không
                                if (_flagTrangThai.Equals(ControllerHomePage.DefaultFilterVTBD.Status) &&
                                _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate) &&
                                _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVTBD.StartDate) &&
                                _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVTBD.EndDate))
                                {
                                    _currentFragment._flagIsFiltering_VTBD = 0;
                                }
                                else // Filter khác trạng thái Default
                                {
                                    _currentFragment._flagIsFiltering_VTBD = 1;
                                }
                                #endregion

                                #region Set giá trị và Filter
                                _currentFragment.SetLinearFilter_ByFlag(_currentFragment._flagIsFiltering_VTBD);
                                CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                                    new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };

                                _currentFragment.SetView();

                                if (_currentFragment._flagIsFiltering_VTBD == 0) // nếu xài local -> set lại title
                                {
                                    _currentFragment.SetData();
                                    if (_currentFragment._flagCurrentTask == 2) // VTBD
                                    {
                                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _currentFragment._tvVDT, _currentFragment._tvVDT.Text, null, null);
                                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _currentFragment._tvVTBD, _currentFragment._tvVTBD.Text, "(", ")");
                                    }
                                }

                                List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerHomePageSingleList).Name);
                                foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                                {
                                    PagerHomePageSingleList _pager = (PagerHomePageSingleList)temp;
                                    if (_pager._type.ToLowerInvariant().Equals("vtbd"))
                                    {
                                        _pager._isShowDialog = true;
                                        _pager._lstFilterCondition = CTRLHomePage.LstFilterCondition_VTBD;
                                        _pager.SetData();
                                    }
                                }
                                #endregion

                                _currentFragment.Click_tvVTBD(null, null); // focus lại trang VTBD
                                break;
                            }
                        case (int)FlagViewFilterVTBD.SingleListVTBD:
                            {
                                FragmentSingleListVTBD _currentFragment = (FragmentSingleListVTBD)_fragment;

                                string _flagTrangThai = String.Join(",", _lstTrangThai.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());
                                string _flagHanXuLy = String.Join(",", _lstHanXuLy.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());

                                #region Check xem có phải là Default Filter không
                                if (_flagTrangThai.Equals(ControllerHomePage.DefaultFilterVTBD.Status) &&
                                             _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate) &&
                                             _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVTBD.StartDate) &&
                                             _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVTBD.EndDate))
                                {
                                    _currentFragment._flagIsFiltering = 0;
                                }
                                else // Filter khác trạng thái Default
                                {
                                    _currentFragment._flagIsFiltering = 1;
                                }
                                #endregion

                                #region Set giá trị và Filter
                                _currentFragment._isShowDialog = true;
                                _currentFragment.SetLinearFilter_ByFlag(_currentFragment._flagIsFiltering);
                                CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái", _flagTrangThai),
                                    new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };
                                _currentFragment._edtSearch.Text = ""; // để trigger lại hàm text changed
                                #endregion
                                break;
                            }
                        case (int)FlagViewFilterVTBD.ChildAppHomePage:
                            {
                                FragmentChildAppHomePage _currentFragment = (FragmentChildAppHomePage)_fragment;

                                string _flagTrangThai = String.Join(",", _lstTrangThai.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());
                                string _flagHanXuLy = String.Join(",", _lstHanXuLy.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());

                                #region Check xem có phải là Default Filter không
                                if (_flagTrangThai.Equals(ControllerHomePage.DefaultFilterVTBD.Status) &&
                                _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate) &&
                                _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVTBD.StartDate) &&
                                _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVTBD.EndDate))
                                {
                                    _currentFragment._flagIsFiltering_VTBD = 0;
                                }
                                else // Filter khác trạng thái Default
                                {
                                    _currentFragment._flagIsFiltering_VTBD = 1;
                                }
                                #endregion

                                #region Set giá trị và Filter
                                _currentFragment.SetLinearFilter_ByFlag(_currentFragment._flagIsFiltering_VTBD);
                                CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                                    new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };

                                _currentFragment.SetView();

                                if (_currentFragment._flagIsFiltering_VTBD == 0) // nếu xài local -> set lại title
                                {
                                    _currentFragment.SetData();
                                    if (_currentFragment._flagCurrentTask == 2) // VTBD
                                    {
                                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _currentFragment._tvVDT, _currentFragment._tvVDT.Text, null, null);
                                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _currentFragment._tvVTBD, _currentFragment._tvVTBD.Text, "(", ")");
                                    }
                                }

                                List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerChildAppHomePageSingleList).Name);
                                foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                                {
                                    PagerChildAppHomePageSingleList _pager = (PagerChildAppHomePageSingleList)temp;
                                    if (_pager._type.ToLowerInvariant().Equals("vtbd"))
                                    {
                                        _pager._isShowDialog = true;
                                        _pager._lstFilterCondition = CTRLHomePage.LstFilterCondition_VTBD;
                                        _pager.SetData();
                                    }
                                }
                                #endregion

                                _currentFragment.Click_tvVTBD(null, null); // focus lại trang VTBD
                                break;
                            }
                        case (int)FlagViewFilterVTBD.ChildAppSingleListVTBD:
                            {
                                FragmentChildAppSingleListVTBD _currentFragment = (FragmentChildAppSingleListVTBD)_fragment;

                                string _flagTrangThai = String.Join(",", _lstTrangThai.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());
                                string _flagHanXuLy = String.Join(",", _lstHanXuLy.Where(x => x.IsSelected == true).Select(x => x.ID).ToArray());

                                #region Check xem có phải là Default Filter không
                                if (_flagTrangThai.Equals(ControllerHomePage.DefaultFilterVTBD.Status) &&
                                             _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate) &&
                                             _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVTBD.StartDate) &&
                                             _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVTBD.EndDate))
                                {
                                    _currentFragment._flagIsFiltering = 0;
                                }
                                else // Filter khác trạng thái Default
                                {
                                    _currentFragment._flagIsFiltering = 1;
                                }
                                #endregion

                                #region Set giá trị và Filter
                                _currentFragment._isShowDialog = true;
                                _currentFragment.SetLinearFilter_ByFlag(_currentFragment._flagIsFiltering);
                                CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái", _flagTrangThai),
                                    new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };
                                _currentFragment._edtSearch.Text =""; // để trigger lại hàm text changed
                                #endregion
                                break;
                            }
                    }

                    _popupFilter.Dismiss();

                };

                #endregion

                #endregion

                _tvTrangThai_Content.Text = _tvTrangThai_Content.Text;  // trigger Italic
                _tvNgayTuNgay.Text = _tvNgayTuNgay.Text;                // trigger Italic
                _tvNgayDenNgay.Text = _tvNgayDenNgay.Text;              // trigger Italic
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }

        #region Event
        private void TextChanged_tvTrangThaiContent(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvTrangThai_Content.Text))
                    _tvTrangThai_Content.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _tvTrangThai_Content.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_tvTrangThaiContent", ex);
#endif
            }
        }

        private void Click_lnTrangThaiContent(object sender, EventArgs e)
        {
            try
            {
                // Show popup Child
                InitializeView_PopupChild_TrangThai();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnTrangThaiContent", ex);
#endif
            }
        }

        private void Click_lnHanXuLyContent(object sender, EventArgs e)
        {
            try
            {
                // Show popup Child
                InitializeView_PopupChild_HanXuLy();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnHanXuLyContent", ex);
#endif
            }
        }

        private void Click_lnNgayTuNgay(object sender, EventArgs e)
        {
            try
            {
                _tvNgayTuNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                _tvNgayDenNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                InitializeView_PopupChild_Calendar(1);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnNgayTuNgay", ex);
#endif
            }
        }

        private void Click_imgTuNgayDelete(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTuNgay.SetBackgroundResource(Resource.Color.clTransparent);

                _flagTuNgay = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayTuNgay.Text = _flagTuNgay;
                else
                {
                    try
                    {
                        DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                        _tvNgayTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayTuNgay.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgTuNgayDelete", ex);
#endif
            }
        }

        private void Click_imgTuNgayToday(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTuNgay.SetBackgroundResource(Resource.Color.clTransparent);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTuNgay.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                else
                    _tvNgayTuNgay.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                _tvNgayTuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgTuNgayToday", ex);
#endif
            }
        }

        private void Click_lnNgayDenNgay(object sender, EventArgs e)
        {
            try
            {
                _tvNgayTuNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                _tvNgayDenNgay.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                InitializeView_PopupChild_Calendar(2);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnNgayDenNgay", ex);
#endif
            }
        }

        private void Click_imgDenNgayDelete(object sender, EventArgs e)
        {
            try
            {
                _lnNgayDenNgay.SetBackgroundResource(Resource.Color.clTransparent);

                _flagDenNgay = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayDenNgay.Text = _flagDenNgay;
                else
                {
                    try
                    {
                        DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                        _tvNgayDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayDenNgay.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDenNgayDelete", ex);
#endif
            }
        }

        private void Click_imgDenNgayToday(object sender, EventArgs e)
        {
            try
            {
                _lnNgayDenNgay.SetBackgroundResource(Resource.Color.clTransparent);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayDenNgay.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                else
                    _tvNgayDenNgay.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                _tvNgayDenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDenNgayToday", ex);
#endif
            }
        }

        private void CellClick_CalendarTuNgay(object sender, RadCalendarView.CellClickEventArgs e)
        {
            try
            {
                _lnNgayTuNgay.SetBackgroundResource(Resource.Color.clTransparent);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTuNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "dd/MM/yyyy");
                else
                    _tvNgayTuNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "MM/dd/yyyy");

                _tvNgayTuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CellClick_CalendarTuNgay", ex);
#endif
            }
        }

        private void CellClick_CalendarDenNgay(object sender, RadCalendarView.CellClickEventArgs e)
        {
            try
            {
                _lnNgayDenNgay.SetBackgroundResource(Resource.Color.clTransparent);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayDenNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "dd/MM/yyyy");
                else
                    _tvNgayDenNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "MM/dd/yyyy");

                _tvNgayDenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CellClick_CalendarDenNgay", ex);
#endif
            }
        }

        private void TextChanged_tvNgayTuNgay(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvNgayTuNgay.Text))
                    _tvNgayTuNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _tvNgayTuNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);

                if (!_tvNgayTuNgay.Text.Equals("Từ ngày") && !_tvNgayTuNgay.Text.Equals("Start date"))
                {
                    DateTime _temp = DateTime.ParseExact(_tvNgayTuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                    _flagTuNgay = _temp.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception)
            {
                _flagTuNgay = "";
            }
        }

        private void TextChanged_tvNgayDenNgay(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvNgayDenNgay.Text))
                    _tvNgayDenNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _tvNgayDenNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);

                if (!_tvNgayDenNgay.Text.Equals("Đến ngày") && !_tvNgayDenNgay.Text.Equals("End date"))
                {
                    DateTime _temp = DateTime.ParseExact(_tvNgayDenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                    _flagDenNgay = _temp.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception)
            {
                _flagDenNgay = "";
            }
        }

        private void Click_tvMacDinh(object sender, EventArgs e)
        {
            try
             {
                // Default Trạng thái
                string[] arrayStatus = ControllerHomePage.DefaultFilterVTBD.Status.Split(",");
                for (int i = 0; i < _lstTrangThai.Count; i++)
                {
                    if (arrayStatus.Contains(_lstTrangThai[i].ID.ToString()))
                        _lstTrangThai[i].IsSelected = true;
                    else
                        _lstTrangThai[i].IsSelected = false;
                }
                //CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);

                //Default Hạn xử lý
                for (int i = 0; i < _lstHanXuLy.Count; i++)
                {
                    _lstHanXuLy[i].IsSelected = (_lstHanXuLy[i].ID.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate)) ? true : false;

                    if (_lstHanXuLy[i].IsSelected == true) // gán vào TextView
                        _tvHanXuLy_Content.Text = _lstHanXuLy[i].Title;
                }
                //_tvHanXuLy_Content.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                //Default Ngày gửi đến
                //_lnNgayDenNgay.SetBackgroundResource(Resource.Color.clTransparent);
                //_lnNgayTuNgay.SetBackgroundResource(Resource.Color.clTransparent);
                _flagTuNgay = ControllerHomePage.DefaultFilterVTBD.StartDate;
                _flagDenNgay = ControllerHomePage.DefaultFilterVTBD.EndDate;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayTuNgay.Text = _flagTuNgay;
                else
                {
                    try
                    {
                        DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                        _tvNgayTuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayTuNgay.Text = "";
                    }
                }

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayDenNgay.Text = _flagDenNgay;
                else
                {
                    try
                    {
                        DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                        _tvNgayDenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayDenNgay.Text = "";
                    }
                }
                _tvApDung.PerformClick();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvMacDinh", ex);
#endif
            }
        }

        /// <summary>
        /// Popup khi click vào trạng thái
        /// </summary>
        public void InitializeView_PopupChild_TrangThai()
        {
            try
            {
                List<BeanAppStatus> _lstTrangThaiTemp = new List<BeanAppStatus>();
                _lstTrangThaiTemp.Add(new BeanAppStatus() { ID = IDStatusAll, Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả"), TitleEN = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") });
                _lstTrangThaiTemp.AddRange(_lstTrangThai.ToList());

                int currentSelectedCount = _lstTrangThaiTemp.Where(x => x.ID != IDStatusAll && x.IsSelected == true).ToList().Count();
                if (currentSelectedCount == _lstTrangThaiTemp.Count - 1 || currentSelectedCount == 0) // check Item tất cả
                    _lstTrangThaiTemp[0].IsSelected = true;
                else
                    _lstTrangThaiTemp[0].IsSelected = false;
                List<string> AllstatusIsSelect = new List<string>();
                foreach (var item in _lstTrangThaiTemp)
                {
                    if (item.IsSelected)
                    {
                        AllstatusIsSelect.Add(item.ID.ToString());
                    }
                }
                #region Get View - Show View 
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                Dialog _dialogPopup = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                _dialogPopup.RequestWindowFeature(1);
                _dialogPopup.SetCanceledOnTouchOutside(false);
                _dialogPopup.SetCancelable(false);

                Window window = _dialogPopup.Window;
                WindowManagerLayoutParams _params = window.Attributes;
                _params.Width = WindowManagerLayoutParams.MatchParent;
                _params.Height = WindowManagerLayoutParams.MatchParent;
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                window.Attributes = _params;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                _dialogPopup.SetContentView(_viewPopupControl);
                _dialogPopup.Show();

                #endregion

                #region Init Data
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");

                AdapterPopupFilterMultiChoice _adapterPopupFilterMultiChoice = new AdapterPopupFilterMultiChoice((MainActivity)base._mainAct, _rootView.Context, _lstTrangThaiTemp);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterPopupFilterMultiChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);
                #endregion

                #region Event

                _imgClose.Click += (sender, e) =>
                {
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    _lstTrangThai = conn.Query<BeanAppStatus>(String.Format("SELECT ID, Title, TitleEN FROM BeanAppStatus WHERE ID IN ({0})", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS)));
                    conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    _lstTrangThai = conn.Query<BeanAppStatus>(String.Format("SELECT ID, Title, TitleEN FROM BeanAppStatus WHERE ID IN ({0})", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS)));
                    if (_lstTrangThai != null && _lstTrangThai.Count > 0)
                    {

                        for (int i = 0; i < _lstTrangThai.Count; i++)
                        {
                            if (AllstatusIsSelect.Contains(_lstTrangThai[i].ID.ToString()))
                                _lstTrangThai[i].IsSelected = true;
                        }
                    }
                    conn.Close();
                    _dialogPopup.Dismiss();
                };

                _adapterPopupFilterMultiChoice.CustomItemClick += (sender, e) =>
                {
                    try
                    {
                        _recyData.Enabled = false;

                        if (e.ID == IDStatusAll) // luôn là check tất cả
                        {
                            foreach (var item in _lstTrangThaiTemp)
                                item.IsSelected = true; // gán toàn bộ lên list
                        }
                        else // check hoặc uncheck còn lại
                        {
                            for (int i = 0; i < _lstTrangThaiTemp.Count; i++)
                            {
                                if (_lstTrangThaiTemp[i].ID.Equals(e.ID))
                                {
                                    _lstTrangThaiTemp[i].IsSelected = !_lstTrangThaiTemp[i].IsSelected;

                                    #region Handle Item All
                                    int currentSelectedCount = _lstTrangThaiTemp.Where(x => x.ID != IDStatusAll && x.IsSelected == true).ToList().Count();
                                    if (currentSelectedCount >= _lstTrangThaiTemp.Count - 1 || currentSelectedCount == 0) // check Item tất cả
                                    {
                                        _lstTrangThaiTemp[i].IsSelected = true;
                                    }
                                    else // uncheck Item tất cả
                                    {
                                        _lstTrangThaiTemp[0].IsSelected = false;
                                    }
                                    #endregion
                                    break;
                                }
                            }
                        }
                        Action action = new Action(() =>
                        {
                            _adapterPopupFilterMultiChoice.NotifyDataSetChanged();
                            _recyData.Enabled = true;
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                    }
                    catch (Exception ex)
                    {
                        _dialogPopup.Dismiss();
                    }
                };

                _imgDone.Click += (sender, e) =>
                {
                    BeanAppStatus itemAll = _lstTrangThaiTemp.Where(x => x.ID == IDStatusAll).ToList().First();

                    if (itemAll.IsSelected == true) // đang check tất cả -> gán nguyên list và cập nhật lại
                    {
                        foreach (var item in _lstTrangThai)
                            item.IsSelected = true;
                    }
                    else
                    {
                        _lstTrangThai = _lstTrangThaiTemp.Where(x => x.ID != IDStatusAll).ToList();
                    }
                    CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);
                    _dialogPopup.Dismiss();
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

        /// <summary>
        /// Popup khi click vào hạn xử lý
        /// </summary>
        public void InitializeView_PopupChild_HanXuLy()
        {
            try
            {
                #region Get View - Show View  
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);
                _imgDone.Visibility = ViewStates.Invisible;

                Dialog _dialogPopup = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                _dialogPopup.RequestWindowFeature(1);
                _dialogPopup.SetCanceledOnTouchOutside(false);
                _dialogPopup.SetCancelable(false);

                Window window = _dialogPopup.Window;
                WindowManagerLayoutParams _params = window.Attributes;
                _params.Width = WindowManagerLayoutParams.MatchParent;
                _params.Height = WindowManagerLayoutParams.MatchParent;
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                window.Attributes = _params;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                _dialogPopup.SetContentView(_viewPopupControl);
                _dialogPopup.Show();
                #endregion

                #region Init Data
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn xử lý");

                AdapterPopupFilterSingleChoice _adapterPopupFilterSingleChoice = new AdapterPopupFilterSingleChoice(base._mainAct, _rootView.Context, _lstHanXuLy);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterPopupFilterSingleChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);

                #endregion

                #region Event
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopup.Dismiss();
                };

                _adapterPopupFilterSingleChoice.CustomItemClick += (sender, e) =>
                {
                    try
                    {
                        for (int i = 0; i < _lstHanXuLy.Count; i++)
                        {
                            if (_lstHanXuLy[i].ID.Equals(e.ID))
                                _lstHanXuLy[i].IsSelected = true;
                            else
                                _lstHanXuLy[i].IsSelected = false;
                        }
                        _tvHanXuLy_Content.Text = e.Title;
                        _tvHanXuLy_Content.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    }
                    catch (Exception ex) { }
                    _dialogPopup.Dismiss();
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

        /// <summary>
        /// Popup khi click vào Calendar
        /// </summary>
        /// <param name="flagCategory"></param>
        public void InitializeView_PopupChild_Calendar(int _flagCategory)
        {
            try
            {
                #region Get View - Init Data
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DatePicker_Ver2, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Ver2_Close);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Ver2_Delete);
                ImageView _imgToday = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Ver2_Today);
                RadCalendarView _calendar = _viewPopupControl.FindViewById<RadCalendarView>(Resource.Id.Calendar_PopupControl_DatePicker_Ver2);

                CTRLHomePage.InitRadCalendarView(_calendar, null);

                DateTime _initDate;
                try
                {
                    if (_flagCategory == 1) // từ ngày
                        _initDate = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                    else
                        _initDate = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);

                    Calendar calendar = new GregorianCalendar(_initDate.Year, _initDate.Month - 1, _initDate.Day);
                    _calendar.DisplayDate = calendar.TimeInMillis;
                }
                catch (Exception)
                {
                    _initDate = DateTime.Now;
                }

                #endregion

                #region Show View                
                //Dialog _dialogPopup = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_NoTitleBar_FullScreen);
                Dialog _dialogPopup = new Dialog(_rootView.Context);
                _dialogPopup.RequestWindowFeature(1);
                _dialogPopup.SetCanceledOnTouchOutside(false);
                _dialogPopup.SetCancelable(false);
                _dialogPopup.SetContentView(_viewPopupControl);
                _dialogPopup.Show();

                Window window = _dialogPopup.Window;
                WindowManagerLayoutParams _params = window.Attributes;
                _params.Width = _mainAct.Resources.DisplayMetrics.WidthPixels;
                _params.Height = WindowManagerLayoutParams.WrapContent;
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                window.Attributes = _params;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopup.Dismiss();
                };

                _imgToday.Click += (sender, e) =>
                {
                    if (_flagCategory == 1) // từ ngày
                        Click_imgTuNgayToday(sender, e);
                    else
                        Click_imgDenNgayToday(sender, e);
                    _dialogPopup.Dismiss();
                };

                _imgDelete.Click += (sender, e) =>
                {
                    if (_flagCategory == 1) // từ ngày
                        Click_imgTuNgayDelete(sender, e);
                    else
                        Click_imgDenNgayDelete(sender, e);
                    _dialogPopup.Dismiss();
                };

                _calendar.CellClick += (sender, e) =>
                {
                    if (_flagCategory == 1) // từ ngày
                        CellClick_CalendarTuNgay(sender, e);
                    else
                        CellClick_CalendarDenNgay(sender, e);
                    _dialogPopup.Dismiss();
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
        #endregion
    }
}