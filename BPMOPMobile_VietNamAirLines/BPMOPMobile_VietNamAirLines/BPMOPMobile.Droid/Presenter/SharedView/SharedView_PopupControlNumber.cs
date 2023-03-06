using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Fragment;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlNumber : SharedView_PopupControlBase
    {
        public SharedView_PopupControlNumber(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                View _viewPopupControl = _inflater.Inflate(Resource.Layout.PopupControl_Number, null);
                ImageView _imgBack = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_Close);
                TextView _tvTitle = _viewPopupControl.FindViewById<TextView>(Resource.Id.tv_PopupControl_Number_Title);
                ImageView _imgDone = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_Done);
                ImageView _imgDelete = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_Delete);
                EditText _edtContent = _viewPopupControl.FindViewById<EditText>(Resource.Id.edt_PopupControl_Number);
                ImageView _imgClearText = _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_Number_ClearText);

                //_edtContent.InputType = InputTypes.ClassNumber; // Only Number
                _imgDelete.Visibility = ViewStates.Gone;
                _edtContent.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal; // Only Number
                _edtContent.SetFilters(new Android.Text.IInputFilter[] { new Android.Text.InputFilterLengthFilter(255) }); // Limit lại 255 kí tự

                switch (base.flagView)
                {
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                    default:
                        {
                            _tvTitle.Text = elementParent.Title;

                            CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                            CultureInfo _culEN = CultureInfo.GetCultureInfo("en-US");

                            if (!String.IsNullOrEmpty(elementParent.Value))
                            {
                                double _customValue = double.Parse(elementParent.Value, _culEN);
                                DecimalFormat df = new DecimalFormat("0");
                                df.MaximumFractionDigits = int.MaxValue;
                                _edtContent.Text = df.Format(_customValue);
                            }
                            else
                                _edtContent.Text = "";

                            break;
                        }
                    case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                        {
                            _tvTitle.Text = elementPopup.Title;

                            CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                            CultureInfo _culEN = CultureInfo.GetCultureInfo("en-US");

                            if (!String.IsNullOrEmpty(elementPopup.Value))
                            {
                                double _customValue = double.Parse(elementPopup.Value, _culEN);
                                DecimalFormat df = new DecimalFormat("0");
                                df.MaximumFractionDigits = int.MaxValue;
                                _edtContent.Text = df.Format(_customValue);
                            }
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
                _edtContent.TextChanged += (sender, e) =>
                {

                };
                _edtContent.AfterTextChanged += (sender, e) =>
                {
                    //double _customValue;
                    //CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");
                    //string _currentTextValue = _edtContent.Text.Trim()/*.Replace(".", "")*/;

                    //if (_currentTextValue.Contains("E+"))
                    //{
                    //    if (double.TryParse(_currentTextValue, out _customValue))
                    //    {
                    //        if (!_customValue.ToString("N0", _culVN).Equals(_edtContent.Text))
                    //        {
                    //            _edtContent.Text = _customValue.ToString("N0", _culVN);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (double.TryParse(_currentTextValue.Replace(".", ""), out _customValue))
                    //    {
                    //        if (!_customValue.ToString("N0", _culVN).Equals(_edtContent.Text))
                    //        {
                    //            _edtContent.Text = _customValue.ToString("N0", _culVN);
                    //        }
                    //    }
                    //}
                    //_edtContent.SetSelection(_edtContent.Text.Length); // focus vào character cuối cùng
                };
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
                                    CultureInfo _culEN = CultureInfo.GetCultureInfo("en-US");
                                    double _customValue = double.Parse(_edtContent.Text.Trim().Replace(',','.'), _culEN);

                                    if (!String.IsNullOrEmpty(elementParent.DataSource)) // _element.DataSource trả ra là số chữ số decimal
                                    {
                                        int _demical = int.Parse(elementParent.DataSource);
                                        _result = Math.Round(_customValue, _demical > 0 ? _demical : 0).ToString();
                                    }
                                    else
                                        _result = Math.Round(_customValue, 0).ToString();
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
                                    CultureInfo _culEN = CultureInfo.GetCultureInfo("en-US");
                                    double _customValue = double.Parse(_edtContent.Text.Trim().Replace(',', '.'), _culEN);

                                    if (!String.IsNullOrEmpty(elementPopup.DataSource)) // _element.DataSource trả ra là số chữ số decimal
                                    {
                                        int _demical = int.Parse(elementPopup.DataSource);
                                        _result = Math.Round(_customValue, _demical > 0 ? _demical : 0).ToString();
                                    }
                                    else
                                        _result = Math.Round(_customValue, 0).ToString();
                                }

                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
                };
                #endregion

                _edtContent.Text = _edtContent.Text;
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