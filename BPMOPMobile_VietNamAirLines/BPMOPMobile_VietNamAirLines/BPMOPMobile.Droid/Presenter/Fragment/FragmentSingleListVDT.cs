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
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
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
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.SharedView;
using Com.Telerik.Widget.Calendar;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    public class FragmentSingleListVDT : CustomBaseFragment
    {

        public MainActivity _mainAct;
        public View _rootView;
        public ControllerHomePage CTRLHomePage = new ControllerHomePage();

        public List<BeanAppBaseExt> _lstAppBaseVDT = new List<BeanAppBaseExt>();

        public SwipeRefreshLayout _swipe;
        public RelativeLayout _relaToolbar;
        public CircleImageView _imgAvata;
        public TextView _tvName, _tvNoData, _tvTabDangXuLy, _tvTabDaXuLy;
        public ImageView _imgFilter, _imgDeleteSearch, _imgShowSearch;
        public EditText _edtSearch;
        public RecyclerView _recyData;
        public LinearLayout _lnAll, _lnFilter, _lnNoData, _lnContent, _lnBlackFilter, _lnDisablePager, _lnBottomNavigation, _lnSearch, _lnTab, _lnRecycleHaveData;
        public AdapterHomePageRecyVDT_Ver2 _adapterHomePageRecyVDT;
        private LinearLayout _lnRecycle;
        // Variable Flag
        public string _queryVDT = "";
        public int _flagIsFiltering = 0;
        public bool _allowLoadMore = true;
        public bool _isLocalDataLoading = true; // nếu = false là call API từ Server
        public bool _isShowDialog = false; // nếu = false là không hiện khi call API từ Server
        public int _flagCurrentTab = (int)ControllerHomePage.FlagStateFilterVDT.InProcess;
        private bool fromSetData = false;
        private ControllerBase controllerBase = new ControllerBase();
        public bool _IsInProcess;
        private static FragmentSingleListVDT fragment;
        private SharedView_PopupFilterVDT sharedView_PopupFilterVDT;
        public static CustomBaseFragment NewInstance()
        {
            fragment = new FragmentSingleListVDT();
            return fragment;
        }
        #region Life Cycle

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            MinionAction.RenewFragmentSingleVDT -= RenewData;
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra
            CTRLHomePage.InitListFilterCondition("BOTH");
            base.OnDestroyView();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewListWorkflow, null);

                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_All);
                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewListWorkflow_Toolbar);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewListWorkflow_Avata);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Name);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Filter);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Filter);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ListWorkflowView_Search_Delete);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ListWorkflowView_Search);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewListWorkflow);
                //_lvData = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewListWorkflow);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_NoData);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Content);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_NoData);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_BottomNavigation);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_DisablePager);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_BlackFilter);
                _lnRecycleHaveData = _rootView.FindViewById<LinearLayout>(Resource.Id.lnRecycleHaveData);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewListWorkflow);
                _lnRecycle = _rootView.FindViewById<LinearLayout>(Resource.Id.lnRecycle);

                // chỉ VDT mới có
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_ShowSearch);
                _lnTab = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Tab);
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Search);
                _tvTabDangXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_TabDangXuLy);
                _tvTabDaXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_TabDaXuLy);

                _lnTab.Visibility = ViewStates.Visible;
                _lnSearch.Visibility = ViewStates.Gone;
                _imgShowSearch.Visibility = ViewStates.Visible;

                _imgShowSearch.Click += Click_imgShowSearch;
                _tvTabDangXuLy.Click += Click_tvTabDangXuLy;
                _tvTabDaXuLy.Click += Click_tvTabDaXuLy;
                //-----

                _tvName.Click += Click_tvName;
                _imgAvata.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _lnFilter.Click += Click_lnFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };
                _lnAll.Touch += _lnAll_Touch;
                _lnRecycle.Touch += _lnAll_Touch;
                _imgDeleteSearch.Visibility = ViewStates.Gone;
                _IsInProcess = true;
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                SetDataTitleCount();
                SetData();

                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation);
                bottomNavigation.InitializeView();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLHomePage = new ControllerHomePage();
                    CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);

                    _flagIsFiltering = 0;

                    SetDataTitleCount();
                    SetColor_ImageShowSearch_ByFlag(false);
                    SetColor_LinearFilter_ByFlag(false);

                    if (_lnSearch.Visibility == ViewStates.Visible) // ẩn Search đi để hiện tab ra
                    {
                        _lnSearch.Visibility = ViewStates.Gone;
                        _lnTab.Visibility = ViewStates.Visible;
                    }

                    if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.Processed) // focus lại tab Đang xử lý default
                        Click_tvTabDangXuLy(null, null);

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
                    else
                    {
                        SetData();
                    }
                }
            }
            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.RenewFragmentSingleVDT += RenewData;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            SetViewByLanguage(null, null);

            return _rootView;
        }


        public FragmentSingleListVDT()
        {

        }
        #endregion

        #region Event
        private void _lnAll_Touch(object sender, View.TouchEventArgs e)
        {
            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
        }

        public virtual void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                _tvName.Text = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                _tvTabDangXuLy.Text = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý") + " " + CmmDroidFunction.GetCountNumOfText(_tvTabDangXuLy.Text);
                _tvTabDaXuLy.Text = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý");

                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");

                if (_adapterHomePageRecyVDT != null)
                    _adapterHomePageRecyVDT.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        public virtual void SetColor_LinearFilter_ByFlag(bool flag)
        {
            if (flag == true)
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGreenDueDate)));
            else
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
        }

        public virtual void SetColor_ImageShowSearch_ByFlag(bool flag)
        {
            if (flag == true)
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
            else
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
        }

        public virtual async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                _swipe.Refreshing = true;
                await Task.Run(async () =>
                {
                    ProviderBase pBase = new ProviderBase();
                    await pBase.UpdateAllDynamicDataAndroid(true);
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
                            CmmDroidFunction.ShowVibrateEvent(0.2);
                            CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);

                            if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.Processed) // focus lại tab Processed
                            {
                                // NOTE: Đã xử lý ko có default nên phải khởi tạo lại
                                //CTRLHomePage.LstFilterCondition_VDT = CTRLHomePage.GetListDefault_FilterVDT_Processed();
                            }

                            //SetColor_ImageShowSearch_ByFlag(false); // có trigger ở dưới
                            //SetColor_LinearFilter_ByFlag(false);
                            if (_flagIsFiltering == 0) // nếu đang filter API -> để API tự cập nhật
                                SetDataTitleCount();
                            SetData();
                            SetViewByLanguage(null, null);

                            if (!String.IsNullOrEmpty(_edtSearch.Text))
                            {
                                _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                                _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
                            }

                            if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.Processed) // focus lại tab Processed
                            {
                                CTRLHomePage.SetTextview_NotSelected(_mainAct, _tvTabDangXuLy);
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, null, null);
                                _tvTabDangXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
                            }
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

        public virtual void Click_tvName(object sender, EventArgs e)
        {
            try
            {
                _tvName.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (_lstAppBaseVDT != null && _lstAppBaseVDT.Count > 0)
                    _recyData.SmoothScrollToPosition(0);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvName", ex);
#endif
            }
        }

        public virtual void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                MainActivity.FlagNavigation = (int)EnumBottomNavigationView.SingleListVDT;
                MinionAction.OnRenewDataAndShowFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Menu", ex);
