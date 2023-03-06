using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;

class Custom_TableTask : UITableViewSource
{

    public List<BeanTaskDetail> mAllNodes; // =    các dữ liệu đang hiển thi + con của cấp đang hiển thị ( đã được sắp sếp theo thứ tự)
    public List<BeanTaskDetail> mNodes; // từ data đã được sắp xếp lọc ra các phần tử đủ tiêu chi hiển thị
    FormTaskDetails formTaskDetails;
    ControlInputTasks controlInputTask;
    long parentID;

    //int IdGroup;
    public Custom_TableTask(List<BeanTaskDetail> data, ControlInputTasks _parent, FormTaskDetails _formTaskDetails, long _parentID)
    {
        parentID = _parentID;
        controlInputTask = _parent;
        formTaskDetails = _formTaskDetails;
        mAllNodes = data;
        mNodes = Custom_TableTaskHelper.Instance().filterVisibleNode(mAllNodes, _parentID);
    }
    public override nint NumberOfSections(UITableView tableView)
    {
        return 1;
    }
    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
        return 90;
    }
    public override nint RowsInSection(UITableView tableview, nint section)
    {
        return (mNodes.Count);
    }
    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {

        NSString cellIdentifier = new NSString("nodecell");
        var cell = tableView.DequeueReusableCell(cellIdentifier) as Custom_TableTaskCell;
        if (cell == null)
            cell = new Custom_TableTaskCell(cellIdentifier);
        BeanTaskDetail node = mNodes[indexPath.Row];
        BeanTaskDetail parentNode = mAllNodes.Find(s => s.ID == node.Parent);
        if (node.session % 2 == 0)
            cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);
        //if (indexPath.Section % 2 == 0)
        //    cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);

        BeanTask nodeBefore = mNodes[indexPath.Row];
        if (formTaskDetails != null)
            cell.UpdateCell(node, parentNode, null, formTaskDetails);
        else if (controlInputTask != null)
            cell.UpdateCell(node, parentNode, controlInputTask, null);
        return cell;
    }


    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        tableView.DeselectRow(indexPath, true);
        var parentNode = mNodes[indexPath.Row];
        int startPosition = indexPath.Row + 1;
        var endPosition = startPosition;
        tableView.DeselectRow(indexPath, false);
        //Neu khong phải là node lá
        if (!parentNode.isLeaf())
        {
            //Dong/mo cay
            expandOrCollapse(ref endPosition, parentNode);
            //Get lại dữ liệu hiển thị
            mNodes = Custom_TableTaskHelper.Instance().filterVisibleNode(mAllNodes, parentID);
            //Insert/ delete reload dòng lại để hiển thị 
            List<NSIndexPath> indexPathArray = new List<NSIndexPath> { };
            NSIndexPath tempIndexPath;
            for (int i = startPosition; i < endPosition; i++)
            {
                tempIndexPath = NSIndexPath.FromItemSection(i, 0);
                indexPathArray.Add(tempIndexPath);
            }
            if (parentNode.isExpand)
            {
                tableView.InsertRows(indexPathArray.ToArray(), UITableViewRowAnimation.None);
            }
            else
            {
                tableView.DeleteRows(indexPathArray.ToArray(), UITableViewRowAnimation.None);
            }

            tableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromItemSection(indexPath.Row, 0) }, UITableViewRowAnimation.None);
        }
        if (controlInputTask != null)
        {
            controlInputTask.HandleReloadTableTask();
        }
        else if (formTaskDetails != null)
        {
            //formTaskDetails.HandleReloadTableTask();
        }

    }
    public void expandOrCollapse(ref int count, BeanTaskDetail node)
    {

        if (node.isExpand)
        {
            closedChildNode(ref count, node);
        }
        else
        {
            count += mAllNodes.FindAll(s => s.Parent == node.ID).Count;
            node.setExpand(true);
        }
    }
    //Dong tat ca cac phan tu con
    public void closedChildNode(ref int count, BeanTaskDetail node)
    {
        if (node.isLeaf())
            return;
        if (node.isExpand)
        {
            var items = mAllNodes.FindAll(s => s.Parent == node.ID);
            node.isExpand = false;
            foreach (var item in items)
            {
                count += 1;
                closedChildNode(ref count, node: item);
            }
        }
    }
}
