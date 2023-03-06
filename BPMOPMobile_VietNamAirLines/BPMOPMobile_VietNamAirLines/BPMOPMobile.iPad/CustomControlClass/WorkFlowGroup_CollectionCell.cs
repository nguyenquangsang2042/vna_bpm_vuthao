using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class WorkFlowGroup_CollectionCell : UICollectionViewCell
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static NSString CellID = new NSString("packageCellID");
        UIView bg_view = new UIView();
        UIView topview = new UIView();
        UIImageView img;
        UILabel lbl_title, lbl_description;
        UIImageView img_favorite;
        UIViewController controller { get; set; }
        UIButton BT_favorite;
        UIActivityIndicatorView favoriteLoading;
        BeanWorkflow workflowSelected;
        bool isShowIconFollow;

        [Export("initWithFrame:")]
        public WorkFlowGroup_CollectionCell(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }
        private void ViewConfiguration()
        {
            //ContentView.Transform = CGAffineTransform.MakeScale(-1, 1);
            //ContentView.BackgroundColor = UIColor.Purple;

            bg_view = new UIView();
            bg_view.BackgroundColor = UIColor.White;
            bg_view.Layer.ShadowOffset = new CGSize(1, 2);
            bg_view.Layer.ShadowRadius = 3;
            bg_view.Layer.ShadowColor = UIColor.FromRGB(51, 95, 179).CGColor;
            bg_view.Layer.ShadowOpacity = 0.4f;
            bg_view.Layer.CornerRadius = 5;

            topview = new UIView();
            topview.BackgroundColor = UIColor.FromRGB(251, 251, 251);

            img_favorite = new UIImageView();
            img_favorite.ContentMode = UIViewContentMode.ScaleAspectFit;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 15f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            lbl_description = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 13f),
                TextColor = UIColor.FromRGB(94, 94, 94),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            img = new UIImageView();
            img.ContentMode = UIViewContentMode.ScaleAspectFit;
            //img.Layer.ShadowOffset = new CGSize(1, 1);
            //img.Layer.ShadowRadius = 1;
            //img.Layer.ShadowOpacity = 0.3f;

            topview.Add(img);


            favoriteLoading = new UIActivityIndicatorView();
            favoriteLoading.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Medium;
            favoriteLoading.TintColor = UIColor.FromRGB(65, 80, 134);
            favoriteLoading.HidesWhenStopped = true;

            BT_favorite = new UIButton();
            BT_favorite.TouchUpInside += BT_favorite_TouchUpInside;



            bg_view.AddSubviews(new UIView[] { topview, lbl_title, lbl_description, img_favorite, BT_favorite, favoriteLoading });
            ContentView.AddSubviews(bg_view);
        }

        private async void BT_favorite_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                img_favorite.Hidden = true;
                favoriteLoading.StartAnimating();
                await Task.Run(() =>
                {
                    if (p_dynamic.SetFavoriteWorkflow(workflowSelected.WorkflowID, !workflowSelected.Favorite))
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1f));
                        ProviderBase p_base = new ProviderBase();
                        p_base.UpdateMasterData<BeanWorkflow>(null, true, CmmVariable.SysConfig.DataLimitDay, false);
                        InvokeOnMainThread(() =>
                        {
                            if (controller.GetType() == typeof(BroadView))
                            {
                                BroadView broadView = controller as BroadView;
                                //if (broadView.currentWorkFlowCateSelected != null)
                                //    broadView.loadContentByCateID(broadView.currentWorkFlowCateSelected.ID);
                                //else
                                broadView.LoadContent();
                            }
                            favoriteLoading.StopAnimating();
                            img_favorite.Hidden = false;
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            CmmIOSFunction.commonAlertMessage(controller, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                            favoriteLoading.StopAnimating();
                            img_favorite.Hidden = false;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                favoriteLoading.StopAnimating();
                Console.WriteLine("WorkFlowGroup_collectionCell - BT_favorite_TouchUpInside - ERR: " + ex.ToString());
            }

        }

        private void BT_List_TouchUpInside(object sender, EventArgs e)
        {
            if (controller.GetType() == typeof(BroadView))
            {
                BroadView broadView = controller as BroadView;
                broadView.NavigateToKanBan(workflowSelected, 0);
            }
        }

        private void BT_Board_TouchUpInside(object sender, EventArgs e)
        {
            if (controller.GetType() == typeof(BroadView))
            {
                BroadView broadView = controller as BroadView;
                broadView.NavigateToKanBan(workflowSelected, 1);
            }
        }

        public void UpdateRow(BeanWorkflow element, UIViewController _controller, bool _isShowIconFollow)
        {
            workflowSelected = element;
            controller = _controller;
            isShowIconFollow = _isShowIconFollow;

            if (CmmVariable.SysConfig.LangCode == "1033")
            {
                lbl_title.Text = element.TitleEN;
                //lbl_description.Text = @"Xamarin.iOS allows developers to create native iOS applications using the same UI controls that are available
                //                    in Objective-C and Xcode, except with the flexibility and elegance of a modern language (C#), the power of the .NET Base Class Library (BCL),
                //                    and two first-class IDEs - Visual Studio for Mac and Visual Studio. This series introduces how to setup
                //                    and install Xamarin.iOS and addresses the basics of Xamarin.iOS development.
                //                    ";
            }
            else //if (CmmVariable.SysConfig.LangCode == "1066")
            {
                lbl_title.Text = element.Title;
                //lbl_description.Text = "NASA khuấy đảo Hành tinh Đỏ: Không chỉ tạo ra 5,4 gram oxy quý hiếm, trực thăng sao Hỏa Ingenuity còn bay cao kỷ lục trong lần thứ ba!";
            }

            lbl_description.Text = "";

            if (!string.IsNullOrEmpty(element.ImageURL))
                checkFileLocalIsExist(element.ImageURL, img);
            else
                img.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

            if (isShowIconFollow)
            {
                img_favorite.Hidden = false;
                if (element.Favorite)
                    img_favorite.Image = UIImage.FromFile("Icons/icon_favorite_on.png");
                else
                    img_favorite.Image = UIImage.FromFile("Icons/icon_favorite_off.png");
            }
            else
                img_favorite.Hidden = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            bg_view.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height);
            topview.Frame = new CGRect(0, 0, bg_view.Frame.Width, bg_view.Frame.Height / 2.1f);
            img.Frame = new CGRect((topview.Frame.Width - 70) / 2, (topview.Frame.Height - 70) / 2, 70, 70);
            lbl_title.Frame = new CGRect(20, topview.Frame.Bottom + 5, topview.Frame.Width - 40, 40);
            lbl_description.Frame = new CGRect(20, lbl_title.Frame.Bottom, lbl_title.Frame.Width, 60);
            img_favorite.Frame = new CGRect(ContentView.Frame.Width - 40, 20, 14, 14);
            BT_favorite.Frame = new CGRect(ContentView.Frame.Width - 45, 12, 30, 30);
            favoriteLoading.Frame = new CGRect(ContentView.Frame.Width - 43, 15, 20, 20);
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