#endif
            }
        }

        public virtual void Click_lnFilter(object sender, EventArgs e)
        {
            try
            {
                _lnFilter.Enabled = false;
                if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                {
                    SetColor_LinearFilter_ByFlag(true);
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    Action action = new Action(() =>
                    {
                        _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                        sharedView_PopupFilterVDT = new SharedView_PopupFilterVDT(_layoutInflater, _mainAct, this, "FragmentSingleListVDT", _rootView);
                        sharedView_PopupFilterVDT.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVDT.FlagViewFilterVDT.SingleListVDT);
                        sharedView_PopupFilterVDT.InitializeView();
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                _lnFilter.Enabled = true;
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
            }
        }

        public virtual void Click_imgSearch(object sender, EventArgs e)
        {

        }

        public virtual void Click_ItemAppBaseVDT(object sender, BeanAppBase e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == false) return;

            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                // Update IsRead DB
                conn.Execute(CTRLHomePage._queryVDT_UpdateRead, true, e.ID);

                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(e.ItemUrl);
                string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                List<BeanWorkflowItem> lstWorkflowItem = conn.Query<BeanWorkflowItem>(_query);
                if (lstWorkflowItem == null || lstWorkflowItem.Count == 0)
                {
                    ProviderControlDynamic p_Dynamic = new ProviderControlDynamic();
                    try
                    {
                        if (!string.IsNullOrEmpty(_workflowItemID))
                        {
                            lstWorkflowItem = p_Dynamic.getWorkFlowItemByRID(_workflowItemID);
                        }
                        if (lstWorkflowItem != null || lstWorkflowItem.Count != 0)
                        {
                            conn.InsertAll(lstWorkflowItem);
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleAppBaseItemClick getWorkFlowItemByRID", ex);
#endif
                    }
                }
                if (e.ResourceCategoryId.Value == 16) // Task
                {
                    FragmentDetailCreateTask fragmentDetailCreateTask = new FragmentDetailCreateTask(this, e.ID, false, (lstWorkflowItem != null & lstWorkflowItem.Count > 0) ? lstWorkflowItem[0] : null);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailCreateTask, "FragmentDetailCreateTask", 0);
                }
                else  // Phiếu Quy trình
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow((lstWorkflowItem != null & lstWorkflowItem.Count > 0) ? lstWorkflowItem[0] : null, null, this.GetType().Name);
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemAppBaseVDT", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public virtual void Click_DeleteSearch(object sender, EventArgs e)
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

        public virtual void TextChanged_edtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(_edtSearch.Text))
                {
                    fromSetData = true;
                    SetColor_ImageShowSearch_ByFlag(true);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
                    _imgDeleteSearch.Visibility = ViewStates.Visible;

                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text.ToString()).ToLowerInvariant();

                    List<BeanAppBaseExt> _lstSearch = (from item in _lstAppBaseVDT
                                                       where (!String.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                                                       select item).ToList();

                    if (_lstSearch != null && _lstSearch.Count > 0)
                    {
                        _lnNoData.Visibility = ViewStates.Gone;
                        //_recyData.Visibility = ViewStates.Visible;
                        _lnRecycleHaveData.Visibility = ViewStates.Visible;
                        _allowLoadMore = false;

                        _adapterHomePageRecyVDT._lstAppBase = _lstSearch;
                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        _adapterHomePageRecyVDT.NotifyDataSetChanged();
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        //_recyData.Visibility = ViewStates.Gone;
                        _lnRecycleHaveData.Visibility = ViewStates.Gone;
                    }
                }
                else // empty -> tra lai ban dau -> giống như Set Data
                {
                    SetColor_ImageShowSearch_ByFlag(false);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                    _imgDeleteSearch.Visibility = ViewStates.Gone;
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

        public virtual async void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (fromSetData)
                    {
                        fromSetData = false;
                    }
                    else
                    {
                        var inputMethodManager = (InputMethodManager)this.Context.GetSystemService(Context.InputMethodService);
                        if (inputMethodManager.IsAcceptingText)
                        {
                            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                        }
                        fromSetData = true;
                    }
                    CustomSpeedLinearLayoutManager _customLNM = (CustomSpeedLinearLayoutManager)_recyData.GetLayoutManager();
                    int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();

                    if (_tempLastVisible == _lstAppBaseVDT.Count - 1 && _allowLoadMore == true)
                    {
                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                        if (_isLocalDataLoading == true)
                        {
                            new Handler(Looper.MainLooper).PostDelayed(() =>
                            {

                                List<BeanAppBaseExt> _lstMore = new List<BeanAppBaseExt>();
                                bool _IsInProcess = (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.InProcess) ? true : false;
                                if (controllerBase.CheckAppHasConnection())
                                {
                                    if (_IsInProcess)
                                    {
                                        _lstMore = _pControlDynamic.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_TOME_INPROCESS, offset: _lstAppBaseVDT.Count);
                                        if(_lstMore!=null)
                                        {
                                            new Handler(Looper.MainLooper).PostDelayed(() =>
                                            {
                                                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                                                foreach (var item in _lstMore)
                                                {
                                                    conn.InsertOrReplace(item);
                                                }
                                                conn.Close();
                                            }, 1000);
                                        }    
                                    }
                                    else
                                    {
                                        _lstMore = _pControlDynamic.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_TOME_PROCESSED, offset: _lstAppBaseVDT.Count);
                                        if (_lstMore != null)
                                        {
                                            new Handler(Looper.MainLooper).PostDelayed(() =>
                                            {
                                                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                                                foreach (var item in _lstMore)
                                                {
                                                    conn.InsertOrReplace(item);
                                                }
                                                conn.Close();
                                            }, 1000);
                                        }
                                    }
                                }
                                else
                                {
                                    _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count - 1);
                                }
                                _mainAct.RunOnUiThread(() =>
                                {
                                    if (_lstMore != null && _lstMore.Count > 0)
                                    {
                                        if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                            _allowLoadMore = false;
                                        else
                                            _allowLoadMore = true;

                                        _lstAppBaseVDT.AddRange(_lstMore);
                                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                        _adapterHomePageRecyVDT.LoadMore(_lstMore);
                                        _adapterHomePageRecyVDT.NotifyDataSetChanged();
                                        _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);

                                    }
                                    else
                                    {
                                        _allowLoadMore = false;
                                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                    }
                                });
                            }, CmmDroidVariable.M_ActionDelayTime - 100);
                        }
                        else // API Filter server
                        {
                            await Task.Run(() =>
                            {
                                int temp = 0;
                                List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VDT, true), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);
                                _mainAct.RunOnUiThread(() =>
                                {
                                    if (_lstMore != null && _lstMore.Count > 0)
                                    {
                                        if (_lstMore.Count < CmmVariable.M_DataFilterAPILimitData)
                                            _allowLoadMore = false;
                                        else
                                            _allowLoadMore = true;

                                        _lstAppBaseVDT.AddRange(_lstMore);
                                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                        _adapterHomePageRecyVDT.LoadMore(_lstMore);
                                        _adapterHomePageRecyVDT.NotifyDataSetChanged();
                                        _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);
                                    }
                                    else
                                    {
                                        _allowLoadMore = false;
                                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                    }
                                });
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_RecyData", ex);
#endif
                }
            });
        }

        #region VDT Event
        public virtual void Click_tvTabDaXuLy(object sender, EventArgs e)
        {
            try
            {
                _IsInProcess = false;
                //if (CmmDroidFunction.PreventMultipleClick() == false) return;
                if (sharedView_PopupFilterVDT != null)
                    sharedView_PopupFilterVDT._tvMacDinh.PerformClick();
                if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.Processed) return;

                _flagCurrentTab = (int)ControllerHomePage.FlagStateFilterVDT.Processed;

                _tvTabDangXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
                _tvTabDaXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                CTRLHomePage.SetTextview_NotSelected(_mainAct, _tvTabDangXuLy);
                CTRLHomePage.SetTextview_Selected(_mainAct, _tvTabDaXuLy);
                CTRLHomePage.InitListFilterCondition("BOTH"); // Đang xử lý lấy Default dc

                // NOTE: Đã xử lý ko có default nên phải khởi tạo lại
                CTRLHomePage.LstFilterCondition_VDT = CTRLHomePage.GetListDefault_FilterVDT_Processed();

                SetColor_LinearFilter_ByFlag(false);
                SetColor_ImageShowSearch_ByFlag(false);

                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, null, null);
                SetData();

                //_tvTabDaXuLy.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                //Action action = new Action(() =>
                //{
                //    _edtSearch.Text = ""; // để triggered lại setData();
                //});
                //new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 100);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvTabDaXuLy", ex);
