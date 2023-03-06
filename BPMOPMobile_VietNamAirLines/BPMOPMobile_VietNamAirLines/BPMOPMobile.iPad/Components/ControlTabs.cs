using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlTabs: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }
        UISegmentedControl segmentedControl;
        List<KeyValuePair<string, string>> lst_data = new List<KeyValuePair<string, string>>() { };

        /// <summary>
        /// Yêu cầu Data source từ 2 - 3 element.
        /// </summary>
        /// <param name="_parentView"></param>
        /// <param name="_element"></param>
        /// <param name="_indexPath"></param>
        public ControlTabs(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            var selectedValue = element.Value.Split(new string[] { ";#" }, StringSplitOptions.None)[1].ToLower();
            var arrayValue = element.DataSource.Split(new string[] { ";#" }, StringSplitOptions.None);
            int selectedIndex = 0;
            if (arrayValue.Length > 2)
            {
                string[] arraySetValue = new string[arrayValue.Length / 2];
                int index = 0;
                for (var i = 0; i < arrayValue.Length; i += 2)
                {
                    lst_data.Add(new KeyValuePair<string, string>(arrayValue[i], arrayValue[i + 1]));
                    arraySetValue[index] = arrayValue[i + 1];
                    if (selectedValue == arrayValue[i + 1].ToLower())
                        selectedIndex = index;

                    index++;
                }

                segmentedControl = new UISegmentedControl(arraySetValue);
                segmentedControl.SelectedSegment = selectedIndex;
            }
            else
                segmentedControl = new UISegmentedControl();

            this.Add(segmentedControl);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);

            segmentedControl.AddTarget(HandleChangeValue, UIControlEvent.ValueChanged);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;

            var height = frame.Height - ((paddingTop * 2) + spaceView);

            lbl_title.TranslatesAutoresizingMaskIntoConstraints = false;
            lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
            segmentedControl.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;

            segmentedControl.HeightAnchor.ConstraintEqualTo(height - 20).Active = true;
            NSLayoutConstraint.Create(this.segmentedControl, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_title, NSLayoutAttribute.Bottom, 1.0f, spaceView).Active = true;
            NSLayoutConstraint.Create(this.segmentedControl, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(this.segmentedControl, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
        }

        public override void SetProprety()
        {
            if (element.ListProprety != null)
            {
                foreach (var item in element.ListProprety)
                {
                    CmmIOSFunction.SetPropertyValueByNameCustom(segmentedControl, item.Key, item.Value);
                }
            }
        }

        public override void SetEnable()
        {
            segmentedControl.Enabled = element.Enable;
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override string Value
        {
            set 
            {
                var selectedIndex = Convert.ToInt32(value);
                segmentedControl.SelectedSegment = selectedIndex;
            }
            get { return segmentedControl.SelectedSegment.ToString(); }
        }

        public override void SetValue(){ }

        protected virtual void HandleChangeValue(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                var selectedIndex = lst_data[Convert.ToInt32(Value)];
                element.Value = selectedIndex.Key + ";#" + selectedIndex.Value;
            }
        }
    }
}