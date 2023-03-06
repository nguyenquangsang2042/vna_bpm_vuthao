using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using Newtonsoft.Json.Linq;

namespace BPMOPMobile.Droid.Core.Component
{
    public class TemplateValueType : ComponentBase
    {
        public ViewElement _elementParent { get; set; } // Element của Control Grid
        public ViewElement _elementChild { get; set; } // Element child của Item dc click vào
        public LinearLayout _parentView { get; set; }
        public Activity _mainAct;
        public ControlBase control;
        public int _flagView = -1; // để xác định xem là view nào
        public bool _visibleItemLine; // true = hiện Line từng item, false = hiện line của nguyên row
        public JObject _JObjectChild;

        public TemplateValueType(Activity _mainAct, LinearLayout _parentView, ViewElement _elementParent, ViewElement _elementChild, JObject _JObjectChild, bool _visibleItemLine, int _flagView)
        {
            this._mainAct = _mainAct;
            this._parentView = _parentView;
            this._elementParent = _elementParent;
            this._elementChild = _elementChild;
            this._JObjectChild = _JObjectChild;
            this._visibleItemLine = _visibleItemLine;
            this._flagView = _flagView;
            InitializeComponent();
        }
        public override void InitializeCategory(int Category = (int)EnumDynamicControlCategory.TemplateValue)
        {
            base.InitializeCategory(Category);
        }
        public override void InitializeComponent()
        {
            switch (_elementChild.DataType) //_elementChild.InternalName
            {
                case "selectuser":
                case "selectusergroup":
                    control = new ControlSelectUser(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "selectusermulti":
                case "selectusergroupmulti":
                    control = new ControlMultiSelectUser(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "date":
                case "datetime":
                case "time":
                    control = new TemplateValueType_ControlDate(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "singlechoice":
                    control = new ControlSingleChoice(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "multiplechoice":
                    control = new ControlMultiChoice(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "singlelookup":
                    control = new ControlSingleLookup(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "multiplelookup":
                    control = new ControlMultiLookup(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "number":
                    control = new TemplateValueType_ControlNumber(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "yesno":
                    control = new ControlYesNo(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "textinput":
                    control = new ControlTextInput(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                case "textinputmultiline":
                    control = new ControlTextInputMultiLine(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
                    break;
                //case "textinputformat": // Text Editor
                //    control = new ControlTextInputFormat(_mainAct, _parentView, _element);
                //    break;
                case "text":
                default:
                    control = new TemplateValueType_ControlText(_mainAct, _parentView, _elementParent, _elementChild, _JObjectChild, _flagView);
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