using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Text;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Controller;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using Xamarin.Essentials;

namespace BPMOPMobile.Droid.Core.Common
{
    public static class CmmDroidFunction
    {
        public static Dialog _dialogProgressing = null;
        public static Dialog _dialogAlert = null;

        public static View _viewProgressing = null;
        public static View _viewAlert = null;

        public static ImageLoader _imageLoader = null;
        public static long _lastClickTime = SystemClock.ElapsedRealtime(); // lưu lại thời điểm cuối cùng thao tác app

        public static void ShowProcessingDialog(Activity _mainAct, string _messageVN, string _messageEN, bool _flgCancelable = true)
        {
            try
            {
                if (_dialogProgressing != null && _dialogProgressing.IsShowing) // Is running -> ko để bị chồng lên
                {
                    return;
                }

                if (_viewProgressing == null)
                {
                    _viewProgressing = _mainAct.LayoutInflater.Inflate(Resource.Layout.PopupCustomProgressDialog, null);
                    _dialogProgressing = new Dialog(_mainAct/*, Resource.Style.AlertCustomProgressDialog*/);
                    _dialogProgressing.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                    _dialogProgressing.Window.ClearFlags(WindowManagerFlags.DimBehind);
                    _dialogProgressing.Window.Attributes.Width = _mainAct.Resources.DisplayMetrics.WidthPixels;
                    _dialogProgressing.Window.Attributes.Height = _mainAct.Resources.DisplayMetrics.HeightPixels;
                    _dialogProgressing.SetContentView(_viewProgressing);
                }
                else
                {
                    _viewProgressing = null;
                    _dialogProgressing = null;
                    _viewProgressing = _mainAct.LayoutInflater.Inflate(Resource.Layout.PopupCustomProgressDialog, null);
                    _dialogProgressing = new Dialog(_mainAct/*, Resource.Style.AlertCustomProgressDialog*/);
                    _dialogProgressing.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                    _dialogProgressing.Window.ClearFlags(WindowManagerFlags.DimBehind);
                    _dialogProgressing.Window.Attributes.Width = _mainAct.Resources.DisplayMetrics.WidthPixels;
                    _dialogProgressing.Window.Attributes.Height = _mainAct.Resources.DisplayMetrics.HeightPixels;
                    _dialogProgressing.SetContentView(_viewProgressing);
                }    

                TextView _tvTitle = _viewProgressing.FindViewById<TextView>(Resource.Id.tv_PopupCustomProgressDialog);
                LinearLayout _lnAll = _viewProgressing.FindViewById<LinearLayout>(Resource.Id.ln_PopupCustomProgressDialog_Content);
                //_lnAll.LayoutParameters = new LinearLayout.LayoutParams(_mainAct.Resources.DisplayMetrics.WidthPixels, _mainAct.Resources.DisplayMetrics.HeightPixels);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvTitle.Text = !String.IsNullOrEmpty(_messageVN) ? _messageVN : "";
                else
                    _tvTitle.Text = !String.IsNullOrEmpty(_messageEN) ? _messageEN : "";

                _dialogProgressing.SetCancelable(_flgCancelable);
                _dialogProgressing.Show();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowProcessingDialog - Error: " + ex.Message);
#endif
            }
        }

        public static void HideProcessingDialog()
        {
            try
            {
                if (_dialogProgressing != null)
                    _dialogProgressing.Dismiss();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - HideProcessingDialog - Error: " + ex.Message);
#endif
            }
        }

        public static void ShowSoftKeyBoard(View _mRootView, Activity _mainAct)
        {
            try
            {
                InputMethodManager inputMethodManager = _mainAct.GetSystemService(Context.InputMethodService) as InputMethodManager;
                if (inputMethodManager != null)
                {
                    inputMethodManager.ShowSoftInput(_mRootView, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowSoftKeyBroad - Error: " + ex.Message);
#endif
            }
        }

        public static void ShowAlertDialog(Activity _mainAct, string _messageVN, string _messageEN, string _titleVN = "Thông báo", string _titleEN = "Alert", string _negativeVN = "Đóng", string _negativeEN = "Close")
        {
            try
            {
                if (_viewAlert == null)
                {
                    _viewAlert = _mainAct.LayoutInflater.Inflate(Resource.Layout.PopupCustomAlertDialog, null);
                    _dialogAlert = new Dialog(_mainAct);
                    _dialogAlert.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                    _dialogAlert.SetCancelable(true);
                    _dialogAlert.SetContentView(_viewAlert);
                }
                else
                {
                    _viewAlert = null;
                    _dialogAlert = null;
                    _viewAlert = _mainAct.LayoutInflater.Inflate(Resource.Layout.PopupCustomAlertDialog, null);
                    _dialogAlert = new Dialog(_mainAct);
                    _dialogAlert.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                    _dialogAlert.SetCancelable(true);
                    _dialogAlert.SetContentView(_viewAlert);
                } 
                    

                TextView _tvTitle = _viewAlert.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Title);
                TextView _tvMessage = _viewAlert.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Message);
                TextView _tvPositive = _viewAlert.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Positive);
                TextView _tvNegative = _viewAlert.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Negative);
                View _vwDivider = _viewAlert.FindViewById<View>(Resource.Id.vw_PopupCustomAlertDialog_Divider);


                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvTitle.Text = !String.IsNullOrEmpty(_titleVN) ? _titleVN : "";
                    _tvMessage.Text = !String.IsNullOrEmpty(_messageVN) ? _messageVN : "";
                    _tvNegative.Text = !String.IsNullOrEmpty(_negativeVN) ? _negativeVN : "";
                }
                else
                {
                    _tvTitle.Text = !String.IsNullOrEmpty(_titleEN) ? _titleEN : "";
                    _tvMessage.Text = !String.IsNullOrEmpty(_messageEN) ? _messageEN : "";
                    _tvNegative.Text = !String.IsNullOrEmpty(_negativeEN) ? _negativeEN : "";
                }

                _tvPositive.Visibility = ViewStates.Gone;
                _vwDivider.Visibility = ViewStates.Gone;

                _tvNegative.Click += (sender, e) =>
                {
                    Action action = new Action(() =>
                    {
                        _dialogAlert.Dismiss();
                    });
                    new Handler().PostDelayed(action, 250);
                };

