using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Util;
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
using Com.Telerik.Widget.Calendar;
using Com.Telerik.Widget.Calendar.Events;
using Newtonsoft.Json;
using Refractored.Controls;
using static Android.App.DatePickerDialog;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentHomePage : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private CircleImageView _imgAvata;
        private TextView _tvTrangChuTitle;
        private ImageView _imgCreateWorkflow, _imgFilter;
        public LinearLayout _lnToolbar, _lnFilter, _lnContent, _lnBlackFilter, _lnDisablePager, _lnBottomNavigation;
        public TextView _tvVDT, _tvVTBD;
        private Class.MyCustomViewPager _pagerListTask;
        private AdapterViewPagerHomePage _mViewPagerHomeAdapter;
        private SwipeRefreshLayout _swipe;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        public int _flagIsFiltering_VDT = 0;
        public int _flagIsFiltering_VTBD = 0;
        public int _flagCurrentTask = 1; // 1 = việc đến tôi, 2 = việc tôi bắt đầu
        private static FragmentHomePage fragment;
        public static CustomBaseFragment newInstance()
        {
            fragment = new FragmentHomePage();
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewHomePage, null);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewHomePage_Avata);
                _tvVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VDT);
                _tvVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VTBD);
                _tvTrangChuTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_TrangChu);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Filter);
                _lnToolbar = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Toolbar);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Content);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_BlackFilter);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_BottomNavigation);
                _pagerListTask = _rootView.FindViewById<MyCustomViewPager>(Resource.Id.pager_ViewHomePage);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_DisablePager);
                _imgCreateWorkflow = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewHomePage_CreateWorkflow);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewHomePage_Filter);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewHomePage);

                _swipe.Enabled = true;
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                _pagerListTask.VerticalFadingEdgeEnabled = true;
                SetView();
                _pagerListTask.ScrollChange += ScrollChange_pagerListTask;
                _pagerListTask.PageScrolled += PageScrolled_pagerListTask;
                _tvTrangChuTitle.Click += Click_Menu;
                _imgAvata.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _tvVDT.Click += Click_tvVDT;
                _tvVTBD.Click += Click_tvVTBD;
                _lnFilter.Click += Click_lnFilter;
                _imgCreateWorkflow.Click += Click_imgCreateWorkflow;
                _lnDisablePager.Click += (sender, e) => { };

                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                SetViewPager();
                SetData();

                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation);
                bottomNavigation.InitializeView();

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
                    CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                    SetData();
                    SetView();
                    SetLinearFilter_ByFlag(0);

                    try // Set lại dữ liệu cho Pager
                    {
                        List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerHomePageSingleList).Name);
                        foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                        {
                            PagerHomePageSingleList _pager = (PagerHomePageSingleList)temp;
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

            if (!String.IsNullOrEmpty(MainActivity._notificationID)) // Nếu có notification 
            {
                MainActivity._notificationID = "";
                FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(null, null, "FragmentHomePage");
                _mainAct.AddFragment(_mainAct.SupportFragmentManager, detailWorkFlow, "FragmentHomePage", 0);
            }

            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.HomePage; // Home
            MinionAction.RefreshFragmentHomePage += EventHandler_RefreshFragmentHomePage;
            MinionAction.MoveToHeadHomePage += MinionAction_MoveToHeadHomePage;
            CmmEvent.UpdateLangComplete += EventHandler_UpdateLanguage;
            return _rootView;
        }

        private void MinionAction_MoveToHeadHomePage(object sender, EventArgs e)
        {
            Action action = new Action(() =>
            {
                SetView();
                SetViewPager();
                SetData();
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            MinionAction.RefreshFragmentHomePage -= EventHandler_RefreshFragmentHomePage;
        }

        public FragmentHomePage()
        {

        }

        #region Event
        public void SetView()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvVDT.Text = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi") + " " + CmmDroidFunction.GetCountNumOfText(_tvVDT.Text);
                    _tvVTBD.Text = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu") + " " + CmmDroidFunction.GetCountNumOfText(_tvVTBD.Text);
                    _tvTrangChuTitle.Text = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ");
                }
                else
                {
                    _tvVDT.Text = CmmFunction.GetTitle("TEXT_TOME", "To me") + " " + CmmDroidFunction.GetCountNumOfText(_tvVDT.Text);
                    _tvVTBD.Text = CmmFunction.GetTitle("TEXT_FROMME", "From me") + " " + CmmDroidFunction.GetCountNumOfText(_tvVTBD.Text);
                    _tvTrangChuTitle.Text = CmmFunction.GetTitle("TEXT_MAINVIEW", "Homepage");
                }
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
                _imgAvata.Enabled = flag;
                _tvVDT.Enabled = flag;
                _tvVTBD.Enabled = flag;
                _lnFilter.Enabled = flag;
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

        private void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.HomePage; // Home
                    MinionAction.OnRenewDataAndShowFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Menu", ex);
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

        private void Click_lnFilter(object sender, EventArgs e)
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
                            SharedView_PopupFilterVDT sharedView_PopupFilterVDT = new SharedView_PopupFilterVDT(_layoutInflater, _mainAct, this, "FragmentHomePage", _rootView);
                            sharedView_PopupFilterVDT.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVDT.FlagViewFilterVDT.HomePage);
                            sharedView_PopupFilterVDT.InitializeView();
                        }
                        else // Việc tôi bắt đầu
                        {
                            SharedView_PopupFilterVTBD sharedView_PopupFilterVTBD = new SharedView_PopupFilterVTBD(_layoutInflater, _mainAct, this, "FragmentHomePage", _rootView);
                            sharedView_PopupFilterVTBD.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVTBD.FlagViewFilterVTBD.HomePage);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
            }
        }

        public async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                SetViewTouchable(false);
                await Task.Run(async () =>
                {
                    ProviderUser pUser = new ProviderUser();

                    await pUser.UpdateAllDynamicDataAndroid(true);
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
                            CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);

                            if (_flagIsFiltering_VDT == 0 && _flagIsFiltering_VTBD == 0) // 2 list đều ko filter -> set bình thường
                            {
                                SetView();
                                //SetData(); // nếu đang filter -> ko set local lại
                                SetViewPager();
                            }
                            else // có list call API
                            {
                                SetView();
                                //SetData(_flagIsFiltering_VDT == 1 ? false : true, _flagIsFiltering_VTBD == 1 ? false : true); // nếu đang filter -> ko set local lại
                                List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerHomePageSingleList).Name);
                                foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                                {
                                    PagerHomePageSingleList _pager = (PagerHomePageSingleList)temp;

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

                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, null, null);
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
        public async void SetData(bool setCountVDT = true, bool setCountVTBD = true)
        {
            await Task.Run(() =>
            {

                try
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    string CountVDTTBD = p_dynamic.GetListCountVDT_VTBD(String.Format("{0}|{1}", CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS, CmmVariable.KEY_COUNT_FROMME_INPROCESS));
                    List<string> CountVDTTBD_split = CountVDTTBD.Split("|").ToList();
                    #region Count VDT
                    if (setCountVDT)
                    {
                        /*string _queryVDTCount = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(CTRLHomePage.LstFilterCondition_VDT, false, true);
                        List<CountNum> _lstVDTCount = conn.Query<CountNum>(_queryVDTCount);

                        if (_lstVDTCount != null && _lstVDTCount.Count > 0)*/
                        int _lstVDTCount = Convert.ToInt32(CountVDTTBD_split.Where(x => x.Contains(CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS)).First().Split(";#").Last());
                        if (_lstVDTCount != 0 && _lstVDTCount != null)
                            _mainAct.RunOnUiThread(() =>
                            {
                                CTRLHomePage.SetTextview_FormatItemCount(_tvVDT, _lstVDTCount, "VDT");
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, "(", ")");
                            });

                    }
                    #endregion

                    #region Count VTBD
                    if (setCountVTBD)
                    {
                        /*string _queryVTBDCount = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(CTRLHomePage.LstFilterCondition_VTBD, false, true);
                        List<CountNum> _lstVTBDCount = conn.Query<CountNum>(_queryVTBDCount);*/
                        int _lstVTBDCount = Convert.ToInt32(CountVDTTBD_split.Where(x => x.Contains(CmmVariable.KEY_COUNT_FROMME_INPROCESS)).First().Split(";#").Last());
                        if (_lstVTBDCount != null && _lstVTBDCount != 0)
                            _mainAct.RunOnUiThread(() =>
                            {
                                CTRLHomePage.SetTextview_FormatItemCount(_tvVTBD, _lstVTBDCount, "VTBD");
                            });

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
                }
            });
        }

        public async void SetViewPager()
        {

            try
            {
                List<string> _lstService = new List<string>();
                _lstService.Add("VDT");
                _lstService.Add("VTBD");
                List<List<KeyValuePair<string, string>>> _lstFilterCondition = new List<List<KeyValuePair<string, string>>>();
                _lstFilterCondition.Add(CTRLHomePage.LstFilterCondition_VDT);
                _lstFilterCondition.Add(CTRLHomePage.LstFilterCondition_VTBD);

                //_mViewPagerHomeAdapter = new AdapterViewPagerHomePage(_mainAct.SupportFragmentManager, this, _lstService, _lstFilterCondition);
                //_pagerListTask.OffscreenPageLimit = 2;
                _mainAct.RunOnUiThread(() =>
                {
                    _mViewPagerHomeAdapter = new AdapterViewPagerHomePage(_mainAct.SupportFragmentManager, this, _lstService, _lstFilterCondition, _isFollowScreen: false);
                    _pagerListTask.SaveEnabled = true; // save lại trạng thái trc đó
                        _pagerListTask.Adapter = _mViewPagerHomeAdapter;
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewPager", ex);
#endif
            }
        }

        public async void EventHandler_RefreshFragmentHomePage(object arg1, EventArgs arg2)
        {
            try
            {
                //Swipe_RefreshData(null, null);
                ProviderBase p_base= new ProviderBase();
                await p_base.UpdateAllDynamicDataAndroid(true);
                _mainAct.RunOnUiThread(() =>
                {
                    CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                    SetData();
                    
                    SetView();
                    SetViewPager();
                    MinionAction.OnRefreshFragmentViewPager(null, null);
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
                    SetView(); // Handle parent View

                    List<Android.Support.V4.App.Fragment> _listFragment = ((MainActivity)_mainAct).FindListFragmentByName(typeof(PagerHomePageSingleList).Name);
                    foreach (Android.Support.V4.App.Fragment temp in _listFragment)
                    {
                        PagerHomePageSingleList _pager = (PagerHomePageSingleList)temp;
                        if (_pager._type.ToLowerInvariant().Equals("vdt") && _pager.adapterHomePageRecyVDT != null)
                            _pager.adapterHomePageRecyVDT.NotifyDataSetChanged();
                        if (_pager._type.ToLowerInvariant().Equals("vtbd") && _pager.adapterHomePageRecyVTBD != null)
                            _pager.adapterHomePageRecyVTBD.NotifyDataSetChanged();
                    }
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