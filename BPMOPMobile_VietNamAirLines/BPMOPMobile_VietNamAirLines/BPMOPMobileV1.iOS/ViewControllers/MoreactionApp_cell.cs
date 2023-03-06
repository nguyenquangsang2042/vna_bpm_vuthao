using System;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MoreactionApp_cell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("MoreactionApp_cell");
        public static readonly UINib Nib;

        static MoreactionApp_cell()
        {
            Nib = UINib.FromName("MoreactionApp_cell", NSBundle.MainBundle);
        }

        protected MoreactionApp_cell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            
        }
        public void UpdateCell(ButtonActionApp _menu, bool is_lasted)
        {
            lbl_title.Text = _menu.title;
            img_action.Image = UIImage.FromFile(_menu.iconUrl);

            if (is_lasted)
                lbl_line.Hidden = true;
            else
                lbl_line.Hidden = false;
        }
    }
}
