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
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using Com.Telerik.Widget.Calendar;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupFilterBoard : SharedViewBase
    {
        public enum FlagViewFilterBoard
        {
            [Description("ChildAppKanBan")]
            ChildAppKanBan = 1,
        }
        public ControllerBoard CTRLBoard { get; set; }
        public ControllerHomePage CTRLHomePage { get; set; }
        public int _flagview { get; set; }

        private string _flagTuNgay { get; set; }        // Lưu Flag ngày khởi tạo - từ ngày hiện hành
        private string _flagDenNgay { get; set; }       // Lưu Flag ngày khởi tạo - đến ngày hiện hành
        private string _flagTrangThai { get; set; }     // Lưu Flag Trạng thái hiện hành

        // Ngày tạo
        private TextView _tvToday;
        private TextView _tvYesterday;
        private TextView _tv7Days;
        private TextView _tv30Days;

        private TextView _tvNgayTao;

        private RelativeLayout _relaNgayTao_TuNgay;
        private LinearLayout _lnNgayTao_TuNgay;
        private RadCalendarView _calendarTuNgay;
        private ImageView _imgNgayTao_TuNgay_Today;
        private ImageView _imgNgayTao_TuNgay_Delete;
        private ImageView _imgNgayTao_TuNgay;
        private TextView _tvNgayTao_TuNgay;

        private RelativeLayout _relaNgayTao_DenNgay;
        private LinearLayout _lnNgayTao_DenNgay;
        private RadCalendarView _calendarDenNgay;
        private ImageView _imgNgayTao_DenNgay_Today;
        private ImageView _imgNgayTao_DenNgay_Delete;
        private ImageView _imgNgayTao_DenNgay;
        private TextView _tvNgayTao_DenNgay;

        // Tình trạng
        private TextView _tvTinhTrang;
        private TextView _tvTatCa;
        private TextView _tvChoPheDuyet;
        private TextView _tvDaPheDuyet;
        private TextView _tvTuChoi;

        private TextView _tvApDung;
        private TextView _tvMacDinh;
        private LinearLayout _lnTopBlur;
        private LinearLayout _lnBottomBlur;


        public SharedView_PopupFilterBoard(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
        }

        public void InitializeValue(ControllerBoard CTRLBoard, int _flagview)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                this.CTRLHomePage = new ControllerHomePage();
                this.CTRLBoard = CTRLBoard;
                this._flagview = _flagview;
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
                _flagTuNgay = ""; _flagDenNgay = ""; _flagTrangThai = "";
                List<KeyValuePair<string, string>> _lstCurrentValueFilter = CTRLBoard.GetCurrentValue_Filter();

                #region Get View - Init Data
                View _popupViewFilter = _inflater.Inflate(Resource.Layout.PopupVer2FilterBoard, null);
                PopupWindow _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);

                _tvToday = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_Today);
                _tvYesterday = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_Yesterday);
                _tv7Days = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_7Days);
                _tv30Days = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_30Days);

                _relaNgayTao_TuNgay = _popupViewFilter.FindViewById<RelativeLayout>(Resource.Id.rela_PopupVer2FilterBoard_TuNgay);
                _tvNgayTao = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_NgayTao);
                _lnNgayTao_TuNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterBoard_NgayTao_TuNgay);
                _calendarTuNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer2FilterBoard_TuNgay);
                _imgNgayTao_TuNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_NgayTao_TuNgay);
                _imgNgayTao_TuNgay_Today = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_TuNgay_Today);
                _imgNgayTao_TuNgay_Delete = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_TuNgay_Delete);
                _tvNgayTao_TuNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_NgayTao_TuNgay);

                _relaNgayTao_DenNgay = _popupViewFilter.FindViewById<RelativeLayout>(Resource.Id.rela_PopupVer2FilterBoard_NgayTao_DenNgay);
                _lnNgayTao_DenNgay = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterBoard_NgayTao_DenNgay);
                _calendarDenNgay = _popupViewFilter.FindViewById<RadCalendarView>(Resource.Id.calendar_PopupVer2FilterBoard_DenNgay);
                _imgNgayTao_DenNgay = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_NgayTao_DenNgay);
                _imgNgayTao_DenNgay_Today = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_DenNgay_Today);
                _imgNgayTao_DenNgay_Delete = _popupViewFilter.FindViewById<ImageView>(Resource.Id.img_PopupVer2FilterBoard_DenNgay_Delete);
                _tvNgayTao_DenNgay = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_NgayTao_DenNgay);

                _tvTinhTrang = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_TinhTrang);
                _tvTatCa = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_TinhTrang_TatCa);
                _tvChoPheDuyet = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_TinhTrang_ChoPheDuyet);
                _tvDaPheDuyet = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_TinhTrang_DaPheDuyet);
                _tvTuChoi = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_TinhTrang_TuChoi);

                _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_ApDung);
                _tvMacDinh = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupVer2FilterBoard_MacDinh);

                _lnTopBlur = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterBoard_TopBlur);
                _lnBottomBlur = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupVer2FilterBoard_BottomBlur);

                _tvToday.Text = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
                _tv7Days.Text = CmmFunction.GetTitle("TEXT_7DAYS", "7 ngày");
                _tv30Days.Text = CmmFunction.GetTitle("TEXT_30DAYS", "30 ngày");
                _tvYesterday.Text = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                _tvNgayTao.Text = CmmFunction.GetTitle("TEXT_CREATEDDATE", "Ngày khởi tạo");
                _tvTinhTrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Trạng thái");
                _tvTatCa.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                _tvChoPheDuyet.Text = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Đang thực hiện");
                _tvDaPheDuyet.Text = CmmFunction.GetTitle("TEXT_APPROVED", "Hoàn tất");
                _tvTuChoi.Text = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
                _tvMacDinh.Text = CmmFunction.GetTitle("TEXT_RESET_FILTER", "Thiết lập lại");
                _tvApDung.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");

                DateTime _tempTuNgay = DateTime.Now, _tempDenNgay = DateTime.Now;
                foreach (KeyValuePair<string, string> _item in _lstCurrentValueFilter)
                {
                    switch (_item.Key)
                    {
                        case "Trạng thái":
                            {
                                _flagTrangThai = _item.Value;

                                if (_item.Value.Equals("1"))
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTatCa);
                                else if (_item.Value.Equals("2"))
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
                                else if (_item.Value.Equals("3"))
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
                                else if (_item.Value.Equals("4"))
                                    CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTuChoi);
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
                                catch (System.Exception)
                                {
                                    _flagTuNgay = "";
                                    _tempTuNgay = DateTime.Now.AddYears(10);
                                    _tvNgayTao_TuNgay.Text = "";
                                }
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
                                catch (System.Exception)
                                {
                                    _flagDenNgay = "";
                                    _tempDenNgay = DateTime.Now.AddYears(-10);
                                    _tvNgayTao_DenNgay.Text = "";
                                }
                                break;
                            }
                    }
                }
                CTRLBoard.InitTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, _tempTuNgay, _tempDenNgay); // Để init màu Text
                CTRLBoard.InitRadCalendarView(_calendarTuNgay, null);
                CTRLBoard.InitRadCalendarView(_calendarDenNgay, null);
                #endregion

                #region Show View

                switch (_flagview)
                {
                    case (int)FlagViewFilterBoard.ChildAppKanBan:
                        {
                            FragmentChildAppKanban _currentFragment = (FragmentChildAppKanban)_fragment;
                            _currentFragment._imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGreenDueDate)));

                            _popupFilter.DismissEvent += (sender, e) =>
                            {
                                _currentFragment.SetColor_ImgFilter_ByFlag(_currentFragment._flagIsFiltering);
                            };

                            _popupFilter.Focusable = true;
                            _popupFilter.OutsideTouchable = false;
                            _popupFilter.ShowAsDropDown(_currentFragment._relaToolBar);
                            break;
                        }
                }

                #endregion

                #region Event
                _tvToday.Click += Click_tvToday;

                _tvYesterday.Click += Click_tvYesterday;

                _tv7Days.Click += Click_tv7Days;

                _tv30Days.Click += Click_tv30Days;

                _lnNgayTao_TuNgay.Click += Click_lnNgayTaoTuNgay;

                _lnNgayTao_DenNgay.Click += Click_lnNgayTaoDenNgay;

                _calendarTuNgay.CellClick += CellClick_calendarTuNgay;

                _calendarDenNgay.CellClick += CellClick_calendarDenNgay;

                _imgNgayTao_TuNgay_Today.Click += Click_imgTuNgayToday;

                _imgNgayTao_DenNgay_Today.Click += Click_imgDenNgayToday;

                _imgNgayTao_TuNgay_Delete.Click += Click_imgTuNgayDelete;

                _imgNgayTao_DenNgay_Delete.Click += Click_imgDenNgayDelete;

                _tvNgayTao_TuNgay.TextChanged += TextChanged_tvTuNgay;

                _tvNgayTao_DenNgay.TextChanged += TextChanged_tvDenNgay;

                _tvTatCa.Click += Click_tvTatCa;

                _tvChoPheDuyet.Click += Click_tvChoPheDuyet;

                _tvDaPheDuyet.Click += Click_tvDaPheDuyet;

                _tvTuChoi.Click += Click_tvTuChoi;

                _tvMacDinh.Click += Click_tvMacDinh;


                _lnTopBlur.Click += (sender, e) =>
                {
                    if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                        _popupFilter.Dismiss();
                };

                _lnBottomBlur.Click += (sender, e) =>
                {
                    if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                        _popupFilter.Dismiss();
                };

                _tvApDung.Click += (sender, e) =>
                {
                    switch (_flagview)
                    {

                        case (int)FlagViewFilterBoard.ChildAppKanBan:
                            {
                                FragmentChildAppKanban _currentFragment = (FragmentChildAppKanban)_fragment;
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

                                #region Check xem có phải là Default Filter không
                                if (_flagTrangThai.Equals(ControllerBoard.DefaultFilterBoard.Status) &&
                                             _flagTuNgay.Equals(ControllerBoard.DefaultFilterBoard.StartDate) &&
                                             _flagDenNgay.Equals(ControllerBoard.DefaultFilterBoard.EndDate))
                                {
                                    _currentFragment.SetColor_ImgFilter_ByFlag(false);
                                    _currentFragment._flagIsFiltering = false;
                                }
                                else // Filter khác trạng thái Default
                                {
                                    _currentFragment.SetColor_ImgFilter_ByFlag(true);
                                    _currentFragment._flagIsFiltering = true;
                                }
                                #endregion

                                _lstCurrentValueFilter = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Trạng thái", _flagTrangThai),
                                    new KeyValuePair<string, string>("Từ ngày", _flagTuNgay),
                                    new KeyValuePair<string, string>("Đến ngày", _flagDenNgay),
                                };

                                //_currentFragment._isShowDialog = true;
                                CTRLBoard.LstFilterCondition = _lstCurrentValueFilter;
                                _currentFragment._edtSearch.Text = ""; // triggered lại
                                _currentFragment._edtSearch.SetSelection(_currentFragment._edtSearch.Text.Length);
                                _currentFragment.FilterAndSearchData();
                                _popupFilter.Dismiss();
                                break;
                            }
                    }
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


        #region Event

        #region Date
        private void Click_tvToday(object sender, EventArgs e)
        {
            _flagTuNgay = DateTime.Now.ToString("dd/MM/yyyy");
            _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

            _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
            _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);

            _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 1);

            UncheckViewTuNgayDenNgay();
        }

        private void Click_tvYesterday(object sender, EventArgs e)
        {
            _flagTuNgay = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            _flagDenNgay = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");

            _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-1));
            _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-1));

            _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 2);

            UncheckViewTuNgayDenNgay();
        }

        private void Click_tv7Days(object sender, EventArgs e)
        {
            _flagTuNgay = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

            _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddDays(-7));
            _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);

            _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 3);

            UncheckViewTuNgayDenNgay();
        }

        private void Click_tv30Days(object sender, EventArgs e)
        {
            _flagTuNgay = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

            _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddMonths(-1));
            _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);

            _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 4);

            UncheckViewTuNgayDenNgay();
        }

        private void Click_lnNgayTaoTuNgay(object sender, EventArgs e)
        {
            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 0);

            if (_relaNgayTao_TuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
            {
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            }
            else
            {
                _relaNgayTao_TuNgay.Visibility = ViewStates.Visible;
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            }

            //if (_relaNgayTao_TuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
            //{
            //    _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
            //    _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

            //    _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
            //    _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

            //    _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            //    _relaNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_relaNgayTao_TuNgay, duration: CmmDroidVariable.M_ActionDelayTime));
            //    Action action = new Action(() =>
            //    {
            //        _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
            //    });
            //    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            //}
            //else
            //{
            //    _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
            //    _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

            //    _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
            //    _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

            //    _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            //    _relaNgayTao_TuNgay.Visibility = ViewStates.Visible;
            //    _relaNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_relaNgayTao_TuNgay, 1000f, CmmDroidVariable.M_ActionDelayTime));
            //}

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
        }

        private void Click_lnNgayTaoDenNgay(object sender, EventArgs e)
        {
            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 0);

            if (_relaNgayTao_DenNgay.Visibility == ViewStates.Visible)
            {
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            }
            else
            {
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
                _relaNgayTao_DenNgay.Visibility = ViewStates.Visible;
            }

            //if (_relaNgayTao_DenNgay.Visibility == ViewStates.Visible)
            //{
            //    _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
            //    _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

            //    _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
            //    _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

            //    _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
            //    _relaNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_relaNgayTao_DenNgay, duration: CmmDroidVariable.M_ActionDelayTime));
            //    Action action = new Action(() =>
            //    {
            //        _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
            //    });
            //    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            //}
            //else
            //{
            //    _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
            //    _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

            //    _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
            //    _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

            //    _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
            //    _relaNgayTao_DenNgay.Visibility = ViewStates.Visible;
            //    _relaNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_relaNgayTao_DenNgay, 1000f, CmmDroidVariable.M_ActionDelayTime));

            //}

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
        }

        private void CellClick_calendarTuNgay(object sender, RadCalendarView.CellClickEventArgs e)
        {
            try
            {
                _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTao_TuNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "dd/MM/yyyy");
                else
                    _tvNgayTao_TuNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "MM/dd/yyyy");

                _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CellClick_CalendarTuNgay", ex);
