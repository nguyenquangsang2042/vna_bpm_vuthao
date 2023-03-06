using System;
using System.Drawing;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FullTextView : UIViewController
    {
        string titleText = string.Empty;
        string fullText = string.Empty;
        bool isHTML = false;
        public FullTextView(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LoadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        public void SetContent(string _titleText, string _fullText, bool _isHTML = false)
        {
            this.titleText = _titleText;
            this.fullText = _fullText;
            this.isHTML = _isHTML;
        }

        private void LoadContent()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();
            lbl_title.Text = titleText;
            if (isHTML)
            {
                var nsError = new NSError();
                var atts = new NSAttributedStringDocumentAttributes
                {
                    DocumentType = NSDocumentType.HTML,
                    StringEncoding = NSStringEncoding.UTF8,
                };

                var myHtmlText = fullText.Trim();
                var attStr = new NSAttributedString(NSData.FromString(myHtmlText), atts, ref nsError);

                txt_content.AttributedText = attStr;
            }
            else
            {
                var myHtmlText = fullText.Trim();
                NSMutableAttributedString attStr = new NSMutableAttributedString(myHtmlText);
                attStr.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, myHtmlText.Length));

                txt_content.AttributedText = attStr;
            }

            txt_content.ScrollEnabled = true;
            txt_content.Editable = false;
        }

        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        #endregion
    }
}