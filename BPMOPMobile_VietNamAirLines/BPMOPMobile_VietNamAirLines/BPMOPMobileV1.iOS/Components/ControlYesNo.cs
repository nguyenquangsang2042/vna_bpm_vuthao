﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
//using BPMOPMobileV1.Class;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlYesNo : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UISwitch switch_view;

        public ControlYesNo(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            switch_view = new UISwitch();
            switch_view.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
            switch_view.ValueChanged += Switch_view_ValueChanged;
            switch_view.Enabled = false;

            this.Add(switch_view);

            this.WillRemoveSubview(lbl_value);
            //this.WillRemoveSubview(lbl_line);
            this.WillRemoveSubview(BT_action);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;

            var height = frame.Height - ((paddingTop * 2) + spaceView);

            lbl_title.TranslatesAutoresizingMaskIntoConstraints = false;
            switch_view.TranslatesAutoresizingMaskIntoConstraints = false;
            lbl_value.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_title.HeightAnchor.ConstraintEqualTo(height / 2).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            switch_view.WidthAnchor.ConstraintEqualTo(100).Active = true;
            switch_view.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(this.switch_view, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, 0).Active = true;
            NSLayoutConstraint.Create(this.switch_view, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, -10.0f).Active = true;

            lbl_value.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lbl_value.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(this.lbl_value, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, 0).Active = true;
            NSLayoutConstraint.Create(this.lbl_value, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;


            lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            //if (parentView != null && parentView.GetType() == typeof(ViewDetailController))
            //{
            //    ViewDetailController controller = (ViewDetailController)parentView;
            //    controller.NavigatorToView(element, indexPath, this);
            //}
        }
        private void Switch_view_ValueChanged(object sender, EventArgs e)
        {
            string jsonString = string.Empty;

            if (switch_view.On)
                element.Value = "True";
            else
                element.Value = "False";

            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.Handle_YesNoResult(element);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                controller.Handle_YesNoResult(element);
            }
        }
        public override void SetProprety()
        {
            if (element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(switch_view, item.Key, item.Value);
                }
            }
        }

        public override void SetEnable()
        {
            base.SetEnable();

            if (element.Enable)
            {
                switch_view.Hidden = false;
                switch_view.Enabled = true;
                lbl_value.Hidden = true;
            }
            else
            {
                switch_view.Hidden = true;
                switch_view.Enabled = false;
                lbl_value.Hidden = false;
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
                if (value == "True")
                {
                    this.switch_view.On = true;
                    this.lbl_value.Text = CmmFunction.GetTitle("TEXT_LABEL_YES", "Có");
                }
                else
                {
                    this.switch_view.On = false;
                    this.lbl_value.Text = CmmFunction.GetTitle("TEXT_LABEL_NO", "Không");
                }
            }
            get
            {
                return this.switch_view.On.ToString();
            }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }
    }
}