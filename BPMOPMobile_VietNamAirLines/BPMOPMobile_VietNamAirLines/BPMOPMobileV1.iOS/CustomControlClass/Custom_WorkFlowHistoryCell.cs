using System;
using System.Globalization;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_WorkFlowHistoryCell : UITableViewCell
    {
        BeanWorkflowItem workFlowItem { get; set; }
        bool isOdd;

        UILabel lbl_imgCover, lbl_status, lbl_title, lbl_sentTime, lbl_subTitle, lbl_duedate, lbl_leftLine;
        UIImageView iv_avatar, img_attach, img_priority;

        NSLayoutConstraint constraintRightIconAttachment;

        public Custom_WorkFlowHistoryCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
        }

        public Custom_WorkFlowHistoryCell(IntPtr handle) : base(handle)
        {
        }

        public Custom_WorkFlowHistoryCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(BeanWorkflowItem _workflowItem, bool _isOdd)
        {
            workFlowItem = _workflowItem;
            isOdd = _isOdd;
            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_avatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
            iv_avatar.Hidden = true;

            lbl_imgCover = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.White
            };
            lbl_imgCover.Layer.CornerRadius = 20;
            lbl_imgCover.ClipsToBounds = true;

            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold),
                TextColor = UIColor.FromRGB(25, 25, 30),
                TextAlignment = UITextAlignment.Left,
            };

            lbl_sentTime = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(11, UIFontWeight.Regular),
                TextColor = UIColor.DarkGray,
                TextAlignment = UITextAlignment.Right,
            };

            lbl_subTitle = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(94, 94, 94),
            };

            img_attach = new UIImageView();
            img_attach.ContentMode = UIViewContentMode.ScaleAspectFit;
            img_attach.Image = UIImage.FromFile("Icons/icon_attachment.png");
            img_attach.TranslatesAutoresizingMaskIntoConstraints = false;

            img_priority = new UIImageView();
            img_priority.ContentMode = UIViewContentMode.ScaleAspectFit;
            img_priority.Image = UIImage.FromFile("Icons/icon_flag.png");
            img_priority.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_duedate = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Right,
            };

            lbl_status = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Center,
            };

            lbl_status.ClipsToBounds = true;
            lbl_status.Layer.CornerRadius = 3;
            lbl_status.Lines = 1;

            lbl_leftLine = new UILabel();
            lbl_leftLine.BackgroundColor = UIColor.FromRGB(51, 95, 179);
            lbl_leftLine.Hidden = true;

            ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_sentTime, lbl_subTitle, img_priority, img_attach, lbl_duedate, lbl_status, lbl_leftLine });
        }

        private void LoadData()
        {
            try
            {
                if (!workFlowItem.Read)
                    lbl_title.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Bold);
                else
                    lbl_title.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular);

                CultureInfo culture = new CultureInfo("vi-VN");
                if (workFlowItem.Created.HasValue)
                {
                    if (workFlowItem.Created.Value.Date == DateTime.Now.Date)
                    {
                        var hour = workFlowItem.Created.Value.Hour;
                        var minute = workFlowItem.Created.Value.Minute;

                        var hourNow = DateTime.Now.Hour;
                        var minuteNow = DateTime.Now.Minute;

                        if (hour == hourNow && minute == minuteNow)
                            lbl_sentTime.Text = "Vài giây trước";
                        else
                        {
                            var totalMinutes = (DateTime.Now - workFlowItem.Created.Value).TotalMinutes;
                            lbl_sentTime.Text = Math.Round(totalMinutes) + " Phút trước";
                        }
                    }
                    else if (workFlowItem.Created < DateTime.Now)
                        lbl_sentTime.Text = culture.DateTimeFormat.GetDayName(workFlowItem.Created.Value.DayOfWeek);
                }

                if (!string.IsNullOrEmpty(workFlowItem.AssignedToName))
                {
                    lbl_imgCover.Hidden = false;
                    iv_avatar.Hidden = true;
                    lbl_imgCover.Text = workFlowItem.AssignedToName.Substring(0, 1).ToUpper();
                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                }
                else
                {
                    iv_avatar.Hidden = false;
                    lbl_imgCover.Hidden = true;
                }

                //if (workFlowItem.HasFile.HasValue && workFlowItem.HasFile.Value)
                //    img_attach.Hidden = false;
                //else
                //    img_attach.Hidden = true;

                //if (workFlowItem.Priority == 2)
                //    img_priority.Hidden = false;
                //else
                //    img_priority.Hidden = true;

                if (workFlowItem.IssueDate.HasValue)
                {
                    lbl_duedate.Text = workFlowItem.IssueDate.Value.ToString("dd/MM/yy HH:mm");

                    if (workFlowItem.IssueDate.Value.Date < DateTime.Now.Date)//(noti.Status == -1)
                    {
                        var numDate = (DateTime.Now.Date - workFlowItem.IssueDate.Value.Date).Days;
                        lbl_duedate.Text = "Quá hạn " + numDate.ToString() + " ngày";
                        lbl_duedate.TextColor = UIColor.Red;
                    }
                    else if (workFlowItem.IssueDate.Value.Date == DateTime.Now.Date)//(noti.Status == 0)
                    {
                        lbl_duedate.Text = "Còn 1 ngày";
                        lbl_duedate.TextColor = UIColor.FromRGB(9, 171, 78);
                    }
                    else //if (noti.Status == 1)
                    {
                        lbl_duedate.TextColor = UIColor.FromRGB(161, 175, 182);
                    }
                }

                if (!string.IsNullOrEmpty(workFlowItem.ActionStatus))
                {
                    //lbl_status.BackgroundColor = UIColor.FromRGB(197, 221, 249);
                    //lbl_status.TextColor = UIColor.FromRGB(51, 95, 179);
                    //lbl_status.Hidden = false;
                    switch (workFlowItem.ActionStatus.ToLower())
                    {
                        case "chờ phê duyệt":
                            lbl_status.BackgroundColor = UIColor.FromRGB(197, 221, 249);
                            lbl_status.TextColor = UIColor.FromRGB(51, 95, 179);
                            break;
                        case "đã phê duyệt":
                            lbl_status.BackgroundColor = UIColor.FromRGB(119, 224, 117);
                            lbl_status.TextColor = UIColor.White;
                            break;
                        case "bổ sung thông tin":
                            lbl_status.BackgroundColor = UIColor.FromRGB(255, 160, 114);
                            lbl_status.TextColor = UIColor.White;
                            break;
                        case "từ chối":
                            lbl_status.BackgroundColor = UIColor.Red;
                            lbl_status.TextColor = UIColor.White;
                            break;
                        case "hủy":
                            lbl_status.BackgroundColor = UIColor.FromRGB(216, 216, 216);
                            lbl_status.TextColor = UIColor.White;
                            break;
                        default:
                            lbl_status.BackgroundColor = UIColor.FromRGB(197, 221, 249);
                            lbl_status.TextColor = UIColor.FromRGB(51, 95, 179);
                            break;
                    }
                }
                else
                    lbl_status.Hidden = true;

                lbl_title.Text = workFlowItem.Title;
                lbl_subTitle.Text = workFlowItem.ListName;
                lbl_status.Text = workFlowItem.ActionStatus;
                lbl_leftLine.Hidden = !workFlowItem.IsSelected;
                if (workFlowItem.IsSelected)
                    ContentView.BackgroundColor = UIColor.FromRGB(246, 249, 255);
                else
                {
                    if (isOdd)
                        ContentView.BackgroundColor = UIColor.White;
                    else
                        ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Custom_WorkFlowCell - loaddata- ERR: " + ex.ToString());
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            lbl_leftLine.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);

            iv_avatar.Frame = new CGRect(18, 20, 40, 40);
            lbl_imgCover.Frame = new CGRect(18, 20, 40, 40);
            lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 20, ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 10 + 90), 25);
            lbl_sentTime.Frame = new CGRect(ContentView.Frame.Width - 90, lbl_title.Frame.Y, 80, 25);
            lbl_subTitle.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, lbl_sentTime.Frame.Bottom, ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 10 + 65), 20);

            var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 13).Width + 20;
            var maxStatusWidth = ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 10 + 110);
            if (widthStatus < maxStatusWidth)
                lbl_status.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, lbl_subTitle.Frame.Bottom + 5, widthStatus, 25);
            else
                lbl_status.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 25);

            lbl_duedate.Frame = new CGRect(ContentView.Frame.Width - 110, lbl_status.Frame.Y, 100, 25);

            //img_attach.HeightAnchor.ConstraintEqualTo(16).Active = true;
            //img_attach.WidthAnchor.ConstraintEqualTo(16).Active = true;
            //NSLayoutConstraint.Create(this.img_attach, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_sentTime, NSLayoutAttribute.Bottom, 1.0f, 5).Active = true;
            //if (workFlowItem.HasFile.HasValue && workFlowItem.HasFile.Value)
            //{
            //    constraintRightIconAttachment = NSLayoutConstraint.Create(this.img_attach, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Right, 1.0f, -18.0f);
            //    constraintRightIconAttachment.Active = true;
            //}
            //else
            //{
            //    constraintRightIconAttachment = NSLayoutConstraint.Create(this.img_attach, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Right, 1.0f, 10.0f);
            //    constraintRightIconAttachment.Active = true;
            //}

            //img_priority.HeightAnchor.ConstraintEqualTo(16).Active = true;
            //img_priority.WidthAnchor.ConstraintEqualTo(16).Active = true;
            //NSLayoutConstraint.Create(this.img_priority, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_sentTime, NSLayoutAttribute.Bottom, 1.0f, 5).Active = true;
            //NSLayoutConstraint.Create(this.img_priority, NSLayoutAttribute.Right, NSLayoutRelation.Equal, img_attach, NSLayoutAttribute.Left, 1.0f, -5.0f).Active = true;
        }
    }
}
