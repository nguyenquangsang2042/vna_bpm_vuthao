package com.vuthao.bpmop.base.model.app;

public class Status {
    private String status;
    private mess mess;
    private String dateNow;

    public Status() { }

    public String getStatus() {
        return status;
    }

    public Status.mess getMess() {
        return mess;
    }

    public String getDateNow() {
        return dateNow;
    }

    public void setDateNow(String dateNow) {
        this.dateNow = dateNow;
    }

    public class mess
    {
        private String Key;
        private  String Value;

        public String getKey() {
            return Key;
        }

        public String getValue() {
            return Value;
        }
    }
}
