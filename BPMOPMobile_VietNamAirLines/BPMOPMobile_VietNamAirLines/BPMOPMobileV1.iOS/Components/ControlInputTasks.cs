
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    public class ControlInputTasks : ControlBase
    {
        string currentWorkFlowItemID;
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        public bool isAllowEditTask;
        ViewElement element { get; set; }
        List<KeyValuePair<int, bool>> lst_sectionState;
        UITableView tableView_tasks;
        public static List<BeanTask> lst_task = new List<BeanTask>();
        UIView view_header;
        UILabel lbl_header_title, lbl_nguoiXL, lbl_hanXL, lbl_trangthai, lbl_line;
        UIView view_content;

        public ControlInputTasks(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
            //this.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            //this.Layer.BorderWidth = 0.8f;
            //this.Layer.CornerRadius = 3;

            lbl_title.Font = UIFont.BoldSystemFontOfSize(12);
            lbl_title.TextColor = UIColor.Black;

            view_header = new UIView();
            view_header.BackgroundColor = UIColor.FromRGB(249, 249, 249);

            lbl_header_title = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                Text = CmmFunction.GetTitle("TEXT_TITLE", "Tiêu đề")
            };

            lbl_nguoiXL = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                TextAlignment = UITextAlignment.Left,
                Text = CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý")
            };

            lbl_hanXL = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                TextAlignment = UITextAlignment.Left,
                Text = CmmFunction.GetTitle("TEXT_PROCESSING_TERM", "Hạn xử lý")
            };

            lbl_trangthai = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                TextAlignment = UITextAlignment.Left,
                Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng")//104
            };

            view_header.AddSubviews(new UIView[] { lbl_header_title, lbl_nguoiXL, lbl_hanXL, lbl_trangthai });

            tableView_tasks = new UITableView(new CGRect(0, 0, 0, 0), UITableViewStyle.Grouped);
            tableView_tasks.BackgroundColor = UIColor.White;
            tableView_tasks.ContentInset = new UIEdgeInsets(-32, 0, 0, 0);
            tableView_tasks.ScrollEnabled = false;
            tableView_tasks.ReloadData();

            view_content = new UIView();
            view_content.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            view_content.Layer.BorderWidth = 0.8f;
            view_content.Layer.CornerRadius = 3;
            view_content.AddSubview(tableView_tasks);

            lbl_line = new UILabel();
            lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
            lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

            //this.AddSubviews(view_header, view_content);
            this.AddSubviews(view_content);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

        }

        public void HandleRemoveItem(BeanTask _task)
        {
            //if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            //{
            //    var index = lst_workRelated.FindIndex(item => item.ID == _beanWorkFlowRelated.ID);
            //    if (index != -1)
            //        lst_workRelated.RemoveAt(index);

            //    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
            //    element.Value = jsonString;

            //    CreateNewTaskView controller = (CreateNewTaskView)parentView;
            //    controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            //}
            //else if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            //{
            //    var index = lst_workRelated.FindIndex(item => item.ID == _beanWorkFlowRelated.ID);
            //    if (index != -1)
            //        lst_workRelated.RemoveAt(index);

            //    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
            //    element.Value = jsonString;

            //    ToDoDetailView controller = (ToDoDetailView)parentView;
            //    controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            //}
        }

        public void HandleSelectedItem(BeanTask _task)
        {
            //if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            //{
            //    //var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
            //    //if (index != -1)
            //    //    lst_workRelated.RemoveAt(index);

            //    //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_workRelated);
            //    //element.Value = jsonString;

            //    //CreateTicketFormView controller = (CreateTicketFormView)parentView;
            //    //controller.HandleWorkRelatedResult(element, indexPath, this, lst_workRelated.Count);
            //}
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.Handle_TaskSelected(_task, indexPath);

            }
            //else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            //{
            //    WorkflowDetailView controller = (WorkflowDetailView)parentView;
            //    controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);

            //}
            //else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            //{
            //    FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
            //    controller.HandleWorkRelatedSelected(beanWorkFlowRelated, indexPath);

            //}
        }

        public void HandleRemoveTask(BeanTask _task)
        {
            try
            {
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.Handle_RemoveTask(_task, null);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ControlInputTask - HandleRemoveTask - Err: " + ex.ToString());
            }
        }
        public void HandleReloadTableTask()
        {
            string value = "";
            value = JsonConvert.SerializeObject(lst_task);

            element.Value = value;
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.Reloadrow(element, indexPath);
            }
        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            //if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            //{
            //    //string custID = lst_attachment.Count + 1 + "";
            //    //BeanAttachFile attachmentEmpty = new BeanAttachFile() { ID = custID };
            //    //lst_attachment.Add(attachmentEmpty);

            //    //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
            //    //element.Value = jsonString;

            //    //WorkflowDetailView controller = (WorkflowDetailView)parentView;
            //    //controller.HandleAddAttachment(element, indexPath, this);
            //}
            //else if (parentView.GetType() == typeof(ToDoDetailView))
            //{
            //    ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
            //    requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            //}
            //else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            //{
            //    FormWorkFlowDetails requestDetailsV2 = parentView as FormWorkFlowDetails;
            //    requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            //}

            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }

        }

        public void UpdateTableSections(NSIndexPath nSIndexPath, KeyValuePair<int, bool> section_keyValuePair)
        {
            for (int i = 0; i <= lst_sectionState.Count; i++)
            {
                if (i == nSIndexPath.Section)
                    lst_sectionState[i] = new KeyValuePair<int, bool>(section_keyValuePair.Key, !section_keyValuePair.Value);
            }

            tableView_tasks.ReloadSections(NSIndexSet.FromIndex((int)nSIndexPath.Section), UITableViewRowAnimation.Automatic);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;
            this.ClipsToBounds = true;
            const int paddingTop = 5;
            const int spaceView = 5;
            var width = Frame.Width;

            lbl_title.HeightAnchor.ConstraintEqualTo(14).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            view_content.Frame = new CGRect(0, 36, frame.Width, frame.Height - 36);
            tableView_tasks.Frame = new CGRect(0, 0, frame.Width, view_content.Frame.Height - 20);// padding bottom 20
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
                RequestDetailsV2 toDoDetailView = parentView as RequestDetailsV2;
                //BeanWorkflowItem beanWorkflowItem = toDoDetailView.workflowItem;
            }

            if (!string.IsNullOrEmpty(data))
            {
                JArray json = JArray.Parse(data);
                lst_task = json.ToObject<List<BeanTask>>();
            }
            long parentID = 0;
            Custom_TableTask datasource = new Custom_TableTask(BPMOPMobileV1.iOS.ViewControllers.RequestDetailsV2.lst_tasks, this, null, parentID);
            tableView_tasks.Source = datasource;
            tableView_tasks.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_tasks.ReloadData();
        }

        public override string Value { get => element.Value; set => base.Value = value; }

        #region custom views
        #region Task source table
        private class Tasks_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellTaskID");
            ControlInputTasks parentView;
            Dictionary<BeanTask, List<BeanTask>> dict_tasks = new Dictionary<BeanTask, List<BeanTask>>();
            List<BeanTask> lst_parent { get; set; }
            List<BeanTask> lst_task { get; set; }
            List<BeanTask> lst_lv1 { get; set; }
            List<KeyValuePair<int, bool>> lst_sectionState;
            int rowNum;
            int count = 0;
            nfloat height = 73;

            public Tasks_TableSource(List<BeanTask> _lst_task, ControlInputTasks _parentview)
            {
                lst_task = _lst_task;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {
                if (lst_sectionState == null)
                    lst_sectionState = new List<KeyValuePair<int, bool>>();

                try
                {
                    lst_task = lst_task.OrderBy(i => i.Created).ThenBy(i => i.Created).ToList();
                    lst_parent = lst_task.Where(i => i.Parent == 0).ToList();

                    foreach (var parent in lst_parent)
                    {
                        KeyValuePair<int, bool> keypair_section = new KeyValuePair<int, bool>(parent.ID, true);
                        lst_sectionState.Add(keypair_section);

                        lst_lv1 = lst_task.Where(i => i.Parent == parent.ID).ToList();
                        List<BeanTask> childTasks = new List<BeanTask>();
                        foreach (var i1 in lst_lv1)
                        {
                            childTasks.Add(i1);
                            List<BeanTask> childTasks2 = new List<BeanTask>();
                            var lst_lv2 = lst_task.Where(i => i.Parent == i1.ID).ToList();
                            foreach (var i2 in lst_lv2)
                            {
                                childTasks2.Add(i2);
                                GetSubTask(i2, childTasks2, lst_task);
                            }

                            if (i1.ChildTask == null)
                                i1.ChildTask = new List<BeanTask>();

                            i1.ChildTask.AddRange(childTasks2);
                        }

                        parent.ChildTask = childTasks;
                    }

                    parentView.lst_sectionState = lst_sectionState;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - LoadData - Err: " + ex.ToString());
                }

            }

            private void GetSubTask(BeanTask task, List<BeanTask> lst_lv2, List<BeanTask> lst_all)
            {
                var res = lst_all.Where(i => i.Parent == task.ID).ToList();
                foreach (var item in res)
                {
                    lst_lv2.Add(item);
                    GetSubTask(item, lst_lv2, lst_all);
                }
            }

            private void LoadCountSubTask(BeanTask parent_task)
            {
                if (parent_task.ChildTask != null)
                {
                    foreach (var i2 in parent_task.ChildTask)
                    {
                        rowNum++;
                        var lv2 = lst_task.Where(i => i.Parent == i2.ID).ToList();
                        if (lv2 != null && lv2.Count() > 0)
                        {
                            LoadCountSubTask(i2);
                        }
                    }
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return 0;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var parent = lst_parent[indexPath.Row];
                if (parent.IsExpand)
                {
                    rowNum = 1;
                    LoadCountSubTask(parent);
                }
                else
                    rowNum = 1;

                return (rowNum * 70) + 20;

            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                int numrow;
                if (lst_sectionState[(int)section].Value)
                    numrow = lst_parent.Count;
                else
                    numrow = 0;

                //numrow = lst_parent.Count;

                return numrow;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                BeanTask task = lst_parent[indexPath.Section].ChildTask[indexPath.Row];
                //var item = lst_task[indexPath.Row];
                parentView.HandleSelectedItem(task);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                var task_parent = lst_parent[indexPath.Row];

                Custom_GroupTaskCellView cell = new Custom_GroupTaskCellView(cellIdentifier, parentView);
                cell.UpdateCell(task_parent, lst_task, true, indexPath);// tam set lvl = 0
                cell.BackgroundColor = UIColor.Green;
                return cell;
            }
        }

        private class Tasks_TableSource_BK : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellTaskID");
            ControlInputTasks parentView;
            Dictionary<BeanTask, List<BeanTask>> dict_tasks = new Dictionary<BeanTask, List<BeanTask>>();
            List<BeanTask> lst_parent { get; set; }
            List<BeanTask> lst_task { get; set; }
            List<BeanTask> lst_lv1 { get; set; }
            List<KeyValuePair<int, bool>> lst_sectionState;
            int rowNum;
            int count = 0;
            nfloat height = 73;

            public Tasks_TableSource_BK(List<BeanTask> _lst_task, ControlInputTasks _parentview)
            {
                lst_task = _lst_task;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {
                if (lst_sectionState == null)
                    lst_sectionState = new List<KeyValuePair<int, bool>>();

                try
                {
                    lst_task = lst_task.OrderBy(i => i.Created).ThenBy(i => i.Created).ToList();
                    lst_parent = lst_task.Where(i => i.Parent == 0).ToList();

                    foreach (var parent in lst_parent)
                    {
                        KeyValuePair<int, bool> keypair_section = new KeyValuePair<int, bool>(parent.ID, true);
                        lst_sectionState.Add(keypair_section);

                        lst_lv1 = lst_task.Where(i => i.Parent == parent.ID).ToList();
                        List<BeanTask> childTasks = new List<BeanTask>();
                        foreach (var i1 in lst_lv1)
                        {
                            childTasks.Add(i1);
                            List<BeanTask> childTasks2 = new List<BeanTask>();
                            var lst_lv2 = lst_task.Where(i => i.Parent == i1.ID).ToList();
                            foreach (var i2 in lst_lv2)
                            {
                                childTasks2.Add(i2);
                                GetSubTask(i2, childTasks2, lst_task);
                            }

                            if (i1.ChildTask == null)
                                i1.ChildTask = new List<BeanTask>();

                            i1.ChildTask.AddRange(childTasks2);
                        }

                        parent.ChildTask = childTasks;
                    }

                    parentView.lst_sectionState = lst_sectionState;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - LoadData - Err: " + ex.ToString());
                }

            }

            private void GetSubTask(BeanTask task, List<BeanTask> lst_lv2, List<BeanTask> lst_all)
            {
                var res = lst_all.Where(i => i.Parent == task.ID).ToList();
                foreach (var item in res)
                {
                    lst_lv2.Add(item);
                    GetSubTask(item, lst_lv2, lst_all);
                }
            }

            private void LoadSubTaskData(BeanTask parent_task)
            {
                if (parent_task.ChildTask != null)
                {
                    count++;
                    foreach (var i2 in parent_task.ChildTask)// child1
                    {
                        var lv2 = lst_lv1.Where(i => i.Parent == i2.ID).ToList();
                        if (lv2 != null && lv2.Count() > 0)
                        {
                            if (i2.ChildTask == null)
                                i2.ChildTask = new List<BeanTask>();

                            count++;
                            foreach (var s in lv2)
                            {
                                i2.ChildTask.Add(s);
                                var lv3 = lst_lv1.Where(i => i.Parent == s.ID).ToList();
                                if (lv3 != null && lv3.Count > 0)
                                {
                                    LoadSubTaskData(s);
                                }
                                else
                                {
                                    if (count >= 2)
                                        i2.ChildTask.AddRange(lv2);
                                }
                            }
                            //i2.ChildTask = lv2;
                            //if (count >= 2)
                            //    i2.ChildTask.AddRange(lv2);
                            //else
                            //    LoadSubTaskData(i2);

                        }
                    }
                }
            }

            private void loadSubTask(BeanTask task, List<BeanTask> _lstLv1)
            {
                var lv_sub = lst_lv1.Where(i => i.Parent == task.ID).ToList();
                if (lv_sub != null && lv_sub.Count > 0)
                {
                    foreach (var item in lv_sub)
                    {
                        _lstLv1.Add(item);
                        loadSubTask(item, _lstLv1);
                    }
                }
            }

            private void LoadCountSubTask(BeanTask parent_task)
            {
                if (parent_task.ChildTask != null)
                {
                    foreach (var i2 in parent_task.ChildTask)
                    {
                        rowNum++;
                        var lv2 = lst_lv1.Where(i => i.Parent == i2.ID).ToList();
                        if (lv2 != null && lv2.Count() > 0)
                        {
                            LoadCountSubTask(i2);
                        }
                    }
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return 1;
            }

            //hien khong dung
            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var key = lst_parent[(int)section];

                UIButton btn_expand = new UIButton();
                btn_expand.TouchUpInside += delegate
                {
                    if (lst_sectionState[(int)section].Key == key.ID)
                        lst_sectionState[(int)section] = new KeyValuePair<int, bool>(key.ID, !lst_sectionState[(int)section].Value);

                    parentView.tableView_tasks.ReloadSections(NSIndexSet.FromIndex((int)section), UITableViewRowAnimation.Automatic);
                };

                UIButton btn_action = new UIButton();
                btn_action.TouchUpInside += delegate
                {
                    parentView.HandleSelectedItem(key);
                };

                CGRect frame = new CGRect(0, -10, tableView.Frame.Width - 0, 70);
                btn_action.Frame = frame;
                btn_expand.Frame = new CGRect(0, 0, 30, frame.Height);
                btn_expand.BackgroundColor = UIColor.Red;
                btn_expand.SetImage(UIImage.FromFile("Icons/icon_colapse.png"), UIControlState.Normal);
                btn_expand.ContentEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);

                Custom_TaskSessionHeader headerView = new Custom_TaskSessionHeader(frame, false);
                headerView.LoadData(key, tableView, true, parentView);
                if (key.ChildTask != null && key.ChildTask.Count == 0)
                {
                    headerView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                }
                else
                {
                    headerView.InsertSubview(btn_action, 99);
                    headerView.InsertSubview(btn_expand, 100);
                }
                return headerView;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 70;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var parent = lst_parent[indexPath.Section].ChildTask[indexPath.Row];
                rowNum = 1;
                LoadCountSubTask(parent);
                return rowNum * 70;

            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_parent.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                int numrow;
                if (lst_sectionState[(int)section].Value)
                    numrow = lst_parent[(int)section].ChildTask.Count;
                else
                    numrow = 0;

                return numrow;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                BeanTask task = lst_parent[indexPath.Section].ChildTask[indexPath.Row];
                //var item = lst_task[indexPath.Row];
                parentView.HandleSelectedItem(task);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                BeanTask task = new BeanTask();
                task = lst_parent[indexPath.Section].ChildTask[indexPath.Row];

                Custom_TaskCell cell = new Custom_TaskCell(cellIdentifier);
                cell.UpdateCell(task, indexPath, false, true, parentView.currentWorkFlowItemID, parentView);// tam set lvl = 0
                return cell;
            }
        }
        #endregion
        #endregion
    }
}
