using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlYesNo : ControlBase
    {
        public LinearLayout _parentView { get; set; }
        public ViewElement _element { get; set; }

        private Switch _switch;

        // Input grid Details 
        public ViewElement _elementParent { get; set; } // Element của Control Grid
        public ViewElement _elementChild { get; set; } // Element child của Item dc click vào
        public int _flagView { get; set; }
        public JObject _JObjectChild { get; set; }

        public override string Value
        {
            set
            {
                if (_element.Enable) // cho chỉnh sửa -> hiện Switch
                {
                    if (value.ToLowerInvariant() == "true")
                        this._switch.Checked = true;
                    else
                        this._switch.Checked = false;
                }
                else // hiện lên _tvValue
                {
                    if (value.ToLowerInvariant() == "true")
                        this._tvValue.Text = CmmFunction.GetTitle("TEXT_LABEL_YES", "Có");
                    else
                        this._tvValue.Text = CmmFunction.GetTitle("TEXT_LABEL_NO", "Không");
                }
            }
            get { return this._switch.Text.ToString(); }
        }

        public ControlYesNo(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }
        /// <summary>
        /// Constructor của Input Grid Details
        /// </summary>
        public ControlYesNo(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, int _flagView) : base(_mainAct)
        {
            // Để render theo base
            this._parentView = _parentView;
            this._element = _elementChild;

            // Input grid Details
            this._elementParent = _elementParent;
            this._elementChild = _elementChild;
            this._JObjectChild = _JObjectChild;
            this._flagView = _flagView;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            _switch = new Switch(_mainAct);

            //if (_lnContent != null)
            //{
            //    _lnContent.Click += HandleTouchDown;
            //}
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            base.InitializeFrameView(frame);

            if (_element.Enable == true) // cho chỉnh sửa -> hiện Switch
            {
                this._tvValue.Visibility = ViewStates.Gone;

                int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
                LinearLayout.LayoutParams _switchParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                _switch.SetPadding(0, _padding, _padding, _padding);
                _switch.LayoutParameters = _switchParams;

                _switch.CheckedChange += HandleTouchDown;

                _lnContent.AddView(_switch);
            }
            else // hiện lên _tvValue
            {
                this._tvValue.Visibility = ViewStates.Visible;
            }
        }

        /// <summary>
        /// Event khi click vào Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleTouchDown(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked == true)
                _element.Value = "True";
            else
                _element.Value = "False";

            if (_parentView != null)
            {
                if (_JObjectChild != null) // Input grid Detail
                {
                    MinionActionCore.OnElementGridChildActionClick(null, new MinionActionCore.ElementGridChildActionClick(_elementParent, _elementChild, _JObjectChild, _flagView));
                }
                else
                {
                    MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
                }
            }
        }
        public override void SetProprety()
        {
            if (_element.ListProprety != null)
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

            if (_element.Enable)
            {
                _switch.Clickable = true;
                _parentView.Clickable = true;
            }
            else
            {
                _switch.Clickable = false;
                _parentView.Clickable = false;
            }
        }
        public override void SetTitle()
        {
            base.SetTitle();

            _tvTitle.Text = _element.Title;

            if (_element.IsRequire && _element.Enable)
            {
                _tvTitle.Text += " (*)";
                CmmDroidFunction.SetTextViewHighlightControl(_mainAct, _tvTitle);
            }
        }
        public override void SetValue()
        {
            base.SetValue();

            if (_element.Enable == true)
                _switch.CheckedChange -= HandleTouchDown; // để khỏi trigger event

            Value = _element.Value;

            if (_element.Enable == true)
                _switch.CheckedChange += HandleTouchDown;

        }
    }
}