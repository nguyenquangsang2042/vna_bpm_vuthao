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
    class Custom_NewWorkFlowCell : UITableViewCell
    {
        BeanAppBaseExt appBase { get; set; }
        BeanWorkflowItem workFlowItem { get; set; }
        UIView backgroundLogo;
        UIImageView iv_logo;
        UILabel lbl_workflow_name, lbl_title, lbl_create_date, lbl_toUser, lbl_status, lbl_bot_line;
        UIView view_info;
        int styleCell = 0;// 0: icon logo bên trái | 1: icon logo bên phải
        Custom_UserList custom_UserList;
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        List<BeanUser> lst_user = new List<BeanUser>();

        public Custom_NewWorkFlowCell(NSString cellID, int _styleCell) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
            styleCell = _styleCell;
        }

        public Custom_NewWorkFlowCell(IntPtr handle) : base(handle)
        {
        }

        public Custom_NewWorkFlowCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(BeanAppBaseExt _appbase)
        {
            appBase = _appbase;

            string workflowID = "";
            if (!string.IsNullOrEmpty(appBase.ItemUrl))
                workflowID = CmmFunction.GetWorkflowItemIDByUrl(appBase.ItemUrl);

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowID);
            var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

            if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                workFlowItem = _list_workFlowItem[0];

            //test data
            //lst_user.Add(new KeyValuePair<string, string>("Nguyen Van A", "url"));

            //var name = "";
            //if (!string.IsNullOrEmpty(workFlowItem.AssignedToName))
            //    name = workFlowItem.AssignedToName;
            //else
            //    name = workFlowItem.AssignedToName;

            //lst_user.Add(new KeyValuePair<string, string>("Nguyen Van B", name));
            //lst_user.Add(new KeyValuePair<string, string>("Nguyen Van C", "abt"));
            //end test

            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            backgroundLogo = new UIView();
            backgroundLogo.BackgroundColor = UIColor.White;
            backgroundLogo.Layer.MasksToBounds = false;
            backgroundLogo.Layer.CornerRadius = 6;
            backgroundLogo.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.6f).CGColor;
            backgroundLogo.Layer.BorderWidth = 0.2f;
            backgroundLogo.Layer.ShadowOffset = new CGSize(2, 2);
            backgroundLogo.Layer.ShadowRadius = 2;
            backgroundLogo.Layer.ShadowOpacity = 0.3f;

            iv_logo = new UIImageView();
            iv_logo.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_logo.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

            lbl_workflow_name = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 15f),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Center
            };

            view_info = new UIView();

            lbl_title = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(14),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left
            };

            lbl_create_date = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13),
                TextColor = UIColor.FromRGB(94, 94, 94),
                TextAlignment = UITextAlignment.Right
            };

            custom_UserList = new Custom_UserList();

            lbl_toUser = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.FromRGB(94, 94, 94),
                TextAlignment = UITextAlignment.Left
            };

            lbl_status = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Center,
            };

            lbl_status.ClipsToBounds = true;
            lbl_status.Layer.CornerRadius = 3;
            lbl_status.Lines = 1;
            lbl_status.SizeToFit();

            lbl_bot_line = new UILabel();
            lbl_bot_line.BackgroundColor = UIColor.FromRGB(234, 237, 243);

            view_info.AddSubviews(new UIView[] { lbl_title, lbl_create_date, custom_UserList, lbl_toUser, lbl_status });
            ContentView.AddSubviews(new UIView[] { backgroundLogo, iv_logo, lbl_workflow_name, view_info, lbl_bot_line });
        }

        private void LoadData()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);

                if (!workFlowItem.Read)
                    lbl_title.Font = UIFont.FromName("Arial-BoldMT", 15f);
                else
                    lbl_title.Font = UIFont.FromName("ArialMT", 15f);

                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_workflow_name.Text = workFlowItem.WorkflowTitleEN;
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_workflow_name.Text = workFlowItem.WorkflowTitle;

                string assignedTo = "";
                string[] arr_assignedTo = null;
                List<string> lst_userName = new List<string>();
                if (!string.IsNullOrEmpty(workFlowItem.AssignedTo))
                {
                    if (workFlowItem.AssignedTo.Contains(','))
                    {
                        arr_assignedTo = workFlowItem.AssignedTo.Split(',');
                        for (int i = 0; i < arr_assignedTo.Length; i++)
                        {
                            List<BeanUser> lst_userResult = new List<BeanUser>();
                            string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                            lst_userResult = conn.Query<BeanUser>(query_user, arr_assignedTo[i].Trim().ToLower());

                            if (lst_userResult != null && lst_userResult.Count > 0)
                            {
                                lst_userName.Add(lst_userResult[0].FullName);
                                lst_user.Add(lst_userResult[0]);
                            }
                        }

                        if (lst_userName.Count > 1)
                        {
                            int num_remain = lst_userName.Count - 1;
                            assignedTo = lst_userName[0] + ", +" + num_remain.ToString();
                        }
                        else
                            assignedTo = lst_userName[0];

                        custom_UserList.SetValue(lst_user);
                    }
                    else
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        lst_userResult = conn.Query<BeanUser>(query_user, workFlowItem.AssignedTo.Trim().ToLower());

                        if (lst_userResult.Count > 0)
                        {
                            assignedTo = lst_userResult[0].FullName;
                            lst_user.Add(lst_userResult[0]);
                            custom_UserList.SetValue(lst_user);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(workFlowItem.WFImageURL))
                    checkFileLocalIsExist(workFlowItem.WFImageURL, iv_logo);
                else
                    iv_logo.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    lbl_title.Text = workFlowItem.Content;
                    //line 2
                    lbl_toUser.Text = "To: " + assignedTo;
                }
                else // if (CmmVariable.SysConfig.LangCode == "1066")
                {
                    lbl_title.Text = workFlowItem.Content;
                    //line 2
                    lbl_toUser.Text = "Đến: " + assignedTo;
                }

                if (!string.IsNullOrEmpty(workFlowItem.ActionStatus))
                {
                    lbl_status.Hidden = false;
                    lbl_status.Frame = new CGRect(90, 60, 50, 20);

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_status.Text = workFlowItem.ActionStatusEN + "  ";
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_status.Text = workFlowItem.ActionStatus + "  ";

                    lbl_status.Lines = 1;
                    lbl_status.SizeToFit();

                    if (workFlowItem.ActionStatusID == 10) // Đã phê duyệt
                        lbl_status.BackgroundColor = UIColor.FromRGB(229, 255, 228); // xanh la
                    else if (workFlowItem.ActionStatusID == -2 || workFlowItem.ActionStatusID == -1 || workFlowItem.ActionStatusID == 4) // Xoa, huy, tu choi
                        lbl_status.BackgroundColor = UIColor.FromRGB(255, 230, 230); // đỏ
                    else if (workFlowItem.ActionStatusID == 0) // Đang lưu
                        lbl_status.BackgroundColor = UIColor.FromRGB(243, 243, 243);// xam
                    else // con lai
                        lbl_status.BackgroundColor = UIColor.FromRGB(197, 221, 249); // xanh da troi
                }
                else
                {
                    lbl_status.Hidden = true;
                }

                if (workFlowItem.Created.HasValue)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_create_date.Text = workFlowItem.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_create_date.Text = workFlowItem.Created.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Custom_NewWorkFlowCell - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var padding = 40;
            var widthNoPadding = ContentView.Frame.Width - (padding * 2);
            var withLogo = widthNoPadding / 3.5;

            if ((styleCell % 2) == 0)
            {
                backgroundLogo.Frame = new CGRect(padding, 25, withLogo, 114);
                iv_logo.Frame = new CGRect(backgroundLogo.Frame.X + 25, backgroundLogo.Frame.Y + 20, backgroundLogo.Frame.Width - 50, backgroundLogo.Frame.Height - 40);
                lbl_workflow_name.Frame = new CGRect(padding, backgroundLogo.Frame.Bottom + 10, withLogo, 20);

                view_info.Frame = new CGRect(backgroundLogo.Frame.Right + 20, 25, widthNoPadding - (withLogo + 20), ContentView.Frame.Height - 50);
            }
            else
            {
                backgroundLogo.Frame = new CGRect((withLogo * 2.5) + padding, 25, withLogo, 114);
                iv_logo.Frame = new CGRect(backgroundLogo.Frame.X + 25, backgroundLogo.Frame.Y + 20, backgroundLogo.Frame.Width - 50, backgroundLogo.Frame.Height - 40);
                lbl_workflow_name.Frame = new CGRect((withLogo * 2.5) + padding, backgroundLogo.Frame.Bottom + 10, withLogo, 20);

                view_info.Frame = new CGRect(padding, 25, widthNoPadding - (withLogo + 20), ContentView.Frame.Height - 50);
            }

            lbl_title.Frame = new CGRect(0, 0, view_info.Frame.Width, 20);
            lbl_create_date.Frame = new CGRect(0, lbl_title.Frame.Bottom, view_info.Frame.Width, 20);
            var withUserList = 40 + (20 * (lst_user.Count - 1)) + 10;
            custom_UserList.InitializeFrameView(new CGRect(0, lbl_create_date.Frame.Bottom, withUserList, 40));
            //lbl_toUser.Frame = new CGRect(custom_UserList.Frame.Right + 10, lbl_create_date.Frame.Bottom, view_info.Frame.Width - (custom_UserList.Frame.Right + 10), 20);
            lbl_toUser.Frame = new CGRect(50, lbl_create_date.Frame.Bottom, view_info.Frame.Width - (custom_UserList.Frame.Right + 10), 20);
            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 14).Width + 20;
            var maxStatusWidth = view_info.Frame.Width - (custom_UserList.Frame.Right + 10);
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(50, lbl_toUser.Frame.Bottom + 5, widthStatus, 22);
            else
                lbl_status.Frame = new CGRect(50, lbl_toUser.Frame.Bottom + 5, maxStatusWidth, 22);

            lbl_bot_line.Frame = new CGRect(padding, ContentView.Frame.Bottom - 1, ContentView.Frame.Width - (padding * 2), 1);
        }

        private async void checkFileLocalIsExist(string url, UIImageView image_view)
        {
            try
            {
                string filename = url.Split('/').Last();
                string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + url;
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
                                        //avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                        image_view.Image = image;
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

                                image_view.Hidden = false;
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
                                image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    image_view.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
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