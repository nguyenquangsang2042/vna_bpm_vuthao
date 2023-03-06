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
using Android.Support.V4.Text;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using Java.Interop;
using Jp.Wasabeef;

namespace BPMOPMobile.Droid.Core.Component
{
    class ControlTextInputFormat : ControlBase
    {
        private LinearLayout _parentView { get; set; }
        private ViewElement _element { get; set; }
        private RelativeLayout _relaEditor { get; set; }
        private LinearLayout _lnEditor { get; set; } // lớp đè lên rich Editor
        private TextView _tvReadMore { get; set; }

        private EditText _edt;
        private RichEditor _richEditor;
        public override string Value
        {
            set
            {
                var data = value.Trim();
                //this._richEditor.SetHtml(data.ToString());
                //this._edt.TextFormatted = Html.FromHtml(data.ToString());
                this._edt.Text = HtmlCompat.FromHtml(data.ToString(), HtmlCompat.FromHtmlModeLegacy).ToString();

                if (_element.Enable) // Cho chỉnh sửa mới hiện ra Hint
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        this._tvValue.Visibility = ViewStates.Visible;
                        this._tvReadMore.Visibility = ViewStates.Gone;
                        this._edt.Visibility = ViewStates.Gone;

                        this._tvValue.SetTypeface(this._tvValue.Typeface, TypefaceStyle.Italic);
                        this._tvValue.SetTextSize(ComplexUnitType.Sp, 12);
                        this._tvValue.Visibility = ViewStates.Visible;

                        this._tvValue.Text = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung...");
                    }
                    else
                    {
                        this._tvValue.Visibility = ViewStates.Gone;
                        this._edt.Visibility = ViewStates.Visible;

                        this._tvReadMore.Post(() =>
                        {
                            // sau khi Set Layout xong sẽ gọi vào hàm này
                            int lines = _edt.LineCount;
                            if (lines > 5)
                                this._tvReadMore.Visibility = ViewStates.Visible;
                            else
                                this._tvReadMore.Visibility = ViewStates.Gone;
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        this._tvValue.Visibility = ViewStates.Gone;
                        this._tvReadMore.Post(() =>
                        {
                            // sau khi Set Layout xong sẽ gọi vào hàm này
                            int lines = _edt.LineCount;
                            if (lines > 5)
                                this._tvReadMore.Visibility = ViewStates.Visible;
                            else
                                this._tvReadMore.Visibility = ViewStates.Gone;
                        });
                    }
                }
            }
            get { return this._richEditor.GetHtml(); }
        }

        public ControlTextInputFormat(Activity _mainAct, LinearLayout _parentView, ViewElement _element) : base(_mainAct)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            _edt = new EditText(_mainAct);
            _relaEditor = new RelativeLayout(_mainAct);
            _lnEditor = new LinearLayout(_mainAct);
            _richEditor = new RichEditor(_mainAct);
            _tvReadMore = new TextView(_mainAct);

            _edt.SetMaxLines(5);
            _edt.Ellipsize = TextUtils.TruncateAt.End;
            _edt.Gravity = GravityFlags.Top;
            _edt.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clBlack)));
            _edt.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.transparent)));
            //_edt.Enabled = false;
            _edt.Focusable = false;

            _tvReadMore.SetTextSize(ComplexUnitType.Sp, 12);
            _tvReadMore.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clOrangeFilter)));
            _tvReadMore.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Italic);
            _tvReadMore.Ellipsize = TextUtils.TruncateAt.End;
            _tvReadMore.Gravity = GravityFlags.Right;
            _tvReadMore.Visibility = ViewStates.Gone; // Khởi tạo thì ko cần hiện lên, lát check sau
            _tvReadMore.Text = CmmFunction.GetTitle("TEXT_CONTROL_READMORE", "Read more ...");

            _lnEditor.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.transparent)));
        }

        public override void InitializeFrameView(LinearLayout frame)
        {
            if (_element.Hidden == true) // Check xem có ẩn view hay không
                return;

            base.InitializeFrameView(frame);

            _lnContent.RemoveView(_lnLine); // Remove line ra để add lại dưới edit text
            _tvValue.Visibility = ViewStates.Gone;

            int _padding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, _mainAct.Resources.DisplayMetrics);

            RelativeLayout.LayoutParams _paramsRelaEditor = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams _paramsRichEditor = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.ConvertDpToPixel(100, frame.Context));
            LinearLayout.LayoutParams _paramsEditText = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            //inearLayout.LayoutParams _paramsLnEditorButton = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)CmmDroidFunction.convertDpToPixel(30, frame.Context));
            //LinearLayout.LayoutParams _paramsImgAction = new LinearLayout.LayoutParams((int)CmmDroidFunction.convertDpToPixel(30, frame.Context), (int)CmmDroidFunction.convertDpToPixel(30, frame.Context));

            _edt.LayoutParameters = _paramsEditText;
            _tvReadMore.LayoutParameters = _paramsEditText;

            _relaEditor.LayoutParameters = _paramsRelaEditor;
            _lnEditor.LayoutParameters = _paramsRichEditor;


            _paramsRichEditor.SetMargins(_padding, 0, _padding, 0);
            _tvReadMore.SetPadding(_padding, 0, _padding, 2 * _padding); // đã có padding bottom của Textview Value nên ko cần padding Top nữa
            _richEditor.LayoutParameters = _paramsRichEditor;
            _richEditor.Enabled = false; // disable không cho click

            //_paramsLnEditorButton.SetMargins(_padding, _padding, _padding, _padding);
            //_lnEditorButton.LayoutParameters = _paramsLnEditorButton;
            //_lnEditorButton.Background = ContextCompat.GetDrawable(frame.Context, Resource.Drawable.textcornergray);

            ////_richEditor.Click += HandleTouchDown;
            ////_edt.Click += HandleTouchDown;

            ////_lnEditor.AddView(_lnEditorButton);
            ////_lnEditor.AddView(_edt);

            //_relaEditor.AddView(_richEditor);
            //_relaEditor.AddView(_lnEditor);
            //_relaEditor.AddView(_edt);
            //_relaEditor.AddView(_tvReadMore);

            _lnContent.Click += HandleTouchDown;
            _edt.Click += HandleTouchDown;
            _tvReadMore.Click += HandleTouchDown;
            _lnEditor.Click += HandleTouchDown;

            frame.AddView(_edt);
            frame.AddView(_tvReadMore);
        }
        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (_parentView != null)
            {
                MinionActionCore.OnElementFormClick(null, new MinionActionCore.ElementFormClick(_element));
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
            if (!String.IsNullOrEmpty(Value))
            {
                _edt.Text = Value;
            }
        }
    }
}