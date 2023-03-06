using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.KanBanCustomClass
{
    public partial class CustomKanbanTableView_Drag : UITableViewController, IUITableViewDragDelegate
    {
        KanBanView kanbanView { get; set; }
        NSIndexPath collection_indexPath;
        List<BeanAppBaseExt> lst_drag_workflowItem { get; set; }
        UIDragItem dragItem;
        KanBanModel kanBanModel;
        KanBanModel KanBanModel(NSIndexPath indexPath)
        {
            return kanBanModel;
        }

        public CustomKanbanTableView_Drag(IntPtr handle) : base(handle)
        {
            
        }

        public CustomKanbanTableView_Drag(KanBanView _kanBanView, List<BeanAppBaseExt> _lst_workflowItem, NSIndexPath _indexPath)
        {
            lst_drag_workflowItem = _lst_workflowItem;
            kanbanView = _kanBanView;
            collection_indexPath = _indexPath;
        }

        UIDragItem DragItems(NSIndexPath forAlbumAt)
        {
            NSItemProvider itemProvider = new NSItemProvider();
            var dragItem = new UIDragItem(itemProvider);
            return dragItem;
        }

        /// <summary>
        /// Required for drag operations from a table
        /// </summary>
        public UIDragItem[] GetItemsForBeginningDragSession(UITableView tableView, IUIDragSession session, NSIndexPath indexPath)
        {
            if (tableView.Editing)
            {
                return new UIDragItem[0];
            }
            KanBanModel model = new KanBanModel();
            model.Title = "day la sample";
            model.CardIndexPath = collection_indexPath;

            kanbanView.kanBanModel = model;
            var dragItems = DragItems(indexPath);
            //dragItems.LocalObject = model;
            //dragItem.LocalObject = model;
            kanbanView.workflowItemSelected = lst_drag_workflowItem[indexPath.Row];
            return new UIDragItem[] { dragItems };

        }

        [Export("tableView:dragSessionWillBegin:")]
        public void DragSessionWillBegin(UITableView tableView, IUIDragSession session)
        {
            //NavigationItem.RightBarButtonItem.Enabled = false;
        }

        [Export("tableView:dragSessionDidEnd:")]
        public void DragSessionDidEnd(UITableView tableView, IUIDragSession session)
        {
            //NavigationItem.RightBarButtonItem.Enabled = true;
        }

        
    }
}
