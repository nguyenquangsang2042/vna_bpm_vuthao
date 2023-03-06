using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    class Custom_WorkFlowView : UIView
    {
        UIButton BT_back;
        UILabel lbl_title;
        UIView view_topLine;
        UITableView table_QTLC;

        private Custom_WorkFlowView()
        {
            this.BackgroundColor = UIColor.White;

            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;

            lbl_title = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(15),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Left
            };

            BT_back = new UIButton();
            BT_back.SetImage(UIImage.FromFile("Icons/icon_arrow_left.png"), UIControlState.Normal);
            BT_back.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

            view_topLine = new UIView();
            view_topLine.BackgroundColor = UIColor.FromRGB(216, 216, 216);

            table_QTLC = new UITableView();
            table_QTLC.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_QTLC.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            this.AddSubviews(new UIView[] { BT_back, table_QTLC, lbl_title, view_topLine });

            if (BT_back != null)
                BT_back.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (viewController != null && viewController.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)viewController;
                controller.HandleWorkFollowViewResult();
            }
            else if (viewController != null && viewController.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)viewController;
                controller.HandleWorkFollowViewResult();
            }
            else if (viewController != null && viewController.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)viewController;
                controller.HandleWorkFollowViewResult();
            }
            else if (viewController != null && viewController.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)viewController;
                controller.HandleWorkFollowViewResult();
            }
        }

        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    TableLoadData();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }
        private static Custom_WorkFlowView instance = null;
        public static Custom_WorkFlowView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_WorkFlowView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            if (this.Frame == CGRect.Empty)
            {
                this.Frame = frame;

                BT_back.Frame = new CGRect(11, 10, 30, 30);
                lbl_title.Frame = new CGRect(60, 10, Frame.Width - (60 + 18), 30);
                view_topLine.Frame = new CGRect(18, 49, Frame.Width - 36, 1);
                table_QTLC.Frame = new CGRect(18, 60, Frame.Width - 36, Frame.Height - 50);
            }
        }

        public void TableLoadData()
        {
            lbl_title.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_HISTORY", "Luồng phê duyệt");
            table_QTLC.Source = new QTLC_DataSource(list_QTLC, this);
            table_QTLC.ReloadData();
        }

        public UIViewController viewController { get; set; }

        public List<BeanQuaTrinhLuanChuyen> list_QTLC { get; set; }

        public UITableView tableWorkFollow
        {
            get
            {
                return table_QTLC;
            }
        }

        #region thong tin luan chuyen
        private class QTLC_DataSource : UITableViewSource
        {
            List<BeanQuaTrinhLuanChuyen> lst_qtlc;

            NSString cellIdentifier = new NSString("cell_qtlc");
            Custom_WorkFlowView parentView;
            Dictionary<BeanQuaTrinhLuanChuyen, List<BeanQuaTrinhLuanChuyen>> dict_qtlc = new Dictionary<BeanQuaTrinhLuanChuyen, List<BeanQuaTrinhLuanChuyen>>();
            List<BeanQuaTrinhLuanChuyen> sectionKeys;
            nfloat height = 73;

            public QTLC_DataSource(List<BeanQuaTrinhLuanChuyen> _lst_qtlc, Custom_WorkFlowView _parentview)
            {
                parentView = _parentview;
                lst_qtlc = _lst_qtlc != null && _lst_qtlc.Count > 0 ? _lst_qtlc : new List<BeanQuaTrinhLuanChuyen>();
                LoadData();
            }

            private void LoadData()
            {
                try
                {
                    lst_qtlc = lst_qtlc.OrderBy(i => i.Created).ThenBy(i => i.Created).ToList();
                    foreach (var lv1 in lst_qtlc)
                    {
                        List<BeanQuaTrinhLuanChuyen> lst_childQTLC = new List<BeanQuaTrinhLuanChuyen>();
                        lst_childQTLC = lv1.ChildHistory;
                        List<BeanQuaTrinhLuanChuyen> subFinal = new List<BeanQuaTrinhLuanChuyen>(); ;
                        foreach (var lv2 in lst_childQTLC)
                        {
                            List<BeanQuaTrinhLuanChuyen> lst_subChildQTLC = new List<BeanQuaTrinhLuanChuyen>();
                            if (lv2.ChildHistory != null)
                            {
                                subFinal.Add(lv2);
                                lst_subChildQTLC = lv2.ChildHistory;
                                lst_subChildQTLC.Select(q => { q.IsSublevel2 = true; return q; }).ToList();
                                subFinal.AddRange(lst_subChildQTLC);
                            }
                            else
                                subFinal.Add(lv2);
                        }

                        dict_qtlc.Add(lv1, subFinal);

                    }

                    sectionKeys = dict_qtlc.Keys.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - LoadData - Err: " + ex.ToString());
                }
            }

            //private void LoadData()
            //{
            //    //sort list QTLC by Step
            //    lst_qtlc = lst_qtlc.OrderBy(i => i.Step).ThenBy(i => i.Created).ToList();

            //    foreach (var item in lst_qtlc)
            //    {
            //        if (dict_qtlc.ContainsKey(item.Step.ToString()))
            //        {
            //            dict_qtlc[item.Step.ToString()].Add(item);
            //        }
            //        else
            //        {
            //            List<BeanQuaTrinhLuanChuyen> lst_temp = new List<BeanQuaTrinhLuanChuyen>();
            //            lst_temp.Add(item);
            //            dict_qtlc.Add(item.Step.ToString(), lst_temp);
            //        }
            //    }

            //    sectionKeys = dict_qtlc.Keys.ToList();
            //}

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_qtlc.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var numRow = dict_qtlc[sectionKeys[Convert.ToInt32(section)]].Count;
                return numRow;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var key = dict_qtlc.ElementAt(Convert.ToInt32(section)).Key;
                if (dict_qtlc[key].Count > 0)
                {
                    var isSectionLast = (section == sectionKeys.Count - 1) ? true : false;

                    CGRect frame = new CGRect(0, -10, tableView.Frame.Width, 30);
                    Custom_QTLCHeader headerView = new Custom_QTLCHeader(frame);
                    headerView.LoadData(isSectionLast, section, key, 0);
                    return headerView;
                }
                else
                    return null;

            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 30;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var item = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];
                //var isFirst = (indexPath.Row == 0) ? true : false;

                if (string.IsNullOrEmpty(item.Comment))
                {
                    return 60;
                }
                else
                {
                    CGRect rect = new CGRect();

                    rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_QTLC.Frame.Width / 5) * 3);
                    if (rect.Height > 0 && rect.Height < 20)
                        rect.Height = 20;

                    return rect.Height + 60;

                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //var qtlc = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];
                //if (!string.IsNullOrEmpty(qtlc.Note))
                //{
                //    parentView.NavigateToFullText(qtlc);
                //}
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                //var qtlc = lst_qtlc[indexPath.Row];
                var qtlc = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];


                //var isFirst = (indexPath.Row == 0) ? true : false;
                //var isLast = (indexPath.Section == sectionKeys.Count - 1 && indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? true : false;
                //var isShowBotLine = (indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? false : true;
                var isSectionLast = (indexPath.Section == sectionKeys.Count - 1) ? true : false;
                //Custom_qtlc_Cell cell = new Custom_qtlc_Cell(cellIdentifier);
                //cell.UpdateCell(qtlc, isSectionLast, isFirst, isLast, dict_qtlc[sectionKeys[indexPath.Section]].Count, isShowBotLine);

                Custom_qtlc_Cell cell = new Custom_qtlc_Cell(cellIdentifier);
                cell.UpdateCell(qtlc, isSectionLast);

                return cell;
            }
        }

        private class Custom_qtlc_Cell : UITableViewCell
        {
            UILabel lbl_hanhdong, lbl_avatarCover, lbl_NXLy, lbl_chucvu, lbl_date, lbl_ykien, lbl_line, lbl_botLine;
            UILabel sub_verticalLine, sub_horizonLine;
            UIImageView img_avatar, img_qtlc_sub;
            BeanQuaTrinhLuanChuyen qtlc { get; set; }
            bool isFirst { get; set; }
            bool isLast { get; set; }
            bool isShowBotLine { get; set; }
            bool isSectionLast { get; set; }
            int numInSection;

            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            public Custom_qtlc_Cell(NSString cellID)
            {
                ViewConfiguration();
            }

            public void ViewConfiguration()
            {
                lbl_line = new UILabel();
                lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                sub_verticalLine = new UILabel();
                sub_verticalLine.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                sub_horizonLine = new UILabel();
                sub_horizonLine.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                lbl_hanhdong = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.Black
                };
                lbl_hanhdong.ClipsToBounds = true;
                lbl_hanhdong.Layer.CornerRadius = 3;
                lbl_hanhdong.Lines = 1;

                lbl_avatarCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 13f),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };

                lbl_avatarCover.Layer.BorderColor = UIColor.White.CGColor;
                lbl_avatarCover.Layer.BorderWidth = 2;
                lbl_avatarCover.Layer.CornerRadius = 20;
                lbl_avatarCover.ClipsToBounds = true;

                UIImage uIImage = new UIImage();
                //uIImage.sc

                img_qtlc_sub = new UIImageView();
                img_qtlc_sub.ContentMode = UIViewContentMode.Center;
                img_qtlc_sub.Image = UIImage.FromFile("Icons/icon_qtlc_subStep.png").Scale(new CGSize(16, 17), 2);
                img_qtlc_sub.BackgroundColor = UIColor.White;
                img_qtlc_sub.Hidden = true;

                img_avatar = new UIImageView();
                img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                CreateCircle();
                img_avatar.Hidden = true;

                lbl_NXLy = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black
                };

                lbl_date = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                lbl_chucvu = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                lbl_ykien = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    TextColor = UIColor.Black,
                    Hidden = false
                };

                lbl_botLine = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(216, 216, 216),
                    Hidden = true
                };

                ContentView.AddSubviews(new UIView[] { sub_verticalLine, sub_horizonLine, lbl_line, img_avatar, img_qtlc_sub, lbl_avatarCover, lbl_hanhdong, lbl_NXLy, lbl_chucvu, lbl_date, lbl_ykien, lbl_botLine });
            }

            public void UpdateCell(BeanQuaTrinhLuanChuyen _qtlc, bool _isLast)
            {
                try
                {
                    isLast = _isLast;
                    qtlc = _qtlc;

                    if (!string.IsNullOrEmpty(qtlc.AssignUserId))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        lst_userResult = conn.Query<BeanUser>(query_user, qtlc.AssignUserId);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                        {
                            BeanUser bUser = lst_userResult[0];
                            user_imagePath = bUser.ImagePath;

                            lbl_NXLy.Text = bUser.FullName;

                            if (string.IsNullOrEmpty(user_imagePath))
                            {
                                lbl_avatarCover.Hidden = false;
                                img_avatar.Hidden = true;
                                lbl_avatarCover.Text = CmmFunction.GetAvatarName(bUser.FullName);
                                lbl_avatarCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatarCover.Text));

                            }
                            else
                            {
                                lbl_avatarCover.Hidden = false;
                                img_avatar.Hidden = true;
                                lbl_avatarCover.Text = CmmFunction.GetAvatarName(bUser.FullName);
                                lbl_avatarCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatarCover.Text));
                                checkFileLocalIsExist(bUser, img_avatar);
                                //kiem tra xong cap nhat lai avatar
                                lbl_avatarCover.Hidden = true;
                                img_avatar.Hidden = false;
                            }

                            if (!string.IsNullOrEmpty(bUser.Position))
                                lbl_chucvu.Text = bUser.Position;
                        }
                    }

                    if (qtlc.CompletedDate.HasValue)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_date.Text = qtlc.CompletedDate.Value.ToString("MM/dd/yy HH:mm");
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            lbl_date.Text = qtlc.CompletedDate.Value.ToString("dd/MM/yy HH:mm");

                        if (qtlc.CompletedDate.HasValue && (qtlc.CompletedDate.Value.Date > qtlc.CompletedDate.Value))
                            lbl_date.TextColor = UIColor.Red;
                    }
                    else
                        lbl_date.Text = string.Empty;

                    string _submitaction = "";

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        _submitaction = qtlc.SubmitActionEN;
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        _submitaction = qtlc.SubmitAction;

                    if (!string.IsNullOrEmpty(_submitaction))
                    {
                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - 10, lbl_NXLy.Frame.Bottom + 5, 0, 18);
                        NSLayoutConstraint.Create(this.lbl_hanhdong, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Right, 1.0f, -10.0f).Active = true;
                        NSLayoutConstraint.Create(this.lbl_hanhdong, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1.0f, 45.0f).Active = true;
                        lbl_hanhdong.Text = _submitaction + " ";
                        lbl_hanhdong.Lines = 1;
                        lbl_hanhdong.SizeToFit();

                        if (qtlc.SubmitActionId.HasValue)
                        {
                            switch (qtlc.SubmitActionId.Value)
                            {
                                case 12: // Gửi
                                case 1: // Đồng ý
                                case 53: // Thực hiện điều chỉnh
                                case 7: // Yêu cầu bổ sung
                                case 9: // Yêu cầu tham vấn
                                case 10: // Thực hiện tham vấn
                                case 3: // Chuyển xử lý
                                case 6: // Thu hồi
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(229, 255, 228); // xanh da troi
                                    break;
                                case 2:
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(229, 255, 228); // xanh la
                                    break;
                                case 4: // Yêu cầu hiệu chỉnh
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(255, 225, 245); // pink
                                    break;
                                case 51: // Hủy
                                case 5: // Từ chối
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(255, 203, 203);// red
                                    break;
                                case 47: // Chờ xử lý
                                case 48: // Chờ xử lý
                                case 49: // Chờ xử lý
                                case 50: // Chờ xử lý
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(254, 237, 187);// yellow
                                    break;
                                default:
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(197, 221, 249); // xanh da troi
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - Custom_qtlc_Cell - UpdateCell: " + ex.ToString());
                }
            }

            private void CreateCircle()
            {
                try
                {
                    double min = Math.Min(img_avatar.Frame.Width, img_avatar.Frame.Height);
                    img_avatar.Layer.CornerRadius = (float)(min / 2.0);
                    img_avatar.Layer.MasksToBounds = false;
                    img_avatar.Layer.BorderColor = UIColor.White.CGColor;
                    img_avatar.Layer.BorderWidth = 2f;
                    img_avatar.ClipsToBounds = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - CreateCircle - Err: " + ex.ToString());
                }
            }

            private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
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
                                            image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_profile.png");

                                    img_avatar.Hidden = false;
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
                                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                                    img_avatar.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        img_avatar.Hidden = false;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
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
                            image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                    }

                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (qtlc.IsSublevel2)
                {
                    lbl_line.Frame = new CGRect(24, 0, 1, ContentView.Frame.Height);
                    lbl_avatarCover.Frame = new CGRect(80, 0, 40, 40);
                    img_avatar.Frame = new CGRect(80, 0, 40, 40);
                    if (numInSection > 1)
                    {
                        sub_verticalLine.Frame = new CGRect(62.5f, img_avatar.Frame.Bottom, 1, ContentView.Frame.Height - img_avatar.Frame.Bottom);
                        sub_verticalLine.Hidden = false;
                    }
                    lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, 0, ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10), 20);
                    lbl_date.Frame = new CGRect(ContentView.Frame.Width - 120, lbl_NXLy.Frame.Y, 110, 20);
                    lbl_chucvu.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, lbl_NXLy.Frame.Bottom, ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10), 20);

                    var widthStatus = StringExtensions.MeasureString(lbl_hanhdong.Text, 13).Width;
                    var maxStatusWidth = 120;
                    if (widthStatus < maxStatusWidth)
                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (widthStatus + 30), lbl_date.Frame.Bottom + 2, widthStatus + 20, 20);
                    else
                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (maxStatusWidth + 30), lbl_date.Frame.Bottom + 2, maxStatusWidth + 20, 20);

                    if (!string.IsNullOrEmpty(qtlc.Comment))
                    {
                        lbl_ykien.Hidden = false;
                        lbl_ykien.Frame = new CGRect(lbl_chucvu.Frame.X, lbl_chucvu.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_avatarCover.Frame.X + 10), 25);
                        lbl_ykien.Text = qtlc.Comment;
                        lbl_ykien.SizeToFit();
                    }

                    lbl_botLine.Frame = new CGRect(lbl_NXLy.Frame.X, ContentView.Frame.Bottom - 1, this.ContentView.Frame.Width - lbl_NXLy.Frame.X, 1);
                }
                else
                {
                    lbl_line.Frame = new CGRect(24, 0, 1, ContentView.Frame.Height);
                    lbl_avatarCover.Frame = new CGRect(40, 0, 40, 40);
                    img_avatar.Frame = new CGRect(40, 0, 40, 40);
                    if (numInSection > 1)
                    {
                        sub_verticalLine.Frame = new CGRect(62.5f, img_avatar.Frame.Bottom, 1, ContentView.Frame.Height - img_avatar.Frame.Bottom);
                        sub_verticalLine.Hidden = false;
                    }
                    lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, 0, ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10), 20);
                    lbl_date.Frame = new CGRect(ContentView.Frame.Width - 120, lbl_NXLy.Frame.Y, 110, 20);
                    lbl_chucvu.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, lbl_NXLy.Frame.Bottom, ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10), 20);

                    var widthStatus = StringExtensions.MeasureString(lbl_hanhdong.Text, 13).Width;
                    var maxStatusWidth = 120;
                    if (widthStatus < maxStatusWidth)
                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (widthStatus + 30), lbl_date.Frame.Bottom + 2, widthStatus + 20, 20);
                    else
                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (maxStatusWidth + 30), lbl_date.Frame.Bottom + 2, maxStatusWidth + 20, 20);

                    if (!string.IsNullOrEmpty(qtlc.Comment))
                    {
                        lbl_ykien.Hidden = false;
                        lbl_ykien.Frame = new CGRect(lbl_chucvu.Frame.X, lbl_chucvu.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_avatarCover.Frame.X + 10), 25);
                        lbl_ykien.Text = qtlc.Comment;
                        lbl_ykien.SizeToFit();
                    }

                    lbl_botLine.Frame = new CGRect(lbl_NXLy.Frame.X, ContentView.Frame.Bottom - 1, this.ContentView.Frame.Width - lbl_NXLy.Frame.X, 1);
                }

                if (isLast)
                    lbl_line.Hidden = true;

                CreateCircle();
            }
        }
        #endregion

        #region thong tin luan chuyen Backup
        private class QTLC_DataSource_bk : UITableViewSource
        {
            List<BeanQuaTrinhLuanChuyen> lst_qtlc;

            NSString cellIdentifier = new NSString("cell_qtlc");
            Custom_WorkFlowView parentView;
            Dictionary<string, List<BeanQuaTrinhLuanChuyen>> dict_qtlc = new Dictionary<string, List<BeanQuaTrinhLuanChuyen>>();
            List<string> sectionKeys;
            nfloat height = 73;

            public QTLC_DataSource_bk(List<BeanQuaTrinhLuanChuyen> _lst_qtlc, Custom_WorkFlowView _parentview)
            {
                parentView = _parentview;
                lst_qtlc = _lst_qtlc;
                LoadData();
            }

            private void LoadData_bk()
            {
                //foreach (var item in lst_qtlc)
                //{
                //    if (dict_qtlc.ContainsKey(item.Action))
                //    {
                //        dict_qtlc[item.Action].Add(item);
                //    }
                //    else
                //    {
                //        List<BeanQuaTrinhLuanChuyen> lst_temp = new List<BeanQuaTrinhLuanChuyen>();
                //        lst_temp.Add(item);
                //        dict_qtlc.Add(item.Action, lst_temp);
                //    }
                //}

                //sectionKeys = dict_qtlc.Keys.ToList();
            }

            private void LoadData()
            {
                //sort list QTLC by Step
                lst_qtlc = lst_qtlc.OrderBy(i => i.Step).ThenBy(i => i.Created).ToList();

                foreach (var item in lst_qtlc)
                {


                    if (dict_qtlc.ContainsKey(item.Step.ToString()))
                    {
                        dict_qtlc[item.Step.ToString()].Add(item);
                    }
                    else
                    {
                        List<BeanQuaTrinhLuanChuyen> lst_temp = new List<BeanQuaTrinhLuanChuyen>();
                        lst_temp.Add(item);
                        dict_qtlc.Add(item.Step.ToString(), lst_temp);
                    }
                }

                sectionKeys = dict_qtlc.Keys.ToList();
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_qtlc.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var numRow = dict_qtlc[sectionKeys[Convert.ToInt32(section)]].Count;
                return numRow;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var item = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];
                var isFirst = (indexPath.Row == 0) ? true : false;

                if (string.IsNullOrEmpty(item.Comment))
                {
                    return isFirst ? 80 : 60;
                }
                else
                {
                    CGRect rect = new CGRect();
                    //rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_content.Frame.Width / 5) * 2);
                    //if (rect.Height > 0 && rect.Height < 20)
                    //    rect.Height = 20;

                    if (isFirst)
                    {
                        rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_QTLC.Frame.Width / 5) * 4);
                        if (rect.Height > 0 && rect.Height < 20)
                            rect.Height = 20;
                        return rect.Height + 80;
                    }
                    else
                    {
                        rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_QTLC.Frame.Width / 5) * 3);
                        if (rect.Height > 0 && rect.Height < 20)
                            rect.Height = 20;

                        return rect.Height + 50;
                    }
                    //return isFirst ? heg.Height + 80 : heg.Height + 50;
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //var qtlc = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];
                //if (!string.IsNullOrEmpty(qtlc.Note))
                //{
                //    parentView.NavigateToFullText(qtlc);
                //}
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var qtlc = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];

                var isFirst = (indexPath.Row == 0) ? true : false;
                var isLast = (indexPath.Section == sectionKeys.Count - 1 && indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? true : false;
                var isShowBotLine = (indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? false : true;
                var isSectionLast = (indexPath.Section == sectionKeys.Count - 1) ? true : false;
                Custom_qtlc_Cell cell = new Custom_qtlc_Cell(cellIdentifier);
                cell.UpdateCell(qtlc, isLast);

                //cell.RowHeight += (object sender, EventArgs e) =>
                //{
                //    height = Convert.ToInt32(sender);
                //    if (!string.IsNullOrEmpty( qtlc.Note))
                //    {
                //        cell.BackgroundColor = UIColor.Red;
                //    }
                //};

                return cell;
            }
        }

        private class Custom_qtlc_Cell_bk : UITableViewCell
        {
            UILabel lbl_title, lbl_hanhdong, lbl_avatarCover, lbl_NXLy, lbl_date, lbl_ykien, lbl_line, lbl_botLine;
            UILabel sub_verticalLine, sub_horizonLine;
            UIImageView icon_status, img_avatar, img_qtlc_sub;
            BeanQuaTrinhLuanChuyen qtlc { get; set; }
            bool isFirst { get; set; }
            bool isLast { get; set; }
            bool isShowBotLine { get; set; }
            bool isSectionLast { get; set; }
            int numInSection;

            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            public Custom_qtlc_Cell_bk(NSString cellID)
            {
                ViewConfiguration();
            }

            public void ViewConfiguration()
            {
                lbl_line = new UILabel();
                lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                sub_verticalLine = new UILabel();
                sub_verticalLine.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                sub_horizonLine = new UILabel();
                sub_horizonLine.BackgroundColor = UIColor.FromRGB(229, 229, 229);

                lbl_title = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("Arial-BoldMT", 13f),
                    TextColor = UIColor.Black
                };

                lbl_hanhdong = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.Black
                };
                lbl_hanhdong.ClipsToBounds = true;
                lbl_hanhdong.Layer.CornerRadius = 3;
                lbl_hanhdong.Lines = 1;

                lbl_avatarCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 13f),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };

                lbl_avatarCover.Layer.BorderColor = UIColor.White.CGColor;
                lbl_avatarCover.Layer.BorderWidth = 2;
                lbl_avatarCover.Layer.CornerRadius = 20;
                lbl_avatarCover.ClipsToBounds = true;

                UIImage uIImage = new UIImage();
                //uIImage.sc

                img_qtlc_sub = new UIImageView();
                img_qtlc_sub.ContentMode = UIViewContentMode.Center;
                img_qtlc_sub.Image = UIImage.FromFile("Icons/icon_qtlc_subStep.png").Scale(new CGSize(16, 17), 2);
                img_qtlc_sub.BackgroundColor = UIColor.White;
                img_qtlc_sub.Hidden = true;

                img_avatar = new UIImageView();
                img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                CreateCircle();
                img_avatar.Hidden = true;

                lbl_NXLy = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black
                };

                lbl_date = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.DarkGray
                };

                lbl_ykien = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    TextColor = UIColor.Black,
                    Hidden = false
                };

                icon_status = new UIImageView();

                lbl_botLine = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(216, 216, 216),
                    Hidden = true
                };

                ContentView.AddSubviews(new UIView[] { sub_verticalLine, sub_horizonLine, lbl_line, icon_status, img_avatar, img_qtlc_sub, lbl_avatarCover, lbl_title, lbl_hanhdong, lbl_NXLy, lbl_date, lbl_ykien, lbl_botLine });
            }

            public void UpdateCell(BeanQuaTrinhLuanChuyen _qtlc, bool _isSectionLast, bool _isFirst, bool _isLast, int numInSec, bool _isShowBotLine)
            {
                try
                {
                    qtlc = _qtlc;
                    isFirst = _isFirst;
                    isLast = _isLast;
                    isSectionLast = _isSectionLast;
                    numInSection = numInSec;
                    isShowBotLine = _isShowBotLine;

                    lbl_title.Hidden = !_isFirst;
                    icon_status.Hidden = !_isFirst;
                    lbl_line.Hidden = _isLast;
                    //lbl_botLine.Hidden = !_isShowBotLine;
                    lbl_botLine.Hidden = true;

                    if (qtlc.Status)
                        icon_status.Image = UIImage.FromFile("Icons/icon_point_green.png");
                    else
                        icon_status.Image = UIImage.FromFile("Icons/icon_point_orange.png");

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_title.Text = qtlc.TitleEN;
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_title.Text = qtlc.Title;

                    if (!string.IsNullOrEmpty(qtlc.AssignedTo))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        lst_userResult = conn.Query<BeanUser>(query_user, qtlc.AssignUserId);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                            user_imagePath = lst_userResult[0].ImagePath;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {
                            lbl_avatarCover.Hidden = false;
                            img_avatar.Hidden = true;
                            lbl_avatarCover.Text = CmmFunction.GetAvatarName(qtlc.AssignedTo);
                            lbl_avatarCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatarCover.Text));

                        }
                        else
                        {
                            lbl_avatarCover.Hidden = false;
                            img_avatar.Hidden = true;
                            lbl_avatarCover.Text = CmmFunction.GetAvatarName(qtlc.AssignedTo);
                            lbl_avatarCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatarCover.Text));
                            checkFileLocalIsExist(lst_userResult[0], img_avatar);
                            //kiem tra xong cap nhat lai avatar
                            lbl_avatarCover.Hidden = true;
                            img_avatar.Hidden = false;
                        }
                    }

                    //if (!string.IsNullOrEmpty(qtlc.ChucDanh))
                    //    lbl_NXLy.Text = qtlc.AssignedTo + " - " + qtlc.ChucDanh;
                    //else
                    //lbl_NXLy.Text = qtlc.AssignedTo;

                    if (lbl_NXLy.Text.Contains("-"))
                    {
                        string str_transalte = lbl_NXLy.Text;
                        var indexA = str_transalte.IndexOf('-');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(25, 25, 30), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 12), new NSRange(indexA, str_transalte.Length - indexA));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(indexA, str_transalte.Length - indexA));
                        lbl_NXLy.AttributedText = att;
                    }

                    if (qtlc.CompletedDate.HasValue)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_date.Text = qtlc.CompletedDate.Value.ToString("MM/dd/yy HH:mm");
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            lbl_date.Text = qtlc.CompletedDate.Value.ToString("dd/MM/yy HH:mm");

                        if (qtlc.CompletedDate.HasValue && (qtlc.CompletedDate.Value.Date > qtlc.CompletedDate.Value))
                            lbl_date.TextColor = UIColor.Red;
                    }
                    else
                        lbl_date.Text = string.Empty;

                    if (!string.IsNullOrEmpty(qtlc.SubmitAction))
                    {
                        string _submitaction = qtlc.SubmitAction;

                        if (CmmVariable.SysConfig.LangCode == "1033")
                            _submitaction = qtlc.SubmitActionEN;
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            _submitaction = qtlc.SubmitAction;

                        lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - 10, lbl_NXLy.Frame.Bottom + 5, 0, 18);
                        NSLayoutConstraint.Create(this.lbl_hanhdong, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Right, 1.0f, -10.0f).Active = true;
                        NSLayoutConstraint.Create(this.lbl_hanhdong, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1.0f, 45.0f).Active = true;
                        lbl_hanhdong.Text = _submitaction + " ";
                        lbl_hanhdong.Lines = 1;
                        lbl_hanhdong.SizeToFit();

                        if (qtlc.SubmitActionId.HasValue)
                        {
                            switch (qtlc.SubmitActionId.Value)
                            {
                                case 12: // Gửi
                                case 1: // Đồng ý
                                case 53: // Thực hiện điều chỉnh
                                case 7: // Yêu cầu bổ sung
                                case 9: // Yêu cầu tham vấn
                                case 10: // Thực hiện tham vấn
                                case 3: // Chuyển xử lý
                                case 6: // Thu hồi
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(229, 255, 228); // xanh da troi
                                    break;
                                case 2:
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(229, 255, 228); // xanh la
                                    break;
                                case 4: // Yêu cầu hiệu chỉnh
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(255, 225, 245); // pink
                                    break;
                                case 51: // Hủy
                                case 5: // Từ chối
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(255, 203, 203);// red
                                    break;
                                case 47: // Chờ xử lý
                                case 48: // Chờ xử lý
                                case 49: // Chờ xử lý
                                case 50: // Chờ xử lý
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(254, 237, 187);// yellow
                                    break;
                                default:
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(197, 221, 249); // xanh da troi
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - Custom_qtlc_Cell - UpdateCell: " + ex.ToString());
                }
            }

            private void CreateCircle()
            {
                try
                {
                    double min = Math.Min(img_avatar.Frame.Width, img_avatar.Frame.Height);
                    img_avatar.Layer.CornerRadius = (float)(min / 2.0);
                    img_avatar.Layer.MasksToBounds = false;
                    img_avatar.Layer.BorderColor = UIColor.White.CGColor;
                    img_avatar.Layer.BorderWidth = 2f;
                    img_avatar.ClipsToBounds = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_WorkFlowView - CreateCircle - Err: " + ex.ToString());
                }
            }

            private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ID + "_Avatar.jpg";//?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                                                 //string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath + "?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
                                            image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_profile.png");

                                    img_avatar.Hidden = false;
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
                                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                                    img_avatar.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        img_avatar.Hidden = false;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
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
                            image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (isFirst)
                {
                    if (qtlc.Count == 0) // item thu 1 - cap 1
                    {
                        //ContentView.BackgroundColor = UIColor.Red;
                        icon_status.Frame = new CGRect(15, 0, 20, 20);
                        lbl_line.Frame = new CGRect(24, icon_status.Frame.Bottom, 1, ContentView.Frame.Height - 20);
                        lbl_title.Frame = new CGRect(icon_status.Frame.GetMaxX() + 10, 2, 240, 16);

                        lbl_avatarCover.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 10, 40, 40);
                        img_avatar.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 10, 40, 40);
                        if (numInSection > 1)
                        {
                            sub_verticalLine.Frame = new CGRect(62.5f, img_avatar.Frame.Bottom, 1, ContentView.Frame.Height - img_avatar.Frame.Bottom);
                            sub_verticalLine.Hidden = false;
                        }
                        lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, lbl_title.Frame.Bottom + 10, ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10), 20);

                        //if (!isShowBotLine)
                        //sub_verticalLine.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() - (img_qtlc_sub.Frame.Width / 2), 0, 1, ContentView.Frame.Height / 2);
                    }
                    else // cap 2
                    {
                        lbl_line.Frame = new CGRect(24, 0, 1, ContentView.Frame.Height);
                        lbl_line.Hidden = false;

                        img_qtlc_sub.Frame = new CGRect(51, 20, 24, 24);
                        img_qtlc_sub.Hidden = false;

                        sub_verticalLine.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() - (img_qtlc_sub.Frame.Width / 2), 0, 1, ContentView.Frame.Height);
                        sub_verticalLine.Hidden = false;

                        lbl_avatarCover.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() + 20, 10, 40, 40);
                        img_avatar.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() + 20, 10, 40, 40);

                        lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, 10, (ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 20)), 20);
                        lbl_NXLy.BackgroundColor = UIColor.Purple;

                        if (!isShowBotLine)
                            sub_verticalLine.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() - (img_qtlc_sub.Frame.Width / 2), 0, 1, ContentView.Frame.Height / 2);
                    }
                }
                else
                {
                    if (qtlc.Count == 0) // item > thu 1 - cap 1
                    {
                        icon_status.Frame = new CGRect(15, 0, 20, 20);
                        lbl_title.Frame = new CGRect(icon_status.Frame.GetMaxX() + 10, 2, 240, 0);

                        lbl_avatarCover.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 10, 40, 40);
                        img_avatar.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 10, 40, 40);

                        //sub_verticalLine.Frame = new CGRect(62.5f, 0, 1, ContentView.Frame.Height / 2);
                        sub_verticalLine.Frame = new CGRect(62.5f, 0, 1, ContentView.Frame.Height);
                        sub_verticalLine.Hidden = false;
                        lbl_line.Frame = new CGRect(24, 0, 1, ContentView.Frame.Height);
                        lbl_line.Hidden = false;

                        if (!isShowBotLine)
                        {
                            sub_verticalLine.Frame = new CGRect(62.5f, 0, 1, ContentView.Frame.Height / 2);
                        }
                        lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, lbl_title.Frame.Bottom + 10, (ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 10)), 20);
                    }
                    else
                    {
                        //ContentView.BackgroundColor = UIColor.Orange;
                        lbl_line.Frame = new CGRect(24, 0, 1, ContentView.Frame.Height);
                        lbl_line.Hidden = false;

                        img_qtlc_sub.Frame = new CGRect(50.5f, 20, 24, 24);
                        img_qtlc_sub.Hidden = false;

                        sub_verticalLine.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() - (img_qtlc_sub.Frame.Width / 2), 0, 1, ContentView.Frame.Height);
                        sub_verticalLine.Hidden = false;

                        sub_horizonLine.Frame = new CGRect(70, img_qtlc_sub.Frame.GetMaxY() - (img_qtlc_sub.Frame.Height / 2), 30, 1);
                        sub_horizonLine.Hidden = false;

                        lbl_avatarCover.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() + 20, 10, 40, 40);
                        img_avatar.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() + 20, 10, 40, 40);

                        lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.GetMaxX() + 10, 10, (ContentView.Frame.Width - (lbl_avatarCover.Frame.GetMaxX() + 20)), 20);

                        if (!isShowBotLine)
                        {
                            sub_verticalLine.Frame = new CGRect(img_qtlc_sub.Frame.GetMaxX() - (img_qtlc_sub.Frame.Width / 2), 0, 1, ContentView.Frame.Height / 2);
                        }
                    }
                }

                lbl_date.Frame = new CGRect(lbl_NXLy.Frame.X, lbl_NXLy.Frame.Bottom, 110, 20);

                var widthStatus = StringExtensions.MeasureString(lbl_hanhdong.Text, 13).Width;
                var maxStatusWidth = 120;
                if (widthStatus < maxStatusWidth)
                    lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (widthStatus + 30), lbl_date.Frame.Y, widthStatus + 20, 20);
                else
                    lbl_hanhdong.Frame = new CGRect(ContentView.Frame.Width - (maxStatusWidth + 30), lbl_date.Frame.Y, maxStatusWidth + 20, 20);

                if (!string.IsNullOrEmpty(qtlc.Comment))
                {
                    lbl_ykien.Hidden = false;
                    lbl_ykien.Frame = new CGRect(lbl_date.Frame.X, lbl_date.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_date.Frame.X + 10), 30);
                    lbl_ykien.Text = qtlc.Comment;
                    lbl_ykien.SizeToFit();
                }

                lbl_botLine.Frame = new CGRect(lbl_NXLy.Frame.X, ContentView.Frame.Bottom - 1, this.ContentView.Frame.Width - lbl_NXLy.Frame.X, 1);

                CreateCircle();
            }
        }
        #endregion
    }
}