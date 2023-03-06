using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlDate : ControlBase
    {
        public LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }
        public override string Value
        {
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var _customValue = "";
                    DateTime dateValue = new DateTime();

                    try
                    {
                        //dateValue = DateTime.Parse(value, new CultureInfo("en", false));
                        dateValue = DateTime.Parse(value);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unable to convert '{0}'.", value);
                    }

                    if (_element.DataType == "date")
                    {
                        if (BPMOPMobile.Class.CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _customValue = dateValue.ToString(@"dd/MM/yy", new CultureInfo("en-US"));
                        else
                            _customValue = dateValue.ToString(@"MM/dd/yy", new CultureInfo("en-US"));
                    }
                    else if (_element.DataType == "datetime")
                    {
                        ////_customValue = dateValue.ToString(@"dd/MM/yyyy hh:mm tt", new CultureInfo("en-US")).ToLower();
                        if (BPMOPMobile.Class.CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                            _customValue = dateValue.ToString(@"dd/MM/yy HH:mm").ToLower();
                        else
                            _customValue = dateValue.ToString(@"MM/dd/yy HH:mm").ToLower();
                    }
                    else if (_element.DataType == "time")
                    {
                        ////_customValue = dateValue.ToString("HH:mm", new CultureInfo("en-US"));
                        _customValue = dateValue.ToString("HH:mm");
                    }
                    this._tvValue.Text = _customValue;
                }
                else
                {
                    if (_element.Enable)
                    {
                        if (_element.DataType == "date")
                        {
                            this._tvValue.Text = CmmFunction.GetTitle("TEXT_CHOOSE_DATE", "Chọn ngày...");
                        }
                        else if (_element.DataType == "datetime")
                        {
                            this._tvValue.Text = CmmFunction.GetTitle("TEXT_CHOOSE_DATETIME", "Chọn ngày giờ...");
                        }
                        this._tvValue.SetTypeface(this._tvValue.Typeface, TypefaceStyle.Italic);
                        _tvValue.SetTextSize(ComplexUnitType.Sp, 12);
                    }
                    else
                    {
                        this._tvValue.Text = "";
                    }
                }
            }
            get { return this._tvValue.Text; }
        }

        public ControlDate(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
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
            if (_parentView != null /* && _element.Enable == true*/)
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