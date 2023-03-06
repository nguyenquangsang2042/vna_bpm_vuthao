using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
    public class PagerChildAppHomePageSingleList : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private RecyclerView _recyData;
        private LinearLayout _lnNoData;
        private TextView _tvNoData;
        private List<BeanAppBaseExt> _lstAppBaseVDT = new List<BeanAppBaseExt>();
        private List<BeanAppBaseExt> _lstAppBaseVTBD = new List<BeanAppBaseExt>();

        public AdapterHomePageRecyVDT_Ver2 adapterHomePageRecyVDT;
        public AdapterHomePageRecyVTBD_Ver2 adapterHomePageRecyVTBD;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private CustomBaseFragment _parentFragment;
        public List<KeyValuePair<string, string>> _lstFilterCondition = new List<KeyValuePair<string, string>>();
        private string _queryVDT = "", _queryVTBD = "";
        public string _type = ""; // VDT - VTBD
        private bool _allowLoadMore = true;
        public bool _isLocalDataLoading = true; // nếu = false là call API từ Server
        public bool _isShowDialog = false; // nếu = false là không hiện khi call API từ Server

        private LinearLayoutManager _layoutManager;

        #region Constructor
        public PagerChildAppHomePageSingleList() { }

        public PagerChildAppHomePageSingleList(CustomBaseFragment _parentFragment, string _type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            this._parentFragment = _parentFragment;
            this._type = _type;
            this._lstFilterCondition = _lstFilterCondition;
        }

        public static CustomBaseFragment NewInstance(CustomBaseFragment _parentFragment, string type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            PagerChildAppHomePageSingleList fragment = new PagerChildAppHomePageSingleList(_parentFragment, type, _lstFilterCondition);
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;

            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewHomePagePager, null);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewHomePagePager);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePagePager_NotData);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePagePager_NotData);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewHomePagePager);
                _layoutManager = new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false);

                // _recyData.SetOnFlingListener(new CustomFlingListener(5000));
                _recyData.ScrollChange += ScrollChange_RecyData;

                SetViewByLanguage();
                SetData();
            }
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
                _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
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

        private async void ScrollChange_RecyData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                int _tempLastVisible = _layoutManager.FindLastCompletelyVisibleItemPosition();

                if (_type.ToLowerInvariant().Equals("vdt"))
                {
                    if (_tempLastVisible == _lstAppBaseVDT.Count - 1 && _allowLoadMore == true)
                    {
                        adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
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
                                    adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                    adapterHomePageRecyVDT.LoadMore(_lstMore);
                                    adapterHomePageRecyVDT.NotifyDataSetChanged();
                                }
                                else
                                {
                                    _allowLoadMore = false;
                                    adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                }
                            }, CmmDroidVariable.M_ActionDelayTime - 100);
                        }
                        else // API Filter server
                        {
                            await Task.Run(() =>
                            {
                                int temp = 0;
                                List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, true, MainActivity.ChildAppWorkflow.WorkflowID.ToString()), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count) ;
                                _mainAct.RunOnUiThread(() =>
                                {
                                    if (_lstMore != null && _lstMore.Count > 0)
                                    {
                                        if (_lstMore.Count < CmmVariable.M_DataFilterAPILimitData)
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
                            });
                        }
                    }
                }
                else if (_type.ToLowerInvariant().Equals("vtbd"))
                {
                    if (_tempLastVisible == _lstAppBaseVTBD.Count - 1 && _allowLoadMore == true)
                    {
                        adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
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

                                    adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                    adapterHomePageRecyVTBD.LoadMore(_lstMore);
                                    adapterHomePageRecyVTBD.NotifyDataSetChanged();
                                }
                                else
                                {
                                    _allowLoadMore = false;
                                    adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                }
                            }, CmmDroidVariable.M_ActionDelayTime - 100);
                        }
                        else // API Filter server
                        {
                            await Task.Run(() =>
                            {
                                int temp = 0;
                                List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, false, MainActivity.ChildAppWorkflow.WorkflowID.ToString()), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVTBD.Count);
                                _mainAct.RunOnUiThread(() =>
                                {
                                    if (_lstMore != null && _lstMore.Count > 0)
                                    {
                                        if (_lstMore.Count < CmmVariable.M_DataFilterAPILimitData)
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
                            });
                        }
                    }
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_RecyData", ex);
#endif
            }
        }
        #endregion

        #region Data
        public async void SetData()
        {
            try
            {
                ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();

                if (_type.ToLowerInvariant().Equals("vdt"))
                {
                    if (CTRLHomePage.CheckListFilterIsDefault_VDT(_lstFilterCondition, true)) // Default Filter -> offline
                    {
                        _isLocalDataLoading = true;
                        _lstAppBaseVDT = new List<BeanAppBaseExt>();

                        string _valueCondition = _lstFilterCondition.Where(x => x.Key.ToLowerInvariant().Contains("tình trạng")).FirstOrDefault().Value;
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(_lstFilterCondition, true, false, _orderByColumn: _valueCondition.Equals("1") ? "NOTI.StartDate" : "AB.Modified", _workflowID: MainActivity.ChildAppWorkflow.WorkflowID);
                        _lstAppBaseVDT = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);
                        HandleBindingData_VDT();
                    }
                    else // Filter value -> filter Server
                    {
                        if (_isShowDialog)
                            CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), true);

                        await Task.Run(() =>
                        {
                            _isLocalDataLoading = false;
                            int _totalRecord = 0;
                            _lstAppBaseVDT = new List<BeanAppBaseExt>();
                            _lstAppBaseVDT = pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, true, MainActivity.ChildAppWorkflow.WorkflowID.ToString()), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }

                                HandleBindingData_VDT();
                                // Set Textview count lại cho Parent
                                FragmentChildAppHomePage _temp = ((FragmentChildAppHomePage)_parentFragment);
                                CTRLHomePage.SetTextview_FormatItemCount(((FragmentChildAppHomePage)_parentFragment)._tvVDT, _totalRecord, "VDT");
                                if (_temp._flagCurrentTask == 1) // đang là VDY -> mới tô màu
                                {
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _temp._tvVDT, _temp._tvVDT.Text, "(", ")");
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _temp._tvVTBD, _temp._tvVTBD.Text, null, null);
                                }
                            });
                        });
                    }
                }
                else if (_type.ToLowerInvariant().Equals("vtbd"))
                {
                    if (CTRLHomePage.CheckListFilterIsDefault_VTBD(_lstFilterCondition)) // Default Filter -> offline
                    {
                        _isLocalDataLoading = true;
                        _lstAppBaseVTBD = new List<BeanAppBaseExt>();
                        _queryVTBD = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(_lstFilterCondition, true, _workflowID: MainActivity.ChildAppWorkflow.WorkflowID);
                        _lstAppBaseVTBD = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);
                        HandleBindingData_VTBD();
                    }
                    else // Filter value -> filter Server
                    {
                        if (_isShowDialog)
                            CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), true);

                        await Task.Run(() =>
                        {
                            _isLocalDataLoading = false;
                            int _totalRecord = 0;
                            _lstAppBaseVTBD = new List<BeanAppBaseExt>();
                            _lstAppBaseVTBD = pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, false, MainActivity.ChildAppWorkflow.WorkflowID.ToString()), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }

                                HandleBindingData_VTBD();
                                // Set Textview count lại cho Parent

                                FragmentChildAppHomePage _temp = ((FragmentChildAppHomePage)_parentFragment);
                                CTRLHomePage.SetTextview_FormatItemCount(_temp._tvVTBD, _totalRecord, "VTBD");
                                if (_temp._flagCurrentTask == 2) // đang là VTBD -> mới tô màu
                                {
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _temp._tvVDT, _temp._tvVDT.Text, null, null);
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _temp._tvVTBD, _temp._tvVTBD.Text, "(", ")");
                                }
                            });
                        });
                    }
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
        }

        public void HandleBindingData_VDT()
        {
            try
            {
                if (_lstAppBaseVDT != null && _lstAppBaseVDT.Count > 0)
                {
                    //_recyData.Animation = ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context);

                    if (_isLocalDataLoading)
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

                    if (_isLocalDataLoading)
                        adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_Local);
                    else
                        adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_API);

                    adapterHomePageRecyVDT.HasStableIds = true;
                    adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                    adapterHomePageRecyVDT.CustomItemClick -= Click_ItemAppBaseVDT;
                    adapterHomePageRecyVDT.CustomItemClick += Click_ItemAppBaseVDT;
                    _recyData.SetAdapter(adapterHomePageRecyVDT);
                    _recyData.SetLayoutManager(_layoutManager);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleBindingData_VDT", ex);
#endif
            }
        }

        public void HandleBindingData_VTBD()
        {
            try
            {
                if (_lstAppBaseVTBD != null && _lstAppBaseVTBD.Count > 0)
                {
                    //_recyData.Animation = ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context);

                    if (_isLocalDataLoading)
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

                    _lnNoData.Visibility = ViewStates.Gone;
                    _recyData.Visibility = ViewStates.Visible;

                    adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD_Ver2(_mainAct, _rootView.Context, _lstAppBaseVTBD);
                    adapterHomePageRecyVTBD.HasStableIds = true;
                    adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                    adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemAppBaseVTBD;
                    adapterHomePageRecyVTBD.CustomItemClick += Click_ItemAppBaseVTBD;
                    _recyData.SetAdapter(adapterHomePageRecyVTBD);
                    _recyData.SetLayoutManager(_layoutManager);
                }
                else
                {
                    _allowLoadMore = false;
                    _lnNoData.Visibility = ViewStates.Visible;
                    _recyData.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleBindingData_VTBD", ex);
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
                    FragmentDetailCreateTask fragmentDetailCreateTask = new FragmentDetailCreateTask(_parentFragment, e.ID, false, (lstWorkflowItem != null & lstWorkflowItem.Count > 0) ? lstWorkflowItem[0] : null);
                    _mainAct.AddFragment(FragmentManager, fragmentDetailCreateTask, "FragmentDetailCreateTask", 0);
                }
                else  // Phiếu Quy trình
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow((lstWorkflowItem != null & lstWorkflowItem.Count > 0) ? lstWorkflowItem[0] : null, null, _parentFragment.GetType().Name);
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
                //if (MainActivity.FlagRefreshDataFragment == true) // chuyển trang -> phải làm mới flag
                //{
                //    if (_type.ToLowerInvariant().Equals("vdt"))
                //    {
                //        _lstFilterCondition = CTRLHomePage.LstFilterCondition_VDT; // gán cờ default lại
                //    }
                //    else if (_type.ToLowerInvariant().Equals("vtbd"))
                //    {
                //        _lstFilterCondition = CTRLHomePage.LstFilterCondition_VTBD;  // gán cờ default lại
                //    }
                //}
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
    }
}