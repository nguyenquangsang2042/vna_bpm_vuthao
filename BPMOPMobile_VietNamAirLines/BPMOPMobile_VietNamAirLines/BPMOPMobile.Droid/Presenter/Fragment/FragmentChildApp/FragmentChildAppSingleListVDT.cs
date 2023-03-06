using System;
using System.Collections.Generic;
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
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppSingleListVDT : FragmentSingleListVDT
    {
        private ImageView _imgBack;
        public BeanWorkflow _currentWorkflow;

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
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewChildAppListWorkflow, null);

                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_All);
                _relaToolbar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewChildAppListWorkflow_Toolbar);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_Back);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppListWorkflow_Name);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Filter);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_Filter);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ListWorkflowView_Search_Delete);
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

                // chỉ VDT mới có
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppListWorkflow_ShowSearch);
                _lnTab = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Tab);
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppListWorkflow_Search);
                _tvTabDangXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppListWorkflow_TabDangXuLy);
                _tvTabDaXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppListWorkflow_TabDaXuLy);

                _lnTab.Visibility = ViewStates.Visible;
                _lnSearch.Visibility = ViewStates.Gone;
                _imgShowSearch.Visibility = ViewStates.Visible;

                _imgShowSearch.Click += Click_imgShowSearch;
                _tvTabDangXuLy.Click += Click_tvTabDangXuLy;
                _tvTabDaXuLy.Click += Click_tvTabDaXuLy;
                //-----

                _imgBack.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _lnFilter.Click += Click_lnFilter;
                _imgDeleteSearch.Click += Click_DeleteSearch;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _recyData.ScrollChange += ScrollChange_RecyData;
                _lnDisablePager.Click += (sender, e) => { };

                _imgDeleteSearch.Visibility = ViewStates.Gone;

                // Child App
                _imgBack.Visibility = ViewStates.Visible;

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                SetDataTitleCount();
                SetData();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLHomePage = new ControllerHomePage();

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
            // Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppSingleListVDT;
            CmmEvent.UpdateLangComplete += SetViewByLanguage;
            MinionAction.RenewFragmentSingleVDT += RenewData;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            SetViewByLanguage(null, null);

            return _rootView;
        }

        public FragmentChildAppSingleListVDT(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }
        #endregion

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

        public override void SetColor_LinearFilter_ByFlag(bool flag)
        {
            base.SetColor_LinearFilter_ByFlag(flag);
        }

        public override void SetColor_ImageShowSearch_ByFlag(bool flag)
        {
            base.SetColor_ImageShowSearch_ByFlag(flag);
        }

        public override async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                _swipe.Refreshing = true;
                await Task.Run(() =>
                {
                    _flagIsFiltering = 0;
                    CTRLHomePage.InitListFilterCondition("BOTH");

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
                            CTRLHomePage.InitListFilterCondition("BOTH");

                            if (_flagCurrentTab == (int)ControllerHomePage.FlagStateFilterVDT.Processed) // focus lại tab Processed
                            {
                                // NOTE: Đã xử lý ko có default nên phải khởi tạo lại
                                CTRLHomePage.LstFilterCondition_VDT = CTRLHomePage.GetListDefault_FilterVDT_Processed();
                            }

                            //SetColor_ImageShowSearch_ByFlag(false); // có trigger ở dưới
                            SetColor_LinearFilter_ByFlag(false);
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
                                _tvTabDangXuLy.BackgroundTintList = ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
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
                if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                {
                    SetColor_LinearFilter_ByFlag(true);
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    Action action = new Action(() =>
                    {
                        _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                        SharedView_PopupFilterVDT sharedView_PopupFilterVDT = new SharedView_PopupFilterVDT(_layoutInflater, _mainAct, this, this.GetType().Name, _rootView);
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

        public override void Click_imgSearch(object sender, EventArgs e)
        {

        }

        public override void Click_ItemAppBaseVDT(object sender, BeanAppBase e)
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

        public override void Click_DeleteSearch(object sender, EventArgs e)
        {
            try
            {
                base.Click_DeleteSearch(sender, e);
                //_edtSearch.Text = "";
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

                    List<BeanAppBaseExt> _lstSearch = (from item in _lstAppBaseVDT
                                                       where (!String.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                                                       select item).ToList();

                    if (_lstSearch != null && _lstSearch.Count > 0)
                    {
                        _lnNoData.Visibility = ViewStates.Gone;
                        _recyData.Visibility = ViewStates.Visible;

                        _allowLoadMore = false;

                        _adapterHomePageRecyVDT._lstAppBase = _lstSearch;
                        _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        _adapterHomePageRecyVDT.NotifyDataSetChanged();
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        _recyData.Visibility = ViewStates.Gone;
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

        public override async void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                CustomSpeedLinearLayoutManager _customLNM = (CustomSpeedLinearLayoutManager)_recyData.GetLayoutManager();
                int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();

                if (_tempLastVisible == _lstAppBaseVDT.Count - 1 && _allowLoadMore == true)
                {
                    _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    if (_isLocalDataLoading == true)
                    {
                        new Handler().PostDelayed(() =>
                        {
                            List<BeanAppBaseExt> _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count - 1);

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
                            }
                            else
                            {
                                _allowLoadMore = false;
                                _adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                            }
                        }, CmmDroidVariable.M_ActionDelayTime - 100);
                    }
                    else // API Filter server
                    {
                        await Task.Run(() =>
                        {
                            int temp = 0;
                            List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VDT, true, _currentWorkflow.WorkflowID.ToString()), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);
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
        }

        #region VDT Event
        public override void Click_tvTabDaXuLy(object sender, EventArgs e)
        {
            try
            {
                base.Click_tvTabDaXuLy(sender, e);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvTabDaXuLy", ex);
#endif
            }
        }

        public override void Click_tvTabDangXuLy(object sender, EventArgs e)
        {
            try
            {
                base.Click_tvTabDangXuLy(sender, e);            
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvTabDangXuLy", ex);
#endif
            }
        }

        public override void Click_imgShowSearch(object sender, EventArgs e)
        {
            try
            {
                base.Click_imgShowSearch(sender, e);
                //if (CmmDroidFunction.PreventMultipleClick() == false) return;

                //_imgShowSearch.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                //if (_lnSearch.Visibility == ViewStates.Gone)
                //{
                //    _lnSearch.Visibility = ViewStates.Visible;
                //    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                //    Action action = new Action(() =>
                //    {
                //        _edtSearch.RequestFocus();
                //        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                //        _lnTab.Visibility = ViewStates.Gone;
                //    });
                //    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                //}
                //else
                //{
                //    _lnTab.Visibility = ViewStates.Visible;
                //    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                //    Action action = new Action(() =>
                //    {
                //        CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);

                //        _lnSearch.Visibility = ViewStates.Gone;
                //    });
                //    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                //}
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
        public override void SetDataTitleCount()
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                #region Count In Process
                List<KeyValuePair<string, string>> LstFilterDefault = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Tình trạng","1"),
                    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày")),
                };

                string _queryTitleCount = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(LstFilterDefault, false, true, _workflowID: _currentWorkflow.WorkflowID);
                var _lstTitleCount = conn.Query<CountNum>(_queryTitleCount);

                if (_lstTitleCount != null && _lstTitleCount.Count > 0)
                {
                    CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, _lstTitleCount[0].Count, "VDT");
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                }
                else
                {
                    CTRLHomePage.SetTextview_FormatItemCount(_tvTabDangXuLy, 0, "VDT");
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDangXuLy, _tvTabDangXuLy.Text, "(", ")");
                }
                #endregion

                #region Count Processed
                List<KeyValuePair<string, string>> LstFilterDefault_Processed = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Tình trạng","2"),
                    new KeyValuePair<string, string>("Trạng thái",CTRLHomePage.GetDefaultValue_FilterVDT("Trạng thái")),
                    new KeyValuePair<string, string>("Hạn xử lý", CTRLHomePage.GetDefaultValue_FilterVDT("Hạn xử lý")),
                    new KeyValuePair<string, string>("Từ ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Từ ngày")),
                    new KeyValuePair<string, string>("Đến ngày", CTRLHomePage.GetDefaultValue_FilterVDT("Đến ngày")),
                };

                string _queryTitleCount_Processed = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(LstFilterDefault_Processed, false, true, _workflowID: _currentWorkflow.WorkflowID);
                var _lstTitleCount_Processed = conn.Query<CountNum>(_queryTitleCount_Processed);

                if (_lstTitleCount_Processed != null && _lstTitleCount_Processed.Count > 0)
                {
                    CTRLHomePage.SetTextview_FormatItemCount(_tvTabDaXuLy, _lstTitleCount_Processed[0].Count, "VDT");
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDaXuLy, _tvTabDaXuLy.Text, "(", ")");
                }
                else
                {
                    CTRLHomePage.SetTextview_FormatItemCount(_tvTabDaXuLy, 0, "VDT");
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvTabDaXuLy, _tvTabDaXuLy.Text, "(", ")");
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
        }

        public override async void SetData()
        {
            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
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

                if (CTRLHomePage.CheckListFilterIsDefault_VDT(CTRLHomePage.LstFilterCondition_VDT, _IsInProcess)) // Default Filter -> offline
                {
                    _isLocalDataLoading = true;

                    if (_IsInProcess)
                    {
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(CTRLHomePage.LstFilterCondition_VDT, true, _searchString: _edtSearch.Text.ToString(), _orderByColumn: "NOTI.StartDate", _workflowID: _currentWorkflow.WorkflowID);
                    }
                    else
                    {
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(CTRLHomePage.LstFilterCondition_VDT, true, _searchString: _edtSearch.Text.ToString(), _orderByColumn: "AB.Modified", _workflowID: _currentWorkflow.WorkflowID);
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
                        _lstAppBaseVDT = pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(CTRLHomePage.LstFilterCondition_VDT, true, _currentWorkflow.WorkflowID.ToString()), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

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
                conn.Close();
            }
        }

        public override void HandleBindingData()
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

                    _lnNoData.Visibility = ViewStates.Gone;
                    _recyData.Visibility = ViewStates.Visible;

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
                //SetColor_ImageShowSearch_ByFlag(false); // có trigger ở dưới
                SetColor_LinearFilter_ByFlag(false);
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

        public override void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
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