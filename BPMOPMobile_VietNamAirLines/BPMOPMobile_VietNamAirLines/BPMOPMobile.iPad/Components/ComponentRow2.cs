using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ComponentRow2: ComponentBase
    {
        UIViewController parentView { get; set; }
        ViewRow control { get; set; }
        NSIndexPath currentIndexPath { get; set; }
        public ComponentRow1 row1, row2;

        public ComponentRow2(UIViewController _parentView, ViewRow _control, NSIndexPath _currentIndexPath)
        {
            parentView = _parentView;
            control = _control;
            currentIndexPath = _currentIndexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            row1 = new ComponentRow1(parentView, control.Elements[0], currentIndexPath);
            row2 = new ComponentRow1(parentView, control.Elements[1], currentIndexPath);

            this.Add(row1);
            this.Add(row2);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);
            var widthRow = (frame.Width / 2) - 5;

            row1.InitializeFrameView(new CGRect(0, 0, widthRow, frame.Height));
            row2.InitializeFrameView(new CGRect(widthRow + 10, 0, widthRow, frame.Height));
        }

        public override void SetTitle()
        {
            row1.SetTitle();
            row2.SetTitle();
        }

        public override void SetValue()
        {
            base.SetValue();

            row1.SetValue();
            row2.SetValue();
        }

        public override void SetProprety()
        {
            row1.SetProprety();
            row2.SetProprety();
        }

        public override void SetEnable()
        {
            base.SetEnable();

            row1.SetEnable();
            row2.SetEnable();
        }

        public override void SetRequire()
        {
            base.SetRequire();

            row1.SetRequire();
            row2.SetRequire();
        }
    }
}