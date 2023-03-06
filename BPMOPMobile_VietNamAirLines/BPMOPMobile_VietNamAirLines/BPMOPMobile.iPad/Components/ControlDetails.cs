using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlDetails : ControlBase
    {
        UIViewController parentView { get; set; }
        UIScrollView scrollView_content;
        NSIndexPath indexPath { get; set; }
        ViewRow viewRow = new ViewRow();
        ViewElement element { get; set; }
        List<BeanWFDetailsValue> lst_value = new List<BeanWFDetailsValue>();
        List<JObject> lst_jobject;
        UITableView tableViewDetails;
        public List<BeanWFDetailsHeader> lst_titleHeader;
        List<KeyValuePair<string, bool>> lst_sectionState;
        CollectionDetailHeader_Source detailHeader_Source;
        List<KeyValuePair<string, double>> lst_sumValueColumn = new List<KeyValuePair<string, double>>();
        int itemIndex = 0;

        UICollectionView collectionview_header;
        UIButton btn_add;

        public ControlDetails(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            this.BackgroundColor = UIColor.Red;
            scrollView_content = new UIScrollView();
            scrollView_content.ClipsToBounds = true;
            scrollView_content.BackgroundColor = UIColor.White;
            scrollView_content.DirectionalLockEnabled = true;
            var flowLayout = new UICollectionViewFlowLayout()
            {
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 5, // minimum spacing between cells
                MinimumLineSpacing = 0 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };

            CGRect frame = CGRect.Empty;
            collectionview_header = new UICollectionView(frame: frame, layout: flowLayout);
            collectionview_header.BackgroundColor = UIColor.FromRGB(229, 229, 229);
            collectionview_header.SetCollectionViewLayout(flowLayout, true);
            collectionview_header.RegisterClassForCell(typeof(DetailsHeader_CollectionCell), DetailsHeader_CollectionCell.CellID);
            collectionview_header.AllowsMultipleSelection = false;

            scrollView_content.AddSubview(collectionview_header);

            tableViewDetails = new UITableView();
            tableViewDetails.ScrollEnabled = false;
            scrollView_content.AddSubview(tableViewDetails);

            btn_add = new UIButton();
            btn_add.SetTitle(CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"), UIControlState.Normal);
            btn_add.SetImage(UIImage.FromFile("Icons/icon_document_add.png"), UIControlState.Normal);
            btn_add.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            btn_add.Font = UIFont.FromName("ArialMT", 12);

            if (element.Enable)
                btn_add.Hidden = false;
            else
                btn_add.Hidden = true;

            this.ClipsToBounds = true;
            this.BackgroundColor = UIColor.White;
            this.Add(scrollView_content);
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
            scrollView_content.Frame = new CGRect(0, 30, Frame.Width, frame.Height);
            float headerCollection_est_widht = 0;
            //var collection_width = lst_titleHeader.Count * 150;
            ///Tạm đóng
            /*foreach (var item in lst_titleHeader)
            {
                string content = "";
                if (item.require)
                    content = item.Title + " (*)";
                else
                    content = item.Title;

                var cGSize = StringExtensions.MeasureString(content, 18);
                item.EstWidth = (float)cGSize.Width;

                headerCollection_est_widht = headerCollection_est_widht + item.EstWidth;
            }*/

            // sau nay wweb tra ve width tam thoi moi item 200
            headerCollection_est_widht = 200 * lst_titleHeader.Count;

            if (headerCollection_est_widht < this.Frame.Width)
                collectionview_header.Frame = new CGRect(0, 0, this.Frame.Width, 40);
            else
                collectionview_header.Frame = new CGRect(0, 0, headerCollection_est_widht, 40);

            if (lst_sumValueColumn.Count > 0)
                tableViewDetails.Frame = new CGRect(0, collectionview_header.Frame.Bottom, collectionview_header.Frame.Width, (lst_jobject.Count * 50));
            else
                tableViewDetails.Frame = new CGRect(0, collectionview_header.Frame.Bottom, collectionview_header.Frame.Width, lst_jobject.Count * 50);

            scrollView_content.ContentSize = new CGSize(collectionview_header.Frame.Width, tableViewDetails.Frame.Bottom);

            const int paddingTop = 2;
            const int spaceView = 5;
            var width = Frame.Width;
            btn_add.Frame = new CGRect(Frame.Width - 110, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 22, 22);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0);
            btn_add.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override void SetEnable()
        {
            base.SetEnable();

            //cho phep enable vi truong hop can xem full noi dung van touch vao dc
            BT_action.UserInteractionEnabled = true;
        }

        public override void SetValue()
        {
            base.SetValue();

            try
            {
                var data_source = element.DataSource.Trim();
                var data_value = element.Value.Trim();

                //danh sach column header
                lst_titleHeader = new List<BeanWFDetailsHeader>();
                if (!string.IsNullOrEmpty(data_source) && data_source != "[]")
                {
                    JArray json = JArray.Parse(data_source);
                    lst_titleHeader = json.ToObject<List<BeanWFDetailsHeader>>();

                    lst_titleHeader = lst_titleHeader.Where(x => x.dataType != null).ToList();

                    detailHeader_Source = new CollectionDetailHeader_Source(this, lst_titleHeader);
                    collectionview_header.Source = detailHeader_Source;
                    collectionview_header.ScrollEnabled = false;
                    collectionview_header.Delegate = new CustomFlowLayoutDelegate(this, detailHeader_Source, collectionview_header);
                    collectionview_header.ReloadData();
                }

                //danh sach value
                JArray rowListItem = new JArray();
                lst_jobject = new List<JObject>();
                if (!string.IsNullOrEmpty(data_value))
                {
                    rowListItem = JArray.Parse(data_value);

                    foreach (JObject row in rowListItem)
                    {
                        lst_jobject.Add(row);
                    }
                }

                //calculator sum
                foreach (var item in lst_titleHeader)
                {
                    if (item.isSum)
                    {
                        foreach (var ob in lst_jobject)
                        {
                            if (ob.ContainsKey(item.internalName))
                            {
                                double value = rowListItem.Sum(x => ((double)x[item.internalName]));
                                lst_sumValueColumn.Add(new KeyValuePair<string, double>(item.internalName, value));
                                break;
                            }
                        }
                    }
                }

                tableViewDetails.Source = new values_TableSource(lst_titleHeader, lst_jobject, lst_sumValueColumn, this);
                tableViewDetails.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                tableViewDetails.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ControlDetails - Err: " + ex.ToString());
            }
        }

        public void HandleRemoveItem(BeanWFDetailsValue beanWFDetailsValue)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                List<BeanWFDetailsValue> lst_attach_remove = new List<BeanWFDetailsValue>();
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    var index = lst_value.FindIndex(item => item.ID == beanWFDetailsValue.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_value[index]);
                        lst_value.RemoveAt(index);
                    }

                    var json_attachRemove = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_value);
                    element.Value = jsonString;

                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.HandleAttachmentRemove(element, indexPath, this, lst_value.Count, json_attachRemove);
                }
                else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
                {
                    var index = lst_value.FindIndex(item => item.ID == beanWFDetailsValue.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_value[index]);
                        lst_value.RemoveAt(index);
                    }

                    var json_attachRemove = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_value);
                    element.Value = jsonString;

                    FollowListViewController controller = (FollowListViewController)parentView;
                    controller.HandleAttachmentRemove(element, indexPath, this, lst_value.Count, json_attachRemove);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
        }

        public void HandleSeclectItem(JObject _object, int index)
        {
            itemIndex = index;
            viewRow = new ViewRow();
            viewRow.RowType = 1;
            viewRow.Enable = true;
            viewRow.Elements = new List<ViewElement>();

            foreach (var item in _object)
            {
                if (item.Value.GetType() != typeof(JArray))
                {
                    foreach (var header in lst_titleHeader)
                    {
                        if (item.Key == header.internalName)
                        {
                            ViewElement element = new ViewElement();
                            element.InternalName = header.internalName;//item.Key
                            element.Title = header.Title;//item.Key
                            element.Value = item.Value.ToString();
                            element.DataType = header.dataType;
                            //element.Enable = header.viewOnly;
                            //element.DataSource = header.DataSource;
                            //element.IsRequire = header.require;
                            element.Enable = this.element.Enable;
                            element.DataSource = header.DataSource;
                            //element.Enable = true;

                            if (header.isSum)
                            {
                                element.Enable = false;
                                element.Formula = "formula";
                            }

                            viewRow.Elements.Add(element);
                        }
                    }
                }
            }

            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                /*foreach (var header in lst_titleHeader)
                {
                    //if(header.internalName == _object.)
                    //ViewElement viewElement = new ViewElement();
                    //viewElement.internalName = header.internalName;
                }*/

                ToDoDetailView requestDetailsV2 = (ToDoDetailView)parentView;
                requestDetailsV2.NavigateToPropertyDetails(element, viewRow, _object, itemIndex, false);
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetail = (WorkflowDetailView)parentView;
                workflowDetail.NavigateToPropertyDetails(element, viewRow, _object, itemIndex, false);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails workflowDetail = (FormWorkFlowDetails)parentView;
                workflowDetail.NavigateToPropertyDetails(element, viewRow, _object, itemIndex, false);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = (FollowListViewController)parentView;
                followListViewController.NavigateToPropertyDetails(element, viewRow, _object, itemIndex, false);
            }
        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            viewRow = new ViewRow();
            viewRow.RowType = 1;
            viewRow.Enable = true;
            viewRow.Elements = new List<ViewElement>();

            //ViewElement element = new ViewElement();

            foreach (var header in lst_titleHeader)
            {
                ViewElement element = new ViewElement();
                element.InternalName = header.internalName;
                element.Title = header.Title;
                element.Value = "";
                element.DataType = header.dataType;
                element.Enable = header.viewOnly;
                element.DataSource = header.DataSource;
                element.IsRequire = header.require;

                if (header.isSum)
                {
                    element.Enable = true;
                    element.Formula = header.formula;
                }

                viewRow.Elements.Add(element);
            }

            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.NavigateToPropertyDetails(element, viewRow, null, itemIndex, true);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDoDetail = parentView as ToDoDetailView;
                toDoDetail.NavigateToPropertyDetails(element, viewRow, null, itemIndex, true);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                formWorkFlowDetails.NavigateToPropertyDetails(element, viewRow, null, itemIndex, true);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.NavigateToPropertyDetails(element, viewRow, null, itemIndex, true);
            }
        }

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            //tableViewDetails.Source = new Attachment_TableSource(lst_attachment, this, lst_sectionState);
            tableViewDetails.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.None);
        }

        #region custom class
        #region collection header
        public class CollectionDetailHeader_Source : UICollectionViewSource
        {
            ControlDetails parentView { get; set; }
            public List<BeanWFDetailsHeader> lst_title;

            public CollectionDetailHeader_Source(ControlDetails _parentview, List<BeanWFDetailsHeader> _lst_title)
            {
                parentView = _parentview;
                lst_title = _lst_title;
                LoadData();
            }

            public void LoadData()
            {
                lst_title = lst_title.Where(i => i.internalName != "ID").ToList();
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_title.Count;
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
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                BeanWFDetailsHeader item = lst_title[indexPath.Row];
                var cell = (DetailsHeader_CollectionCell)collectionView.DequeueReusableCell(DetailsHeader_CollectionCell.CellID, indexPath);
                cell.UpdateRow(item, parentView, indexPath, isOdd);
                //if (isOdd)
                //    cell.BackgroundColor = UIColor.Red;
                //else
                //    cell.BackgroundColor = UIColor.Green;

                return cell;
            }
        }
        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static ControlDetails parentView;
            CollectionDetailHeader_Source collect_source;
            UICollectionView CollectionView;

            #region Constructors
            public CustomFlowLayoutDelegate(ControlDetails _parent, CollectionDetailHeader_Source _collect_source, UICollectionView collectionView)
            {
                collect_source = _collect_source;
                CollectionView = collectionView;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                /*var itemSelected = collect_source.lst_title[indexPath.Row];
                string content = "";
                if (itemSelected.require)
                    content = itemSelected.Title + " (*)";
                else
                    content = itemSelected.Title;

                var cGSize = StringExtensions.MeasureString(content, 18);
               
                return new CGSize(cGSize.Width + 20, 30);
                */
                return new CGSize(200, 30);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //var itemSelected = collect_source.lst_workFlow[indexPath.Row];
                //parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }
        public class DetailsHeader_CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            BeanWFDetailsHeader beanWFDetailsHeader;
            UILabel lbl_title;
            ControlDetails controller { get; set; }

            [Export("initWithFrame:")]
            public DetailsHeader_CollectionCell(RectangleF frame) : base(frame)
            {
                ViewConfiguration();
            }

            private void ViewConfiguration()
            {
                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("Arial-BoldMT", 14f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                    TextAlignment = UITextAlignment.Left,
                };

                ContentView.AddSubviews(lbl_title);
            }

            public void UpdateRow(BeanWFDetailsHeader _beanWFDetailsHeader, ControlDetails _controller, NSIndexPath indexPath, bool _isodd)
            {
                //if (_isodd)
                //    ContentView.BackgroundColor = UIColor.Yellow;
                //else
                ContentView.BackgroundColor = UIColor.Clear;

                beanWFDetailsHeader = _beanWFDetailsHeader;

                controller = _controller;

                if (beanWFDetailsHeader.require)
                {
                    lbl_title.Text = beanWFDetailsHeader.Title + " (*)";

                    var str_transalte = lbl_title.Text;
                    var indexA = str_transalte.IndexOf('*');
                    NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                    //att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Red, new NSRange(indexA, 1));
                    lbl_title.AttributedText = att;
                }
                else
                    lbl_title.Text = beanWFDetailsHeader.Title;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(10, 0, ContentView.Frame.Width - 20, 30);
            }
        }
        #endregion

        #region table value
        private class values_TableSource : UITableViewSource
        {
            List<BeanWFDetailsHeader> lst_titleHeader;
            List<JObject> lst_rowValue { get; set; }
            List<KeyValuePair<string, double>> lst_sumValueColumn;
            NSString cellIdentifier = new NSString("cell");
            ControlDetails parentView;
            bool isRowSum;
            ViewRow viewRow = new ViewRow();

            public values_TableSource(List<BeanWFDetailsHeader> _lst_titleHeader, List<JObject> _lst_rowValue, List<KeyValuePair<string, double>> _lst_sumValueColumn, ControlDetails _parentview)
            {
                lst_titleHeader = _lst_titleHeader;
                parentView = _parentview;
                lst_sumValueColumn = _lst_sumValueColumn;
                if (_lst_rowValue != null)
                    lst_rowValue = _lst_rowValue;

                if (lst_sumValueColumn.Count > 0)
                    lst_rowValue.Add(new JObject(new JProperty(lst_sumValueColumn[0].Key, lst_sumValueColumn[0].Value)));
            }
            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_rowValue.Count;
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 50;
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var rowvalue = lst_rowValue[indexPath.Row];
                parentView.HandleSeclectItem(rowvalue, indexPath.Row);
                tableView.DeselectRow(indexPath, false);
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                if (lst_sumValueColumn.Count > 0 && indexPath.Row == lst_rowValue.Count - 1)
                {
                    isRowSum = true;
                }
                else
                    isRowSum = false;

                Custom_ValueDetailsCell cell = new Custom_ValueDetailsCell(cellIdentifier);
                var rowvalue = lst_rowValue[indexPath.Row];

                cell.UpdateCell(parentView.lst_titleHeader, rowvalue, lst_sumValueColumn, isOdd, indexPath, isRowSum);
                return cell;
            }

            #endregion
            #endregion
        }
    }
}