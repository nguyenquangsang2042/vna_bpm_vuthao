using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    [Obsolete]
    class PagerHomePageFragment : Android.App.Fragment, IUpdateable
    {
        private MainActivity _mainAct;
        private View _rootView, _footerLv;
        private LayoutInflater _inflater;
        private ListView _lvVDT, _lvVTBD;
        private List<BeanNotify> _lstVDT = new List<BeanNotify>();
        private List<BeanNotify> _lstVDT_Filter = new List<BeanNotify>();
        private List<BeanWorkflowItem> _lstVTBD = new List<BeanWorkflowItem>();
        private List<BeanWorkflowItem> _lstVTBD_Filter = new List<BeanWorkflowItem>();
        private AdapterHomePageVDT _NotifyVDTAdapter;
        private AdapterHomePageVTBD _NotifyVTBDAdapter;
        private LinearLayout _lnNoData;
        private TextView _tvNoData;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();

        private string _type = ""; // VDT - VTBD
        List<KeyValuePair<string, string>> _lstFilterCondition = new List<KeyValuePair<string, string>>();
        private int _limit = 20, _firstItemVDT, _firstItemVTBD;
        private bool _checkMoreVDT, _checkMoreVTBD;
        private string _queryVDT = "", _queryVTBD = "";
        public PagerHomePageFragment(string _type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            this._type = _type;
            this._lstFilterCondition = _lstFilterCondition;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.PagerHomePage, null);
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _lvVDT = _rootView.FindViewById<ListView>(Resource.Id.lv_PagerHomePage_VDT);
            _lvVTBD = _rootView.FindViewById<ListView>(Resource.Id.lv_PagerHomePage_VTBD);
            _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_PagerHomePage_NotData);
            _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_PagerHomePage_NotData);
            SetData();

            _lvVDT.Scroll += Scroll_LoadMoreVDT;
            //_lvVDT.ItemClick += Click_ItemlvVDT;

            _lvVTBD.Scroll += Scroll_LoadMoreVTBD;
            //_lvVTBD.ItemClick += Click_ItemlvTBD;
            return _rootView;
        }
        private void Refesh()
        {

        }
        private void Click_ItemlvVDT(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanWorkflowItem WHERE ItemID = {0} AND ListName = '{1}'", _lstVDT_Filter[e.Position].DocumentID, _lstVDT_Filter[e.Position].ListName);
                var lstWorkflowItem = conn.Query<BeanWorkflowItem>(queryNotify);
                if (lstWorkflowItem != null & lstWorkflowItem.Count > 0)
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(lstWorkflowItem[0], _lstVDT_Filter[e.Position], "FragmentHomePage");
                    _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemlvVDT - Error: " + ex.Message);
#endif
            }
        }
        private void Click_ItemlvTBD(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanNotify WHERE DocumentID = {0} AND ListName = '{1}'", _lstVTBD_Filter[e.Position].ItemID, _lstVTBD_Filter[e.Position].ListName);
                var lstNotify = conn.Query<BeanNotify>(queryNotify);
                BeanWorkflowItem _temp = _lstVTBD[e.Position];
                if (lstNotify != null && lstNotify.Count > 0)
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_lstVTBD_Filter[e.Position], lstNotify[0], "FragmentHomePage");
                    _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
                else
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_lstVTBD[e.Position], null, "FragmentHomePage");
                    _mainAct.ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemlvTBD - Error: " + ex.Message);
