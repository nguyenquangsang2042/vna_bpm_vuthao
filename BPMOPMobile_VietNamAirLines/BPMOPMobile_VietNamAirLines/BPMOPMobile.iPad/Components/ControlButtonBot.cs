using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlButtonBot: ControlBase
    {
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }

        UIImageView iv_action;

        public ControlButtonBot()
        { 
        }

        public ControlButtonBot(UIViewController _parentView, ViewElement _element)
        {
            parentView = _parentView;
            element = _element;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            iv_action = new UIImageView();
            iv_action.Image = UIImage.FromFile("Icons/button_close3x.png");
            iv_action.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_action.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_title.Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular);
            lbl_title.TextColor = UIColor.Black;

            this.Add(iv_action);
            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(lbl_line);

            if (BT_action != null)
                BT_action.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            if (element != null)
            {
                var width = frame.Width - 45;

                //if (string.IsNullOrEmpty(element.Title))
                //{
                iv_action.WidthAnchor.ConstraintEqualTo(20).Active = true;
                iv_action.HeightAnchor.ConstraintEqualTo(20).Active = true;
                NSLayoutConstraint.Create(this.iv_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, -(frame.Width / 4)).Active = true;
                NSLayoutConstraint.Create(this.iv_action, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;
                //}
                //else
                //{
                //    lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
                //    lbl_title.WidthAnchor.ConstraintEqualTo(width).Active = true;
                //    NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1.0f, ((width / 2))).Active = true;
                //    NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                //    iv_action.WidthAnchor.ConstraintEqualTo(20).Active = true;
                //    iv_action.HeightAnchor.ConstraintEqualTo(20).Active = true;
                //    NSLayoutConstraint.Create(this.iv_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Left, 1.0f, -5.0f).Active = true;
                //    NSLayoutConstraint.Create(this.iv_action, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;
                //}

                NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f).Active = true;
            }
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleButtonBot(element);
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleButtonBot(element);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                controller.HandleButtonBot(element);
            }
        }

        public override void SetProprety()
        {
            if (element != null && element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(lbl_title, item.Key, item.Value);
                }
            }
        }

        public override void SetEnable()
        {
            base.SetEnable();

        }

        public override void SetTitle()
        {
            base.SetTitle();
            
            //if(element != null)
            //    lbl_title.Text = element.Title;
        }

        public override void SetValue()
        {
            base.SetValue();

            if (element != null && !string.IsNullOrEmpty(element.ID))
                iv_action.Image = UIImage.FromFile("Icons/icon_Btn_action_" + element.ID).ImageWithAlignmentRectInsets(new UIEdgeInsets(top: -2, left: 0, bottom: -2, right: 0));
        }
    }
}