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
using BPMOPMobile.Droid.Presenter.Fragment;
using Jp.Wasabeef;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    class SharedView_PopupControlTextInputFormat : SharedView_PopupControlBase
    {
        public SharedView_PopupControlTextInputFormat(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
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
                bool isChanged = true;

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

                _imgDelete.Visibility = ViewStates.Gone;
                _lnContentClick.Visibility = ViewStates.Gone;

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

                #region Editor Library
                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_undo).Click += delegate { mEditor.Undo(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_redo).Click += delegate { mEditor.Redo(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bold).Click += delegate { mEditor.SetBold(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_italic).Click += delegate { mEditor.SetItalic(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_subscript).Click += delegate { mEditor.SetSubscript(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_superscript).Click += delegate { mEditor.SetSuperscript(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_strikethrough).Click += delegate { mEditor.SetStrikeThrough(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_underline).Click += delegate { mEditor.SetUnderline(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading1).Click += delegate { mEditor.SetHeading(1); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading2).Click += delegate { mEditor.SetHeading(2); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading3).Click += delegate { mEditor.SetHeading(3); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading4).Click += delegate { mEditor.SetHeading(4); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading5).Click += delegate { mEditor.SetHeading(5); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_heading6).Click += delegate { mEditor.SetHeading(6); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_txt_color).Click += delegate { mEditor.SetTextColor(isChanged ? Color.Black : Color.Red); isChanged = !isChanged; };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_bg_color).Click += delegate { mEditor.SetTextBackgroundColor(isChanged ? Color.Transparent : Color.Yellow); isChanged = !isChanged; };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_indent).Click += delegate { mEditor.SetIndent(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_outdent).Click += delegate { mEditor.SetOutdent(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_left).Click += delegate { mEditor.SetAlignLeft(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_center).Click += delegate { mEditor.SetAlignCenter(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_align_right).Click += delegate { mEditor.SetAlignRight(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_blockquote).Click += delegate { mEditor.SetBlockquote(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_numbers).Click += delegate { mEditor.SetNumbers(); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_image).Click += delegate { mEditor.InsertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_link).Click += delegate { mEditor.InsertLink("https://github.com/wasabeef", "wasabeef"); };

                _viewPopupControl.FindViewById<ImageView>(Resource.Id.img_PopupControl_InputTextFormat_insert_checkbox).Click += delegate { mEditor.InsertTodo(); };
                #endregion

                _imgBack.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    _dialogPopupControl.Dismiss();
                };

                _imgDelete.Click += (sender, e) =>
                {
                    mEditor.SetHtml("");
                    _imgDone.PerformClick();
                };

                _imgDone.Click += delegate
                {
                    CmmDroidFunction.HideSoftKeyBoard(_mainAct);
                    string _result = "";
                    if (!String.IsNullOrEmpty(mEditor.GetHtml().ToString()))
                    {
                        _result = mEditor.GetHtml().ToString();
                    }

                    switch (base.flagView)
                    {
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow: // Control of Master
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForElement(elementParent, _result);
                                _frag._adapterDetailExpandControl.NotifyDataSetChanged();
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                        case (int)SharedView_PopupControlBase.FlagViewControlDynamic.DetailWorkflow_InputGridDetail:
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.UpdateValueForPopupGridDetail(elementParent, elementPopup, JObjectChild, _result);
                                _dialogPopupControl.Dismiss();
                                break;
                            }
                    }
                };
                #endregion

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