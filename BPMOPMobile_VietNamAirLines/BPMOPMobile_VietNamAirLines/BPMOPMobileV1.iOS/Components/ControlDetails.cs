
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
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
        public List<KeyValuePair<string, float>> lstColumnTextLength = new List<KeyValuePair<string, float>>();

        UICollectionView collectionview_header;
        UIButton btn_add;

        public ControlDetails(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            //test devop
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            lbl_title.Font = UIFont.BoldSystemFontOfSize(12);
            lbl_title.TextColor = UIColor.FromRGB(0, 0, 0);
            //this.BackgroundColor = UIColor.Red;
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
            collectionview_header.BackgroundColor = UIColor.FromRGB(245, 245, 245);
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
            btn_add.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
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

        private List<KeyValuePair<string, float>> GetListColumnTextLength(List<BeanWFDetailsHeader> _lstHeader, List<JObject> _lstJObjectRow)
        {
            List<KeyValuePair<string, float>> _lstResult = new List<KeyValuePair<string, float>>();
            try
            {
                foreach (BeanWFDetailsHeader _itemHeader in _lstHeader)
                {
                    bool getTitleHeader = true;
                    float _maxColumnLength = _itemHeader.Title.Length; // Lấy title length làm chuẩn
                    if (_lstJObjectRow != null && _lstJObjectRow.Count > 0)
                    {
                        float _maxObjectLength;
                        try
                        {
                            var value = _lstJObjectRow
                                               .Where(x => x.ContainsKey(_itemHeader.internalName))// get value
                                               .Max(x => (x[_itemHeader.internalName].ToString()));

                            if (_itemHeader.dataType == "selectusergroup" || _itemHeader.dataType == "selectusergroupmulti")
                            {
                                var data = value.Trim();
                                string result;
                                if (data.Contains(";#"))
                                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                else if (data.Contains("[{")) // cau truc json
                                {
                                    string str_value = string.Empty;
                                    List<BeanUserAndGroup> lst_user = new List<BeanUserAndGroup>();
                                    lst_user = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(data);

                                    if (lst_user != null && lst_user.Count > 0)
                                    {
                                        foreach (var item in lst_user)
                                        {
                                            str_value = str_value + item.Name + ", ";
                                        }
                                    }

                                    result = str_value.Trim().TrimEnd(',');
                                }
                                else if (data.Contains("[]"))
                                    result = string.Empty;
                                else
                                    result = data;
                                _maxObjectLength = result.Length;
                            }
                            else if (_itemHeader.dataType == "selectuser" || _itemHeader.dataType == "selectusermulti")
                            {
                                var data = value.Trim();
                                string result;
                                if (data.Contains(";#"))
                                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                else if (data.Contains("[{")) // cau truc json
                                {
                                    string str_value = string.Empty;
                                    List<BeanUser> lst_user = new List<BeanUser>();
                                    lst_user = JsonConvert.DeserializeObject<List<BeanUser>>(data);

                                    if (lst_user != null && lst_user.Count > 0)
                                    {
                                        foreach (var item in lst_user)
                                        {
                                            str_value = str_value + item.Name + ", ";
                                        }
                                    }

                                    result = str_value.Trim().TrimEnd(',');
                                }
                                else
                                    result = data;
                                _maxObjectLength = result.Length;
                            }
                            else if (_itemHeader.dataType == "multiplechoice")
                            {
                                //BeanDataLookup res = JsonConvert.DeserializeObject<List<BeanDataLookup>>(_data.DataValue.ToString()).FirstOrDefault();

                                var data = value.Trim();
                                string result;
                                if (data.Contains(";#"))
                                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                                else if (data.Contains("[{")) // cau truc json
                                {
                                    string str_value = string.Empty;
                                    List<BeanDataLookup> lst_user = new List<BeanDataLookup>();
                                    lst_user = JsonConvert.DeserializeObject<List<BeanDataLookup>>(data);

                                    if (lst_user != null && lst_user.Count > 0)
                                    {
                                        foreach (var item in lst_user)
                                        {
                                            str_value = str_value + item.Title + ", ";
                                        }
                                    }

                                    result = str_value.TrimEnd(',', ' ');
                                }
                                else if (data.Contains("[]"))
                                    result = string.Empty;
                                else
                                    result = data;

                                //this.lbl_value.Text = result;
                                _maxObjectLength = result.Length;
                            }
                            else
                            {
                                _maxObjectLength = _lstJObjectRow
                                               .Where(x => x.ContainsKey(_itemHeader.internalName))// loại mấy thằng null ra
                                               .Max(x => (x[_itemHeader.internalName].ToString().Length));
                            }
                        }
                        catch (Exception)
                        {
                            _maxObjectLength = 0;
                        }

                        if (_maxObjectLength > _maxColumnLength)
                        {
                            _maxColumnLength = _maxObjectLength;
                            if (_maxColumnLength > 30) // limit lại nếu quá dài
                                _maxColumnLength = 30;
                            else if (_maxColumnLength == 0) // limit lại nếu quá dài
                                _maxColumnLength = 3;
                            getTitleHeader = true;
                        }
                    }
                    if (_itemHeader.require)
                        _maxColumnLength += 3; // " (*)"

                    string[] values = Enumerable.Repeat("a", (int)_maxColumnLength).ToArray();
                    string content = string.Join("", values);
                    if (getTitleHeader)
                    {
                        var cGSize = StringExtensions.MeasureString(content, 14);
                        _itemHeader.EstWidth = (float)cGSize.Width + 20;
                    }
                    else
                    {
                        var cGSize = StringExtensions.MeasureString(content, 16);
                        _itemHeader.EstWidth = (float)cGSize.Width + 20;
                    }
                }

                CheckTotalWidth(_lstHeader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListColumnTextLength - Err: " + ex.ToString());
            }
            return _lstResult;
        }

        /// <summary>
        /// Nếu tổng chiều rộng hiển thị nhỏ hơn chiều rộng màn hình thiết bị thì tính lại với công thức: chiều rộng hiển thị / tổng số cột).
        /// </summary>
        void CheckTotalWidth(List<BeanWFDetailsHeader> _lstHeader)
        {
            float totalWidth = 0;
            foreach (var header in _lstHeader)
            {
                totalWidth += header.EstWidth;
            }
            var dislayingWidth = UIScreen.MainScreen.Bounds.Width - 40;// trừ leading và trailing constraint value = 15*2 + 5*2
            if (totalWidth <= dislayingWidth)
            {
                var reCalculatedWidth = dislayingWidth / _lstHeader.Count();
                _lstHeader.ForEach(o => o.EstWidth = (float)reCalculatedWidth);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;
            scrollView_content.Frame = new CGRect(0, 30, Frame.Width, frame.Height);
            nfloat collection_width = 0;

            float headerCollection_est_widht = 0;
            foreach (var item in lst_titleHeader)
            {
                headerCollection_est_widht += item.EstWidth;// item.EstWidth;
            }
            headerCollection_est_widht += 20;
            collectionview_header.Frame = new CGRect(0, 0, headerCollection_est_widht, 50);
            tableViewDetails.Frame = new CGRect(0, collectionview_header.Frame.Bottom, headerCollection_est_widht, (lst_jobject.Count * 50));
            scrollView_content.ContentSize = new CGSize(headerCollection_est_widht, tableViewDetails.Frame.Bottom);

            const int paddingTop = 2;
            btn_add.Frame = new CGRect(Frame.Width - 120, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, btn_add, NSLayoutAttribute.Left, 1.0f, 5.0f).Active = true;

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 22, 22);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0);
            btn_add.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            ////bo goc
            //scrollView_content.ClipsToBounds = true;
            //scrollView_content.Layer.CornerRadius = 6;
            //scrollView_content.Layer.BorderColor = UIColor.FromRGB(245, 245, 245).CGColor;
            //scrollView_content.Layer.BorderWidth = 0.8f;
            //bo goc cho view header
            collectionview_header.ClipsToBounds = true;
            UIBezierPath mPath_view_header = UIBezierPath.FromRoundedRect(collectionview_header.Layer.Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_view_header = new CAShapeLayer();
            maskLayer_view_header.Frame = collectionview_header.Layer.Bounds;
            maskLayer_view_header.Path = mPath_view_header.CGPath;
            collectionview_header.Layer.Mask = maskLayer_view_header;

            // bo goc cho  tableView_attachment
            tableViewDetails.ClipsToBounds = true;
            UIBezierPath mPath_tableView_attachment = UIBezierPath.FromRoundedRect(tableViewDetails.Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 6, height: 6));
            //UIBezierPath mPath_tableView_attachment = centerStartBezierPath(tableView_attachment.Layer.Bounds, 6.0f);
            CAShapeLayer maskLayer_tableView_attachment = new CAShapeLayer();
            maskLayer_tableView_attachment.Frame = tableViewDetails.Layer.Bounds;
            maskLayer_tableView_attachment.Path = mPath_tableView_attachment.CGPath;
            tableViewDetails.Layer.Mask = maskLayer_tableView_attachment;
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

            var data_source = element.DataSource.Trim();
            var data_value = element.Value.Trim();

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
            //danh sach column header
            lst_titleHeader = new List<BeanWFDetailsHeader>();
            if (!string.IsNullOrEmpty(data_source) && data_source != "[]")
            {
                JArray json = JArray.Parse(data_source);
                lst_titleHeader = json.ToObject<List<BeanWFDetailsHeader>>();
                lst_titleHeader = lst_titleHeader.Where(x => x.dataType != null).ToList();

                //insert collumn mis in lst_jobject from lst_titleHeader
                foreach (var header in lst_titleHeader)
                {
                    for (int i = 0; i < lst_jobject.Count; i++)
                    {

                        if (!lst_jobject[i].ContainsKey(header.internalName))
                        {
                            lst_jobject[i].Add(new JProperty(header.internalName, ""));
                        }
                    }
                }

                lstColumnTextLength = GetListColumnTextLength(lst_titleHeader, lst_jobject); // List Length

                detailHeader_Source = new CollectionDetailHeader_Source(this, lst_titleHeader);
                collectionview_header.Source = detailHeader_Source;
                collectionview_header.ScrollEnabled = false;
                collectionview_header.Delegate = new CustomFlowLayoutDelegate(this, detailHeader_Source, collectionview_header);


                var flowLayout = collectionview_header.CollectionViewLayout as UICollectionViewFlowLayout;
                if ((flowLayout) != null)
                {
                    flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
                }

                collectionview_header.ReloadData();
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

            tableViewDetails.Source = new values_TableSource(lst_jobject, lst_sumValueColumn, this);
            tableViewDetails.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableViewDetails.ReloadData();
        }

        public void HandleRemoveItem(BeanWFDetailsValue beanWFDetailsValue)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                List<BeanWFDetailsValue> lst_attach_remove = new List<BeanWFDetailsValue>();
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
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

                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    //controller.HandleAttachmentRemove(element, indexPath, this, lst_value.Count, json_attachRemove);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
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

            foreach (var header in lst_titleHeader)
            {
                bool flagAlive = false;
                foreach (var item in _object)
                {
                    if (item.Key == header.internalName)
                    {
                        if (item.Value.GetType() != typeof(JArray))
                        {
                            ViewElement _element = new ViewElement();
                            _element.InternalName = item.Key;
                            _element.Title = header.Title;
                            _element.Value = item.Value.ToString();
                            _element.DataType = header.dataType;
                            //_element.Enable = header.viewOnly;
                            _element.Enable = this.element.Enable;

                            _element.DataSource = header.DataSource;

                            if (header.isSum)
                            {
                                _element.Enable = false;
                                _element.Formula = "formula";
                            }
                            if (header.require)
                                _element.IsRequire = true;

                            ////for test
                            //element.Enable = true;
                            viewRow.Elements.Add(_element);
                            flagAlive = true;
                        }
                    }
                }
                if (!flagAlive)
                {
                    ViewElement _element = new ViewElement();
                    _element.InternalName = header.internalName;
                    _element.Title = header.Title;
                    //if (CmmVariable.SysConfig.LangCode == "1066")
                    //    element.Title = header.Title;
                    //else
                    //    element.Title = header.TitleEN;
                    _element.Value = "";
                    _element.DataType = header.dataType;
                    //_element.Enable = header.viewOnly;
                    _element.Enable = this.element.Enable;
                    _element.DataSource = header.DataSource;

                    if (header.isSum)
                    {
                        _element.Enable = false;
                        _element.Formula = "formula";
                    }
                    if (header.require)
                        _element.IsRequire = true;
                    //for test
                    //element.Enable = true;
                    viewRow.Elements.Add(_element);
                }
            }

            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                /*foreach (var header in lst_titleHeader)
                {
                    //if(header.internalName == _object.)
                    //ViewElement viewElement = new ViewElement();
                    //viewElement.internalName = header.internalName;
                }*/

                RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                requestDetailsV2.NavigateToPropertyDetails(element, viewRow, _object, itemIndex, false);
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
                //element.Enable = header.viewOnly;
                element.Enable = this.element.Enable;
                element.DataSource = header.DataSource;
                //element.Enable = true;

                if (header.isSum)
                {
                    element.Enable = true;
                    element.Formula = header.formula;
                }
                if (header.require)
                    element.IsRequire = true;

                viewRow.Elements.Add(element);
            }

            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.NavigateToPropertyDetails(element, viewRow, null, itemIndex, true);
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
            int tag = 0;

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
                return cell;
            }
        }
        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static ControlDetails parentView;
            CollectionDetailHeader_Source collect_source;
            UICollectionView CollectionView;
            List<KeyValuePair<string, float>> lstColumnTextLength = new List<KeyValuePair<string, float>>();

            #region Constructors


            public CustomFlowLayoutDelegate(ControlDetails _parent, CollectionDetailHeader_Source _collect_source, UICollectionView collectionView)
            {
                collect_source = _collect_source;
                CollectionView = collectionView;
                parentView = _parent;
                lstColumnTextLength = parentView.lstColumnTextLength;

            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                var res = parentView.lst_titleHeader[indexPath.Row];
                return new CGSize((float)res.EstWidth, 50);
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
                    Font = UIFont.FromName("Arial-BoldMT", 11),
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
                lbl_title.Frame = new CGRect(10, 0, ContentView.Frame.Width - 10, ContentView.Frame.Height);
            }
        }
        #endregion

        #region table value
        private class values_TableSource : UITableViewSource
        {
            List<JObject> lst_rowValue { get; set; }
            List<KeyValuePair<string, double>> lst_sumValueColumn;
            NSString cellIdentifier = new NSString("cell");
            ControlDetails parentView;
            bool isRowSum;
            ViewRow viewRow = new ViewRow();

            public values_TableSource(List<JObject> _lst_rowValue, List<KeyValuePair<string, double>> _lst_sumValueColumn, ControlDetails _parentview)
            {
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
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 != 0)
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