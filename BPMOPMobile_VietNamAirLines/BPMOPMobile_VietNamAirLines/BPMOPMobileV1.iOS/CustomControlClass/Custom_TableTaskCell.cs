
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;
class Custom_TableTaskCell : UITableViewCell
{

    public UIView background;
    public UILabel nodeName;
    public UIImageView nodeIMG;
    public UILabel nodeDesc;
    public UILabel lbltest;
    public NSLayoutConstraint lead;

    string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    UILabel lbl_title, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang, lbl_verticalLine, lbl_verticalLine2, lbl_horizonSubNode;
    UILabel lbl_bottomLine;
    BeanTaskDetail beanTask;
    BeanTaskDetail parentNode;
    UIImageView icon_expand, iv_avatar;
    public UIButton BT_modifer;
    FormTaskDetails formTaskDetails;

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        if (beanTask != null)
            InitFrameViews();
    }

    public Custom_TableTaskCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
    {
        InitControlsView();
    }
    ControlBase controlInputTask { get; set; }

    public void UpdateCell(BeanTaskDetail _task, BeanTaskDetail _parentNode, ControlBase _parentView, FormTaskDetails _formTaskDetails)
    {
        try
        {
            beanTask = _task;
            parentNode = _parentNode;
            controlInputTask = _parentView;
            formTaskDetails = _formTaskDetails;

            // nếu có phần tử con thì kiểm tra đâng đóng thì hiển thị image +
            //nếu đang mở thì hiển thị image -
            if (beanTask.children != null && beanTask.children.Count > 0)
            {
                if (beanTask.isExpand)
                    icon_expand.Image = UIImage.FromFile("Icons/icon_colapse.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else
                    icon_expand.Image = UIImage.FromFile("Icons/icon_expand2.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                icon_expand.Hidden = false;
            }
            else//nếu không có phần tử con thì cho image = null và tất line
            {
                if (beanTask.index == 0)
                {
                    lbl_verticalLine.Hidden = true;
                    lbl_bottomLine.Hidden = true;
                }
                icon_expand.Hidden = true;
                icon_expand.Image = null;
            }


            if (beanTask.Parent == 0)
                lbl_title.Font = UIFont.FromName("Arial-BoldMT", 15f);
            else
                lbl_title.Font = UIFont.FromName("ArialMT", 15f);

            lbl_title.Text = beanTask.Title;

            if (!string.IsNullOrEmpty(beanTask.AssignedId))
            {
                List<BeanUser> lst_userResult = new List<BeanUser>();
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                lst_userResult = conn.Query<BeanUser>(query_user, beanTask.AssignedId);

                string user_imagePath = "";
                if (lst_userResult.Count > 0)
                {
                    if (beanTask.AssignedName.Contains(','))
                        lbl_nguoiXL.Text = beanTask.AssignedName;
                    else
                        lbl_nguoiXL.Text = lst_userResult[0].FullName;

                    lbl_chucvu.Text = lst_userResult[0].Position;
                    user_imagePath = lst_userResult[0].ImagePath;

                    if (!string.IsNullOrEmpty(user_imagePath))
                        checkFileLocalIsExist(lst_userResult[0], iv_avatar);
                }
                else
                {
                    lbl_nguoiXL.Text = "";
                    lbl_chucvu.Text = "";
                    iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                }
            }
            else
            {
                // ---- Set Avatar ----
                iv_avatar.Image = UIImage.FromFile("Icons/icon_group.png");
                // ---- Set Name ----
                if (!String.IsNullOrEmpty(beanTask.AssignedName)) // group -> gán tên vào
                    lbl_nguoiXL.Text = beanTask.AssignedName;
                else
                    lbl_nguoiXL.Text = "";

                //lbl_nguoiXL.Text = "";
                lbl_chucvu.Text = "";

            }

            if (beanTask.DueDate.HasValue)
                lbl_hanXL.Text = beanTask.DueDate.Value.ToString("dd/MM/yy HH:mm");
            else
                lbl_hanXL.Text = string.Empty;

            //lbl_tinhtrang.Hidden = false;
            lbl_tinhtrang.Lines = 1;
            lbl_tinhtrang.SizeToFit();

            //Đang thực hiện
            if (beanTask.Status == 1)
            {
                lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_INPROGRESS", "Đang thực hiện");
                lbl_tinhtrang.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi
            }
            else if (beanTask.Status == 2)
            {
                lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_COMPLETED", "Hoàn tất");
                lbl_tinhtrang.BackgroundColor = UIColor.FromRGB(220, 255, 218); // xanh la
            }
            else if (beanTask.Status == 3)
            {
                lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_HOLD", "Tạm hoãn");
                lbl_tinhtrang.BackgroundColor = UIColor.FromRGB(255, 203, 203); // đỏ
            }
            else if (beanTask.Status == 4)
            {
                lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_CANCEL", "Hủy");
                lbl_tinhtrang.BackgroundColor = UIColor.FromRGB(255, 203, 203); // đỏ
            }
            else
            {
                lbl_tinhtrang.Text = CmmFunction.GetTitle("TEXT_NOPROCESS", "Chưa thực hiện");
                lbl_tinhtrang.BackgroundColor = UIColor.FromRGB(240, 240, 240); // xam
            }

            var width = this.Frame.Width;
            nfloat width_remain = width - lbl_title.Frame.Width;
            lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
            BT_modifer.TouchUpInside += BT_modifer_TouchUpInside;
        }
        catch (Exception ex)
        {
            Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
        }
    }
    private void BT_modifer_TouchUpInside(object sender, EventArgs e)
    {
        BT_modifer.Enabled = false;
        if (formTaskDetails != null)
            formTaskDetails.NavigateToSubTask(beanTask);
        else if (controlInputTask != null)
            ((ControlInputTasks)controlInputTask).HandleSelectedItem(beanTask);
        BT_modifer.Enabled = true;
    }

    private void InitFrameViews()
    {
        var indnex = beanTask.index;

        if (beanTask.index == 0)
        {
            icon_expand.Frame = new CGRect(10, 10, 13, 13);
            lbl_verticalLine.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), icon_expand.Frame.Bottom, 1, this.Frame.Height - icon_expand.Frame.Bottom);
            lbl_title.Frame = new CGRect(icon_expand.Frame.Right + 7, 10, 273, 17);
            iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
            lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, Frame.Width - 85 - 10 - (iv_avatar.Frame.Right + 9 + 10), 16); // left lbl_hanXL - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
            lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, Frame.Width - 100 - 10 - (iv_avatar.Frame.Right + 9 + 10), 14); // left lbl_tinhtrang - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
            lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
            lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
            lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, ContentView.Frame.Bottom - 1, ContentView.Frame.Width, 1);
            lbl_horizonSubNode.Hidden = true; // -
            lbl_verticalLine2.Hidden = true; // line2 |
        }

        if (beanTask.index == 1)
        {
            if (beanTask.isLeaf())
            {
                icon_expand.Hidden = true;
                lbl_verticalLine2.Hidden = true;

                if (beanTask.isRoorFinal)
                    lbl_verticalLine.Frame = new CGRect(16.5 * indnex, 0, 1, 17);
                else
                    lbl_verticalLine.Frame = new CGRect(16.5 * indnex, 0, 1, ContentView.Frame.Height);

                lbl_horizonSubNode.Frame = new CGRect(lbl_verticalLine.Frame.Right, 17, 6, 1);
                lbl_title.Frame = new CGRect(lbl_horizonSubNode.Frame.Right + 7, 10, 273, 17);
                iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
                lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, Frame.Width - 85 - 10 - (iv_avatar.Frame.Right + 9 + 10), 16); // left lbl_hanXL - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
                lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, Frame.Width - 100 - 10 - (iv_avatar.Frame.Right + 9 + 10), 14); // left lbl_tinhtrang - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
                //lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, 205, 16);
                //lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
                lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
                lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, ContentView.Frame.Bottom - 1, ContentView.Frame.Width, 1);

            }
            else
            {
                icon_expand.Hidden = false;
                lbl_verticalLine2.Hidden = false;
                if (beanTask.isRoorFinal)
                    lbl_verticalLine.Frame = new CGRect(16.5 * indnex, 0, 1, 17);
                else
                    lbl_verticalLine.Frame = new CGRect(16.5 * indnex, 0, 1, ContentView.Frame.Height);
                //lbl_verticalLine.Frame = new CGRect(16.5, 0, 1, this.Frame.Height);
                lbl_horizonSubNode.Frame = new CGRect(lbl_verticalLine.Frame.Right - 1, 17, 6, 1);
                icon_expand.Frame = new CGRect(lbl_horizonSubNode.Frame.Right, 10, 13, 13);
                lbl_verticalLine2.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), icon_expand.Frame.Bottom, 1, this.Frame.Height - icon_expand.Frame.Bottom);
                lbl_title.Frame = new CGRect(icon_expand.Frame.Right + 7, 10, 273, 17);
                iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
                lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, Frame.Width - 85 - 10 - (iv_avatar.Frame.Right + 9 + 10), 16); // left lbl_hanXL - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
                lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, Frame.Width - 100 - 10 - (iv_avatar.Frame.Right + 9 + 10), 14); // left lbl_tinhtrang - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
                //lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, 205, 16);
                //lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
                lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
                lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, ContentView.Frame.Bottom - 1, ContentView.Frame.Width, 1);
            }
        }

        if (beanTask.index == 2)
        {
            icon_expand.Hidden = true;
            lbl_verticalLine.Frame = new CGRect(16.5, 0, 1, this.Frame.Height);
            if (parentNode != null)
            {
                if (parentNode.isRoorFinal)
                    lbl_verticalLine.Hidden = true;
                else
                    lbl_verticalLine.Hidden = false;
            }
            if (beanTask.isRoorFinal)
                lbl_verticalLine2.Frame = new CGRect(lbl_verticalLine.Frame.Right + 12, 0, 1, 17);
            else
                lbl_verticalLine2.Frame = new CGRect(lbl_verticalLine.Frame.Right + 12, 0, 1, ContentView.Frame.Height);

            lbl_horizonSubNode.Frame = new CGRect(lbl_verticalLine2.Frame.Right - 1, 17, 6, 1);
            lbl_title.Frame = new CGRect(lbl_horizonSubNode.Frame.Right + 7, 10, 273, 17);
            iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
            lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, Frame.Width - 85 - 10 - (iv_avatar.Frame.Right + 9 + 10), 16); // left lbl_hanXL - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
            lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, Frame.Width - 100 - 10 - (iv_avatar.Frame.Right + 9 + 10), 14); // left lbl_tinhtrang - (iv_avatar.Frame.Right + 9 + 10 pixel padding)
            //lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, 205, 16);
            //lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
            lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
            lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
            lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, ContentView.Frame.Bottom - 1, ContentView.Frame.Width, 1);
        }
        //hien tai khong dung lbl_bottomLine

        BT_modifer.Frame = new CGRect(icon_expand.Frame.Right + 10, 0, ContentView.Frame.Width - lbl_title.Frame.Left, ContentView.Frame.Height);
        lbl_bottomLine.Hidden = true;
    }

    private void InitControlsView()
    {
        icon_expand = new UIImageView();
        icon_expand.ContentMode = UIViewContentMode.ScaleAspectFill;

        lbl_verticalLine = new UILabel();
        lbl_verticalLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

        lbl_horizonSubNode = new UILabel();
        lbl_horizonSubNode.BackgroundColor = UIColor.FromRGB(232, 232, 232);

        lbl_verticalLine2 = new UILabel();
        lbl_verticalLine2.BackgroundColor = UIColor.FromRGB(232, 232, 232);
        lbl_title = new UILabel()
        {
            Font = UIFont.FromName("ArialMT", 15f),
            TextColor = UIColor.FromRGB(0, 95, 212),
            Lines = 2,
            TextAlignment = UITextAlignment.Left,
        };

        lbl_nguoiXL = new UILabel()
        {
            Font = UIFont.FromName("ArialMT", 14f),
            TextColor = UIColor.FromRGB(0, 0, 0),
            TextAlignment = UITextAlignment.Left,
        };

        lbl_chucvu = new UILabel()
        {
            Font = UIFont.FromName("ArialMT", 12f),
            TextColor = UIColor.FromRGB(94, 94, 94),
        };

        lbl_hanXL = new UILabel()
        {
            Font = UIFont.FromName("ArialMT", 12f),
            TextColor = UIColor.FromRGB(94, 94, 94),
            TextAlignment = UITextAlignment.Left,
        };

        iv_avatar = new UIImageView();
        iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
        iv_avatar.ClipsToBounds = true;
        iv_avatar.Layer.CornerRadius = 20;
        iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //thuyngo del
        //iv_avatar.Hidden = true;

        lbl_tinhtrang = new UILabel();
        lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 12f);
        lbl_tinhtrang.ClipsToBounds = true;
        lbl_tinhtrang.TextAlignment = UITextAlignment.Center;
        lbl_tinhtrang.Layer.CornerRadius = 3;

        lbl_bottomLine = new UILabel();
        lbl_bottomLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

        BT_modifer = new UIButton();
        this.AddSubviews(new UIView[] { lbl_title, iv_avatar, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang, lbl_verticalLine, icon_expand, lbl_bottomLine });
        this.AddSubviews(lbl_horizonSubNode, lbl_verticalLine2);
        ContentView.AddSubviews(BT_modifer);
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
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                            }
                            else
                                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                            //iv_avatar.Hidden = false;

                            //kiem tra xong cap nhat lai avatar
                            //iv_avatar.Hidden = false;
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
                            //thuyngo del
                            //iv_avatar.Hidden = true;
                        });
                    }
                });
            }
            else
            {
                openFile(filename, image_view);
                //iv_avatar.Hidden = false;
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
