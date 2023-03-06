
using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_GroupTaskCellView : UITableViewCell
    {
        BeanTask parent_task { get; set; }
        List<BeanTask> lst_subTask { get; set; }
        public List<BeanTask> lst_allSubTask { get; set; }
        NSIndexPath parent_indexPath;
        UITableView tableView { get; set; }
        ControlInputTasks viewController { get; set; }
        bool isRoot;

        public Custom_GroupTaskCellView(IntPtr handle) : base(handle)
        {
        }

        public Custom_GroupTaskCellView(NSString cellID, ControlInputTasks _viewController) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
            viewController = _viewController;

        }
        public Custom_GroupTaskCellView(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        private void ViewConfiguration()
        {
            ContentView.BackgroundColor = UIColor.FromRGB(251, 251, 251);
            //ContentView.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            ContentView.Layer.BorderColor = UIColor.White.CGColor;
            ContentView.Layer.BorderWidth = 0.8f;
            ContentView.Layer.CornerRadius = 3;

            tableView = new UITableView(new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height), UITableViewStyle.Grouped);
            //tableView.BackgroundColor = UIColor.FromRGB(232,232,232);
            tableView.BackgroundColor = UIColor.White;
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.ScrollEnabled = false;
            this.AddSubview(tableView);
        }

        public void UpdateCell(BeanTask _parentTask, List<BeanTask> _lst_allSubTask, bool _isRoot, NSIndexPath nSIndexPath)
        {
            parent_task = _parentTask;
            lst_allSubTask = _lst_allSubTask;
            isRoot = _isRoot;
            lst_subTask = parent_task.ChildTask;
            parent_indexPath = nSIndexPath;
            ViewConfiguration();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                tableView.Source = new Tasks_TableSource(parent_task, lst_subTask, lst_allSubTask, isRoot, parent_indexPath, viewController);
                tableView.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            tableView.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height);
            tableView.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            //tableView.Layer.BorderColor = UIColor.Red.CGColor;
            tableView.Layer.BorderWidth = 0.8f;
        }

        #region custom class
        private class Tasks_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellTaskID");
            NSIndexPath parent_indexPath;
            ControlInputTasks parentView;
            Dictionary<BeanTask, List<BeanTask>> dict_tasks = new Dictionary<BeanTask, List<BeanTask>>();
            BeanTask parent_task { get; set; }
            List<BeanTask> lst_subTask { get; set; }
            List<BeanTask> lst_allSubTask { get; set; }
            List<BeanTask> lst_lv1 { get; set; }
            List<KeyValuePair<int, bool>> lst_sectionState;
            int rowNum;
            bool isRoot;
            bool isExpand = true;
            nfloat height = 73;
            bool isHasSubTask;

            public Tasks_TableSource(BeanTask _parentTask, List<BeanTask> _lst_subTask, List<BeanTask> _lst_allSubTask, bool _isRoot, NSIndexPath nSIndexPath, ControlInputTasks _parentview)
            {
                parent_task = _parentTask;
                lst_subTask = _lst_subTask;
                lst_allSubTask = _lst_allSubTask;
                isRoot = _isRoot;
                parentView = _parentview;
                parent_indexPath = nSIndexPath;

                if (lst_sectionState == null)
                    lst_sectionState = new List<KeyValuePair<int, bool>>();

                KeyValuePair<int, bool> keypair_section = new KeyValuePair<int, bool>(parent_task.ID, true);
                lst_sectionState.Add(keypair_section);

                LoadData();
            }

            private void LoadData()
            {
                try
                {
                    if (lst_subTask == null && lst_subTask.Count == 0)
                    {
                        isHasSubTask = false;
                    }
                    else
                    {
                        isHasSubTask = true;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_GroupTaskCellView - LoadData - Err: " + ex.ToString());
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
                if (parent_task.ChildTask != null && parent_task.ChildTask.Count != 0)
                {
                    foreach (var i2 in parent_task.ChildTask)
                    {
                        rowNum++;
                        var lv2 = lst_allSubTask.Where(i => i.Parent == i2.ID).ToList();
                        if (lv2 != null && lv2.Count() > 0)
                        {
                            LoadCountSubTask(i2);
                        }
                    }
                }
               // else rowNum = 0;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var key = parent_task;
                lst_sectionState[(int)section] = new KeyValuePair<int, bool>((int)key.ID, !lst_sectionState[(int)section].Value);
                UIButton btn_expand = new UIButton();
                btn_expand.TouchUpInside += delegate
                {
                    //Tam khoa do chua xu ly duoc

                    //if (isExpand)
                    //    isExpand = false;
                    //else
                    //    isExpand = true;

                    //tableView.ReloadSections(NSIndexSet.FromIndex((int)section), UITableViewRowAnimation.Automatic);
                    //parentView.UpdateTableSections(parent_indexPath, lst_sectionState[(int)section]);
                };

                UIButton btn_action = new UIButton();
                btn_action.TouchUpInside += delegate
                {
                    parentView.HandleSelectedItem(key);
                };

                CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 90);
                btn_action.Frame = frame;
                btn_expand.Frame = new CGRect(0, 0, 30, frame.Height);

                Custom_TaskSessionHeader headerView = new Custom_TaskSessionHeader(frame, false);
                headerView.LoadData(key, tableView, isRoot, parentView);
                headerView.BackgroundColor = UIColor.FromRGB(251, 251, 251);
                if (key.ChildTask != null && key.ChildTask.Count == 0)
                {
                    headerView.InsertSubview(btn_action, 99);
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
                return 90;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var sub_parent = lst_subTask[indexPath.Row];

                rowNum = 1;
                LoadCountSubTask(sub_parent);
                return rowNum * 90;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (isExpand)
                    return lst_subTask.Count;
                else
                    return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                BeanTask task = lst_subTask[indexPath.Row];
                parentView.HandleSelectedItem(task);
                tableView.DeselectRow(indexPath, true);
            }

            public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
            {
                var flagAction = ContextualFlagAction(indexPath.Row);
                var trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction });

                trailingSwipe.PerformsFirstActionWithFullSwipe = false;
                return trailingSwipe;
            }

            // delete item
            public UIContextualAction ContextualFlagAction(int row)
            {
                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                          "",
                                                                          (FlagAction, view, success) =>
                                                                          {
                                                                              if (lst_subTask.Count > 0)
                                                                              {
                                                                                  var item = lst_subTask[row];
                                                                                  parentView.HandleRemoveTask(item);

                                                                              }
                                                                              success(true);
                                                                          });

                action.Image = UIImage.FromFile("Icons/icon_swipe_delete.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).Scale(new CGSize(20, 20), 3);
                action.BackgroundColor = UIColor.FromRGB(235, 52, 46);
                return action;
            }
            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                var task = lst_subTask[indexPath.Row];
                if (task.ChildTask != null && task.ChildTask.Count == 0)
                {
                    if (task.Status != (int)ActionStatusID.Completed && task.CreatedBy == CmmVariable.SysConfig.UserId)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                var task = lst_subTask[indexPath.Row];

                Custom_TaskCell cell = new Custom_TaskCell(cellIdentifier);
                cell.UpdateCell(task, indexPath, false, isRoot, "", parentView);// tam set lvl = 0
                return cell;
            }
        }

        public class Custom_GroupTaskCell : UITableViewCell
        {
            UIView viewBackground;
            UITableView tableGroupTask;
            BeanTask parent_task;
            ControlInputTasks parentView { get; set; }
            ControlInputTasks controlBase { get; set; }
            NSIndexPath root_nSIndexPath;

            List<BeanTask> lst_task { get; set; }
            List<BeanTask> lst_lv1 { get; set; }

            public Custom_GroupTaskCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;

            }

            public void UpdateCell(BeanTask _parent_task, ControlInputTasks _controlBase, bool _isOdd)
            {
                parent_task = _parent_task;
                controlBase = _controlBase;
                ViewConfiguration();
                LoadData();

                if (_isOdd)
                    tableGroupTask.BackgroundColor = UIColor.Magenta;
                else
                    tableGroupTask.BackgroundColor = UIColor.Yellow;
            }

            private void ViewConfiguration()
            {
                tableGroupTask = new UITableView();
                tableGroupTask.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                tableGroupTask.ScrollEnabled = false;
                ContentView.Add(tableGroupTask);
            }

            private void LoadData()
            {
                try
                {


                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
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

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                tableGroupTask.Frame = new CGRect(ContentView.Frame.X, ContentView.Frame.Y, ContentView.Frame.Width, ContentView.Frame.Height);

            }
        }
        #endregion
    }


}
