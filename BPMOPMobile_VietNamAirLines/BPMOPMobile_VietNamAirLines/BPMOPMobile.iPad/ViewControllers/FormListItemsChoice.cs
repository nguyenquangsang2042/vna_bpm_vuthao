using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FormListItemsChoice : UIViewController
    {
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }
        bool isMultiSelect;
        BeanDataLookup currentDataSelected { get; set; }
        List<BeanDataLookup> lst_data_selected = new List<BeanDataLookup>();
        List<BeanDataLookup> lst_data;


        public FormListItemsChoice(IntPtr handle) : base(handle)
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
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            #endregion
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
            BT_clear.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            if (isMultiSelect)
                BT_approve.Hidden = false;
            else
            {
                BT_approve.Hidden = true;
                Constant_width_BT_approve.Constant = 0;
                Constant_left_BT_approve.Constant = 0;
            }

            table_content.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            table_content.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
        }

        private void loadContent()
        {
            try
            {
                if (element != null)
                {
                    lbl_title.Text = element.Title;
                    lst_data = new List<BeanDataLookup>();
                    lbl_title.Text = element.Title;
                    JArray json = JArray.Parse(element.DataSource);
                    lst_data = json.ToObject<List<BeanDataLookup>>();
                    if (!string.IsNullOrEmpty(element.Value) && element.Value.Contains("[{"))
                        lst_data_selected = JsonConvert.DeserializeObject<List<BeanDataLookup>>(element.Value);

                    if (!string.IsNullOrEmpty(element.Value))
                    {
                        if (element.Value.Contains("[{"))
                            lst_data_selected = JsonConvert.DeserializeObject<List<BeanDataLookup>>(element.Value);
                        else
                        {
                            BeanDataLookup beanDataLookup = new BeanDataLookup();
                            beanDataLookup.ID = element.ID;
                            beanDataLookup.Title = element.Value;
                            beanDataLookup.IsSelected = true;
                            lst_data_selected.Add(beanDataLookup);
                        }
                    }

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
            catch (Exception ex)
            {
                Console.WriteLine("FromListItemsChoice - loadContent - ERR: " + ex.ToString());
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

                string jsonString = string.Empty;
                if (lst_data_selected != null && lst_data_selected.Count > 0)
                    jsonString = JsonConvert.SerializeObject(lst_data_selected);

                element.Value = jsonString;

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView Parentview = parentView as ToDoDetailView;
                    Parentview.HandleChoiceSelected(element);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView Parentview = parentView as WorkflowDetailView;
                    Parentview.HandleChoiceSelected(element);
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails Parentview = parentView as FormWorkFlowDetails;
                    Parentview.HandleChoiceSelected(element);
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty Parentview = parentView as FormWFDetailsProperty;
                    Parentview.HandleChoiceSelected(element);
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController Parentview = parentView as FollowListViewController;
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
                table_content.ReloadData();
            }

            table_content.ReloadData();
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }
        void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            string jsonString = string.Empty;
            lst_data_selected = new List<BeanDataLookup>();
            foreach (var item in lst_data)
            {
                if (item.IsSelected)
                    lst_data_selected.Add(item);
            }
            jsonString = JsonConvert.SerializeObject(lst_data_selected);

            element.Value = jsonString;

            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                toDoDetailView.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetail = parentView as WorkflowDetailView;
                workflowDetail.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleChoiceSelected(element);
            }

            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissModalViewController(true);
        }
        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            element.Value = "";

            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                toDoDetailView.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetail = parentView as WorkflowDetailView;
                workflowDetail.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleChoiceSelected(element);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleChoiceSelected(element);
            }

            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissModalViewController(true);
        }
        #endregion

        #region custom class

        #region table data source user
        private class Multichoice_TableSource : UITableViewSource
        {
            List<BeanDataLookup> lst_controlvalue;
            NSString cellIdentifier = new NSString("cell");
            FormListItemsChoice parentView;
            nint docCount = 0;

            public Multichoice_TableSource(List<BeanDataLookup> _lst_controlvalue, FormListItemsChoice _parentview)
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

