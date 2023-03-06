using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class CreateTicketRequest : UIViewController
    {
        CollectionTicket_Source collectionTicket_Source;
        List<BeanTickit> lst_titket;
        List<KeyValuePair<string, bool>> lst_sectionState;
        List<ClassMenu> lst_ClassMenus;
        Custom_MenuOption menu_cate = Custom_MenuOption.Instance;
        UIView maskView;

        public CreateTicketRequest(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            LoadContent();
            loadListCategory();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_filter.TouchUpInside += BT_filter_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            var flowLayout = new UICollectionViewFlowLayout()
            {
                ItemSize = new CGSize((float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f, (float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f),
                HeaderReferenceSize = new CGSize(Collection_ticket.Frame.Width, 30),
                //FooterReferenceSize = new CGSize(Collection_ticket.Frame.Width, 10),
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                MinimumInteritemSpacing = 2, // minimum spacing between cells
                MinimumLineSpacing = 2 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };
            Collection_ticket.SetCollectionViewLayout(flowLayout, true);
            Collection_ticket.RegisterClassForSupplementaryView(typeof(Custom_CollectionHeader), UICollectionElementKindSection.Header, Custom_CollectionHeader.Key);
            //Collection_ticket.RegisterClassForSupplementaryView(typeof(UIView), UICollectionElementKindSection.Footer, "");
            Collection_ticket.RegisterClassForCell(typeof(Ticket_CollectionCell), Ticket_CollectionCell.CellID);
            Collection_ticket.AllowsMultipleSelection = false;
            //Collection_ticket.Transform = CGAffineTransform.MakeScale(-1, 1);

            maskView = new UIView();
            maskView.Frame = new CGRect(0, header_view.Frame.Bottom, this.View.Frame.Width, this.View.Frame.Height - header_view.Frame.Height);
            maskView.BackgroundColor = UIColor.Black.ColorWithAlpha(0.2f);
            maskView.Alpha = 0;

            menu_cate.InitFrameView(new CGRect(this.View.Frame.Width - 180, header_view.Frame.Bottom + 5, 175, 0));
            menu_cate.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
            menu_cate.Layer.BorderWidth = 0.5f;
            menu_cate.ClipsToBounds = true;
            menu_cate.Layer.ShadowColor = UIColor.LightGray.CGColor;
            menu_cate.Layer.ShadowRadius = 3;
            menu_cate.Layer.ShadowOffset = new CGSize(2, 2);
            menu_cate.Layer.CornerRadius = 10;

            this.View.InsertSubview(maskView, 99);
            this.View.InsertSubview(menu_cate, 100);
        }

        private void LoadContent()
        {
            BeanTickit b1 = new BeanTickit { ID = "1", Title = "Đăng ký làm ngoài giờ", Type = "Nhân sự", Path = "IMG_temp/img_recruitment.png" };
            BeanTickit b2 = new BeanTickit { ID = "2", Title = "Thanh toán công tác phí", Type = "Nhân sự", Path = "IMG_temp/img_learning.png" };
            BeanTickit b3 = new BeanTickit { ID = "3", Title = "Đề nghị tuyển dụng", Type = "Nhân sự", Path = "IMG_temp/img_training.png" };
            BeanTickit b4 = new BeanTickit { ID = "4", Title = "Đề nghị thăng cấp", Type = "Nhân sự", Path = "IMG_temp/img_hr.png" };
            BeanTickit b5 = new BeanTickit { ID = "5", Title = "Quy trình đào tạo", Type = "Nhân sự", Path = "IMG_temp/img_learning.png" };
            BeanTickit b6 = new BeanTickit { ID = "6", Title = "Quản cáo thương hiệu", Type = "Marketing", Path = "IMG_temp/img_training.png" };
            BeanTickit b7 = new BeanTickit { ID = "7", Title = "Nhận dạng sản phẩm", Type = "Marketing", Path = "IMG_temp/img_recruitment.png" };
            BeanTickit b8 = new BeanTickit { ID = "8", Title = "Thuê Showroom", Type = "Kinh doanh", Path = "IMG_temp/img_hr.png", IsExpand = true };
            BeanTickit b9 = new BeanTickit { ID = "9", Title = "Mở rộng thị trường", Type = "Kinh doanh", Path = "IMG_temp/img_recruitment.png" };
            BeanTickit b10 = new BeanTickit { ID = "10", Title = "Dịch vụ xã hội", Type = "Kinh doanh", Path = "IMG_temp/img_learning.png" };

            lst_titket = new List<BeanTickit>();
            lst_titket.Add(b1); lst_titket.Add(b2); lst_titket.Add(b3); lst_titket.Add(b4);
            lst_titket.Add(b5); lst_titket.Add(b6); lst_titket.Add(b7); lst_titket.Add(b8); lst_titket.Add(b9); lst_titket.Add(b10);

            collectionTicket_Source = new CollectionTicket_Source(this, lst_titket, null);
            //collectionTicket_Source.Rows.AddRange(lst_titket);

            Collection_ticket.Source = collectionTicket_Source;
        }

        private void loadListCategory()
        {
            ClassMenu m1 = new ClassMenu() { ID = 0, section = 0, title = "Tất cả", isSelected = true };
            ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Chung" };
            ClassMenu m3 = new ClassMenu() { ID = 2, section = 0, title = "Tài chính" };
            ClassMenu m4 = new ClassMenu() { ID = 3, section = 0, title = "Pháp chế" };
            ClassMenu m5 = new ClassMenu() { ID = 4, section = 0, title = "Cung ứng" };
            ClassMenu m6 = new ClassMenu() { ID = 5, section = 0, title = "Quản lý đội xe" };
            ClassMenu m7 = new ClassMenu() { ID = 6, section = 0, title = "Công trình" };
            ClassMenu m8 = new ClassMenu() { ID = 7, section = 0, title = "Xây dựng" };


            lst_ClassMenus = new List<ClassMenu>();
            lst_ClassMenus.AddRange(new[] { m1, m2, m3, m4, m5, m6, m7, m8 });

            menu_cate.lst_menu = lst_ClassMenus;
            menu_cate.TableLoadData();
        }

        public void UpdateColletionView(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            collectionTicket_Source = new CollectionTicket_Source(this, lst_titket, lst_sectionState);
            Collection_ticket.ReloadSections(NSIndexSet.FromIndex(index));
        }

        private void toggleMenu()
        {
            if (maskView.Alpha == 0)
            {
                menu_cate.Frame = new CGRect(menu_cate.Frame.X, menu_cate.Frame.Y, menu_cate.Frame.Width, 0);
                maskView.Alpha = 0;
                UIView.BeginAnimations("toogle_docmenu_slideShow_show");
                UIView.SetAnimationDuration(0.4f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                menu_cate.InitFrameView(new CGRect(menu_cate.Frame.X, menu_cate.Frame.Y, menu_cate.Frame.Width, 400));
                maskView.Alpha = 1;
                UIView.CommitAnimations();
            }
            else
            {
                menu_cate.Frame = new CGRect(menu_cate.Frame.X, menu_cate.Frame.Y, menu_cate.Frame.Width, 400);
                maskView.Alpha = 1;
                UIView.BeginAnimations("toogle_docmenu_slideShow_collapse");
                UIView.SetAnimationDuration(0.4f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                menu_cate.InitFrameView(new CGRect(menu_cate.Frame.X, menu_cate.Frame.Y, menu_cate.Frame.Width, 0));
                maskView.Alpha = 0;

                UIView.CommitAnimations();
            }
        }

        private void NavigateToViewByCate(BeanTickit cate, int index)
        {
            CreateTicketFormView formView = Storyboard.InstantiateViewController("CreateTicketFormView") as CreateTicketFormView;
            formView.SetContent(index);
            this.NavigationController.PushViewController(formView, true);
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void BT_filter_TouchUpInside(object sender, EventArgs e)
        {
            toggleMenu();
        }
        #endregion

        #region cusrom class

        #region collection ticket categories
        public class CollectionTicket_Source : UICollectionViewSource
        {
            CreateTicketRequest parentView { get; set; }
            public static Dictionary<string, List<BeanTickit>> indexedSession;
            List<string> sectionKeys;
            //bool => Colapse
            List<KeyValuePair<string, bool>> lst_sectionState;
            public List<BeanTickit> items;
            public CollectionTicket_Source(CreateTicketRequest _parentview, List<BeanTickit> _items, List<KeyValuePair<string, bool>> _sectionState)
            {
                parentView = _parentview;
                lst_sectionState = _sectionState;
                items = _items;
                LoadData();
            }

            public void LoadData()
            {
                indexedSession = new Dictionary<string, List<BeanTickit>>();

                if (lst_sectionState == null)
                {
                    lst_sectionState = new List<KeyValuePair<string, bool>>();

                    sectionKeys = items.Select(x => x.Type).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        List<BeanTickit> lst_item = items.Where(x => x.Type == section).ToList();
                        indexedSession.Add(section, lst_item);

                        KeyValuePair<string, bool> keypair_section;
                        keypair_section = new KeyValuePair<string, bool>(section, false);
                        //if (lst_item[0].IsExpand)
                        //    keypair_section = new KeyValuePair<string, bool>(section, false);
                        //else
                        //    keypair_section = new KeyValuePair<string, bool>(section, true);

                        //keypair_section = new KeyValuePair<string, bool>(section, true);
                        lst_sectionState.Add(keypair_section);
                    }
                    parentView.lst_sectionState = lst_sectionState;
                }
                else
                {
                    sectionKeys = items.Select(x => x.Type).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        List<BeanTickit> lst_item = items.Where(x => x.Type == section).ToList();
                        indexedSession.Add(section, lst_item);
                    }
                }
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return sectionKeys.Count;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                foreach (var i in lst_sectionState)
                {
                    if (i.Key == sectionKeys[(int)section] && i.Value == false)
                        return indexedSession[sectionKeys[(int)section]].Count;
                    else if (i.Key == sectionKeys[(int)section] && i.Value == true)
                        return 0;
                }
                //for (int i = 0; i < sectionKeys.Count; i++)
                //{
                //    if (section == i)
                //    {
                //        if (sectionState == true)
                //            return indexedSession[sectionKeys[i]].Count;
                //        else
                //            return 0;
                //    }
                //}
                return 0;
            }
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
            {
                if (elementKind == "UICollectionElementKindSectionHeader")
                {
                    Custom_CollectionHeader headerView = collectionView.DequeueReusableSupplementaryView(elementKind, Custom_CollectionHeader.Key, indexPath) as Custom_CollectionHeader;
                    headerView.createTicketView = parentView;
                    headerView.UpdateRow(lst_sectionState[indexPath.Section]);
                    return headerView;
                }
                else if (elementKind == "UICollectionElementKindSectionFooter")
                {
                    return null;
                }
                else
                    return null;
            }
            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                parentView.NavigateToViewByCate(items[indexPath.Row], indexPath.Row);
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                KeyValuePair<string, List<BeanTickit>> lst_itemInSection = indexedSession.Where(x => x.Key == sectionKeys[indexPath.Section]).First();
                BeanTickit tickit = lst_itemInSection.Value[indexPath.Row];
                var cell = (Ticket_CollectionCell)collectionView.DequeueReusableCell(Ticket_CollectionCell.CellID, indexPath);
                cell.UpdateRow(tickit);
                return cell;
            }
        }
        private class Ticket_CollectionCell : UICollectionViewCell
        {
            public static NSString CellID = new NSString("packageCellID");
            UIView bg_view = new UIView();
            UIImageView img;
            UILabel lbl_title;

            [Export("initWithFrame:")]
            public Ticket_CollectionCell(RectangleF frame) : base(frame)
            {
                ViewConfiguration();
            }
            private void ViewConfiguration()
            {
                //ContentView.Transform = CGAffineTransform.MakeScale(-1, 1);
                //ContentView.BackgroundColor = UIColor.Green;
                bg_view = new UIView();
                bg_view.BackgroundColor = UIColor.White;
                bg_view.Layer.BorderColor = UIColor.Red.ColorWithAlpha(0.3f).CGColor;
                bg_view.Layer.BorderWidth = 0.2f;
                bg_view.Layer.ShadowOffset = new CGSize(1, 1);
                bg_view.Layer.ShadowRadius = 1;
                bg_view.Layer.ShadowOpacity = 0.3f;
                bg_view.Layer.CornerRadius = 10;

                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(11, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Center,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 2
                };

                img = new UIImageView();
                img.ContentMode = UIViewContentMode.ScaleAspectFit;
                img.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
                img.Layer.BorderWidth = 0.2f;
                img.Layer.ShadowOffset = new CGSize(1, 1);
                img.Layer.ShadowRadius = 1;
                img.Layer.ShadowOpacity = 0.3f;
                img.Layer.CornerRadius = 10;

                //bg_view.AddSubviews(new UIView[] { lbl_title, img });
                //ContentView.AddSubview(bg_view);
                ContentView.AddSubviews(img, lbl_title);
            }
            public void UpdateRow(BeanTickit element)
            {
                lbl_title.Text = element.Title;
                img.Image = UIImage.FromFile(element.Path);

            }
            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                //bg_view.Frame = new CGRect(0, 0, ContentView.Frame.Width - 10, ContentView.Frame.Height);                
                img.Frame = new CGRect(15, 10, 100, 76);
                lbl_title.Frame = new CGRect(10, img.Frame.Bottom + 5, ContentView.Frame.Width - 20, 30);
            }
        }


        #endregion
        #endregion
    }
}

