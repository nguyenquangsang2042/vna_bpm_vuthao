using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
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
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppHomePage : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        public TextView _tvTrangChuTitle, _tvVDT, _tvVTBD;
        private ImageView  _imgFilter, _imgBack;
        public LinearLayout _lnToolbar, _lnContent, _lnDisablePager, _lnBottomNavigation;
        private Class.MyCustomViewPager _pagerListTask;
        private AdapterViewPagerHomePage _mViewPagerHomeAdapter;
        private SwipeRefreshLayout _swipe;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public int _flagIsFiltering_VDT = 0;
        public int _flagIsFiltering_VTBD = 0;
        public int _flagCurrentTask = 1; // 1 = việc đến tôi, 2 = việc tôi bắt đầu

        public BeanWorkflow _currentWorkflow;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewChildAppHomePage, null);
                _tvVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppHomePage_VDT);
                _tvVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppHomePage_VTBD);
                _tvTrangChuTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppHomePage_TrangChu);
                _lnToolbar = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppHomePage_Toolbar);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppHomePage_Content);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppHomePage_BottomNavigation);
                _pagerListTask = _rootView.FindViewById<MyCustomViewPager>(Resource.Id.pager_ViewChildAppHomePage);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppHomePage_DisablePager);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppHomePage_Back);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppHomePage_Filter);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewChildAppHomePage);

                _swipe.Enabled = true;
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                _pagerListTask.VerticalFadingEdgeEnabled = true;

                _pagerListTask.ScrollChange += ScrollChange_pagerListTask;
                _pagerListTask.PageScrolled += PageScrolled_pagerListTask;
                _swipe.Refresh += Swipe_RefreshData;
                _tvVDT.Click += Click_tvVDT;
                _tvVTBD.Click += Click_tvVTBD;
                _imgFilter.Click += Click_imgFilter;
                _imgBack.Click += Click_imgBack;

                _lnDisablePager.Click += (sender, e) => { };

                SetViewPager();
                SetData();
                SetView();

                SetViewTouchable(true); // enable cho bấm tào lao
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    SetViewTouchable(false);

                    _flagIsFiltering_VDT = 0;
                    _flagIsFiltering_VTBD = 0;
                    CTRLHomePage = new ControllerHomePage();
                    SetData();
                    SetView();
                    SetLinearFilter_ByFlag(0);

                    try // Set lại dữ liệu cho Pager
                    {
                        List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerChildAppHomePageSingleList).Name);
                        foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                        {
                            PagerChildAppHomePageSingleList _pager = (PagerChildAppHomePageSingleList)temp;
                            if (_pager._type.ToLowerInvariant().Equals("vdt"))
                                _pager._lstFilterCondition = CTRLHomePage.LstFilterCondition_VDT;
                            else
                                _pager._lstFilterCondition = CTRLHomePage.LstFilterCondition_VTBD;
                        }
                        MinionAction.OnRefreshFragmentViewPager(null, null);
                    }
                    catch (Exception ex)
                    {

                    }

                    //Click_tvVDT(null, null);

                    Action action = new Action(() =>
                    {
                        _pagerListTask.SetCurrentItem(0, false); // false là ko có animation
                        MainActivity.FlagRefreshDataFragment = false; // Đặt ở cuối vì để triggger OnRefreshFragmentViewPager() trước
                        SetViewTouchable(true);
                    });
                    new Handler().PostDelayed(action, 1 /*CmmDroidVariable.M_ActionDelayTime + 350*/);
                }
            }
            // Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppHomePage; // Home
            MinionAction.RefreshFragmentHomePage += EventHandler_RefreshFragmentHomePage;
            CmmEvent.UpdateLangComplete += EventHandler_UpdateLanguage;
            return _rootView;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            MinionAction.RefreshFragmentHomePage -= EventHandler_RefreshFragmentHomePage;
        }

        public FragmentChildAppHomePage(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }

        #region Event
        public void SetView()
        {
            try
            {
                _tvVDT.Text = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi") + " " + CmmDroidFunction.GetCountNumOfText(_tvVDT.Text);
                _tvVTBD.Text = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu") + " " + CmmDroidFunction.GetCountNumOfText(_tvVTBD.Text);
                _tvTrangChuTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _currentWorkflow.Title : _currentWorkflow.TitleEN;

                // Set màu cho Text
                if (_flagCurrentTask == 1) // VDT
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, "(", ")");
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, null, null);
                }
                else // VTBD
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, null, null);
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, "(", ")");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView", ex);
#endif
            }
        }

        private void SetViewTouchable(bool flag)
        {
            try
            {
                _tvTrangChuTitle.Enabled = flag;
                _tvVDT.Enabled = flag;
                _tvVTBD.Enabled = flag;
                _pagerListTask.SetViewPagerTouchable(flag);
                // Disable click Viewpager
                if (flag == true)
                    _lnDisablePager.Visibility = ViewStates.Gone;
                else
                    _lnDisablePager.Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewTouchable", ex);
#endif
            }
        }

        public void SetLinearFilter_ByFlag(int flag)
        {
            try
            {
                if (flag == 1) // đang lọc
                {
                    _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGreenDueDate)));
                }
                else // ko phải đang lọc
                {
                    _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetLinearFilter_ByFlag", ex);
#endif
            }
        }

        private void Click_imgCreateWorkflow(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    FragmentCreateWorkflow fragmentCreateWorkflow = new FragmentCreateWorkflow();
                    _mainAct.ShowFragment(FragmentManager, fragmentCreateWorkflow, "FragmentCreateWorkflow", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_CreateWorkflow", ex);
#endif
            }
        }

        private void Click_tvVDT(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(100) == true)
                {
                    _flagCurrentTask = 1;
                    _pagerListTask.CurrentItem = 0;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvVDT", ex);
#endif
            }
        }

        public void Click_tvVTBD(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(100) == true)
                {
                    _flagCurrentTask = 2;
                    _pagerListTask.CurrentItem = 1;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvVTBD", ex);
#endif
            }
        }

        private void Click_imgFilter(object sender, EventArgs e)
        {
            try
            {
                SetViewTouchable(false);
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    SetLinearFilter_ByFlag(1); // click vào là xanh lá
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    Action action = new Action(() =>
                    {
                        _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                        if (_flagCurrentTask == 1) // Việc đến tôi
                        {
                            SharedView_PopupFilterVDT sharedView_PopupFilterVDT = new SharedView_PopupFilterVDT(_layoutInflater, _mainAct, this, this.GetType().Name, _rootView);
                            sharedView_PopupFilterVDT.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVDT.FlagViewFilterVDT.ChildAppHomePage);
                            sharedView_PopupFilterVDT.InitializeView();
                        }
                        else // Việc tôi bắt đầu
                        {
                            SharedView_PopupFilterVTBD sharedView_PopupFilterVTBD = new SharedView_PopupFilterVTBD(_layoutInflater, _mainAct, this, this.GetType().Name, _rootView);
                            sharedView_PopupFilterVTBD.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVTBD.FlagViewFilterVTBD.ChildAppHomePage);
                            sharedView_PopupFilterVTBD.InitializeView();
                        }

                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 300); // ko cần ẩn bàn phím nên giảm time đi
                }
                SetViewTouchable(true);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFilter", ex);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board; // luôn Back về Board
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        public async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                SetViewTouchable(false);
                await Task.Run(() =>
                {
                    ProviderUser pUser = new ProviderUser();
                    pUser.UpdateAllMasterData(true);
                    pUser.UpdateAllDynamicData(true);

                    string _preValueLang = CmmVariable.SysConfig.LangCode;
                    pUser.UpdateCurrentUserInfo(CmmVariable.M_Avatar);

                    // Check xem có bị thay đổi giá trị LangCode không
                    if (!_preValueLang.Equals(CmmVariable.SysConfig.LangCode))
                    {
                        pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    _mainAct.RunOnUiThread(() =>
                    {
                        _swipe.Refreshing = false;
                        Action action = new Action(() =>
                        {
                            CmmDroidFunction.ShowVibrateEvent(0.2);
                            if (_flagIsFiltering_VDT == 0 && _flagIsFiltering_VTBD == 0) // 2 list đều ko filter -> set bình thường
                            {
                                SetData();
                                SetView();
                                SetViewPager();
                            }
                            else
                            {
                                SetView();
                                SetData(_flagIsFiltering_VDT == 1 ? false : true, _flagIsFiltering_VTBD == 1 ? false : true); // nếu đang filter -> ko set local lại
                                List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerChildAppHomePageSingleList).Name);
                                foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                                {
                                    PagerChildAppHomePageSingleList _pager = (PagerChildAppHomePageSingleList)temp;

                                    if (_pager._type.ToLowerInvariant().Equals("vdt"))
                                        _pager.SetData();
                                    else if (_pager._type.ToLowerInvariant().Equals("vtbd"))
                                        _pager.SetData();
                                }
                            }

                            if (_flagCurrentTask == 2)
                            {
                                Click_tvVTBD(null, null); // focus lại trang VTBD
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, null, null);
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, "(", ")");
                            }
                            else
                            {
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, "(", ")");
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, null, null);
                            }

                            SetViewTouchable(true);
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
                    SetViewTouchable(true);
                });
            }
        }

        private void PageScrolled_pagerListTask(object sender, ViewPager.PageScrolledEventArgs e)
        {
            try
            {
                int _index = e.Position;
                if (_index == 0) // VDT
                {
                    _flagCurrentTask = 1;
                    CTRLHomePage.SetTextview_Selected(_mainAct, _tvVDT);
                    CTRLHomePage.SetTextview_NotSelected(_mainAct, _tvVTBD);
                    SetLinearFilter_ByFlag(_flagIsFiltering_VDT);
                    if (_tvVDT.Text.Contains("("))
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, "(", ")");
                    else
                    {
                        ISpannable spannable = new SpannableString(_tvVDT.Text.Trim());
                        ColorStateList White = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)) });
                        TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, White, null);
                        spannable.SetSpan(highlightSpan, 0, _tvVDT.Text.Length - 1, SpanTypes.ExclusiveExclusive);
                        _tvVDT.SetText(spannable, TextView.BufferType.Spannable);
                    }
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, null, null);
                }
                else if (_index == 1) // VTBD
                {
                    _flagCurrentTask = 2;
                    CTRLHomePage.SetTextview_NotSelected(_mainAct, _tvVDT);
                    CTRLHomePage.SetTextview_Selected(_mainAct, _tvVTBD);
                    SetLinearFilter_ByFlag(_flagIsFiltering_VTBD);
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, null, null);
                    if (_tvVTBD.Text.Contains("("))
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, "(", ")");
                    else
                    {
                        ISpannable spannable = new SpannableString(_tvVTBD.Text.Trim());
                        ColorStateList White = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)) });
                        TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, White, null);
                        spannable.SetSpan(highlightSpan, 0, _tvVTBD.Text.Length - 1, SpanTypes.ExclusiveExclusive);
                        _tvVTBD.SetText(spannable, TextView.BufferType.Spannable);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ChangePagerListener", ex);
