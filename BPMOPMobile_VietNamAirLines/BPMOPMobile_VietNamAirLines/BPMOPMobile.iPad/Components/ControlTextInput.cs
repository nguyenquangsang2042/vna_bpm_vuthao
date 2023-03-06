using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using BPMOPMobile.iPad.ViewControllers.Applications;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlTextInput : ControlBase
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

            //tf_input = new Cust_TextField()
            //{
            //    Font = UIFont.SystemFontOfSize(14),
            //    TranslatesAutoresizingMaskIntoConstraints = false
            //};
            //tf_input.Layer.BorderWidth = 1;
            //tf_input.Layer.CornerRadius = 3;
            //tf_input.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

            //this.AddSubview(tf_input);

            if (BT_action != null)
                BT_action.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);

            //if (tf_input != null)
            //    tf_input.EditingDidEnd += Tf_input_EditingDidEnd;
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {

            if (element.Enable)
            {
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController controller = (FollowListViewController)parentView;
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

                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.NavigatorToEditTextView(element, indexPath, this);
                }
            }

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
                //BT_action.UserInteractionEnabled = true;
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);

            }
            else
            {
                //BT_action.UserInteractionEnabled = false;
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
            set { this.lbl_value.Text = value.Trim(); }
            get { return this.lbl_value.Text; }
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