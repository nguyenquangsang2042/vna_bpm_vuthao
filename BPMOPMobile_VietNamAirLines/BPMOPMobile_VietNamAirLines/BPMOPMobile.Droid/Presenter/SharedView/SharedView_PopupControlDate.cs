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
using Com.Telerik.Widget.Calendar;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupControlDate : SharedView_PopupControlBase
    {
        private ControllerBase CTRLBase = new ControllerBase();
        public SharedView_PopupControlDate(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_DatePicker, null);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Done);
                ImageView _imgClose = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_DatePicker_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DatePicker_Title);
                RadCalendarView _calendar = _viewPopupControl.FindViewById<RadCalendarView>(Resource.Id.Calendar_PopupControl_DatePicker);
                TextView _tvCurrentDate = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DatePicker_CurrentDate);
                LinearLayout _lnClear = _viewPopupControl.FindViewById<LinearLayout>(Resource.Id.ln_PopupControl_DatePicker_Clear);
                TextView _tvClear = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_DatePicker_Clear);

                CTRLBase.InitRadCalendarView(_calendar, _tvCurrentDate);
                DateTime _initDate;


                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;

                            try
                            {
                                _initDate = DateTime.Parse(elementParent.Value, new CultureInfo("en", false));
                            }
                            catch (Exception)
                            {
                                _initDate = DateTime.Now;
                            }

                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                            else
                                _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");

                            if (elementParent.Enable)
                            {
                                _imgDone.Visibility = ViewStates.Visible;
                                _lnClear.Visibility = ViewStates.Visible;
                                _calendar.Enabled = true;
                            }
                            else
                            {
                                _imgDone.Visibility = ViewStates.Invisible;
                                _lnClear.Visibility = ViewStates.Gone;
                                _calendar.Enabled = false;
                            }

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;

                            try
                            {
                                _initDate = DateTime.Parse(elementPopup.Value, new CultureInfo("en", false));
                            }
                            catch (Exception)
                            {
                                _initDate = DateTime.Now;
                            }

                            if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                _tvTitle.Text = _initDate.ToString("dd/MM/yyyy");
                            else
                                _tvTitle.Text = _initDate.ToString("MM/dd/yyyy");

                            if (elementPopup.Enable)
                            {
                                _imgDone.Visibility = ViewStates.Visible;
                                _lnClear.Visibility = ViewStates.Visible;
                                _calendar.Enabled = true;
                            }
                            else
                            {
                                _imgDone.Visibility = ViewStates.Invisible;
                                _lnClear.Visibility = ViewStates.Gone;
                                _calendar.Enabled = false;
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

                _imgDone.Click += (sender, e) =>
                {
                    try
                    {
                        switch (base.flagView)
                        {
                            case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                                {
                                    DateTime _resultDate = DateTime.Now;
                                    try
                                    {
                                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                            _resultDate = DateTime.ParseExact(_tvCurrentDate.Text, "dd/MM/yyyy", null);
                                        else
                                            _resultDate = DateTime.ParseExact(_tvCurrentDate.Text, "MM/dd/yyyy", null);
                                    }
                                    catch (Exception)
                                    {
                                        _resultDate = DateTime.Now;
                                    }

                                    string _result = _resultDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);

                                    FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                    _frag.UpdateValueForElement(elementParent, _result);
                                    _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                    _dialogPopupControl.Dismiss();
                                    break;
                                }
                            case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                                {
                                    DateTime _resultDate = DateTime.Now;
                                    try
                                    {
                                        if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                                            _resultDate = DateTime.ParseExact(_tvCurrentDate.Text, "dd/MM/yyyy", null);
                                        else
                                            _resultDate = DateTime.ParseExact(_tvCurrentDate.Text, "MM/dd/yyyy", null);
                                    }
                                    catch (Exception)
                                    {
                                        _resultDate = DateTime.Now;
                                    }

                                    string _result = _resultDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);

                                    FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                    _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                    _dialogPopupControl.Dismiss();
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        CmmDroidFunction.WriteTrackingError(this.GetType().Name, "imgDoneClick", ex);
#endif
                    }
                };

                _lnClear.Click += (sender, e) =>
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