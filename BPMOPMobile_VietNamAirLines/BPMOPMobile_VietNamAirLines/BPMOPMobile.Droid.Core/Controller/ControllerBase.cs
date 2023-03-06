using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Com.Telerik.Widget.Calendar;
using Com.Telerik.Widget.Calendar.Decorations;
using Com.Telerik.Widget.Calendar.Events;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerBase
    {
        /// <summary>
        /// Hàm này trả về màu để Tô Textview Avatar dựa vào kí tự [0]
        /// </summary>
        /// <param name="_username"></param>
        public Color GetColorByUserName(Context _context, string _username)
        {
            try
            {
                if (!String.IsNullOrEmpty(_username))
                {
                    switch (_username[0].ToString().ToUpperInvariant())
                    {
                        case "A":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterA));
                        case "B":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterB));
                        case "C":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterC));
                        case "D":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterD));
                        case "E":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterE));
                        case "F":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterF));
                        case "G":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterG));
                        case "H":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterH));
                        case "I":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterI));
                        case "J":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterJ));
                        case "K":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterK));
                        case "L":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterL));
                        case "M":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterM));
                        case "N":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterN));
                        case "O":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterO));
                        case "P":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterP));
                        case "Q":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterQ));
                        case "R":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterR));
                        case "S":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterS));
                        case "T":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterT));
                        case "U":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterU));
                        case "V":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterV));
                        case "W":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterW));
                        case "X":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterX));
                        case "Y":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterY));
                        case "Z":
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterZ));
                        default:
                            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterA));
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_Selected - Error: " + ex.Message);
#endif
            }
            return new Color(ContextCompat.GetColor(_context, Resource.Color.clCharacterA));
        }

        /// <summary>
        /// Set Text Like: @nvien1 - Nhân Viên
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_tv"></param>
        /// <param name="_Priority"></param>
        public void SetTextViewPositionFormat(TextView _tv, BeanUser _user)
        {
            try
            {
                if (!String.IsNullOrEmpty(_user.Position))
                {
                    String[] temp = _user.Position.Split(";#");
                    _tv.Text = "@" + _user.AccountName + " - " + temp[1];
                }
                else
                {
                    _tv.Text = "";
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextView_DetailBoardList_Position - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Init các thuộc tính cho Rad Calendarview
        /// </summary>
        public void InitRadCalendarView(RadCalendarView _radCalendarView, TextView _tvToSetResult)
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _radCalendarView.Locale = new Java.Util.Locale("vi");
                else
                    _radCalendarView.Locale = new Java.Util.Locale("en");

                _radCalendarView.SelectionMode = CalendarSelectionMode.Single;
                _radCalendarView.DisplayMode = CalendarDisplayMode.Month;
                _radCalendarView.EventsDisplayMode = EventsDisplayMode.Inline;
                _radCalendarView.HorizontalScroll = true;
                _radCalendarView.GridLinesLayer.Color = Color.White;
                _radCalendarView.GestureManager.SetDoubleTapToChangeDisplayMode(false);
                _radCalendarView.GestureManager.SetTapToChangeDisplayMode(false);
                _radCalendarView.GestureManager.SetPinchCloseToChangeDisplayMode(false);
                _radCalendarView.GestureManager.SetPinchOpenToChangeDisplayMode(false);

               CalendarAdapter adapter = _radCalendarView.Adapter;
                CalendarDayCellFilter weekendCellFilter = new CalendarDayCellFilter();
                weekendCellFilter.IsWeekend = new Java.Lang.Boolean(true);
                CalendarDayCellStyle weekendCellStyle = new CalendarDayCellStyle();
                weekendCellStyle.Filter = weekendCellFilter;
                //weekendCellStyle.TextColor = new Java.Lang.Integer(Color.Red.ToArgb());

                adapter.SetDateCellBackgroundColor(Color.Transparent, Color.Transparent);
                adapter.TodayCellBackgroundColor = Color.Transparent;
                adapter.SelectedCellBackgroundColor = Color.Transparent;
                adapter.TodayCellSelectedTextColor = Color.ParseColor("#005FD4");
                adapter.DayNameBackgroundColor = Color.Transparent;
                adapter.TodayCellBorderColor = Color.Transparent;
                adapter.SelectedCellTextColor = Color.ParseColor("#005FD4");
                adapter.TodayCellTextColor = Color.ParseColor("#005FD4");


                adapter.DateTextPosition = CalendarElement.Center;
                adapter.DayNameTextPosition = CalendarElement.Center;
                adapter.TitleTextColor = Color.ParseColor("#005FD4");
                adapter.TitleTextSize = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 17, Resources.System.DisplayMetrics); 
                //adapter.TitleTextPosition = /*CalendarElement.AlignLeft |*/ CalendarElement.CenterVertical;
                adapter.TodayTextColor = Color.ParseColor("#005FD4");
                adapter.TodayCellTextColor = Color.ParseColor("#005FD4");
                _radCalendarView.Title().SetPadding(20, 0, 0, 0);
                _radCalendarView.AddDayCellStyle(weekendCellStyle);



                CalendarDayCellFilter todayCellFilter = new CalendarDayCellFilter();
                todayCellFilter.IsToday = new Java.Lang.Boolean(true);
                CalendarDayCellStyle todayCellStyle = new CalendarDayCellStyle();
                todayCellStyle.Filter = todayCellFilter;
                todayCellStyle.BackgroundColor = new Java.Lang.Integer(new Color(ContextCompat.GetColor(Application.Context, Resource.Color.clWhite)).ToArgb());
                todayCellStyle.BorderColor = new Java.Lang.Integer(new Color(ContextCompat.GetColor(Application.Context, Resource.Color.clWhite)).ToArgb());
                float widthInDp = 1;
                float widthInPixels = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, widthInDp, Resources.System.DisplayMetrics);
                todayCellStyle.BorderWidth = new Java.Lang.Float(widthInPixels);
                _radCalendarView.AddDayCellStyle(todayCellStyle);

                SquareCellDecorator decorator = new SquareCellDecorator(_radCalendarView);
                decorator.Stroked = false;
                decorator.Color = Color.ParseColor("#CDE3FF");
                decorator.Scale = .7F;
                _radCalendarView.CellDecorator = decorator;

                //custom item clicked
                _radCalendarView.CellDecorationsLayer.Color = Resource.Color.clVer2BlueMain;
                _radCalendarView.CellDecorationsLayer.StrokeWidth = 5; // pixel

                if (_tvToSetResult != null) // set event khi click ngày sẽ binding dữ liệu vào textview 
                    _radCalendarView.OnSelectedDatesChangedListener = new RadCalendar_SelectedDatesChangedListener(_tvToSetResult);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitRadCalendarView", ex);
            }
        }


        /// <summary>
        /// return theo thứ tự Section index -  Row Index - element Index. Nếu return ra -1 là ko có
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="_lstRows"></param>
        /// <returns></returns>
        public static Tuple<int, int, int> Find_SectionIndex_RowIndex_ElementIndex_ListControl(ViewElement _element, List<ViewSection> _lstSections)
        {
            Tuple<int, int, int> _result = new Tuple<int, int, int>(-1, -1, -1);
            try
            {
                for (int i = 0; i < _lstSections.Count; i++)
                {
                    Tuple<int, int> _index = CmmDroidFunction.Find_RowIndex_ElementIndex_ListControl(_element, _lstSections[i].ViewRows);
                    if (_index.Item1 != -1 && _index.Item2 != -1)
                    {
                        _result = new Tuple<int, int, int>(i, _index.Item1, _index.Item2);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - Find_SectionIndex_RowIndex_ElementIndex_ListControl - Error: " + ex.Message);
#endif
            }
            return _result;
        }

        /// <summary>
        /// return theo thứ tự Section index -  Row Index - element Index. Nếu return ra -1 -1 -1 là ko có
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="_lstRows"></param>
        /// <returns></returns>
        public Tuple<int, int, int> FindElementIndexInListSection(ViewElement _element, List<ViewSection> _lstSections)
        {
            Tuple<int, int, int> _result = new Tuple<int, int, int>(-1, -1, -1);
            bool _flagisFound = false; // flag để dừng For lại
            try
            {
                if (_lstSections != null && _lstSections.Count > 0)
                {
                    for (int i = 0; i < _lstSections.Count; i++) // Check position Section
                    {
                        #region Tìm Row Position
                        if (_lstSections[i].ViewRows != null && _lstSections[i].ViewRows.Count > 0)
                        {
                            for (int j = 0; j < _lstSections[i].ViewRows.Count; j++) // Check position List Rows
                            {
                                #region Tìm Element Position
                                if (_lstSections[i].ViewRows[j].Elements != null && _lstSections[i].ViewRows[j].Elements.Count > 0)
                                {
                                    for (int k = 0; k < _lstSections[i].ViewRows[j].Elements.Count; k++)  // Check position List Elememts
                                    {
                                        if (_lstSections[i].ViewRows[j].Elements[k].ID.Equals(_element.ID))
                                        {
                                            _result = new Tuple<int, int, int>(i, j, k);
                                            _flagisFound = true; // đã tìm ra -> dừng
                                            break;
                                        }
                                    }
                                }
                                #endregion
                                if (_flagisFound) break;
                            }
                        }
                        #endregion
                        if (_flagisFound) break;
                    }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - Find_SectionIndex_RowIndex_ElementIndex_ListControl - Error: " + ex.Message);
#endif
            }
            return _result;
        }

        /// <summary>
        /// Hoàng Đăng Khoa -> HD / Hoàng -> HO
        /// </summary>
        /// <param name="_username"></param>
        public string GetDisplayTextByUserName(string _username)
        {
            string _res = "";
            try
            {
                string[] data = _username.Split(' ');
                if (data.Length > 2)
                {
                    _res = (data[0].Substring(0, 1) + data[1].Substring(0, 1)).ToUpperInvariant();
                }
                else
                {
                    if (_username.Length > 2)
                        _res = _username.Substring(0, 2).ToUpperInvariant();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDisplayTextByUserName", ex);
#endif
            }
            return _res;
        }

        /// <summary>
        /// Định dạng ngày của app
        /// </summary>
        /// <param name="_date"></param>
        /// <returns></returns>
        public string GetFormatDateLang(DateTime _date)
        {
            string _res = "";
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _res = _date.ToString("dd/MM/yy HH:mm");
                else
                    _res = _date.ToString("MM/dd/yy HH:mm");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDisplayTextByUserName", ex);
#endif
            }
            return _res;
        }

        /// <summary>
        /// Định dạng ngày của filter
        /// </summary>
        /// <param name="_date"></param>
        /// <returns></returns>
        public string GetFormatDateFilter(DateTime _date)
        {
            string _res = "";
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    return _date.ToString("dd/MM/yyyy");
                else
                    return _date.ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDisplayTextByUserName", ex);
#endif
            }
            return _res;
        }

        /// <summary>
        /// Format: To me (5)
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_tv"></param>
        public void SetTextview_FormatItemCount(TextView _tv, int _listCount, string _category, string _categoryVDT = "")
        {
            string _result = "";
            try
            {
                string _numFormat = "";

                if (_listCount > 99)
                {
                    if (String.IsNullOrEmpty(_category)) // Left menu
                        _numFormat = "99+";//string.Format("Đến tôi (99+)");
                    else
                        _numFormat = "(99+)";//string.Format("Đến tôi (99+)");
                }
                else if (_listCount >= 10)
                {
                    if (String.IsNullOrEmpty(_category)) // Left menu
                        _numFormat = _listCount.ToString();
                    else
                        _numFormat = "(" + _listCount + ")";
                }
                else if (_listCount > 0 && _listCount < 10)// 01
                {
                    if (String.IsNullOrEmpty(_category)) // Left menu
                        _numFormat = "0" + _listCount.ToString();
                    else
                        _numFormat = "(0" + _listCount + ")";
                }
                else
                {
                    _numFormat = "";
                }


                switch (_category.ToLowerInvariant())
                {
                    case "vdt": // đến tôi
                        switch (_categoryVDT.ToLowerInvariant())
                        {
                            default:
                                _result = string.Format("{0} {1}", CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)
                                 ? CmmFunction.GetTitle("TEXT_TOME", "Đến tôi")
                                 : CmmFunction.GetTitle("TEXT_TOME", "To me"), _numFormat);
                                break;
                            case "inprocess":
                                _result = string.Format("{0} {1}", CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)
                                 ? CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý")
                                 : CmmFunction.GetTitle("TEXT_INPROCESS", "In process"), _numFormat);
                                break;
                            case "processed":
                                _result = string.Format("{0} {1}", CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)
                                 ? CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý")
                                 : CmmFunction.GetTitle("TEXT_PROCESSED", "Processed"), _numFormat);
                                break;
                        }
                        break;
                    case "vtbd":// tôi bắt đầu
                        _result = string.Format("{0} {1}", CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)
                                                         ? CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu")
                                                         : CmmFunction.GetTitle("TEXT_FROMME", "From me"), _numFormat);
                        break;
                    case "follow":// tôi bắt đầu
                        _result = string.Format("{0} {1}", CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN)
                                                         ? CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi")
                                                         : CmmFunction.GetTitle("TEXT_FOLLOW", "Follow"), _numFormat);
                        break;
                    case "": // Left menu
                    default:
                        _result = _numFormat;
                        break;
                }
                if (_tv != null)
                    _tv.Text = _result;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerHomePage - SetTextview_NotSelected - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Nhập List<ID> vào -> trả ra List<Fullname>
        /// </summary>
        /// <param name="_arrayID"></param>
        /// <returns></returns>
        public string[] GetArrayFullNameFromArrayID(string[] _arrayID, SQLite.SQLiteConnection conn = null)
        {
            conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            string[] _result = new string[_arrayID.Length];
            try
            {
                for (int i = 0; i < _arrayID.Count(); i++)
                {
                    string queryUser = string.Format(@"SELECT FullName FROM BeanUser WHERE ID = '{0}' LIMIT 1 OFFSET 0", _arrayID[i].ToLowerInvariant().Trim());
                    List<BeanUser> _lstUser = conn.Query<BeanUser>(queryUser);
                    if (_lstUser != null && _lstUser.Count > 0)
                        _result[i] = _lstUser[0].FullName;
                    else // Nếu ko có user -> searchGroup
                    {
                        string queryGroup = string.Format(@"SELECT Title, 0 as Type FROM BeanGroup WHERE ID = '{0}' LIMIT 1 OFFSET 0", _arrayID[i].ToLowerInvariant().Trim());
                        List<BeanGroup> _lstGroup = conn.Query<BeanGroup>(queryGroup);
                        if (_lstGroup != null && _lstGroup.Count > 0)
                            _result[i] = _lstGroup[0].Title;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetArrayFullNameFromArrayID", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
            return _result;
        }

        /// <summary>
        /// Gán avatar default app cho ImageView
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_imgAvatar"></param>
        public async void SetAvataForImageView(Activity _mainAct, ImageView _imgAvatar, int _widthPixel = 200)
        {
            try
            {
                string url = CmmVariable.M_Domain + CmmVariable.SysConfig.AvatarPath;
                ProviderBase pUser = new ProviderBase();
                bool result;
                if (!File.Exists(CmmVariable.M_Avatar))
                {
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, CmmVariable.M_Avatar, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(CmmVariable.M_Avatar, _widthPixel, _widthPixel);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _imgAvatar.SetImageBitmap(myBitmap);
                                });
                            }
                        }
                    });
                }
                else
                {
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(CmmVariable.M_Avatar, 200, 200);
                    if (myBitmap != null)
                    {
                        _imgAvatar.SetImageBitmap(myBitmap);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Check BeanAttachFile có tồn tại trong List Item không
        /// </summary>
        /// <param name="_lstItem"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        public bool CheckFileExistInList(List<BeanAttachFile> _lstItem, BeanAttachFile _item)
        {
            bool _res = false;
            try
            {
                foreach (BeanAttachFile item in _lstItem)
                {
                    if (item.Path.Equals(_item.Path))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerBase - CheckFileExistInList - Error: " + ex.Message);
#endif
            }
            return _res;
        }
        /// <summary>
        /// Clone list<BeanAttachFile> về dạng <Tittle,Path> để gửi lên server
        /// </summary>
        /// <param name="_lstItem"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> CloneKeyValueFromListAtt(List<BeanAttachFile> _lstItem)
        {
            List<KeyValuePair<string, string>> _res = new List<KeyValuePair<string, string>>();
            try
            {
                if (_lstItem != null && _lstItem.Count > 0)
                {
                    foreach (var item in _lstItem)
                    {
                        if (item.ID == "")
                        {
                            KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(item.Title, item.Path);
                            _res.Add(_UploadFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerBase - CloneKeyValueFromListAtt - Error: " + ex.Message);
#endif
            }
            return _res;
        }

        /// <summary>
        /// Khới tạo giá trị tracking: ObjectSubmitDetailComment
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public ObjectSubmitDetailComment InitTrackingObjectSubmitDetail(Activity activity)
        {
            ObjectSubmitDetailComment _objSubmitDetailComment = new ObjectSubmitDetailComment();
            try
            {
                // tracking
                _objSubmitDetailComment.DeviceName = "App Android";
                _objSubmitDetailComment.CodeName = "AppAndroid";
                _objSubmitDetailComment.AppName = "Digital Process";
                _objSubmitDetailComment.Platform = "Android API" + Build.VERSION.SdkInt;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitTrackingObjectSubmitDetail", ex);
#endif
            }
            return _objSubmitDetailComment;
        }

        [Obsolete]
        /// <summary>
        /// Cấp quyền app
        /// </summary>
        /// <param name="_mainAct"></param>
        public void RequestAppPermission(Activity _mainAct)
        {
            try
            {
                if (ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted ||
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted ||
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted ||
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.ClearAppCache) != Android.Content.PM.Permission.Granted ||
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.Vibrate) != Android.Content.PM.Permission.Granted ||
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.UseBiometric) != Android.Content.PM.Permission.Granted || 
                ContextCompat.CheckSelfPermission(_mainAct, Manifest.Permission.UseFingerprint) != Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(_mainAct, new[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage, Manifest.Permission.ClearAppCache,
                        Manifest.Permission.Vibrate,Manifest.Permission.UseBiometric,Manifest.Permission.UseFingerprint }, CmmDroidVariable.M_MyPermissionsRequestReadExternalStorage);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerStartView - RequestAppPermission - Error: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Return true if app has connection
        /// </summary>
        public bool CheckAppHasConnection()
        {
            try
            {
                ConnectivityManager cm = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
                NetworkInfo activeNetwork = cm?.ActiveNetworkInfo;
                if (activeNetwork != null && activeNetwork.IsConnected)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerStartView - CheckAppHasConnection - Error: " + ex.Message);
#endif
            }
            return false;
        }

        /// <summary>
        /// Trả về Resource Drawable ID của file đính kèm ứng với Path
        /// </summary>
        public int GetResourceIDAttachment(string _path)
        {
            int _result = Resource.Drawable.icon_attachFile_other;
            try
            {
                if (!string.IsNullOrEmpty(_path))
                {
                    string _extend = System.IO.Path.GetExtension(_path).ToLowerInvariant();

                    switch (_extend)
                    {
                        case ".doc":
                        case ".docx":
                            _result = Resource.Drawable.icon_word;
                            break;
                        case ".txt":
                            _result = Resource.Drawable.icon_attachFile_txt;
                            break;
                        case ".png":
                        case ".jpeg":
                        case ".jpg":
                            _result = Resource.Drawable.icon_attachFile_photo;
                            break;
                        case ".xls":
                        case ".xlsx":
                            _result = Resource.Drawable.icon_attachFile_excel;
                            break;
                        case ".pdf":
                            _result = Resource.Drawable.icon_attachFile_pdf;
                            break;
                        case ".ppt":
                            _result = Resource.Drawable.icon_attachFile_ppt;
                            break;
                        default:
                            _result = Resource.Drawable.icon_attachFile_other;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - ControllerStartView - CheckAppHasConnection - Error: " + ex.Message);
#endif
            }
            return _result;
        }

        /// <summary>
        /// Để binding đúng kiểu dữ liệu lên Textview TRạng thái - popup filter
        /// </summary>
        /// <param name="_lstTrangThai"></param>
        /// <param name="_tvTrangThai"></param>
        public void BindingList_ToTextViewTrangThai(Context _context, List<BeanAppStatus> _lstTrangThai, TextView _tvTrangThai) // Đang lưu, (+4)
        {
            try
            {
                List<BeanAppStatus> _lstSelected = _lstTrangThai.Where(x => x.IsSelected == true).ToList();

                if (_lstSelected.Count == _lstTrangThai.Count || _lstSelected.Count == 0) // check Case tất cả
                {
                    _tvTrangThai.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                    _tvTrangThai.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_context));
                    return;
                }

                if (_lstSelected.Count > 1) // từ 2 item trở lên
                {
                    _tvTrangThai.Text = String.Format("{0}, (+{1})",
                        CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstSelected[0].Title : _lstSelected[0].TitleEN,
                        (_lstSelected.Count - 1).ToString());
                }
                else
                    _tvTrangThai.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstSelected[0].Title : _lstSelected[0].TitleEN;
            }
            catch (Exception ex)
            {
                _tvTrangThai.Text = "";
#if DEBUG
                CmmDroidFunction.WriteTrackingError("SharedView_PopupFilterVDT", "BindingList_ToTvTrangThai", ex);
#endif
            }
        }

        /// <summary>
        /// Update lại giá trị của Element trong List Section
        /// </summary>
        /// <param name="_lstSections"></param>
        /// <param name="_element"></param>
        public void UpdateValueElement_InListSection(ref List<ViewSection> _lstSections, ViewElement _element, bool _reCalculated = false)
        {
            try
            {
                JObject _JObjectSource = new JObject(); // JObject những Element ko phải calculated

                foreach (ViewSection section in _lstSections ?? new List<ViewSection>())
                    foreach (ViewRow row in section.ViewRows ?? new List<ViewRow>())
                        foreach (ViewElement element in row.Elements ?? new List<ViewElement>())
                        {
                            if (element.ID.Equals(_element.ID)) // Cập nhật giá trị
                                element.Value = _element.Value;

                            if (_reCalculated == true /*&& String.IsNullOrEmpty(element.Formula)*/) // Add control vào
                                _JObjectSource.Add(element.Title, element.Value);
                        }

                if (_reCalculated) // Tính Formula lại các Element
                {
                    foreach (ViewSection section in _lstSections ?? new List<ViewSection>())
                        foreach (ViewRow row in section.ViewRows ?? new List<ViewRow>())
                            foreach (ViewElement element in row.Elements ?? new List<ViewElement>())
                            {
                                if (!String.IsNullOrEmpty(element.Formula))
                                {
                                    JToken _JTokenFormula = JToken.Parse(CmmFunction.CalculateObject(element.Formula, _JObjectSource).ToString());
                                    element.Value = _JTokenFormula.ToString();
                                }
                            }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateValue_ForElement", ex);
#endif
            }
        }
    }
}