                _dialogAlert.Show();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowAlertDialog - Error: " + ex.Message);
#endif
            }
        }

        public static void ShowAlertDialogWithAction(Activity _mainAct, string _message, Action _actionPositiveButton, Action _actionNegativeButton,
            string _title = "", string _positive = "Đồng ý", string _negative = "Đóng", bool _cancelable = true)
        {
            try
            {
                //_alertDialogBuilder = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                View _view = _mainAct.LayoutInflater.Inflate(Resource.Layout.PopupCustomAlertDialog, null);
                TextView _tvTitle = _view.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Title);
                TextView _tvMessage = _view.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Message);
                TextView _tvPositive = _view.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Positive);
                TextView _tvNegative = _view.FindViewById<TextView>(Resource.Id.tv_PopupCustomAlertDialog_Negative);
                View _vwDivider = _view.FindViewById<View>(Resource.Id.vw_PopupCustomAlertDialog_Divider);

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _tvTitle.Text = !String.IsNullOrEmpty(_title) ? _title : "Thông báo";
                else
                    _tvTitle.Text = !String.IsNullOrEmpty(_title) ? _title : "Alert";

                _tvMessage.Text = _message;
                _tvPositive.Text = _positive;
                _tvNegative.Text = _negative;

                if (_actionPositiveButton != null)
                {
                    _tvPositive.Click += (sender, e) =>
                    {
                        Action action = new Action(() =>
                        {
                            _actionPositiveButton();
                            _dialogAlert.Dismiss();
                        });
                        new Handler().PostDelayed(action, 250);
                    };
                }
                else
                {
                    _vwDivider.Visibility = ViewStates.Gone;
                    _tvPositive.Visibility = ViewStates.Gone;
                }

                if (_actionNegativeButton != null)
                {
                    _tvNegative.Click += (sender, e) =>
                    {
                        Action action = new Action(() =>
                        {
                            _actionNegativeButton();
                            _dialogAlert.Dismiss();
                        });
                        new Handler().PostDelayed(action, 250);
                    };
                }
                else
                {
                    _vwDivider.Visibility = ViewStates.Gone;
                    _tvNegative.Visibility = ViewStates.Gone;
                }

                //_alertDialogBuilder.SetView(_view);
                _dialogAlert = new Dialog(_mainAct);
                _dialogAlert.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                //_dialogAlert = _alertDialogBuilder.Create();
                _dialogAlert.SetCancelable(_cancelable);
                _dialogAlert.SetContentView(_view);
                _dialogAlert.Show();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowAlertDialog - Error: " + ex.Message);
