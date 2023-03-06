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
    public class Custom_WorkFlowCell : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        BeanAppBaseExt appBaseRequest { get; set; }
        UILabel lbl_imgCover, lbl_status, lbl_title, lbl_sentTime, lbl_subTitle, lbl_duedate, lbl_leftLine;
        private UIImageView iv_avatar, img_follow, img_attach, img_priority;
        private bool isOdd;

        public Custom_WorkFlowCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;

        }

        public void UpdateCell(BeanAppBaseExt _workflowItem, bool _isOdd)
        {
            isOdd = _isOdd;
            appBaseRequest = _workflowItem;
            viewConfiguration();
            loadData();
        }

        private void viewConfiguration()
        {
            if (isOdd)
                ContentView.BackgroundColor = UIColor.White;
            else
                ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            iv_avatar.ClipsToBounds = true;
            iv_avatar.Layer.CornerRadius = 20;
            iv_avatar.Hidden = true;

            lbl_imgCover = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("ArialMT", 15f),
                BackgroundColor = UIColor.Blue,
                TextColor = UIColor.White
            };
            lbl_imgCover.Layer.CornerRadius = 20;
            lbl_imgCover.ClipsToBounds = true;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 15f),
                TextColor = UIColor.FromRGB(25, 25, 30),
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
            img_attach.Hidden = true;

            img_priority = new UIImageView();
            img_priority.ContentMode = UIViewContentMode.ScaleAspectFit;
            img_priority.Image = UIImage.FromFile("Icons/icon_flag.png");
            img_priority.Hidden = true;

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

            ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_sentTime, lbl_subTitle, img_priority, img_follow, img_attach, lbl_duedate, lbl_status, lbl_leftLine });
        }

        private void loadData()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string workflowID = "";
                if (!string.IsNullOrEmpty(appBaseRequest.ItemUrl))
                    workflowID = CmmFunction.GetWorkflowItemIDByUrl(appBaseRequest.ItemUrl);

                BeanWorkflowItem workflowItem = new BeanWorkflowItem();
                string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowID);
                var _list_workFlowItem = conn.Query<BeanWorkflowItem>(query_workflowItem);
                if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                    workflowItem = _list_workFlowItem[0];

                List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, appBaseRequest.ID);

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

                if (!workflowItem.Read)
                    lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
                else
                    lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular);

                string assignedTo = "";
                string[] arr_assignedTo;
                List<string> lst_userName = new List<string>();
                //var maxStatusWidth = (270 / 3) * 2;
                nfloat temp_width = 0;
                if (!string.IsNullOrEmpty(appBaseRequest.AssignedTo))
                {
                    if (appBaseRequest.AssignedTo.Contains(','))
                    {
                        arr_assignedTo = appBaseRequest.AssignedTo.Split(',');
                        //string res = string.Empty;

                        for (int i = 0; i < arr_assignedTo.Length; i++)
                        {
                            List<BeanUser> lst_userResult = new List<BeanUser>();
                            string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                            lst_userResult = conn.Query<BeanUser>(query_user, arr_assignedTo[i].Trim().ToLower());

                            if (lst_userResult != null && lst_userResult.Count > 0)
                            {
                                lst_userName.Add(lst_userResult[0].FullName);
                            }
                        }

                        if (lst_userName.Count > 1)
                        {
                            int num_remain = lst_userName.Count - 1;
                            assignedTo = lst_userName[0] + ", +" + num_remain.ToString();
                        }
                        else if (lst_userName.Count > 0)
                            assignedTo = lst_userName[0];
                        else
                            assignedTo = "";
                    }
                    else
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        lst_userResult = conn.Query<BeanUser>(query_user, appBaseRequest.AssignedTo.Trim().ToLower());
                        if (lst_userResult != null && lst_userResult.Count > 0)
                        {
                            assignedTo = lst_userResult[0].FullName;
                        }
                    }
                }

                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    lbl_title.Text = appBaseRequest.Content;
                    //line 2
                    if (!string.IsNullOrEmpty(assignedTo))
                        lbl_subTitle.Text = "To: " + assignedTo;
                }
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                {
                    lbl_title.Text = appBaseRequest.Content;
                    //line 2
                    if (!string.IsNullOrEmpty(assignedTo))
                        lbl_subTitle.Text = "Đến: " + assignedTo;
                }

                //line 1
                if (!string.IsNullOrEmpty(appBaseRequest.AssignedTo))
                {
                    string str_assignedTo = appBaseRequest.AssignedTo;

                    string first_user = "";
                    if (str_assignedTo.Contains(','))
                        first_user = str_assignedTo.Split(',')[0];
                    else
                        first_user = str_assignedTo;

                    List<BeanUser> lst_userResult = new List<BeanUser>();
                    string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                    lst_userResult = conn.Query<BeanUser>(query_user, first_user.ToLower());

                    string user_imagePath = "";
                    if (lst_userResult.Count > 0)
                    {
                        user_imagePath = lst_userResult[0].ImagePath;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {
                            lbl_imgCover.Hidden = false;
                            iv_avatar.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName.TrimEnd());
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                        }

                        else
                        {
                            lbl_imgCover.Hidden = false;
                            iv_avatar.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName.TrimEnd());
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                            checkFileLocalIsExist(lst_userResult[0], iv_avatar);
                        }
                    }
                }

                if (appBaseRequest.FileCount > 0)
                    img_attach.Hidden = false;

                if (appBaseRequest.Created.HasValue)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseRequest.Created.Value, 0, 1033);
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(appBaseRequest.Created.Value, 0, 1066);

                }

                string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", appBaseRequest.StatusGroup);
                List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

                if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                {
                    lbl_status.Hidden = false;
                    lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1033") ? _lstAppStatus[0].TitleEN : _lstAppStatus[0].Title;
                    lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(_lstAppStatus[0].ID);
                }
                else
                    lbl_status.Hidden = true;

                if (appBaseRequest.DueDate.HasValue)
                {
                    //if (CmmVariable.SysConfig.LangCode == "1033")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseRequest.DueDate.Value, 1, 1033);
                    //else if (CmmVariable.SysConfig.LangCode == "1066")
                    //    lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(appBaseRequest.DueDate.Value, 1, 1066);

                    string queryTimeLang = string.Format("SELECT * FROM BeanTimeLanguage WHERE Type = {0} AND Devices <> 2 AND Title LIKE '%yy%' ORDER BY [Index]", 0);
                    var format = conn.Query<BeanTimeLanguage>(queryTimeLang).FirstOrDefault();
                    if (format != null)
                        lbl_duedate.Text = appBaseRequest.DueDate.Value.ToString(CmmVariable.SysConfig.LangCode == "1033" ? format.TitleEN : format.Title);
                    else
                    {
                        lbl_duedate.Text = appBaseRequest.DueDate.Value.ToString("dd/MM/yyyy");
#if DEBUG
                        Console.WriteLine("Không lấy được format ngày nên gán tạm");
#endif
                    }

                    if (appBaseRequest.DueDate.Value.Date < DateTime.Now.Date) // qua han => mau do
                        lbl_duedate.TextColor = UIColor.Red;
                    else if (appBaseRequest.DueDate.Value.Date == DateTime.Now.Date) // trong ngay => mau tim=> màu đen
                        lbl_duedate.TextColor = UIColor.Black; //UIColor.FromRGB(139, 79, 183);
                    else if (appBaseRequest.DueDate.Value.Date > DateTime.Now.Date && appBaseRequest.DueDate.Value.Date < DateTime.Now.Date.AddDays(5))
                    {
                        //lbl_duedate.TextColor = UIColor.FromRGB(119, 224, 117);
                        // chuyen mau xanh la => black
                        lbl_duedate.TextColor = UIColor.Black;
                    }
                    else //if (noti.Status == 1)
                    {
                        lbl_duedate.TextColor = UIColor.Black;
                    }
                }

                lbl_leftLine.Hidden = !appBaseRequest.IsSelected;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Custom_WorkFlowItemCell - loaddata- ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            iv_avatar.Frame = new CGRect(15, 13, 40, 40);
            lbl_imgCover.Frame = new CGRect(15, 13, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - (60 + 90), 20);
            lbl_sentTime.Frame = new CGRect(ContentView.Frame.Width - 85, lbl_title.Frame.Y, 75, 20);

            lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, this.ContentView.Frame.Width - 120, 20);
            img_follow.Frame = new CGRect(ContentView.Frame.Width - 50, lbl_subTitle.Frame.Y + 4, 16, 16);
            //img_priority.Frame = new CGRect(img_follow.Frame.Right + 10, lbl_subTitle.Frame.Y + 4, 16, 16);
            img_attach.Frame = new CGRect(img_follow.Frame.Right + 10, lbl_subTitle.Frame.Y + 4, 16, 16);

            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 13).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - (lbl_subTitle.Frame.X + 110);
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
            else
                lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

            lbl_duedate.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_status.Frame.Y, 120, 20);
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
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                iv_avatar.Hidden = false;

                                //kiem tra xong cap nhat lai avatar
                                lbl_imgCover.Hidden = true;
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
                                lbl_imgCover.Hidden = false;
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    iv_avatar.Hidden = false;
                    lbl_imgCover.Hidden = true;
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