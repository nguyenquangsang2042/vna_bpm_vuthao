using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
//using BPMOPMobileV1.Class;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlSingleChoice: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        public ControlSingleChoice(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            if (BT_action != null)
                BT_action.AddTarget(HandleTouchDown, UIControlEvent.TouchUpInside);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);
        }

        protected virtual void HandleTouchDown(object sender, EventArgs e)
        {
            //if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            //{
            //    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
            //    controller.NavigatorToChoiceView(element, indexPath, this);
            //}
            if (element.Enable)
            {
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorToChoiceView(element, indexPath, this);
                }
            }
            else 
            {
                if (!string.IsNullOrEmpty(lbl_value.Text))
                {
                    var flagcheckassignName = CmmIOSFunction.CheckStringTrunCated(lbl_value);
                    if (!flagcheckassignName)
                        return;
                }
                else
                    return;
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.NavigatorTouchMoreToFullTextView(element.Title, lbl_value.Text);
                }
            }
        }

        public override void SetProprety()
        {
            if (element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(lbl_value, item.Key, item.Value);
                }
            }
        }

        public override void SetEnable()
        {
            base.SetEnable();

            if (element.Enable)
            {
                if (string.IsNullOrEmpty(Value))
                {
                    lbl_value.Text = "Chọn...";
                    lbl_value.Font = UIFont.FromName("Arial-ItalicMT", 11);
                }

                BT_action.UserInteractionEnabled = true;
                lbl_value.TextColor = UIColor.FromRGB(51, 95, 179);
            }
            ////else
            ////    BT_action.UserInteractionEnabled = false;
            //else
            //{
            //    if (!string.IsNullOrEmpty(Value))
            //    {
            //        var flagcheckassignName = CmmIOSFunction.CheckStringTrunCated(lbl_value);
            //        if (flagcheckassignName)
            //        {
            //            BT_action.UserInteractionEnabled = true;
            //            BT_action.Enabled = true;
            //        }
            //        else
            //        {
            //            BT_action.UserInteractionEnabled = false;
            //            BT_action.Enabled = false;
            //        }
            //    }
            //    else
            //        BT_action.UserInteractionEnabled = false;
            //}
        }

        public override void SetTitle()
        {
            base.SetTitle();

            lbl_title.Text = element.Title;
            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override string Value
        {
            set 
            {
                var data = value.Trim();
                string result;
                if (data.Contains(";#"))
                    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                else if (data.Contains("[{")) // cau truc json
                {
                    string str_value = string.Empty;
                    List<BeanDataLookup> lst_user = new List<BeanDataLookup>();
                    lst_user = JsonConvert.DeserializeObject<List<BeanDataLookup>>(data);

                    if (lst_user != null && lst_user.Count > 0)
                    {
                        foreach (var item in lst_user)
                        {
                            str_value = str_value + item.Title + "; ";
                        }
                    }

                    result = str_value.TrimEnd(';', ' ');
                }
                else if (data.Contains("[]"))
                    result = string.Empty;
                else
                    result = data;

                this.lbl_value.Text = result;
                //var data = value.Trim();
                //string result;
                //if (data.Contains(";#"))
                //    result = data.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                //else
                //    result = data;

                //this.lbl_value.Text = result;
            }
            get { return this.lbl_value.Text; }
        }

        public override void SetValue()
        {
            base.SetValue();

            Value = element.Value;
        }
    }
}