using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allyants.BoardViewLib;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using SQLite;
using static BPMOPMobile.Droid.Presenter.Adapter.AdapterBoardDetailGroup;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    public class FragmentBoardDetailGroup_Backup : CustomBaseFragment, BoardView.IDragItemStartCallback, BoardView.IItemClickListener
    {
        private LayoutInflater _inflater;
        private Dialog _dialogAction;
        private MainActivity _mainAct;
        private View _rootView;
        private EditText _edtSearch;
        private TextView _tvTitle, _tvNoDataBoard, _tvNoDataList, _tvNoDataReport, _tvBoard, _tvList, _tvReport;
        private ImageView _imgBack, _imgBoard, _imgList, _imgReport, _imgDelete;
        public RelativeLayout _relaToolBar, _relaBoard, _relaList, _relaReport, _relaDataBoard, _relaDataList, _relaDataReport;
        public LinearLayout _lnFilter, _lnBlackFilter, _lnNoDataBoard, _lnNoDataList, _lnNoDataReport, _lnBottomNavigation;
        private View _vwBoard, _vwList, _vwReport;
        private SwipeRefreshLayout _swipe;
        private MyCustomBoardView _boardView;
        private RecyclerView _recyList;

        private AdapterBoardKanbanView _adapterBoardKanBan;
        private AdapterHomePageRecyVDT_Ver2 _adapterRecyList;
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private ControllerBoard CTRLBoard = new ControllerBoard();

        private List<BeanWorkflowStepDefine> _lstStepDefine = new List<BeanWorkflowStepDefine>(); // Chỉ bao gồm bước của Quy Trình
        private List<BeanBoardKanBan> _lstBoardKanBan_Full = new List<BeanBoardKanBan>();

        private BeanWorkflow _beanWorkflow = new BeanWorkflow();
        private int _flagCurrentPage = 1; // 1 = Board, 2 = List, 3 = Report

        private const int _ApprovedStepID = -1;
        private const int _RejectedStepID = -2;

        #region Life Cycle
        public FragmentBoardDetailGroup_Backup() { }

        public FragmentBoardDetailGroup_Backup(BeanWorkflow _beanWorkflow, int _flagCurrentPage)
        {
            this._beanWorkflow = _beanWorkflow;
            this._flagCurrentPage = _flagCurrentPage;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MinionAction.RenewItem_AfterFollowEvent -= MinionAction_RenewItem_AfterFollowEvent;
            MinionAction.RenewFragmentBoardDetailGroup -= MinionAction_FragmentBoardDetailGroup;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _rootView = inflater.Inflate(Resource.Layout.ViewBoardDetailGroup, null);
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView != null)
            {
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Back);

                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewBoardDetailGroup);
                _relaToolBar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Toolbar);
                _lnBlackFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_BlackFilter);
                _lnFilter = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Filter);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewBoardDetailGroup_Search);
                _imgDelete = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Search_Delete);
                _tvBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Board);
                _tvList = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_List);
                _tvReport = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Report);
                _imgBoard = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Board);
                _imgList = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_List);
                _imgReport = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoardDetailGroup_Report);
                _boardView = _rootView.FindViewById<MyCustomBoardView>(Resource.Id.boardView_ViewBoardDetailGroup);
                _recyList = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewBoardDetailGroup_List);
                _relaBoard = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Board);
                _relaList = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_List);
                _relaReport = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Report);
                _relaDataBoard = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_Board);
                _relaDataList = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_List);
                _relaDataReport = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoardDetailGroup_Data_Report);
                _lnNoDataBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_Board_NoData);
                _lnNoDataList = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_List_NoData);
                _lnNoDataReport = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_Data_Report_NoData);
                _tvNoDataBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Board_NoData);
                _tvNoDataList = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_List_NoData);
                _tvNoDataReport = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoardDetailGroup_Data_Report_NoData);
                _vwBoard = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Board);
                _vwList = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_List);
                _vwReport = _rootView.FindViewById<View>(Resource.Id.vw_ViewBoardDetailGroup_Report);

                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoardDetailGroup_BottomNavigation);
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                _swipe.Refresh += Swipe_RefreshData;
                _imgBack.Click += Click_imgBack;
                _imgDelete.Click += Click_imgDelete;
                _lnFilter.Click += Click_lnFilter;
                _relaBoard.Click += Click_relaBoard;
                _relaList.Click += Click_relaList;
                _relaReport.Click += Click_relaReport;
                _edtSearch.TextChanged += TextChanged_EdtSearch;
                _edtSearch.EditorAction += EditorAction_EdtSearch;

                _boardView.SetOnItemClickListener(this);
                _boardView.SetOnDragItemListener(this);

                SetViewNavigationByCurrentPage(); // Set View bằng _flagCurrentPage
                _imgDelete.Visibility = ViewStates.Gone;

                MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board;
                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation);
                bottomNavigation.InitializeView();
            }
            SetViewByLanguage();
            SetListWorkflowStepDefine();
            Action action = new Action(() =>
            {
                SetData();
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 500);
            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board;
            MinionAction.RenewItem_AfterFollowEvent += MinionAction_RenewItem_AfterFollowEvent;
            MinionAction.RenewFragmentBoardDetailGroup += MinionAction_FragmentBoardDetailGroup;
            return _rootView;
        }

        #endregion

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (_beanWorkflow != null)
                {
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                    _tvNoDataList.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                        _tvTitle.Text = !string.IsNullOrEmpty(_beanWorkflow.Title) ? _beanWorkflow.Title : "";
                    else
                        _tvTitle.Text = !string.IsNullOrEmpty(_beanWorkflow.TitleEN) ? _beanWorkflow.TitleEN : "";
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        /// <summary>
        /// Chuyễn trang khi click Tab
        /// </summary>
        private void SetViewNavigationByCurrentPage()
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                switch (_flagCurrentPage)
                {
                    case 1:
                        {
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgBoard, _tvBoard, _vwBoard);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgList, _tvList, _vwList, 2);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgReport, _tvReport, _vwReport, 3);

                            _relaDataBoard.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                            _relaDataBoard.Visibility = ViewStates.Visible;
                            _relaDataList.Visibility = ViewStates.Gone;
                            _relaDataReport.Visibility = ViewStates.Gone;
                            break;
                        }
                    case 2:
                        {
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgBoard, _tvBoard, _vwBoard, 1);
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgList, _tvList, _vwList);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgReport, _tvReport, _vwReport, 3);

                            _relaDataList.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in);
                            _relaDataBoard.Visibility = ViewStates.Gone;
                            _relaDataList.Visibility = ViewStates.Visible;
                            _relaDataReport.Visibility = ViewStates.Gone;
                            break;
                        }
                    case 3:
                        {
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgBoard, _tvBoard, _vwBoard, 1);
                            CTRLBoard.SetViewCurrentPage_NotSelected(_mainAct, _imgList, _tvList, _vwList, 2);
                            CTRLBoard.SetViewCurrentPage_Selected(_mainAct, _imgReport, _tvReport, _vwReport);

                            _relaDataBoard.Visibility = ViewStates.Gone;
                            _relaDataList.Visibility = ViewStates.Gone;
                            _relaDataReport.Visibility = ViewStates.Visible;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void Click_imgBack(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Back", ex);
#endif
            }
        }

        private void Click_imgDelete(object sender, EventArgs e)
        {
            try
            {
                _edtSearch.Text = "";
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Click_Back", ex);
#endif
            }
        }

        private void Click_lnFilter(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                //LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);
                //SharedView_PopupFilterBoard sharedView_PopupFilterBoard = new SharedView_PopupFilterBoard(_layoutInflater, _mainAct, this, "FragmentBoardDetailGroup", _rootView);
                //sharedView_PopupFilterBoard.InitializeValue(CTRLBoard, (int)SharedView_PopupFilterBoard.FlagViewFilterBoard.BoardDetailGroup);
                //sharedView_PopupFilterBoard.InitializeView();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
            }
        }

        private void Click_relaBoard(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 1;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaBoard", ex);
#endif
            }
        }

        private void Click_relaList(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 2;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaList", ex);
#endif
            }
        }

        private void Click_relaReport(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _flagCurrentPage = 3;
                    SetViewNavigationByCurrentPage();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_relaReport", ex);
#endif
            }
        }

        private void Click_ItemList(object sender, BeanAppBaseExt e)
        {
            Handle_ClickItemAppBase(e);
        }

        private void TextChanged_EdtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.Text.ToString())) // Không Search
                {
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                    _imgDelete.Visibility = ViewStates.Gone;
                }
                else
                {
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
                    _imgDelete.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_EdtSearch", ex);
#endif
            }
        }

        private void EditorAction_EdtSearch(object sender, TextView.EditorActionEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next) // Bấm nút Done trên bàn phím
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    if (!String.IsNullOrEmpty(_edtSearch.Text))
                    {
                        FilterData();
                    }
                    else
                    {
                        SetData_Board(_lstBoardKanBan_Full);
                        SetData_List(_lstBoardKanBan_Full);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "EditorAction_EdtSearch", ex);
