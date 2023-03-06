using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allyants.BoardViewLib;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Views;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppKanban : CustomBaseFragment, BoardView.IDragItemStartCallback, BoardView.IItemClickListener
    {
        public LayoutInflater _inflater;
        public Dialog _dialogAction;
        public MainActivity _mainAct;
        public View _rootView;
        public EditText _edtSearch;
        public TextView _tvTitle, _tvSubtitle, _tvNoData;
        public ImageView _imgBack, _imgFilter, _imgShowSearch, _imgSubtitlePrevious, _imgSubtitleNext, _imgDelete;
        public RelativeLayout _relaToolBar;
        public LinearLayout _lnSubtitle, _lnSearch, _lnBottomNavigation;
        public SwipeRefreshLayout _swipe;
        public MyCustomBoardView _boardView;

        public AdapterBoardKanbanView _adapterBoardKanBan;
        public ControllerBoard CTRLBoard = new ControllerBoard();

        public List<BeanWorkflowStepDefine> _lstStepDefine = new List<BeanWorkflowStepDefine>(); // Chỉ bao gồm bước của Quy Trình
        public List<BeanBoardKanBan> _lstBoardKanBan_Full = new List<BeanBoardKanBan>();

        public BeanWorkflow _currentWorkflow = new BeanWorkflow();
        public int _flagCurrentPage = 1; // 1 = Board, 2 = List, 3 = Report

        public const int _ApprovedStepID = -1;
        public const int _RejectedStepID = -2;

        public bool _flagIsFiltering = false;
        public ControllerBase c_base = new ControllerBase();

        #region Life Cycle
        public FragmentChildAppKanban() { }

        public FragmentChildAppKanban(BeanWorkflow _currentWorkflow, int _flagCurrentPage)
        {
            this._currentWorkflow = _currentWorkflow;
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
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = _inflater.Inflate(Resource.Layout.ViewChildAppKanBan, null);
                //Toolbar
                _relaToolBar = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewChildAppKanBan_Toolbar);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppKanBan_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_Back);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_Filter);
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_ShowSearch);
                // Subtitle
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewChildAppKanBan);
                _lnSubtitle = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppKanBan_SubTitle);
                _tvSubtitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppKanBan_SubTitle);
                _imgSubtitlePrevious = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_SubTitle_Previous);
                _imgSubtitleNext = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_SubTitle_Next);
                // Search
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppKanBan_Search);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewChildAppKanBan_Search);
                _imgDelete = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewChildAppKanBan_Search_Delete);
                // Data
                _boardView = _rootView.FindViewById<MyCustomBoardView>(Resource.Id.boardView_ViewChildAppKanBan);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewChildAppKanBan_NoData);
                // Bottom Navigation
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewChildAppKanBan_BottomNavigation);
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                _swipe.SetDistanceToTriggerSync(10); // in dips   

                _swipe.Refresh += Swipe_RefreshData;
                _swipe.Enabled = false;
                _imgBack.Click += Click_imgBack;
                _imgDelete.Click += Click_imgDelete;
                _imgShowSearch.Click += Click_imgShowSearch;
                _imgFilter.Click += Click_imgFilter;

                _imgSubtitlePrevious.Click += Click_imgSubtitlePrevious;
                _imgSubtitleNext.Click += Click_imgSubtitleNext;

                _edtSearch.TextChanged += TextChanged_EdtSearch;
                _edtSearch.EditorAction += EditorAction_EdtSearch;

                _boardView.SetOnItemClickListener(this);
                _boardView.SetOnDragItemListener(this);

                _imgDelete.Visibility = ViewStates.Gone;

                SetViewByLanguage();
                SetData_ListStepDefine();
                new Handler().PostDelayed(() =>
                {
                    SetData();
                }, CmmDroidVariable.M_ActionDelayTime + 500);

                _imgSubtitlePrevious.Visibility = ViewStates.Gone;
                _imgSubtitleNext.Visibility = ViewStates.Gone;
                _lnSubtitle.Visibility = ViewStates.Gone;
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    //SetViewByLanguage();
                    //SetData_ListStepDefine();

                    //_boardView.Visibility = ViewStates.Gone;
                    //_tvNoData.Visibility = ViewStates.Visible;

                    //new Handler().PostDelayed(() =>
                    //{
                    //    SetData();
                    //}, CmmDroidVariable.M_ActionDelayTime + 500);
                }
            }

            /*// Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();*/

            SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, "FragmentBoard", _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppKanban;
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
                if (_currentWorkflow != null)
                {
                    _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");

                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.Title : "";
                        _tvSubtitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.Title : "";
                    }
                    else
                    {
                        _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.TitleEN : "";
                        _tvSubtitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.TitleEN : "";
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

        public void SetColor_ImgFilter_ByFlag(bool flag)
        {
            _flagIsFiltering = flag;
            if (flag == true)
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGreenDueDate)));
            else
                _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
        }

        public void SetColor_ImageShowSearch_ByFlag(bool flag)
        {
            if (flag == true)
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
            else
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Back", ex);
#endif
            }
        }

        private void Click_imgDelete(object sender, EventArgs e)
        {
            try
            {
                _imgDelete.Visibility = ViewStates.Gone; // ẩn trước để bỏ event ra
                _edtSearch.Text = ""; // trigger lại
                _edtSearch.SetSelection(_edtSearch.Text.Length);
                FilterAndSearchData();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Back", ex);
#endif
            }
        }

        private void Click_imgShowSearch(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                _imgShowSearch.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (_lnSearch.Visibility == ViewStates.Gone)
                {
                    SetColor_ImageShowSearch_ByFlag(true);
                    _lnSearch.Visibility = ViewStates.Visible;
                    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _edtSearch.RequestFocus();
                        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                        //_lnSubtitle.Visibility = ViewStates.Gone;
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    SetColor_ImageShowSearch_ByFlag(false);
                    //_lnSubtitle.Visibility = ViewStates.Visible;
                    _lnSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_lnSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);

                        _lnSearch.Visibility = ViewStates.Gone;
                        if (!String.IsNullOrEmpty(_edtSearch.Text))
                            _edtSearch.Text = "";
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

        private void Click_imgFilter(object sender, EventArgs e)
        {
            try
            {
                _imgFilter.Enabled = false;
                if (CmmDroidFunction.PreventMultipleClick())
                {
                    // set màu xanh lá trong Sharedview
                    //SetColor_ImgFilter_ByFlag(true);
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    Action action = new Action(() =>
                    {
                        LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);
                        SharedView_PopupFilterBoard sharedView_PopupFilterBoard = new SharedView_PopupFilterBoard(_layoutInflater, _mainAct, this, this.GetType().Name, _rootView);
                        sharedView_PopupFilterBoard.InitializeValue(CTRLBoard, (int)SharedView_PopupFilterBoard.FlagViewFilterBoard.ChildAppKanBan);
                        sharedView_PopupFilterBoard.InitializeView();
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                _imgFilter.Enabled = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_lnFilter", ex);
#endif
            }
        }

        private void Click_imgSubtitlePrevious(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                string _queryPrevious = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID < {0} AND StatusName = 'Active' ORDER BY WorkflowID DESC LIMIT 1 OFFSET 0", _currentWorkflow.WorkflowID);

                List<BeanWorkflow> _lstPrevious = conn.Query<BeanWorkflow>(_queryPrevious);

                _imgSubtitlePrevious.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                Action action = new Action(() =>
                {
                    if (_lstPrevious != null && _lstPrevious.Count > 0)
                    {
                        _currentWorkflow = _lstPrevious[0];
                        CTRLBoard = new ControllerBoard();
                        if (!String.IsNullOrEmpty(_edtSearch.Text)) // Xóa trạng thái Search
                        {
                            _imgDelete.Visibility = ViewStates.Gone;
                            _edtSearch.Text = "";
                        }
                        SetViewByLanguage();
                        SetColor_ImgFilter_ByFlag(false);
                        SetData_ListStepDefine();
                        SetData();
                    }
                    else
                    {

                    }
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgSubtitlePrevious", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void Click_imgSubtitleNext(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                string _queryNext = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID > {0} AND StatusName = 'Active' ORDER BY WorkflowID ASC LIMIT 1 OFFSET 0", _currentWorkflow.WorkflowID);

                List<BeanWorkflow> _lstNext = conn.Query<BeanWorkflow>(_queryNext);

                _imgSubtitleNext.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                Action action = new Action(() =>
                {
                    if (_lstNext != null && _lstNext.Count > 0)
                    {
                        _currentWorkflow = _lstNext[0];
                        CTRLBoard = new ControllerBoard(); // Init lại giá trị fitler
                        if (!String.IsNullOrEmpty(_edtSearch.Text)) // Xóa trạng thái Search
                        {
                            _imgDelete.Visibility = ViewStates.Gone;
                            _edtSearch.Text = "";
                        }
                        SetViewByLanguage();
                        SetColor_ImgFilter_ByFlag(false);
                        SetData_ListStepDefine();
                        SetData();
                    }
                    else
                    {

                    }
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgSubtitleNext", ex);
#endif
            }
        }

        private void TextChanged_EdtSearch(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(_edtSearch.Text.ToString())) // Không Search
            {
                _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                _imgDelete.Visibility = ViewStates.Gone;
            }
            else
            {
                _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
                _imgDelete.Visibility = ViewStates.Visible;
            }
            // Handle Search bên EditorAction_EdtSearch;
        }

        public void EditorAction_EdtSearch(object sender, TextView.EditorActionEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next) // Bấm nút Done trên bàn phím
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    _edtSearch.Text = _edtSearch.Text; // trigger lại view
                    _edtSearch.SetSelection(_edtSearch.Text.Length);
                    FilterAndSearchData();
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
                    ProviderUser pUser = new ProviderUser();
                    pUser.UpdateAllMasterData(true);
                    pUser.UpdateAllDynamicData(true);

                    string _preValueLang = CmmVariable.SysConfig.LangCode;
                    pUser.UpdateCurrentUserInfo(CmmVariable.M_Avatar);

                    // Check xem có bị thay đổi giá trị LangCode không
                    if (!_preValueLang.Equals(CmmVariable.SysConfig.LangCode))
                    {
                        pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    _mainAct.RunOnUiThread(() =>
                    {
                        CmmDroidFunction.ShowVibrateEvent(0.2);
                        SetViewByLanguage();
                        SetData_ListStepDefine();
                        SetData_ListBoardKanBan();

                        // đã có adapter từ trc -> trigger lại là dc
                        if (!string.IsNullOrEmpty(_edtSearch.Text))
                        {
                            _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                            _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng
                        }
                        else
                            _edtSearch.Text = "";
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
        private async void SetData(bool _updateMasterData = true, bool _runUIThread = true)
        {
            ProviderBase pBase = new ProviderBase();
            try
            {
                if (_updateMasterData)
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    if (_updateMasterData)
                    {
                        pBase.UpdateMasterData<BeanWorkflowFollow>();
                        pBase.UpdateMasterData<BeanAppBase>();
                    }

                    SetData_ListBoardKanBan();

                    _mainAct.RunOnUiThread(() =>
                    {
                        if (_runUIThread)
                            SetData_KanbanView(_lstBoardKanBan_Full);
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
        }

        private void SetData_KanbanView(List<BeanBoardKanBan> _lstBoard)
        {
            try
            {
                if (_lstBoard != null && _lstBoard.Count > 0)
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        _adapterBoardKanBan = new AdapterBoardKanbanView(_mainAct, _rootView.Context, _lstBoard);
                        _boardView.SetAdapter(_adapterBoardKanBan);
                    });
                }
                else
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        _boardView.Visibility = ViewStates.Gone;
                        _tvNoData.Visibility = ViewStates.Visible;
                    });

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void SetData_ListStepDefine()
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                string _queryStepDefine = String.Format(CTRLBoard._QueryStepDefine_Distinct, _currentWorkflow.WorkflowID);
                _lstStepDefine = new List<BeanWorkflowStepDefine>();
                _lstStepDefine = conn.Query<BeanWorkflowStepDefine>(_queryStepDefine);
                _lstStepDefine.Add(new BeanWorkflowStepDefine()
                {
                    WorkflowStepDefineID = _ApprovedStepID,
                    Title = CmmFunction.GetTitle("TEXT_APPROVED", "Đã phê duyệt")
                });
                _lstStepDefine.Add(new BeanWorkflowStepDefine()
                {
                    WorkflowStepDefineID = _RejectedStepID,
                    Title = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối")
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_ListStepDefine", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void SetData_ListBoardKanBan()
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (_lstStepDefine == null || _lstStepDefine.Count == 0)
                    return;

                _lstBoardKanBan_Full = new List<BeanBoardKanBan>();
                string _queryAppBase = "";
                List<BeanAppBaseExt> _lstAppBaseByStep = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> _lstAppBaseByStepAPI = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> _lstAppBaseByStepTemp = new List<BeanAppBaseExt>();

                if (c_base.CheckAppHasConnection())
                {
                    ProviderBase p_base = new ProviderBase();
                    _lstAppBaseByStepAPI = p_base.getAllItemKanBan(_currentWorkflow.WorkflowID.ToString());
                }

                foreach (BeanWorkflowStepDefine item in _lstStepDefine)
                {
                    BeanBoardKanBan _tempBeanBoardKanBan = new BeanBoardKanBan { itemStepDefine = item, lstAppBase = new List<BeanAppBaseExt>() };

                    if (c_base.CheckAppHasConnection())
                    {
                        switch (item.WorkflowStepDefineID)
                        {
                            case _ApprovedStepID: // Phê duyệt
                                {
                                    /*_queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                      WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                      ORDER BY Created DESC",
                                                                     _currentWorkflow.WorkflowID, String.Join(",", CTRLBoard.ApprovedListID));*/
                                    _lstAppBaseByStepTemp = _lstAppBaseByStepAPI.Where(x =>x.Step == _ApprovedStepID).ToList();
                                    break;
                                }
                            case _RejectedStepID: // Từ chối
                                {
                                    /* _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                     WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                     ORDER BY Created DESC",
                                                                     _currentWorkflow.WorkflowID, String.Join(",", CTRLBoard.RejectedListID));*/
                                    _lstAppBaseByStepTemp = _lstAppBaseByStepAPI.Where(x =>
                                  x.Step == _RejectedStepID
                                  ).ToList();
                                    break;
                                }
                            default:
                                {

                                    /* _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                     WHERE WorkflowID = {0} AND Step = {1} AND ResourceCategoryId <> 16 AND StatusGroup NOT IN ({2})
                                                                     ORDER BY Created DESC",
                                                                     _currentWorkflow.WorkflowID, item.Step, String.Join(",", CTRLBoard.ApprovedListID.Concat(CTRLBoard.RejectedListID).ToArray()));*/
                                    _lstAppBaseByStepTemp = _lstAppBaseByStepAPI.Where(x =>
                                   x.Step == item.Step
                                   ).ToList();
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (item.WorkflowStepDefineID)
                        {
                            case _ApprovedStepID: // Phê duyệt
                                {
                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                     WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                     ORDER BY Created DESC",
                                                                     _currentWorkflow.WorkflowID, String.Join(",", CTRLBoard.ApprovedListID));
                                    break;
                                }
                            case _RejectedStepID: // Từ chối
                                {
                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                    WHERE WorkflowID = {0} AND ResourceCategoryId <> 16 AND StatusGroup IN ({1})
                                                                    ORDER BY Created DESC",
                                                                    _currentWorkflow.WorkflowID, String.Join(",", CTRLBoard.RejectedListID));
                                    break;
                                }
                            default:
                                {

                                    _queryAppBase = String.Format(@"SELECT * FROM BeanAppBase 
                                                                    WHERE WorkflowID = {0} AND Step = {1} AND ResourceCategoryId <> 16 AND StatusGroup NOT IN ({2})
                                                                    ORDER BY Created DESC",
                                                                    _currentWorkflow.WorkflowID, item.Step, String.Join(",", CTRLBoard.ApprovedListID.Concat(CTRLBoard.RejectedListID).ToArray()));
                                    break;
                                }
                        }
                        _lstAppBaseByStep = conn.Query<BeanAppBaseExt>(_queryAppBase);

                    }


                    /*var _lstAppBaseByStepTem = _lstAppBaseByStep.Where(x => x.ApprovalStatus != 0).ToList();
                    _lstAppBaseByStep = _lstAppBaseByStepTem;*/
                    _lstAppBaseByStep = _lstAppBaseByStepTemp;
                    _lstAppBaseByStep = CTRLBoard.FilterListAppbaseByCondition(_lstAppBaseByStep, CTRLBoard.GetCurrentValue_Filter());
                    _lstBoardKanBan_Full.Add(new BeanBoardKanBan() { itemStepDefine = item, lstAppBase = _lstAppBaseByStep });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_ListBoardKanBan", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        public async void FilterAndSearchData()
        {
            Task.Run(() =>
            {
                try
                {
                    List<BeanBoardKanBan> _lstFilter_Kanban = new List<BeanBoardKanBan>();

                    foreach (BeanBoardKanBan item in _lstBoardKanBan_Full)
                    {
                        BeanBoardKanBan _tempItemList = new BeanBoardKanBan
                        {
                            itemStepDefine = item.itemStepDefine,
                            lstAppBase = CTRLBoard.FilterListAppbaseByCondition(item.lstAppBase.ToList(), CTRLBoard.GetCurrentValue_Filter(), _edtSearch.Text)
                        };

                        _lstFilter_Kanban.Add(_tempItemList);
                    }
                    SetData_KanbanView(_lstFilter_Kanban);
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "FilterData", ex);
#endif
                }
            });
        }

        private void Handle_ClickItemAppBase(BeanAppBaseExt e)
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
                if (e.ResourceCategoryId.Value != 16) // Task không cho click
                {
                    FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow((lstWorkflowItem != null & lstWorkflowItem.Count > 0) ? lstWorkflowItem[0] : null, null, this.GetType().Name);
                    _mainAct.AddFragment(FragmentManager, detailWorkFlow, typeof(FragmentDetailWorkflow).Name, 0);
                }
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
                string _workflowItemID = e._workflowItemID;
                for (int i = 0; i < _lstBoardKanBan_Full.Count; i++)
                {
                    for (int j = 0; j < _lstBoardKanBan_Full[i].lstAppBase.Count; j++)
                    {
                        if (_lstBoardKanBan_Full[i].lstAppBase[j].ResourceCategoryId != 16 &&
                            _workflowItemID == CmmFunction.GetWorkflowItemIDByUrl(_lstBoardKanBan_Full[i].lstAppBase[j].ItemUrl))
                        {
                            _lstBoardKanBan_Full[i].lstAppBase[j].IsFollow = e._IsFollow;
                            SetData_KanbanView(_lstBoardKanBan_Full);
                        }
                    }
                }
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
                SetData_KanbanView(_adapterBoardKanBan.GetListData().ToList());
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

        public async void EndDrag(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn)
        {
            try
            {
                await Task.Run(() =>
                {
                    //_adapterBoardKanBan
                    if (CTRLBoard.CheckAppHasConnection() == false)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_OFFLINE", "Bạn đang ở chế độ offline"),
                                                    CmmFunction.GetTitle("TEXT_OFFLINE", "You are in offline mode"));
                        });
                        RollbackData_ViewBoard();
                        return;
                    }
                    if (originalColumn >= _adapterBoardKanBan.GetListData().Count - 2) // Không được thao tác 2 cột cuối: Từ chối - Phê duyệt
                    {
                        RollbackData_ViewBoard();
                        _mainAct.RunOnUiThread(() =>
                        {

                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_BOARD_APPROVE_REJECT", "Không được thao tác trên cột Hoàn tất và Từ chối"),
                                                                       CmmFunction.GetTitle("MESS_BOARD_APPROVE_REJECT", "Can't perform this action on rejected and approved column"));
                        });
                    }
                    else
                    {
                        if (System.Math.Abs(newColumn - originalColumn) >= 2) // không cho skip - back quá 1 bước
                        {
                            RollbackData_ViewBoard();
                            _mainAct.RunOnUiThread(() =>
                            {

                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("MESS_BOARD_AJACENTSTEP", "Chỉ được thao tác trên hai bước liền kề"),
                                CmmFunction.GetTitle("MESS_BOARD_AJACENTSTEP", "Can only perform action on two adjacent steps"));
                            });
                            return;
                        }

                        if (newColumn > originalColumn) // Drag qua phải
                        {
                            _mainAct.RunOnUiThread(() =>
                            {

                                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                            });
                            BeanAppBaseExt _itemAction = _adapterBoardKanBan.GetListData()[originalColumn].lstAppBase[originalPosition];
                            //RollbackData_ViewBoard();
                            HandleDragAction(_itemAction, true);

                        }
                        else if (newColumn < originalColumn) // Drag qua trái
                        {
                            _mainAct.RunOnUiThread(() =>
                            {

                                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                            });
                            BeanAppBaseExt _itemAction = _adapterBoardKanBan.GetListData()[originalColumn].lstAppBase[originalPosition];
                            //RollbackData_ViewBoard();
                            HandleDragAction(_itemAction, false);
                        }
                        else if (newColumn == originalColumn) // Drag tại chỗ -> rollback
                            RollbackData_ViewBoard();
                    }
                });
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

                if (lstWorkflowItem == null || lstWorkflowItem.Count == 0) // Không tìm thấy WorkflowItem
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
                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ChildAppKanban getWorkFlowItemByRID", ex);
#endif
                    }
                    if (lstWorkflowItem == null || lstWorkflowItem.Count == 0)
                        return;
                }

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
                        if (IsNextAction == true && (
                            item.ID == ((int)WorkflowAction.Action.Next).ToString() ||
                            item.ID == ((int)WorkflowAction.Action.Approve).ToString() ||
                            item.ID == ((int)WorkflowAction.Action.Submit).ToString())) // kiểm tra xem có action Next không
                        {
                            buttonAction = new ButtonAction { ID = Convert.ToInt32(item.ID), Title = item.Title, Value = item.Value, Notes = item.Notes };
                            break;
                        }
                        else if (IsNextAction == false && item.ID == ((int)WorkflowAction.Action.Return).ToString()) // kiểm tra xem có action Return không
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
                //CmmDroidFunction.HideProcessingDialog();
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
                            ////SetData(_lstBoardKanBan_Filter);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Action_SendAPI", ex);
#endif
            }
        }

        #endregion

        #endregion
    }
}