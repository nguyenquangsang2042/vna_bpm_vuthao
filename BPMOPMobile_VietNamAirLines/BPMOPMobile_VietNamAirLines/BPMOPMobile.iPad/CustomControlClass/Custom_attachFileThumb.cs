using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.Components;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_attachFileThumb : UITableViewCell
    {
    //
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        ControlInputComments controlInputComments { get; set; }
        UIViewController parentView { get; set; }
        NSIndexPath currentIndexPath { get; set; }
        UIActivityIndicatorView img_loading;
        BeanAttachFile attachment { get; set; }
        UIImageView img_type;
        UILabel lbl_title;
        bool isThumb;

        public Custom_attachFileThumb(ControlInputComments _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
        {
            controlInputComments = _parentView;
            attachment = _attachment;
            currentIndexPath = _currentIndexPath;
            Accessory = UITableViewCellAccessory.None;

            viewConfiguration();
            UpdateCell();
        }

        public Custom_attachFileThumb(UIViewController _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
        {
            parentView = _parentView;
            attachment = _attachment;
            currentIndexPath = _currentIndexPath;
            Accessory = UITableViewCellAccessory.None;

            viewConfiguration();
            UpdateCell();
        }

        private void viewConfiguration()
        {
            img_type = new UIImageView();
            img_type.ContentMode = UIViewContentMode.ScaleAspectFill;

            img_loading = new UIActivityIndicatorView();
            img_loading.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Medium;
            img_loading.Color = UIColor.FromRGB(0, 95, 212);
            img_loading.HidesWhenStopped = true;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 14f),
                TextColor = UIColor.FromRGB(59, 95, 179),
                TextAlignment = UITextAlignment.Left,
            };

            ContentView.AddSubviews(new UIView[] { img_type, img_loading, lbl_title });
        }
        
        public void UpdateCell()
        {
            try
            {
                string fileExt = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Url))
                        fileExt = attachment.Url.Split('.').Last();

                isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);

                if (isThumb)
                {
                    img_loading.StartAnimating();
                    checkFileLocalIsExist(img_type, attachment);
                }
                else
                {
                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_word.png");
                            break;
                        case "pdf":
                            img_type.Image = UIImage.FromFile("Icons/icon_pdf.png");
                            break;
                        case "xls":
                        case "xlsx":
                            img_type.Image = UIImage.FromFile("Icons/icon_xlsx.png");
                            break;
                        case "jpg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "png":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "jpeg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        default:
                            img_type.Image = UIImage.FromFile("Icons/icon_file_blank.png");
                            break;
                    }
                    lbl_title.Text = attachment.Title;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (isThumb)
            {
                img_type.ClipsToBounds = true;
                img_type.Layer.BorderColor = UIColor.FromRGB(94, 94, 94).CGColor;
                img_loading.Frame = new CGRect(100, 100, 30, 30);
                //img_type.Layer.BorderWidth = 0f;
                //img_type.Layer.CornerRadius = 10;
                //img_type.Hidden = true;

            }
            else
            {
                img_type.Frame = new CGRect(0, 10, 20,20);
                lbl_title.Frame = new CGRect(img_type.Frame.Right + 10, 5, ContentView.Frame.Width - 40, 30);
            }
        }

        private async void checkFileLocalIsExist(UIImageView image_view, BeanAttachFile beanAttachFile)
        {
            try
            {
                string filename = beanAttachFile.Title;
                string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + beanAttachFile.Url;
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
                                        if (image.Size.Height > 200)
                                            avatar = CmmIOSFunction.ScaleImageFollowHeight(image, 200);
                                        //avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                        image_view.Image = avatar;
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Images/inAppLogo.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Images/inAppLogo.png");

                                img_loading.StopAnimating();
                                image_view.Hidden = false;
                                image_view.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.6f).CGColor;
                                image_view.Layer.BorderWidth = 0.5f;
                                image_view.Layer.ShadowOffset = new CGSize(1, 2);
                                image_view.Layer.ShadowRadius = 3;
                                image_view.Layer.ShadowOpacity = 0.4f;
                                image_view.Layer.CornerRadius = 10;

                                if (controlInputComments != null)
                                {
                                    controlInputComments.table_comment.ReloadSections(new NSIndexSet((uint)currentIndexPath.Section), UITableViewRowAnimation.Fade);
                                }
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
                                image_view.Image = UIImage.FromFile("Images/inAppLogo.png");
                                image_view.Hidden = false;
                            });
                        }
                    });
                }
                else
                {
                    img_loading.StopAnimating();
                    openFile(filename, image_view);
                    image_view.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Images/inAppLogo.png");
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

                        if(image.Size.Height > 200)
                            image = CmmIOSFunction.ScaleImageFollowHeight(image, 200);

                        image_view.Frame = new CGRect(image_view.Frame.X, image_view.Frame.Y, image.Size.Width, 190);
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

                image_view.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.6f).CGColor;
                image_view.Layer.BorderWidth = 0.5f;
                image_view.Layer.ShadowOffset = new CGSize(1, 2);
                image_view.Layer.ShadowRadius = 3;
                image_view.Layer.ShadowOpacity = 0.4f;
                image_view.Layer.CornerRadius = 10;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }
    }

}
