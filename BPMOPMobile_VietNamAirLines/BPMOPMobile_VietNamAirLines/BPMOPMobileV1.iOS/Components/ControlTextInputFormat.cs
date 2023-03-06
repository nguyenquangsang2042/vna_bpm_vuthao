using System;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlTextInputFormat : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UITextView txt_input;

        public ControlTextInputFormat(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            txt_input = new UITextView()
            {
                Font = UIFont.FromName("ArialMT", 14f),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txt_input.Layer.BorderWidth = 1;
            txt_input.Layer.CornerRadius = 3;
            txt_input.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            txt_input.ContentInset = new UIEdgeInsets(0, 15, 0, 0);

            this.AddSubview(txt_input);
            this.WillRemoveSubview(lbl_value);
            this.BringSubviewToFront(BT_action);

            if (BT_action != null)
                BT_action.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);
        }

        private void Txt_input_Ended(object sender, EventArgs e)
        {
            element.Value = txt_input.Text;
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.NavigatorToView(element, indexPath, this);
            }
            //RequestDetailsV2
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.NavigatorToViewAsync(element, indexPath, this);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;
            const int lbl_title_height = 30;

            var heightTxt = frame.Height - ((paddingTop * 2) + spaceView + lbl_title_height);

            lbl_title.HeightAnchor.ConstraintEqualTo(lbl_title_height).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            txt_input.HeightAnchor.ConstraintEqualTo(heightTxt).Active = true;
            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.txt_input, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            BT_action.HeightAnchor.ConstraintEqualTo(heightTxt).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
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
            if (!element.Enable)
            {
                BT_action.UserInteractionEnabled = false;
            }
            else
            {
                txt_input.TextColor = UIColor.FromRGB(51, 95, 179);
                BT_action.UserInteractionEnabled = true;
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
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        NSString htmlString = new NSString(value);
                        NSData htmlData = NSData.FromString(getHtmlStyle() + htmlString);
                        NSAttributedStringDocumentAttributes importParams = new NSAttributedStringDocumentAttributes();
                        importParams.DocumentType = NSDocumentType.HTML;
                        importParams.StringEncoding = NSStringEncoding.UTF8;

                        NSError error = new NSError();
                        error = null;
                        NSDictionary dict = new NSDictionary();
                        UIFont font = UIFont.SystemFontOfSize(14);
                        if (font != null)
                        {
                            dict = new NSMutableDictionary()
                            {
                                {
                                    UIStringAttributeKey.Font,
                                    font
                                }
                            };
                        }

                        var attrString = new NSAttributedString(htmlData, importParams, out dict, ref error);

                        txt_input.AttributedText = attrString;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ControlTextInputFormat - Value - err: " + ex.ToString());
                        throw;
                    }
                }
            }
            get { return this.txt_input.Text; }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }

        public override NSIndexPath IndexPath => this.indexPath;

        private string getHtmlStyle()
        {
            string htmlStyle = "";
            try
            {
                htmlStyle = string.Format("<style>body{{font-family:'{0}'; font-size:{1}px;}}</style>",
                            "Helvetica",
                            14);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("getHtmlStyle - err: " + ex.ToString());
#endif
            }
            return htmlStyle;
        }
    }
}
