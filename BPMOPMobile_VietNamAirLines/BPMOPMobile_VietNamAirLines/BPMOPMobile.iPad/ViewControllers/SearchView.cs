using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class SearchView : UIViewController
    {
        MainView mainView { get; set; }
        int currentIndexTome = 0; //0: den toi - 1: tu toi
        bool flag_tome;
        nfloat defaultConstraintHeightRoot;

        Custom_CalendarView custom_CalendarView;

        public List<ClassMenu> lst_trangthai;
        public ClassMenu TrangThaiSelected;
        public List<BeanAppStatus> lst_appStatus;
        private List<BeanAppStatus> lst_appStatus_selected;
        List<ClassMenu> lst_dueDateMenu;
        ClassMenu DuedateSelected;
        string hint_status = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
        string hint_fromDate = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");
        string hint_toDate = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");
        bool isDangXuLy = true;
        //value form default tome
        DateTime tome_fromDate_default;
        DateTime tome_toDate_default;

        DateTime tome_fromDateSelected;
        DateTime tome_toDateSelected;


        //value form default from me
        DateTime fromme_fromDate_default;
        DateTime fromme_toDate_default;

        DateTime fromme_fromDateSelected;
        DateTime fromme_toDateSelected;
        int fromme_status_selected_index = 1;
        int fromme_status_selected_index_default = 1;
        int fromme_duedate_selected_index = 0;
        int fromme_duedate_selected_index_default = 0;

        public SearchView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            flag_tome = true;
            defaultConstraintHeightRoot = constraintBottom.Constant;
            CloseCustomCalendar();
            ViewConfiguration();
            LoadTrangThaiCategory();
            LoadStatusCategory();
            LoadDueDateCategory();
            SetLangTitle();

            LoadContentToMe();
            //dang xu ly
            if (TrangThaiSelected.ID == 1)
            {
                BT_status.Enabled = false;
                BT_status.BackgroundColor = UIColor.White.ColorWithAlpha(0.7f);
            }
            else
            {
                BT_status.Enabled = true;
                BT_status.BackgroundColor = UIColor.Clear;
            }

            #region delegate
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_tome.TouchUpInside += BT_tome_TouchUpInside;
            BT_fromme.TouchUpInside += BT_fromme_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            tf_keyword.EditingChanged += Tf_keyword_EditingChanged;

            #region Tome
            BT_filter_tome_from.TouchUpInside += BT_filter_tome_from_TouchUpInside;
            BT_filter_tome_to.TouchUpInside += BT_filter_tome_to_TouchUpInside;

            //trang thai
            BT_trangthai.TouchUpInside += BT_trangthai_TouchUpInside;

            BT_status.TouchUpInside += BT_status_TouchUpInside;
            BT_duedate.TouchUpInside += BT_duedate_TouchUpInside;
            #endregion

            #region From me
            BT_filter_fromme_from.TouchUpInside += BT_filter_fromme_from_TouchUpInside;
            BT_filter_fromme_to.TouchUpInside += BT_filter_fromme_to_TouchUpInside;
            BT_fromme_status.TouchUpInside += BT_fromme_status_TouchUpInside; ;
            BT_fromme_duedate.TouchUpInside += BT_fromme_duedate_TouchUpInside;
            #endregion
            #endregion
        }

        #endregion

        #region private  - public method
        public void setContent(MainView parent)
        {
            mainView = parent;
        }
        private void ViewConfiguration()
        {
            BT_accept.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_reset_filter.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

            custom_CalendarView = Custom_CalendarView.Instance;
            custom_CalendarView.viewController = this;

            //Tome
            view_trangthai.Layer.BorderWidth = 1;
            view_trangthai.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_trangthai.Layer.CornerRadius = 3;
            view_trangthai.ClipsToBounds = true;

            view_status.Layer.BorderWidth = 1;
            view_status.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_status.Layer.CornerRadius = 3;
            view_status.ClipsToBounds = true;

            view_duedate.Layer.BorderWidth = 1;
            view_duedate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_duedate.Layer.CornerRadius = 3;
            view_duedate.ClipsToBounds = true;

            view_filter_tome_from.Layer.BorderWidth = 1;
            view_filter_tome_from.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_filter_tome_from.Layer.CornerRadius = 3;
            view_filter_tome_from.ClipsToBounds = true;

            view_filter_tome_to.Layer.BorderWidth = 1;
            view_filter_tome_to.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_filter_tome_to.Layer.CornerRadius = 3;
            view_filter_tome_to.ClipsToBounds = true;

            //From Me
            view_fromMe_status.Layer.BorderWidth = 1;
            view_fromMe_status.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_fromMe_status.Layer.CornerRadius = 3;
            view_fromMe_status.ClipsToBounds = true;

            view_fromMe_duedate.Layer.BorderWidth = 1;
            view_fromMe_duedate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_fromMe_duedate.Layer.CornerRadius = 3;
            view_fromMe_duedate.ClipsToBounds = true;

            view_filterFormMe_From.Layer.BorderWidth = 1;
            view_filterFormMe_From.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_filterFormMe_From.Layer.CornerRadius = 3;
            view_filterFormMe_From.ClipsToBounds = true;

            view_filterFormMe_To.Layer.BorderWidth = 1;
            view_filterFormMe_To.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_filterFormMe_To.Layer.CornerRadius = 3;
            view_filterFormMe_To.ClipsToBounds = true;


        }
        private void SetLangTitle()
        {

            if (tome_fromDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("dd/MM/yyyy");
            }
            else
                lbl_filter_tome_from.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");

            if (tome_toDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_tome_to.Text = tome_toDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_tome_to.Text = tome_toDateSelected.ToString("dd/MM/yyyy");
            }
            else
                lbl_filter_tome_to.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");

            CmmIOSFunction.SetLangToView(this.View);
        }
        private void LoadStatusCategory()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query_worflowCategory = @"SELECT * FROM BeanAppStatus";
            lst_appStatus = conn.Query<BeanAppStatus>(query_worflowCategory);
        }

        private void LoadTrangThaiCategory()
        {
            lst_trangthai = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý") };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý") };

            lst_trangthai.AddRange(new[] { m1, m2 });
        }

        private void LoadDueDateCategory()
        {
            lst_dueDateMenu = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_TODAY1", "Trong ngày") };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_OVERDUE", "Trễ hạn") };

            lst_dueDateMenu.AddRange(new[] { m1, m2, m3 });
        }
        private void LoadContentToMe()
        {
            string temp = "";
            string res_title = "";

            // TrangThaiSelected = null => Default => ID = 1 : Dang Xu Ly
            TrangThaiSelected = lst_trangthai.Where(s => s.isSelected == true).FirstOrDefault();
            if (TrangThaiSelected != null)
            {
                lbl_trangthai.Text = DuedateSelected.title;
            }
            else
            {
                TrangThaiSelected = lst_trangthai[0]; // Tat ca
                TrangThaiSelected.isSelected = true;
                lbl_trangthai.Text = TrangThaiSelected.title;
            }

            //lst_appStatus_selected = null => Default => ID = 4 : Dang thuc hien
            lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
            if (lst_appStatus_selected == null || lst_appStatus_selected.Count == 0)
            {
                lst_appStatus_selected = new List<BeanAppStatus>();
                lst_appStatus_selected.Add(lst_appStatus.Where(s => s.ID == 4).FirstOrDefault());

                var res = lst_appStatus.Where(i => i.ID == lst_appStatus_selected[0].ID).FirstOrDefault();
                res.IsSelected = true;
            }

            if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
            {
                if (lst_appStatus_selected.Count == 1)
                {
                    lbl_status.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_status.TextColor = UIColor.Black;

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_status.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_status.Text = lst_appStatus_selected[0].Title;
                }
                else if (lst_appStatus_selected.Count > 1)
                {
                    lbl_status.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_status.TextColor = UIColor.Black;

                    List<string> lst_titleStatus = new List<string>();

                    foreach (var item in lst_appStatus_selected)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lst_titleStatus.Add(item.TitleEN);
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            lst_titleStatus.Add(item.Title);
                    }

                    temp = string.Join(", ", lst_titleStatus);
                    var widthStatus = StringExtensions.MeasureString(temp, 14).Width + 50;

                    nfloat max_width = 0;
                    for (int i = 0; i < lst_titleStatus.Count; i++)
                    {
                        max_width += StringExtensions.MeasureString(lst_titleStatus[i], 14).Width + 10;
                        if (max_width > lbl_status.Frame.Width)
                        {
                            lbl_status.Text = res_title + " +" + (lst_titleStatus.Count - i);
                            break;
                        }
                        else
                        {
                            res_title += lst_titleStatus[i] + ", ";
                            lbl_status.Text = temp.TrimEnd(',');
                        }
                    }

                }
                else
                {
                    lbl_status.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                    lbl_status.TextColor = UIColor.FromRGB(229, 229, 229);
                    lbl_status.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                }

            }
            else
            {
                lbl_status.TextColor = UIColor.FromRGB(229, 229, 229);
                lbl_status.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                lbl_status.Text = hint_status;
            }

            // DuedateSelected = null => Default => ID = 1 : Tat ca
            DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
            if (DuedateSelected != null)
            {
                lbl_dueDate.Text = DuedateSelected.title;
            }
            else
            {
                DuedateSelected = lst_dueDateMenu[0]; // Tat ca
                DuedateSelected.isSelected = true;
                lbl_dueDate.Text = DuedateSelected.title;
            }

            // Check FromDate
            if (tome_fromDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("dd/MM/yyyy");

                lbl_filter_tome_from.TextColor = UIColor.Black;
                lbl_filter_tome_from.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                lbl_filter_tome_from.Text = hint_fromDate;
                lbl_filter_tome_from.TextColor = UIColor.FromRGB(153, 153, 153);
                lbl_filter_tome_from.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }

            //Check ToDate
            if (tome_toDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_tome_to.Text = tome_toDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_tome_to.Text = tome_toDateSelected.ToString("dd/MM/yyyy");

                lbl_filter_tome_to.TextColor = UIColor.Black;
                lbl_filter_tome_to.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                lbl_filter_tome_to.Text = hint_toDate;
                lbl_filter_tome_to.TextColor = UIColor.FromRGB(153, 153, 153);
                lbl_filter_tome_to.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }
        }
        private void LoadContent_FromMe()
        {
            string temp = "";
            string res_title = "";

            //lst_appStatus_selected = null => Default => ID = 4 : Dang thuc hien
            lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
            if (lst_appStatus_selected == null || lst_appStatus_selected.Count == 0)
            {
                lst_appStatus_selected = new List<BeanAppStatus>();
                lst_appStatus_selected.Add(lst_appStatus.Where(s => s.ID == 4).FirstOrDefault());

                var res = lst_appStatus.Where(i => i.ID == lst_appStatus_selected[0].ID).FirstOrDefault();
                res.IsSelected = true;
            }

            if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
            {
                if (lst_appStatus_selected.Count == 1)
                {
                    lbl_fromMe_status.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_fromMe_status.TextColor = UIColor.Black;

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_fromMe_status.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_fromMe_status.Text = lst_appStatus_selected[0].Title;
                }
                else if (lst_appStatus_selected.Count > 1)
                {
                    lbl_fromMe_status.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_fromMe_status.TextColor = UIColor.Black;

                    List<string> lst_titleStatus = new List<string>();

                    foreach (var item in lst_appStatus_selected)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lst_titleStatus.Add(item.TitleEN);
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            lst_titleStatus.Add(item.Title);
                    }

                    temp = string.Join(", ", lst_titleStatus);
                    var widthStatus = StringExtensions.MeasureString(temp, 14).Width + 50;

                    nfloat max_width = 0;
                    for (int i = 0; i < lst_titleStatus.Count; i++)
                    {
                        max_width += StringExtensions.MeasureString(lst_titleStatus[i], 14).Width + 10;
                        if (max_width > lbl_fromMe_status.Frame.Width)
                        {
                            lbl_fromMe_status.Text = res_title + " +" + (lst_titleStatus.Count - i);
                            break;
                        }
                        else
                        {
                            res_title += lst_titleStatus[i] + ", ";
                            lbl_fromMe_status.Text = temp.TrimEnd(',');
                        }
                    }

                }
                else
                {
                    lbl_fromMe_status.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                    lbl_fromMe_status.TextColor = UIColor.FromRGB(229, 229, 229);
                    lbl_fromMe_status.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                }

            }
            else
            {
                lbl_fromMe_status.TextColor = UIColor.FromRGB(229, 229, 229);
                lbl_fromMe_status.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                lbl_fromMe_status.Text = hint_status;
            }

            // DuedateSelected = null => Default => ID = 1 : Tat ca
            DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
            if (DuedateSelected != null)
            {
                lbl_fromMe_dueDate.Text = DuedateSelected.title;
            }
            else
            {
                DuedateSelected = lst_dueDateMenu[0]; // Tat ca
                DuedateSelected.isSelected = true;
                lbl_fromMe_dueDate.Text = DuedateSelected.title;
            }

            // Check FromDate
            if (tome_fromDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_fromme_from.Text = tome_fromDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_fromme_from.Text = tome_fromDateSelected.ToString("dd/MM/yyyy");

                lbl_filter_fromme_from.TextColor = UIColor.Black;
                lbl_filter_fromme_from.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                lbl_filter_fromme_from.Text = hint_fromDate;
                lbl_filter_fromme_from.TextColor = UIColor.FromRGB(153, 153, 153);
                lbl_filter_fromme_from.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }

            //Check ToDate
            if (tome_toDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_filter_fromme_to.Text = tome_toDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_filter_fromme_to.Text = tome_toDateSelected.ToString("dd/MM/yyyy");

                lbl_filter_fromme_to.TextColor = UIColor.Black;
                lbl_filter_fromme_to.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                lbl_filter_fromme_to.Text = hint_toDate;
                lbl_filter_fromme_to.TextColor = UIColor.FromRGB(153, 153, 153);
                lbl_filter_fromme_to.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }
        }
        public void BT_switch(bool _flag_tome)
        {
            flag_tome = _flag_tome;

            if (custom_CalendarView.Superview != null)
                custom_CalendarView.RemoveFromSuperview();

            if (flag_tome)
            {
                BT_tome.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_tome.BackgroundColor = UIColor.FromRGB(65, 84, 134);

                BT_fromme.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_fromme.BackgroundColor = UIColor.FromRGB(245, 245, 245);

                view_filterTome.Hidden = false;
                view_filterFromme.Hidden = true;
                LoadContentToMe();
            }
            else
            {
                BT_tome.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_tome.BackgroundColor = UIColor.FromRGB(245, 245, 245);

                BT_fromme.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_fromme.BackgroundColor = UIColor.FromRGB(65, 84, 134);

                view_filterTome.Hidden = true;
                view_filterFromme.Hidden = false;
                LoadContent_FromMe();
            }
        }
        private void SetStatusButton(bool isActive, UIButton btn)
        {
            if (isActive)
            {
                btn.SetTitleColor(UIColor.White, UIControlState.Normal);
                btn.BackgroundColor = UIColor.FromRGB(65, 80, 134);
            }
            else
            {
                btn.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                btn.BackgroundColor = UIColor.FromRGB(245, 245, 245);
            }
        }
        public void CloseCustomCalendar()
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null)
            {
                custom_CalendarView.RemoveFromSuperview();
                custom_CalendarView.reset();
            }
        }

        private void ToggleMenuTrangThai()
        {
            Custom_DuedateCategory custom_DuedateCategory = Custom_DuedateCategory.Instance;
            if (custom_DuedateCategory.Superview != null)
                custom_DuedateCategory.RemoveFromSuperview();
            else
            {
                custom_DuedateCategory.ItemNoIcon = false;
                custom_DuedateCategory.viewController = this;
                custom_DuedateCategory.LBL_inputView = lbl_trangthai;
                custom_DuedateCategory.InitFrameView(new CGRect(view_trangthai.Frame.X + 10, view_trangthai.Frame.Bottom + 5, view_trangthai.Frame.Width, lst_trangthai.Count * 40), false);
                custom_DuedateCategory.AddShadowForView();
                custom_DuedateCategory.ClassMenu_selected = TrangThaiSelected;

                custom_DuedateCategory.ListClassMenu = lst_trangthai;
                custom_DuedateCategory.TableLoadData();

                view_content.AddSubview(custom_DuedateCategory);
                view_content.BringSubviewToFront(custom_DuedateCategory);
            }
        }

        private void ToggleMenuStatus()
        {
            Custom_AppStatusCategory custom_AppStatusCategory = Custom_AppStatusCategory.Instance;
            if (custom_AppStatusCategory.Superview != null)
                custom_AppStatusCategory.RemoveFromSuperview();
            else
            {
                custom_AppStatusCategory.ItemNoIcon = false;
                custom_AppStatusCategory.viewController = this;
                if (flag_tome)
                {
                    custom_AppStatusCategory.LBL_inputView = lbl_status;
                    custom_AppStatusCategory.InitFrameView(new CGRect(view_status.Frame.X + 10, view_status.Frame.Bottom + 5, view_status.Frame.Width, 230));
                }
                else
                {
                    custom_AppStatusCategory.LBL_inputView = lbl_fromMe_status;
                    custom_AppStatusCategory.InitFrameView(new CGRect(view_fromMe_status.Frame.X + 10, view_fromMe_status.Frame.Bottom + 5, view_fromMe_status.Frame.Width, 230));
                }
                custom_AppStatusCategory.AddShadowForView();
                custom_AppStatusCategory.ListAppStatus = lst_appStatus;
                custom_AppStatusCategory.TableLoadData();

                view_content.AddSubview(custom_AppStatusCategory);
                view_content.BringSubviewToFront(custom_AppStatusCategory);
            }
        }

        private void ToggleMenuDueDate()
        {
            Custom_DuedateCategory custom_DuedateCategory = Custom_DuedateCategory.Instance;
            if (custom_DuedateCategory.Superview != null)
                custom_DuedateCategory.RemoveFromSuperview();
            else
            {
                custom_DuedateCategory.ItemNoIcon = false;
                custom_DuedateCategory.viewController = this;
                if (flag_tome)
                {
                    custom_DuedateCategory.LBL_inputView = lbl_dueDate;
                    custom_DuedateCategory.InitFrameView(new CGRect(view_duedate.Frame.X + 10, view_duedate.Frame.Bottom + 5, view_duedate.Frame.Width, 150), false);
                }
                else
                {
                    custom_DuedateCategory.LBL_inputView = lbl_fromMe_dueDate;
                    custom_DuedateCategory.InitFrameView(new CGRect(view_fromMe_duedate.Frame.X + 10, view_fromMe_duedate.Frame.Bottom + 5, view_fromMe_duedate.Frame.Width, 150), false);
                }

                custom_DuedateCategory.AddShadowForView();
                custom_DuedateCategory.ClassMenu_selected = DuedateSelected;

                custom_DuedateCategory.ListClassMenu = lst_dueDateMenu;
                custom_DuedateCategory.TableLoadData();

                view_content.AddSubview(custom_DuedateCategory);
                view_content.BringSubviewToFront(custom_DuedateCategory);
            }
        }

        public void CloseDueDateCateInstance()
        {
            Custom_DuedateCategory custom_DuedateCategory = Custom_DuedateCategory.Instance;
            if (custom_DuedateCategory.Superview != null)
                custom_DuedateCategory.RemoveFromSuperview();

            if (TrangThaiSelected.ID == 1)
            {
                BT_status.Enabled = false;
                BT_status.BackgroundColor = UIColor.White.ColorWithAlpha(0.7f);
            }
            else
            {
                BT_status.Enabled = true;
                BT_status.BackgroundColor = UIColor.Clear;
            }
        }

        public void CloseCalendarInstance()
        {

            BT_filter_tome_from.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_from.Layer.BorderWidth = 0;

            BT_filter_tome_to.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_to.Layer.BorderWidth = 0;

            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
        }

        #region to me
        private bool Check_ToMeFiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        {
            lst_appStatus_selected = lst_appStatus.Where(c => c.IsSelected == true).ToList();

            if (lst_appStatus_selected != null && (lst_appStatus_selected.Count != 1 || lst_appStatus_selected[0].ID != 4))
                return true;

            DuedateSelected = lst_dueDateMenu.Where(c => c.isSelected == true).FirstOrDefault();
            if (DuedateSelected.ID != 1) //!= tat ca
                return true;

            if (tome_fromDate_default != _fromDateSelected)
                return true;

            if (tome_toDate_default != _toDateSelected)
                return true;

            return false;
        }

        #endregion

        #region from me
        private bool FromMe_Check_FiltedStatus()
        {
            if (fromme_status_selected_index_default != fromme_status_selected_index)
                return false;

            if (fromme_duedate_selected_index_default != fromme_duedate_selected_index)
                return false;

            if (fromme_fromDate_default != fromme_fromDateSelected)
                return false;

            if (fromme_toDate_default != fromme_toDateSelected)
                return false;

            return true;
        }

        #endregion
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }
        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            if (currentIndexTome == 0) //to me
            {

                if (lbl_filter_tome_from.Text != hint_fromDate)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tome_fromDateSelected = DateTime.Parse(lbl_filter_tome_from.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tome_fromDateSelected = DateTime.Parse(lbl_filter_tome_from.Text, new CultureInfo("vi", false));
                }

                if (lbl_filter_tome_to.Text != hint_toDate)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tome_toDateSelected = DateTime.Parse(lbl_filter_tome_to.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tome_toDateSelected = DateTime.Parse(lbl_filter_tome_to.Text, new CultureInfo("vi", false));
                }

                if (tome_fromDateSelected > tome_toDateSelected)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");

                    BT_filter_tome_from.Layer.BorderColor = UIColor.Red.CGColor;
                    BT_filter_tome_from.Layer.BorderWidth = 1;

                    BT_filter_tome_to.Layer.BorderColor = UIColor.Red.CGColor;
                    BT_filter_tome_to.Layer.BorderWidth = 1;
                }
                else
                {
                    bool filter = (Check_ToMeFiltedStatus(tome_fromDateSelected, tome_toDateSelected));
                    mainView.NavigateToDetailsFromSearch(true, TrangThaiSelected.ID, filter, lst_appStatus, lst_dueDateMenu, tome_fromDateSelected, tome_toDateSelected, tf_keyword.Text);
                    this.DismissModalViewController(true);
                }
            }
            else
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    //fromme_fromDateSelected = DateTime.Parse(lbl_filter_fromme_from.Text, new CultureInfo("en", false));
                    //fromme_toDateSelected = DateTime.Parse(lbl_filter_fromme_to.Text, new CultureInfo("en", false));
                }
                else if (CmmVariable.SysConfig.LangCode == "1066")
                {
                    //fromme_fromDateSelected = DateTime.Parse(lbl_filter_fromme_from.Text, new CultureInfo("vi", false));
                    //fromme_toDateSelected = DateTime.Parse(lbl_filter_fromme_to.Text, new CultureInfo("vi", false));
                }

                if (fromme_fromDateSelected > fromme_toDateSelected)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");

                    //BT_filter_fromme_from.Layer.BorderColor = UIColor.Red.CGColor;
                    //BT_filter_fromme_from.Layer.BorderWidth = 1;

                    //BT_filter_fromme_to.Layer.BorderColor = UIColor.Red.CGColor;
                    //BT_filter_fromme_to.Layer.BorderWidth = 1;
                }
                else
                {
                    bool filter = false;
                    if (FromMe_Check_FiltedStatus())
                        filter = false;
                    else
                        filter = true;

                    mainView.NavigateToDetailsFromSearch(false, 0, filter, lst_appStatus, lst_dueDateMenu, fromme_fromDateSelected, fromme_toDateSelected, tf_keyword.Text);
                    this.DismissModalViewController(true);
                }
            }

            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == lbl_filter_tome_from)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
        }
        private void BT_tome_TouchUpInside(object sender, EventArgs e)
        {
            currentIndexTome = 0;
            BT_switch(true);
        }
        private void BT_fromme_TouchUpInside(object sender, EventArgs e)
        {
            currentIndexTome = 1;
            BT_switch(false);
        }
        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {
            tf_keyword.Text = "";

            LoadContentToMe();
        }
        private void Tf_keyword_EditingChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tf_keyword.Text))
            {
                tf_keyword.Font = UIFont.FromName("ArialMT", 14f);
                tf_keyword.TextColor = UIColor.Black;
            }
            else
            {
                tf_keyword.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                tf_keyword.TextColor = UIColor.LightGray;
            }
        }

        #region To me
        private void BT_filter_tome_from_TouchUpInside(object sender, EventArgs e)
        {
            BT_filter_tome_to.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_from.Layer.BorderWidth = 1;

            BT_filter_tome_to.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_to.Layer.BorderWidth = 0;

            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == lbl_filter_tome_from)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
            else
            {
                custom_CalendarView.inputView = lbl_filter_tome_from;
                custom_CalendarView.viewController = this;
                custom_CalendarView.InitFrameView(new CGRect(view_filter_tome_from.Frame.X, view_filter_tome_from.Frame.Bottom, view_filter_tome_from.Frame.Width, 260));
                custom_CalendarView.SetUpDate();

                view_filterTome.AddSubview(custom_CalendarView);
                constraintBottom.Constant = defaultConstraintHeightRoot - 100;
            }
        }
        private void BT_filter_tome_to_TouchUpInside(object sender, EventArgs e)
        {
            BT_filter_tome_to.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_to.Layer.BorderWidth = 1;

            BT_filter_tome_from.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_filter_tome_from.Layer.BorderWidth = 0;

            //Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == lbl_filter_tome_to)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
            else
            {
                custom_CalendarView.inputView = lbl_filter_tome_to;
                custom_CalendarView.viewController = this;
                custom_CalendarView.InitFrameView(new CGRect(view_filter_tome_to.Frame.X, view_filter_tome_to.Frame.Bottom, view_filter_tome_to.Frame.Width, 260));
                custom_CalendarView.SetUpDate();

                view_filterTome.AddSubview(custom_CalendarView);
                constraintBottom.Constant = defaultConstraintHeightRoot - 100;
            }
        }
        private void BT_trangthai_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuTrangThai();
        }
        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuStatus();
        }
        private void BT_duedate_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuDueDate();
        }
        #endregion

        #region tab_fromMe
        private void BT_filter_fromme_from_TouchUpInside(object sender, EventArgs e)
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == lbl_filter_fromme_from)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
            else
            {
                custom_CalendarView.viewController = this;
                custom_CalendarView.inputView = lbl_filter_fromme_from;
                view_filterFromme.AddSubview(custom_CalendarView);
                custom_CalendarView.InitFrameView(new CGRect(view_filterFormMe_From.Frame.X, view_filterFormMe_From.Frame.Bottom, view_filterFormMe_From.Frame.Width, 260));
                custom_CalendarView.SetUpDate();
                constraintBottom.Constant = defaultConstraintHeightRoot - 100;
            }
        }
        private void BT_filter_fromme_to_TouchUpInside(object sender, EventArgs e)
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == lbl_filter_fromme_to)
            {
                custom_CalendarView.RemoveFromSuperview();
                constraintBottom.Constant = defaultConstraintHeightRoot;
            }
            else
            {
                custom_CalendarView.viewController = this;
                custom_CalendarView.inputView = lbl_filter_fromme_to;
                view_filterFromme.AddSubview(custom_CalendarView);
                custom_CalendarView.InitFrameView(new CGRect(view_filterFormMe_To.Frame.X, view_filterFormMe_To.Frame.Bottom, view_filterFormMe_To.Frame.Width, 260));
                custom_CalendarView.SetUpDate();
                constraintBottom.Constant = defaultConstraintHeightRoot - 100;
            }
        }
        private void BT_fromme_status_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuStatus();
        }
        private void BT_fromme_duedate_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuDueDate();
        }

        #endregion

        #endregion

    }
}

