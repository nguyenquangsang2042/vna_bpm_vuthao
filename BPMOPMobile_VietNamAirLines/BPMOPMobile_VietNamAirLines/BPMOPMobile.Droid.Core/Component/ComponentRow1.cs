using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Class;

namespace BPMOPMobile.Droid.Core.Component
{
    public class ComponentRow1 : ComponentBase
    {
        public ViewElement _element { get; set; }
        public LinearLayout _parentView { get; set; }
        public Activity _mainAct;
        public ControlBase control;
        public int _widthScreenTablet = -1;
        public int _flagView = -1;
        public bool _visibleItemLine; // true = hiện Line từng item, false = hiện line của nguyên row
        public ComponentRow1(Activity _mainAct, LinearLayout _parentView, ViewElement _element, int _widthScreenTablet, bool _visibleItemLine, int _flagView)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._element = _element;
            this._widthScreenTablet = _widthScreenTablet;
            this._visibleItemLine = _visibleItemLine;
            this._flagView = _flagView;
            InitializeComponent();
        }
        public override void InitializeComponent()
        {
            switch (_element.DataType)
            {
                case "selectuser":
                case "selectusergroup":
                    control = new ControlSelectUser(_mainAct, _parentView, _element);
                    break;
                case "selectusermulti":
                case "selectusergroupmulti":
                    control = new ControlMultiSelectUser(_mainAct, _parentView, _element);
                    break;
                case "date":
                case "datetime":
                case "time":
                    control = new ControlDate(_mainAct, _parentView, _element);
                    break;
                case "singlechoice":
                    control = new ControlSingleChoice(_mainAct, _parentView, _element);
                    break;
                case "multiplechoice":
                    control = new ControlMultiChoice(_mainAct, _parentView, _element);
                    break;
                case "singlelookup":
                    control = new ControlSingleLookup(_mainAct, _parentView, _element);
                    break;
                case "multiplelookup":
                    control = new ControlMultiLookup(_mainAct, _parentView, _element);
                    break;
                case "number":
                    control = new ControlNumber(_mainAct, _parentView, _element);
                    break;
                case "tabs":
                    control = new ControlTabs(_mainAct, _parentView, _element, _widthScreenTablet);
                    break;
                case "attachment":
                    control = new ControlAttachment(_mainAct, _parentView, _element, _widthScreenTablet);
                    break;
                case "attachmentvertical":
                    control = new ControlAttachmentVertical(_mainAct, _parentView, _element);
                    break;
                case "yesno":
                    control = new ControlYesNo(_mainAct, _parentView, _element);
                    break;
                case "tree":
                    control = new ControlTree(_mainAct, _parentView, _element);
                    break;
                case "attachmentverticalformframe": // chưa có
                    //control = new ControlAttachmentVerticalWithFormFrame(parentView, element, currentIndexPath);
                    break;
                case "textinput":
                    control = new ControlTextInput(_mainAct, _parentView, _element);
                    break;
                case "textinputmultiline":
                    control = new ControlTextInputMultiLine(_mainAct, _parentView, _element);
                    break;
                case "textinputformat": // Text Editor
                    control = new ControlTextInputFormat(_mainAct, _parentView, _element);
                    break;
                case "inputattachmenthorizon":
                //control = new ControlInputAttachmentHorizontal(_mainAct, _parentView, _element, _widthScreenTablet);
                //break;
                case "inputattachmentvertical": // testting
                    control = new ControlInputAttachmentVertical(_mainAct, _parentView, _element, _widthScreenTablet, (int)EnumFormControlView.FlagViewControlAttachment.DetailWorkflow);
                    break;
                case "inputworkrelated": // quy trình liên kết
                    control = new ControlInputWorkRelated(_mainAct, _parentView, _element, _widthScreenTablet);
                    break;
                case "inputgriddetails":
                    control = new ControlInputGridDetails(_mainAct, _parentView, _element, _flagView);
                    break;
                default:
                    control = new ControlText(_mainAct, _parentView, _element);
                    break;
            }
        }
        public override void InitializeFrameView(LinearLayout frame)
        {
            if (control == null)
                return;

            base.InitializeFrameView(frame);
            control.InitializeFrameView(frame);

            if (_visibleItemLine == false) // Ẩn Line của Item -> Load Line của nguyên Row
            {
                control._lnLine.Visibility = ViewStates.Gone;
            }
            else
            {
                control._lnLine.Visibility = ViewStates.Visible;
            }
        }
        public override void SetTitle()
        {
            if (control == null)
                return;

            base.SetTitle();
            control.SetTitle();
        }
        public override void SetValue()
        {
            if (control == null)
                return;

            base.SetValue();
            control.SetValue();
        }
        public override void SetProprety()
        {
            if (control == null)
                return;

            base.SetProprety();
            control.SetProprety();
        }
        public override void SetEnable()
        {
            if (control == null)
                return;

            base.SetEnable();
            control.SetEnable();
        }
    }
}