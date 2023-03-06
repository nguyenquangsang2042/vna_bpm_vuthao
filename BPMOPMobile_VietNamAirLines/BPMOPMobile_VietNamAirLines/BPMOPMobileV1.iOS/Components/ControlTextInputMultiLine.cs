using System;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlTextInputMultiLine : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UITextView txt_input;
        UIButton BT_readmore;

        public ControlTextInputMultiLine(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            //update
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            if (BT_action != null)
                BT_action.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);

            txt_input = new UITextView()
            {
                Font = UIFont.FromName("ArialMT", 14f),
                TextColor = UIColor.Yellow,
                TranslatesAutoresizingMaskIntoConstraints = false,
                ScrollEnabled = false
            };
            txt_input.UserInteractionEnabled = false;
            txt_input.Layer.BorderWidth = 0;
            var padding = txt_input.TextContainer.LineFragmentPadding;
            txt_input.ContentInset = new UIEdgeInsets(0, -padding, 0, 0);
            txt_input.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;

            BT_readmore = new UIButton();
            BT_readmore.SetTitle(CmmFunction.GetTitle("TEXT_MORE", "xem thêm ...").ToLower(), UIControlState.Normal);
            BT_readmore.Font = UIFont.FromName("Arial-ItalicMT", 11);
            BT_readmore.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
            BT_readmore.SetTitleColor(UIColor.FromRGB(255, 122, 58), UIControlState.Normal);
            BT_readmore.Hidden = true;
            BT_readmore.TouchUpInside += BT_readmore_TouchUpInside;

            this.AddSubviews(txt_input, BT_readmore);
            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
        }

        private void BT_readmore_TouchUpInside(object sender, EventArgs e)
        {
            if (element.Enable)
            {
                if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
            }
            else
            {
                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                    requestDetailsV2.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                    formWFDetailsProperty.NavigatorToFullTextView(element, indexPath, this);
                }
            }
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                controller.NavigatorToEditTextView(element, indexPath, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.NavigatorToEditTextView(element, indexPath, this);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 2;
            const int lbl_title_height = 20;

            //var heightTxt = frame.Height - ((paddingTop * 2) + spaceView + lbl_title_height);

            lbl_title.HeightAnchor.ConstraintEqualTo(lbl_title_height).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            string value = CmmFunction.StripHTML(element.Value);
            var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), this.Frame.Width);
            if (height_ets.Height < 55)//3 dong
            {
                if (height_ets.Height > 30)
                    txt_input.HeightAnchor.ConstraintEqualTo(65).Active = true;
                else
                    txt_input.HeightAnchor.ConstraintEqualTo(height_ets.Height + 25).Active = true;
            }
            else
                txt_input.HeightAnchor.ConstraintEqualTo(65).Active = true;

            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            //BT_action.HeightAnchor.ConstraintEqualTo(heightTxt - 30).Active = true;
            //NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            //NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            //NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f).Active = true;

            if (height_ets.Height > 55)
            {
                BT_readmore.TranslatesAutoresizingMaskIntoConstraints = false;
                NSLayoutConstraint.Create(this.BT_readmore, NSLayoutAttribute.Top, NSLayoutRelation.Equal, txt_input, NSLayoutAttribute.Bottom, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.BT_readmore, NSLayoutAttribute.Right, NSLayoutRelation.Equal, txt_input, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                BT_readmore.WidthAnchor.Equals(100);
                BT_readmore.HeightAnchor.Equals(20);

                //BT_readmore.Frame = new CGRect(this.Frame.Width - 120, this.Frame.Bottom - 30, 100, 20);
                txt_input.TextContainer.MaximumNumberOfLines = 3;
                BT_readmore.Hidden = false;
            }
            else
                BT_readmore.Hidden = true;

        }

        public override void SetProprety()
        {
            if (element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(txt_input, item.Key, item.Value);
                }
            }
        }

        public override void SetEnable()
        {
            base.SetEnable();
            if (element.Enable)
            {
                if (string.IsNullOrEmpty(Value))
                {
                    txt_input.Text = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_CONTENT", "Nhập nội dung...");
                    txt_input.Font = UIFont.FromName("Arial-ItalicMT", 11);
                }
                BT_action.UserInteractionEnabled = true;
                txt_input.TextColor = UIColor.FromRGB(51, 95, 179);

            }
            else
            {
                txt_input.TextColor = UIColor.Black;
                BT_action.UserInteractionEnabled = false;
            }
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override string Value
        {
            set
            {
                var nsError = new NSError();
                var atts = new NSAttributedStringDocumentAttributes
                {
                    DocumentType = NSDocumentType.HTML,
                    StringEncoding = NSStringEncoding.UTF8
                };

                var myHtmlText = value.Trim();
                NSMutableAttributedString att = new NSMutableAttributedString(myHtmlText);
                att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, myHtmlText.Length));

                this.txt_input.AttributedText = att;
            }
            get
            {
                return this.txt_input.Text;
            }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }

        public override NSIndexPath IndexPath => this.indexPath;
    }
}