#endif
            }
        }

        private void CellClick_calendarDenNgay(object sender, RadCalendarView.CellClickEventArgs e)
        {
            try
            {
                _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTao_DenNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "dd/MM/yyyy");
                else
                    _tvNgayTao_DenNgay.Text = CmmDroidFunction.GetDateTimeFromMillis(e.P0.Date, "MM/dd/yyyy");

                _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CellClick_CalendarDenNgay", ex);
#endif
            }
        }

        private void Click_imgTuNgayToday(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTao_TuNgay.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                else
                    _tvNgayTao_TuNgay.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                _tvNgayTao_TuNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgTuNgayToday", ex);
#endif
            }
        }

        private void Click_imgDenNgayToday(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvNgayTao_DenNgay.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                else
                    _tvNgayTao_DenNgay.Text = DateTime.Now.Date.ToString("MM/dd/yyyy");

                _tvNgayTao_DenNgay.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgDenNgayToday", ex);
#endif
            }
        }

        private void Click_imgTuNgayDelete(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;

                _flagTuNgay = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayTao_TuNgay.Text = _flagTuNgay;
                else
                {
                    try
                    {
                        DateTime _tempTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                        _tvNgayTao_TuNgay.Text = _tempTuNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayTao_TuNgay.Text = "";
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

        private void Click_imgDenNgayDelete(object sender, EventArgs e)
        {
            try
            {
                _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.drawable_popupfilter_notselected);
                _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraytitle)));
                _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;

                _flagDenNgay = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)) // dd/MM/yyyy
                    _tvNgayTao_DenNgay.Text = _flagDenNgay;
                else
                {
                    try
                    {
                        DateTime _tempDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                        _tvNgayTao_DenNgay.Text = _tempDenNgay.ToString("MM/dd/yyyy");
                    }
                    catch (Exception)
                    {
                        _tvNgayTao_DenNgay.Text = "";
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

        private void TextChanged_tvTuNgay(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvNgayTao_TuNgay.Text))
                    _tvNgayTao_TuNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _tvNgayTao_TuNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);

                if (!_tvNgayTao_TuNgay.Text.Equals("Từ ngày") && !_tvNgayTao_TuNgay.Text.Equals("Start date"))
                {
                    DateTime _temp = DateTime.ParseExact(_tvNgayTao_TuNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                    _flagTuNgay = _temp.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception)
            {
                _flagTuNgay = "";
            }
        }

        private void TextChanged_tvDenNgay(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_tvNgayTao_DenNgay.Text))
                    _tvNgayTao_DenNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _tvNgayTao_DenNgay.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);

                if (!_tvNgayTao_DenNgay.Text.Equals("Đến ngày") && !_tvNgayTao_DenNgay.Text.Equals("End date"))
                {
                    DateTime _temp = DateTime.ParseExact(_tvNgayTao_DenNgay.Text, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "dd/MM/yyyy" : "MM/dd/yyyy", null);
                    _flagDenNgay = _temp.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception)
            {
                _flagDenNgay = "";
            }
        }
        #endregion

        #region Status

        private void Click_tvTatCa(object sender, EventArgs e)
        {
            _flagTrangThai = "1";
            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTatCa);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTuChoi);
        }

        private void Click_tvChoPheDuyet(object sender, EventArgs e)
        {
            _flagTrangThai = "2";
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTatCa);
            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTuChoi);
        }

        private void Click_tvDaPheDuyet(object sender, EventArgs e)
        {
            _flagTrangThai = "3";
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTatCa);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTuChoi);
        }

        private void Click_tvTuChoi(object sender, EventArgs e)
        {
            _flagTrangThai = "4";
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTatCa);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTuChoi);
        }

        private void Click_tvMacDinh(object sender, EventArgs e)
        {
            // 30 ngày 
            _flagTuNgay = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            _flagDenNgay = DateTime.Now.ToString("dd/MM/yyyy");

            _tvNgayTao_TuNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now.AddMonths(-1));
            _tvNgayTao_DenNgay.Text = CTRLBoard.GetFormatDateFilter(DateTime.Now);
            CTRLBoard.SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 4);

            // Trạng thái
            _flagTrangThai = "1";
            CTRLHomePage.SetTextview_Selected_Filter_Ver2(_mainAct, _tvTatCa);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvChoPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvDaPheDuyet);
            CTRLHomePage.SetTextview_NotSelected_Filter_Ver2(_mainAct, _tvTuChoi);

            _tvApDung.PerformClick();
        }
        #endregion

        #endregion

        #region Data
        private void UncheckViewTuNgayDenNgay()
        {
            // Disable Từ ngày và đến ngày
            _imgNgayTao_TuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
            _imgNgayTao_DenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
            _lnNgayTao_TuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
            _lnNgayTao_DenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
            _relaNgayTao_TuNgay.Visibility = ViewStates.Gone;
            _relaNgayTao_DenNgay.Visibility = ViewStates.Gone;
        }

        #endregion
    }
}