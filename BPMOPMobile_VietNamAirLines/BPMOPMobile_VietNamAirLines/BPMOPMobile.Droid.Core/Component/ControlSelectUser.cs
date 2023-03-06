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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlSelectUser : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }

        // Input grid Details 
        public ViewElement _elementParent { get; set; } // Element của Control Grid
        public ViewElement _elementChild { get; set; } // Element child của Item dc click vào
        public int _flagView { get; set; }
        public JObject _JObjectChild { get; set; }
        public override string Value
        {
            set
            {
                var _data = value.Trim();
                string _result = "";

                List<BeanUserAndGroup> _beanUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(_data);
                if (_beanUserAndGroup != null && _beanUserAndGroup.Count > 0)
                {
                    _result = _beanUserAndGroup[0].Name;
                    this._tvValue.Text = _result;
                }
                else // Hiện Hint lên + chữ nghiêng
                {
                    if (_element.Enable)
                    {
                        if (_element.DataType.Equals("selectuser"))
                            this._tvValue.Text = CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người...");
                        else
                            this._tvValue.Text = CmmFunction.GetTitle("TEXT_CONTROL_USERGROUP", "Chọn người hoặc nhóm ...");

                        this._tvValue.SetTypeface(this._tvValue.Typeface, TypefaceStyle.Italic);
                        _tvValue.SetTextSize(ComplexUnitType.Sp, 12);
                    }
                }
            }
            get { return this._tvValue.Text; }
        }

        public ControlSelectUser(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }
        public ControlSelectUser(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, int _flagView) : base(_mainAct)
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