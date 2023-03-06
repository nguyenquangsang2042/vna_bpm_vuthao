using System;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    public class ComponentSection: UIView
    {
        ViewSection section { get; set; }
        UIViewController parentView { get; set; }
        nint currentSection { get; set; }

        UIView view_background;
        UILabel lbl_title, lbl_line, lbl_note;
        UIImageView iv_arrow;
        UIButton btn_section;

        public ComponentSection(UIViewController _parentView, ViewSection _section, nint _currentSection)
        {
            parentView = _parentView;
            section = _section;
            currentSection = _currentSection;

            InitializeComponent();
        }

        public void InitializeComponent()
        {
            view_background = new UIView()
            {
                BackgroundColor = UIColor.White
            };

            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold),
                TextAlignment = UITextAlignment.Left
            };

            lbl_title.Layer.BorderColor = UIColor.White.CGColor;
            lbl_title.Layer.BorderWidth = 1;
            lbl_title.Layer.CornerRadius = 4;
            lbl_title.Layer.MasksToBounds = true;
            
            iv_arrow = new UIImageView();

            lbl_line = new UILabel()
            {
                BackgroundColor = UIColor.FromRGB(245, 245, 245)
            };

            btn_section = new UIButton();
            btn_section.TranslatesAutoresizingMaskIntoConstraints = false;
            btn_section.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);

            lbl_note = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(79, 195, 247),
                Text = "Nhấp vào để xem nội dung",
                Hidden = true
            };

            this.AddSubviews(new[] { view_background, lbl_title });
            //this.AddSubviews(new UIView[] { view_background, lbl_title, lbl_note, iv_arrow, lbl_line, btn_section });

            //RequestDetailsView controller = (RequestDetailsView)parentView;
            //CmmIOSFunction.UpdateScrollTableView(this, controller.GetTableView);
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                if (section.IsShowHint)
                {
                    if (!section.ShowType)
                    {
                        view_background.Frame = new CGRect(view_background.Frame.X, view_background.Frame.Y, view_background.Frame.Width, view_background.Frame.Height - lbl_note.Frame.Height);
                        lbl_note.Hidden = true;
                    }
                }

                //WorkflowDetailView controller = (WorkflowDetailView)parentView;
                //controller.HandleSectionTable(section, currentSection);
            }
        }

        public void UpdateContentSection()
        {
            lbl_title.Text = section.Title;

            if (section.ShowType)
                iv_arrow.Image = UIImage.FromFile("Icons/icon_chevron_up.png");
            else
                iv_arrow.Image = UIImage.FromFile("Icons/icon_chevron_down.png");

            if (currentSection == 0)
                lbl_line.Hidden = true;

            if (section.IsShowHint)
                lbl_note.Hidden = section.ShowType;
        }

        public void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            //var widthTitle = StringExtensions.MeasureString(section.Title, 14).Width + 30;
            //var widthMaxTitle = parentView.View.Bounds.Width - 50;
            //if (widthTitle > widthMaxTitle)
            //    widthTitle = widthMaxTitle;

            view_background.Frame = frame;
            lbl_title.Frame = new CGRect(18, 0, frame.Width, 20);

            //lbl_title.Frame = new CGRect(10, 20, widthTitle, 30);
            //lbl_note.Frame = new CGRect(30, 50, 300, 20);
            //iv_arrow.Frame = new CGRect(frame.Width - 30, 25, 20, 20);
            //lbl_line.Frame = new CGRect(0, 0, frame.Width, 1);

            //NSLayoutConstraint.Create(btn_section, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
            //NSLayoutConstraint.Create(btn_section, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            //NSLayoutConstraint.Create(btn_section, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            //NSLayoutConstraint.Create(btn_section, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, 0.0f).Active = true;
        }
    }
}