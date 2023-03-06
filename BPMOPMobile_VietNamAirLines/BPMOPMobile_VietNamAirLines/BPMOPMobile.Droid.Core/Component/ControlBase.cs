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
using Android.Support.V4.Content.Res;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ControlBase : ComponentBase
    {
        public LinearLayout _lnContent, _lnLine;
        public TextView _tvTitle, _tvValue;
        public Button _btnAction;
        public Activity _mainAct;

        public ControlBase(Activity _mainAct)
        {
            this._mainAct = _mainAct;
        }

        public override void InitializeCategory(int flagCategory)
        {
            base.InitializeCategory(flagCategory);
        }

        /// <summary>
        /// Cấu hình component mặc định gồm tiều đề và giá trị
        /// </summary>
        public override void InitializeComponent()
        {
            _tvTitle = new TextView(_mainAct);
            _tvValue = new TextView(_mainAct);
            _lnLine = new LinearLayout(_mainAct);
            _lnContent = new LinearLayout(_mainAct);
            _btnAction = new Button(_mainAct);

            _tvTitle.SetTextSize(ComplexUnitType.Sp, 12);
            _tvTitle.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBottomDisable)));
            _tvTitle.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tvTitle.Ellipsize = TextUtils.TruncateAt.End;

            _tvValue.SetTextSize(ComplexUnitType.Sp, 15);
            _tvValue.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _tvValue.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Normal);
            _tvValue.Ellipsize = TextUtils.TruncateAt.End;

            _lnLine.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clControlGrayLight)));

            _lnContent.Orientation = Orientation.Vertical;
            _lnLine.Orientation = Orientation.Vertical;
           
        }
        /// <summary>
        /// Vẽ các itewm len LinearLayout
        /// </summary>
        /// <param name="frame"></param>
        public override void InitializeFrameView(LinearLayout frame)
        {
            base.InitializeFrameView(frame);

            frame.RemoveAllViews();
            frame.SetPadding(0, 0, 0, 0);

            if (_lnContent != null && _lnLine != null && _tvTitle != null && _tvValue != null)
            {
                _lnContent.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                _lnLine.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)TypedValue.ApplyDimension(ComplexUnitType.Px, 0, _mainAct.Resources.DisplayMetrics)); // Bỏ Line vì không cần nữa
                _tvTitle.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                _tvValue.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);

                int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);

                _lnContent.SetPadding(_padding, 2 * _padding, _padding, 2 * _padding);
                _tvTitle.SetPadding(_padding, 2 * _padding, _padding, _padding);
                _tvValue.SetPadding(_padding, _padding / 2, _padding, 2 * _padding);
                _lnLine.SetPadding(_padding, 2 * _padding, _padding, 2 * _padding);

                _lnContent.AddView(_tvTitle);
                _lnContent.AddView(_tvValue);
                _lnContent.AddView(_lnLine);
                ////_lnContent.AddView(_btnAction); // Android khong cần

                frame.AddView(_lnContent);
            }
        }
    }
}