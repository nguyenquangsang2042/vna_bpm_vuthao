using System;
using System.Collections.Generic;
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
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlDateTime : SharedView_PopupControlBase
    {
        private ControllerBase CTRLBase = new ControllerBase();
        public SharedView_PopupControlDateTime(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DateTimePicker, null);
                DatePicker _datePicker = _viewPopupControl.FindViewById<DatePicker>(Resource.Id.dp_PopupControl_DateTimePicker);
                TimePicker _timePicker = _viewPopupControl.FindViewById<TimePicker>(Resource.Id.tp_PopupControl_DateTimePicker);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Delete);
                ImageView _imgToday = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Today);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DateTimePicker_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DateTimePicker_Title);
                LinearLayout _lnApply = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_DateTimePicker_Clear);
                TextView _tvApply = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DateTimePicker_Apply);

                _timePicker.SetIs24HourView(Java.Lang.Boolean.True);

                _tvApply.Text = CmmFunction.GetTitle("TEXT_APPLY", "Áp dụng");

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            DateTime _initDate = new DateTime();
                            try
                            {
                                _initDate = DateTime.Parse(elementParent.Value, new CultureInfo("en", false));
                                _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                                _timePicker.Hour = _initDate.Hour;
                                _timePicker.Minute = _initDate.Minute;
                            }
                            catch
                            {
                                _initDate = DateTime.Now;
                            }

                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                            else
                                _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");


                            if (elementParent.Enable)
                            {
                                _imgDelete.Visibility = ViewStates.Visible;
                                _imgToday.Visibility = ViewStates.Visible;
                                _tvApply.Visibility = ViewStates.Visible;
                                _lnApply.Visibility = ViewStates.Visible;
                                _datePicker.Enabled = true;
                                _timePicker.Enabled = true;
                            }
                            else
                            {
                                _imgDelete.Visibility = ViewStates.Invisible;
                                _imgToday.Visibility = ViewStates.Invisible;
                                _tvApply.Visibility = ViewStates.Gone;
                                _lnApply.Visibility = ViewStates.Gone;
                                _datePicker.Enabled = false;
                                _timePicker.Enabled = false;
                            }

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            DateTime _initDate = new DateTime();
                            try
                            {
                                _initDate = DateTime.Parse(elementPopup.Value, new CultureInfo("en", false));
                                _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                                _timePicker.Hour = _initDate.Hour;
                                _timePicker.Minute = _initDate.Minute;
                            }
                            catch
                            {
                                _initDate = DateTime.Now;
                            }

                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                            else
                                _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");

                            if (elementPopup.Enable)
                            {
                                _imgDelete.Visibility = ViewStates.Visible;
                                _imgToday.Visibility = ViewStates.Visible;
                                _tvApply.Visibility = ViewStates.Visible;
                                _lnApply.Visibility = ViewStates.Visible;
                                _datePicker.Enabled = true;
                                _timePicker.Enabled = true;
                            }
                            else
                            {
                                _imgDelete.Visibility = ViewStates.Invisible;
                                _imgToday.Visibility = ViewStates.Invisible;
                                _tvApply.Visibility = ViewStates.Gone;
                                _lnApply.Visibility = ViewStates.Gone;
                                _datePicker.Enabled = false;
                                _timePicker.Enabled = false;
                            }
                            break;
                        }
                }
                #endregion

                #region Show View                
                Dialog _dialogPopupControl = new Dialog(_rootView.Context);
                Window window = _dialogPopupControl.Window;
                _dialogPopupControl.RequestWindowFeature(1);
                _dialogPopupControl.SetCanceledOnTouchOutside(false);
                _dialogPopupControl.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogPopupControl.SetContentView(_viewPopupControl);
                _dialogPopupControl.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels;
                s.Height = WindowManagerLayoutParams.WrapContent;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event
                _imgClose.Click += (sender, e) =>
                {
                    _dialogPopupControl.Dismiss();
                };

                _imgToday.Click += (sender, e) =>
                {
                    DateTime _initDate = DateTime.Now;
                    _datePicker.Init(_initDate.Year, _initDate.Month - 1, _initDate.Day, null);
                    _timePicker.Hour = _initDate.Hour;
                    _timePicker.Minute = _initDate.Minute;
                };

                _imgDelete.Click += (sender, e) =>
                {
                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, "");
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, "");
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
                };

                _lnApply.Click += delegate
                {
                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                            {
                                DateTime _resultDate = new DateTime(_datePicker.Year, _datePicker.Month + 1, _datePicker.DayOfMonth, _timePicker.Hour, _timePicker.Minute, 0);
                                string _result = _resultDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);

                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, _result);
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                _dialogPopupControl.Dismiss();

                                break;
                            }
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                DateTime _resultDate = new DateTime(_datePicker.Year, _datePicker.Month + 1, _datePicker.DayOfMonth, _timePicker.Hour, _timePicker.Minute, 0);
                                string _result = _resultDate.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);

                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
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