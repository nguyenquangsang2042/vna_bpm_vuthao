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
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentRow3 : ComponentBase
    {
        public ViewElement _element { get; set; }
        public LinearLayout _parentView { get; set; }
        public Activity _mainAct;
        public ViewRow _controlData;
        public ComponentRow1 row1, row2, row3;
        public int _widthScreenTablet = -1;
        public int _flagView = -1;
        public bool _visibleItemLine; // true = hiện Line từng item, false = hiện line của nguyên row
        public ComponentRow3(Activity _mainAct, LinearLayout _parentView, ViewRow _controlData, int _widthScreenTablet, bool _visibleItemLine,int _flagView)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._controlData = _controlData;
            this._widthScreenTablet = _widthScreenTablet;
            this._visibleItemLine = _visibleItemLine;
            this._flagView = _flagView;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            row1 = new ComponentRow1(_mainAct, _parentView, _controlData.Elements[0], (int)_widthScreenTablet / 3, _visibleItemLine, _flagView);
            row2 = new ComponentRow1(_mainAct, _parentView, _controlData.Elements[1], (int)_widthScreenTablet / 3, _visibleItemLine, _flagView);
            row3 = new ComponentRow1(_mainAct, _parentView, _controlData.Elements[2], (int)_widthScreenTablet / 3, _visibleItemLine, _flagView);
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            base.InitializeFrameView(frame);

            int _widthRow;
            if (_widthScreenTablet == -1) // ForPhone
            {
                DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                _widthRow = dm.WidthPixels / 3;
            }
            else // For Tablet
            {
                _widthRow = _widthScreenTablet / 3;
            }

            LinearLayout _lncontent = new LinearLayout(_mainAct);
            _lncontent.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _lncontent.Orientation = Android.Widget.Orientation.Horizontal;

            LinearLayout _lnRow1 = new LinearLayout(_mainAct);
            LinearLayout _lnRow2 = new LinearLayout(_mainAct);
            LinearLayout _lnRow3 = new LinearLayout(_mainAct);
            _lnRow1.LayoutParameters = new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WrapContent);
            _lnRow2.LayoutParameters = new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WrapContent);
            _lnRow3.LayoutParameters = new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WrapContent);
            row1.InitializeFrameView(_lnRow1); // Init View vào LinearLayout
            row2.InitializeFrameView(_lnRow2); // Init View vào LinearLayout
            row3.InitializeFrameView(_lnRow3); // Init View vào LinearLayout

            LinearLayout _lnRowLine = new LinearLayout(_mainAct);
            if (_visibleItemLine == false) // Ẩn Line của Item -> Load Line của nguyên Row
            {
                int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
                _lnRowLine.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clControlGrayLight)));
                _lnRowLine.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 1, _mainAct.Resources.DisplayMetrics));
                _lnRowLine.SetPadding(_padding * 2, _padding, _padding * 2, _padding);

            }

            _lncontent.AddView(_lnRow1);
            _lncontent.AddView(_lnRow2);
            _lncontent.AddView(_lnRow3);
            frame.AddView(_lncontent);
            //frame.AddView(_lnRowLine);
        }

        public override void SetTitle()
        {
            row1.SetTitle();
            row2.SetTitle();
            row3.SetTitle();
        }
        public override void SetValue()
        {
            base.SetValue();

            row1.SetValue();
            row2.SetValue();
            row3.SetValue();
        }
        public override void SetProprety()
        {
            row1.SetProprety();
            row2.SetProprety();
            row3.SetProprety();
        }
        public override void SetEnable()
        {
            base.SetEnable();

            row1.SetEnable();
            row2.SetEnable();
            row3.SetEnable();
        }
    }
}