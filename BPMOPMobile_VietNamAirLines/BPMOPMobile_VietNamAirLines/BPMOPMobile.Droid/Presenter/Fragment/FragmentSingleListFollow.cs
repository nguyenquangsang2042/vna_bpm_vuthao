using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
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
using Com.Telerik.Widget.Calendar.Events;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentSingleListFollow : CustomBaseFragment
    {
        public MainActivity _mainAct;
        public View _rootView;
        public ControllerHomePage CTRLHomePage = new ControllerHomePage();

        public List<BeanAppBaseExt> _lstAppBaseVTBD = new List<BeanAppBaseExt>();
        public List<BeanAppBaseExt> _lstAppBaseVDT = new List<BeanAppBaseExt>();
        public List<BeanAppBaseExt> _lstAppBaseFollow = new List<BeanAppBaseExt>();
        public SwipeRefreshLayout _swipe;
        public RelativeLayout _relaToolbar;
        public CircleImageView _imgAvata;
        public TextView _tvName, _tvNoData;
        public ImageView _imgFilter, _imgDeleteSearch, _imgShowSearch, _imgBack;
        public EditText _edtSearch;
        public RecyclerView _recyData;
        public LinearLayout _lnAll, _lnTab, _lnSearch, _lnFilter, _lnNoData, _lnContent, _lnBlackFilter, _lnDisablePager, _lnBottomNavigation, _lnRecycleHaveData;
        public AdapterHomePageRecyVTBD_Ver2 _adapterHomePageRecyVTBD;
        private LinearLayout _lnRecycle;

        public string _queryVTBD = "";
        public string _queryVDT = "";
        public int _flagIsFiltering = 0;
        public bool _allowLoadMore = true;
        public bool _isLocalDataLoading = true; // nếu = false là call API từ Server
        public bool _isShowDialog = false; // nếu = false là không hiện khi call API từ Server
        private bool fromSetData = false;
        private ControllerBase controllerBase = new ControllerBase();
        ProviderBase p_base = new ProviderBase();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            MinionAction.RenewFragmentSingleFollow -= RenewData;
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra
            CTRLHomePage.InitListFilterCondition("BOTH");
            base.OnDestroyView();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewFollowWorkflow, null);

                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_All);
                _lnTab = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Tab);
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Search);
                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewListWorkflow_Toolbar);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewListWorkflow_Avata);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_Name);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Filter);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Filter);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ListWorkflowView_Search_Delete);
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_ShowSearch);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewListWorkflow_Back);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ListWorkflowView_Search);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewListWorkflow);
                //_lvData = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewListWorkflow);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_NoData);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_Content);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewListWorkflow_NoData);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_BottomNavigation);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_DisablePager);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewListWorkflow_BlackFilter);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewListWorkflow);
                _lnRecycle = _rootView.FindViewById<LinearLayout>(Resource.Id.lnRecycle);
                _lnRecycleHaveData = _rootView.FindViewById<LinearLayout>(Resource.Id.lnRecycleHaveData);
                _imgAvata.Visibility = ViewStates.Gone;
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                _imgFilter.Visibility = ViewStates.Gone;
                _tvName.Click += Click_tvName;
                _swipe.Refresh += Swipe_RefreshData;
                _lnFilter.Click += Click_lnFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _imgShowSearch.Click += Click_imgShowSearch;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };
                _imgBack.Visibility = ViewStates.Visible;
                _imgBack.Click += _imgBack_Click;
                _lnAll.Touch += _lnAll_Touch;
                _lnRecycle.Touch += _lnAll_Touch;
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                SetColor_ImageShowSearch_ByFlag(false);
                SetDataTitle();
                CmmDroidFunction.ShowProcessingDialog(_mainAct, "Vui lòng đợi...", "Please wait...");
                Action action = new Action(() =>
                {
                    SetData();
                    CmmDroidFunction.HideProcessingDialog();
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime * 4);
                _imgDeleteSearch.Visibility = ViewStates.Gone;
                _imgShowSearch.Visibility = ViewStates.Visible;
                _lnSearch.Visibility = ViewStates.Gone;
                _lnTab.Visibility = ViewStates.Gone;
                _lnBottomNavigation.Visibility = ViewStates.Gone;
                /* SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                 bottomNavigation.InitializeValue(_lnBottomNavigation);
                 bottomNavigation.InitializeView();*/
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLHomePage = new ControllerHomePage();
                    CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                    SetColor_ImageShowSearch_ByFlag(false);
                    SetDataTitle();
                    SetLinearFilter_ByFlag(0);

                    if (_lnSearch.Visibility == ViewStates.Visible) // ẩn view search như default
                    {
                        _lnSearch.Visibility = ViewStates.Gone;
                        _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                    }

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
            MinionAction.RenewFragmentSingleFollow += RenewData;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            SetViewByLanguage(null, null);

            return _rootView;
        }


        public FragmentSingleListFollow()
        {

        }

        #region Event
        private void _lnAll_Touch(object sender, View.TouchEventArgs e)
        {
            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
        }

        private void _imgBack_Click(object sender, EventArgs e)
        {
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            FragmentManager.PopBackStack();

        }

        public virtual void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");

                }
                else
                {
                    _tvName.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Follow") + " " + CmmDroidFunction.GetCountNumOfText(_tvName.Text);
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Search");
                }
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");

                if (_adapterHomePageRecyVTBD != null)
                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        public virtual void SetTitleByListCount(int _count)
        {
            CTRLHomePage.SetTextview_FormatItemCount(_tvName, _count, "follow");
            CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvName, _tvName.Text, "(", ")");
        }

        private async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                _swipe.Refreshing = true;
                await Task.Run(() =>
                {
                    //_flagIsFiltering = 0;
                    //CTRLHomePage.InitListFilterCondition("BOTH");

                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);

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

                            if (_flagIsFiltering == 0) // nếu đang filter API -> để API tự cập nhật
                                SetDataTitle();

                            SetColor_ImageShowSearch_ByFlag(false);
                            SetData();
                            SetViewByLanguage(null, null);

                            if (!string.IsNullOrEmpty(_edtSearch.Text))
                            {
                                _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                                _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
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
                if (_lstAppBaseVTBD != null && _lstAppBaseVTBD.Count > 0)
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
                MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Follow;
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
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    SetLinearFilter_ByFlag(1);
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    Action action = new Action(() =>
                    {
                        _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                        SharedView_PopupFilterVTBD sharedView_PopupFilterVTBD = new SharedView_PopupFilterVTBD(_layoutInflater, _mainAct, this, "FragmentSingleListVTBD", _rootView);
                        sharedView_PopupFilterVTBD.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVTBD.FlagViewFilterVTBD.SingleListVTBD);
                        sharedView_PopupFilterVTBD.InitializeView();
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

        public virtual void Click_imgShowSearch(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                _imgShowSearch.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (_lnSearch.Visibility == ViewStates.Gone)
                {
                    _lnSearch.Visibility = ViewStates.Visible;
                    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _edtSearch.RequestFocus();
                        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                        //_lnTab.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    //_lnTab.Visibility = ViewStates.Visible;
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

        public virtual void Click_ItemAppBaseVTBD(object sender, BeanAppBaseExt e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == false) return;

            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
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
                        lstWorkflowItem = p_Dynamic.getWorkFlowItemByRID(_workflowItemID);
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

                    List<BeanAppBaseExt> _lstSearch = (from item in _lstAppBaseFollow
                                                       where (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                                                       orderby item.Created descending
                                                       select item).ToList();

                    if (_lstSearch != null && _lstSearch.Count > 0)
                    {
                        _lnNoData.Visibility = ViewStates.Gone;
                        //_recyData.Visibility = ViewStates.Visible;
                        _lnRecycleHaveData.Visibility = ViewStates.Visible;

                        _allowLoadMore = false;

                        _adapterHomePageRecyVTBD._lstAppBase = _lstSearch;
                        _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        //_recyData.Visibility = ViewStates.Gone;
                        _lnRecycleHaveData.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    SetColor_ImageShowSearch_ByFlag(false);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                    _imgDeleteSearch.Visibility = ViewStates.Gone;
                    HandleBindingData();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_edtSearch", ex);
#endif
            }
        }

        public virtual void SetLinearFilter_ByFlag(int flag)
        {
            if (flag == 1)
            {
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGreenDueDate)));
            }
            else
            {
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
            }
        }

        public virtual async void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
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

                if (_tempLastVisible == _lstAppBaseFollow.Count - 1 && _allowLoadMore == true)
                {
                    _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    if (_isLocalDataLoading == true)
                    {
                        new Handler().PostDelayed(() =>
                        {
                            List<BeanAppBaseExt> _lstMore = new List<BeanAppBaseExt>();
                            if (controllerBase.CheckAppHasConnection())
                            {
                                _lstMore = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_FOLLOW, offset: _lstAppBaseFollow.Count);
                            }
                            else
                            {
                                _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseFollow.Count);

                            }
                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstAppBaseFollow.AddRange(_lstMore);
                                _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                _adapterHomePageRecyVTBD.LoadMore(_lstMore);
                                _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                            }
                            else
                            {
                                _allowLoadMore = false;
                                _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                            }
                        }, CmmDroidVariable.M_ActionDelayTime - 100);
                    }
                    else  // API Filter server
                    {
                        await Task.Run(() =>
                        {
                            int temp = 0;
                            List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VTBD, false), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseFollow.Count);
                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_lstMore != null && _lstMore.Count > 0)
                                {
                                    if (_lstMore.Count < CmmVariable.M_DataFilterAPILimitData)
                                        _allowLoadMore = false;
                                    else
                                        _allowLoadMore = true;

                                    _lstAppBaseFollow.AddRange(_lstMore);
                                    _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                    _adapterHomePageRecyVTBD.LoadMore(_lstMore);
                                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                                }
                                else
                                {
                                    _allowLoadMore = false;
                                    _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
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
        }
        #endregion

        #region Data
        public virtual void SetDataTitle()
        {
            try
            {
                /*var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVTBD("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVTBD("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Đến ngày")),
                };

                string _queryTitleCount = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(LstFilterDefault, false, true);
                var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);
                conn.Close();
                if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                    SetTitleByListCount(_lstTitleCount[0].Count);
                else
                    SetTitleByListCount(0);
                 */
                int _lstTitleCount = 0;
                ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();
                _lstTitleCount = Convert.ToInt32(pControlDynamic.GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FOLLOW).Split(";#").Last());

                if (_lstTitleCount != null && _lstTitleCount != 0)
                    SetTitleByListCount(_lstTitleCount);
                else
                    SetTitleByListCount(0);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDataTitle", ex);
#endif
            }
        }

        public virtual async void SetData()
        {
            try
            {
                _lstAppBaseFollow = new List<BeanAppBaseExt>();
                if (controllerBase.CheckAppHasConnection())
                {
                    _lstAppBaseFollow = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_FOLLOW, offset: _lstAppBaseFollow.Count());
                }
                else
                {
                    fromSetData = true;
                    _lstAppBaseFollow.Clear();
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string queryGetAllBeanAppBase = String.Format("select * from BeanAppBase where StatusGroup in ({0})", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS));
                    List<BeanAppBaseExt> allBeanAppBase = conn.Query<BeanAppBaseExt>(queryGetAllBeanAppBase);
                    foreach (BeanAppBaseExt appbase in allBeanAppBase)
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(appbase.ItemUrl);
                        string queryGetAllBeanWorkFlowFollow = String.Format("select * from BeanWorkflowFollow where WorkflowItemId = {0}", _workflowItemID);
                        List<BeanWorkflowFollow> _lstBeanWorkflowFollow = conn.Query<BeanWorkflowFollow>(queryGetAllBeanWorkFlowFollow);
                        if (_lstBeanWorkflowFollow != null && _lstBeanWorkflowFollow.Count != 0)
                        {
                            _lstAppBaseFollow.Add(appbase);
                        }


                    }
                    List<BeanAppBaseExt> _lstAppBaseTemp = new List<BeanAppBaseExt>();
                    for (int i = 0; i < _lstAppBaseFollow.Count; i++)
                    {
                        if (_lstAppBaseFollow[i].ResourceCategoryId.Value != 16) // Task không có follow
                        {
                            string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBaseFollow[i].ItemUrl);
                            string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                            List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                            if (_lstFollow != null && _lstFollow.Count > 0)
                                _lstAppBaseFollow[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                            if (_lstAppBaseFollow[i].IsFollow)
                            {
                                _lstAppBaseTemp.Add(_lstAppBaseFollow[i]);
                            }

                        }
                    }
                    conn.Close();
                    _lstAppBaseFollow = new List<BeanAppBaseExt>();
                    _lstAppBaseFollow.AddRange(_lstAppBaseTemp);
                }
                _mainAct.RunOnUiThread(() =>
                {
                    HandleBindingData();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error set data SingleListFollow:" + ex.ToString());
            }

        }

        public virtual void SetColor_ImageShowSearch_ByFlag(bool flag)
        {
            if (flag == true)
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
            else
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
        }

        public void HandleBindingData()
        {
            try
            {
                if (_lstAppBaseFollow != null && _lstAppBaseFollow.Count > 0)
                {




                    _adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD_Ver2(_mainAct, _rootView.Context, _lstAppBaseFollow, isScreenFollow: true);
                    _adapterHomePageRecyVTBD.SetAllowLoadMore(false);
                    _adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemAppBaseVTBD;
                    _adapterHomePageRecyVTBD.CustomItemClick += Click_ItemAppBaseVTBD;
                    _recyData.SetAdapter(_adapterHomePageRecyVTBD);
                    _recyData.SetLayoutManager(new CustomSpeedLinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));

                    _lnNoData.Visibility = ViewStates.Gone;
                    //_recyData.Visibility = ViewStates.Visible;
                    _lnRecycleHaveData.Visibility = ViewStates.Visible;
                }
                else
                {
                    _lnNoData.Visibility = ViewStates.Visible;
                    //_recyData.Visibility = ViewStates.Gone;
                    _lnRecycleHaveData.Visibility = ViewStates.Gone;
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
                SetLinearFilter_ByFlag(0);
                SetDataTitle();
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata, 80);
                SetViewByLanguage(null, null);
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
                    SetDataTitle();
                    _adapterHomePageRecyVTBD.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();

                    if (e._IsFollow)
                    {
                        SetData();
                    }
                    else
                    {
                        List<BeanAppBaseExt> lstRemove = _lstAppBaseFollow.Where(x => x.ItemUrl.Contains("ItemId=" + e._workflowItemID)).ToList();
                        if (lstRemove != null && lstRemove.Count != 0)
                        {
                            _lstAppBaseFollow.Remove(lstRemove[0]);
                            HandleBindingData();
                        }
                    }



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