#endif
            }
        }

        public static void HideSoftKeyBoard(EditText _edt, Activity _mainAct)
        {
            try
            {
                InputMethodManager inputMethodManager = (InputMethodManager)_mainAct.GetSystemService(Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(_edt.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - HideSoftKeyBroad - Error: " + ex.Message);
#endif
            }
        }

        public static void HideSoftKeyBoard(Activity _mainAct)
        {
            try
            {
                InputMethodManager inputMethodManager = (InputMethodManager)_mainAct.GetSystemService(Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(_mainAct.CurrentFocus?.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - HideSoftKeyBroad - Error: " + ex.Message);
#endif
            }
        }

        public static void SetupSwipeRefreshLayout(SwipeRefreshLayout _swipe)
        {
            try
            {
                _swipe.SetDistanceToTriggerSync(CmmDroidVariable.M_SwipeRefreshLayoutDistance); // in dips    
                _swipe.SetColorSchemeResources(Resource.Color.clVer2BlueMain);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetupSwipeRefreshLayout - Error: " + ex.Message);
#endif
            }
        }

        public static void ShowVibrateEvent(double _second = -1)
        {
            try
            {
                if (_second == -1) // Default -> 500ms
                {
                    Vibration.Vibrate();
                }
                else if (_second < 10)
                {
                    Vibration.Vibrate(TimeSpan.FromSeconds(_second));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowVibrateEvent - Error: " + ex.Message);
#endif
            }
        }

        public static void ShowLeftMenu(DrawerLayout _drawerLayout, FrameLayout _frame)
        {
            try
            {
                if (_drawerLayout != null)
                {
                    _drawerLayout.OpenDrawer(_frame);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - ShowVibrateEvent - Error: " + ex.Message);
#endif
            }
        }

        public static void WriteLogError(Exception ex, string _extentMess = "", string _logPath = "")
        {
            try
            {
                if (string.IsNullOrEmpty(_logPath))
                {
                    ////logPath = CmmVariable.M_LogPath;
                }

                if (!File.Exists(_logPath))
                {
                    using (StreamWriter sw = File.CreateText(_logPath))
                    {
                        sw.WriteLine("============================ "
                                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                    + " ============================");
                        if (ex != null)
                        {
                            sw.WriteLine(ex.StackTrace);
                            sw.WriteLine("MESS:" + ex.Message);
                        }
                        if (_extentMess.Length > 0) sw.WriteLine("MESS EX:" + _extentMess);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(_logPath))
                    {
                        sw.WriteLine("============================ "
                                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                    + " customer Id: "
                                    + " ============================");
                        if (ex != null)
                        {
                            sw.WriteLine(ex.StackTrace);
                            sw.WriteLine("MESS:" + ex.Message);
                        }
                        if (_extentMess.Length > 0) sw.WriteLine("MESS EX:" + _extentMess);
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - WriteLogError - Error: " + e.Message);
#endif
            }
        }

        public static void RemoveRootViewMain(View _rootView)
        {
            if (_rootView != null)
            {
                ViewGroup parent = (ViewGroup)_rootView.Parent;
                if (parent != null)
                {
                    // parent.RemoveAllViews();
                }
            }
        }

        /// <summary>
        /// Hàm để chống click nhiều lần -> return true là được phép xử lý tiếp
        /// </summary>
        /// <param name="_milisecond"> nếu ko nhập mặc định là 500</param>
        /// <returns></returns>
        public static bool PreventMultipleClick(int _milisecond = -1)
        {
            try
            {
                if (SystemClock.ElapsedRealtime() - _lastClickTime < (_milisecond == -1 ? CmmDroidVariable.M_MulticlickPreventTime : _milisecond)) // không cho click nhiều lần
                    return false;
                else
                {
                    _lastClickTime = SystemClock.ElapsedRealtime();
                    return true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - PreventMulticlickTime - Error: " + ex.Message);
#endif
            }
            return false;
        }

        /// <summary>
        /// Hàm để chống click nhiều lần trên 1 View -> sẽ Enable View lại sau thời gian quy định
        /// </summary>
        /// <param name="_milisecond"> nếu ko nhập mặc định là 500</param>
        /// <returns></returns>
        public static bool PreventClickOnView(View _view, int _milisecond = -1)
        {
            try
            {
                _view.Enabled = false;
                Action action = new Action(() =>
                {
                    _view.Enabled = true;
                });
                new Handler().PostDelayed(action, (_milisecond == -1 ? CmmDroidVariable.M_MulticlickPreventTime : _milisecond));
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - PreventClickOnView - Error: " + ex.Message);
#endif
            }
            return false;
        }

        public static void SetTextViewHighlightColor(Context _context, TextView _textview, string _content, string _starChar, string _endChar, int _type = 0)
        {
            try
            {
                int _startPos = -1, _endPos = -1;

                if (_starChar == null || _endChar == null) // nếu null -> trả về màu trắng
                {
                    ISpannable spannable = new SpannableString(_content.Trim());
                    ColorStateList White = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clWhite)) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, White, null);
                    spannable.SetSpan(highlightSpan, 0, _content.Length - 1, SpanTypes.ExclusiveExclusive);
                    _textview.SetText(spannable, TextView.BufferType.Spannable);
                    return;
                }

                if (_type == 0) // tô màu từ _starChar đến _endChar
                {
                    _startPos = _content.IndexOf(_starChar);
                    _endPos = _content.IndexOf(_endChar) + 1;
                }
                else if (_type == 1) // tô màu phần nằm trong _starChar và _endChar
                {
                    _startPos = _content.IndexOf(_starChar) + 1;
                    _endPos = _content.IndexOf(_endChar);
                }
                if (_startPos != -1 && _endPos != -1)
                {
                    ISpannable spannable = new SpannableString(_content.Trim());
                    ColorStateList Red = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clOrangeFilter)) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, Red, null);
                    spannable.SetSpan(highlightSpan, _startPos, _endPos, SpanTypes.ExclusiveExclusive);
                    _textview.SetText(spannable, TextView.BufferType.Spannable);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetTextViewHighlightColor - Error: " + ex.Message);
#endif
            }
        }

        public static void SetTextViewHighlightControl(Context _context, TextView _textview)
        {
            try
            {
                int _startPos = -1, _endPos = -1;

                for (int i = 0; i < _textview.Text.Length; i++)
                {
                    if (_textview.Text[i].Equals('('))
                        _startPos = i;
                    if (_textview.Text[i].Equals(')'))
                        _endPos = i;
                }

                if (_startPos != -1 && _endPos != -1)
                {
                    ISpannable spannable = new SpannableString(_textview.Text.Trim());
                    ColorStateList Red = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_context, Resource.Color.clActionRed)) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Bold, -1, Red, null);
                    spannable.SetSpan(highlightSpan, _startPos, _endPos + 1, SpanTypes.ExclusiveExclusive);
                    _textview.SetText(spannable, TextView.BufferType.Spannable);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetTextViewHighlightColor - Error: " + ex.Message);
#endif
            }
        }

        public static void SetExpandListViewHeight(ExpandableListView lv) // tạo list có chiều cao full item
        {
            try
            {
                int totalHeight = lv.PaddingTop + lv.PaddingBottom;
                for (int i = 0; i < lv.Count; i++)
                {
                    View listItem = lv.Adapter.GetView(i, null, lv);
                    if (listItem.GetType() == typeof(ViewGroup))
                    {
                        listItem.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    }
                    listItem.Measure(0, 0);
                    totalHeight += listItem.MeasuredHeight;
                    //if (i == lv.Count - 1)
                    //{
                    //    totalHeight += listItem.MeasuredHeight / 3;
                    //}
                }
                lv.LayoutParameters.Height = totalHeight; //+ (lv.DividerHeight * (lv.Count - 1));
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetListViewHeight - Error: " + ex.Message);
#endif
            }
        }
        /// <summary>
        /// Nếu Expand height > screen height thì limit lại và cho nested scroll -> để ko bị lag
        /// </summary>
        /// <param name="lv"></param>

        public static void SetDynamicExpandListViewHeight(Activity _mainAct, ExpandableListView _expand, int _screenHeight = -1) // tạo list có chiều cao full item
        {
            try
            {
                if (_screenHeight == -1) // lấy default screen height
                {
                    DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                    _screenHeight = dm.HeightPixels;
                }


                int _expandListHeight = CmmDroidFunction.GetExpandListViewHeight(_expand);

                if (_expandListHeight > _screenHeight) // Limit lại và cho scroll inside
                {
                    _expand.LayoutParameters.Height = _screenHeight;
                    _expand.NestedScrollingEnabled = true;
                }
                else
                {
                    _expand.LayoutParameters.Height = _expandListHeight;
                    _expand.NestedScrollingEnabled = false;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetDynamicExpandListViewHeight - Error: " + ex.Message);
#endif
            }
        }

        public static int GetExpandListViewHeight(ExpandableListView lv) // tạo list có chiều cao full item
        {
            try
            {
                int totalHeight = lv.PaddingTop + lv.PaddingBottom;
                for (int i = 0; i < lv.Count; i++)
                {
                    View listItem = lv.Adapter.GetView(i, null, lv);
                    if (listItem.GetType() == typeof(ViewGroup))
                    {
                        listItem.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    }
                    listItem.Measure(0, 0);
                    totalHeight += listItem.MeasuredHeight;
                    //if (i == lv.Count - 1)
                    //{
                    //    totalHeight += listItem.MeasuredHeight / 3;
                    //}
                }
                return totalHeight;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - SetListViewHeight - Error: " + ex.Message);
#endif

            }
            return 0;
        }
        /// <summary>
        /// trả về số item count (Group + child) Expandable Listview. -1 là sai
        /// </summary>
        /// <param name="_expandAdapter"></param>
        /// <returns></returns>

        public static int GetExpandAdapterItemCount(BaseExpandableListAdapter _expandAdapter)
        {
            int _count = -1;
            try
            {
                for (int i = 0; i < _expandAdapter.GroupCount; i++)
                {
                    _count += _expandAdapter.GetChildrenCount(i) + 1; // + thêm group
                }
            }
            catch (Exception ex)
            {
                _count = -1;
#if DEBUG
                CmmDroidFunction.WriteTrackingError("ControllerBoard", "GetDefaultValue_Filter", ex);
#endif
            }
            return _count;

        }

        public static void SetListViewHeight(ListView lv)
        {
            int totalHeight = lv.PaddingTop + lv.PaddingBottom;
            for (int i = 0; i < lv.Adapter.Count; i++)
            {
                View listItem = lv.Adapter.GetView(i, null, lv);
                if (listItem.GetType() == typeof(ViewGroup))
                {
                    listItem.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                }
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }
            lv.LayoutParameters.Height = totalHeight + 10;
        }

        public static int GetRecyclerViewHeight(Activity _mainAct, RecyclerView _recy)
        {
            try
            {
                int _recyHeight = _recy.PaddingTop + _recy.PaddingBottom;

                if (_recy.GetAdapter().ItemCount > 0)
                {
                    RecyclerView.ViewHolder holderItem = _recy.FindViewHolderForAdapterPosition(0);

                    if (holderItem != null)
                    {
                        holderItem.ItemView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
                        _recyHeight += (holderItem.ItemView.MeasuredHeight * _recy.GetAdapter().ItemCount);
                    }
                }
                //_recyHeight += 10;
                return _recyHeight;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static void SetRecyclerViewHeight(Activity _mainAct, RecyclerView _recy, int _screenHeight = -1)
        {
            try
            {
                if (_screenHeight == -1) // lấy default screen height
                {
                    DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                    _screenHeight = dm.HeightPixels;
                }

                int _recyHeight = _recy.PaddingTop + _recy.PaddingBottom;

                if (_recy.GetAdapter().ItemCount > 0)
                {
                    RecyclerView.ViewHolder holderItem = _recy.FindViewHolderForAdapterPosition(0);
                    if (holderItem != null)
                    {
                        holderItem.ItemView.Measure(0, 0);
                        _recyHeight += (holderItem.ItemView.MeasuredHeight * _recy.GetAdapter().ItemCount);
                    }
                }
                _recyHeight += 10;

                if (_recyHeight > _screenHeight) // Limit lại và cho scroll inside
                {
                    _recy.LayoutParameters.Height = _screenHeight;
                    _recy.NestedScrollingEnabled = true;
                }
                else
                {

                    _recy.LayoutParameters.Height = _recyHeight;
                    _recy.NestedScrollingEnabled = false;
                }

            }

            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "SetRecyclerViewHeight", ex);
#endif
            }
        }

        public static void SetRecyclerViewWidth(RecyclerView _recy)
        {
            int totalWidth = _recy.PaddingLeft + _recy.PaddingRight;
            for (int i = 0; i < _recy.GetAdapter().ItemCount; i++)
            {
                RecyclerView.ViewHolder holderItem = _recy.FindViewHolderForAdapterPosition(i);
                holderItem.ItemView.Measure(0, 0);
                totalWidth += holderItem.ItemView.MeasuredWidth;
            }
            _recy.LayoutParameters.Width = totalWidth + 10;
        }

        public static void RemoveEventHandlerLinearLayout(LinearLayout rootView)
        {
            try
            {
                FieldInfo f1 = typeof(Java.Util.ResourceBundle.Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);

                if (f1 != null)
                {
                    object obj = f1.GetValue(rootView);
                    PropertyInfo pi = rootView.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

                    System.ComponentModel.EventHandlerList list = (System.ComponentModel.EventHandlerList)pi.GetValue(rootView, null);
                    list.RemoveHandler(obj, list[obj]);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - RemoveEventHandlerLinearLayout - Error: " + ex.Message);
#endif
            }
        }

        public static float ConvertDpToPixel(float dp, Context context)
        {
            //return dp * ((float)context.Resources.DisplayMetrics.DensityDpi / (float)DisplayMetrics.DensityDefault);
            return dp * ((float)context.Resources.DisplayMetrics.DensityDpi / (float)Android.Util.DisplayMetricsDensity.Default);
        }

        public static float ConvertPixelsToDp(float px, Context context)
        {
            //return px / ((float)context.Resources.DisplayMetrics.DensityDpi / (float)DisplayMetrics.DensityDefault);
            return px / ((float)context.Resources.DisplayMetrics.DensityDpi / (float)Android.Util.DisplayMetricsDensity.Default);
        }

        public static void SetPropertyValueByNameCustom(object obj, string strPropName, object value)
        {
            PropertyInfo propInfo = GetProperty(obj, strPropName);
            if (propInfo != null)
            {
                Type typeObject = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                Type typePropretyBase = typeof(PropertyBase);
                MethodInfo theMethod = typePropretyBase.GetMethod("Get" + typeObject.Name);

                if (theMethod != null)
                {
                    PropertyBase proprety = new PropertyBase();
                    var result = theMethod.Invoke(proprety, new[] { value });
                    propInfo.SetValue(obj, result, null);
                }
                else
                    SetPropertyValue(obj, propInfo, value);
            }
        }

        public static PropertyInfo GetProperty(object obj, string strPropName)
        {
            Type type = obj.GetType();
            return type.GetProperty(strPropName);
        }

        /// <summary>
        ///  Set giá trị cho thuộc tính của Object
        /// </summary>
        /// <param name="obj">Object muốn set giá trị</param>
        /// <param name="propInfo">Thuộc tính propertyInfo thuộc Class Object</param>
        /// <param name="value">Giá trị muốn set</param>
        /// <returns></returns>
        public static void SetPropertyValue(object obj, PropertyInfo propInfo, object value)
        {
            Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
            propInfo.SetValue(obj, safeValue, null);
        }

        public static bool CheckSdCard()
        {
            string auxSdCardStatus = Android.OS.Environment.ExternalStorageState;
            if (auxSdCardStatus == Android.OS.Environment.MediaMounted)
                return true;
            else
                return false;
        }

        public static void SetTitleToView(View vi)
        {
            if (vi.GetType() == typeof(LinearLayout))
            {
                LinearLayout vil = (LinearLayout)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else if (vi.GetType() == typeof(RelativeLayout))
            {
                RelativeLayout vil = (RelativeLayout)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else if (vi.GetType() == typeof(FrameLayout))
            {
                FrameLayout vil = (FrameLayout)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else if (vi.GetType() == typeof(RadioGroup))
            {
                RadioGroup vil = (RadioGroup)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else if (vi.GetType() == typeof(ScrollView))
            {
                ScrollView vil = (ScrollView)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else if (vi.GetType() == typeof(Android.Support.V4.Widget.SwipeRefreshLayout))
            {
                Android.Support.V4.Widget.SwipeRefreshLayout vil = (Android.Support.V4.Widget.SwipeRefreshLayout)vi;
                for (int i = 0; i < vil.ChildCount; i++)
                {
                    SetTitleToView(vil.GetChildAt(i));
                }
            }
            else
            {

                if (vi.Tag != null)
                {
                    PropertyInfo prop;
                    if (vi.GetType().ToString().Contains("EditText"))
                    {
                        prop = vi.GetType().GetProperty("hint");
                    }
                    else
                    {
                        prop = vi.GetType().GetProperty("Text");
                    }


                    if (prop != null)
                    {
                        string strDefaultValue;
                        object defaultValue = CmmFunction.GetPropertyValue(vi, prop);
                        strDefaultValue = defaultValue == null ? "" : defaultValue.ToString();

                        string langValue = CmmFunction.GetTitle(vi.Tag.ToString(), strDefaultValue);
                        CmmDroidFunction.SetPropertyValue(vi, prop, langValue);
                    }
                }
            }

        }

        /// <summary>
        /// Example: "My job (99+)" -> return (99+)
        /// </summary>
        /// <param name="_content"></param>
        /// <returns></returns>
        public static string GetCountNumOfText(string _content)
        {
            Regex reg = new Regex("[0-9+()]+");
            Match match = reg.Match(_content);
            return match.Value;
        }

        /// <summary>
        /// tính độ dài của chuỗi bằng InDp
        /// </summary>
        /// <param name="myTextView"></param>
        /// <returns></returns>
        public static bool CalculateTextLength(TextView _myTextView, DisplayMetrics _displayMetrics)
        {
            try
            {
                // get the screen width
                var metrics = _displayMetrics;
                var width = _myTextView.MeasuredWidth;
                var widthInDp = (int)((width - 10) / metrics.Density);


                // now calculating the text length in dp            
                Paint paint = new Paint();
                paint.TextSize = _myTextView.TextSize;
                var textLength = (int)Math.Ceiling(paint.MeasureText(_myTextView.Text, 0, _myTextView.Text.Length) / metrics.Density);
                if (widthInDp >= textLength)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void LogErr(Exception ex, string extentMess = "", string logPath = "")
        {
            try
            {
                if (string.IsNullOrEmpty(logPath))
                {
                    logPath = CmmVariable.M_LogPath;
                }

                if (!File.Exists(logPath))
                {
                    using (StreamWriter sw = File.CreateText(logPath))
                    {
                        sw.WriteLine("============================ "
                                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                    + " ============================");
                        if (ex != null)
                        {
                            sw.WriteLine(ex.StackTrace);
                            sw.WriteLine("MESS:" + ex.Message);
                        }
                        if (extentMess.Length > 0) sw.WriteLine("MESS EX:" + extentMess);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(logPath))
                    {
                        sw.WriteLine("============================ "
                                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                    + " customer Id: "
                                    + " ============================");

                        if (ex != null)
                        {
                            sw.WriteLine(ex.StackTrace);
                            sw.WriteLine("MESS:" + ex.Message);
                        }
                        if (extentMess.Length > 0) sw.WriteLine("MESS EX:" + extentMess);
                    }
                }

            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - Error: " + e.Message);
#endif
            }
        }

        public static void WriteTrackingError(string _fragmentName, string _functionName, Exception _ex, string _author = "khoahd")
        {
            try
            {
                if (_ex != null)
                {
                    Console.WriteLine(String.Format("Author: {0} - {1} - {2} - Error: {3}", _author, _fragmentName, _functionName, _ex.Message));
                }
                else
                {
                    Console.WriteLine(String.Format("Author: {0} - {1} - {2}", _author, _fragmentName, _functionName));
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - CmmDroidFunction - Error: " + e.Message);
#endif
            }
        }

        /// <summary>
        /// Kiểm tra Bean User và load theo dạng Text hay dạng Circle Image
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="_context"></param>
        /// <param name="_selectedUser"></param>
        /// <param name="path"></param>
        /// <param name="imgAvatar"></param>
        /// <param name="tvAvatar"></param>
        public static async void SetAvataByBeanUser(Activity _mainAct, Context _context, string _valueToSearch, string _columnToSearch, ImageView imgAvatar, TextView tvAvatar, int _widthheightBitmapSize)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            //bool _flagIsTextAvatar = true; // Check xem load avatar theo dạng nào: True là dạng Text
            ControllerBase CTRLBase = new ControllerBase();
            try
            {
                if (!String.IsNullOrEmpty(_valueToSearch))
                {
                    #region Query User
                    string queryUser = "";
                    if (String.IsNullOrEmpty(_columnToSearch)) // Search Default
                    {
                        queryUser = string.Format("SELECT ImagePath FROM BeanUser WHERE AccountName = '{0}' LIMIT 1 OFFSET 0", _valueToSearch.Trim());
                    }
                    else // Search theo Cột
                    {
                        queryUser = string.Format("SELECT ImagePath FROM BeanUser WHERE {0} = '{1}' LIMIT 1 OFFSET 0", _columnToSearch, _valueToSearch.Trim());
                    }
                    #endregion

                    List<BeanUser> lstUser = conn.Query<BeanUser>(queryUser);
                    if (lstUser != null && lstUser.Count > 0)
                    {
                        imgAvatar.Visibility = ViewStates.Visible;
                        tvAvatar.Visibility = ViewStates.Gone;
                        //CmmDroidFunction.SetAvataByImagePath(_mainAct, lstUser[0].ImagePath, imgAvatar, tvAvatar);
                        string path = lstUser[0].ImagePath;

                        #region Download và load ảnh
                        if (!String.IsNullOrEmpty(path))
                        {
                            string imgFilePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException());
                            string url = CmmVariable.M_Domain + path;
                            ProviderBase pUser = new ProviderBase();
                            bool result;
                            if (!File.Exists(imgFilePath))
                            {
                                await Task.Run(() =>
                                {
                                    result = pUser.DownloadFile(url, imgFilePath, CmmVariable.M_AuthenticatedHttpClient);
                                    if (result)
                                    {
                                        Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(imgFilePath, _widthheightBitmapSize, _widthheightBitmapSize);
                                        if (myBitmap != null)
                                        {
                                            _mainAct.RunOnUiThread(() =>
                                            {
                                                imgAvatar.SetImageBitmap(myBitmap);
                                            });
                                        }
                                        else
                                        {
                                            _mainAct.RunOnUiThread(() =>
                                            {
                                                imgAvatar.Visibility = ViewStates.Gone;
                                                tvAvatar.Visibility = ViewStates.Visible;

                                                tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                                                tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
                                            });
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(imgFilePath, _widthheightBitmapSize, _widthheightBitmapSize);
                                if (myBitmap != null)
                                {
                                    imgAvatar.SetImageBitmap(myBitmap);
                                }
                                else
                                {
                                    imgAvatar.Visibility = ViewStates.Gone;
                                    tvAvatar.Visibility = ViewStates.Visible;

                                    tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                                    tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
                                }
                            }
                        }
                        else
                        {
                            imgAvatar.Visibility = ViewStates.Gone;
                            tvAvatar.Visibility = ViewStates.Visible;

                            tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                            tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
                        }
                        #endregion
                    }
                    else
                    {
                        imgAvatar.Visibility = ViewStates.Gone;
                        tvAvatar.Visibility = ViewStates.Visible;

                        tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                        tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
                    }
                    conn.Close();
                }
                else
                {
                    imgAvatar.Visibility = ViewStates.Gone;
                    tvAvatar.Visibility = ViewStates.Visible;

                    tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                    tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
                }
            }
            catch (Exception ex)
            {
                imgAvatar.Visibility = ViewStates.Gone;
                tvAvatar.Visibility = ViewStates.Visible;

                tvAvatar.Text = CmmFunction.GetAvatarName(_valueToSearch);
                tvAvatar.BackgroundTintList = ColorStateList.ValueOf(CTRLBase.GetColorByUserName(_context, _valueToSearch));
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetAvataByImagePath", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public static async void SetAvataByImagePath(Activity _mainAct, string path, ImageView img, TextView tv, int _widthheightBitmapSize)
        {
            try
            {
                string pdfFilePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException());
                string url = CmmVariable.M_Domain + path;
                ProviderBase pUser = new ProviderBase();
                bool result;
                if (!File.Exists(pdfFilePath))
                {
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, pdfFilePath, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(pdfFilePath, _widthheightBitmapSize, _widthheightBitmapSize);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.Visibility = ViewStates.Visible;
                                    tv.Visibility = ViewStates.Gone;
                                    img.SetImageBitmap(myBitmap);
                                });
                            }
                            else
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.Visibility = ViewStates.Gone;
                                    tv.Visibility = ViewStates.Visible;
                                    img.SetImageResource(Resource.Drawable.icon_avatar64);
                                });
                            }
                        }
                    });
                }
                else
                {
                    img.Visibility = ViewStates.Visible;
                    tv.Visibility = ViewStates.Gone;
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(pdfFilePath, _widthheightBitmapSize, _widthheightBitmapSize);
                    if (myBitmap != null)
                    {
                        img.SetImageBitmap(myBitmap);
                    }
                    else
                    {
                        img.SetImageResource(Resource.Drawable.icon_avatar64);
                    }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetAvataByImagePath", ex);
#endif
            }
        }

        /// <summary>
        /// giống SetAvataByImagePath nhưng chỉ có imageview -> ko có textview ẩn ở dưới
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="path"></param>
        /// <param name="img"></param>
        /// <param name="tv"></param>
        /// <param name="_widthheightBitmapSize"></param>
        public static async void SetAvataByImagePath2(Activity _mainAct, string path, ImageView img, int _widthheightBitmapSize)
        {
            try
            {
                string filePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException());
                if (!File.Exists(filePath))
                {
                    ProviderBase pUser = new ProviderBase();
                    string url = CmmVariable.M_Domain + path;
                    bool result;
                    await Task.Run(() =>
                    {
                        result = pUser.DownloadFile(url, filePath, CmmVariable.M_AuthenticatedHttpClient);
                        if (result)
                        {
                            Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(filePath, _widthheightBitmapSize, _widthheightBitmapSize);
                            if (myBitmap != null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.Visibility = ViewStates.Visible;
                                    img.SetImageBitmap(myBitmap);
                                    img.StartAnimation(AnimationUtils.LoadAnimation(_mainAct, Resource.Animation.anim_clickview));
                                });
                            }
                            else
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    img.Visibility = ViewStates.Gone;
                                    img.SetImageResource(Resource.Drawable.img_ver3_error);
                                });
                            }
                        }
                    });
                }
                else
                {
                    img.Visibility = ViewStates.Visible;
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(filePath, _widthheightBitmapSize, _widthheightBitmapSize);
                    if (myBitmap != null)
                    {
                        img.SetImageBitmap(myBitmap);
                        img.StartAnimation(AnimationUtils.LoadAnimation(_mainAct, Resource.Animation.anim_clickview));
                    }
                    else
                        img.SetImageResource(Resource.Drawable.img_ver3_error);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetAvataByImagePath", ex);
#endif
            }
        }

        /// <summary>
        /// Hàm xử lý Path to ImageView
        /// </summary>
        /// <param name="_mainAct"></param>
        /// <param name="img"></param>
        /// <param name="path"></param>
        /// <param name="_widthPixel"></param>
        public static async void SetContentToImageView(Activity _mainAct, ImageView img, string path, int _widthPixel, int _resourceIdDefaultImage = -123)
        {
            try
            {
                if (_resourceIdDefaultImage == -123)
                    _resourceIdDefaultImage = Resource.Drawable.icon_avatar64;

                string filePath = System.IO.Path.Combine(CmmVariable.M_Folder_Avatar + "/", System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException());
                if (!File.Exists(filePath))
                {
                    await Task.Run(() =>
                    {
                        Bitmap myBitmap = BitmapHelper.DownloadAndResizeBitmap(CmmVariable.M_Domain + path, _widthPixel, filePath, CmmVariable.M_AuthenticatedHttpClient);
                        _mainAct.RunOnUiThread(() =>
                        {
                            if (myBitmap != null)
                                img.SetImageBitmap(myBitmap);
                            else
                                img.SetImageResource(_resourceIdDefaultImage);
                        });
                    });
                }
                else
                {
                    Bitmap myBitmap = BitmapHelper.LoadAndResizeBitmap(filePath, _widthPixel, _widthPixel);
                    if (myBitmap != null)
                        img.SetImageBitmap(myBitmap);
                    else
                        img.SetImageResource(_resourceIdDefaultImage);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "SetAvataByImagePath", ex);
#endif
            }
        }

        public static string GetDisplayNameOfURI(Context _context, Android.Net.Uri uri)
        {
            string result = null;
            if (uri.Scheme.Equals("content"))
            {
                ICursor cursor = Application.Context.ContentResolver.Query(uri, null, null, null, null);
                try
                {
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                    }
                }
                finally
                {
                    cursor.Close();
                }
            }
            if (result == null)
            {
                result = uri.Path;
                int cut = result.LastIndexOf('/');
                if (cut != -1)
                {
                    result = result.Substring(cut + 1);
                }
            }
            return result;
        }

        /// <summary>
        /// Nhập vào số Bytes - > trả về định dạng phù hợp
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetFormatFileSize(long sizeBytes)
        {
            try
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                int order = 0;
                while (sizeBytes >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    sizeBytes = sizeBytes / 1024;
                }
                return String.Format("{0:0.##} {1}", sizeBytes, sizes[order]);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetFormatFileSize", ex);
#endif
            }
            return "";
        }

        /// <summary>
        /// Screensdawdaw.jpg;#10:14 AM -> bỏ ;#10:14 AM
        /// </summary>
        /// <returns></returns>
        public static string GetFormatTitleFile(string title)
        {
            try
            {
                string[] titleArray = title.Split(";#");

                if (titleArray.Length > 1)
                    return titleArray[0];
                else
                    return title;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetFormatFileSize", ex);
#endif
            }
            return title;
        }

        /// <summary>
        /// return theo thứ tự Row Index - element Index. Nếu return ra -1 là ko có
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="_lstRows"></param>
        /// <returns></returns>
        public static Tuple<int, int> Find_RowIndex_ElementIndex_ListControl(ViewElement _element, List<ViewRow> _lstRows)
        {
            Tuple<int, int> _result = new Tuple<int, int>(-1, -1);
            try
            {
                int _posRow = -1, _posElement = -1;
                for (int i = 0; i < _lstRows.Count; i++)
                {
                    if (_lstRows[i].Elements != null && _lstRows[i].Elements.Count > 0)
                        for (int j = 0; j < _lstRows[i].Elements.Count; j++)
                        {
                            if (_lstRows[i].Elements[j].ID.Equals(_element.ID))
                            {
                                _posRow = i;
                                _posElement = j;
                                break;
                            }
                        }
                }
                _result = new Tuple<int, int>(_posRow, _posElement);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - Find_RowIndex_ElementIndex_ListControl - Error: " + ex.Message);
#endif
            }
            return _result;
        }

        public static int FindIndexOfItemInListAttach(BeanAttachFile _searchItem, List<BeanAttachFile> _lstAttach)
        {
            int _result = -1;
            try
            {
                for (int i = 0; i < _lstAttach.Count; i++)
                {
                    if (_lstAttach[i].ID.Equals(_searchItem.ID) &&
                        _lstAttach[i].Path == _searchItem.Path &&
                        _lstAttach[i].Title == _searchItem.Title &&
                        _lstAttach[i].IsAuthor == _searchItem.IsAuthor)
                    {
                        _result = i;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentCreateWorkflow - Find_RowIndex_ElementIndex_ListControl - Error: " + ex.Message);
#endif
            }
            return _result;
        }

        public static string DownloadAndGetPathFile(Activity _mainAct, Context _context, string link)
        {
            try
            {
                string pdfFilePath = System.IO.Path.Combine(CmmVariable.M_DataFolder + "/", System.IO.Path.GetFileName(link) ?? throw new InvalidOperationException());
                ProviderUser pUser = new ProviderUser();
                bool result;

                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                
                    result = pUser.DownloadFile(link, pdfFilePath, CmmVariable.M_AuthenticatedHttpClient);
                    if (result)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                           
                        });
                        return pdfFilePath;
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                               CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                        return null;

                    }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentDetailWorkflow - Error: " + ex.Message);
#endif
                return null;
            }
        }
        public static async void DownloadAndOpenFile(Activity _mainAct, Context _context, string link)
        {
            try
            {
                string pdfFilePath = System.IO.Path.Combine(CmmVariable.M_DataFolder + "/", System.IO.Path.GetFileName(link) ?? throw new InvalidOperationException());
                ProviderUser pUser = new ProviderUser();
                bool result;

                if (System.IO.File.Exists(pdfFilePath)) // Có file rồi -> mở lên
                {
                    OpenFile(_mainAct, _context, pdfFilePath);
                    return;
                }

                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    result = pUser.DownloadFile(link, pdfFilePath, CmmVariable.M_AuthenticatedHttpClient);
                    if (result)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            OpenFile(_mainAct, _context, pdfFilePath);
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                               CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                    }
                });

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentDetailWorkflow - Error: " + ex.Message);
#endif
            }
        }

        public static void OpenFile(Activity _mainAct, Context _context, string localpath)
        {
            try
            {
                Android.Net.Uri _mUri;
                Java.IO.File file = new Java.IO.File(localpath);

                //file.SetReadable(true);
                _mUri = Android.Support.V4.Content.FileProvider.GetUriForFile(_context, CmmDroidVariable.M_PackageProvider, file);
                _context.RevokeUriPermission(_mUri, ActivityFlags.GrantReadUriPermission);
                //Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
                string extension = System.IO.Path.GetExtension(localpath);
                string application = "";
                if (extension != null)
                    switch (extension.ToLower())
                    {
                        case ".doc":
                        case ".docx":
                            application = "application/msword";
                            break;
                        case ".pdf":
                            application = "application/pdf";
                            break;
                        case ".xls":
                        case ".xlsx":
                            application = "application/vnd.ms-excel";
                            break;
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            application = "image/*";
                            break;
                        default:
                            application = "*/*";
                            break;
                    }

                Intent intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetDataAndType(_mUri, application);
                _context.StartActivity(intent);

            }
            catch (Exception)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    Toast.MakeText(_mainAct, "Bạn không có ứng dụng có thể mở loại tệp này.", ToastLength.Long).Show();
                });
            }
        }

        public static string FormatHTMLToString(string _htmlString)
        {
            string _res = "";
            try
            {
                //_res = Html.FromHtml(_htmlString.Replace("\n", "<br/>")).ToString();
                _res = HtmlCompat.FromHtml(_htmlString.Replace("\n", "<br/>"), HtmlCompat.FromHtmlModeLegacy).ToString();
            }
            catch (Exception)
            {
                _res = _htmlString;
            }
            return _res;
        }

        public static bool CheckTextViewIsEllipsized(TextView _textView)
        {
            try
            {
                // Check if the supplied TextView is not null
                if (_textView == null)
                    return false;

                // Check if ellipsizing the text is enabled
                TextUtils.TruncateAt truncateAt = _textView.Ellipsize;
                if (truncateAt == null || TextUtils.TruncateAt.Marquee.Equals(truncateAt))
                    return false;

                // Retrieve the layout in which the text is rendered
                Layout layout = _textView.Layout;
                if (layout == null)
                    return false;

                // Iterate all lines to search for ellipsized text
                for (int line = 0; line < layout.LineCount; ++line)
                {
                    // Check if characters have been ellipsized away within this line of text
                    if (layout.GetEllipsisCount(line) > 0)
                        return true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetFormatFileSize", ex);
#endif
            }
            return false;
        }

        public static string GetApplicationName(Context _context)
        {
            try
            {
                return _context.ApplicationInfo.LoadLabel(_context.PackageManager);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetApplicationName", ex);
#endif
            }
            return "";
        }

        /// <summary>
        /// Tô màu nền chữ lên
        /// </summary>
        /// <param name="editText"></param>
        /// <param name="mainString"></param>
        /// <param name="subString"></param>
        public static void HightLightTextSpecific(EditText editText, string mainString, string subString, string color = "#ff0000")
        {
            try
            {
                if (mainString.Contains(subString))
                {
                    int startIndex = mainString.IndexOf(subString);
                    int endIndex = startIndex + subString.Length;
                    SpannableString spannableString = new SpannableString(mainString);
                    spannableString.SetSpan(new Android.Text.Style.BackgroundColorSpan(Color.ParseColor(color)), startIndex, endIndex,
                    SpanTypes.ExclusiveExclusive);
                    editText.SetText(spannableString, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetApplicationName", ex);
#endif
            }
        }

        /// <summary>
        /// Load Image vào ImageView bởi URL
        /// </summary>
        /// <param name="_img"></param>
        /// <param name="_imgURL"></param>
        public static void LoadImageUniversalURL(Activity _mainAct, Context _context, ImageView _img, string _imgURL)
        {
            try
            {
                //CircularProgressDrawable circularProgressDrawable = new CircularProgressDrawable(_context);
                //circularProgressDrawable.StrokeWidth = 5f;
                //circularProgressDrawable.BackgroundColor = Color.Red;

                //circularProgressDrawable.CenterRadius = 30f;
                //circularProgressDrawable.Start();
                //Glide.With(mainAct).Load(imgURL).Apply(new RequestOptions().Override(200, 200).Placeholder(circularProgressDrawable).Error(Resource.Drawable.icon_avatar64).InvokeDiskCacheStrategy(Com.Bumptech.Glide.Load.Engine.DiskCacheStrategy.All)).Into(imgView);

                if (_imageLoader == null)
                {
                    ImageLoaderConfiguration config = new ImageLoaderConfiguration.Builder(_mainAct)
                                                    .ThreadPoolSize(3)
                                                    .DiskCacheExtraOptions(100, 100, null)
                                                    .Build();
                    ImageLoader.Instance.Init(config);
                    _imageLoader = ImageLoader.Instance;
                }

                DisplayImageOptions options = new DisplayImageOptions.Builder()
                                            .CacheInMemory(true)
                                            .CacheOnDisk(true)
                                            .BitmapConfig(Bitmap.Config.Rgb565)
                                            .ImageScaleType(ImageScaleType.Exactly)
                                            .ShowImageForEmptyUri(Resource.Drawable.img_ver3_error)
                                            .ShowImageOnLoading(Resource.Drawable.img_ver3_loading)
                                            .ShowImageOnFail(Resource.Drawable.img_ver3_error)
                                            .Build();

                _imageLoader.DisplayImage(_imgURL, _img, options);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "GetApplicationName", ex);
#endif
            }
        }
        /// <summary>
        /// Phân loại ra xem cái nào là image -> IsImage = true
        /// </summary>

        public static List<BeanAttachFile> ClassifyListAttachFile(List<BeanAttachFile> _lstAttachFile)
        {
            try
            {
                if (_lstAttachFile != null && _lstAttachFile.Count > 0)
                {
                    string[] lstExtend = new string[] { ".png", ".jpeg", ".jpg" };
                    for (int i = 0; i < _lstAttachFile.Count; i++)
                    {
                        // Check Path
                        string path = (!String.IsNullOrEmpty(_lstAttachFile[i].Path) ? _lstAttachFile[i].Path : "").ToLowerInvariant();
                        foreach (string ext in lstExtend)
                        {
                            if (path.Contains(ext))
                            {
                                _lstAttachFile[i].IsImage = true;
                                break;
                            }
                        }

                        // Nếu Path chưa có -> check URL
                        string url = (!String.IsNullOrEmpty(_lstAttachFile[i].Url) ? _lstAttachFile[i].Url : "").ToLowerInvariant();
                        foreach (string ext in lstExtend)
                        {
                            if (url.Contains(ext))
                            {
                                _lstAttachFile[i].IsImage = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("CmmDroidFunction", "ClassifyListAttachFile", ex);
#endif
            }
            return _lstAttachFile;
        }

        public static void LogErrToSDCard(String messagebody)
        {
            string folderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
            string filePath = System.IO.Path.Combine(folderPath, "logBPM.txt");
            try
            {
                if (!File.Exists(filePath))
                {
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine("============================" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ============================");
                        sw.WriteLine(messagebody);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("============================" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ============================");
                        sw.WriteLine(messagebody);
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Trả ra string dateTime theo format
        /// </summary>
        /// <param name="millis"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDateTimeFromMillis(long millis, string format)
        {
            string _result = "";
            try
            {
                Android.Icu.Text.SimpleDateFormat simpleDateFormat = new Android.Icu.Text.SimpleDateFormat(format);
                _result = simpleDateFormat.Format(millis);
            }
            catch (Exception ex)
            {

            }
            return _result;
        }

        public static long GetMiliFromDateTimeString(string date)
        {
            SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yy");
            try
            {
                Java.Util.Date d = sdf.Parse(date);
                long l = d.Time;
                return l;
            }
            catch (System.Exception ex)
            {
#if DEBUG
                LogErr(ex);
#endif
                return 0;
            }
        }
    }
}
