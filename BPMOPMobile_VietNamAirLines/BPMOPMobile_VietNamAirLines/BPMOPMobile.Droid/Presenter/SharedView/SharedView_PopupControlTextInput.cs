using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using Com.Telerik.Widget.Calendar;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlTextInput : SharedView_PopupControlBase
    {
        private ControllerBase CTRLBase = new ControllerBase();
        public SharedView_PopupControlTextInput(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_InputText, null);
                ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_InputText_Title);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Done);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_Delete);
                EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_InputText);
                ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputText_ClearText);

                _imgDelete.Visibility = ViewStates.Gone;

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;

                            if (!String.IsNullOrEmpty(elementParent.Value))
                                _edtContent.Text = CmmDroidFunction.FormatHTMLToString(elementParent.Value);
                            else
                                _edtContent.Text = "";

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;

                            if (!String.IsNullOrEmpty(elementPopup.Value))
                                _edtContent.Text = CmmDroidFunction.FormatHTMLToString(elementPopup.Value);
                            else
                                _edtContent.Text = "";

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
                    CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);
                    _dialogPopupControl.Dismiss();
                };

                _imgClearText.Click += (sender, e) =>
                {
                    _edtContent.Text = "";
                };

                _imgDelete.Click += (sender, e) =>
                {
                    _edtContent.Text = "";
                    _imgDone.PerformClick();
                };

                _imgDone.Click += delegate
                {
                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                            {
                                CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);

                                string _result = "";
                                if (!String.IsNullOrEmpty(_edtContent.Text))
                                {
                                    _result = _edtContent.Text;
                                }

                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, _result);
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                CmmDroidFunction.HideSoftKeyBoard(_edtContent, _mainAct);

                                string _result = "";
                                if (!String.IsNullOrEmpty(_edtContent.Text))
                                {
                                    _result = _edtContent.Text;
                                }

                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
                };
                #endregion

                _edtContent.RequestFocus();
                CmmDroidFunction.ShowSoftKeyBoard(_rootView, _mainAct);
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