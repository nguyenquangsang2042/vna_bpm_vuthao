using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    class Custom_UserListVerticalView: UIView
    {
        UITableView table_user;

        private Custom_UserListVerticalView()
        {
            this.BackgroundColor = UIColor.White;

            table_user = new UITableView();
            table_user.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_user.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            this.AddSubview(table_user);
        }

        private static Custom_UserListVerticalView instance = null;
        public static Custom_UserListVerticalView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_UserListVerticalView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;

            table_user.Frame = new CGRect(20, 20, Frame.Width - 40, Frame.Height - 40);
        }

        public void TableLoadData()
        {
            table_user.Source = new MenuOption_TableSource(ListUser, this);
        }

        public void AddShadowForView()
        {
            this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, Frame.Width, Frame.Height)).CGPath;
            this.Layer.ShadowRadius = 5;
            this.Layer.ShadowOffset = new CGSize(0, 2);
            this.Layer.ShadowOpacity = 1;
            this.ClipsToBounds = false;
        }

        public List<BeanUser> ListUser { get; set; }

        public UIViewController viewController { get; set; }

        public int RowHeigth => 78;

        private void HandleItemSelect(BeanUser _user)
        {
            if (viewController != null && viewController.GetType() == typeof(FormAdditionalInformationView))
            {
                FormAdditionalInformationView controller = (FormAdditionalInformationView)viewController;
                controller.selectedUser(_user);
            }
        }

        #region custom views
        #region table data source user
        private class MenuOption_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cellUser");
            Custom_UserListVerticalView parentView;

            public MenuOption_TableSource(List<BeanUser> _users, Custom_UserListVerticalView _parentview)
            {
                parentView = _parentview;
                lst_user = _users;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_user != null) ? lst_user.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 78;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_user[indexPath.Row];
                parentView.HandleItemSelect(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Custom_UserCell cell = new Custom_UserCell(cellIdentifier, null);
                var user = lst_user[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                cell.UpdateCell(user, false, isOdd);
                return cell;
            }
        }
        #endregion
        #endregion
    }
}