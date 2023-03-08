package com.vuthao.bpmop.base.model.dynamic;

import com.vuthao.bpmop.base.model.custom.ObjectElementNote;

import java.util.ArrayList;
import java.util.HashMap;

public class ViewElement extends ViewBase {
    private String DataType;
    private String DataSource;
    private String TypeSP;
    private String InternalName;
    private String Formula;
    private boolean Hidden;
    private ArrayList<ObjectElementNote> Notes;
    private ArrayList<HashMap<String, String>> ListProprety;

    public ViewElement() {
    }

    public String getDataType() {
        return DataType;
    }

    public void setDataType(String dataType) {
        DataType = dataType;
    }

    public String getDataSource() {
        return DataSource;
    }

    public void setDataSource(String dataSource) {
        DataSource = dataSource;
    }

    public String getTypeSP() {
        return TypeSP;
    }

    public void setTypeSP(String typeSP) {
        TypeSP = typeSP;
    }

    public String getInternalName() {
        return InternalName;
    }

    public void setInternalName(String internalName) {
        InternalName = internalName;
    }

    public String getFormula() {
        return Formula;
    }

    public void setFormula(String formula) {
        Formula = formula;
    }

    public boolean isHidden() {
        return Hidden;
    }

    public void setHidden(boolean hidden) {
        Hidden = hidden;
    }

    public ArrayList<ObjectElementNote> getNotes() {
        return Notes;
    }

    public void setNotes(ArrayList<ObjectElementNote> notes) {
        Notes = notes;
    }

    public ArrayList<HashMap<String, String>> getListProprety() {
        return ListProprety;
    }

    public void setListProprety(ArrayList<HashMap<String, String>> listProprety) {
        ListProprety = listProprety;
    }
}