#endif
            }
        }

        private async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                await Task.Run(() =>
                {
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
                        _flagCurrentPage = 1;
                        SetViewByLanguage();
                        SetViewNavigationByCurrentPage();
                        SetListWorkflowStepDefine();
                        SetData(_updateMasterData: false);
                    });
                });
                _swipe.Refreshing = false;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Swipe_RefreshData", ex);
#endif
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                });
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// Hàm Gọi Data, nếu _updateMasterData = false thì ko gọi API
        /// </summary>
        /// <param name="_updateMasterData"></param>
        private async void SetData(bool _updateMasterData = true)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            ProviderBase pBase = new ProviderBase();
            try
            {
                if (_updateMasterData)
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                _lstBoardKanBan_Full.Clear();

                await Task.Run(() =>
                {
                    if (_updateMasterData)
                        pBase.UpdateMasterData<BeanAppBase>();

                    string _queryAppBase = "";
                    foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                    {
                        BeanBoardKanBan _tempBeanBoardKanBan = new BeanBoardKanBan { itemStepDefine = item, lstAppBase = new List<BeanAppBaseExt>() };

                        switch (item.WorkflowStepDefineID)
                        {
                            case _ApprovedStepID:// Phê duyệt
                                {
                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                     WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                     ORDER BY Created DESC",
                                                                     _beanWorkflow.WorkflowID, String.Join(",", CTRLBoard.ApprovedListID));
                                    break;
                                }
                            case _RejectedStepID: // Từ chối
                                {
                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                    WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                    ORDER BY Created DESC",
                                                                    _beanWorkflow.WorkflowID, String.Join(",", CTRLBoard.RejectedListID));
                                    break;
                                }
                            default:
                                {
                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                    WHERE WorkflowID = {0} AND Step = {1} AND ResourceCategoryId <> 16 AND StatusGroup NOT IN ({2})
                                                                    ORDER BY Created DESC",
                                                                    _beanWorkflow.WorkflowID, item.Step, String.Join(",", CTRLBoard.ApprovedListID.Concat(CTRLBoard.RejectedListID).ToArray()));
                                    break;
                                }
                        }

                        List<BeanAppBaseExt> _lstAppBaseByStep = conn.Query<BeanAppBaseExt>(_queryAppBase);
                        _lstAppBaseByStep = CTRLBoard.FilterListAppbaseByCondition(_lstAppBaseByStep, CTRLBoard.GetCurrentValue_Filter());

                        _lstBoardKanBan_Full.Add(new BeanBoardKanBan() { itemStepDefine = item, lstAppBase = _lstAppBaseByStep });
                    }

                    _mainAct.RunOnUiThread(() =>
                    {
                        SetData_Board(_lstBoardKanBan_Full);
                        SetData_List(_lstBoardKanBan_Full);
                        if (_updateMasterData)
                            CmmDroidFunction.HideProcessingDialog();
                    });
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    if (_updateMasterData)
                        CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void SetListWorkflowStepDefine()
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                string _queryStepDefine = String.Format(CTRLBoard._QueryStepDefine, _beanWorkflow.WorkflowID);
                _lstStepDefine.Clear();
                _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = _ApprovedStepID, Title = "Phê duyệt" });
                _lstStepDefine.Add(new BeanWorkflowStepDefine() { WorkflowStepDefineID = _RejectedStepID, Title = "Từ chối" });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetListWorkflowStepDefine", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public void FilterData()
        {
            try
            {
                List<BeanBoardKanBan> _lstFilter_Kanban = new List<BeanBoardKanBan>();

                foreach (BeanBoardKanBan item in _lstBoardKanBan_Full)
                {
                    BeanBoardKanBan _tempItemList = new BeanBoardKanBan
                    {
                        itemStepDefine = item.itemStepDefine,
                        lstAppBase = CTRLBoard.FilterListAppbaseByCondition(item.lstAppBase, CTRLBoard.GetCurrentValue_Filter(), _edtSearch.Text)
                    };

                    _lstFilter_Kanban.Add(_tempItemList);
                }
                SetData_Board(_lstFilter_Kanban);
                SetData_List(_lstFilter_Kanban);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "FilterData", ex);
#endif
            }
        }

        private void SetData_Board(List<BeanBoardKanBan> _lstBoard)
        {
            try
            {
                if (_lstBoard != null && _lstBoard.Count > 0)
                {
                    _adapterBoardKanBan = new AdapterBoardKanbanView(_mainAct, _rootView.Context, _lstBoard);
                    _boardView.SetAdapter(_adapterBoardKanBan);
                }
                else
                {
                    _boardView.Visibility = ViewStates.Gone;
                    _lnNoDataBoard.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_Board", ex);
#endif
            }
        }

        private void SetData_List(List<BeanBoardKanBan> _lstBoard)
        {
            try
            {
                if (_lstBoard != null && _lstBoard.Count > 0)
                {
                    List<BeanAppBaseExt> _lstAppBase = new List<BeanAppBaseExt>();
                    for (int i = 0; i < _lstBoard.Count; i++)
                    {
                        _lstAppBase.AddRange(_lstBoard[i].lstAppBase);
                    }

                    if (_lstAppBase.Count == 0)
                    {
                        _recyList.Visibility = ViewStates.Gone;
                        _lnNoDataList.Visibility = ViewStates.Visible;
                        return;
                    }

                    _recyList.Visibility = ViewStates.Visible;
                    _lnNoDataList.Visibility = ViewStates.Gone;

                    _adapterRecyList = new AdapterHomePageRecyVDT_Ver2(_mainAct, _rootView.Context, _lstAppBase.OrderByDescending(x => x.Created).ToList(), (int)AdapterHomePageRecyVDT_Ver2.SessionCategory.InProcess_Local);  // phải sắp xếp theo ngày
                    _adapterRecyList.CustomItemClick -= Click_ItemList;
                    _adapterRecyList.CustomItemClick += Click_ItemList;
                    _recyList.SetAdapter(_adapterRecyList);
                    _recyList.SetLayoutManager(new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical));
                }
                else
                {
                    _recyList.Visibility = ViewStates.Gone;
                    _lnNoDataList.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_List", ex);
#endif
            }
        }

        private void Handle_ClickItemAppBase(BeanAppBaseExt e)
        {
            if (CmmDroidFunction.PreventMultipleClick() == false) return;

            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                //Dictionary<string, string> _dictURL = CTRLHomePage.GetDictionaryFromAppBaseURL(e.ItemUrl);

                //if (!_dictURL.ContainsKey("TID")) // Task không dc click
                //{
                //    string _workflowItemID = "";
                //    if (_dictURL.ContainsKey("rid"))
                //        _workflowItemID = _dictURL["rid"];
                //    else
                //        _workflowItemID = _dictURL["ItemId"];

                //    // Query Workflow Item
                //    string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                //    List<BeanWorkflowItem> lstWorkflowItem = conn.Query<BeanWorkflowItem>(_query);
                //    if (lstWorkflowItem != null & lstWorkflowItem.Count > 0)
                //    {
                //        FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(lstWorkflowItem[0], null, "FragmentBoardDetailGroup");
                //        _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                //    }
                //    else
                //    {
                //        FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(null, null, "FragmentBoardDetailGroup");
                //        _mainAct.AddFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                //    }
                //}
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Handle_ClickItemAppBase", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Renew lại data khi FragmentDetailWorkflow send Action đi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinionAction_FragmentBoardDetailGroup(object sender, EventArgs e)
        {
            try
            {
                // Vì đã gọi API bên trang DetailWorkflow trước khi back về -> chỉ cần SetData
                SetData(_updateMasterData: false);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_FragmentBoardDetailGroup", ex);
#endif
            }
        }

        /// <summary>
        /// Renew Data nếu Follow/ Unfollow Từ FragmentDetailWorkflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinionAction_RenewItem_AfterFollowEvent(object sender, MinionAction.RenewItem_AfterFollow e)
        {
            try
            {
                SetData(false);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "MinionAction_RenewItem_AfterFollowEvent", ex);
#endif
            }
        }

        #region KanBan Board override

        /// <summary>
        /// Trản lại trạng thái trước khi Drag
        /// </summary>
        private void RollbackData_ViewBoard()
        {
            try
            {
                _boardView.Enabled = false;
                SetData_Board(_adapterBoardKanBan.GetListData().ToList());
                _boardView.Enabled = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Rollback_ViewBoard", ex);
#endif
            }
        }

        public void OnClick(View view, int columnPosition, int itemPosition)
        {
            try
            {
                BeanAppBaseExt _clickedItem = _adapterBoardKanBan.GetItemByPostion(columnPosition, itemPosition);
                Handle_ClickItemAppBase(_clickedItem);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnClick", ex);
#endif
            }

        }

        public void ChangedPosition(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn)
        {

        }

        public void StartDrag(View itemView, int originalPosition, int originalColumn)
        {

        }

        public void Dragging(View itemView, MotionEvent @event)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        public void EndDrag(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn)
        {
            try
            {
                //_adapterBoardKanBan
                if (CTRLBoard.CheckAppHasConnection() == false)
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_OFFLINE", "Bạn đang ở chế độ offline"),
                                            CmmFunction.GetTitle("TEXT_OFFLINE", "You are in offline mode"));
                    RollbackData_ViewBoard();
                    return;
                }
                if (originalColumn >= _adapterBoardKanBan.GetListData().Count - 2) // Không được thao tác 2 cột cuối: Từ chối - Phê duyệt
                {
                    RollbackData_ViewBoard();
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_BOARD_APPROVE_REJECT", "Không được thao tác trên cột Phê duyệt và Từ chối"),
                                                               CmmFunction.GetTitle("MESS_BOARD_APPROVE_REJECT", "Can't perform this action on rejected and approved column"));
                }
                else
                {
                    if (System.Math.Abs(newColumn - originalColumn) >= 2) // không cho skip - back quá 1 bước
                    {
                        RollbackData_ViewBoard();
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_BOARD_AJACENTSTEP", "Chỉ được thao tác trên hai bước liền kề"),
                        CmmFunction.GetTitle("MESS_BOARD_AJACENTSTEP", "Can only perform action on two adjacent steps"));
                        return;
                    }

                    if (newColumn > originalColumn) // Drag qua phải
                    {
                        BeanAppBaseExt _itemAction = _adapterBoardKanBan.GetListData()[originalColumn].lstAppBase[originalPosition];
                        RollbackData_ViewBoard();
                        HandleDragAction(_itemAction, true);
                    }
                    else if (newColumn < originalColumn) // Drag qua trái
                    {
                        BeanAppBaseExt _itemAction = _adapterBoardKanBan.GetListData()[originalColumn].lstAppBase[originalPosition];
                        RollbackData_ViewBoard();
                        HandleDragAction(_itemAction, false);
                    }
                    else if (newColumn == originalColumn) // Drag tại chỗ -> rollback
                        RollbackData_ViewBoard();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "EndDrag", ex);
