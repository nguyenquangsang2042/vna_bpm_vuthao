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
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Component
{
    class ControlTextInputMultiLine : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }

        private TextView _tvReadMore { get; set; }
        //private EditText _edt;

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
                    this._tvValue.Text = CmmDroidFunction.FormatHTMLToString(_element.Value);
                    this._tvReadMore.Post(() =>
                    {
                        // sau khi Set Layout xong sẽ gọi vào hàm này
                        int lines = _tvValue.LineCount;
                        if (lines >= 3)
                        {
                            this._tvReadMore.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            this._tvReadMore.Visibility = ViewStates.Gone;
                        }
                    });
                }
                else
                {
                    if (_element.Enable) // Cho chỉnh sửa mới hiện ra Hint
                    {
                        this._tvValue.Text = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung...");

                        this._tvValue.SetTypeface(this._tvValue.Typeface, TypefaceStyle.Italic);
                        this._tvValue.SetTextSize(ComplexUnitType.Sp, 12);

                        this._tvReadMore.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        this._tvValue.Visibility = ViewStates.Invisible;
                        this._tvReadMore.Visibility = ViewStates.Gone;
                    }
                }
            }
            get { return this._tvValue.Text; }
        }
        public override LinearLayout Frame { get => base.Frame; set => base.Frame = value; }

        public ControlTextInputMultiLine(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }

        /// <summary>
        /// Constructor của Input Grid Details
        /// </summary>
        public ControlTextInputMultiLine(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, int _flagView) : base(_mainAct)
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
            //_edt = new EditText(_mainAct);
            _tvReadMore = new TextView(_mainAct);
            if (_lnContent != null)
            {
                _tvReadMore.Click += HandleTouchDown;
                if(_element.Enable)
                {
                    _lnContent.Click += HandleTouchDown;
                }    
            }
            if (BPMOPMobile.Class.CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
            {
                _tvReadMore.Text = CmmFunction.GetTitle("TEXT_CONTROL_READMORE", "Xem thêm ...");
            }
            else
            {
                _tvReadMore.Text = CmmFunction.GetTitle("TEXT_CONTROL_READMORE", "Read more ...");
            }

            _tvReadMore.SetTextSize(ComplexUnitType.Sp, 12);
            _tvReadMore.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clOrangeFilter)));
            _tvReadMore.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
            _tvReadMore.Ellipsize = TextUtils.TruncateAt.End;
            _tvReadMore.Gravity = GravityFlags.Right;
            _tvReadMore.Visibility = ViewStates.Gone; // Khởi tạo thì ko cần hiện lên, lát check sau
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            base.InitializeFrameView(frame);

            _tvValue.SetMaxLines(3);
            _tvValue.Ellipsize = TextUtils.TruncateAt.End;

            _tvReadMore.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, _mainAct.Resources.DisplayMetrics);
            _tvReadMore.SetPadding(_padding, 0, _padding, 2 * _padding); // đã có padding bottom của Textview Value nên ko cần padding Top nữa

            _lnContent.RemoveView(_lnLine); // Remove line ra để add lại dưới textview more
            frame.AddView(_tvReadMore);
            frame.AddView(_lnLine);

            #region Code cũ

            //_tvValue.Visibility = ViewStates.Gone;
            //int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);
            //LinearLayout.LayoutParams _edtParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            //_edtParams.SetMargins(_padding, 0, _padding, 2 * _padding);
            //_edt.LayoutParameters = _edtParams;
            //_edt.SetPadding(2 * _padding, _padding, 2 * _padding, _padding);
            //_edt.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.textcontrolstrokegray);

            //_edt.InputType = InputTypes.TextFlagMultiLine;
            //_edt.SetLines(5); // multiline

            //_edt.TextChanged += Handle_TextChanged;
            //frame.AddView(_edt);
            //frame.AddView(_lnLine);
            //private void Handle_TextChanged(object sender, TextChangedEventArgs e)
            //{
            //    _element.Value = _edt.Text;
            //    if (_parentView != null)
            //    {
            //        MinionActionCore.OnElementClickDetailWorkflow(null, new MinionActionCore.ElementClick(_element));
            //    }
            //}
            #endregion
        }
        private void HandleTouchDown(object sender, EventArgs e)
        {
            if (_parentView != null /*&& _element.Enable == true*/)
            {
                if (_element.Enable == true)
                {
                    if (_JObjectChild != null) // Input grid Detail
                        MinionActionCore.OnElementGridChildActionClick(null, new MinionActionCore.ElementGridChildActionClick(_elementParent, _elementChild, _JObjectChild, _flagView));
                    else
                        MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
                }
                else
                {
                    // không cho Edit + có ellipsized mới được cho xem thêm

                    Layout layout = _tvValue.Layout;
                    int lines = _tvValue.LineCount;

                    int ellipsisCount = layout.GetEllipsisCount(lines - 1);
                    if (ellipsisCount > 0) // có ellipsized
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

        public override void SetProprety()
        {
            base.SetProprety();
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