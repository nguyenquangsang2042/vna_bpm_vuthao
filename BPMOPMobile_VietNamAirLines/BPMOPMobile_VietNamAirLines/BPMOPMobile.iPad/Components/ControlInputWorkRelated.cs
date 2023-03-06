using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlInputWorkRelated: ControlBase
    {
        string currentWorkFlowItemID;
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        public bool isAllowEditWorkFlowRelate;
        ViewElement element { get; set; }

        UITableView tableView_workRelated;
        UILabel lbl_workflowRelate_title, lbl_line;
        List<BeanWorkFlowRelated> lst_workRelated = new List<BeanWorkFlowRelated>();
        public BeanAttachFile currentAttachment { get; set; }

        public ControlInputWorkRelated(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
                currentWorkFlowItemID = requestDetailsV2.workflowItem.ID;
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetail = parentView as WorkflowDetailView;
                currentWorkFlowItemID = workflowDetail.workflowItem.ID;
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                currentWorkFlowItemID = formWorkFlowDetails.currentItemSelected.ID;
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController formWorkFlowDetails = parentView as FollowListViewController;
                currentWorkFlowItemID = formWorkFlowDetails.workflowItem.ID;
            }

            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            tableView_workRelated = new UITableView();
            tableView_workRelated.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            tableView_workRelated.ScrollEnabled = false;

            lbl_line = new UILabel();
            lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
            lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

            lbl_workflowRelate_title = new UILabel
            {
                Font = UIFont.FromName("Arial-BoldMT", 14f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                Text = "Quy trình / Công việc liên kết",
            };

            this.AddSubviews(tableView_workRelated);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            lbl_title.Font = UIFont.FromName("Arial-BoldMT", 14f);

        }

        public void HandleRemoveItem(BeanWorkFlowRelated _beanWorkFlowRelated)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                var index = lst_workRelated.FindIndex(item => item.ID == _beanWorkFlowRelated.ID);
                if (index != -1)
                    lst_workRelated.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                element.Value = jsonString;

                CreateNewTaskView controller = (CreateNewTaskView)parentView;
                controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
            else if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                var index = lst_workRelated.FindIndex(item => item.ID == _beanWorkFlowRelated.ID);
                if (index != -1)
                    lst_workRelated.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                element.Value = jsonString;

                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                var index = lst_workRelated.FindIndex(item => item.ID == _beanWorkFlowRelated.ID);
                if (index != -1)
                    lst_workRelated.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                element.Value = jsonString;

                FollowListViewController controller = (FollowListViewController)parentView;
                controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
        }

        public void HandleSelectedItem(BeanWorkFlowRelated beanWorkFlowRelated)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                //var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
                //if (index != -1)
                //    lst_workRelated.RemoveAt(index);

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
                //element.Value = jsonString;

                //CreateTicketFormView controller = (CreateTicketFormView)parentView;
                //controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            }
            else if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)parentView;
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

            tableView_workRelated.Frame = new CGRect(0, (20 + paddingTop) + 10, frame.Width, frame.Height - ((20 + paddingTop) + 10));
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

            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
                BeanWorkflowItem beanWorkflowItem = requestDetailsV2.workflowItem;
                if (beanWorkflowItem.CreatedBy == CmmVariable.SysConfig.UserId && beanWorkflowItem.Status == "Soạn thảo")
                    isAllowEditWorkFlowRelate = true;
                else
                    isAllowEditWorkFlowRelate = false;
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController requestDetailsV2 = parentView as FollowListViewController;
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
                return 1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 80;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_workRelated.Count;
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
                //Custom_WorkRelatedCell cell = new Custom_WorkRelatedCell(cellIdentifier, null, parentView);
                //var workRelated = lst_workRelated[indexPath.Row];

                //bool isOdd = true;
                //if (indexPath.Row % 2 == 0)
                //    isOdd = false;

                //cell.UpdateCell(workRelated, true, isOdd);
                //return cell;
                return null;
            }
        }
        #endregion
        #endregion
    }
}