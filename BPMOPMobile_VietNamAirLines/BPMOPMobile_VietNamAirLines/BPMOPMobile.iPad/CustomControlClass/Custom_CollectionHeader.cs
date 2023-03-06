using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_CollectionHeader : UICollectionReusableView
    {
        public static NSString Key = new NSString("headerSessionID");
        UIImageView img_arrow;
        UILabel lbl_title;
        UIButton BT_section;
        KeyValuePair<string, bool> section;

        public BroadView boardView { get; set; }

        [Export("initWithFrame:")]
        public Custom_CollectionHeader(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }
        private void ViewConfiguration()
        {
            this.BackgroundColor = UIColor.White;

            img_arrow = new UIImageView();
            img_arrow.ContentMode = UIViewContentMode.ScaleAspectFill;

            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold),
                TextColor = UIColor.FromRGB(0 , 95, 212),
                TextAlignment = UITextAlignment.Left,
            };

            BT_section = new UIButton();
            BT_section.TouchUpInside += delegate
            {
                KeyValuePair<string, bool> sectionState;

                sectionState = new KeyValuePair<string, bool>(section.Key, !section.Value);
                boardView.UpdateColletionView(sectionState);
            };

            this.AddSubviews(img_arrow, lbl_title, BT_section);
        }
        public void UpdateRow(KeyValuePair<string, bool> _section)
        {
            section = _section;
            lbl_title.Text = _section.Key;
            if (_section.Value)
                img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown_colapse.png");
            else
                img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown.png");

        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            img_arrow.Frame = new CGRect(20, 9, 10, 10);
            lbl_title.Frame = new CGRect(img_arrow.Frame.Right + 10, 5, this.Frame.Width - 50, 20);
            BT_section.Frame = new CGRect(0, 0, this.Frame.Width, 40);
        }
    }
}
