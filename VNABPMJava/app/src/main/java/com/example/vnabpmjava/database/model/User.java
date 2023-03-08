package com.example.vnabpmjava.database.model;

import androidx.annotation.NonNull;
import androidx.room.Entity;
import androidx.room.Ignore;
import androidx.room.PrimaryKey;

import java.util.Date;
@Entity
public class User {
    @PrimaryKey
    @NonNull
    private String ID;
    private String UserId;
    private String AccountID;
    private String AccountName;
    private String FullName;
    private String Name;
    private Integer DepartmentID;
    private String Department;
    private String DepartmentManager;
    private String Manager;
    private Boolean Gender;
    private String BirthDay;
    private String Address;
    private String Image;
    private String StaffID;
    private String dateOfHire;
    private String mobile;
    private String ext;
    private String notify;
    private String reminder;
    private String receiveMail;
    private String email;
    private String position;
    private String experienceLevel;
    private Integer afterCompletedDate;
    private String phongBan;
    private String publicSiteRedirect;
    private String subSiteRedirect;
    private String fullNameVn;
    private String departmentTitle;
    private String imagePath;
    private Integer userStatus;
    private Integer language;
    private String positionTitle;
    private Integer positionID;
    private String siteName;
    private Integer status;
    @Ignore
    private short deviceOS;
    @Ignore
    private String deviceInfo;
    @Ignore
    private String modified;
    @Ignore
    private String editor;
    @Ignore
    private String created;
    @Ignore
    private Boolean isSelected;
    @Ignore
    private Boolean isGroup;

    @NonNull
    public String getID() {
        return ID;
    }

    public void setID(@NonNull String ID) {
        this.ID = ID;
    }

    public String getUserId() {
        return UserId;
    }

    public void setUserId(String userId) {
        UserId = userId;
    }

    public String getAccountID() {
        return AccountID;
    }

    public void setAccountID(String accountID) {
        AccountID = accountID;
    }

    public String getAccountName() {
        return AccountName;
    }

    public void setAccountName(String accountName) {
        AccountName = accountName;
    }

    public String getFullName() {
        return FullName;
    }

    public void setFullName(String fullName) {
        FullName = fullName;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public Integer getDepartmentID() {
        return DepartmentID;
    }

    public void setDepartmentID(Integer departmentID) {
        DepartmentID = departmentID;
    }

    public String getDepartment() {
        return Department;
    }

    public void setDepartment(String department) {
        Department = department;
    }

    public String getDepartmentManager() {
        return DepartmentManager;
    }

    public void setDepartmentManager(String departmentManager) {
        DepartmentManager = departmentManager;
    }

    public String getManager() {
        return Manager;
    }

    public void setManager(String manager) {
        Manager = manager;
    }

    public Boolean getGender() {
        return Gender;
    }

    public void setGender(Boolean gender) {
        Gender = gender;
    }

    public String getBirthDay() {
        return BirthDay;
    }

    public void setBirthDay(String birthDay) {
        BirthDay = birthDay;
    }

    public String getAddress() {
        return Address;
    }

    public void setAddress(String address) {
        Address = address;
    }

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public String getStaffID() {
        return StaffID;
    }

    public void setStaffID(String staffID) {
        StaffID = staffID;
    }

    public String getDateOfHire() {
        return dateOfHire;
    }

    public void setDateOfHire(String dateOfHire) {
        this.dateOfHire = dateOfHire;
    }

    public String getMobile() {
        return mobile;
    }

    public void setMobile(String mobile) {
        this.mobile = mobile;
    }

    public String getExt() {
        return ext;
    }

    public void setExt(String ext) {
        this.ext = ext;
    }

    public String getNotify() {
        return notify;
    }

    public void setNotify(String notify) {
        this.notify = notify;
    }

    public String getReminder() {
        return reminder;
    }

    public void setReminder(String reminder) {
        this.reminder = reminder;
    }

    public String getReceiveMail() {
        return receiveMail;
    }

    public void setReceiveMail(String receiveMail) {
        this.receiveMail = receiveMail;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPosition() {
        return position;
    }

    public void setPosition(String position) {
        this.position = position;
    }

    public String getExperienceLevel() {
        return experienceLevel;
    }

    public void setExperienceLevel(String experienceLevel) {
        this.experienceLevel = experienceLevel;
    }

    public Integer getAfterCompletedDate() {
        return afterCompletedDate;
    }

    public void setAfterCompletedDate(Integer afterCompletedDate) {
        this.afterCompletedDate = afterCompletedDate;
    }

    public String getPhongBan() {
        return phongBan;
    }

    public void setPhongBan(String phongBan) {
        this.phongBan = phongBan;
    }

    public String getPublicSiteRedirect() {
        return publicSiteRedirect;
    }

    public void setPublicSiteRedirect(String publicSiteRedirect) {
        this.publicSiteRedirect = publicSiteRedirect;
    }

    public String getSubSiteRedirect() {
        return subSiteRedirect;
    }

    public void setSubSiteRedirect(String subSiteRedirect) {
        this.subSiteRedirect = subSiteRedirect;
    }

    public String getFullNameVn() {
        return fullNameVn;
    }

    public void setFullNameVn(String fullNameVn) {
        this.fullNameVn = fullNameVn;
    }

    public String getDepartmentTitle() {
        return departmentTitle;
    }

    public void setDepartmentTitle(String departmentTitle) {
        this.departmentTitle = departmentTitle;
    }

    public String getImagePath() {
        return imagePath;
    }

    public void setImagePath(String imagePath) {
        this.imagePath = imagePath;
    }

    public Integer getUserStatus() {
        return userStatus;
    }

    public void setUserStatus(Integer userStatus) {
        this.userStatus = userStatus;
    }

    public Integer getLanguage() {
        return language;
    }

    public void setLanguage(Integer language) {
        this.language = language;
    }

    public String getPositionTitle() {
        return positionTitle;
    }

    public void setPositionTitle(String positionTitle) {
        this.positionTitle = positionTitle;
    }

    public Integer getPositionID() {
        return positionID;
    }

    public void setPositionID(Integer positionID) {
        this.positionID = positionID;
    }

    public String getSiteName() {
        return siteName;
    }

    public void setSiteName(String siteName) {
        this.siteName = siteName;
    }

    public Integer getStatus() {
        return status;
    }

    public void setStatus(Integer status) {
        this.status = status;
    }

    public short getDeviceOS() {
        return deviceOS;
    }

    public void setDeviceOS(short deviceOS) {
        this.deviceOS = deviceOS;
    }

    public String getDeviceInfo() {
        return deviceInfo;
    }

    public void setDeviceInfo(String deviceInfo) {
        this.deviceInfo = deviceInfo;
    }

    public String getModified() {
        return modified;
    }

    public void setModified(String modified) {
        this.modified = modified;
    }

    public String getEditor() {
        return editor;
    }

    public void setEditor(String editor) {
        this.editor = editor;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }

    public Boolean getSelected() {
        return isSelected;
    }

    public void setSelected(Boolean selected) {
        isSelected = selected;
    }

    public Boolean getGroup() {
        return isGroup;
    }

    public void setGroup(Boolean group) {
        isGroup = group;
    }
}