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
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlText : ControlBase
    {
        public LinearLayout _parentView { get; set; }
        public ViewElement _element { get; set; }
        public override string Value
        {
            set { this._tvValue.Text = value.Trim(); }
            get { return this._tvValue.Text; }
        }

        public ControlText(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();



            if (_lnContent != null)
            {
                _lnContent.Click += HandleTouchDown;
            }
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            base.InitializeFrameView(frame);
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
                MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
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
                _tvValue.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlueEnable)));
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

            Value = _element.Value;
        }
    }
}