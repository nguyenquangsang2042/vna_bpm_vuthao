using System;
using System.Collections.Generic;
using System.Drawing;
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
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    class Custom_UserList: UIView
    {
        UICollectionView collection_user;

        public Custom_UserList()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout();
            flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            flowLayout.MinimumInteritemSpacing = 0;
            flowLayout.MinimumLineSpacing = 0;

            CGRect frame = CGRect.Empty;
            collection_user = new UICollectionView(frame: frame, layout: flowLayout);
            collection_user.RegisterClassForCell(typeof(CollectionCell), CollectionCell.CellID);
            collection_user.ShowsHorizontalScrollIndicator = false;
            collection_user.AllowsMultipleSelection = false;
            collection_user.BackgroundColor = UIColor.White;
            collection_user.ScrollEnabled = false;

            this.Add(collection_user);
        }

        public void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            collection_user.Frame = new CGRect(0, 0, frame.Width, frame.Height);
        }

        public void SetValue(List<BeanUser> _lst_user)
        {
            List<BeanUser> lst_user = new List<BeanUser>();
            lst_user.Add(_lst_user[0]);
            Collection_Source collectionCate_Source = new Collection_Source(this, lst_user);
            collection_user.Source = collectionCate_Source;
            collection_user.Delegate = new CustomFlowLayoutDelegate(this, collectionCate_Source);
            collection_user.ReloadData();
        }

        #region collection menu
        private class Collection_Source : UICollectionViewSource
        {
            Custom_UserList parentView { get; set; }
            public List<BeanUser> lst_user { get; private set; }

            public Collection_Source(Custom_UserList _parentview, List<BeanUser> _lst_user)
            {
                parentView = _parentview;
                lst_user = _lst_user;
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return lst_user.Count;
            }

            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                CollectionCell cell = (CollectionCell)collectionView.DequeueReusableCell(CollectionCell.CellID, indexPath);
                cell.UpdateRow(lst_user[indexPath.Row], parentView, indexPath);

                return cell;
            }
        }
        private class CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            UIImageView iv_avatar;
            UILabel lbl_imgCover;
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            NSIndexPath indexPath { get; set; }

            [Export("initWithFrame:")]
            public CollectionCell(RectangleF frame) : base(frame)
            {
                ContentView.BackgroundColor = UIColor.White;

                iv_avatar = new UIImageView();
                iv_avatar.Image = UIImage.FromFile("Icons/icon_profile.png");
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.Layer.BorderColor = UIColor.LightGray.CGColor;
                iv_avatar.Layer.BorderWidth = 1;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.White
                };
                lbl_imgCover.Layer.CornerRadius = 15;
                lbl_imgCover.ClipsToBounds = true;

                ContentView.AddSubviews(lbl_imgCover, iv_avatar);
            }

            public async void UpdateRow(BeanUser _user, Custom_UserList _parentView, NSIndexPath _indexPath)
            {
                try
                {
                    indexPath = _indexPath;

                    //string filepathURL = "https://png.pngtree.com/png-clipart/20200225/original/pngtree-happy-holi-colorful-splatter-color-splash-free-png-and-psd-png-image_5308640.jpg";
                    if (string.IsNullOrEmpty(_user.ImagePath))
                    {
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Hidden = false;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(_user.FullName);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    }
                    else
                    {
                        iv_avatar.Hidden = false;
                        lbl_imgCover.Hidden = true;

                        CheckFileLocalIsExist(_user, iv_avatar);

                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("ControlAttachment - CollectionCell - UpdateRow - Err: " + ex.ToString());
#endif
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var height = ContentView.Frame.Height;
                var positionX = -(height / 2) * indexPath.Row;

                lbl_imgCover.Frame = new CGRect(positionX, 0, height, height);

                lbl_imgCover.Layer.CornerRadius = height / 2;
                lbl_imgCover.ClipsToBounds = true;

                iv_avatar.Frame = new CGRect(positionX, 0, height, height);

                iv_avatar.Layer.CornerRadius = height / 2;
                iv_avatar.ClipsToBounds = true;
            }

            private async void CheckFileLocalIsExist(BeanUser contact, UIImageView image_view)
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
        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static Custom_UserList parentView;
            Collection_Source lst_cate;

            #region Constructors
            public CustomFlowLayoutDelegate(Custom_UserList _parent, Collection_Source _lst_cate)
            {
                lst_cate = _lst_cate;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                return new CGSize(collectionView.Bounds.Height, collectionView.Bounds.Height);
            }
            #endregion
        }

        

        #endregion
    }
}