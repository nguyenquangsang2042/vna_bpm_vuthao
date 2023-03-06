using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class BroadView_copy : UIViewController
    {
        AppDelegate appD;
        ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;
        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();

        public BroadView_copy(IntPtr handle) : base (handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (buttonActionTopBar != null && buttonActionBotBar != null)
            {
                view_top_bar.AddSubviews(buttonActionTopBar);
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
        }

        #region public - private method
        private void ViewConfiguration()
        {
            buttonActionTopBar = ButtonsActionTopBar.Instance;
            buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            view_top_bar.AddSubviews(buttonActionTopBar);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(3);
            view_bot_bar.AddSubviews(buttonActionBotBar);

            CmmIOSFunction.MakeCornerTopLeftRight(view_lstWorkFlow, 8);

            CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_avatar_temp.png"), UIControlState.Normal);

            CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
        }

        private void LoadContent()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory";
            List<BeanWorkflowCategory> lst_category = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
            if (lst_category != null && lst_category.Count > 0)
            {
                foreach (var item in lst_category)
                {
                    string query_worflow = @"SELECT * FROM BeanWorkflow WHERE WorkflowCategoryID = ?";
                    List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, item.ID);
                    if (lst_workflow != null && lst_workflow.Count > 0)
                    {
                        dict_groupWorkFlow.Add(item.Title, lst_workflow);
                    }
                }
            }
              
            table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow);
            table_groupWorkFlow.ReloadData();
            //ClassMenu m1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            //ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Nhân sự" };
            //ClassMenu m3 = new ClassMenu() { ID = 2, section = 1, title = "Marketing" };
            //ClassMenu m4 = new ClassMenu() { ID = 3, section = 2, title = "Kinh doanh" };
            //ClassMenu m5 = new ClassMenu() { ID = 4, section = 3, title = "Kế toán" };

            //lst_menuItem.AddRange(new[] { m1, m2, m3, m4, m5 });
            //currentMenu = lst_menuItem[0];

            //string dataString1 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            //string dataString2 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmentvertical','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':7,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputworkrelated','DataSource':null,'ListProprety':null,'ID':4,'Title':'Quy trình / Công việc liên kết','Value':'','Enable':false}],'ID':8,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";

            //JArray json = JArray.Parse(dataString2);
            //lst_section = json.ToObject<List<ViewSection>>();

            //table_content_right.Source = new Control_TableSource(lst_section, this);
            //table_content_right.ReloadData();
        }
        private void LoadContent_bk()
        {
            //WorkFlowItemDemo item = new WorkFlowItemDemo() { ID = 0, iconUrl = "icon_learn_temp.png", title = "Đăng kí đi làm ngoài giờ", isExpand = true, workFlowID = 2 };
            //WorkFlowItemDemo item1 = new WorkFlowItemDemo() { ID = 1, iconUrl = "icon_pay_temp.png", title = "Thanh toán công tác phí", isExpand = true, workFlowID = 1 };
            //WorkFlowItemDemo item2 = new WorkFlowItemDemo() { ID = 2, iconUrl = "icon_recruitment_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 3 };
            //WorkFlowItemDemo item3 = new WorkFlowItemDemo() { ID = 3, iconUrl = "icon_training_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 4 };
            //WorkFlowItemDemo item4 = new WorkFlowItemDemo() { ID = 4, iconUrl = "", title = "Quy trình đào tạo", isExpand = true, workFlowID = 5 };

            //List<WorkFlowItemDemo> lst_item = new List<WorkFlowItemDemo>();
            //lst_item.AddRange(new[] { item, item1, item2, item3, item4, item2, item3, item4 });

            //List<WorkFlowItemDemo> lst_item1 = new List<WorkFlowItemDemo>();
            //lst_item1.AddRange(new[] { item1, item2 });

            //List<WorkFlowItemDemo> lst_item2 = new List<WorkFlowItemDemo>();
            //lst_item2.AddRange(new[] { item2, item1, item2 });

            //List<WorkFlowItemDemo> lst_item3 = new List<WorkFlowItemDemo>();
            //lst_item3.AddRange(new[] { item3, item1, item2, item3, item4, item1, item2 });

            //dict_groupWorkFlow.Add("Nhân sự", lst_item);
            //dict_groupWorkFlow.Add("Marketing", lst_item1);
            //dict_groupWorkFlow.Add("Kinh doanh", lst_item2);
            //dict_groupWorkFlow.Add("Kế toán", lst_item3);

            //table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow);

            //ClassMenu m1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            //ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Nhân sự" };
            //ClassMenu m3 = new ClassMenu() { ID = 2, section = 1, title = "Marketing" };
            //ClassMenu m4 = new ClassMenu() { ID = 3, section = 2, title = "Kinh doanh" };
            //ClassMenu m5 = new ClassMenu() { ID = 4, section = 3, title = "Kế toán" };

            //lst_menuItem.AddRange(new[] { m1, m2, m3, m4, m5 });
            //currentMenu = lst_menuItem[0];

            //string dataString1 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            //string dataString2 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmentvertical','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':7,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputworkrelated','DataSource':null,'ListProprety':null,'ID':4,'Title':'Quy trình / Công việc liên kết','Value':'','Enable':false}],'ID':8,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";

            //JArray json = JArray.Parse(dataString2);
            //lst_section = json.ToObject<List<ViewSection>>();

            //table_content_right.Source = new Control_TableSource(lst_section, this);
            //table_content_right.ReloadData();
        }
        #endregion

        #region event
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(3);
            appD.SlideMenuController.OpenLeft();
        }
        #endregion

        #region custom
        #region group work flow table source
        private class GroupWorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            List<string> lst_key = new List<string>();
            Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow { get; set; }
            BroadView parentView;
            const int HEIGHT_HEADER = 30;
            const int HEIGHT_ROW = 163;

            public GroupWorkFlow_TableSource(BroadView _parentview, Dictionary<string, List<BeanWorkflow>> _dict_todo)
            {
                parentView = _parentview;
                dict_groupWorkFlow = _dict_todo;
                GetListKey();
            }

            private void GetListKey()
            {
                lst_key = dict_groupWorkFlow.Keys.ToList();
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_groupWorkFlow.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var keyItem = lst_key[Convert.ToInt32(section)];
                var fistItem = dict_groupWorkFlow[keyItem][0];
                return 1;
                //return fistItem.isExpand ? 1 : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var keyItem = lst_key[indexPath.Section];
                var count = dict_groupWorkFlow[keyItem].Count % 5;
                var temp = dict_groupWorkFlow[keyItem].Count / 5;

                return (count > 0) ? (HEIGHT_ROW * (temp + 1)) + 25 : (HEIGHT_ROW * temp) + 25;//footer height: 25
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return HEIGHT_HEADER;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var keyItem = lst_key[Convert.ToInt32(section)];
                var fistItem = dict_groupWorkFlow[keyItem][0];

                UIView rooView = new UIView();
                rooView.Frame = new CGRect(0, 0, tableView.Frame.Width, HEIGHT_HEADER);
                rooView.BackgroundColor = UIColor.White;

                UILabel lbl_title = new UILabel()
                {
                    Font = UIFont.BoldSystemFontOfSize(14),
                    TextColor = UIColor.FromRGB(65, 85, 134)
                };

                UIImageView iv_left = new UIImageView();
                iv_left.ContentMode = UIViewContentMode.ScaleAspectFit;
                UIImage img_left = UIImage.FromFile("Icons/icon_arrow_down_white.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                iv_left.Image = img_left;
                iv_left.TintColor = UIColor.FromRGB(65, 80, 134);

                UIButton btn_action = new UIButton();
                btn_action.TouchUpInside += (sender, ev) =>
                {
                    //fistItem.isExpand = !fistItem.isExpand;
                    tableView.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
                };

                rooView.AddSubviews(new UIView[] { iv_left, lbl_title, btn_action });

                iv_left.Frame = new CGRect(0, 9, 12, 12);
                lbl_title.Frame = new CGRect(iv_left.Frame.Right + 10, 0, rooView.Frame.Width - (iv_left.Frame.Right + 10), rooView.Frame.Height);
                btn_action.Frame = new CGRect(0, 0, rooView.Bounds.Width, rooView.Bounds.Height);

                lbl_title.Text = keyItem;

                //if (fistItem.isExpand)
                //    iv_left.Transform = CGAffineTransform.MakeRotation(0);
                //else
                //    iv_left.Transform = CGAffineTransform.MakeRotation(-((nfloat)Math.PI / 2));

                return rooView;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var keyItem = lst_key[indexPath.Section];
                var lst_workFlow = dict_groupWorkFlow[keyItem];

                Custom_GroupWorkFlowCell cell = new Custom_GroupWorkFlowCell(cellIdentifier, parentView);
                cell.UpdateCell(lst_workFlow);

                return cell;
            }
        }
        #endregion
        #endregion
    }
}