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
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class PagerHomePageSingleList : CustomBaseFragment
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
        private bool _isFollowScreen = false;
        private LinearLayoutManager _layoutManager;
        private SQLiteConnection conn;
        private ProviderBase p_base = new ProviderBase();
        private ControllerBase controllerBase = new ControllerBase();

        #region Constructor
        public PagerHomePageSingleList() { }

        public PagerHomePageSingleList(CustomBaseFragment _parentFragment, string _type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            this._parentFragment = _parentFragment;
            this._type = _type;
            this._lstFilterCondition = _lstFilterCondition;
        }
        public PagerHomePageSingleList(CustomBaseFragment _parentFragment, string _type, List<KeyValuePair<string, string>> _lstFilterCondition, bool isFollowScreen = false)
        {
            this._parentFragment = _parentFragment;
            this._type = _type;
            this._lstFilterCondition = _lstFilterCondition;
            this._isFollowScreen = isFollowScreen;
        }

        public static CustomBaseFragment NewInstance(CustomBaseFragment _parentFragment, string type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            PagerHomePageSingleList fragment = new PagerHomePageSingleList(_parentFragment, type, _lstFilterCondition);
            return fragment;
        }
        public static CustomBaseFragment NewInstance(CustomBaseFragment _parentFragment, string type, List<KeyValuePair<string, string>> _lstFilterCondition, bool _isFollowScreen = false)
        {
            PagerHomePageSingleList fragment = new PagerHomePageSingleList(_parentFragment, type, _lstFilterCondition, isFollowScreen: _isFollowScreen);
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
            await Task.Run(async () =>
            {
                try
                {
                    int _tempLastVisible = _layoutManager.FindLastCompletelyVisibleItemPosition();

                    if (_type.ToLowerInvariant().Equals("vdt") && _tempLastVisible == _lstAppBaseVDT.Count - 1)
                    {
                        if (_allowLoadMore == true)
                        {
                            adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                            ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                            if (_isLocalDataLoading == true)
                            {
                                new Handler(Looper.MainLooper).PostDelayed(() =>
                                {
                                    List<BeanAppBaseExt> _lstMore = new List<BeanAppBaseExt>();

                                    if (controllerBase.CheckAppHasConnection())
                                    {
                                        _lstMore = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_TOME_INPROCESS, offset: _lstAppBaseVDT.Count);
                                        if(_lstMore!=null)
                                        {
                                            new Handler(Looper.MainLooper).PostDelayed(() =>
                                            {
                                                conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
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
                                        _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);

                                    }


                                    if (_lstMore != null && _lstMore.Count > 0)
                                    {
                                        if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                            _allowLoadMore = false;
                                        else
                                            _allowLoadMore = true;
                                        _mainAct.RunOnUiThread(() =>
                                        {
                                            _lstAppBaseVDT.AddRange(_lstMore);
                                            adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                            adapterHomePageRecyVDT.LoadMore(_lstMore);
                                            adapterHomePageRecyVDT.NotifyDataSetChanged();
                                            _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);
                                        });
                                    }
                                    else
                                    {
                                        _mainAct.RunOnUiThread(() =>
                                        {
                                            _allowLoadMore = false;
                                            adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                                        });

                                    }
                                }, CmmDroidVariable.M_ActionDelayTime - 100);
                            }
                            else // API Filter server
                            {
                                await Task.Run(() =>
                                {
                                    int temp = 0;
                                    List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, true), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);
                                    //List<BeanAppBaseExt> _lstMore = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_FROMME_INPROCESS, offset: _lstAppBaseVTBD.Count);
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
                                            _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);
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
                    else if (_type.ToLowerInvariant().Equals("vtbd") && _tempLastVisible == _lstAppBaseVTBD.Count - 1)
                    {
                        if (_allowLoadMore == true)
                        {
                            adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                            ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                            if (_isLocalDataLoading == true)
                            {
                                new Handler(Looper.MainLooper).PostDelayed(() =>
                                {
                                    List<BeanAppBaseExt> _lstMore = new List<BeanAppBaseExt>();
                                    if (controllerBase.CheckAppHasConnection())
                                    {
                                        _lstMore = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_FROMME_INPROCESS, offset: _lstAppBaseVTBD.Count);
                                        if (_lstMore != null)
                                        {
                                            new Handler(Looper.MainLooper).PostDelayed(() =>
                                            {
                                                conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
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
                                        _lstMore = _pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);
                                    }
                                    _mainAct.RunOnUiThread(() =>
                                    {
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
                                            _recyData.SetItemViewCacheSize(_lstAppBaseVTBD.Count);
                                        }
                                        else
                                        {
                                            _allowLoadMore = false;
                                            adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                                        }
                                    });
                                }, CmmDroidVariable.M_ActionDelayTime - 100);
                            }
                            else // API Filter server
                            {
                                await Task.Run(() =>
                                {
                                    int temp = 0;
                                    List<BeanAppBaseExt> _lstMore = _pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, false), ref temp, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVTBD.Count);
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
                                            _recyData.SetItemViewCacheSize(_lstAppBaseVTBD.Count);

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
            });
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
                        /*if (controllerBase.CheckAppHasConnection())
                        {
                            _lstAppBaseVDT = p_base.LoadMoreDataTFromSerVer(FuncName: CmmVariable.KEY_GET_TOME_INPROCESS, offset: _lstAppBaseVDT.Count);
                        }
                        else
                        {*/
                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition_Ver2(1);
                        _lstAppBaseVDT = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);
                        /*}*/
                        if (_isFollowScreen)
                        {
                            _lstAppBaseVDT = _lstAppBaseVDT.Where(x => x.Status == 1).ToList();
                        }
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
                            _lstAppBaseVDT = pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, true), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }
                                if (_isFollowScreen)
                                {
                                    _lstAppBaseVDT = _lstAppBaseVDT.Where(x => x.Status == 1).ToList();
                                }
                                HandleBindingData_VDT();
                                // Set Textview count lại cho Parent
                                FragmentHomePage _temp = ((FragmentHomePage)_parentFragment);
                                CTRLHomePage.SetTextview_FormatItemCount(((FragmentHomePage)_parentFragment)._tvVDT, _totalRecord, "VDT");
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

                        _queryVTBD = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition_Ver2(1);
                        _lstAppBaseVTBD = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);

                        if (_isFollowScreen)
                        {
                            _lstAppBaseVTBD = _lstAppBaseVTBD.Where(x => x.Status == 1).ToList();
                        }
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
                            _lstAppBaseVTBD = pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, false), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }
                                if (_isFollowScreen)
                                {
                                    _lstAppBaseVTBD = _lstAppBaseVTBD.Where(x => x.Status == 1).ToList();
                                }
                                HandleBindingData_VTBD();
                                // Set Textview count lại cho Parent

                                FragmentHomePage _temp = ((FragmentHomePage)_parentFragment);
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
        public List<BeanAppBaseExt> beanAppBaseExtsSearch(String textSearch, int location)
        {
            string _content = CmmFunction.removeSignVietnamese(textSearch).ToLowerInvariant();
            List<BeanAppBaseExt> _lstSearch = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> _lstAppBaseTemp = new List<BeanAppBaseExt>();
            if (location != 1)
            {
                for (int i = 0; i < _lstAppBaseVTBD.Count; i++)
                {
                    if (_lstAppBaseVTBD[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBaseVTBD[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBaseVTBD[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                        if (_lstAppBaseVTBD[i].IsFollow)
                        {
                            _lstAppBaseTemp.Add(_lstAppBaseVTBD[i]);
                        }

                    }

                }
                _lstAppBaseVTBD.Clear();
                _lstAppBaseVTBD.AddRange(_lstAppBaseTemp);
                _lstSearch = (from item in _lstAppBaseVTBD
                              where (!String.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                              select item).ToList();
            }
            else
            {
                for (int i = 0; i < _lstAppBaseVDT.Count; i++)
                {
                    if (_lstAppBaseVDT[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBaseVDT[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBaseVDT[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                        if (_lstAppBaseVDT[i].IsFollow)
                        {
                            _lstAppBaseTemp.Add(_lstAppBaseVDT[i]);
                        }

                    }

                }
                _lstAppBaseVDT.Clear();
                _lstAppBaseVDT.AddRange(_lstAppBaseTemp);
                _lstSearch = (from item in _lstAppBaseVDT
                              where (!String.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(_content))
                              select item).ToList();
            }
            return _lstSearch;
        }
        public async void Search(String textSearch, int location)
        {
            try
            {
                ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();

                if (location == 1)
                {
                    if (CTRLHomePage.CheckListFilterIsDefault_VDT(_lstFilterCondition, true)) // Default Filter -> offline
                    {
                        _isLocalDataLoading = true;
                        _lstAppBaseVDT = new List<BeanAppBaseExt>();

                        string _valueCondition = _lstFilterCondition.Where(x => x.Key.ToLowerInvariant().Contains("tình trạng")).FirstOrDefault().Value;

                        _queryVDT = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition_Ver2(0);
                        _lstAppBaseVDT = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVDT, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVDT.Count);
                        if (_isFollowScreen)
                        {
                            _lstAppBaseVDT = _lstAppBaseVDT.Where(x => x.Status == 1).ToList();
                        }


                        _lstAppBaseVDT = beanAppBaseExtsSearch(textSearch, location);




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
                            _lstAppBaseVDT = pControlDynamic.GetListFilterMyTask(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, true), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }
                                if (_isFollowScreen)
                                {
                                    _lstAppBaseVDT = _lstAppBaseVDT.Where(x => x.Status == 1).ToList();
                                }
                                _lstAppBaseVDT = beanAppBaseExtsSearch(textSearch, location);

                                HandleBindingData_VDT();
                                // Set Textview count lại cho Parent

                            });
                        });
                    }
                }
                else
                {
                    if (CTRLHomePage.CheckListFilterIsDefault_VTBD(_lstFilterCondition)) // Default Filter -> offline
                    {
                        _isLocalDataLoading = true;
                        _lstAppBaseVTBD = new List<BeanAppBaseExt>();
                        _queryVTBD = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(_lstFilterCondition, true);
                        _lstAppBaseVTBD = pControlDynamic.LoadMoreDataT<BeanAppBaseExt>(_queryVTBD, CmmVariable.M_DataLimitRow, CmmVariable.M_DataLimitRow, _lstAppBaseVTBD.Count);
                        if (_isFollowScreen)
                        {
                            _lstAppBaseVTBD = _lstAppBaseVTBD.Where(x => x.Status == 1).ToList();
                        }
                        _lstAppBaseVTBD = beanAppBaseExtsSearch(textSearch, location);

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
                            _lstAppBaseVTBD = pControlDynamic.GetListFilterMyRequest(CTRLHomePage.GetDictionaryFilter(_lstFilterCondition, false), ref _totalRecord, CmmVariable.M_DataFilterAPILimitData, _lstAppBaseVDT.Count);

                            _mainAct.RunOnUiThread(() =>
                            {
                                if (_isShowDialog)
                                {
                                    _isShowDialog = false;
                                    CmmDroidFunction.HideProcessingDialog();
                                }
                                if (_isFollowScreen)
                                {
                                    _lstAppBaseVTBD = _lstAppBaseVTBD.Where(x => x.Status == 1).ToList();
                                }
                                _lstAppBaseVTBD = beanAppBaseExtsSearch(textSearch, location);
                                HandleBindingData_VTBD();

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
                        adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_Local, isScreenFollow: _isFollowScreen);
                    else
                        adapterHomePageRecyVDT = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBaseVDT, (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_API, isScreenFollow: _isFollowScreen);
                    _mainAct.RunOnUiThread(() =>
                    {
                        adapterHomePageRecyVDT.HasStableIds = true;
                        adapterHomePageRecyVDT.SetAllowLoadMore(_allowLoadMore);
                        adapterHomePageRecyVDT.CustomItemClick -= Click_ItemAppBaseVDT;
                        adapterHomePageRecyVDT.CustomItemClick += Click_ItemAppBaseVDT;
                        _recyData.SetAdapter(adapterHomePageRecyVDT);
                        _recyData.SetLayoutManager(_layoutManager);
                        _recyData.SetItemViewCacheSize(_lstAppBaseVDT.Count);
                    });
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
                    _mainAct.RunOnUiThread(() =>
                    {
                        adapterHomePageRecyVTBD = new AdapterHomePageRecyVTBD_Ver2(_mainAct, _rootView.Context, _lstAppBaseVTBD, isScreenFollow: _isFollowScreen);
                        adapterHomePageRecyVTBD.HasStableIds = true;
                        adapterHomePageRecyVTBD.SetAllowLoadMore(_allowLoadMore);
                        adapterHomePageRecyVTBD.CustomItemClick -= Click_ItemAppBaseVTBD;
                        adapterHomePageRecyVTBD.CustomItemClick += Click_ItemAppBaseVTBD;
                        _recyData.SetAdapter(adapterHomePageRecyVTBD);
                        _recyData.SetLayoutManager(_layoutManager);
                        _recyData.SetItemViewCacheSize(_lstAppBaseVTBD.Count);

                    });
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