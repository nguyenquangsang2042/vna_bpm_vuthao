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
    class ControlTree: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        public ControlTree(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                //WorkflowDetailView controller = (WorkflowDetailView)parentView;
                //controller.NavigatorToView(element, indexPath, this);
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
                BT_action.UserInteractionEnabled = true;
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);
            }
            else
                BT_action.UserInteractionEnabled = false;
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
                var data = value.Trim();
                string result;
                if (data.Contains(";#"))
                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                else
                    result = data;

                this.lbl_value.Text = result;
            }
            get { return this.lbl_value.Text; }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }
    }
}