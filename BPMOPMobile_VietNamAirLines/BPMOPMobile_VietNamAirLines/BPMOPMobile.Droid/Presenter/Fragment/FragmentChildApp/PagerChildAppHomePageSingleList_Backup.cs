using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
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
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class PagerChildAppHomePageSingleList_Backup : CustomBaseFragment
    {
        public enum FlagTabLayoutVDT_ChildAppPagerHomePage
        {
            [Description("Đang xử lý")]
            InProcess = 1,
            [Description("Đã xử lý")]
            Processed = 2
        }

        private MainActivity _mainAct;
        private View _rootView;
        private RecyclerView _recyData;
        private LinearLayout _lnNoData, _lnCondition, _lnDangXuLy, _lnDaXuLy;
        private TextView _tvNoData, _tvDangXuLy, _tvDaXuLy;
        private View _vwDangXuLy, _vwDaXuLy;
        private List<BeanAppBaseExt> _lstAppBaseVDT = new List<BeanAppBaseExt>();
        private List<BeanAppBaseExt> _lstAppBaseVTBD = new List<BeanAppBaseExt>();

        private AdapterHomePageRecyVDT_Ver2 adapterHomePageRecyVDT;
        private AdapterHomePageRecyVTBD_Ver2 adapterHomePageRecyVTBD;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private CustomBaseFragment _parentFragment;
        public List<KeyValuePair<string, string>> _lstFilterCondition = new List<KeyValuePair<string, string>>();
        private string _queryVDT = "", _queryVTBD = "";
        private string _type = ""; // VDT - VTBD
        private bool _allowLoadMore = true;

        private LinearLayoutManager _layoutManager;
        public int _flagCurrentTab = (int)FlagTabLayoutVDT_ChildAppPagerHomePage.InProcess;

        #region Constructor
        public PagerChildAppHomePageSingleList_Backup() { }

        public PagerChildAppHomePageSingleList_Backup(CustomBaseFragment _parentFragment, string _type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            this._parentFragment = _parentFragment;
            this._type = _type;
            this._lstFilterCondition = _lstFilterCondition;
        }

        public static CustomBaseFragment NewInstance(CustomBaseFragment _parentFragment, string type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            PagerChildAppHomePageSingleList_Backup fragment = new PagerChildAppHomePageSingleList_Backup(_parentFragment, type, _lstFilterCondition);
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _rootView = inflater.Inflate(Resource.Layout.PagerChildAppHomePage, null);
            // Condition
            _lnCondition = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_PagerChildAppHomePage_Condition);
            _lnDangXuLy = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_PagerChildAppHomePage_DangXuLy);
            _lnDaXuLy = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_PagerChildAppHomePage_DaXuLy);
            _tvDangXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_PagerChildAppHomePage_DangXuLy);
            _tvDaXuLy = _rootView.FindViewById<TextView>(Resource.Id.tv_PagerChildAppHomePage_DaXuLy);
            _vwDangXuLy = _rootView.FindViewById<View>(Resource.Id.vw_PagerChildAppHomePage_DangXuLy);
            _vwDaXuLy = _rootView.FindViewById<View>(Resource.Id.vw_PagerChildAppHomePage_DaXuLy);
            // Content
            _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_PagerChildAppHomePage);
            _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_PagerChildAppHomePage_NotData);
            _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_PagerChildAppHomePage_NotData);
            _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_PagerChildAppHomePage);
            _layoutManager = new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false);

            _recyData.ScrollChange += ScrollChange_RecyData;
            _lnDangXuLy.Click += Click_lnDangXuLy;
            _lnDaXuLy.Click += Click_lnDaXuLy;

            SetViewByLanguage();
            SetData();

            MinionAction.RefreshFragmentViewPager += MinionAction_RefreshFragmentViewPager;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra
            return _rootView;
        }

        public override void OnDestroyView()
        {
            MinionAction.RefreshFragmentViewPager -= MinionAction_RefreshFragmentViewPager;
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent; // Renew nếu có follow xong back ra

            base.OnDestroyView();
        }
        #endregion

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (_type.ToLowerInvariant().Equals("vdt"))
                    _lnCondition.Visibility = ViewStates.Visible;
                else
                    _lnCondition.Visibility = ViewStates.Gone;

                _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                _tvDangXuLy.Text = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
                _tvDaXuLy.Text = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void Click_ItemAppBaseVDT(object sender, BeanAppBaseExt e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == false) return;
            HandleAppBaseItemClick(e);
        }

        private void Click_ItemAppBaseVTBD(object sender, BeanAppBaseExt e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == false) return;
            HandleAppBaseItemClick(e);
        }

        private void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                //LinearLayoutManager _customLNM = (LinearLayoutManager)_recyData.GetLayoutManager();
                //int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();
                int _tempLastVisible = _layoutManager.FindLastCompletelyVisibleItemPosition();

                if (_type.ToLowerInvariant().Equals("vdt"))
                {
                    if (_tempLastVisible == _lstAppBaseVDT.Count - 1 && _allowLoadMore == true)
                    {
                        adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        Action action = new Action(() =>
                        {
                            ProviderBase _pBase = new ProviderBase();
                            List<BeanAppBaseExt> _lstMore = _pBase.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count - 1);

                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstAppBaseVDT.AddRange(_lstMore);
                                adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                adapterHomePageRecyVDT.LoadMore(_lstMore);
                                adapterHomePageRecyVDT.NotifyDataSetChanged();
                            }
                            else
                            {
                                _allowLoadMore = false;
                                adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                            }
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                    }
                }
                else if (_type.ToLowerInvariant().Equals("vtbd"))
                {
                    if (_tempLastVisible == _lstAppBaseVTBD.Count - 1 && _allowLoadMore == true)
                    {
                        adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        Action action = new Action(() =>
                        {
                            ProviderBase _pBase = new ProviderBase();
                            List<BeanAppBaseExt> _lstMore = _pBase.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);

                            if (_lstMore != null && _lstMore.Count > 0)
                            {
                                if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                    _allowLoadMore = false;
                                else
                                    _allowLoadMore = true;

                                _lstAppBaseVTBD.AddRange(_lstMore);

                                adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                adapterHomePageRecyVTBD.LoadMore(_lstMore);
                                adapterHomePageRecyVTBD.NotifyDataSetChanged();
                            }
                            else
                            {
                                _allowLoadMore = false;
                                adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                            }
                        });
                        new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
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

        private void Click_lnDaXuLy(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                if (_flagCurrentTab == (int)FlagTabLayoutVDT_ChildAppPagerHomePage.Processed) return;

                SetTabView(_tvDangXuLy, _vwDangXuLy, false);
                SetTabView(_tvDaXuLy, _vwDaXuLy, true);

                _flagCurrentTab = (int)FlagTabLayoutVDT_ChildAppPagerHomePage.Processed;

                SetData();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnDaXuLy", ex);
#endif
            }
        }

        private void Click_lnDangXuLy(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                if (_flagCurrentTab == (int)FlagTabLayoutVDT_ChildAppPagerHomePage.InProcess) return;

                SetTabView(_tvDangXuLy, _vwDangXuLy, true);
                SetTabView(_tvDaXuLy, _vwDaXuLy, false);

                _flagCurrentTab = (int)FlagTabLayoutVDT_ChildAppPagerHomePage.InProcess;

                SetData();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnDaXuLy", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                ProviderBase pBase = new ProviderBase();

                if (_type.ToLowerInvariant().Equals("vdt"))
                {
                    if (_flagCurrentTab == (int)FlagTabLayoutVDT_ChildAppPagerHomePage.InProcess)
                    {
                        List<KeyValuePair<string, string>> _lstFilterInProcess = CTRLHomePage.LstFilterCondition_VDT;

                        for (int i = 0; i < _lstFilterInProcess.Count; i++)
                        {
                            if (_lstFilterInProcess[i].Key.Equals("Tình trạng"))
                            {
                                _lstFilterInProcess[i] = new KeyValuePair<string, string>("Tình trạng", "1");
                                break;
                            }
                        }
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(_lstFilterInProcess, true, _workflowID: _mainAct.ChildAppWorkflow.WorkflowID);
                    }
                    else
                    {
                        List<KeyValuePair<string, string>> _lstFilterProcessed = CTRLHomePage.LstFilterCondition_VDT;
                        for (int i = 0; i < _lstFilterProcessed.Count; i++)
                        {
                            if (_lstFilterProcessed[i].Key.Equals("Tình trạng"))
                            {
                                _lstFilterProcessed[i] = new KeyValuePair<string, string>("Tình trạng", "2");
                                break;
                            }
                        }
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(_lstFilterProcessed, true, _workflowID: _mainAct.ChildAppWorkflow.WorkflowID);
                    }

                    _lstAppBaseVDT.Clear();
                    //_queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(_lstFilterCondition, true, false, _workflowID: _currentWorkflow.WorkflowID);
                    _lstAppBaseVDT = pBase.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);

                    if (_lstAppBaseVDT != null && _lstAppBaseVDT.Count > 0)
                    {
                        _recyData.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);

                        if (_lstAppBaseVDT.Count < CmmVariable.M_DataLimitRow)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;

                        _lnNoData.Visibility = ViewStates.Gone;
                        _recyData.Visibility = ViewStates.Visible;

                        adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_Local);
                        adapterHomePageRecyVDT.HasStableIds = true;
                        adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        adapterHomePageRecyVDT.CustomItemClick -= Click_ItemAppBaseVDT;
                        adapterHomePageRecyVDT.CustomItemClick += Click_ItemAppBaseVDT;
                        _recyData.SetAdapter(adapterHomePageRecyVDT);
                        //_recyData.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));

                        _recyData.SetLayoutManager(_layoutManager);
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        _recyData.Visibility = ViewStates.Gone;
                    }
                }
                else if (_type.ToLowerInvariant().Equals("vtbd"))
                {
                    _lstAppBaseVTBD.Clear();
                    _queryVTBD = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(_lstFilterCondition, true, _workflowID: _mainAct.ChildAppWorkflow.WorkflowID);

                    _lstAppBaseVTBD = pBase.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);

                    if (_lstAppBaseVTBD != null && _lstAppBaseVTBD.Count > 0)
                    {
                        _recyData.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);

                        if (_lstAppBaseVTBD.Count < CmmVariable.M_DataLimitRow)
                            _allowLoadMore = false;
                        else
                            _allowLoadMore = true;

                        _lnNoData.Visibility = ViewStates.Gone;
                        _recyData.Visibility = ViewStates.Visible;

                        adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD_Ver2(_mainAct, _rootView.Context, _lstAppBaseVTBD);
                        adapterHomePageRecyVTBD.HasStableIds = true;
                        adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemAppBaseVTBD;
                        adapterHomePageRecyVTBD.CustomItemClick += Click_ItemAppBaseVTBD;
                        _recyData.SetAdapter(adapterHomePageRecyVTBD);
                        //_recyData.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                        _recyData.SetLayoutManager(_layoutManager);
                    }
                    else
                    {
                        _allowLoadMore = false;
                        _lnNoData.Visibility = ViewStates.Visible;
                        _recyData.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void HandleAppBaseItemClick(BeanAppBaseExt e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(e.ItemUrl);
                string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                List<BeanWorkflowItem> lstWorkflowItem = conn.Query<BeanWorkflowItem>(_query);

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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleAppBaseItemClick", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void MinionAction_RefreshFragmentViewPager(object sender, EventArgs e)
        {
            try
            {
                if (MainActivity.FlagRefreshDataFragment == true) // chuyển trang -> phải làm mới flag
                {
                    if (_type.ToLowerInvariant().Equals("vdt"))
                    {
                        _lstFilterCondition = CTRLHomePage.LstFilterCondition_VDT;
                    }
                    else if (_type.ToLowerInvariant().Equals("vtbd"))
                    {
                        _lstFilterCondition = CTRLHomePage.LstFilterCondition_VTBD;
                    }
                }
                SetViewByLanguage();
                SetData();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_RefreshFragmentViewPagerVDT", ex);
#endif
            }
        }

        private void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                if (e != null && _type.ToLowerInvariant().Equals("vtbd"))
                {
                    adapterHomePageRecyVTBD.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    adapterHomePageRecyVTBD.NotifyDataSetChanged();
                }
                if (e != null && _type.ToLowerInvariant().Equals("vdt"))
                {
                    adapterHomePageRecyVDT.UpDateItemFollow(e._workflowItemID, e._IsFollow);
                    adapterHomePageRecyVDT.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_RenewItemVTBD_AfterFollowEvent", ex);
#endif
            }
        }
        #endregion

        private void SetTabView(TextView _tv, View _vw, bool flag)
        {
            if (flag)
            {
                _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
                //_vw.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                _vw.Visibility = ViewStates.Visible;
            }
            else
            {
                _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                //_vw.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                _vw.Visibility = ViewStates.Invisible;
            }
            //_tv.StartAnimation(CTRLHomePage.GetAnimationClick_FadeIn(_rootView.Context));
            //_vw.StartAnimation(CTRLHomePage.GetAnimationClick_FadeIn(_rootView.Context));
        }
    }
}