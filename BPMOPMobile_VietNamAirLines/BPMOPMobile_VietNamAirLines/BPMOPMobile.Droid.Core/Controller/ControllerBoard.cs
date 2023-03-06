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
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Com.Telerik.Widget.Calendar;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerBoard : ControllerBase
    {
        public readonly int?[] WaitingListID = { 1, 4 };                // AppStatusID Chờ phê duyệt (đang lưu - chờ xử lý - bổ sung thông tin - tham vấn - yêu cầu hiệu chỉnh)
        public readonly int?[] ApprovedListID = { 8 };                  // AppStatusID Phê duyệt
        public readonly int?[] RejectedListID = { 16, 64 };             // AppStatusID Từ chối (từ chối - hủy)
        public const int _ApprovedStepID = -1;
        public const int _RejectedStepID = -2;
        public static class DefaultFilterBoard // giá trị filter mặc định 
        {
            [Description("Trạng thái")]
            public static string Status = "1";
            [Description("Từ ngày")]
            public static string StartDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            [Description("Đến ngày")]
            public static string EndDate = DateTime.Now.ToString("dd/MM/yyyy");
        }

        public List<KeyValuePair<string, string>> LstFilterCondition = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Trạng thái",DefaultFilterBoard.Status),
            new KeyValuePair<string, string>("Từ ngày",  DefaultFilterBoard.StartDate),
            new KeyValuePair<string, string>("Đến ngày", DefaultFilterBoard.EndDate),
        };

        // Query của trang Board Main
        public string _QueryFavorite = @"SELECT * FROM BeanWorkflow WHERE Favorite = 1 AND StatusName = 'Active' 
                                         AND (Title LIKE '%{0}%' OR TitleEN LIKE '%{0}%') ORDER BY WorkflowID ASC LIMIT ? OFFSET ?";
        public string _QueryFavoriteFull = @"SELECT * FROM BeanWorkflow WHERE Favorite = 1 AND StatusName = 'Active' 
                                             ORDER BY WorkflowID"; // không có search

        // Query của trang Detail Board
        public string _QueryStepDefine = "SELECT * FROM BeanWorkflowStepDefine WHERE WorkflowID = {0}";
        public string _QueryStepDefine_Distinct = "SELECT DISTINCT Title,Step FROM BeanWorkflowStepDefine WHERE WorkflowID = {0}";
        public string _QueryWorkflowItem = "SELECT * FROM BeanWorkflowItem WHERE WorkflowID = {0}";

        public string _QueryWorkflowItemByStep = "SELECT * FROM BeanWorkflowItem WHERE WorkflowID = {0} AND Step = {1} AND ActionStatusId NOT IN (10,6) ORDER BY Created DESC"; // ko query phê duyệt và từ chối lên
        public string _QueryWorkflowItemByStep_Exception = "SELECT * FROM BeanWorkflowItem WHERE WorkflowID = {0} AND ActionStatusId = {1} ORDER BY Created DESC";

        // ko query phê duyệt và từ chối lên
        //public string _QueryAppBaseByStep = @"SELECT * FROM BeanAppBase 
        //                                      WHERE WorkflowID = {0} AND Step = {1} AND ResourceCategoryId <> 16 AND StatusGroup NOT IN ({2})
        //                                      ORDER BY Created DESC";

        //public string _QueryAppBaseByStep_Exception = @"SELECT * FROM BeanAppBase 
        //                                                WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup {1}
        //                                                ORDER BY Created DESC";

        /// <summary>
        /// Set Linear Selected Category: Board - List - Report 
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_ln"></param>
        /// <param name="_img"></param>
        /// <param name="_tv"></param>
        public void SetLinear_Selected_DetailBoardCategory(Activity _mainAct, LinearLayout _ln, ImageView _img, TextView _tv)
        {
            try
            {
                if (_ln != null)
                {
                    _ln.SetBackgroundResource(Resource.Drawable.textboardcategoryselected);
                }
                if (_img != null)
                {
                    _img.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable))));
                }
                if (_tv != null)
                {
                    _tv.SetTypeface(_tv.Typeface, TypefaceStyle.Bold);
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Set Linear Un-Selected Category: Board - List - Report 
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_ln"></param>
        /// <param name="_img"></param>
        /// <param name="_tv"></param>
        public void SetLinear_UnSelected_DetailBoardCategory(Activity _mainAct, LinearLayout _ln, ImageView _img, TextView _tv)
        {
            try
            {
                if (_ln != null)
                {
                    _ln.SetBackgroundResource(Resource.Drawable.textboardcategoryunselected);
                }
                if (_img != null)
                {
                    _img.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle))));
                }
                if (_tv != null)
                {
                    _tv.SetTypeface(null, TypefaceStyle.Normal);
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Set Text Style By Priority - Item DetailBoardList
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_tv"></param>
        /// <param name="_Priority"></param>
        public void SetTextView_DetailBoardList_ByPriority(Activity _mainAct, TextView _tv, int _Priority, string _LangCode)
        {
            try
            {
                switch (_Priority)
                {
                    case 1: // Emergency
                        {
                            _tv.SetBackgroundResource(Resource.Drawable.textboardemergency);
                            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                            if (_LangCode.Equals("VN"))
                                _tv.Text = CmmFunction.GetTitle("K_Board_Priority", "Khẩn cấp");
                            else
                                _tv.Text = CmmFunction.GetTitle("K_Board_Priority", "Emergency");
                            break;
                        }
                    case 2: // Important
                        {
                            _tv.SetBackgroundResource(Resource.Drawable.textboardimportant);
                            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                            if (_LangCode.Equals("VN"))
                                _tv.Text = CmmFunction.GetTitle("K_Board_Priority", "Quan trọng");
                            else
                                _tv.Text = CmmFunction.GetTitle("K_Board_Priority", "Important");
                            break;
                        }
                    default:
                        {
                            //_tv.SetBackgroundResource(null);
                            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
                            _tv.Text = "";
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextView_DateFilter(Activity _mainAct, TextView _tvToday, TextView _tvYesterday, TextView _tv7Days, TextView _tv30Days, int _case)
        {
            try
            {
                switch (_case)
                {
                    case 0: // Bỏ chọn
                    default: // Bỏ chọn
                        {
                            _tvToday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tvYesterday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv7Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv30Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

                            _tvToday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tvYesterday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv7Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv30Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            break;
                        }
                    case 1: // Today
                        {
                            _tvToday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                            _tvYesterday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv7Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv30Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

                            _tvToday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                            _tvYesterday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv7Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv30Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            break;
                        }
                    case 2: // Yesterday
                        {
                            _tvToday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tvYesterday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                            _tv7Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv30Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

                            _tvToday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tvYesterday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                            _tv7Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv30Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            break;
                        }
                    case 3: // 7 Days
                        {
                            _tvToday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tvYesterday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv7Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                            _tv30Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));

                            _tvToday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tvYesterday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv7Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                            _tv30Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            break;
                        }
                    case 4: // 30 Days
                        {
                            _tvToday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tvYesterday.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv7Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                            _tv30Days.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));

                            _tvToday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tvYesterday.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv7Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            _tv30Days.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Xử lý event bấm qua lại 2 calendar
        /// </summary>
        /// <param name="_calendarTuNgay"></param>
        public void SetLinearCalendar(Activity _mainAct, RadCalendarView _calendarTuNgay, LinearLayout _lnTuNgay, ImageView _imgTuNgay,
                                      RadCalendarView _calendarDenNgay, LinearLayout _lnDenNgay, ImageView _imgDenNgay, int _case)
        {
            if (_case == 1) // click Tu ngay
            {
                if (_calendarTuNgay.Visibility == ViewStates.Visible) // Đang mở Từ ngày
                {
                    _imgTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
                    _imgDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

                    _lnTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                    _lnDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                    _calendarTuNgay.Visibility = ViewStates.Gone;
                    _calendarDenNgay.Visibility = ViewStates.Gone;
                }
                else
                {
                    _imgTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
                    _imgDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

                    _lnTuNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                    _lnDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                    if (_calendarDenNgay.Visibility == ViewStates.Visible)
                        _calendarDenNgay.Visibility = ViewStates.Gone;

                    _calendarTuNgay.Animation = ControllerAnimation.GetAnimationSwipe_TopToBot(_calendarTuNgay);
                    _calendarTuNgay.Visibility = ViewStates.Visible;
                }
            }
            else // click đến ngày
            {
                if (_calendarDenNgay.Visibility == ViewStates.Visible)
                {
                    _imgTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));
                    _imgDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

                    _lnTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);
                    _lnDenNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                    _calendarTuNgay.Visibility = ViewStates.Gone;
                    _calendarDenNgay.Visibility = ViewStates.Gone;
                }
                else
                {
                    _imgDenNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clViolet)));
                    _imgTuNgay.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGraytitle)));

                    _lnDenNgay.SetBackgroundResource(Resource.Drawable.textcornerviolet_whitesolid);
                    _lnTuNgay.SetBackgroundResource(Resource.Drawable.textcornerstrokegray);

                    if (_calendarTuNgay.Visibility == ViewStates.Visible)
                        _calendarTuNgay.Visibility = ViewStates.Gone;

                    _calendarDenNgay.Animation = ControllerAnimation.GetAnimationSwipe_TopToBot(_calendarDenNgay);
                    _calendarDenNgay.Visibility = ViewStates.Visible;

                }
            }
        }

        public void InitTextView_DateFilter(Activity _mainAct, TextView _tvToday, TextView _tvYesterday, TextView _tv7Days, TextView _tv30Days, DateTime _tempTuNgay, DateTime _tempDenNgay)
        {
            try
            {
                if (_tempTuNgay.Date == DateTime.Now.Date && _tempDenNgay.Date == DateTime.Now.Date)
                {
                    SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 1);  // Today
                }
                else if (_tempTuNgay.Date == DateTime.Now.Date.AddDays(-1) && _tempDenNgay.Date == DateTime.Now.Date.AddDays(-1))
                {
                    SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 2);  // Yesterday
                }
                else if (_tempTuNgay.Date == DateTime.Now.Date.AddDays(-7) && _tempDenNgay.Date == DateTime.Now.Date)
                {
                    SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 3);  // 7 Days
                }
                else if (_tempTuNgay.Date == DateTime.Now.Date.AddMonths(-1) && _tempDenNgay.Date == DateTime.Now.Date)
                {
                    SetTextView_DateFilter(_mainAct, _tvToday, _tvYesterday, _tv7Days, _tv30Days, 4);  // 1 months
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        public List<KeyValuePair<string, string>> GetCurrentValue_Filter()
        {
            return LstFilterCondition;
        }

        public List<BeanWorkflowItem> FilterListByCondition(List<BeanWorkflowItem> _lstData, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            foreach (var item in _lstFilterCondition)
            {
                if (item.Key.Equals("Trạng thái"))
                {
                    switch (item.Value.ToString())
                    {
                        ////case "1": // "Tất cả" (Khác nháp)
                        ////    { break; }
                        ////case "2": // Chờ phê duyệt
                        ////    { _lstData = _lstData.Where(x => WaitingListID.Contains(x.ActionStatusID)).ToList(); break; }
                        ////case "3": // đã duyệt
                        ////    { _lstData = _lstData.Where(x => ApprovedListID.Contains(x.ActionStatusID)).ToList(); break; }
                        ////case "4": // từ chối
                        ////    { _lstData = _lstData.Where(x => RejectedListID.Contains(x.ActionStatusID)).ToList(); break; }
                    }
                }
                else if (item.Key.Equals("Từ ngày"))
                {
                    switch (item.Value.ToString())
                    {
                        case "": { break; } // "Tất cả"
                        default: { _lstData = _lstData.Where(x => DateTime.ParseExact(item.Value, "dd/MM/yyyy", null) <= x.Created).ToList(); break; }
                    }
                }
                else if (item.Key.Equals("Đến ngày"))
                {
                    switch (item.Value.ToString())
                    {
                        case "": { break; } // "Tất cả"
                        default: { _lstData = _lstData.Where(x => x.Created <= DateTime.ParseExact(item.Value, "dd/MM/yyyy", null).AddDays(1)).ToList(); break; }
                    }
                }
            }
            return _lstData;
        }

        /// <summary>
        /// Filter List và Search Content cho List<AppBase>
        /// </summary>
        /// <param name="_lstData"></param>
        /// <param name="_lstFilterCondition"></param>
        /// <param name="_searchContent"></param>
        /// <returns></returns>
        public List<BeanAppBaseExt> FilterListAppbaseByCondition(List<BeanAppBaseExt> _lstData, List<KeyValuePair<string, string>> _lstFilterCondition, string _searchContent = "")
        {
            if (_lstData != null && _lstData.Count > 0)
            {
                switch (_lstFilterCondition.Where(x => x.Key.ToLowerInvariant().Equals("trạng thái")).FirstOrDefault().Value)
                {
                    /* default:
                     case "1": // "Tất cả" 
                         { break; }
                     case "2": // Chờ phê duyệt 
                         { _lstData = _lstData.Where(x => WaitingListID.Contains(x.StatusGroup)).ToList(); break; }
                     case "3": // đã duyệt
                         { _lstData = _lstData.Where(x => ApprovedListID.Contains(x.StatusGroup)).ToList(); break; }
                     case "4": // từ chối
                         { _lstData = _lstData.Where(x => RejectedListID.Contains(x.StatusGroup)).ToList(); break; }*/
                    default:
                    case "1": // "Tất cả" 
                        { break; }
                    case "2": // Chờ phê duyệt 
                        { _lstData = _lstData.Where(x => x.Step!= _RejectedStepID && x.Step!=_ApprovedStepID).ToList(); break; }
                    case "3": // đã duyệt
                        { _lstData = _lstData.Where(x => x.Step== _ApprovedStepID).ToList(); break; }
                    case "4": // từ chối
                        { _lstData = _lstData.Where(x => x.Step==_RejectedStepID).ToList(); break; }
                }
                string _tungay = _lstFilterCondition.Where(x => x.Key.ToLowerInvariant().Equals("từ ngày")).FirstOrDefault().Value;
                string _denngay = _lstFilterCondition.Where(x => x.Key.ToLowerInvariant().Equals("đến ngày")).FirstOrDefault().Value;

                if (!string.IsNullOrEmpty(_tungay))
                    _lstData = _lstData.Where(x => x.Created >= DateTime.ParseExact(_tungay, "dd/MM/yyyy", null)).ToList();

                if (!string.IsNullOrEmpty(_denngay))
                    _lstData = _lstData.Where(x => x.Created <= DateTime.ParseExact(_denngay, "dd/MM/yyyy", null).AddDays(1)).ToList();

                if (!String.IsNullOrEmpty(_searchContent)) // có nội dung mới search
                {
                    _searchContent = CmmFunction.removeSignVietnamese(_searchContent.ToLowerInvariant().Trim());
                    _lstData = _lstData.Where(x => CmmFunction.removeSignVietnamese(x.Content.ToLowerInvariant().Trim()).Contains(_searchContent)).ToList();
                }
            }
            return _lstData;
        }

        #region Board Detail Group

        public void SetViewCurrentPage_Selected(Activity _mainAct, ImageView _img, TextView _tv, View _vw)
        {
            _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
            _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
            _vw.Visibility = ViewStates.Visible;
        }

        public void SetViewCurrentPage_NotSelected(Activity _mainAct, ImageView _img, TextView _tv, View _vw, int _case)
        {
            switch (_case)
            {
                case 1: // Board
                    {
                        _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBoardDetailBlue)));
                        break;
                    }
                case 2: // List
                    {
                        _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBoardDetailGreen)));
                        break;
                    }
                case 3: // Report
                    {
                        _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBoardDetailRed)));
                        break;
                    }
            }
            _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
            _vw.Visibility = ViewStates.Invisible;
        }

        #endregion

    }
}