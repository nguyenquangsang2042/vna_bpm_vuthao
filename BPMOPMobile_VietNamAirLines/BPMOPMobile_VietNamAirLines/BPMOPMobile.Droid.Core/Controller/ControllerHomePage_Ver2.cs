using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerHomePage_Ver2 : ControllerBase
    {
        public enum AdapterCategory
        {
            [Description("Hôm nay - Today")]
            CategoryToday = 0,
            [Description("Hôm qua - Yesterday")]
            CategoryYesterday = 1,
            [Description("Cũ hơn - Older")]
            CategoryOlder = 2,
        }

        public enum FlagStateFilterVDT
        {
            [Description("Đang xử lý")]
            InProcess = 1,
            [Description("Đã xử lý")]
            Processed = 2
        }

        public static string Hint_StartDate = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString("dd/MM/yyyy");
        public static string Hint_EndDate = DateTime.Now.ToString("dd/MM/yyyy");

        public static class DefaultFilterVDT // giá trị filter mặc định của VDT
        {
            [Description("Tình trạng")]
            public static string Condition = "1"; // 1 = đang xử lý, 2 = đã xử lý, "" = tất cả
            [Description("Trạng thái")]
            public static string Status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // Default ID = 4 (đang thực hiện) trong BeanAppstatus, dạng "1,2,3,64,128" , nên lấy theo server
            [Description("Hạn xử lý")]
            public static string DueDate = "1";
            [Description("Từ ngày")]
            public static string StartDate = DateTime.Now.AddYears(-1000).ToString("dd/MM/yyyy");
            [Description("Đến ngày")]
            public static string EndDate = DateTime.Now.AddYears(1000).ToString("dd/MM/yyyy");
        }

        public static class DefaultFilterVTBD_Ver2 // giá trị filter mặc định của VTBD
        {
            [Description("Trạng thái")]
            public static string Status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME); // Default ID = 4 (đang thực hiện) trong BeanAppstatus, dạng "1,2,3,64,128"
            public static string Status_InProcess = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DANGXULY); // Default ID = 4 (đang thực hiện) trong BeanAppstatus, dạng "1,2,3,64,128"
            public static string Status_Processed = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DAXULY); // Default ID = 4 (đang thực hiện) trong BeanAppstatus, dạng "1,2,3,64,128
            public static string DueDate = "1";
            [Description("Từ ngày")]
            public static string StartDate = DateTime.Now.AddYears(-1000).ToString("dd/MM/yyyy");
            [Description("Đến ngày")]
            public static string EndDate = DateTime.Now.AddYears(1000).ToString("dd/MM/yyyy");
        }

        public List<KeyValuePair<string, string>> LstFilterCondition_VDT = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Tình trạng", DefaultFilterVDT.Condition), // 1 = đang xử lý, 2 = đã xử lý, "" = tất cả
            new KeyValuePair<string, string>("Trạng thái", DefaultFilterVDT.Status),    // Default ID = 2,4 (đang thực hiện) trong BeanAppstatus, dạng "1,2,3,64,128"
            new KeyValuePair<string, string>("Hạn xử lý", DefaultFilterVDT.DueDate),    // dd/MM/yyyy
            new KeyValuePair<string, string>("Từ ngày",  DefaultFilterVDT.StartDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Đến ngày", DefaultFilterVDT.EndDate),     // dd/MM/yyyy
        };
        public List<KeyValuePair<string, string>> LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Trạng thái", DefaultFilterVTBD_Ver2.Status),
            new KeyValuePair<string, string>("Hạn xử lý", DefaultFilterVTBD_Ver2.DueDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Từ ngày", DefaultFilterVTBD_Ver2.StartDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Đến ngày", DefaultFilterVTBD_Ver2.EndDate),    // dd/MM/yyyy
        };
        public List<KeyValuePair<string, string>> LstFilterCondition_VTBD_Processed = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Trạng thái", DefaultFilterVTBD_Ver2.Status_Processed),
            new KeyValuePair<string, string>("Hạn xử lý", DefaultFilterVTBD_Ver2.DueDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Từ ngày", DefaultFilterVTBD_Ver2.StartDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Đến ngày", DefaultFilterVTBD_Ver2.EndDate),    // dd/MM/yyyy
        };
        public List<KeyValuePair<string, string>> LstFilterCondition_VTBD_InProcess = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Trạng thái", DefaultFilterVTBD_Ver2.Status_InProcess),
            new KeyValuePair<string, string>("Hạn xử lý", DefaultFilterVTBD_Ver2.DueDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Từ ngày", DefaultFilterVTBD_Ver2.StartDate),   // dd/MM/yyyy
            new KeyValuePair<string, string>("Đến ngày", DefaultFilterVTBD_Ver2.EndDate),    // dd/MM/yyyy
        };

        public string _queryVDT_UpdateRead = @"UPDATE BeanNotify Set Read = ? WHERE SPItemId = ?"; // Cập nhật lại trường Read cho BeanNotify khi click vào
        public string _queryFavorite = @"SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = {0} LIMIT 1 OFFSET 0"; // lấy những quy trình đã có favorite ra
        public string _queryWorkflow_ByID = @"SELECT Title, TitleEN FROM BeanWorkflow WHERE WorkflowID = {0} LIMIT 1 OFFSET 0"; // Lấy title lên để hiển thị list

        public void SetTextview_Selected(Activity _mainAct, TextView _tv)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.SetBackgroundResource(Resource.Drawable.drawable_tabselected);
                    //_tv.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                    _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_Selected - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextview_NotSelected(Activity _mainAct, TextView _tv)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.SetBackgroundResource(Resource.Drawable.drawable_tabnotselected);
                    //_tv.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                    _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextview_Selected_Filter(Activity _mainAct, TextView _tv, bool _changeTextColor = true)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_mainAct));
                    _tv.SetBackgroundResource(Resource.Drawable.textcornerviolet2);
                    if (_changeTextColor == true)
                    {
                        _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clWhite)));
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_Selected_Filter - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextview_NotSelected_Filter(Activity _mainAct, TextView _tv, bool _changeTextColor = true)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.SetBackgroundResource(Resource.Drawable.textcornergray);
                    if (_changeTextColor == true)
                    {
                        _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected_Filter - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextview_Selected_Filter_Ver2(Activity _mainAct, TextView _tv, bool _changeTextColor = true)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_mainAct));
                    _tv.SetBackgroundResource(Resource.Drawable.textcornerviolet2);
                    _tv.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueBackground)));
                    _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                    if (_changeTextColor == true)
                    {
                        _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_Selected_Filter - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextview_NotSelected_Filter_Ver2(Activity _mainAct, TextView _tv, bool _changeTextColor = true)
        {
            try
            {
                if (_tv != null)
                {
                    _tv.SetBackgroundResource(Resource.Drawable.textcornergray);
                    _tv.BackgroundTintList = null;
                    _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                    if (_changeTextColor == true)
                    {
                        _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected_Filter - Error: " + ex.Message);
#endif
            }
        }

        public void SetTextView_FormatMultiUser(Context _context, TextView _tv, string[] _lstUser, int _limitcharacterLength, int _maxline = 1, bool IsHighLightColor = true)
        {
            try
            {
                ISpannable spannable = new SpannableString("");
                string _result = "";

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _result += "Đến: ";
                }
                else
                {
                    _result += "To: ";
                }

                if (_tv != null)
                {
                    for (int i = 0; i < _lstUser.Length; i++)
                    {
                        if ((_result.Length + _lstUser[i].Length) > _limitcharacterLength) // i đã vượt quá
                        {
                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            {
                                _result += ", +" + (_lstUser.Length - i).ToString() + " người khác";
                            }
                            else
                            {
                                _result += ", +" + (_lstUser.Length - i).ToString() + " others";
                            }
                            if (IsHighLightColor && _result.Contains("+")) // Tô màu từ + -> hết
                            {
                                int startPosition = _result.IndexOf('+');
                                spannable = new SpannableString(_result.Trim());
                                ColorStateList Color = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomEnable)) });
                                TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, Color, null);
                                spannable.SetSpan(highlightSpan, startPosition, _result.Length, SpanTypes.ExclusiveExclusive);

                            }
                            break;
                        }
                        else
                        {
                            if (i == 0) _result += _lstUser[i].Trim();
                            else _result += ", " + _lstUser[i].Trim();
                        }
                    }
                }
                if (IsHighLightColor && _result.Contains("+")) // Có tô màu
                {
                    _tv.SetText(spannable, TextView.BufferType.Spannable);
                }
                else
                {
                    _tv.Text = _result;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextView_FormatMultiUser - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Format: Nguyễn Văn A, +n 
        /// </summary>
        public void SetTextView_FormatMultiUser2(Context _context, TextView _tv, string[] _lstUser, bool IsHighLightColor = true, bool showFromText = true)
        {
            try
            {
                ISpannable spannable = new SpannableString("");
                string _result = "";

                if (showFromText == true)
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _result += "Đến: ";
                    }
                    else
                    {
                        _result += "To: ";
                    }
                }

                if (_tv != null)
                {
                    if (_lstUser.Length < 2) // 1 người
                    {
                        _result += _lstUser[0];
                    }
                    else
                    {
                        _result += String.Format("{0}, +{1}", _lstUser[0], _lstUser.Length - 1);
                    }
                }
                if (IsHighLightColor && _result.Contains("+")) // Có tô màu
                {
                    int startPosition = _result.IndexOf('+');
                    spannable = new SpannableString(_result.Trim());
                    ColorStateList Color = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomEnable)) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, Color, null);
                    spannable.SetSpan(highlightSpan, startPosition, _result.Length, SpanTypes.ExclusiveExclusive);
                    _tv.SetText(spannable, TextView.BufferType.Spannable);
                }
                else
                {
                    _tv.Text = _result;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextView_FormatMultiUser - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Set dạng multi user cho textview item Detail workflow
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="_tv"></param>
        /// <param name="_lstUser"></param>
        public void SetTextView_FormatMultiUser_DetailWorkflow(Context _context, TextView _tv, string[] _lstUser, bool _plusMoreFormat = true)
        {
            try
            {
                string _result = CmmFunction.GetTitle("TEXT_TO", "Đến: ");

                if (_plusMoreFormat == true)
                {
                    if (_lstUser.Length < 2) // 1 người
                        _result += _lstUser[0];
                    else
                        _result += String.Format("{0}, +{1}", _lstUser[0], _lstUser.Length - 1);

                    _tv.Text = _result;
                    _tv.Post(() =>
                    {
                        bool isEllipsized = CmmDroidFunction.CheckTextViewIsEllipsized(_tv);
                        // Tô màu phần more user
                        if (isEllipsized == false) // ko có ellip -> có hiện +n -> tô màu lên
                        {
                            int startPosition = _result.IndexOf('+');
                            if (startPosition != -1)
                            {
                                SpannableString spannable = new SpannableString(_result.Trim());
                                ColorStateList Color = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clBottomEnable)) });
                                TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, Color, null);
                                spannable.SetSpan(highlightSpan, startPosition, _result.Length, SpanTypes.ExclusiveExclusive);
                                _tv.SetText(spannable, TextView.BufferType.Spannable);
                            }
                            else
                            {
                                _tv.Text = _result;
                            }
                        }
                    });
                }
                else
                {
                    _result += String.Join(", ", _lstUser);
                    _tv.Text = _result;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextView_FormatMultiUser - Error: " + ex.Message);
#endif
            }
        }

        public string GetDateTimeCondition_CreatedDay(DateTime _time, string _LangCode)
        {
            string _res = "";
            try
            {
                if ((_time.Year == DateTime.Now.Year) && (_time.Month == DateTime.Now.Month) && (_time.Day == DateTime.Now.Day)) // Inday
                {
                    if ((_time.Hour == DateTime.Now.Hour))
                    {
                        if (_time.Minute == DateTime.Now.Minute)
                        {
                            if (_LangCode.Equals("VN"))
                                _res = "Vài giây trước";
                            else
                                _res = "A few seconds ago";
                        }
                        else
                        {
                            if (_LangCode.Equals("VN"))
                                _res = (Math.Abs(_time.Minute - DateTime.Now.Minute)).ToString() + " phút trước";
                            else
                                _res = (Math.Abs(_time.Minute - DateTime.Now.Minute)).ToString() + " minutes ago";
                        }
                    }
                    else
                    {
                        if (_LangCode.Equals("VN"))
                            _res = (Math.Abs(_time.Hour - DateTime.Now.Hour)).ToString() + " giờ trước";
                        else
                            _res = (Math.Abs(_time.Hour - DateTime.Now.Hour)).ToString() + " hours ago";
                    }
                }
                else if ((_time.Year == DateTime.Now.Year) && (_time.Month == DateTime.Now.Month) && (_time.Day == DateTime.Now.AddDays(-1).Day))// Yesterday
                {
                    _res = _time.ToString("HH:mm");
                }
                else // Cũ hơn
                {
                    _res = _time.ToString("dd'/'MM'/'yy");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - GetDateTimeCondition - Error: " + ex.Message);
#endif
            }
            return _res;
        }

        public string GetDateTimeCondition_IssueDate(DateTime _time, string _LangCode)
        {
            string _res = "";
            try
            {
                if (_time.Date == DateTime.Now.Date) // TH1: Hạn hoàn tất = Ngày hiện tại
                {
                    if (_LangCode.Equals("VN"))
                        _res = "Hết hạn hôm nay";
                    else
                        _res = "Expired today";
                }
                else // Other day
                {
                    if (_time.Date > DateTime.Now.Date) // TH2: Hạn hoàn tất > Ngày hiện tại
                    {
                        if (_time.Date <= DateTime.Now.Date.AddDays(7)) // Nếu từ 7 ngày thì sẽ hiển thị format: => Còn "x" ngày
                        {
                            if (_LangCode.Equals("VN"))
                                _res = String.Format("Còn {0} ngày", _time.Date.Subtract(DateTime.Now.Date).Days);
                            else
                                _res = String.Format("{0} days", _time.Date.Subtract(DateTime.Now.Date).Days);
                        }
                        else // Nếu lớn hơn 7 ngày sẽ hiển thị format: => dd / mm / yyyy hh: mm
                        {
                            _res = _time.ToString("dd'/'MM'/'yy HH:mm");
                        }
                    }
                    else if (_time.Date < DateTime.Now.Date) // TH3: Hạn hoàn tất < Ngày hiện tại
                    {
                        if (_time.Date <= DateTime.Now.Date.AddDays(-7)) // Nếu từ 7 ngày thì sẽ hiển thị format: => Còn "x" ngày
                        {
                            if (_LangCode.Equals("VN"))
                                _res = String.Format("Trễ hạn {0} ngày", DateTime.Now.Date.Subtract(_time.Date).Days);
                            else
                                _res = String.Format("Expired {0} days", DateTime.Now.Date.Subtract(_time.Date).Days);
                        }
                        else
                        {
                            _res = _time.ToString("dd'/'MM'/'yy HH:mm");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - GetDateTimeCondition - Error: " + ex.Message);
#endif
            }
            return _res;
        }

        public Color GetColorByActionID(Context _context, int ActionID)
        {
            try
            {
                switch (ActionID)
                {
                    case -1: // hủy
                    case 4: // từ chối
                    case 6: // Từ chối
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusRed));
                    case 10: // Đã phê duyệt
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusGreen));
                    case 1: // Chờ phê duyệt
                    case 2: // Chờ phê duyệt
                    case 3: // Chờ phê duyệt
                    case 5: // Chờ phê duyệt
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusBlue));
                    case 0: // Đang lưu
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusGray));
                    default:
                        return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusBlue));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerHomePage", "GetColorByActionID", ex);
