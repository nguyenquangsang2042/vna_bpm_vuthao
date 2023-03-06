using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Tablet.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using Refractored.Controls;
using static Android.App.DatePickerDialog;
using BPMOPMobile.Droid.Tablet;
using Android.Text;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    [Obsolete]
    public class FragmentHomePage : Android.App.Fragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private View _rootView, _footerLv;
        private CircleImageView _imgAvata;
        private TextView _tvNoDataVDT, _tvNoDataVTBD, _tvCreateTask;
        private ListView _lvVDT, _lvVTBD;
        private EditText _edtSearch;
        private LinearLayout _lnToolbar, _lnFilter, _lnContent, _lnVDT, _lnVTBD, _lnNoDataVDT, _lnNoDataVTBD, _lnBottomHome, _lnBottomVDT, _lnBottomVTBD, _lnBottomBoard,_lnCreateTask;
        private TextView _tvVDT, _tvVTBD, _tvToolbarNotification;
        private SwipeRefreshLayout _swipe;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private List<BeanNotify> _lstVDT = new List<BeanNotify>();
        private List<BeanNotify> _lstVDT_Filter = new List<BeanNotify>();
        private List<BeanWorkflowItem> _lstVTBD = new List<BeanWorkflowItem>();
        private List<BeanWorkflowItem> _lstVTBD_Filter = new List<BeanWorkflowItem>();
        private AdapterHomePageVDT _NotifyVDTAdapter;
        private AdapterHomePageVTBD _NotifyVTBDAdapter;
        private int _flagTask = 1; // 1 = việc đến tôi, 2 = việc tôi bắt đầu
        private int _flagDatePicker = 0; // 1 = Ngày gửi đến VDT Start | 2 = Ngày gửi đến VDT End | 3 = Ngày khởi tạo VTBD Start | 4 = Ngày khởi tạo VTBD End
        private string _queryVDT, _queryVTBD;
        private int _limit = 20, _firstItemVDT, _firstItemVTBD;
        private bool _checkMoreVDT, _checkMoreVTBD;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _inflater = inflater;
            if (_rootView == null)
            {
                _mainAct.Window.SetNavigationBarColor(Color.Black);
                _rootView = inflater.Inflate(Resource.Layout.ViewHomePage, null);
                _imgAvata = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewHomePage_Avata);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewHomePage_Search); 
                _tvCreateTask = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_Toolbar_Create);
                _tvVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VDT);
                _tvVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VTBD);
                _tvNoDataVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VDT_NoData);
                _tvNoDataVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_VTBD_NoData);
                _tvToolbarNotification = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewHomePage_Toolbar_Notification);  
                _lnCreateTask = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Toolbar_Create);
                _lnToolbar = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Toolbar);
                _lnBottomHome = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Bottom_Home);
                _lnBottomVDT = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Bottom_VDT);
                _lnBottomVTBD = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Bottom_VTBD);
                _lnBottomBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_Bottom_Board);
                _lnNoDataVDT = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_VDT_NoData);
                _lnNoDataVTBD = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_VTBD_NoData);
                _lnVDT = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_VDT);
                _lnVTBD = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewHomePage_VTBD);
                _lvVDT = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewHomePage_VDT);
                _lvVTBD = _rootView.FindViewById<ListView>(Resource.Id.lv_ViewHomePage_VTBD);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewHomePage);
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                _swipe.Enabled = true;
                _imgAvata.Click += Click_Menu;
                _swipe.Refresh += Swipe_RefreshData;
                _tvVDT.Click += Click_tvVDT;
                _tvVTBD.Click += Click_tvVTBD;
                _edtSearch.TextChanged += TextChanged_Search;
                _lnCreateTask.Click += Click_CreateTask;
                _lnBottomVDT.Click += Click_BottomVDT;
                _lnBottomVTBD.Click += Click_BottomVTBD;
                _lnBottomBoard.Click += Click_BottomBoard;
                _lvVDT.Scroll += Scroll_LoadMoreVDT;
                _lvVTBD.Scroll += Scroll_LoadMoreLvVTBD;
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvata);
                SetData();
                LoadList();
                SetViewByLanguage();
            }
            else
            {

            }
            MinionAction._FlagNavigation = 1; // Home
            MinionAction.OnRefreshFragmentLeftMenu(null, null);
            MinionAction.RefreshFragmentHomePage -= RefreshFragmentHomePage;
            MinionAction.RefreshFragmentHomePage += RefreshFragmentHomePage;
            CmmEvent.UpdateLangComplete += ChangeLanguage;
            return _rootView;
        }

        public FragmentHomePage(bool v, bool checkAotoLogin)
        {
        }
        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode == "VN")
                {
                    _edtSearch.Hint = CmmFunction.GetTitle("K_Mess_NoData", "Tìm kiếm ..."); 
                    _tvCreateTask.Text = CmmFunction.GetTitle("K_Mess_NoData", "Tạo mới");
                    _tvVDT.Text.Replace("To me", "Đến tôi");
                    _tvVTBD.Text.Replace("From me", "Tôi bắt đầu");
                    _tvVDT.Text = CmmFunction.GetTitle("K_Mess_ViecDenToi", "Đến tôi " + CmmDroidFunction.GetCountNumOfText(_tvVDT.Text));
                    _tvVTBD.Text = CmmFunction.GetTitle("K_Mess_ViecToiBatDau", "Tôi bắt đầu " + CmmDroidFunction.GetCountNumOfText(_tvVTBD.Text));
                    _tvNoDataVDT.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                    _tvNoDataVTBD.Text = CmmFunction.GetTitle("K_Mess_NoData", "Không có dữ liệu");
                }
                else
                {
                    _edtSearch.Hint = CmmFunction.GetTitle("K_Mess_NoData", "Searching ...");
                    _tvCreateTask.Text = CmmFunction.GetTitle("K_Mess_NoData", "Create");
                    _tvVDT.Text.Replace("Đến tôi", "To me");
                    _tvVTBD.Text.Replace("Tôi bắt đầu", "From me");
                    _tvVDT.Text = CmmFunction.GetTitle("K_Mess_ViecDenToi", "To me " + CmmDroidFunction.GetCountNumOfText(_tvVDT.Text));
                    _tvVTBD.Text = CmmFunction.GetTitle("K_Mess_ViecToiBatDau", "From me " + CmmDroidFunction.GetCountNumOfText(_tvVTBD.Text));
                    _tvNoDataVDT.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                    _tvNoDataVTBD.Text = CmmFunction.GetTitle("K_Mess_NoData", "No data");
                }
                // Tô màu đỏ
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, _tvVDT.Text, "(", ")");
                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, _tvVTBD.Text, "(", ")");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - SetView - Error: " + ex.Message);
