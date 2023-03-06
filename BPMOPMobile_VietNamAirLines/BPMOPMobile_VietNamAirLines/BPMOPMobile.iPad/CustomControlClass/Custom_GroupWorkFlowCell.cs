using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    class Custom_GroupWorkFlowCell: UITableViewCell
    {
        UICollectionView collection_groupWorkFlow;
        List<BeanWorkflow> lst_workFlow { get; set; }
        UIViewController viewController { get; set; }

        public Custom_GroupWorkFlowCell(NSString cellID, UIViewController _viewController) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
            viewController = _viewController;
        }

        public Custom_GroupWorkFlowCell(IntPtr handle) : base(handle)
        {
        }

        public Custom_GroupWorkFlowCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(List<BeanWorkflow> _lst_workFlow)
        {
            lst_workFlow = _lst_workFlow;
            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout();
            //flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            flowLayout.MinimumInteritemSpacing = 0;
            flowLayout.MinimumLineSpacing = 0;

            CGRect frame = CGRect.Empty;
            collection_groupWorkFlow = new UICollectionView(frame: frame, layout: flowLayout);
            collection_groupWorkFlow.RegisterClassForCell(typeof(CollectionCell), CollectionCell.CellID);
            collection_groupWorkFlow.ShowsHorizontalScrollIndicator = false;
            collection_groupWorkFlow.AllowsMultipleSelection = false;
            collection_groupWorkFlow.BackgroundColor = UIColor.White;

            this.AddSubview(collection_groupWorkFlow);
        }

        private void LoadData()
        {
            try
            {
                Collection_Source collectionCate_Source = new Collection_Source(this, lst_workFlow);
                collection_groupWorkFlow.Source = collectionCate_Source;
                collection_groupWorkFlow.Delegate = new CustomFlowLayoutDelegate(this, collectionCate_Source);
                collection_groupWorkFlow.ReloadData();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Custom_GroupWorkFlowCell - LoadData - Err: " + ex.ToString());
#endif
            }
        }

        private void HandleSeclectItem(BeanWorkflow _workFlowItem)
        {
            if (viewController != null && viewController.GetType() == typeof(CreateNewTaskView))
            {
                CreateNewTaskView controller = (CreateNewTaskView)viewController;
                controller.HandleGroupWorkFlowCellResult(_workFlowItem);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            collection_groupWorkFlow.Frame = new CGRect(0, 0, this.ContentView.Frame.Width, this.ContentView.Frame.Height - 25);//footer height: 25
        }

        #region collection menu
        private class Collection_Source : UICollectionViewSource
        {
            Custom_GroupWorkFlowCell parentView { get; set; }
            public List<BeanWorkflow> lst_workFlow { get; set; }

            public Collection_Source(Custom_GroupWorkFlowCell _parentview, List<BeanWorkflow> _lst_workFlow)
            {
                parentView = _parentview;
                lst_workFlow = _lst_workFlow;
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_workFlow.Count;
            }

            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                CollectionCell cell = (CollectionCell)collectionView.DequeueReusableCell(CollectionCell.CellID, indexPath);
                cell.UpdateRow(lst_workFlow[indexPath.Row], parentView, indexPath);
                
                return cell;
            }
        }
        private class CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            UIImageView iv_cover;
            UILabel lbl_name;
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            NSIndexPath indexPath { get; set; }

            [Export("initWithFrame:")]
            public CollectionCell(RectangleF frame) : base(frame)
            {
                ContentView.BackgroundColor = UIColor.White;

                iv_cover = new UIImageView();
                iv_cover.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                iv_cover.ContentMode = UIViewContentMode.ScaleAspectFit;

                lbl_name = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                    TextColor = UIColor.Black
                };

                ContentView.AddSubviews(iv_cover, lbl_name);
            }

            public async void UpdateRow(BeanWorkflow _workFlow, Custom_GroupWorkFlowCell _parentView, NSIndexPath _indexPath)
            {
                try
                {
                    indexPath = _indexPath;
                    lbl_name.Text = _workFlow.Title;

                    string filepathURL = "https://png.pngtree.com/png-clipart/20200225/original/pngtree-happy-holi-colorful-splatter-color-splash-free-png-and-psd-png-image_5308640.jpg";
                    if (string.IsNullOrEmpty(_workFlow.ImageURL))
                    {
                        var result = await CmmIOSFunction.CheckFileLocalIsExist(filepathURL);
                        if (!string.IsNullOrEmpty(result))
                            CmmIOSFunction.openFile(result, "Icons/icon_learn_temp.png", iv_cover);
                    }
                    else
                    {
                        iv_cover.Image = UIImage.FromFile("Icons/" + _workFlow.ImageURL);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("Custom_GroupWorkFlowCell - CollectionCell - UpdateRow - Err: " + ex.ToString());
#endif
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var width = ContentView.Frame.Width;
                var height = ContentView.Frame.Height;

                iv_cover.Frame = new CGRect(25, 25, width - 50, height - 70);
                lbl_name.Frame = new CGRect(0, iv_cover.Frame.Bottom + 25, width, 20);
            }
        }
        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static Custom_GroupWorkFlowCell parentView;
            Collection_Source collect_source;

            #region Constructors
            public CustomFlowLayoutDelegate(Custom_GroupWorkFlowCell _parent, Collection_Source _collect_source)
            {
                collect_source = _collect_source;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize(collectionView.Bounds.Width / 5, 163);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var itemSelected = collect_source.lst_workFlow[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }
        #endregion
    }
}