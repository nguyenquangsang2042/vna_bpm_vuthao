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
using Android.Support.V4.Widget;
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
using BPMOPMobile.Droid.Presenter.Adapter;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentCreateWorkflow : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private Dialog _dialogPopupControl;
        private View _rootView;
        private ImageView _imgBack, _imgFilter;
        private TextView _tvNoData, _tvCurrentGroup, _tvtitle;
        private LinearLayout _lnCurrentGroupWorkflow, _lnNoData, _lnAll;
        private SwipeRefreshLayout _swipe;
        private ExpandableListView _expandData;
        private AdapterExpandCreateWorkflow_Main _adapterExpandCreateWorkflow;

        private List<BeanWorkflowCategory> _lstWorkflowCategory_Full = new List<BeanWorkflowCategory>();
        private List<BeanBoardWorkflow> _lstWFBoardExpand_Full = new List<BeanBoardWorkflow>(); // Không thao tác trên List này
        private List<BeanBoardWorkflow> _lstWFBoardExpand_Filter = new List<BeanBoardWorkflow>(); // Dành cho Adapter Expand
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _inflater = inflater;
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewCreateWorkflow, null);
                if (_mainAct._drawerLayout != null)
                {
                    _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                }
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflow_All);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewCreateWorkflow);
                _tvtitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewCreateWorkflow_Name);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewCreateWorkflow_Back);
                _imgFilter = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewCreateWorkflow_Filter);
                _lnCurrentGroupWorkflow = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflow_CurrentGroupWorkflow);
                _tvCurrentGroup = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewCreateWorkflow_CurrentGroupWorkflow);
                _expandData = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewCreateWorkflow_Content);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewCreateWorkflow_NoData);
                _tvNoData = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewCreateWorkflow_NoData);

                _expandData.SetGroupIndicator(null);
                _expandData.SetChildIndicator(null);
                _expandData.DividerHeight = 0;

                _lnCurrentGroupWorkflow.Click += Click_lnCurrentGroupWorkflow;
                _imgBack.Click += Click_Back;
                _swipe.Refresh += Swipe_RefreshData;
                //_lnAll.Click += delegate { };
                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);
                SetViewByLanguage();
                GetNewSetData();
            }
            return _rootView;
        }
        public FragmentCreateWorkflow()
        {

        }

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                    _tvtitle.Text = CmmFunction.GetTitle("TEXT_CREATENEW_WORKFLOW", "Tạo mới");
                }
                else
                {
                    _tvNoData.Text = CmmFunction.GetTitle("TEXT_NODATA", "No data");
                    _tvtitle.Text = CmmFunction.GetTitle("TEXT_CREATENEW_WORKFLOW", "Create new");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "SetViewByLanguage", ex);
#endif
            }
        }

        private void Click_Back(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "Click_Back", ex);
#endif
            }
        }

        private void Click_lnCurrentGroupWorkflow(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    //#region Get View - Init Data

                    //View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_SingleChoice, null);
                    //ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Close);
                    //TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_SingleChoice_Title);
                    //RecyclerView _recyData = _viewPopupControl.FindViewById<RecyclerView>(Resource.Id.recy_PopupControl_SingleChoice_Data);
                    //ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_SingleChoice_Done);

                    //_imgDone.Visibility = ViewStates.Invisible;
                    //_tvTitle.Text = CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "Nhóm quy trình" : "Group workflow";

                    //AdapterBoardChooseCategory _adapterCategory = new AdapterBoardChooseCategory(_mainAct, _rootView.Context, _lstWorkflowCategory_Full);
                    //StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.Vertical);
                    //_recyData.SetAdapter(_adapterCategory);
                    //_recyData.SetLayoutManager(staggeredGridLayoutManager);
                    //_adapterCategory.CustomItemClick += (sender, e) =>
                    //{
                    //    SetDataByGroupWorkflow(e);
                    //    _dialogPopupControl.Dismiss();
                    //};

                    //#endregion

                    //#region Event
                    //_imgClose.Click += (sender, e) =>
                    //{
                    //    _dialogPopupControl.Dismiss();
                    //};
                    //#endregion

                    //#region Show View                
                    //_dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
                    //Window window = _dialogPopupControl.Window;
                    //_dialogPopupControl.RequestWindowFeature(1);
                    //_dialogPopupControl.SetCanceledOnTouchOutside(false);
                    //_dialogPopupControl.SetCancelable(true);
                    //window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                    //window.SetGravity(GravityFlags.Bottom);
                    //var dm = _mainAct.Resources.DisplayMetrics;

                    //_dialogPopupControl.SetContentView(_viewPopupControl);
                    //_dialogPopupControl.Show();
                    //WindowManagerLayoutParams s = window.Attributes;
                    //s.Width = WindowManagerLayoutParams.MatchParent;
                    //s.Height = WindowManagerLayoutParams.MatchParent;
                    //window.Attributes = s;
                    //window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                    //#endregion
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentDetailWorkflow", "ShowPopup_ControlDate", ex);
#endif
            }
        }

        private void Click_ItemGroupworkflow(object sender, int e)
        {
            try
            {
                //adapterGroupWorkflow.UpdateCurrentSelectedGroup(e);
                //adapterGroupWorkflow.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "Click_ItemGroupworkflow", ex);
#endif
            }
        }

        private void Click_ItemChildExpandCreateWorkflow(object sender, BeanWorkflow e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    FragmentCreateWorkflowDetail fragmentCreateWorkflow = new FragmentCreateWorkflowDetail(e);
                    _mainAct.AddFragment(FragmentManager, fragmentCreateWorkflow, "FragmentCreateWorkflowDetail", 0);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "Click_ItemChildExpandCreateWorkflow", ex);
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
                        SetData();
                    });
                });
                _swipe.Refreshing = false;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "Swipe_RefreshData", ex);
