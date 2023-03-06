using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_ValueDetailsCell : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        JObject value { get; set; }
        UICollectionView collectionRow;
        private bool isOdd;
        bool isRowSum;
        int rowIndex;
        public List<KeyValuePair<string, double>> lst_sumValueColumn;
        public List<BeanWFDetailsHeader> lst_beanWFDetailsHeaders { get; set; }

        //NSLayoutConstraint constraintRightIconAttachment;

        public Custom_ValueDetailsCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
        }
        public Custom_ValueDetailsCell(IntPtr handle) : base(handle)
        {
        }
        public Custom_ValueDetailsCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(List<BeanWFDetailsHeader> _lst_beanWFDetailsHeaders, JObject _value, List<KeyValuePair<string, double>> _lst_sumValueColumn, bool _isOdd, NSIndexPath _rowIndex, bool _isRowSum)
        {
            lst_sumValueColumn = _lst_sumValueColumn;
            lst_beanWFDetailsHeaders = _lst_beanWFDetailsHeaders;
            isRowSum = _isRowSum;
            value = _value;
            isOdd = _isOdd;
            rowIndex = _rowIndex.Row;
            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            if (isOdd)
                ContentView.BackgroundColor = UIColor.White;
            else
                ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

            var flowLayout = new UICollectionViewFlowLayout()
            {
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 5, // minimum spacing between cells
                MinimumLineSpacing = 0 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };

            CGRect frame = CGRect.Empty;
            collectionRow = new UICollectionView(frame: frame, layout: flowLayout);
            collectionRow.BackgroundColor = UIColor.Clear;
            collectionRow.SetCollectionViewLayout(flowLayout, true);
            collectionRow.RegisterClassForCell(typeof(CollectionValueRow_Cell), CollectionValueRow_Cell.CellID);
            collectionRow.AllowsMultipleSelection = false;
            collectionRow.UserInteractionEnabled = false;
            collectionRow.ScrollEnabled = false;

            ContentView.AddSubview(collectionRow);
        }

        private void LoadData()
        {
            try
            {
                CollectionValueRow_Source collectionCate_Source = new CollectionValueRow_Source(this, value, rowIndex, isRowSum, lst_sumValueColumn);
                collectionRow.Source = collectionCate_Source;
                collectionRow.Delegate = new CustomFlowLayoutDelegate(this, collectionCate_Source);
                collectionRow.ReloadData();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Custom_GroupWorkFlowCell - LoadData - Err: " + ex.ToString());
#endif
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            collectionRow.Frame = new CGRect(0, 0, ContentView.Frame.Width, 50);
        }

        #region collection value cell
        private class CollectionValueRow_Source : UICollectionViewSource
        {
            Custom_ValueDetailsCell parentView { get; set; }
            public JObject lst_value { get; set; }
            public List<BeanWFDetailLookup> lst_data;
            List<KeyValuePair<string, double>> lst_sumValueColumn;
            bool isRowSum;
            int rowIndex;

            public CollectionValueRow_Source(Custom_ValueDetailsCell _parentview, JObject _value, int _rowIndex, bool _isRowSum, List<KeyValuePair<string, double>> _lst_sumValueColumn)
            {
                try
                {
                    parentView = _parentview;
                    lst_data = new List<BeanWFDetailLookup>();
                    rowIndex = _rowIndex;
                    isRowSum = _isRowSum;
                    lst_sumValueColumn = _lst_sumValueColumn;

                    BeanWFDetailLookup stt = new BeanWFDetailLookup();
                    stt.ID = rowIndex.ToString();
                    stt.DataValue = rowIndex.ToString();
                    //lst_data.Insert(0, stt);

                    if (isRowSum)
                    {
                        foreach (var item in _value)
                        {
                            if (item.Key == _lst_sumValueColumn[0].Key)
                            {
                                BeanWFDetailLookup data = new BeanWFDetailLookup();
                                data.ID = item.Key;
                                data.DataValue = item.Value;
                                lst_data.Add(data);
                            }
                        }
                    }
                    else
                    {
                        foreach (var header in parentView.lst_beanWFDetailsHeaders)
                        {
                            foreach (var item in _value)
                            {
                                if (header.internalName == item.Key)
                                {
                                    if (item.Value.GetType() != typeof(JArray))
                                    {
                                        BeanWFDetailLookup data = new BeanWFDetailLookup();
                                        data.ID = item.Key;
                                        data.DataValue = item.Value;
                                        data.DataType = header.dataType;
                                        data.viewOnly = header.viewOnly;
                                        lst_data.Add(data);
                                    }
                                    else
                                    {
                                        foreach (var item1 in item.Value)
                                        {
                                            BeanWFDetailLookup data = new BeanWFDetailLookup();
                                            data.ID = item.Key;
                                            data.DataValue = item1;
                                            data.DataType = header.dataType;
                                            data.viewOnly = header.viewOnly;
                                            lst_data.Add(data);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_ValueDetailsCell - CollectionValueRow_Source - Err: " + ex.ToString());
                }
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return parentView.lst_beanWFDetailsHeaders.Count;
                //eturn lst_data.Count;
            }
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                bool isFirst;
                if (indexPath.Row == 0)
                    isFirst = true;
                else
                    isFirst = false;

                CollectionValueRow_Cell cell = (CollectionValueRow_Cell)collectionView.DequeueReusableCell(CollectionValueRow_Cell.CellID, indexPath);
                try
                {
                    if (isRowSum)
                    {
                        if (parentView.lst_beanWFDetailsHeaders[indexPath.Row].internalName == lst_data[0].ID)
                        {
                            cell.UpdateRow(lst_data[0], parentView, rowIndex, isFirst, isOdd, isRowSum);
                        }
                    }
                    else
                    {
                        var item = lst_data[indexPath.Row];
                        cell.UpdateRow(item, parentView, rowIndex, isFirst, isOdd, isRowSum);
                    }

                    return cell;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_ValueDetailsCell - GetCell - Err: " + ex.ToString());
                    return cell;
                }
            }
        }

        private class CollectionValueRow_Cell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            UIImageView iv_cover;
            UILabel lbl_name;
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            int rowIndex { get; set; }
            bool isRowSum;

            [Export("initWithFrame:")]
            public CollectionValueRow_Cell(RectangleF frame) : base(frame)
            {
                lbl_name = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14),
                    TextColor = UIColor.Black
                };

                ContentView.AddSubviews(lbl_name);
            }

            public void UpdateRow(BeanWFDetailLookup _data, Custom_ValueDetailsCell _parentView, int _rowIndex, bool _isFirst, bool _isodd, bool isRowSum)
            {
                try
                {
                    rowIndex = _rowIndex;

                    if (isRowSum)
                    {
                        lbl_name.Font = UIFont.FromName("ArialMT", 14);
                    }

                    //Code add STT dong khong dung
                    //if (_isFirst)
                    //lbl_name.Text = (rowIndex + 1).ToString();
                    //else
                    //{
                    CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
                    CultureInfo culVN = CultureInfo.GetCultureInfo("vi-VN");

                    if (((JValue)_data.DataValue).Type == JTokenType.Float)
                    {
                        var custValue = double.Parse(_data.DataValue.ToString().Trim(), cul).ToString("N0", culVN);
                        lbl_name.Text = custValue;
                    }
                    else if (((JValue)_data.DataValue).Type == JTokenType.Integer)
                    {
                        var custValue = double.Parse(_data.DataValue.ToString().Trim(), cul).ToString("N0", culVN);
                        lbl_name.Text = custValue;
                    }
                    else if (_data.DataType == "yesno")
                    {
                        bool yesNo = Convert.ToBoolean(_data.DataValue);
                        if (yesNo)
                            lbl_name.Text = CmmFunction.GetTitle("TEXT_LABEL_YES", "Có");
                        else
                            lbl_name.Text = CmmFunction.GetTitle("TEXT_LABEL_NO", "Không");
                    }
                    else if (_data.DataType == "date")
                    {
                        try
                        {
                            DateTime dateValue = new DateTime();
                            dateValue = DateTime.Parse(_data.DataValue.ToString());
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_name.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                            else
                                lbl_name.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateVN);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Custom_valueDetailsCell - Unable to convert DATE ");
                        }
                    }
                    else if (_data.DataType == "datetime")
                    {
                        DateTime dateValue = new DateTime();
                        try
                        {
                            dateValue = DateTime.Parse(_data.DataValue.ToString());
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_name.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                            else
                                lbl_name.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Custom_valueDetailsCell - Unable to convert DATETIME ");
                        }
                    }
                    else if (_data.DataType == "singlechoice")
                    {
                        BeanDataLookup res = JsonConvert.DeserializeObject<List<BeanDataLookup>>(_data.DataValue.ToString()).FirstOrDefault();
                        lbl_name.Text = res.Title;
                    }
                    else if (_data.DataType == "multiplechoice")
                    {
                        BeanDataLookup res = JsonConvert.DeserializeObject<List<BeanDataLookup>>(_data.DataValue.ToString()).FirstOrDefault();

                        var data = _data.DataValue.ToString().Trim();
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
                                    str_value = str_value + item.Title + "; ";
                                }
                            }

                            result = str_value.TrimEnd(';', ' ');
                        }
                        else if (data.Contains("[]"))
                            result = string.Empty;
                        else
                            result = data;

                        //this.lbl_value.Text = result;
                        lbl_name.Text = result;
                    }
                    else if (_data.DataType == "singlelookup")
                    {
                        BeanDataLookup res = JsonConvert.DeserializeObject<List<BeanDataLookup>>(_data.DataValue.ToString()).FirstOrDefault();
                        lbl_name.Text = res.Title;
                    }
                    else
                        lbl_name.Text = _data.DataValue.ToString();

                    //}
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

                lbl_name.Frame = new CGRect(10, 0, ContentView.Frame.Width - 20, ContentView.Frame.Height);//30
            }
        }

        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            Custom_ValueDetailsCell parentView;
            CollectionValueRow_Source collect_source;

            #region Constructors
            public CustomFlowLayoutDelegate(Custom_ValueDetailsCell _parent, CollectionValueRow_Source _collect_source)
            {
                collect_source = _collect_source;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                // tam khoa check column ID
                //if (!parentView.isRowSum && collect_source.lst_data[indexPath.Row].ID == "ID")
                //    return new CGSize(0, 60);
                //else
                var res = parentView.lst_beanWFDetailsHeaders[indexPath.Row];
                return new CGSize(200, 30);
                //return new CGSize(res.EstWidth, 60);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //var itemSelected = collect_source.lst_workFlow[indexPath.Row];
                //parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }
        #endregion
    }
}
