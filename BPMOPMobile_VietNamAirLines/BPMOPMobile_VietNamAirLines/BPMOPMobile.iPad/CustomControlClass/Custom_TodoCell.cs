using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_TodoCell : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        BeanAppBaseExt appBaseTodo { get; set; }
        UILabel lbl_imgCover, lbl_status, lbl_title, lbl_sentTime, lbl_subTitle, lbl_duedate, lbl_leftLine;
        private UIImageView iv_avatar, img_follow, img_attach, img_priority;
        private bool isOdd;

        NSLayoutConstraint constraintRightIconAttachment;

        public Custom_TodoCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
        }

        public Custom_TodoCell(IntPtr handle) : base(handle)
        {
        }

        public Custom_TodoCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(BeanAppBaseExt _appBaseTodo, bool _isOdd)
        {
            appBaseTodo = _appBaseTodo;
            isOdd = _isOdd;
            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            if (isOdd)
                ContentView.BackgroundColor = UIColor.White;
            else
                ContentView.BackgroundColor = UIColor.FromRGB(243, 249, 255);

            iv_avatar = new UIImageView
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromFile("Icons/icon_profile.png"),
                ClipsToBounds = true
            };
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
            img_attach.ContentMode = UIViewContentMode.ScaleAspectFill;
            img_attach.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            img_attach.TintColor = UIColor.FromRGB(94, 94, 94);

            img_priority = new UIImageView();
            img_priority.ContentMode = UIViewContentMode.ScaleAspectFit;
            img_priority.Image = UIImage.FromFile("Icons/icon_flag.png");

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

            lbl_leftLine = new UILabel();
            lbl_leftLine.BackgroundColor = UIColor.FromRGB(51, 95, 179);
            lbl_leftLine.Hidden = true;

            ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_sentTime, lbl_subTitle, img_follow, img_priority, img_attach, lbl_duedate, lbl_status, lbl_leftLine });
        }

        private void LoadData()
        {

            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            try
            {
                if (appBaseTodo.Read.HasValue && !appBaseTodo.Read.Value)
                    lbl_title.Font = UIFont.FromName("Arial-BoldMT", 13f);
                else
                    lbl_title.Font = UIFont.FromName("ArialMT", 13f);
                string workflowID = "";
                if (!string.IsNullOrEmpty(appBaseTodo.ItemUrl))
                    workflowID = CmmFunction.GetWorkflowItemIDByUrl(appBaseTodo.ItemUrl);

                BeanWorkflowItem workflowItem = new BeanWorkflowItem();
                string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
                var _list_workFlowItem = conn.Query<BeanWorkflowItem>(query_workflowItem, workflowID);
                if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                    workflowItem = _list_workFlowItem[0];

                List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, workflowID);

                if (lst_followResult != null && lst_followResult.Count > 0)
                {
                    if (lst_followResult[0].Status == 1)
                    {
                        img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                        img_follow.Hidden = false;
                    }
                    else
                    {
                        img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                        img_follow.Hidden = true;
                    }
                }
                else
                {
                    img_follow.Hidden = true;
                    img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                }

                if (appBaseTodo.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                {

                }
                else
                {

                }

                lbl_status.Lines = 1;
                lbl_status.SizeToFit();

                if (appBaseTodo.Created.HasValue)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.Created.Value, 0, 1033);
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.Created.Value, 0, 1066);
                }

                if (!string.IsNullOrEmpty(appBaseTodo.AssignedBy))
                {
                    List<BeanUser> lst_userResult = new List<BeanUser>();
                    string query_user = string.Format("SELECT * FROM BeanUser WHERE ID LIKE ?");
                    lst_userResult = conn.Query<BeanUser>(query_user, appBaseTodo.AssignedBy);

                    string user_imagePath = "";
                    if (lst_userResult.Count > 0)
                        user_imagePath = lst_userResult[0].ImagePath;

                    if (string.IsNullOrEmpty(user_imagePath))
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(appBaseTodo.AssignedBy);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    }
                    else
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName.TrimEnd());
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                        checkFileLocalIsExist(lst_userResult[0], iv_avatar);

                        //kiem tra xong cap nhat lai avatar
                        lbl_imgCover.Hidden = true;
                        iv_avatar.Hidden = false;
                    }
                }
                else
                {
                    iv_avatar.Hidden = false;
                    lbl_imgCover.Hidden = true;
                }

                lbl_status.Frame = new CGRect(90, 60, 50, 20);

                //Task
                if (appBaseTodo.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                {
                    lbl_title.Text = appBaseTodo.Content;
                    lbl_subTitle.Text = CmmFunction.GetTitle("TEXT_TASK", "Công việc");
                }
                // Workflow
                else
                {
                    //noi dung phieu
                    lbl_title.Text = appBaseTodo.Content;
                    //lbl_title.Text = workflowItem.Content;

                    //Load ten quy trinh
                    string query_workflow = string.Format("SELECT * FROM BeanWorkflow WHERE WorkflowID = '{0}'", appBaseTodo.WorkflowId);
                    List<BeanWorkflow> beanWorkflows = conn.Query<BeanWorkflow>(query_workflow);
                    if (beanWorkflows != null && beanWorkflows.Count > 0)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_subTitle.Text = beanWorkflows[0].TitleEN;
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            lbl_subTitle.Text = beanWorkflows[0].Title;
                    }
                }

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

                if (appBaseTodo.FileCount > 0)
                    img_attach.Hidden = false;
                else
                    img_attach.Hidden = true;

                if (appBaseTodo.Priority == 3)
                    img_priority.Hidden = false;
                else
                    img_priority.Hidden = true;

                if (appBaseTodo.DueDate.HasValue)
                {
                    //if (CmmVariable.SysConfig.LangCode == "1033")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.DueDate.Value, 1, 1033);
                    //else if (CmmVariable.SysConfig.LangCode == "1066")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseTodo.DueDate.Value, 1, 1066);

                    string queryTimeLang = string.Format("SELECT * FROM BeanTimeLanguage WHERE Type = {0} AND Devices <> 2 AND Title LIKE '%yy%' ORDER BY [Index]", 0);
                    var format = conn.Query<BeanTimeLanguage>(queryTimeLang).FirstOrDefault();
                    if (format != null)
                        lbl_duedate.Text = appBaseTodo.DueDate.Value.ToString(CmmVariable.SysConfig.LangCode == "1033" ? format.TitleEN : format.Title);
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
                        lbl_duedate.TextColor = UIColor.Black; //UIColor.FromRGB(139, 79, 183);
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
                else
                {
                    // show for test
                    //lbl_duedate.Text = "null";
                }

                if (appBaseTodo.IsSelected)
                    lbl_leftLine.Hidden = false;
                else
                    lbl_leftLine.Hidden = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            iv_avatar.Frame = new CGRect(15, 20, 40, 40);
            lbl_imgCover.Frame = new CGRect(15, 20, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 20, ContentView.Frame.Width - (60 + 90), 22);
            lbl_sentTime.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_title.Frame.Y, 120, 22);//75
            lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, this.ContentView.Frame.Width - 120, 22);
            img_follow.Frame = new CGRect(ContentView.Frame.Width - 50, lbl_subTitle.Frame.Y + 4, 16, 16);
            //img_priority.Frame = new CGRect(img_follow.Frame.Right + 10, lbl_subTitle.Frame.Y + 4, 16, 16);
            img_attach.Frame = new CGRect(img_follow.Frame.Right + 10, lbl_subTitle.Frame.Y + 4, 16, 16);

            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 13).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - (lbl_title.Frame.X + 5 + 110);
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
            else
                lbl_status.Frame = new CGRect(lbl_title.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

            lbl_duedate.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_status.Frame.Y, 120, 22);
            lbl_leftLine.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);
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
                                image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
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
    }
}