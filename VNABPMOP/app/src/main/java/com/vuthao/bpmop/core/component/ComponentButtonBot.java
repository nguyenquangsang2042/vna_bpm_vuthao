package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.res.Resources;
import android.util.TypedValue;
import android.widget.LinearLayout;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

public class ComponentButtonBot extends ComponentBase {
    private ViewElement element;
    private final LinearLayout parentView;
    private final Activity mainAct;
    private final ViewRow controlData;
    private final Resources resource;
    private ControlButtonBot btnAction1, btnAction2, btnAction3;
    private ArrayList<ButtonAction> lstActionMore = new ArrayList<>();

    public ComponentButtonBot(Activity mainAct, LinearLayout parentView, ViewRow controlData, Resources resource) {
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.controlData = controlData;
        this.resource = resource;

        this.controlData.setElements(DetailFunc.share.sortListElementAction(this.controlData.getElements()));
        initializeComponent();
    }

    public ArrayList<ButtonAction> getLstActionMore() {
        return lstActionMore;
    }

    public void setLstActionMore(ArrayList<ButtonAction> lstActionMore) {
        this.lstActionMore = lstActionMore;
    }

    @Override
    public void initializeComponent() {
        if (controlData.getElements().size() > 0) {
            btnAction1 = new ControlButtonBot(mainAct, parentView, controlData.getElements().get(0), resource);
        }

        if (controlData.getElements().size() > 1) {
            btnAction2 = new ControlButtonBot(mainAct, parentView, controlData.getElements().get(1), resource);
        }

        if (controlData.getElements().size() > 2) {
            ViewElement elementMore = new ViewElement();
            elementMore.setID("more");
            elementMore.setValue("more");
            elementMore.setHidden(false);

            btnAction3 = new ControlButtonBot(mainAct, parentView, elementMore, resource);
            lstActionMore = new ArrayList<>();
            for (int i = 2; i < controlData.getElements().size(); i++) {
                ButtonAction action = new ButtonAction();
                action.setID(Integer.parseInt(controlData.getElements().get(i).getID()));
                action.setTitle(controlData.getElements().get(i).getTitle());
                action.setValue(controlData.getElements().get(i).getValue());

                lstActionMore.add(action);
            }
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        super.initializeFrameView(frame);

        int height = Functions.share.convertDpToPixel(35, frame.getContext());
        LinearLayout lncontent = new LinearLayout(mainAct);
        int paddingLeftRight = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 12, mainAct.getResources().getDisplayMetrics());
        lncontent.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT, 1));
        lncontent.setOrientation(LinearLayout.HORIZONTAL);
        lncontent.setPadding(paddingLeftRight, 0, paddingLeftRight, 0);

        LinearLayout lnAction1 = new LinearLayout(mainAct);
        LinearLayout lnAction2 = new LinearLayout(mainAct);
        LinearLayout lnActionMore = new LinearLayout(mainAct);

        lnAction1.setLayoutParams(new LinearLayout.LayoutParams(0, height, 0.5f));
        lnAction2.setLayoutParams(new LinearLayout.LayoutParams(0, height, 0.5f));
        lnActionMore.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, height));

        if (btnAction1 != null) {
            btnAction1.initializeFrameView(lnAction1);
        }

        if (btnAction2 != null) {
            btnAction2.initializeFrameView(lnAction2);
        }

        if (btnAction3 != null) {
            btnAction3.initializeFrameView(lnActionMore);
        }

        lncontent.addView(lnAction1);
        lncontent.addView(lnAction2);
        lncontent.addView(lnActionMore);
        frame.addView(lncontent);
    }

    @Override
    public void setTitle() {
        if (btnAction1 != null) {
            btnAction1.setTitle();
        }
        if (btnAction2 != null) {
            btnAction2.setTitle();
        }
        if (btnAction3 != null) {
            btnAction3.setTitle();
        }
    }

    @Override
    public void setValue() {
        super.setValue();

        if (btnAction1 != null) {
            btnAction1.setValue();
        }
        if (btnAction2 != null) {
            btnAction2.setValue();
        }
        if (btnAction3 != null) {
            btnAction3.setValue();
        }
    }

    @Override
    public void setProprety() {
        if (btnAction1 != null) {
            btnAction1.setProprety();
        }
        if (btnAction2 != null) {
            btnAction2.setProprety();
        }
        if (btnAction3 != null) {
            btnAction3.setProprety();
        }
    }

    @Override
    public void setEnable() {
        if (btnAction1 != null) {
            btnAction1.setEnable();
        }
        if (btnAction2 != null) {
            btnAction2.setEnable();
        }
        if (btnAction3 != null) {
            btnAction3.setEnable();
        }
    }
}
