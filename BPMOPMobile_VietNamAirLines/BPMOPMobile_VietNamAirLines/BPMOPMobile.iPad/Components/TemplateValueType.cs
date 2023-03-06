using System;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    public class TemplateValueType : ComponentBase
    {
        ViewElement element { get; set; }
        UIViewController parentView { get; set; }
        NSIndexPath currentIndexPath { get; set; }

        public ControlBase control;

        public TemplateValueType(UIViewController _parentView, ViewElement _element, NSIndexPath _currentIndexPath)
        {
            parentView = _parentView;
            element = _element;
            currentIndexPath = _currentIndexPath;
            InitializeComponent();

        }
        public override void InitializeComponent()
        {
            switch (element.DataType)
            {
                case "textinput":
                    control = new ControlText(parentView, element, currentIndexPath);
                    break;
                case "number":
                    control = new ControlNumber(parentView, element, currentIndexPath);
                    break;
                case "date":
                case "datetime":
                case "time":
                    control = new ControlDate(parentView, element, currentIndexPath);
                    break;
                case "singlechoice":
                case "multiplechoice":
                    control = new ControlItemsChoice(parentView, element, currentIndexPath);
                    break;
                case "yesno":
                    control = new ControlYesNo(parentView, element, currentIndexPath);
                    break;
                case "textinputmultiline":
                    control = new ControlTextInputMultiLine(parentView, element, currentIndexPath);
                    break;
                default:
                    control = new ControlText(parentView, null, currentIndexPath);
                    break;
            }
            if (control != null)
                this.Add(control);

            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                CmmIOSFunction.UpdateScrollTableView(control, controller.GetTableView);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            base.InitializeFrameView(frame);

            if (control != null)
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

        public override void SetRequire()
        {
            base.SetRequire();
            control.SetRequire();
        }
    }
}
