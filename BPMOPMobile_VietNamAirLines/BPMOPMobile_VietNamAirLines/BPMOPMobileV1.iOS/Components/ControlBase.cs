using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    public class ControlBase : ComponentBase
    {
        public UILabel lbl_title, lbl_value, lbl_line;
        public UIButton BT_action;

        /// <summary>
        /// Cấu hình component mặc định gồm tiều đề và giá trị
        /// </summary>
        public override void InitializeComponent()
        {
            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 11f),
                TextColor = UIColor.FromRGB(94, 94, 94),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            lbl_value = new UILabel()
            {
                //Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                Font = UIFont.FromName("ArialMT", 14f),
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(25,25,30),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            lbl_line = new UILabel()
            {
                BackgroundColor = UIColor.FromRGB(144, 164, 174),
                Hidden = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            BT_action = new UIButton();
            BT_action.TranslatesAutoresizingMaskIntoConstraints = false;

            this.Add(lbl_title);
            this.Add(lbl_value);
            this.Add(lbl_line);
            this.Add(BT_action);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);

            const int paddingTop = 10;
            const int spaceView = 3;

            var height = frame.Height - ((paddingTop * 2) + spaceView);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            //lbl_value.HeightAnchor.ConstraintEqualTo(height / 2).Active = true;
            NSLayoutConstraint.Create(this.lbl_value, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, 10).Active = true;
            NSLayoutConstraint.Create(this.lbl_value, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_value, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;

            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.BT_action, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f).Active = true;
        }

        public override void SetRequire()
        {
            base.SetRequire();

            if (lbl_title.Text.Contains("(*)"))
            {
                var str_transalte = lbl_title.Text;
                var indexA = str_transalte.IndexOf('*');
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Red, new NSRange(indexA, 1));
                lbl_title.AttributedText = att;
            }
        }
    }
}