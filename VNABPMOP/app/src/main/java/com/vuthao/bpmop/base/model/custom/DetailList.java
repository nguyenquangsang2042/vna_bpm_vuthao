package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;

import java.util.ArrayList;

public class DetailList extends Status {
    private ArrayList<Headers> Data;

    public DetailList() {
    }

    public ArrayList<Headers> getData() {
        return Data;
    }

    public void setData(ArrayList<Headers> data) {
        Data = data;
    }

    public static class Headers {
        private String Title;
        private String TitleEN;
        private String InternalName;
        private int FieldIDiNT;
        private String FieldID;
        private boolean AllowSort;
        private boolean AllowFilter;
        private boolean IsSum;
        private int Type;
        private String Formular;
        private int FieldTypeId;
        private String DataType;
        private String FieldMapping;
        private String Option;

        public Headers() {
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

        public String getInternalName() {
            return InternalName;
        }

        public void setInternalName(String internalName) {
            InternalName = internalName;
        }

        public int getFieldIDiNT() {
            return FieldIDiNT;
        }

        public void setFieldIDiNT(int fieldIDiNT) {
            FieldIDiNT = fieldIDiNT;
        }

        public String getFieldID() {
            return FieldID;
        }

        public void setFieldID(String fieldID) {
            FieldID = fieldID;
        }

        public boolean isAllowSort() {
            return AllowSort;
        }

        public void setAllowSort(boolean allowSort) {
            AllowSort = allowSort;
        }

        public boolean isAllowFilter() {
            return AllowFilter;
        }

        public void setAllowFilter(boolean allowFilter) {
            AllowFilter = allowFilter;
        }

        public boolean isSum() {
            return IsSum;
        }

        public void setSum(boolean sum) {
            IsSum = sum;
        }

        public String getFormular() {
            return Formular;
        }

        public void setFormular(String formular) {
            Formular = formular;
        }

        public int getFieldTypeId() {
            return FieldTypeId;
        }

        public void setFieldTypeId(int fieldTypeId) {
            FieldTypeId = fieldTypeId;
        }

        public String getDataType() {
            return DataType;
        }

        public void setDataType(String dataType) {
            DataType = dataType;
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

        public int getType() {
            return Type;
        }

        public void setType(int type) {
            Type = type;
        }
    }

    public static class Option {
        private boolean PeopleOnly;
        private boolean AllowMultipleValues;
        private boolean AllowMultipleTask;
        private boolean AllowMultipleTaskText;
        private boolean DateOnly;
        private ArrayList<String> Choices;
        private String LookupWebId;
        private String LookupListId;
        private String LookupField;
        private boolean IsSignature;
        private int RichTextMode;

        public Option() {
        }

        public boolean isPeopleOnly() {
            return PeopleOnly;
        }

        public void setPeopleOnly(boolean peopleOnly) {
            PeopleOnly = peopleOnly;
        }

        public boolean isAllowMultipleValues() {
            return AllowMultipleValues;
        }

        public void setAllowMultipleValues(boolean allowMultipleValues) {
            AllowMultipleValues = allowMultipleValues;
        }

        public boolean isAllowMultipleTask() {
            return AllowMultipleTask;
        }

        public void setAllowMultipleTask(boolean allowMultipleTask) {
            AllowMultipleTask = allowMultipleTask;
        }

        public boolean isAllowMultipleTaskText() {
            return AllowMultipleTaskText;
        }

        public void setAllowMultipleTaskText(boolean allowMultipleTaskText) {
            AllowMultipleTaskText = allowMultipleTaskText;
        }

        public boolean isDateOnly() {
            return DateOnly;
        }

        public void setDateOnly(boolean dateOnly) {
            DateOnly = dateOnly;
        }

        public boolean isSignature() {
            return IsSignature;
        }

        public void setSignature(boolean signature) {
            IsSignature = signature;
        }

        public int getRichTextMode() {
            return RichTextMode;
        }

        public void setRichTextMode(int richTextMode) {
            RichTextMode = richTextMode;
        }

        public String getLookupWebId() {
            return LookupWebId;
        }

        public void setLookupWebId(String lookupWebId) {
            LookupWebId = lookupWebId;
        }

        public String getLookupListId() {
            return LookupListId;
        }

        public void setLookupListId(String lookupListId) {
            LookupListId = lookupListId;
        }

        public String getLookupField() {
            return LookupField;
        }

        public void setLookupField(String lookupField) {
            LookupField = lookupField;
        }

        public ArrayList<String> getChoices() {
            return Choices;
        }

        public void setChoices(ArrayList<String> choices) {
            Choices = choices;
        }
    }
}
