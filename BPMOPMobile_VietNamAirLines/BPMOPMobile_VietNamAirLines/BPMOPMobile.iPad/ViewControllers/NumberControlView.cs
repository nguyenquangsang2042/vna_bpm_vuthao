using System;
using System.Globalization;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class NumberControlView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        UIViewController viewController { get; set; }
        ViewElement element { get; set; }
        CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
        CultureInfo culVN = CultureInfo.GetCultureInfo("vi-VN");

        public NumberControlView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            //gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            //{
            //    var touchView = touch.View.Class.Name;
            //    if (touchView == "UITextField" || touchView == "UITextView")
            //        positionBotOfCurrentViewInput = GetPositionBotView(touch.View);
            //    else
            //        positionBotOfCurrentViewInput = 0.0f;

            //    return true;
            //};

            gesture.CancelsTouchesInView = true;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            loadContent();
            setlangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_agree.TouchUpInside += BT_agree_TouchUpInside;
            tf_content.EditingChanged += Tf_content_EditingChanged;
            tf_content.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
                int number;
                return newContent.Length <= 1000 && (replacement.Length == 0 || (replacement == ".") || int.TryParse(replacement, out number));
            };
            #endregion
        }

        private void Tf_content_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                //co decimal h
                if (!string.IsNullOrEmpty(tf_content.Text))
                {
                    string temp = tf_content.Text;
                    if (!string.IsNullOrEmpty(element.DataSource))
                    {
                        var arrText = temp.Split(".");
                        if (arrText.Length >= 2)
                        {
                            if (string.IsNullOrEmpty(arrText[0]) || arrText.Length >= 3) // neu ki tu dau la "." hoac co 2 dau "."
                            {
                                tf_content.Text = tf_content.Text.Remove(tf_content.Text.Length - 1, 1);
                                CmmIOSFunction.commonAlertMessage(this, "BPM", "This is a number only field");
                                return;
                            }
                        }
                        //dau phay dong
                        temp = String.Format("{0:n0}", float.Parse(arrText[0]));
                        if (arrText.Length == 2)
                            temp = temp.Split(".")[0] + "." + arrText[1];

                        tf_content.Text = temp;

                    }
                    else//Parse khong co decimal
                    {
                        var custValue = double.Parse(temp, cul).ToString("N0", cul);
                        tf_content.Text = custValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("NumberControlView - Tf_content_EditingChanged - Err: " + ex.ToString());
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
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
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_agree.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            tf_content.Layer.CornerRadius = 4;
            tf_content.Layer.BorderColor = UIColor.DarkGray.CGColor;
            tf_content.Layer.BorderWidth = 0.5f;
            //textview_content.BecomeFirstResponder();
        }

        private void loadContent()
        {
            try
            {
                if (element != null)
                {
                    lbl_noteTitle.Text = element.Title;

                    //var custValue = double.Parse(element.Value.ToString().Trim(), cul).ToString("N0", culVN);
                    var custValue = CmmFunction.GetFormatControlDecimal(element);
                    tf_content.Text = custValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("NumberControlView - loadContent - Err: " + ex.ToString());
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
            try
            {
                this.View.EndEditing(true);
                string text_num = tf_content.Text;

                //chuan hoa value
                if (!string.IsNullOrEmpty(element.DataSource))
                {
                    if (string.IsNullOrEmpty(text_num[text_num.Length - 1].ToString())) // ki tu cuoi cung la "." thi xoa "." luu temp
                        text_num = tf_content.Text.Remove(tf_content.Text.Length - 1, 1);
                    //Parse theo decimal
                    int _demicalCount = int.Parse(element.DataSource);
                    var custValue = double.Parse(text_num, cul).ToString("N" + ((_demicalCount > 0) ? _demicalCount.ToString() : "0"), cul);
                    element.Value = custValue;
                }
                else
                    element.Value = text_num;

                if (viewController.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView Parentview = viewController as ToDoDetailView;
                    Parentview.HandleEditNumber(element);
                }
                else if (viewController.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty Parentview = viewController as FormWFDetailsProperty;
                    Parentview.HandleEditNumber(element);
                }
                else if (viewController.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController Parentview = viewController as FollowListViewController;
                    Parentview.HandleEditNumber(element);
                }

                this.DismissModalViewController(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("NumberControlView - BT_agree_TouchUpInside - Err: " + ex.ToString());
            }
        }
        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                //if (View.Frame.Y == 0)
                //{
                //    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);

                //    var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                //    if (topKeybroad < positionBotOfCurrentViewInput)
                //    {
                //        CGRect custFrame = View.Frame;
                //        custFrame.Y -= (positionBotOfCurrentViewInput - topKeybroad);
                //        View.Frame = custFrame;
                //    }

                //}
            }
            catch (Exception ex)
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                //if (View.Frame.Y != 0)
                //{
                //    CGRect custFrame = View.Frame;
                //    custFrame.Y = 0;
                //    View.Frame = custFrame;
                //}
            }
            catch (Exception ex)
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
        }
        #endregion
    }
}

