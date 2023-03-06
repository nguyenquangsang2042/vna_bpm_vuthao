using System;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;
using static BPMOPMobile.iPad.IOSClass.CmmIOSFunction;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FormEditTextController : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        bool editable;
        UIViewController viewController { get; set; }
        ViewElement element { get; set; }
        string content = "";

        public FormEditTextController(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_willResignActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);

            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;
                if (touchView == "UITextField" || touchView == "UITextView")
                    positionBotOfCurrentViewInput = GetPositionBotView(touch.View);
                else
                    positionBotOfCurrentViewInput = 0.0f;

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            loadContent();
            setlangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_agree.TouchUpInside += BT_agree_TouchUpInside; ;
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
        public void setContent(UIViewController _parentView, int _type, bool _editable, ViewElement _element, string _content)
        {
            element = _element;
            viewController = _parentView;
            editable = _editable;
            content = _content;
        }

        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_agree.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            textview_content.Layer.CornerRadius = 4;
            textview_content.Layer.BorderColor = UIColor.DarkGray.CGColor;
            textview_content.Layer.BorderWidth = 0.5f;
            textview_content.ScrollEnabled = true;

            if (!editable)
            {
                //textview_content.BecomeFirstResponder();
                BT_agree.Hidden = true;
                textview_content.Editable = false;
            }
        }

        private void loadContent()
        {
            if (element != null)
            {
                lbl_noteTitle.Text = element.Title;

                if (element.DataType == "textinput" || element.DataType == "textinputmultiline")
                {
                    textview_content.Text = element.Value;
                }
                else if (element.DataType == "textinputformat")
                {
                    var nsError = new NSError();
                    var atts = new NSAttributedStringDocumentAttributes
                    {
                        DocumentType = NSDocumentType.HTML,
                        StringEncoding = NSStringEncoding.UTF8
                    };

                    var myHtmlText = element.Value.Trim();
                    var attStr = new NSAttributedString(NSData.FromString(myHtmlText), atts, ref nsError);

                    textview_content.Text = attStr.Value;
                }
            }
            else
            {
                LoadContentHtml();
            }
        }

        private void LoadContentHtml()
        {
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    //var nsError = new NSError();
                    //var atts = new NSAttributedStringDocumentAttributes
                    //{
                    //    DocumentType = NSDocumentType.HTML,
                    //    StringEncoding = NSStringEncoding.UTF8
                    //};

                    //var myHtmlText = task.Content.Trim();
                    //NSMutableAttributedString att = new NSMutableAttributedString(myHtmlText);
                    //att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, myHtmlText.Length));

                    //txt_note.AttributedText = att;

                    NSString htmlString = new NSString(content);
                    NSData htmlData = NSData.FromString(GetHtmlStyle() + htmlString);
                    NSAttributedStringDocumentAttributes importParams = new NSAttributedStringDocumentAttributes();
                    importParams.DocumentType = NSDocumentType.HTML;
                    importParams.StringEncoding = NSStringEncoding.UTF8;

                    NSError error = new NSError();
                    error = null;
                    NSDictionary dict = new NSDictionary();
                    UIFont font = UIFont.FromName("ArialMT", 14f);
                    if (font != null)
                    {
                        dict = new NSMutableDictionary()
                            {
                                {
                                    UIStringAttributeKey.Font,
                                    font
                                }
                            };
                    }

                    var attrString = new NSAttributedString(htmlData, importParams, out dict, ref error);
                    textview_content.AttributedText = attrString;

                    //var height = StringExtensions.StringHeight(textview_content.Text, UIFont.SystemFontOfSize(14), txt_note.Frame.Width);
                    //if (height > textview_content.Frame.Height)
                    //    BT_contentMore.Hidden = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ControlTextInputFormat - Value - err: " + ex.ToString());
                    throw;
                }
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private nfloat GetPositionBotView(UIView view)
        {
            nfloat bottom = view.Frame.Height;
            UIView supperView = view;
            do
            {
                bottom += supperView.Frame.Y;
                supperView = supperView.Superview;

            } while (supperView != this.View);

            return bottom + 20;
        }
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }
        private void BT_agree_TouchUpInside(object sender, EventArgs e)
        {
            this.View.EndEditing(true);
            string note = textview_content.Text;
            element.Value = note;

            if (viewController.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView Parentview = viewController as ToDoDetailView;
                Parentview.HandleSingleLine(element);
            }
            else if (viewController.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView Parentview = viewController as WorkflowDetailView;
                Parentview.HandleSingleLine(element);
            }
            else if (viewController.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails Parentview = viewController as FormWorkFlowDetails;
                Parentview.HandleSingleLine(element);
            }
            else if (viewController.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty Parentview = viewController as FormWFDetailsProperty;
                Parentview.HandleSingleLine(element);
            }
            else if (viewController.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController Parentview = viewController as FollowListViewController;
                Parentview.HandleSingleLine(element);
            }

            this.DismissModalViewController(true);
        }
        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y == 0)
                {
                    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);

                    var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                    if (topKeybroad < positionBotOfCurrentViewInput)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y -= (positionBotOfCurrentViewInput - topKeybroad);
                        View.Frame = custFrame;
                    }
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y != 0)
                {
                    CGRect custFrame = View.Frame;
                    custFrame.Y = 0;
                    View.Frame = custFrame;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
        }
        #endregion
    }
}

