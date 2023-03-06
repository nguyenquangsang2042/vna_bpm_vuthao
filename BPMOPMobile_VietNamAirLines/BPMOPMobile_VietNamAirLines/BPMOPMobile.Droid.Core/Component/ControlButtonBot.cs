using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlButtonBot : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }
        private LinearLayout _lnAction { get; set; }
        private ImageView _imgAction { get; set; }
        private TextView _tvAction { get; set; }
        private Resources _resource;
        public ControlButtonBot(Activity _mainAct) : base(_mainAct)
        {
        }
        public ControlButtonBot(Activity _mainAct, LinearLayout _parentView, ViewElement _element, Resources _resource) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            this._resource = _resource;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            _lnAction = new LinearLayout(_mainAct);
            _imgAction = new ImageView(_mainAct);
            _tvAction = new TextView(_mainAct);

            _imgAction.SetScaleType(ImageView.ScaleType.Center);
            _imgAction.SetAdjustViewBounds(true);

            _tvAction.SetTextSize(ComplexUnitType.Sp, 14);
            _tvAction.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvAction.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tvAction.SetMaxLines(1);
            _tvAction.Ellipsize = TextUtils.TruncateAt.End;
            _tvAction.Gravity = GravityFlags.CenterVertical;

            _lnAction.Orientation = Android.Widget.Orientation.Horizontal;

            if (_lnContent != null)
            {
                _lnContent.Click += HandleTouchDown;
            }
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element != null)
            {
                if (_element.Hidden == true) // Check xem có ẩn view hay không
                    return;
            }
            base.InitializeFrameView(frame);
            if (_lnContent != null)
                _lnContent.Visibility = ViewStates.Gone;
            if (_element != null)
            {
                int _paddingLnAction = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);
                int _paddingImage = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
                LinearLayout.LayoutParams _paramAction = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                _paramAction.Gravity = GravityFlags.Center;
                _lnAction.LayoutParameters = _paramAction;
                _lnAction.SetPadding(0, _paddingLnAction, 0, _paddingLnAction);
                _lnAction.SetGravity(GravityFlags.Center);

                if (!String.IsNullOrEmpty(_element.Title)) // Button Action -> có Title và Image
                {
                    LinearLayout.LayoutParams _paramsImage = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context));
                    _paramsImage.Gravity = GravityFlags.Center;
                    _imgAction.LayoutParameters = _paramsImage;
                    _imgAction.SetPadding(_paddingImage, _paddingImage, _paddingImage, _paddingImage);

                    LinearLayout.LayoutParams _paramtvAction = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                    _paramtvAction.SetMargins(_paddingImage, 0, 0, 0);
                    _paramtvAction.Gravity = GravityFlags.Center;
                    _tvAction.LayoutParameters = _paramtvAction;
                    _tvAction.SetForegroundGravity(GravityFlags.Center);
                    _tvAction.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
                    _tvAction.SetPadding(0, 0, 0, 0); // cho cách Imageview action ra
                    _tvAction.TextAlignment = TextAlignment.Center;

                    _lnAction.AddView(_imgAction);
                    _lnAction.AddView(_tvAction);
                }
                else // Button More -> chỉ có Image
                {
                    LinearLayout.LayoutParams _paramsImage = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context));
                    _paramsImage.Gravity = GravityFlags.Center;
                    _imgAction.LayoutParameters = _paramsImage;
                    _imgAction.SetPadding(_paddingImage, _paddingImage, _paddingImage, _paddingImage);
                    _imgAction.SetColorFilter(new Color(ContextCompat.GetColor(frame.Context, Resource.Color.clActionBlack))); // Nút more luôn màu xám

                    _lnAction.AddView(_imgAction);
                }

                if (_lnAction != null)
                {
                    _lnAction.Click += HandleTouchDown;
                }

                frame.AddView(_lnAction);
            }
        }
        /// <summary>
        /// Event khi click vào Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (_parentView != null)
            {
                MinionActionCore.OnElementActionClick(null, new MinionActionCore.ElementActionClick(_element));
            }
        }
        public override void SetProprety()
        {
            if (_element != null && _element.ListProprety != null)
            {
                foreach (var item in _element.ListProprety)
                {
                    CmmDroidFunction.SetPropertyValueByNameCustom(_tvValue, item.Key, item.Value);
                }
            }
        }
        public override void SetEnable()
        {
            base.SetEnable();
        }
        public override void SetTitle()
        {
            base.SetTitle();
            if (_element != null)
            {
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                {
                    if (!String.IsNullOrEmpty(_element.Title))
                        _tvAction.Text = _element.Title;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_element.Value))
                        _tvAction.Text = _element.Value;
                }
            }
        }
        public override void SetValue()
        {
            //base.SetValue();

            if (_element != null && !string.IsNullOrEmpty(_element.Value))
            {
                string _imageName = "icon_bpm_Btn_action_" + _element.ID.ToString();
                int resId = _mainAct.Resources.GetIdentifier(_imageName.ToLowerInvariant(), "drawable", _mainAct.PackageName);
                _imgAction.SetImageResource(resId);
            }
        }
    }
}