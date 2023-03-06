using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    public class ControlInputAttachmentHorizon : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }
        CollectionAttachment_Source collectionAttachment_Source;
        UICollectionView collection_attachment;
        List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
        public BeanAttachFile currentAttachment { get; set; }

        UIView view_header;
        UILabel lbl_type, lbl_name;
        UIButton btn_add;

        public ControlInputAttachmentHorizon(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            btn_add = new UIButton();
            btn_add.SetTitle("Tạo mới", UIControlState.Normal);
            btn_add.SetImage(UIImage.FromFile("Icons/icon_document_add.png"), UIControlState.Normal);
            btn_add.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            btn_add.Font = UIFont.SystemFontOfSize(14);

            var flowLayout = new UICollectionViewFlowLayout()
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 0, // minimum spacing between cells
                MinimumLineSpacing = 0 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };

            collection_attachment = new UICollectionView(new CGRect(), flowLayout);
            collection_attachment.BackgroundColor = UIColor.White;

            //Collection_ticket.RegisterClassForSupplementaryView(typeof(Custom_CollectionHeader), UICollectionElementKindSection.Header, Custom_CollectionHeader.Key);
            this.Add(collection_attachment);
            this.Add(btn_add);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            if (btn_add != null)
                btn_add.AddTarget(HandleBtnAdd, UIControlEvent.TouchUpInside);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;
            var width = Frame.Width;
            const int paddingLeft = 25;
            var widthlblHeader = width - 30;

            btn_add.Frame = new CGRect(Frame.Width - 100, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, btn_add.Frame.Width).Active = true;

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 20, 20);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: btn_add.ImageView.Frame.Width / 2);
            btn_add.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            collection_attachment.Frame = new CGRect(0, btn_add.Frame.Bottom + 5, parentView.View.Frame.Width, frame.Height - (btn_add.Frame.Bottom + 5));

        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                //string custID = lst_attachment.Count + 1 + "";
                //BeanAttachFile attachmentEmpty = new BeanAttachFile() { ID = custID };
                //lst_attachment.Add(attachmentEmpty);

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                //element.Value = jsonString;

                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.HandleAddAttachment(element, indexPath, this);
            }
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                currentAttachment = _attachment;
                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.NavigatorToView(element, indexPath, this);
            }
        }

        public void HandleRemoveItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                var index = lst_attachment.FindIndex(item => item.ID == _attachment.ID);
                if (index != -1)
                    lst_attachment.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                element.Value = jsonString;

                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.HandleRemoveAttachment(element, indexPath, this, lst_attachment.Count);
            }
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
            //data = "[{\"ID\":\"1\",\"Path\":\"/var/mobile/Containers/Data/Application/66CA2A6B-A6E8-4E6F-A229-608B8BDD23F0/Documents/data/82952346-8e6f-45d5-81a9-07efc37a91d6.xlsx\",\"Title\":\"82952346-8e6f-45d5-81a9-07efc37a91d6.xlsx\",\"Type\":null,\"Size\":\"19.0 KB\"},{\"ID\":\"2\",\"Path\":\"/var/mobile/Containers/Data/Application/66CA2A6B-A6E8-4E6F-A229-608B8BDD23F0/Documents/data/902c36f4-7e5e-4522-a045-f6d46172f4f2.pdf\",\"Title\":\"902c36f4-7e5e-4522-a045-f6d46172f4f2.pdf\",\"Type\":null,\"Size\":\"335.0 KB\"},{\"ID\":\"3\",\"Path\":\"/var/mobile/Containers/Data/Application/66CA2A6B-A6E8-4E6F-A229-608B8BDD23F0/Documents/data/9452b690-9d79-4a84-8210-873eaf624312.docx\",\"Title\":\"9452b690-9d79-4a84-8210-873eaf624312.docx\",\"Type\":null,\"Size\":\"24.6 KB\"}]";
            if (!string.IsNullOrEmpty(data))
            {
                JArray json = JArray.Parse(data);
                lst_attachment = json.ToObject<List<BeanAttachFile>>();
            }

            collection_attachment.RegisterClassForCell(typeof(Attach_CollectionCell), Attach_CollectionCell.CellID);
            collectionAttachment_Source = new CollectionAttachment_Source(this, lst_attachment);
            collection_attachment.Source = collectionAttachment_Source;
            collection_attachment.Delegate = new CollectionAttachmentFlowLayoutDelegate(collectionAttachment_Source, this);
            collection_attachment.ReloadData();
        }

        public override string Value { get => element.Value; set => base.Value = value; }

        #region custom views
        #region collection attachments
        public class CollectionAttachment_Source : UICollectionViewSource
        {
            ControlInputAttachmentHorizon parentView { get; set; }
            public List<BeanAttachFile> lst_items;
            public CollectionAttachment_Source(ControlInputAttachmentHorizon _parentview, List<BeanAttachFile> _items)
            {
                parentView = _parentview;
                lst_items = _items;
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_items.Count;
            }
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //parentView.NavigateToViewByCate(items[indexPath.Row], indexPath.Row);
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                BeanAttachFile attach = lst_items[indexPath.Row];
                var cell = (Attach_CollectionCell)collectionView.DequeueReusableCell(Attach_CollectionCell.CellID, indexPath);
                cell.UpdateRow(attach);
                return cell;
            }
        }
        private class Attach_CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("AttachCellID");
            UIView bg_view = new UIView();
            UIImageView img;
            UIButton BT_close;
            UILabel lbl_title;

            [Export("initWithFrame:")]
            public Attach_CollectionCell(RectangleF frame) : base(frame)
            {
                ViewConfiguration();
            }
            private void ViewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(11, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Center,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 2
                };

                BT_close = new UIButton();
                BT_close.SetImage(UIImage.FromFile("Icons/icon_close.png"), UIControlState.Normal);
                BT_close.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
                BT_close.Layer.BorderWidth = 0.2f;
                BT_close.Layer.ShadowOffset = new CGSize(1, 1);
                BT_close.Layer.ShadowRadius = 1;
                BT_close.Layer.ShadowOpacity = 0.3f;
                BT_close.Layer.CornerRadius = 10;

                img = new UIImageView();
                img.ContentMode = UIViewContentMode.ScaleToFill;
                img.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
                img.Layer.BorderWidth = 0.2f;
                img.Layer.ShadowOffset = new CGSize(1, 1);
                img.Layer.ShadowRadius = 1;
                img.Layer.ShadowOpacity = 0.3f;
                img.Layer.CornerRadius = 10;

                ContentView.AddSubviews(img, lbl_title, BT_close);
            }
            public void UpdateRow(BeanAttachFile attachment)
            {
                lbl_title.Text = attachment.Title;

                var index = attachment.Path.LastIndexOf('.');
                var filetype = attachment.Path.Substring((index + 1), attachment.Path.Length - (index + 1));


                switch (filetype.ToLower())
                {
                    case "doc":
                    case "docx":
                        img.Image = UIImage.FromFile("Icons/thumb_word.png");
                        break;
                    case "xls":
                    case "xlsx":
                        img.Image = UIImage.FromFile("Icons/thumb_excel.png");
                        break;
                    case "pdf":
                        img.Image = UIImage.FromFile("Icons/thumb_pdf.png");
                        break;
                    case "png":
                    case "jpeg":
                    case "jpg":
                        img.Image = UIImage.FromFile("Icons/thumb_img.png");
                        break;
                    case "ppt":
                    case "pptx":
                    case "ppsx":
                        img.Image = UIImage.FromFile("Icons/thumb_ppt.png");
                        break;
                    default:
                        img.Image = UIImage.FromFile("Icons/icon_attachFile_other.png");
                        break;
                }

            }
            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                img.Frame = new CGRect((ContentView.Frame.Width - 70) / 2, 0, 70, 104);
                BT_close.Frame = new CGRect(img.Frame.Right - 25, img.Frame.Y + 5, 20, 20);
                lbl_title.Frame = new CGRect(10, img.Frame.Bottom + 5, ContentView.Frame.Width - 20, 30);
            }
        }
        private class CollectionAttachmentFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static ControlInputAttachmentHorizon parent;
            CollectionAttachment_Source lst_items;
            nfloat titleWidth = 0, height = 0;
            BeanAttachFile beanAttach { get; set; }
            bool isSubcate = false;

            #region Constructors
            public CollectionAttachmentFlowLayoutDelegate(CollectionAttachment_Source _lst_items, ControlInputAttachmentHorizon _parent)
            {
                lst_items = _lst_items;
                parent = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize((float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f, collectionView.Frame.Height);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                parent.HandleSeclectItem(lst_items.lst_items[indexPath.Row]);
            }

            #endregion
        }

        #endregion
        #endregion
    }
}
