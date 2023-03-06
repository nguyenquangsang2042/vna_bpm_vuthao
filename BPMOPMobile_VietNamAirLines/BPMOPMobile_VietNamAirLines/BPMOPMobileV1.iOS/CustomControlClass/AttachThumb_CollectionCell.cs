
using System;
using System.Drawing;
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
    public class AttachThumb_CollectionCell : UICollectionViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static NSString CellID = new NSString("attachThumbCellID");
        ControlInputComments controller { get; set; }
        FormCommentView formCommentView { get; set; }
        UIImageView img;
        UIButton BT_delete;
        UIActivityIndicatorView indicatorloading;
        BeanAttachFile attachThumb;


        [Export("initWithFrame:")]
        public AttachThumb_CollectionCell(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }
        private void ViewConfiguration()
        {
            //ContentView.Transform = CGAffineTransform.MakeScale(-1, 1);
            //ContentView.BackgroundColor = UIColor.Purple;
            indicatorloading = new UIActivityIndicatorView();

            img = new UIImageView();
            img.ContentMode = UIViewContentMode.ScaleAspectFill;
            img.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
            img.Layer.BorderWidth = 0.2f;
            img.Layer.ShadowOffset = new CGSize(1, 1);
            img.Layer.ShadowRadius = 1;
            img.Layer.ShadowOpacity = 0.3f;
            //img.Layer.CornerRadius = 10;

            BT_delete = new UIButton();
            BT_delete.SetImage(UIImage.FromFile("Icons/icon_close_circle_red.png"), UIControlState.Normal);
            BT_delete.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_delete.TouchUpInside += BT_delete_TouchUpInside; ;

            ContentView.AddSubviews(img, BT_delete, indicatorloading);
        }

        private void BT_delete_TouchUpInside(object sender, EventArgs e)
        {
            //thuyngo waiting update
            if(controller != null)
                controller.HandleRemoveThumbItem(attachThumb);
            if (formCommentView != null)
                formCommentView.HandleRemoveThumbItem(attachThumb);
        }

        public void UpdateRow(BeanAttachFile _attachThumb, ControlInputComments _controller)
        {
            attachThumb = _attachThumb;
            controller = _controller;
            if (attachThumb.ID == "")
            {
                if (!string.IsNullOrEmpty(attachThumb.Path))
                    openFileFromAddNew(attachThumb.Path);
                else
                    img.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
            }
        }

        public void UpdateRowReply(BeanAttachFile _attachThumb, FormCommentView _formCommentView)
        {
            attachThumb = _attachThumb;
            formCommentView = _formCommentView;
            if (attachThumb.ID == "")
            {
                if (!string.IsNullOrEmpty(attachThumb.Path))
                    openFileFromAddNew(attachThumb.Path);
                else
                    img.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            img.Frame = new CGRect(0, 5, 90, 50);
            BT_delete.Frame = new CGRect(img.Frame.Right, 0, 25, 25);
        }

        private void openFileFromAddNew(string fileAddNew)
        {
            Uri fileUri = new Uri(fileAddNew);
            img.Image = UIImage.FromFile(fileAddNew);

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
                        image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                    }
                }
                else
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }
    }
}

