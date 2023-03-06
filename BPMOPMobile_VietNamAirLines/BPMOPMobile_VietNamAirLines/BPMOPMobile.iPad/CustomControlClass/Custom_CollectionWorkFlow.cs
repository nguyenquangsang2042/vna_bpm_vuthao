using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_CollectionWorkFlow : UICollectionViewSource
    {
        MainView parentView { get; set; }
        List<BeanWorkflow> lst_workflow { get; set; }
        bool isShowIconFollow;

        public Custom_CollectionWorkFlow(MainView _parentview, List<BeanWorkflow> _lst_workflow, bool _isShowIconFollow)
        {
            parentView = _parentview;
            lst_workflow = _lst_workflow;
            isShowIconFollow = _isShowIconFollow;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return lst_workflow.Count;
        }
        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }
       
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            parentView.NavigateToViewByCate(lst_workflow[indexPath.Row]);
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            BeanWorkflow tickit = lst_workflow[indexPath.Row];
            var cell = (WorkFlowGroup_CollectionCell)collectionView.DequeueReusableCell(WorkFlowGroup_CollectionCell.CellID, indexPath);
            cell.UpdateRow(tickit, parentView, isShowIconFollow);
            return cell;
        }
    }
}
