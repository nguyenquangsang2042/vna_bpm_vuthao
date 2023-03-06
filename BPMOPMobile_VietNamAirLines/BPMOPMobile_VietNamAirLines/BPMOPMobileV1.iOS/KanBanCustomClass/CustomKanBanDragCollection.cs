
using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.ViewControllers;
using Foundation;
using UIKit;
using static BPMOPMobileV1.iOS.ViewControllers.KanBanView;

namespace BPMOPMobileV1.iOS.KanBanCustomClass
{
    public class CustomKanBanDragCollection : UICollectionViewDragDelegate
    {
        KanBanView kanBanView { get; set; }
        CollectionWorkFlowStep_Source collect_source;
        UICollectionView CollectionView { get; set; }

        public CustomKanBanDragCollection(KanBanView parentview, CollectionWorkFlowStep_Source _collect_source, UICollectionView collectionView)
        {
            collect_source = _collect_source;
            CollectionView = collectionView;
            kanBanView = parentview;
        }

        Photo Photo(NSIndexPath atPath)
        {
            return Album?.Photos[(int)atPath.Item];
        }

        PhotoAlbum album;
        PhotoAlbum Album
        {
            get { return album; }
            set
            {
                album = value;
                //Title = album?.Title;
            }
        }

        public override UIDragItem[] GetItemsForBeginningDragSession(UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
        {
            var dragItem = DragItem(indexPath);
            return new UIDragItem[] { dragItem };
        }

        [Export("collectionView:itemsForAddingToDragSession:atIndexPath:point:")]
        public UIDragItem[] GetItemsForAddingToDragSession(UICollectionView collectionView, IUIDragSession session, Foundation.NSIndexPath indexPath, CoreGraphics.CGPoint point)
        {
            var dragItem = DragItem(indexPath);
            return new UIDragItem[] { dragItem };
        }

        // Helper method to obtain a drag item for the photo at the index path.
        UIDragItem DragItem(NSIndexPath indexPath)
        {
            KeyValuePair<string, List<BeanAppBaseExt>> stepItem = collect_source.dict_groupWorkFlow.Where(x => x.Key == collect_source.sectionKeys[indexPath.Row]).First();
            BeanAppBaseExt workflowItem = stepItem.Value[0];
            NSItemProvider itemProvider = new NSItemProvider(UIImage.FromFile("Icons/icon_training_temp.png"));
            var dragItem = new UIDragItem(itemProvider);
            dragItem.LocalObject = UIImage.FromFile("Icons/icon_training_temp.png");
            return dragItem;
        }

        //[Export("collectionView:dragPreviewParametersForItemAtIndexPath:")]
        //public UIDragPreviewParameters GetDragPreviewParameters(UICollectionView collectionView, NSIndexPath indexPath)
        //{
        //    return PreviewParameters(indexPath);
        //}
        //UIDragPreviewParameters PreviewParameters(NSIndexPath indexPath)
        //{
        //    var cell = CollectionView.CellForItem(indexPath) as WorkFlowItem_CollectionCell;
        //    var previewParameters = new UIDragPreviewParameters();
        //    previewParameters.VisiblePath = UIBezierPath.FromRect(cell.ClippingRectForPhoto);
        //    return previewParameters;
        //}

        /// <summary>Stores the album state when the drag begins.</summary>
        //PhotoAlbum albumBeforeDrag;
        [Export("collectionView:dragSessionWillBegin:")]
        public void DragSessionWillBegin(UICollectionView collectionView, IUIDragSession session)
        {
            //albumBeforeDrag = album;
        }

        [Export("collectionView:dragSessionDidEnd:")]
        public void DragSessionDidEnd(UICollectionView collectionView, IUIDragSession session)
        {
            //ReloadAlbumFromPhotoLibrary();
            //DeleteItems(collectionView);
            //albumBeforeDrag = null;
        }

        /// <summary>
        /// Compares the album state before and after the drag to delete items in the collection view that represent photos moved elsewhere.
        /// </summary>
        void DeleteItems(UICollectionView collectionView)
        {
            //var albumAfterDrag = album;
            //if (albumBeforeDrag is null || albumAfterDrag is null) return;

            //var indexPathsToDelete = new List<NSIndexPath>();

            //for (var i = 0; i < albumBeforeDrag.Photos.Count; i++)
            //{
            //    var photo = albumBeforeDrag.Photos[i];
            //    if (!albumAfterDrag.Contains(photo))
            //    {
            //        indexPathsToDelete.Add(NSIndexPath.FromItemSection(i, 0));
            //    }
            //}
            //if (indexPathsToDelete.Count > 0)
            //{
            //    collectionView.PerformBatchUpdates(() => {
            //        collectionView.DeleteItems(indexPathsToDelete.ToArray());
            //    }, (finished) => { });
            //}
        }


    }
}
