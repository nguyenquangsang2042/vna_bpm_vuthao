package com.vuthao.bpmop.core.component;

import android.widget.LinearLayout;

public class ComponentBase {
    private String Value;
    private LinearLayout Frame;
    private int Category;

    public int getCategory() {
        return Category;
    }

    public void setCategory(int category) {
        Category = category;
    }

    public void initializeCategory(int Category) {
        this.Category = Category;
    }

    public void initializeFrameView(LinearLayout frame) {
        this.Frame = frame;
    }

    public void initializeComponent() { }

    public void setTitle() {
    }

    /// <summary>
    /// Gán giá trị cho component
    /// </summary>
    public void setValue() {
    }

    /// <summary>
    /// Gán các thuộc tính cơ bản cho element
    /// </summary>
    public void setProprety() {
    }

    /// <summary>
    /// Bật tắt action cho component
    /// </summary>
    public void setEnable() {
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }

    public LinearLayout getFrame() {
        return Frame;
    }

    public void setFrame(LinearLayout frame) {
        Frame = frame;
    }
}
