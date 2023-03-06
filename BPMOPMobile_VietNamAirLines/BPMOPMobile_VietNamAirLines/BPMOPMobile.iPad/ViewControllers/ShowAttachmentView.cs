using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class ShowAttachmentView : UIViewController
    {
        BeanAttachFile attachment { get; set; }
        string localDocumentFilepath = "";
        CmmLoading loading;
        //ToDoDetailView viewcontroller { get; set; }
        UIViewController viewcontroller { get; set; }

        public ShowAttachmentView(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), CmmVariable.M_DataFolder);
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
            //loadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            loadContent();
        }

        #endregion

        #region private - public method
        public void setContent(UIViewController _parentView, BeanAttachFile _attachment)
        {
            //if (_parentView.GetType() == typeof(ToDoDetailView))
            //    viewcontroller = (ToDoDetailView)_parentView;
            viewcontroller = _parentView;
            attachment = _attachment;
        }

        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
        }

        private void loadContent()
        {
            lbl_title.Text = attachment.Title;
            if (string.IsNullOrEmpty(attachment.ID))
            {
                if (!string.IsNullOrEmpty(attachment.Path))
                    openFileFromAddNew(attachment.Path);
                else
                    openFileFromAddNew(attachment.Url);
            }
            else
            {
                if (!string.IsNullOrEmpty(attachment.Path))
                    checkFileLocalIsExist(attachment.Path);
                else
                    checkFileLocalIsExist(attachment.Url);
            }
        }

        private async void checkFileLocalIsExist(string filepathURL)
        {
            try
            {
                //loading = new CmmLoading(new CGRect((viewcontroller.View.Bounds.Width - 200) / 2, (viewcontroller.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");//this.

                loading = new CmmLoading(new CGRect(this.View.Center.X - 100, this.View.Center.Y - 100, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                bool result = false;
                string filename = filepathURL.Split('/').Last();
                filepathURL = CmmVariable.M_Domain + filepathURL;
                string localfilePath = Path.Combine(localDocumentFilepath, filename);

                if (!File.Exists(localfilePath))
                {
                    await Task.Run(() =>
                    {
                        ProviderBase provider = new ProviderBase();
                        if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            result = true;
                        else
                            result = false;
                    });

                    if (result == true)
                    {
                        loading.Hide();
                        openFile(filename);
                    }
                    else
                    {
                        loading.Hide();
                        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_CheckInternet", "Kiểm tra kết nối Internet"));
                    }
                }
                else
                {
                    openFile(localfilePath);
                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("FileViewController - checkFileLocalIsExist - Err: " + ex.ToString());
            }
        }

        private void openFile(string fileName)
        {
            string localfilePath = Path.Combine(localDocumentFilepath, fileName);
            Uri fileUri = new Uri(localfilePath);
            webview_content.LoadFileUrl(new NSUrl(fileUri.AbsoluteUri), new NSUrl(fileUri.AbsoluteUri));
        }

        private void openFileFromAddNew(string fileAddNew)
        {
            Uri fileUri = new Uri(fileAddNew);
            webview_content.LoadFileUrl(new NSUrl(fileUri.AbsoluteUri), new NSUrl(fileUri.AbsoluteUri));
            //webview_content.LoadRequest(new NSUrlRequest(new NSUrl(fileAddNew, false)));
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewControllerAsync(true);
        }

        #endregion
    }
}

