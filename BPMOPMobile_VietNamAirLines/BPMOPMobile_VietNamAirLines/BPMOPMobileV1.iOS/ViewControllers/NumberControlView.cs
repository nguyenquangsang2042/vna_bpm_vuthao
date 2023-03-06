using System;
using System.Drawing;
using System.Globalization;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class NumberControlView : UIViewController
    {
        UIViewController parent { get; set; }
        ViewElement element { get; set; }
        CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
        CultureInfo culVN = CultureInfo.GetCultureInfo("vi-VN");

        public NumberControlView(IntPtr handle) : base(handle)
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
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_done.TouchUpInside += BT_done_TouchUpInside;
            textfield_num.EditingChanged += Textfield_num_EditingChanged;
            textfield_num.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();
                int number;
                return newContent.Length <= 1000 && (replacement.Length == 0 || (replacement == ".") || int.TryParse(replacement, out number));
            };
            #endregion
        }

        private void Textfield_num_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                //co decimal h
                if (!string.IsNullOrEmpty(textfield_num.Text))
                {
                    string temp = textfield_num.Text;
                    if (!string.IsNullOrEmpty(element.DataSource))
                    {
                        var arrText = temp.Split(".");
                        if (arrText.Length >= 2)
                        {
                            if (string.IsNullOrEmpty(arrText[0]) || arrText.Length >= 3) // neu ki tu dau la "." hoac co 2 dau "."
                            {
                                textfield_num.Text = textfield_num.Text.Remove(textfield_num.Text.Length - 1, 1);
                                CmmIOSFunction.commonAlertMessage(this, "BPM", "This is a number only field");
                                return;
                            }
                        }
                        //dau phay dong
                        temp = string.Format("{0:n0}", float.Parse(arrText[0]));
                        if (arrText.Length == 2)
                            temp = temp.Split(".")[0] + "." + arrText[1];

                        textfield_num.Text = temp;
                    }
                    else//Parse khong co decimal
                    {
                        var custValue = double.Parse(temp, cul).ToString("N0", cul);
                        textfield_num.Text = custValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("NumberControlView - Tf_content_EditingChanged - Err: " + ex.ToString());
            }
        }

        #endregion

        #region private - public method
        public void setContent(UIViewController _parentView, int _type, ViewElement _element)
        {
            element = _element;
            parent = _parentView;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();

            textfield_num.BecomeFirstResponder();

            if (!string.IsNullOrEmpty(element.DataSource))
                textfield_num.KeyboardType = UIKeyboardType.DecimalPad;
            else
                textfield_num.KeyboardType = UIKeyboardType.NumberPad;
        }

        private void loadContent()
        {

            if (element != null)
            {
                lbl_title.Text = element.Title;

                //var custValue = double.Parse(element.Value.ToString().Trim(), cul).ToString("N0", culVN);
                var custValue = CmmFunction.GetFormatControlDecimal(element);
                textfield_num.Text = custValue;
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events

        private void BT_done_TouchUpInside(object sender, EventArgs e)
        {
            this.View.EndEditing(true);
            string text_num = textfield_num.Text;

            //chuan hoa value
            if (!string.IsNullOrEmpty(element.DataSource) && !string.IsNullOrEmpty(text_num))
            {
                if (string.IsNullOrEmpty(text_num[text_num.Length - 1].ToString())) // ki tu cuoi cung la "." thi xoa "." luu temp
                    text_num = textfield_num.Text.Remove(textfield_num.Text.Length - 1, 1);
                //Parse theo decimal
                int _demicalCount = int.Parse(element.DataSource);
                var custValue = double.Parse(text_num, cul).ToString("N" + ((_demicalCount > 0) ? _demicalCount.ToString() : "0"), cul);
                element.Value = custValue;
            }
            else
                element.Value = text_num;

            if (parent.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parent as RequestDetailsV2;
                requestDetailsV2.HandleEditNumber(element);

            }
            else if (parent.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty Parentview = parent as FormWFDetailsProperty;
                Parentview.HandleEditNumber(element);
            }

            this.NavigationController.PopViewController(true);
        }

        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        #endregion
    }
}