#endif
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusBlue));
        }

        /// <summary>
        /// dành riêng cho Bean Notify
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="AppStatusID"></param>
        /// <returns></returns>
        public Color GetColorByAppStatus(Context _context, int AppStatusID)
        {
            try
            {
                switch (AppStatusID)
                {
                    // TASK
                    case 2: // Chưa thực hiện - Task
                        return new Color(Color.ParseColor("#F4F4F4"));
                    case 4: // Đang thực hiện - Task
                        return new Color(Color.ParseColor("#FFF9C8"));
                    case 16: // Từ chối
                    case 64: // Hủy - Task
                    case 128: // Tạm hoãn - Task
                        return new Color(Color.ParseColor("#FFE1E1"));
                    // NOTIFY
                    case 1: // Soạn thảo
                        return new Color(Color.ParseColor("#F4F4F4"));
                    case 8: // Hoàn tất - Task
                        return new Color(Color.ParseColor("#D9FFDA"));
                    //case 16: // Từ chối
                    case 32: // Thu hồi
                        return new Color(Color.ParseColor("#FFE1E1"));
                    default:
                        return new Color(Color.ParseColor("#F4F4F4"));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerHomePage", "GetColorByActionID", ex);
#endif
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clStatusBlue));
        }

        public Color GetColorByDueDate(Context _context, DateTime DueDate)
        {
            //if (DueDate.Date > DateTime.Now.Date && DueDate.Date < DateTime.Now.AddDays(8).Date) // 1 - 7 ngày tới
            //{
            //    return new Color(ContextCompat.GetColor(_context, Resource.Color.clGreenDueDate));
            //}
            if (DueDate.Date >= DateTime.Now.Date && DueDate.Date < DateTime.Now.AddDays(1).Date) // Today
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clPurpleToday)); // purple
            }
            else if (DueDate.Date < DateTime.Now.Date) // Over
            {
                return new Color(ContextCompat.GetColor(_context, Resource.Color.clRedMain)); // red
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clBlack)); // gray                 
        }

        public string GetDefaultValue_FilterVDT(string _Category)
        {
            switch (_Category)
            {
                case "Tình trạng":
                    { return DefaultFilterVDT.Condition; }
                case "Trạng thái":
                    { return DefaultFilterVDT.Status; }
                case "Hạn xử lý":
                    { return DefaultFilterVDT.DueDate; }
                case "Từ ngày":
                    { return DefaultFilterVDT.StartDate; }
                case "Đến ngày":
                    { return DefaultFilterVDT.EndDate; }
            }
            return "";
        }

        public string GetDefaultValue_FilterVTBD(string _Category)
        {
            switch (_Category)
            {
                case "Trạng thái":
                    { return DefaultFilterVTBD_Ver2.Status; }
                case "Hạn xử lý":
                    { return DefaultFilterVTBD_Ver2.DueDate; }
                case "Từ ngày":
                    { return DefaultFilterVTBD_Ver2.StartDate; }
                case "Đến ngày":
                    { return DefaultFilterVTBD_Ver2.EndDate; }
            }
            return "";
        }

        /// <summary>
        /// Trả ra list filter cho trường hợp VDT - đã xử lý
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetListDefault_FilterVDT_Processed()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Tình trạng", ((int)ControllerHomePage.FlagStateFilterVDT.Processed).ToString()),
                new KeyValuePair<string, string>("Trạng thái", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DAXULY)),
                new KeyValuePair<string, string>("Hạn xử lý", GetDefaultValue_FilterVDT("Hạn xử lý")),
                new KeyValuePair<string, string>("Từ ngày", GetDefaultValue_FilterVDT("Từ ngày")),
                new KeyValuePair<string, string>("Đến ngày", GetDefaultValue_FilterVDT("Đến ngày")),
            };
        }

        public void InitListFilterCondition(string _type)
        {
            if (_type.Equals("VDT"))
            {
                LstFilterCondition_VDT = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Tình trạng",GetDefaultValue_FilterVDT("Tình trạng")),
                    new KeyValuePair<string, string>("Trạng thái",GetDefaultValue_FilterVDT("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", GetDefaultValue_FilterVDT("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", GetDefaultValue_FilterVDT("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", GetDefaultValue_FilterVDT("Đến ngày")),
                };
            }
            else if (_type.Equals("VTBD"))
            {
                LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Trạng thái",GetDefaultValue_FilterVTBD("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", GetDefaultValue_FilterVTBD("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", GetDefaultValue_FilterVTBD("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", GetDefaultValue_FilterVTBD("Đến ngày")),
                };
            }
            else if (_type.Equals("BOTH"))
            {
                LstFilterCondition_VDT = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Tình trạng",GetDefaultValue_FilterVDT("Tình trạng")),
                    new KeyValuePair<string, string>("Trạng thái",GetDefaultValue_FilterVDT("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", GetDefaultValue_FilterVDT("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", GetDefaultValue_FilterVDT("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", GetDefaultValue_FilterVDT("Đến ngày")),
                };
                LstFilterCondition_VTBD = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Trạng thái",GetDefaultValue_FilterVTBD("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", GetDefaultValue_FilterVTBD("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", GetDefaultValue_FilterVTBD("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", GetDefaultValue_FilterVTBD("Đến ngày")),
                };
            }
        }

        /// <summary>
        /// Khi click vào Nháp -> Disable Linear DueDate và ngược lại
        /// </summary>
        /// <param name="_ln"></param>
        public void SetVisibleLinearDueDate(LinearLayout _ln, TextView _tvAll, TextView _tvOverDue, TextView _tvInDue, bool _flag)
        {
            if (_flag == true)
            {
                _ln.Alpha = (float)1;
                _ln.Clickable = true;
                _ln.Enabled = _tvAll.Enabled = _tvOverDue.Enabled = _tvInDue.Enabled = true;
            }
            else
            {
                _ln.Alpha = (float)0.5;
                _ln.Clickable = false;
                _ln.Enabled = _tvAll.Enabled = _tvOverDue.Enabled = _tvInDue.Enabled = false;
            }

        }

        public Dictionary<string, string> GetDictionaryFilter(List<KeyValuePair<string, string>> _lstFilter, bool IsVDT, string _workflowID = "")
        {
            Dictionary<string, string> _dict = new Dictionary<string, string>();
            string _flagTrangThai = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("trạng thái")).First().Value;
            string _flagHanXuLy = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("hạn xử lý")).First().Value;
            string _flagTuNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("từ ngày")).First().Value;
            string _flagDenNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("đến ngày")).First().Value;

            DateTime _dateTuNgay = new DateTime();
            DateTime _dateDenNgay = new DateTime();
            if (!String.IsNullOrEmpty(_flagTuNgay))
            {
                _dateTuNgay = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
            }
            if (!String.IsNullOrEmpty(_flagDenNgay))
            {
                _dateDenNgay = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
            }

            if (IsVDT) // VDT có tình trạng
            {
                string _flagTinhTrang = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("tình trạng")).First().Value;
                _dict = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_ToMe,
                                                             _flagTinhTrang.Equals("1") ? "2" : "4",
                                                             _flagTrangThai,
                                                             int.Parse(_flagHanXuLy),
                                                             _dateTuNgay,
                                                             _dateDenNgay);
            }
            else
            {
                _dict = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_FromMe,
                                                             "",
                                                             _flagTrangThai,
                                                             int.Parse(_flagHanXuLy),
                                                             _dateTuNgay,
                                                             _dateDenNgay);
            }
            return _dict;
        }

        /// <summary>
        /// Check xem listfilter condition phải default ko
        /// </summary>
        /// <param name="_lstFilter"></param>
        /// <returns></returns>
        public bool CheckListFilterIsDefault_VDT(List<KeyValuePair<string, string>> _lstFilter, bool checkInProcess)
        {
            string _flagTinhTrang = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("tình trạng")).First().Value;
            string _flagTrangThai = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("trạng thái")).First().Value;
            string _flagHanXuLy = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("hạn xử lý")).First().Value;
            string _flagTuNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("từ ngày")).First().Value;
            string _flagDenNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("đến ngày")).First().Value;

            if (checkInProcess == true) // đang xử lý
            {
                if (_flagTinhTrang.Equals(ControllerHomePage.DefaultFilterVDT.Condition) &&
                    _flagTrangThai.Equals(ControllerHomePage.DefaultFilterVDT.Status) &&
                    _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVDT.DueDate) &&
                    _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate) &&
                    _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                {
                    return true;
                }
                else // Filter khác trạng thái Default
                {
                    return false;
                }
            }
            else // đã xử lý
            {
                if (_flagTinhTrang.Equals(((int)ControllerHomePage.FlagStateFilterVDT.Processed).ToString()) &&
                    _flagTrangThai.Equals(CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DAXULY)) &&
                    _flagHanXuLy.Equals(ControllerHomePage.DefaultFilterVDT.DueDate) &&
                    _flagTuNgay.Equals(ControllerHomePage.DefaultFilterVDT.StartDate) &&
                    _flagDenNgay.Equals(ControllerHomePage.DefaultFilterVDT.EndDate))
                {
                    return true;
                }
                else // Filter khác trạng thái Default
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Check xem listfilter condition phải default ko
        /// </summary>
        /// <param name="_lstFilter"></param>
        /// <returns></returns>
        public bool CheckListFilterIsDefault_VTBD(List<KeyValuePair<string, string>> _lstFilter)
        {
            string _flagTrangThai = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("trạng thái")).First().Value;
            string _flagHanXuLy = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("hạn xử lý")).First().Value;
            string _flagTuNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("từ ngày")).First().Value;
            string _flagDenNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("đến ngày")).First().Value;

            if (/*_flagTrangThai.Equals(ControllerHomePage.DefaultFilterVTBD.Status) &&*/
                (_flagTrangThai.Equals(ControllerHomePage_Ver2.DefaultFilterVTBD_Ver2.Status_InProcess) || _flagTrangThai.Equals(ControllerHomePage_Ver2.DefaultFilterVTBD_Ver2.Status_Processed)) &&
                 _flagHanXuLy.Equals(ControllerHomePage_Ver2.DefaultFilterVTBD_Ver2.DueDate) &&
                 _flagTuNgay.Equals(ControllerHomePage_Ver2.DefaultFilterVTBD_Ver2.StartDate) &&
                 _flagDenNgay.Equals(ControllerHomePage_Ver2.DefaultFilterVTBD_Ver2.EndDate))
            {
                return true;
            }
            else // Filter khác trạng thái Default
            {
                return false;
            }
        }

        public string GetQueryStringVDT_ByCondition(List<KeyValuePair<string, string>> _lstFilterCondition, bool _WithLimitOffset = true, bool IsCountQuery = false, string _searchString = "")
        {
            string _result = "", trangthai = "", hanxuly = "", tungay = "", denngay = "";

            foreach (var item in _lstFilterCondition)
            {
                if (item.Key.Equals("Trạng thái"))
                {
                    switch (item.Value.ToString())
                    {
                        case "1": // "Tất cả"
                            { trangthai = ""; break; }
                        case "2": // "Chưa xử lý"
                            { trangthai = "Status = 0 AND"; break; }
                        case "3": // "Đã xử lý"
                            { trangthai = "Status = 1 AND"; break; }
                    }
                }
                else if (item.Key.Equals("Hạn xử lý"))
                {
                    switch (item.Value.ToString())
                    {
                        case "1": // "Tất cả"
                            { hanxuly = ""; break; }
                        case "2": // "Quá hạn"
                            { hanxuly = String.Format("DueDate < '{0}' AND", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                        case "3": // "Trong hạn"
                            { hanxuly = String.Format("DueDate >= '{0}' AND", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                    }
                }
                else if (item.Key.Equals("Từ ngày"))
                {
                    try
                    {
                        DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                        tungay = String.Format("Created >= '{0}' AND", _temp.Date/*.AddDays(-1)*/.ToString("yyyy-MM-dd"));
                    }
                    catch (Exception)
                    {
                        tungay = String.Format("Created >= '1000-01-01' AND");
                    }

                }
                else if (item.Key.Equals("Đến ngày"))
                {
                    try
                    {
                        DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                        denngay = String.Format("Created < '{0}'", _temp.Date.AddDays(1).ToString("yyyy-MM-dd"));
                    }
                    catch (Exception)
                    {
                        denngay = String.Format("Created < '3000-01-01'");
                    }

                }
            }

            if (IsCountQuery == false) // Get * 
                _result = @"SELECT * FROM BeanNotify WHERE (Action = 'Task' OR ( Action <> 'Task' AND SubmitActionId <> 0)) And {0} {1} {2} {3} AND StartDate IS NOT NULL ORDER BY Created DESC {4}";
            else // Get Object Count
                _result = @"SELECT Count(*) as count FROM BeanNotify WHERE (Action = 'Task' OR ( Action <> 'Task' AND SubmitActionId <> 0)) And {0} {1} {2} {3} AND StartDate IS NOT NULL ORDER BY Created DESC {4}";

            // Search
            if (String.IsNullOrEmpty(_searchString))
                _result = String.Format(_result, trangthai, hanxuly, tungay, denngay, _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "");
            else // thêm điều kiện search
                _result = String.Format(_result, trangthai, hanxuly, tungay, denngay, _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "").Replace("ORDER BY", String.Format(" And Title LIKE '%{0}%' ORDER BY", _searchString));

            return _result;
        }

        public string GetQueryStringVTBD_ByCondition(List<KeyValuePair<string, string>> _lstFilterCondition, bool _WithLimitOffset = true, bool IsCountQuery = false, string _searchString = "")
        {
            string _result = "", trangthai = "", hanxuly = "", tungay = "", denngay = "";

            foreach (var item in _lstFilterCondition)
            {
                if (item.Key.Equals("Trạng thái"))
                {
                    switch (item.Value.ToString())
                    {
                        case "1": // "Tất cả" (Khác nháp)
                            { trangthai = "ActionStatusID <> 0 AND"; break; } /*"ActionStatusID <> 0 AND"*/
                        case "2": // Chưa kết thúc
                            { trangthai = "ActionStatusID NOT IN (10,0,6,-1) AND"; break; } //ActionStatusID <> 10 AND
                        case "3": // đã duyệt
                            { trangthai = "ActionStatusID = 10 AND"; break; }
                        case "4": // từ chối
                            { trangthai = "ActionStatusID = 6 AND"; break; }
                        case "5": // đã hủy
                            { trangthai = "ActionStatusID = -1 AND"; break; }
                    }
                }
                else if (item.Key.Equals("Hạn xử lý"))
                {
                    switch (item.Value.ToString())
                    {
                        case "1": // "Tất cả"
                            { hanxuly = ""; break; }
                        case "2": // "Quá hạn"
                            { hanxuly = String.Format("DueDate < '{0}' AND", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                        case "3": // "Trong hạn"
                            { hanxuly = String.Format("DueDate >= '{0}' AND", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                    }
                }
                else if (item.Key.Equals("Từ ngày"))
                {
                    try
                    {
                        DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                        tungay = String.Format("Created >= '{0}' AND", _temp.Date/*.AddDays(-1)*/.ToString("yyyy-MM-dd"));
                    }
                    catch (Exception)
                    {
                        tungay = String.Format("Created >= '1000-01-01' AND");
                    }

                }
                else if (item.Key.Equals("Đến ngày"))
                {
                    try
                    {
                        DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                        denngay = String.Format("Created < '{0}'", _temp.Date.AddDays(1).ToString("yyyy-MM-dd"));
                    }
                    catch (Exception)
                    {
                        denngay = String.Format("Created < '3000-01-01'");
                    }
                }
            }

            if (IsCountQuery == false) // Get * 
                _result = @"SELECT * FROM BeanWorkflowItem WHERE {0} {1} {2} {3} AND CreatedBy = '" + CmmVariable.SysConfig.UserId + "' ORDER BY Created DESC {4}";
            else // Get Object Count
                _result = @"SELECT Count(*) as count FROM BeanWorkflowItem WHERE {0} {1} {2} {3} AND CreatedBy = '" + CmmVariable.SysConfig.UserId + "' ORDER BY Created DESC {4}";

            if (String.IsNullOrEmpty(_searchString))
                return String.Format(_result, trangthai, hanxuly, tungay, denngay, _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "");
            else // thêm điều kiện search
                return String.Format(_result, trangthai, hanxuly, tungay, denngay, _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "").Replace("ORDER BY", String.Format(" And Content LIKE '%{0}%' ORDER BY", _searchString));
        }

        public string GetQueryStringAppBaseVDT_ByCondition(List<KeyValuePair<string, string>> _lstFilterCondition, bool _WithLimitOffset = true, bool _IsCountQuery = false, string _searchString = "", int _workflowID = -1, string _orderByColumn = "NOTI.StartDate")
        {
            string _tinhtrang = "", _trangthai = "", _hanxuly = "", _tungay = "", _denngay = "";

            #region Handle KeyValue Condition
            foreach (var item in _lstFilterCondition) // Trạng thái sẽ theo tùy trường hợp nên ko cần filter
            {
                switch (item.Key.ToLowerInvariant())
                {
                    case "tình trạng":
                        {
                            //switch (item.Value.ToString())
                            //{
                            //    case "1": // Đang xử lý
                            //        { _tinhtrang = "AND NOTI.Status = 0"; break; }
                            //    case "2": // Đã xử lý
                            //        { _tinhtrang = "AND NOTI.Status = 1"; break; }
                            //    default: // Tất cả
                            //        { _tinhtrang = ""; break; }
                            //}
                            break;
                        }
                    case "trạng thái":
                        {
                            switch (item.Value.ToString())
                            {
                                case "": // Tất cả:
                                    { _trangthai = ""; break; }
                                default: // "1,2,4,8"
                                    { _trangthai = String.Format("AND AB.StatusGroup IN ({0})", item.Value); break; }
                            }
                            break;
                        }
                    case "hạn xử lý":
                        {
                            switch (item.Value.ToString())
                            {
                                case "1": // "Tất cả"
                                    { _hanxuly = ""; break; }
                                case "2": // "Trong ngày"
                                    { _hanxuly = String.Format("AND AB.DueDate > '{0}' AND AB.DueDate < '{1}'", DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd")); break; }
                                case "3": // "Trễ hạn"
                                    { _hanxuly = String.Format("AND AB.DueDate < '{0}'", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                            }
                            break;
                        }
                    case "từ ngày":
                        {
                            try
                            {
                                DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                                _tungay = String.Format("AND AB.Created >= '{0}'", _temp.Date/*.AddDays(-1)*/.ToString("yyyy-MM-dd"));
                            }
                            catch (Exception)
                            {
                                _tungay = ""; //String.Format("AND AB.Created >= '1000-01-01'");
                            }
                            break;
                        }
                    case "đến ngày":
                        {
                            try
                            {
                                DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                                _denngay = String.Format("AND AB.Created < '{0}'", _temp.Date.AddDays(1).ToString("yyyy-MM-dd"));
                            }
                            catch (Exception)
                            {
                                _denngay = ""; //String.Format("AND AB.Created < '3000-01-01'");
                            }
                            break;
                        }
                }
            }
            #endregion


            KeyValuePair<string, string> _keyVarCondition = _lstFilterCondition.Where(x => x.Key.ToLowerInvariant().ToString().Equals("tình trạng")).ToList().First();

            if (_keyVarCondition.Value.Equals("1")) // Đang xử lý
            {
                _tinhtrang = "AND NOTI.Status = 0";

                return String.Format(@"SELECT {0}
                                     FROM BeanAppBase AB
                                     INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId
                                     WHERE (AB.AssignedTo LIKE '%{1}%' OR AB.NotifiedUsers LIKE '%{1}%') AND NOTI.Type = 1 
                                     {2} {3} {4} {5} {6} {7}
                                     Order By {8} DESC {9}",

                                     _IsCountQuery == true ? "Count(*) as count" : "AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit",
                                     CmmVariable.SysConfig.UserId.ToUpper(),
                                     _tinhtrang, _trangthai, _hanxuly, _tungay, _denngay,
                                     _workflowID == -1 ? "" : String.Format("AND (AB.WorkflowId IS NOT NULL AND AB.WorkflowId = {0})", _workflowID),
                                     _orderByColumn,
                                     _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "");
            }
            else // = "2" Đã xử lý
            {
                _tinhtrang = "";

                return String.Format(@"SELECT {0} FROM BeanAppBase AB
                                       WHERE AB.StatusGroup <> 256 AND AB.NotifiedUsers LIKE '%{1}%'
                                       AND (AB.ResourceCategoryId <> 16 OR AB.CreatedBy <> '%{1}%') 
                                       {2} {3} {4} {5} {6} {7}
                                       ORDER BY {8} DESC {9}",

                                       _IsCountQuery == true ? "Count(*) as count" : "AB.*",
                                       CmmVariable.SysConfig.UserId.ToLower(),
                                       _tinhtrang, _trangthai, _hanxuly, _tungay, _denngay,
                                       _workflowID == -1 ? "" : String.Format("AND (AB.WorkflowId IS NOT NULL AND AB.WorkflowId = {0})", _workflowID),
                                       _orderByColumn,
                                       _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "");
            }

        }
        public string GetQueryStringAppBaseVTBD_ByCondition_Ver2(int case_get_data = 0)
        {
            switch (case_get_data)
            {
                case 1: // inprocess
                    return String.Format(@"SELECT * FROM BeanAppBaseExt WHERE PositionView = '{0}' Order By Created DESC {1}", CmmVariable.KEY_GET_FROMME_INPROCESS, "LIMIT {0} OFFSET {1}");
                case 2: //processed
                    return String.Format(@"SELECT * FROM BeanAppBaseExt WHERE PositionView = '{0}' Order By Created DESC {1}", CmmVariable.KEY_GET_FROMME_PROCESSED, "LIMIT {0} OFFSET {1}");
                default:// inprocess and processed
                    return String.Format(@"SELECT * FROM BeanAppBaseExt WHERE PositionView = '{0}' OR PositionView = '{1}' Order By Created DESC {2}", CmmVariable.KEY_GET_FROMME_INPROCESS, CmmVariable.KEY_GET_FROMME_PROCESSED, "LIMIT {0} OFFSET {1}");

            }

        }

        public string GetQueryStringAppBaseVTBD_ByCondition(List<KeyValuePair<string, string>> _lstFilterCondition, bool _WithLimitOffset = true, bool _IsCountQuery = false, string _searchString = "", int _workflowID = -1)
        {
            string _trangthai = "", _hanxuly = "", _tungay = "", _denngay = "";

            #region Handle KeyValue Condition
            foreach (var item in _lstFilterCondition) // Trạng thái sẽ theo tùy trường hợp nên ko cần filter
            {
                switch (item.Key.ToLowerInvariant())
                {
                    case "trạng thái":
                        {
                            switch (item.Value.ToString())
                            {
                                case "": // Tất cả:
                                    { _trangthai = ""; break; }
                                default: // "1,2,4,8"
                                    { _trangthai = String.Format("AND AB.StatusGroup IN ({0})", item.Value); break; }

                                    //case "1": // "Tất cả" (Khác Soạn thảo)
                                    //    { trangthai = "StatusGroup <> 1 AND"; break; }
                                    //case "2": // Chưa kết thúc - khác: 64 (Hủy) - 32 (Thu hồi) - 16 (Từ chối) - 8 (Hoàn tất) - 1 (Soạn thảo)
                                    //    { trangthai = "StatusGroup NOT IN (64,32,16,8,1) AND"; break; }
                                    //case "3": // đã duyệt - 8 (Hoàn tất)
                                    //    { trangthai = "StatusGroup = 8 AND"; break; }
                                    //case "4": // từ chối - 16 (Từ chối)
                                    //    { trangthai = "StatusGroup = 16 AND"; break; }
                                    //case "5": // đã hủy - 64 (Hủy)
                                    //    { trangthai = "StatusGroup = 64 AND"; break; }
                            }
                            break;
                        }
                    case "hạn xử lý":
                        {
                            switch (item.Value.ToString())
                            {
                                case "1": // "Tất cả"
                                    { _hanxuly = ""; break; }
                                case "2": // "Trong ngày"
                                    { _hanxuly = String.Format("AND AB.DueDate > '{0}' AND AB.DueDate < '{1}'", DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd")); break; }
                                case "3": // "Trễ hạn"
                                    { _hanxuly = String.Format("AND AB.DueDate < '{0}'", DateTime.Now.ToString("yyyy-MM-dd")); break; }
                            }
                            break;
                        }
                    case "từ ngày":
                        {
                            try
                            {
                                DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                                _tungay = String.Format("AND AB.Created >= '{0}'", _temp.Date/*.AddDays(-1)*/.ToString("yyyy-MM-dd"));
                            }
                            catch (Exception)
                            {
                                _tungay = ""; /*String.Format("AND AB.Created >= '1000-01-01' AND")*/
                            }
                            break;
                        }
                    case "đến ngày":
                        {
                            try
                            {
                                DateTime _temp = DateTime.ParseExact(item.Value, "dd/MM/yyyy", null);
                                _denngay = String.Format("AND AB.Created < '{0}'", _temp.Date.AddDays(1).ToString("yyyy-MM-dd"));
                            }
                            catch (Exception)
                            {
                                _denngay = ""; /*String.Format("AND AB.Created < '3000-01-01'");*/
                            }
                            break;
                        }
                }
            }
            #endregion

            #region Handle Query String

            return String.Format(@"SELECT {0}
                                   FROM BeanAppBase AB                                
                                   WHERE AB.CreatedBy LIKE '%{1}%' 
                                   {2} {3} {4} {5} {6}
                                   Order By AB.Created DESC {7}",

                                _IsCountQuery == true ? "Count(*) as count" : "AB.*",
                                CmmVariable.SysConfig.UserId,
                                _trangthai, _hanxuly, _tungay, _denngay,
                                _workflowID == -1 ? "" : String.Format("AND (AB.WorkflowId IS NOT NULL AND AB.WorkflowId = {0})", _workflowID),
                                _WithLimitOffset == true ? "LIMIT ? OFFSET ?" : "");
            #endregion
        }
    }
}