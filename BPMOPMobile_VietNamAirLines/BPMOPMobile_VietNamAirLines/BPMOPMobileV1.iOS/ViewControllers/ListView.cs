using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.KanBanCustomClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ListView : UIViewController
    {
        BeanWorkflow beanWorkflow = new BeanWorkflow();
        BeanResourceView beanResourceView = new BeanResourceView();
        private string hintTextSearchTitle = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm theo tên quy trình");
        ButtonsActionBroadBotBarApplication buttonActionBotBarApplication;
        bool isListDefault = true;
        bool isSearch = false;
        List<BeanResourceView> lst_resourceview;
        private List<BeanWFDetailsHeader> _lstHeaderDynamic = new List<BeanWFDetailsHeader>();
        private List<JObject> _lstJObjectDynamic = new List<JObject>();
        bool isLoadMore = true;

        public ListView(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public override void ViewWillAppear(bool animated)
        {
            //isLoadMore = true;
            base.ViewWillAppear(animated);
            if (buttonActionBotBarApplication != null)
            {
                bottom_view.AddSubviews(buttonActionBotBarApplication);
            }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           

            ViewConfiguration();
            SetLangTitle();

            LoadData(0);

            #region delelegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            bt_change_style_list.TouchUpInside += Bt_change_style_list_TouchUpInside;
            BT_Previous.TouchUpInside += BT_Previous_TouchUpInside;
            BT_Next.TouchUpInside += BT_Next_TouchUpInside;

            BT_search.TouchUpInside += BT_search_TouchUpInside;
            tf_search_title.Started += Tf_search_title_Started;
            tf_search_title.Ended += Tf_search_title_Ended;
            tf_search_title.EditingChanged += Tf_search_title_EditingChanged;
            #endregion
        }

        private void Tf_search_title_EditingChanged(object sender, EventArgs e)
        {
            
        }

        private void SearchToggle()
        {
            if (!isSearch) // search dang dong
            {
                if (view_search.Frame.Height > 0)
                {
                    if (!string.IsNullOrEmpty(tf_search_title.Text.Trim()))
                    {
                        isSearch = true;
                        tf_search_title.BecomeFirstResponder();
                        BT_search.Enabled = true;
                    }
                    else
                    {
                        isSearch = false;
                        UIView.BeginAnimations("search_slideAnimationShow");
                        UIView.SetAnimationDuration(0.4f);
                        UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                        UIView.SetAnimationRepeatCount(0);
                        UIView.SetAnimationRepeatAutoreverses(false);
                        UIView.SetAnimationDelegate(this);
                        view_search.Alpha = 0;
                        view_search.Hidden = true;
                        constraint_heightSearch.Constant = 0;
                        this.View.EndEditing(true);
                        UIView.CommitAnimations();
                        BT_search.Enabled = true;
                        BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                        BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                    }
                }
                else
                {
                    isSearch = true;

                    view_search.Alpha = 1;
                    view_search.Hidden = false;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    constraint_heightSearch.Constant = 44;
                    UIView.CommitAnimations();
                    tf_search_title.BecomeFirstResponder();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(40, 176, 255);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tf_search_title.Text.Trim()))
                {
                    isSearch = false;
                    this.View.EndEditing(true);
                    BT_search.Enabled = true;
                }
                else
                {
                    isSearch = false;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    view_search.Alpha = 0;
                    view_search.Hidden = true;
                    constraint_heightSearch.Constant = 0;
                    this.View.EndEditing(true);
                    UIView.CommitAnimations();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                }
            }
        }

        #endregion

        #region private - public method
        public void SetContent(BeanWorkflow _workflow)
        {
            beanWorkflow = _workflow;
        }

        private void ViewConfiguration()
        {
            
            SetConstraint();
            BT_search.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            bt_change_style_list.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_filterDate.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_back.ContentEdgeInsets = new UIEdgeInsets(5, 0, 5, 0);

            BT_Next.SetImage(UIImage.FromFile("Icons/icon_next_catagory.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            BT_Previous.SetImage(UIImage.FromFile("Icons/icon_previous_catagory.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            lbl_fromdate.Layer.BorderWidth = 0;
            lbl_todate.Layer.BorderWidth = 0;

            table_list_special.Hidden = true;
            table_list_special.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
            table_list_special.TableFooterView = new UIView();
            table_list_special.TableHeaderView = new UIView();
            table_list_special.RowHeight = UITableView.AutomaticDimension;
            table_list_special.EstimatedRowHeight = 100f;

            table_list_default.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
            table_list_default.TableFooterView = new UIView();
            table_list_default.TableHeaderView = new UIView();

            constraint_heightSearch.Constant = 0;
            view_choose_list.Layer.CornerRadius = 4;
            view_choose_list.ClipsToBounds = true;
            view_search.Layer.CornerRadius = 4;
            view_search.ClipsToBounds = true;
          
            //bttom view
            buttonActionBotBarApplication = ButtonsActionBroadBotBarApplication.Instance;
            CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            buttonActionBotBarApplication.InitFrameView(bottomBarFrame);
            buttonActionBotBarApplication.LoadStatusButton(2);
            bottom_view.AddSubview(buttonActionBotBarApplication);
            CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

        }
        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        private void SetConstraint()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
                
            }
        }
        //option = 0 default
        //option = 1 bt_next
        //option = 2 bt_pre
        private void LoadData(int option)
        {
            //lbl_title
            if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_title.Text = beanWorkflow.Title;
            else
                lbl_title.Text = beanWorkflow.TitleEN;
            //BeanResourceView
            if(option == 0)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string query = string.Format(@"SELECT *, (CASE WHEN[Index] = 0 THEN NULL ELSE 1 END) AS sort_order_rule
                                               FROM BeanResourceView 
                                               WHERE ResourceId = {0} AND Status = 1 AND TypeId = 0 AND [Index] >= 0
                                               ORDER BY sort_order_rule DESC, MenuId ASC", beanWorkflow.WorkflowID);
                lst_resourceview = conn.Query<BeanResourceView>(query);
                if (lst_resourceview != null && lst_resourceview.Count > 0)
                    beanResourceView = lst_resourceview.First();
            } else if(option == 1) {
                int index_next  = lst_resourceview.FindIndex(s => s.ID == beanResourceView.ID);
                if (index_next >= 0 && index_next != lst_resourceview.Count - 1)
                    beanResourceView = lst_resourceview[index_next + 1];
            }
            else if (option == 2)
            {
                int index_pre = lst_resourceview.FindIndex(s => s.ID == beanResourceView.ID);
                if (index_pre > 0)
                    beanResourceView = lst_resourceview[index_pre -1];
            }
            // get data
            if (beanResourceView != null)
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                {
                    lbl_titleResourceView.Text = beanResourceView.Title;
                }
                else
                {
                    lbl_titleResourceView.Text = beanResourceView.TitleEN;
                }
                SettingButtonTitle();

                ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                _lstHeaderDynamic = new List<BeanWFDetailsHeader>();
                _lstJObjectDynamic = new List<JObject>();
                _lstHeaderDynamic = _pConTrolDynamic.GetDynamicFormField(beanResourceView.ID, CmmVariable.M_DataFilterAPILimitData, 0);
                _lstJObjectDynamic = _pConTrolDynamic.GetDynamicWorkflowItem(beanResourceView.ID, null, CmmVariable.M_DataFilterAPILimitData, 0);
                if (_lstHeaderDynamic != null && _lstHeaderDynamic.Count > 0 && _lstJObjectDynamic != null && _lstJObjectDynamic.Count > 0)
                {
                    table_list_special.Source = new List_Dynamic_TableSource(this, _lstHeaderDynamic, _lstJObjectDynamic);
                    table_list_default.Source = new List_Default_TableSource(this, _lstHeaderDynamic, _lstJObjectDynamic);
                    if (!isListDefault)
                    {
                        table_list_special.ReloadData();
                        table_list_special.Hidden = false;
                        lbl_nodata.Hidden = true;
                        table_list_default.Hidden = true;
                    }
                    else
                    {
                        table_list_default.ReloadData();
                        table_list_default.Hidden = false;
                        lbl_nodata.Hidden = true;
                        table_list_special.Hidden = true;
                    }
                }
                else
                {
                    lbl_nodata.Hidden = false;
                    table_list_special.Hidden = true;
                    table_list_default.Hidden = true;
                }
            }
        }
        bool isFilterServer = false;
        public async void loadmoreData()
        {
            view_loadmore.Hidden = false;
            indicator_loadmore.StartAnimating();

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (isFilterServer)
                    {
                        
                    }
                    else
                    {
                            LoadMoreData();
                    }
                    indicator_loadmore.StopAnimating();
                    view_loadmore.Hidden = true;
                });
            });
        }
        private void LoadMoreData()
        {
            try
            {
                if (isListDefault)
                {
                    ProviderControlDynamic _pConTrolDynamic = new ProviderControlDynamic();
                    List<JObject> _lstJObjectDynamic_temp = new List<JObject>();
                    _lstJObjectDynamic_temp = _pConTrolDynamic.GetDynamicWorkflowItem(beanResourceView.ID, null, CmmVariable.M_DataFilterAPILimitData, _lstJObjectDynamic.Count);
                    if (_lstJObjectDynamic_temp != null && _lstJObjectDynamic_temp.Count > 0 )
                    {
                        _lstJObjectDynamic.AddRange(_lstJObjectDynamic_temp);
                        if (_lstJObjectDynamic_temp.Count < CmmVariable.M_DataFilterAPILimitData)
                            isLoadMore = false;
                        table_list_special.ReloadData();

                    }
                    else
                    {
                        isLoadMore = false;
                    }
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListView - LoadMoreData - Err: " + ex.ToString());
            }
        }

        //private void a()
        //{
        //    for (int i = 0; i < _lstHeaderDynamic.Count; i++)
        //    {
        //        BeanWFDetailsHeader _rowView = LayoutInflater.From(_holder.ItemView.Context).Inflate(Resource.Layout.ItemDynamicListItem, null);
        //        TextView _tvTitle = _rowView.FindViewById<TextView>(Resource.Id.tv_ItemDynamicListItem_Title);
        //        TextView _tvValue = _rowView.FindViewById<TextView>(Resource.Id.tv_ItemDynamicListItem_Value);



        //        _tvTitle.Text = (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? _lstHeader[i].Title : _lstHeader[i].TitleEN) + ":";
        //        _tvTitle.SetMaxWidth((int)(_mainAct.Resources.DisplayMetrics.WidthPixels * 0.3));



        //        BindingValueData(_lstHeaderDynamic[i], _lstJObjectDynamic[position], _tvValue);
        //    }
        //}
        //        private void BindingValueData(BeanWFDetailsHeader _itemHeader, JObject _currentJObjectRow)
        //        {
        //            try
        //            {
        //                Newtonsoft.Json.Linq.JToken _curJToken = null;
        //                if (!String.IsNullOrEmpty(_itemHeader.FieldMapping)) // ưu tiền FieldMapping, nếu không có thì xài internalName
        //                    _curJToken = _currentJObjectRow[_itemHeader.FieldMapping.ToString()];
        //                else
        //                    _curJToken = _currentJObjectRow[_itemHeader.internalName.ToString()];



        //                switch (_itemHeader.FieldTypeId)
        //                {
        //                    case (int)CmmFunction.DynamicFieldTypeID.UserGroup: // User group
        //                        {
        //                            string[] _lstFullName = CTRLHomePage.GetArrayFullNameFromArrayID(_curJToken.ToString().Trim().ToLowerInvariant().Split(","));
        //                            CTRLHomePage.SetTextView_FormatMultiUser2(_context, _tvContent, _lstFullName, IsHighLightColor: false, showFromText: false);
        //                            break;
        //                        }
        //                    case (int)CmmFunction.DynamicFieldTypeID.DateAndTime: // Date time
        //                        {
        //                            DateTime dateValue;
        //                            try
        //                            {
        //                                dateValue = DateTime.Parse(_curJToken.ToString());



        //                                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
        //                                    _tvContent.Text = dateValue.ToString("dd/MM/yy HH:mm").ToLower().Trim();
        //                                else
        //                                    _tvContent.Text = dateValue.ToString("MM/dd/yy HH:mm").ToLower().Trim();
        //                            }
        //                            catch (Exception)
        //                            {
        //                                _tvContent.Text = "";
        //                            }
        //                            break;
        //                        }
        //                    case (int)CmmFunction.DynamicFieldTypeID.Lookup:
        //                    case (int)CmmFunction.DynamicFieldTypeID.ComboBox:
        //                    case (int)CmmFunction.DynamicFieldTypeID.DropDownList:
        //                    case (int)CmmFunction.DynamicFieldTypeID.Radio:
        //                    case (int)CmmFunction.DynamicFieldTypeID.CheckBox:
        //                    case (int)CmmFunction.DynamicFieldTypeID.Choice:
        //                        {
        //                            _tvContent.Text = _curJToken.ToString();
        //                            break;
        //                        }
        //                    case (int)CmmFunction.DynamicFieldTypeID.Calculated:
        //                    case (int)CmmFunction.DynamicFieldTypeID.Currency:
        //                    case (int)CmmFunction.DynamicFieldTypeID.Number:
        //                    case (int)CmmFunction.DynamicFieldTypeID.MultipleLinesText:
        //                    case (int)CmmFunction.DynamicFieldTypeID.SingleLineText:
        //                    default:
        //                        {
        //                            _tvContent.Text = _curJToken.ToString();
        //                            break;
        //                        }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _tvContent.Text = "";
        //#if DEBUG
        //                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnBindViewHolder", ex);
        //#endif
        //            }



        //        }

        private void SettingButtonTitle()
        {
            //bt_next
            int index = lst_resourceview.FindIndex(s => s.ID == beanResourceView.ID);
            if (index >= 0 && index != lst_resourceview.Count - 1)
            {
                BT_Next.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                BT_Next.Enabled = true;
            }
            else
            {
                BT_Next.Enabled = false;
                BT_Next.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                BT_Next.ImageView.TintColor = UIColor.LightGray;
            }
            //bt_privios
            if (index > 0)
            {
                BT_Previous.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                BT_Previous.Enabled = true;
            }
            else
            {
                BT_Previous.Enabled = false;
                BT_Previous.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                BT_Previous.ImageView.TintColor = UIColor.LightGray;
            }
        }
        public void reloadData()
        {
            //LoadContent(tf_search_title.Text);
        }

        private void ToggleMenuDate()
        {
            if (!isFilter)
            {
                isFilter = true;
                view_filter.Hidden = false;
                //custom_CalendarView.Hidden = false;
            }
            else
            {
                isFilter = false;
                view_filter.Hidden = true;
                //custom_CalendarView.Hidden = true;
            }
        }
        public void NavigateToWorkFlowDetails(BeanAppBaseExt _beanAppBaseExt)
        {
            RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
            requestDetailsV2.setContent(_beanAppBaseExt);
            this.NavigationController.PushViewController(requestDetailsV2, true);
        }
        #endregion

        #region events

        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            buttonActionBotBarApplication.broadView.BackFromeKanBanView();
        }

        private void BT_Next_TouchUpInside(object sender, EventArgs e)
        {
            LoadData(1);
        }
        private void BT_Previous_TouchUpInside(object sender, EventArgs e)
        {
            LoadData(2);
        }
        private void Bt_change_style_list_TouchUpInside(object sender, EventArgs e)
        {
            if (_lstHeaderDynamic != null && _lstHeaderDynamic.Count > 0 && _lstJObjectDynamic != null && _lstJObjectDynamic.Count > 0)
            {
                
                if (isListDefault)//table_list_special
                {
                    table_list_special.ReloadData();
                    table_list_special.Hidden = false;

                    lbl_nodata.Hidden = true;
                    table_list_default.Hidden = true;
                    bt_change_style_list.SetImage(UIImage.FromFile("Icons/icon_change_style_list.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    bt_change_style_list.ImageView.TintColor = UIColor.FromRGB(0,95,212);
                }
                else//table_list_default
                {
                    table_list_default.ReloadData();
                    table_list_default.Hidden = false;

                    lbl_nodata.Hidden = true;
                    table_list_special.Hidden = true;
                    bt_change_style_list.SetImage(UIImage.FromFile("Icons/icon_change_style_list.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    bt_change_style_list.ImageView.TintColor = UIColor.LightGray;
                }
                isListDefault = !isListDefault;
            }
        }
        private void Tf_search_title_Started(object sender, EventArgs e)
        {
            tf_search_title.Text = "";
        }
        private void Tf_search_title_Ended(object sender, EventArgs e)
        {
            if (tf_search_title.Text == "")
            {
                tf_search_title.Text = hintTextSearchTitle;
            }
        }
        private void BT_search_TouchUpInside(object sender, EventArgs e)
        {

            if (isFilter)
            {
                ToggleMenuDate();
            }
            tf_search_title.Text = "";
            SearchToggle();
        }
        bool isFilter = false;
        #endregion



        #region list dynamic source table
        private class List_Dynamic_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellBroadID");
            ListView parentView { get; set; }
            private List<BeanWFDetailsHeader> lstHeaderDynamic { get; set; }
            private List<JObject> lstJObjectDynamic { get; set; }
            //bool isFilterServer = false;

            public List_Dynamic_TableSource(ListView _parentview, List<BeanWFDetailsHeader> _lstHeaderDynamic, List<JObject> _lstJObjectDynamic)
            {
                parentView = _parentview;
                lstHeaderDynamic = _lstHeaderDynamic;
                lstJObjectDynamic = _lstJObjectDynamic;
            }
           

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lstJObjectDynamic.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                JObject value = lstJObjectDynamic[indexPath.Row];
                Dynamic_cell_custom cell = new Dynamic_cell_custom(cellIdentifier);
                if (indexPath.Row % 2 == 0)
                    cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);
                cell.UpdateRow(lstHeaderDynamic,value, parentView);
                return cell;
            }
            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    if (indexPath.Row % CmmVariable.M_DataFilterAPILimitData == 0 && lstJObjectDynamic.Count == indexPath.Row) // boi so cua 20
                    {
                        parentView.view_loadmore.Hidden = false;
                        parentView.indicator_loadmore.StartAnimating();
                        parentView.loadmoreData();
                    }
                }
            }
        }
        private class Dynamic_cell_custom : UITableViewCell
        {
            List<BeanWFDetailsHeader> lstHeader = new List<BeanWFDetailsHeader>();
            List<BeanWFDetailsHeader> lstHeaderDynamic = new List<BeanWFDetailsHeader>();
            List<BeanWFDetailsHeader> lstHeaderDefautl = new List<BeanWFDetailsHeader>();
            UILabel lbl_title;
            UIImageView img_follow, img_attachment;

            List<UILabel> lst_lable = new List<UILabel>();

            public Dynamic_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                configview();
            }

            private void configview()
            {
                lbl_title = new UILabel
                {
                    Font = UIFont.FromName("Arial-BoldMT", 15),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                    TextAlignment = UITextAlignment.Left,
                    Lines = 2
                };
                img_follow = new UIImageView();
                img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                img_follow.ContentMode = UIViewContentMode.ScaleToFill;

                img_attachment = new UIImageView();
                img_attachment.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_attachment.ContentMode = UIViewContentMode.ScaleToFill;
                img_attachment.Hidden = true;

                ContentView.AddSubviews(lbl_title, img_follow, img_attachment);
            }
            private void loadata()
            {
                foreach (var item in lstHeader)
                {
                    switch (item.internalName.ToLowerInvariant())
                    {
                        case "title":
                        case "isfollow":
                        case "filecount":
                            lstHeaderDefautl.Add(item);
                            break;
                        default:
                            lstHeaderDynamic.Add(item);
                            break;
                    }
                }
            }

            public void UpdateRow(List<BeanWFDetailsHeader> _lstHeader, JObject _value, UIViewController _controller)
            {
                lstHeader = _lstHeader;
                loadata();

                foreach (var header in lstHeaderDefautl)
                {
                    var title = header.internalName.ToLowerInvariant();
                    if (title == "title" )
                    {
                        string valueOfTitle = CmmFunction.GetFormattedValueByHeader(header, _value);
                        lbl_title.Text = valueOfTitle;
                    }
                    if (title == "isfollow")
                    {
                       string isfollow = CmmFunction.GetFormattedValueByHeader(header, _value);
                        if (isfollow == "1")
                            img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                        else
                            img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                    }
                    if (title == "filecount")
                    {
                        int filecount = int.Parse( CmmFunction.GetFormattedValueByHeader(header, _value));
                        if (filecount > 0)
                            img_attachment.Hidden = false;
                        else
                            img_attachment.Hidden = true;
                    }
                }
                var heightTitle = StringExtensions.StringRect(lbl_title.Text, UIFont.FromName("Arial-BoldMT", 14f), ContentView.Frame.Width - 100).Height;
                if (heightTitle > 45)
                    heightTitle = 45;
                else
                    heightTitle = 20;
                lbl_title.TranslatesAutoresizingMaskIntoConstraints = false;
                lbl_title.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 15).Active = true;
                lbl_title.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 15).Active = true;
                lbl_title.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor , -70).Active = true;
                lbl_title.HeightAnchor.ConstraintEqualTo(heightTitle).Active = true;
                //lbl_title.BackgroundColor = UIColor.Red;

                img_attachment.TranslatesAutoresizingMaskIntoConstraints = false;
                img_attachment.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 15).Active = true;
                img_attachment.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -15).Active = true;
                img_attachment.HeightAnchor.ConstraintEqualTo(20).Active = true;
                img_attachment.WidthAnchor.ConstraintEqualTo(20).Active = true;
                //img_attachment.BackgroundColor = UIColor.Red;

                img_follow.TranslatesAutoresizingMaskIntoConstraints = false;
                img_follow.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, 15).Active = true;
                img_follow.TrailingAnchor.ConstraintEqualTo(img_attachment.LeadingAnchor, -10).Active = true;
                img_follow.HeightAnchor.ConstraintEqualTo(20).Active = true;
                img_follow.WidthAnchor.ConstraintEqualTo(20).Active = true;
                //img_follow.BackgroundColor = UIColor.Yellow;

                int i = 0;
                int padding = 10;
                int heightItemInCell = 15;
                foreach (var header in lstHeaderDynamic)
                {
                    var title = "";
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        title = header.Title;
                    else
                        title = header.TitleEN;

                    var textValue = CmmFunction.GetFormattedValueByHeader(header, _value);
                    UILabel lbl_titledynamic;
                    //UILabel lbl_titledynamic, lbl_valuedynamic;
                    lbl_titledynamic = new UILabel
                    {
                        Font = UIFont.FromName("ArialMT", 12),
                        TextColor = UIColor.FromRGB(94, 94, 94),
                        TextAlignment = UITextAlignment.Left,
                        Text = title + ":  " + textValue
                    };
                    //lbl_valuedynamic = new UILabel
                    //{
                    //    Font = UIFont.FromName("ArialMT", 12),
                    //    TextColor = UIColor.FromRGB(0, 0, 0),
                    //    TextAlignment = UITextAlignment.Left,
                    //    //Text = textValue
                    //    Text = "waheriweryiwerytiuyweriutyiweruytiuerytkuerytkjaeyrktyerityikert"

                    //};

                    //ContentView.AddSubviews(lbl_valuedynamic, lbl_titledynamic);
                    //lst_lable.Add(new Tuple<UILabel, UILabel>(lbl_valuedynamic, lbl_titledynamic));
                    ContentView.AddSubviews( lbl_titledynamic);
                    lst_lable.Add(lbl_titledynamic);
                    if (i != lstHeaderDynamic.Count - 1)
                    {
                        lbl_titledynamic.TranslatesAutoresizingMaskIntoConstraints = false;
                        lbl_titledynamic.TopAnchor.ConstraintEqualTo(lbl_title.BottomAnchor, (lst_lable.Count - 1) * (padding + heightItemInCell) + 15).Active = true;
                        lbl_titledynamic.LeadingAnchor.ConstraintEqualTo(lbl_title.LeadingAnchor, 0).Active = true;
                        lbl_titledynamic.HeightAnchor.ConstraintEqualTo(20).Active = true;
                        lbl_titledynamic.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -15).Active = true;
                        //lbl_titledynamic.BackgroundColor = UIColor.Green;
                    }
                    else
                    {
                        lbl_titledynamic.TranslatesAutoresizingMaskIntoConstraints = false;
                        lbl_titledynamic.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, (lst_lable.Count - 1) * (padding + heightItemInCell) + heightTitle + 30).Active = true;
                        lbl_titledynamic.BottomAnchor.ConstraintLessThanOrEqualTo(ContentView.BottomAnchor, -15).Active = true;
                        lbl_titledynamic.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor, 15).Active = true;
                        lbl_titledynamic.TrailingAnchor.ConstraintEqualTo(ContentView.TrailingAnchor, -15).Active = true;
                        //lbl_titledynamic.BackgroundColor = UIColor.Green;
                    }
                    i++;
                }
            }
        }
        #endregion

        #region list dynamic source table
        private class List_Default_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellBroadID");
            ListView parentView { get; set; }
            private List<BeanWFDetailsHeader> lstHeaderDynamic { get; set; }
            private List<JObject> lstJObjectDynamic { get; set; }
            //bool isFilterServer = false;

            public List_Default_TableSource(ListView _parentview, List<BeanWFDetailsHeader> _lstHeaderDynamic, List<JObject> _lstJObjectDynamic)
            {
                parentView = _parentview;
                lstHeaderDynamic = _lstHeaderDynamic;
                lstJObjectDynamic = _lstJObjectDynamic;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lstJObjectDynamic.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 90;
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                JObject value = lstJObjectDynamic[indexPath.Row];
                Default_cell_custom cell = new Default_cell_custom(cellIdentifier);
                if (indexPath.Row % 2 == 0)
                    cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);
                cell.UpdateRow(lstHeaderDynamic, value, parentView);
                return cell;
            }
            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    if (indexPath.Row % CmmVariable.M_DataFilterAPILimitData == 0 && lstJObjectDynamic.Count == indexPath.Row) // boi so cua 20
                    {
                        parentView.view_loadmore.Hidden = false;
                        parentView.indicator_loadmore.StartAnimating();
                        parentView.loadmoreData();
                    }
                }
            }
        }
        private class Default_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UILabel lbl_imgCover, lbl_status, lbl_title, lbl_sentTime, lbl_subTitle, lbl_duedate;
            private UIImageView iv_avatar, img_follow, img_attach, img_priority;

            public Default_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

           
            private void viewConfiguration()
            {
                iv_avatar = new UIImageView();
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.ClipsToBounds = true;
                iv_avatar.Layer.CornerRadius = 20;
                iv_avatar.Hidden = true;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 15f),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };
                lbl_imgCover.Layer.CornerRadius = 20;
                lbl_imgCover.ClipsToBounds = true;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("Arial-BoldMT", 15f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_sentTime = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Right,
                };

                lbl_subTitle = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                };

                img_follow = new UIImageView();
                img_follow.ContentMode = UIViewContentMode.ScaleAspectFill;
                img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                

                img_attach = new UIImageView();
                img_attach.ContentMode = UIViewContentMode.ScaleAspectFit;
                img_attach.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_attach.TintColor = UIColor.FromRGB(94, 94, 94);
                img_attach.Hidden = true;

                //img_priority = new UIImageView();
                //img_priority.ContentMode = UIViewContentMode.ScaleAspectFit;
                //img_priority.Image = UIImage.FromFile("Icons/icon_flag.png");
                //img_priority.Hidden = true;

                lbl_duedate = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Right,
                };

                lbl_status = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Center,
                };

                lbl_status.ClipsToBounds = true;
                lbl_status.Layer.BorderColor = UIColor.White.CGColor;
                lbl_status.Layer.BorderWidth = 0.5f;
                lbl_status.Layer.CornerRadius = 5;


                ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_sentTime, lbl_subTitle, img_follow, img_attach, lbl_duedate, lbl_status });
            }

            public void UpdateRow(List<BeanWFDetailsHeader> _lstHeader, JObject _value, UIViewController _controller)
            {
               
                try
                {
                    SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string assignedTo = "";
                    foreach (var item in _lstHeader)
                    {
                        BeanWFDetailsHeader _header = item;
                        switch (item.internalName.ToLowerInvariant())
                        {
                            case "title":
                            case "tieude":
                                lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular);
                                string valueOfTitle = CmmFunction.GetFormattedValueByHeader(_header, _value);
                                lbl_title.Text = valueOfTitle;
                                break;
                            case "assignedto":
                                //assignedto
                                string lst_assignedTo = CmmFunction.GetRawValueByHeader(_header, _value);
                                string[] arr_assignedTo;
                                List<string> lst_userName = new List<string>();
                                nfloat temp_width = 0;
                                if (!string.IsNullOrEmpty(lst_assignedTo))
                                {
                                    if (lst_assignedTo.Contains(','))
                                    {
                                        arr_assignedTo = lst_assignedTo.Split(',');
                                        //string res = string.Empty;

                                        for (int i = 0; i < arr_assignedTo.Length; i++)
                                        {
                                            List<BeanUserAndGroup> lst_userAndGroupResult = new List<BeanUserAndGroup>();
                                            string _queryBeanUserGroup = string.Format(@"SELECT Image as ImagePath, 1 as Type, Title as Name FROM BeanGroup WHERE ID LIKE '{0}'
                                             UNION SELECT ImagePath, 0 as Type, FullName as Name   FROM BeanUser WHERE ID LIKE '{0}'", arr_assignedTo[i].Trim().ToLower());
                                            lst_userAndGroupResult = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                                            if (lst_userAndGroupResult != null && lst_userAndGroupResult.Count > 0)
                                            {
                                                lst_userName.Add(lst_userAndGroupResult[0].Name);
                                            }
                                        }

                                        if (lst_userName.Count > 1)
                                        {
                                            int num_remain = lst_userName.Count - 1;
                                            assignedTo = lst_userName[0] + ", +" + num_remain.ToString();
                                        }
                                        else
                                            assignedTo = lst_userName[0];
                                    }
                                    else
                                    {
                                        List<BeanUserAndGroup> lst_userAndGroupResult = new List<BeanUserAndGroup>();
                                        string _queryBeanUserGroup = string.Format(@"SELECT Image as ImagePath, 1 as Type, Title as Name FROM BeanGroup WHERE ID LIKE '{0}'
                                             UNION SELECT ImagePath, 0 as Type, FullName as Name   FROM BeanUser WHERE ID LIKE '{0}'", lst_assignedTo.Trim().ToLower());
                                        lst_userAndGroupResult = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                                        if (lst_userAndGroupResult != null && lst_userAndGroupResult.Count > 0)
                                        {
                                            assignedTo = lst_userAndGroupResult[0].Name;
                                        }
                                    }
                                }
                                //avata
                                if (!string.IsNullOrEmpty(lst_assignedTo))
                                {
                                    string first_user = "";
                                    if (lst_assignedTo.Contains(','))
                                        first_user = lst_assignedTo.Split(',')[0];
                                    else
                                        first_user = lst_assignedTo;

                                    List<BeanUserAndGroup> lst_userAndGroupResult = new List<BeanUserAndGroup>();
                                    string _queryBeanUserGroup = string.Format(@"SELECT Image as ImagePath, 1 as Type, Title as Name FROM BeanGroup WHERE ID LIKE '{0}'
                                             UNION SELECT ImagePath, 0 as Type, FullName as Name   FROM BeanUser WHERE ID LIKE '{0}'", first_user.ToLower());
                                    lst_userAndGroupResult = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                                    string user_imagePath = "";
                                    lbl_imgCover.Hidden = false;
                                    if (lst_userAndGroupResult != null && lst_userAndGroupResult.Count > 0)
                                    {
                                        user_imagePath = lst_userAndGroupResult[0].ImagePath;
                                        lbl_imgCover.Hidden = true;
                                        if (string.IsNullOrEmpty(user_imagePath))
                                        {
                                            iv_avatar.Hidden = false;
                                            if (lst_userAndGroupResult[0].Type == 1)
                                                iv_avatar.Image = UIImage.FromFile("Icons/icon_group.png");
                                            else if (lst_userAndGroupResult[0].Type == 0)
                                                iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                        }
                                        else
                                        {
                                            lbl_imgCover.Hidden = false;
                                            iv_avatar.Hidden = true;
                                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userAndGroupResult[0].Name);
                                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                                            checkFileLocalIsExist(lst_userAndGroupResult[0], iv_avatar);
                                        }
                                    }
                                    else
                                    {
                                        lbl_imgCover.Hidden = true;
                                        iv_avatar.Hidden = false;
                                        iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                }
                                else
                                {
                                    lbl_imgCover.Hidden = true;
                                    iv_avatar.Hidden = false;
                                    iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                }
                                break;
                            case "created":
                                // title lbl_sentTime   
                                string created = CmmFunction.GetRawValueByHeader(_header, _value);
                                if (!string.IsNullOrEmpty(created))
                                {
                                    DateTime dateCreate = new DateTime();
                                    try
                                    {
                                        dateCreate = DateTime.Parse(created);
                                    }
                                    catch(Exception ex)
                                    {
                                        Console.WriteLine("default_customcel- create - err:" + ex.ToString());
                                    }
                                    if (CmmVariable.SysConfig.LangCode == "1033")
                                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(dateCreate, 0, 1033);
                                    else if (CmmVariable.SysConfig.LangCode == "1066")
                                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(dateCreate, 0, 1066);

                                }
                                break;
                            case "status":
                                //line 2
                                string res = string.Empty;
                                string StatusGroup = CmmFunction.GetRawValueByHeader(_header, _value);
                                string queryLine2 = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", StatusGroup);
                                List<BeanAppStatus> _lstAppStatusLine2 = conn.Query<BeanAppStatus>(queryLine2);

                                if (_lstAppStatusLine2 != null && _lstAppStatusLine2.Count > 0)
                                {
                                    if (_lstAppStatusLine2[0].ID == 8) // da phe duyet
                                        res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
                                    else if (_lstAppStatusLine2[0].ID == 64) // da huy
                                        res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
                                    else if (_lstAppStatusLine2[0].ID == 16)
                                        res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
                                    else
                                        res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
                                    if (res.Contains('+'))
                                    {
                                        var indexA = res.IndexOf('+');
                                        NSMutableAttributedString att = new NSMutableAttributedString(res);
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
                                        lbl_subTitle.AttributedText = att;
                                    }
                                    lbl_subTitle.Text = res.TrimEnd(','); // nguoi xu ly hien tai

                                    //status background
                                    lbl_status.Hidden = false;
                                    lbl_status.Frame = new CGRect(90, 60, 50, 20);
                                    lbl_status.Lines = 1;
                                    lbl_status.SizeToFit();

                                    lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1066") ? _lstAppStatusLine2[0].Title : _lstAppStatusLine2[0].TitleEN;
                                    lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(_lstAppStatusLine2[0].ID);

                                }
                                else
                                {
                                    res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
                                    if (res.Contains('+'))
                                    {
                                        var indexA = res.IndexOf('+');
                                        NSMutableAttributedString att = new NSMutableAttributedString(res);
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
                                        lbl_subTitle.AttributedText = att;
                                    }
                                    lbl_subTitle.Text = res.TrimEnd(','); // nguoi xu ly hien tai
                                    //status
                                    lbl_status.Hidden = true;
                                }
                                break;
                            case "isfollow":// sau nay chinh lai theo local may
                                string isfollow = CmmFunction.GetFormattedValueByHeader(_header, _value);
                                if (isfollow == "1")
                                    img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                                else
                                    img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");

                                // sau nay chuyen sang dang local nha

                                ////BeanWorkflowFollow
                                //List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                                //string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = '?'     LIMIT 1 OFFSET 0");
                                //var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, appBaseRequest.ID);

                                ////img_follow
                                //if (lst_followResult != null && lst_followResult.Count > 0)
                                //{
                                //    if (lst_followResult[0].Status == 1)
                                //    {
                                //        img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                                //        //img_follow.Hidden = false;
                                //    }
                                //    else
                                //    {
                                //        img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                                //        //img_follow.Hidden = true;
                                //    }
                                //}
                                //else
                                //{
                                //    //img_follow.Hidden = true;
                                //    img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                                //}
                                break;
                            case "filecount"://img_attach
                                int filecount = int.Parse(CmmFunction.GetFormattedValueByHeader(_header, _value));
                                if (filecount > 0)
                                    img_attach.Hidden = false;
                                else
                                    img_attach.Hidden = true;
                                break;
                            case "duedate":
                                // lbl_duedate TextColor + Text
                                string strDueDate = CmmFunction.GetFormattedValueByHeader(_header, _value);
                                if (string.IsNullOrEmpty(strDueDate))
                                {
                                    DateTime DueDate = new DateTime();
                                    try
                                    {
                                        DueDate = DateTime.Parse(strDueDate);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("default_customcel- create - err:" + ex.ToString());
                                    }
                                    if (CmmVariable.SysConfig.LangCode == "1033")
                                        lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(DueDate, 1, 1033);
                                    else if (CmmVariable.SysConfig.LangCode == "1066")
                                        lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(DueDate, 1, 1066);

                                    if (DueDate < DateTime.Now.Date) // qua han => mau do
                                        lbl_duedate.TextColor = UIColor.Red;
                                    else if (DueDate == DateTime.Now.Date) // trong ngay => mau tim
                                        lbl_duedate.TextColor = UIColor.FromRGB(139, 79, 183);
                                    else if (DueDate > DateTime.Now.Date && DueDate < DateTime.Now.Date.AddDays(5))
                                    {
                                        // chuyen mau xanh la => black
                                        lbl_duedate.TextColor = UIColor.Black;
                                    }
                                    else //if (noti.Status == 1)
                                    {
                                        lbl_duedate.TextColor = UIColor.Black;
                                    }
                                }
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Custom_appBaseRequestCell - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                iv_avatar.Frame = new CGRect(15, 13, 40, 40);
                lbl_imgCover.Frame = new CGRect(15, 13, 40, 40);
                lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - (60 + 90), 20);
                lbl_sentTime.Frame = new CGRect(ContentView.Frame.Width - 85, lbl_title.Frame.Y, 75, 20);

                lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, 270, 20);
                //img_priority.Frame = new CGRect(img_follow.Frame.Right + 10, lbl_subTitle.Frame.Y + 4, 16, 16);
                img_attach.Frame = new CGRect(ContentView.Frame.Width - 30, lbl_title.Frame.Bottom + 4, 16, 16);
                img_follow.Frame = new CGRect(img_attach.Frame.Left - 30, lbl_title.Frame.Bottom + 4, 16, 16);
                lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, ContentView.Frame.Width - lbl_title.Frame.Left - 70, 20);

                var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 13).Width + 20;
                var maxStatusWidth = ContentView.Frame.Width - (lbl_subTitle.Frame.X + 110);
                if (widthStatus < maxStatusWidth)
                    lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
                else
                    lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

                lbl_duedate.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_status.Frame.Y, 120, 20);
            }

            private async void checkFileLocalIsExist(BeanUserAndGroup contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
                    string localfilePath = Path.Combine(localDocumentFilepath, filename);

                    if (!File.Exists(localfilePath))
                    {
                        UIImage avatar = null;
                        await Task.Run(() =>
                        {
                            ProviderBase provider = new ProviderBase();
                            if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            {
                                NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

                                InvokeOnMainThread(() =>
                                {
                                    if (data != null)
                                    {
                                        UIImage image = UIImage.LoadFromData(data);
                                        if (image != null)
                                        {
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                    iv_avatar.Hidden = false;

                                    //kiem tra xong cap nhat lai avatar
                                    lbl_imgCover.Hidden = true;
                                    iv_avatar.Hidden = false;
                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    iv_avatar.Hidden = true;
                                    lbl_imgCover.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        iv_avatar.Hidden = false;
                        lbl_imgCover.Hidden = true;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void openFile(string localfilename, UIImageView image_view)
            {
                try
                {
                    NSData data = null;
                    await Task.Run(() =>
                    {
                        string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                        data = NSData.FromUrl(new NSUrl(localfilePath, false));
                    });

                    if (data != null)
                    {
                        UIImage image = UIImage.LoadFromData(data);
                        if (image != null)
                        {
                            image_view.Image = image;
                        }
                        else
                        {
                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }
        }
        #endregion
    }
}

