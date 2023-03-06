using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ProgressView : UIViewController
    {
        List<BeanQuaTrinhLuanChuyen> lst_progress;
        BeanWorkflowItem workflowItem;
        CmmLoading loading;
        string title;

        public ProgressView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            SetLangTitle();
            loadQuaTrinhluanchuyen();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_code_fulltext.TouchUpInside += BT_code_fulltext_TouchUpInside;
            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        #endregion

        #region private - public methdod
        public void SetContent(BeanWorkflowItem _workflowitem, string _title)
        {
            title = _title;
            workflowItem = _workflowitem;
        }

        private void ViewConfiguration()
        {
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            lbl_title.LineBreakMode = UILineBreakMode.TailTruncation;
            lbl_title.Lines = 2;

            //table_content.ContentInset = new UIEdgeInsets(-10, 0, 0, 0);
            table_content.RowHeight = UITableView.AutomaticDimension;
            table_content.EstimatedRowHeight = new nfloat(15.0);
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private async void loadQuaTrinhluanchuyen()
        {
            lbl_title.Text = title;
            CGRect size = StringExtensions.StringRect(lbl_title.Text, UIFont.FromName("Arial-BoldMT", 15), 15);
            if (size.Height / 40 > 40)
                BT_code_fulltext.UserInteractionEnabled = true;
            else
                BT_code_fulltext.UserInteractionEnabled = false;

            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            await Task.Run(() =>
            {
                lst_progress = new List<BeanQuaTrinhLuanChuyen>();
                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                lst_progress = p_dynamic.GetListProcessHistory(workflowItem);
                lst_progress = lst_progress.OrderByDescending(o => o.Created).ToList();
                if (lst_progress != null)
                {
                    foreach (var item in lst_progress)
                    {
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                        string query = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        var lst_user = conn.Query<BeanUser>(query, item.AssignUserId);

                        if (lst_user != null && lst_user.Count > 0)
                        {
                            item.AssignPositionTitle = lst_user[0].Position;
                            item.AssignedTo = lst_user[0].FullName;
                        }
                    }

                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                        table_content.Source = new QTLC_DataSource(lst_progress, this);
                        table_content.ReloadData();
                    });

                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                    });
                }
            });
        }

        private void NavigateToFullText(BeanQuaTrinhLuanChuyen qtlc)
        {
            var size = StringExtensions.MeasureString(qtlc.Comment, 15);
            if (size.Height > 15)
            {
                FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
                string title = "";
                if (!string.IsNullOrEmpty(qtlc.AssignPositionTitle))
                    title = qtlc.AssignedTo + " (" + qtlc.AssignPositionTitle + ")";
                else
                    title = qtlc.AssignedTo;

                fullTextView.SetContent(title, qtlc.Comment);
                this.NavigationController.PushViewController(fullTextView, true);
            }
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewControllerAsync(true);
        }

        private void BT_code_fulltext_TouchUpInside(object sender, EventArgs e)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent("", lbl_title.Text);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        #endregion

        #region custom class
        #region thong tin luan chuyen
        private class QTLC_DataSource : UITableViewSource
        {
            List<BeanQuaTrinhLuanChuyen> lst_qtlc;

            NSString cellIdentifier = new NSString("cell_qtlc");
            ProgressView parentView;
            Dictionary<BeanQuaTrinhLuanChuyen, List<BeanQuaTrinhLuanChuyen>> dict_qtlc = new Dictionary<BeanQuaTrinhLuanChuyen, List<BeanQuaTrinhLuanChuyen>>();
            List<BeanQuaTrinhLuanChuyen> sectionKeys;
            nfloat height = 73;

            public QTLC_DataSource(List<BeanQuaTrinhLuanChuyen> _lst_qtlc, ProgressView _parentview)
            {
                parentView = _parentview;
                lst_qtlc = _lst_qtlc;
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

                    rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_content.Frame.Width / 5) * 3);
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
                var qtlc = dict_qtlc[sectionKeys[indexPath.Section]][indexPath.Row];

                //var isFirst = (indexPath.Row == 0) ? true : false;
                //var isLast = (indexPath.Section == sectionKeys.Count - 1 && indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? true : false;
                //var isShowBotLine = (indexPath.Row == (dict_qtlc[sectionKeys[indexPath.Section]].Count - 1)) ? false : true;
                var isSectionLast = (indexPath.Section == sectionKeys.Count - 1) ? true : false;
                //Custom_qtlc_Cell cell = new Custom_qtlc_Cell(cellIdentifier);
                //cell.UpdateCell(qtlc, isSectionLast, isFirst, isLast, dict_qtlc[sectionKeys[indexPath.Section]].Count, isShowBotLine);

                Custom_qtlc_Cell cell = new Custom_qtlc_Cell(cellIdentifier);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.UpdateCell(qtlc, isSectionLast);

                return cell;
            }
        }

        private class Custom_qtlc_Cell : UITableViewCell
        {
            UILabel lbl_title, lbl_hanhdong, lbl_avatarCover, lbl_NXLy, lbl_chucvu, lbl_date, lbl_ykien, lbl_line, lbl_botLine;
            UILabel sub_verticalLine, sub_horizonLine;
            UIImageView icon_status, img_avatar, img_qtlc_sub;
            BeanQuaTrinhLuanChuyen qtlc { get; set; }
            bool isLast { get; set; }
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
                    TextAlignment = UITextAlignment.Right,
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.DarkGray
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

                icon_status = new UIImageView();

                lbl_botLine = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(216, 216, 216),
                    Hidden = true
                };

                ContentView.AddSubviews(new UIView[] { sub_verticalLine, sub_horizonLine, lbl_line, icon_status, img_avatar, img_qtlc_sub, lbl_avatarCover, lbl_title, lbl_hanhdong, lbl_NXLy, lbl_chucvu, lbl_date, lbl_ykien, lbl_botLine });
            }
            public void UpdateCell(BeanQuaTrinhLuanChuyen _qtlc, bool _isLast)
            {
                try
                {
                    isLast = _isLast;
                    qtlc = _qtlc;

                    if (!string.IsNullOrEmpty(qtlc.AssignUserName) && !string.IsNullOrEmpty(qtlc.AssignUserAvatar))//AssignUserId
                    {
                        //List<BeanUser> lst_userResult = new List<BeanUser>();
                        #region 04.08.22 không dùng query bean User nữa = > chuyển sang dùng thông tin bean ttlc trả về
                        //var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                        //string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        //lst_userResult = conn.Query<BeanUser>(query_user, qtlc.AssignUserId);
                        #endregion
                        string user_imagePath = "";
                        //if (lst_userResult.Count > 0)
                        //{
                        //BeanUser bUser = lst_userResult[0];
                        BeanUser bUser = new BeanUser
                        {
                            ID = qtlc.AssignUserId,//Thêm để dùng làm tên file avatar lưu lại
                            ImagePath = qtlc.AssignUserAvatar,
                            FullName = qtlc.AssignUserName
                        };

                        user_imagePath = bUser.ImagePath;

                        lbl_NXLy.Text = bUser.FullName;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {
                            if (!string.IsNullOrEmpty(bUser.FullName))
                            {
                                lbl_avatarCover.Hidden = false;
                                img_avatar.Hidden = true;
                                lbl_avatarCover.Text = CmmFunction.GetAvatarName(bUser.FullName);
                                lbl_avatarCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatarCover.Text));
                            }
                            else
                            {
                                lbl_avatarCover.Hidden = true;
                                img_avatar.Hidden = false;
                                img_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                            }
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
                    //}

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
                                    lbl_hanhdong.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi
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
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

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
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
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
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
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

                    lbl_date.Frame = new CGRect(ContentView.Frame.Width - 115, lbl_NXLy.Frame.Y, 100, 20);
                    lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.Right + 10, 0, (lbl_date.Frame.Left + 10) - (lbl_avatarCover.Frame.Right + 10), 20);

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
                    lbl_date.Frame = new CGRect(ContentView.Frame.Width - 115, lbl_NXLy.Frame.Y, 100, 20);
                    lbl_NXLy.Frame = new CGRect(lbl_avatarCover.Frame.Right + 10, 0, (lbl_date.Frame.Left + 10) - (lbl_avatarCover.Frame.Right + 10), 20);
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
                        lbl_ykien.Frame = new CGRect(lbl_chucvu.Frame.X, lbl_chucvu.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_chucvu.Frame.X + 10), 25);
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
        #endregion
    }
}

