package com.vuthao.bpmop.base.model.custom;

public class WFDetailsHeader {
    private String title;
    private String internalName;
    private String fieldID;
    private String field;
    private boolean allowSort;
    private boolean allowFilter;
    private boolean allowGroup;
    private boolean hidden;
    private String kendoFieldType;
    private String formula;
    private boolean isSum;
    private String dataType;
    private boolean viewOnly;
    private boolean require;
    private String titleEN;
    private String dataSource;
    private int fieldIDInt;     // giống FieldID nhưng là dạng int
    private int FieldTypeId;    // để xác định xem là control nào: 1 2 4 8 9 -> textbox.
    private String FieldMapping;// dùng để map, nếu rỗng thì dùng internal name map
    private String Option;      // Bao gồm Formular, dataSource, ...
    private float EstWidth;     // Chieu rong contet cua header

    public WFDetailsHeader() {
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        title = title;
    }

    public String getInternalName() {
        return internalName;
    }

    public void setInternalName(String internalName) {
        this.internalName = internalName;
    }

    public String getFieldID() {
        return fieldID;
    }

    public void setFieldID(String fieldID) {
        this.fieldID = fieldID;
    }

    public String getField() {
        return field;
    }

    public void setField(String field) {
        this.field = field;
    }

    public boolean isAllowSort() {
        return allowSort;
    }

    public void setAllowSort(boolean allowSort) {
        this.allowSort = allowSort;
    }

    public boolean isAllowFilter() {
        return allowFilter;
    }

    public void setAllowFilter(boolean allowFilter) {
        this.allowFilter = allowFilter;
    }

    public boolean isAllowGroup() {
        return allowGroup;
    }

    public void setAllowGroup(boolean allowGroup) {
        this.allowGroup = allowGroup;
    }

    public boolean isHidden() {
        return hidden;
    }

    public void setHidden(boolean hidden) {
        this.hidden = hidden;
    }

    public String getKendoFieldType() {
        return kendoFieldType;
    }

    public void setKendoFieldType(String kendoFieldType) {
        this.kendoFieldType = kendoFieldType;
    }

    public String getFormula() {
        return formula;
    }

    public void setFormula(String formula) {
        this.formula = formula;
    }

    public boolean isSum() {
        return isSum;
    }

    public void setSum(boolean sum) {
        isSum = sum;
    }

    public String getDataType() {
        return dataType;
    }

    public void setDataType(String dataType) {
        this.dataType = dataType;
    }

    public boolean isViewOnly() {
        return viewOnly;
    }

    public void setViewOnly(boolean viewOnly) {
        this.viewOnly = viewOnly;
    }

    public boolean isRequire() {
        return require;
    }

    public void setRequire(boolean require) {
        this.require = require;
    }

    public String getDataSource() {
        return dataSource;
    }

    public void setDataSource(String dataSource) {
        dataSource = dataSource;
    }

    public int getFieldIDInt() {
        return fieldIDInt;
    }

    public void setFieldIDInt(int fieldIDInt) {
        this.fieldIDInt = fieldIDInt;
    }

    public int getFieldTypeId() {
        return FieldTypeId;
    }

    public void setFieldTypeId(int fieldTypeId) {
        FieldTypeId = fieldTypeId;
    }

    public String getFieldMapping() {
        return FieldMapping;
    }

    public void setFieldMapping(String fieldMapping) {
        FieldMapping = fieldMapping;
    }

    public String getOption() {
        return Option;
    }

    public void setOption(String option) {
        Option = option;
    }

    public float getEstWidth() {
        return EstWidth;
    }

    public void setEstWidth(float estWidth) {
        EstWidth = estWidth;
    }

    public String getTitleEN() {
        return titleEN;
    }

    public void setTitleEN(String titleEN) {
        this.titleEN = titleEN;
    }
}
