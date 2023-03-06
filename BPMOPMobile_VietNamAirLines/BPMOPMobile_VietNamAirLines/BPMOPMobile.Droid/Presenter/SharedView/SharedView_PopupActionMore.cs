using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupActionMore : SharedView_PopupActionBase
    {
        public SharedView_PopupActionMore(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
        }

        public override void InitializeValue_DetailWorkflow_ActionMore(List<ButtonAction> lstActionMore)
        {
            base.InitializeValue_DetailWorkflow_ActionMore(lstActionMore);
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data
                View scheduleDetail = _inflater.Inflate(Resource.Layout.PopupActionMore, null);
                ListView _lvAction = scheduleDetail.FindViewById<ListView>(Resource.Id.lv_PopupActionMore);
                TextView _tvClose = scheduleDetail.FindViewById<TextView>(Resource.Id.tv_PopupActionMore_Close);

                _tvClose.Text = CmmFunction.GetTitle("TEXT_CLOSE", "Thoát");
                #endregion

                #region Show View
                Dialog _dialogActionMore = new Dialog(_rootView.Context);
                Window window = _dialogActionMore.Window;
                _dialogActionMore.RequestWindowFeature(1);
                _dialogActionMore.SetCanceledOnTouchOutside(false);
                _dialogActionMore.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Bottom);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogActionMore.SetContentView(scheduleDetail);
                _dialogActionMore.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels;
                s.Height = WindowManagerLayoutParams.WrapContent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event

                AdapterControlActionMore _adapterControlActionMore = new AdapterControlActionMore(_rootView.Context, base.lstActionMore);
                _lvAction.Adapter = _adapterControlActionMore;
                _lvAction.ItemClick += (sender, e) =>
                {
                    try
                    {
                        _dialogActionMore.Dismiss();

                        switch (flagView)
                        {
                            case (int)FlagViewControlAction.DetailWorkflow:
                                {
                                    FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                    _frag.Click_Action(base.lstActionMore[e.Position]);
                                    break;
                                }
                        }
                    }
                    catch (Exception)
                    {

                    }
                };
                _tvClose.Click += delegate
                {
                    _dialogActionMore.Dismiss();
                };

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }
    }
}