#endif
            }
        }

        public virtual void Click_tvTabDangXuLy(object sender, EventArgs e)
        {
            try
            {
                _IsInProcess = true;
                SetDataTitleCount();
                if (sharedView_PopupFilterVDT != null)
                    sharedView_PopupFilterVDT._tvMacDinh.PerformClick();
                //if (CmmDroidFunction.PreventMultipleClick() == false) return;
                if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.InProcess) return;

                _flagCurrentTab = (int)ControllerHomePage.FlagStateFilterVDT.InProcess;

                _tvTabDaXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
                _tvTabDangXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                CTRLHomePage.SetTextview_NotSelected(_mainAct, _tvTabDaXuLy);
                CTRLHomePage.SetTextview_Selected(_mainAct, _tvTabDangXuLy);
                CTRLHomePage.InitListFilterCondition("BOTH");

                SetColor_LinearFilter_ByFlag(false);
                SetColor_ImageShowSearch_ByFlag(false);

                if (_tvTabDangXuLy.Text.Contains("("))
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                else
                {
                    ISpannable spannable = new SpannableString(_tvTabDangXuLy.Text.Trim());
                    ColorStateList White = new ColorStateList(new int[][] { new int[] { } }, new int[] { new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Android.Graphics.TypefaceStyle.Normal, -1, White, null);
                    spannable.SetSpan(highlightSpan, 0, _tvTabDangXuLy.Text.Length - 1, SpanTypes.ExclusiveExclusive);
                    _tvTabDangXuLy.SetText(spannable, TextView.BufferType.Spannable);
                }
                _edtSearch.Text = ""; // để triggered lại setData();
                SetData();
                //_tvTabDangXuLy.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                //Action action = new Action(() =>
                //{
                //    _edtSearch.Text = ""; // để triggered lại setData();
                //});
                //new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 100);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvTabDangXuLy", ex);
#endif
            }
        }

        public virtual void Click_imgShowSearch(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                _imgShowSearch.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (_lnSearch.Visibility == ViewStates.Gone)
                {
                    _edtSearch.TextChanged += TextChanged_edtSearch;
                    _lnSearch.Visibility = ViewStates.Visible;
                    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _edtSearch.RequestFocus();
                        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                        _lnTab.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    _edtSearch.TextChanged -= TextChanged_edtSearch;
                    _lnTab.Visibility = ViewStates.Visible;
                    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                        _lnSearch.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgShowSearch", ex);
#endif
            }
        }
        #endregion

        #endregion

        #region Data
        /// <summary>
        /// Gán dữ liệu cho list All, sau đó set couunt cho 2 Tab Title
        /// </summary>
        public async virtual void SetDataTitleCount()
        {
            await Task.Run(() =>
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                try
                {
                    #region Count In Process
                    /*List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Tình trạng","1"),
                        new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái")),
                        new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý")),
                        new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày")),
                        new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày")),
                    };

                    string _queryTitleCount = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(LstFilterDefault, false, true);
                    var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);*/
                    int _lstTitleCount = 0;
                    ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();
                    _lstTitleCount = Convert.ToInt32(pControlDynamic.GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS).Split(";#").Last());
                    //if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                    if (_lstTitleCount != 0)
                    {
                        //CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, _lstTitleCount[0].Count, "VDT");
                        //CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, _lstTitleCount[0].Count, "VDT", "inprocess");
                        _mainAct.RunOnUiThread(() =>
                         {
                             CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, _lstTitleCount, "VDT", "inprocess");
                             if (_IsInProcess) CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                         });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, 0, "VDT", "inprocess");
                            if (_IsInProcess) CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                        });
                    }
                    #endregion
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataListAll", ex);
#endif
                }
                finally
                {
                    conn.Close();
                }
            });
        }

        public virtual async void SetData()
        {


            fromSetData = true;
            //var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();

                bool _IsInProcess = (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.InProcess) ? true : false;

                for (int i = 0; i < CTRLHomePage.LstFilterCondition_VDT.Count; i++)
                {
                    if (CTRLHomePage.LstFilterCondition_VDT[i].Key.Equals("Tình trạng"))
                    {
                        if (_IsInProcess)
                        {
                            CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Tình trạng", "1");
                        }
                        else
                        {
                            CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Tình trạng", "2");
                        }
                        break;
                    }
                }

                _lstAppBaseVDT = new List<BeanAppBaseExt>();

                if (_flagIsFiltering==0) // Default Filter -> offline
                {
                    _isLocalDataLoading = true;

                    if (_IsInProcess)
                    {
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition_Ver2(1);
                    }
                    else
                    {
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition_Ver2(2);
                    }

                    _lstAppBaseVDT = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);




                    HandleBindingData();
                }
                else // Filter value -> filter Server
                {
                    if (_isShowDialog)
                        CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                    await Task.Run(() =>
                    {
                        _isLocalDataLoading = false;
                        int _totalRecord = 0;
                        _lstAppBaseVDT = pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VDT, true), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_isShowDialog)
                            {
                                _isShowDialog = false;
                                CmmDroidFunction.HideProcessingDialog();
                            }
                            // Set Textview count lại nếu đang là Tab đang xử lý
                            if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.InProcess)
                            {
                                CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, _totalRecord, "VDT", "inprocess");
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                            }
                            HandleBindingData();
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                if (_isLocalDataLoading == false && _isShowDialog)
                {
                    _isShowDialog = false;
                    CmmDroidFunction.HideProcessingDialog();
                }
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
            finally
            {
                //conn.Close();
            }
        }

        public virtual void HandleBindingData()
        {
            try
            {
                if (_lstAppBaseVDT != null && _lstAppBaseVDT.Count > 0)
                {
                    _recyData.Animation = ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context);

                    if (_isLocalDataLoading == true)
                    {
                        if (_lstAppBaseVDT.Count < CmmVariable.M_DataLimitRow)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;
                    }
                    else
                    {
                        if (_lstAppBaseVDT.Count < CmmVariable.M_DataFilterAPILimitData)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;
                    }
                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnNoData.Visibility = ViewStates.Gone;
                        //_recyData.Visibility = ViewStates.Visible;
                        _lnRecycleHaveData.Visibility = ViewStates.Visible;
                        if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.InProcess)
                        {
                            if (_isLocalDataLoading)
                                _adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_Local);
                            else
                                _adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_API);
                        }
                        else
                        {
                            if (_isLocalDataLoading)
                                _adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.Processed_Local);
                            else
                                _adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.Processed_API);
                        }



                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        _adapterHomePageRecyVDT.CustomItemClick -= Click_ItemAppBaseVDT;
                        _adapterHomePageRecyVDT.CustomItemClick += Click_ItemAppBaseVDT;
                        _recyData.SetAdapter(_adapterHomePageRecyVDT);
                        _recyData.SetLayoutManager(new CustomSpeedLinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                        _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);

                    });
                }
                else
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        //_recyData.Visibility = ViewStates.Gone;
                        _lnRecycleHaveData.Visibility = ViewStates.Gone;
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleBindingData", ex);
#endif
            }
        }

        public virtual void RenewData(object sender, EventArgs e)
        {
            try
            {
                //SetColor_ImageShowSearch_ByFlag(false); // có trigger ở dưới
                SetColor_LinearFilter_ByFlag(false);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                SetViewByLanguage(null, null);
                SetDataTitleCount();
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

        public virtual void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                if (e != null)
                {
                    _adapterHomePageRecyVDT.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    _adapterHomePageRecyVDT.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("PagerHomePageSingleList", "MinionAction_RenewItemVTBD_AfterFollowEvent", ex);
#endif
            }
        }
        #endregion

    }
}