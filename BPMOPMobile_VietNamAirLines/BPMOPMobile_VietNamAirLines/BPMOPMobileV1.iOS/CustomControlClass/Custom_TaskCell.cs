
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
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_TaskCell : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UIView viewBackground;
        UILabel lbl_title, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang, lbl_verticalLine, lbl_verticalLine2, lbl_horizonLine, lbl_bottomLine;
        UITableView tableSubTask;
        UIButton btn_action;
        BeanTask beanTask;
        string currentTaskID;
        bool isHasSubTask;
        bool isRoot;
        bool lv2;
        UIImageView icon_expand, iv_avatar;
        UIViewController parentView { get; set; }
        ControlInputTasks controlBase { get; set; }
        NSIndexPath root_nSIndexPath;

        public Custom_TaskCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;

        }

        public void UpdateCell(BeanTask _beanTask, NSIndexPath _root_indexpath, bool _lv2, bool _isRoot, string _currentTaskID, ControlInputTasks _controlBase)
        {
            isRoot = _isRoot;
            root_nSIndexPath = _root_indexpath;
            beanTask = _beanTask;
            lv2 = _lv2;
            currentTaskID = _currentTaskID;
            controlBase = _controlBase;

            if (beanTask.ChildTask != null && beanTask.ChildTask.Count > 0)
                isHasSubTask = true;
            else
                isHasSubTask = false;

            ViewConfiguration();
        }

        private void ViewConfiguration()
        {
            ContentView.BackgroundColor = UIColor.FromRGB(251, 251, 251);

            viewBackground = new UIView();
            //viewBackground.BackgroundColor = UIColor.Orange;

            if (!isHasSubTask)
            {
                icon_expand = new UIImageView();
                icon_expand.BackgroundColor = UIColor.White;
                icon_expand.Image = UIImage.FromFile("Icons/icon_colapse.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate); //icon_expand2
                icon_expand.ContentMode = UIViewContentMode.ScaleAspectFill;
                icon_expand.Hidden = true;

                lbl_verticalLine = new UILabel();
                lbl_verticalLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);
                lbl_verticalLine.Frame = new CGRect(10, 0, 1, ContentView.Frame.Height);

                lbl_verticalLine2 = new UILabel();
                lbl_verticalLine2.BackgroundColor = UIColor.FromRGB(232, 232, 232);

                lbl_horizonLine = new UILabel();
                lbl_horizonLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

                lbl_bottomLine = new UILabel();
                //dang tam khong hien thi
                lbl_bottomLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(51, 95, 179),
                    Lines = 2,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_nguoiXL = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
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
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                iv_avatar = new UIImageView();
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.ClipsToBounds = true;
                iv_avatar.Layer.CornerRadius = 18;
                iv_avatar.Hidden = true;

                lbl_tinhtrang = new UILabel();
                lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 12f);
                lbl_tinhtrang.ClipsToBounds = true;
                lbl_tinhtrang.TextAlignment = UITextAlignment.Center;
                lbl_tinhtrang.Layer.CornerRadius = 3;

                viewBackground.AddSubviews(new UIView[] { icon_expand, lbl_title, iv_avatar, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang });
                ContentView.Add(lbl_verticalLine); ContentView.Add(lbl_verticalLine2);
                ContentView.Add(lbl_horizonLine);
                ContentView.Add(viewBackground);
                ContentView.Add(lbl_bottomLine);
                LoadData();

            }
            else
            {
                tableSubTask = new UITableView(new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height), UITableViewStyle.Grouped);
                tableSubTask.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                tableSubTask.ScrollEnabled = false;
                ContentView.Add(tableSubTask);
                LoadSubTask();
            }
        }

        private void LoadSubTask()
        {
            tableSubTask.Source = new Custom_TableTaskAssignment(beanTask, controlBase, tableSubTask, isRoot, root_nSIndexPath);
            tableSubTask.ReloadData();
        }

        //khong co sub task - row -> header
        private void LoadData()
        {
            try
            {
                if (!string.IsNullOrEmpty(beanTask.AssignedId))
                {

                    lbl_title.Text = beanTask.Title;

                    //if (isRoot)
                    //    lbl_title.Font = UIFont.FromName("Arial-BoldMT", 14f);

                    if (beanTask.ChildTask != null && beanTask.ChildTask.Count > 0)
                        lbl_title.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    else
                        lbl_title.Font = UIFont.FromName("ArialMT", 14f);

                    List<BeanUser> lst_userResult = new List<BeanUser>();
                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                    lst_userResult = conn.Query<BeanUser>(query_user, beanTask.AssignedId);

                    string user_imagePath = "";
                    if (lst_userResult.Count > 0)
                    {
                        lbl_nguoiXL.Text = lst_userResult[0].FullName;
                        lbl_chucvu.Text = lst_userResult[0].Position;
                        user_imagePath = lst_userResult[0].ImagePath;

                        if (!string.IsNullOrEmpty(user_imagePath))
                            checkFileLocalIsExist(lst_userResult[0], iv_avatar);
                    }

                }

                if (beanTask.DueDate.HasValue)
                    lbl_hanXL.Text = beanTask.DueDate.Value.ToString("dd/MM/yy HH:mm");
                else
                    lbl_hanXL.Text = string.Empty;

                lbl_tinhtrang.Frame = new CGRect(ContentView.Frame.Width - 110, ContentView.Frame.Height - 30, 100, 20);
                //lbl_tinhtrang.Frame = new CGRect(90, 60, 50, 20);
                lbl_tinhtrang.Hidden = false;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat width;
            if (!isHasSubTask)
            {
                if (lv2)
                {
                    lbl_verticalLine.Frame = new CGRect(10, 0, 1, ContentView.Frame.Height);
                    lbl_verticalLine2.Frame = new CGRect(33, 0, 1, ContentView.Frame.Height);
                    lbl_horizonLine.Frame = new CGRect(lbl_verticalLine2.Frame.Right, ContentView.Frame.Height / 2, 10, 1);

                    viewBackground.Frame = new CGRect(40, 2, ContentView.Frame.Width - 40, 66);
                    width = viewBackground.Frame.Width;

                    //cell thi lui vao 20
                    lbl_title.Frame = new CGRect(20, 10, ((width / 6) * 1.8f) - 35, 50);

                }
                else if (root_nSIndexPath == null) // danh sach sub task trong view
                {
                    lbl_verticalLine.Frame = new CGRect(7, 0, 1, ContentView.Frame.Height);
                    lbl_horizonLine.Frame = new CGRect(lbl_verticalLine.Frame.X, ContentView.Frame.Height / 2, 20, 1);

                    viewBackground.Frame = new CGRect(20, 2, ContentView.Frame.Width - 20, 66);
                    width = viewBackground.Frame.Width;

                    //cell thi lui vao 20
                    lbl_title.Frame = new CGRect(20, 10, ((width / 6) * 1.8f) - 22, 50);
                }
                else
                {
                    //cell thi lui vao 20
                    lbl_title.Frame = new CGRect(20, 10, 326, 17);

                    viewBackground.Frame = new CGRect(20, 2, ContentView.Frame.Width - 20, 90);
                    width = viewBackground.Frame.Width;

                    icon_expand.Frame = new CGRect(0, 10, 13, 13);
                    lbl_verticalLine.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), 0, 1, lbl_title.Frame.Top + (lbl_title.Frame.Height / 2));
                    //lbl_verticalLine.Frame = new CGRect(20, 0, 1, ContentView.Frame.Height);
                    lbl_horizonLine.Frame = new CGRect(lbl_verticalLine.Frame.X, lbl_title.Frame.Top + (lbl_title.Frame.Height / 2), 20, 1);
                    //lbl_horizonLine.Frame = new CGRect(lbl_verticalLine.Frame.X, lbl_title.Frame.Top + (lbl_title.Frame.Height / 2), 20, 1);
                    icon_expand.Frame = new CGRect(0, 0, 0, 0);

                }
                lbl_title.BackgroundColor = UIColor.Yellow;
                lbl_tinhtrang.BackgroundColor = UIColor.Black;
                lbl_hanXL.BackgroundColor = UIColor.Green;
                lbl_nguoiXL.BackgroundColor = UIColor.Gray;
                lbl_chucvu.BackgroundColor = UIColor.LightGray;
                icon_expand.Frame = new CGRect(0, 10, 13, 13);

                //nfloat width_remain = width - lbl_title.Frame.Width;
                iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
                lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 10, iv_avatar.Frame.Top, 205, 16);
                lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 10, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
                lbl_hanXL.Frame = new CGRect(viewBackground.Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
                lbl_tinhtrang.Frame = new CGRect(viewBackground.Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, viewBackground.Frame.Bottom - 1, viewBackground.Frame.Width, 1);
            }
            else
            {
                tableSubTask.Frame = new CGRect(ContentView.Frame.X, ContentView.Frame.Y, ContentView.Frame.Width, ContentView.Frame.Height);
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
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                iv_avatar.Hidden = false;

                                //kiem tra xong cap nhat lai avatar
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
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    iv_avatar.Hidden = false;
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
}
