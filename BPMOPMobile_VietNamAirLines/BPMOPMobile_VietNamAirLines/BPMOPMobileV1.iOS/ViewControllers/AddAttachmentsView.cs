using System;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class AddAttachmentsView : UIViewController
    {
        UIViewController parentView { get; set; }
        CreateTicketFormView createTicketFormView { get; set; }
        Custom_AttachFileView custom_attachFileView { get; set; }
        UIImagePickerController imagePicker;
        public ViewElement element { get; set; }

        public AddAttachmentsView(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();

            custom_attachFileView = Custom_AttachFileView.Instance;

            if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                custom_attachFileView.parentview = formTaskDetails;
            }
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                custom_attachFileView.parentview = requestDetailsV2;
            }
            custom_attachFileView.element = element;
            custom_attachFileView.supperview = this;
            custom_attachFileView.InitFrameView(new CGRect(0, headerView_constantHeight.Constant, this.View.Frame.Width, this.View.Frame.Height - headerView_constantHeight.Constant)); //this.View.Frame
            custom_attachFileView.TableLoadData();
            this.View.Add(custom_attachFileView);
        }

        public void SetContent(UIViewController _parentView, ViewElement _element)
        {
            parentView = _parentView;
            element = _element;
        }

        private void ViewConfiguration()
        {
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            headerView_constantHeight.Constant = 10 + CmmIOSFunction.GetHeaderViewHeight();
        }

        public void HandleAddAttachFileClose()
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
            {
                custom_attachFileView.RemoveFromSuperview();
                this.DismissViewController(true, null);
            }
        }
    }
}

