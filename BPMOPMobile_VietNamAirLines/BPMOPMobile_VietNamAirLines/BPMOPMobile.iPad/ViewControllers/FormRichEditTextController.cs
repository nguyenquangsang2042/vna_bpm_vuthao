using System;
using System.Threading.Tasks;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using TEditor;
using TEditor.Abstractions;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FormRichEditTextController : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        bool editable;
        string content { get; set; }

        UIViewController viewController { get; set; }

        public FormRichEditTextController(IntPtr handle) : base(handle)
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
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
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

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            LoadContent();

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
        public void setContent(UIViewController _parentView, bool _editable, string _content)
        {
            content = _content;
            viewController = _parentView;
            editable = _editable;
        }

        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_agree.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
        }

        private async void LoadContent()
        {
            TEditorResponse response = await ShowTEditor("", null, false);
        }

        public void SetValueResult(string value)
        {
            //if (!string.IsNullOrEmpty(value))
            //{
            //    if (currentElement != null)
            //    {
            //        currentElement.Value = value;
            //        table_content_right.ReloadRows(new NSIndexPath[] { currentIndexPath }, UITableViewRowAnimation.None);
            //    }
            //}
        }
        public Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false)
        {
            TaskCompletionSource<TEditorResponse> taskRes = new TaskCompletionSource<TEditorResponse>();
            if (editable)
            {
                var tvc = new TEditorViewController();
                ToolbarBuilder builder = toolbarBuilder;
                if (toolbarBuilder == null)
                    builder = new ToolbarBuilder().AddStandard();

                tvc.BuildToolbar(builder);
                tvc.SetHTML(html);
                tvc.SetAutoFocusInput(autoFocusInput);
                tvc.Title = "";

                tvc.View.Frame = new CGRect(0, 0, viewRichText.Frame.Width, viewRichText.Frame.Height);
                //viewRichText.Add(tvc.View);
                this.PresentViewControllerAsync(tvc, false);
            }
            else
            {
                var myHtmlText = html.Trim();
                NSMutableAttributedString attStr = new NSMutableAttributedString(myHtmlText);
                attStr.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, myHtmlText.Length));

                var view = new UIViewController();
                UITextView txt_input = new UITextView()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    UserInteractionEnabled = false,
                    TextColor = UIColor.Black,
                    AttributedText = attStr,
                };
                txt_input.Layer.BorderWidth = 0;
                var padding = txt_input.TextContainer.LineFragmentPadding;
                txt_input.ContentInset = new UIEdgeInsets(0, -padding, 0, 0);

                view.View.AddSubview(txt_input);
                view.View.Frame = new CGRect(0, 0, viewRichText.Frame.Width, viewRichText.Frame.Height);
                txt_input.Frame = new CGRect(0, 0, view.View.Frame.Width, view.View.Frame.Height);
                this.PresentViewControllerAsync(view, false);
            }

            return taskRes.Task;
        }
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (viewController.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView formCreateTaskView = viewController as FormCreateTaskView;
                formCreateTaskView.View.EndEditing(true);
            }

            this.DismissModalViewController(true);
        }
        private void BT_agree_TouchUpInside(object sender, EventArgs e)
        {
            this.View.EndEditing(true);

            this.DismissModalViewController(true);
        }

        #endregion
    }
}

