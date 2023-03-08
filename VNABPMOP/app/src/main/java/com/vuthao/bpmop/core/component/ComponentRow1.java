package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.view.View;
import android.widget.LinearLayout;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.core.Vars;

public class ComponentRow1 extends ComponentBase {
    protected ViewElement element;
    protected LinearLayout parentView;
    protected Activity mainAct;
    protected ControlBase control;
    protected int widthScreenTablet = -1;
    protected int flagView = -1;
    protected boolean visibleItemLine; // true = hiện Line từng item, false = hiện line của nguyên row

    public ComponentRow1() {

    }

    public ComponentRow1(Activity mainAct, LinearLayout parentView, ViewElement element, int widthScreenTablet, boolean visibleItemLine, int flagView) {
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.element = element;
        this.widthScreenTablet = widthScreenTablet;
        this.visibleItemLine = visibleItemLine;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();
        switch (element.getDataType()) {
            case VarsControl.SELECTUSER:
            case VarsControl.SELECTUSERGROUP: {
                control = new ControlSelectUser(mainAct, parentView, element);
                break;
            }
            case VarsControl.SELECTUSERMULTI:
            case VarsControl.SELECTUSERGROUPMULTI: {
                control = new ControlMultiSelectUser(mainAct, parentView, element);
                break;
            }
            case VarsControl.DATE:
            case VarsControl.DATETIME:
            case VarsControl.TIME: {
                control = new ControlDate(mainAct, parentView, element);
                break;
            }
            case VarsControl.SINGLECHOICE: {
                control = new ControlSingleChoice(mainAct, parentView, element);
                break;
            }
            case VarsControl.MULTIPLECHOICE: {
                control = new ControlMultiChoice(mainAct, parentView, element);
                break;
            }
            case VarsControl.SINGLELOOKUP: {
                control = new ControlSingleLookup(mainAct, parentView, element);
                break;
            }
            case VarsControl.MULTIPLELOOKUP: {
                control = new ControlMultiLookup(mainAct, parentView, element);
                break;
            }
            case VarsControl.NUMBER: {
                control = new ControlNumber(mainAct, parentView, element);
                break;
            }
            case VarsControl.TABS: {
                control = new ControlTabs(mainAct, parentView, element, widthScreenTablet);
                break;
            }
            case VarsControl.ATTACHMENT: {
                control = new ControlAttachment(mainAct, parentView, element, widthScreenTablet);
                break;
            }
            case VarsControl.ATTACHMENTVERTICAL: {
                control = new ControlAttachmentVertical(mainAct, parentView, element);
                break;
            }
            case VarsControl.YESNO: {
                control = new ControlYesNo(mainAct, parentView, element);
                break;
            }
            case VarsControl.TREE: {
                control = new ControlTree(mainAct, parentView, element);
                break;
            }
            // Chưa có
            case VarsControl.ATTACHMENTVERTICALFORMFRAME: {
                break;
            }
            case VarsControl.TEXTINPUT: {
                control = new ControlTextInput(mainAct, parentView, element);
                break;
            }
            case VarsControl.TEXTINPUTMULTILINE: {
                control = new ControlTextInputMultiLine(mainAct, parentView, element);
                break;
            }
            case VarsControl.TEXTINPUTFORMAT: {
                control = new ControlTextInputFormat(mainAct, parentView, element);
                break;
            }
            case VarsControl.INPUTATTACHMENTHORIZON:
            case VarsControl.INPUTATTACHMENTVERTICAL: {
                control = new ControlInputAttachmentVertical(mainAct, parentView, element, widthScreenTablet, (int) Vars.FlagViewControlAttachment.DetailWorkflow);
                break;
            }
            case VarsControl.INPUTWORKRELATED: {
                control = new ControlInputWorkRelated(mainAct, parentView, element, widthScreenTablet);
                break;
            }
            case VarsControl.INPUTGRIDDETAILS: {
                if (!Functions.isNullOrEmpty(element.getDataSource())) {
                    control = new ControlInputGridDetails(mainAct, parentView, element, flagView);
                }
                break;
            }
            default:
                control = new ControlText(mainAct, parentView, element);
                break;
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (control == null) return;
        super.initializeFrameView(frame);

        control.initializeFrameView(frame);

        if (!visibleItemLine) {
            control.lnLine.setVisibility(View.GONE);
        } else {
            control.lnLine.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void setTitle() {
        if (control == null) return;
        super.setTitle();

        control.setTitle();
    }

    @Override
    public void setValue() {
        if (control == null) return;
        super.setValue();

        control.setValue();
    }

    @Override
    public void setProprety() {
        if (control == null) return;
        super.setProprety();

        control.setProprety();
    }

    @Override
    public void setEnable() {
        if (control == null) return;
        super.setEnable();

        control.setEnable();
    }
}
