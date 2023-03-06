using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Component;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentButtonBot : ComponentBase
    {
        public ViewElement _element { get; set; }
        public LinearLayout _parentView { get; set; }
        public Activity _mainAct;
        public ViewRow _controlData;
        public Resources _resource;
        public ControlButtonBot _btnAction1, _btnAction2, _btnAction3;
        public List<ButtonAction> _lstActionMore = new List<ButtonAction>();
        public ComponentButtonBot(Activity _mainAct, LinearLayout _parentView, ViewRow _controlData, Resources _resource)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._controlData = _controlData;
            this._resource = _resource;

            _controlData.Elements = CmmFunction.SortListElementAction(_controlData.Elements);
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            if (_controlData.Elements.Any())
                _btnAction1 = new ControlButtonBot(_mainAct, _parentView, _controlData.Elements[0], _resource);
            else
                _btnAction1 = new ControlButtonBot(_mainAct);

            if (_controlData.Elements.Count < 2)
                _btnAction2 = new ControlButtonBot(_mainAct);
            else
                _btnAction2 = new ControlButtonBot(_mainAct, _parentView, _controlData.Elements[1], _resource);

            if (_controlData.Elements.Count < 3)
                _btnAction3 = new ControlButtonBot(_mainAct);
            else
            {
                ViewElement _elementMore = new ViewElement { ID = "more", Value = "more", Hidden = false };
                _btnAction3 = new ControlButtonBot(_mainAct, _parentView, _elementMore, _resource);
                _lstActionMore = new List<ButtonAction>();
                for (int i = 2; i < _controlData.Elements.Count; i++)
                {
                    ButtonAction action = new ButtonAction { ID = Convert.ToInt32(_controlData.Elements[i].ID), Title = _controlData.Elements[i].Title, Value = _controlData.Elements[i].Value };
                    _lstActionMore.Add(action);
                }
            }
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            base.InitializeFrameView(frame);
            int _height = (int)CmmDroidFunction.ConvertDpToPixel(35, frame.Context);

            LinearLayout _lncontent = new LinearLayout(_mainAct);
            int _paddingLeftRight = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 12, _mainAct.Resources.DisplayMetrics);
            _lncontent.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent, 1);
            _lncontent.Orientation = Android.Widget.Orientation.Horizontal;
            _lncontent.SetPadding(_paddingLeftRight, 0, _paddingLeftRight, 0);

            LinearLayout _lnAction1 = new LinearLayout(_mainAct);
            LinearLayout _lnAction2 = new LinearLayout(_mainAct);
            LinearLayout _lnActionMore = new LinearLayout(_mainAct);

            _lnAction1.LayoutParameters = new LinearLayout.LayoutParams(0, _height, 0.5f); // 0.5
            _lnAction2.LayoutParameters = new LinearLayout.LayoutParams(0, _height, 0.5f); // 0.5
            _lnActionMore.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, _height);

            _btnAction1.InitializeFrameView(_lnAction1); // Init View vào LinearLayout
            _btnAction2.InitializeFrameView(_lnAction2); // Init View vào LinearLayout
            _btnAction3.InitializeFrameView(_lnActionMore); // Init View vào LinearLayout

            _lncontent.AddView(_lnAction1);
            _lncontent.AddView(_lnAction2);
            _lncontent.AddView(_lnActionMore);
            frame.AddView(_lncontent);
        }
        public override void SetTitle()
        {
            _btnAction1.SetTitle();
            _btnAction2.SetTitle();
            _btnAction3.SetTitle();
        }
        public override void SetValue()
        {
            base.SetValue();

            _btnAction1.SetValue();
            _btnAction2.SetValue();
            _btnAction3.SetValue();
        }
        public override void SetProprety()
        {
            _btnAction1.SetProprety();
            _btnAction2.SetProprety();
            _btnAction3.SetProprety();
        }
        public override void SetEnable()
        {
            base.SetEnable();

            _btnAction1.SetEnable();
            _btnAction2.SetEnable();
            _btnAction3.SetProprety();
        }
    }
}