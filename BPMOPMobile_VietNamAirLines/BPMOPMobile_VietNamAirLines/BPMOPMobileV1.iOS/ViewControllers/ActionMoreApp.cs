using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ActionMoreApp : UIViewController
    {
        UIViewController parent;
        List<ButtonActionApp> classMenuOption;

        public ActionMoreApp(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            ViewConfiguration();
            LoadContent();
           
            setlangTitle();
            BT_cancel.TouchUpInside += BT_cancel_TouchUpInside;
            
        }
        private void ViewConfiguration()
        {
            table_moreAction.TableHeaderView = new UIView();
            table_moreAction.TableFooterView = new UIView();
            table_moreAction.BackgroundColor = UIColor.Red;
        }
        private void LoadContent()
        {
            if (classMenuOption != null && classMenuOption.Count > 0)
            {
                constraint_heightTable.Constant = classMenuOption.Count * 60;
                table_moreAction.Source = new MenuAction_TableSource(classMenuOption, this);
                table_moreAction.ReloadData();
            }
           
        }
        public void setContent(UIViewController _parent, List<ButtonActionApp> _classMenuOption)
        {
            parent = _parent;
            classMenuOption = _classMenuOption;
        }
        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        public void ActionSelected(ButtonActionApp buttonActionApp)
        {
            this.DismissViewControllerAsync(true);
            ButtonsActionBroadBotBarApplication buttonActionBotBarApplication = ButtonsActionBroadBotBarApplication.Instance;
            buttonActionBotBarApplication.SelectFromeMoreAcTion(buttonActionApp);
        }
        private void BT_cancel_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private class MenuAction_TableSource : UITableViewSource
        {
            static readonly NSString cellIdentifier = new NSString("Moreaction_cell");
            List<ButtonActionApp> items;
            ActionMoreApp parentview;

            public MenuAction_TableSource(List<ButtonActionApp> lst_items, ActionMoreApp controler)
            {
                parentview = controler;
                items = lst_items;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return items.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                parentview.ActionSelected(items[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                MoreactionApp_cell cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as MoreactionApp_cell;
                if (indexPath.Row == items.Count - 1)
                    cell.UpdateCell(items[indexPath.Row], true);
                else
                    cell.UpdateCell(items[indexPath.Row], false);

                return cell;

            }
        }
    }

}


