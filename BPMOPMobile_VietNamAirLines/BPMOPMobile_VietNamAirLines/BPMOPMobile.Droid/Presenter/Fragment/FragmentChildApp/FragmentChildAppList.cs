using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
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
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.SharedView;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppList : CustomBaseFragment
    {
        private enum EnumCurrentCategory
        {
            [Description("Current Page is List")]
            List = 1,
            [Description("Current Page is Dynamic")]
            Dynamic = 2,
        }
        private MainActivity _mainAct;
        private View _rootView, _popupViewFilter;
        private ImageView _imgBack, _imgCategory, _imgFilter, _imgSubPrevious, _imgSubNext, _imgShowSearch, _imgDeleteSearch;
        private LinearLayout _lnCategoryDynamic, _lnCategoryList, _lnNoData1, _lnNoData2, _lnBottomNavigation, _lnToolbar, _lnSearch;
        private SwipeRefreshLayout _swipe;
        private EditText _edtSearch;
        private TextView _tvTitle, _tvSubTitle, _tvNoData1, _tvNoData2;
        private RecyclerView _recyCategoryList, _recyCategoryDynamic;
        private PopupWindow _popupFilter;

        private AdapterFragmentListDynamic _adapterListDynamic;
        private AdapterFragmentListStable _adapterListStable;

        private List<BeanWFDetailsHeader> _lstHeaderDynamic = new List<BeanWFDetailsHeader>();
        private List<JObject> _lstJObjectDynamic = new List<JObject>();
        private List<BeanResourceView> _lstResourceView = new List<BeanResourceView>();
        private bool _allowLoadMore = true;
        private int _flagCurrentCategory = (int)EnumCurrentCategory.List;
        private bool _flagIsFiltering = false;

        public BeanWorkflow _currentWorkflow;
        public BeanResourceView _currentResourceView;

        public FragmentChildAppList(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewList, null);
                _lnToolbar = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_Toolbar);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_Back);
                _imgCategory = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_Category);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_Filter);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewList_Title);
                _tvSubTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewList_SubTitle);
                _imgSubPrevious = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_SubTitle_Previous);
                _imgSubNext = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_SubTitle_Next);
                _tvNoData1 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewList_NoData1);
                _tvNoData2 = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewList_NoData2);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewList);

                // Search
                _lnSearch = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_Search);
                _edtSearch = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewList_Search);
                _imgDeleteSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_Search_Delete);
                _imgShowSearch = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewList_ShowSearch);

                _recyCategoryList = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewList_Category_List);
                _recyCategoryDynamic = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewList_Category_Dynamic);
                _lnCategoryDynamic = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_Category_Dynamic);
                _lnCategoryList = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_Category_List);
                _lnNoData1 = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_NoData1);
                _lnNoData2 = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_NoData2);
                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewList_BottomNavigation);

                _swipe.Refresh += Swipe_RefreshData;
                _imgCategory.Click += Click_imgCategory;
                _imgShowSearch.Click += Click_imgShowSearch;
                _imgFilter.Click += Click_imgFilter;
                _imgBack.Click += Click_Menu;
                _imgSubPrevious.Click += Click_imgSubPrevious;
                _edtSearch.TextChanged += TextChanged_edtSearch;
                _imgSubNext.Click += Click_imgSubNext;
                _recyCategoryDynamic.ScrollChange += ScrollChange_RecyDynamic;
                _recyCategoryList.ScrollChange += ScrollChange_RecyStable;

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                _params.AddRule(LayoutRules.AlignParentRight);
                _imgFilter.LayoutParameters = _params;

                SetViewByLanguague();
                SetData();
            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    SetViewByLanguague();
                    SetData();
                }
            }
            CmmEvent.UpdateLangComplete += CmmEvent_ChangeLanguage;

            // Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            return _rootView;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        #region Event
        private void SetViewByLanguague()
        {
            try
            {
                _tvTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangEN) ? _currentWorkflow.TitleEN : _currentWorkflow.Title;
                _tvNoData1.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
                _tvNoData2.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView", ex);
#endif
            }
        }

        public void SetColor_ImageShowSearch_ByFlag(bool flag)
        {
            if (flag == true)
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clVer2BlueMain)));
            else
                _imgShowSearch.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomDisable)));
        }

        private void ScrollChange_RecyStable(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                LinearLayoutManager _managerStableList = (LinearLayoutManager)_recyCategoryList.GetLayoutManager();
                int _tempLastVisible = _managerStableList.FindLastCompletelyVisibleItemPosition();

                if (_tempLastVisible == _lstJObjectDynamic.Count - 1 && _allowLoadMore == true)
                {
                    _adapterListStable.SetAllowLoadMore(_allowLoadMore);
                    ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                    List<JObject> _lstMore = _pConTrolDynamic.GetDynamicWorkflowItem(_currentResourceView.ID, null, CmmVariable.M_DataLimitRow, _lstJObjectDynamic.Count);

                    Action action = new Action(() =>
                    {
                        if (_lstMore != null && _lstMore.Count > 0)
                        {
                            if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                _allowLoadMore = false;
                            else
                                _allowLoadMore = true;

                            _lstJObjectDynamic.AddRange(_lstMore);
                            _adapterListStable.SetAllowLoadMore(_allowLoadMore);
                            _adapterListStable.NotifyDataSetChanged();
                        }
                        else
                        {
                            _allowLoadMore = false;
                            _adapterListStable.SetAllowLoadMore(_allowLoadMore);
                        }
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_RecyDynamic", ex);
#endif
            }
        }

        private void ScrollChange_RecyDynamic(object sender, View.ScrollChangeEventArgs e)
        {
            try
            {
                LinearLayoutManager _customLNM = (LinearLayoutManager)_recyCategoryDynamic.GetLayoutManager();
                int _tempLastVisible = _customLNM.FindLastCompletelyVisibleItemPosition();

                if (_tempLastVisible == _lstJObjectDynamic.Count - 1 && _allowLoadMore == true)
                {
                    _adapterListDynamic.SetAllowLoadMore(_allowLoadMore);
                    ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                    List<JObject> _lstMore = _pConTrolDynamic.GetDynamicWorkflowItem(_currentResourceView.ID, null, CmmVariable.M_DataLimitRow, _lstJObjectDynamic.Count);

                    Action action = new Action(() =>
                    {
                        if (_lstMore != null && _lstMore.Count > 0)
                        {
                            if (_lstMore.Count < CmmVariable.M_DataLimitRow)
                                _allowLoadMore = false;
                            else
                                _allowLoadMore = true;

                            _lstJObjectDynamic.AddRange(_lstMore);
                            _adapterListDynamic.SetAllowLoadMore(_allowLoadMore);
                            _adapterListDynamic.NotifyDataSetChanged();
                        }
                        else
                        {
                            _allowLoadMore = false;
                            _adapterListDynamic.SetAllowLoadMore(_allowLoadMore);
                        }
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime - 100);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ScrollChange_RecyDynamic", ex);
#endif
            }
        }

        private void CmmEvent_ChangeLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                SetViewByLanguague();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CmmEvent_ChangeLanguage", ex);
#endif
            }
        }

        private void Click_Menu(object sender, EventArgs e)
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

        private void Click_imgSubNext(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    int _currentIndex = _lstResourceView.FindIndex(x => x.ID == _currentResourceView.ID);
                    if (_currentIndex != (_lstResourceView.Count - 1))
                    {
                        _imgSubNext.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        SetData_ResourceViewID(_lstResourceView[_currentIndex + 1]);

                        if (!String.IsNullOrEmpty(_edtSearch.Text)) // xóa trạng thái Search ra
                        {
                            _imgDeleteSearch.Visibility = ViewStates.Gone;
                            _edtSearch.TextChanged -= TextChanged_edtSearch;
                            _edtSearch.Text = "";
                            _edtSearch.TextChanged += TextChanged_edtSearch;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgSubNext", ex);
#endif
            }
        }

        private void Click_imgSubPrevious(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    int _currentIndex = _lstResourceView.FindIndex(x => x.ID == _currentResourceView.ID);
                    if (_currentIndex != 0)
                    {
                        _imgSubPrevious.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                        SetData_ResourceViewID(_lstResourceView[_currentIndex - 1]);

                        if (!String.IsNullOrEmpty(_edtSearch.Text)) // xóa trạng thái Search ra
                        {
                            _imgDeleteSearch.Visibility = ViewStates.Gone;
                            _edtSearch.TextChanged -= TextChanged_edtSearch;
                            _edtSearch.Text = "";
                            _edtSearch.TextChanged += TextChanged_edtSearch;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgSubPrevious", ex);
#endif
            }
        }

        private void Click_imgCategory(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _imgCategory.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                    if (_flagCurrentCategory == (int)EnumCurrentCategory.List)
                    {
                        _imgCategory.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                        _flagCurrentCategory = (int)EnumCurrentCategory.Dynamic;
                        _lnCategoryDynamic.Visibility = ViewStates.Visible;
                        _lnCategoryList.Visibility = ViewStates.Gone;
                    }
                    else if (_flagCurrentCategory == (int)EnumCurrentCategory.Dynamic)
                    {
                        _imgCategory.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                        _flagCurrentCategory = (int)EnumCurrentCategory.List;
                        _lnCategoryDynamic.Visibility = ViewStates.Gone;
                        _lnCategoryList.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgBack", ex);
#endif
            }
        }

        private void Click_imgFilter(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                {
                    List<ComponentRow1> _lstControl = new List<ComponentRow1>();

                    #region Get View
                    LayoutInflater _layoutInflater = (LayoutInflater)_mainAct.GetSystemService(Android.Content.Context.LayoutInflaterService);

                    _popupViewFilter = _layoutInflater.Inflate(Resource.Layout.PopupListFilter, null);
                    _popupFilter = new PopupWindow(_popupViewFilter, WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
                    LinearLayout _lnDynamic = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupListFilter_Dynamic);
                    LinearLayout _lnTopBlur = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupListFilter_TopBlur);
                    LinearLayout _lnBottomBlur = _popupViewFilter.FindViewById<LinearLayout>(Resource.Id.ln_PopupListFilter_BottomBlur);
                    TextView _tvThietLapLai = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupListFilter_ThietLapLai);
                    TextView _tvApDung = _popupViewFilter.FindViewById<TextView>(Resource.Id.tv_PopupListFilter_ApDung);

                    #endregion

                    #region Show View
                    _popupFilter.Focusable = true;
                    _popupFilter.OutsideTouchable = false;
                    _popupFilter.ShowAsDropDown(_lnToolbar);
                    #endregion

                    #region Init Data

                    if (_lstHeaderDynamic != null && _lstHeaderDynamic.Count > 0)
                    {
                        foreach (BeanWFDetailsHeader beanWFDetailsHeader in _lstHeaderDynamic)
                        {
                            ViewElement _element = new ViewElement();
                            _element.IsRequire = true;
                            _element.Enable = true;
                            _element.InternalName = beanWFDetailsHeader.internalName;
                            _element.Title = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? beanWFDetailsHeader.Title : beanWFDetailsHeader.TitleEN;
                            _element.Value = "";

                            switch (beanWFDetailsHeader.FieldTypeId)
                            {
                                case (int)CmmFunction.DynamicFieldTypeID.UserGroup: // User group
                                    {
                                        _element.DataType = "selectusergroup";
                                        break;
                                    }
                                case (int)CmmFunction.DynamicFieldTypeID.DateAndTime: // Date time
                                    {
                                        _element.DataType = "datetime";
                                        break;
                                    }
                                case (int)CmmFunction.DynamicFieldTypeID.Lookup:
                                case (int)CmmFunction.DynamicFieldTypeID.ComboBox:
                                case (int)CmmFunction.DynamicFieldTypeID.DropDownList:
                                case (int)CmmFunction.DynamicFieldTypeID.Radio:
                                case (int)CmmFunction.DynamicFieldTypeID.CheckBox:
                                case (int)CmmFunction.DynamicFieldTypeID.Choice:
                                    {
                                        _element.DataType = "singlechoice";
                                        break;
                                    }
                                case (int)CmmFunction.DynamicFieldTypeID.Calculated:
                                case (int)CmmFunction.DynamicFieldTypeID.Currency:
                                case (int)CmmFunction.DynamicFieldTypeID.Number:
                                case (int)CmmFunction.DynamicFieldTypeID.MultipleLinesText:
                                case (int)CmmFunction.DynamicFieldTypeID.SingleLineText:
                                default:
                                    {
                                        _element.DataType = "textinputmultiline";
                                        break;
                                    }
                            }

                            ComponentRow1 _component = new ComponentRow1(_mainAct, _lnDynamic, _element, -1, true, 1);
                            _component.InitializeFrameView(_lnDynamic);
                            _component.SetTitle();
                            _component.SetValue();
                            _component.SetEnable();
                            _component.SetProprety();

                            _lstControl.Add(_component);
                        }
                    }
                    #endregion

                    #region Event
                    _lnTopBlur.Click += (sender, e) =>
                    {
                        _popupFilter.Dismiss();
                    };

                    _lnBottomBlur.Click += (sender, e) =>
                    {
                        _popupFilter.Dismiss();
                    };

                    _tvApDung.Click += (sender, e) =>
                    {
                        List<KeyValuePair<string, string>> _lstSearch = new List<KeyValuePair<string, string>>();
                        foreach (var item in _lstControl)
                        {
                            _lstSearch.Add(new KeyValuePair<string, string>(item._element.InternalName, item._element.Value));
                        }

                        _flagIsFiltering = true;
                        SetViewByFilterFlag(_flagIsFiltering);
                        _popupFilter.Dismiss();
                    };
                    #endregion
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgFilter", ex);
#endif
            }
        }

        public void TextChanged_edtSearch(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(_edtSearch.Text))
                {
                    //SetColor_ImageShowSearch_ByFlag(true);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
                    _imgDeleteSearch.Visibility = ViewStates.Visible;

                    string _content = CmmFunction.removeSignVietnamese(_edtSearch.Text.ToString()).ToLowerInvariant();

                    List<JObject> _lstSearch = _lstJObjectDynamic.Where(x => x != null && x.ToString().ToLowerInvariant().Contains(_content)).ToList();
                    if (_lstSearch != null && _lstSearch.Count > 0)
                    {
                        _allowLoadMore = false;

                        SetData_ListDynamic(_lstHeaderDynamic, _lstSearch);
                        SetData_ListStable(_lstHeaderDynamic, _lstSearch);

                    }
                    else
                    {
                        _recyCategoryDynamic.Visibility = ViewStates.Gone;
                        _recyCategoryList.Visibility = ViewStates.Gone;
                    }
                }
                else // empty -> tra lai ban dau -> giống như Set Data
                {
                    // SetColor_ImageShowSearch_ByFlag(false);
                    _edtSearch.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                    _imgDeleteSearch.Visibility = ViewStates.Gone;

                    SetData_ListDynamic(_lstHeaderDynamic, _lstJObjectDynamic);
                    SetData_ListStable(_lstHeaderDynamic, _lstJObjectDynamic);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_edtSearch", ex);
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
                        _swipe.Refreshing = false;
                        Action action = new Action(() =>
                        {
                            _flagIsFiltering = false;

                            SetViewByFilterFlag(_flagIsFiltering);
                            SetViewByLanguague();
                            SetData(_isShowDialog: false, _setResourceViewID: false);
                            SetData_ResourceViewID(_currentResourceView, false);

                            if (_flagCurrentCategory == (int)EnumCurrentCategory.Dynamic) // focus lại trang stable
                                Click_imgCategory(null, null);
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
                });
            }
        }

        private void Click_ItemList(object sender, JObject e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    string _query = String.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", e["ID"].ToString());
                    List<BeanWorkflowItem> _lstWFItem = conn.Query<BeanWorkflowItem>(_query);
                    if (_lstWFItem != null && _lstWFItem.Count > 0)
                    {
                        FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(_lstWFItem[0], null, this.GetType().Name);
                        _mainAct.AddFragment(FragmentManager, detailWorkFlow, typeof(FragmentDetailWorkflow).Name, 0);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_ItemDynamicList", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region Data
        private void SetData(bool _isShowDialog = true, bool _setResourceViewID = true)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                //string query = @"SELECT * FROM BeanResourceView WHERE ResourceId = {0} AND Status = 1 AND [Index] {1} ORDER BY MenuId ASC";
                //string query = @"SELECT *, IIF([Index] = 0, NULL, [Index]) as sort_order_rule
                //                 FROM BeanResourceView 
                //                 WHERE ResourceId = 1206 AND Status = 1 AND TypeId = 0 AND [Index] >= 0
                //                 ORDER BY sort_order_rule DESC, MenuId ASC";

                string query = string.Format(@"SELECT *, (CASE WHEN[Index] = 0 THEN NULL ELSE 1 END) AS sort_order_rule
                                               FROM BeanResourceView 
                                               WHERE ResourceId = {0} AND Status = 1 AND TypeId = 0 AND [Index] >= 0
                                               ORDER BY sort_order_rule DESC, MenuId ASC",_currentWorkflow.WorkflowID);

                _lstResourceView = conn.Query<BeanResourceView>(query);

                if (_setResourceViewID && (_lstResourceView != null && _lstResourceView.Count > 0))
                {
                    SetData_ResourceViewID(_lstResourceView[0], _isShowDialog);
                }
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

        /// <summary>
        /// Gọi Data theo Resource view Item
        /// </summary>
        /// <param name="_itemResourceView"></param>
        /// <param name="_isShowDialog"></param>
        private async void SetData_ResourceViewID(BeanResourceView _itemResourceView, bool _isShowDialog = true)
        {
            try
            {
                _currentResourceView = _itemResourceView;
                _tvSubTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangEN) ? _itemResourceView.TitleEN : _itemResourceView.Title;

                if (_isShowDialog)
                    CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                    _lstHeaderDynamic = _pConTrolDynamic.GetDynamicFormField(_itemResourceView.ID, CmmVariable.M_DataFilterAPILimitData, 0);
                    _lstJObjectDynamic = _pConTrolDynamic.GetDynamicWorkflowItem(_itemResourceView.ID, null, CmmVariable.M_DataFilterAPILimitData, 0);

                    _mainAct.RunOnUiThread(() =>
                    {
                        if ((_lstHeaderDynamic != null && _lstHeaderDynamic.Count > 0) && (_lstJObjectDynamic != null && _lstJObjectDynamic.Count > 0))
                        {
                            if (_lstJObjectDynamic.Count < CmmVariable.M_DataLimitRow)
                                _allowLoadMore = false;
                            else
                                _allowLoadMore = true;

                            SetData_ListDynamic(_lstHeaderDynamic, _lstJObjectDynamic);
                            SetData_ListStable(_lstHeaderDynamic, _lstJObjectDynamic);

                        }
                        else
                        {
                            _recyCategoryDynamic.Visibility = ViewStates.Gone;
                            _recyCategoryList.Visibility = ViewStates.Gone;
                            _allowLoadMore = false;
                        }

                        if (_isShowDialog)
                            CmmDroidFunction.HideProcessingDialog();
                    });
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
                if (_isShowDialog)
                    CmmDroidFunction.HideProcessingDialog();
            }
        }

        private void SetData_ListDynamic(List<BeanWFDetailsHeader> lstHeaderDynamic, List<JObject> lstJObjectDynamic)
        {
            if ((lstHeaderDynamic != null && lstHeaderDynamic.Count > 0) && (lstJObjectDynamic != null && lstJObjectDynamic.Count > 0))
            {
                _recyCategoryDynamic.Visibility = ViewStates.Visible;
                _recyCategoryDynamic.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                _adapterListDynamic = new AdapterFragmentListDynamic(_mainAct, _rootView.Context, _lstHeaderDynamic, _lstJObjectDynamic);
                _adapterListDynamic.CustomItemClick += Click_ItemList;
                _adapterListDynamic.SetAllowLoadMore(_allowLoadMore);
                _recyCategoryDynamic.SetAdapter(_adapterListDynamic);
                _recyCategoryDynamic.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
            }
            else
                _recyCategoryDynamic.Visibility = ViewStates.Gone;
        }

        private void SetData_ListStable(List<BeanWFDetailsHeader> lstHeaderDynamic, List<JObject> lstJObjectDynamic)
        {
            if ((lstHeaderDynamic != null && lstHeaderDynamic.Count > 0) && (lstJObjectDynamic != null && lstJObjectDynamic.Count > 0))
            {
                _recyCategoryList.Visibility = ViewStates.Visible;
                _recyCategoryList.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                _adapterListStable = new AdapterFragmentListStable(_rootView.Context, lstHeaderDynamic, _lstJObjectDynamic, _mainAct);
                _adapterListStable.CustomItemClick += Click_ItemList;
                _adapterListStable.SetAllowLoadMore(_allowLoadMore);
                _recyCategoryList.SetAdapter(_adapterListStable);
                _recyCategoryList.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
            }
            else
                _recyCategoryDynamic.Visibility = ViewStates.Gone;
        }

        private void SetViewByFilterFlag(bool _flagIsFiltering)
        {
            try
            {
                _imgFilter.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (!_flagIsFiltering)
                {
                    _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
                }
                else
                {
                    _imgFilter.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomEnable)));
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByFilterFlag", ex);
#endif
            }
        }
        #endregion
    }
}