using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using Newtonsoft.Json;
using UIKit;
using Newtonsoft.Json.Linq;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MultiChoiceView : UIViewController
    {
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }
        bool isMultiSelect;
        BeanDataLookup currentDataSelected { get; set; }
        List<BeanDataLookup> lst_data_selected = new List<BeanDataLookup>();
        List<BeanDataLookup> lst_data = new List<BeanDataLookup>();

        public MultiChoiceView(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            loadContent();
            setlangTitle();

            #region delegate
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_done.TouchUpInside += BT_Done_TouchUpInside;

            #endregion
        }

        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            lst_data_selected = new List<BeanDataLookup>();
            AcctionDone();
        }

        #endregion

        #region private - public method
        public void setContent(UIViewController _parentView, bool _isMultiSelect, ViewElement _element)
        {
            isMultiSelect = _isMultiSelect;
            parentView = _parentView;
            element = _element;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();

            BT_clear.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            BT_back.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            BT_done.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);

            table_content.ContentInset = new UIEdgeInsets(-35, 0, 0, 0);
        }

        private void loadContent()
        {
            if (!isMultiSelect)
            {
                BT_done.Hidden = true;
                Constraint_rightBTClear.Constant = 0;
            }
            else
                BT_done.Hidden = false;


            if (element != null)
            {
                lbl_title.Text = element.Title;
                JArray json = JArray.Parse(element.DataSource);

                lst_data = json.ToObject<List<BeanDataLookup>>();

                if (!string.IsNullOrEmpty(element.Value) && element.Value.Contains("[{"))
                    lst_data_selected = JsonConvert.DeserializeObject<List<BeanDataLookup>>(element.Value);

                //item choose content... when signlechoose
                //if (!isMultiSelect)
                //{
                //BeanDataLookup beanDataLookup = new BeanDataLookup();
                //beanDataLookup.ID = "-1";
                //beanDataLookup.Title = CmmFunction.GetTitle("TEXT_CHOOSE_CONTENT", "Choose content...");
                //beanDataLookup.IsSelected = false;
                //lst_data.Insert(0, beanDataLookup);// insert choose content into list
                //if (lst_data_selected == null || lst_data_selected.Count == 0) // lst_data_selected = beanDataLookup)(choose content)
                //{
                //    beanDataLookup.IsSelected = true;
                //    lst_data_selected.Add(beanDataLookup);
                //}
                //}


                if (lst_data_selected != null)
                {
                    if (isMultiSelect)
                    {
                        foreach (var item in lst_data)
                        {
                            foreach (var item2 in lst_data_selected)
                            {
                                if (item.Title.ToLower() == item2.Title.ToLower())
                                    item.IsSelected = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in lst_data)
                        {
                            foreach (var item2 in lst_data_selected)
                            {
                                if (item.Title.ToLower() == item2.Title.ToLower())
                                    item.IsSelected = true;
                            }
                        }
                    }

                    if (lst_data != null && lst_data.Count > 0)
                    {
                        table_content.Source = new Multichoice_TableSource(lst_data, this);
                        table_content.ReloadData();
                    }
                }
            }
        }

        public void HandleSeclectItem(BeanDataLookup _data, NSIndexPath _indexPath)
        {
            if (!isMultiSelect)
            {
                if (lst_data_selected != null)
                {
                    lst_data_selected = new List<BeanDataLookup>();
                    lst_data_selected.Add(_data);
                }
                //if (currentDataSelected != null && currentDataSelected.ID != _data.ID)
                //    currentDataSelected.IsSelected = false;

                //currentDataSelected = _data;
                //element.Value = currentDataSelected.Title;

                //if(_data.ID == "-1")
                //    element.Value = "";
                //else
                //{
                //    string jsonString = string.Empty;
                //    if (lst_data_selected != null && lst_data_selected.Count > 0)
                //        jsonString = JsonConvert.SerializeObject(lst_data_selected);

                //    element.Value = jsonString;
                //}
                string jsonString = string.Empty;
                if (lst_data_selected != null && lst_data_selected.Count > 0)
                    jsonString = JsonConvert.SerializeObject(lst_data_selected);

                element.Value = jsonString;

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {

                    RequestDetailsV2 Parentview = parentView as RequestDetailsV2;
                    Parentview.HandleChoiceSelected(element);
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty Parentview = parentView as FormWFDetailsProperty;
                    Parentview.HandleChoiceSelected(element);
                }
                _data.IsSelected = !_data.IsSelected;
                this.View.EndEditing(true);
                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewControllerAsync(true);
            }
            else
            {
                _data.IsSelected = !_data.IsSelected;
                if (_data.IsSelected == true) // them vao
                {
                    if (lst_data_selected == null)
                        lst_data_selected = new List<BeanDataLookup>();
                    lst_data_selected.Add(_data);
                }
                else // remove
                {
                    lst_data_selected = lst_data_selected.FindAll(s => s.ID != _data.ID);
                }



                table_content.ReloadData();
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }

        void BT_Done_TouchUpInside(object sender, EventArgs e)
        {
            AcctionDone();
        }
        private void AcctionDone()
        {
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                string jsonString = string.Empty;
                if (lst_data_selected != null && lst_data_selected.Count > 0)
                    jsonString = JsonConvert.SerializeObject(lst_data_selected);

                element.Value = jsonString;


                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleChoiceSelected(element);
                //requestDetailsV2.HandleUserMultiChoiceSelected(element, lst);

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                string jsonString = string.Empty;
                if (lst_data_selected != null && lst_data_selected.Count > 0)
                    jsonString = JsonConvert.SerializeObject(lst_data_selected);

                element.Value = jsonString;


                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleChoiceSelected(element);
                //requestDetailsV2.HandleUserMultiChoiceSelected(element, lst);

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissModalViewController(true);
            }
        }
        #endregion

        #region custom class

        #region table data source user
        private class Multichoice_TableSource : UITableViewSource
        {
            List<BeanDataLookup> lst_controlvalue;
            NSString cellIdentifier = new NSString("cell");
            MultiChoiceView parentView;
            nint docCount = 0;

            public Multichoice_TableSource(List<BeanDataLookup> _lst_controlvalue, MultiChoiceView _parentview)
            {
                parentView = _parentview;

                if (_lst_controlvalue != null)
                {
                    lst_controlvalue = _lst_controlvalue;
                }
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_controlvalue.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 50;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_controlvalue[indexPath.Row];
                parentView.HandleSeclectItem(item, indexPath);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                ControlValue_cell_custom cell = new ControlValue_cell_custom(cellIdentifier);
                var controlvalue = lst_controlvalue[indexPath.Row];
                cell.UpdateCell(controlvalue);
                return cell;
            }
        }
        private class ControlValue_cell_custom : UITableViewCell
        {

            UILabel lbl_title;
            UILabel line;

            public ControlValue_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                viewConfiguration();
            }
            private void viewConfiguration()
            {
                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                ContentView.AddSubviews(new UIView[] { lbl_title });
            }
            public void UpdateCell(BeanDataLookup _controlvalue)
            {
                if (_controlvalue.IsSelected)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                lbl_title.Text = _controlvalue.Title;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(25, 15, ContentView.Frame.Width - 50, 20);
            }
        }

        #endregion

        #endregion
    }
}

