using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class ViewSearchV2 : UIViewController
    {
        AppDelegate appD { get; set; }
        MainView mainView { get; set; }
        public List<ClassMenu> lst_trangthai;
        public ClassMenu TrangThaiSelected;
        public List<BeanAppStatus> lst_appStatus;
        private List<BeanAppStatus> lst_appStatus_selected;
        List<ClassMenu> lst_dueDateMenu;
        bool flag_tome;
        ClassMenu DuedateSelected;
        string hint_status = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
        string condition_AppStatus = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_ENABLE");
        string condition_DangXuLy = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DANGXULY");
        string condition_DaXuLy = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DAXULY");
        string condition_FromMe = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_FROMME");
        string hint_fromDate = "";
        string hint_toDate = "";

        //value Date tome
        DateTime fromDateSelected;
        DateTime toDateSelected;
        DateTime fromDate_default;
        DateTime toDate_default;

        //value Date from me
        DateTime fromme_fromDate_default;
        DateTime fromme_toDate_default;

        DateTime fromme_fromDateSelected;
        DateTime fromme_toDateSelected;


        public ViewSearchV2(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            flag_tome = true;
            this.PreferredContentSize = new CGSize(510, 510);
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            mainView = appD.mainView;

            //set value default filter date = 30 days
            toDateSelected = DateTime.Now.Date;
            toDate_default = toDateSelected;

            fromDateSelected = toDateSelected.AddDays(-CmmVariable.M_DataFilterDefaultDays);
            fromDate_default = fromDateSelected;

            LoadTrangThaiCategory();
            LoadStatusCategory();
            LoadDueDateCategory();
            SetLangTitle();
            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_ToMe.TouchUpInside += BT_ToMe_TouchUpInside;
            BT_fromMe.TouchUpInside += BT_fromMe_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_tome_trangthai.TouchUpInside += BT_tome_trangthai_TouchUpInside;
            BT_tinhtrang_tome.TouchUpInside += BT_tinhtrang_tome_TouchUpInside;
            BT_hanxuly_tome.TouchUpInside += BT_hanxuly_tome_TouchUpInside;
            BT_tome_fromdate.TouchUpInside += BT_tome_fromdate_TouchUpInside;
            BT_tome_todate.TouchUpInside += BT_tome_todate_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            tf_keyword.EditingChanged += delegate
            {
                if (!string.IsNullOrEmpty(tf_keyword.Text))
                    tf_keyword.Font = UIFont.FromName("ArialMT", 14);
                else
                    tf_keyword.Font = UIFont.FromName("Arial - ItalicMT", 14f);
            };
            #endregion
        }

        #endregion

        #region private - public method
        public void setContent()
        {

        }

        private void ViewConfiguration()
        {
            BT_accept.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_reset_filter.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
        }

        private void LoadTrangThaiCategory()
        {
            lst_trangthai = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý") };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý") };

            lst_trangthai.AddRange(new[] { m1, m2 });
        }
        public void ReloadTrangThaiCategory(List<ClassMenu> _newList)
        {
            lst_trangthai = _newList;
            if (flag_tome)
            {
                //Dang xu ly
                if (TrangThaiSelected.ID == 1)
                    DangXuLy_Default();
                //Da xu ly
                else if (TrangThaiSelected.ID == 2)
                    DaXuLy_Default();

                LoadContent();
            }
            else
                LoadContent_FromMe();
        }

        private void LoadStatusCategory()
        {
            if (lst_appStatus == null || lst_appStatus.Count == 0)
            {
                var condition = condition_AppStatus;
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus WHERE ID IN ({0})", condition);
                lst_appStatus = conn.Query<BeanAppStatus>(query_worflowCategory);
            }
        }
        public void ReloadTinhtrangCategory(List<BeanAppStatus> _newList)
        {
            //string temp = "";
            lst_appStatus = _newList;
            lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
            if (lst_appStatus_selected != null)
            {
                if (lst_appStatus_selected.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                }
                else if (lst_appStatus_selected.Count > 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;


                    lbl_tinhtrang.Text = lbl_tinhtrang.Text + " +" + (lst_appStatus_selected.Count - 1).ToString();
                }
                else
                {
                    lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
            }
            else
            {
                //TrangThaiSelected = lst_trangthai[0]; // Dang xu ly
                //TrangThaiSelected.isSelected = true;
                lbl_tinhtrang.Text = TrangThaiSelected.title;
            }
        }

        private void LoadDueDateCategory()
        {
            lst_dueDateMenu = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_TODAY1", "Trong ngày") };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_OVERDUE", "Trễ hạn") };

            lst_dueDateMenu.AddRange(new[] { m1, m2, m3 });
        }
        public void ReloadDueDateCategory(List<ClassMenu> _newList)
        {
            lst_dueDateMenu = _newList;
            DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
            if (DuedateSelected != null)
            {
                lbl_hanxuly.Text = DuedateSelected.title;
            }
            else
            {
                DuedateSelected = lst_trangthai[0]; // Dang xu ly
                DuedateSelected.isSelected = true;
                lbl_hanxuly.Text = DuedateSelected.title;
            }
        }

        private void SetLangTitle()
        {

            //if (tome_fromDateSelected.Year != 1)
            //{
            //    if (CmmVariable.SysConfig.LangCode == "1066")
            //        lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("dd/MM/yyyy");
            //    else
            //        lbl_filter_tome_from.Text = tome_fromDateSelected.ToString("MM/dd/yyyy");
            //}
            //else
            //    lbl_filter_tome_from.Text = CmmFunction.GetTitle("TEXT_FROMDATE", "Từ ngày");

            //if (tome_toDateSelected.Year != 1)
            //{
            //    if (CmmVariable.SysConfig.LangCode == "1066")
            //        lbl_filter_tome_to.Text = tome_toDateSelected.ToString("dd/MM/yyyy");
            //    else
            //        lbl_filter_tome_to.Text = tome_toDateSelected.ToString("MM/dd/yyyy");
            //}
            //else
            //    lbl_filter_tome_to.Text = CmmFunction.GetTitle("TEXT_TODATE", "Đến ngày");

            tf_keyword.Placeholder = CmmFunction.GetTitle("TEXT_CONTENT", "Nội dung");
            CmmIOSFunction.SetLangToView(this.View);
        }

        public void BT_switch(bool _flag_tome)
        {
            flag_tome = _flag_tome;
            lbl_fromdate.Text = string.Empty;
            lbl_todate.Text = string.Empty;

            if (flag_tome)
            {
                BT_ToMe.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_ToMe.BackgroundColor = UIColor.White;

                BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_fromMe.BackgroundColor = UIColor.Clear;

                view_trangthai.Hidden = false;
                constraintY_tinhtrang.Constant = 86;

                LoadContent();
            }
            else
            {
                BT_ToMe.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_ToMe.BackgroundColor = UIColor.Clear;

                BT_fromMe.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_fromMe.BackgroundColor = UIColor.White;

                view_trangthai.Hidden = true;
                view_tinhtrang_tome.Hidden = false;
                constraintHeight_groupTinhtrang.Constant = 92;
                constraintY_tinhtrang.Constant = 20;
                BT_tinhtrang_tome.BackgroundColor = UIColor.Clear;

                LoadContent_FromMe();
            }
        }

        #region tome
        private void LoadContent()
        {
            try
            {
                //string temp = "";
                //string res_title = "";

                // TrangThaiSelected = null => Default => ID = 1 : Dang Xu Ly
                TrangThaiSelected = lst_trangthai.Where(s => s.isSelected == true).FirstOrDefault();

                if (TrangThaiSelected != null)
                {
                    lbl_trangthai_tome.Text = TrangThaiSelected.title;
                }
                else
                {
                    TrangThaiSelected = lst_trangthai[0]; // Dang xu ly
                    TrangThaiSelected.isSelected = true;
                    lbl_trangthai_tome.Text = TrangThaiSelected.title;
                }

                //Trang thai Dang xu ly => Tinh trang => Dang thuc hien va khong cho chon
                if (TrangThaiSelected.ID == 1)
                {
                    view_tinhtrang_tome.Hidden = true;
                    constraintHeight_groupTinhtrang.Constant = 46;

                    var condition_dangXL = condition_DangXuLy.Split(',');
                    //cap nhat lai tat ca IsSelected = false
                    lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
                    //gan lai gia tri IsSeleted theo condition_dangXL
                    lst_appStatus.Where(f => condition_dangXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
                    //lay lai gia tri lst_appStatus_selected
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                }
                else
                {
                    view_tinhtrang_tome.Hidden = false;
                    constraintHeight_groupTinhtrang.Constant = 92;

                    var condition_daXL = condition_DaXuLy.Split(',');
                    //cap nhat lai tat ca IsSelected = false
                    lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
                    //gan lai gia tri IsSeleted theo condition_daXL
                    lst_appStatus.Where(f => condition_daXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
                    //lay lai gia tri lst_appStatus_selected
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                }


                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                {
                    if (lst_appStatus_selected.Count == 1)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                        else
                            lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                    }
                    else if (lst_appStatus_selected.Count > 1)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        //tat ca
                        if (lst_appStatus_selected.Count == lst_appStatus.Count)
                            lbl_tinhtrang.Text = hint_status;
                        else
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                            else
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;

                            lbl_tinhtrang.Text = lbl_tinhtrang.Text + " +" + (lst_appStatus_selected.Count - 1).ToString();
                        }
                    }
                }
                else
                {
                    lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
                    lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                    lbl_tinhtrang.Text = hint_status;
                }

                // DuedateSelected = null => Default => ID = 1 : Tat ca
                DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
                if (DuedateSelected != null)
                {
                    lbl_hanxuly.Text = DuedateSelected.title;
                }
                else
                {
                    DuedateSelected = lst_dueDateMenu[0]; // Tat ca
                    DuedateSelected.isSelected = true;
                    lbl_hanxuly.Text = DuedateSelected.title;
                }

                // Check FromDate
                if (fromDateSelected.Year != 1)
                {

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
                    else
                        lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");

                    lbl_fromdate.TextColor = UIColor.Black;
                    lbl_fromdate.Font = UIFont.FromName("ArialMT", 14f);
                }
                else
                {
                    //lbl_fromdate.Text = hint_fromDate;
                    //lbl_fromdate.TextColor = UIColor.FromRGB(153, 153, 153);
                    //lbl_fromdate.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                }

                //Check ToDate
                if (toDateSelected.Year != 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_todate.Text = toDateSelected.ToString("MM/dd/yyyy");
                    else
                        lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");

                    lbl_todate.TextColor = UIColor.Black;
                    lbl_todate.Font = UIFont.FromName("ArialMT", 14f);
                }
                else
                {
                    //lbl_todate.Text = hint_toDate;
                    //lbl_todate.TextColor = UIColor.FromRGB(153, 153, 153);
                    //lbl_todate.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ViewSearchV2 - LoadContent_ToMe - ERR: " + ex.ToString());
            }
        }
        private bool Check_ToMeFiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        {
            try
            {
                if (TrangThaiSelected.ID == 1)
                {
                    string[] status_Dangxuly = condition_DangXuLy.Split(',');
                    string[] str_statusSelected = lst_appStatus_selected.Select(s => s.ID.ToString()).ToArray<string>();

                    if (!status_Dangxuly.SequenceEqual(str_statusSelected))
                        return true;

                    DuedateSelected = lst_dueDateMenu.Where(c => c.isSelected == true).FirstOrDefault();
                    if (DuedateSelected.ID != 1) //!= tat ca
                        return true;

                    if (fromDate_default != _fromDateSelected)
                        return true;

                    if (toDateSelected != _toDateSelected)
                        return true;

                    return false;
                }
                else
                {
                    string[] status_Dangxuly = condition_DaXuLy.Split(',');
                    string[] str_statusSelected = lst_appStatus_selected.Select(s => s.ID.ToString()).ToArray<string>();

                    if (!status_Dangxuly.SequenceEqual(str_statusSelected))
                        return true;

                    DuedateSelected = lst_dueDateMenu.Where(c => c.isSelected == true).FirstOrDefault();
                    if (DuedateSelected.ID != 1) //!= tat ca
                        return true;

                    if (fromDate_default != _fromDateSelected)
                        return true;

                    if (toDate_default != _toDateSelected)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ViewSearchV2 - Check_ToMeFiltedStatus - ERR: " + ex.ToString());
                return false;
            }
        }
        private void DangXuLy_Default()
        {
            try
            {
                //Default tinh trang
                foreach (var item in lst_appStatus)
                {
                    if (item.ID == 4)
                    {
                        item.IsSelected = true;
                        lst_appStatus_selected = new List<BeanAppStatus>();
                        lst_appStatus_selected.Add(item);
                    }
                    else
                        item.IsSelected = false;
                }
                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                {
                    lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_tinhtrang.TextColor = UIColor.Black;

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                }

                //Default Duedate - Tat ca
                foreach (var item in lst_dueDateMenu)
                {
                    if (item.ID == 1)
                    {
                        item.isSelected = true;
                        DuedateSelected = item;
                    }
                    else
                        item.isSelected = false;
                }
                lbl_hanxuly.Text = DuedateSelected.title;


                //Default FromDate, DueDate
                lbl_fromdate.Text = "";
                lbl_todate.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("ViewSearchV2 - DangXuLy_Default - ERR: " + ex.ToString());
            }
        }

        private void DaXuLy_Default()
        {
            try
            {
                //Default tinh trang
                foreach (var item in lst_appStatus)
                {
                    if (item.ID == 4)
                    {
                        item.IsSelected = true;
                        lst_appStatus_selected = new List<BeanAppStatus>();
                        lst_appStatus_selected.Add(item);
                    }
                    else
                        item.IsSelected = false;
                }
                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                {
                    lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                    lbl_tinhtrang.TextColor = UIColor.Black;

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                    else
                        lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                }

                //Default Duedate - Tat ca
                foreach (var item in lst_dueDateMenu)
                {
                    if (item.ID == 1)
                    {
                        item.isSelected = true;
                        DuedateSelected = item;
                    }
                    else
                        item.isSelected = false;
                }
                lbl_hanxuly.Text = DuedateSelected.title;

                //Default FromDate, DueDate
                lbl_fromdate.Text = "";
                lbl_todate.Text = "";
            }

            catch (Exception ex)
            {
                Console.WriteLine("ViewSearchV2 - DangXuLy_Default - ERR: " + ex.ToString());
            }
        }

        #endregion

        #region from me
        private void LoadContent_FromMe()
        {
            string temp = "";
            string res_title = "";

            var condition_fromme = condition_FromMe.Split(',');
            //cap nhat lai tat ca IsSelected = false
            lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
            //gan lai gia tri IsSeleted theo condition_dangXL
            lst_appStatus.Where(f => condition_fromme.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
            //lay lai gia tri lst_appStatus_selected
            lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

            if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
            {
                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                {
                    if (lst_appStatus_selected.Count == 1)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                        else
                            lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                    }
                    else if (lst_appStatus_selected.Count > 1)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        //tat ca
                        if (lst_appStatus_selected.Count == lst_appStatus.Count)
                            lbl_tinhtrang.Text = hint_status;
                        else
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                            else
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;

                            lbl_tinhtrang.Text = lbl_tinhtrang.Text + " +" + (lst_appStatus_selected.Count - 1).ToString();
                        }
                    }
                }
                else
                {
                    lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
                    lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                    lbl_tinhtrang.Text = hint_status;
                }

            }
            else
            {
                lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
                lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                lbl_tinhtrang.Text = hint_status;
            }

            // DuedateSelected = null => Default => ID = 1 : Tat ca
            DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
            if (DuedateSelected != null)
            {
                lbl_hanxuly.Text = DuedateSelected.title;
            }
            else
            {
                DuedateSelected = lst_dueDateMenu[0]; // Tat ca
                DuedateSelected.isSelected = true;
                lbl_hanxuly.Text = DuedateSelected.title;
            }

            // Check FromDate
            if (fromDateSelected.Year != 1)
            {

                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");

                lbl_fromdate.TextColor = UIColor.Black;
                lbl_fromdate.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                //lbl_fromdate.Text = hint_fromDate;
                //lbl_fromdate.TextColor = UIColor.FromRGB(153, 153, 153);
                //lbl_fromdate.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }

            //Check ToDate
            if (toDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_todate.Text = toDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");

                lbl_todate.TextColor = UIColor.Black;
                lbl_todate.Font = UIFont.FromName("ArialMT", 14f);
            }
            else
            {
                //lbl_todate.Text = hint_toDate;
                //lbl_todate.TextColor = UIColor.FromRGB(153, 153, 153);
                //lbl_todate.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }
        }
        private bool Check_FromMeFiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        {
            try
            {
                string[] status_Dangxuly = condition_FromMe.Split(',');
                string[] str_statusSelected = lst_appStatus_selected.Select(s => s.ID.ToString()).ToArray<string>();

                if (!status_Dangxuly.SequenceEqual(str_statusSelected))
                    return true;

                DuedateSelected = lst_dueDateMenu.Where(c => c.isSelected == true).FirstOrDefault();
                if (DuedateSelected.ID != 1) //!= tat ca
                    return true;

                if (fromme_fromDate_default != _fromDateSelected)
                    return true;

                if (fromme_toDateSelected != _toDateSelected)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ViewSearchV2 - Check_FromMeFiltedStatus - ERR: " + ex.ToString());
                return false;
            }
        }
        #endregion

        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }
        private void BT_tome_trangthai_TouchUpInside(object sender, EventArgs e)
        {
            FormListItemsChoiceV2 itemsChoiceView = (FormListItemsChoiceV2)Storyboard.InstantiateViewController("FormListItemsChoiceV2");
            itemsChoiceView.setContent(this, lst_trangthai, null, null, false, 0);
            this.NavigationController.PushViewController(itemsChoiceView, true);
        }
        private void BT_tinhtrang_tome_TouchUpInside(object sender, EventArgs e)
        {
            FormListItemsChoiceV2 itemsChoiceView = (FormListItemsChoiceV2)Storyboard.InstantiateViewController("FormListItemsChoiceV2");
            itemsChoiceView.setContent(this, null, lst_appStatus, null, false, 1);
            this.NavigationController.PushViewController(itemsChoiceView, true);
        }
        private void BT_hanxuly_tome_TouchUpInside(object sender, EventArgs e)
        {
            FormListItemsChoiceV2 itemsChoiceView = (FormListItemsChoiceV2)Storyboard.InstantiateViewController("FormListItemsChoiceV2");
            itemsChoiceView.setContent(this, null, null, lst_dueDateMenu, false, 2);
            this.NavigationController.PushViewController(itemsChoiceView, true);
        }
        private void BT_tome_todate_TouchUpInside(object sender, EventArgs e)
        {
            FormChoiceDateController formChoiceDateController = (FormChoiceDateController)Storyboard.InstantiateViewController("FormChoiceDateController");
            formChoiceDateController.setContent(this, CmmFunction.GetTitle("TEXT_FROMDATE", ""), lbl_todate);
            this.NavigationController.PresentViewController(formChoiceDateController, true, null);
        }
        private void BT_tome_fromdate_TouchUpInside(object sender, EventArgs e)
        {
            FormChoiceDateController formChoiceDateController = (FormChoiceDateController)Storyboard.InstantiateViewController("FormChoiceDateController");
            formChoiceDateController.setContent(this, CmmFunction.GetTitle("TEXT_FROMDATE", ""), lbl_fromdate);
            this.PresentViewController(formChoiceDateController, true, null);
        }
        private void BT_ToMe_TouchUpInside(object sender, EventArgs e)
        {
            BT_switch(true);
        }
        private void BT_fromMe_TouchUpInside(object sender, EventArgs e)
        {
            BT_switch(false);
        }
        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            if (flag_tome) //to me
            {
                DateTime temp_fromDateSelected = new DateTime();
                DateTime temp_toDateSelected = new DateTime();

                if (!string.IsNullOrEmpty(lbl_fromdate.Text))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        temp_fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        temp_fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("vi", false));
                }

                if (!string.IsNullOrEmpty(lbl_todate.Text))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        temp_toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        temp_toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("vi", false));
                }

                if (temp_fromDateSelected.Year != 1 && temp_toDateSelected.Year != 1 && temp_fromDateSelected > temp_toDateSelected)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");
                    lbl_fromdate.TextColor = UIColor.Red;
                    lbl_todate.TextColor = UIColor.Red;
                }
                else
                {
                    bool filter = Check_ToMeFiltedStatus(temp_fromDateSelected, temp_fromDateSelected);
                    int trangthaiID = TrangThaiSelected.ID;
                    mainView.NavigateToDetailsFromSearch(true, trangthaiID, filter, lst_appStatus, lst_dueDateMenu, temp_fromDateSelected, temp_toDateSelected, tf_keyword.Text);
                    this.DismissModalViewController(true);
                }
            }
            //toi bat dau
            else
            {
                DateTime temp_fromDateSelected = new DateTime();
                DateTime temp_toDateSelected = new DateTime();

                if (!string.IsNullOrEmpty(lbl_fromdate.Text))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        temp_fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        temp_fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("vi", false));
                }

                if (!string.IsNullOrEmpty(lbl_todate.Text))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        temp_toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        temp_toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("vi", false));
                }


                if (temp_fromDateSelected > temp_toDateSelected)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");

                    lbl_fromdate.TextColor = UIColor.Red;
                    lbl_todate.TextColor = UIColor.Red;
                }
                else
                {
                    bool filter = Check_FromMeFiltedStatus(temp_fromDateSelected, temp_toDateSelected);
                    mainView.NavigateToDetailsFromSearch(false, 0, filter, lst_appStatus, lst_dueDateMenu, temp_fromDateSelected, temp_toDateSelected, tf_keyword.Text);
                    this.DismissModalViewController(true);
                }
            }
        }
        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {
            if (flag_tome)
            {
                try
                {
                    if (TrangThaiSelected.ID == 1)
                    {
                        //Default tinh trang
                        var condition_dangXL = condition_DangXuLy.Split(',');
                        //cap nhat lai tat ca IsSelected = false
                        lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
                        //gan lai gia tri IsSeleted theo condition_dangXL
                        lst_appStatus.Where(f => condition_dangXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();

                    }
                    else
                    {
                        //Default tinh trang
                        var condition_daXL = condition_DaXuLy.Split(',');
                        //cap nhat lai tat ca IsSelected = false
                        lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
                        //gan lai gia tri IsSeleted theo condition_daXL
                        lst_appStatus.Where(f => condition_daXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
                    }

                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

                    if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        if (lst_appStatus_selected.Count == lst_appStatus.Count)
                            lbl_tinhtrang.Text = hint_status;
                        else
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                            else
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                        }
                    }

                    //Default Duedate - Tat ca
                    foreach (var item in lst_dueDateMenu)
                    {
                        if (item.ID == 1)
                        {
                            item.isSelected = true;
                            DuedateSelected = item;
                        }
                        else
                            item.isSelected = false;
                    }
                    lbl_hanxuly.Text = DuedateSelected.title;


                    //Default FromDate, DueDate
                    lbl_fromdate.Text = "";
                    lbl_todate.Text = "";

                }
                catch (Exception ex)
                {
                    Console.WriteLine("FormFilterTodoView - BT_reset_filter_TouchUpInside - Err: " + ex.ToString());
                }
            }
            else
            {
                try
                {
                    //Default tinh trang
                    var condition_daXL = condition_FromMe.Split(',');
                    //cap nhat lai tat ca IsSelected = false
                    lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
                    //gan lai gia tri IsSeleted theo condition_daXL
                    lst_appStatus.Where(f => condition_daXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();

                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                    if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                    {
                        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
                        lbl_tinhtrang.TextColor = UIColor.Black;

                        if (lst_appStatus_selected.Count == lst_appStatus.Count)
                            lbl_tinhtrang.Text = hint_status;
                        else
                        {
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
                            else
                                lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
                        }
                    }

                    //Default Duedate - Tat ca
                    foreach (var item in lst_dueDateMenu)
                    {
                        if (item.ID == 1)
                        {
                            item.isSelected = true;
                            DuedateSelected = item;
                        }
                        else
                            item.isSelected = false;
                    }
                    lbl_hanxuly.Text = DuedateSelected.title;


                    //Default FromDate, DueDate
                    lbl_fromdate.Text = "";
                    lbl_todate.Text = "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FormFilterWorkFlowView - BT_reset_filter_TouchUpInside - Err: " + ex.ToString());
                }
            }
        }

        #endregion
    }
}

