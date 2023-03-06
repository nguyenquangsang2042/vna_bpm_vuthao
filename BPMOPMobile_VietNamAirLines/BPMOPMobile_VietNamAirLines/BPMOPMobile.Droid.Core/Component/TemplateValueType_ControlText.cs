using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Component
{
    class TemplateValueType_ControlText : ControlText
    {
        public ViewElement _elementParent { get; set; } // Element của Control Grid
        public ViewElement _elementChild { get; set; } // Element child của Item dc click vào
        public int _flagView { get; set; }
        public JObject _JObjectChild { get; set; }

        public TemplateValueType_ControlText(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, int _flagView) : base(_mainAct, _parentView, _elementChild)
        {
            this._elementParent = _elementParent;
            this._elementChild = _elementChild;
            this._JObjectChild = _JObjectChild;
            this._flagView = _flagView;
        }

        public override LinearLayout Frame { get => base.Frame; set => base.Frame = value; }
        public override string Value { get => base.Value; set => base.Value = value; }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            base.InitializeFrameView(frame);
        }

        public override void SetEnable()
        {
            base.SetEnable();
        }

        public override void SetProprety()
        {
            base.SetProprety();
        }

        public override void SetTitle()
        {
            base.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();
        }

        protected override void HandleTouchDown(object sender, EventArgs e)
        {
            //base.HandleTouchDown(sender, e);
            if (base._parentView != null)
            {
                MinionActionCore.OnElementGridChildActionClick(null, new MinionActionCore.ElementGridChildActionClick(_elementParent, _elementChild, _JObjectChild, _flagView));
            }
        }
    }
}