using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ComponentRow1 : ComponentBase
    {
        ViewElement element { get; set; }
        UIViewController parentView { get; set; }
        NSIndexPath currentIndexPath { get; set; }

        public ControlBase control;

        public ComponentRow1(UIViewController _parentView, ViewElement _element, NSIndexPath _currentIndexPath)
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
                case "selectuser":
                    control = new ControlSelectUser(parentView, element, currentIndexPath);
                    break;
                case "selectusermulti":
                    control = new ControlSelectUser(parentView, element, currentIndexPath);
                    break;
                case "selectusergroup":
                    control = new ControlSelectUserOrGroup(parentView, element, currentIndexPath);
                    break;
                case "selectusergroupmulti":
                    control = new ControlSelectUserOrGroup(parentView, element, currentIndexPath);
                    break;
                case "date":
                case "datetime":
                case "time":
                    control = new ControlDate(parentView, element, currentIndexPath);
                    break;
                case "singlechoice": // value
                    control = new ControlSingleChoice(parentView, element, currentIndexPath);
                    break;
                case "multiplechoice": // value;#value;#value;#...
                    control = new ControlMultiChoice(parentView, element, currentIndexPath);
                    break;
                case "singlelookup": // ID;#value
                    control = new ControlSingleChoice(parentView, element, currentIndexPath);
                    break;
                case "multilookup": // ID;#value;#ID;#value;#...
                case "multiplelookup":
                    control = new ControlSingleChoice(parentView, element, currentIndexPath);
                    break;
                case "number":
                    control = new ControlNumber(parentView, element, currentIndexPath);
                    break;
                case "tabs":
                    control = new ControlTabs(parentView, element, currentIndexPath);
                    break;
                case "attachment":
                    control = new ControlAttachment(parentView, element, currentIndexPath);
                    break;
                case "attachmentvertical":
                    control = new ControlAttachmentVertical(parentView, element, currentIndexPath);
                    break;
                case "attachmentverticalformframe":
                    control = new ControlAttachmentVerticalWithFormFrame(parentView, element, currentIndexPath);
                    break;
                case "yesno":
                    control = new ControlYesNo(parentView, element, currentIndexPath);
                    break;
                case "tree":
                    control = new ControlTree(parentView, element, currentIndexPath);
                    break;
                case "textinput":
                    control = new ControlTextInput(parentView, element, currentIndexPath);
                    break;
                case "textinputmultiline":
                    control = new ControlTextInputMultiLine(parentView, element, currentIndexPath);
                    break;
                case "textinputformat":
                    control = new ControlTextInputFormat(parentView, element, currentIndexPath);
                    break;
                case "inputattachmenthorizon": //iphone
                    control = new ControlAttachmentVerticalWithFormFrame(parentView, element, currentIndexPath);
                    //RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    //requestDetailsV2.AttachmentRowIndexPath = currentIndexPath;
                    //control = new ControlInputAttachmentHorizon(parentView, element, currentIndexPath);
                    break;
                case "inputworkrelated":
                    control = new ControlInputWorkRelated(parentView, element, currentIndexPath);
                    break;
                //thuyngo add
                //case "inputcomments":
                //    control = new ControlInputComments(parentView, element, currentIndexPath);
                //    break;
                case "inputtasks":
                    control = new ControlInputTasks(parentView, element, currentIndexPath);
                    break;
                case "inputcomments":
                    control = new ControlInputComments(parentView, element, currentIndexPath);
                    break;
                case "inputgriddetails":
                    control = new ControlDetails(parentView, element, currentIndexPath);
                    break;
                default:
                    control = new ControlText(parentView, element, currentIndexPath);
                    break;
            }

            this.Add(control);

            //ViewDetailController controller = (ViewDetailController)parentView;
            //CmmIOSFunction.UpdateScrollTableView(control, controller.GetTableView);
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

        public override void SetRequire()
        {
            base.SetRequire();

            control.SetRequire();
        }
    }
}