#endif
            }
        }
        private void Click_ItemNotifyAdapter(object sender, BeanNotify e)
        {
            try
            {
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanWorkflowItem WHERE ItemID = {0} AND ListName = '{1}'", e.DocumentID, e.ListName);
                var lstWorkflowItem = conn.Query<BeanWorkflowItem>(queryNotify);
                if (lstWorkflowItem != null & lstWorkflowItem.Count > 0)
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(lstWorkflowItem[0], e, "FragmentHomePage");
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemNotifyAdapter - Error: " + ex.Message);
#endif
            }
        }
        private void Click_ItemWorkflowAdapter(object sender, BeanWorkflowItem e)
        {
            try
            {
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string queryNotify = string.Format("SELECT * FROM BeanNotify WHERE DocumentID = {0} AND ListName = '{1}'", e.ItemID, e.ListName);
                var lstNotify = conn.Query<BeanNotify>(queryNotify);
                BeanWorkflowItem _temp = e;
                if (lstNotify != null && lstNotify.Count > 0)
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_temp, lstNotify[0], "FragmentHomePage");
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
                else
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_temp, null, "FragmentHomePage");
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Click_ItemWorkflowAdapter - Error: " + ex.Message);
#endif
            }
        }
        private void Scroll_LoadMoreVDT(object sender, AbsListView.ScrollEventArgs e)
        {
            try
            {
                if (_NotifyVDTAdapter != null)
                {
                    if ((_firstItemVDT != e.FirstVisibleItem))
                    {
                        _firstItemVDT = e.FirstVisibleItem;
                        if (e.FirstVisibleItem == (e.TotalItemCount - e.VisibleItemCount))
                        {
                            if (_footerLv == null)
                            {
                                _footerLv = _inflater.Inflate(Resource.Layout.ViewFooterList, null);
                            }

                            if (_checkMoreVDT)
                            {
                                _lvVDT.AddFooterView(_footerLv);
                                MoreData("VDT");
                            }
                        }
                        else
                        {
                            if (_footerLv != null)
                            {
                                _lvVDT.RemoveFooterView(_footerLv);
                            }
                        }
                    }
                    else if ((e.TotalItemCount <= e.VisibleItemCount))
                    {
                        _footerLv = _inflater.Inflate(Resource.Layout.ViewFooterList, null);
                        if (_checkMoreVDT)
                        {
                            _lvVDT.AddFooterView(_footerLv);
                            MoreData("VDT");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - Scroll_LoadMoreVTBD - Error: " + ex.Message);
#endif
            }

        }
        private void Scroll_LoadMoreVTBD(object sender, AbsListView.ScrollEventArgs e)
        {
            try
            {
                if (_NotifyVTBDAdapter != null)
                {
                    if ((_firstItemVTBD != e.FirstVisibleItem))
                    {
                        _firstItemVTBD = e.FirstVisibleItem;
                        if (e.FirstVisibleItem == (e.TotalItemCount - e.VisibleItemCount))
                        {
                            if (_footerLv == null)
                            {
                                _footerLv = _inflater.Inflate(Resource.Layout.ViewFooterList, null);
                            }

                            if (_checkMoreVTBD)
                            {
                                _lvVTBD.AddFooterView(_footerLv);
                                MoreData("VTBD");
                            }
                        }
                        else
                        {
                            if (_footerLv != null)
                            {
                                _lvVTBD.RemoveFooterView(_footerLv);
                            }
                        }
                    }
                    else if ((e.TotalItemCount <= e.VisibleItemCount))
                    {
                        _footerLv = _inflater.Inflate(Resource.Layout.ViewFooterList, null);
                        if (_checkMoreVTBD)
                        {
                            _lvVTBD.AddFooterView(_footerLv);
                            MoreData("VTBD");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - MoreData - Error: " + ex.Message);
#endif
            }
        }
        private void MoreData(string type)
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                if (type == "VDT")
                {
                    List<BeanNotify> lstMore = pBase.LoadMoreDataT<BeanNotify>(_queryVDT, _limit, _limit, _lstVDT.Count);
                    List<BeanNotify> lstMore_Filter = CTRLHomePage.FilterListVDT_ByCondition(lstMore, _lstFilterCondition);
                    if (lstMore != null && lstMore.Count > 0)
                    {
                        _lstVDT.AddRange(lstMore);
                        _lstVDT_Filter.AddRange(lstMore_Filter);
                        _NotifyVDTAdapter.NotifyDataSetChanged();
                        _lvVDT.RemoveFooterView(_footerLv);
                        if (lstMore.Count < _limit)
                        {
                            _checkMoreVDT = false;
                        }
                        else
                        {
                            _checkMoreVDT = true;
                        }
                    }
                    else
                    {
                        _checkMoreVDT = false;
                        _lvVDT.RemoveFooterView(_footerLv);
                    }
                }
                else if (type == "VTBD")
                {
                    List<BeanWorkflowItem> lstMore = pBase.LoadMoreDataT<BeanWorkflowItem>(_queryVTBD, _limit, CmmVariable.SysConfig.LoginName, _limit, _lstVTBD.Count);
                    List<BeanWorkflowItem> lstMore_Filter = CTRLHomePage.FilterListVTBD_ByCondition(lstMore, _lstFilterCondition);
                    if (lstMore != null && lstMore.Count > 0)
                    {
                        _lstVTBD.AddRange(lstMore);
                        _lstVTBD_Filter.AddRange(lstMore_Filter);
                        _NotifyVTBDAdapter.NotifyDataSetChanged();
                        _lvVTBD.RemoveFooterView(_footerLv);
                        if (lstMore.Count < _limit)
                        {
                            _checkMoreVTBD = false;
                        }
                        else
                        {
                            _checkMoreVTBD = true;
                        }
                    }
                    else
                    {
                        _checkMoreVTBD = false;
                        _lvVTBD.RemoveFooterView(_footerLv);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - MoreData - Error: " + ex.Message);
#endif
            }
        }

        private void SetData()
        {
            try
            {
                _lvVDT.Divider = null;
                _lvVTBD.Divider = null;
                ProviderBase pBase = new ProviderBase();
                if (_type == "VDT")
                {
                    _lstVDT.Clear();
                    _lvVDT.Visibility = ViewStates.Visible;
                    _lvVTBD.Visibility = ViewStates.Gone;

                    _queryVDT = CTRLHomePage._queryVDT;
                    _lstVDT = pBase.LoadMoreDataT<BeanNotify>(_queryVDT, _limit, _limit, _lstVDT.Count);

                    if (_lstVDT != null)
                    {
                        if (_lstVDT.Count < _limit)
                        {
                            _checkMoreVDT = false;
                        }
                        else
                        {
                            _checkMoreVDT = true;
                        }
                        //_NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT, _mainAct);
                        _lstVDT_Filter = CTRLHomePage.FilterListVDT_ByCondition(_lstVDT, _lstFilterCondition);
                        _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT_Filter, _mainAct);
               

                        _NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                        _lvVDT.Adapter = _NotifyVDTAdapter;
                        //if (_lvVDT.Count == 0)
                        if (_lvVDT.Count == 0)
                        {
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvVDT.Visibility = ViewStates.Gone;
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                            }
                            else
                            {
                                _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                            }
                        }
                        else
                        {
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvVDT.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvVDT.Visibility = ViewStates.Gone;
                    }
                }
                else if (_type == "VTBD")
                {
                    _lstVTBD.Clear();
                    _lvVTBD.Visibility = ViewStates.Visible;
                    _lvVDT.Visibility = ViewStates.Gone;

                    _queryVTBD = CTRLHomePage._queryVTBD;
                    _lstVTBD = pBase.LoadMoreDataT<BeanWorkflowItem>(_queryVTBD, _limit, _limit, _lstVTBD.Count);
                    if (_lstVTBD != null)
                    {
                        if (_lstVTBD.Count < _limit)
                        {
                            _checkMoreVTBD = false;
                        }
                        else
                        {
                            _checkMoreVTBD = true;
                        }

                        _lstVTBD_Filter = CTRLHomePage.FilterListVTBD_ByCondition(_lstVTBD, _lstFilterCondition);
                        _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD_Filter, _mainAct);
                        _NotifyVTBDAdapter.CustomItemClick += Click_ItemWorkflowAdapter;
                        _lvVTBD.Adapter = _NotifyVTBDAdapter;
                        if (_lvVTBD.Count == 0)
                        {
                            _lnNoData.Visibility = ViewStates.Visible;
                            _lvVTBD.Visibility = ViewStates.Gone;
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                            }
                            else
                            {
                                _tvNoData.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                            }
                        }
                        else
                        {
                            _lnNoData.Visibility = ViewStates.Gone;
                            _lvVTBD.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        _lnNoData.Visibility = ViewStates.Visible;
                        _lvVTBD.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - PagerHomePageFragment - SetData - Error: " + ex.Message);
#endif
            }
        }
        public static Android.App.Fragment NewInstance(string type, List<KeyValuePair<string, string>> _lstFilterCondition)
        {
            PagerHomePageFragment fragment = new PagerHomePageFragment(type, _lstFilterCondition);
            return fragment;
        }
        public void Update()
        {
            try
            {
                SetData();
            }
            catch (Exception)
            {
                //
            }
        }

    }

    public interface IUpdateable
    {
        void Update();
    }
}