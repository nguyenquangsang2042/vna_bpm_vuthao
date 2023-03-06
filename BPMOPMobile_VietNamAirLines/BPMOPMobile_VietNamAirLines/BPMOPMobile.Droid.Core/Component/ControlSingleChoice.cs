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
    public class ControlSingleChoice : ControlBase
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
                if (!String.IsNullOrEmpty(_element.Value))
                {
                    string _data = value.Trim();
                    string _result = "";

                    List<BeanLookupData> _lstObject = JsonConvert.DeserializeObject<List<BeanLookupData>>(_data);
                    List<string> _lstValue = new List<string>();

                    if (_lstObject != null && _lstObject.Count > 0)
                    {
                        foreach (BeanLookupData item in _lstObject)
                        {
                            _lstValue.Add(item.Title);
                        }
                        _result = String.Join(", ", _lstValue.ToArray());
                    }
                    this._tvValue.Text = _result;
                }
                else
                {
                    if (_element.Enable) // Cho chỉnh sửa mới hiện ra Hint
                    {
                        this._tvValue.Text = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung...");

                        this._tvValue.SetTypeface(this._tvValue.Typeface, TypefaceStyle.Italic);
                        this._tvValue.SetTextSize(ComplexUnitType.Sp, 12);
                    }
                    else
                    {
                        this._tvValue.Text = "";
                    }
                }
            }
            get { return this._tvValue.Text; }
        }

        public ControlSingleChoice(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }
        public ControlSingleChoice(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, int _flagView) : base(_mainAct)
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

            _tvValue.SetMaxLines(1);

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