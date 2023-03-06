using System;
using System.Collections.Generic;
using System.Globalization;
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
    class ControlNumber: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        public ControlNumber(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (element.Enable)
            {
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorToEditNumberView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigatorToEditNumberView(element, indexPath, this);
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
                else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigatorTouchMoreToFullTextView(element.Title, lbl_value.Text);
                }

                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorTouchMoreToFullTextView(element.Title, lbl_value.Text);
                }
            }
            //if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            //{
            //    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
            //    controller.NavigatorToEditNumberView(element, indexPath, this);
            //}
            //else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
            //{
            //    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
            //    controller.NavigatorToEditNumberView(element, indexPath, this);
            //}
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
                    lbl_value.Text = "0";
                    lbl_value.Font = UIFont.FromName("Arial-ItalicMT", 11);
                }
                BT_action.UserInteractionEnabled = true;
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);
            }
            ////else
            ////    BT_action.UserInteractionEnabled = false;
            //else
            //{
            //    if (!string.IsNullOrEmpty(Value))
            //    {
            //        var flagcheckassignName = CmmIOSFunction.CheckStringTrunCated(lbl_value);
            //        if (flagcheckassignName)
            //        {
            //            BT_action.UserInteractionEnabled = true;
            //            BT_action.Enabled = true;
            //        }
            //        else
            //        {
            //            BT_action.UserInteractionEnabled = false;
            //            BT_action.Enabled = false;
            //        }
            //    }
            //    else
            //        BT_action.UserInteractionEnabled = false;
            //}
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
                string custValue = string.Empty;
                if (!string.IsNullOrEmpty(value))
                    custValue = value;
                else
                    custValue = "";

                this.lbl_value.Text = custValue;
            }
            get { return this.lbl_value.Text; }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = CmmFunction.GetFormatControlDecimal(element);
            //Value = element.Value;
        }
    }
}