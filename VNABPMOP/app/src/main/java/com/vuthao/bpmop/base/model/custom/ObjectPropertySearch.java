package com.vuthao.bpmop.base.model.custom;

public class ObjectPropertySearch {
    private String lstProSeach;
    private int limit;
    private int offset;
    private int total;

    public ObjectPropertySearch(String lstProSeach, int limit, int offset, int total) {
        this.lstProSeach = lstProSeach;
        this.limit = limit;
        this.offset = offset;
        this.total = total;
    }

    public ObjectPropertySearch() {
    }

    public String getLstProSeach() {
        return lstProSeach;
    }

    public void setLstProSeach(String lstProSeach) {
        this.lstProSeach = lstProSeach;
    }

    public int getLimit() {
        return limit;
    }

    public void setLimit(int limit) {
        this.limit = limit;
    }

    public int getOffset() {
        return offset;
    }

    public void setOffset(int offset) {
        this.offset = offset;
    }

    public int getTotal() {
        return total;
    }

    public void setTotal(int total) {
        this.total = total;
    }
}