#endif
            }
        }
        private void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                MinionAction._FlagNavigation = 1; // Home
                MinionAction.OnRefreshFragmentLeftMenu(null, null);
                _mainAct.OpenDawer();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - SetView - Error: " + ex.Message);
#endif
            }
        }
        private void Click_tvVDT(object sender, EventArgs e)
        {
            try
            {
                CTRLHomePage.SetTextview_Selected_Tablet(_mainAct, _tvVDT);
                CTRLHomePage.SetTextview_NotSelected_Tablet(_mainAct, _tvVTBD);
                _flagTask = 1;
                _lnVDT.Visibility = ViewStates.Visible;
                _lnVTBD.Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_tvVDT - Error: " + ex.Message);
#endif
            }
        }
        private void Click_tvVTBD(object sender, EventArgs e)
        {
            try
            {
                CTRLHomePage.SetTextview_Selected_Tablet(_mainAct, _tvVTBD);
                CTRLHomePage.SetTextview_NotSelected_Tablet(_mainAct, _tvVDT);
                _flagTask = 2;
                _lnVDT.Visibility = ViewStates.Gone;
                _lnVTBD.Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_tvVTBD - Error: " + ex.Message);
#endif
            }
        }
        private void Click_CreateTask(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_CreateTask - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomVDT(object sender, EventArgs e)
        {
            try
            {
                MinionAction._FlagNavigation = 2; // VDT
                MinionAction.OnRefreshFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomVTBD(object sender, EventArgs e)
        {
            try
            {
                MinionAction._FlagNavigation = 3; // VTBD
                MinionAction.OnRefreshFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }
        private void Click_BottomBoard(object sender, EventArgs e)
        {
            try
            {
                //MinionAction._FlagNavigation = 4; // VDT
                // MinionAction.OnRefreshFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomBoard - Error: " + ex.Message);
#endif
            }
        }
        private void TextChanged_Search(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtSearch.Text)) // empty -> tra lai ban dau
                {
                    SetData();
                    LoadList(); // Load lai tu dau
                }
                else
                {
                    ProviderBase pBase = new ProviderBase();
                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    string _content = BPMOPMobile.Class.CmmFunction.removeSignVietnamese(_edtSearch.Text).ToLowerInvariant().Trim();

                    if (_flagTask==1) // VDT
                    {
                        _lvVDT.Divider = null;
                        _lstVDT_Filter.Clear();

                        _queryVDT = CTRLHomePage._queryVDT_Count;
                        _lstVDT_Filter = conn.Query<BeanNotify>(_queryVDT);
                        _lstVDT_Filter = _lstVDT_Filter.Where(x => x.DocumentID.ToString().Contains(_content) || x.Title.ToString().Contains(_content)).ToList();

                        if (_lstVDT_Filter != null)
                        {
                            _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT_Filter, _mainAct);
                            ////_NotifyVDTAdapter.CustomItemClick -= Click_ItemNotifyAdapter;
                            ////_NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                            _lvVDT.Adapter = _NotifyVDTAdapter;

                            if (_lstVDT_Filter.Count == 0)
                            {
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (0)", "(", ")");
                                _lnNoDataVDT.Visibility = ViewStates.Visible;
                                _lvVDT.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                if (_lstVDT_Filter.Count > 99)
                                {
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (99+)", "(", ")");
                                }
                                else
                                {
                                    _tvToolbarNotification.Text = _lstVDT_Filter.Count.ToString();
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (" + _lstVDT_Filter.Count + ")", "(", ")");
                                }
                                _lnNoDataVDT.Visibility = ViewStates.Gone;
                                _lvVDT.Visibility = ViewStates.Visible;
                            }
                        }
                        else
                        {
                            CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (0)", "(", ")");
                            _lnNoDataVDT.Visibility = ViewStates.Visible;
                            _lvVDT.Visibility = ViewStates.Gone;
                        }
                    }
                    else // VTBD
                    {
                        #region Load List VTBD
                        _lvVTBD.Divider = null;
                        _lstVTBD_Filter.Clear();

                        _queryVTBD = CTRLHomePage._queryVTBD_Count;
                        _lstVTBD_Filter = conn.Query<BeanWorkflowItem>(_queryVTBD);
                        _lstVTBD_Filter = _lstVTBD_Filter.Where(x => x.WorkflowID.ToString().Contains(_content) || x.Content.ToString().Contains(_content)).ToList();

                        if (_lstVTBD_Filter != null)
                        {
                            _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD, _mainAct);
                            ////_NotifyVDTAdapter.CustomItemClick -= Click_ItemNotifyAdapter;
                            ////_NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                            _lvVTBD.Adapter = _NotifyVTBDAdapter;

                            if (_lstVTBD_Filter.Count == 0)
                            {
                                CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (0)", "(", ")");
                                _lnNoDataVTBD.Visibility = ViewStates.Visible;
                                _lvVTBD.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                if (_lstVTBD_Filter.Count > 99)
                                {
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (99+)", "(", ")");
                                }
                                else
                                {
                                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (" + _lstVTBD_Filter.Count + ")", "(", ")");
                                }
                                _lnNoDataVTBD.Visibility = ViewStates.Gone;
                                _lvVTBD.Visibility = ViewStates.Visible;
                            }
                        }
                        else
                        {
                            CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (0)", "(", ")");
                            _lnNoDataVTBD.Visibility = ViewStates.Visible;
                            _lvVTBD.Visibility = ViewStates.Gone;
                        }

                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - TextChanged_Search - Error: " + ex.Message);
#endif
            }
        }


        private async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                SetViewTouchable(false);
                await Task.Run(() =>
                {
                    CTRLHomePage.InitListFilterCondition("BOTH");

                    SQLite.SQLiteConnection con = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    con.Execute(CTRLHomePage._QueryDeleteNotify);
                    con.Close();

                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateMasterData<BeanNotify>(null, false, 30, true);
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);
                    _mainAct.RunOnUiThread(() =>
                    {
                        SetData();
                        LoadList();
                        SetViewByLanguage();
                        MinionAction.OnRefreshFragmentViewPager(null, null);
                    });
                });
                _swipe.Refreshing = false;
                SetViewTouchable(true);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.LogErr(ex, "MainView - refesh_data - Er: " + ex);
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                    SetViewTouchable(true);
                });
            }
        }
        private async void SyncData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                SetViewTouchable(false);
                await Task.Run(() =>
                {
                    CTRLHomePage.InitListFilterCondition("BOTH");

                    SQLite.SQLiteConnection con = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                    con.Execute(CTRLHomePage._QueryDeleteNotify);

                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateMasterData<BeanNotify>(null, false, 30, true);
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);
                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnFilter.SetBackgroundResource(Resource.Drawable.textcornerviolet2);
                        SetData();
                        SetViewByLanguage();
                        MinionAction.OnRefreshFragmentViewPager(null, null);
                    });
                });
                _swipe.Refreshing = false;
                SetViewTouchable(true);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.LogErr(ex, "MainView - refesh_data - Er: " + ex);
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                    SetViewTouchable(true);
                });
            }
        }

        #endregion
        private void SetData()
        {
            try
            {
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

                #region Count VDT

                string _queryTest = @"SELECT * FROM BeanNotify";

                List<BeanNotify> _lstTest = conn.Query<BeanNotify>(_queryTest);

                string _queryVDT = CTRLHomePage._queryVDT_Count;
                var _lstVDTCount = conn.Query<BeanNotify>(_queryVDT);
                if (_lstVDTCount != null && _lstVDTCount.Count >= 0)
                {
                    if (_lstVDTCount.Count > 99)
                    {
                        _tvToolbarNotification.Text = "99";
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (99+)", "(", ")");
                    }
                    else
                    {
                        _tvToolbarNotification.Text = _lstVDTCount.Count.ToString();
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (" + _lstVDTCount.Count + ")", "(", ")");
                    }
                }
                else
                {
                    _tvToolbarNotification.Text = "0";
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVDT, "Đến tôi (0)", "(", ")");
                }
                #endregion

                #region Count VTBD
                string _queryVTBD = CTRLHomePage._queryVTBD_Count;
                var _lstVTBDCount = conn.Query<BeanWorkflowItem>(_queryVTBD);
                if (_lstVTBDCount != null)
                {
                    if (_lstVTBDCount.Count > 99)
                    {
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (99+)", "(", ")");
                    }
                    else
                    {
                        CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (" + _lstVTBDCount.Count + ")", "(", ")");
                    }
                }
                else
                {
                    CmmDroidFunction.SetTextViewHighlightColor(_mainAct, _tvVTBD, "Tôi bắt đầu (0)", "(", ")");
                }
                #endregion

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - SetData - Error: " + ex.Message);
#endif
            }
        }
        private void LoadList()
        {
            try
            {
                #region Load List VDT
                _lvVDT.Divider = null;
                _lstVDT.Clear();

                ProviderBase pBase = new ProviderBase();
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
                    _NotifyVDTAdapter = new AdapterHomePageVDT(_rootView.Context, _lstVDT, _mainAct);
                    ////_NotifyVDTAdapter.CustomItemClick -= Click_ItemNotifyAdapter;
                    ////_NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                    _lvVDT.Adapter = _NotifyVDTAdapter;

                    if (_lstVDT.Count == 0)
                    {
                        _lnNoDataVDT.Visibility = ViewStates.Visible;
                        _lvVDT.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        _lnNoDataVDT.Visibility = ViewStates.Gone;
                        _lvVDT.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    _lnNoDataVDT.Visibility = ViewStates.Visible;
                    _lvVDT.Visibility = ViewStates.Gone;
                }

                #endregion

                #region Load List VTBD
                _lvVTBD.Divider = null;
                _lstVTBD.Clear();

                pBase = new ProviderBase();
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
                    _NotifyVTBDAdapter = new AdapterHomePageVTBD(_rootView.Context, _lstVTBD, _mainAct);
                    ////_NotifyVDTAdapter.CustomItemClick -= Click_ItemNotifyAdapter;
                    ////_NotifyVDTAdapter.CustomItemClick += Click_ItemNotifyAdapter;
                    _lvVTBD.Adapter = _NotifyVTBDAdapter;

                    if (_lstVTBD.Count == 0)
                    {
                        _lnNoDataVTBD.Visibility = ViewStates.Visible;
                        _lvVTBD.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        _lnNoDataVTBD.Visibility = ViewStates.Gone;
                        _lvVTBD.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    _lnNoDataVTBD.Visibility = ViewStates.Visible;
                    _lvVTBD.Visibility = ViewStates.Gone;
                }

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - LoadList - Error: " + ex.Message);
#endif
            }
        }
        private void Scroll_LoadMoreLvVTBD(object sender, AbsListView.ScrollEventArgs e)
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
                Console.WriteLine("Author: khoahd - FragmentHomePage - Scroll_LoadMoreLvVTBD - Error: " + ex.Message);
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
                Console.WriteLine("Author: khoahd - FragmentHomePage - Scroll_LoadMoreVDT - Error: " + ex.Message);
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
                    if (lstMore != null && lstMore.Count > 0)
                    {
                        _lstVDT.AddRange(lstMore);
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
                    if (lstMore != null && lstMore.Count > 0)
                    {
                        _lstVTBD.AddRange(lstMore);
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
        private void RefreshFragmentHomePage(object arg1, EventArgs arg2)
        {
            try
            {
                Swipe_RefreshData(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - RefreshFragmentHomePage - Error: " + ex.Message);
#endif
            }
        }
        private void ChangeLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetViewByLanguage();                    
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - ChangeLanguage - Error: " + ex.Message);
#endif
            }
        }
        private void SetViewTouchable(bool flag)
        {
            try
            {
                _imgAvata.Enabled = flag;
                _tvVDT.Enabled = flag;
                _tvVTBD.Enabled = flag;
                _lnFilter.Enabled = flag;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - SetViewTouchable - Error: " + ex.Message);
#endif
            }
        }
    }
}