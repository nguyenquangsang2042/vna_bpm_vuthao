using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlAttachment: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UICollectionView collection_attachment;
        List<KeyValuePair<string,string>> lst_attachment = new List<KeyValuePair<string, string>>();
        public KeyValuePair<string, string> currentAttachment { get; set; }

        public ControlAttachment(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout();
            flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            flowLayout.MinimumInteritemSpacing = 0;
            flowLayout.MinimumLineSpacing = 0;

            CGRect frame = CGRect.Empty;
            collection_attachment = new UICollectionView(frame: frame, layout: flowLayout);
            collection_attachment.RegisterClassForCell(typeof(CollectionCell), CollectionCell.CellID);
            collection_attachment.ShowsHorizontalScrollIndicator = false;
            collection_attachment.AllowsMultipleSelection = false;
            collection_attachment.BackgroundColor = UIColor.White;

            this.Add(collection_attachment);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);
        }

        public void HandleSeclectItem(KeyValuePair<string, string> _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                currentAttachment = _attachment;
                //WorkflowDetailView controller = (WorkflowDetailView)parentView;
                //controller.NavigatorToView(element, indexPath, this);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            var topAnchor = (20 + paddingTop + spaceView);
            collection_attachment.Frame = new CGRect(0, topAnchor, frame.Width, frame.Height - topAnchor);
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
            if (data.Contains(";#"))
            {
                var arrAttachment = data.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (arrAttachment.Length > 2)
                {
                    for (var i = 0; i < arrAttachment.Length; i += 2)
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(arrAttachment[i], arrAttachment[i+1]);
                        lst_attachment.Add(item);
                    }
                }
                else
                    lst_attachment.Add(new KeyValuePair<string, string>(arrAttachment[0], arrAttachment[1]));
            }

            Collection_Source collectionCate_Source = new Collection_Source(this, lst_attachment);
            collection_attachment.Source = collectionCate_Source;
            collection_attachment.Delegate = new CustomFlowLayoutDelegate(this, collectionCate_Source);
            collection_attachment.ReloadData();
        }

        #region collection menu
        private class Collection_Source : UICollectionViewSource
        {
            ControlAttachment parentView { get; set; }
            public List<KeyValuePair<string, string>> lst_attachment { get; private set; }

            public Collection_Source(ControlAttachment _parentview, List<KeyValuePair<string, string>> _lst_attachment)
            {
                parentView = _parentview;
                lst_attachment = _lst_attachment;
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_attachment.Count;
            }

            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                CollectionCell cell = (CollectionCell)collectionView.DequeueReusableCell(CollectionCell.CellID, indexPath);
                cell.UpdateRow(lst_attachment[indexPath.Row], parentView);

                return cell;
            }
        }
        private class CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            KeyValuePair<string, string> attachment;
            UILabel fileName;
            UIImageView iv_type;
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            [Export("initWithFrame:")]
            public CollectionCell(RectangleF frame) : base(frame)
            {
                ContentView.BackgroundColor = UIColor.White;

                iv_type = new UIImageView();
                iv_type.Image = UIImage.FromFile("Icons/icon_attachment_pdf.png");
                iv_type.ContentMode = UIViewContentMode.ScaleAspectFit;

                fileName = new UILabel();
                fileName.Font = UIFont.SystemFontOfSize(11, UIFontWeight.Bold);
                fileName.LineBreakMode = UILineBreakMode.WordWrap;
                fileName.Lines = 2;
                fileName.TextAlignment = UITextAlignment.Center;

                ContentView.AddSubviews(iv_type, fileName);
            }

            public async void UpdateRow(KeyValuePair<string, string> _attachment, ControlAttachment _parentView)
            {
                try
                {
                    attachment = _attachment;
                    fileName.Text = attachment.Value;

                    string filepathURL = "https://png.pngtree.com/png-clipart/20200225/original/pngtree-happy-holi-colorful-splatter-color-splash-free-png-and-psd-png-image_5308640.jpg";
                    if (attachment.Key == "1" || attachment.Key == "3")//if (string.IsNullOrEmpty(filepathURL))
                        iv_type.Image = UIImage.FromFile("Icons/icon_attachment_pdf.png");
                    else
                    {
                        var result = await CmmIOSFunction.CheckFileLocalIsExist(filepathURL);
                        if (!string.IsNullOrEmpty(result))
                            CmmIOSFunction.openFile(result, "Icons/icon_attachment_pdf.png", iv_type);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("ControlAttachment - CollectionCell - UpdateRow - Err: " + ex.ToString());
#endif
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var width = ContentView.Frame.Width;
                var height = ContentView.Frame.Height;

                var padding = (width - 70) / 2;
                iv_type.Frame = new CGRect(padding, 0, 70, height - 40);
                fileName.Frame = new CGRect(0, height - 40, width, 30);
            }
        }
        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static ControlAttachment parentView;
            Collection_Source lst_cate;

            #region Constructors
            public CustomFlowLayoutDelegate(ControlAttachment _parent, Collection_Source _lst_cate)
            {
                lst_cate = _lst_cate;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize(collectionView.Bounds.Width/3, collectionView.Bounds.Height);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var itemSelected = lst_cate.lst_attachment[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }
        #endregion
    }
}