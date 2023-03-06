using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
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
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentBoard : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private SwipeRefreshLayout _swipe;
        private View _rootView;
        private Dialog _dialogPopupControl;
        private TextView _tvtitle, _tvCurrentGroup, _tvNoDataExpand, _tvNoDataFavorite;
        private ImageView _imgFavorite, _imgShowSearch, _imgDelete;
        private LinearLayout _lnCurrentGroup, _lnSearch, _lnNoDataExpand, _lnNoDataFavorite, _lnBottomNavigation;
        private CardView _cardGroupWorkflow, _cardSearch;
        private RelativeLayout _relaDataExpand, _relaDataFavorite;
        private EditText _edtSearch;
        private CircleImageView _imgAvatar;
        private ExpandableListView _expandData;
        private RecyclerView _recyData;

        private ControllerBoard CTRLBoard = new ControllerBoard();
        private List<BeanWorkflowCategory> _lstWFCategory = new List<BeanWorkflowCategory>(); // List nhóm quy trình
        private List<BeanBoardWorkflow> _lstExpand = new List<BeanBoardWorkflow>(); // List của Expandable ListView
        private List<BeanWorkflow> _lstFavorite = new List<BeanWorkflow>(); // List của Recycler View
        private AdapterExpandBoard_MainGroup _adapterExpandMainGroup;
        private AdapterRecyBoard_MainGroup _adapterRecyFavorite;

        private int _idCategoryAll = -99; // ID của tất cả
        private BeanWorkflowCategory _currentExpandWFCategory = null;
        private string _queryExpand = "";
        private string _queryFavorite = "";
        private bool _allowLoadMore_Expand = false;
        private bool _allowLoadMore_Favorite = false;
        private bool _flagShowFavorite = false; // bằng true là chỉ hiện sao vàng
        private LinearLayout _lnBoard;
        private static FragmentBoard fragment;
        public FragmentBoard() { }
        public static CustomBaseFragment newInstance()
        {
            fragment = new FragmentBoard();
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            //_mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            CmmEvent.UpdateLangComplete -= EventHandler_UpdateLanguage;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewBoard, null);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewBoard);
                _tvtitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoard_Name);
                _imgAvatar = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewBoard_Avatar);
                _imgFavorite = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoard_Subcribe);
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoard_ShowSearch);
                _cardSearch = _rootView.FindViewById<CardView>(Resource.Id.card_ViewBoard_Search);
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoard_Search);
                _expandData = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewBoard_Data);
                _recyData = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewBoard_Data);
                _relaDataExpand = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoard_Data);
                _relaDataFavorite = _rootView.FindViewById<RelativeLayout>(Resource.Id.rela_ViewBoard_Data_Subcribe);
                _lnNoDataExpand = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoard_NoData);
                _lnNoDataFavorite = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoard_NoData_Subcribe);
                _tvNoDataExpand = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoard_NoData);
                _tvNoDataFavorite = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoard_NoData_Subcribe);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewBoard_BottomNavigation);
                _cardGroupWorkflow = _rootView.FindViewById<CardView>(Resource.Id.card_ViewBoard_GroupWorkflow);
                _tvCurrentGroup = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewBoard_CurrentGroupWorkflow);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewBoard_Search);
                _imgDelete = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewBoard_Search_Delete);
                _lnBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.lnBoard);
                _lnBoard.Touch += _lnBoard_Touch;
                _swipe.Refresh += Swipe_RefreshData;
                _imgAvatar.Click += Click_imgAvatar;
                _tvtitle.Click += Click_tvTitle;
                _imgShowSearch.Click += Click_imgShowSearch;
                _imgFavorite.Click += Click_imgFavorite;
                _imgDelete.Click += Click_imgDelete;
                _edtSearch.TextChanged += TextChanged_EdtSearch;
                _recyData.ScrollChange += ScrollChange_RecyFavorite;
                _cardGroupWorkflow.Click += Click_lnCurrentGroupWorkflow;
                //_expandData.ScrollChange += ScrollChange_ExpandData;
                _expandData.SetGroupIndicator(null);
                _expandData.SetChildIndicator(null);
                _expandData.DividerHeight = 0;
                _edtSearch.Click += _edtSearch_Click;
                _edtSearch.PerformClick();
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                CTRLBoard.SetAvataForImageView(_mainAct, _imgAvatar, 80);
                SetViewByLanguage();
                SetData_ListExpand();
                SetData_RecyFavorite();
                _relaDataExpand.StartAnimation(ControllerAnimation.GetAnimationFallDown(_rootView.Context, 100f));

                SharedView_BottomNavigation bottomNavigation = new SharedView_BottomNavigation(inflater, _mainAct, this, this.GetType().Name, _rootView);
                bottomNavigation.InitializeValue(_lnBottomNavigation);
                bottomNavigation.InitializeView();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    CTRLBoard.SetAvataForImageView(_mainAct, _imgAvatar, 80);
                    SetViewByLanguage();
                    SetData_ListExpand();
                    SetData_RecyFavorite();
                    _relaDataExpand.StartAnimation(ControllerAnimation.GetAnimationFallDown(_rootView.Context, 100f));

                    if (_cardSearch.Visibility == ViewStates.Visible) // ẩn view search như default
                    {
                        _cardSearch.Visibility = ViewStates.Gone;
                        _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                    }

                    if (!String.IsNullOrEmpty(_edtSearch.Text)) // Bỏ trạng thái Search
                        _edtSearch.Text = "";
                }
            }
            MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board; // Board
            CmmEvent.UpdateLangComplete += EventHandler_UpdateLanguage;
            return _rootView;
        }



        #region Event
        private void _edtSearch_Click(object sender, EventArgs e)
        {
            _expandData.Touch += _expandData_Touch;
        }

        private void _expandData_Touch(object sender, View.TouchEventArgs e)
        {
            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
            _expandData.Touch -= _expandData_Touch;
        }

        private void _lnBoard_Touch(object sender, View.TouchEventArgs e)
        {
            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
        }

        private void Click_imgShowSearch(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;

                _imgShowSearch.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                if (_cardSearch.Visibility == ViewStates.Gone)
                {
                    _cardSearch.Visibility = ViewStates.Visible;

                    _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
                    _cardSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_TopToBot(_cardSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        _edtSearch.RequestFocus();
                        CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
                }
                else
                {
                    _edtSearch.PerformClick();
                    _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
                    _cardSearch.StartAnimation(ControllerAnimation.GetAnimationSwipe_BotToTop(_cardSearch, duration: CmmDroidVariable.M_ActionDelayTime));
                    Action action = new Action(() =>
                    {
                        CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                        _cardSearch.Visibility = ViewStates.Gone;

                        if (_flagShowFavorite == true)
                            _recyData.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        else
                            _expandData.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                        if (!String.IsNullOrEmpty(_edtSearch.Text)) // Xóa trạng thái Search nếu có
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

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            if (!String.IsNullOrEmpty(_edtSearch.Text))
                _edtSearch.Text = "";
        }
        private void SetViewByLanguage()
        {
            try
            {
                _tvtitle.Text = CmmFunction.GetTitle("TEXT_BOARD", "Ứng dụng");
                _tvNoDataExpand.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                _tvNoDataFavorite.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                _edtSearch.Hint = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void EventHandler_UpdateLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetViewByLanguage();

                    if (_currentExpandWFCategory.ID == _idCategoryAll)
                    {
                        _lstWFCategory[0].Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                        _tvCurrentGroup.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                    }

                    if (_adapterExpandMainGroup != null)
                        _adapterExpandMainGroup.NotifyDataSetChanged();
                    if (_adapterRecyFavorite != null)
                        _adapterRecyFavorite.NotifyDataSetChanged();
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ChangeLanguage", ex);
#endif
            }
        }

        private async void Swipe_RefreshData(object sender, EventArgs e)
        {
            try
            {
                _swipe.Refreshing = true;
                await Task.Run(async() =>
                {
                    ProviderBase pBase = new ProviderBase();
                    await pBase.UpdateAllDynamicDataAndroid(true);

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
                        Action action = new Action(() =>
                        {
                            CmmDroidFunction.ShowVibrateEvent(0.2);
                            //_imgDelete.Visibility = ViewStates.Gone;
                            //_edtSearch.TextChanged -= TextChanged_EdtSearch;
                            //_edtSearch.Text = "";
                            //_edtSearch.TextChanged += TextChanged_EdtSearch;
                            //SetData_ListExpand();
                            //SetData_RecyFavorite();

                            SetData_ListExpand();
                            SetData_RecyFavorite();
                            _edtSearch.Text = _edtSearch.Text; //Lưu lại trạng thái search hiện tại
                            _edtSearch.SetSelection(_edtSearch.Text.Length); // focus vào character cuối cùng

                            CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
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
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                });
            }
        }

        private void Click_imgAvatar(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board; // Board
                MinionAction.OnRenewDataAndShowFragmentLeftMenu(null, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentHomePage - Click_BottomHome - Error: " + ex.Message);
#endif
            }
        }

        public virtual void Click_tvTitle(object sender, EventArgs e)
        {
            try
            {
                _tvtitle.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (_flagShowFavorite == true)
                {
                    if (_lstFavorite != null && _lstFavorite.Count > 0)
                        _recyData.SmoothScrollToPosition(0);
                }
                else
                {
                    if (_lstExpand != null && _lstExpand.Count > 0)
                        _expandData.SmoothScrollToPosition(0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvTitle", ex);
#endif
            }
        }

        private void Click_imgFavorite(object sender, EventArgs e)
        {
            try
            {
                ProviderBase p_base = new ProviderBase();
                p_base.UpdateMasterData<BeanWorkflowCategory>(null, true);
                p_base.UpdateMasterData<BeanWorkflow>(null, true);
                SetData_ListExpand();
                SetData_RecyFavorite();
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    _imgFavorite.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    _flagShowFavorite = !_flagShowFavorite;
                    if (_flagShowFavorite == true)
                    {
                        _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_check);
                        _imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));

                        _cardGroupWorkflow.Visibility = ViewStates.Gone; // Favorite ko hiện loại
                        _relaDataExpand.Visibility = ViewStates.Gone;
                        _relaDataFavorite.Visibility = ViewStates.Visible;

                        _relaDataFavorite.StartAnimation(ControllerAnimation.GetAnimationFallDown(_rootView.Context, 100f));

                        Click_imgDelete(null, null); // xóa trạng thái Search hiện tại

                    }
                    else
                    {
                        _imgFavorite.SetImageResource(Resource.Drawable.icon_ver2_favorite_uncheck);
                        _imgFavorite.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));

                        _cardGroupWorkflow.Visibility = ViewStates.Visible; // Favorite ko hiện loại
                        _relaDataExpand.Visibility = ViewStates.Visible;
                        _relaDataFavorite.Visibility = ViewStates.Gone;

                        _relaDataExpand.StartAnimation(ControllerAnimation.GetAnimationFallDown(_rootView.Context, 100f));

                        foreach (var item in _lstWFCategory) // Set lại current group thành all
                        {
                            if (item.ID == _idCategoryAll)
                            {
                                _currentExpandWFCategory = item;
                                _tvCurrentGroup.Text = item.Title;
                            }
                        }
                        Click_imgDelete(null, null); // xóa trạng thái Search hiện tại
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFavorite", ex);
#endif
            }
        }

        private void TextChanged_EdtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                _edtSearch.PerformClick();
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

                if (_flagShowFavorite)
                {
                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text.ToString()).ToLowerInvariant();

                    List<BeanWorkflow> _lstSearch = (from item in _lstFavorite
                                                     where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(_content))
                                                         || (!string.IsNullOrEmpty(item.TitleEN) && CmmFunction.removeSignVietnamese(item.TitleEN.ToLowerInvariant()).Contains(_content)))
                                                     select item).ToList();

                    InitRecyclerViewFavorite(_lstSearch);
                    ////SetData_RecyFavorite(); // đã bao gồm search trong câu query
                }
                else
                {
                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text.ToString()).ToLowerInvariant();

                    List<BeanBoardWorkflow> _lstSearch = new List<BeanBoardWorkflow>();
                    if (_currentExpandWFCategory.ID == _idCategoryAll) // Tất cả
                    {
                        foreach (BeanBoardWorkflow item in _lstExpand)
                        {
                            List<BeanWorkflow> _lstWF = item.lstBeanWorkflow.Where(x => CmmFunction.removeSignVietnamese(x.Title).ToLowerInvariant().Contains(_content) ||
                                                                                        CmmFunction.removeSignVietnamese(x.TitleEN).ToLowerInvariant().Contains(_content))
                                                                            .ToList();
                            if (_lstWF != null && _lstWF.Count > 0)
                                _lstSearch.Add(new BeanBoardWorkflow() { beanWorkflowCategory = item.beanWorkflowCategory, lstBeanWorkflow = _lstWF });

                        }
                    }
                    else // không phải Category tất cả -> chỉ search trên category hiện hành
                    {
                        BeanBoardWorkflow item = _lstExpand.Where(x => x.beanWorkflowCategory.Title.Equals(_currentExpandWFCategory.Title)).FirstOrDefault();
                        List<BeanWorkflow> _lstWF = item.lstBeanWorkflow.Where(x => CmmFunction.removeSignVietnamese(x.Title).ToLowerInvariant().Contains(_content) ||
                                                                                    CmmFunction.removeSignVietnamese(x.TitleEN).ToLowerInvariant().Contains(_content))
                                                                        .ToList();
                        if (_lstWF != null && _lstWF.Count > 0)
                            _lstSearch.Add(new BeanBoardWorkflow() { beanWorkflowCategory = item.beanWorkflowCategory, lstBeanWorkflow = _lstWF });
                    }
                    InitExpandableListViewBoard(_lstSearch);
                    ////SetData_ListExpand(); 
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFavorite", ex);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFavorite", ex);
#endif
            }
        }

        private void Click_lnCurrentGroupWorkflow(object sender, EventArgs e)
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);

                #region Get View - Init Data
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                _imgDone.Visibility = ViewStates.Invisible;
                _tvTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "Nhóm quy trình" : "Group workflow";

                AdapterBoardChooseCategory _adapterCategory = new AdapterBoardChooseCategory(_mainAct, _rootView.Context, _lstWFCategory);
                StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                _recyData.SetAdapter(_adapterCategory);
                _recyData.SetLayoutManager(staggeredGridLayoutManager);
                _adapterCategory.CustomItemClick += (sender, e) =>
                {
                    try
                    {
                        // Remove search Content
                        if (!String.IsNullOrEmpty(_edtSearch.Text))
                        {
                            _imgDelete.Visibility = ViewStates.Gone;
                            _edtSearch.TextChanged -= TextChanged_EdtSearch;
                            _edtSearch.Text = "";
                            _edtSearch.TextChanged += TextChanged_EdtSearch;
                        }

                        // Handle Data
                        if (e != null && !String.IsNullOrEmpty(e.Title))
                        {
                            _currentExpandWFCategory = e;
                            _tvCurrentGroup.Text = e.Title;

                            if (_currentExpandWFCategory.Title.Equals(CmmFunction.GetTitle("TEXT_ALL", "Tất cả")))
                                InitExpandableListViewBoard(_lstExpand);
                            else // không phải Category tất cả
                            {
                                BeanBoardWorkflow _itemTemp = _lstExpand.Where(x => x.beanWorkflowCategory.Title.Equals(_currentExpandWFCategory.Title)).FirstOrDefault();
                                InitExpandableListViewBoard(_itemTemp != null ? new List<BeanBoardWorkflow>() { _itemTemp } : null); // đã xử lý null trong hàm
                            }
                        }
                        _dialogPopupControl.Dismiss();

                    }
                    catch (Exception) { }
                };

                #endregion

                #region Event
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                #region Show View                
                _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                Window window = _dialogPopupControl.Window;
                _dialogPopupControl.RequestWindowFeature(1);
                _dialogPopupControl.SetCanceledOnTouchOutside(false);
                _dialogPopupControl.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogPopupControl.SetContentView(_viewPopupControl);
                _dialogPopupControl.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = WindowManagerLayoutParams.MatchParent;
                s.Height = WindowManagerLayoutParams.MatchParent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private async void Click_Item_Favorite(object sender, BeanWorkflow e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    
                    //CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);
                    bool saveFavoritePrevios = e.Favorite;
                    e.Favorite = !e.Favorite;
                    _adapterExpandMainGroup.UpdateItemData(e);
                    _adapterExpandMainGroup.NotifyDataSetChanged();
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    conn.Update(e);
                    conn.Close();
                    await Task.Run(() =>
                    {
                        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                        bool _result = _pControlDynamic.SetFavoriteWorkflow(e.WorkflowID, !saveFavoritePrevios);
                        if (_result)
                        {
                            _pControlDynamic.UpdateMasterData<BeanWorkflowCategory>(null, true);
                            _pControlDynamic.UpdateMasterData<BeanWorkflow>(null, true);
                            /* e.Favorite = !e.Favorite;
                             _adapterExpandMainGroup.UpdateItemData(e);
                             _adapterExpandMainGroup.NotifyDataSetChanged();*/
                            _mainAct.RunOnUiThread(() =>
                            {
                                #region Update Left Menu
                                try
                                {
                                    FragmentLeftMenu _frag = (FragmentLeftMenu)_mainAct.SupportFragmentManager.FindFragmentByTag("FragmentLeftMenu");
                                    _frag.SetData(_setAllData: false);
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Item_Favorite", ex);
#endif
                                }
                                #endregion

                                #region Update List Expand
                                if (_lstExpand != null && _lstExpand.Count > 0)
                                {
                                    _expandData.Visibility = ViewStates.Visible;
                                    _lnNoDataExpand.Visibility = ViewStates.Gone;
                                    if (_adapterExpandMainGroup != null) // Đã có adapter
                                    {
                                        //_adapterExpandMainGroup.UpdateListData(_lstExpand);
                                        //_adapterExpandMainGroup.NotifyDataSetChanged();

                                        _adapterExpandMainGroup.UpdateItemData(e);
                                        _adapterExpandMainGroup.NotifyDataSetChanged();
                                    }
                                    else
                                    {
                                        InitExpandableListViewBoard(_lstExpand);
                                    }
                                }
                                else
                                {
                                    _expandData.Visibility = ViewStates.Gone;
                                    _lnNoDataExpand.Visibility = ViewStates.Visible;
                                }
                                #endregion

                                #region Update Recy Favorite

                                if (e.Favorite == true) // Click Favorite -> Add thêm item vào list _lstWFFavorite_Full
                                    _lstFavorite.Add(e);
                                else // Click UnFavorite -> remove item trong list _lstWFFavorite_Full
                                    _lstFavorite = _lstFavorite.Where(x => x.WorkflowID != e.WorkflowID).ToList();

                                if (_lstFavorite != null && _lstFavorite.Count > 0)
                                {
                                    _recyData.Visibility = ViewStates.Visible;
                                    _lnNoDataFavorite.Visibility = ViewStates.Gone;

                                    if (_adapterRecyFavorite != null)// Đã có adapter
                                    {
                                        _adapterRecyFavorite.UpdateListData(_lstFavorite);
                                        _adapterRecyFavorite.NotifyDataSetChanged();
                                    }
                                    else
                                    {
                                        InitRecyclerViewFavorite(_lstFavorite);
                                    }
                                }
                                else
                                {
                                    _recyData.Visibility = ViewStates.Gone;
                                    _lnNoDataFavorite.Visibility = ViewStates.Visible;
                                }
                                #endregion

                                CmmDroidFunction.HideProcessingDialog();
                            });
                        }
                        else
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                CmmDroidFunction.HideProcessingDialog();
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"),
                                                  CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                            });
                        }
                    });
                    
                }
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Item_Favorite", ex);
#endif
            }
        }

        private void Click_Item_Board(object sender, BeanWorkflow e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);

                    //FragmentBoardDetailGroup fragmentBoardDetailGroup = new FragmentBoardDetailGroup(e, 1);
                    //_mainAct.AddFragment(FragmentManager, fragmentBoardDetailGroup, "FragmentBoardDetailGroup", 0);

                    MainActivity.ChildAppWorkflow = e;
                    /* MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppHomePage;
                     MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppSingleListVTBD;*/ // khởi tạo lại optional luôn là Child VTBD
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.ChildAppKanban;
                    MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppKanban;
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ChildExpand_Board", ex);
#endif
            }
        }

        private void Click_Item_List(object sender, BeanWorkflow e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    FragmentBoardDetailGroup fragmentBoardDetailGroup = new FragmentBoardDetailGroup(e, 2);
                    _mainAct.AddFragment(FragmentManager, fragmentBoardDetailGroup, "FragmentBoardDetailGroup", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ChildExpand_List", ex);
#endif
            }
        }

        private void Click_Item_Report(object sender, BeanWorkflow e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtSearch, _mainAct);
                    FragmentBoardDetailGroup fragmentBoardDetailGroup = new FragmentBoardDetailGroup(e, 3);
                    _mainAct.AddFragment(FragmentManager, fragmentBoardDetailGroup, "FragmentBoardDetailGroup", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ChildExpand_Report", ex);
#endif
            }
        }

        private void ScrollChange_RecyFavorite(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                // tạm đóng do chưa có solution search

                ////LinearLayoutManager _manager = (LinearLayoutManager)_recyData.GetLayoutManager();
                ////int _tempLastVisible = _manager.FindLastCompletelyVisibleItemPosition();

                ////if (_tempLastVisible == _lstFavorite.Count - 1 && _allowLoadMore_Favorite == true)
                ////{
                ////    _adapterRecyFavorite.SetAllowLoadMore(_allowLoadMore_Favorite);
                ////    Action action = new Action(() =>
                ////    {
                ////        ProviderBase pBase = new ProviderBase();
                ////        List<BeanWorkflow> _lstMore = pBase.LoadMoreDataT<BeanWorkflow>(_queryFavorite, CmmDroidVariable.M_LoadMoreLimit, CmmDroidVariable.M_LoadMoreLimit, _lstFavorite.Count);

                ////        if (_lstMore != null && _lstMore.Count > 0)
                ////        {
                ////            if (_lstMore.Count < CmmDroidVariable.M_LoadMoreLimit)
                ////                _allowLoadMore_Favorite = false;
                ////            else
                ////                _allowLoadMore_Favorite = true;

                ////            _lstFavorite.AddRange(_lstMore);
                ////            _adapterRecyFavorite.SetAllowLoadMore(_allowLoadMore_Favorite);
                ////            //_adapterHomePageRecyVTBD.LoadMore(_lstMore);
                ////            _adapterRecyFavorite.NotifyDataSetChanged();
                ////        }
                ////        else
                ////        {
                ////            _allowLoadMore_Favorite = false;
                ////            _adapterRecyFavorite.SetAllowLoadMore(_allowLoadMore_Favorite);
                ////        }
                ////    });
                ////    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                ////}
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_ExpandData", ex);
#endif
            }
        }

        private void ScrollChange_ExpandData(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                //int _tempLastVisible = _expandData.LastVisiblePosition;
                //int _count = CmmDroidFunction.GetExpandAdapterItemCount(_adapterExpandMainGroup);

                //if (_tempLastVisible == _count - 1 && _allowLoadMore_Expand == true)
                //{

                //}
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_ExpandData", ex);
#endif
            }

        }
        #endregion

        #region Data
        private void SetData_ListExpand()
        {
            using SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                List<BeanWorkflowCategory> _lstCategoryTemp = new List<BeanWorkflowCategory>();

                _lstWFCategory.Clear();

                BeanWorkflowCategory _categoryAll = new BeanWorkflowCategory()
                {
                    Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả"),
                    ID = _idCategoryAll,
                    IsSelected = true
                }; // Add tất cả vào

                _lstCategoryTemp.Add(_categoryAll);
                _lstCategoryTemp.AddRange(conn.Query<BeanWorkflowCategory>("SELECT * FROM BeanWorkflowCategory ORDER BY Title ASC"));

                #region handle Expand
                _lstExpand.Clear();

                for (int i = 0; i < _lstCategoryTemp.Count; i++)
                {
                    if (_lstCategoryTemp[i].ID != _idCategoryAll) // Tất cả ko cần 
                    {
                        string _queryWorkflow = string.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowCategoryID = {0} AND StatusName = 'Active' AND IsPermission = 1 
                                                                ORDER BY WorkflowID ASC", _lstCategoryTemp[i].ID);

                        List<BeanWorkflow> _lstWorkflowTemp = conn.Query<BeanWorkflow>(_queryWorkflow);
                        if (_lstWorkflowTemp != null && _lstWorkflowTemp.Count > 0) // nếu có quy trình mới add
                        {
                            // Category
                            _lstWFCategory.Add(_lstCategoryTemp[i]);
                            // Expand
                            _lstExpand.Add(new BeanBoardWorkflow()
                            {
                                beanWorkflowCategory = _lstCategoryTemp[i],
                                lstBeanWorkflow = _lstWorkflowTemp
                            });
                        }
                    }
                    else // Tất cả
                        _lstWFCategory.Add(_lstCategoryTemp[i]);
                }

                // gán Category All vào current Category
                if (_categoryAll != null && !String.IsNullOrEmpty(_categoryAll.Title))
                {
                    _currentExpandWFCategory = _categoryAll;
                    _tvCurrentGroup.Text = _currentExpandWFCategory.Title;
                }
                InitExpandableListViewBoard(_lstExpand);
                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void SetData_RecyFavorite()
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            ProviderBase pBase = new ProviderBase();
            try
            {
                _lstFavorite.Clear();

                _queryFavorite = CTRLBoard._QueryFavoriteFull; // không có Search + ko có limit offset + load all
                _lstFavorite = conn.Query<BeanWorkflow>(_queryFavorite); // List All

                InitRecyclerViewFavorite(_lstFavorite);

                // Region load More (tạm đóng đến khi có solution)

                ////_queryFavorite = String.Format(CTRLBoard._QueryFavorite, _edtSearch.Text);

                ////_lstFavorite = pBase.LoadMoreDataT<BeanWorkflow>(_queryFavorite, CmmDroidVariable.M_LoadMoreLimit, CmmDroidVariable.M_LoadMoreLimit, _lstFavorite.Count);

                ////if (_lstFavorite != null && _lstFavorite.Count > 0)
                ////{
                ////    if (_lstFavorite.Count < CmmDroidVariable.M_LoadMoreLimit)
                ////        _allowLoadMore_Favorite = false;
                ////    else
                ////        _allowLoadMore_Favorite = true;

                ////    InitRecyclerViewFavorite(_lstFavorite);
                ////}
                ////else
                ////{
                ////    InitRecyclerViewFavorite(_lstFavorite);
                ////}
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData_RecyFavorite", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private void InitExpandableListViewBoard(List<BeanBoardWorkflow> _lstBoardWorkflow)
        {
            try
            {
                if (_lstBoardWorkflow != null && _lstBoardWorkflow.Count > 0)
                {
                    _expandData.Visibility = ViewStates.Visible;
                    _lnNoDataExpand.Visibility = ViewStates.Gone;

                    _expandData.StartAnimation(AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in));
                    _adapterExpandMainGroup = new AdapterExpandBoard_MainGroup(_mainAct, _rootView.Context, _lstBoardWorkflow);

                    _adapterExpandMainGroup.CustomItemClickChild_Favorite += Click_Item_Favorite;
                    _adapterExpandMainGroup.CustomItemClickChild_Board += Click_Item_Board;
                    _adapterExpandMainGroup.CustomItemClickChild_List += Click_Item_List;
                    _adapterExpandMainGroup.CustomItemClickChild_Report += Click_Item_Report;
                    _expandData.SetAdapter(_adapterExpandMainGroup);
                    for (int i = 0; i < _adapterExpandMainGroup.GroupCount; i++)
                    {
                        if (_lstBoardWorkflow[i].lstBeanWorkflow.Count > 0)
                            _expandData.ExpandGroup(i);
                    }
                }
                else
                {
                    _expandData.Visibility = ViewStates.Gone;
                    _lnNoDataExpand.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetListBoardExpand", ex);
#endif
            }
        }

        private void InitRecyclerViewFavorite(List<BeanWorkflow> _lstWorkflow)
        {
            try
            {
                if (_lstWorkflow != null && _lstWorkflow.Count > 0)
                {
                    _recyData.Visibility = ViewStates.Visible;
                    _lnNoDataFavorite.Visibility = ViewStates.Gone;

                    _recyData.StartAnimation(AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_fade_in));
                    _adapterRecyFavorite = new AdapterRecyBoard_MainGroup(_mainAct, _rootView.Context, _lstWorkflow);
                    _adapterRecyFavorite.SetAllowLoadMore(_allowLoadMore_Favorite);

                    _adapterRecyFavorite.CustomItemClick_Favorite += Click_Item_Favorite;
                    _adapterRecyFavorite.CustomItemClick_Board += Click_Item_Board;
                    _adapterRecyFavorite.CustomItemClick_List += Click_Item_List;
                    _adapterRecyFavorite.CustomItemClick_Report += Click_Item_Report;

                    _recyData.SetAdapter(_adapterRecyFavorite);
                    _recyData.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                }
                else
                {
                    _recyData.Visibility = ViewStates.Gone;
                    _lnNoDataFavorite.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetListBoardExpand", ex);
#endif
            }
        }
        #endregion
    }
}