using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlDate: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        public ControlDate(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView toDoDetailView = (ToDoDetailView)parentView;
                    toDoDetailView.NavigatorToDateTimeChoice(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView toDoDetailView = (WorkflowDetailView)parentView;
                    toDoDetailView.NavigatorToDateTimeChoice(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails toDoDetailView = (FormWorkFlowDetails)parentView;
                    toDoDetailView.NavigatorToDateTimeChoice(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigatorToDateTimeChoice(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.NavigatorToDateTimeChoice(element, indexPath, this);
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
                    ToDoDetailView toDoDetailView = (ToDoDetailView)parentView;
                    toDoDetailView.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView toDoDetailView = (WorkflowDetailView)parentView;
                    toDoDetailView.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails toDoDetailView = (FormWorkFlowDetails)parentView;
                    toDoDetailView.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.NavigatorToFullTextView(element, indexPath, this);
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
                BT_action.UserInteractionEnabled = true;
                if (string.IsNullOrEmpty(Value))
                {
                    if (element.DataType == "date")
                        lbl_value.Text = CmmFunction.GetTitle("TEXT_CHOOSE_DATE", "Chọn ngày...");
                    else if (element.DataType == "datetime")
                        lbl_value.Text = CmmFunction.GetTitle("TEXT_CHOOSE_DATETIME", "Chọn ngày giờ...");

                    lbl_value.Font = UIFont.FromName("Arial-ItalicMT", 11);
                }
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
                var custValue = "";
                DateTime dateValue = new DateTime();

                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        dateValue = DateTime.Parse(value);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unable to convert '{0}'.", value);
                    }

                    if (CmmVariable.SysConfig.LangCode == "1066")
                    {
                        if (element.DataType == "date")
                            custValue = dateValue.ToString(CmmVariable.M_WorkDateFormatDateVN);
                        else if (element.DataType == "datetime")
                            custValue = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                    }
                    else if (CmmVariable.SysConfig.LangCode == "1033")
                    {
                        if (element.DataType == "date")
                            custValue = dateValue.ToString(CmmVariable.M_WorkDateFormatDayEN);
                        else if (element.DataType == "datetime")
                            custValue = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                    }
                    else if (element.DataType == "time")
                        custValue = dateValue.ToString("HH:mm");
                }

                this.lbl_value.Text = custValue;
            }
            get { return this.lbl_value.Text; }
        }

        public override void SetValue()
        {
            base.SetValue();

            if (string.IsNullOrEmpty(element.Value))
                this.lbl_value.Text = "";
            else
                Value = element.Value;
        }
    }
}