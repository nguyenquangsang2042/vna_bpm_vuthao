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
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_WorkRelatedCell : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UILabel lbl_imgCover, lbl_status, lbl_title, lbl_date, lbl_subTitle;
        private bool isOdd;
        private UIImageView iv_avatar;
        BeanWorkFlowRelated beanWorkFlowRelate;
        string currentWorkFlowID;

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
                ContentView.BackgroundColor = UIColor.FromRGB(243, 249, 255);

            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            iv_avatar.ClipsToBounds = true;
            iv_avatar.Layer.CornerRadius = 20;
            iv_avatar.Hidden = true;

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
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Medium),
                TextColor = UIColor.FromRGB(25, 25, 30),
                TextAlignment = UITextAlignment.Left,
            };

            lbl_date = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(94, 94, 94),
                TextAlignment = UITextAlignment.Right,
            };

            lbl_subTitle = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(94, 94, 94),
            };

            lbl_status = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Center,
            };

            lbl_status.ClipsToBounds = true;
            lbl_status.TextAlignment = UITextAlignment.Center;
            lbl_status.Layer.CornerRadius = 5;

            ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_date, lbl_subTitle, lbl_status });
        }



        private void LoadData()
        {
            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
            try
            {

                if (!string.IsNullOrEmpty(beanWorkFlowRelate.CreatedBy))
                {
                    List<BeanUser> lst_userResult = new List<BeanUser>();
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


                if (currentWorkFlowID == beanWorkFlowRelate.ItemRLID.ToString()) // bindding theo ItemID
                {
                    lbl_title.Text = beanWorkFlowRelate.WorkflowContent;
                    lbl_subTitle.Text = beanWorkFlowRelate.ItemCode;
                    if (beanWorkFlowRelate.Created.HasValue)
                        lbl_date.Text = beanWorkFlowRelate.Created.Value.ToString("dd/MM/yy HH:mm");


                    lbl_status.Frame = new CGRect(90, 60, 50, 20);

                    if (!string.IsNullOrEmpty(beanWorkFlowRelate.StatusWorkflow))
                    {
                        lbl_status.Hidden = false;
                        lbl_status.Frame = new CGRect(90, 60, 50, 20);
                        lbl_status.Lines = 1;
                        lbl_status.SizeToFit();

                        lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(beanWorkFlowRelate.StatusWorkflowID ?? 0);

                        string query = String.Format("SELECT * FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", beanWorkFlowRelate.StatusWorkflowID);
                        List<BeanAppStatus> _result = conn.Query<BeanAppStatus>(query);

                        if (_result != null && _result.Count > 0)
                            lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1033") ? _result[0].TitleEN : _result[0].Title;
                        else
                            lbl_status.Text = beanWorkFlowRelate.StatusWorkflow;
                    }
                    else
                    {
                        lbl_status.Hidden = true;
                    }
                }
                else if (currentWorkFlowID == beanWorkFlowRelate.ItemID.ToString())
                {
                    lbl_title.Text = beanWorkFlowRelate.WorkflowContentRL;
                    lbl_subTitle.Text = beanWorkFlowRelate.RelatedCode;
                    if (beanWorkFlowRelate.CreatedRL.HasValue)
                        lbl_date.Text = beanWorkFlowRelate.CreatedRL.Value.ToString("dd/MM/yy HH:mm");

                    lbl_status.Frame = new CGRect(90, 60, 50, 20);

                    if (!string.IsNullOrEmpty(beanWorkFlowRelate.StatusWorkflowRL))
                    {
                        lbl_status.Hidden = false;
                        lbl_status.Frame = new CGRect(90, 60, 50, 20);
                        lbl_status.Lines = 1;
                        lbl_status.SizeToFit();

                        lbl_status.BackgroundColor = CmmIOSFunction.GetColorByAppStatus(beanWorkFlowRelate.StatusWorkflowRLID ?? 0);

                        string query = String.Format("SELECT * FROM BeanAppStatus WHERE ID = {0} LIMIT 1 OFFSET 0", beanWorkFlowRelate.StatusWorkflowRLID);
                        List<BeanAppStatus> _result = conn.Query<BeanAppStatus>(query);

                        if (_result != null && _result.Count > 0)
                            lbl_status.Text = CmmVariable.SysConfig.LangCode.Equals("1033") ? _result[0].TitleEN : _result[0].Title;
                        else
                            lbl_status.Text = beanWorkFlowRelate.StatusWorkflowRL;
                    }
                    else
                    {
                        lbl_status.Hidden = true;
                    }
                }


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
            iv_avatar.Frame = new CGRect(5, 15, 40, 40);
            lbl_imgCover.Frame = new CGRect(5, 15, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - 60, 20);
            lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, 270, 20);
            lbl_date.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_title.Frame.Bottom, 110, 20);
            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 12).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - 180;
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
            else
                lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

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
