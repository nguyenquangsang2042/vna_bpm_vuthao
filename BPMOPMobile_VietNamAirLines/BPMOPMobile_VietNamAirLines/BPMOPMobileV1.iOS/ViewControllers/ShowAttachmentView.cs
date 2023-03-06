using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ShowAttachmentView : UIViewController
    {
        ViewRequestDetails viewRequestDetails { get; set; }
        BeanAttachFile attachment { get; set; }
        string localDocumentFilepath = "";
        CmmLoading loading;

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
            loadContent();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside; ;
            #endregion
        }

        #endregion

        #region private - public method
        public void setContent(UIViewController _parentView, BeanAttachFile _attachment)
        {
            if (_parentView.GetType() == typeof(ViewRequestDetails))
                viewRequestDetails = (ViewRequestDetails)_parentView;

            attachment = _attachment;
        }

        private void ViewConfiguration()
        {
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            webview_content.ContentMode = UIViewContentMode.ScaleToFill;
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
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
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
            webview_content.LoadRequest(new NSUrlRequest(new NSUrl(localfilePath, false)));
            //webview_content.ScalesPageToFit = true;
        }

        private void openFileFromAddNew(string fileAddNew)
        {
            webview_content.LoadRequest(new NSUrlRequest(new NSUrl(fileAddNew, false)));
            //webview_content.ScalesPageToFit = true;
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewControllerAsync(true);
        }

        #endregion
    }
}

