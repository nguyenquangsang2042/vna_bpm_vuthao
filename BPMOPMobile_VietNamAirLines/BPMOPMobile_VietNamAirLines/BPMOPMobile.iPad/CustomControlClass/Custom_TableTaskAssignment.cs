using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.Components;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_TableTaskAssignment : UITableViewSource
    {
        NSString cellIdentifier = new NSString("cellTaskID");
        NSIndexPath root_nSIndexPath;
        ControlInputTasks parentView;
        Dictionary<BeanTask, List<BeanTask>> dict_tasks = new Dictionary<BeanTask, List<BeanTask>>();
        BeanTask task { get; set; }
        List<BeanTask> lst_sub { get; set; }
        List<KeyValuePair<int, bool>> lst_sectionState;
        nfloat height = 73;
        UITableView tableView;
        bool isRoot;

        public Custom_TableTaskAssignment(BeanTask _task, ControlInputTasks _parentview, UITableView _table, bool _isRoot, NSIndexPath _root_nSIndexPath)
        {
            task = _task;
            root_nSIndexPath = _root_nSIndexPath;

            if (_task.ChildTask != null && _task.ChildTask.Count > 0)
                lst_sub = _task.ChildTask;
            tableView = _table;
            isRoot = _isRoot;
            parentView = _parentview;

            if (lst_sectionState == null)
                lst_sectionState = new List<KeyValuePair<int, bool>>();

            KeyValuePair<int, bool> keypair_section = new KeyValuePair<int, bool>(task.ID, true);
            lst_sectionState.Add(keypair_section);
            //LoadSubTaskData(task);
        }

        private void LoadSubTaskData(BeanTask parent_task)
        {
            //if (parent_task.ChildTask != null)
            //{
            //    lst_parent.Add(parent_task);
            //    foreach (var i2 in parent_task.ChildTask)
            //    {
            //        var lv2 = lst_lv1.Where(i => i.Parent == i2.ID).ToList();
            //        if (lv2 != null && lv2.Count() > 0)
            //        {
            //            i2.ChildTask = lv2;
            //            if (count >= 3)
            //                i2.ChildTask.AddRange(lv2);
            //            else
            //                LoadSubTaskData(i2);

            //        }
            //    }
            //}

        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            return 1;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIButton btn_expand = new UIButton();
            btn_expand.TouchUpInside += delegate
            {
                lst_sectionState[(int)section] = new KeyValuePair<int, bool>((int)task.Parent, !lst_sectionState[(int)section].Value);
                task.IsExpand = !task.IsExpand;
                tableView.ReloadSections(NSIndexSet.FromIndex((int)section), UITableViewRowAnimation.Automatic);
                parentView.UpdateTableSections(root_nSIndexPath, lst_sectionState[(int)section]);
            };

            UIButton btn_action = new UIButton();
            btn_action.TouchUpInside += delegate
            {
                parentView.HandleSelectedItem(task);
            };

            CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 70);
            btn_action.Frame = frame;
            btn_expand.Frame = new CGRect(0, 0, 40, frame.Height);
            //btn_expand.SetImage(UIImage.FromFile("Icons/icon_colapse.png"), UIControlState.Normal);
            btn_expand.ContentEdgeInsets = new UIEdgeInsets(4,4,4,4);

            Custom_TaskSessionHeader headerView = new Custom_TaskSessionHeader(frame, true);
            headerView.BackgroundColor = UIColor.FromRGB(251, 251, 251);
            headerView.LoadData(task, tableView, isRoot, null);
            headerView.InsertSubview(btn_action, 99);
            headerView.InsertSubview(btn_expand, 100);
            return headerView;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 70;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            int row = 0;
            var childLvl1 = lst_sub[indexPath.Section].ChildTask;
            if (childLvl1 != null)
                row = childLvl1.Count;
            else
                row = 1;

            //if (childLvl1 != null)
            //{
            //    row = row + 1;

            //    foreach (var item in lst_lv1)
            //    {
            //        if (item.Parent == childLvl1.ID)
            //        {
            //            row = row + 1;
            //            var childLvl2 = lst_lv1.Where(i => i.Parent == childLvl1.ID).ToList();
            //            if (childLvl2 != null && childLvl2.Count > 0)
            //            {
            //                row = row + 1;
            //                childLvl1.ChildTask = childLvl2;
            //            }
            //        }
            //    }
            //}

            return row * 70;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            int numSection = 0;

            if (lst_sub != null && lst_sub.Count > 0)
                numSection = 1;
            else
                numSection = 0;

            return numSection;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            int numRow;
            if (lst_sectionState[(int)section].Value)
            {
                if (lst_sub != null && lst_sub.Count > 0)
                    numRow = lst_sub.Count;
                else
                    numRow = 0;
            }
            else
                numRow = 0;

            return numRow;

            //int numRow;
            //if (lst_sub != null && lst_sub.Count > 0)
            //    numRow = lst_sub.Count;
            //else
            //    numRow = 0;

            //return numRow;
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            //if (lst_workRelated.Count > 0)
            //{
            //    var item = lst_workRelated[indexPath.Row];
            //    parentView.HandleRemoveItem(item);
            //}
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
                                                                          if (lst_sub.Count > 0)
                                                                          {
                                                                              var item = lst_sub[row];
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
            var task = lst_sub[indexPath.Row];

            if (task.Status != (int)ActionStatusID.Completed && task.CreatedBy == CmmVariable.SysConfig.UserId)
                return true;
            else
                return false;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = lst_sub[indexPath.Row];
            parentView.HandleSelectedItem(item);
            tableView.DeselectRow(indexPath, true);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            bool isOdd = true;
            if (indexPath.Row % 2 == 0)
                isOdd = false;

            var task = lst_sub[indexPath.Row];
            Custom_TaskCell cell = new Custom_TaskCell(cellIdentifier);
            cell.UpdateCell(task, indexPath, true, false, "0", parentView);// tam set lvl = 0
            return cell;
        }
    }
}
