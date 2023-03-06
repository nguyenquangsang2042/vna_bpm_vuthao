using System;
using System.Drawing;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class TextViewControlView : UIViewController
    {
        UIViewController viewController { get; set; }
        ViewElement element { get; set; }

        public TextViewControlView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            loadContent();
            setlangTitle();

            #region delegate
            BT_back.TouchUpInside += BT_close_TouchUpInside;
            BT_done.TouchUpInside += BT_done_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parentView"></param>
        /// <param name="_type">1: Singleline - 2: Multiline</param>
        /// <param name="_control"></param>
        //public void setContent(UIViewController _parentView, int _type, BeanControlDynamicDetail _control)
        public void setContent(UIViewController _parentView, int _type, ViewElement _element)
        {
            element = _element;
            viewController = _parentView;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();
            BT_back.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            BT_done.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);

            textview_content.Layer.CornerRadius = 4;
            textview_content.Layer.BorderColor = UIColor.DarkGray.CGColor;
            textview_content.Layer.BorderWidth = 0.5f;
            textview_content.BecomeFirstResponder();
        }

        private void loadContent()
        {
            if (element != null)
            {
                var nsError = new NSError();
                var atts = new NSAttributedStringDocumentAttributes
                {
                    DocumentType = NSDocumentType.HTML,
                    StringEncoding = NSStringEncoding.UTF8
                };

                var myHtmlText = element.Value.Trim();
                var attStr = new NSAttributedString(NSData.FromString(myHtmlText), atts, ref nsError);

                lbl_title.Text = element.Title;
                textview_content.Text = attStr.Value;
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void BT_done_TouchUpInside(object sender, EventArgs e)
        {
            this.View.EndEditing(true);
            string note = textview_content.Text;
            element.Value = note;

            if (viewController.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 Parentview = viewController as RequestDetailsV2;
                Parentview.HandleSingleLine(element);
            }
            else if (viewController.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty Parentview = viewController as FormWFDetailsProperty;
                Parentview.HandleSingleLine(element);
            }

            this.NavigationController.PopViewController(true);
        }
        #endregion
    }
}