#endif
            }
        }

        private void ScrollChange_pagerListTask(object sender, View.ScrollChangeEventArgs e)
        {
            if ((e.ScrollX > e.ScrollY) && (e.ScrollX < e.V.Width))
            {
                _swipe.Enabled = false;
            }
            else
            {
                _swipe.Enabled = true;
            }
        }
        #endregion

        #region Data
        public void SetData(bool setCountVDT = true, bool setCountVTBD = true)
        {
            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

            try
            {
                #region Count VDT
                if (setCountVDT)
                {
                    for (int i = 0; i < CTRLHomePage.LstFilterCondition_VDT.Count; i++) // Chỉ hiện đang xử lý
                    {
                        if (CTRLHomePage.LstFilterCondition_VDT[i].Key.Equals("Tình trạng"))
                        {
                            CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Tình trạng", "1");
                            break;
                        }
                    }

                    string _queryVDTCount = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(CTRLHomePage.LstFilterCondition_VDT, false, true, _workflowID: _currentWorkflow.WorkflowID);
                    List<CountNum> _lstVDTCount = conn.Query<CountNum>(_queryVDTCount);

                    if (_lstVDTCount != null && _lstVDTCount.Count > 0)
                        CTRLHomePage.SetTextview_FormatItemCount(_tvVDT, _lstVDTCount[0].Count, "VDT");
                    else
                        _tvVDT.Text = "";
                }
                #endregion

                #region Count VTBD
                if (setCountVTBD)
                {
                    string _queryVTBDCount = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(CTRLHomePage.LstFilterCondition_VTBD, false, true, _workflowID: _currentWorkflow.WorkflowID);
                    List<CountNum> _lstVTBDCount = conn.Query<CountNum>(_queryVTBDCount);

                    if (_lstVTBDCount != null && _lstVTBDCount.Count > 0)
                        CTRLHomePage.SetTextview_FormatItemCount(_tvVTBD, _lstVTBDCount[0].Count, "VTBD");
                    else
                        _tvVTBD.Text = "";
                }
                #endregion

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public void SetViewPager()
        {
            try
            {
                List<string> _lstService = new List<string>();
                _lstService.Add("VDT");
                _lstService.Add("VTBD");
                List<List<KeyValuePair<string, string>>> _lstFilterCondition = new List<List<KeyValuePair<string, string>>>();
                _lstFilterCondition.Add(CTRLHomePage.LstFilterCondition_VDT);
                _lstFilterCondition.Add(CTRLHomePage.LstFilterCondition_VTBD);

                _mViewPagerHomeAdapter = new AdapterViewPagerHomePage(_mainAct.SupportFragmentManager, this, _lstService, _lstFilterCondition, IsChildApp: true);
                //_pagerListTask.OffscreenPageLimit = 2;
                _pagerListTask.SaveEnabled = true; // save lại trạng thái trc đó
                _pagerListTask.Adapter = _mViewPagerHomeAdapter;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewPager", ex);
#endif
            }
        }

        public void EventHandler_RefreshFragmentHomePage(object arg1, EventArgs arg2)
        {
            try
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetData();
                    SetView();
                    SetViewPager();
                    //MinionAction.OnRefreshFragmentViewPager(null, null);
                    if (_flagCurrentTask == 2)
                    {
                        Click_tvVTBD(null, null); // focus lại trang VTBD
                    }
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "RefreshFragmentHomePage", ex);
#endif
            }
        }

        private void EventHandler_UpdateLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetView();
                    //MinionAction.OnRefreshFragmentViewPager(null, null);
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ChangeLanguage", ex);
#endif
            }
        }
        #endregion
    }
}