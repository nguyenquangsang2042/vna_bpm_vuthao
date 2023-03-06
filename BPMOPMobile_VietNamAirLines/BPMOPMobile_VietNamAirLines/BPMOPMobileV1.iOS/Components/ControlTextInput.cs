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
    public class ControlTextInput : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        //Cust_TextField tf_input;

        public ControlTextInput(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
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

            this.AddSubviews(lbl_value);
            this.WillRemoveSubview(BT_action);

        }

        private void Tf_input_EditingDidEnd(object sender, EventArgs e)
        {
            //element.Value = tf_input.Text;
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);

            const int paddingTop = 10;
            const int spaceView = 5;

            var height = frame.Height - ((paddingTop * 2) + spaceView);

            //tf_input.HeightAnchor.ConstraintEqualTo(height / 2).Active = true;
            //NSLayoutConstraint.Create(this.tf_input, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            //NSLayoutConstraint.Create(this.tf_input, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            //NSLayoutConstraint.Create(this.tf_input, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (element.Enable)
            {
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lbl_value.Text))
                {
                    var flagcheckassignName = CmmIOSFunction.CheckStringTrunCated(lbl_value);
                    if (!flagcheckassignName)
                        return;
                }
                else
                    return;
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorTouchMoreToFullTextView(element.Title, lbl_value.Text);
                }
            }
           
        }

        public override void SetProprety()
        {
            if (element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(lbl_value, item.Key, item.Value);
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
                    lbl_value.Text = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_CONTENT", "Nhập nội dung...");
                    lbl_value.Font = UIFont.FromName("Arial-ItalicMT", 11);
                }
                BT_action.UserInteractionEnabled = true;
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);
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
            set {this.lbl_value.Text = value.Trim();}
            get{return this.lbl_value.Text;}
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }

        public override NSIndexPath IndexPath => this.indexPath;
    }

    /// <summary>
    /// custom text field canh chỉnh padding content
    /// </summary>
    class Cust_TextField : UITextField
    {
        public override CGRect TextRect(CGRect forBounds)
        {
            return RectangleFExtensions.Inset(forBounds, 18, 0);
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            return RectangleFExtensions.Inset(forBounds, 18, 0);
        }
    }
}