#endif
            }
        }

        public async void HandleDragAction(BeanAppBaseExt _itemAction, bool IsNextAction)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                // Query WorkflowItem
                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_itemAction.ItemUrl);

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
                if (lstWorkflowItem == null || lstWorkflowItem.Count == 0) // Không tìm thấy WorkflowItem
                {
                    return;
                }

                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                await Task.Run(() =>
                {
                    #region Prepare Data
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                    string _resultString = _pControlDynamic.GetTicketRequestControlDynamicForm(lstWorkflowItem[0], CmmVariable.SysConfig.LangCode); // List Form control
                    JObject _OBJFORMACTION = JObject.Parse(_resultString);
                    JArray jArrayForm = JArray.Parse(_OBJFORMACTION["form"].ToString());

                    ViewRow _LISTACTION = JsonConvert.DeserializeObject<ViewRow>(_OBJFORMACTION["action"].ToString());
                    string _formDefineInfo = jArrayForm[0]["FormDefineInfo"].ToString();
                    #endregion

                    #region Check Validate Button Action
                    ButtonAction buttonAction = null;
                    foreach (ViewElement item in _LISTACTION.Elements)
                    {
                        if (IsNextAction == true && item.Value.ToLowerInvariant().Equals("next")) // kiểm tra xem có action Next không
                        {
                            buttonAction = new ButtonAction { ID = Convert.ToInt32(item.ID), Title = item.Title, Value = item.Value, Notes = item.Notes };
                            break;
                        }
                        else if (IsNextAction == false && item.Value.ToLowerInvariant().Equals("return")) // kiểm tra xem có action Return không
                        {
                            buttonAction = new ButtonAction { ID = Convert.ToInt32(item.ID), Title = item.Title, Value = item.Value, Notes = item.Notes };
                            break;
                        }
                    }
                    #endregion

                    #region Handle Button Action
                    if (buttonAction == null)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            //RollbackData_ViewBoard();
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_BOARD_NOACTION", "Phiếu không có hành động tương ứng!"),
                                                                       CmmFunction.GetTitle("MESS_BOARD_NOACTION", "This workflow item not have corresponding action!"));
                        });

                        return;
                    }

                    if (IsNextAction == true) // Next thì ko cần ý kiến -> gọi API luôn
                    {
                        Action_SendAPI(lstWorkflowItem[0], buttonAction, _formDefineInfo, "");
                    }
                    else // Back thì cần ý kiến
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            //Update View trước - Lát rollback sau
                            //_boardView.NotifyDataSetChanged(); // Notify lại boardView nếu trường hợp listChild.count = 0
                            //bool _flagRollback = true;  // nếu tắt dialog - hủy thì rollback, send action thì ko cần
                            //RollbackData_ViewBoard(); // Rollback trước, nếu Hủy thì không bị lag, nếu send action thì có refresh
                            CmmDroidFunction.HideProcessingDialog();

                            #region Get View - Init Data
                            View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_Accept, null);
                            TextView _tvTitle = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Title);
                            ImageView _imgAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_Accept);
                            EditText _edtComment = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_Accept_YKien);
                            TextView _tvCancel = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Huy);
                            TextView _tvAccept = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_HoanTat);

                            _tvTitle.Text = buttonAction.Title;

                            _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                            _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                            _tvAccept.Text = buttonAction.Title;

                            string _imageName = "icon_bpm_Btn_action_" + buttonAction.ID.ToString();
                            int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                            _imgAction.SetImageResource(resId);

                            #endregion

                            #region Show View
                            _dialogAction = new Dialog(_rootView.Context);
                            Window window = _dialogAction.Window;
                            _dialogAction.RequestWindowFeature(1);
                            _dialogAction.SetCanceledOnTouchOutside(false);
                            _dialogAction.SetCancelable(true);
                            window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                            window.SetGravity(GravityFlags.Center);
                            var dm = Resources.DisplayMetrics;

                            _dialogAction.SetContentView(_viewPopupAction);
                            _dialogAction.Show();
                            WindowManagerLayoutParams s = window.Attributes;
                            s.Width = dm.WidthPixels;
                            window.Attributes = s;
                            window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                            #endregion

                            #region Event
                            _tvAccept.Click += (sender, e) =>
                            {
                                if (new ControllerDetailWorkflow().CheckActionHasComment(_mainAct, _edtComment) == true)
                                {
                                    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                                    _dialogAction.Dismiss();
                                    Action_SendAPI(lstWorkflowItem[0], buttonAction, _formDefineInfo, _edtComment.Text);
                                }
                            };
                            _tvCancel.Click += (sender, e) =>
                            {
                                CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                                _dialogAction.Dismiss();
                            };
                            _edtComment.TextChanged += (sender, e) =>
                            {
                                if (string.IsNullOrEmpty(_edtComment.Text))
                                    _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                                else
                                    _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                            };

                            #endregion

                            _edtComment.Text = ""; // để trigger lại Function TextChanged
                        });
                    }

                    #endregion
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    //RollbackData_ViewBoard();
                    CmmDroidFunction.HideProcessingDialog();
                });

