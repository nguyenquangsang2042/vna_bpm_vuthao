using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
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

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentDetailProcess : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private View _rootView;
        private TextView _tvTitle, _tvWorkflowTitle;
        private ImageView _imgBack;
        private LinearLayout _lnAll;
        private ExpandableListView _expandProcess;
        private LinearLayout _lnProcess, _lnNoData;
        private AdapterExpandDetailProcess_Ver2 _adapterExpandDetailProcess;
        private List<BeanQuaTrinhLuanChuyen> _lstQTLC = new List<BeanQuaTrinhLuanChuyen>();
        private BeanWorkflowItem _workflowItem = new BeanWorkflowItem();
        private BeanNotify _notifyItem = new BeanNotify();
        private ControllerDetailWorkflow CTRLDetailWorkflow = new ControllerDetailWorkflow();
        private FragmentDetailWorkflow _fragmentDetailWorkflow;
 
        public FragmentDetailProcess(FragmentDetailWorkflow _fragmentDetailWorkflow, BeanWorkflowItem _workflowItem, BeanNotify _notifyItem, List<BeanQuaTrinhLuanChuyen> _lstQTLC)
        {
            this._workflowItem = _workflowItem;
            this._notifyItem = _notifyItem;
            this._lstQTLC = _lstQTLC;
            this._fragmentDetailWorkflow = _fragmentDetailWorkflow;
        }
   
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
      
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewDetailProcess, null);
                _lnAll = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailProcess_All);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewDetailProcess_Back);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailProcess_Name);
                _tvWorkflowTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewDetailProcess_WorkflowName);
                _lnProcess = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailProcess_Process);
                _lnNoData = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewDetailProcess_NoData);

                _expandProcess = _rootView.FindViewById<ExpandableListView>(Resource.Id.expand_ViewDetailProcess_Process);
                _expandProcess.SetGroupIndicator(null);
                _expandProcess.SetChildIndicator(null);
                _expandProcess.DividerHeight = 0;
            }
            _tvWorkflowTitle.Click += Click_tvTitle;
            _imgBack.Click += Click_imgBack;
            _lnAll.Click += (sender, e) => { };
            SetView();

            Action action = new Action(() =>
            {
                SetData();
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 600);

            return _rootView;
        }

        #region Event
        private void SetView()
        {
            try
            {
                CTRLDetailWorkflow.SetTitleByItem(_tvWorkflowTitle, _workflowItem, _notifyItem);
                _tvTitle.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_HISTORY", "Luồng phê duyệt");
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - SetView - Error: " + ex.Message);
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
                Console.WriteLine("Author: khoahd - Click_imgBack - Error: " + ex.Message);
#endif
            }
        }

        private void Click_tvTitle(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true && CmmDroidFunction.CheckTextViewIsEllipsized(_tvWorkflowTitle) == true)
                {
                    // gọi lại hàm xem full info
                    ViewElement tempElement = new ViewElement();
                    tempElement.Title = "";
                    tempElement.Value = _tvWorkflowTitle.Text;

                    SharedView_PopupControlViewFullInfo _popupControlViewFullInfo = new SharedView_PopupControlViewFullInfo(_inflater, _mainAct, this, "FragmentDetailProcess", _rootView);
                    _popupControlViewFullInfo.InitializeValue_Master(tempElement);
                    _popupControlViewFullInfo.InitializeView();
                    //_fragmentDetailWorkflow.ShowPopup_ViewFullInformation(tempElement);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetView_Action", ex);
#endif
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                if (_lstQTLC != null && _lstQTLC.Count > 0)
                {
                    _lstQTLC = _lstQTLC.OrderBy(x => x.Created).ToList();

                    // Set Adapter
                    _adapterExpandDetailProcess = new AdapterExpandDetailProcess_Ver2(_mainAct, _rootView.Context, _lstQTLC);
                    _expandProcess.SetAdapter(_adapterExpandDetailProcess);
                    for (int i = 0; i < _adapterExpandDetailProcess.GroupCount; i++)
                    {
                        _expandProcess.ExpandGroup(i);
                    }

                    _lnProcess.Visibility = ViewStates.Visible;
                    _lnNoData.Visibility = ViewStates.Gone;
                    _lnProcess.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                }
                else
                {
                    _lnProcess.Visibility = ViewStates.Gone;
                    _lnNoData.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }
        #endregion
    }
}