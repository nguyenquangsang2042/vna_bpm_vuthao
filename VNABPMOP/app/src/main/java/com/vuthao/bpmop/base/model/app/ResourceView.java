package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;

public class ResourceView extends RealmObject {
    private int ID;
    private int ResourceId; // map với WorkflowID của BeanWorkflow
    private int TypeId;
    private String Title;
    private String TitleEN;
    private int Index;
    private int Status;
    private String OptionChart;
    private boolean ExportPDF;
    private boolean ExportExcel;
    private int ExportLimit;
    private String Created;
    private String Modified;
    private String CreatedBy;
    private String ModifiedBy;
    private String Image;
    private String ListId;
    private int ResourceCategoryId;
    private int ResourceSubCategoryId;
    private int DataPermission;
    private boolean DefaultFilter;
    private int MenuId;
    private String Description;
    private String QuerySQLSelect;
    private String QuerySQLSorting;
    private String QuerySQLWhere;
    private String QuerySQLWherePara;
    private String QuerySQLWhereParaAdv;
    private int ViewType;

    public ResourceView() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public int getResourceId() {
        return ResourceId;
    }

    public void setResourceId(int resourceId) {
        ResourceId = resourceId;
    }

    public int getTypeId() {
        return TypeId;
    }

    public void setTypeId(int typeId) {
        TypeId = typeId;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getTitleEN() {
        return TitleEN;
    }

    public void setTitleEN(String titleEN) {
        TitleEN = titleEN;
    }

    public int getIndex() {
        return Index;
    }

    public void setIndex(int index) {
        Index = index;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }

    public String getOptionChart() {
        return OptionChart;
    }

    public void setOptionChart(String optionChart) {
        OptionChart = optionChart;
    }

    public boolean isExportPDF() {
        return ExportPDF;
    }

    public void setExportPDF(boolean exportPDF) {
        ExportPDF = exportPDF;
    }

    public boolean isExportExcel() {
        return ExportExcel;
    }

    public void setExportExcel(boolean exportExcel) {
        ExportExcel = exportExcel;
    }

    public int getExportLimit() {
        return ExportLimit;
    }

    public void setExportLimit(int exportLimit) {
        ExportLimit = exportLimit;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getCreatedBy() {
        return CreatedBy;
    }

    public void setCreatedBy(String createdBy) {
        CreatedBy = createdBy;
    }

    public String getModifiedBy() {
        return ModifiedBy;
    }

    public void setModifiedBy(String modifiedBy) {
        ModifiedBy = modifiedBy;
    }

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public String getListId() {
        return ListId;
    }

    public void setListId(String listId) {
        ListId = listId;
    }

    public int getResourceCategoryId() {
        return ResourceCategoryId;
    }

    public void setResourceCategoryId(int resourceCategoryId) {
        ResourceCategoryId = resourceCategoryId;
    }

    public int getResourceSubCategoryId() {
        return ResourceSubCategoryId;
    }

    public void setResourceSubCategoryId(int resourceSubCategoryId) {
        ResourceSubCategoryId = resourceSubCategoryId;
    }

    public int getDataPermission() {
        return DataPermission;
    }

    public void setDataPermission(int dataPermission) {
        DataPermission = dataPermission;
    }

    public boolean isDefaultFilter() {
        return DefaultFilter;
    }

    public void setDefaultFilter(boolean defaultFilter) {
        DefaultFilter = defaultFilter;
    }

    public int getMenuId() {
        return MenuId;
    }

    public void setMenuId(int menuId) {
        MenuId = menuId;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public String getQuerySQLSelect() {
        return QuerySQLSelect;
    }

    public void setQuerySQLSelect(String querySQLSelect) {
        QuerySQLSelect = querySQLSelect;
    }

    public String getQuerySQLSorting() {
        return QuerySQLSorting;
    }

    public void setQuerySQLSorting(String querySQLSorting) {
        QuerySQLSorting = querySQLSorting;
    }

    public String getQuerySQLWhere() {
        return QuerySQLWhere;
    }

    public void setQuerySQLWhere(String querySQLWhere) {
        QuerySQLWhere = querySQLWhere;
    }

    public String getQuerySQLWherePara() {
        return QuerySQLWherePara;
    }

    public void setQuerySQLWherePara(String querySQLWherePara) {
        QuerySQLWherePara = querySQLWherePara;
    }

    public String getQuerySQLWhereParaAdv() {
        return QuerySQLWhereParaAdv;
    }

    public void setQuerySQLWhereParaAdv(String querySQLWhereParaAdv) {
        QuerySQLWhereParaAdv = querySQLWhereParaAdv;
    }

    public int getViewType() {
        return ViewType;
    }

    public void setViewType(int viewType) {
        ViewType = viewType;
    }
}
