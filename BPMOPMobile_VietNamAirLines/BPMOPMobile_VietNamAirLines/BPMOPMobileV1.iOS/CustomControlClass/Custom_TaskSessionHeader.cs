
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
    public class Custom_TaskSessionHeader : UIView
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UILabel lbl_title, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang, lbl_verticalLine, lbl_verticalLine2, lbl_horizonSubNode;
        UILabel lbl_bottomLine;
        bool isSubNode;
        bool isRoot;
        UITableView tableView { get; set; }
        string currentTaskID;
        bool itemSelected;
        BeanTask beanTask;
        UIImageView icon_expand, iv_avatar;
        ControlBase parentView { get; set; }


        public Custom_TaskSessionHeader(CGRect _frame, bool _isSubNode)
        {
            isSubNode = _isSubNode;
            InitControlsView();
            InitFrameViews(_frame);

        }

        private void InitControlsView()
        {
            icon_expand = new UIImageView();
            icon_expand.Image = UIImage.FromFile("Icons/icon_collapse.png");
            icon_expand.ContentMode = UIViewContentMode.ScaleAspectFill;
            icon_expand.BackgroundColor = UIColor.FromRGB(251, 251, 251);
            icon_expand.Hidden = true;

            lbl_verticalLine = new UILabel();
            lbl_verticalLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

            if (isSubNode)
            {
                lbl_horizonSubNode = new UILabel();
                lbl_horizonSubNode.BackgroundColor = UIColor.FromRGB(232, 232, 232);

                lbl_verticalLine2 = new UILabel();
                lbl_verticalLine2.BackgroundColor = UIColor.FromRGB(232, 232, 232);
            }

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
            iv_avatar.Layer.CornerRadius = 20;
            iv_avatar.Hidden = true;

            lbl_tinhtrang = new UILabel();
            lbl_tinhtrang.Font = UIFont.FromName("ArialMT", 12f);
            lbl_tinhtrang.ClipsToBounds = true;
            lbl_tinhtrang.TextAlignment = UITextAlignment.Center;
            lbl_tinhtrang.Layer.CornerRadius = 3;

            lbl_bottomLine = new UILabel();
            lbl_bottomLine.BackgroundColor = UIColor.FromRGB(232, 232, 232);

            this.AddSubviews(new UIView[] { lbl_title, iv_avatar, lbl_nguoiXL, lbl_chucvu, lbl_hanXL, lbl_tinhtrang, lbl_verticalLine, icon_expand, lbl_bottomLine });

            if (isSubNode)
                this.AddSubviews(lbl_horizonSubNode, lbl_verticalLine2);
        }

        private void InitFrameViews(CGRect _frame)
        {
            this.Frame = _frame;
            //this.BackgroundColor = UIColor.White;

            var width = this.Frame.Width;
            if (isSubNode)
            {
                //set temp icon
                icon_expand.Frame = new CGRect(10, 10, 13, 13);
                lbl_verticalLine.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), icon_expand.Frame.Bottom, 1, this.Frame.Height - icon_expand.Frame.Bottom);
                //lbl_verticalLine.Frame = new CGRect(10, 0, 1, this.Frame.Height);
                lbl_horizonSubNode.Frame = new CGRect(lbl_verticalLine.Frame.Right, this.Frame.Height / 2, 15, 1);
                icon_expand.Frame = new CGRect(lbl_horizonSubNode.Frame.Right, 10, 13, 13);
                lbl_verticalLine2.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), icon_expand.Frame.Bottom, 1, this.Frame.Height - icon_expand.Frame.Bottom);
                //lbl_verticalLine2.Frame = new CGRect(icon_expand.Frame.Right - 6, icon_expand.Frame.Bottom + 5, 1, this.Frame.Height);
                lbl_title.Frame = new CGRect(icon_expand.Frame.Right + 7, 10, 273, 17);
                //nfloat width_remain = width - lbl_title.Frame.Width;
                iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
                lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, 205, 16);
                lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
                lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
            }
            else
            {
                //lbl_tinhtrang.Frame = new CGRect(lbl_chucvu.Frame.Right + 13, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                lbl_chucvu.BackgroundColor = UIColor.Yellow;
                lbl_tinhtrang.BackgroundColor = UIColor.Black;
                lbl_hanXL.BackgroundColor = UIColor.Green;
                lbl_nguoiXL.BackgroundColor = UIColor.Gray;
                lbl_chucvu.BackgroundColor = UIColor.LightGray;
                nfloat widthContent = Frame.Width;
                nfloat widthContentDefaultIP8 = 385;
                icon_expand.Frame = new CGRect(10, 10, 13, 13);
                lbl_verticalLine.Frame = new CGRect(icon_expand.Frame.Left + (icon_expand.Frame.Width / 2), icon_expand.Frame.Bottom, 1, this.Frame.Height - icon_expand.Frame.Bottom);
                //lbl_title.Frame = new CGRect(icon_expand.Frame.Right + 10, 10, widthContent * ((float)270 / widthContentDefaultIP8), 17);
                lbl_title.Frame = new CGRect(icon_expand.Frame.Right + 7, 10, 273, 17);
                iv_avatar.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 11, 40, 40);
                //lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, widthContent * ((float)200 / widthContentDefaultIP8), 16);
                //lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, widthContent * ((float)175 / widthContentDefaultIP8), 14);
                lbl_nguoiXL.Frame = new CGRect(iv_avatar.Frame.Right + 9, iv_avatar.Frame.Top, 205, 16);
                lbl_chucvu.Frame = new CGRect(iv_avatar.Frame.Right + 9, lbl_nguoiXL.Frame.Bottom + 9, 175, 14);
                lbl_hanXL.Frame = new CGRect(Frame.Width - 85 - 10, lbl_nguoiXL.Frame.Top, 85, 14);
            }

            //lbl_tinhtrang.Frame = new CGRect(this.Frame.Width - 210, 22, 210, 26);
            lbl_bottomLine.Frame = new CGRect(lbl_title.Frame.X, _frame.Bottom - 1, _frame.Width, 1);
            
        }

        public void LoadData(BeanTask _task, UITableView _tableView, bool _isRoot, ControlBase _parentView)
        {
            try
            {
                beanTask = _task;
                tableView = _tableView;
                isRoot = _isRoot;
                parentView = _parentView;

                if (isRoot)
                    lbl_title.Font = UIFont.FromName("Arial-BoldMT", 15f);

                if (beanTask.ChildTask != null && beanTask.ChildTask.Count == 0)
                {
                    lbl_verticalLine.Hidden = true;
                    lbl_bottomLine.Hidden = true;
                    icon_expand.Hidden = true;
                    lbl_title.Font = UIFont.FromName("ArialMT", 15f);
                }
                else
                {
                    icon_expand.Hidden = false;
                    lbl_title.Font = UIFont.FromName("Arial-BoldMT", 15f);
                }

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
                }

                if (beanTask.DueDate.HasValue)
                    lbl_hanXL.Text = beanTask.DueDate.Value.ToString("dd/MM/yy HH:mm");
                else
                    lbl_hanXL.Text = string.Empty;

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

                var width = this.Frame.Width;
                nfloat width_remain = width - lbl_title.Frame.Width;
                lbl_tinhtrang.Frame = new CGRect(Frame.Width - 100 - 10, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                //if (isSubNode)
                //    lbl_tinhtrang.Frame = new CGRect(lbl_chucvu.Frame.Right + 13, lbl_hanXL.Frame.Bottom + 5, 100, 20);
                //else
                //    lbl_tinhtrang.Frame = new CGRect(lbl_chucvu.Frame.Right + 5, 22, lbl_hanXL.Frame.Width, 26);

            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
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

