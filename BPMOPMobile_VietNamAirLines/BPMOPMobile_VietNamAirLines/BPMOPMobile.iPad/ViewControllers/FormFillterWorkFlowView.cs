using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormFillterWorkFlowView : UIViewController
    {
        WorkflowDetailView workflowDetailView { get; set; }

        List<BeanAppStatus> lst_appStatus_origin;
        List<BeanAppStatus> lst_appStatus = new List<BeanAppStatus>();
        List<BeanAppStatus> lst_appStatus_selected;
        List<ClassMenu> lst_dueDateMenu;
        ClassMenu DuedateSelected;
        string hint_status = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
        string condition_FromMe = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_FROMME");
        bool isFilter;
        bool tab_inprogress;

        //value form default
        DateTime fromDate_default;
        DateTime toDate_default;

        DateTime fromDateSelected;
        DateTime toDateSelected;
        private int status_selected_index, duedate_selected_index;

        public FormFillterWorkFlowView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.PreferredContentSize = new CGSize(510, 510);
            ViewConfiguration();
            SetLangTitle();
            LoadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_fromdate.TouchUpInside += BT_fromDate_TouchUpInside;
            BT_todate.TouchUpInside += BT_toDate_TouchUpInside;
            BT_accept.TouchUpInside += BT_approve_TouchUpInside;
            BT_reset.TouchUpInside += BT_reset_filter_TouchUpInside;

            BT_tinhtrang.TouchUpInside += BT_status_TouchUpInside;
            BT_hanxuly.TouchUpInside += BT_duedate_TouchUpInside;
            #endregion
        }

        #region public - private method
        public void SetContent(WorkflowDetailView _workflowDetailView, List<BeanAppStatus> _lst_Status, List<ClassMenu> _lst_duedate, bool _isFilter, bool _tab_inprogress)
        {
            workflowDetailView = _workflowDetailView;

            lst_appStatus_origin = _lst_Status;
            lst_appStatus = ExtensionCopy.CopyAll(_lst_Status);

            lst_dueDateMenu = _lst_duedate;
            isFilter = _isFilter;
            tab_inprogress = _tab_inprogress;
        }

        private void SetLangTitle()
        {
            if (fromDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
            }

            if (toDateSelected.Year != 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_todate.Text = toDateSelected.ToString("MM/dd/yyyy");
                else
                    lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
            }

            CmmIOSFunction.SetLangToView(this.View);
        }
        private void ViewConfiguration()
        {
            BT_reset.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_accept.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

        }
        public void ReloadTinhtrangCategory(List<BeanAppStatus> _newList)
        {
            string temp = "";
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
                //lbl_tinhtrang.Text = TrangThaiSelected.title;
            }
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
                DuedateSelected = lst_dueDateMenu[0]; // tatca
                DuedateSelected.isSelected = true;
                lbl_hanxuly.Text = DuedateSelected.title;
            }
        }
        private void LoadContent()
        {
            string temp = "";
            string res_title = "";

            condition_FromMe = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
            var condition_daXL = condition_FromMe.Split(',');
            lst_appStatus = lst_appStatus_origin.FindAll(o => condition_daXL.Contains(o.ID.ToString()));
            #region
            ////cap nhat lai tat ca IsSelected = false
            //lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();
            ////gan lai gia tri IsSeleted theo condition_dangXL
            //lst_appStatus.Where(f => condition_FromMe.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
            ////lay lai gia tri lst_appStatus_selected
            //lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

            //if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
            //{
            //    if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
            //    {
            //        if (lst_appStatus_selected.Count == 1)
            //        {
            //            lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
            //            lbl_tinhtrang.TextColor = UIColor.Black;

            //            if (CmmVariable.SysConfig.LangCode == "1066")
            //                lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
            //            else
            //                lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;
            //        }
            //        else if (lst_appStatus_selected.Count > 1)
            //        {
            //            lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 14f);
            //            lbl_tinhtrang.TextColor = UIColor.Black;

            //            //tat ca
            //            if (lst_appStatus_selected.Count == lst_appStatus.Count)
            //                lbl_tinhtrang.Text = hint_status;
            //            else
            //            {
            //                if (CmmVariable.SysConfig.LangCode == "1066")
            //                    lbl_tinhtrang.Text = lst_appStatus_selected[0].Title;
            //                else
            //                    lbl_tinhtrang.Text = lst_appStatus_selected[0].TitleEN;

            //                lbl_tinhtrang.Text = lbl_tinhtrang.Text + " +" + (lst_appStatus_selected.Count - 1).ToString();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
            //        lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            //        lbl_tinhtrang.Text = hint_status;
            //    }

            //}
            //else
            //{
            //    lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
            //    lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            //    lbl_tinhtrang.Text = hint_status;
            //}

            ////Duedate
            //DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();
            //if (DuedateSelected != null)
            //{
            //    lbl_hanxuly.Text = DuedateSelected.title;
            //}
            //else
            //{
            //    DuedateSelected = lst_dueDateMenu[0]; // Tat ca
            //    DuedateSelected.isSelected = true;
            //    lbl_hanxuly.Text = DuedateSelected.title;
            //}
            #endregion
            #region Tinh trang
            //Trang thai Dang xu ly => Tinh trang => Dang thuc hien va khong cho chon
            if (!isFilter)
            {
                ////cap nhat lai tat ca IsSelected = false
                //lst_appStatus = lst_appStatus.Select(u => { u.IsSelected = false; return u; }).ToList();

                ////gan lai gia tri IsSeleted theo condition_daXL
                //lst_appStatus.Where(f => condition_daXL.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();

                lst_appStatus.ForEach(o => o.IsSelected = true);
            }
            //lay lai gia tri lst_appStatus_selected
            lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

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
                else
                {
                    lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                    lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
                    lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                }
            }
            else
            {
                lbl_tinhtrang.TextColor = UIColor.FromRGB(229, 229, 229);
                lbl_tinhtrang.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                lbl_tinhtrang.Text = hint_status;
            }
            #endregion

            #region Han xu ly
            // DuedateSelected = null => Default => ID = 1 : Tat ca
            //Default Duedate - Tat ca

            DuedateSelected = lst_dueDateMenu.Where(d => d.isSelected).FirstOrDefault();
            if (DuedateSelected != null)
                lbl_hanxuly.Text = DuedateSelected.title;
            else
            {
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
            }
            lbl_hanxuly.Text = DuedateSelected.title;

            #endregion

            #region Date

            fromDateSelected = workflowDetailView.fromDateSelected;
            toDateSelected = workflowDetailView.toDateSelected;

            fromDate_default = workflowDetailView.fromDate_default;
            toDate_default = workflowDetailView.toDate_default;

            if (string.IsNullOrEmpty(workflowDetailView.date_filter))
            {
                fromDate_default = workflowDetailView.fromDate_default;
                toDate_default = workflowDetailView.toDate_default;
            }
            else
            {
                fromDateSelected = workflowDetailView.fromDateSelected;
                toDateSelected = workflowDetailView.toDateSelected;

                fromDate_default = workflowDetailView.fromDate_default;
                toDate_default = workflowDetailView.toDate_default;
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
            #endregion 
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

        private bool Check_FiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        {
            string[] status_Dangxuly = condition_FromMe.Split(',');
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
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_fromDate_TouchUpInside(object sender, EventArgs e)
        {
            FormChoiceDateController formChoiceDateController = (FormChoiceDateController)Storyboard.InstantiateViewController("FormChoiceDateController");
            formChoiceDateController.setContent(this, CmmFunction.GetTitle("TEXT_FROMDATE", ""), lbl_fromdate);
            this.PresentViewController(formChoiceDateController, true, null);
        }

        private void BT_toDate_TouchUpInside(object sender, EventArgs e)
        {
            FormChoiceDateController formChoiceDateController = (FormChoiceDateController)Storyboard.InstantiateViewController("FormChoiceDateController");
            formChoiceDateController.setContent(this, CmmFunction.GetTitle("TEXT_FROMDATE", ""), lbl_todate);
            this.PresentViewController(formChoiceDateController, true, null);
        }

        private void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            try
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

                if ((temp_fromDateSelected.Year != 1 && temp_toDateSelected.Year != 1) && temp_fromDateSelected > temp_toDateSelected)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_DATE_INVALID", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"));

                    lbl_fromdate.TextColor = UIColor.Red;
                    lbl_todate.TextColor = UIColor.Red;
                }
                else
                {
                    /////Set lại default filter
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                    //var condition_default = condition_FromMe.Split(',');
                    //lst_appStatus.Where(f => condition_default.Contains(f.ID.ToString())).Select(u => { u.IsSelected = true; return u; }).ToList();
                    lst_appStatus_origin.ForEach(o =>
                    {
                        var stat = lst_appStatus.FindAll(a => a.ID == o.ID).FirstOrDefault();
                        if (stat != null)
                            o.IsSelected = stat.IsSelected;
                    });
                    lst_appStatus = lst_appStatus_origin;

                    DuedateSelected = lst_dueDateMenu.Where(d => d.isSelected == true).FirstOrDefault();
                    bool filter = Check_FiltedStatus(temp_fromDateSelected, temp_toDateSelected);

                    workflowDetailView.isFilter = filter;
                    workflowDetailView.date_filter = CmmIOSFunction.GetStringDateFilter(temp_fromDateSelected, temp_toDateSelected);
                    workflowDetailView.fromDateSelected = temp_fromDateSelected;
                    workflowDetailView.toDateSelected = temp_toDateSelected;
                    workflowDetailView.lst_appStatus = lst_appStatus;//lst_appStatus_selected
                    workflowDetailView.lst_dueDateMenu = lst_dueDateMenu;
                    if (filter)
                        workflowDetailView.LoadDataFilterFromServer(false);
                    else
                        workflowDetailView.ReLoadDataFromServer(true);

                    this.DismissModalViewController(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormFilterWorkFlowView - BT_approve_TouchUpInside - Err: " + ex.ToString());
            }
        }

        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                //Default tinh trang
                var condition_daXL = condition_FromMe.Split(',');
                //cap nhat lai tat ca IsSelected = false
                lst_appStatus = lst_appStatus_origin.Select(u => { u.IsSelected = false; return u; }).ToList();//lst_appStatus
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
                toDateSelected = DateTime.Now.Date;
                fromDateSelected = toDateSelected.AddDays(-CmmVariable.M_DataFilterDefaultDays);

                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    lbl_todate.Text = toDateSelected.ToString("MM/dd/yyyy");
                    lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
                }
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                {
                    lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
                    lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
                }

                BT_approve_TouchUpInside(null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormFilterWorkFlowView - BT_reset_filter_TouchUpInside - Err: " + ex.ToString());
            }
        }

        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            FormListItemsChoiceV2 itemsChoiceView = (FormListItemsChoiceV2)Storyboard.InstantiateViewController("FormListItemsChoiceV2");
            itemsChoiceView.setContent(this, null, lst_appStatus, null, false, 1);
            this.NavigationController.PushViewController(itemsChoiceView, true);
        }

        private void BT_duedate_TouchUpInside(object sender, EventArgs e)
        {
            FormListItemsChoiceV2 itemsChoiceView = (FormListItemsChoiceV2)Storyboard.InstantiateViewController("FormListItemsChoiceV2");
            itemsChoiceView.setContent(this, null, null, lst_dueDateMenu, false, 2);
            this.NavigationController.PushViewController(itemsChoiceView, true);
        }

        #endregion
    }
}