using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
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
using Com.Telerik.Widget.Calendar;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupFilterVTBD_Backup : SharedViewBase
    {
        public enum FlagViewFilterVTBD
        {
            [Description("HomePage")]
            HomePage = 0,

            [Description("SingleVTBD")]
            SingleListVTBD = 1,
        }
        public ControllerHomePage CTRLHomePage { get; set; }
        public int _flagview { get; set; }
        private List<BeanAppStatus> _lstTrangThai { get; set; }  // Lưu Flag Trạng thái hiện hành
        private List<BeanLookupData> _lstHanXuLy { get; set; }   // Lưu Flag hạn xử lý hiện hành
        private string _flagTuNgay { get; set; }                 // Lưu Flag ngày gửi đến - từ ngày lý hiện hành
        private string _flagDenNgay { get; set; }                // Lưu Flag ngày gửi đến - đến ngày lý hiện hành

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
        private ImageView _imgNgayDenNgay;
        private RelativeLayout _relaCalendarDenNgay;
        private RadCalendarView _calendarDenNgay;
        private ImageView _imgCalendarDenNgay_Delete;
        private ImageView _imgCalendarDenNgay_Today;

        private LinearLayout _lnNgayTuNgay;
        private TextView _tvNgayTuNgay;
        private ImageView _imgNgayTuNgay;
        private RelativeLayout _relaCalendarTuNgay;
        private RadCalendarView _calendarTuNgay;
        private ImageView _imgCalendarTuNgay_Delete;
        private ImageView _imgCalendarTuNgay_Today;
        // Default
        private TextView _tvMacDinh;
        private TextView _tvApDung;
        private LinearLayout _lnBlurTop;

        public SharedView_PopupFilterVTBD_Backup(LayoutInflater _inflater, Activity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                _lstTrangThai = conn.Query<BeanAppStatus>("SELECT ID, Title, TitleEN FROM BeanAppStatus");
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
                View _popupViewFilter = _inflater.Inflate(Resource.Layout.PopupVer3FilterAppBase, null);
                PopupWindow _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);

                // Trạng thái
                _tvTrangThai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_TrangThai);
                _lnTrangThai_Content = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterAppBase_TrangThai_Content);
                _tvTrangThai_Content = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_TrangThai_Content);

                // Hạn hoàn tất
                _tvHanXuLy = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_HanXuLy);
                _lnHanXuLy_Content = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterAppBase_HanXuLy_Content);
                _tvHanXuLy_Content = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_HanXuLy_Content);

                // Ngày khởi tạo
                _tvNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_Ngay);
                _tvNgayTuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_Ngay_TuNgay);
                _tvNgayDenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_Ngay_DenNgay);

                _lnNgayTuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterAppBase_Ngay_TuNgay);
                _calendarTuNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer3FilterAppBase_Ngay_TuNgay);
                _imgNgayTuNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_TuNgay);
                _relaCalendarTuNgay = _popupViewFilter.FindViewById<RelativeLayout>(Resource.Id.rela_PopupVer3FilterAppBase_Ngay_TuNgay);
                _imgCalendarTuNgay_Delete = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_TuNgay_Delete);
                _imgCalendarTuNgay_Today = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_TuNgay_Today);

                _lnNgayDenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterAppBase_Ngay_DenNgay);
                _calendarDenNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer3FilterAppBase_Ngay_DenNgay);
                _imgNgayDenNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_DenNgay);
                _relaCalendarDenNgay = _popupViewFilter.FindViewById<RelativeLayout>(Resource.Id.rela_PopupVer3FilterAppBase_Ngay_DenNgay);
                _imgCalendarDenNgay_Delete = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_DenNgay_Delete);
                _imgCalendarDenNgay_Today = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer3FilterAppBase_Ngay_DenNgay_Today);

                _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_MacDinh);
                _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterAppBase_Ngay_ApDung);
                _lnBlurTop = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterAppBase_TopBlur);

                CTRLHomePage.InitRadCalendarView(_calendarTuNgay, null);
                CTRLHomePage.InitRadCalendarView(_calendarDenNgay, null);

                switch (_flagview)
                {
                    case (int)FlagViewFilterVTBD.HomePage:
                        {
                            _lnBlurTop.LayoutParameters.Height = ((FragmentHomePage)_fragment)._lnToolbar.Height; // 90dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(((FragmentHomePage)_fragment)._lnToolbar);
                            break;
                        }
                    case (int)FlagViewFilterVTBD.SingleListVTBD:
                        {
                            _lnBlurTop.LayoutParameters.Height = ((FragmentSingleListVTBD)_fragment)._relaToolbar.Height; // 45dp
                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(((FragmentSingleListVTBD)_fragment)._relaToolbar);
                            break;
                        }
                }
                #endregion

                #region Init Data
                // LANGUAGE
                _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");
                _tvTrangThai_Content.Hint = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                _tvHanXuLy.Text = CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn xử lý");
                _tvNgay.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Ngày khởi tạo");
                _tvNgayTuNgay.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                _tvNgayTuNgay.Hint = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
                _tvNgayDenNgay.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
                _tvNgayDenNgay.Hint = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
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

                _calendarTuNgay.CellClick += CellClick_CalendarTuNgay;

                _imgCalendarTuNgay_Today.Click += Click_imgTuNgayToday;

                _imgCalendarTuNgay_Delete.Click += Click_imgTuNgayDelete;

                _calendarDenNgay.CellClick += CellClick_CalendarDenNgay;

                _imgCalendarDenNgay_Today.Click += Click_imgDenNgayToday;

                _imgCalendarDenNgay_Delete.Click += Click_imgDenNgayDelete;

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

                        _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                        _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.textcornerred_whitesolid);
                        return;
                    }
                    #endregion

                    switch (_flagview)
                    {
                        case (int)FlagViewFilterVTBD.HomePage:
                            {
                                FragmentHomePage _currentFragment = (FragmentHomePage)_fragment;

                                #region Check xem có phải là Default Filter không
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
                                _currentFragment.SetData();
                                _currentFragment.SetView();
                                _currentFragment.SetViewPager();
                                CmmDroidFunction.HideProcessingDialog();
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
                                _currentFragment.SetLinearFilter_ByFlag(_currentFragment._flagIsFiltering);
                                CTRLHomePage.LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái",_flagTrangThai),
                                    new KeyValuePair<string, string>("Hạn xử lý", _flagHanXuLy),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };
                                _currentFragment.SetData();
                                CmmDroidFunction.HideProcessingDialog();
                                _currentFragment._edtSearch.Text = _currentFragment._edtSearch.Text; // để trigger lại hàm text changed
                                _currentFragment._edtSearch.SetSelection(_currentFragment._edtSearch.Text.Length); // focus vào character cuối cùng
                                #endregion
                                break;
                            }
                    }

                    _popupFilter.Dismiss();

                };

                #endregion

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
                _lnTrangThai_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_selected);
                _lnHanXuLy_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                if (_relaCalendarTuNgay.Visibility == ViewStates.Visible) // Nếu đang mở từ ngày
                {
                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _relaCalendarTuNgay.Visibility = ViewStates.Gone;
                }

                if (_relaCalendarDenNgay.Visibility == ViewStates.Visible) // Nếu đang mở đến ngày
                {
                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _relaCalendarDenNgay.Visibility = ViewStates.Gone;
                }
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
                _lnTrangThai_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _lnHanXuLy_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_selected);

                if (_relaCalendarTuNgay.Visibility == ViewStates.Visible) // Nếu đang mở từ ngày
                {
                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _relaCalendarTuNgay.Visibility = ViewStates.Gone;
                }

                if (_relaCalendarDenNgay.Visibility == ViewStates.Visible) // Nếu đang mở đến ngày
                {
                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _relaCalendarDenNgay.Visibility = ViewStates.Gone;
                }
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
                _lnTrangThai_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _lnHanXuLy_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                if (_relaCalendarTuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
                {
                    _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                    _relaCalendarDenNgay.Visibility = ViewStates.Gone;
                    _relaCalendarTuNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_relaCalendarTuNgay, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _relaCalendarTuNgay.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                    _relaCalendarDenNgay.Visibility = ViewStates.Gone;
                    _relaCalendarTuNgay.Visibility = ViewStates.Visible;
                    _relaCalendarTuNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_relaCalendarTuNgay, 1000f, CmmDroidVariable.M_ActionDelayTime));

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
                _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarTuNgay.Visibility = ViewStates.Gone;

                _flagTuNgay = ControllerHomePage.DefaultFilterVTBD.StartDate;

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
                _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarTuNgay.Visibility = ViewStates.Gone;

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
                _lnTrangThai_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _lnHanXuLy_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                if (_relaCalendarDenNgay.Visibility == ViewStates.Visible)
                {
                    _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                    _relaCalendarTuNgay.Visibility = ViewStates.Gone;
                    _relaCalendarDenNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_relaCalendarDenNgay, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _relaCalendarDenNgay.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                    _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));

                    _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                    _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);

                    _relaCalendarTuNgay.Visibility = ViewStates.Gone;
                    _relaCalendarDenNgay.Visibility = ViewStates.Visible;
                    _relaCalendarDenNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_relaCalendarDenNgay, 1000f, CmmDroidVariable.M_ActionDelayTime));

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
                _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarDenNgay.Visibility = ViewStates.Gone;

                _flagDenNgay = ControllerHomePage.DefaultFilterVTBD.StartDate;

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
                _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarDenNgay.Visibility = ViewStates.Gone;

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
                _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarTuNgay.Visibility = ViewStates.Gone;

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
                _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaCalendarDenNgay.Visibility = ViewStates.Gone;

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
                CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);

                //Default Hạn xử lý
                for (int i = 0; i < _lstHanXuLy.Count; i++)
                {
                    _lstHanXuLy[i].IsSelected = (_lstHanXuLy[i].ID.Equals(ControllerHomePage.DefaultFilterVTBD.DueDate)) ? true : false;

                    if (_lstHanXuLy[i].IsSelected == true) // gán vào TextView
                        _tvHanXuLy_Content.Text = _lstHanXuLy[i].Title;
                }
                _tvHanXuLy_Content.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                //Default Ngày gửi đến
                _lnNgayDenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _lnNgayTuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
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
                #region Get View - Show view
                View _popupViewFilter = _inflater.Inflate(Resource.Layout.PopupVer3FilterChild, null);
                PopupWindow _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                RecyclerView _recyData = _popupViewFilter.FindViewById<RecyclerView>(Resource.Id.recy_PopupVer3FilterChild);
                LinearLayout _lnDelete = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer3FilterChild_Delete);
                TextView _tvDelete = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer3FilterChild_Delete);

                _lnDelete.Visibility = ViewStates.Visible;
                _popupFilter.Focusable = true;
                _popupFilter.OutsideTouchable = true;
                _popupFilter.ShowAsDropDown(_lnTrangThai_Content);

                _tvDelete.Text = CmmFunction.GetTitle("TEXT_DELETE", "Xóa");

                #endregion

                AdapterPopupFilterMultiChoice _adapterPopupFilterMultiChoice = new AdapterPopupFilterMultiChoice((MainActivity)base._mainAct, _rootView.Context, _lstTrangThai);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterPopupFilterMultiChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);

                _adapterPopupFilterMultiChoice.CustomItemClick += (sender, e) =>
                {
                    try
                    {
                        for (int i = 0; i < _lstTrangThai.Count; i++)
                        {
                            if (_lstTrangThai[i].ID.Equals(e.ID))
                            {
                                _lstTrangThai[i].IsSelected = !_lstTrangThai[i].IsSelected;
                                CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);
                                _adapterPopupFilterMultiChoice.NotifyDataSetChanged();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _popupFilter.Dismiss();
                    }

                };

                _lnDelete.Click += (sender, e) =>
                {
                    for (int i = 0; i < _lstTrangThai.Count; i++)
                    {
                        _lstTrangThai[i].IsSelected = false;
                    }
                    _adapterPopupFilterMultiChoice.NotifyDataSetChanged();
                    CTRLHomePage.BindingList_ToTextViewTrangThai(_rootView.Context, _lstTrangThai, _tvTrangThai_Content);
                };

                _popupFilter.DismissEvent += (sender, e) =>
                {
                    _lnTrangThai_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                };

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
                #region Get View - Show view
                View _popupViewFilter = _inflater.Inflate(Resource.Layout.PopupVer3FilterChild, null);
                PopupWindow _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                RecyclerView _recyData = _popupViewFilter.FindViewById<RecyclerView>(Resource.Id.recy_PopupVer3FilterChild);

                _popupFilter.Focusable = true;
                _popupFilter.OutsideTouchable = true;
                _popupFilter.ShowAsDropDown(_lnHanXuLy_Content);

                #endregion

                AdapterPopupFilterSingleChoice _adapterPopupFilterSingleChoice = new AdapterPopupFilterSingleChoice(base._mainAct, _rootView.Context, _lstHanXuLy);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterPopupFilterSingleChoice);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);

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
                    _popupFilter.Dismiss();

                };

                _popupFilter.DismissEvent += (sender, e) =>
                {
                    _lnHanXuLy_Content.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                };

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