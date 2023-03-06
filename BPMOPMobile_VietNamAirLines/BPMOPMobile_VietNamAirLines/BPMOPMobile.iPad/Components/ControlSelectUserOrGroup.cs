using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlSelectUserOrGroup : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        public ControlSelectUserOrGroup(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.NavigateToUserOrGroupChoiceView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.NavigateToUserOrGroupChoiceView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                    controller.NavigateToUserOrGroupChoiceView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty controller = (FormWFDetailsProperty)parentView;
                    controller.NavigateToUserOrGroupChoiceView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.NavigateToUserOrGroupChoiceView(element, indexPath, this);
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
                    controller.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.NavigatorToFullTextView(element, indexPath, this);
                }
                else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                    controller.NavigatorToFullTextView(element, indexPath, this);
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
                    lbl_value.Text = CmmFunction.GetTitle("TEXT_CONTROL_USERGROUP", "chọn users hoặc groups...");
                    lbl_value.Font = UIFont.FromName("Arial-ItalicMT", 13);
                }
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);
            }
            else
                BT_action.UserInteractionEnabled = false;
        }

        public override void SetTitle()
        {
            base.SetTitle();

            lbl_title.Text = element.Title;
        }

        public override string Value
        {
            set
            {
                var data = value.Trim();
                string result;
                if (data.Contains(";#"))
                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                else if (data.Contains("[{")) // cau truc json
                {
                    string str_value = string.Empty;
                    List<BeanUserAndGroup> lst_user = new List<BeanUserAndGroup>();
                    lst_user = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(data);

                    if (lst_user != null && lst_user.Count > 0)
                    {
                        foreach (var item in lst_user)
                        {
                            str_value = str_value + item.Name + "; ";
                        }
                    }

                    result = str_value.Trim();
                }
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
