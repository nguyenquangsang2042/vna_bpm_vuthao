using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using Jp.Wasabeef;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlViewFullInfoFormat : SharedView_PopupControlBase
    {
        public SharedView_PopupControlViewFullInfoFormat(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {

        }

        public override void InitializeValue_Master(ViewElement clickedElement)
        {
            base.InitializeValue_Master(clickedElement);
        }

        public override void InitializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JObject JObjectChild)
        {
            base.InitializeValue_InputGridDetail(elementParent, elementPopup, JObjectChild);
        }
        public override void InitializeValue_Master_CreateTask(ViewElement elementParent)
        {
            base.InitializeValue_Master_CreateTask(elementParent);
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputTextFormat, null);
                RelativeLayout _relaContent = _viewPopupControl.FindViewById<RelativeLayout>(Resource.Id.rela_PopupControl_InputTextFormat_Content);
                LinearLayout _lnContentClick = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_InputTextFormat_ContentClick);
                ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputTextFormat_Title);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Done);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_Delete);
                RichEditor mEditor = _viewPopupControl.FindViewById<RichEditor>(Resource.Id.editor_PopupControl_InputTextFormat);
                HorizontalScrollView _scrollAction = _viewPopupControl.FindViewById<HorizontalScrollView>(Resource.Id.scroll_PopupControl_InputTextFormat_Action);

                LinearLayout.LayoutParams _params = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                _params.SetMargins(15, 15, 15, 30);
                _relaContent.LayoutParameters = _params;// set cho full màn hình
                _relaContent.Enabled = false;

                _lnContentClick.Visibility = ViewStates.Visible;
                _lnContentClick.Click += (sender, e) => { };  // disable click

                _scrollAction.Visibility = ViewStates.Gone;
                _imgDone.Visibility = ViewStates.Invisible;
                _imgDelete.Visibility = ViewStates.Gone;

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailCreateTask:
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;

                            if (!String.IsNullOrEmpty(elementParent.Value))
                            {
                                mEditor.SetHtml(elementParent.Value);

                            }

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;

                            if (!String.IsNullOrEmpty(elementPopup.Value))
                            {
                                mEditor.SetHtml(elementPopup.Value);
                            }

                            break;
                        }
                }
                #endregion

                #region Show View                
                Dialog _dialogPopupControl = new Dialog(_rootView.Context, Resource.Style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
                #endregion

                #region Event
                _imgBack.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    _dialogPopupControl.Dismiss();
                };
                #endregion

                //CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
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