using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Todo_cell_custom : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        BeanAppBaseExt appBaseTodo { get; set; }
        UILabel lbl_imgCover, lbl_status, lbl_title, lbl_sentTime, lbl_subTitle, lbl_duedate;
        private UIImageView iv_avatar, img_follow, img_attach, img_priority;
        private bool isOdd, isFollow;
        UIColor color;

        public Todo_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
        }

        public void UpdateCell(BeanAppBaseExt _noti, bool _isOdd, bool _isFollow = false)
        {
            isOdd = _isOdd;
            appBaseTodo = _noti;
            isFollow = _isFollow;
            viewConfiguration();
            loadData();
        }

        private void viewConfiguration()
        {
            if (isOdd)
                ContentView.BackgroundColor = UIColor.White;
            else
                ContentView.BackgroundColor = UIColor.FromRGB(243, 249, 255);

            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
            iv_avatar.ClipsToBounds = true;
            iv_avatar.Layer.CornerRadius = 20;
            iv_avatar.Hidden = true;

            lbl_imgCover = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("ArialMT", 15f),
                TextColor = UIColor.White
            };
            lbl_imgCover.Layer.CornerRadius = 20;
            lbl_imgCover.ClipsToBounds = true;

            lbl_title = new UILabel()
            {
                //Font = UIFont.SystemFontOfSize(15, UIFontWeight.Medium),
                TextColor = UIColor.FromRGB(25, 25, 30),
                Font = UIFont.FromName("Arial-BoldMT", 15f),
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

            img_attach = new UIImageView();
            img_attach.ContentMode = UIViewContentMode.ScaleAspectFit;
            img_attach.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            img_attach.TintColor = UIColor.FromRGB(94, 94, 94);

            //img_priority = new UIImageView();
            //img_priority.ContentMode = UIViewContentMode.ScaleAspectFit;
            //img_priority.Image = UIImage.FromFile("Icons/icon_flag.png");

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
            //ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_sentTime, lbl_subTitle, img_follow, img_priority, img_attach, lbl_duedate, lbl_status });
        }

        private void loadData()
        {
            try
            {
                if (appBaseTodo.Read.HasValue && !appBaseTodo.Read.Value)
                    lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
                else
                    lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular);

                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                BeanWorkflow workflowItem = new BeanWorkflow();
                if (appBaseTodo.WorkflowId == null)
                {

                }
                string query_workflowItem = string.Format("SELECT * FROM BeanWorkflow WHERE WorkflowID = '{0}'  LIMIT 1 OFFSET 0", appBaseTodo.WorkflowId);
                var _list_workFlowItem = conn.Query<BeanWorkflow>(query_workflowItem);
                if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                    workflowItem = _list_workFlowItem[0];

                List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, appBaseTodo.ID);

                if (lst_followResult != null && lst_followResult.Count > 0)
                {
                    if (lst_followResult[0].Status == 1)
                    {
                        img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                        //img_follow.Hidden = false;
                    }
                    else
                    {
                        img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                        //img_follow.Hidden = true;
                    }
                }
                else
                {
                    //img_follow.Hidden = true;
                    img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                }

                lbl_status.Lines = 1;
                lbl_status.SizeToFit();

                if (appBaseTodo.Created.HasValue)
                {
                    if (CmmVariable.SysConfig.LangCode.Equals("1033"))
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.Created.Value, 0, 1033);
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.Created.Value, 0, 1066);
                }

                #region Set Avatar
                //if (!string.IsNullOrEmpty(appBaseTodo.AssignedBy) || !string.IsNullOrEmpty(appBaseTodo.AssignedTo))
                //{
                //    //List<BeanUser> lst_userResult = new List<BeanUser>();
                //    //string query_user = string.Format("SELECT * FROM BeanUser WHERE ID LIKE ?");
                //    //lst_userResult = conn.Query<BeanUser>(query_user, appBaseTodo.AssignedBy);
                //    List<BeanUserAndGroup> lst_userAndGroupResult = new List<BeanUserAndGroup>();
                //    string _queryBeanUserGroup = string.Format(@"SELECT Image as ImagePath, 1 as Type, Title as Name FROM BeanGroup WHERE ID LIKE '{0}' "
                //        + "UNION SELECT ImagePath, 0 as Type, FullName as Name   FROM BeanUser WHERE ID LIKE '{0}'"
                //        , !isFollow ? appBaseTodo.AssignedBy : appBaseTodo.AssignedTo);
                //    lst_userAndGroupResult = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                //    string user_imagePath = "";
                //    if (lst_userAndGroupResult != null && lst_userAndGroupResult.Count > 0)
                //    {
                //        lbl_imgCover.Hidden = true;
                //        user_imagePath = lst_userAndGroupResult[0].ImagePath;
                //        if (string.IsNullOrEmpty(user_imagePath))
                //        {
                //            iv_avatar.Hidden = false;
                //            if (lst_userAndGroupResult[0].Type == 1)
                //                iv_avatar.Image = UIImage.FromFile("Icons/icon_group.png");
                //            else if (lst_userAndGroupResult[0].Type == 0)
                //                iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                //        }
                //        else
                //        {
                //            lbl_imgCover.Hidden = false;
                //            iv_avatar.Hidden = true;
                //            lbl_imgCover.Text = CmmFunction.GetAvatarName(appBaseTodo.AssignedBy);
                //            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                //            checkFileLocalIsExist(lst_userAndGroupResult[0], iv_avatar);
                //            //kiem tra xong cap nhat lai avatar
                //            lbl_imgCover.Hidden = true;
                //            iv_avatar.Hidden = false;
                //        }
                //    }
                //    else
                //    {
                //        iv_avatar.Hidden = false;
                //        lbl_imgCover.Hidden = true;
                //    }
                //}
                //else
                //{
                //    iv_avatar.Hidden = false;
                //    lbl_imgCover.Hidden = true;
                //}
                #endregion
                #region Set Avatar new way để giảm thời gian load phiếu, dùng thông tin trong beanAppBaseExt
                if (!string.IsNullOrEmpty(appBaseTodo.UserImage) && !string.IsNullOrEmpty(appBaseTodo.UserName))
                {
                    lbl_imgCover.Hidden = false;
                    iv_avatar.Hidden = true;
                    lbl_imgCover.Text = CmmFunction.GetAvatarName(appBaseTodo.UserName);
                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    checkFileLocalIsExist(new BeanUserAndGroup { ImagePath = appBaseTodo.UserImage }, iv_avatar);
                    //kiem tra xong cap nhat lai avatar
                    lbl_imgCover.Hidden = true;
                    iv_avatar.Hidden = false;
                }
                else
                {
                    iv_avatar.Hidden = false;
                    lbl_imgCover.Hidden = true;
                }
                #endregion
                lbl_status.Frame = new CGRect(90, 60, 50, 20);

                if (appBaseTodo.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                {
                    lbl_title.Text = appBaseTodo.Content;
                    lbl_subTitle.Text = CmmFunction.GetTitle("TEXT_TASK", "Công việc");

                    string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", appBaseTodo.StatusGroup);
                    List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

                    if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                    {
                        lbl_status.Hidden = false;
                        lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1033") ? _lstAppStatus[0].TitleEN : _lstAppStatus[0].Title;
                        lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(_lstAppStatus[0].ID);
                        if (_lstAppStatus[0].ID == 4 || _lstAppStatus[0].ID == 8) // Đang thực hiện // Hoàn tất
                            lbl_duedate.Hidden = true;
                    }
                    else
                        lbl_status.Hidden = true;

                }
                else
                {
                    string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", appBaseTodo.StatusGroup);
                    List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

                    if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                    {
                        lbl_status.Hidden = false;
                        lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1033") ? _lstAppStatus[0].TitleEN : _lstAppStatus[0].Title;
                        lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(_lstAppStatus[0].ID);
                    }
                    else
                        lbl_status.Hidden = true;

                    if (CmmVariable.SysConfig.LangCode.Equals("1033"))
                    {
                        lbl_title.Text = appBaseTodo.Content;
                        lbl_subTitle.Text = workflowItem.TitleEN;
                        //lbl_status.Text = workflowItem.ActionStatusEN;
                    }
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                    {
                        lbl_title.Text = appBaseTodo.Content;
                        lbl_subTitle.Text = workflowItem.Title;
                        //lbl_status.Text = workflowItem.ActionStatus;
                    }
                }

                if (appBaseTodo.FileCount > 0)
                    img_attach.Hidden = false;
                else
                    img_attach.Hidden = true;

                //if (appBaseTodo.Priority == 3)
                //    img_priority.Hidden = false;
                //else
                //    img_priority.Hidden = true;

                if (appBaseTodo.DueDate.HasValue)
                {
                    //if (CmmVariable.SysConfig.LangCode == "1033")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.DueDate.Value, 1, 1033);
                    //else if (CmmVariable.SysConfig.LangCode == "1066")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.DueDate.Value, 1, 1066);

                    string queryTimeLang = string.Format("SELECT * FROM BeanTimeLanguage WHERE Type = {0} AND Devices <> 2 AND Title LIKE '%yy%' ORDER BY [Index]", 0);
                    var format = conn.Query<BeanTimeLanguage>(queryTimeLang).FirstOrDefault();
                    if (format != null)
                        lbl_duedate.Text = appBaseTodo.DueDate.Value.ToString(CmmVariable.SysConfig.LangCode.Equals("1033") ? format.TitleEN : format.Title);
                    else
                    {
                        lbl_duedate.Text = appBaseTodo.DueDate.Value.ToString("dd/MM/yyyy");
#if DEBUG
                        Console.WriteLine("Không lấy được format ngày nên gán tạm");
#endif
                    }

                    if (appBaseTodo.DueDate.Value.Date < DateTime.Now.Date) // qua han => mau do
                        lbl_duedate.TextColor = UIColor.Red;
                    else if (appBaseTodo.DueDate.Value.Date == DateTime.Now.Date) // trong ngay => mau tim => màu đen
                        //lbl_duedate.TextColor = UIColor.FromRGB(139, 79, 183);
                        // chuyen mau tim => black
                        lbl_duedate.TextColor = UIColor.Black;
                    else if (appBaseTodo.DueDate.Value.Date > DateTime.Now.Date && appBaseTodo.DueDate.Value.Date < DateTime.Now.Date.AddDays(5))
                    {
                        //lbl_duedate.TextColor = UIColor.FromRGB(119, 224, 117);
                        // chuyen mau xanh la => black
                        lbl_duedate.TextColor = UIColor.Black;
                    }
                    else
                    {
                        lbl_duedate.TextColor = UIColor.Black;
                    }
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
            iv_avatar.Frame = new CGRect(15, 13, 40, 40);
            lbl_imgCover.Frame = new CGRect(15, 13, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - (60 + 90), 20);
            lbl_sentTime.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_title.Frame.Y, 120, 20);//
            //img_priority.Frame = new CGRect(ContentView.Frame.Width - 50, lbl_subTitle.Frame.Y + 4, 16, 16);

            img_attach.Frame = new CGRect(ContentView.Frame.Width - 30, lbl_title.Frame.Bottom + 4, 16, 16);
            img_follow.Frame = new CGRect(img_attach.Frame.Left - 30, lbl_title.Frame.Bottom + 4, 16, 16);
            lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, ContentView.Frame.Width - lbl_title.Frame.Left - 70, 20);

            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 13).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - (lbl_title.Frame.X + 5 + 110);
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
            else
                lbl_status.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

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
                                iv_avatar.Hidden = false;
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
