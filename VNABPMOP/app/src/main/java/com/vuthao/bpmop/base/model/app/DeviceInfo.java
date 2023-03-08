package com.vuthao.bpmop.base.model.app;

public class DeviceInfo {
    private String DeviceId ;
    private String DevicePushToken;
    private int DeviceOS;
    private String AppVersion;
    private String DeviceOSVersion;
    private String DeviceModel;
    private String DeviceName;

    public DeviceInfo() { }

    public String getDeviceName() {
        return DeviceName;
    }

    public void setDeviceName(String deviceName) {
        DeviceName = deviceName;
    }

    public String getDeviceId() {
        return DeviceId;
    }

    public void setDeviceId(String deviceId) {
        DeviceId = deviceId;
    }

    public String getDevicePushToken() {
        return DevicePushToken;
    }

    public void setDevicePushToken(String devicePushToken) {
        DevicePushToken = devicePushToken;
    }

    public int getDeviceOS() {
        return DeviceOS;
    }

    public void setDeviceOS(int deviceOS) {
        DeviceOS = deviceOS;
    }

    public String getAppVersion() {
        return AppVersion;
    }

    public void setAppVersion(String appVersion) {
        AppVersion = appVersion;
    }

    public String getDeviceOSVersion() {
        return DeviceOSVersion;
    }

    public void setDeviceOSVersion(String deviceOSVersion) {
        DeviceOSVersion = deviceOSVersion;
    }

    public String getDeviceModel() {
        return DeviceModel;
    }

    public void setDeviceModel(String deviceModel) {
        DeviceModel = deviceModel;
    }
}