#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetDragAction_NextPrevious", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Hàm send API lên để xử lý trên Server
        /// </summary>
        /// <param name="buttonAction">ButtonAction tương ứng</param>
        /// <param name="comment">ý kiến của Action nếu có</param>
        /// <param name="_lstExtent">List các column thêm nếu cần như: uservalues, ...</param>
        /// <param name="IsFragmentDetailWorkflow">check xem trang thực hiện API là trang nào</param>
        private async void Action_SendAPI(BeanWorkflowItem _itemAction, ButtonAction buttonAction, string _formDefineInfo, string comment)
        {
            bool _result = false;
            try
            {
                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                    #region Comment
                    // Nếu Action Có comment -> Add thêm cột idea
                    List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
                    if (!string.IsNullOrEmpty(comment))
                    {
                        KeyValuePair<string, string> _KeyValueComment = new KeyValuePair<string, string>("idea", comment);
                        if (_lstExtent == null) _lstExtent = new List<KeyValuePair<string, string>>();
                        _lstExtent.Add(_KeyValueComment);
                    }
                    #endregion

                    string _messageAPI = "";
                    _result = _pControlDynamic.SendControlDynamicAction(buttonAction.Value, _itemAction.ID, _formDefineInfo, JsonConvert.SerializeObject(new List<ObjectSubmitAction>()), ref _messageAPI, new List<KeyValuePair<string, string>>(), _lstExtent);

                    if (_result)
                    {
                        _pControlDynamic.UpdateAllMasterData(true);
                        _pControlDynamic.UpdateAllDynamicData(true);

                        _mainAct.RunOnUiThread(() =>
                        {
                            SetData(_updateMasterData: false);
                            CmmDroidFunction.HideProcessingDialog();
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            ////SetData_Board(_lstBoardKanBan_Filter);
                            CmmDroidFunction.HideProcessingDialog();
                            if (!String.IsNullOrEmpty(_messageAPI))
                                CmmDroidFunction.ShowAlertDialog(_mainAct, _messageAPI, _messageAPI);
                            else
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                   CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                CmmDroidFunction.HideProcessingDialog();
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "Action_SendAPI", ex);
#endif
            }
        }

        #endregion

        #endregion

    }
}