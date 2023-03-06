
using System;
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
    public class Custom_attachFileThumb : UITableViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //ControlInputComments parentView { get; set; }
        ControlInputComments controlInputComments { get; set; }
        UIViewController parentView { get; set; }
        NSIndexPath currentIndexPath { get; set; }
        BeanAttachFile attachment { get; set; }
        UIImageView img_type;
        UILabel lbl_title;
        bool isThumb;
        UIActivityIndicatorView activi;
        UIView viewActive;

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

            viewActive = new UIView();
            viewActive.Frame = new CGRect(2, 2, 176, 96);

            img_type = new UIImageView();
            img_type.ContentMode = UIViewContentMode.ScaleAspectFill;
            

            activi = new UIActivityIndicatorView();
            activi.TranslatesAutoresizingMaskIntoConstraints = false;
            activi.TintColor = UIColor.Gray;
            activi.StartAnimating();
            activi.HidesWhenStopped = true;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 14f),
                TextColor = UIColor.FromRGB(51, 95, 179),
                TextAlignment = UITextAlignment.Left,
            };
            
            viewActive.AddSubview(activi);

            ContentView.AddSubviews(new UIView[] { img_type, lbl_title, viewActive });

            activi.CenterXAnchor.ConstraintEqualTo(viewActive.CenterXAnchor).Active = true;
            activi.CenterYAnchor.ConstraintEqualTo(viewActive.CenterYAnchor).Active = true;
            activi.WidthAnchor.ConstraintEqualTo(30).Active = true;
            activi.HeightAnchor.ConstraintEqualTo(30).Active = true;

        }

        public void UpdateCell()
        {
            try
            {
                string fileExt = string.Empty;
                if (!string.IsNullOrEmpty(attachment.Url))
                    fileExt = attachment.Url.Split('.').Last().ToLower();

                isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);

                if (isThumb)
                {
                    checkFileLocalIsExist(img_type, attachment);
                }
                else
                {
                    activi.StopAnimating();
                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_word.png");
                            break;
                        case "txt":
                            img_type.Image = UIImage.FromFile("Icons/icon_attachFile_txt.png");
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
                //img_type.Frame = new CGRect(0, 10, 180, 100);
                img_type.ClipsToBounds = true;
                img_type.Layer.BorderColor = UIColor.FromRGB(94, 94, 94).CGColor;
                img_type.Layer.BorderWidth = 0.5f;
                img_type.Layer.CornerRadius = 10;
                img_type.ContentMode = UIViewContentMode.ScaleAspectFit;
            }
            else
            {
                img_type.Frame = new CGRect(0, 10, 20, 20);
                lbl_title.Frame = new CGRect(img_type.Frame.Right + 14, 10, ContentView.Frame.Width - 50, 20);
                activi.Hidden = true;
            }
        }

        //private async void checkFileLocalIsExist(UIImageView image_view, BeanAttachFile beanAttachFile)
        //{
        //    try
        //    {
        //        string filename = beanAttachFile.Title;
        //        string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + beanAttachFile.Url;
        //        string localfilePath = Path.Combine(localDocumentFilepath, filename);

        //        if (!File.Exists(localfilePath))
        //        {
        //            UIImage avatar = null;
        //            await Task.Run(() =>
        //            {
        //                ProviderBase provider = new ProviderBase();
        //                if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
        //                {
        //                    NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

        //                    InvokeOnMainThread(() =>
        //                    {
        //                        if (data != null)
        //                        {
        //                            UIImage image = UIImage.LoadFromData(data);
        //                            if (image != null)
        //                            {
        //                                avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
        //                                image_view.Image = avatar;
        //                            }
        //                            else
        //                                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //                        }
        //                        else
        //                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

        //                        image_view.Hidden = false;

        //                    });

        //                    if (data != null && avatar != null)
        //                    {
        //                        NSError err = null;
        //                        NSData imgData = avatar.AsPNG();
        //                        if (imgData.Save(localfilePath, false, out err))
        //                            Console.WriteLine("saved as " + localfilePath);


        //                        //if (image_view.Image.Size.Height > 100)
        //                        //    image_view.Image = CmmIOSFunction.ScaleImageFollowHeight(image_view.Image, 100);

        //                        //image_view.Frame = new CGRect(image_view.Frame.X, image_view.Frame.Y, image_view.Image.Size.Width, 100);

        //                        viewActive.Hidden = true;
        //                        activi.StopAnimating();
        //                        return;
        //                    }
        //                }
        //                else
        //                {
        //                    InvokeOnMainThread(() =>
        //                    {
        //                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //                        image_view.Hidden = false;
        //                    });
        //                }
        //            });
        //        }
        //        else
        //        {
        //            openFile(filename, image_view);
        //            image_view.Hidden = false;

        //        }
        //        activi.StopAnimating();
        //    }
        //    catch (Exception ex)
        //    {
        //        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //        Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
        //        //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
        //        activi.StopAnimating();
        //    }
        //}

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
                                        avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                        image_view.Image = avatar;
                                        openFile(filename, image_view);
                                        //image_view.Hidden = false;
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                activi.StopAnimating();
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
                                //image_view.Hidden = false;
                                activi.StopAnimating();
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    //image_view.Hidden = false;
                    activi.StopAnimating();
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                activi.StopAnimating();
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

                        if (image.Size.Height > 100)
                            image = CmmIOSFunction.ScaleImageFollowHeight(image, 100);

                        image_view.Frame = new CGRect(image_view.Frame.X, image_view.Frame.Y, image.Size.Width, 100);
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
