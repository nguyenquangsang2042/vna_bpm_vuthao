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
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    public class SharedView_PopupActionAccept : SharedView_PopupActionBase
    {
        public SharedView_PopupActionAccept(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
        }

        public override void InitializeValue_DetailWorkflow(ButtonAction buttonAction)
        {
            base.InitializeValue_DetailWorkflow(buttonAction);
        }

        public override void InitializeView()
        {
            base.InitializeView();
            try
            {
                #region Get View - Init Data
                View _viewPopupAction = _inflater.Inflate(Resource.Layout.PopupAction_Accept, null);
                TextView _tvTitle = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Title);
                ImageView _imgAction = _viewPopupAction.FindViewById<ImageView>(Resource.Id.img_PopupAction_Accept);
                EditText _edtComment = _viewPopupAction.FindViewById<EditText>(Resource.Id.edt_PopupAction_Accept_YKien);
                TextView _tvCancel = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Huy);
                TextView _tvAccept = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_HoanTat);
                TextView _tvNote = _viewPopupAction.FindViewById<TextView>(Resource.Id.tv_PopupAction_Accept_Note);

                _tvTitle.Text = buttonAction.Title;
                _edtComment.Hint = CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến");
                _tvCancel.Text = CmmFunction.GetTitle("TEXT_EXIT", "Thoát");
                _tvAccept.Text = buttonAction.Title;

                string _imageName = "icon_bpm_Btn_action_" + buttonAction.ID.ToString();
                int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                _imgAction.SetImageResource(resId);

                if (buttonAction.Notes != null && buttonAction.Notes.Count > 0) // Check xem phiếu có đang tham vấn ko
                {
                    _tvNote.Text = buttonAction.Notes[0].Value;
                }

                #endregion

                #region Show View
                Dialog _dialogAction = new Dialog(_rootView.Context);
                Window window = _dialogAction.Window;
                _dialogAction.RequestWindowFeature(1);
                _dialogAction.SetCanceledOnTouchOutside(false);
                _dialogAction.SetCancelable(true);
                window.SetSoftInputMode(SoftInput.AdjustPan); // Để Hide soft keyboard ko bị Resize
                window.SetGravity(GravityFlags.Center);
                var dm = _mainAct.Resources.DisplayMetrics;

                _dialogAction.SetContentView(_viewPopupAction);
                _dialogAction.Show();
                WindowManagerLayoutParams s = window.Attributes;
                s.Width = dm.WidthPixels;
                window.Attributes = s;
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                #endregion

                #region Event
                _tvCancel.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                    _dialogAction.Dismiss();
                };

                _edtComment.TextChanged += (sender, e) =>
                {
                    if (String.IsNullOrEmpty(_edtComment.Text))
                        _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
                    else
                        _edtComment.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
                };

                _tvAccept.Click += (sender, e) =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtComment, _mainAct);
                    _dialogAction.Dismiss();

                    switch (flagView)
                    {
                        case (int)FlagViewControlAction.DetailWorkflow: // Action Trang Detail Workflow
                            {
                                FragmentDetailWorkflow _frag = (FragmentDetailWorkflow)_fragment;
                                _frag.Action_SendAPI(buttonAction, !String.IsNullOrEmpty(_edtComment.Text) ? _edtComment.Text : "", null); // Check xem có comment không
                                break;
                            }
                    }
                };
                #endregion

                _edtComment.Text = "";
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