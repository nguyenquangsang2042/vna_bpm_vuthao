﻿using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlInputWorkRelated : ControlBase
    {
        string currentWorkFlowItemID;
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }
        public bool isAllowEditWorkFlowRelate;
        UILabel lbl_workflowRelate_title, lbl_line;
        UITableView tableView_workRelated;
        List<BeanWorkFlowRelated> lst_workRelated = new List<BeanWorkFlowRelated>();
        public BeanAttachFile currentAttachment { get; set; }

        public ControlInputWorkRelated(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                currentWorkFlowItemID = requestDetailsV2.workflowItem.ID;
            }
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            tableView_workRelated = new UITableView();
            tableView_workRelated.ScrollEnabled = false;

            lbl_line = new UILabel();
            lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
            lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

            lbl_workflowRelate_title = new UILabel
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Semibold),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            this.AddSubviews(tableView_workRelated);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            lbl_title.Font = UIFont.BoldSystemFontOfSize(12);
            lbl_title.TextColor = UIColor.FromRGB(0, 0, 0);
        }

        public void HandleRemoveItem(BeanWorkFlowRelated _notify)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
                if (index != -1)
                    lst_workRelated.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                element.Value = jsonString;

                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
            else if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
                if (index != -1)
                    lst_workRelated.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                element.Value = jsonString;

                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
        }

        public void HandleSelectedItem(BeanWorkFlowRelated beanWorkFlowRelated)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                //var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
                //if (index != -1)
                //    lst_workRelated.RemoveAt(index);

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                //element.Value = jsonString;

                //CreateTicketFormView controller = (CreateTicketFormView)parentView;
                //controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
            else if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);

            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            //lbl_workflowRelate_title.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_RELATE", "Quy trình / Công việc liên kết");
            tableView_workRelated.Frame = new CGRect(0, (20 + paddingTop) + 10, frame.Width, frame.Height - ((20 + paddingTop) + 10) - 20);//padding bottom 20
            //border
            tableView_workRelated.ClipsToBounds = true;
            tableView_workRelated.Layer.BorderColor = UIColor.FromRGB(245, 245, 245).CGColor;
            tableView_workRelated.Layer.BorderWidth = 0.8f;
            tableView_workRelated.Layer.CornerRadius = 6;
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override void SetValue()
        {
            base.SetValue();

            var data = element.Value.Trim();

            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                BeanWorkflowItem beanWorkflowItem = requestDetailsV2.workflowItem;
                if (beanWorkflowItem.CreatedBy == CmmVariable.SysConfig.UserId && beanWorkflowItem.Status == "Soạn thảo")
                    isAllowEditWorkFlowRelate = true;
                else
                    isAllowEditWorkFlowRelate = false;
            }

            if (!string.IsNullOrEmpty(data))
            {
                JArray json = JArray.Parse(data);
                lst_workRelated = json.ToObject<List<BeanWorkFlowRelated>>();
            }

            tableView_workRelated.Source = new WorkRelated_TableSource(lst_workRelated, this);
            tableView_workRelated.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_workRelated.ReloadData();
        }

        public override string Value { get => element.Value; set => base.Value = value; }

        #region custom views
        #region attachment source table
        private class WorkRelated_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            ControlInputWorkRelated parentView;
            List<BeanWorkFlowRelated> lst_workRelated { get; set; }
            public WorkRelated_TableSource(List<BeanWorkFlowRelated> _lst_workRelated, ControlInputWorkRelated _parentview)
            {
                lst_workRelated = _lst_workRelated;
                parentView = _parentview;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 100;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_workRelated.Count;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                if (lst_workRelated.Count > 0)
                {
                    var item = lst_workRelated[indexPath.Row];
                    parentView.HandleRemoveItem(item);
                }
            }

            public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
            {
                return "Xoá";
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                if (parentView.isAllowEditWorkFlowRelate)
                    return true;
                else
                    return false;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_workRelated[indexPath.Row];
                parentView.HandleSelectedItem(item);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var todo = lst_workRelated[indexPath.Row];
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;
                
                Custom_WorkRelatedCell cell = new Custom_WorkRelatedCell(cellIdentifier);
                cell.UpdateCell(todo, true, isOdd, parentView.currentWorkFlowItemID);
                return cell;
            }
        }
        #endregion
        #endregion
    }
}
