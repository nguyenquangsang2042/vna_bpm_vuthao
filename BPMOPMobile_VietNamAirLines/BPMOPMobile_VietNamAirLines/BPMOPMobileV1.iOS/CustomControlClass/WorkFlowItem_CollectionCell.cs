//using System;
//namespace BPMOPMobileV1.iOS.CustomControlClass
//{
//    public class WorkFlowItem_CollectionCell
//    {
//        public WorkFlowItem_CollectionCell()
//        {
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.KanBanCustomClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class WorkFlowItem_CollectionCell : UICollectionViewCell
    {
        public static NSString CellID = new NSString("packageCellID");

        UILabel lbl_title;
        UIView view_base;
        UITableView table_stepItem;
        KeyValuePair<string, List<BeanAppBaseExt>> itemStep;
        ItemsInStep_TableSource itemsInStep_TableSource;
        KanBanView controller { get; set; }
        List<PhotoAlbum> albums = PhotoLibrary.SharedInstance.Albums;
        bool isOdd;


        [Export("initWithFrame:")]
        public WorkFlowItem_CollectionCell(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }

        private void ViewConfiguration()
        {
            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 16f),
                TextColor = UIColor.FromRGB(0, 95, 212),
                TextAlignment = UITextAlignment.Left,

            };

            view_base = new UIView();

            table_stepItem = new UITableView();
            table_stepItem.BackgroundColor = UIColor.Clear;
            table_stepItem.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_stepItem.ShowsHorizontalScrollIndicator = false;
            table_stepItem.ShowsVerticalScrollIndicator = false;
            //table_stepItem.TableHeaderView = new UIView();
            //table_stepItem.TableFooterView = new UIView();
            table_stepItem.ScrollEnabled = true;
            view_base.AddSubviews(lbl_title, table_stepItem);
            ContentView.AddSubviews(view_base);

            ContentView.BackgroundColor = UIColor.Clear;
            view_base.BackgroundColor = UIColor.FromRGB(245, 245, 245);
            view_base.Layer.CornerRadius = 10;
            view_base.ClipsToBounds = true;
        }

        public void UpdateRow(KeyValuePair<string, List<BeanAppBaseExt>> element, KanBanView _controller, bool _isOdd, NSIndexPath indexPath)
        {

            itemStep = element;
            controller = _controller;

            lbl_title.Text = element.Key;
            if (element.Value == null || element.Value.Count == 0)
            {
                table_stepItem.ScrollEnabled = false;
            }
            else
            {
                table_stepItem.ScrollEnabled = true;
            }

            itemsInStep_TableSource = new ItemsInStep_TableSource(element.Value, controller, this);
            table_stepItem.DragDelegate = new CustomKanbanTableView_Drag(controller, itemStep.Value, indexPath);
            table_stepItem.DropDelegate = new CustomKanbanTableView_Drop(controller, itemStep.Value, indexPath);
            table_stepItem.Source = itemsInStep_TableSource;
            table_stepItem.DragInteractionEnabled = true;

            if (itemStep.Value != null && itemStep.Value.Count > 0)
                view_base.Frame = new CGRect(0, 0, ContentView.Frame.Width, itemStep.Value.Count * 150 + 55);
            else
                view_base.Frame = new CGRect(0, 0, ContentView.Frame.Width, 150 + 55);
        }

        #region tableview
        void HandleActionTableItemDrag(BeanAppBaseExt beanWorkflowItem)
        {
            controller.workflowItemSelected = beanWorkflowItem;
        }

        /// <summary>
        /// Performs updates to the photo library backing store, then loads the latest album values from it.
        /// </summary>
        void UpdatePhotoLibrary(Action<PhotoLibrary> updates)
        {
            updates(PhotoLibrary.SharedInstance);
            ReloadAlbumsFromPhotoLibrary();
        }
        /// <summary>
        /// Loads the latest album values from the photo library backing store.
        /// </summary>
        public void ReloadAlbumsFromPhotoLibrary()
        {
            albums = PhotoLibrary.SharedInstance.Albums;
        }
        /// <summary>
        /// Updates the visible cells to display the latest values for albums.
        /// </summary>
        public void UpdateVisibleAlbumCells()
        {
            var visibleIndexPaths = table_stepItem.IndexPathsForVisibleRows;
            if (visibleIndexPaths is null) return;

            foreach (var indexPath in visibleIndexPaths)
            {
                var cell = table_stepItem.CellAt(indexPath) as ItemsInStep_Cell_Custom;
                if (cell != null)
                {
                    //cell.Configure(Album(indexPath));
                    cell.SetNeedsLayout();
                }
            }
        }

        void UpdateVisibleAlbumsAndPhotos()
        {
            UpdateVisibleAlbumCells();
        }
        #endregion

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (itemStep.Value != null && itemStep.Value.Count > 0)
                view_base.Frame = new CGRect(0, 0, ContentView.Frame.Width, itemStep.Value.Count * 150 + 55);
            else
                view_base.Frame = new CGRect(0, 0, ContentView.Frame.Width, 150 + 55);
            lbl_title.Frame = new CGRect(10, 10, ContentView.Frame.Width - 20, 30);
            table_stepItem.Frame = new CGRect(10, lbl_title.Frame.Bottom + 5, ContentView.Frame.Width - 20, ContentView.Frame.Height - (lbl_title.Frame.Bottom + 5));
        }

        #region custom class
        #region table data source user
        private class ItemsInStep_TableSource : UITableViewSource
        {
            List<BeanAppBaseExt> lst_menu;
            NSString cellIdentifier = new NSString("cellMenuOption");
            KanBanView parentView;
            WorkFlowItem_CollectionCell parent;

            public ItemsInStep_TableSource(List<BeanAppBaseExt> _menu, KanBanView _parentview, WorkFlowItem_CollectionCell _parent)
            {
                parentView = _parentview;
                lst_menu = _menu;
                parent = _parent;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_menu != null && lst_menu.Count > 0) ? lst_menu.Count : 1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 150;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.None;
            }

            public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
            {
                return false;
            }

            public override bool CanMoveRow(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                return false;
            }

            public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
            {
                //var itemSelected = lst_menu[destinationIndexPath.Row];
                //parent.HandleActionTableItemDrag(itemSelected);
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (lst_menu != null && lst_menu.Count > 0)
                {
                    var itemSelected = lst_menu[indexPath.Row];
                    parentView.NavigateToWorkFlowDetails(itemSelected);
                }
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                if (lst_menu != null && lst_menu.Count > 0)
                {
                    var workflowItem = lst_menu[indexPath.Row];
                    ItemsInStep_Cell_Custom cell = new ItemsInStep_Cell_Custom(parentView, cellIdentifier, workflowItem, indexPath);
                    cell.UpdateCell();
                    return cell;
                }
                else
                {
                    ItemsInStep_Cell_Custom cell = new ItemsInStep_Cell_Custom(parentView, cellIdentifier, null, indexPath);
                    return cell;
                }

            }
        }

        private class ItemsInStep_Cell_Custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            KanBanView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAppBaseExt workflowItem { get; set; }
            UIView bg_view;
            UILabel lbl_hanhoantat, lbl_content, lbl_name, lbl_nxlCount, lbl_position, lbl_attachCount, lbl_commentCount, lbl_noData;
            UIImageView iv_star, iv_nguoitao, iv_nguoixuly, iv_attachment, iv_comment;
            UIImageView photoImageView = new UIImageView();

            public readonly int?[] WaitingListID = { 1, 4 };                // AppStatusID Chờ phê duyệt (đang lưu - chờ xử lý - bổ sung thông tin - tham vấn - yêu cầu hiệu chỉnh)
            public readonly int?[] ApprovedListID = { 8 };                  // AppStatusID Phê duyệt
            public readonly int?[] RejectedListID = { 16, 64 };             // AppStatusID Từ chối (từ chối - hủy)


            public ItemsInStep_Cell_Custom(KanBanView _parentView, NSString cellID, BeanAppBaseExt _wkflowitem, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                workflowItem = _wkflowitem;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                if (workflowItem == null)
                    viewConfigurationNodata();
                else
                    viewConfiguration();
            }

            /// <summary>
            /// Returns a rect for the image view that displays the album thumbnail in the coordinate space of the cell, if it is visible.
            /// </summary>
            public CGRect? RectForAlbumThumbnail
            {
                get
                {
                    var imageView = this.ImageView;

                    if (imageView != null && imageView.Bounds.Size.Width > 0 && imageView.Bounds.Size.Height > 0 && imageView.Superview != null)
                    {
                        return this.ConvertRectToView(imageView.Bounds, imageView);
                    }
                    return null;
                }
            }

            private void viewConfigurationNodata()
            {
                ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);
                //bg_view = new UIView();
                //bg_view.Layer.CornerRadius = 5;
                //bg_view.ClipsToBounds = true;
                //bg_view.BackgroundColor = UIColor.White;
                lbl_noData = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    TextAlignment = UITextAlignment.Center,
                };

                lbl_noData.Text = CmmFunction.GetTitle("TEXT_NODATA", "Không có dữ liệu");
                //bg_view.Add(lbl_noData);
                //ContentView.Add(bg_view);
                ContentView.Add(lbl_noData);
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);

                bg_view = new UIView();
                bg_view.BackgroundColor = UIColor.White;
                bg_view.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.6f).CGColor;
                bg_view.Layer.BorderWidth = 0.5f;
                //bg_view.Layer.ShadowOffset = new CGSize(1, 2);
                //bg_view.Layer.ShadowRadius = 3;
                //bg_view.Layer.ShadowOpacity = 0.4f;
                bg_view.Layer.CornerRadius = 5;

                lbl_hanhoantat = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_content = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 2,
                    TextAlignment = UITextAlignment.Left
                };

                lbl_name = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 13f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_position = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Left,
                };

                iv_star = new UIImageView();
                iv_star.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_star.Image = UIImage.FromFile("Icons/icon_Star_off.png");

                iv_attachment = new UIImageView();
                iv_attachment.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_attachment.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                iv_attachment.TintColor = UIColor.FromRGB(94, 94, 94);

                lbl_attachCount = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Left,
                };

                iv_comment = new UIImageView();
                iv_comment.ContentMode = UIViewContentMode.ScaleAspectFit;
                iv_comment.Image = UIImage.FromFile("Icons/icon_comment.png");

                lbl_commentCount = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Left,
                };

                iv_nguoitao = new UIImageView();
                iv_nguoitao.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_nguoitao.ClipsToBounds = true;
                iv_nguoitao.Layer.CornerRadius = 15;

                iv_nguoixuly = new UIImageView();
                iv_nguoixuly.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_nguoixuly.ClipsToBounds = true;
                iv_nguoixuly.Layer.CornerRadius = 13;

                lbl_nxlCount = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Center,
                };

                bg_view.AddSubviews(new UIView[] { lbl_hanhoantat, iv_star, lbl_content, iv_nguoitao, lbl_name, lbl_position, iv_attachment, lbl_attachCount, iv_comment, lbl_commentCount, iv_nguoixuly, lbl_nxlCount });

                photoImageView.Frame = new CGRect(ContentView.Frame.X, ContentView.Frame.Y, 280, 177);
                photoImageView.AlignmentRectForFrame(photoImageView.Frame);

                ContentView.Add(bg_view);
            }

            //public override bool Editing
            //{
            //    get
            //    {
            //        return base.Editing;
            //    }
            //    set
            //    {
            //        //TODO: TableView.SpringLoaded = !value;
            //        base.Editing = value;
            //    }
            //}

            public void UpdateCell()
            {
                try
                {
                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                    //if (workflowItem.ActionStatusID == 10)
                    //    bg_view.BackgroundColor = UIColor.FromRGB(229, 255, 228);
                    //else if (workflowItem.ActionStatusID == 6)
                    //    bg_view.BackgroundColor = UIColor.FromRGB(255, 230, 230);

                    //if (workflowItem.IsInfo) // cam
                    //    bg_view.BackgroundColor = UIColor.FromRGB(255, 230, 218);
                    //else if (workflowItem.IsConsul) // xanh duong nhat
                    //    bg_view.BackgroundColor = UIColor.FromRGB(197, 221, 249);


                    if (ApprovedListID.Contains(workflowItem.StatusGroup)) // phê duyệt -> xanh.
                        bg_view.BackgroundColor = UIColor.FromRGB(229, 255, 228);
                    else if (RejectedListID.Contains(workflowItem.StatusGroup)) // Từ chối -> Đỏ
                        bg_view.BackgroundColor = UIColor.FromRGB(255, 230, 230);
                    else
                        bg_view.BackgroundColor = UIColor.White;

                    if ((workflowItem.AppFlg & 1) > 0) // Đang tham vấn -> cam
                    {
                        bg_view.BackgroundColor = UIColor.FromRGB(255, 230, 218);
                    }
                    else if ((workflowItem.AppFlg & 2) > 0) // Đang chờ bổ sung -> xanh duong nhat
                    {
                        bg_view.BackgroundColor = UIColor.FromRGB(197, 221, 249);
                    }

                    if (workflowItem.DueDate.HasValue)
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            lbl_hanhoantat.Text = workflowItem.DueDate.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                        else
                            lbl_hanhoantat.Text = workflowItem.DueDate.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
                    }
                    lbl_content.Text = workflowItem.Content;

                    // Nguoi xu ly
                    List<string> lst_userName = new List<string>();
                    nfloat temp_width = 0;
                    if (!string.IsNullOrEmpty(workflowItem.AssignedTo))
                    {
                        string str_assignedTo = workflowItem.AssignedTo;

                        string first_user = "";
                        if (str_assignedTo.Contains(','))
                        {
                            first_user = str_assignedTo.Split(',')[0];
                            lbl_nxlCount.Text = "+" + (str_assignedTo.Split(',').Count() - 1).ToString();
                        }
                        else
                            first_user = str_assignedTo;

                        //List<BeanUser> lst_userResult = new List<BeanUser>();
                        //string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        //lst_userResult = conn.Query<BeanUser>(query_user, first_user.ToLower());

                        //string user_imagePath = "";
                        //if (lst_userResult.Count > 0)
                        //    user_imagePath = lst_userResult[0].ImagePath;

                        //if (!string.IsNullOrEmpty(user_imagePath))
                        //{
                        //    checkFileLocalIsExist(lst_userResult[0], iv_nguoixuly);
                        //}


                        List<BeanUserAndGroup> lst_userAndGroupResult = new List<BeanUserAndGroup>();
                        string _queryBeanUserGroup = string.Format(@"SELECT Image as ImagePath, 1 as Type, Title as Name FROM BeanGroup WHERE ID LIKE '{0}'
                                             UNION SELECT ImagePath, 0 as Type, FullName as Name   FROM BeanUser WHERE ID LIKE '{0}'", first_user.ToLower());
                        lst_userAndGroupResult = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                        string user_imagePath = "";
                        if (lst_userAndGroupResult != null && lst_userAndGroupResult.Count > 0)
                        {
                            user_imagePath = lst_userAndGroupResult[0].ImagePath;
                            if (string.IsNullOrEmpty(user_imagePath))
                            {
                                if (lst_userAndGroupResult[0].Type == 1)
                                    iv_nguoixuly.Image = UIImage.FromFile("Icons/icon_group.png");
                                else if (lst_userAndGroupResult[0].Type == 0)
                                    iv_nguoixuly.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                            }
                            else
                                checkFileLocalIsExist(lst_userAndGroupResult[0], iv_nguoixuly);
                        }
                        else
                            iv_nguoixuly.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }

                    // Nguoi tao
                    if (!string.IsNullOrEmpty(workflowItem.CreatedBy))
                    {
                        string str_CreatedBy = workflowItem.CreatedBy;

                        List<BeanUser> lst_nguoitao = new List<BeanUser>();
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                        lst_nguoitao = conn.Query<BeanUser>(query_user, str_CreatedBy.ToLower());

                        string user_imagePath = "";
                        if (lst_nguoitao.Count > 0)
                            user_imagePath = lst_nguoitao[0].ImagePath;

                        if (!string.IsNullOrEmpty(user_imagePath))
                        {
                            checkFileLocalIsExist(lst_nguoitao[0], iv_nguoitao);
                        }

                        //lbl_name.Text = lst_nguoitao[0].FullName;
                        //lbl_position.Text = lst_nguoitao[0].Position;
                    }

                    List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                    string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                    var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, workflowItem.ID);

                    if (lst_followResult != null && lst_followResult.Count > 0)
                    {
                        if (lst_followResult[0].Status == 1)
                            iv_star.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                        else
                            iv_star.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                    }
                    else
                        iv_star.Image = UIImage.FromFile("Icons/icon_Star_off.png");

                    if (workflowItem.FileCount > 0)
                        lbl_attachCount.Text = workflowItem.FileCount.ToString();
                    else
                        lbl_attachCount.Text = "0";

                    if (workflowItem.CommentCount > 0)
                        lbl_commentCount.Text = workflowItem.CommentCount.ToString();
                    else
                        lbl_commentCount.Text = "0";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WorkFlowItemCollection_Cell_Custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (workflowItem == null)
                {
                    //bg_view.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height - 10);
                    lbl_noData.Frame = ContentView.Frame;
                    return;
                }
                bg_view.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height - 10);

                lbl_hanhoantat.Frame = new CGRect(15, 15, ContentView.Frame.Width - 50, 20);
                iv_star.Frame = new CGRect(ContentView.Frame.Width - 50, 15, 18, 18);
                iv_nguoitao.Frame = new CGRect(15, lbl_hanhoantat.Frame.Bottom + 13, 30, 30);
                lbl_content.Frame = new CGRect(iv_nguoitao.Frame.Right + 10, lbl_hanhoantat.Frame.Bottom + 5, ContentView.Frame.Width - (50 + iv_nguoitao.Frame.Right + 10), 45);


                //lbl_name.Frame = new CGRect(iv_nguoitao.Frame.Right + 10, iv_nguoitao.Frame.Y, ContentView.Frame.Width - (iv_nguoitao.Frame.Right + 40), 20);
                //lbl_position.Frame = new CGRect(iv_nguoitao.Frame.Right + 10, lbl_name.Frame.Bottom, ContentView.Frame.Width - (iv_nguoitao.Frame.Right + 40), 20);

                iv_attachment.Frame = new CGRect(15, lbl_content.Frame.Bottom + 25, 16, 16);
                lbl_attachCount.Frame = new CGRect(iv_attachment.Frame.Right + 5, iv_attachment.Frame.Y, 40, 20);

                iv_comment.Frame = new CGRect(lbl_attachCount.Frame.Right + 5, lbl_content.Frame.Bottom + 25, 16, 16);
                lbl_commentCount.Frame = new CGRect(iv_comment.Frame.Right + 5, iv_comment.Frame.Y, 40, 20);

                iv_nguoixuly.Frame = new CGRect(ContentView.Frame.Width - 80, iv_attachment.Frame.Y - 5, 26, 26);
                lbl_nxlCount.Frame = new CGRect(iv_nguoixuly.Frame.Right, iv_attachment.Frame.Y - 5, 26, 26);
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

                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void checkFileLocalIsExist(BeanUserAndGroup contact, UIImageView image_view)
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

                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
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
        #endregion
        #endregion
    }
}