#endif
                _mainAct.RunOnUiThread(() =>
                {
                    _swipe.Refreshing = false;
                });
            }
        }
        #endregion

        #region Data
        private async void GetNewSetData()
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                await Task.Run(() =>
                {
                    //pBase.UpdateAllDynamicData(true);
                    _mainAct.RunOnUiThread(() =>
                    {

                        SetData();
                    });
                });
            }
            catch (System.Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetData();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData", ex);
#endif
            }
        }

        private void SetData(bool _isRenewAdapter = true)
        {
            try
            {
                _lstWorkflowCategory_Full.Clear();
                _lstWFBoardExpand_Full.Clear();

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string _queryCategory = string.Format("SELECT * FROM BeanWorkflowCategory");
                BeanWorkflowCategory _categoryAll = new BeanWorkflowCategory() { Title = "Tất cả", ID = -99, IsSelected = true }; // Add tất cả vào

                _lstWorkflowCategory_Full.Add(_categoryAll);
                _lstWorkflowCategory_Full.AddRange(conn.Query<BeanWorkflowCategory>(_queryCategory));

                foreach (BeanWorkflowCategory item in _lstWorkflowCategory_Full)
                {
                    if (item.ID != -99) // Tất cả ko cần Search
                    {
                        string _queryWorkflow = string.Format("SELECT * FROM BeanWorkflow WHERE WorkflowCategoryID = {0}", item.ID);
                        List<BeanWorkflow> _lstWorkflowTemp = conn.Query<BeanWorkflow>(_queryWorkflow);

                        if (_lstWorkflowTemp != null && _lstWorkflowTemp.Count > 0)
                        {
                            #region Handle Expandable List
                            BeanBoardWorkflow _temp = new BeanBoardWorkflow(); // Cho Expandable List
                            _temp.beanWorkflowCategory = item;
                            _temp.lstBeanWorkflow = _lstWorkflowTemp;
                            _temp.IsExpand = true;
                            _lstWFBoardExpand_Full.Add(_temp);
                            #endregion
                        }
                    }

                }
                conn.Close();
                _lstWFBoardExpand_Filter = _lstWFBoardExpand_Full.ToList();
                if (_isRenewAdapter == true)
                {
                    SetCurrentGroupWorkflow(_categoryAll); // Đặt trạng thái tất cả
                    SetListExpandBoard(_lstWFBoardExpand_Filter);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentCreateWorkflow", "SetData", ex);
#endif
            }
        }

        private void SetDataByGroupWorkflow(BeanWorkflowCategory _WFcategory)
        {
            try
            {
                if (_WFcategory.Title.Equals("Tất cả"))
                    _lstWFBoardExpand_Filter = _lstWFBoardExpand_Full.ToList();
                else
                    _lstWFBoardExpand_Filter = _lstWFBoardExpand_Full.Where(x => x.beanWorkflowCategory.Title.Equals(_WFcategory.Title)).ToList();

                SetCurrentGroupWorkflow(_WFcategory);
                SetListExpandBoard(_lstWFBoardExpand_Filter);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetData", ex);
#endif
            }
        }

        private void SetCurrentGroupWorkflow(BeanWorkflowCategory _category)
        {
            try
            {
                if (_category != null && !String.IsNullOrEmpty(_category.Title))
                    _tvCurrentGroup.Text = _category.Title;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetCurrentGroupWorkflow", ex);
#endif
            }
        }

        private void SetListExpandBoard(List<BeanBoardWorkflow> _lstBoardWorkflow)
        {
            try
            {
                if (_lstBoardWorkflow != null && _lstBoardWorkflow.Count > 0)
                {
                    _expandData.Visibility = ViewStates.Visible;
                    _lnNoData.Visibility = ViewStates.Gone;

                    _expandData.Animation = AnimationUtils.LoadAnimation(_rootView.Context, Resource.Animation.anim_falldown);
                    _adapterExpandCreateWorkflow = new AdapterExpandCreateWorkflow_Main(_mainAct, _rootView.Context, _lstBoardWorkflow);
                    _adapterExpandCreateWorkflow.CustomItemClickChild -= Click_ItemChildExpandCreateWorkflow;
                    _adapterExpandCreateWorkflow.CustomItemClickChild += Click_ItemChildExpandCreateWorkflow;
                    _expandData.SetAdapter(_adapterExpandCreateWorkflow);

                    for (int i = 0; i < _lstBoardWorkflow.Count; i++)
                    {
                        if (_lstBoardWorkflow[i].IsExpand == true)
                            _expandData.ExpandGroup(i);
                        else
                            _expandData.CollapseGroup(i);
                    }
                }
                else
                {
                    _expandData.Visibility = ViewStates.Gone;
                    _lnNoData.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError("FragmentBoard", "SetListBoardExpand", ex);
#endif
            }
        }
        #endregion

    }
}