using System;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    public class ComponentWorkFlowRelate : ComponentBase
    {
        ViewElement element { get; set; }
        UIViewController parentView { get; set; }
        NSIndexPath currentIndexPath { get; set; }

        public ControlBase control;

        public ComponentWorkFlowRelate(UIViewController _parentView, ViewElement _element, NSIndexPath _currentIndexPath)
        {
            parentView = _parentView;
            element = _element;
            currentIndexPath = _currentIndexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            control = new ControlInputWorkRelated(parentView, element, currentIndexPath);
            this.Add(control);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);

            control.InitializeFrameView(new CGRect(0, 0, frame.Width, frame.Height));
        }

        public override void SetTitle()
        {
            base.SetTitle();

            control.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            control.SetValue();
        }

        public override void SetProprety()
        {
            base.SetProprety();

            control.SetProprety();
        }

        public override void SetEnable()
        {
            base.SetEnable();

            control.SetEnable();
        }
    }
}
