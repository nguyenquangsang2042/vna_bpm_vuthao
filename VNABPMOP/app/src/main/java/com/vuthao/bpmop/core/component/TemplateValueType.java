package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.view.View;
import android.widget.LinearLayout;

import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.core.Vars;

import org.json.JSONObject;

public class TemplateValueType extends ComponentBase {
    private final ViewElement elementParent; // Element của Control Grid
    private final ViewElement elementChild; // Element child của Item dc click vào
    private final LinearLayout parentView;
    private final Activity mainAct;
    private ControlBase control;
    private int flagView = -1; // để xác định xem là view nào
    private final boolean visibleItemLine; // true = hiện Line từng item, false = hiện line của nguyên row
    private final JSONObject JObjectChild;

    public TemplateValueType(Activity mainAct, LinearLayout parentView, ViewElement elementParent, ViewElement elementChild, JSONObject JObjectChild, boolean visibleItemLine, int flagView) {
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.elementParent = elementParent;
        this.elementChild = elementChild;
        this.JObjectChild = JObjectChild;
        this.visibleItemLine = visibleItemLine;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeCategory(int Category) {
        Category = Vars.EnumDynamicControlCategory.TemplateValue;
        super.initializeCategory(Category);
    }

    @Override
    public void initializeComponent() {
        switch (elementChild.getDataType()) {
            case VarsControl.SELECTUSER:
            case VarsControl.SELECTUSERGROUP:
                control = new ControlSelectUser(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.SELECTUSERMULTI:
            case VarsControl.SELECTUSERGROUPMULTI:
                control = new ControlMultiSelectUser(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.DATE:
            case VarsControl.DATETIME:
            case VarsControl.TIME:
                control = new TemplateValueType_ControlDate(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.SINGLECHOICE:
                control = new ControlSingleChoice(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.MULTIPLECHOICE:
                control = new ControlMultiChoice(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.SINGLELOOKUP:
                control = new ControlSingleLookup(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.MULTIPLELOOKUP:
                control = new ControlMultiLookup(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.NUMBER:
                control = new TemplateValueType_ControlNumber(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.YESNO:
                control = new ControlYesNo(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.TEXTINPUT:
                control = new ControlTextInput(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.TEXTINPUTMULTILINE:
                control = new ControlTextInputMultiLine(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
            case VarsControl.TEXT:
            default:
                control = new TemplateValueType_ControlText(mainAct, parentView, elementParent, elementChild, JObjectChild, flagView);
                break;
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (control == null) {
            return;
        }
        super.initializeFrameView(frame);
        control.initializeFrameView(frame);

        if (visibleItemLine) {
            control.lnLine.setVisibility(View.VISIBLE);
        } else {
            control.lnLine.setVisibility(View.GONE);
        }
    }

    @Override
    public void setTitle() {
        if (control == null) {
            return;
        }
        super.setTitle();
        control.setTitle();
    }

    @Override
    public void setValue() {
        if (control == null) {
            return;
        }
        super.setValue();
        control.setValue();
    }

    @Override
    public void setProprety() {
        if (control == null) {
            return;
        }
        super.setProprety();
        control.setProprety();
    }

    @Override
    public void setEnable() {
        if (control == null) {
            return;
        }
        super.setEnable();
        control.setEnable();
    }
}
