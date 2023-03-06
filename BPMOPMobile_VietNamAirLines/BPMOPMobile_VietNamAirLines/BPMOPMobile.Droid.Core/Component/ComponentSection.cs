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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentSection
    {
        public ViewSection _section { get; set; }
        public LinearLayout _parentView { get; set; }
        public Activity _mainAct;
        public ControlBase control;
        public LinearLayout _lnContent, _lnLine, _lnExpand;
        public TextView _tvTitle, _tvNote;
        public ImageView _imgExpand;
        public Button _btnAction;
        public int _currentSection;
        public int _widthScreenTablet = -1;
        public ComponentSection(Activity _mainAct, LinearLayout _parentView, ViewSection _section, int _currentSection, int _widthScreenTablet)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._section = _section;
            this._currentSection = _currentSection;
            this._widthScreenTablet = _widthScreenTablet;
            InitializeComponent();
        }
        public void InitializeComponent()
        {
            _tvTitle = new TextView(_mainAct);
            _tvNote = new TextView(_mainAct);
            _lnLine = new LinearLayout(_mainAct);
            _lnContent = new LinearLayout(_mainAct);
            _lnExpand = new LinearLayout(_mainAct);
            _btnAction = new Button(_mainAct);
            _imgExpand = new ImageView(_mainAct);

            _tvTitle.SetTextSize(ComplexUnitType.Sp, 14);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvTitle.SetBackgroundResource(Resource.Drawable.textcornergray);
            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;

            _tvNote.SetTextSize(ComplexUnitType.Sp, 12);
            _tvNote.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clControlHint)));
            _tvNote.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            _tvNote.Ellipsize = TextUtils.TruncateAt.End;

            _imgExpand.SetBackgroundResource(Resource.Drawable.icon_back2);
            _lnLine.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clGrayNavigator2)));

            _lnContent.Orientation = Android.Widget.Orientation.Vertical;
            _lnExpand.Orientation = Android.Widget.Orientation.Horizontal;
            _lnLine.Orientation = Android.Widget.Orientation.Vertical;
        }
        /// <summary>
        /// Vẽ các itewm len LinearLayout
        /// </summary>
        /// <param name="frame"></param>
        public void InitializeFrameView(LinearLayout frame)
        {
            LinearLayout _lnTitle = new LinearLayout(_mainAct);

            int _widthRow;
            if (_widthScreenTablet == -1) // ForPhone
            {
                DisplayMetrics dm = _mainAct.Resources.DisplayMetrics;
                _widthRow = dm.WidthPixels;
            }
            else // For Tablet
            {
                _widthRow = _widthScreenTablet;
            }
            _lnContent.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _lnExpand.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _lnLine.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 1, _mainAct.Resources.DisplayMetrics));
            _imgExpand.LayoutParameters = new LinearLayout.LayoutParams((int)CmmDroidFunction.ConvertDpToPixel(10, frame.Context), (int)CmmDroidFunction.ConvertDpToPixel(10, frame.Context));
            _tvTitle.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            _tvNote.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            _lnTitle.LayoutParameters = new LinearLayout.LayoutParams(_widthRow - (int)CmmDroidFunction.ConvertDpToPixel(30, frame.Context), LinearLayout.LayoutParams.WrapContent);

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, _mainAct.Resources.DisplayMetrics);
            _tvTitle.SetPadding(2 * _padding, _padding, 2 * _padding, _padding);
            _tvNote.SetPadding(_padding, _padding, _padding, _padding);
            _lnLine.SetPadding(_padding, _padding, _padding, _padding);
            _lnExpand.SetPadding(_padding, _padding, _padding, _padding);

            _lnTitle.AddView(_tvTitle);
            _lnExpand.AddView(_lnTitle);
            _lnExpand.AddView(_imgExpand);

            _lnContent.AddView(_lnExpand);
            _lnContent.AddView(_tvNote);
            _lnContent.AddView(_lnLine);

            ////_lnContent.AddView(_btnAction);

            frame.AddView(_lnContent);
        }
        /// <summary>
        /// Update lại View đóng hay mở
        /// </summary>
        public void UpdateContentSection()
        {
            _tvTitle.Text = _section.Title;
            _tvNote.Text = "Nhấn vào để xem thêm nội dung";
            if (_section.ShowType)
            {
                _imgExpand.Rotation = 90; // /\
                _tvNote.Visibility = ViewStates.Gone;
            }
            else
            {
                _imgExpand.Rotation = -90; // \/
                _tvNote.Visibility = ViewStates.Visible;
            }
            //if (_currentSection == 0)
            //    _lnLine.Visibility = ViewStates.Gone;

            if (_section.IsShowHint)
                _tvNote.Visibility = ViewStates.Gone;
        }
        public void ExpandSectionListener()
        {
            if (_tvNote.Visibility == ViewStates.Visible)
            {
                _imgExpand.Rotation = 90; // /\
                _tvNote.Visibility = ViewStates.Gone;
            }
            else
            {
                _imgExpand.Rotation = -90; // \/
                _tvNote.Visibility = ViewStates.Visible;
            }
        }
    }
}