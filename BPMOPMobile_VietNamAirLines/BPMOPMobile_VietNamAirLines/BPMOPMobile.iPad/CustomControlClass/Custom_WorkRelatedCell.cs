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
using BPMOPMobile.iPad.Components;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_WorkRelatedCell: UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UILabel lbl_imgCover, lbl_title, lbl_code, lbl_date, lbl_status, lbl_leftLine;
        UIButton btn_action;
        BeanWorkFlowRelated beanWorkFlowRelate;
        string currentWorkFlowID;
        bool itemSelected;
        bool isOdd;
        UIImageView iv_avatar;
        UIViewController parentView { get; set; }
        ControlBase controlBase { get; set; }

        public Custom_WorkRelatedCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;

        }

        public void UpdateCell(BeanWorkFlowRelated _beanWorkFlow, bool _selected, bool _isOdd, string _currentWorkFlowID)
        {
            beanWorkFlowRelate = _beanWorkFlow;
            isOdd = _isOdd;
            currentWorkFlowID = _currentWorkFlowID;
            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            if (isOdd)
                ContentView.BackgroundColor = UIColor.White;
            else
                ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

            lbl_imgCover = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.White
            };
            lbl_imgCover.Layer.CornerRadius = 20;
            lbl_imgCover.ClipsToBounds = true;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 14f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
            };

            lbl_date = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 12f),
                TextColor = UIColor.FromRGB(94,94,94),
                TextAlignment = UITextAlignment.Right,
            };

            lbl_code = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 13f),
                TextColor = UIColor.FromRGB(94, 94, 94),
            };

            lbl_status = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
            };

            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            iv_avatar.ClipsToBounds = true;
            iv_avatar.Layer.CornerRadius = 20;
            iv_avatar.Hidden = true;

            lbl_status.ClipsToBounds = true;
            lbl_status.TextAlignment = UITextAlignment.Center;
            lbl_status.Layer.CornerRadius = 3;

            ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_date, lbl_code, lbl_status });
        }

        private void LoadData()
        {
            try
            {
                if (!string.IsNullOrEmpty(beanWorkFlowRelate.CreatedBy))
                {
                    List<BeanUser> lst_userResult = new List<BeanUser>();
                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query_user = string.Format("SELECT * FROM BeanUser WHERE FullName =?");
                    lst_userResult = conn.Query<BeanUser>(query_user, beanWorkFlowRelate.CreatedBy);

                    string user_imagePath = "";
                    if (lst_userResult.Count > 0)
                        user_imagePath = lst_userResult[0].ImagePath;

                    if (string.IsNullOrEmpty(user_imagePath))
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(beanWorkFlowRelate.CreatedBy);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    }

                    else
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(beanWorkFlowRelate.CreatedBy);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                        checkFileLocalIsExist(lst_userResult[0], iv_avatar);
                    }
                }

                if (currentWorkFlowID == beanWorkFlowRelate.ItemRLID.ToString())
                {
                    lbl_title.Text = beanWorkFlowRelate.WorkflowContent;
                    lbl_code.Text = beanWorkFlowRelate.ItemCode;
                    if (beanWorkFlowRelate.Created.HasValue)
                        lbl_date.Text = beanWorkFlowRelate.Created.Value.ToString("dd/MM/yy HH:mm");
                }
                else if (currentWorkFlowID == beanWorkFlowRelate.ItemID.ToString())
                {
                    lbl_title.Text = beanWorkFlowRelate.WorkflowContentRL;
                    lbl_code.Text = beanWorkFlowRelate.RelatedCode;
                    if (beanWorkFlowRelate.CreatedRL.HasValue)
                        lbl_date.Text = beanWorkFlowRelate.CreatedRL.Value.ToString("dd/MM/yy HH:mm");
                }
                lbl_status.Frame = new CGRect(90, 60, 50, 20);

                if (!string.IsNullOrEmpty(beanWorkFlowRelate.StatusWorkflowRL))
                {
                    lbl_status.Hidden = false;
                    lbl_status.Frame = new CGRect(90, 60, 50, 20);
                    lbl_status.Text = beanWorkFlowRelate.StatusWorkflowRL + "  ";
                    lbl_status.Lines = 1;
                    lbl_status.SizeToFit();

                    if (beanWorkFlowRelate.StatusWorkflowRLID == 10) // Đã phê duyệt - xanh la - 220,255,218
                        lbl_status.BackgroundColor = UIColor.FromRGB(229, 255, 228);
                    else if (beanWorkFlowRelate.StatusWorkflowRLID == -1 || beanWorkFlowRelate.StatusWorkflowRLID == 6) // huy, tu choi
                        lbl_status.BackgroundColor = UIColor.FromRGB(255, 203, 203); // đỏ
                    else if (beanWorkFlowRelate.StatusWorkflowRLID == 0) // Đang lưu
                        lbl_status.BackgroundColor = UIColor.FromRGB(243, 243, 243);// xam
                    else if (beanWorkFlowRelate.StatusWorkflowRLID == 1)
                        lbl_status.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi
                    else
                        lbl_status.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi
                }
                else
                {
                    lbl_status.Hidden = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
            }
        }

        private void HandleBtnDelete(object sender, EventArgs e)
        {
            if (parentView != null)
            {
            }
            else if (controlBase != null)
            {
                if (controlBase.GetType() == typeof(ControlInputWorkRelated))
                {
                    ControlInputWorkRelated controlInputWorkRelated = (ControlInputWorkRelated)controlBase;
                    //controlInputWorkRelated.HandleRemoveItem(currentNotify);
                }
            }
        }

        public void LoadData_bk()
        {
            //lbl_title.Text = currentNotify.Title;

            //if (!string.IsNullOrEmpty(currentNotify.AssignedBy))
            //{
            //    lbl_imgCover.Hidden = false;
            //    lbl_imgCover.Text = currentNotify.AssignedBy.Substring(0, 1).ToUpper();
            //}

            //if (!string.IsNullOrEmpty(currentNotify.EmailUpdate))
            //{
            //    lbl_imgCover.Hidden = false;
            //    lbl_imgCover.Text = currentNotify.EmailUpdate.Substring(0, 1).ToUpper();
            //}

            //lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

            //CultureInfo culture = new CultureInfo("en-US");
            //if (currentNotify.Created.HasValue)
            //    lbl_date.Text = currentNotify.Created.Value.ToString("dd/MM/yy HH:mm");

            //lbl_code.Text = currentNotify.RequestID;

            //lbl_status.Text = currentNotify.Note;
            //switch (currentNotify.Status)
            //{
            //    case 0:// "chờ phê duyệt"
            //        lbl_status.BackgroundColor = UIColor.FromRGB(197, 221, 249);
            //        lbl_status.TextColor = UIColor.FromRGB(51, 95, 179);
            //        break;
            //    case 1:// "đã phê duyệt"
            //        lbl_status.BackgroundColor = UIColor.FromRGB(119, 224, 117);
            //        lbl_status.TextColor = UIColor.White;
            //        break;
            //    case 2:// "bổ sung thông tin"
            //        lbl_status.BackgroundColor = UIColor.FromRGB(255, 160, 114);
            //        lbl_status.TextColor = UIColor.White;
            //        break;
            //    case 3:// "từ chối"
            //        lbl_status.BackgroundColor = UIColor.Red;
            //        lbl_status.TextColor = UIColor.White;
            //        break;
            //    case 4:// "hủy"
            //        lbl_status.BackgroundColor = UIColor.FromRGB(216, 216, 216);
            //        lbl_status.TextColor = UIColor.White;
            //        break;
            //    default:
            //        break;
            //}

            //if (itemSelected)
            //{
            //    btn_action.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            //    btn_action.SetImage(UIImage.FromFile("Icons/icon_close_circle_red.png"), UIControlState.Normal);
            //    btn_action.Hidden = false;
            //    lbl_leftLine.Hidden = true;
            //    if (isOdd)
            //        ContentView.BackgroundColor = UIColor.White;
            //    else
            //        ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            //}
            //else
            //{
            //    if (currentNotify.IsSelected)
            //    {
            //        btn_action.ContentEdgeInsets = new UIEdgeInsets(8, 7, 8, 7);
            //        ContentView.BackgroundColor = UIColor.FromRGB(246, 249, 255);
            //        btn_action.Hidden = false;
            //        lbl_leftLine.Hidden = false;
            //    }
            //    else
            //    {
            //        btn_action.Hidden = true;
            //        lbl_leftLine.Hidden = true;
            //        if (isOdd)
            //            ContentView.BackgroundColor = UIColor.White;
            //        else
            //            ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            //    }
            //}
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            lbl_imgCover.Frame = new CGRect(10, 20, 40, 40);
            iv_avatar.Frame = new CGRect(10, 20, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 15, 15, ContentView.Frame.Width - 170, 25);
            lbl_date.Frame = new CGRect(ContentView.Frame.Width - 115, 15, 110, 25);
            lbl_code.Frame = new CGRect(lbl_imgCover.Frame.Right + 15, lbl_title.Frame.Bottom + 5, 270, 25);
            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 12).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - 180;
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(ContentView.Frame.Width - (widthStatus + 5), lbl_date.Frame.Bottom + 5, widthStatus, 25);
            else
                lbl_status.Frame = new CGRect(ContentView.Frame.Width - (widthStatus + 5), lbl_date.Frame.Bottom + 5, maxStatusWidth, 25);
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

        //public override void LayoutSubviews()
        //{
        //    base.LayoutSubviews();

        //    lbl_leftLine.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);
        //    lbl_imgCover.Frame = new CGRect(25, 18, 40, 40);
        //    lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 20, 18, ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 20 + 75), 20);
        //    lbl_code.Frame = new CGRect(lbl_imgCover.Frame.Right + 20, lbl_title.Frame.Bottom, lbl_title.Frame.Width / 3, 20);
        //    lbl_date.Frame = new CGRect(lbl_code.Frame.Right, lbl_title.Frame.Bottom, lbl_title.Frame.Width / 3, 20);

        //    btn_action.Frame = new CGRect(ContentView.Frame.Width - 55, 11, 30, 30);

        //    lbl_imgCover.Layer.CornerRadius = lbl_imgCover.Frame.Width / 2;
        //    lbl_imgCover.ClipsToBounds = true;

        //    var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 12).Width + 20;
        //    var maxStatusWidth = lbl_title.Frame.Width / 3;
        //    if (widthStatus < maxStatusWidth)
        //        lbl_status.Frame = new CGRect(ContentView.Frame.Width - (95 + widthStatus), lbl_title.Frame.Bottom, widthStatus, 20);
        //    else
        //        lbl_status.Frame = new CGRect(lbl_date.Frame.Right, lbl_title.Frame.Bottom, maxStatusWidth, 20);
        //}
    }
}