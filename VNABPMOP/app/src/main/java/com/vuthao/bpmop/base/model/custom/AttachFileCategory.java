package com.vuthao.bpmop.base.model.custom;

public class AttachFileCategory {
    private int ID;
    private int DocumentTypeID;
    private String Title;
    private String DocumentTypeValue;
    private boolean Required;
    private boolean ExportQR;
    private boolean Signature;
    private boolean IsSelected;

    public AttachFileCategory() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public int getDocumentTypeID() {
        return DocumentTypeID;
    }

    public void setDocumentTypeID(int documentTypeID) {
        DocumentTypeID = documentTypeID;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getDocumentTypeValue() {
        return DocumentTypeValue;
    }

    public void setDocumentTypeValue(String documentTypeValue) {
        DocumentTypeValue = documentTypeValue;
    }

    public boolean isRequired() {
        return Required;
    }

    public void setRequired(boolean required) {
        Required = required;
    }

    public boolean isExportQR() {
        return ExportQR;
    }

    public void setExportQR(boolean exportQR) {
        ExportQR = exportQR;
    }

    public boolean isSignature() {
        return Signature;
    }

    public void setSignature(boolean signature) {
        Signature = signature;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }
}
