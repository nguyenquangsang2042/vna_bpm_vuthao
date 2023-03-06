using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using Foundation;
using System;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public partial class Moreaction_cell : UITableViewCell
    {

        public Moreaction_cell(IntPtr handle) : base(handle)
        {

        }

        public Moreaction_cell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            
        }

        public void UpdateCell(ButtonAction _menu, bool is_lasted)
        {
            lbl_title.Text = _menu.Title;
            img_action.Image = UIImage.FromFile("Icons/icon_Btn_action_" + _menu.ID);

            if (is_lasted)
                lbl_line.Hidden = true;
            else
                lbl_line.Hidden = false;
        }
    }
}