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
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
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

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppSingleListVTBD : FragmentSingleListVTBD
    {
        private ImageView _imgBack;
        public BeanWorkflow _currentWorkflow;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            CmmEvent.UpdateLangComplete -= SetViewByLanguage;
            MinionAction.RenewFragmentSingleVTBD -= RenewData;
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
                _rootView = inflater.Inflate(Resource.Layout.ViewChildAppListWorkflow, null);

                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_All);
                _lnTab = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Tab);
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Search);
                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewChildAppListWorkflow_Toolbar);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppListWorkflow_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_Back);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Filter);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_Filter);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ListWorkflowView_Search_Delete);
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_ShowSearch);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ListWorkflowView_Search);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewChildAppListWorkflow);
                //_lvData = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewChildAppListWorkflow);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_NoData);
                _lnContent = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Content);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppListWorkflow_NoData);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_BottomNavigation);
                _lnDisablePager = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_DisablePager);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_BlackFilter);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewChildAppListWorkflow);

                _tvName.Click += Click_tvName;
                _imgBack.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _lnFilter.Click += Click_lnFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _imgShowSearch.Click += Click_imgShowSearch;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                SetColor_ImageShowSearch_ByFlag(false);
                SetDataTitle();
                SetData();

                _imgDeleteSearch.Visibility = ViewStates.Gone;
                _imgShowSearch.Visibility = ViewStates.Visible;
                _lnSearch.Visibility = ViewStates.Gone;
                _lnTab.Visibility = ViewStates.Gone;
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLHomePage = new ControllerHomePage();
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
            // Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.RenewFragmentSingleVTBD += RenewData;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            SetViewByLanguage(null, null);

            return _rootView;
        }

        public FragmentChildAppSingleListVTBD(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }

        #region Event
        public override void SetViewByLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                base.SetViewByLanguage(sender, e);
                _tvName.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _currentWorkflow.Title : _currentWorkflow.TitleEN;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        public override void SetTitleByListCount(int _count)
        {
            base.SetTitleByListCount(_count);
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

        public override void Click_tvName(object sender, EventArgs e)
        {
            try
            {
                base.Click_tvName(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvName", ex);
#endif
            }
        }

        public override void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment(typeof(FragmentBoard).Name);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Menu", ex);
#endif
            }
        }

        public override void Click_lnFilter(object sender, EventArgs e)
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

                        SharedView_PopupFilterVTBD sharedView_PopupFilterVTBD = new SharedView_PopupFilterVTBD(_layoutInflater, _mainAct, this, this.GetType().Name, _rootView);
                        sharedView_PopupFilterVTBD.InitializeValue(CTRLHomePage, (int)SharedView_PopupFilterVTBD.FlagViewFilterVTBD.ChildAppSingleListVTBD);
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

        public override void Click_imgShowSearch(object sender, EventArgs e)
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

        public override void Click_ItemAppBaseVTBD(object sender, BeanAppBaseExt e)
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

        public override void Click_DeleteSearch(object sender, EventArgs e)
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

        public override void TextChanged_edtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(_edtSearch.Text))
                {
                    SetColor_ImageShowSearch_ByFlag(true);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
                    _imgDeleteSearch.Visibility = ViewStates.Visible;
                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text.ToString()).ToLowerInvariant();

                    List<BeanAppBaseExt> _lstSearch = (from item in _lstAppBaseVTBD
                                                       where (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                                                       orderby item.Created descending
                                                       select item).ToList();

                    if (_lstSearch != null && _lstSearch.Count > 0)
                    {
                        _lnNoData.Visibility = ViewStates.Gone;
                        _recyData.Visibility = ViewStates.Visible;

                        _allowLoadMore = false;

                        _adapterHomePageRecyVTBD._lstAppBase = _lstSearch;
                        _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        _adapterHomePageRecyVTBD.NotifyDataSetChanged();
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        _recyData.Visibility = ViewStates.Gone;
                    }
                }
                else
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

        public override void SetLinearFilter_ByFlag(int flag)
        {
            base.SetLinearFilter_ByFlag(flag);
        }

        public override async void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                CustomSpeedLinearLayoutManager _customLNM = (CustomSpeedLinearLayoutManager)_recyData.GetLayoutManager();
                int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();

                if (_tempLastVisible == _lstAppBaseVTBD.Count - 1 && _allowLoadMore == true)
                {
                    _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    if (_isLocalDataLoading == true)
                    {
                        new Handler().PostDelayed(() =>
                        {
                            List<BeanAppBaseExt> _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);

                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstAppBaseVTBD.AddRange(_lstMore);
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
                            List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VTBD, false, _currentWorkflow.WorkflowID.ToString()), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVTBD.Count);
                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_lstMore != null && _lstMore.Count > 0)
                                {
                                    if (_lstMore.Count < CmmVariable.M_DataFilterAPILimitData)
                                        _allowLoadMore = false;
                                    else
                                        _allowLoadMore = true;

                                    _lstAppBaseVTBD.AddRange(_lstMore);
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
        public override void SetDataTitle()
        {
            try
            {
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVTBD("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVTBD("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVTBD("Đến ngày")),
                };

                string _queryTitleCount = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(LstFilterDefault, false, true, _workflowID: _currentWorkflow.WorkflowID);
                var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);
                conn.Close();

                if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                    SetTitleByListCount(_lstTitleCount[0].Count);
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

        public override async void SetData()
        {
            ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();
            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                _lstAppBaseVTBD = new List<BeanAppBaseExt>();
                if (CTRLHomePage.CheckListFilterIsDefault_VTBD(CTRLHomePage.LstFilterCondition_VTBD)) // Default Filter -> offline
                {
                    _isLocalDataLoading = true;
                    _queryVTBD = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(CTRLHomePage.LstFilterCondition_VTBD, true, _workflowID: _currentWorkflow.WorkflowID);
                    _lstAppBaseVTBD = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);
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
                        _lstAppBaseVTBD = pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VTBD, false, _currentWorkflow.WorkflowID.ToString()), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVTBD.Count);

                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_isShowDialog)
                            {
                                _isShowDialog = false;
                                CmmDroidFunction.HideProcessingDialog();
                            }
                            // Set Textview count lại cho Parent
                            //SetTitleByListCount(_totalRecord); // ko cần set vì chỉ cần hiện title
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
                conn.Close();
            }
        }

        public override void SetColor_ImageShowSearch_ByFlag(bool flag)
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
                if (_lstAppBaseVTBD != null && _lstAppBaseVTBD.Count > 0)
                {
                    _recyData.Animation = ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context);

                    if (_isLocalDataLoading == true)
                    {
                        if (_lstAppBaseVTBD.Count < CmmVariable.M_DataLimitRow)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;
                    }
                    else
                    {
                        if (_lstAppBaseVTBD.Count < CmmVariable.M_DataFilterAPILimitData)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;
                    }

                    _adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD_Ver2(_mainAct, _rootView.Context, _lstAppBaseVTBD);
                    _adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                    _adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemAppBaseVTBD;
                    _adapterHomePageRecyVTBD.CustomItemClick += Click_ItemAppBaseVTBD;
                    _recyData.SetAdapter(_adapterHomePageRecyVTBD);
                    _recyData.SetLayoutManager(new CustomSpeedLinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));

                    _lnNoData.Visibility = ViewStates.Gone;
                    _recyData.Visibility = ViewStates.Visible;
                }
                else
                {
                    _lnNoData.Visibility = ViewStates.Visible;
                    _recyData.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleBindingData", ex);
#endif
            }
        }

        public override void RenewData(object sender, EventArgs e)
        {
            try
            {
                SetLinearFilter_ByFlag(0);
                SetViewByLanguage(null, null);
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

        public override void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                if (e != null)
                {
                    _adapterHomePageRecyVTBD.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    _adapterHomePageRecyVTBD.NotifyDataSetChanged();
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